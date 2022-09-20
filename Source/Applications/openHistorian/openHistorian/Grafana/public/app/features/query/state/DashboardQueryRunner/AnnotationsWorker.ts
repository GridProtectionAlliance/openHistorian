import { cloneDeep } from 'lodash';
import { from, merge, Observable, of } from 'rxjs';
import { catchError, filter, finalize, map, mergeAll, mergeMap, reduce, takeUntil } from 'rxjs/operators';

import { AnnotationQuery, DataSourceApi } from '@grafana/data';
import { getDataSourceSrv } from '@grafana/runtime';

import { AnnotationQueryFinished, AnnotationQueryStarted } from '../../../../types/events';
import { DashboardModel } from '../../../dashboard/state';

import { AnnotationsQueryRunner } from './AnnotationsQueryRunner';
import { getDashboardQueryRunner } from './DashboardQueryRunner';
import { LegacyAnnotationQueryRunner } from './LegacyAnnotationQueryRunner';
import {
  AnnotationQueryRunner,
  DashboardQueryRunnerOptions,
  DashboardQueryRunnerWorker,
  DashboardQueryRunnerWorkerResult,
} from './types';
import { emptyResult, handleDatasourceSrvError, translateQueryResult } from './utils';

export class AnnotationsWorker implements DashboardQueryRunnerWorker {
  constructor(
    private readonly runners: AnnotationQueryRunner[] = [
      new LegacyAnnotationQueryRunner(),
      new AnnotationsQueryRunner(),
    ]
  ) {}

  canWork({ dashboard }: DashboardQueryRunnerOptions): boolean {
    const annotations = dashboard.annotations.list.find(AnnotationsWorker.getAnnotationsToProcessFilter);
    // We shouldn't return annotations for public dashboards v1
    return Boolean(annotations) && !this.publicDashboardViewMode(dashboard);
  }

  work(options: DashboardQueryRunnerOptions): Observable<DashboardQueryRunnerWorkerResult> {
    if (!this.canWork(options)) {
      return emptyResult();
    }

    const { dashboard, range } = options;
    const annotations = dashboard.annotations.list.filter(AnnotationsWorker.getAnnotationsToProcessFilter);
    const observables = annotations.map((annotation) => {
      const datasourceObservable = from(getDataSourceSrv().get(annotation.datasource)).pipe(
        catchError(handleDatasourceSrvError) // because of the reduce all observables need to be completed, so an erroneous observable wont do
      );
      return datasourceObservable.pipe(
        mergeMap((datasource?: DataSourceApi) => {
          const runner = this.runners.find((r) => r.canRun(datasource));
          if (!runner) {
            return of([]);
          }

          dashboard.events.publish(new AnnotationQueryStarted(annotation));

          return runner.run({ annotation, datasource, dashboard, range }).pipe(
            takeUntil(
              getDashboardQueryRunner()
                .cancellations()
                .pipe(filter((a) => a === annotation))
            ),
            map((results) => {
              // store response in annotation object if this is a snapshot call
              if (dashboard.snapshot) {
                annotation.snapshotData = cloneDeep(results);
              }
              // translate result
              return translateQueryResult(annotation, results);
            }),
            finalize(() => {
              dashboard.events.publish(new AnnotationQueryFinished(annotation));
            })
          );
        })
      );
    });

    return merge(observables).pipe(
      mergeAll(),
      reduce((acc, value) => {
        // should we use scan or reduce here
        // reduce will only emit when all observables are completed
        // scan will emit when any observable is completed
        // choosing reduce to minimize re-renders
        return acc.concat(value);
      }),
      map((annotations) => {
        return { annotations, alertStates: [] };
      })
    );
  }

  private static getAnnotationsToProcessFilter(annotation: AnnotationQuery): boolean {
    return annotation.enable && !Boolean(annotation.snapshotData);
  }

  publicDashboardViewMode(dashboard: DashboardModel): boolean {
    return dashboard.meta.publicDashboardAccessToken !== undefined && dashboard.meta.publicDashboardAccessToken !== '';
  }
}

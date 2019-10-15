import cloneDeep from 'lodash/cloneDeep';
import groupBy from 'lodash/groupBy';
import { from, of, Observable, merge } from 'rxjs';
import { tap } from 'rxjs/operators';

import { LoadingState } from '@grafana/data';
import { DataSourceApi, DataQuery, DataQueryRequest, DataQueryResponse, DataSourceInstanceSettings } from '@grafana/ui';
import { getDataSourceSrv } from '@grafana/runtime';
import { mergeMap, map } from 'rxjs/operators';

export const MIXED_DATASOURCE_NAME = '-- Mixed --';

export class MixedDatasource extends DataSourceApi<DataQuery> {
  constructor(instanceSettings: DataSourceInstanceSettings) {
    super(instanceSettings);
  }

  query(request: DataQueryRequest<DataQuery>): Observable<DataQueryResponse> {
    // Remove any invalid queries
    const queries = request.targets.filter(t => {
      return t.datasource !== MIXED_DATASOURCE_NAME;
    });

    if (!queries.length) {
      return of({ data: [] } as DataQueryResponse); // nothing
    }

    const sets: { [key: string]: DataQuery[] } = groupBy(queries, 'datasource');
    const observables: Array<Observable<DataQueryResponse>> = [];
    let runningSubRequests = 0;

    for (const key in sets) {
      const targets = sets[key];
      const dsName = targets[0].datasource;

      const observable = from(getDataSourceSrv().get(dsName)).pipe(
        mergeMap((dataSourceApi: DataSourceApi) => {
          const datasourceRequest = cloneDeep(request);

          // Remove any unused hidden queries
          let newTargets = targets.slice();
          if (!dataSourceApi.meta.hiddenQueries) {
            newTargets = newTargets.filter((t: DataQuery) => !t.hide);
          }

          datasourceRequest.targets = newTargets;
          datasourceRequest.requestId = `${dsName}${datasourceRequest.requestId || ''}`;

          // all queries hidden return empty result for for this requestId
          if (datasourceRequest.targets.length === 0) {
            return of({ data: [], key: datasourceRequest.requestId });
          }

          runningSubRequests++;
          let hasCountedAsDone = false;

          return from(dataSourceApi.query(datasourceRequest)).pipe(
            tap(
              (response: DataQueryResponse) => {
                if (
                  hasCountedAsDone ||
                  response.state === LoadingState.Streaming ||
                  response.state === LoadingState.Loading
                ) {
                  return;
                }
                runningSubRequests--;
                hasCountedAsDone = true;
              },
              () => {
                if (hasCountedAsDone) {
                  return;
                }
                hasCountedAsDone = true;
                runningSubRequests--;
              }
            ),
            map((response: DataQueryResponse) => {
              return {
                ...response,
                data: response.data || [],
                state: runningSubRequests === 0 ? LoadingState.Done : LoadingState.Loading,
                key: `${dsName}${response.key || ''}`,
              } as DataQueryResponse;
            })
          );
        })
      );

      observables.push(observable);
    }

    return merge(...observables);
  }

  testDatasource() {
    return Promise.resolve({});
  }
}

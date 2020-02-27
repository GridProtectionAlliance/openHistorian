// Libaries
import angular from 'angular';
import _ from 'lodash';

// Components
import './editor_ctrl';
import coreModule from 'app/core/core_module';

// Utils & Services
import { dedupAnnotations } from './events_processing';

// Types
import { DashboardModel } from '../dashboard/state/DashboardModel';
import DatasourceSrv from '../plugins/datasource_srv';
import { BackendSrv } from 'app/core/services/backend_srv';
import { TimeSrv } from '../dashboard/services/TimeSrv';
import { DataSourceApi, PanelEvents, AnnotationEvent, AppEvents, PanelModel, TimeRange } from '@grafana/data';
import { GrafanaRootScope } from 'app/routes/GrafanaCtrl';

export class AnnotationsSrv {
  globalAnnotationsPromise: any;
  alertStatesPromise: any;
  datasourcePromises: any;

  /** @ngInject */
  constructor(
    private $rootScope: GrafanaRootScope,
    private datasourceSrv: DatasourceSrv,
    private backendSrv: BackendSrv,
    private timeSrv: TimeSrv
  ) {}

  init(dashboard: DashboardModel) {
    // always clearPromiseCaches when loading new dashboard
    this.clearPromiseCaches();
    // clear promises on refresh events
    dashboard.on(PanelEvents.refresh, this.clearPromiseCaches.bind(this));
  }

  clearPromiseCaches() {
    this.globalAnnotationsPromise = null;
    this.alertStatesPromise = null;
    this.datasourcePromises = null;
  }

  getAnnotations(options: { dashboard: DashboardModel; panel: PanelModel; range: TimeRange }) {
    return Promise.all([this.getGlobalAnnotations(options), this.getAlertStates(options)])
      .then(results => {
        // combine the annotations and flatten results
        let annotations: AnnotationEvent[] = _.flattenDeep(results[0]);

        // filter out annotations that do not belong to requesting panel
        annotations = _.filter(annotations, item => {
          // if event has panel id and query is of type dashboard then panel and requesting panel id must match
          if (item.panelId && item.source.type === 'dashboard') {
            return item.panelId === options.panel.id;
          }
          return true;
        });

        annotations = dedupAnnotations(annotations);

        // look for alert state for this panel
        const alertState: any = _.find(results[1], { panelId: options.panel.id });

        return {
          annotations: annotations,
          alertState: alertState,
        };
      })
      .catch(err => {
        if (!err.message && err.data && err.data.message) {
          err.message = err.data.message;
        }
        console.log('AnnotationSrv.query error', err);
        this.$rootScope.appEvent(AppEvents.alertError, ['Annotation Query Failed', err.message || err]);
        return [];
      });
  }

  getAlertStates(options: any) {
    if (!options.dashboard.id) {
      return Promise.resolve([]);
    }

    // ignore if no alerts
    if (options.panel && !options.panel.alert) {
      return Promise.resolve([]);
    }

    if (options.range.raw.to !== 'now') {
      return Promise.resolve([]);
    }

    if (this.alertStatesPromise) {
      return this.alertStatesPromise;
    }

    this.alertStatesPromise = this.backendSrv.get('/api/alerts/states-for-dashboard', {
      dashboardId: options.dashboard.id,
    });
    return this.alertStatesPromise;
  }

  getGlobalAnnotations(options: { dashboard: DashboardModel; panel: PanelModel; range: TimeRange }) {
    const dashboard = options.dashboard;

    if (this.globalAnnotationsPromise) {
      return this.globalAnnotationsPromise;
    }

    const range = this.timeSrv.timeRange();
    const promises = [];
    const dsPromises = [];

    for (const annotation of dashboard.annotations.list) {
      if (!annotation.enable) {
        continue;
      }

      if (annotation.snapshotData) {
        return this.translateQueryResult(annotation, annotation.snapshotData);
      }
      const datasourcePromise = this.datasourceSrv.get(annotation.datasource);
      dsPromises.push(datasourcePromise);
      promises.push(
        datasourcePromise
          .then((datasource: DataSourceApi) => {
            // issue query against data source
            return datasource.annotationQuery({
              range,
              rangeRaw: range.raw,
              annotation: annotation,
              dashboard: dashboard,
            });
          })
          .then(results => {
            // store response in annotation object if this is a snapshot call
            if (dashboard.snapshot) {
              annotation.snapshotData = angular.copy(results);
            }
            // translate result
            return this.translateQueryResult(annotation, results);
          })
      );
    }
    this.datasourcePromises = Promise.all(dsPromises);
    this.globalAnnotationsPromise = Promise.all(promises);
    return this.globalAnnotationsPromise;
  }

  saveAnnotationEvent(annotation: AnnotationEvent) {
    this.globalAnnotationsPromise = null;
    return this.backendSrv.post('/api/annotations', annotation);
  }

  updateAnnotationEvent(annotation: AnnotationEvent) {
    this.globalAnnotationsPromise = null;
    return this.backendSrv.put(`/api/annotations/${annotation.id}`, annotation);
  }

  deleteAnnotationEvent(annotation: AnnotationEvent) {
    this.globalAnnotationsPromise = null;
    const deleteUrl = `/api/annotations/${annotation.id}`;

    return this.backendSrv.delete(deleteUrl);
  }

  translateQueryResult(annotation: any, results: any) {
    // if annotation has snapshotData
    // make clone and remove it
    if (annotation.snapshotData) {
      annotation = angular.copy(annotation);
      delete annotation.snapshotData;
    }

    for (const item of results) {
      item.source = annotation;
      item.isRegion = item.timeEnd && item.time !== item.timeEnd;
    }

    return results;
  }
}

coreModule.service('annotationsSrv', AnnotationsSrv);

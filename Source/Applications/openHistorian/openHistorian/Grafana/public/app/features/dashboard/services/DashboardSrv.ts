import { lastValueFrom } from 'rxjs';

import { BackendSrvRequest } from '@grafana/runtime';
import { appEvents } from 'app/core/app_events';
import { getBackendSrv } from 'app/core/services/backend_srv';
import { saveDashboard } from 'app/features/manage-dashboards/state/actions';
import { DashboardMeta } from 'app/types';

import { RemovePanelEvent } from '../../../types/events';
import { DashboardModel } from '../state/DashboardModel';
import { removePanel } from '../utils/panel';

export interface SaveDashboardOptions {
  /** The complete dashboard model. If `dashboard.id` is not set a new dashboard will be created. */
  dashboard: DashboardModel;
  /** Set a commit message for the version history. */
  message?: string;
  /** The id of the folder to save the dashboard in. */
  folderId?: number;
  /** The UID of the folder to save the dashboard in. Overrides `folderId`. */
  folderUid?: string;
  /** Set to `true` if you want to overwrite existing dashboard with newer version,
   *  same dashboard title in folder or same dashboard uid. */
  overwrite?: boolean;
  /** Set the dashboard refresh interval.
   *  If this is lower than the minimum refresh interval, Grafana will ignore it and will enforce the minimum refresh interval. */
  refresh?: string;
}

interface SaveDashboardResponse {
  id: number;
  slug: string;
  status: string;
  uid: string;
  url: string;
  version: number;
}

export class DashboardSrv {
  dashboard?: DashboardModel;

  constructor() {
    appEvents.subscribe(RemovePanelEvent, (e) => this.onRemovePanel(e.payload));
  }

  create(dashboard: any, meta: DashboardMeta) {
    return new DashboardModel(dashboard, meta);
  }

  setCurrent(dashboard: DashboardModel) {
    this.dashboard = dashboard;
  }

  getCurrent(): DashboardModel | undefined {
    if (!this.dashboard) {
      console.warn('Calling getDashboardSrv().getCurrent() without calling getDashboardSrv().setCurrent() first.');
    }
    return this.dashboard;
  }

  onRemovePanel = (panelId: number) => {
    const dashboard = this.getCurrent();
    if (dashboard) {
      removePanel(dashboard, dashboard.getPanelById(panelId)!, true);
    }
  };

  saveJSONDashboard(json: string) {
    const parsedJson = JSON.parse(json);
    return saveDashboard({
      dashboard: parsedJson,
      folderId: this.dashboard?.meta.folderId || parsedJson.folderId,
    });
  }

  saveDashboard(
    data: SaveDashboardOptions,
    requestOptions?: Pick<BackendSrvRequest, 'showErrorAlert' | 'showSuccessAlert'>
  ) {
    return lastValueFrom(
      getBackendSrv().fetch<SaveDashboardResponse>({
        url: '/api/dashboards/db/',
        method: 'POST',
        data: {
          ...data,
          dashboard: data.dashboard.getSaveModelClone(),
        },
        ...requestOptions,
      })
    );
  }

  starDashboard(dashboardId: string, isStarred: any) {
    const backendSrv = getBackendSrv();
    let promise;

    if (isStarred) {
      promise = backendSrv.delete('/api/user/stars/dashboard/' + dashboardId).then(() => {
        return false;
      });
    } else {
      promise = backendSrv.post('/api/user/stars/dashboard/' + dashboardId).then(() => {
        return true;
      });
    }

    return promise.then((res: boolean) => {
      if (this.dashboard && this.dashboard.id === dashboardId) {
        this.dashboard.meta.isStarred = res;
      }
      return res;
    });
  }
}

//
// Code below is to export the service to React components
//

let singletonInstance: DashboardSrv;

export function setDashboardSrv(instance: DashboardSrv) {
  singletonInstance = instance;
}

export function getDashboardSrv(): DashboardSrv {
  if (!singletonInstance) {
    singletonInstance = new DashboardSrv();
  }
  return singletonInstance;
}

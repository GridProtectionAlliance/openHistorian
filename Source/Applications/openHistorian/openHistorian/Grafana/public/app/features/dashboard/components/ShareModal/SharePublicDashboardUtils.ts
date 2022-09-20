import { getBackendSrv } from '@grafana/runtime';
import { notifyApp } from 'app/core/actions';
import { createSuccessNotification } from 'app/core/copy/appNotification';
import { VariableModel } from 'app/features/variables/types';
import { dispatch } from 'app/store/store';
import { DashboardDataDTO, DashboardMeta } from 'app/types/dashboard';

export interface PublicDashboard {
  accessToken?: string;
  isEnabled: boolean;
  uid: string;
  dashboardUid: string;
  timeSettings?: object;
}

export interface DashboardResponse {
  dashboard: DashboardDataDTO;
  meta: DashboardMeta;
}

export const getPublicDashboardConfig = async (
  dashboardUid: string,
  setPublicDashboard: React.Dispatch<React.SetStateAction<PublicDashboard>>
) => {
  const url = `/api/dashboards/uid/${dashboardUid}/public-config`;
  const pdResp: PublicDashboard = await getBackendSrv().get(url);
  setPublicDashboard(pdResp);
};

export const savePublicDashboardConfig = async (
  dashboardUid: string,
  publicDashboardConfig: PublicDashboard,
  setPublicDashboard: React.Dispatch<React.SetStateAction<PublicDashboard>>
) => {
  const url = `/api/dashboards/uid/${dashboardUid}/public-config`;
  const pdResp: PublicDashboard = await getBackendSrv().post(url, publicDashboardConfig);

  // Never allow a user to send the orgId
  // @ts-ignore
  delete pdResp.orgId;

  dispatch(notifyApp(createSuccessNotification('Dashboard sharing configuration saved')));
  setPublicDashboard(pdResp);
};

// Instance methods
export const dashboardHasTemplateVariables = (variables: VariableModel[]): boolean => {
  return variables.length > 0;
};

export const publicDashboardPersisted = (publicDashboard: PublicDashboard): boolean => {
  return publicDashboard.uid !== '' && publicDashboard.uid !== undefined;
};

export const generatePublicDashboardUrl = (publicDashboard: PublicDashboard): string => {
  return `${window.location.origin}/public-dashboards/${publicDashboard.accessToken}`;
};

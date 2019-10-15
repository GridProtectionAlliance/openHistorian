import { Organization, ThunkResult } from 'app/types';
import { getBackendSrv } from '@grafana/runtime';

export enum ActionTypes {
  LoadOrganization = 'LOAD_ORGANIZATION',
  SetOrganizationName = 'SET_ORGANIZATION_NAME',
}

interface LoadOrganizationAction {
  type: ActionTypes.LoadOrganization;
  payload: Organization;
}

interface SetOrganizationNameAction {
  type: ActionTypes.SetOrganizationName;
  payload: string;
}

const organizationLoaded = (organization: Organization) => ({
  type: ActionTypes.LoadOrganization,
  payload: organization,
});

export const setOrganizationName = (orgName: string) => ({
  type: ActionTypes.SetOrganizationName,
  payload: orgName,
});

export type Action = LoadOrganizationAction | SetOrganizationNameAction;

export function loadOrganization(): ThunkResult<any> {
  return async dispatch => {
    const organizationResponse = await getBackendSrv().get('/api/org');
    dispatch(organizationLoaded(organizationResponse));

    return organizationResponse;
  };
}

export function updateOrganization(): ThunkResult<any> {
  return async (dispatch, getStore) => {
    const organization = getStore().organization.organization;

    await getBackendSrv().put('/api/org', { name: organization.name });

    dispatch(loadOrganization());
  };
}

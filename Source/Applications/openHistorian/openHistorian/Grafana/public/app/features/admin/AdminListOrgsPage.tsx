import React, { useEffect } from 'react';
import { useSelector } from 'react-redux';
import useAsyncFn from 'react-use/lib/useAsyncFn';

import { getBackendSrv } from '@grafana/runtime';
import { LinkButton } from '@grafana/ui';
import { Page } from 'app/core/components/Page/Page';
import { getNavModel } from 'app/core/selectors/navModel';
import { contextSrv } from 'app/core/services/context_srv';
import { AccessControlAction } from 'app/types';
import { StoreState } from 'app/types/store';

import { AdminOrgsTable } from './AdminOrgsTable';

const deleteOrg = async (orgId: number) => {
  return await getBackendSrv().delete('/api/orgs/' + orgId);
};

const getOrgs = async () => {
  return await getBackendSrv().get('/api/orgs');
};

const getErrorMessage = (error: any) => {
  return error?.data?.message || 'An unexpected error happened.';
};

export default function AdminListOrgsPages() {
  const navIndex = useSelector((state: StoreState) => state.navIndex);
  const navModel = getNavModel(navIndex, 'global-orgs');
  const [state, fetchOrgs] = useAsyncFn(async () => await getOrgs(), []);
  const canCreateOrg = contextSrv.hasPermission(AccessControlAction.OrgsCreate);

  useEffect(() => {
    fetchOrgs();
  }, [fetchOrgs]);

  return (
    <Page navModel={navModel}>
      <Page.Contents>
        <>
          <div className="page-action-bar">
            <div className="page-action-bar__spacer" />
            <LinkButton icon="plus" href="org/new" disabled={!canCreateOrg}>
              New org
            </LinkButton>
          </div>
          {state.error && getErrorMessage(state.error)}
          {state.loading && 'Fetching organizations'}
          {state.value && (
            <AdminOrgsTable
              orgs={state.value}
              onDelete={(orgId) => {
                deleteOrg(orgId).then(() => fetchOrgs());
              }}
            />
          )}
        </>
      </Page.Contents>
    </Page>
  );
}

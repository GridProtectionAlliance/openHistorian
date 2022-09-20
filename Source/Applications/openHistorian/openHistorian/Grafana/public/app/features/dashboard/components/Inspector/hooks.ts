import { t } from '@lingui/macro';
import { useMemo } from 'react';
import useAsync from 'react-use/lib/useAsync';

import { DataQueryError, DataSourceApi, PanelData, PanelPlugin } from '@grafana/data';
import { getDataSourceSrv } from '@grafana/runtime';
import { DashboardModel, PanelModel } from 'app/features/dashboard/state';
import { InspectTab } from 'app/features/inspector/types';

import { supportsDataQuery } from '../PanelEditor/utils';

import { PanelInspectActionSupplier } from './PanelInspectActions';

/**
 * Given PanelData return first data source supporting metadata inspector
 */
export const useDatasourceMetadata = (data?: PanelData) => {
  const state = useAsync(async () => {
    const targets = data?.request?.targets || [];

    if (data && data.series && targets.length) {
      for (const frame of data.series) {
        if (frame.meta && frame.meta.custom) {
          // get data source from first query
          const dataSource = await getDataSourceSrv().get(targets[0].datasource);
          if (dataSource && dataSource.components?.MetadataInspector) {
            return dataSource;
          }
        }
      }
    }

    return undefined;
  }, [data]);
  return state.value;
};

/**
 * Configures tabs for PanelInspector
 */
export const useInspectTabs = (
  panel: PanelModel,
  dashboard: DashboardModel,
  plugin: PanelPlugin | undefined | null,
  error?: DataQueryError,
  metaDs?: DataSourceApi
) => {
  return useMemo(() => {
    const tabs = [];
    if (supportsDataQuery(plugin)) {
      tabs.push({ label: t({ id: 'dashboard.inspect.data-tab', message: 'Data' }), value: InspectTab.Data });
      tabs.push({ label: t({ id: 'dashboard.inspect.stats-tab', message: 'Stats' }), value: InspectTab.Stats });
    }

    if (metaDs) {
      tabs.push({ label: t({ id: 'dashboard.inspect.meta-tab', message: 'Meta Data' }), value: InspectTab.Meta });
    }

    tabs.push({ label: t({ id: 'dashboard.inspect.json-tab', message: 'JSON' }), value: InspectTab.JSON });

    if (error && error.message) {
      tabs.push({ label: t({ id: 'dashboard.inspect.error-tab', message: 'Error' }), value: InspectTab.Error });
    }

    // This is a quick internal hack to allow custom actions in inspect
    // For 8.1, something like this should be exposed through grafana/runtime
    const supplier = (window as any).grafanaPanelInspectActionSupplier as PanelInspectActionSupplier;
    if (supplier && supplier.getActions(panel)) {
      tabs.push({
        label: t({ id: 'dashboard.inspect.actions-tab', message: 'Actions' }),
        value: InspectTab.Actions,
      });
    }

    if (dashboard.meta.canEdit && supportsDataQuery(plugin)) {
      tabs.push({ label: t({ id: 'dashboard.inspect.query-tab', message: 'Query' }), value: InspectTab.Query });
    }
    return tabs;
  }, [panel, plugin, metaDs, dashboard, error]);
};

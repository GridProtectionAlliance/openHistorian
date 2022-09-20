import { lastValueFrom } from 'rxjs';

import { getBackendSrv } from '@grafana/runtime';
import { RuleNamespace } from 'app/types/unified-alerting';
import { PromRulesResponse } from 'app/types/unified-alerting-dto';

import { getDatasourceAPIUid, GRAFANA_RULES_SOURCE_NAME } from '../utils/datasource';

export interface FetchPromRulesFilter {
  dashboardUID: string;
  panelId?: number;
}

export interface PrometheusDataSourceConfig {
  dataSourceName: string;
}

export function prometheusUrlBuilder(dataSourceConfig: PrometheusDataSourceConfig) {
  const { dataSourceName } = dataSourceConfig;

  return {
    rules: (filter?: FetchPromRulesFilter) => {
      const searchParams = new URLSearchParams();
      const params = prepareRulesFilterQueryParams(searchParams, filter);

      return {
        url: `/api/prometheus/${getDatasourceAPIUid(dataSourceName)}/api/v1/rules`,
        params: params,
      };
    },
  };
}

export function prepareRulesFilterQueryParams(
  params: URLSearchParams,
  filter?: FetchPromRulesFilter
): Record<string, string> {
  if (filter?.dashboardUID) {
    params.set('dashboard_uid', filter.dashboardUID);
    if (filter?.panelId) {
      params.set('panel_id', String(filter.panelId));
    }
  }

  return Object.fromEntries(params);
}

export async function fetchRules(dataSourceName: string, filter?: FetchPromRulesFilter): Promise<RuleNamespace[]> {
  if (filter?.dashboardUID && dataSourceName !== GRAFANA_RULES_SOURCE_NAME) {
    throw new Error('Filtering by dashboard UID is only supported for Grafana Managed rules.');
  }

  const { url, params } = prometheusUrlBuilder({ dataSourceName }).rules(filter);

  const response = await lastValueFrom(
    getBackendSrv().fetch<PromRulesResponse>({
      url,
      params,
      showErrorAlert: false,
      showSuccessAlert: false,
    })
  ).catch((e) => {
    if ('status' in e && e.status === 404) {
      throw new Error('404 from rule state endpoint. Perhaps ruler API is not enabled?');
    }
    throw e;
  });

  const nsMap: { [key: string]: RuleNamespace } = {};
  response.data.data.groups.forEach((group) => {
    group.rules.forEach((rule) => {
      rule.query = rule.query || '';
    });
    if (!nsMap[group.file]) {
      nsMap[group.file] = {
        dataSourceName,
        name: group.file,
        groups: [group],
      };
    } else {
      nsMap[group.file].groups.push(group);
    }
  });

  return Object.values(nsMap);
}

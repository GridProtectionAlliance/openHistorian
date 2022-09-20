import { isEmpty } from 'lodash';

import { PanelProps } from '@grafana/data';
import { Alert, hasAlertState } from 'app/types/unified-alerting';
import { GrafanaAlertState, PromAlertingRuleState } from 'app/types/unified-alerting-dto';

import { UnifiedAlertListOptions } from './types';

export function filterAlerts(options: PanelProps<UnifiedAlertListOptions>['options'], alerts: Alert[]): Alert[] {
  const { stateFilter } = options;

  if (isEmpty(stateFilter)) {
    return alerts;
  }

  return alerts.filter((alert) => {
    return (
      (stateFilter.firing &&
        (hasAlertState(alert, GrafanaAlertState.Alerting) || hasAlertState(alert, PromAlertingRuleState.Firing))) ||
      (stateFilter.pending &&
        (hasAlertState(alert, GrafanaAlertState.Pending) || hasAlertState(alert, PromAlertingRuleState.Pending))) ||
      (stateFilter.noData && hasAlertState(alert, GrafanaAlertState.NoData)) ||
      (stateFilter.normal && hasAlertState(alert, GrafanaAlertState.Normal)) ||
      (stateFilter.error && hasAlertState(alert, GrafanaAlertState.Error)) ||
      (stateFilter.inactive && hasAlertState(alert, PromAlertingRuleState.Inactive))
    );
  });
}

export function isPrivateLabel(label: string) {
  return !(label.startsWith('__') && label.endsWith('__'));
}

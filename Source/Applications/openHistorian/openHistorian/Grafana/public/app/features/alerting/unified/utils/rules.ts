import { capitalize } from 'lodash';

import { AlertState } from '@grafana/data';
import {
  Alert,
  AlertingRule,
  CloudRuleIdentifier,
  CombinedRuleGroup,
  GrafanaRuleIdentifier,
  PrometheusRuleIdentifier,
  PromRuleWithLocation,
  RecordingRule,
  Rule,
  RuleIdentifier,
  RuleNamespace,
} from 'app/types/unified-alerting';
import {
  GrafanaAlertState,
  GrafanaAlertStateWithReason,
  mapStateWithReasonToBaseState,
  PromAlertingRuleState,
  PromRuleType,
  RulerAlertingRuleDTO,
  RulerGrafanaRuleDTO,
  RulerRecordingRuleDTO,
  RulerRuleDTO,
} from 'app/types/unified-alerting-dto';

import { State } from '../components/StateTag';

import { RULER_NOT_SUPPORTED_MSG } from './constants';
import { AsyncRequestState } from './redux';

export function isAlertingRule(rule: Rule | undefined): rule is AlertingRule {
  return typeof rule === 'object' && rule.type === PromRuleType.Alerting;
}

export function isRecordingRule(rule: Rule): rule is RecordingRule {
  return rule.type === PromRuleType.Recording;
}

export function isAlertingRulerRule(rule?: RulerRuleDTO): rule is RulerAlertingRuleDTO {
  return typeof rule === 'object' && 'alert' in rule;
}

export function isRecordingRulerRule(rule?: RulerRuleDTO): rule is RulerRecordingRuleDTO {
  return typeof rule === 'object' && 'record' in rule;
}

export function isGrafanaRulerRule(rule?: RulerRuleDTO): rule is RulerGrafanaRuleDTO {
  return typeof rule === 'object' && 'grafana_alert' in rule;
}

export function alertInstanceKey(alert: Alert): string {
  return JSON.stringify(alert.labels);
}

export function isRulerNotSupportedResponse(resp: AsyncRequestState<any>) {
  return resp.error && resp.error?.message?.includes(RULER_NOT_SUPPORTED_MSG);
}

export function isGrafanaRuleIdentifier(identifier: RuleIdentifier): identifier is GrafanaRuleIdentifier {
  return 'uid' in identifier;
}

export function isCloudRuleIdentifier(identifier: RuleIdentifier): identifier is CloudRuleIdentifier {
  return 'rulerRuleHash' in identifier;
}

export function isPrometheusRuleIdentifier(identifier: RuleIdentifier): identifier is PrometheusRuleIdentifier {
  return 'ruleHash' in identifier;
}

export function alertStateToReadable(state: PromAlertingRuleState | GrafanaAlertStateWithReason | AlertState): string {
  if (state === PromAlertingRuleState.Inactive) {
    return 'Normal';
  }
  return capitalize(state);
}

export const flattenRules = (rules: RuleNamespace[]) => {
  return rules.reduce<PromRuleWithLocation[]>((acc, { dataSourceName, name: namespaceName, groups }) => {
    groups.forEach(({ name: groupName, rules }) => {
      rules.forEach((rule) => {
        if (isAlertingRule(rule)) {
          acc.push({ dataSourceName, namespaceName, groupName, rule });
        }
      });
    });
    return acc;
  }, []);
};

export function alertStateToState(state: PromAlertingRuleState | GrafanaAlertStateWithReason | AlertState): State {
  let key: PromAlertingRuleState | GrafanaAlertState | AlertState;
  if (Object.values(AlertState).includes(state as AlertState)) {
    key = state as AlertState;
  } else {
    key = mapStateWithReasonToBaseState(state as GrafanaAlertStateWithReason | PromAlertingRuleState);
  }

  return alertStateToStateMap[key];
}

const alertStateToStateMap: Record<PromAlertingRuleState | GrafanaAlertState | AlertState, State> = {
  [PromAlertingRuleState.Inactive]: 'good',
  [PromAlertingRuleState.Firing]: 'bad',
  [PromAlertingRuleState.Pending]: 'warning',
  [GrafanaAlertState.Alerting]: 'bad',
  [GrafanaAlertState.Error]: 'bad',
  [GrafanaAlertState.NoData]: 'info',
  [GrafanaAlertState.Normal]: 'good',
  [GrafanaAlertState.Pending]: 'warning',
  [AlertState.NoData]: 'info',
  [AlertState.Paused]: 'warning',
  [AlertState.Alerting]: 'bad',
  [AlertState.OK]: 'good',
  [AlertState.Pending]: 'warning',
  [AlertState.Unknown]: 'info',
};

export function getFirstActiveAt(promRule: AlertingRule) {
  if (!promRule.alerts) {
    return null;
  }
  return promRule.alerts.reduce((prev, alert) => {
    const isNotNormal =
      mapStateWithReasonToBaseState(alert.state as GrafanaAlertStateWithReason) !== GrafanaAlertState.Normal;
    if (alert.activeAt && isNotNormal) {
      const activeAt = new Date(alert.activeAt);
      if (prev === null || prev.getTime() > activeAt.getTime()) {
        return activeAt;
      }
    }
    return prev;
  }, null as Date | null);
}

/**
 * A rule group is "federated" when it has at least one "source_tenants" entry, federated rule groups will evaluate rules in multiple tenants
 * Non-federated rules do not have this property
 *
 * see https://grafana.com/docs/metrics-enterprise/latest/tenant-management/tenant-federation/#cross-tenant-alerting-and-recording-rule-federation
 */
export function isFederatedRuleGroup(group: CombinedRuleGroup) {
  return Array.isArray(group.source_tenants);
}

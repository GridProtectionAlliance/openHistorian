import { css, cx } from '@emotion/css';
import { countBy } from 'lodash';
import React, { useMemo, useState } from 'react';

import { GrafanaTheme } from '@grafana/data';
import { LinkButton, useStyles } from '@grafana/ui';
import { MatcherFilter } from 'app/features/alerting/unified/components/alert-groups/MatcherFilter';
import {
  AlertInstanceStateFilter,
  InstanceStateFilter,
} from 'app/features/alerting/unified/components/rules/AlertInstanceStateFilter';
import { labelsMatchMatchers, parseMatchers } from 'app/features/alerting/unified/utils/alertmanager';
import { createViewLink, sortAlerts } from 'app/features/alerting/unified/utils/misc';
import { SortOrder } from 'app/plugins/panel/alertlist/types';
import { Alert, CombinedRule, PaginationProps } from 'app/types/unified-alerting';
import { mapStateWithReasonToBaseState } from 'app/types/unified-alerting-dto';

import { GRAFANA_RULES_SOURCE_NAME, isGrafanaRulesSource } from '../../utils/datasource';
import { isAlertingRule } from '../../utils/rules';
import { DetailsField } from '../DetailsField';

import { AlertInstancesTable } from './AlertInstancesTable';

interface Props {
  rule: CombinedRule;
  pagination?: PaginationProps;
  itemsDisplayLimit?: number;
}

interface ShowMoreStats {
  totalItemsCount: number;
  visibleItemsCount: number;
}

function ShowMoreInstances(props: { ruleViewPageLink: string; stats: ShowMoreStats }) {
  const styles = useStyles(getStyles);
  const { ruleViewPageLink, stats } = props;

  return (
    <div className={styles.footerRow}>
      <div>
        Showing {stats.visibleItemsCount} out of {stats.totalItemsCount} instances
      </div>
      {ruleViewPageLink && (
        <LinkButton href={ruleViewPageLink} size="sm" variant="secondary">
          Show all {stats.totalItemsCount} alert instances
        </LinkButton>
      )}
    </div>
  );
}

export function RuleDetailsMatchingInstances(props: Props): JSX.Element | null {
  const {
    rule: { promRule, namespace },
    itemsDisplayLimit = Number.POSITIVE_INFINITY,
    pagination,
  } = props;

  const [queryString, setQueryString] = useState<string>();
  const [alertState, setAlertState] = useState<InstanceStateFilter>();

  // This key is used to force a rerender on the inputs when the filters are cleared
  const [filterKey] = useState<number>(Math.floor(Math.random() * 100));
  const queryStringKey = `queryString-${filterKey}`;

  const styles = useStyles(getStyles);

  const stateFilterType = isGrafanaRulesSource(namespace.rulesSource) ? GRAFANA_RULES_SOURCE_NAME : 'prometheus';

  const alerts = useMemo(
    (): Alert[] =>
      isAlertingRule(promRule) && promRule.alerts?.length
        ? filterAlerts(queryString, alertState, sortAlerts(SortOrder.Importance, promRule.alerts))
        : [],
    [promRule, alertState, queryString]
  );

  if (!isAlertingRule(promRule)) {
    return null;
  }

  const visibleInstances = alerts.slice(0, itemsDisplayLimit);

  const countAllByState = countBy(promRule.alerts, (alert) => mapStateWithReasonToBaseState(alert.state));
  const hiddenItemsCount = alerts.length - visibleInstances.length;

  const stats: ShowMoreStats = {
    totalItemsCount: alerts.length,
    visibleItemsCount: visibleInstances.length,
  };

  const ruleViewPageLink = createViewLink(namespace.rulesSource, props.rule, location.pathname + location.search);

  const footerRow = hiddenItemsCount ? (
    <ShowMoreInstances stats={stats} ruleViewPageLink={ruleViewPageLink} />
  ) : undefined;

  return (
    <DetailsField label="Matching instances" horizontal={true}>
      <div className={cx(styles.flexRow, styles.spaceBetween)}>
        <div className={styles.flexRow}>
          <MatcherFilter
            className={styles.rowChild}
            key={queryStringKey}
            defaultQueryString={queryString}
            onFilterChange={(value) => setQueryString(value)}
          />
          <AlertInstanceStateFilter
            className={styles.rowChild}
            filterType={stateFilterType}
            stateFilter={alertState}
            onStateFilterChange={setAlertState}
            itemPerStateStats={countAllByState}
          />
        </div>
      </div>

      <AlertInstancesTable instances={visibleInstances} pagination={pagination} footerRow={footerRow} />
    </DetailsField>
  );
}

function filterAlerts(
  alertInstanceLabel: string | undefined,
  alertInstanceState: InstanceStateFilter | undefined,
  alerts: Alert[]
): Alert[] {
  let filteredAlerts = [...alerts];
  if (alertInstanceLabel) {
    const matchers = parseMatchers(alertInstanceLabel || '');
    filteredAlerts = filteredAlerts.filter(({ labels }) => labelsMatchMatchers(labels, matchers));
  }
  if (alertInstanceState) {
    filteredAlerts = filteredAlerts.filter((alert) => {
      return mapStateWithReasonToBaseState(alert.state) === alertInstanceState;
    });
  }

  return filteredAlerts;
}

const getStyles = (theme: GrafanaTheme) => {
  return {
    flexRow: css`
      display: flex;
      flex-direction: row;
      align-items: flex-end;
      width: 100%;
      flex-wrap: wrap;
      margin-bottom: ${theme.spacing.sm};
    `,
    spaceBetween: css`
      justify-content: space-between;
    `,
    rowChild: css`
      margin-right: ${theme.spacing.sm};
    `,
    footerRow: css`
      display: flex;
      flex-direction: column;
      gap: ${theme.spacing.sm};
      justify-content: space-between;
      align-items: center;
      width: 100%;
    `,
  };
};

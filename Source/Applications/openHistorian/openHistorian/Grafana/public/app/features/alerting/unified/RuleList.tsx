import { css } from '@emotion/css';
import React, { useEffect, useMemo, useState } from 'react';
import { useDispatch } from 'react-redux';
import { useLocation } from 'react-router-dom';

import { GrafanaTheme2, urlUtil } from '@grafana/data';
import { Button, LinkButton, useStyles2, withErrorBoundary } from '@grafana/ui';
import { useQueryParams } from 'app/core/hooks/useQueryParams';

import { AlertingPageWrapper } from './components/AlertingPageWrapper';
import { NoRulesSplash } from './components/rules/NoRulesCTA';
import { RuleListErrors } from './components/rules/RuleListErrors';
import { RuleListGroupView } from './components/rules/RuleListGroupView';
import { RuleListStateView } from './components/rules/RuleListStateView';
import { RuleStats } from './components/rules/RuleStats';
import RulesFilter from './components/rules/RulesFilter';
import { useCombinedRuleNamespaces } from './hooks/useCombinedRuleNamespaces';
import { useFilteredRules } from './hooks/useFilteredRules';
import { useUnifiedAlertingSelector } from './hooks/useUnifiedAlertingSelector';
import { fetchAllPromAndRulerRulesAction } from './state/actions';
import { useRulesAccess } from './utils/accessControlHooks';
import { RULE_LIST_POLL_INTERVAL_MS } from './utils/constants';
import { getAllRulesSourceNames } from './utils/datasource';
import { getFiltersFromUrlParams } from './utils/misc';

const VIEWS = {
  groups: RuleListGroupView,
  state: RuleListStateView,
};

const RuleList = withErrorBoundary(
  () => {
    const dispatch = useDispatch();
    const styles = useStyles2(getStyles);
    const rulesDataSourceNames = useMemo(getAllRulesSourceNames, []);
    const location = useLocation();
    const [expandAll, setExpandAll] = useState(false);

    const [queryParams] = useQueryParams();
    const filters = getFiltersFromUrlParams(queryParams);
    const filtersActive = Object.values(filters).some((filter) => filter !== undefined);

    const { canCreateGrafanaRules, canCreateCloudRules } = useRulesAccess();

    const view = VIEWS[queryParams['view'] as keyof typeof VIEWS]
      ? (queryParams['view'] as keyof typeof VIEWS)
      : 'groups';

    const ViewComponent = VIEWS[view];

    // fetch rules, then poll every RULE_LIST_POLL_INTERVAL_MS
    useEffect(() => {
      dispatch(fetchAllPromAndRulerRulesAction());
      const interval = setInterval(() => dispatch(fetchAllPromAndRulerRulesAction()), RULE_LIST_POLL_INTERVAL_MS);
      return () => {
        clearInterval(interval);
      };
    }, [dispatch]);

    const promRuleRequests = useUnifiedAlertingSelector((state) => state.promRules);
    const rulerRuleRequests = useUnifiedAlertingSelector((state) => state.rulerRules);

    const dispatched = rulesDataSourceNames.some(
      (name) => promRuleRequests[name]?.dispatched || rulerRuleRequests[name]?.dispatched
    );
    const loading = rulesDataSourceNames.some(
      (name) => promRuleRequests[name]?.loading || rulerRuleRequests[name]?.loading
    );
    const haveResults = rulesDataSourceNames.some(
      (name) =>
        (promRuleRequests[name]?.result?.length && !promRuleRequests[name]?.error) ||
        (Object.keys(rulerRuleRequests[name]?.result || {}).length && !rulerRuleRequests[name]?.error)
    );

    const showNewAlertSplash = dispatched && !loading && !haveResults;

    const combinedNamespaces = useCombinedRuleNamespaces();
    const filteredNamespaces = useFilteredRules(combinedNamespaces);
    return (
      <AlertingPageWrapper pageId="alert-list" isLoading={loading && !haveResults}>
        <RuleListErrors />
        {!showNewAlertSplash && (
          <>
            <RulesFilter />
            <div className={styles.break} />
            <div className={styles.buttonsContainer}>
              <div className={styles.statsContainer}>
                {view === 'groups' && filtersActive && (
                  <Button
                    className={styles.expandAllButton}
                    icon={expandAll ? 'angle-double-up' : 'angle-double-down'}
                    variant="secondary"
                    onClick={() => setExpandAll(!expandAll)}
                  >
                    {expandAll ? 'Collapse all' : 'Expand all'}
                  </Button>
                )}
                <RuleStats showInactive={true} showRecording={true} namespaces={filteredNamespaces} />
              </div>
              {(canCreateGrafanaRules || canCreateCloudRules) && (
                <LinkButton
                  href={urlUtil.renderUrl('alerting/new', { returnTo: location.pathname + location.search })}
                  icon="plus"
                >
                  New alert rule
                </LinkButton>
              )}
            </div>
          </>
        )}
        {showNewAlertSplash && <NoRulesSplash />}
        {haveResults && <ViewComponent expandAll={expandAll} namespaces={filteredNamespaces} />}
      </AlertingPageWrapper>
    );
  },
  { style: 'page' }
);

const getStyles = (theme: GrafanaTheme2) => ({
  break: css`
    width: 100%;
    height: 0;
    margin-bottom: ${theme.spacing(2)};
    border-bottom: solid 1px ${theme.colors.border.medium};
  `,
  buttonsContainer: css`
    margin-bottom: ${theme.spacing(2)};
    display: flex;
    justify-content: space-between;
  `,
  statsContainer: css`
    display: flex;
    flex-direction: row;
    align-items: center;
  `,
  expandAllButton: css`
    margin-right: ${theme.spacing(1)};
  `,
});

export default RuleList;

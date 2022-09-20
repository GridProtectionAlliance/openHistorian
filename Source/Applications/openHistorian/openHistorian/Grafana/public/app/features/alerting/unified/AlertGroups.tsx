import { css } from '@emotion/css';
import React, { useEffect } from 'react';
import { useDispatch } from 'react-redux';

import { GrafanaTheme2 } from '@grafana/data';
import { Alert, LoadingPlaceholder, useStyles2 } from '@grafana/ui';
import { useQueryParams } from 'app/core/hooks/useQueryParams';

import { AlertingPageWrapper } from './components/AlertingPageWrapper';
import { NoAlertManagerWarning } from './components/NoAlertManagerWarning';
import { AlertGroup } from './components/alert-groups/AlertGroup';
import { AlertGroupFilter } from './components/alert-groups/AlertGroupFilter';
import { useAlertManagerSourceName } from './hooks/useAlertManagerSourceName';
import { useAlertManagersByPermission } from './hooks/useAlertManagerSources';
import { useFilteredAmGroups } from './hooks/useFilteredAmGroups';
import { useGroupedAlerts } from './hooks/useGroupedAlerts';
import { useUnifiedAlertingSelector } from './hooks/useUnifiedAlertingSelector';
import { fetchAlertGroupsAction } from './state/actions';
import { NOTIFICATIONS_POLL_INTERVAL_MS } from './utils/constants';
import { getFiltersFromUrlParams } from './utils/misc';
import { initialAsyncRequestState } from './utils/redux';

const AlertGroups = () => {
  const alertManagers = useAlertManagersByPermission('instance');
  const [alertManagerSourceName] = useAlertManagerSourceName(alertManagers);
  const dispatch = useDispatch();
  const [queryParams] = useQueryParams();
  const { groupBy = [] } = getFiltersFromUrlParams(queryParams);
  const styles = useStyles2(getStyles);

  const alertGroups = useUnifiedAlertingSelector((state) => state.amAlertGroups);
  const {
    loading,
    error,
    result: results = [],
  } = alertGroups[alertManagerSourceName || ''] ?? initialAsyncRequestState;

  const groupedAlerts = useGroupedAlerts(results, groupBy);
  const filteredAlertGroups = useFilteredAmGroups(groupedAlerts);

  useEffect(() => {
    function fetchNotifications() {
      if (alertManagerSourceName) {
        dispatch(fetchAlertGroupsAction(alertManagerSourceName));
      }
    }
    fetchNotifications();
    const interval = setInterval(fetchNotifications, NOTIFICATIONS_POLL_INTERVAL_MS);
    return () => {
      clearInterval(interval);
    };
  }, [dispatch, alertManagerSourceName]);

  if (!alertManagerSourceName) {
    return (
      <AlertingPageWrapper pageId="groups">
        <NoAlertManagerWarning availableAlertManagers={alertManagers} />
      </AlertingPageWrapper>
    );
  }

  return (
    <AlertingPageWrapper pageId="groups">
      <AlertGroupFilter groups={results} />
      {loading && <LoadingPlaceholder text="Loading notifications" />}
      {error && !loading && (
        <Alert title={'Error loading notifications'} severity={'error'}>
          {error.message || 'Unknown error'}
        </Alert>
      )}
      {results &&
        filteredAlertGroups.map((group, index) => {
          return (
            <React.Fragment key={`${JSON.stringify(group.labels)}-group-${index}`}>
              {((index === 1 && Object.keys(filteredAlertGroups[0].labels).length === 0) ||
                (index === 0 && Object.keys(group.labels).length > 0)) && (
                <p className={styles.groupingBanner}>Grouped by: {Object.keys(group.labels).join(', ')}</p>
              )}
              <AlertGroup alertManagerSourceName={alertManagerSourceName || ''} group={group} />
            </React.Fragment>
          );
        })}
      {results && !filteredAlertGroups.length && <p>No results.</p>}
    </AlertingPageWrapper>
  );
};

const getStyles = (theme: GrafanaTheme2) => ({
  groupingBanner: css`
    margin: ${theme.spacing(2, 0)};
  `,
});

export default AlertGroups;

import React from 'react';
import { RefreshPicker, defaultIntervals } from '@grafana/ui';
import { selectors } from '@grafana/e2e-selectors';
import memoizeOne from 'memoize-one';
import { css } from 'emotion';
import classNames from 'classnames';

import { ResponsiveButton } from './ResponsiveButton';

import { getTimeSrv } from 'app/features/dashboard/services/TimeSrv';

const getStyles = memoizeOne(() => {
  return {
    selectButtonOverride: css`
      label: selectButtonOverride;
      .select-button-value {
        color: white !important;
      }
    `,
  };
});

export type Props = {
  splitted: boolean;
  loading: boolean;
  isLive: boolean;
  onRun: (loading: boolean) => void;
  refreshInterval?: string;
  onChangeRefreshInterval: (interval: string) => void;
  showDropdown: boolean;
};

export function RunButton(props: Props) {
  const { splitted, loading, onRun, onChangeRefreshInterval, refreshInterval, showDropdown, isLive } = props;
  const styles = getStyles();
  const intervals = getTimeSrv().getValidIntervals(defaultIntervals);

  const runButton = (
    <ResponsiveButton
      splitted={splitted}
      title={loading && !isLive ? 'Cancel' : 'Run Query'}
      onClick={() => onRun(loading)}
      buttonClassName={classNames({
        'navbar-button--primary': isLive || !loading,
        'navbar-button--danger': loading && !isLive,
        'btn--radius-right-0': showDropdown,
      })}
      icon={loading ? 'fa fa-spinner' : 'sync'}
      iconClassName={loading ? ' fa-spin run-icon' : undefined}
      aria-label={selectors.pages.Explore.General.runButton}
    />
  );

  if (showDropdown) {
    return (
      <RefreshPicker
        onIntervalChanged={onChangeRefreshInterval}
        value={refreshInterval}
        buttonSelectClassName={`${loading ? 'navbar-button--danger' : 'navbar-button--primary'} ${
          styles.selectButtonOverride
        }`}
        refreshButton={runButton}
        intervals={intervals}
      />
    );
  }
  return runButton;
}

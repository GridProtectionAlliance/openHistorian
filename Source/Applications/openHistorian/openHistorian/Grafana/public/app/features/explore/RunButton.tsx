import React from 'react';
import { RefreshPicker } from '@grafana/ui';
import memoizeOne from 'memoize-one';
import { css } from 'emotion';
import classNames from 'classnames';

import { ResponsiveButton } from './ResponsiveButton';

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

type Props = {
  splitted: boolean;
  loading: boolean;
  onRun: () => void;
  refreshInterval: string;
  onChangeRefreshInterval: (interval: string) => void;
  showDropdown: boolean;
};

export function RunButton(props: Props) {
  const { splitted, loading, onRun, onChangeRefreshInterval, refreshInterval, showDropdown } = props;
  const styles = getStyles();
  const runButton = (
    <ResponsiveButton
      splitted={splitted}
      title="Run Query"
      onClick={onRun}
      buttonClassName={classNames('navbar-button--secondary', { 'btn--radius-right-0': showDropdown })}
      iconClassName={loading ? 'fa fa-spinner fa-fw fa-spin run-icon' : 'fa fa-refresh fa-fw'}
    />
  );

  if (showDropdown) {
    return (
      <RefreshPicker
        onIntervalChanged={onChangeRefreshInterval}
        value={refreshInterval}
        buttonSelectClassName={`navbar-button--secondary ${styles.selectButtonOverride}`}
        refreshButton={runButton}
      />
    );
  }
  return runButton;
}

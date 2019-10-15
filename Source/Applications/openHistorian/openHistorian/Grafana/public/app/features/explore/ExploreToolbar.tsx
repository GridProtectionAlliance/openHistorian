import omitBy from 'lodash/omitBy';
import React, { PureComponent } from 'react';
import { connect } from 'react-redux';
import { hot } from 'react-hot-loader';
import memoizeOne from 'memoize-one';
import classNames from 'classnames';
import { css } from 'emotion';

import { ExploreId, ExploreItemState, ExploreMode } from 'app/types/explore';
import {
  DataSourceSelectItem,
  ToggleButtonGroup,
  ToggleButton,
  DataQuery,
  Tooltip,
  ButtonSelect,
  RefreshPicker,
  SetInterval,
} from '@grafana/ui';
import { RawTimeRange, TimeZone, TimeRange, SelectableValue } from '@grafana/data';
import { DataSourcePicker } from 'app/core/components/Select/DataSourcePicker';
import { StoreState } from 'app/types/store';
import {
  changeDatasource,
  clearQueries,
  splitClose,
  runQueries,
  splitOpen,
  changeRefreshInterval,
  changeMode,
  clearOrigin,
} from './state/actions';
import { changeRefreshIntervalAction, setPausedStateAction } from './state/actionTypes';
import { updateLocation } from 'app/core/actions';
import { getTimeZone } from '../profile/state/selectors';
import { getDashboardSrv } from '../dashboard/services/DashboardSrv';
import kbn from '../../core/utils/kbn';
import { ExploreTimeControls } from './ExploreTimeControls';
import { LiveTailButton } from './LiveTailButton';
import { ResponsiveButton } from './ResponsiveButton';
import { RunButton } from './RunButton';

const getStyles = memoizeOne(() => {
  return {
    liveTailButtons: css`
      margin-left: 10px;
    `,
  };
});

interface OwnProps {
  exploreId: ExploreId;
  onChangeTime: (range: RawTimeRange, changedByScanner?: boolean) => void;
}

interface StateProps {
  datasourceMissing: boolean;
  exploreDatasources: DataSourceSelectItem[];
  loading: boolean;
  range: TimeRange;
  timeZone: TimeZone;
  selectedDatasource: DataSourceSelectItem;
  splitted: boolean;
  refreshInterval: string;
  supportedModeOptions: Array<SelectableValue<ExploreMode>>;
  selectedModeOption: SelectableValue<ExploreMode>;
  hasLiveOption: boolean;
  isLive: boolean;
  isPaused: boolean;
  originPanelId: number;
  queries: DataQuery[];
}

interface DispatchProps {
  changeDatasource: typeof changeDatasource;
  clearAll: typeof clearQueries;
  runQueries: typeof runQueries;
  closeSplit: typeof splitClose;
  split: typeof splitOpen;
  changeRefreshInterval: typeof changeRefreshInterval;
  changeMode: typeof changeMode;
  clearOrigin: typeof clearOrigin;
  updateLocation: typeof updateLocation;
  changeRefreshIntervalAction: typeof changeRefreshIntervalAction;
  setPausedStateAction: typeof setPausedStateAction;
}

type Props = StateProps & DispatchProps & OwnProps;

export class UnConnectedExploreToolbar extends PureComponent<Props, {}> {
  constructor(props: Props) {
    super(props);
  }

  onChangeDatasource = async (option: { value: any }) => {
    this.props.changeDatasource(this.props.exploreId, option.value);
  };

  onClearAll = () => {
    this.props.clearAll(this.props.exploreId);
  };

  onRunQuery = () => {
    return this.props.runQueries(this.props.exploreId);
  };

  onChangeRefreshInterval = (item: string) => {
    const { changeRefreshInterval, exploreId } = this.props;
    changeRefreshInterval(exploreId, item);
  };

  onModeChange = (mode: ExploreMode) => {
    const { changeMode, exploreId } = this.props;
    changeMode(exploreId, mode);
  };

  returnToPanel = async ({ withChanges = false } = {}) => {
    const { originPanelId } = this.props;

    const dashboardSrv = getDashboardSrv();
    const dash = dashboardSrv.getCurrent();
    const titleSlug = kbn.slugifyForUrl(dash.title);

    if (!withChanges) {
      this.props.clearOrigin();
    }

    const dashViewOptions = {
      fullscreen: withChanges || dash.meta.fullscreen,
      edit: withChanges || dash.meta.isEditing,
    };

    this.props.updateLocation({
      path: `/d/${dash.uid}/:${titleSlug}`,
      query: {
        ...omitBy(dashViewOptions, v => !v),
        panelId: originPanelId,
      },
    });
  };

  stopLive = () => {
    const { exploreId } = this.props;
    this.pauseLive();
    // TODO referencing this from perspective of refresh picker when there is designated button for it now is not
    //  great. Needs another refactor.
    this.props.changeRefreshIntervalAction({ exploreId, refreshInterval: RefreshPicker.offOption.value });
  };

  startLive = () => {
    const { exploreId } = this.props;
    this.props.changeRefreshIntervalAction({ exploreId, refreshInterval: RefreshPicker.liveOption.value });
  };

  pauseLive = () => {
    const { exploreId } = this.props;
    this.props.setPausedStateAction({ exploreId, isPaused: true });
  };

  resumeLive = () => {
    const { exploreId } = this.props;
    this.props.setPausedStateAction({ exploreId, isPaused: false });
  };

  render() {
    const {
      datasourceMissing,
      exploreDatasources,
      closeSplit,
      exploreId,
      loading,
      range,
      timeZone,
      selectedDatasource,
      splitted,
      refreshInterval,
      onChangeTime,
      split,
      supportedModeOptions,
      selectedModeOption,
      hasLiveOption,
      isLive,
      isPaused,
      originPanelId,
    } = this.props;

    const styles = getStyles();
    const originDashboardIsEditable = Number.isInteger(originPanelId);
    const panelReturnClasses = classNames('btn', 'navbar-button', {
      'btn--radius-right-0': originDashboardIsEditable,
      'navbar-button navbar-button--border-right-0': originDashboardIsEditable,
    });

    return (
      <div className={splitted ? 'explore-toolbar splitted' : 'explore-toolbar'}>
        <div className="explore-toolbar-item">
          <div className="explore-toolbar-header">
            <div className="explore-toolbar-header-title">
              {exploreId === 'left' && (
                <span className="navbar-page-btn">
                  <i className="gicon gicon-explore" />
                  Explore
                </span>
              )}
            </div>
            {splitted && (
              <a className="explore-toolbar-header-close" onClick={() => closeSplit(exploreId)}>
                <i className="fa fa-times fa-fw" />
              </a>
            )}
          </div>
        </div>
        <div className="explore-toolbar-item">
          <div className="explore-toolbar-content">
            {!datasourceMissing ? (
              <div className="explore-toolbar-content-item">
                <div className="datasource-picker">
                  <DataSourcePicker
                    onChange={this.onChangeDatasource}
                    datasources={exploreDatasources}
                    current={selectedDatasource}
                  />
                </div>
                {supportedModeOptions.length > 1 ? (
                  <div className="query-type-toggle">
                    <ToggleButtonGroup label="" transparent={true}>
                      <ToggleButton
                        key={ExploreMode.Metrics}
                        value={ExploreMode.Metrics}
                        onChange={this.onModeChange}
                        selected={selectedModeOption.value === ExploreMode.Metrics}
                      >
                        {'Metrics'}
                      </ToggleButton>
                      <ToggleButton
                        key={ExploreMode.Logs}
                        value={ExploreMode.Logs}
                        onChange={this.onModeChange}
                        selected={selectedModeOption.value === ExploreMode.Logs}
                      >
                        {'Logs'}
                      </ToggleButton>
                    </ToggleButtonGroup>
                  </div>
                ) : null}
              </div>
            ) : null}

            {Number.isInteger(originPanelId) && !splitted && (
              <div className="explore-toolbar-content-item">
                <Tooltip content={'Return to panel'} placement="bottom">
                  <button className={panelReturnClasses} onClick={() => this.returnToPanel()}>
                    <i className="fa fa-arrow-left" />
                  </button>
                </Tooltip>
                {originDashboardIsEditable && (
                  <ButtonSelect
                    className="navbar-button--attached btn--radius-left-0$"
                    options={[{ label: 'Return to panel with changes', value: '' }]}
                    onChange={() => this.returnToPanel({ withChanges: true })}
                    maxMenuHeight={380}
                  />
                )}
              </div>
            )}

            {exploreId === 'left' && !splitted ? (
              <div className="explore-toolbar-content-item">
                <ResponsiveButton
                  splitted={splitted}
                  title="Split"
                  onClick={split}
                  iconClassName="fa fa-fw fa-columns icon-margin-right"
                  disabled={isLive}
                />
              </div>
            ) : null}
            {!isLive && (
              <div className="explore-toolbar-content-item">
                <ExploreTimeControls
                  exploreId={exploreId}
                  range={range}
                  timeZone={timeZone}
                  onChangeTime={onChangeTime}
                />
              </div>
            )}

            <div className="explore-toolbar-content-item">
              <button className="btn navbar-button" onClick={this.onClearAll}>
                Clear All
              </button>
            </div>
            <div className="explore-toolbar-content-item">
              <RunButton
                refreshInterval={refreshInterval}
                onChangeRefreshInterval={this.onChangeRefreshInterval}
                splitted={splitted}
                loading={loading || (isLive && !isPaused)}
                onRun={this.onRunQuery}
                showDropdown={!isLive}
              />
              {refreshInterval && <SetInterval func={this.onRunQuery} interval={refreshInterval} loading={loading} />}
            </div>

            {hasLiveOption && (
              <div className={`explore-toolbar-content-item ${styles.liveTailButtons}`}>
                <LiveTailButton
                  isLive={isLive}
                  isPaused={isPaused}
                  start={this.startLive}
                  pause={this.pauseLive}
                  resume={this.resumeLive}
                  stop={this.stopLive}
                />
              </div>
            )}
          </div>
        </div>
      </div>
    );
  }
}

const getModeOptionsMemoized = memoizeOne(
  (
    supportedModes: ExploreMode[],
    mode: ExploreMode
  ): [Array<SelectableValue<ExploreMode>>, SelectableValue<ExploreMode>] => {
    const supportedModeOptions: Array<SelectableValue<ExploreMode>> = [];
    let selectedModeOption = null;
    for (const supportedMode of supportedModes) {
      switch (supportedMode) {
        case ExploreMode.Metrics:
          const option1 = {
            value: ExploreMode.Metrics,
            label: ExploreMode.Metrics,
          };
          supportedModeOptions.push(option1);
          if (mode === ExploreMode.Metrics) {
            selectedModeOption = option1;
          }
          break;
        case ExploreMode.Logs:
          const option2 = {
            value: ExploreMode.Logs,
            label: ExploreMode.Logs,
          };
          supportedModeOptions.push(option2);
          if (mode === ExploreMode.Logs) {
            selectedModeOption = option2;
          }
          break;
      }
    }
    return [supportedModeOptions, selectedModeOption];
  }
);

const mapStateToProps = (state: StoreState, { exploreId }: OwnProps): StateProps => {
  const splitted = state.explore.split;
  const exploreItem: ExploreItemState = state.explore[exploreId];
  const {
    datasourceInstance,
    datasourceMissing,
    exploreDatasources,
    range,
    refreshInterval,
    loading,
    supportedModes,
    mode,
    isLive,
    isPaused,
    originPanelId,
    queries,
  } = exploreItem;
  const selectedDatasource = datasourceInstance
    ? exploreDatasources.find(datasource => datasource.name === datasourceInstance.name)
    : undefined;
  const hasLiveOption =
    datasourceInstance && datasourceInstance.meta && datasourceInstance.meta.streaming ? true : false;

  const [supportedModeOptions, selectedModeOption] = getModeOptionsMemoized(supportedModes, mode);

  return {
    datasourceMissing,
    exploreDatasources,
    loading,
    range,
    timeZone: getTimeZone(state.user),
    selectedDatasource,
    splitted,
    refreshInterval,
    supportedModeOptions,
    selectedModeOption,
    hasLiveOption,
    isLive,
    isPaused,
    originPanelId,
    queries,
  };
};

const mapDispatchToProps: DispatchProps = {
  changeDatasource,
  updateLocation,
  changeRefreshInterval,
  clearAll: clearQueries,
  runQueries,
  closeSplit: splitClose,
  split: splitOpen,
  changeMode: changeMode,
  clearOrigin,
  changeRefreshIntervalAction,
  setPausedStateAction,
};

export const ExploreToolbar = hot(module)(
  connect(
    mapStateToProps,
    mapDispatchToProps
  )(UnConnectedExploreToolbar)
);

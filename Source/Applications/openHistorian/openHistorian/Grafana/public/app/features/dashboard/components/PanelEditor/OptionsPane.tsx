import { css } from '@emotion/css';
import React from 'react';
import { useSelector } from 'react-redux';

import { GrafanaTheme } from '@grafana/data';
import { selectors } from '@grafana/e2e-selectors';
import { useStyles } from '@grafana/ui';
import { StoreState } from 'app/types';

import { OptionsPaneOptions } from './OptionsPaneOptions';
import { VisualizationButton } from './VisualizationButton';
import { VisualizationSelectPane } from './VisualizationSelectPane';
import { OptionPaneRenderProps } from './types';
import { usePanelLatestData } from './usePanelLatestData';

export const OptionsPane: React.FC<OptionPaneRenderProps> = ({
  plugin,
  panel,
  onFieldConfigsChange,
  onPanelOptionsChanged,
  onPanelConfigChange,
  dashboard,
  instanceState,
}) => {
  const styles = useStyles(getStyles);
  const isVizPickerOpen = useSelector((state: StoreState) => state.panelEditor.isVizPickerOpen);
  const { data } = usePanelLatestData(panel, { withTransforms: true, withFieldConfig: false }, true);

  return (
    <div className={styles.wrapper} aria-label={selectors.components.PanelEditor.OptionsPane.content}>
      {!isVizPickerOpen && (
        <>
          <div className={styles.vizButtonWrapper}>
            <VisualizationButton panel={panel} />
          </div>
          <div className={styles.optionsWrapper}>
            <OptionsPaneOptions
              panel={panel}
              dashboard={dashboard}
              plugin={plugin}
              instanceState={instanceState}
              data={data}
              onFieldConfigsChange={onFieldConfigsChange}
              onPanelOptionsChanged={onPanelOptionsChanged}
              onPanelConfigChange={onPanelConfigChange}
            />
          </div>
        </>
      )}
      {isVizPickerOpen && <VisualizationSelectPane panel={panel} data={data} />}
    </div>
  );
};

const getStyles = (theme: GrafanaTheme) => {
  return {
    wrapper: css`
      height: 100%;
      width: 100%;
      display: flex;
      flex: 1 1 0;
      flex-direction: column;
      padding: 0;
    `,
    optionsWrapper: css`
      flex-grow: 1;
      min-height: 0;
    `,
    vizButtonWrapper: css`
      padding: 0 ${theme.spacing.md} ${theme.spacing.md} 0;
    `,
    legacyOptions: css`
      label: legacy-options;
      .panel-options-grid {
        display: flex;
        flex-direction: column;
      }
      .panel-options-group {
        margin-bottom: 0;
      }
      .panel-options-group__body {
        padding: ${theme.spacing.md} 0;
      }

      .section {
        display: block;
        margin: ${theme.spacing.md} 0;

        &:first-child {
          margin-top: 0;
        }
      }
    `,
  };
};

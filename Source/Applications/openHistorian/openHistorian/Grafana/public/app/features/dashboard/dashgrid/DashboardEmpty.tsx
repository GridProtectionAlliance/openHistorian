import { css, cx } from '@emotion/css';
import React from 'react';

import { GrafanaTheme2 } from '@grafana/data';
import { locationService, reportInteraction } from '@grafana/runtime';
import { Button, useStyles2 } from '@grafana/ui';
import { DashboardModel } from 'app/features/dashboard/state';
import { onAddLibraryPanel, onCreateNewPanel, onCreateNewRow } from 'app/features/dashboard/utils/dashboard';

export interface Props {
  dashboard: DashboardModel;
  canCreate: boolean;
}

export const DashboardEmpty = ({ dashboard, canCreate }: Props) => {
  const styles = useStyles2(getStyles);

  return (
    <div className={styles.centeredContent}>
      <div className={cx(styles.centeredContent, styles.wrapper)}>
        <div className={cx(styles.containerBox, styles.centeredContent, styles.visualizationContainer)}>
          <h1 className={cx(styles.headerSection, styles.headerBig)}>
            Start your new dashboard by adding a visualization
          </h1>
          <div className={cx(styles.bodySection, styles.bodyBig)}>
            Select a data source and then query and visualize your data with charts, stats and tables or create lists,
            markdowns and other widgets.
          </div>
          <Button
            size="lg"
            icon="plus"
            aria-label="Add new panel"
            onClick={() => {
              reportInteraction('Create new panel');
              const id = onCreateNewPanel(dashboard);
              locationService.partial({ editPanel: id });
            }}
            disabled={!canCreate}
          >
            Add visualization
          </Button>
        </div>
        <div className={cx(styles.centeredContent, styles.others)}>
          <div className={cx(styles.containerBox, styles.centeredContent, styles.rowContainer)}>
            <h3 className={cx(styles.headerSection, styles.headerSmall)}>Add a row</h3>
            <div className={cx(styles.bodySection, styles.bodySmall)}>
              Group your visualizations into expandable sections.
            </div>
            <Button
              icon="plus"
              fill="outline"
              aria-label="Add new row"
              onClick={() => {
                reportInteraction('Create new row');
                onCreateNewRow(dashboard);
              }}
              disabled={!canCreate}
            >
              Add row
            </Button>
          </div>
          <div className={cx(styles.containerBox, styles.centeredContent, styles.libraryContainer)}>
            <h3 className={cx(styles.headerSection, styles.headerSmall)}>Import panel</h3>
            <div className={cx(styles.bodySection, styles.bodySmall)}>
              Import visualizations that are shared with other dashboards.
            </div>
            <Button
              icon="plus"
              fill="outline"
              aria-label="Add new panel from panel library"
              onClick={() => {
                reportInteraction('Add a panel from the panel library');
                onAddLibraryPanel(dashboard);
              }}
              disabled={!canCreate}
            >
              Import library panel
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};

function getStyles(theme: GrafanaTheme2) {
  return {
    wrapper: css({
      label: 'dashboard-empty-wrapper',
      flexDirection: 'column',
      maxWidth: '890px',
      gap: theme.spacing.gridSize * 4,
    }),
    containerBox: css({
      label: 'container-box',
      flexDirection: 'column',
      boxSizing: 'border-box',
      border: '1px dashed rgba(110, 159, 255, 0.5)',
    }),
    centeredContent: css({
      label: 'centered',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
    }),
    visualizationContainer: css({
      label: 'visualization-container',
      padding: theme.spacing.gridSize * 4,
    }),
    others: css({
      label: 'others-wrapper',
      alignItems: 'stretch',
      flexDirection: 'row',
      gap: theme.spacing.gridSize * 4,

      [theme.breakpoints.down('sm')]: {
        flexDirection: 'column',
      },
    }),
    rowContainer: css({
      label: 'row-container',
      padding: theme.spacing.gridSize * 3,
    }),
    libraryContainer: css({
      label: 'library-container',
      padding: theme.spacing.gridSize * 3,
    }),
    visualizationContent: css({
      gap: theme.spacing.gridSize * 2,
    }),
    headerSection: css({
      label: 'header-section',
      fontWeight: 600,
      textAlign: 'center',
    }),
    headerBig: css({
      marginBottom: theme.spacing.gridSize * 2,
    }),
    headerSmall: css({
      marginBottom: theme.spacing.gridSize,
    }),
    bodySection: css({
      label: 'body-section',
      fontWeight: theme.typography.fontWeightRegular,
      fontSize: theme.typography.body.fontSize,
      lineHeight: theme.typography.body.lineHeight,
      color: theme.colors.text.secondary,
      textAlign: 'center',
    }),
    bodyBig: css({
      maxWidth: '75%',
      marginBottom: theme.spacing.gridSize * 4,
    }),
    bodySmall: css({
      marginBottom: theme.spacing.gridSize * 3,
    }),
  };
}

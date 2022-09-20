import { css } from '@emotion/css';
import React, { useEffect } from 'react';

import { GrafanaTheme } from '@grafana/data';
import { Button, ConfirmModal, Modal, stylesFactory, useTheme } from '@grafana/ui';
import { DashboardModel } from 'app/features/dashboard/state';

import { SaveDashboardAsButton } from './SaveDashboardButton';
import { SaveDashboardModalProps } from './types';
import { useDashboardSave } from './useDashboardSave';

interface SaveDashboardErrorProxyProps {
  /** original dashboard */
  dashboard: DashboardModel;
  /** dashboard save model with applied modifications, i.e. title */
  dashboardSaveModel: any;
  error: any;
  onDismiss: () => void;
}

export const SaveDashboardErrorProxy: React.FC<SaveDashboardErrorProxyProps> = ({
  dashboard,
  dashboardSaveModel,
  error,
  onDismiss,
}) => {
  const { onDashboardSave } = useDashboardSave(dashboard);

  useEffect(() => {
    if (error.data && isHandledError(error.data.status)) {
      error.isHandled = true;
    }
  }, [error]);

  return (
    <>
      {error.data && error.data.status === 'version-mismatch' && (
        <ConfirmModal
          isOpen={true}
          title="Conflict"
          body={
            <div>
              Someone else has updated this dashboard <br /> <small>Would you still like to save this dashboard?</small>
            </div>
          }
          confirmText="Save and overwrite"
          onConfirm={async () => {
            await onDashboardSave(dashboardSaveModel, { overwrite: true }, dashboard);
            onDismiss();
          }}
          onDismiss={onDismiss}
        />
      )}
      {error.data && error.data.status === 'name-exists' && (
        <ConfirmModal
          isOpen={true}
          title="Conflict"
          body={
            <div>
              A dashboard with the same name in selected folder already exists. <br />
              <small>Would you still like to save this dashboard?</small>
            </div>
          }
          confirmText="Save and overwrite"
          onConfirm={async () => {
            await onDashboardSave(dashboardSaveModel, { overwrite: true }, dashboard);
            onDismiss();
          }}
          onDismiss={onDismiss}
        />
      )}
      {error.data && error.data.status === 'plugin-dashboard' && (
        <ConfirmPluginDashboardSaveModal dashboard={dashboard} onDismiss={onDismiss} />
      )}
    </>
  );
};

const ConfirmPluginDashboardSaveModal: React.FC<SaveDashboardModalProps> = ({ onDismiss, dashboard }) => {
  const theme = useTheme();
  const { onDashboardSave } = useDashboardSave(dashboard);
  const styles = getConfirmPluginDashboardSaveModalStyles(theme);

  return (
    <Modal className={styles.modal} title="Plugin dashboard" icon="copy" isOpen={true} onDismiss={onDismiss}>
      <div className={styles.modalText}>
        Your changes will be lost when you update the plugin.
        <br />
        <small>
          Use <strong>Save As</strong> to create custom version.
        </small>
      </div>
      <Modal.ButtonRow>
        <Button variant="secondary" onClick={onDismiss} fill="outline">
          Cancel
        </Button>
        <SaveDashboardAsButton dashboard={dashboard} onSaveSuccess={onDismiss} />
        <Button
          variant="destructive"
          onClick={async () => {
            await onDashboardSave(dashboard.getSaveModelClone(), { overwrite: true }, dashboard);
            onDismiss();
          }}
        >
          Overwrite
        </Button>
      </Modal.ButtonRow>
    </Modal>
  );
};

const isHandledError = (errorStatus: string) => {
  switch (errorStatus) {
    case 'version-mismatch':
    case 'name-exists':
    case 'plugin-dashboard':
      return true;

    default:
      return false;
  }
};

const getConfirmPluginDashboardSaveModalStyles = stylesFactory((theme: GrafanaTheme) => ({
  modal: css`
    width: 500px;
  `,
  modalText: css`
    font-size: ${theme.typography.heading.h4};
    color: ${theme.colors.link};
    margin-bottom: calc(${theme.spacing.d} * 2);
    padding-top: ${theme.spacing.d};
  `,
  modalButtonRow: css`
    margin-bottom: 14px;
    a,
    button {
      margin-right: ${theme.spacing.d};
    }
  `,
}));

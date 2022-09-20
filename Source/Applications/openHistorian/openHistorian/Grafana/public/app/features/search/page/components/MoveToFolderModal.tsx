import { css } from '@emotion/css';
import React, { FC, useState } from 'react';

import { GrafanaTheme } from '@grafana/data';
import { Button, HorizontalGroup, Modal, stylesFactory, useTheme } from '@grafana/ui';
import { FolderPicker } from 'app/core/components/Select/FolderPicker';
import { useAppNotification } from 'app/core/copy/appNotification';
import { moveDashboards } from 'app/features/manage-dashboards/state/actions';
import { FolderInfo } from 'app/types';

import { OnMoveOrDeleleSelectedItems } from '../../types';

interface Props {
  onMoveItems: OnMoveOrDeleleSelectedItems;
  results: Map<string, Set<string>>;
  isOpen: boolean;
  onDismiss: () => void;
}

export const MoveToFolderModal: FC<Props> = ({ results, onMoveItems, isOpen, onDismiss }) => {
  const [folder, setFolder] = useState<FolderInfo | null>(null);
  const theme = useTheme();
  const styles = getStyles(theme);
  const notifyApp = useAppNotification();
  const selectedDashboards = Array.from(results.get('dashboard') ?? []);
  const [moving, setMoving] = useState(false);

  const moveTo = () => {
    if (folder && selectedDashboards.length) {
      const folderTitle = folder.title ?? 'General';
      setMoving(true);
      moveDashboards(selectedDashboards, folder).then((result: any) => {
        if (result.successCount > 0) {
          const ending = result.successCount === 1 ? '' : 's';
          const header = `Dashboard${ending} Moved`;
          const msg = `${result.successCount} dashboard${ending} moved to ${folderTitle}`;
          notifyApp.success(header, msg);
        }

        if (result.totalCount === result.alreadyInFolderCount) {
          notifyApp.error('Error', `Dashboard already belongs to folder ${folderTitle}`);
        } else {
          //update the list
          onMoveItems();
        }

        setMoving(false);
        onDismiss();
      });
    }
  };

  return isOpen ? (
    <Modal
      className={styles.modal}
      title="Choose Dashboard Folder"
      icon="folder-plus"
      isOpen={isOpen}
      onDismiss={onDismiss}
    >
      <>
        <div className={styles.content}>
          <p>
            Move the {selectedDashboards.length} selected dashboard{selectedDashboards.length === 1 ? '' : 's'} to the
            following folder:
          </p>
          <FolderPicker onChange={(f) => setFolder(f as FolderInfo)} />
        </div>

        <HorizontalGroup justify="center">
          <Button icon={moving ? 'fa fa-spinner' : undefined} variant="primary" onClick={moveTo}>
            Move
          </Button>
          <Button variant="secondary" onClick={onDismiss}>
            Cancel
          </Button>
        </HorizontalGroup>
      </>
    </Modal>
  ) : null;
};

const getStyles = stylesFactory((theme: GrafanaTheme) => {
  return {
    modal: css`
      width: 500px;
    `,
    content: css`
      margin-bottom: ${theme.spacing.lg};
    `,
  };
});

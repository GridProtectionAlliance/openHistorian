import React, { FC } from 'react';

import { Icon, ModalsController } from '@grafana/ui';

import { OnRowOptionsUpdate } from './RowOptionsForm';
import { RowOptionsModal } from './RowOptionsModal';

export interface RowOptionsButtonProps {
  title: string;
  repeat?: string | null;
  onUpdate: OnRowOptionsUpdate;
}

export const RowOptionsButton: FC<RowOptionsButtonProps> = ({ repeat, title, onUpdate }) => {
  const onUpdateChange = (hideModal: () => void) => (title: string, repeat?: string | null) => {
    onUpdate(title, repeat);
    hideModal();
  };

  return (
    <ModalsController>
      {({ showModal, hideModal }) => {
        return (
          <a
            className="pointer"
            role="button"
            aria-label="Row options"
            onClick={() => {
              showModal(RowOptionsModal, { title, repeat, onDismiss: hideModal, onUpdate: onUpdateChange(hideModal) });
            }}
          >
            <Icon name="cog" />
          </a>
        );
      }}
    </ModalsController>
  );
};

RowOptionsButton.displayName = 'RowOptionsButton';

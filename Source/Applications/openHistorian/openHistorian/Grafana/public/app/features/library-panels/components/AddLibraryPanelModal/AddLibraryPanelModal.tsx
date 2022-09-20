import React, { useCallback, useEffect, useState } from 'react';
import { useAsync, useDebounce } from 'react-use';

import { isFetchError } from '@grafana/runtime';
import { Button, Field, Input, Modal } from '@grafana/ui';
import { FolderPicker } from 'app/core/components/Select/FolderPicker';

import { PanelModel } from '../../../dashboard/state';
import { getLibraryPanelByName } from '../../state/api';
import { usePanelSave } from '../../utils/usePanelSave';

interface AddLibraryPanelContentsProps {
  onDismiss: () => void;
  panel: PanelModel;
  initialFolderId?: number;
}

export const AddLibraryPanelContents = ({ panel, initialFolderId, onDismiss }: AddLibraryPanelContentsProps) => {
  const [folderId, setFolderId] = useState(initialFolderId);
  const [panelName, setPanelName] = useState(panel.title);
  const [debouncedPanelName, setDebouncedPanelName] = useState(panel.title);
  const [waiting, setWaiting] = useState(false);

  useEffect(() => setWaiting(true), [panelName]);
  useDebounce(() => setDebouncedPanelName(panelName), 350, [panelName]);

  const { saveLibraryPanel } = usePanelSave();
  const onCreate = useCallback(() => {
    panel.libraryPanel = { uid: undefined, name: panelName };
    saveLibraryPanel(panel, folderId!).then((res) => {
      if (!(res instanceof Error)) {
        onDismiss();
      }
    });
  }, [panel, panelName, folderId, onDismiss, saveLibraryPanel]);
  const isValidName = useAsync(async () => {
    try {
      return !(await getLibraryPanelByName(panelName)).some((lp) => lp.folderId === folderId);
    } catch (err) {
      if (isFetchError(err)) {
        err.isHandled = true;
      }
      return true;
    } finally {
      setWaiting(false);
    }
  }, [debouncedPanelName, folderId]);

  const invalidInput =
    !isValidName?.value && isValidName.value !== undefined && panelName === debouncedPanelName && !waiting;

  return (
    <>
      <Field
        label="Library panel name"
        invalid={invalidInput}
        error={invalidInput ? 'Library panel with this name already exists' : ''}
      >
        <Input
          id="share-panel-library-panel-name-input"
          name="name"
          value={panelName}
          onChange={(e) => setPanelName(e.currentTarget.value)}
        />
      </Field>
      <Field label="Save in folder" description="Library panel permissions are derived from the folder permissions">
        <FolderPicker
          onChange={({ id }) => setFolderId(id)}
          initialFolderId={initialFolderId}
          inputId="share-panel-library-panel-folder-picker"
        />
      </Field>

      <Modal.ButtonRow>
        <Button variant="secondary" onClick={onDismiss} fill="outline">
          Cancel
        </Button>
        <Button onClick={onCreate} disabled={invalidInput}>
          Create library panel
        </Button>
      </Modal.ButtonRow>
    </>
  );
};

interface Props extends AddLibraryPanelContentsProps {
  isOpen?: boolean;
}

export const AddLibraryPanelModal: React.FC<Props> = ({ isOpen = false, panel, initialFolderId, ...props }) => {
  return (
    <Modal title="Create library panel" isOpen={isOpen} onDismiss={props.onDismiss}>
      <AddLibraryPanelContents panel={panel} initialFolderId={initialFolderId} onDismiss={props.onDismiss} />
    </Modal>
  );
};

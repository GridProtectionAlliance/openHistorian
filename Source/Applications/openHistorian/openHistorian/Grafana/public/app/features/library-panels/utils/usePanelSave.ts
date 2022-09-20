import { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import useAsyncFn from 'react-use/lib/useAsyncFn';

import { isFetchError } from '@grafana/runtime';
import { notifyApp } from 'app/core/actions';
import { PanelModel } from 'app/features/dashboard/state';

import {
  createPanelLibraryErrorNotification,
  createPanelLibrarySuccessNotification,
  saveAndRefreshLibraryPanel,
} from '../utils';

export const usePanelSave = () => {
  const dispatch = useDispatch();
  const [state, saveLibraryPanel] = useAsyncFn(async (panel: PanelModel, folderId: number) => {
    try {
      return await saveAndRefreshLibraryPanel(panel, folderId);
    } catch (err) {
      if (isFetchError(err)) {
        err.isHandled = true;
        throw new Error(err.data.message);
      }
      throw err;
    }
  }, []);

  useEffect(() => {
    if (state.error) {
      dispatch(notifyApp(createPanelLibraryErrorNotification(`Error saving library panel: "${state.error.message}"`)));
    }
    if (state.value) {
      dispatch(notifyApp(createPanelLibrarySuccessNotification('Library panel saved')));
    }
  }, [dispatch, state]);

  return { state, saveLibraryPanel };
};

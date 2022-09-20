import { createAction, createAsyncThunk, Update } from '@reduxjs/toolkit';

import { PanelPlugin } from '@grafana/data';
import { getBackendSrv, isFetchError } from '@grafana/runtime';
import { importPanelPlugin } from 'app/features/plugins/importPanelPlugin';
import { StoreState, ThunkResult } from 'app/types';

import { invalidatePluginInCache } from '../../pluginCacheBuster';
import {
  getRemotePlugins,
  getPluginErrors,
  getLocalPlugins,
  getPluginDetails,
  installPlugin,
  uninstallPlugin,
} from '../api';
import { STATE_PREFIX } from '../constants';
import { mergeLocalsAndRemotes, updatePanels } from '../helpers';
import { CatalogPlugin, RemotePlugin } from '../types';

export const fetchAll = createAsyncThunk(`${STATE_PREFIX}/fetchAll`, async (_, thunkApi) => {
  try {
    const { dispatch } = thunkApi;
    const [localPlugins, pluginErrors, { payload: remotePlugins }] = await Promise.all([
      getLocalPlugins(),
      getPluginErrors(),
      dispatch(fetchRemotePlugins()),
    ]);

    return mergeLocalsAndRemotes(localPlugins, remotePlugins, pluginErrors);
  } catch (e) {
    return thunkApi.rejectWithValue('Unknown error.');
  }
});

export const fetchRemotePlugins = createAsyncThunk<RemotePlugin[], void, { rejectValue: RemotePlugin[] }>(
  `${STATE_PREFIX}/fetchRemotePlugins`,
  async (_, thunkApi) => {
    try {
      return await getRemotePlugins();
    } catch (error) {
      if (isFetchError(error)) {
        error.isHandled = true;
      }
      return thunkApi.rejectWithValue([]);
    }
  }
);

export const fetchDetails = createAsyncThunk(`${STATE_PREFIX}/fetchDetails`, async (id: string, thunkApi) => {
  try {
    const details = await getPluginDetails(id);

    return {
      id,
      changes: { details },
    } as Update<CatalogPlugin>;
  } catch (e) {
    return thunkApi.rejectWithValue('Unknown error.');
  }
});

// We are also using the install API endpoint to update the plugin
export const install = createAsyncThunk(
  `${STATE_PREFIX}/install`,
  async ({ id, version, isUpdating = false }: { id: string; version?: string; isUpdating?: boolean }, thunkApi) => {
    const changes = isUpdating
      ? { isInstalled: true, installedVersion: version, hasUpdate: false }
      : { isInstalled: true, installedVersion: version };
    try {
      await installPlugin(id);
      await updatePanels();

      if (isUpdating) {
        invalidatePluginInCache(id);
      }

      return { id, changes } as Update<CatalogPlugin>;
    } catch (e) {
      console.error(e);

      return thunkApi.rejectWithValue('Unknown error.');
    }
  }
);

export const uninstall = createAsyncThunk(`${STATE_PREFIX}/uninstall`, async (id: string, thunkApi) => {
  try {
    await uninstallPlugin(id);
    await updatePanels();

    invalidatePluginInCache(id);

    return {
      id,
      changes: { isInstalled: false, installedVersion: undefined },
    } as Update<CatalogPlugin>;
  } catch (e) {
    console.error(e);

    return thunkApi.rejectWithValue('Unknown error.');
  }
});

// We need this to be backwards-compatible with other parts of Grafana.
// (Originally in "public/app/features/plugins/state/actions.ts")
// TODO<remove once the "plugin_admin_enabled" feature flag is removed>
export const loadPluginDashboards = createAsyncThunk(`${STATE_PREFIX}/loadPluginDashboards`, async (_, thunkApi) => {
  const state = thunkApi.getState() as StoreState;
  const dataSourceType = state.dataSources.dataSource.type;
  const url = `api/plugins/${dataSourceType}/dashboards`;

  return getBackendSrv().get(url);
});

export const panelPluginLoaded = createAction<PanelPlugin>(`${STATE_PREFIX}/panelPluginLoaded`);

// We need this to be backwards-compatible with other parts of Grafana.
// (Originally in "public/app/features/plugins/state/actions.ts")
// It cannot be constructed with `createAsyncThunk()` as we need the return value on the call-site,
// and we cannot easily change the call-site to unwrap the result.
// TODO<remove once the "plugin_admin_enabled" feature flag is removed>
export const loadPanelPlugin = (id: string): ThunkResult<Promise<PanelPlugin>> => {
  return async (dispatch, getStore) => {
    let plugin = getStore().plugins.panels[id];

    if (!plugin) {
      plugin = await importPanelPlugin(id);

      // second check to protect against raise condition
      if (!getStore().plugins.panels[id]) {
        dispatch(panelPluginLoaded(plugin));
      }
    }

    return plugin;
  };
};

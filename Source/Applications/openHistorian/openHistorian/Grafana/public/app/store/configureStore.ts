import { configureStore as reduxConfigureStore, MiddlewareArray } from '@reduxjs/toolkit';
import { AnyAction } from 'redux';
import { ThunkMiddleware } from 'redux-thunk';

import { StoreState } from 'app/types/store';

import { buildInitialState } from '../core/reducers/navModel';
import { addReducer, createRootReducer } from '../core/reducers/root';

import { setStore } from './store';

export function addRootReducer(reducers: any) {
  // this is ok now because we add reducers before configureStore is called
  // in the future if we want to add reducers during runtime
  // we'll have to solve this in a more dynamic way
  addReducer(reducers);
}

export function configureStore(initialState?: Partial<StoreState>) {
  const store = reduxConfigureStore<StoreState, AnyAction, MiddlewareArray<[ThunkMiddleware<StoreState, AnyAction>]>>({
    reducer: createRootReducer(),
    middleware: (getDefaultMiddleware) =>
      getDefaultMiddleware({ thunk: true, serializableCheck: false, immutableCheck: false }),
    devTools: process.env.NODE_ENV !== 'production',
    preloadedState: {
      navIndex: buildInitialState(),
      ...initialState,
    },
  });

  setStore(store);
  return store;
}

/*
function getActionsToIgnoreSerializableCheckOn() {
  return [
    'dashboard/setPanelAngularComponent',
    'dashboard/panelModelAndPluginReady',
    'dashboard/dashboardInitCompleted',
    'plugins/panelPluginLoaded',
    'explore/initializeExplore',
    'explore/changeRange',
    'explore/updateDatasourceInstance',
    'explore/queryStoreSubscription',
    'explore/queryStreamUpdated',
  ];
}

function getPathsToIgnoreMutationAndSerializableCheckOn() {
  return [
    'plugins.panels',
    'dashboard.panels',
    'dashboard.getModel',
    'payload.plugin',
    'panelEditorNew.getPanel',
    'panelEditorNew.getSourcePanel',
    'panelEditorNew.getData',
    'explore.left.queryResponse',
    'explore.right.queryResponse',
    'explore.left.datasourceInstance',
    'explore.right.datasourceInstance',
    'explore.left.range',
    'explore.left.eventBridge',
    'explore.right.eventBridge',
    'explore.right.range',
    'explore.left.querySubscription',
    'explore.right.querySubscription',
  ];
}
*/

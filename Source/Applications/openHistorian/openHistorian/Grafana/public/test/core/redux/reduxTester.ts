import {
  AnyAction,
  configureStore,
  EnhancedStore,
  Reducer,
  getDefaultMiddleware,
  CombinedState,
  PreloadedState,
} from '@reduxjs/toolkit';
import { NoInfer } from '@reduxjs/toolkit/dist/tsHelpers';
import { Dispatch, Middleware, MiddlewareAPI } from 'redux';
import thunk, { ThunkMiddleware } from 'redux-thunk';

import { setStore } from '../../../app/store/store';
import { StoreState } from '../../../app/types';

export interface ReduxTesterGiven<State> {
  givenRootReducer: (rootReducer: Reducer<State>) => ReduxTesterWhen<State>;
}

export interface ReduxTesterWhen<State> {
  whenActionIsDispatched: (
    action: any,
    clearPreviousActions?: boolean
  ) => ReduxTesterWhen<State> & ReduxTesterThen<State>;
  whenAsyncActionIsDispatched: (
    action: any,
    clearPreviousActions?: boolean
  ) => Promise<ReduxTesterWhen<State> & ReduxTesterThen<State>>;
}

export interface ReduxTesterThen<State> {
  thenDispatchedActionsShouldEqual: (...dispatchedActions: AnyAction[]) => ReduxTesterWhen<State>;
  thenDispatchedActionsPredicateShouldEqual: (
    predicate: (dispatchedActions: AnyAction[]) => boolean
  ) => ReduxTesterWhen<State>;
  thenNoActionsWhereDispatched: () => ReduxTesterWhen<State>;
}

export interface ReduxTesterArguments<State> {
  preloadedState?: PreloadedState<CombinedState<NoInfer<State>>>;
  debug?: boolean;
}

export const reduxTester = <State>(args?: ReduxTesterArguments<State>): ReduxTesterGiven<State> => {
  const dispatchedActions: AnyAction[] = [];
  const logActionsMiddleWare: Middleware<{}, Partial<StoreState>> =
    (store: MiddlewareAPI<Dispatch, Partial<StoreState>>) => (next: Dispatch) => (action: AnyAction) => {
      // filter out thunk actions
      if (action && typeof action !== 'function') {
        dispatchedActions.push(action);
      }

      return next(action);
    };

  const preloadedState = args?.preloadedState ?? ({} as unknown as PreloadedState<CombinedState<NoInfer<State>>>);
  const debug = args?.debug ?? false;
  let store: EnhancedStore<State, AnyAction, []> | null = null;

  const defaultMiddleware = getDefaultMiddleware<State>({
    thunk: false,
    serializableCheck: false,
    immutableCheck: false,
  } as any);

  const givenRootReducer = (rootReducer: Reducer<State>): ReduxTesterWhen<State> => {
    store = configureStore<State, AnyAction, Array<Middleware<State>>>({
      reducer: rootReducer,
      middleware: [...defaultMiddleware, logActionsMiddleWare, thunk] as unknown as [ThunkMiddleware<State>],
      preloadedState,
    });

    setStore(store as any);

    return instance;
  };

  const whenActionIsDispatched = (
    action: any,
    clearPreviousActions?: boolean
  ): ReduxTesterWhen<State> & ReduxTesterThen<State> => {
    if (clearPreviousActions) {
      dispatchedActions.length = 0;
    }

    if (store === null) {
      throw new Error('Store was not setup properly');
    }

    store.dispatch(action);
    return instance;
  };

  const whenAsyncActionIsDispatched = async (
    action: any,
    clearPreviousActions?: boolean
  ): Promise<ReduxTesterWhen<State> & ReduxTesterThen<State>> => {
    if (clearPreviousActions) {
      dispatchedActions.length = 0;
    }

    if (store === null) {
      throw new Error('Store was not setup properly');
    }

    await store.dispatch(action);
    return instance;
  };

  const thenDispatchedActionsShouldEqual = (...actions: AnyAction[]): ReduxTesterWhen<State> => {
    if (debug) {
      console.log('Dispatched Actions', JSON.stringify(dispatchedActions, null, 2));
    }

    if (!actions.length) {
      throw new Error('thenDispatchedActionShouldEqual has to be called with at least one action');
    }

    expect(dispatchedActions).toEqual(actions);
    return instance;
  };

  const thenDispatchedActionsPredicateShouldEqual = (
    predicate: (dispatchedActions: AnyAction[]) => boolean
  ): ReduxTesterWhen<State> => {
    if (debug) {
      console.log('Dispatched Actions', JSON.stringify(dispatchedActions, null, 2));
    }

    expect(predicate(dispatchedActions)).toBe(true);
    return instance;
  };

  const thenNoActionsWhereDispatched = (): ReduxTesterWhen<State> => {
    if (debug) {
      console.log('Dispatched Actions', JSON.stringify(dispatchedActions, null, 2));
    }

    expect(dispatchedActions.length).toBe(0);
    return instance;
  };

  const instance = {
    givenRootReducer,
    whenActionIsDispatched,
    whenAsyncActionIsDispatched,
    thenDispatchedActionsShouldEqual,
    thenDispatchedActionsPredicateShouldEqual,
    thenNoActionsWhereDispatched,
  };

  return instance;
};

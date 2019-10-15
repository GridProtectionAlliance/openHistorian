import { refreshExplore, testDatasource, loadDatasource } from './actions';
import { ExploreId, ExploreUrlState, ExploreUpdateState, ExploreMode } from 'app/types';
import { thunkTester } from 'test/core/thunk/thunkTester';
import {
  initializeExploreAction,
  InitializeExplorePayload,
  updateUIStateAction,
  setQueriesAction,
  testDataSourcePendingAction,
  testDataSourceSuccessAction,
  testDataSourceFailureAction,
  loadDatasourcePendingAction,
  loadDatasourceReadyAction,
} from './actionTypes';
import { Emitter } from 'app/core/core';
import { ActionOf } from 'app/core/redux/actionCreatorFactory';
import { makeInitialUpdateState } from './reducers';
import { DataQuery } from '@grafana/ui/src/types/datasource';
import { DefaultTimeZone, RawTimeRange, LogsDedupStrategy, toUtc } from '@grafana/data';

jest.mock('app/features/plugins/datasource_srv', () => ({
  getDatasourceSrv: () => ({
    getExternal: jest.fn().mockReturnValue([]),
    get: jest.fn().mockReturnValue({
      testDatasource: jest.fn(),
      init: jest.fn(),
    }),
  }),
}));

jest.mock('../../dashboard/services/TimeSrv', () => ({
  getTimeSrv: jest.fn().mockReturnValue({
    init: jest.fn(),
  }),
}));

const t = toUtc();
const testRange = {
  from: t,
  to: t,
  raw: {
    from: t,
    to: t,
  },
};
jest.mock('app/core/utils/explore', () => ({
  ...jest.requireActual('app/core/utils/explore'),
  getTimeRangeFromUrl: (range: RawTimeRange) => testRange,
}));

const setup = (updateOverides?: Partial<ExploreUpdateState>) => {
  const exploreId = ExploreId.left;
  const containerWidth = 1920;
  const eventBridge = {} as Emitter;
  const ui = { dedupStrategy: LogsDedupStrategy.none, showingGraph: false, showingLogs: false, showingTable: false };
  const timeZone = DefaultTimeZone;
  const range = testRange;
  const urlState: ExploreUrlState = {
    datasource: 'some-datasource',
    queries: [],
    range: range.raw,
    mode: ExploreMode.Metrics,
    ui,
  };
  const updateDefaults = makeInitialUpdateState();
  const update = { ...updateDefaults, ...updateOverides };
  const initialState = {
    user: {
      orgId: '1',
      timeZone,
    },
    explore: {
      [exploreId]: {
        initialized: true,
        urlState,
        containerWidth,
        eventBridge,
        update,
        datasourceInstance: { name: 'some-datasource' },
        queries: [] as DataQuery[],
        range,
        ui,
        refreshInterval: {
          label: 'Off',
          value: 0,
        },
      },
    },
  };

  return {
    initialState,
    exploreId,
    range,
    ui,
    containerWidth,
    eventBridge,
  };
};

describe('refreshExplore', () => {
  describe('when explore is initialized', () => {
    describe('and update datasource is set', () => {
      it('then it should dispatch initializeExplore', async () => {
        const { exploreId, ui, initialState, containerWidth, eventBridge } = setup({ datasource: true });

        const dispatchedActions = await thunkTester(initialState)
          .givenThunk(refreshExplore)
          .whenThunkIsDispatched(exploreId);

        const initializeExplore = dispatchedActions[2] as ActionOf<InitializeExplorePayload>;
        const { type, payload } = initializeExplore;

        expect(type).toEqual(initializeExploreAction.type);
        expect(payload.containerWidth).toEqual(containerWidth);
        expect(payload.eventBridge).toEqual(eventBridge);
        expect(payload.queries.length).toBe(1); // Queries have generated keys hard to expect on
        expect(payload.range.from).toEqual(testRange.from);
        expect(payload.range.to).toEqual(testRange.to);
        expect(payload.range.raw.from).toEqual(testRange.raw.from);
        expect(payload.range.raw.to).toEqual(testRange.raw.to);
        expect(payload.ui).toEqual(ui);
      });
    });

    describe('and update ui is set', () => {
      it('then it should dispatch updateUIStateAction', async () => {
        const { exploreId, initialState, ui } = setup({ ui: true });

        const dispatchedActions = await thunkTester(initialState)
          .givenThunk(refreshExplore)
          .whenThunkIsDispatched(exploreId);

        expect(dispatchedActions[0].type).toEqual(updateUIStateAction.type);
        expect(dispatchedActions[0].payload).toEqual({ ...ui, exploreId });
      });
    });

    describe('and update queries is set', () => {
      it('then it should dispatch setQueriesAction', async () => {
        const { exploreId, initialState } = setup({ queries: true });

        const dispatchedActions = await thunkTester(initialState)
          .givenThunk(refreshExplore)
          .whenThunkIsDispatched(exploreId);

        expect(dispatchedActions[0].type).toEqual(setQueriesAction.type);
        expect(dispatchedActions[0].payload).toEqual({ exploreId, queries: [] });
      });
    });
  });

  describe('when update is not initialized', () => {
    it('then it should not dispatch any actions', async () => {
      const exploreId = ExploreId.left;
      const initialState = { explore: { [exploreId]: { initialized: false } } };

      const dispatchedActions = await thunkTester(initialState)
        .givenThunk(refreshExplore)
        .whenThunkIsDispatched(exploreId);

      expect(dispatchedActions).toEqual([]);
    });
  });
});

describe('test datasource', () => {
  describe('when testDatasource thunk is dispatched', () => {
    describe('and testDatasource call on instance is successful', () => {
      it('then it should dispatch testDataSourceSuccessAction', async () => {
        const exploreId = ExploreId.left;
        const mockDatasourceInstance = {
          testDatasource: () => {
            return Promise.resolve({ status: 'success' });
          },
        };

        const dispatchedActions = await thunkTester({})
          .givenThunk(testDatasource)
          .whenThunkIsDispatched(exploreId, mockDatasourceInstance);

        expect(dispatchedActions).toEqual([
          testDataSourcePendingAction({ exploreId }),
          testDataSourceSuccessAction({ exploreId }),
        ]);
      });
    });

    describe('and testDatasource call on instance is not successful', () => {
      it('then it should dispatch testDataSourceFailureAction', async () => {
        const exploreId = ExploreId.left;
        const error = 'something went wrong';
        const mockDatasourceInstance = {
          testDatasource: () => {
            return Promise.resolve({ status: 'fail', message: error });
          },
        };

        const dispatchedActions = await thunkTester({})
          .givenThunk(testDatasource)
          .whenThunkIsDispatched(exploreId, mockDatasourceInstance);

        expect(dispatchedActions).toEqual([
          testDataSourcePendingAction({ exploreId }),
          testDataSourceFailureAction({ exploreId, error }),
        ]);
      });
    });

    describe('and testDatasource call on instance throws', () => {
      it('then it should dispatch testDataSourceFailureAction', async () => {
        const exploreId = ExploreId.left;
        const error = 'something went wrong';
        const mockDatasourceInstance = {
          testDatasource: () => {
            throw { statusText: error };
          },
        };

        const dispatchedActions = await thunkTester({})
          .givenThunk(testDatasource)
          .whenThunkIsDispatched(exploreId, mockDatasourceInstance);

        expect(dispatchedActions).toEqual([
          testDataSourcePendingAction({ exploreId }),
          testDataSourceFailureAction({ exploreId, error }),
        ]);
      });
    });
  });
});

describe('loading datasource', () => {
  describe('when loadDatasource thunk is dispatched', () => {
    describe('and all goes fine', () => {
      it('then it should dispatch correct actions', async () => {
        const exploreId = ExploreId.left;
        const name = 'some-datasource';
        const initialState = { explore: { [exploreId]: { requestedDatasourceName: name } } };
        const mockDatasourceInstance = {
          testDatasource: () => {
            return Promise.resolve({ status: 'success' });
          },
          name,
          init: jest.fn(),
          meta: { id: 'some id' },
        };

        const dispatchedActions = await thunkTester(initialState)
          .givenThunk(loadDatasource)
          .whenThunkIsDispatched(exploreId, mockDatasourceInstance);

        expect(dispatchedActions).toEqual([
          loadDatasourcePendingAction({
            exploreId,
            requestedDatasourceName: mockDatasourceInstance.name,
          }),
          testDataSourcePendingAction({ exploreId }),
          testDataSourceSuccessAction({ exploreId }),
          loadDatasourceReadyAction({ exploreId, history: [] }),
        ]);
      });
    });

    describe('and user changes datasource during load', () => {
      it('then it should dispatch correct actions', async () => {
        const exploreId = ExploreId.left;
        const name = 'some-datasource';
        const initialState = { explore: { [exploreId]: { requestedDatasourceName: 'some-other-datasource' } } };
        const mockDatasourceInstance = {
          testDatasource: () => {
            return Promise.resolve({ status: 'success' });
          },
          name,
          init: jest.fn(),
          meta: { id: 'some id' },
        };

        const dispatchedActions = await thunkTester(initialState)
          .givenThunk(loadDatasource)
          .whenThunkIsDispatched(exploreId, mockDatasourceInstance);

        expect(dispatchedActions).toEqual([
          loadDatasourcePendingAction({
            exploreId,
            requestedDatasourceName: mockDatasourceInstance.name,
          }),
          testDataSourcePendingAction({ exploreId }),
          testDataSourceSuccessAction({ exploreId }),
        ]);
      });
    });
  });
});

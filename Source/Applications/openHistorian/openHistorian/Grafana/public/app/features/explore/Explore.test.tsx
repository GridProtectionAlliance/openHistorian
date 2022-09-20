import { render, screen } from '@testing-library/react';
import React from 'react';
import { Provider } from 'react-redux';
import { AutoSizerProps } from 'react-virtualized-auto-sizer';

import { DataSourceApi, LoadingState, CoreApp, createTheme } from '@grafana/data';
import { configureStore } from 'app/store/configureStore';
import { ExploreId } from 'app/types/explore';

import { Explore, Props } from './Explore';
import { scanStopAction } from './state/query';
import { createEmptyQueryResponse } from './state/utils';

const makeEmptyQueryResponse = (loadingState: LoadingState) => {
  const baseEmptyResponse = createEmptyQueryResponse();

  baseEmptyResponse.request = {
    requestId: '1',
    intervalMs: 0,
    interval: '1s',
    dashboardId: 0,
    panelId: 1,
    range: baseEmptyResponse.timeRange,
    scopedVars: {
      apps: {
        value: 'value',
        text: 'text',
      },
    },
    targets: [
      {
        refId: 'A',
      },
    ],
    timezone: 'UTC',
    app: CoreApp.Explore,
    startTime: 0,
  };

  baseEmptyResponse.state = loadingState;

  return baseEmptyResponse;
};

const dummyProps: Props = {
  logsResult: undefined,
  changeSize: jest.fn(),
  datasourceInstance: {
    meta: {
      metrics: true,
      logs: true,
    },
    components: {
      QueryEditorHelp: {},
    },
  } as DataSourceApi,
  datasourceMissing: false,
  exploreId: ExploreId.left,
  loading: false,
  modifyQueries: jest.fn(),
  scanStart: jest.fn(),
  scanStopAction: scanStopAction,
  setQueries: jest.fn(),
  queryKeys: [],
  isLive: false,
  syncedTimes: false,
  updateTimeRange: jest.fn(),
  makeAbsoluteTime: jest.fn(),
  graphResult: [],
  absoluteRange: {
    from: 0,
    to: 0,
  },
  timeZone: 'UTC',
  queryResponse: makeEmptyQueryResponse(LoadingState.NotStarted),
  addQueryRow: jest.fn(),
  theme: createTheme(),
  showMetrics: true,
  showLogs: true,
  showTable: true,
  showTrace: true,
  showNodeGraph: true,
  splitOpen: (() => {}) as any,
  changeGraphStyle: () => {},
  graphStyle: 'lines',
};

jest.mock('@grafana/runtime/src/services/dataSourceSrv', () => {
  return {
    getDataSourceSrv: () => ({
      get: () => Promise.resolve({}),
      getList: () => [],
      getInstanceSettings: () => {},
    }),
  };
});

jest.mock('app/core/core', () => ({
  contextSrv: {
    hasAccess: () => true,
  },
}));

// for the AutoSizer component to have a width
jest.mock('react-virtualized-auto-sizer', () => {
  return ({ children }: AutoSizerProps) => children({ height: 1, width: 1 });
});

const setup = (overrideProps?: Partial<Props>) => {
  const store = configureStore();
  const exploreProps = { ...dummyProps, ...overrideProps };

  return render(
    <Provider store={store}>
      <Explore {...exploreProps} />
    </Provider>
  );
};

describe('Explore', () => {
  it('should not render no data with not started loading state', () => {
    setup();
    expect(screen.queryByTestId('explore-no-data')).not.toBeInTheDocument();
  });

  it('should render no data with done loading state', async () => {
    const queryResp = makeEmptyQueryResponse(LoadingState.Done);
    setup({ queryResponse: queryResp });
    expect(screen.getByTestId('explore-no-data')).toBeInTheDocument();
  });
});

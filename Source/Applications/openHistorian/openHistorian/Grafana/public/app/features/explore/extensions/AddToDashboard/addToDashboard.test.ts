import { MutableDataFrame } from '@grafana/data';
import { DataQuery, defaultDashboard } from '@grafana/schema';
import * as api from 'app/features/dashboard/state/initDashboard';
import { ExplorePanelData } from 'app/types';

import { createEmptyQueryResponse } from '../../state/utils';

import { setDashboardInLocalStorage } from './addToDashboard';

let mockDashboard = {} as unknown;
jest.mock('app/features/dashboard/api/dashboard_api', () => ({
  getDashboardAPI: () => ({
    getDashboardDTO: () => {
      return Promise.resolve(mockDashboard);
    },
  }),
}));

describe('addPanelToDashboard', () => {
  let spy: jest.SpyInstance;
  beforeAll(() => {
    spy = jest.spyOn(api, 'setDashboardToFetchFromLocalStorage');
  });

  afterEach(() => {
    jest.resetAllMocks();
  });

  it('Correct datasource ref is used', async () => {
    await setDashboardInLocalStorage({
      queries: [],
      queryResponse: createEmptyQueryResponse(),
      datasource: { type: 'loki', uid: 'someUid' },
      time: { from: 'now-1h', to: 'now' },
    });
    expect(spy).toHaveBeenCalledWith(
      expect.objectContaining({
        dashboard: expect.objectContaining({
          panels: expect.arrayContaining([expect.objectContaining({ datasource: { type: 'loki', uid: 'someUid' } })]),
        }),
      })
    );
  });

  it('Correct time range is used', async () => {
    await setDashboardInLocalStorage({
      queries: [],
      queryResponse: createEmptyQueryResponse(),
      datasource: { type: 'loki', uid: 'someUid' },
      time: { from: 'now-10h', to: 'now' },
    });

    expect(spy).toHaveBeenCalledWith(
      expect.objectContaining({
        dashboard: expect.objectContaining({
          time: expect.objectContaining({ from: 'now-10h', to: 'now' }),
        }),
      })
    );
  });

  it('All queries are correctly passed through', async () => {
    const queries: DataQuery[] = [{ refId: 'A' }, { refId: 'B', hide: true }];

    await setDashboardInLocalStorage({
      queries,
      queryResponse: createEmptyQueryResponse(),
      time: { from: 'now-1h', to: 'now' },
    });
    expect(spy).toHaveBeenCalledWith(
      expect.objectContaining({
        dashboard: expect.objectContaining({
          panels: expect.arrayContaining([expect.objectContaining({ targets: expect.arrayContaining(queries) })]),
        }),
      })
    );
  });

  it('Previous panels should not be removed', async () => {
    const queries: DataQuery[] = [{ refId: 'A' }];
    const existingPanel = { prop: 'this should be kept' };

    // Set the mocked dashboard
    mockDashboard = {
      dashboard: {
        ...defaultDashboard,
        templating: { list: [] },
        title: 'Previous panels should not be removed',
        uid: 'someUid',
        panels: [existingPanel],
      },
      meta: {},
    };

    await setDashboardInLocalStorage({
      queries,
      queryResponse: createEmptyQueryResponse(),
      dashboardUid: 'someUid',
      datasource: { type: '' },
      time: { from: 'now-1h', to: 'now' },
    });

    expect(spy).toHaveBeenCalledWith(
      expect.objectContaining({
        dashboard: expect.objectContaining({
          panels: expect.arrayContaining([
            expect.objectContaining({ targets: expect.arrayContaining(queries) }),
            existingPanel,
          ]),
        }),
      })
    );
  });

  describe('Setting visualization type', () => {
    describe('Defaults to table', () => {
      const cases: Array<[string, DataQuery[], ExplorePanelData]> = [
        ['If response is empty', [{ refId: 'A' }], createEmptyQueryResponse()],
        ['If no query is active', [{ refId: 'A', hide: true }], createEmptyQueryResponse()],
        [
          'If no query is active, even when there is a response from a previous execution',
          [{ refId: 'A', hide: true }],
          { ...createEmptyQueryResponse(), logsFrames: [new MutableDataFrame({ refId: 'A', fields: [] })] },
        ],
      ];

      it.each(cases)('%s', async (_, queries, queryResponse) => {
        await setDashboardInLocalStorage({ queries, queryResponse, time: { from: 'now-1h', to: 'now' } });
        expect(spy).toHaveBeenCalledWith(
          expect.objectContaining({
            dashboard: expect.objectContaining({
              panels: expect.arrayContaining([expect.objectContaining({ type: 'table' })]),
            }),
          })
        );
      });
    });

    describe('Correctly set visualization based on response', () => {
      type TestArgs = {
        framesType: string;
        expectedPanel: string;
      };
      it.each`
        framesType            | expectedPanel
        ${'logsFrames'}       | ${'logs'}
        ${'graphFrames'}      | ${'timeseries'}
        ${'nodeGraphFrames'}  | ${'nodeGraph'}
        ${'flameGraphFrames'} | ${'flamegraph'}
        ${'traceFrames'}      | ${'traces'}
      `(
        'Sets visualization to $expectedPanel if there are $frameType frames',
        async ({ framesType, expectedPanel }: TestArgs) => {
          const queries = [{ refId: 'A' }];
          const queryResponse: ExplorePanelData = {
            ...createEmptyQueryResponse(),
            [framesType]: [new MutableDataFrame({ refId: 'A', fields: [] })],
          };

          await setDashboardInLocalStorage({ queries, queryResponse, time: { from: 'now-1h', to: 'now' } });
          expect(spy).toHaveBeenCalledWith(
            expect.objectContaining({
              dashboard: expect.objectContaining({
                panels: expect.arrayContaining([expect.objectContaining({ type: expectedPanel })]),
              }),
            })
          );
        }
      );

      it('Sets visualization to plugin panel ID if there are custom panel frames', async () => {
        const queries = [{ refId: 'A' }];
        const queryResponse: ExplorePanelData = {
          ...createEmptyQueryResponse(),
          ['customFrames']: [
            new MutableDataFrame({
              refId: 'A',
              fields: [],
              meta: { preferredVisualisationPluginId: 'someCustomPluginId' },
            }),
          ],
        };

        await setDashboardInLocalStorage({ queries, queryResponse, time: { from: 'now-1h', to: 'now' } });
        expect(spy).toHaveBeenCalledWith(
          expect.objectContaining({
            dashboard: expect.objectContaining({
              panels: expect.arrayContaining([expect.objectContaining({ type: 'someCustomPluginId' })]),
            }),
          })
        );
      });
    });
  });
});

import { alertRulesReducer, initialState, loadAlertRules, loadedAlertRules, setSearchQuery } from './reducers';
import { AlertRuleDTO, AlertRulesState } from 'app/types';
import { reducerTester } from '../../../../test/core/redux/reducerTester';

describe('Alert rules', () => {
  const payload: AlertRuleDTO[] = [
    {
      id: 2,
      dashboardId: 7,
      dashboardUid: 'ggHbN42mk',
      dashboardSlug: 'alerting-with-testdata',
      panelId: 4,
      name: 'TestData - Always Alerting',
      state: 'alerting',
      newStateDate: '2018-09-04T10:00:30+02:00',
      evalDate: '0001-01-01T00:00:00Z',
      evalData: { evalMatches: [{ metric: 'A-series', tags: null, value: 215 }] },
      executionError: '',
      url: '/d/ggHbN42mk/alerting-with-testdata',
    },
    {
      id: 1,
      dashboardId: 7,
      dashboardUid: 'ggHbN42mk',
      dashboardSlug: 'alerting-with-testdata',
      panelId: 3,
      name: 'TestData - Always OK',
      state: 'ok',
      newStateDate: '2018-09-04T10:01:01+02:00',
      evalDate: '0001-01-01T00:00:00Z',
      evalData: {},
      executionError: '',
      url: '/d/ggHbN42mk/alerting-with-testdata',
    },
    {
      id: 3,
      dashboardId: 7,
      dashboardUid: 'ggHbN42mk',
      dashboardSlug: 'alerting-with-testdata',
      panelId: 3,
      name: 'TestData - ok',
      state: 'ok',
      newStateDate: '2018-09-04T10:01:01+02:00',
      evalDate: '0001-01-01T00:00:00Z',
      evalData: {},
      executionError: 'error',
      url: '/d/ggHbN42mk/alerting-with-testdata',
    },
    {
      id: 4,
      dashboardId: 7,
      dashboardUid: 'ggHbN42mk',
      dashboardSlug: 'alerting-with-testdata',
      panelId: 3,
      name: 'TestData - Paused',
      state: 'paused',
      newStateDate: '2018-09-04T10:01:01+02:00',
      evalDate: '0001-01-01T00:00:00Z',
      evalData: {},
      executionError: 'error',
      url: '/d/ggHbN42mk/alerting-with-testdata',
    },
    {
      id: 5,
      dashboardId: 7,
      dashboardUid: 'ggHbN42mk',
      dashboardSlug: 'alerting-with-testdata',
      panelId: 3,
      name: 'TestData - Ok',
      state: 'ok',
      newStateDate: '2018-09-04T10:01:01+02:00',
      evalDate: '0001-01-01T00:00:00Z',
      evalData: {
        noData: true,
      },
      executionError: 'error',
      url: '/d/ggHbN42mk/alerting-with-testdata',
    },
  ];

  describe('when loadAlertRules is dispatched', () => {
    it('then state should be correct', () => {
      reducerTester<AlertRulesState>()
        .givenReducer(alertRulesReducer, { ...initialState })
        .whenActionIsDispatched(loadAlertRules())
        .thenStateShouldEqual({ ...initialState, isLoading: true });
    });
  });

  describe('when setSearchQuery is dispatched', () => {
    it('then state should be correct', () => {
      reducerTester<AlertRulesState>()
        .givenReducer(alertRulesReducer, { ...initialState })
        .whenActionIsDispatched(setSearchQuery('query'))
        .thenStateShouldEqual({ ...initialState, searchQuery: 'query' });
    });
  });

  describe('when loadedAlertRules is dispatched', () => {
    it('then state should be correct', () => {
      reducerTester<AlertRulesState>()
        .givenReducer(alertRulesReducer, { ...initialState, isLoading: true })
        .whenActionIsDispatched(loadedAlertRules(payload))
        .thenStateShouldEqual({
          ...initialState,
          isLoading: false,
          items: [
            {
              dashboardId: 7,
              dashboardSlug: 'alerting-with-testdata',
              dashboardUid: 'ggHbN42mk',
              evalData: {
                evalMatches: [
                  {
                    metric: 'A-series',
                    tags: null,
                    value: 215,
                  },
                ],
              },
              evalDate: '0001-01-01T00:00:00Z',
              executionError: '',
              id: 2,
              name: 'TestData - Always Alerting',
              newStateDate: '2018-09-04T10:00:30+02:00',
              panelId: 4,
              state: 'alerting',
              stateAge: 'a year',
              stateClass: 'alert-state-critical',
              stateIcon: 'icon-gf icon-gf-critical',
              stateText: 'ALERTING',
              url: '/d/ggHbN42mk/alerting-with-testdata',
            },
            {
              dashboardId: 7,
              dashboardSlug: 'alerting-with-testdata',
              dashboardUid: 'ggHbN42mk',
              evalData: {},
              evalDate: '0001-01-01T00:00:00Z',
              executionError: '',
              id: 1,
              name: 'TestData - Always OK',
              newStateDate: '2018-09-04T10:01:01+02:00',
              panelId: 3,
              state: 'ok',
              stateAge: 'a year',
              stateClass: 'alert-state-ok',
              stateIcon: 'icon-gf icon-gf-online',
              stateText: 'OK',
              url: '/d/ggHbN42mk/alerting-with-testdata',
            },
            {
              dashboardId: 7,
              dashboardSlug: 'alerting-with-testdata',
              dashboardUid: 'ggHbN42mk',
              evalData: {},
              evalDate: '0001-01-01T00:00:00Z',
              executionError: 'error',
              id: 3,
              info: 'Execution Error: error',
              name: 'TestData - ok',
              newStateDate: '2018-09-04T10:01:01+02:00',
              panelId: 3,
              state: 'ok',
              stateAge: 'a year',
              stateClass: 'alert-state-ok',
              stateIcon: 'icon-gf icon-gf-online',
              stateText: 'OK',
              url: '/d/ggHbN42mk/alerting-with-testdata',
            },
            {
              dashboardId: 7,
              dashboardSlug: 'alerting-with-testdata',
              dashboardUid: 'ggHbN42mk',
              evalData: {},
              evalDate: '0001-01-01T00:00:00Z',
              executionError: 'error',
              id: 4,
              name: 'TestData - Paused',
              newStateDate: '2018-09-04T10:01:01+02:00',
              panelId: 3,
              state: 'paused',
              stateAge: 'a year',
              stateClass: 'alert-state-paused',
              stateIcon: 'fa fa-pause',
              stateText: 'PAUSED',
              url: '/d/ggHbN42mk/alerting-with-testdata',
            },
            {
              dashboardId: 7,
              dashboardSlug: 'alerting-with-testdata',
              dashboardUid: 'ggHbN42mk',
              evalData: {
                noData: true,
              },
              evalDate: '0001-01-01T00:00:00Z',
              executionError: 'error',
              id: 5,
              info: 'Query returned no data',
              name: 'TestData - Ok',
              newStateDate: '2018-09-04T10:01:01+02:00',
              panelId: 3,
              state: 'ok',
              stateAge: 'a year',
              stateClass: 'alert-state-ok',
              stateIcon: 'icon-gf icon-gf-online',
              stateText: 'OK',
              url: '/d/ggHbN42mk/alerting-with-testdata',
            },
          ],
        });
    });
  });
});

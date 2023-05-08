import { render, screen } from '@testing-library/react';
import React from 'react';
import { Provider } from 'react-redux';
import { match, Router } from 'react-router-dom';
import { useEffectOnce } from 'react-use';
import { AutoSizerProps } from 'react-virtualized-auto-sizer';
import { mockToolkitActionCreator } from 'test/core/redux/mocks';
import { getGrafanaContextMock } from 'test/mocks/getGrafanaContextMock';

import { createTheme } from '@grafana/data';
import { selectors } from '@grafana/e2e-selectors';
import { config, locationService, setDataSourceSrv } from '@grafana/runtime';
import { Dashboard } from '@grafana/schema';
import { notifyApp } from 'app/core/actions';
import { GrafanaContext } from 'app/core/context/GrafanaContext';
import { getRouteComponentProps } from 'app/core/navigation/__mocks__/routeProps';
import { RouteDescriptor } from 'app/core/navigation/types';
import { HOME_NAV_ID } from 'app/core/reducers/navModel';
import { DashboardInitPhase, DashboardMeta, DashboardRoutes } from 'app/types';

import { configureStore } from '../../../store/configureStore';
import { Props as LazyLoaderProps } from '../dashgrid/LazyLoader';
import { DashboardSrv, setDashboardSrv } from '../services/DashboardSrv';
import { DashboardModel } from '../state';
import { createDashboardModelFixture } from '../state/__fixtures__/dashboardFixtures';

import { Props, UnthemedDashboardPage } from './DashboardPage';

jest.mock('app/features/dashboard/dashgrid/LazyLoader', () => {
  const LazyLoader = ({ children, onLoad }: Pick<LazyLoaderProps, 'children' | 'onLoad'>) => {
    useEffectOnce(() => {
      onLoad?.();
    });
    return <>{typeof children === 'function' ? children({ isInView: true }) : children}</>;
  };
  return { LazyLoader };
});

jest.mock('app/features/dashboard/components/DashboardSettings/GeneralSettings', () => {
  class GeneralSettings extends React.Component<{}, {}> {
    render() {
      return <>general settings</>;
    }
  }

  return { GeneralSettings };
});

jest.mock('app/features/query/components/QueryGroup', () => {
  return {
    QueryGroup: () => null,
  };
});

jest.mock('app/core/core', () => ({
  appEvents: {
    subscribe: () => {
      return { unsubscribe: () => {} };
    },
  },
}));

jest.mock('react-virtualized-auto-sizer', () => {
  // The size of the children need to be small enough to be outside the view.
  // So it does not trigger the query to be run by the PanelQueryRunner.
  return ({ children }: AutoSizerProps) => children({ height: 1, width: 1 });
});

interface ScenarioContext {
  dashboard?: DashboardModel | null;
  container?: HTMLElement;
  mount: (propOverrides?: Partial<Props>) => void;
  unmount: () => void;
  props: Props;
  rerender: (propOverrides?: Partial<Props>) => void;
  setup: (fn: () => void) => void;
}

function getTestDashboard(overrides?: Partial<Dashboard>, metaOverrides?: Partial<DashboardMeta>): DashboardModel {
  const data = Object.assign(
    {
      title: 'My dashboard',
      panels: [
        {
          id: 1,
          type: 'timeseries',
          title: 'My panel title',
          gridPos: { x: 0, y: 0, w: 1, h: 1 },
        },
      ],
    },
    overrides
  );

  return createDashboardModelFixture(data, metaOverrides);
}

function dashboardPageScenario(description: string, scenarioFn: (ctx: ScenarioContext) => void) {
  describe(description, () => {
    let setupFn: () => void;

    const ctx: ScenarioContext = {
      setup: (fn) => {
        setupFn = fn;
      },
      mount: (propOverrides?: Partial<Props>) => {
        config.bootData.navTree = [
          { text: 'Dashboards', id: 'dashboards' },
          { text: 'Home', id: HOME_NAV_ID },
        ];

        const store = configureStore();
        const props: Props = {
          ...getRouteComponentProps({
            match: { params: { slug: 'my-dash', uid: '11' } } as unknown as match,
            route: { routeName: DashboardRoutes.Normal } as RouteDescriptor,
          }),
          navIndex: {
            dashboards: { text: 'Dashboards', id: 'dashboards', parentItem: { text: 'Home', id: HOME_NAV_ID } },
            [HOME_NAV_ID]: { text: 'Home', id: HOME_NAV_ID },
          },
          initPhase: DashboardInitPhase.NotStarted,
          initError: null,
          initDashboard: jest.fn(),
          notifyApp: mockToolkitActionCreator(notifyApp),
          cleanUpDashboardAndVariables: jest.fn(),
          cancelVariables: jest.fn(),
          templateVarsChangedInUrl: jest.fn(),
          dashboard: null,
          theme: createTheme(),
        };

        Object.assign(props, propOverrides);

        ctx.props = props;
        ctx.dashboard = props.dashboard;

        const context = getGrafanaContextMock();

        const { container, rerender, unmount } = render(
          <GrafanaContext.Provider value={context}>
            <Provider store={store}>
              <Router history={locationService.getHistory()}>
                <UnthemedDashboardPage {...props} />
              </Router>
            </Provider>
          </GrafanaContext.Provider>
        );

        ctx.container = container;

        ctx.rerender = (newProps?: Partial<Props>) => {
          Object.assign(props, newProps);

          rerender(
            <GrafanaContext.Provider value={context}>
              <Provider store={store}>
                <Router history={locationService.getHistory()}>
                  <UnthemedDashboardPage {...props} />
                </Router>
              </Provider>
            </GrafanaContext.Provider>
          );
        };

        ctx.unmount = unmount;
      },
      props: {} as Props,
      rerender: () => {},
      unmount: () => {},
    };

    beforeEach(() => {
      setupFn();
    });

    scenarioFn(ctx);
  });
}

describe('DashboardPage', () => {
  dashboardPageScenario('Given initial state', (ctx) => {
    ctx.setup(() => {
      ctx.mount();
    });

    it('Should call initDashboard on mount', () => {
      expect(ctx.props.initDashboard).toBeCalledWith({
        fixUrl: true,
        routeName: 'normal-dashboard',
        urlSlug: 'my-dash',
        urlUid: '11',
        keybindingSrv: expect.anything(),
      });
    });
  });

  dashboardPageScenario('Given a simple dashboard', (ctx) => {
    ctx.setup(() => {
      ctx.mount();
      ctx.rerender({ dashboard: getTestDashboard() });
    });

    it('Should render panels', () => {
      expect(screen.getByText('My panel title')).toBeInTheDocument();
    });

    it('Should update title', () => {
      expect(document.title).toBe('My dashboard - Dashboards - Grafana');
    });
  });

  dashboardPageScenario('When going into view mode', (ctx) => {
    ctx.setup(() => {
      setDataSourceSrv({
        get: jest.fn().mockResolvedValue({ getRef: jest.fn(), query: jest.fn().mockResolvedValue([]) }),
        getInstanceSettings: jest.fn().mockReturnValue({ meta: {} }),
        getList: jest.fn(),
        reload: jest.fn(),
      });
      setDashboardSrv({
        getCurrent: () => getTestDashboard(),
      } as DashboardSrv);
      ctx.mount({
        dashboard: getTestDashboard(),
        queryParams: { viewPanel: '1' },
      });
    });

    it('Should render panel in view mode', () => {
      expect(ctx.dashboard?.panelInView).toBeDefined();
      expect(ctx.dashboard?.panels[0].isViewing).toBe(true);
    });

    it('Should reset state when leaving', () => {
      ctx.rerender({ queryParams: {} });

      expect(ctx.dashboard?.panelInView).toBeUndefined();
      expect(ctx.dashboard?.panels[0].isViewing).toBe(false);
    });
  });

  dashboardPageScenario('When going into edit mode', (ctx) => {
    ctx.setup(() => {
      ctx.mount({
        dashboard: getTestDashboard(),
        queryParams: { editPanel: '1' },
      });
    });

    it('Should render panel in edit mode', () => {
      expect(ctx.dashboard?.panelInEdit).toBeDefined();
    });

    it('Should render panel editor', () => {
      expect(screen.getByTitle('Apply changes and go back to dashboard')).toBeInTheDocument();
    });

    it('Should reset state when leaving', () => {
      ctx.rerender({ queryParams: {} });
      expect(screen.queryByTitle('Apply changes and go back to dashboard')).not.toBeInTheDocument();
    });
  });

  dashboardPageScenario('When dashboard unmounts', (ctx) => {
    ctx.setup(() => {
      ctx.mount();
      ctx.rerender({ dashboard: getTestDashboard() });
      ctx.unmount();
    });

    it('Should call close action', () => {
      expect(ctx.props.cleanUpDashboardAndVariables).toHaveBeenCalledTimes(1);
    });
  });

  dashboardPageScenario('When dashboard changes', (ctx) => {
    ctx.setup(() => {
      ctx.mount();
      ctx.rerender({ dashboard: getTestDashboard() });
      ctx.rerender({
        match: {
          params: { uid: 'new-uid' },
        } as unknown as match,
        dashboard: getTestDashboard({ title: 'Another dashboard' }),
      });
    });

    it('Should call clean up action and init', () => {
      expect(ctx.props.cleanUpDashboardAndVariables).toHaveBeenCalledTimes(1);
      expect(ctx.props.initDashboard).toHaveBeenCalledTimes(2);
    });
  });

  dashboardPageScenario('No kiosk mode tv', (ctx) => {
    ctx.setup(() => {
      ctx.mount({ dashboard: getTestDashboard() });
      ctx.rerender({ dashboard: ctx.dashboard });
    });

    it('should render dashboard page toolbar and submenu', () => {
      expect(screen.queryAllByTestId(selectors.pages.Dashboard.DashNav.navV2)).toHaveLength(1);
      expect(screen.queryAllByLabelText(selectors.pages.Dashboard.SubMenu.submenu)).toHaveLength(1);
    });
  });

  dashboardPageScenario('When in full kiosk mode', (ctx) => {
    ctx.setup(() => {
      ctx.mount({
        queryParams: { kiosk: true },
        dashboard: getTestDashboard(),
      });
      ctx.rerender({ dashboard: ctx.dashboard });
    });

    it('should not render page toolbar and submenu', () => {
      expect(screen.queryAllByTestId(selectors.pages.Dashboard.DashNav.navV2)).toHaveLength(0);
      expect(screen.queryAllByLabelText(selectors.pages.Dashboard.SubMenu.submenu)).toHaveLength(0);
    });
  });
});

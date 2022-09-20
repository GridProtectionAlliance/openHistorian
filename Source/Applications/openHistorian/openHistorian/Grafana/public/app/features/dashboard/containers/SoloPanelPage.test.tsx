import { render, screen } from '@testing-library/react';
import React from 'react';

import { DashboardRoutes } from 'app/types';

import { getRouteComponentProps } from '../../../core/navigation/__mocks__/routeProps';
import { Props as DashboardPanelProps } from '../dashgrid/DashboardPanel';
import { DashboardModel } from '../state';

import { Props, SoloPanelPage } from './SoloPanelPage';

jest.mock('app/features/dashboard/components/DashboardSettings/GeneralSettings', () => ({}));
jest.mock('app/features/dashboard/dashgrid/DashboardPanel', () => {
  class DashboardPanel extends React.Component<DashboardPanelProps> {
    render() {
      // In this test we only check whether a new panel has arrived in the props
      return <>{this.props.panel?.title}</>;
    }
  }

  return { DashboardPanel };
});

interface ScenarioContext {
  dashboard?: DashboardModel | null;
  secondaryDashboard?: DashboardModel | null;
  setDashboard: (overrides?: any, metaOverrides?: any) => void;
  setSecondaryDashboard: (overrides?: any, metaOverrides?: any) => void;
  mount: (propOverrides?: Partial<Props>) => void;
  rerender: (propOverrides?: Partial<Props>) => void;
  setup: (fn: () => void) => void;
}

function getTestDashboard(overrides?: any, metaOverrides?: any): DashboardModel {
  const data = Object.assign(
    {
      title: 'My dashboard',
      panels: [
        {
          id: 1,
          type: 'graph',
          title: 'My graph',
          gridPos: { x: 0, y: 0, w: 1, h: 1 },
        },
      ],
    },
    overrides
  );

  const meta = Object.assign({ canSave: true, canEdit: true }, metaOverrides);
  return new DashboardModel(data, meta);
}

function soloPanelPageScenario(description: string, scenarioFn: (ctx: ScenarioContext) => void) {
  describe(description, () => {
    let setupFn: () => void;

    const ctx: ScenarioContext = {
      setup: (fn) => {
        setupFn = fn;
      },
      setDashboard: (overrides?: any, metaOverrides?: any) => {
        ctx.dashboard = getTestDashboard(overrides, metaOverrides);
      },
      setSecondaryDashboard: (overrides?: any, metaOverrides?: any) => {
        ctx.secondaryDashboard = getTestDashboard(overrides, metaOverrides);
      },
      mount: (propOverrides?: Partial<Props>) => {
        const props: Props = {
          ...getRouteComponentProps({
            match: {
              params: { slug: 'my-dash', uid: '11' },
            } as any,
            queryParams: {
              panelId: '1',
            },
            route: { routeName: DashboardRoutes.Normal } as any,
          }),
          initDashboard: jest.fn(),
          dashboard: null,
        };

        Object.assign(props, propOverrides);

        ctx.dashboard = props.dashboard;
        let { rerender } = render(<SoloPanelPage {...props} />);
        // prop updates will be submitted by rerendering the same component with different props
        ctx.rerender = (newProps?: Partial<Props>) => {
          Object.assign(props, newProps);
          rerender(<SoloPanelPage {...props} />);
        };
      },
      rerender: () => {
        // will be replaced while mount() is called
      },
    };

    beforeEach(() => {
      setupFn();
    });

    scenarioFn(ctx);
  });
}

describe('SoloPanelPage', () => {
  soloPanelPageScenario('Given initial state', (ctx) => {
    ctx.setup(() => {
      ctx.mount();
    });

    it('Should render nothing', () => {
      expect(screen.queryByText(/Loading/)).not.toBeNull();
    });
  });

  soloPanelPageScenario('Dashboard init completed ', (ctx) => {
    ctx.setup(() => {
      // Needed for AutoSizer to work in test
      Object.defineProperty(HTMLElement.prototype, 'offsetHeight', { configurable: true, value: 500 });
      Object.defineProperty(HTMLElement.prototype, 'offsetWidth', { configurable: true, value: 500 });

      ctx.mount();
      ctx.setDashboard();
      expect(ctx.dashboard).not.toBeNull();
      // the componentDidMount will change the dashboard prop to the new dashboard
      // emulate this by rerendering with new props
      ctx.rerender({ dashboard: ctx.dashboard });
    });

    it('Should render dashboard grid', async () => {
      // check if the panel title has arrived in the DashboardPanel mock
      expect(screen.queryByText(/My graph/)).not.toBeNull();
    });
  });

  soloPanelPageScenario('When user navigates to other SoloPanelPage', (ctx) => {
    ctx.setup(() => {
      ctx.mount();
      ctx.setDashboard({ uid: 1, panels: [{ id: 1, type: 'graph', title: 'Panel 1' }] });
      ctx.setSecondaryDashboard({ uid: 2, panels: [{ id: 1, type: 'graph', title: 'Panel 2' }] });
    });

    it('Should show other graph', () => {
      // check that the title in the DashboardPanel has changed
      ctx.rerender({ dashboard: ctx.dashboard });
      expect(screen.queryByText(/Panel 1/)).not.toBeNull();
      ctx.rerender({ dashboard: ctx.secondaryDashboard });
      expect(screen.queryByText(/Panel 1/)).toBeNull();
      expect(screen.queryByText(/Panel 2/)).not.toBeNull();
    });
  });
});

import { render, screen } from '@testing-library/react';
import React from 'react';
import { Provider } from 'react-redux';

import { createTheme } from '@grafana/data';
import { getRouteComponentProps } from 'app/core/navigation/__mocks__/routeProps';
import { User } from 'app/core/services/context_srv';
import { configureStore } from 'app/store/configureStore';

import { OrgRole, Team, TeamMember } from '../../types';

import { Props, TeamPages } from './TeamPages';
import { getMockTeam } from './__mocks__/teamMocks';

jest.mock('app/core/components/Select/UserPicker', () => {
  return { UserPicker: () => null };
});

jest.mock('app/core/services/context_srv', () => ({
  contextSrv: {
    accessControlEnabled: () => false,
    hasPermissionInMetadata: () => false,
    hasAccessInMetadata: () => true,
    user: {},
  },
}));

jest.mock('@grafana/runtime', () => ({
  ...jest.requireActual('@grafana/runtime'),
  getBackendSrv: () => ({
    get: jest.fn().mockResolvedValue([{ userId: 1, login: 'Test' }]),
  }),
  config: {
    licenseInfo: {
      enabledFeatures: { teamsync: true },
      stateInfo: '',
      licenseUrl: '',
    },
    featureToggles: { accesscontrol: false },
    bootData: { navTree: [], user: {} },
    buildInfo: {
      edition: 'Open Source',
    },
    appSubUrl: '',
  },
  featureEnabled: () => true,
}));

// Mock connected child components instead of rendering them
jest.mock('./TeamSettings', () => {
  //eslint-disable-next-line
  return () => <div>Team settings</div>;
});

jest.mock('./TeamGroupSync', () => {
  //eslint-disable-next-line
  return () => <div>Team group sync</div>;
});

const setup = (propOverrides?: object) => {
  const store = configureStore();
  const props: Props = {
    ...getRouteComponentProps({
      match: {
        params: {
          id: '1',
          page: null,
        },
      } as any,
    }),
    pageNav: { text: 'Cool team ' },
    teamId: 1,
    loadTeam: jest.fn(),
    loadTeamMembers: jest.fn(),
    pageName: 'members',
    team: {} as Team,
    members: [] as TeamMember[],
    editorsCanAdmin: false,
    theme: createTheme(),
    signedInUser: {
      id: 1,
      isGrafanaAdmin: false,
      orgRole: OrgRole.Viewer,
    } as User,
  };

  Object.assign(props, propOverrides);

  render(
    <Provider store={store}>
      <TeamPages {...props} />
    </Provider>
  );
};

describe('Render', () => {
  it('should render member page if team not empty', async () => {
    setup({
      team: getMockTeam(),
    });
    expect(await screen.findByRole('button', { name: 'Add member' })).toBeInTheDocument();
  });

  it('should render settings and preferences page', async () => {
    setup({
      team: getMockTeam(),
      pageName: 'settings',
      preferences: {
        homeDashboardUID: 'home-dashboard',
        theme: 'Default',
        timezone: 'Default',
      },
    });

    expect(await screen.findByText('Team settings')).toBeInTheDocument();
  });

  it('should render group sync page', async () => {
    setup({
      team: getMockTeam(),
      pageName: 'groupsync',
    });

    expect(await screen.findByText('Team group sync')).toBeInTheDocument();
  });
});

describe('when feature toggle editorsCanAdmin is turned on', () => {
  it('should render settings page if user is team admin', async () => {
    setup({
      team: getMockTeam(),
      pageName: 'settings',
      preferences: {
        homeDashboardUID: 'home-dashboard',
        theme: 'Default',
        timezone: 'Default',
      },
      editorsCanAdmin: true,
      signedInUser: {
        id: 1,
        isGrafanaAdmin: false,
        orgRole: OrgRole.Admin,
      } as User,
    });

    expect(await screen.findByText('Team settings')).toBeInTheDocument();
  });
});

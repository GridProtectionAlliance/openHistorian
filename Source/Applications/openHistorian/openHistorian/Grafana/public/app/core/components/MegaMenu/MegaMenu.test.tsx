import { render, screen } from '@testing-library/react';
import React from 'react';
import { Provider } from 'react-redux';
import { Router } from 'react-router-dom';

import { NavModelItem, NavSection } from '@grafana/data';
import { locationService } from '@grafana/runtime';
import { configureStore } from 'app/store/configureStore';

import TestProvider from '../../../../test/helpers/TestProvider';

import { MegaMenu } from './MegaMenu';

const setup = () => {
  const navBarTree: NavModelItem[] = [
    {
      text: 'Section name',
      section: NavSection.Core,
      id: 'section',
      url: 'section',
      children: [
        { text: 'Child1', id: 'child1', url: 'section/child1' },
        { text: 'Child2', id: 'child2', url: 'section/child2' },
      ],
    },
    {
      text: 'Profile',
      id: 'profile',
      section: NavSection.Config,
      url: 'profile',
    },
  ];

  const store = configureStore({ navBarTree });

  return render(
    <Provider store={store}>
      <TestProvider>
        <Router history={locationService.getHistory()}>
          <MegaMenu onClose={() => {}} />
        </Router>
      </TestProvider>
    </Provider>
  );
};

describe('MegaMenu', () => {
  it('should render component', async () => {
    setup();

    expect(await screen.findByTestId('navbarmenu')).toBeInTheDocument();
    expect(await screen.findByLabelText('Home')).toBeInTheDocument();
    expect(screen.queryAllByLabelText('Section name').length).toBe(2);
  });

  it('should filter out profile', async () => {
    setup();

    expect(screen.queryByLabelText('Profile')).not.toBeInTheDocument();
  });
});

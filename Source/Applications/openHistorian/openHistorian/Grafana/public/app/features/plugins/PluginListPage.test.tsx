import React from 'react';
import { shallow } from 'enzyme';
import { PluginListPage, Props } from './PluginListPage';
import { LayoutModes } from '../../core/components/LayoutSelector/LayoutSelector';
import { NavModel, PluginMeta } from '@grafana/data';
import { mockToolkitActionCreator } from 'test/core/redux/mocks';
import { setPluginsLayoutMode, setPluginsSearchQuery } from './state/reducers';

const setup = (propOverrides?: object) => {
  const props: Props = {
    navModel: {
      main: {
        text: 'Configuration',
      },
      node: {
        text: 'Plugins',
      },
    } as NavModel,
    plugins: [] as PluginMeta[],
    searchQuery: '',
    setPluginsSearchQuery: mockToolkitActionCreator(setPluginsSearchQuery),
    setPluginsLayoutMode: mockToolkitActionCreator(setPluginsLayoutMode),
    layoutMode: LayoutModes.Grid,
    loadPlugins: jest.fn(),
    hasFetched: false,
  };

  Object.assign(props, propOverrides);

  return shallow(<PluginListPage {...props} />);
};

describe('Render', () => {
  it('should render component', () => {
    const wrapper = setup();

    expect(wrapper).toMatchSnapshot();
  });

  it('should render list', () => {
    const wrapper = setup({
      hasFetched: true,
    });

    expect(wrapper).toMatchSnapshot();
  });
});

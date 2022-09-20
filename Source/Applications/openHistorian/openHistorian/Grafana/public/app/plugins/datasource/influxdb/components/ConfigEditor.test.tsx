import { shallow } from 'enzyme';
import React from 'react';

import ConfigEditor, { Props } from './ConfigEditor';

jest.mock('lodash', () => {
  const uniqueId = (prefix: string) => `${prefix}42`;

  const orig = jest.requireActual('lodash');

  return {
    ...orig,
    uniqueId,
  };
});

const setup = (propOverrides?: object) => {
  const props: Props = {
    options: {
      id: 21,
      uid: 'z',
      orgId: 1,
      name: 'InfluxDB-3',
      type: 'influxdb',
      typeName: 'Influx',
      typeLogoUrl: '',
      access: 'proxy',
      url: '',
      user: '',
      database: '',
      basicAuth: false,
      basicAuthUser: '',
      withCredentials: false,
      isDefault: false,
      jsonData: {
        httpMode: 'POST',
        timeInterval: '4',
      },
      secureJsonFields: {},
      version: 1,
      readOnly: false,
    },
    onOptionsChange: jest.fn(),
  };

  Object.assign(props, propOverrides);

  return shallow(<ConfigEditor {...props} />);
};

describe('Render', () => {
  it('should render component', () => {
    const wrapper = setup();

    expect(wrapper).toMatchSnapshot();
  });

  it('should disable basic auth password input', () => {
    const wrapper = setup({
      secureJsonFields: {
        basicAuthPassword: true,
      },
    });
    expect(wrapper).toMatchSnapshot();
  });

  it('should hide white listed cookies input when browser access chosen', () => {
    const wrapper = setup({
      access: 'direct',
    });
    expect(wrapper).toMatchSnapshot();
  });

  it('should hide basic auth fields when switch off', () => {
    const wrapper = setup({
      basicAuth: false,
    });
    expect(wrapper).toMatchSnapshot();
  });
});

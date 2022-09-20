import React, { useCallback } from 'react';
import { connect } from 'react-redux';
import { useHistory } from 'react-router-dom';

import { NavModel } from '@grafana/data';
import { getBackendSrv } from '@grafana/runtime';
import { Form, Button, Input, Field } from '@grafana/ui';
import { Page } from 'app/core/components/Page/Page';

import { getNavModel } from '../../core/selectors/navModel';
import { StoreState } from '../../types';

interface UserCreatePageProps {
  navModel: NavModel;
}
interface UserDTO {
  name: string;
  password: string;
  email?: string;
  login?: string;
}

const createUser = async (user: UserDTO) => getBackendSrv().post('/api/admin/users', user);

const UserCreatePage: React.FC<UserCreatePageProps> = ({ navModel }) => {
  const history = useHistory();

  const onSubmit = useCallback(
    async (data: UserDTO) => {
      await createUser(data);
      history.push('/admin/users');
    },
    [history]
  );

  return (
    <Page navModel={navModel}>
      <Page.Contents>
        <h1>Add new user</h1>
        <Form onSubmit={onSubmit} validateOn="onBlur">
          {({ register, errors }) => {
            return (
              <>
                <Field
                  label="Name"
                  required
                  invalid={!!errors.name}
                  error={errors.name ? 'Name is required' : undefined}
                >
                  <Input id="name-input" {...register('name', { required: true })} />
                </Field>

                <Field label="Email">
                  <Input id="email-input" {...register('email')} />
                </Field>

                <Field label="Username">
                  <Input id="username-input" {...register('login')} />
                </Field>
                <Field
                  label="Password"
                  required
                  invalid={!!errors.password}
                  error={errors.password ? 'Password is required and must contain at least 4 characters' : undefined}
                >
                  <Input
                    id="password-input"
                    {...register('password', {
                      validate: (value) => value.trim() !== '' && value.length >= 4,
                    })}
                    type="password"
                  />
                </Field>
                <Button type="submit">Create user</Button>
              </>
            );
          }}
        </Form>
      </Page.Contents>
    </Page>
  );
};

const mapStateToProps = (state: StoreState) => ({
  navModel: getNavModel(state.navIndex, 'global-users'),
});

export default connect(mapStateToProps)(UserCreatePage);

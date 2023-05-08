// Libraries
import { css } from '@emotion/css';
import React from 'react';

// Components
import { HorizontalGroup, LinkButton } from '@grafana/ui';
import { Branding } from 'app/core/components/Branding/Branding';
import config from 'app/core/config';

import { ChangePassword } from '../ForgottenPassword/ChangePassword';

import LoginCtrl from './LoginCtrl';
import { LoginForm } from './LoginForm';
import { LoginLayout, InnerBox } from './LoginLayout';
import { LoginServiceButtons } from './LoginServiceButtons';
import { UserSignup } from './UserSignup';

const forgottenPasswordStyles = css`
  padding: 0;
  margin-top: 4px;
`;

export const LoginPage = () => {
  document.title = Branding.AppTitle;
  return (
    <LoginLayout>
      <LoginCtrl>
        {({
          loginHint,
          passwordHint,
          disableLoginForm,
          disableUserSignUp,
          login,
          isLoggingIn,
          changePassword,
          skipPasswordChange,
          isChangingPassword,
        }) => (
          <>
            {!isChangingPassword && (
              <InnerBox>
                {!disableLoginForm && (
                  <LoginForm
                    onSubmit={login}
                    loginHint={loginHint}
                    passwordHint={passwordHint}
                    isLoggingIn={isLoggingIn}
                  >
                    <HorizontalGroup justify="flex-end">
                      <LinkButton
                        className={forgottenPasswordStyles}
                        fill="text"
                        href={`${config.appSubUrl}/user/password/send-reset-email`}
                      >
                        Forgot your password?
                      </LinkButton>
                    </HorizontalGroup>
                  </LoginForm>
                )}
                <LoginServiceButtons />
                {!disableUserSignUp && <UserSignup />}
              </InnerBox>
            )}
            {isChangingPassword && (
              <InnerBox>
                <ChangePassword onSubmit={changePassword} onSkip={() => skipPasswordChange()} />
              </InnerBox>
            )}
          </>
        )}
      </LoginCtrl>
    </LoginLayout>
  );
};

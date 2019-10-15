import React, { PureComponent } from 'react';
import { hot } from 'react-hot-loader';
import { connect } from 'react-redux';
import { NavModel } from '@grafana/data';
import { FormField } from '@grafana/ui';
import { getNavModel } from 'app/core/selectors/navModel';
import config from 'app/core/config';
import Page from 'app/core/components/Page/Page';
import { AlertBox } from 'app/core/components/AlertBox/AlertBox';
import { LdapConnectionStatus } from './LdapConnectionStatus';
import { LdapSyncInfo } from './LdapSyncInfo';
import { LdapUserInfo } from './LdapUserInfo';
import { AppNotificationSeverity, LdapError, LdapUser, StoreState, SyncInfo, LdapConnectionInfo } from 'app/types';
import {
  loadLdapState,
  loadLdapSyncStatus,
  loadUserMapping,
  clearUserError,
  clearUserMappingInfo,
} from '../state/actions';

interface Props {
  navModel: NavModel;
  ldapConnectionInfo: LdapConnectionInfo;
  ldapUser: LdapUser;
  ldapSyncInfo: SyncInfo;
  ldapError: LdapError;
  userError?: LdapError;

  loadLdapState: typeof loadLdapState;
  loadLdapSyncStatus: typeof loadLdapSyncStatus;
  loadUserMapping: typeof loadUserMapping;
  clearUserError: typeof clearUserError;
  clearUserMappingInfo: typeof clearUserMappingInfo;
}

interface State {
  isLoading: boolean;
}

export class LdapPage extends PureComponent<Props, State> {
  state = {
    isLoading: true,
  };

  async componentDidMount() {
    await this.props.clearUserMappingInfo();
    await this.fetchLDAPStatus();
    this.setState({ isLoading: false });
  }

  async fetchLDAPStatus() {
    const { loadLdapState, loadLdapSyncStatus } = this.props;
    return Promise.all([loadLdapState(), loadLdapSyncStatus()]);
  }

  async fetchUserMapping(username: string) {
    const { loadUserMapping } = this.props;
    return await loadUserMapping(username);
  }

  search = (event: any) => {
    event.preventDefault();
    const username = event.target.elements['username'].value;
    if (username) {
      this.fetchUserMapping(username);
    }
  };

  onClearUserError = () => {
    this.props.clearUserError();
  };

  render() {
    const { ldapUser, userError, ldapError, ldapSyncInfo, ldapConnectionInfo, navModel } = this.props;
    const { isLoading } = this.state;

    return (
      <Page navModel={navModel}>
        <Page.Contents isLoading={isLoading}>
          <>
            {ldapError && ldapError.title && (
              <div className="gf-form-group">
                <AlertBox title={ldapError.title} severity={AppNotificationSeverity.Error} body={ldapError.body} />
              </div>
            )}

            <LdapConnectionStatus ldapConnectionInfo={ldapConnectionInfo} />

            {config.buildInfo.isEnterprise && ldapSyncInfo && <LdapSyncInfo ldapSyncInfo={ldapSyncInfo} />}

            <h3 className="page-heading">Test user mapping</h3>
            <div className="gf-form-group">
              <form onSubmit={this.search} className="gf-form-inline">
                <FormField label="Username" labelWidth={8} inputWidth={30} type="text" id="username" name="username" />
                <button type="submit" className="btn btn-primary">
                  Run
                </button>
              </form>
            </div>
            {userError && userError.title && (
              <div className="gf-form-group">
                <AlertBox
                  title={userError.title}
                  severity={AppNotificationSeverity.Error}
                  body={userError.body}
                  onClose={this.onClearUserError}
                />
              </div>
            )}
            {ldapUser && <LdapUserInfo ldapUser={ldapUser} showAttributeMapping={true} />}
          </>
        </Page.Contents>
      </Page>
    );
  }
}

const mapStateToProps = (state: StoreState) => ({
  navModel: getNavModel(state.navIndex, 'ldap'),
  ldapConnectionInfo: state.ldap.connectionInfo,
  ldapUser: state.ldap.user,
  ldapSyncInfo: state.ldap.syncInfo,
  userError: state.ldap.userError,
  ldapError: state.ldap.ldapError,
});

const mapDispatchToProps = {
  loadLdapState,
  loadLdapSyncStatus,
  loadUserMapping,
  clearUserError,
  clearUserMappingInfo,
};

export default hot(module)(
  connect(
    mapStateToProps,
    mapDispatchToProps
  )(LdapPage)
);

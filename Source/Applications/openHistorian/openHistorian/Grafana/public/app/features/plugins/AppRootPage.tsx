// Libraries
import React, { Component } from 'react';
import { hot } from 'react-hot-loader';
import { connect } from 'react-redux';
// Types
import { StoreState } from 'app/types';
import { AppEvents, AppPlugin, AppPluginMeta, NavModel, PluginType, UrlQueryMap } from '@grafana/data';
import { createHtmlPortalNode, InPortal, OutPortal, HtmlPortalNode } from 'react-reverse-portal';

import Page from 'app/core/components/Page/Page';
import { getPluginSettings } from './PluginSettingsCache';
import { importAppPlugin } from './plugin_loader';
import { getNotFoundNav, getWarningNav, getExceptionNav } from 'app/core/nav_model_srv';
import { appEvents } from 'app/core/core';
import PageLoader from 'app/core/components/PageLoader/PageLoader';

interface Props {
  pluginId: string; // From the angular router
  query: UrlQueryMap;
  path: string;
  slug?: string;
}

interface State {
  loading: boolean;
  portalNode: HtmlPortalNode;
  plugin?: AppPlugin | null;
  nav?: NavModel;
}

export function getAppPluginPageError(meta: AppPluginMeta) {
  if (!meta) {
    return 'Unknown Plugin';
  }
  if (meta.type !== PluginType.app) {
    return 'Plugin must be an app';
  }
  if (!meta.enabled) {
    return 'Application Not Enabled';
  }
  return null;
}

class AppRootPage extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = {
      loading: true,
      portalNode: createHtmlPortalNode(),
    };
  }

  async componentDidMount() {
    const { pluginId } = this.props;

    try {
      const app = await getPluginSettings(pluginId).then(info => {
        const error = getAppPluginPageError(info);
        if (error) {
          appEvents.emit(AppEvents.alertError, [error]);
          this.setState({ nav: getWarningNav(error) });
          return null;
        }
        return importAppPlugin(info);
      });
      this.setState({ plugin: app, loading: false });
    } catch (err) {
      this.setState({
        plugin: null,
        loading: false,
        nav: process.env.NODE_ENV === 'development' ? getExceptionNav(err) : getNotFoundNav(),
      });
    }
  }

  onNavChanged = (nav: NavModel) => {
    this.setState({ nav });
  };

  render() {
    const { path, query } = this.props;
    const { loading, plugin, nav, portalNode } = this.state;

    if (plugin && !plugin.root) {
      // TODO? redirect to plugin page?
      return <div>No Root App</div>;
    }

    return (
      <>
        <InPortal node={portalNode}>
          {plugin && plugin.root && (
            <plugin.root meta={plugin.meta} query={query} path={path} onNavChanged={this.onNavChanged} />
          )}
        </InPortal>
        {nav ? (
          <Page navModel={nav}>
            <Page.Contents isLoading={loading}>
              <OutPortal node={portalNode} />
            </Page.Contents>
          </Page>
        ) : (
          <>
            <OutPortal node={portalNode} />
            {loading && <PageLoader />}
          </>
        )}
      </>
    );
  }
}

const mapStateToProps = (state: StoreState) => ({
  pluginId: state.location.routeParams.pluginId,
  slug: state.location.routeParams.slug,
  query: state.location.query,
  path: state.location.path,
});

export default hot(module)(connect(mapStateToProps)(AppRootPage));

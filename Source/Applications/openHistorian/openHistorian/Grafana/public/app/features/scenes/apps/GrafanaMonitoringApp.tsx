// Libraries
import React, { useMemo, useState } from 'react';

import {
  SceneCanvasText,
  SceneFlexLayout,
  SceneApp,
  SceneAppPage,
  SceneRouteMatch,
  EmbeddedScene,
  SceneAppPageLike,
  SceneFlexItem,
} from '@grafana/scenes';
import { usePageNav } from 'app/core/components/Page/usePageNav';
import { PluginPageContext, PluginPageContextType } from 'app/features/plugins/components/PluginPageContext';

import {
  getOverviewScene,
  getHttpHandlerListScene,
  getOverviewLogsScene,
  getHandlerDetailsScene,
  getHandlerLogsScene,
} from './scenes';

export function GrafanaMonitoringApp() {
  const appScene = useMemo(
    () =>
      new SceneApp({
        pages: [getMainPageScene()],
      }),
    []
  );

  const sectionNav = usePageNav('scenes')!;
  const [pluginContext] = useState<PluginPageContextType>({ sectionNav });

  return (
    <PluginPageContext.Provider value={pluginContext}>
      <appScene.Component model={appScene} />
    </PluginPageContext.Provider>
  );
}

export function getMainPageScene() {
  return new SceneAppPage({
    title: 'Grafana Monitoring',
    subTitle: 'A custom app with embedded scenes to monitor your Grafana server',
    url: '/scenes/grafana-monitoring',
    hideFromBreadcrumbs: false,
    getScene: getOverviewScene,
    tabs: [
      new SceneAppPage({
        title: 'Overview',
        url: '/scenes/grafana-monitoring',
        getScene: getOverviewScene,
        preserveUrlKeys: ['from', 'to', 'var-instance'],
      }),
      new SceneAppPage({
        title: 'HTTP handlers',
        url: '/scenes/grafana-monitoring/handlers',
        getScene: getHttpHandlerListScene,
        preserveUrlKeys: ['from', 'to', 'var-instance'],
        drilldowns: [
          {
            routePath: '/scenes/grafana-monitoring/handlers/:handler',
            getPage: getHandlerDrilldownPage,
          },
        ],
      }),
      new SceneAppPage({
        title: 'Logs',
        url: '/scenes/grafana-monitoring/logs',
        getScene: getOverviewLogsScene,
        preserveUrlKeys: ['from', 'to', 'var-instance'],
      }),
    ],
  });
}

export function getHandlerDrilldownPage(
  match: SceneRouteMatch<{ handler: string; tab?: string }>,
  parent: SceneAppPageLike
) {
  const handler = decodeURIComponent(match.params.handler);
  const baseUrl = `/scenes/grafana-monitoring/handlers/${encodeURIComponent(handler)}`;

  return new SceneAppPage({
    title: handler,
    subTitle: 'A grafana http handler is responsible for service a specific API request',
    url: baseUrl,
    getParentPage: () => parent,
    getScene: () => getHandlerDetailsScene(handler),
    tabs: [
      new SceneAppPage({
        title: 'Metrics',
        url: baseUrl,
        routePath: '/scenes/grafana-monitoring/handlers/:handler',
        getScene: () => getHandlerDetailsScene(handler),
        preserveUrlKeys: ['from', 'to', 'var-instance'],
      }),
      new SceneAppPage({
        title: 'Logs',
        url: baseUrl + '/logs',
        routePath: '/scenes/grafana-monitoring/handlers/:handler/logs',
        getScene: () => getHandlerLogsScene(handler),
        preserveUrlKeys: ['from', 'to', 'var-instance'],
        drilldowns: [
          {
            routePath: '/scenes/grafana-monitoring/handlers/:handler/logs/:secondLevel',
            getPage: getSecondLevelDrilldown,
          },
        ],
      }),
    ],
  });
}

export function getSecondLevelDrilldown(
  match: SceneRouteMatch<{ handler: string; secondLevel: string }>,
  parent: SceneAppPageLike
) {
  const handler = decodeURIComponent(match.params.handler);
  const secondLevel = decodeURIComponent(match.params.secondLevel);
  const baseUrl = `/scenes/grafana-monitoring/handlers/${encodeURIComponent(handler)}/logs/${secondLevel}`;

  return new SceneAppPage({
    title: secondLevel,
    subTitle: 'Second level dynamic drilldown',
    url: baseUrl,
    getParentPage: () => parent,
    getScene: () => {
      return new EmbeddedScene({
        body: new SceneFlexLayout({
          children: [
            new SceneFlexItem({
              body: new SceneCanvasText({
                text: 'Drilldown: ' + secondLevel,
              }),
            }),
          ],
        }),
      });
    },
  });
}

export default GrafanaMonitoringApp;

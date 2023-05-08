import type { PluginExtensionLinkConfig } from '@grafana/data';
import type { AppPluginConfig } from '@grafana/runtime';

import * as pluginLoader from './plugin_loader';

export type PluginPreloadResult = {
  pluginId: string;
  error?: unknown;
  extensionConfigs: PluginExtensionLinkConfig[];
};

export async function preloadPlugins(apps: Record<string, AppPluginConfig> = {}): Promise<PluginPreloadResult[]> {
  const pluginsToPreload = Object.values(apps).filter((app) => app.preload);
  return Promise.all(pluginsToPreload.map(preload));
}

async function preload(config: AppPluginConfig): Promise<PluginPreloadResult> {
  const { path, version, id: pluginId } = config;
  try {
    const { plugin } = await pluginLoader.importPluginModule(path, version);
    const { extensionConfigs = [] } = plugin;
    return { pluginId, extensionConfigs };
  } catch (error) {
    console.error(`[Plugins] Failed to preload plugin: ${path} (version: ${version})`, error);
    return { pluginId, extensionConfigs: [], error };
  }
}

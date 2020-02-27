import '@babel/polyfill';
import 'url-search-params-polyfill'; // fetch polyfill needed for PhantomJs rendering
import 'file-saver';
import 'lodash';
import 'jquery';
import 'angular';
import 'angular-route';
import 'angular-sanitize';
import 'angular-native-dragdrop';
import 'angular-bindonce';
import 'react';
import 'react-dom';

import 'vendor/bootstrap/bootstrap';
import 'vendor/angular-other/angular-strap';

import $ from 'jquery';
import angular from 'angular';
import config from 'app/core/config';
// @ts-ignore
import ttiPolyfill from 'tti-polyfill';
// @ts-ignore ignoring this for now, otherwise we would have to extend _ interface with move
import _ from 'lodash';
import { AppEvents, setMarkdownOptions, setLocale } from '@grafana/data';
import appEvents from 'app/core/app_events';
import { addClassIfNoOverlayScrollbar } from 'app/core/utils/scrollbar';
import { checkBrowserCompatibility } from 'app/core/utils/browser';
import { importPluginModule } from 'app/features/plugins/plugin_loader';

// add move to lodash for backward compatabiltiy
// @ts-ignore
_.move = (array: [], fromIndex: number, toIndex: number) => {
  array.splice(toIndex, 0, array.splice(fromIndex, 1)[0]);
  return array;
};

import { coreModule, angularModules } from 'app/core/core_module';
import { registerAngularDirectives } from 'app/core/core';
import { setupAngularRoutes } from 'app/routes/routes';
import { setEchoSrv, registerEchoBackend } from '@grafana/runtime';
import { Echo } from './core/services/echo/Echo';
import { reportPerformance } from './core/services/echo/EchoSrv';
import { PerformanceBackend } from './core/services/echo/backends/PerformanceBackend';

import 'app/routes/GrafanaCtrl';
import 'app/features/all';

// import symlinked extensions
const extensionsIndex = (require as any).context('.', true, /extensions\/index.ts/);
extensionsIndex.keys().forEach((key: any) => {
  extensionsIndex(key);
});

export class GrafanaApp {
  registerFunctions: any;
  ngModuleDependencies: any[];
  preBootModules: any[] | null;

  constructor() {
    addClassIfNoOverlayScrollbar('no-overlay-scrollbar');
    this.preBootModules = [];
    this.registerFunctions = {};
    this.ngModuleDependencies = [];
  }

  useModule(module: angular.IModule) {
    if (this.preBootModules) {
      this.preBootModules.push(module);
    } else {
      _.extend(module, this.registerFunctions);
    }
    this.ngModuleDependencies.push(module.name);
    return module;
  }

  init() {
    const app = angular.module('grafana', []);

    setLocale(config.bootData.user.locale);

    setMarkdownOptions({ sanitize: !config.disableSanitizeHtml });

    app.config(
      (
        $locationProvider: angular.ILocationProvider,
        $controllerProvider: angular.IControllerProvider,
        $compileProvider: angular.ICompileProvider,
        $filterProvider: angular.IFilterProvider,
        $httpProvider: angular.IHttpProvider,
        $provide: angular.auto.IProvideService
      ) => {
        // pre assing bindings before constructor calls
        $compileProvider.preAssignBindingsEnabled(true);

        if (config.buildInfo.env !== 'development') {
          $compileProvider.debugInfoEnabled(false);
        }

        $httpProvider.useApplyAsync(true);

        this.registerFunctions.controller = $controllerProvider.register;
        this.registerFunctions.directive = $compileProvider.directive;
        this.registerFunctions.factory = $provide.factory;
        this.registerFunctions.service = $provide.service;
        this.registerFunctions.filter = $filterProvider.register;

        $provide.decorator('$http', [
          '$delegate',
          '$templateCache',
          ($delegate: any, $templateCache: any) => {
            const get = $delegate.get;
            $delegate.get = (url: string, config: any) => {
              if (url.match(/\.html$/)) {
                // some template's already exist in the cache
                if (!$templateCache.get(url)) {
                  url += '?v=' + new Date().getTime();
                }
              }
              return get(url, config);
            };
            return $delegate;
          },
        ]);
      }
    );

    this.ngModuleDependencies = [
      'grafana.core',
      'ngRoute',
      'ngSanitize',
      '$strap.directives',
      'ang-drag-drop',
      'grafana',
      'pasvaz.bindonce',
      'react',
    ];

    // makes it possible to add dynamic stuff
    _.each(angularModules, (m: angular.IModule) => {
      this.useModule(m);
    });

    // register react angular wrappers
    coreModule.config(setupAngularRoutes);
    registerAngularDirectives();

    // disable tool tip animation
    $.fn.tooltip.defaults.animation = false;

    // bootstrap the app
    angular.bootstrap(document, this.ngModuleDependencies).invoke(() => {
      _.each(this.preBootModules, (module: angular.IModule) => {
        _.extend(module, this.registerFunctions);
      });

      this.preBootModules = null;

      if (!checkBrowserCompatibility()) {
        setTimeout(() => {
          appEvents.emit(AppEvents.alertWarning, [
            'Your browser is not fully supported',
            'A newer browser version is recommended',
          ]);
        }, 1000);
      }
    });

    // Preload selected app plugins
    for (const modulePath of config.pluginsToPreload) {
      importPluginModule(modulePath);
    }
  }

  initEchoSrv() {
    setEchoSrv(new Echo({ debug: process.env.NODE_ENV === 'development' }));

    ttiPolyfill.getFirstConsistentlyInteractive().then((tti: any) => {
      // Collecting paint metrics first
      const paintMetrics = performance && performance.getEntriesByType ? performance.getEntriesByType('paint') : [];

      for (const metric of paintMetrics) {
        reportPerformance(metric.name, Math.round(metric.startTime + metric.duration));
      }
      reportPerformance('tti', tti);
    });

    registerEchoBackend(new PerformanceBackend({}));

    window.addEventListener('DOMContentLoaded', () => {
      reportPerformance('dcl', Math.round(performance.now()));
    });
  }
}

export default new GrafanaApp();

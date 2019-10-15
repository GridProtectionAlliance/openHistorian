import angular from 'angular';

const directiveModule = angular.module('grafana.directives');
const directiveCache: any = {};

/** @ngInject */
function panelEditorTab(dynamicDirectiveSrv: any) {
  return dynamicDirectiveSrv.create({
    scope: {
      ctrl: '=',
      editorTab: '=',
    },
    directive: (scope: any) => {
      const pluginId = scope.ctrl.pluginId;
      const tabName = scope.editorTab.title
        .toLowerCase()
        .replace(' ', '-')
        .replace('&', '')
        .replace(' ', '')
        .replace(' ', '-');

      if (directiveCache[pluginId]) {
        if (directiveCache[pluginId][tabName]) {
          return directiveCache[pluginId][tabName];
        }
      } else {
        directiveCache[pluginId] = [];
      }

      const result = {
        fn: () => scope.editorTab.directiveFn(),
        name: `panel-editor-tab-${pluginId}${tabName}`,
      };

      directiveCache[pluginId][tabName] = result;

      return result;
    },
  });
}

directiveModule.directive('panelEditorTab', panelEditorTab);

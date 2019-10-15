import angular, { IQService, ILocationService } from 'angular';
import { ChangeTracker } from './ChangeTracker';
import { ContextSrv } from 'app/core/services/context_srv';
import { DashboardSrv } from './DashboardSrv';

/** @ngInject */
export function unsavedChangesSrv(
  this: any,
  $rootScope: any,
  $q: IQService,
  $location: ILocationService,
  $timeout: any,
  contextSrv: ContextSrv,
  dashboardSrv: DashboardSrv,
  $window: any
) {
  this.init = function(dashboard: any, scope: any) {
    this.tracker = new ChangeTracker(dashboard, scope, 1000, $location, $window, $timeout, contextSrv, $rootScope);
    return this.tracker;
  };
}

angular.module('grafana.services').service('unsavedChangesSrv', unsavedChangesSrv);

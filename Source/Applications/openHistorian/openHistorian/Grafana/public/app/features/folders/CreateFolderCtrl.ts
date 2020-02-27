import appEvents from 'app/core/app_events';
import locationUtil from 'app/core/utils/location_util';
import { BackendSrv } from 'app/core/services/backend_srv';
import { ILocationService } from 'angular';
import { ValidationSrv } from 'app/features/manage-dashboards';
import { NavModelSrv } from 'app/core/nav_model_srv';
import { AppEvents } from '@grafana/data';

export default class CreateFolderCtrl {
  title = '';
  navModel: any;
  titleTouched = false;
  hasValidationError: boolean;
  validationError: any;

  /** @ngInject */
  constructor(
    private backendSrv: BackendSrv,
    private $location: ILocationService,
    private validationSrv: ValidationSrv,
    navModelSrv: NavModelSrv
  ) {
    this.navModel = navModelSrv.getNav('dashboards', 'manage-dashboards', 0);
  }

  create() {
    if (this.hasValidationError) {
      return;
    }

    return this.backendSrv.createFolder({ title: this.title }).then((result: any) => {
      appEvents.emit(AppEvents.alertSuccess, ['Folder Created', 'OK']);
      this.$location.url(locationUtil.stripBaseFromUrl(result.url));
    });
  }

  titleChanged() {
    this.titleTouched = true;

    this.validationSrv
      .validateNewFolderName(this.title)
      .then(() => {
        this.hasValidationError = false;
      })
      .catch(err => {
        this.hasValidationError = true;
        this.validationError = err.message;
      });
  }
}

import {PiWebApiConfigCtrl} from './config_ctrl'
import {PiWebApiDatasource} from './datasource'
import {PiWebApiDatasourceQueryCtrl} from './query_ctrl'
import {PiWebApiAnnotationsQueryCtrl} from './annotation_ctrl'

class PiWebApiQueryOptionsCtrl { }
PiWebApiQueryOptionsCtrl.templateUrl = 'partials/query.options.html'

export {
  PiWebApiDatasource as Datasource,
  PiWebApiDatasourceQueryCtrl as QueryCtrl,
  PiWebApiConfigCtrl as ConfigCtrl,
  PiWebApiQueryOptionsCtrl as QueryOptionsCtrl,
  PiWebApiAnnotationsQueryCtrl as AnnotationsQueryCtrl
}

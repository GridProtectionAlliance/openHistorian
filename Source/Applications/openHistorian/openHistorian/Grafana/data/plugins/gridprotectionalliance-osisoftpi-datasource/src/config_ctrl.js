import _ from 'lodash'
import './css/query-editor.css!'

export class PiWebApiConfigCtrl {
  constructor ($scope) {
    this.current.jsonData = this.current.jsonData || {}

    if (!this.current.jsonData.url) {
      this.current.jsonData.url = this.current.url;
    }
    if (!this.current.jsonData.access) {
      this.current.jsonData.access = this.current.access;
    }
  }
}


PiWebApiConfigCtrl.templateUrl = 'partials/config.html'

/// <reference path="../node_modules/grafana-sdk-mocks/app/headers/common.d.ts" />
export declare class OpenHistorianDataSource {
    private backendSrv;
    private templateSrv;
    private uiSegmentSrv;
    type: any;
    url: string;
    name: string;
    q: any;
    dataFlags: any;
    options: any;
    /** @ngInject */
    constructor(instanceSettings: any, $q: any, backendSrv: any, templateSrv: any, uiSegmentSrv: any);
    query(options: any): any;
    testDatasource(): any;
    annotationQuery(options: any): any;
    metricFindQuery(options: any): any;
    whereFindQuery(options: any): any;
    mapToTextValue(result: any): any;
    buildQueryParameters(options: any): any;
    filterFindQuery(): any;
    orderByFindQuery(options: any): any;
    getMetaData(options: any): any;
    getAlarmStates(options: any): any;
    fixTemplates(target: any): any;
}

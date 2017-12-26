import { QueryCtrl } from 'app/plugins/sdk';
export declare class OpenHistorianDataSourceQueryCtrl extends QueryCtrl {
    private $scope;
    private $injector;
    private uiSegmentSrv;
    private templateSrv;
    private $compile;
    static templateUrl: string;
    queryTypes: Array<string>;
    queryType: string;
    queryOptionsOpen: boolean;
    constructor($scope: any, $injector: any, uiSegmentSrv: any, templateSrv: any, $compile: any);
    toggleQueryOptions(): void;
    onChangeInternal(): void;
    changeQueryType(): void;
}

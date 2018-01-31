export declare class OpenHistorianElementPickerCtrl {
    private $scope;
    private uiSegmentSrv;
    elementSegment: any;
    segments: Array<any>;
    functionSegment: any;
    functionSegments: Array<any>;
    functions: Array<any>;
    typingTimer: any;
    constructor($scope: any, uiSegmentSrv: any);
    getElementSegments(newSegment: any): any;
    addElementSegment(): void;
    segmentValueChanged(segment: any, index: any): void;
    setTargetWithElements(): void;
    getFunctionsToAddNew(): Promise<any>;
    getFunctionsToEdit(func: any, index: any): any;
    functionValueChanged(func: any, index: any): void;
    addFunctionSegment(): void;
    buildFunctionArray(): void;
    getBooleans(): Promise<any[]>;
    getAngleUnits(): Promise<any[]>;
    getTimeSelect(): Promise<any[]>;
    inputChange(func: any, index: any): void;
}

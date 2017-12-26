export declare const DefaultFlags: {
    'Select All': {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    Normal: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    BadData: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    SuspectData: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    OverRangeError: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    UnderRangeError: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    AlarmHigh: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    AlarmLow: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    WarningHigh: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    WarningLow: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    FlatlineAlarm: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    ComparisonAlarm: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    ROCAlarm: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    ReceivedAsBad: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    CalculatedValue: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    CalculationError: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    CalculationWarning: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    ReservedQualityFlag: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    BadTime: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    SuspectTime: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    LateTimeAlarm: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    FutureTimeAlarm: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    UpSampled: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    DownSampled: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    DiscardedValue: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    ReservedTimeFlag: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    UserDefinedFlag1: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    UserDefinedFlag2: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    UserDefinedFlag3: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    UserDefinedFlag4: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    UserDefinedFlag5: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    SystemError: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    SystemWarning: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
    MeasurementError: {
        Value: boolean;
        Order: number;
        Flag: number;
    };
};
export declare const FunctionList: {
    Set: {
        Function: string;
        Parameters: any[];
    };
    Slice: {
        Function: string;
        Parameters: {
            Default: number;
            Type: string;
            Description: string;
        }[];
    };
    Average: {
        Function: string;
        Parameters: any[];
    };
    Minimum: {
        Function: string;
        Parameters: any[];
    };
    Maximum: {
        Function: string;
        Parameters: any[];
    };
    Total: {
        Function: string;
        Parameters: any[];
    };
    Range: {
        Function: string;
        Parameters: any[];
    };
    Count: {
        Function: string;
        Parameters: any[];
    };
    Distinct: {
        Function: string;
        Parameters: any[];
    };
    AbsoluteValute: {
        Function: string;
        Parameters: any[];
    };
    Add: {
        Function: string;
        Parameters: {
            Default: number;
            Type: string;
            Description: string;
        }[];
    };
    Subtract: {
        Function: string;
        Parameters: {
            Default: number;
            Type: string;
            Description: string;
        }[];
    };
    Multiply: {
        Function: string;
        Parameters: {
            Default: number;
            Type: string;
            Description: string;
        }[];
    };
    Divide: {
        Function: string;
        Parameters: {
            Default: number;
            Type: string;
            Description: string;
        }[];
    };
    Round: {
        Function: string;
        Parameters: {
            Default: number;
            Type: string;
            Description: string;
        }[];
    };
    Floor: {
        Function: string;
        Parameters: any[];
    };
    Ceiling: {
        Function: string;
        Parameters: any[];
    };
    Truncate: {
        Function: string;
        Parameters: any[];
    };
    StandardDeviation: {
        Function: string;
        Parameters: {
            Default: boolean;
            Type: string;
            Description: string;
        }[];
    };
    Median: {
        Function: string;
        Parameters: any[];
    };
    Mode: {
        Function: string;
        Parameters: any[];
    };
    Top: {
        Function: string;
        Parameters: ({
            Default: string;
            Type: string;
            Description: string;
        } | {
            Default: boolean;
            Type: string;
            Description: string;
        })[];
    };
    Bottom: {
        Function: string;
        Parameters: ({
            Default: string;
            Type: string;
            Description: string;
        } | {
            Default: boolean;
            Type: string;
            Description: string;
        })[];
    };
    Random: {
        Function: string;
        Parameters: ({
            Default: string;
            Type: string;
            Description: string;
        } | {
            Default: boolean;
            Type: string;
            Description: string;
        })[];
    };
    First: {
        Function: string;
        Parameters: {
            Default: string;
            Type: string;
            Description: string;
        }[];
    };
    Last: {
        Function: string;
        Parameters: {
            Default: string;
            Type: string;
            Description: string;
        }[];
    };
    Percentile: {
        Function: string;
        Parameters: {
            Default: string;
            Type: string;
            Description: string;
        }[];
    };
    Difference: {
        Function: string;
        Parameters: any[];
    };
    TimeDifference: {
        Function: string;
        Parameters: {
            Default: string;
            Type: string;
            Description: string;
        }[];
    };
    Derivative: {
        Function: string;
        Parameters: {
            Default: string;
            Type: string;
            Description: string;
        }[];
    };
    TimeIntegration: {
        Function: string;
        Parameters: {
            Default: string;
            Type: string;
            Description: string;
        }[];
    };
    Interval: {
        Function: string;
        Parameters: ({
            Default: number;
            Type: string;
            Description: string;
        } | {
            Default: string;
            Type: string;
            Description: string;
        })[];
    };
    IncludeRange: {
        Function: string;
        Parameters: ({
            Default: number;
            Type: string;
            Description: string;
        } | {
            Default: boolean;
            Type: string;
            Description: string;
        })[];
    };
    ExcludeRange: {
        Function: string;
        Parameters: ({
            Default: number;
            Type: string;
            Description: string;
        } | {
            Default: boolean;
            Type: string;
            Description: string;
        })[];
    };
    FilterNaN: {
        Function: string;
        Parameters: {
            Default: boolean;
            Type: string;
            Description: string;
        }[];
    };
    UnwrapAngle: {
        Function: string;
        Parameters: {
            Default: string;
            Type: string;
            Description: string;
        }[];
    };
    WrapAngle: {
        Function: string;
        Parameters: {
            Default: string;
            Type: string;
            Description: string;
        }[];
    };
    Label: {
        Function: string;
        Parameters: {
            Default: string;
            Type: string;
            Description: string;
        }[];
    };
};
export declare const WhereOperators: string[];
export declare const Booleans: string[];
export declare const AngleUnits: string[];
export declare const TimeUnits: string[];

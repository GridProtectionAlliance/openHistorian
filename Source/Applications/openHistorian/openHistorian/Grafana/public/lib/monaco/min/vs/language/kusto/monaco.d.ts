// This file gets bundled as is with monaco-kusto.
// Everything that needs to be exposed to consumers should be typed here.
// This means that all declarations here are duplicated from the actual definitions around the code.
// TODO: think about turning this around - have all other code dependent on this file thus not needing the duplication.
// this was done like this because that's the standard way all other monaco extensions work for some reason.

declare module monaco.editor {
    export interface ICodeEditor {
        getCurrentCommandRange(cursorPosition: monaco.Position): monaco.Range;
    }
}

declare module monaco.languages.kusto {
    export interface LanguageSettings {
        includeControlCommands?: boolean;
        newlineAfterPipe?: boolean;
        syntaxErrorAsMarkDown?: SyntaxErrorAsMarkDownOptions;
        openSuggestionDialogAfterPreviousSuggestionAccepted?: boolean;
        useIntellisenseV2?: boolean;
        useSemanticColorization?: boolean;
        useTokenColorization?: boolean;
        disabledCompletionItems?: string[];
        onDidProvideCompletionItems?: monaco.languages.kusto.OnDidProvideCompletionItems;
        enableHover?: boolean;
        formatter?: FormatterOptions;
        enableQueryWarnings?: boolean;
        enableQuerySuggestions?: boolean;
        disabledDiagnoticCodes?: string[];
    }

    export interface SyntaxErrorAsMarkDownOptions {
        header?: string;
        icon?: string;
        enableSyntaxErrorAsMarkDown?: boolean;
    }

    export interface FormatterOptions {
        indentationSize?: number;
        pipeOperatorStyle?: FormatterPlacementStyle;
    }
    
    export type FormatterPlacementStyle = 'None' | 'NewLine' | 'Smart';

    export interface LanguageServiceDefaults {
        readonly onDidChange: IEvent<LanguageServiceDefaults>;
        readonly languageSettings: LanguageSettings;
        /**
         * Configure language service settings.
         */
        setLanguageSettings(options: LanguageSettings): void;

        /**
         * Configure when the worker shuts down. By default that is 2mins.
         *
         * @param value The maximum idle time in milliseconds. Values less than one
         * mean never shut down.
         */
        setMaximumWorkerIdleTime(value: number): void;
    }

    export var kustoDefaults: LanguageServiceDefaults;

    export interface KustoWorker {
        /**
         * Sets an array of ambient parameters to be known by the language service.
         * Language service assumes that these parameters will be provided externally when query gets executed and does
         * not error-out when they are being referenced in the query.
         * @param parameters the array of parameters
         */
        setParameter(parameters: ScalarParameter[]);
        setSchema(schema: Schema): Promise<void>;
        setSchemaFromShowSchema(
            schema: any,
            clusterConnectionString: string,
            databaseInContextName: string,
            globalParameters: ScalarParameter[]
        ): Promise<void>;
        normalizeSchema(
            schema: any,
            clusterConnectionString: string,
            databaseInContextName: string
        ): Promise<EngineSchema>;
        getCommandInContext(uri: string, cursorOffset: number): Promise<string | null>;
        getCommandAndLocationInContext(
            uri: string,
            offset: number
        ): Promise<{ text: string; range: monaco.Range } | null>;
        getCommandsInDocument(uri: string): Promise<{ absoluteStart: number; absoluteEnd: number; text: string }[]>;
        getClientDirective(
            text: string
        ): Promise<{ isClientDirective: boolean; directiveWithoutLeadingComments: string }>;
        getAdminCommand(text: string): Promise<{ isAdminCommand: boolean; adminCommandWithoutLeadingComments: string }>;

        /**
         * Get all declared query parameters declared in current block if any.
         */
        getQueryParams(uri: string, cursorOffset: number): Promise<{ name: string; type: string }[]>;

        /**
         * Get all the ambient parameters defined in global scope.
         * Ambient parameters are parameters that are not defined in the syntax such as in a query paramter declaration.
         * These are parameters that are injected from outside, usually by a UX application that would like to offer
         * the user intellisense for a symbol, without forcing them to write a query declaration statement.
         * Usually  the same application injects the query declaration statement and the parameter values when
         * executing the query (so it will execute correctly)
         */
        getGlobalParams(uri: string): Promise<{ name: string; type: string }[]>;
        /**
         * Get the global parameters that are actually being referenced in query.
         * This is different from getQueryParams that will return the parameters declare using a query declaration
         * statement.
         * It is also different from getGlobalParams that will return all global parameters whether used or not.
         */
        getReferencedGlobalParams(uri: string, cursorOffset: number): Promise<{ name: string; type: string }[]>;

        /**
         * Get visualization options in render command if present (null otherwise).
         */
        getRenderInfo(uri: string, cursorOffset: number): Promise<RenderInfo | null>;
        doDocumentFormat(uri: string): Promise<ls.TextEdit[]>;
        doRangeFormat(uri: string, range: ls.Range): Promise<ls.TextEdit[]>;
        doCurrentCommandFormat(uri: string, caretPosition: ls.Position): Promise<ls.TextEdit[]>;
        doValidation(uri: string, intervals: { start: number; end: number }[], includeWarnings?: boolean, includeSuggestions?: boolean): Promise<ls.Diagnostic[]>;
        setParameters(parameters: ScalarParameter[]): void;
        /**
         * Get all the database references from the current command. 
         * If database's schema is already cached in previous calls to setSchema or addDatabaseToSchema it will not be returned.
         * This method should be used to get all the cross-databases in a command, then schema for the database should be fetched and added with addDatabaseToSchema.
         * @example
         * If the current command includes: cluster('help').database('Samples') 
         * getDatabaseReferences will return [{ clusterName: 'help', databaseName 'Samples' }]
         */
        getDatabaseReferences(uri: string, cursorOffset: number): Promise<DatabaseReference[]>;
        /**
         * Get all the cluster references from the current command.
         * If cluster's schema is already cached it will not be returned.
         * This method should be used to get all the cross-clusters in a command, then schema for the cluster should be fetched and added with addClusterToSchema.
         * cluster name is returned exactly as written in the KQL `cluster(<cluster name>)` function.
         * @example
         * If the current command includes: cluster('help')
         * it returns [{ clusterName: 'help' }]
         * @example
         * If the current command includes: cluster('https://demo11.westus.kusto.windows.net')
         * getClusterReferences will return [{ clusterName: 'https://demo11.westus.kusto.windows.net' }]
         */
        getClusterReferences(uri: string, cursorOffset: number): Promise<ClusterReference[]>;
        /**
         * Adds a database's scheme. Useful with getDatabaseReferences to load schema for cross-cluster commands.
         * @param clusterName the name of the cluster as returned from getDatabaseReferences/getClusterReferences.
         * @example
         * - User enters cluster('help').database('Samples')
         * - hosting app calls getDatabaseReferences which returns [{ clusterName: 'help', databaseName: 'Samples' }]. 
         * - hosting app fetches the database Schema from https://help.kusto.windows.net
         * - hosting app calls 'addDatabaseToSchema' with the database's schema.
         * - now, when user types cluster('help').database('Samples') then the auto complete list will show all the tables.
         */
        addDatabaseToSchema(uri: string, clusterName: string, databaseSchema: Database): Promise<void>;
        /**
         * Adds a cluster's databases to the schema. Useful when used with getClusterReferences in cross-cluster commands.
         * @param clusterName the name of the cluster as returned in getClusterReferences.
         * @example
         * - User enters cluster('help')
         * - hosting app calls getClusterReferences which returns [{ clusterName: 'help' }]. 
         * - hosting app fetches the list of databases from https://help.kusto.windows.net
         * - hosting app calls addClusterToSchema with the list of databases.
         * - now, when user type `cluster('help').database(` then the auto complete list will show all the databases.
         */
        addClusterToSchema(uri: string, clusterName: string, databasesNames: string[]): Promise<void>;
    }

    /**
     * A function that get a model Uri and returns a kusto worker that knows how to work
     * with that document.
     */
    export interface WorkerAccessor {
        (first: Uri, ...more: Uri[]): Promise<KustoWorker>;
    }

    export interface Column {
        name: string;
        type: string;
        docstring?: string;
    }
    export interface Table {
        name: string;
        columns: Column[];
        docstring?: string;
    }
    export interface ScalarParameter {
        name: string;
        type?: string;
        cslType?: string;
        cslDefaultValue?: string;
    }

    // an input parameter either be a scalar in which case it has a name, type and cslType, or it can be columnar, in which case
    // it will have a name, and a list of scalar types which are the column types.
    export type InputParameter = ScalarParameter & { columns?: ScalarParameter[] };

    export interface Function {
        name: string;
        body: string;
        docstring?: string;
        inputParameters: InputParameter[];
    }
    export interface Database {
        name: string;
        tables: Table[];
        functions: Function[];
        majorVersion: number;
        minorVersion: number;
    }

    export interface EngineSchema {
        clusterType: 'Engine';
        cluster: {
            connectionString: string;
            databases: Database[];
        };
        database: Database | undefined; // a reference to the database that's in current context.
    }

    export interface ClusterMangerSchema {
        clusterType: 'ClusterManager';
        accounts: string[];
        services: string[];
        connectionString: string;
    }

    export interface DataManagementSchema {
        clusterType: 'DataManagement';
    }

    export type Schema = EngineSchema | ClusterMangerSchema | DataManagementSchema;

    export var getKustoWorker: () => Promise<WorkerAccessor>;

    export declare type VisualizationType =
        | 'anomalychart'
        | 'areachart'
        | 'barchart'
        | 'columnchart'
        | 'ladderchart'
        | 'linechart'
        | 'piechart'
        | 'pivotchart'
        | 'scatterchart'
        | 'stackedareachart'
        | 'timechart'
        | 'table'
        | 'timeline'
        | 'timepivot'
        | 'card';

    export declare type Scale = 'linear' | 'log';
    export declare type LegendVisibility = 'visible' | 'hidden';
    export declare type YSplit = 'none' | 'axes' | 'panels';
    export declare type Kind = 'default' | 'unstacked' | 'stacked' | 'stacked100' | 'map';

    export interface RenderOptions {
        visualization?: null | VisualizationType;
        title?: null | string;
        xcolumn?: null | string;
        series?: null | string[];
        ycolumns?: null | string[];
        xtitle?: null | string;
        ytitle?: null | string;
        xaxis?: null | Scale;
        yaxis?: null | Scale;
        legend?: null | LegendVisibility;
        ySplit?: null | YSplit;
        accumulate?: null | boolean;
        kind?: null | Kind;
        anomalycolumns?: null | string[];
        ymin?: null | number;
        ymax?: null | number;
    }

    export interface RenderInfo {
        options: RenderOptions;
        location: { startOffset: number; endOffset: number };
    }

    export interface DatabaseReference {
        databaseName: string;
        clusterName: string; 
    };

    export interface ClusterReference {
        clusterName: string; 
    }

    export type RenderOptionKeys = keyof RenderOptions;

    export type OnDidProvideCompletionItems = (list: ls.CompletionList) => Promise<ls.CompletionList>;
}

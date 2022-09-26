import IEvent = monaco.IEvent;
export declare class LanguageServiceDefaultsImpl implements monaco.languages.kusto.LanguageServiceDefaults {
    private _onDidChange;
    private _languageSettings;
    private _workerMaxIdleTime;
    constructor(languageSettings: monaco.languages.kusto.LanguageSettings);
    get onDidChange(): IEvent<monaco.languages.kusto.LanguageServiceDefaults>;
    get languageSettings(): monaco.languages.kusto.LanguageSettings;
    setLanguageSettings(options: monaco.languages.kusto.LanguageSettings): void;
    setMaximumWorkerIdleTime(value: number): void;
    getWorkerMaxIdleTime(): number;
}
export declare function setupMonacoKusto(monacoInstance: typeof monaco): void;

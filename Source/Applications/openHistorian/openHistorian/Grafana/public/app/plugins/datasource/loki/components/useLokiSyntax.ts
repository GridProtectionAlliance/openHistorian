import { useState, useEffect } from 'react';
import Prism from 'prismjs';
import { AbsoluteTimeRange } from '@grafana/data';
import { CascaderOption } from '@grafana/ui';
import LokiLanguageProvider from 'app/plugins/datasource/loki/language_provider';
import { useLokiLabels } from 'app/plugins/datasource/loki/components/useLokiLabels';
import { useRefMounted } from 'app/core/hooks/useRefMounted';

const PRISM_SYNTAX = 'promql';

/**
 *
 * @param languageProvider
 * @description Initializes given language provider, exposes Loki syntax and enables loading label option values
 */
export const useLokiSyntax = (languageProvider: LokiLanguageProvider, absoluteRange: AbsoluteTimeRange) => {
  const mounted = useRefMounted();
  // State
  const [languageProviderInitialized, setLanguageProviderInitilized] = useState(false);
  const [syntax, setSyntax] = useState(null);

  /**
   * Holds information about currently selected option from rc-cascader to perform effect
   * that loads option values not fetched yet. Based on that useLokiLabels hook decides whether or not
   * the option requires additional data fetching
   */
  const [activeOption, setActiveOption] = useState<CascaderOption[]>();

  const { logLabelOptions, setLogLabelOptions, refreshLabels } = useLokiLabels(
    languageProvider,
    languageProviderInitialized,
    activeOption,
    absoluteRange
  );

  // Async
  const initializeLanguageProvider = async () => {
    languageProvider.initialRange = absoluteRange;
    await languageProvider.start();
    Prism.languages[PRISM_SYNTAX] = languageProvider.getSyntax();
    if (mounted.current) {
      setLogLabelOptions(languageProvider.logLabelOptions);
      setSyntax(languageProvider.getSyntax());
      setLanguageProviderInitilized(true);
    }
  };

  // Effects
  useEffect(() => {
    initializeLanguageProvider();
  }, []);

  return {
    isSyntaxReady: languageProviderInitialized,
    syntax,
    logLabelOptions,
    setActiveOption,
    refreshLabels,
  };
};

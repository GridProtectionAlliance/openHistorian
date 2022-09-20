import { VariableSupportType } from '@grafana/data';

import { LegacyVariableQueryEditor } from './editor/LegacyVariableQueryEditor';
import { StandardVariableQueryEditor } from './editor/getVariableQueryEditor';
import {
  hasCustomVariableSupport,
  hasDatasourceVariableSupport,
  hasLegacyVariableSupport,
  hasStandardVariableSupport,
  isLegacyQueryEditor,
  isQueryEditor,
} from './guard';

describe('type guards', () => {
  describe('hasLegacyVariableSupport', () => {
    describe('when called with a legacy data source', () => {
      it('should return true', () => {
        const datasource: any = { metricFindQuery: () => undefined };
        expect(hasLegacyVariableSupport(datasource)).toBe(true);
      });
    });

    describe('when called with data source without metricFindQuery function', () => {
      it('should return false', () => {
        const datasource: any = {};
        expect(hasLegacyVariableSupport(datasource)).toBe(false);
      });
    });

    describe('when called with a legacy data source with variable support', () => {
      it('should return false', () => {
        const datasource: any = { metricFindQuery: () => undefined, variables: {} };
        expect(hasLegacyVariableSupport(datasource)).toBe(false);
      });
    });
  });

  describe('hasStandardVariableSupport', () => {
    describe('when called with a data source with standard variable support', () => {
      it('should return true', () => {
        const datasource: any = {
          metricFindQuery: () => undefined,
          variables: { getType: () => VariableSupportType.Standard, toDataQuery: () => undefined },
        };
        expect(hasStandardVariableSupport(datasource)).toBe(true);
      });

      describe('and with a custom query', () => {
        it('should return true', () => {
          const datasource: any = {
            metricFindQuery: () => undefined,
            variables: {
              getType: () => VariableSupportType.Standard,
              toDataQuery: () => undefined,
              query: () => undefined,
            },
          };
          expect(hasStandardVariableSupport(datasource)).toBe(true);
        });
      });
    });

    describe('when called with a data source with partial standard variable support', () => {
      it('should return false', () => {
        const datasource: any = {
          metricFindQuery: () => undefined,
          variables: { getType: () => VariableSupportType.Standard, query: () => undefined },
        };
        expect(hasStandardVariableSupport(datasource)).toBe(false);
      });
    });

    describe('when called with a data source without standard variable support', () => {
      it('should return false', () => {
        const datasource: any = { metricFindQuery: () => undefined };
        expect(hasStandardVariableSupport(datasource)).toBe(false);
      });
    });
  });

  describe('hasCustomVariableSupport', () => {
    describe('when called with a data source with custom variable support', () => {
      it('should return true', () => {
        const datasource: any = {
          metricFindQuery: () => undefined,
          variables: { getType: () => VariableSupportType.Custom, query: () => undefined, editor: {} },
        };
        expect(hasCustomVariableSupport(datasource)).toBe(true);
      });
    });

    describe('when called with a data source with custom variable support but without editor', () => {
      it('should return false', () => {
        const datasource: any = {
          metricFindQuery: () => undefined,
          variables: { getType: () => VariableSupportType.Custom, query: () => undefined },
        };
        expect(hasCustomVariableSupport(datasource)).toBe(false);
      });
    });

    describe('when called with a data source with custom variable support but without query', () => {
      it('should return false', () => {
        const datasource: any = {
          metricFindQuery: () => undefined,
          variables: { getType: () => VariableSupportType.Custom, editor: {} },
        };
        expect(hasCustomVariableSupport(datasource)).toBe(false);
      });
    });

    describe('when called with a data source without custom variable support', () => {
      it('should return false', () => {
        const datasource: any = { metricFindQuery: () => undefined };
        expect(hasCustomVariableSupport(datasource)).toBe(false);
      });
    });
  });

  describe('hasDatasourceVariableSupport', () => {
    describe('when called with a data source with datasource variable support', () => {
      it('should return true', () => {
        const datasource: any = {
          metricFindQuery: () => undefined,
          variables: { getType: () => VariableSupportType.Datasource },
        };
        expect(hasDatasourceVariableSupport(datasource)).toBe(true);
      });
    });

    describe('when called with a data source without datasource variable support', () => {
      it('should return false', () => {
        const datasource: any = { metricFindQuery: () => undefined };
        expect(hasDatasourceVariableSupport(datasource)).toBe(false);
      });
    });
  });
});

describe('isLegacyQueryEditor', () => {
  describe('happy cases', () => {
    describe('when called with a legacy query editor but without a legacy data source', () => {
      it('then is should return true', () => {
        const component: any = LegacyVariableQueryEditor;
        const datasource: any = {};

        expect(isLegacyQueryEditor(component, datasource)).toBe(true);
      });
    });

    describe('when called with a legacy data source but without a legacy query editor', () => {
      it('then is should return true', () => {
        const component: any = StandardVariableQueryEditor;
        const datasource: any = { metricFindQuery: () => undefined };

        expect(isLegacyQueryEditor(component, datasource)).toBe(true);
      });
    });
  });

  describe('negative cases', () => {
    describe('when called without component', () => {
      it('then is should return false', () => {
        const component: any = null;
        const datasource: any = { metricFindQuery: () => undefined };

        expect(isLegacyQueryEditor(component, datasource)).toBe(false);
      });
    });

    describe('when called without a legacy query editor and without a legacy data source', () => {
      it('then is should return false', () => {
        const component: any = StandardVariableQueryEditor;
        const datasource: any = {};

        expect(isLegacyQueryEditor(component, datasource)).toBe(false);
      });
    });
  });
});

describe('isQueryEditor', () => {
  describe('happy cases', () => {
    describe('when called without a legacy editor and with a data source with standard variable support', () => {
      it('then is should return true', () => {
        const component: any = StandardVariableQueryEditor;
        const datasource: any = {
          variables: { getType: () => VariableSupportType.Standard, toDataQuery: () => undefined },
        };

        expect(isQueryEditor(component, datasource)).toBe(true);
      });
    });

    describe('when called without a legacy editor and with a data source with custom variable support', () => {
      it('then is should return true', () => {
        const component: any = StandardVariableQueryEditor;
        const datasource: any = {
          variables: { getType: () => VariableSupportType.Custom, query: () => undefined, editor: {} },
        };

        expect(isQueryEditor(component, datasource)).toBe(true);
      });
    });

    describe('when called without a legacy editor and with a data source with datasource variable support', () => {
      it('then is should return true', () => {
        const component: any = StandardVariableQueryEditor;
        const datasource: any = { variables: { getType: () => VariableSupportType.Datasource } };

        expect(isQueryEditor(component, datasource)).toBe(true);
      });
    });
  });

  describe('negative cases', () => {
    describe('when called without component', () => {
      it('then is should return false', () => {
        const component: any = null;
        const datasource: any = { metricFindQuery: () => undefined };

        expect(isQueryEditor(component, datasource)).toBe(false);
      });
    });

    describe('when called with a legacy query editor', () => {
      it('then is should return false', () => {
        const component: any = LegacyVariableQueryEditor;
        const datasource: any = { variables: { getType: () => VariableSupportType.Datasource } };

        expect(isQueryEditor(component, datasource)).toBe(false);
      });
    });

    describe('when called without a legacy query editor but with a legacy data source', () => {
      it('then is should return false', () => {
        const component: any = StandardVariableQueryEditor;
        const datasource: any = { metricFindQuery: () => undefined };

        expect(isQueryEditor(component, datasource)).toBe(false);
      });
    });
  });
});

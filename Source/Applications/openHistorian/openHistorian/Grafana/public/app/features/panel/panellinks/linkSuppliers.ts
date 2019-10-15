import { PanelModel } from 'app/features/dashboard/state/PanelModel';
import { FieldDisplay } from '@grafana/ui';
import { LinkModelSupplier, getTimeField, Labels, ScopedVars, ScopedVar } from '@grafana/data';
import { getLinkSrv } from './link_srv';

interface SeriesVars {
  name?: string;
  labels?: Labels;
  refId?: string;
}

interface FieldVars {
  name: string;
}

interface ValueVars {
  raw: any;
  numeric: number;
  text: string;
  time?: number;
  calc?: string;
}

interface DataLinkScopedVars extends ScopedVars {
  __series?: ScopedVar<SeriesVars>;
  __field?: ScopedVar<FieldVars>;
  __value?: ScopedVar<ValueVars>;
}

/**
 * Link suppliers creates link models based on a link origin
 */

export const getFieldLinksSupplier = (value: FieldDisplay): LinkModelSupplier<FieldDisplay> | undefined => {
  const links = value.field.links;
  if (!links || links.length === 0) {
    return undefined;
  }
  return {
    getLinks: (_scopedVars?: any) => {
      const scopedVars: DataLinkScopedVars = {};

      if (value.view) {
        const { dataFrame } = value.view;

        scopedVars['__series'] = {
          value: {
            name: dataFrame.name,
            labels: dataFrame.labels,
            refId: dataFrame.refId,
          },
          text: 'Series',
        };

        const field = value.colIndex !== undefined ? dataFrame.fields[value.colIndex] : undefined;
        if (field) {
          console.log('Full Field Info:', field);
          scopedVars['__field'] = {
            value: {
              name: field.name,
            },
            text: 'Field',
          };
        }

        if (value.rowIndex) {
          const { timeField } = getTimeField(dataFrame);
          scopedVars['__value'] = {
            value: {
              raw: field.values.get(value.rowIndex),
              numeric: value.display.numeric,
              text: value.display.text,
              time: timeField ? timeField.values.get(value.rowIndex) : undefined,
            },
            text: 'Value',
          };
        } else {
          // calculation
          scopedVars['__value'] = {
            value: {
              raw: value.display.numeric,
              numeric: value.display.numeric,
              text: value.display.text,
              calc: value.name,
            },
            text: 'Value',
          };
        }
      } else {
        console.log('VALUE', value);
      }

      return links.map(link => {
        return getLinkSrv().getDataLinkUIModel(link, scopedVars, value);
      });
    },
  };
};

export const getPanelLinksSupplier = (value: PanelModel): LinkModelSupplier<PanelModel> => {
  const links = value.links;

  if (!links || links.length === 0) {
    return undefined;
  }

  return {
    getLinks: () => {
      return links.map(link => {
        return getLinkSrv().getDataLinkUIModel(link, value.scopedVars, value);
      });
    },
  };
};

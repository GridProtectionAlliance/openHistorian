import { each, flatten, groupBy, isArray } from 'lodash';

import { AnnotationEvent, DataFrame, DataQuery, FieldType, QueryResultMeta } from '@grafana/data';
import { toDataQueryResponse } from '@grafana/runtime';
import TableModel from 'app/core/table_model';

import { InfluxQuery } from './types';

export default class ResponseParser {
  parse(query: string, results: { results: any }) {
    if (!results?.results || results.results.length === 0) {
      return [];
    }

    const influxResults = results.results[0];
    if (!influxResults.series) {
      return [];
    }

    const normalizedQuery = query.toLowerCase();
    const isValueFirst =
      normalizedQuery.indexOf('show field keys') >= 0 || normalizedQuery.indexOf('show retention policies') >= 0;

    const res = new Set<string>();
    each(influxResults.series, (serie) => {
      each(serie.values, (value) => {
        if (isArray(value)) {
          // In general, there are 2 possible shapes for the returned value.
          // The first one is a two-element array,
          // where the first element is somewhat a metadata value:
          // the tag name for SHOW TAG VALUES queries,
          // the time field for SELECT queries, etc.
          // The second shape is an one-element array,
          // that is containing an immediate value.
          // For example, SHOW FIELD KEYS queries return such shape.
          // Note, pre-0.11 versions return
          // the second shape for SHOW TAG VALUES queries
          // (while the newer versions—first).

          if (isValueFirst) {
            addUnique(res, value[0]);
          } else if (value[1] !== undefined) {
            addUnique(res, value[1]);
          } else {
            addUnique(res, value[0]);
          }
        } else {
          addUnique(res, value);
        }
      });
    });

    // NOTE: it is important to keep the order of items in the parsed output
    // the same as it was in the influxdb-response.
    // we use a `Set` to collect the unique-results, and `Set` iteration
    // order is insertion-order, so this should be ok.
    return Array.from(res).map((v) => ({ text: v }));
  }

  getTable(dfs: DataFrame[], target: InfluxQuery, meta: QueryResultMeta): TableModel {
    let table = new TableModel();

    if (dfs.length > 0) {
      table.meta = {
        ...meta,
        executedQueryString: dfs[0].meta?.executedQueryString,
      };

      table.refId = target.refId;
      table = getTableCols(dfs, table, target);

      // if group by tag(s) added
      if (dfs[0].fields[1] && dfs[0].fields[1].labels) {
        let dfsByLabels: any = groupBy(dfs, (df: DataFrame) =>
          df.fields[1].labels ? Object.values(df.fields[1].labels!) : null
        );
        const labels = Object.keys(dfsByLabels);
        dfsByLabels = Object.values(dfsByLabels);

        for (let i = 0; i < dfsByLabels.length; i++) {
          table = getTableRows(dfsByLabels[i], table, [...labels[i].split(',')]);
        }
      } else {
        table = getTableRows(dfs, table, []);
      }
    }

    return table;
  }

  async transformAnnotationResponse(options: any, data: any, target: InfluxQuery): Promise<AnnotationEvent[]> {
    const rsp = toDataQueryResponse(data, [target] as DataQuery[]);

    if (rsp) {
      const table = this.getTable(rsp.data, target, {});
      const list: any[] = [];
      let titleCol: any = null;
      let timeCol: any = null;
      let timeEndCol: any = null;
      const tagsCol: any = [];
      let textCol: any = null;

      each(table.columns, (column, index) => {
        if (column.text.toLowerCase() === 'time') {
          timeCol = index;
          return;
        }
        if (column.text === options.annotation.titleColumn) {
          titleCol = index;
          return;
        }
        if (colContainsTag(column.text, options.annotation.tagsColumn)) {
          tagsCol.push(index);
          return;
        }
        if (column.text.includes(options.annotation.textColumn)) {
          textCol = index;
          return;
        }
        if (column.text === options.annotation.timeEndColumn) {
          timeEndCol = index;
          return;
        }
        // legacy case
        if (!titleCol && textCol !== index) {
          titleCol = index;
        }
      });

      each(table.rows, (value) => {
        const data = {
          annotation: options.annotation,
          time: +new Date(value[timeCol]),
          title: value[titleCol],
          timeEnd: value[timeEndCol],
          // Remove empty values, then split in different tags for comma separated values
          tags: flatten(
            tagsCol
              .filter((t: any) => {
                return value[t];
              })
              .map((t: any) => {
                return value[t].split(',');
              })
          ),
          text: value[textCol],
        };

        list.push(data);
      });

      return list;
    }
    return [];
  }
}

function colContainsTag(colText: string, tagsColumn: string): boolean {
  const tags = (tagsColumn || '').replace(' ', '').split(',');
  for (var tag of tags) {
    if (colText.includes(tag)) {
      return true;
    }
  }
  return false;
}

function getTableCols(dfs: DataFrame[], table: TableModel, target: InfluxQuery): TableModel {
  const selectedParams = getSelectedParams(target);

  dfs[0].fields.forEach((field) => {
    // Time col
    if (field.name === 'time') {
      table.columns.push({ text: 'Time', type: FieldType.time });
    }

    // Group by (label) column(s)
    else if (field.name === 'value') {
      if (field.labels) {
        Object.keys(field.labels).forEach((key) => {
          table.columns.push({ text: key });
        });
      }
    }
  });

  // Get cols for annotationQuery
  if (dfs[0].refId === 'metricFindQuery') {
    dfs.forEach((field) => {
      if (field.name) {
        table.columns.push({ text: field.name });
      }
    });
  }

  // Select (metric) column(s)
  for (let i = 0; i < selectedParams.length; i++) {
    table.columns.push({ text: selectedParams[i] });
  }

  return table;
}

function getTableRows(dfs: DataFrame[], table: TableModel, labels: string[]): TableModel {
  const values = dfs[0].fields[0].values.toArray();

  for (let i = 0; i < values.length; i++) {
    const time = values[i];
    const metrics = dfs.map((df: DataFrame) => {
      return df.fields[1] ? df.fields[1].values.toArray()[i] : null;
    });
    if (metrics.indexOf(null) < 0) {
      table.rows.push([time, ...labels, ...metrics]);
    }
  }
  return table;
}

export function getSelectedParams(target: InfluxQuery): string[] {
  let allParams: string[] = [];
  target.select?.forEach((select) => {
    const selector = select.filter((x) => x.type !== 'field');
    if (selector.length > 0) {
      allParams.push(selector[0].type);
    } else {
      if (select[0] && select[0].params && select[0].params[0]) {
        allParams.push(select[0].params[0].toString());
      }
    }
  });

  let uniqueParams: string[] = [];
  allParams.forEach((param) => {
    uniqueParams.push(incrementName(param, param, uniqueParams, 0));
  });

  return uniqueParams;
}

function incrementName(name: string, nameIncremenet: string, params: string[], index: number): string {
  if (params.indexOf(nameIncremenet) > -1) {
    index++;
    return incrementName(name, name + '_' + index, params, index);
  }
  return nameIncremenet;
}

function addUnique(s: Set<string>, value: string | number) {
  s.add(value.toString());
}

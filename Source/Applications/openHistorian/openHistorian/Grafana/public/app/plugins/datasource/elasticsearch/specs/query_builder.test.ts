import { gte, lt } from 'semver';

import { ElasticQueryBuilder } from '../query_builder';
import { ElasticsearchQuery } from '../types';

describe('ElasticQueryBuilder', () => {
  const builder = new ElasticQueryBuilder({ timeField: '@timestamp', esVersion: '2.0.0' });
  const builder5x = new ElasticQueryBuilder({ timeField: '@timestamp', esVersion: '5.0.0' });
  const builder56 = new ElasticQueryBuilder({ timeField: '@timestamp', esVersion: '5.6.0' });
  const builder6x = new ElasticQueryBuilder({ timeField: '@timestamp', esVersion: '6.0.0' });
  const builder7x = new ElasticQueryBuilder({ timeField: '@timestamp', esVersion: '7.0.0' });
  const builder77 = new ElasticQueryBuilder({ timeField: '@timestamp', esVersion: '7.7.0' });
  const builder8 = new ElasticQueryBuilder({ timeField: '@timestamp', esVersion: '8.0.0' });

  const allBuilders = [builder, builder5x, builder56, builder6x, builder7x, builder77, builder8];

  allBuilders.forEach((builder) => {
    describe(`version ${builder.esVersion}`, () => {
      it('should return query with defaults', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [{ type: 'count', id: '0' }],
          timeField: '@timestamp',
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '1' }],
        });

        expect(query.query.bool.filter[0].range['@timestamp'].gte).toBe('$timeFrom');
        expect(query.aggs['1'].date_histogram.extended_bounds.min).toBe('$timeFrom');
      });

      it('should clean settings from null values', () => {
        const query = builder.build({
          refId: 'A',
          // The following `missing: null as any` is because previous versions of the DS where
          // storing null in the query model when inputting an empty string,
          // which were then removed in the query builder.
          // The new version doesn't store empty strings at all. This tests ensures backward compatinility.
          metrics: [{ type: 'avg', id: '0', settings: { missing: null as any, script: '1' } }],
          timeField: '@timestamp',
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '1' }],
        });

        expect(query.aggs['1'].aggs['0'].avg.missing).not.toBeDefined();
        expect(query.aggs['1'].aggs['0'].avg.script).toBeDefined();
      });

      it('with multiple bucket aggs', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [{ type: 'count', id: '1' }],
          timeField: '@timestamp',
          bucketAggs: [
            { type: 'terms', field: '@host', id: '2' },
            { type: 'date_histogram', field: '@timestamp', id: '3' },
          ],
        });

        expect(query.aggs['2'].terms.field).toBe('@host');
        expect(query.aggs['2'].aggs['3'].date_histogram.field).toBe('@timestamp');
      });

      it('with select field', () => {
        const query = builder.build(
          {
            refId: 'A',
            metrics: [{ type: 'avg', field: '@value', id: '1' }],
            bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '2' }],
          },
          100
        );

        const aggs = query.aggs['2'].aggs;
        expect(aggs['1'].avg.field).toBe('@value');
      });

      it('term agg and order by term', () => {
        const target: ElasticsearchQuery = {
          refId: 'A',
          metrics: [
            { type: 'count', id: '1' },
            { type: 'avg', field: '@value', id: '5' },
          ],
          bucketAggs: [
            {
              type: 'terms',
              field: '@host',
              settings: { size: '5', order: 'asc', orderBy: '_term' },
              id: '2',
            },
            { type: 'date_histogram', field: '@timestamp', id: '3' },
          ],
        };

        const query = builder.build(target, 100);
        const firstLevel = query.aggs['2'];

        if (gte(builder.esVersion, '6.0.0')) {
          expect(firstLevel.terms.order._key).toBe('asc');
        } else {
          expect(firstLevel.terms.order._term).toBe('asc');
        }
      });

      it('with term agg and order by metric agg', () => {
        const query = builder.build(
          {
            refId: 'A',
            metrics: [
              { type: 'count', id: '1' },
              { type: 'avg', field: '@value', id: '5' },
            ],
            bucketAggs: [
              {
                type: 'terms',
                field: '@host',
                settings: { size: '5', order: 'asc', orderBy: '5' },
                id: '2',
              },
              { type: 'date_histogram', field: '@timestamp', id: '3' },
            ],
          },
          100
        );

        const firstLevel = query.aggs['2'];
        const secondLevel = firstLevel.aggs['3'];

        expect(firstLevel.aggs['5'].avg.field).toBe('@value');
        expect(secondLevel.aggs['5'].avg.field).toBe('@value');
      });

      it('with term agg and order by count agg', () => {
        const query = builder.build(
          {
            refId: 'A',
            metrics: [
              { type: 'count', id: '1' },
              { type: 'avg', field: '@value', id: '5' },
            ],
            bucketAggs: [
              {
                type: 'terms',
                field: '@host',
                settings: { size: '5', order: 'asc', orderBy: '1' },
                id: '2',
              },
              { type: 'date_histogram', field: '@timestamp', id: '3' },
            ],
          },
          100
        );

        expect(query.aggs['2'].terms.order._count).toEqual('asc');
        expect(query.aggs['2'].aggs).not.toHaveProperty('1');
      });

      it('with term agg and order by extended_stats agg', () => {
        const query = builder.build(
          {
            refId: 'A',
            metrics: [{ type: 'extended_stats', id: '1', field: '@value', meta: { std_deviation: true } }],
            bucketAggs: [
              {
                type: 'terms',
                field: '@host',
                settings: { size: '5', order: 'asc', orderBy: '1[std_deviation]' },
                id: '2',
              },
              { type: 'date_histogram', field: '@timestamp', id: '3' },
            ],
          },
          100
        );

        const firstLevel = query.aggs['2'];
        const secondLevel = firstLevel.aggs['3'];

        expect(firstLevel.aggs['1'].extended_stats.field).toBe('@value');
        expect(secondLevel.aggs['1'].extended_stats.field).toBe('@value');
      });

      it('with term agg and order by percentiles agg', () => {
        const query = builder.build(
          {
            refId: 'A',
            metrics: [{ type: 'percentiles', id: '1', field: '@value', settings: { percents: ['95', '99'] } }],
            bucketAggs: [
              {
                type: 'terms',
                field: '@host',
                settings: { size: '5', order: 'asc', orderBy: '1[95.0]' },
                id: '2',
              },
              { type: 'date_histogram', field: '@timestamp', id: '3' },
            ],
          },
          100
        );

        const firstLevel = query.aggs['2'];
        const secondLevel = firstLevel.aggs['3'];

        expect(firstLevel.aggs['1'].percentiles.field).toBe('@value');
        expect(secondLevel.aggs['1'].percentiles.field).toBe('@value');
      });

      it('with term agg and valid min_doc_count', () => {
        const query = builder.build(
          {
            refId: 'A',
            metrics: [{ type: 'count', id: '1' }],
            bucketAggs: [
              {
                type: 'terms',
                field: '@host',
                settings: { min_doc_count: '1' },
                id: '2',
              },
              { type: 'date_histogram', field: '@timestamp', id: '3' },
            ],
          },
          100
        );

        const firstLevel = query.aggs['2'];
        expect(firstLevel.terms.min_doc_count).toBe(1);
      });

      it('with term agg and variable as min_doc_count', () => {
        const query = builder.build(
          {
            refId: 'A',
            metrics: [{ type: 'count', id: '1' }],
            bucketAggs: [
              {
                type: 'terms',
                field: '@host',
                settings: { min_doc_count: '$min_doc_count' },
                id: '2',
              },
              { type: 'date_histogram', field: '@timestamp', id: '3' },
            ],
          },
          100
        );

        const firstLevel = query.aggs['2'];
        expect(firstLevel.terms.min_doc_count).toBe('$min_doc_count');
      });

      it('with metric percentiles', () => {
        const percents = ['1', '2', '3', '4'];
        const field = '@load_time';

        const query = builder.build(
          {
            refId: 'A',
            metrics: [
              {
                id: '1',
                type: 'percentiles',
                field,
                settings: {
                  percents,
                },
              },
            ],
            bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '3' }],
          },
          100
        );

        const firstLevel = query.aggs['3'];

        expect(firstLevel.aggs['1'].percentiles.field).toBe(field);
        expect(firstLevel.aggs['1'].percentiles.percents).toEqual(percents);
      });

      it('with filters aggs', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [{ type: 'count', id: '1' }],
          timeField: '@timestamp',
          bucketAggs: [
            {
              id: '2',
              type: 'filters',
              settings: {
                filters: [
                  { query: '@metric:cpu', label: '' },
                  { query: '@metric:logins.count', label: '' },
                ],
              },
            },
            { type: 'date_histogram', field: '@timestamp', id: '4' },
          ],
        });

        expect(query.aggs['2'].filters.filters['@metric:cpu'].query_string.query).toBe('@metric:cpu');
        expect(query.aggs['2'].filters.filters['@metric:logins.count'].query_string.query).toBe('@metric:logins.count');
        expect(query.aggs['2'].aggs['4'].date_histogram.field).toBe('@timestamp');
      });

      it('should return correct query for raw_document metric', () => {
        const target: ElasticsearchQuery = {
          refId: 'A',
          metrics: [{ type: 'raw_document', id: '1', settings: {} }],
          timeField: '@timestamp',
          bucketAggs: [] as any[],
        };

        const query = builder.build(target);
        expect(query).toMatchObject({
          size: 500,
          query: {
            bool: {
              filter: [
                {
                  range: {
                    '@timestamp': {
                      format: 'epoch_millis',
                      gte: '$timeFrom',
                      lte: '$timeTo',
                    },
                  },
                },
              ],
            },
          },
          sort: [{ '@timestamp': { order: 'desc', unmapped_type: 'boolean' } }, { _doc: { order: 'desc' } }],
          script_fields: {},
        });
      });

      it('should set query size from settings when raw_documents', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [{ type: 'raw_document', id: '1', settings: { size: '1337' } }],
          timeField: '@timestamp',
          bucketAggs: [],
        });

        expect(query.size).toBe(1337);
      });

      it('with moving average', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [
            {
              id: '3',
              type: 'sum',
              field: '@value',
            },
            {
              id: '2',
              type: 'moving_avg',
              field: '3',
            },
          ],
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '3' }],
        });

        const firstLevel = query.aggs['3'];

        expect(firstLevel.aggs['2']).not.toBe(undefined);
        expect(firstLevel.aggs['2'].moving_avg).not.toBe(undefined);
        expect(firstLevel.aggs['2'].moving_avg.buckets_path).toBe('3');
      });

      it('with moving average doc count', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [
            {
              id: '3',
              type: 'count',
            },
            {
              id: '2',
              type: 'moving_avg',
              field: '3',
            },
          ],
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '4' }],
        });

        const firstLevel = query.aggs['4'];

        expect(firstLevel.aggs['2']).not.toBe(undefined);
        expect(firstLevel.aggs['2'].moving_avg).not.toBe(undefined);
        expect(firstLevel.aggs['2'].moving_avg.buckets_path).toBe('_count');
      });

      it('with broken moving average', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [
            {
              id: '3',
              type: 'sum',
              field: '@value',
            },
            {
              id: '2',
              type: 'moving_avg',
              field: '3',
            },
            {
              id: '4',
              type: 'moving_avg',
            },
          ],
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '3' }],
        });

        const firstLevel = query.aggs['3'];

        expect(firstLevel.aggs['2']).not.toBe(undefined);
        expect(firstLevel.aggs['2'].moving_avg).not.toBe(undefined);
        expect(firstLevel.aggs['2'].moving_avg.buckets_path).toBe('3');
        expect(firstLevel.aggs['4']).toBe(undefined);
      });

      it('with top_metrics', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [
            {
              id: '2',
              type: 'top_metrics',
              settings: {
                order: 'desc',
                orderBy: '@timestamp',
                metrics: ['@value'],
              },
            },
          ],
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '3' }],
        });

        const firstLevel = query.aggs['3'];

        expect(firstLevel.aggs['2']).not.toBe(undefined);
        expect(firstLevel.aggs['2'].top_metrics).not.toBe(undefined);
        expect(firstLevel.aggs['2'].top_metrics.metrics).not.toBe(undefined);
        expect(firstLevel.aggs['2'].top_metrics.size).not.toBe(undefined);
        expect(firstLevel.aggs['2'].top_metrics.sort).not.toBe(undefined);
        expect(firstLevel.aggs['2'].top_metrics.metrics.length).toBe(1);
        expect(firstLevel.aggs['2'].top_metrics.metrics).toEqual([{ field: '@value' }]);
        expect(firstLevel.aggs['2'].top_metrics.sort).toEqual([{ '@timestamp': 'desc' }]);
        expect(firstLevel.aggs['2'].top_metrics.size).toBe(1);
      });

      it('with derivative', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [
            {
              id: '3',
              type: 'sum',
              field: '@value',
            },
            {
              id: '2',
              type: 'derivative',
              field: '3',
            },
          ],
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '3' }],
        });

        const firstLevel = query.aggs['3'];

        expect(firstLevel.aggs['2']).not.toBe(undefined);
        expect(firstLevel.aggs['2'].derivative).not.toBe(undefined);
        expect(firstLevel.aggs['2'].derivative.buckets_path).toBe('3');
      });

      it('with derivative doc count', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [
            {
              id: '3',
              type: 'count',
            },
            {
              id: '2',
              type: 'derivative',
              field: '3',
            },
          ],
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '4' }],
        });

        const firstLevel = query.aggs['4'];

        expect(firstLevel.aggs['2']).not.toBe(undefined);
        expect(firstLevel.aggs['2'].derivative).not.toBe(undefined);
        expect(firstLevel.aggs['2'].derivative.buckets_path).toBe('_count');
      });

      it('with serial_diff', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [
            {
              id: '3',
              type: 'max',
              field: '@value',
            },
            {
              id: '2',
              type: 'serial_diff',
              field: '3',
              settings: {
                lag: '5',
              },
            },
          ],
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '3' }],
        });

        const firstLevel = query.aggs['3'];

        expect(firstLevel.aggs['2']).not.toBe(undefined);
        expect(firstLevel.aggs['2'].serial_diff).not.toBe(undefined);
        expect(firstLevel.aggs['2'].serial_diff.buckets_path).toBe('3');
        expect(firstLevel.aggs['2'].serial_diff.lag).toBe(5);
      });

      it('with bucket_script', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [
            {
              id: '1',
              type: 'sum',
              field: '@value',
            },
            {
              id: '3',
              type: 'max',
              field: '@value',
            },
            {
              id: '4',
              pipelineVariables: [
                {
                  name: 'var1',
                  pipelineAgg: '1',
                },
                {
                  name: 'var2',
                  pipelineAgg: '3',
                },
              ],
              settings: {
                script: 'params.var1 * params.var2',
              },
              type: 'bucket_script',
            },
          ],
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '2' }],
        });

        const firstLevel = query.aggs['2'];

        expect(firstLevel.aggs['4']).not.toBe(undefined);
        expect(firstLevel.aggs['4'].bucket_script).not.toBe(undefined);
        expect(firstLevel.aggs['4'].bucket_script.buckets_path).toMatchObject({ var1: '1', var2: '3' });
      });

      it('with bucket_script doc count', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [
            {
              id: '3',
              type: 'count',
            },
            {
              id: '4',
              pipelineVariables: [
                {
                  name: 'var1',
                  pipelineAgg: '3',
                },
              ],
              settings: {
                script: 'params.var1 * 1000',
              },
              type: 'bucket_script',
            },
          ],
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '2' }],
        });

        const firstLevel = query.aggs['2'];

        expect(firstLevel.aggs['4']).not.toBe(undefined);
        expect(firstLevel.aggs['4'].bucket_script).not.toBe(undefined);
        expect(firstLevel.aggs['4'].bucket_script.buckets_path).toMatchObject({ var1: '_count' });
      });

      it('with histogram', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [{ id: '1', type: 'count' }],
          bucketAggs: [
            {
              type: 'histogram',
              field: 'bytes',
              id: '3',
              settings: {
                interval: '10',
                min_doc_count: '2',
              },
            },
          ],
        });

        const firstLevel = query.aggs['3'];
        expect(firstLevel.histogram.field).toBe('bytes');
        expect(firstLevel.histogram.interval).toBe('10');
        expect(firstLevel.histogram.min_doc_count).toBe('2');
      });

      it('with adhoc filters', () => {
        const query = builder.build(
          {
            refId: 'A',
            metrics: [{ type: 'count', id: '0' }],
            timeField: '@timestamp',
            bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '3' }],
          },
          [
            { key: 'key1', operator: '=', value: 'value1' },
            { key: 'key2', operator: '=', value: 'value2' },
            { key: 'key2', operator: '!=', value: 'value2' },
            { key: 'key3', operator: '<', value: 'value3' },
            { key: 'key4', operator: '>', value: 'value4' },
            { key: 'key5', operator: '=~', value: 'value5' },
            { key: 'key6', operator: '!~', value: 'value6' },
          ]
        );

        expect(query.query.bool.must[0].match_phrase['key1'].query).toBe('value1');
        expect(query.query.bool.must[1].match_phrase['key2'].query).toBe('value2');
        expect(query.query.bool.must_not[0].match_phrase['key2'].query).toBe('value2');
        expect(query.query.bool.filter[1].range['key3'].lt).toBe('value3');
        expect(query.query.bool.filter[2].range['key4'].gt).toBe('value4');
        expect(query.query.bool.filter[3].regexp['key5']).toBe('value5');
        expect(query.query.bool.filter[4].bool.must_not.regexp['key6']).toBe('value6');
      });

      describe('getTermsQuery', () => {
        function testGetTermsQuery(queryDef: any) {
          const query = builder.getTermsQuery(queryDef);
          return query.aggs['1'].terms.order;
        }

        function checkSort(order: any, expected: string) {
          if (lt(builder.esVersion, '6.0.0')) {
            expect(order._term).toBe(expected);
            expect(order._key).toBeUndefined();
          } else {
            expect(order._term).toBeUndefined();
            expect(order._key).toBe(expected);
          }
        }

        it('should set correct default sorting', () => {
          const order = testGetTermsQuery({});
          checkSort(order, 'asc');
          expect(order._count).toBeUndefined();
        });

        it('should set correct explicit sorting', () => {
          const order = testGetTermsQuery({ order: 'desc' });
          checkSort(order, 'desc');
          expect(order._count).toBeUndefined();
        });

        it('getTermsQuery(orderBy:doc_count) should set desc sorting on _count', () => {
          const query = builder.getTermsQuery({ orderBy: 'doc_count' });
          expect(query.aggs['1'].terms.order._term).toBeUndefined();
          expect(query.aggs['1'].terms.order._key).toBeUndefined();
          expect(query.aggs['1'].terms.order._count).toBe('desc');
        });

        it('getTermsQuery(orderBy:doc_count, order:asc) should set asc sorting on _count', () => {
          const query = builder.getTermsQuery({ orderBy: 'doc_count', order: 'asc' });
          expect(query.aggs['1'].terms.order._term).toBeUndefined();
          expect(query.aggs['1'].terms.order._key).toBeUndefined();
          expect(query.aggs['1'].terms.order._count).toBe('asc');
        });

        describe('lucene query', () => {
          it('should add query_string filter when query is not empty', () => {
            const luceneQuery = 'foo';
            const query = builder.getTermsQuery({ orderBy: 'doc_count', order: 'asc', query: luceneQuery });

            expect(query.query.bool.filter).toContainEqual({
              query_string: { analyze_wildcard: true, query: luceneQuery },
            });
          });

          it('should not add query_string filter when query is empty', () => {
            const query = builder.getTermsQuery({ orderBy: 'doc_count', order: 'asc' });

            expect(
              query.query.bool.filter.find((filter: any) => Object.keys(filter).includes('query_string'))
            ).toBeFalsy();
          });
        });
      });

      describe('lucene query', () => {
        it('should add query_string filter when query is not empty', () => {
          const luceneQuery = 'foo';
          const query = builder.build({ refId: 'A', query: luceneQuery });

          expect(query.query.bool.filter).toContainEqual({
            query_string: { analyze_wildcard: true, query: luceneQuery },
          });
        });

        it('should not add query_string filter when query is empty', () => {
          const query = builder.build({ refId: 'A' });

          expect(
            query.query.bool.filter.find((filter: any) => Object.keys(filter).includes('query_string'))
          ).toBeFalsy();
        });
      });

      describe('getLogsQuery', () => {
        it('should return query with defaults', () => {
          const query = builder.getLogsQuery({ refId: 'A' }, 500, null);

          expect(query.size).toEqual(500);

          const expectedQuery = {
            bool: {
              filter: [{ range: { '@timestamp': { gte: '$timeFrom', lte: '$timeTo', format: 'epoch_millis' } } }],
            },
          };
          expect(query.query).toEqual(expectedQuery);

          expect(query.sort).toEqual([
            { '@timestamp': { order: 'desc', unmapped_type: 'boolean' } },
            { _doc: { order: 'desc' } },
          ]);

          const expectedAggs: any = {
            // FIXME: It's pretty weak to include this '1' in the test as it's not part of what we are testing here and
            // might change as a cause of unrelated changes
            1: {
              aggs: {},
              date_histogram: {
                extended_bounds: { max: '$timeTo', min: '$timeFrom' },
                field: '@timestamp',
                format: 'epoch_millis',
                fixed_interval: '${__interval_ms}ms',
                min_doc_count: 0,
              },
            },
          };

          expect(query.aggs).toMatchObject(expectedAggs);
        });

        describe('lucene query', () => {
          it('should add query_string filter when query is not empty', () => {
            const luceneQuery = 'foo';
            const query = builder.getLogsQuery({ refId: 'A', query: luceneQuery }, 500, null);

            expect(query.query.bool.filter).toContainEqual({
              query_string: { analyze_wildcard: true, query: luceneQuery },
            });
          });

          it('should not add query_string filter when query is empty', () => {
            const query = builder.getLogsQuery({ refId: 'A' }, 500, null);

            expect(
              query.query.bool.filter.find((filter: any) => Object.keys(filter).includes('query_string'))
            ).toBeFalsy();
          });
        });

        it('with adhoc filters', () => {
          // TODO: Types for AdHocFilters
          const adhocFilters = [
            { key: 'key1', operator: '=', value: 'value1' },
            { key: 'key2', operator: '!=', value: 'value2' },
            { key: 'key3', operator: '<', value: 'value3' },
            { key: 'key4', operator: '>', value: 'value4' },
            { key: 'key5', operator: '=~', value: 'value5' },
            { key: 'key6', operator: '!~', value: 'value6' },
          ];
          const query = builder.getLogsQuery({ refId: 'A' }, 500, adhocFilters);

          expect(query.query.bool.must[0].match_phrase['key1'].query).toBe('value1');
          expect(query.query.bool.must_not[0].match_phrase['key2'].query).toBe('value2');
          expect(query.query.bool.filter[1].range['key3'].lt).toBe('value3');
          expect(query.query.bool.filter[2].range['key4'].gt).toBe('value4');
          expect(query.query.bool.filter[3].regexp['key5']).toBe('value5');
          expect(query.query.bool.filter[4].bool.must_not.regexp['key6']).toBe('value6');
        });
      });
    });
  });

  describe('Value casting for settings', () => {
    it('correctly casts values in moving_avg ', () => {
      const query = builder7x.build({
        refId: 'A',
        metrics: [
          { type: 'avg', id: '2' },
          {
            type: 'moving_avg',
            id: '3',
            field: '2',
            settings: {
              window: '5',
              model: 'holt_winters',
              predict: '10',
              settings: {
                alpha: '1',
                beta: '2',
                gamma: '3',
                period: '4',
              },
            },
          },
        ],
        timeField: '@timestamp',
        bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '1' }],
      });

      const movingAvg = query.aggs['1'].aggs['3'].moving_avg;

      expect(movingAvg.window).toBe(5);
      expect(movingAvg.predict).toBe(10);
      expect(movingAvg.settings.alpha).toBe(1);
      expect(movingAvg.settings.beta).toBe(2);
      expect(movingAvg.settings.gamma).toBe(3);
      expect(movingAvg.settings.period).toBe(4);
    });

    it('correctly casts values in serial_diff ', () => {
      const query = builder7x.build({
        refId: 'A',
        metrics: [
          { type: 'avg', id: '2' },
          {
            type: 'serial_diff',
            id: '3',
            field: '2',
            settings: {
              lag: '1',
            },
          },
        ],
        timeField: '@timestamp',
        bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '1' }],
      });

      const serialDiff = query.aggs['1'].aggs['3'].serial_diff;

      expect(serialDiff.lag).toBe(1);
    });

    describe('date_histogram', () => {
      it('should not include time_zone if not present in the query model', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [{ type: 'count', id: '1' }],
          timeField: '@timestamp',
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '2', settings: { min_doc_count: '1' } }],
        });

        expect(query.aggs['2'].date_histogram.time_zone).not.toBeDefined();
      });

      it('should not include time_zone if "utc" in the query model', () => {
        const query = builder.build({
          refId: 'A',
          metrics: [{ type: 'count', id: '1' }],
          timeField: '@timestamp',
          bucketAggs: [
            { type: 'date_histogram', field: '@timestamp', id: '2', settings: { min_doc_count: '1', timeZone: 'utc' } },
          ],
        });

        expect(query.aggs['2'].date_histogram.time_zone).not.toBeDefined();
      });

      it('should include time_zone if not "utc" in the query model', () => {
        const expectedTimezone = 'America/Los_angeles';
        const query = builder.build({
          refId: 'A',
          metrics: [{ type: 'count', id: '1' }],
          timeField: '@timestamp',
          bucketAggs: [
            {
              type: 'date_histogram',
              field: '@timestamp',
              id: '2',
              settings: { min_doc_count: '1', timeZone: expectedTimezone },
            },
          ],
        });

        expect(query.aggs['2'].date_histogram.time_zone).toBe(expectedTimezone);
      });

      describe('field property', () => {
        it('should use timeField from datasource when not specified', () => {
          const query = builder.build({
            refId: 'A',
            metrics: [{ type: 'count', id: '1' }],
            timeField: '@timestamp',
            bucketAggs: [
              {
                type: 'date_histogram',
                id: '2',
                settings: { min_doc_count: '1' },
              },
            ],
          });

          expect(query.aggs['2'].date_histogram.field).toBe('@timestamp');
        });

        it('should use field from bucket agg when specified', () => {
          const query = builder.build({
            refId: 'A',
            metrics: [{ type: 'count', id: '1' }],
            timeField: '@timestamp',
            bucketAggs: [
              {
                type: 'date_histogram',
                id: '2',
                field: '@time',
                settings: { min_doc_count: '1' },
              },
            ],
          });

          expect(query.aggs['2'].date_histogram.field).toBe('@time');
        });

        describe('interval parameter', () => {
          it('should use fixed_interval', () => {
            const query = builder77.build({
              refId: 'A',
              metrics: [{ type: 'count', id: '1' }],
              timeField: '@timestamp',
              bucketAggs: [
                {
                  type: 'date_histogram',
                  id: '2',
                  field: '@time',
                  settings: { min_doc_count: '1', interval: '1d' },
                },
              ],
            });

            expect(query.aggs['2'].date_histogram.fixed_interval).toBe('1d');
          });
        });

        it('should use fixed_interval if Elasticsearch version >=8.0.0', () => {
          const query = builder8.build({
            refId: 'A',
            metrics: [{ type: 'count', id: '1' }],
            timeField: '@timestamp',
            bucketAggs: [
              {
                type: 'date_histogram',
                id: '2',
                field: '@time',
                settings: { min_doc_count: '1', interval: '1d' },
              },
            ],
          });

          expect(query.aggs['2'].date_histogram.interval).toBeUndefined();
          expect(query.aggs['2'].date_histogram.fixed_interval).toBe('1d');
        });
      });
    });
  });
});

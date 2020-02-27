import angular from 'angular';
import coreModule from 'app/core/core_module';
import _ from 'lodash';
import { TemplateSrv } from 'app/features/templating/template_srv';
import DatasourceSrv from 'app/features/plugins/datasource_srv';

export class CloudWatchQueryParameterCtrl {
  /** @ngInject */
  constructor($scope: any, templateSrv: TemplateSrv, uiSegmentSrv: any, datasourceSrv: DatasourceSrv) {
    $scope.init = () => {
      const target = $scope.target;
      target.namespace = target.namespace || '';
      target.metricName = target.metricName || '';
      target.statistics = target.statistics || ['Average'];
      target.dimensions = target.dimensions || {};
      target.period = target.period || '';
      target.region = target.region || 'default';
      target.id = target.id || '';
      target.expression = target.expression || '';

      $scope.regionSegment = uiSegmentSrv.getSegmentForValue($scope.target.region, 'select region');
      $scope.namespaceSegment = uiSegmentSrv.getSegmentForValue($scope.target.namespace, 'select namespace');
      $scope.metricSegment = uiSegmentSrv.getSegmentForValue($scope.target.metricName, 'select metric');

      $scope.dimSegments = _.reduce(
        $scope.target.dimensions,
        (memo, value, key) => {
          memo.push(uiSegmentSrv.newKey(key));
          memo.push(uiSegmentSrv.newOperator('='));
          memo.push(uiSegmentSrv.newKeyValue(value));
          return memo;
        },
        []
      );

      $scope.statSegments = _.map($scope.target.statistics, stat => {
        return uiSegmentSrv.getSegmentForValue(stat);
      });

      $scope.ensurePlusButton($scope.statSegments);
      $scope.ensurePlusButton($scope.dimSegments);
      $scope.removeDimSegment = uiSegmentSrv.newSegment({
        fake: true,
        value: '-- remove dimension --',
      });
      $scope.removeStatSegment = uiSegmentSrv.newSegment({
        fake: true,
        value: '-- remove stat --',
      });

      if (_.isEmpty($scope.target.region)) {
        $scope.target.region = 'default';
      }

      if (!$scope.onChange) {
        $scope.onChange = () => {};
      }
    };

    $scope.getStatSegments = () => {
      return Promise.resolve(
        _.flatten([
          angular.copy($scope.removeStatSegment),
          _.map($scope.datasource.standardStatistics, s => {
            return uiSegmentSrv.getSegmentForValue(s);
          }),
          uiSegmentSrv.getSegmentForValue('pNN.NN'),
        ])
      );
    };

    $scope.statSegmentChanged = (segment: any, index: number) => {
      if (segment.value === $scope.removeStatSegment.value) {
        $scope.statSegments.splice(index, 1);
      } else {
        segment.type = 'value';
      }

      $scope.target.statistics = _.reduce(
        $scope.statSegments,
        (memo, seg) => {
          if (!seg.fake) {
            memo.push(seg.value);
          }
          return memo;
        },
        []
      );

      $scope.ensurePlusButton($scope.statSegments);
      $scope.onChange();
    };

    $scope.ensurePlusButton = (segments: any) => {
      const count = segments.length;
      const lastSegment = segments[Math.max(count - 1, 0)];

      if (!lastSegment || lastSegment.type !== 'plus-button') {
        segments.push(uiSegmentSrv.newPlusButton());
      }
    };

    $scope.getDimSegments = (segment: any, $index: number) => {
      if (segment.type === 'operator') {
        return Promise.resolve([]);
      }

      const target = $scope.target;
      let query = Promise.resolve([]);

      if (segment.type === 'key' || segment.type === 'plus-button') {
        query = $scope.datasource.getDimensionKeys($scope.target.namespace, $scope.target.region);
      } else if (segment.type === 'value') {
        const dimensionKey = $scope.dimSegments[$index - 2].value;
        delete target.dimensions[dimensionKey];
        query = $scope.datasource.getDimensionValues(
          target.region,
          target.namespace,
          target.metricName,
          dimensionKey,
          target.dimensions
        );
      }

      return query.then($scope.transformToSegments(true)).then(results => {
        if (segment.type === 'key') {
          results.splice(0, 0, angular.copy($scope.removeDimSegment));
        }
        return results;
      });
    };

    $scope.dimSegmentChanged = (segment: any, index: number) => {
      $scope.dimSegments[index] = segment;

      if (segment.value === $scope.removeDimSegment.value) {
        $scope.dimSegments.splice(index, 3);
      } else if (segment.type === 'plus-button') {
        $scope.dimSegments.push(uiSegmentSrv.newOperator('='));
        $scope.dimSegments.push(uiSegmentSrv.newFake('select dimension value', 'value', 'query-segment-value'));
        segment.type = 'key';
        segment.cssClass = 'query-segment-key';
      }

      $scope.syncDimSegmentsWithModel();
      $scope.ensurePlusButton($scope.dimSegments);
      $scope.onChange();
    };

    $scope.syncDimSegmentsWithModel = () => {
      const dims: any = {};
      const length = $scope.dimSegments.length;

      for (let i = 0; i < length - 2; i += 3) {
        const keySegment = $scope.dimSegments[i];
        const valueSegment = $scope.dimSegments[i + 2];
        if (!valueSegment.fake) {
          dims[keySegment.value] = valueSegment.value;
        }
      }

      $scope.target.dimensions = dims;
    };

    $scope.getRegions = () => {
      return $scope.datasource
        .metricFindQuery('regions()')
        .then((results: any) => {
          results.unshift({ text: 'default' });
          return results;
        })
        .then($scope.transformToSegments(true));
    };

    $scope.getNamespaces = () => {
      return $scope.datasource.metricFindQuery('namespaces()').then($scope.transformToSegments(true));
    };

    $scope.getMetrics = () => {
      return $scope.datasource
        .metricFindQuery('metrics(' + $scope.target.namespace + ',' + $scope.target.region + ')')
        .then($scope.transformToSegments(true));
    };

    $scope.regionChanged = () => {
      $scope.target.region = $scope.regionSegment.value;
      $scope.onChange();
    };

    $scope.namespaceChanged = () => {
      $scope.target.namespace = $scope.namespaceSegment.value;
      $scope.onChange();
    };

    $scope.metricChanged = () => {
      $scope.target.metricName = $scope.metricSegment.value;
      $scope.onChange();
    };

    $scope.transformToSegments = (addTemplateVars: any) => {
      return (results: any) => {
        const segments = _.map(results, segment => {
          return uiSegmentSrv.newSegment({
            value: segment.text,
            expandable: segment.expandable,
          });
        });

        if (addTemplateVars) {
          _.each(templateSrv.variables, variable => {
            segments.unshift(
              uiSegmentSrv.newSegment({
                type: 'template',
                value: '$' + variable.name,
                expandable: true,
              })
            );
          });
        }

        return segments;
      };
    };

    $scope.init();
  }
}

export function cloudWatchQueryParameter() {
  return {
    templateUrl: 'public/app/plugins/datasource/cloudwatch/partials/query.parameter.html',
    controller: CloudWatchQueryParameterCtrl,
    restrict: 'E',
    scope: {
      target: '=',
      datasource: '=',
      onChange: '&',
    },
  };
}

coreModule.directive('cloudwatchQueryParameter', cloudWatchQueryParameter);

import _ from 'lodash';
import { Parser } from './parser';
import { TemplateSrv } from '@grafana/runtime';
import { ScopedVars } from '@grafana/data';

export default class GraphiteQuery {
  datasource: any;
  target: any;
  functions: any[];
  segments: any[];
  tags: any[];
  error: any;
  seriesByTagUsed: boolean;
  checkOtherSegmentsIndex: number;
  removeTagValue: string;
  templateSrv: any;
  scopedVars: any;

  /** @ngInject */
  constructor(datasource: any, target: any, templateSrv?: TemplateSrv, scopedVars?: ScopedVars) {
    this.datasource = datasource;
    this.target = target;
    this.templateSrv = templateSrv;
    this.scopedVars = scopedVars;
    this.parseTarget();

    this.removeTagValue = '-- remove tag --';
  }

  parseTarget() {
    this.functions = [];
    this.segments = [];
    this.tags = [];
    this.seriesByTagUsed = false;
    this.error = null;

    if (this.target.textEditor) {
      return;
    }

    const parser = new Parser(this.target.target);
    const astNode = parser.getAst();
    if (astNode === null) {
      this.checkOtherSegmentsIndex = 0;
      return;
    }

    if (astNode.type === 'error') {
      this.error = astNode.message + ' at position: ' + astNode.pos;
      this.target.textEditor = true;
      return;
    }

    try {
      this.parseTargetRecursive(astNode, null);
    } catch (err) {
      console.error('error parsing target:', err.message);
      this.error = err.message;
      this.target.textEditor = true;
    }

    this.checkOtherSegmentsIndex = this.segments.length - 1;
  }

  getSegmentPathUpTo(index: number) {
    const arr = this.segments.slice(0, index);

    return _.reduce(
      arr,
      (result, segment) => {
        return result ? result + '.' + segment.value : segment.value;
      },
      ''
    );
  }

  parseTargetRecursive(astNode: any, func: any): any {
    if (astNode === null) {
      return null;
    }

    switch (astNode.type) {
      case 'function':
        const innerFunc = this.datasource.createFuncInstance(astNode.name, {
          withDefaultParams: false,
        });
        _.each(astNode.params, param => {
          this.parseTargetRecursive(param, innerFunc);
        });

        innerFunc.updateText();
        this.functions.push(innerFunc);

        // extract tags from seriesByTag function and hide function
        if (innerFunc.def.name === 'seriesByTag' && !this.seriesByTagUsed) {
          this.seriesByTagUsed = true;
          innerFunc.hidden = true;
          this.tags = this.splitSeriesByTagParams(innerFunc);
        }

        break;
      case 'series-ref':
        if (this.segments.length > 0 || this.getSeriesByTagFuncIndex() >= 0) {
          this.addFunctionParameter(func, astNode.value);
        } else {
          this.segments.push(astNode);
        }
        break;
      case 'bool':
      case 'string':
      case 'number':
        this.addFunctionParameter(func, astNode.value);
        break;
      case 'metric':
        if (this.segments.length || this.tags.length) {
          this.addFunctionParameter(func, _.join(_.map(astNode.segments, 'value'), '.'));
        } else {
          this.segments = astNode.segments;
        }
        break;
    }
  }

  updateSegmentValue(segment: any, index: number) {
    this.segments[index].value = segment.value;
  }

  addSelectMetricSegment() {
    this.segments.push({ value: 'select metric' });
  }

  addFunction(newFunc: any) {
    this.functions.push(newFunc);
  }

  addFunctionParameter(func: any, value: string) {
    if (func.params.length >= func.def.params.length && !_.get(_.last(func.def.params), 'multiple', false)) {
      throw { message: 'too many parameters for function ' + func.def.name };
    }
    func.params.push(value);
  }

  removeFunction(func: any) {
    this.functions = _.without(this.functions, func);
  }

  moveFunction(func: any, offset: number) {
    const index = this.functions.indexOf(func);
    // @ts-ignore
    _.move(this.functions, index, index + offset);
  }

  updateModelTarget(targets: any) {
    const wrapFunction = (target: string, func: any) => {
      return func.render(target, (value: string) => {
        return this.templateSrv.replace(value, this.scopedVars);
      });
    };

    if (!this.target.textEditor) {
      const metricPath = this.getSegmentPathUpTo(this.segments.length).replace(/\.select metric$/, '');
      this.target.target = _.reduce(this.functions, wrapFunction, metricPath);
    }

    this.updateRenderedTarget(this.target, targets);

    // loop through other queries and update targetFull as needed
    for (const target of targets || []) {
      if (target.refId !== this.target.refId) {
        this.updateRenderedTarget(target, targets);
      }
    }
  }

  updateRenderedTarget(target: { refId: string | number; target: any; targetFull: any }, targets: any) {
    // render nested query
    const targetsByRefId = _.keyBy(targets, 'refId');

    // no references to self
    delete targetsByRefId[target.refId];

    const nestedSeriesRefRegex = /\#([A-Z])/g;
    let targetWithNestedQueries = target.target;

    // Use ref count to track circular references
    function countTargetRefs(targetsByRefId: any, refId: string) {
      let refCount = 0;
      _.each(targetsByRefId, (t, id) => {
        if (id !== refId) {
          const match = nestedSeriesRefRegex.exec(t.target);
          const count = match && match.length ? match.length - 1 : 0;
          refCount += count;
        }
      });
      targetsByRefId[refId].refCount = refCount;
    }
    _.each(targetsByRefId, (t, id) => {
      countTargetRefs(targetsByRefId, id);
    });

    // Keep interpolating until there are no query references
    // The reason for the loop is that the referenced query might contain another reference to another query
    while (targetWithNestedQueries.match(nestedSeriesRefRegex)) {
      const updated = targetWithNestedQueries.replace(nestedSeriesRefRegex, (match: string, g1: string) => {
        const t = targetsByRefId[g1];
        if (!t) {
          return match;
        }

        // no circular references
        if (t.refCount === 0) {
          delete targetsByRefId[g1];
        }
        t.refCount--;

        return t.target;
      });

      if (updated === targetWithNestedQueries) {
        break;
      }

      targetWithNestedQueries = updated;
    }

    delete target.targetFull;
    if (target.target !== targetWithNestedQueries) {
      target.targetFull = targetWithNestedQueries;
    }
  }

  splitSeriesByTagParams(func: { params: any }) {
    const tagPattern = /([^\!=~]+)(\!?=~?)(.*)/;
    return _.flatten(
      _.map(func.params, (param: string) => {
        const matches = tagPattern.exec(param);
        if (matches) {
          const tag = matches.slice(1);
          if (tag.length === 3) {
            return {
              key: tag[0],
              operator: tag[1],
              value: tag[2],
            };
          }
        }
        return [];
      })
    );
  }

  getSeriesByTagFuncIndex() {
    return _.findIndex(this.functions, func => func.def.name === 'seriesByTag');
  }

  getSeriesByTagFunc() {
    const seriesByTagFuncIndex = this.getSeriesByTagFuncIndex();
    if (seriesByTagFuncIndex >= 0) {
      return this.functions[seriesByTagFuncIndex];
    } else {
      return undefined;
    }
  }

  addTag(tag: { key: any; operator: string; value: string }) {
    const newTagParam = renderTagString(tag);
    this.getSeriesByTagFunc().params.push(newTagParam);
    this.tags.push(tag);
  }

  removeTag(index: number) {
    this.getSeriesByTagFunc().params.splice(index, 1);
    this.tags.splice(index, 1);
  }

  updateTag(tag: { key: string }, tagIndex: number) {
    this.error = null;

    if (tag.key === this.removeTagValue) {
      this.removeTag(tagIndex);
      return;
    }

    const newTagParam = renderTagString(tag);
    this.getSeriesByTagFunc().params[tagIndex] = newTagParam;
    this.tags[tagIndex] = tag;
  }

  renderTagExpressions(excludeIndex = -1) {
    return _.compact(
      _.map(this.tags, (tagExpr, index) => {
        // Don't render tag that we want to lookup
        if (index !== excludeIndex) {
          return tagExpr.key + tagExpr.operator + tagExpr.value;
        }
      })
    );
  }
}

function renderTagString(tag: { key: any; operator?: any; value?: any }) {
  return tag.key + tag.operator + tag.value;
}

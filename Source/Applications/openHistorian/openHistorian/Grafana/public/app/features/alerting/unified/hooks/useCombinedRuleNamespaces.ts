import { isEqual } from 'lodash';
import { useMemo, useRef } from 'react';

import {
  CombinedRule,
  CombinedRuleGroup,
  CombinedRuleNamespace,
  Rule,
  RuleGroup,
  RuleNamespace,
  RulesSource,
} from 'app/types/unified-alerting';
import { RulerRuleDTO, RulerRuleGroupDTO, RulerRulesConfigDTO } from 'app/types/unified-alerting-dto';

import {
  getAllRulesSources,
  getRulesSourceByName,
  isCloudRulesSource,
  isGrafanaRulesSource,
} from '../utils/datasource';
import { isAlertingRule, isAlertingRulerRule, isRecordingRulerRule } from '../utils/rules';

import { useUnifiedAlertingSelector } from './useUnifiedAlertingSelector';

interface CacheValue {
  promRules?: RuleNamespace[];
  rulerRules?: RulerRulesConfigDTO | null;
  result: CombinedRuleNamespace[];
}

// this little monster combines prometheus rules and ruler rules to produce a unified data structure
// can limit to a single rules source
export function useCombinedRuleNamespaces(rulesSourceName?: string): CombinedRuleNamespace[] {
  const promRulesResponses = useUnifiedAlertingSelector((state) => state.promRules);
  const rulerRulesResponses = useUnifiedAlertingSelector((state) => state.rulerRules);

  // cache results per rules source, so we only recalculate those for which results have actually changed
  const cache = useRef<Record<string, CacheValue>>({});

  const rulesSources = useMemo((): RulesSource[] => {
    if (rulesSourceName) {
      const rulesSource = getRulesSourceByName(rulesSourceName);
      if (!rulesSource) {
        throw new Error(`Unknown rules source: ${rulesSourceName}`);
      }
      return [rulesSource];
    }
    return getAllRulesSources();
  }, [rulesSourceName]);

  return useMemo(() => {
    return rulesSources
      .map((rulesSource): CombinedRuleNamespace[] => {
        const rulesSourceName = isCloudRulesSource(rulesSource) ? rulesSource.name : rulesSource;
        const promRules = promRulesResponses[rulesSourceName]?.result;
        const rulerRules = rulerRulesResponses[rulesSourceName]?.result;

        const cached = cache.current[rulesSourceName];
        if (cached && cached.promRules === promRules && cached.rulerRules === rulerRules) {
          return cached.result;
        }
        const namespaces: Record<string, CombinedRuleNamespace> = {};

        // first get all the ruler rules in
        Object.entries(rulerRules || {}).forEach(([namespaceName, groups]) => {
          const namespace: CombinedRuleNamespace = {
            rulesSource,
            name: namespaceName,
            groups: [],
          };
          namespaces[namespaceName] = namespace;
          addRulerGroupsToCombinedNamespace(namespace, groups);
        });

        // then correlate with prometheus rules
        promRules?.forEach(({ name: namespaceName, groups }) => {
          const ns = (namespaces[namespaceName] = namespaces[namespaceName] || {
            rulesSource,
            name: namespaceName,
            groups: [],
          });

          addPromGroupsToCombinedNamespace(ns, groups);
        });

        const result = Object.values(namespaces);

        cache.current[rulesSourceName] = { promRules, rulerRules, result };
        return result;
      })
      .flat();
  }, [promRulesResponses, rulerRulesResponses, rulesSources]);
}

// merge all groups in case of grafana managed, essentially treating namespaces (folders) as groups
export function flattenGrafanaManagedRules(namespaces: CombinedRuleNamespace[]) {
  return namespaces.map((namespace) => {
    const newNamespace: CombinedRuleNamespace = {
      ...namespace,
      groups: [],
    };

    // add default group with ungrouped rules
    newNamespace.groups.push({
      name: 'default',
      rules: sortRulesByName(namespace.groups.flatMap((group) => group.rules)),
    });

    return newNamespace;
  });
}

export function sortRulesByName(rules: CombinedRule[]) {
  return rules.sort((a, b) => a.name.localeCompare(b.name));
}

function addRulerGroupsToCombinedNamespace(namespace: CombinedRuleNamespace, groups: RulerRuleGroupDTO[] = []): void {
  namespace.groups = groups.map((group) => {
    const combinedGroup: CombinedRuleGroup = {
      name: group.name,
      interval: group.interval,
      source_tenants: group.source_tenants,
      rules: [],
    };
    combinedGroup.rules = group.rules.map((rule) => rulerRuleToCombinedRule(rule, namespace, combinedGroup));
    return combinedGroup;
  });
}

function addPromGroupsToCombinedNamespace(namespace: CombinedRuleNamespace, groups: RuleGroup[]): void {
  const existingGroupsByName = new Map<string, CombinedRuleGroup>();
  namespace.groups.forEach((group) => existingGroupsByName.set(group.name, group));

  groups.forEach((group) => {
    let combinedGroup = existingGroupsByName.get(group.name);
    if (!combinedGroup) {
      combinedGroup = {
        name: group.name,
        rules: [],
      };
      namespace.groups.push(combinedGroup);
      existingGroupsByName.set(group.name, combinedGroup);
    }

    const combinedRulesByName = new Map<string, CombinedRule[]>();
    combinedGroup!.rules.forEach((r) => {
      // Prometheus rules do not have to be unique by name
      const existingRule = combinedRulesByName.get(r.name);
      existingRule ? existingRule.push(r) : combinedRulesByName.set(r.name, [r]);
    });

    (group.rules ?? []).forEach((rule) => {
      const existingRule = getExistingRuleInGroup(rule, combinedRulesByName, namespace.rulesSource);
      if (existingRule) {
        existingRule.promRule = rule;
      } else {
        combinedGroup!.rules.push(promRuleToCombinedRule(rule, namespace, combinedGroup!));
      }
    });
  });
}

function promRuleToCombinedRule(rule: Rule, namespace: CombinedRuleNamespace, group: CombinedRuleGroup): CombinedRule {
  return {
    name: rule.name,
    query: rule.query,
    labels: rule.labels || {},
    annotations: isAlertingRule(rule) ? rule.annotations || {} : {},
    promRule: rule,
    namespace: namespace,
    group,
  };
}

function rulerRuleToCombinedRule(
  rule: RulerRuleDTO,
  namespace: CombinedRuleNamespace,
  group: CombinedRuleGroup
): CombinedRule {
  return isAlertingRulerRule(rule)
    ? {
        name: rule.alert,
        query: rule.expr,
        labels: rule.labels || {},
        annotations: rule.annotations || {},
        rulerRule: rule,
        namespace,
        group,
      }
    : isRecordingRulerRule(rule)
    ? {
        name: rule.record,
        query: rule.expr,
        labels: rule.labels || {},
        annotations: {},
        rulerRule: rule,
        namespace,
        group,
      }
    : {
        name: rule.grafana_alert.title,
        query: '',
        labels: rule.labels || {},
        annotations: rule.annotations || {},
        rulerRule: rule,
        namespace,
        group,
      };
}

// find existing rule in group that matches the given prom rule
function getExistingRuleInGroup(
  rule: Rule,
  existingCombinedRulesMap: Map<string, CombinedRule[]>,
  rulesSource: RulesSource
): CombinedRule | undefined {
  // Using Map of name-based rules is important performance optimization for the code below
  // Otherwise we would perform find method multiple times on (possibly) thousands of rules

  const nameMatchingRules = existingCombinedRulesMap.get(rule.name);
  if (!nameMatchingRules) {
    return undefined;
  }

  if (isGrafanaRulesSource(rulesSource)) {
    // assume grafana groups have only the one rule. check name anyway because paranoid
    return nameMatchingRules[0];
  }

  // try finding a rule that matches name, labels, annotations and query
  const strictlyMatchingRule = nameMatchingRules.find(
    (combinedRule) => !combinedRule.promRule && isCombinedRuleEqualToPromRule(combinedRule, rule, true)
  );
  if (strictlyMatchingRule) {
    return strictlyMatchingRule;
  }

  // if that fails, try finding a rule that only matches name, labels and annotations.
  // loki & prom can sometimes modify the query so it doesnt match, eg `2 > 1` becomes `1`
  const looselyMatchingRule = nameMatchingRules.find(
    (combinedRule) => !combinedRule.promRule && isCombinedRuleEqualToPromRule(combinedRule, rule, false)
  );
  if (looselyMatchingRule) {
    return looselyMatchingRule;
  }

  return undefined;
}

function isCombinedRuleEqualToPromRule(combinedRule: CombinedRule, rule: Rule, checkQuery = true): boolean {
  if (combinedRule.name === rule.name) {
    return isEqual(
      [checkQuery ? hashQuery(combinedRule.query) : '', combinedRule.labels, combinedRule.annotations],
      [checkQuery ? hashQuery(rule.query) : '', rule.labels || {}, isAlertingRule(rule) ? rule.annotations || {} : {}]
    );
  }
  return false;
}

// there can be slight differences in how prom & ruler render a query, this will hash them accounting for the differences
function hashQuery(query: string) {
  // one of them might be wrapped in parens
  if (query.length > 1 && query[0] === '(' && query[query.length - 1] === ')') {
    query = query.slice(1, -1);
  }
  // whitespace could be added or removed
  query = query.replace(/\s|\n/g, '');
  // labels matchers can be reordered, so sort the enitre string, esentially comparing just the character counts
  return query.split('').sort().join('');
}

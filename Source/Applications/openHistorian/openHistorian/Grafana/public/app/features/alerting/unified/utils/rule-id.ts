import { CombinedRule, Rule, RuleIdentifier, RuleWithLocation } from 'app/types/unified-alerting';
import { Annotations, Labels, RulerRuleDTO } from 'app/types/unified-alerting-dto';

import { GRAFANA_RULES_SOURCE_NAME } from './datasource';
import {
  isAlertingRule,
  isAlertingRulerRule,
  isCloudRuleIdentifier,
  isGrafanaRuleIdentifier,
  isGrafanaRulerRule,
  isPrometheusRuleIdentifier,
  isRecordingRule,
  isRecordingRulerRule,
} from './rules';

export function fromRulerRule(
  ruleSourceName: string,
  namespace: string,
  groupName: string,
  rule: RulerRuleDTO
): RuleIdentifier {
  if (isGrafanaRulerRule(rule)) {
    return { uid: rule.grafana_alert.uid!, ruleSourceName: 'grafana' };
  }
  return {
    ruleSourceName,
    namespace,
    groupName,
    rulerRuleHash: hashRulerRule(rule),
  };
}

export function fromRule(ruleSourceName: string, namespace: string, groupName: string, rule: Rule): RuleIdentifier {
  return {
    ruleSourceName,
    namespace,
    groupName,
    ruleHash: hashRule(rule),
  };
}

export function fromCombinedRule(ruleSourceName: string, rule: CombinedRule): RuleIdentifier {
  const namespaceName = rule.namespace.name;
  const groupName = rule.group.name;

  if (rule.rulerRule) {
    return fromRulerRule(ruleSourceName, namespaceName, groupName, rule.rulerRule);
  }

  if (rule.promRule) {
    return fromRule(ruleSourceName, namespaceName, groupName, rule.promRule);
  }

  throw new Error('Could not create an id for a rule that is missing both `rulerRule` and `promRule`.');
}

export function fromRuleWithLocation(rule: RuleWithLocation): RuleIdentifier {
  return fromRulerRule(rule.ruleSourceName, rule.namespace, rule.group.name, rule.rule);
}

export function equal(a: RuleIdentifier, b: RuleIdentifier) {
  if (isGrafanaRuleIdentifier(a) && isGrafanaRuleIdentifier(b)) {
    return a.uid === b.uid;
  }

  if (isCloudRuleIdentifier(a) && isCloudRuleIdentifier(b)) {
    return (
      a.groupName === b.groupName &&
      a.namespace === b.namespace &&
      a.rulerRuleHash === b.rulerRuleHash &&
      a.ruleSourceName === b.ruleSourceName
    );
  }

  if (isPrometheusRuleIdentifier(a) && isPrometheusRuleIdentifier(b)) {
    return (
      a.groupName === b.groupName &&
      a.namespace === b.namespace &&
      a.ruleHash === b.ruleHash &&
      a.ruleSourceName === b.ruleSourceName
    );
  }

  return false;
}

const cloudRuleIdentifierPrefix = 'cri';
const prometheusRuleIdentifierPrefix = 'pri';

function escapeDollars(value: string): string {
  return value.replace(/\$/g, '_DOLLAR_');
}

function unesacapeDollars(value: string): string {
  return value.replace(/\_DOLLAR\_/g, '$');
}

export function parse(value: string, decodeFromUri = false): RuleIdentifier {
  const source = decodeFromUri ? decodeURIComponent(value) : value;
  const parts = source.split('$');

  if (parts.length === 1) {
    return { uid: value, ruleSourceName: 'grafana' };
  }

  if (parts.length === 5) {
    const [prefix, ruleSourceName, namespace, groupName, hash] = parts.map(unesacapeDollars);

    if (prefix === cloudRuleIdentifierPrefix) {
      return { ruleSourceName, namespace, groupName, rulerRuleHash: Number(hash) };
    }

    if (prefix === prometheusRuleIdentifierPrefix) {
      return { ruleSourceName, namespace, groupName, ruleHash: Number(hash) };
    }
  }

  throw new Error(`Failed to parse rule location: ${value}`);
}

export function tryParse(value: string | undefined, decodeFromUri = false): RuleIdentifier | undefined {
  if (!value) {
    return;
  }

  try {
    return parse(value, decodeFromUri);
  } catch (error) {
    return;
  }
}

export function stringifyIdentifier(identifier: RuleIdentifier): string {
  if (isGrafanaRuleIdentifier(identifier)) {
    return identifier.uid;
  }

  if (isCloudRuleIdentifier(identifier)) {
    return [
      cloudRuleIdentifierPrefix,
      identifier.ruleSourceName,
      identifier.namespace,
      identifier.groupName,
      identifier.rulerRuleHash,
    ]
      .map(String)
      .map(escapeDollars)
      .join('$');
  }

  return [
    prometheusRuleIdentifierPrefix,
    identifier.ruleSourceName,
    identifier.namespace,
    identifier.groupName,
    identifier.ruleHash,
  ]
    .map(String)
    .map(escapeDollars)
    .join('$');
}

function hash(value: string): number {
  let hash = 0;
  if (value.length === 0) {
    return hash;
  }
  for (var i = 0; i < value.length; i++) {
    var char = value.charCodeAt(i);
    hash = (hash << 5) - hash + char;
    hash = hash & hash; // Convert to 32bit integer
  }
  return hash;
}

// this is used to identify lotex rules, as they do not have a unique identifier
function hashRulerRule(rule: RulerRuleDTO): number {
  if (isRecordingRulerRule(rule)) {
    return hash(JSON.stringify([rule.record, rule.expr, hashLabelsOrAnnotations(rule.labels)]));
  } else if (isAlertingRulerRule(rule)) {
    return hash(
      JSON.stringify([
        rule.alert,
        rule.expr,
        hashLabelsOrAnnotations(rule.annotations),
        hashLabelsOrAnnotations(rule.labels),
      ])
    );
  } else {
    throw new Error('only recording and alerting ruler rules can be hashed');
  }
}

function hashRule(rule: Rule): number {
  if (isRecordingRule(rule)) {
    return hash(JSON.stringify([rule.type, rule.query, hashLabelsOrAnnotations(rule.labels)]));
  }

  if (isAlertingRule(rule)) {
    return hash(
      JSON.stringify([
        rule.type,
        rule.query,
        hashLabelsOrAnnotations(rule.annotations),
        hashLabelsOrAnnotations(rule.labels),
      ])
    );
  }

  throw new Error('only recording and alerting rules can be hashed');
}

function hashLabelsOrAnnotations(item: Labels | Annotations | undefined): string {
  return JSON.stringify(Object.entries(item || {}).sort((a, b) => a[0].localeCompare(b[0])));
}

export function ruleIdentifierToRuleSourceName(identifier: RuleIdentifier): string {
  return isGrafanaRuleIdentifier(identifier) ? GRAFANA_RULES_SOURCE_NAME : identifier.ruleSourceName;
}

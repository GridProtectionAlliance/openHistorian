import _ from 'lodash';

const keywords = 'by|without|on|ignoring|group_left|group_right|bool|or|and|unless';

// Duplicate from mode-prometheus.js, which can't be used in tests due to global ace not being loaded.
const builtInWords = [
  keywords,
  'count|count_values|min|max|avg|sum|stddev|stdvar|bottomk|topk|quantile',
  'true|false|null|__name__|job',
  'abs|absent|ceil|changes|clamp_max|clamp_min|count_scalar|day_of_month|day_of_week|days_in_month|delta|deriv',
  'drop_common_labels|exp|floor|histogram_quantile|holt_winters|hour|idelta|increase|irate|label_replace|ln|log2',
  'log10|minute|month|predict_linear|rate|resets|round|scalar|sort|sort_desc|sqrt|time|vector|year|avg_over_time',
  'min_over_time|max_over_time|sum_over_time|count_over_time|quantile_over_time|stddev_over_time|stdvar_over_time',
]
  .join('|')
  .split('|');

const metricNameRegexp = /([A-Za-z:][\w:]*)\b(?![\(\]{=!",])/g;
const selectorRegexp = /{([^{]*)}/g;

// addLabelToQuery('foo', 'bar', 'baz') => 'foo{bar="baz"}'
export function addLabelToQuery(query: string, key: string, value: string, operator?: string): string {
  if (!key || !value) {
    throw new Error('Need label to add to query.');
  }

  // Add empty selectors to bare metric names
  let previousWord: string;
  query = query.replace(metricNameRegexp, (match, word, offset) => {
    const insideSelector = isPositionInsideChars(query, offset, '{', '}');
    // Handle "sum by (key) (metric)"
    const previousWordIsKeyWord = previousWord && keywords.split('|').indexOf(previousWord) > -1;

    // check for colon as as "word boundary" symbol
    const isColonBounded = word.endsWith(':');

    previousWord = word;
    if (!insideSelector && !isColonBounded && !previousWordIsKeyWord && builtInWords.indexOf(word) === -1) {
      return `${word}{}`;
    }
    return word;
  });

  // Adding label to existing selectors
  let match = selectorRegexp.exec(query);
  const parts = [];
  let lastIndex = 0;
  let suffix = '';

  while (match) {
    const prefix = query.slice(lastIndex, match.index);
    const selector = match[1];
    const selectorWithLabel = addLabelToSelector(selector, key, value, operator);
    lastIndex = match.index + match[1].length + 2;
    suffix = query.slice(match.index + match[0].length);
    parts.push(prefix, selectorWithLabel);
    match = selectorRegexp.exec(query);
  }

  parts.push(suffix);
  return parts.join('');
}

const labelRegexp = /(\w+)\s*(=|!=|=~|!~)\s*("[^"]*")/g;

export function addLabelToSelector(selector: string, labelKey: string, labelValue: string, labelOperator?: string) {
  const parsedLabels = [];

  // Split selector into labels
  if (selector) {
    let match = labelRegexp.exec(selector);
    while (match) {
      parsedLabels.push({ key: match[1], operator: match[2], value: match[3] });
      match = labelRegexp.exec(selector);
    }
  }

  // Add new label
  const operatorForLabelKey = labelOperator || '=';
  parsedLabels.push({ key: labelKey, operator: operatorForLabelKey, value: `"${labelValue}"` });

  // Sort labels by key and put them together
  const formatted = _.chain(parsedLabels)
    .uniqWith(_.isEqual)
    .compact()
    .sortBy('key')
    .map(({ key, operator, value }) => `${key}${operator}${value}`)
    .value()
    .join(',');

  return `{${formatted}}`;
}

export function keepSelectorFilters(selector: string) {
  // Remove all label-key between {} and return filters. If first character is space, remove it.
  const filters = selector.replace(/\{(.*?)\}/g, '').replace(/^ /, '');
  return filters;
}

function isPositionInsideChars(text: string, position: number, openChar: string, closeChar: string) {
  const nextSelectorStart = text.slice(position).indexOf(openChar);
  const nextSelectorEnd = text.slice(position).indexOf(closeChar);
  return nextSelectorEnd > -1 && (nextSelectorStart === -1 || nextSelectorStart > nextSelectorEnd);
}

export default addLabelToQuery;

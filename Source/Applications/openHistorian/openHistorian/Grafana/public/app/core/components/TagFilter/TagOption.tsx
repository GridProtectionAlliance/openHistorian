import { css, cx } from '@emotion/css';
import React, { FC } from 'react';
import { OptionProps } from 'react-select';

import { GrafanaTheme } from '@grafana/data';
import { useTheme, stylesFactory } from '@grafana/ui';

import { TagBadge } from './TagBadge';

// https://github.com/JedWatson/react-select/issues/3038
interface ExtendedOptionProps extends OptionProps<any, any> {
  data: any;
}

export const TagOption: FC<ExtendedOptionProps> = ({ data, className, label, isFocused, innerProps }) => {
  const theme = useTheme();
  const styles = getStyles(theme);

  return (
    <div className={cx(styles.option, isFocused && styles.optionFocused)} aria-label="Tag option" {...innerProps}>
      <div className={`tag-filter-option ${className || ''}`}>
        {typeof label === 'string' ? <TagBadge label={label} removeIcon={false} count={data.count ?? 0} /> : label}
      </div>
    </div>
  );
};

const getStyles = stylesFactory((theme: GrafanaTheme) => {
  return {
    option: css`
      padding: 8px;
      white-space: nowrap;
      cursor: pointer;
      border-left: 2px solid transparent;
      &:hover {
        background: ${theme.colors.dropdownOptionHoverBg};
      }
    `,
    optionFocused: css`
      background: ${theme.colors.dropdownOptionHoverBg};
      border-style: solid;
      border-top: 0;
      border-right: 0;
      border-bottom: 0;
      border-left-width: 2px;
    `,
  };
});

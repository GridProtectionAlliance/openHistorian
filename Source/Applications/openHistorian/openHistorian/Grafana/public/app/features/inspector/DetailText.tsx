import { css } from '@emotion/css';
import React, { FC } from 'react';

import { GrafanaTheme } from '@grafana/data';
import { useStyles } from '@grafana/ui';

const getStyles = (theme: GrafanaTheme) => css`
  margin: 0;
  margin-left: ${theme.spacing.md};
  font-size: ${theme.typography.size.sm};
  color: ${theme.colors.textWeak};
`;

export const DetailText: FC = ({ children }) => {
  const collapsedTextStyles = useStyles(getStyles);
  return <p className={collapsedTextStyles}>{children}</p>;
};

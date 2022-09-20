import { css, cx } from '@emotion/css';
import React, { useState } from 'react';

import { GrafanaTheme } from '@grafana/data';
import { Icon, InfoBox, stylesFactory, useTheme } from '@grafana/ui';

const getStyles = stylesFactory((theme: GrafanaTheme) => ({
  infoBox: css`
    margin-top: ${theme.spacing.xs};
  `,
}));

export const HelpToggle: React.FunctionComponent = ({ children }) => {
  const [isHelpVisible, setIsHelpVisible] = useState(false);
  const theme = useTheme();
  const styles = getStyles(theme);

  return (
    <>
      <button className="gf-form-label query-keyword pointer" onClick={(_) => setIsHelpVisible(!isHelpVisible)}>
        Help
        <Icon name={isHelpVisible ? 'angle-down' : 'angle-right'} />
      </button>
      {isHelpVisible && <InfoBox className={cx(styles.infoBox)}>{children}</InfoBox>}
    </>
  );
};

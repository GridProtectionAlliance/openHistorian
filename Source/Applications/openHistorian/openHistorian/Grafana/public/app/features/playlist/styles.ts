import { css } from '@emotion/css';

import { GrafanaTheme2 } from '@grafana/data';

export function getPlaylistStyles(theme: GrafanaTheme2) {
  return {
    description: css`
      label: description;
      width: 555px;
      margin-bottom: 20px;
    `,
    subHeading: css`
      label: sub-heading;
      margin-bottom: ${theme.spacing(2)};
    `,
  };
}

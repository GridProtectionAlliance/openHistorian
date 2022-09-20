import { css } from '@emotion/css';
import React, { CSSProperties, PureComponent } from 'react';

import { GrafanaTheme } from '@grafana/data';
import { config } from '@grafana/runtime';
import { stylesFactory } from '@grafana/ui';

export interface OverlayProps {
  topRight?: React.ReactNode[];
  bottomLeft?: React.ReactNode[];
  blStyle?: CSSProperties;
}

export class GeomapOverlay extends PureComponent<OverlayProps> {
  style = getStyles(config.theme);

  constructor(props: OverlayProps) {
    super(props);
  }

  render() {
    const { topRight, bottomLeft } = this.props;
    return (
      <div className={this.style.overlay}>
        {Boolean(topRight?.length) && <div className={this.style.TR}>{topRight}</div>}
        {Boolean(bottomLeft?.length) && (
          <div className={this.style.BL} style={this.props.blStyle}>
            {bottomLeft}
          </div>
        )}
      </div>
    );
  }
}

const getStyles = stylesFactory((theme: GrafanaTheme) => ({
  overlay: css`
    position: absolute;
    width: 100%;
    height: 100%;
    z-index: 500;
    pointer-events: none;
  `,
  TR: css`
    position: absolute;
    top: 8px;
    right: 8px;
    pointer-events: auto;
  `,
  BL: css`
    position: absolute;
    bottom: 8px;
    left: 8px;
    pointer-events: auto;
  `,
}));

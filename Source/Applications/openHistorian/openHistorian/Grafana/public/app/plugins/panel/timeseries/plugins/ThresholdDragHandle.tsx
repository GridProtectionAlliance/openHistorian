import { css } from '@emotion/css';
import React, { useMemo, useState } from 'react';
import Draggable, { DraggableBounds } from 'react-draggable';

import { Threshold, GrafanaTheme2 } from '@grafana/data';
import { useStyles2, useTheme2 } from '@grafana/ui';

type OutOfBounds = 'top' | 'bottom' | 'none';

interface ThresholdDragHandleProps {
  step: Threshold;
  y: number;
  dragBounds: DraggableBounds;
  mapPositionToValue: (y: number) => number;
  onChange: (value: number) => void;
  formatValue: (value: number) => string;
}

export const ThresholdDragHandle: React.FC<ThresholdDragHandleProps> = ({
  step,
  y,
  dragBounds,
  mapPositionToValue,
  formatValue,
  onChange,
}) => {
  const theme = useTheme2();
  let yPos = y;
  let outOfBounds: OutOfBounds = 'none';

  if (y < (dragBounds.top ?? 0)) {
    outOfBounds = 'top';
  }

  // there seems to be a 22px offset at the bottom where the threshold line is still drawn
  // this is probably offset by the size of the x-axis component
  if (y > (dragBounds.bottom ?? 0) + 22) {
    outOfBounds = 'bottom';
  }

  if (outOfBounds === 'bottom') {
    yPos = dragBounds.bottom ?? y;
  }

  if (outOfBounds === 'top') {
    yPos = dragBounds.top ?? y;
  }

  const styles = useStyles2((theme) => getStyles(theme, step, outOfBounds));
  const [currentValue, setCurrentValue] = useState(step.value);

  const textColor = useMemo(() => {
    return theme.colors.getContrastText(theme.visualization.getColorByName(step.color));
  }, [step.color, theme]);

  return (
    <Draggable
      axis="y"
      grid={[1, 1]}
      onStop={(_e, d) => {
        onChange(mapPositionToValue(d.lastY));
        // as of https://github.com/react-grid-layout/react-draggable/issues/390#issuecomment-623237835
        return false;
      }}
      onDrag={(_e, d) => setCurrentValue(mapPositionToValue(d.lastY))}
      position={{ x: 0, y: yPos }}
      bounds={dragBounds}
    >
      <div className={styles.handle} style={{ color: textColor }}>
        <span className={styles.handleText}>{formatValue(currentValue)}</span>
      </div>
    </Draggable>
  );
};

ThresholdDragHandle.displayName = 'ThresholdDragHandle';

const getStyles = (theme: GrafanaTheme2, step: Threshold, outOfBounds: OutOfBounds) => {
  const mainColor = theme.visualization.getColorByName(step.color);
  const arrowStyles = getArrowStyles(outOfBounds);
  const isOutOfBounds = outOfBounds !== 'none';

  return {
    handle: css`
      display: flex;
      align-items: center;
      position: absolute;
      left: 0;
      width: calc(100% - 9px);
      height: 18px;
      margin-top: -9px;
      border-color: ${mainColor};
      cursor: grab;
      border-top-right-radius: ${theme.shape.borderRadius(1)};
      border-bottom-right-radius: ${theme.shape.borderRadius(1)};
      ${isOutOfBounds &&
      css`
        margin-top: 0;
        border-radius: ${theme.shape.borderRadius(1)};
      `}
      background: ${mainColor};
      font-size: ${theme.typography.bodySmall.fontSize};
      &:before {
        ${arrowStyles};
      }
    `,
    handleText: css`
      text-align: center;
      width: 100%;
      display: block;
      text-overflow: ellipsis;
      white-space: nowrap;
      overflow: hidden;
    `,
  };
};

function getArrowStyles(outOfBounds: OutOfBounds) {
  const inBounds = outOfBounds === 'none';

  const triangle = (size: number) => css`
    content: '';
    position: absolute;

    bottom: 0;
    top: 0;
    width: 0;
    height: 0;
    left: 0;

    border-right-style: solid;
    border-right-width: ${size}px;
    border-right-color: inherit;
    border-top: ${size}px solid transparent;
    border-bottom: ${size}px solid transparent;
  `;

  if (inBounds) {
    return css`
      ${triangle(9)};
      left: -9px;
    `;
  }

  if (outOfBounds === 'top') {
    return css`
      ${triangle(5)};
      left: calc(50% - 2.5px);
      top: -7px;
      transform: rotate(90deg);
    `;
  }

  if (outOfBounds === 'bottom') {
    return css`
      ${triangle(5)};
      left: calc(50% - 2.5px);
      top: calc(100% - 2.5px);
      transform: rotate(-90deg);
    `;
  }

  return '';
}

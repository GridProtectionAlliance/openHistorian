import { css } from '@emotion/css';
import memoizeOne from 'memoize-one';
import tinycolor from 'tinycolor2';

import { colorManipulator, GrafanaTheme2, LogLevel } from '@grafana/data';
import { styleMixins } from '@grafana/ui';

export const getLogLevelStyles = (theme: GrafanaTheme2, logLevel?: LogLevel) => {
  let logColor = theme.isLight ? theme.v1.palette.gray5 : theme.v1.palette.gray2;
  switch (logLevel) {
    case LogLevel.crit:
    case LogLevel.critical:
      logColor = '#705da0';
      break;
    case LogLevel.error:
    case LogLevel.err:
      logColor = '#e24d42';
      break;
    case LogLevel.warning:
    case LogLevel.warn:
      logColor = theme.colors.warning.main;
      break;
    case LogLevel.info:
      logColor = '#7eb26d';
      break;
    case LogLevel.debug:
      logColor = '#1f78c1';
      break;
    case LogLevel.trace:
      logColor = '#6ed0e0';
      break;
  }

  return {
    logsRowLevelColor: css`
      &::after {
        background-color: ${logColor};
      }
    `,
  };
};

export const getLogRowStyles = memoizeOne((theme: GrafanaTheme2) => {
  const hoverBgColor = styleMixins.hoverColor(theme.colors.background.secondary, theme);
  const contextOutlineColor = tinycolor(theme.components.dashboard.background).setAlpha(0.7).toRgbString();
  return {
    logsRowLevel: css`
      label: logs-row__level;
      max-width: ${theme.spacing(1.25)};
      cursor: default;
      &::after {
        content: '';
        display: block;
        position: absolute;
        top: 1px;
        bottom: 1px;
        width: 3px;
        left: ${theme.spacing(0.5)};
      }
    `,
    // Compared to logsRowLevel we need to make error logs wider to accommodate the icon
    logsRowWithError: css({
      maxWidth: `${theme.spacing(1.5)}`,
    }),
    logsRowMatchHighLight: css`
      label: logs-row__match-highlight;
      background: inherit;
      padding: inherit;
      color: ${theme.components.textHighlight.text}
      background-color: ${theme.components.textHighlight};
    `,
    logRows: css({
      position: 'relative',
    }),
    logsRowsTable: css`
      label: logs-rows;
      font-family: ${theme.typography.fontFamilyMonospace};
      font-size: ${theme.typography.bodySmall.fontSize};
      width: 100%;
      position: relative;
    `,
    logsRowsTableContain: css`
      contain: strict;
    `,
    highlightBackground: css`
      background-color: ${tinycolor(theme.colors.info.transparent).setAlpha(0.25).toString()};
    `,
    logsRow: css`
      label: logs-row;
      width: 100%;
      cursor: pointer;
      vertical-align: top;

      &:hover {
        .log-row-menu {
          z-index: 1;
        }

        background: ${hoverBgColor};
      }

      td:not(.log-row-menu-cell):last-child {
        width: 100%;
      }

      > td:not(.log-row-menu-cell) {
        position: relative;
        padding-right: ${theme.spacing(1)};
        border-top: 1px solid transparent;
        border-bottom: 1px solid transparent;
        height: 100%;
      }
    `,
    logsRowDuplicates: css`
      label: logs-row__duplicates;
      text-align: right;
      width: 4em;
      cursor: default;
    `,
    logIconError: css({
      color: theme.colors.warning.main,
      position: 'relative',
      top: '-2px',
    }),
    logIconInfo: css({
      color: theme.colors.info.main,
      position: 'relative',
      top: '-2px',
    }),
    logsRowToggleDetails: css`
      label: logs-row-toggle-details__level;
      font-size: 9px;
      padding-top: 5px;
      max-width: 15px;
    `,
    logsRowLocalTime: css`
      label: logs-row__localtime;
      white-space: nowrap;
    `,
    logsRowLabels: css`
      label: logs-row__labels;
      white-space: nowrap;
      max-width: 22em;

      /* This is to make the labels vertical align */
      > span {
        margin-top: 0.75px;
      }
    `,
    logsRowMessage: css`
      label: logs-row__message;
      white-space: pre-wrap;
      word-break: break-all;
      overflow-wrap: anywhere;
      width: 100%;
      text-align: left;
    `,
    copyLogButton: css`
      padding: 0 0 0 ${theme.spacing(0.5)};
      height: ${theme.spacing(3)};
      width: ${theme.spacing(3.25)};
      line-height: ${theme.spacing(2.5)};
      overflow: hidden;
      &:hover {
          background-color: ${colorManipulator.alpha(theme.colors.text.primary, 0.12)};
        }
      }
    `,
    //Log details specific CSS
    logDetailsContainer: css`
      label: logs-row-details-table;
      border: 1px solid ${theme.colors.border.medium};
      padding: 0 ${theme.spacing(1)} ${theme.spacing(1)};
      border-radius: ${theme.shape.radius.default};
      margin: ${theme.spacing(2.5)} ${theme.spacing(1)} ${theme.spacing(2.5)} ${theme.spacing(2)};
      cursor: default;
    `,
    logDetailsTable: css`
      label: logs-row-details-table;
      line-height: 18px;
      width: 100%;
      td:last-child {
        width: 100%;
      }
    `,
    logsDetailsIcon: css`
      label: logs-row-details__icon;
      position: relative;
      color: ${theme.v1.palette.gray3};
      padding-top: 1px;
      padding-bottom: 1px;
      padding-right: ${theme.spacing(0.75)};
    `,
    logDetailsLabel: css`
      label: logs-row-details__label;
      max-width: 30em;
      min-width: 20em;
      padding: 0 ${theme.spacing(1)};
      overflow-wrap: break-word;
    `,
    logDetailsHeading: css`
      label: logs-row-details__heading;
      font-weight: ${theme.typography.fontWeightBold};
      padding: ${theme.spacing(1)} 0 ${theme.spacing(0.5)};
    `,
    logDetailsValue: css`
      label: logs-row-details__row;
      position: relative;
      vertical-align: middle;
      cursor: default;

      &:hover {
        background-color: ${hoverBgColor};
      }
    `,
    // Log row
    topVerticalAlign: css`
      label: topVerticalAlign;
      margin-top: -${theme.spacing(0.9)};
      margin-left: -${theme.spacing(0.25)};
    `,
    detailsOpen: css`
      &:hover {
        background-color: ${styleMixins.hoverColor(theme.colors.background.primary, theme)};
      }
    `,
    errorLogRow: css`
      label: erroredLogRow;
      color: ${theme.colors.text.secondary};
    `,
    // Log Row Message
    positionRelative: css`
      label: positionRelative;
      position: relative;
    `,
    rowWithContext: css`
      label: rowWithContext;
      z-index: 1;
      outline: 9999px solid ${contextOutlineColor};
      display: inherit;
    `,
    horizontalScroll: css`
      label: horizontalScroll;
      white-space: pre;
    `,
    contextNewline: css`
      display: block;
      margin-left: 0px;
    `,
    rowMenu: css`
      label: rowMenu;
      display: flex;
      flex-wrap: nowrap;
      flex-direction: row;
      align-content: flex-end;
      justify-content: space-evenly;
      align-items: center;
      position: absolute;
      top: 0;
      bottom: auto;
      background: ${theme.colors.background.primary};
      box-shadow: ${theme.shadows.z3};
      padding: ${theme.spacing(0.5, 1, 0.5, 1)};
      z-index: 100;
      gap: ${theme.spacing(0.5)};

      & > button {
        margin: 0;
      }
    `,
    logRowMenuCell: css`
      position: sticky;
      z-index: ${theme.zIndex.dropdown};
      margin-top: -${theme.spacing(0.125)};
      right: 0px;

      & > span {
        transform: translateX(-100%);
      }
    `,
    logLine: css`
      background-color: transparent;
      border: none;
      diplay: inline;
      font-family: ${theme.typography.fontFamilyMonospace};
      font-size: ${theme.typography.bodySmall.fontSize};
      letter-spacing: ${theme.typography.bodySmall.letterSpacing};
      text-align: left;
      padding: 0;
      user-select: text;
    `,
    // Log details
    logsRowLevelDetails: css`
      label: logs-row__level_details;
      &::after {
        top: -3px;
      }
    `,
    logDetails: css`
      label: logDetailsDefaultCursor;
      cursor: default;

      &:hover {
        background-color: ${theme.colors.background.primary};
      }
    `,
    visibleRowMenu: css`
      label: visibleRowMenu;
      aspect-ratio: 1/1;
      z-index: 90;
    `,
    linkButton: css`
      label: linkButton;
      > button {
        padding-top: ${theme.spacing(0.5)};
      }
    `,
    hidden: css`
      label: hidden;
      visibility: hidden;
    `,
    unPinButton: css`
      height: ${theme.spacing(3)};
      line-height: ${theme.spacing(2.5)};
    `,
  };
});

export type LogRowStyles = ReturnType<typeof getLogRowStyles>;

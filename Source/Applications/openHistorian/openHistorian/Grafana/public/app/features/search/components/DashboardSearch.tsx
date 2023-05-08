import { css } from '@emotion/css';
import React, { useEffect } from 'react';

import { GrafanaTheme2 } from '@grafana/data';
import { IconButton, stylesFactory, useStyles2 } from '@grafana/ui';

import { useKeyNavigationListener } from '../hooks/useSearchKeyboardSelection';
import { SearchView } from '../page/components/SearchView';
import { getSearchStateManager } from '../state/SearchStateManager';

export interface Props {}

export function DashboardSearch({}: Props) {
  const styles = useStyles2(getStyles);
  const stateManager = getSearchStateManager();
  const state = stateManager.useState();

  useEffect(() => stateManager.initStateFromUrl(), [stateManager]);

  const { onKeyDown, keyboardEvents } = useKeyNavigationListener();

  return (
    <div className={styles.overlay}>
      <div className={styles.container}>
        <div className={styles.searchField}>
          <div>
            <input
              type="text"
              // eslint-disable-next-line jsx-a11y/no-autofocus
              autoFocus
              placeholder={state.includePanels ? 'Search dashboards and panels by name' : 'Search dashboards by name'}
              value={state.query ?? ''}
              onChange={(e) => stateManager.onQueryChange(e.currentTarget.value)}
              onKeyDown={onKeyDown}
              spellCheck={false}
              className={styles.input}
            />
          </div>

          <div className={styles.closeBtn}>
            <IconButton name="times" onClick={stateManager.onCloseSearch} size="xxl" tooltip="Close search" />
          </div>
        </div>
        <div className={styles.search}>
          <SearchView showManage={false} keyboardEvents={keyboardEvents} />
        </div>
      </div>
    </div>
  );
}

const getStyles = stylesFactory((theme: GrafanaTheme2) => {
  return {
    overlay: css`
      left: 0;
      top: 0;
      right: 0;
      bottom: 0;
      z-index: ${theme.zIndex.sidemenu};
      position: fixed;
      background: ${theme.colors.background.canvas};
      padding: ${theme.spacing(1)};

      ${theme.breakpoints.up('md')} {
        left: ${theme.components.sidemenu.width}px;
        z-index: ${theme.zIndex.navbarFixed + 1};
        padding: ${theme.spacing(2)};
      }
    `,
    container: css`
      display: flex;
      flex-direction: column;
      max-width: 1400px;
      margin: 0 auto;
      padding: ${theme.spacing(1)};
      background: ${theme.colors.background.primary};
      border: 1px solid ${theme.components.panel.borderColor};
      height: 100%;

      ${theme.breakpoints.up('md')} {
        padding: ${theme.spacing(3)};
      }
    `,
    closeBtn: css`
      right: -5px;
      top: 2px;
      z-index: 1;
      position: absolute;
    `,
    searchField: css`
      position: relative;
    `,
    search: css`
      display: flex;
      flex-direction: column;
      overflow: hidden;
      height: 100%;
      padding: ${theme.spacing(2, 0, 3, 0)};
    `,
    input: css`
      box-sizing: border-box;
      outline: none;
      background-color: transparent;
      background: transparent;
      border-bottom: 2px solid ${theme.v1.colors.border1};
      font-size: 20px;
      line-height: 38px;
      width: 100%;

      &::placeholder {
        color: ${theme.v1.colors.textWeak};
      }
    `,
  };
});

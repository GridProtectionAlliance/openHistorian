import { css, cx } from '@emotion/css';
import React, { PropsWithChildren } from 'react';

import { GrafanaTheme2 } from '@grafana/data';
import { config } from '@grafana/runtime';
import { useStyles2 } from '@grafana/ui';
import { useGrafana } from 'app/core/context/GrafanaContext';

import { MegaMenu } from '../MegaMenu/MegaMenu';

import { NavToolbar } from './NavToolbar';
import { TopSearchBar } from './TopSearchBar';
import { TOP_BAR_LEVEL_HEIGHT } from './types';

export interface Props extends PropsWithChildren<{}> {}

export function AppChrome({ children }: Props) {
  const styles = useStyles2(getStyles);
  const { chrome } = useGrafana();
  const state = chrome.useState();

  if (state.chromeless || !config.featureToggles.topnav) {
    return <main className="main-view">{children} </main>;
  }

  return (
    <main className="main-view">
      <div className={styles.topNav}>
        {!state.searchBarHidden && <TopSearchBar />}
        <NavToolbar
          searchBarHidden={state.searchBarHidden}
          sectionNav={state.sectionNav}
          pageNav={state.pageNav}
          actions={state.actions}
          onToggleSearchBar={chrome.toggleSearchBar}
          onToggleMegaMenu={chrome.toggleMegaMenu}
        />
      </div>
      <div className={cx(styles.content, state.searchBarHidden && styles.contentNoSearchBar)}>{children}</div>
      {state.megaMenuOpen && <MegaMenu searchBarHidden={state.searchBarHidden} onClose={chrome.toggleMegaMenu} />}
    </main>
  );
}

const getStyles = (theme: GrafanaTheme2) => {
  const shadow = theme.isDark
    ? `0 0.6px 1.5px rgb(0 0 0), 0 2px 4px rgb(0 0 0 / 40%), 0 5px 10px rgb(0 0 0 / 23%)`
    : '0 0.6px 1.5px rgb(0 0 0 / 8%), 0 2px 4px rgb(0 0 0 / 6%), 0 5px 10px rgb(0 0 0 / 5%)';

  return {
    content: css({
      display: 'flex',
      flexDirection: 'column',
      paddingTop: TOP_BAR_LEVEL_HEIGHT * 2,
      flexGrow: 1,
      height: '100%',
    }),
    contentNoSearchBar: css({
      paddingTop: TOP_BAR_LEVEL_HEIGHT,
    }),
    topNav: css({
      display: 'flex',
      position: 'fixed',
      zIndex: theme.zIndex.navbarFixed,
      left: 0,
      right: 0,
      boxShadow: shadow,
      background: theme.colors.background.primary,
      flexDirection: 'column',
    }),
  };
};

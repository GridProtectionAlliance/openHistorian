import { css } from '@emotion/css';
import React from 'react';

import { NavModel, GrafanaTheme2 } from '@grafana/data';
import { IconName, useStyles2, Icon, VerticalTab } from '@grafana/ui';

export interface Props {
  model: NavModel;
}

export function SectionNav(props: Props) {
  const styles = useStyles2(getStyles);

  const main = props.model.main;
  const directChildren = props.model.main.children!.filter((x) => !x.hideFromTabs && !x.children);
  const nestedItems = props.model.main.children!.filter((x) => x.children && x.children.length);

  return (
    <nav className={styles.nav}>
      <h2 className={styles.sectionName}>
        {main.icon && <Icon name={main.icon as IconName} size="lg" />}
        {main.img && <img className={styles.sectionImg} src={main.img} alt={`logo of ${main.text}`} />}
        {props.model.main.text}
      </h2>
      <div className={styles.items}>
        {directChildren.map((child, index) => {
          return (
            !child.hideFromTabs &&
            !child.children && (
              <VerticalTab
                label={child.text}
                active={child.active}
                key={`${child.url}-${index}`}
                // icon={child.icon as IconName}
                href={child.url}
              />
            )
          );
        })}
        {nestedItems.map((child) => (
          <>
            <div className={styles.subSection}>{child.text}</div>
            {child.children!.map((child, index) => {
              return (
                !child.hideFromTabs &&
                !child.children && (
                  <VerticalTab
                    label={child.text}
                    active={child.active}
                    key={`${child.url}-${index}`}
                    // icon={child.icon as IconName}
                    href={child.url}
                  />
                )
              );
            })}
          </>
        ))}
      </div>
    </nav>
  );
}

const getStyles = (theme: GrafanaTheme2) => {
  return {
    nav: css({
      display: 'flex',
      flexDirection: 'column',
      background: theme.colors.background.canvas,
      padding: theme.spacing(3, 2),
      flexShrink: 0,
      [theme.breakpoints.up('md')]: {
        width: '250px',
      },
    }),
    sectionName: css({
      display: 'flex',
      alignItems: 'center',
      gap: theme.spacing(1),
      padding: theme.spacing(0.5, 0, 3, 0.25),
      fontSize: theme.typography.h4.fontSize,
      margin: 0,
    }),
    items: css({
      // paddingLeft: '9px',
    }),
    sectionImg: css({
      height: 48,
    }),
    subSection: css({
      padding: theme.spacing(3, 0, 0.5, 1),
      fontWeight: 500,
    }),
  };
};

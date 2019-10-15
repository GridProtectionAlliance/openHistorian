import React, { FC } from 'react';
import { PanelMenuItem } from '@grafana/ui';

interface Props {
  children: any;
}

export const PanelHeaderMenuItem: FC<Props & PanelMenuItem> = props => {
  const isSubMenu = props.type === 'submenu';
  const isDivider = props.type === 'divider';
  return isDivider ? (
    <li className="divider" />
  ) : (
    <li className={isSubMenu ? 'dropdown-submenu' : null}>
      <a onClick={props.onClick}>
        {props.iconClassName && <i className={props.iconClassName} />}
        <span className="dropdown-item-text" aria-label={`${props.text} panel menu item`}>
          {props.text}
        </span>
        {props.shortcut && <span className="dropdown-menu-item-shortcut">{props.shortcut}</span>}
      </a>
      {props.children}
    </li>
  );
};

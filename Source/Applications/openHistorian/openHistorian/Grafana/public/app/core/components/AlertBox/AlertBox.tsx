import React, { FunctionComponent, ReactNode } from 'react';
import classNames from 'classnames';
import { AppNotificationSeverity } from 'app/types';

interface Props {
  title: string;
  icon?: string;
  body?: ReactNode;
  severity: AppNotificationSeverity;
  onClose?: () => void;
}

function getIconFromSeverity(severity: AppNotificationSeverity): string {
  switch (severity) {
    case AppNotificationSeverity.Error: {
      return 'fa fa-exclamation-triangle';
    }
    case AppNotificationSeverity.Warning: {
      return 'fa fa-exclamation-triangle';
    }
    case AppNotificationSeverity.Info: {
      return 'fa fa-info-circle';
    }
    case AppNotificationSeverity.Success: {
      return 'fa fa-check';
    }
    default:
      return '';
  }
}

export const AlertBox: FunctionComponent<Props> = ({ title, icon, body, severity, onClose }) => {
  const alertClass = classNames('alert', `alert-${severity}`);
  return (
    <div className={alertClass}>
      <div className="alert-icon">
        <i className={icon || getIconFromSeverity(severity)} />
      </div>
      <div className="alert-body">
        <div className="alert-title">{title}</div>
        {body && <div className="alert-text">{body}</div>}
      </div>
      {onClose && (
        <button type="button" className="alert-close" onClick={onClose}>
          <i className="fa fa fa-remove" />
        </button>
      )}
    </div>
  );
};

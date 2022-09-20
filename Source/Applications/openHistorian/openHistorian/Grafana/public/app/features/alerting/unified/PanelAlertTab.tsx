import React, { FC } from 'react';

import { Tab, TabProps } from '@grafana/ui/src/components/Tabs/Tab';
import { DashboardModel, PanelModel } from 'app/features/dashboard/state';

import { usePanelCombinedRules } from './hooks/usePanelCombinedRules';

interface Props extends Omit<TabProps, 'counter' | 'ref'> {
  panel: PanelModel;
  dashboard: DashboardModel;
}

// it will load rule count from backend
export const PanelAlertTab: FC<Props> = ({ panel, dashboard, ...otherProps }) => {
  const { rules, loading } = usePanelCombinedRules({ panel, dashboard });
  return <Tab {...otherProps} counter={loading ? null : rules.length} />;
};

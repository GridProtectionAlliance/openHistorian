// Libraries
import React, { PureComponent } from 'react';
import { connect, MapStateToProps, MapDispatchToProps } from 'react-redux';

// Utils & Services
import { AngularComponent, getAngularLoader } from '@grafana/runtime';

// Types
import { PanelModel, DashboardModel } from '../../state';
import { PanelPlugin, PanelPluginMeta } from '@grafana/data';
import { PanelCtrl } from 'app/plugins/sdk';
import { changePanelPlugin } from '../../state/actions';
import { StoreState } from 'app/types';
import { getSectionOpenState, saveSectionOpenState } from './state/utils';

interface OwnProps {
  panel: PanelModel;
  dashboard: DashboardModel;
  plugin: PanelPlugin;
}

interface ConnectedProps {
  angularPanelComponent?: AngularComponent | null;
}

interface DispatchProps {
  changePanelPlugin: typeof changePanelPlugin;
}

type Props = OwnProps & ConnectedProps & DispatchProps;

export class AngularPanelOptionsUnconnected extends PureComponent<Props> {
  element?: HTMLElement | null;
  angularOptions?: AngularComponent | null;

  constructor(props: Props) {
    super(props);
  }

  componentDidMount() {
    this.loadAngularOptions();
  }

  componentDidUpdate(prevProps: Props) {
    if (this.props.plugin !== prevProps.plugin) {
      this.cleanUpAngularOptions();
    }

    this.loadAngularOptions();
  }

  componentWillUnmount() {
    this.cleanUpAngularOptions();
  }

  cleanUpAngularOptions() {
    if (this.angularOptions) {
      this.angularOptions.destroy();
      this.angularOptions = null;
    }
  }

  loadAngularOptions() {
    const { panel, angularPanelComponent, changePanelPlugin } = this.props;

    if (!this.element || !angularPanelComponent || this.angularOptions) {
      return;
    }

    const scope = angularPanelComponent.getScope();

    // When full page reloading in edit mode the angular panel has on fully compiled & instantiated yet
    if (!scope.$$childHead) {
      setTimeout(() => {
        this.forceUpdate();
      });
      return;
    }

    const panelCtrl: PanelCtrl = scope.$$childHead.ctrl;
    panelCtrl.initEditMode();
    panelCtrl.onPluginTypeChange = (plugin: PanelPluginMeta) => {
      changePanelPlugin(panel, plugin.id);
    };

    let template = '';
    for (let i = 0; i < panelCtrl.editorTabs.length; i++) {
      const tab = panelCtrl.editorTabs[i];
      tab.isOpen = getSectionOpenState(tab.title, i === 0);

      template += `
      <div class="panel-options-group" ng-cloak>        
        <div class="panel-options-group__header" ng-click="toggleOptionGroup(${i})" aria-label="${tab.title} section">
          <div class="panel-options-group__icon">
            <icon name="ctrl.editorTabs[${i}].isOpen ? 'angle-down' : 'angle-right'"></icon>
          </div>
          <div class="panel-options-group__title">${tab.title}</div>
        </div>
        <div class="panel-options-group__body" ng-if="ctrl.editorTabs[${i}].isOpen">
          <panel-editor-tab editor-tab="ctrl.editorTabs[${i}]" ctrl="ctrl"></panel-editor-tab>
        </div>
      </div>
      `;
    }

    const loader = getAngularLoader();
    const scopeProps = {
      ctrl: panelCtrl,
      toggleOptionGroup: (index: number) => {
        const tab = panelCtrl.editorTabs[index];
        tab.isOpen = !tab.isOpen;
        saveSectionOpenState(tab.title, tab.isOpen as boolean);
      },
    };

    this.angularOptions = loader.load(this.element, scopeProps, template);
  }

  render() {
    return <div ref={elem => (this.element = elem)} />;
  }
}

const mapStateToProps: MapStateToProps<ConnectedProps, OwnProps, StoreState> = (state, props) => {
  return {
    angularPanelComponent: state.dashboard.panels[props.panel.id].angularComponent,
  };
};

const mapDispatchToProps: MapDispatchToProps<DispatchProps, OwnProps> = { changePanelPlugin };

export const AngularPanelOptions = connect(mapStateToProps, mapDispatchToProps)(AngularPanelOptionsUnconnected);

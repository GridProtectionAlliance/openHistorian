import React from 'react';
import _ from 'lodash';

import { TemplateSrv } from 'app/features/templating/template_srv';
import { SelectableValue } from '@grafana/data';
import StackdriverDatasource from '../datasource';
import appEvents from 'app/core/app_events';
import { Segment } from '@grafana/ui';
import { MetricDescriptor } from '../types';
import { CoreEvents } from 'app/types';

export interface Props {
  onChange: (metricDescriptor: MetricDescriptor) => void;
  templateSrv: TemplateSrv;
  templateVariableOptions: Array<SelectableValue<string>>;
  datasource: StackdriverDatasource;
  defaultProject: string;
  metricType: string;
  children?: (renderProps: any) => JSX.Element;
}

interface State {
  metricDescriptors: MetricDescriptor[];
  metrics: any[];
  services: any[];
  service: string;
  metric: string;
  metricDescriptor: MetricDescriptor;
  defaultProject: string;
}

export class Metrics extends React.Component<Props, State> {
  state: State = {
    metricDescriptors: [],
    metrics: [],
    services: [],
    service: '',
    metric: '',
    metricDescriptor: null,
    defaultProject: '',
  };

  constructor(props: Props) {
    super(props);
  }

  componentDidMount() {
    this.setState({ defaultProject: this.props.defaultProject }, () => {
      this.getCurrentProject()
        .then(this.loadMetricDescriptors.bind(this))
        .then(this.initializeServiceAndMetrics.bind(this));
    });
  }

  async getCurrentProject() {
    return new Promise(async (resolve, reject) => {
      try {
        if (!this.state.defaultProject || this.state.defaultProject === 'loading project...') {
          const defaultProject = await this.props.datasource.getDefaultProject();
          this.setState({ defaultProject });
        }
        resolve(this.state.defaultProject);
      } catch (error) {
        appEvents.emit(CoreEvents.dsRequestError, error);
        reject();
      }
    });
  }

  async loadMetricDescriptors() {
    if (this.state.defaultProject !== 'loading project...') {
      const metricDescriptors = await this.props.datasource.getMetricTypes(this.state.defaultProject);
      this.setState({ metricDescriptors });
      return metricDescriptors;
    } else {
      return [];
    }
  }

  async initializeServiceAndMetrics() {
    const { metricDescriptors } = this.state;
    const services = this.getServicesList(metricDescriptors);
    const metrics = this.getMetricsList(metricDescriptors);
    const service = metrics.length > 0 ? metrics[0].service : '';
    const metricDescriptor = this.getSelectedMetricDescriptor(this.props.metricType);
    this.setState({ metricDescriptors, services, metrics, service: service, metricDescriptor });
  }

  getSelectedMetricDescriptor(metricType: string) {
    return this.state.metricDescriptors.find(md => md.type === this.props.templateSrv.replace(metricType));
  }

  getMetricsList(metricDescriptors: MetricDescriptor[]) {
    const selectedMetricDescriptor = this.getSelectedMetricDescriptor(this.props.metricType);
    if (!selectedMetricDescriptor) {
      return [];
    }
    const metricsByService = metricDescriptors
      .filter(m => m.service === selectedMetricDescriptor.service)
      .map(m => ({
        service: m.service,
        value: m.type,
        label: m.displayName,
        description: m.description,
      }));
    return metricsByService;
  }

  onServiceChange = ({ value: service }: any) => {
    const { metricDescriptors } = this.state;
    const { metricType, templateSrv } = this.props;

    const metrics = metricDescriptors
      .filter(m => m.service === templateSrv.replace(service))
      .map(m => ({
        service: m.service,
        value: m.type,
        label: m.displayName,
        description: m.description,
      }));

    this.setState({ service, metrics });

    if (metrics.length > 0 && !metrics.some(m => m.value === templateSrv.replace(metricType))) {
      this.onMetricTypeChange(metrics[0]);
    }
  };

  onMetricTypeChange = ({ value }: any) => {
    const metricDescriptor = this.getSelectedMetricDescriptor(value);
    this.setState({ metricDescriptor });
    this.props.onChange({ ...metricDescriptor, type: value });
  };

  getServicesList(metricDescriptors: MetricDescriptor[]) {
    const services = metricDescriptors.map(m => ({
      value: m.service,
      label: _.startCase(m.serviceShortName),
    }));

    return services.length > 0 ? _.uniqBy(services, s => s.value) : [];
  }

  render() {
    const { services, service, metrics } = this.state;
    const { metricType, templateVariableOptions } = this.props;

    return (
      <>
        <div className="gf-form-inline">
          <span className="gf-form-label width-9 query-keyword">Service</span>
          <Segment
            onChange={this.onServiceChange}
            value={[...services, ...templateVariableOptions].find(s => s.value === service)}
            options={[
              {
                label: 'Template Variables',
                options: templateVariableOptions,
              },
              ...services,
            ]}
            placeholder="Select Services"
          ></Segment>
          <div className="gf-form gf-form--grow">
            <div className="gf-form-label gf-form-label--grow" />
          </div>
        </div>
        <div className="gf-form-inline">
          <span className="gf-form-label width-9 query-keyword">Metric</span>

          <Segment
            className="query-part"
            onChange={this.onMetricTypeChange}
            value={[...metrics, ...templateVariableOptions].find(s => s.value === metricType)}
            options={[
              {
                label: 'Template Variables',
                options: templateVariableOptions,
              },
              ...metrics,
            ]}
            placeholder="Select Metric"
          ></Segment>
          <div className="gf-form gf-form--grow">
            <div className="gf-form-label gf-form-label--grow" />
          </div>
        </div>
        {this.props.children(this.state.metricDescriptor)}
      </>
    );
  }
}

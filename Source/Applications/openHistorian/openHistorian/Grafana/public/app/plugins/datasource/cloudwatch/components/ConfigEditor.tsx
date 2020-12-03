import React, { PureComponent } from 'react';
import { InlineFormLabel, LegacyForms, Button } from '@grafana/ui';
const { Select, Input } = LegacyForms;
import {
  AppEvents,
  DataSourcePluginOptionsEditorProps,
  onUpdateDatasourceJsonDataOptionSelect,
  onUpdateDatasourceResetOption,
  onUpdateDatasourceJsonDataOption,
  onUpdateDatasourceSecureJsonDataOption,
} from '@grafana/data';
import { SelectableValue } from '@grafana/data';
import { getDatasourceSrv } from 'app/features/plugins/datasource_srv';
import { CloudWatchDatasource } from '../datasource';
import { CloudWatchJsonData, CloudWatchSecureJsonData } from '../types';
import { CancelablePromise, makePromiseCancelable } from 'app/core/utils/CancelablePromise';
import { appEvents } from 'app/core/core';

const authProviderOptions = [
  { label: 'AWS SDK Default', value: 'default' },
  { label: 'Access & secret key', value: 'keys' },
  { label: 'Credentials file', value: 'credentials' },
] as SelectableValue[];

export type Props = DataSourcePluginOptionsEditorProps<CloudWatchJsonData, CloudWatchSecureJsonData>;

export interface State {
  regions: SelectableValue[];
}

export class ConfigEditor extends PureComponent<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      regions: [],
    };
  }

  loadRegionsPromise: CancelablePromise<any> | null = null;

  componentDidMount() {
    this.loadRegionsPromise = makePromiseCancelable(this.loadRegions());
    this.loadRegionsPromise.promise.catch(({ isCanceled }) => {
      if (isCanceled) {
        console.warn('Cloud Watch ConfigEditor has unmounted, initialization was canceled');
      }
    });

    if (this.props.options.jsonData.authType === 'arn') {
      appEvents.emit(AppEvents.alertWarning, [
        'Since grafana 7.3 authentication type "arn" is deprecated, falling back to default SDK provider',
      ]);
    } else if (
      this.props.options.jsonData.authType === 'credentials' &&
      !this.props.options.jsonData.profile &&
      !this.props.options.jsonData.database
    ) {
      appEvents.emit(AppEvents.alertWarning, [
        'As of grafana 7.3 authentication type "credentials" should be used only for shared file credentials. \
         If you don\'t have a credentials file, switch to the default SDK provider for extracting credentials \
         from environment variables or IAM roles',
      ]);
    }
  }

  componentWillUnmount() {
    if (this.loadRegionsPromise) {
      this.loadRegionsPromise.cancel();
    }
  }

  async loadRegions() {
    await getDatasourceSrv()
      .loadDatasource(this.props.options.name)
      .then((ds: CloudWatchDatasource) => ds.getRegions())
      .then(
        (regions: any) => {
          this.setState({
            regions: regions.map((region: any) => {
              return {
                value: region.value,
                label: region.text,
              };
            }),
          });
        },
        (err: any) => {
          const regions = [
            'af-south-1',
            'ap-east-1',
            'ap-northeast-1',
            'ap-northeast-2',
            'ap-northeast-3',
            'ap-south-1',
            'ap-southeast-1',
            'ap-southeast-2',
            'ca-central-1',
            'cn-north-1',
            'cn-northwest-1',
            'eu-central-1',
            'eu-north-1',
            'eu-west-1',
            'eu-west-2',
            'eu-west-3',
            'me-south-1',
            'sa-east-1',
            'us-east-1',
            'us-east-2',
            'us-gov-east-1',
            'us-gov-west-1',
            'us-iso-east-1',
            'us-isob-east-1',
            'us-west-1',
            'us-west-2',
          ];

          this.setState({
            regions: regions.map((region: string) => ({
              value: region,
              label: region,
            })),
          });

          // expected to fail when creating new datasource
          // console.error('failed to get latest regions', err);
        }
      );
  }

  render() {
    const { regions } = this.state;
    const { options } = this.props;
    const secureJsonData = (options.secureJsonData || {}) as CloudWatchSecureJsonData;
    let profile = options.jsonData.profile;
    if (profile === undefined) {
      profile = options.database;
    }

    return (
      <>
        <h3 className="page-heading">CloudWatch Details</h3>
        <div className="gf-form-group">
          <div className="gf-form-inline">
            <div className="gf-form">
              <InlineFormLabel
                className="width-14"
                tooltip="Specify which AWS credentials chain to use. AWS SDK Default is the recommended option for EKS, ECS, or if you've attached an IAM role to your EC2 instance."
              >
                Authentication Provider
              </InlineFormLabel>
              <Select
                className="width-30"
                value={authProviderOptions.find(authProvider => authProvider.value === options.jsonData.authType)}
                options={authProviderOptions}
                defaultValue={options.jsonData.authType}
                onChange={option => {
                  onUpdateDatasourceJsonDataOptionSelect(this.props, 'authType')(option);
                }}
              />
            </div>
          </div>
          {options.jsonData.authType === 'credentials' && (
            <div className="gf-form-inline">
              <div className="gf-form">
                <InlineFormLabel
                  className="width-14"
                  tooltip="Credentials profile name, as specified in ~/.aws/credentials, leave blank for default."
                >
                  Credentials Profile Name
                </InlineFormLabel>
                <div className="width-30">
                  <Input
                    className="width-30"
                    placeholder="default"
                    value={profile}
                    onChange={onUpdateDatasourceJsonDataOption(this.props, 'profile')}
                  />
                </div>
              </div>
            </div>
          )}
          {options.jsonData.authType === 'keys' && (
            <div>
              {options.secureJsonFields?.accessKey ? (
                <div className="gf-form-inline">
                  <div className="gf-form">
                    <InlineFormLabel className="width-14">Access Key ID</InlineFormLabel>
                    <Input className="width-25" placeholder="Configured" disabled={true} />
                  </div>
                  <div className="gf-form">
                    <div className="max-width-30 gf-form-inline">
                      <Button
                        variant="secondary"
                        type="button"
                        onClick={onUpdateDatasourceResetOption(this.props, 'accessKey')}
                      >
                        Reset
                      </Button>
                    </div>
                  </div>
                </div>
              ) : (
                <div className="gf-form-inline">
                  <div className="gf-form">
                    <InlineFormLabel className="width-14">Access Key ID</InlineFormLabel>
                    <div className="width-30">
                      <Input
                        className="width-30"
                        value={secureJsonData.accessKey || ''}
                        onChange={onUpdateDatasourceSecureJsonDataOption(this.props, 'accessKey')}
                      />
                    </div>
                  </div>
                </div>
              )}
              {options.secureJsonFields?.secretKey ? (
                <div className="gf-form-inline">
                  <div className="gf-form">
                    <InlineFormLabel className="width-14">Secret Access Key</InlineFormLabel>
                    <Input className="width-25" placeholder="Configured" disabled={true} />
                  </div>
                  <div className="gf-form">
                    <div className="max-width-30 gf-form-inline">
                      <Button
                        variant="secondary"
                        type="button"
                        onClick={onUpdateDatasourceResetOption(this.props, 'secretKey')}
                      >
                        Reset
                      </Button>
                    </div>
                  </div>
                </div>
              ) : (
                <div className="gf-form-inline">
                  <div className="gf-form">
                    <InlineFormLabel className="width-14">Secret Access Key</InlineFormLabel>
                    <div className="width-30">
                      <Input
                        className="width-30"
                        value={secureJsonData.secretKey || ''}
                        onChange={onUpdateDatasourceSecureJsonDataOption(this.props, 'secretKey')}
                      />
                    </div>
                  </div>
                </div>
              )}
            </div>
          )}
          <div className="gf-form-inline">
            <div className="gf-form">
              <InlineFormLabel
                className="width-14"
                tooltip="Optionally, specify the ARN of a role to assume. Specifying a role here will ensure that the selected authentication provider is used to assume the specified role rather than using the credentials directly. Leave blank if you don't need to assume a role at all"
              >
                Assume Role ARN
              </InlineFormLabel>
              <div className="width-30">
                <Input
                  className="width-30"
                  placeholder="arn:aws:iam:*"
                  value={options.jsonData.assumeRoleArn || ''}
                  onChange={onUpdateDatasourceJsonDataOption(this.props, 'assumeRoleArn')}
                />
              </div>
            </div>
            <div className="gf-form-inline">
              <div className="gf-form">
                <InlineFormLabel
                  className="width-14"
                  tooltip="If you are assuming a role in another account, that has been created with an external ID, specify the external ID here."
                >
                  External ID
                </InlineFormLabel>
                <div className="width-30">
                  <Input
                    className="width-30"
                    placeholder="External ID"
                    value={options.jsonData.externalId || ''}
                    onChange={onUpdateDatasourceJsonDataOption(this.props, 'externalId')}
                  />
                </div>
              </div>
            </div>
          </div>
          <div className="gf-form-inline">
            <div className="gf-form">
              <InlineFormLabel
                className="width-14"
                tooltip="Specify the region, such as for US West (Oregon) use ` us-west-2 ` as the region."
              >
                Default Region
              </InlineFormLabel>
              <Select
                className="width-30"
                value={regions.find(region => region.value === options.jsonData.defaultRegion)}
                options={regions}
                defaultValue={options.jsonData.defaultRegion}
                onChange={onUpdateDatasourceJsonDataOptionSelect(this.props, 'defaultRegion')}
              />
            </div>
          </div>
          <div className="gf-form-inline">
            <div className="gf-form">
              <InlineFormLabel className="width-14" tooltip="Namespaces of Custom Metrics.">
                Custom Metrics
              </InlineFormLabel>
              <Input
                className="width-30"
                placeholder="Namespace1,Namespace2"
                value={options.jsonData.customMetricsNamespaces || ''}
                onChange={onUpdateDatasourceJsonDataOption(this.props, 'customMetricsNamespaces')}
              />
            </div>
          </div>
        </div>
      </>
    );
  }
}

export default ConfigEditor;

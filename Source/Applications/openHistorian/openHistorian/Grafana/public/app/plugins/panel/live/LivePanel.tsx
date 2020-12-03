import React, { PureComponent } from 'react';
import { Unsubscribable, PartialObserver } from 'rxjs';
import { FeatureInfoBox, stylesFactory, Button, JSONFormatter, CustomScrollbar } from '@grafana/ui';
import {
  GrafanaTheme,
  PanelProps,
  LiveChannelStatusEvent,
  isValidLiveChannelAddress,
  LiveChannel,
  LiveChannelEvent,
  isLiveChannelStatusEvent,
  isLiveChannelMessageEvent,
  LiveChannelConnectionState,
  PanelData,
  LoadingState,
  applyFieldOverrides,
} from '@grafana/data';
import { TablePanel } from '../table/TablePanel';
import { LivePanelOptions, MessageDisplayMode } from './types';
import { config, getGrafanaLiveSrv, MeasurementCollector } from '@grafana/runtime';
import { css, cx } from 'emotion';
import { CodeEditor } from '@grafana/ui';

interface Props extends PanelProps<LivePanelOptions> {}

interface State {
  error?: any;
  channel?: LiveChannel;
  status?: LiveChannelStatusEvent;
  message?: any;
  changed: number;
}

export class LivePanel extends PureComponent<Props, State> {
  private readonly isValid: boolean;
  subscription?: Unsubscribable;
  styles = getStyles(config.theme);

  constructor(props: Props) {
    super(props);

    this.isValid = !!getGrafanaLiveSrv();
    this.state = { changed: 0 };
  }

  async componentDidMount() {
    this.loadChannel();
  }

  componentWillUnmount() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  componentDidUpdate(prevProps: Props): void {
    if (this.props.options?.channel !== prevProps.options?.channel) {
      this.loadChannel();
    }
  }

  streamObserver: PartialObserver<LiveChannelEvent> = {
    next: (event: LiveChannelEvent) => {
      if (isLiveChannelStatusEvent(event)) {
        this.setState({ status: event, changed: Date.now() });
      } else if (isLiveChannelMessageEvent(event)) {
        this.setState({ message: event.message, changed: Date.now() });
      } else {
        console.log('ignore', event);
      }
    },
  };

  unsubscribe = () => {
    if (this.subscription) {
      this.subscription.unsubscribe();
      this.subscription = undefined;
    }
  };

  async loadChannel() {
    const addr = this.props.options?.channel;
    if (!isValidLiveChannelAddress(addr)) {
      console.log('INVALID', addr);
      this.unsubscribe();
      this.setState({
        channel: undefined,
      });
      return;
    }

    const channel = getGrafanaLiveSrv().getChannel(addr);
    const changed = channel.id !== this.state.channel?.id;
    console.log('LOAD', addr, changed, channel);
    if (changed) {
      this.unsubscribe();

      // Subscribe to new events
      try {
        this.subscription = channel.getStream().subscribe(this.streamObserver);
        this.setState({ channel, error: undefined });
      } catch (err) {
        this.setState({ channel: undefined, error: err });
      }
    } else {
      console.log('Same channel', channel);
    }
  }

  renderNotEnabled() {
    const preformatted = `[feature_toggles]
    enable = live`;
    return (
      <FeatureInfoBox
        title="Grafana Live"
        style={{
          height: this.props.height,
        }}
      >
        <p>Grafana live requires a feature flag to run</p>

        <b>custom.ini:</b>
        <pre>{preformatted}</pre>
      </FeatureInfoBox>
    );
  }

  onSaveJSON = (text: string) => {
    const { options, onOptionsChange } = this.props;

    try {
      const json = JSON.parse(text);
      onOptionsChange({ ...options, json });
    } catch (err) {
      console.log('Error reading JSON', err);
    }
  };

  onPublishClicked = async () => {
    const { channel } = this.state;
    if (!channel?.publish) {
      console.log('channel does not support publishing');
      return;
    }
    const json = this.props.options?.json;
    if (json) {
      const rsp = await channel.publish(json);
      console.log('GOT', rsp);
    } else {
      console.log('nothing to publish');
    }
  };

  renderMessage(height: number) {
    const { options } = this.props;
    const { message } = this.state;

    if (!message) {
      return (
        <div>
          <h4>Waiting for data:</h4>
          {options.channel?.scope}/{options.channel?.namespace}/{options.channel?.path}
        </div>
      );
    }

    if (options.message === MessageDisplayMode.JSON) {
      return <JSONFormatter json={message} open={5} />;
    }

    if (options.message === MessageDisplayMode.Auto) {
      if (message instanceof MeasurementCollector) {
        const data: PanelData = {
          series: applyFieldOverrides({
            data: message.getData(),
            theme: config.theme,
            getDataSourceSettingsByUid: () => undefined,
            replaceVariables: (v: string) => v,
            fieldConfig: {
              defaults: {},
              overrides: [],
            },
          }),
          state: LoadingState.Streaming,
        } as PanelData;
        const props = {
          ...this.props,
          options: { frameIndex: 0, showHeader: true },
        } as PanelProps<any>;
        return <TablePanel {...props} data={data} height={height} />;
      }
    }

    return <pre>{JSON.stringify(message)}</pre>;
  }

  renderPublish(height: number) {
    const { channel } = this.state;
    if (!channel?.publish) {
      return <div>This channel does not support publishing</div>;
    }

    const { options } = this.props;

    return (
      <>
        <CodeEditor
          height={height - 32}
          language="json"
          value={options.json ? JSON.stringify(options.json, null, 2) : '{ }'}
          onBlur={this.onSaveJSON}
          onSave={this.onSaveJSON}
          showMiniMap={false}
          showLineNumbers={true}
        />
        <div style={{ height: 32 }}>
          <Button onClick={this.onPublishClicked}>Publish</Button>
        </div>
      </>
    );
  }

  renderStatus() {
    const { status } = this.state;
    if (status?.state === LiveChannelConnectionState.Connected) {
      return; // nothing
    }

    let statusClass = '';
    if (status) {
      statusClass = this.styles.status[status.state];
    }
    return <div className={cx(statusClass, this.styles.statusWrap)}>{status?.state}</div>;
  }

  renderBody() {
    const { status } = this.state;
    const { options, height } = this.props;

    if (options.publish) {
      // Only the publish form
      if (options.message === MessageDisplayMode.None) {
        return <div>{this.renderPublish(height)}</div>;
      }
      // Both message and publish
      const halfHeight = height / 2;
      return (
        <div>
          <div style={{ height: halfHeight, overflow: 'hidden' }}>
            <CustomScrollbar autoHeightMin="100%" autoHeightMax="100%">
              {this.renderMessage(halfHeight)}
            </CustomScrollbar>
          </div>
          <div>{this.renderPublish(halfHeight)}</div>
        </div>
      );
    }
    if (options.message === MessageDisplayMode.None) {
      return <pre>{JSON.stringify(status)}</pre>;
    }

    // Only message
    return (
      <div style={{ overflow: 'hidden', height }}>
        <CustomScrollbar autoHeightMin="100%" autoHeightMax="100%">
          {this.renderMessage(height)}
        </CustomScrollbar>
      </div>
    );
  }

  render() {
    if (!this.isValid) {
      return this.renderNotEnabled();
    }
    const { channel, error } = this.state;
    if (!channel) {
      return (
        <FeatureInfoBox
          title="Grafana Live"
          style={{
            height: this.props.height,
          }}
        >
          <p>Use the panel editor to pick a channel</p>
        </FeatureInfoBox>
      );
    }
    if (error) {
      return (
        <div>
          <h2>ERROR</h2>
          <div>{JSON.stringify(error)}</div>
        </div>
      );
    }
    return (
      <>
        {this.renderStatus()}
        {this.renderBody()}
      </>
    );
  }
}

const getStyles = stylesFactory((theme: GrafanaTheme) => ({
  statusWrap: css`
    margin: auto;
    position: absolute;
    top: 0;
    right: 0;
    background: ${theme.colors.panelBg};
    padding: 10px;
    z-index: ${theme.zIndex.modal};
  `,
  status: {
    [LiveChannelConnectionState.Pending]: css`
      border: 1px solid ${theme.palette.brandPrimary};
    `,
    [LiveChannelConnectionState.Connected]: css`
      border: 1px solid ${theme.palette.brandSuccess};
    `,
    [LiveChannelConnectionState.Disconnected]: css`
      border: 1px solid ${theme.palette.brandWarning};
    `,
    [LiveChannelConnectionState.Shutdown]: css`
      border: 1px solid ${theme.palette.brandDanger};
    `,
    [LiveChannelConnectionState.Invalid]: css`
      border: 1px solid red;
    `,
  },
}));

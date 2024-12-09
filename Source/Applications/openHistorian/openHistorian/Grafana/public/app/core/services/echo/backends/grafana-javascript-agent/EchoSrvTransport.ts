import { BaseTransport, TransportItem } from '@grafana/faro-core';
import { getEchoSrv, EchoEventType, config } from '@grafana/runtime';
export class EchoSrvTransport extends BaseTransport {
  readonly name: string = 'EchoSrvTransport';
  readonly version: string = config.buildInfo.version;
  send(items: TransportItem[]) {
    getEchoSrv().addEvent({
      type: EchoEventType.GrafanaJavascriptAgent,
      payload: items,
    });
  }

  isBatched() {
    return true;
  }

  getIgnoreUrls() {
    return [];
  }
}

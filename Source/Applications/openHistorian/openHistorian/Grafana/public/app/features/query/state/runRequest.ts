// Libraries
import { isString, map as isArray } from 'lodash';
import { from, merge, Observable, of, timer } from 'rxjs';
import { catchError, map, mapTo, share, takeUntil, tap } from 'rxjs/operators';

// Utils & Services
// Types
import {
  CoreApp,
  DataQueryError,
  DataQueryRequest,
  DataQueryResponse,
  DataQueryResponseData,
  DataSourceApi,
  DataTopic,
  dateMath,
  LoadingState,
  PanelData,
  TimeRange,
} from '@grafana/data';
import { toDataQueryError } from '@grafana/runtime';
import { isExpressionReference } from '@grafana/runtime/src/utils/DataSourceWithBackend';
import { backendSrv } from 'app/core/services/backend_srv';
import { queryIsEmpty } from 'app/core/utils/query';
import { dataSource as expressionDatasource } from 'app/features/expressions/ExpressionDatasource';
import { ExpressionQuery } from 'app/features/expressions/types';

import { cancelNetworkRequestsOnUnsubscribe } from './processing/canceler';
import { emitDataRequestEvent } from './queryAnalytics';

type MapOfResponsePackets = { [str: string]: DataQueryResponse };

interface RunningQueryState {
  packets: { [key: string]: DataQueryResponse };
  panelData: PanelData;
}

/*
 * This function should handle composing a PanelData from multiple responses
 */
export function processResponsePacket(packet: DataQueryResponse, state: RunningQueryState): RunningQueryState {
  const request = state.panelData.request!;
  const packets: MapOfResponsePackets = {
    ...state.packets,
  };

  // updates to the same key will replace previous values
  const key = packet.key ?? packet.data?.[0]?.refId ?? 'A';
  packets[key] = packet;

  let loadingState = packet.state || LoadingState.Done;
  let error: DataQueryError | undefined = undefined;
  let errors: DataQueryError[] | undefined = undefined;

  const series: DataQueryResponseData[] = [];
  const annotations: DataQueryResponseData[] = [];

  for (const key in packets) {
    const packet = packets[key];

    if (packet.error || packet.errors?.length) {
      loadingState = LoadingState.Error;
      error = packet.error;
      errors = packet.errors;
    }

    if (packet.data && packet.data.length) {
      for (const dataItem of packet.data) {
        if (dataItem.meta?.dataTopic === DataTopic.Annotations) {
          annotations.push(dataItem);
          continue;
        }

        series.push(dataItem);
      }
    }
  }

  const timeRange = getRequestTimeRange(request, loadingState);

  const panelData: PanelData = {
    state: loadingState,
    series,
    annotations,
    error,
    errors,
    request,
    timeRange,
  };

  return { packets, panelData };
}

function getRequestTimeRange(request: DataQueryRequest, loadingState: LoadingState): TimeRange {
  const range = request.range;

  if (!isString(range.raw.from) || loadingState !== LoadingState.Streaming) {
    return range;
  }

  return {
    ...range,
    from: dateMath.parse(range.raw.from, false)!,
    to: dateMath.parse(range.raw.to, true)!,
  };
}

/**
 * This function handles the execution of requests & and processes the single or multiple response packets into
 * a combined PanelData response. It will
 *  Merge multiple responses into a single DataFrame array based on the packet key
 *  Will emit a loading state if no response after 50ms
 *  Cancel any still running network requests on unsubscribe (using request.requestId)
 */
export function runRequest(
  datasource: DataSourceApi,
  request: DataQueryRequest,
  queryFunction?: typeof datasource.query
): Observable<PanelData> {
  let state: RunningQueryState = {
    panelData: {
      state: LoadingState.Loading,
      series: [],
      request: request,
      timeRange: request.range,
    },
    packets: {},
  };

  // Return early if there are no queries to run
  if (!request.targets.length) {
    request.endTime = Date.now();
    state.panelData.state = LoadingState.Done;
    return of(state.panelData);
  }

  const dataObservable = callQueryMethod(datasource, request, queryFunction).pipe(
    // Transform response packets into PanelData with merged results
    map((packet: DataQueryResponse) => {
      if (!isArray(packet.data)) {
        throw new Error(`Expected response data to be array, got ${typeof packet.data}.`);
      }

      request.endTime = Date.now();

      state = processResponsePacket(packet, state);

      return state.panelData;
    }),
    // handle errors
    catchError((err) => {
      const errLog = typeof err === 'string' ? err : JSON.stringify(err);
      console.error('runRequest.catchError', errLog);
      return of({
        ...state.panelData,
        state: LoadingState.Error,
        error: toDataQueryError(err),
      });
    }),
    tap(emitDataRequestEvent(datasource)),
    // finalize is triggered when subscriber unsubscribes
    // This makes sure any still running network requests are cancelled
    cancelNetworkRequestsOnUnsubscribe(backendSrv, request.requestId),
    // this makes it possible to share this observable in takeUntil
    share()
  );

  // If 50ms without a response emit a loading state
  // mapTo will translate the timer event into state.panelData (which has state set to loading)
  // takeUntil will cancel the timer emit when first response packet is received on the dataObservable
  return merge(timer(200).pipe(mapTo(state.panelData), takeUntil(dataObservable)), dataObservable);
}

export function callQueryMethod(
  datasource: DataSourceApi,
  request: DataQueryRequest,
  queryFunction?: typeof datasource.query
) {
  // If the datasource has defined a default query, make sure it's applied
  request.targets = request.targets.map((t) =>
    queryIsEmpty(t)
      ? {
          ...datasource?.getDefaultQuery?.(CoreApp.PanelEditor),
          ...t,
        }
      : t
  );

  // If its a public datasource, just return the result. Expressions will be handled on the backend.
  if (datasource.type === 'public-ds') {
    return from(datasource.query(request));
  }

  for (const target of request.targets) {
    if (isExpressionReference(target.datasource)) {
      return expressionDatasource.query(request as DataQueryRequest<ExpressionQuery>);
    }
  }

  // Otherwise it is a standard datasource request
  const returnVal = queryFunction ? queryFunction(request) : datasource.query(request);
  return from(returnVal);
}

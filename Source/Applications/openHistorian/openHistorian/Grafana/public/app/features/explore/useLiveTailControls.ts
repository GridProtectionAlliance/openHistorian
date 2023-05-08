import React, { useCallback } from 'react';

import { RefreshPicker } from '@grafana/ui';
import { useDispatch } from 'app/types';

import { ExploreId } from '../../types';

import { setPausedStateAction, runQueries } from './state/query';
import { changeRefreshInterval } from './state/time';

/**
 * Hook that gives you all the functions needed to control the live tailing.
 */
export function useLiveTailControls(exploreId: ExploreId) {
  const dispatch = useDispatch();

  const pause = useCallback(() => {
    dispatch(setPausedStateAction({ exploreId, isPaused: true }));
  }, [exploreId, dispatch]);

  const resume = useCallback(() => {
    dispatch(setPausedStateAction({ exploreId, isPaused: false }));
  }, [exploreId, dispatch]);

  const stop = useCallback(() => {
    // We need to pause here first because there is transition where we are not live but live logs are still shown
    // to cross fade with the normal view. This will prevent reordering of the logs in the live view during the
    // transition.
    pause();

    // TODO referencing this from perspective of refresh picker when there is designated button for it now is not
    //  great. Needs a bit of refactoring.
    dispatch(changeRefreshInterval(exploreId, RefreshPicker.offOption.value));
    dispatch(runQueries(exploreId));
  }, [exploreId, dispatch, pause]);

  const start = useCallback(() => {
    dispatch(changeRefreshInterval(exploreId, RefreshPicker.liveOption.value));
  }, [exploreId, dispatch]);

  return {
    pause,
    resume,
    stop,
    start,
  };
}

type Props = {
  exploreId: ExploreId;
  children: (controls: ReturnType<typeof useLiveTailControls>) => React.ReactElement;
};

/**
 * If you can't use the hook you can use this as a render prop pattern.
 */
export function LiveTailControls(props: Props) {
  const controls = useLiveTailControls(props.exploreId);
  return props.children(controls);
}

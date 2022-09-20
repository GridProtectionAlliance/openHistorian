import hoistNonReactStatics from 'hoist-non-react-statics';
import React, { ComponentType, FunctionComponent, useEffect } from 'react';
import { connect, MapDispatchToPropsParam, MapStateToPropsParam, useDispatch } from 'react-redux';

import { cleanUpAction, StateSelector } from '../actions/cleanUp';

export const connectWithCleanUp =
  <
    TStateProps extends {} = {},
    TDispatchProps = {},
    TOwnProps = {},
    State = {},
    TSelector extends object = {},
    Statics = {}
  >(
    mapStateToProps: MapStateToPropsParam<TStateProps, TOwnProps, State>,
    mapDispatchToProps: MapDispatchToPropsParam<TDispatchProps, TOwnProps>,
    stateSelector: StateSelector<TSelector>
  ) =>
  (Component: ComponentType<any>) => {
    const ConnectedComponent = connect(
      mapStateToProps,
      mapDispatchToProps
      // @ts-ignore
    )(Component);

    const ConnectedComponentWithCleanUp: FunctionComponent = (props) => {
      const dispatch = useDispatch();
      useEffect(() => {
        return function cleanUp() {
          dispatch(cleanUpAction({ stateSelector }));
        };
      }, [dispatch]);
      // @ts-ignore
      return <ConnectedComponent {...props} />;
    };

    ConnectedComponentWithCleanUp.displayName = `ConnectWithCleanUp(${ConnectedComponent.displayName})`;
    hoistNonReactStatics(ConnectedComponentWithCleanUp, Component);
    type Hoisted = typeof ConnectedComponentWithCleanUp & Statics;

    return ConnectedComponentWithCleanUp as Hoisted;
  };

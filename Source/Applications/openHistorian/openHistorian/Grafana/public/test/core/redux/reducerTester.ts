import { Reducer } from 'redux';

import { ActionOf } from 'app/core/redux/actionCreatorFactory';

export interface Given<State> {
  givenReducer: (reducer: Reducer<State, ActionOf<any>>, state: State) => When<State>;
}

export interface When<State> {
  whenActionIsDispatched: (action: ActionOf<any>) => Then<State>;
}

export interface Then<State> {
  thenStateShouldEqual: (state: State) => When<State>;
}

interface ObjectType extends Object {
  [key: string]: any;
}

export const deepFreeze = <T>(obj: T): T => {
  Object.freeze(obj);

  const isNotException = (object: any, propertyName: any) =>
    typeof object === 'function'
      ? propertyName !== 'caller' && propertyName !== 'callee' && propertyName !== 'arguments'
      : true;
  const hasOwnProp = Object.prototype.hasOwnProperty;

  if (obj && obj instanceof Object) {
    const object: ObjectType = obj;
    Object.getOwnPropertyNames(object).forEach(propertyName => {
      const objectProperty: any = object[propertyName];
      if (
        hasOwnProp.call(object, propertyName) &&
        isNotException(object, propertyName) &&
        objectProperty &&
        (typeof objectProperty === 'object' || typeof objectProperty === 'function') &&
        Object.isFrozen(objectProperty) === false
      ) {
        deepFreeze(objectProperty);
      }
    });
  }

  return obj;
};

interface ReducerTester<State> extends Given<State>, When<State>, Then<State> {}

export const reducerTester = <State>(): Given<State> => {
  let reducerUnderTest: Reducer<State, ActionOf<any>>;
  let resultingState: State;
  let initialState: State;

  const givenReducer = (reducer: Reducer<State, ActionOf<any>>, state: State): When<State> => {
    reducerUnderTest = reducer;
    initialState = { ...state };
    initialState = deepFreeze(initialState);

    return instance;
  };

  const whenActionIsDispatched = (action: ActionOf<any>): Then<State> => {
    resultingState = reducerUnderTest(resultingState || initialState, action);

    return instance;
  };

  const thenStateShouldEqual = (state: State): When<State> => {
    expect(state).toEqual(resultingState);

    return instance;
  };

  const instance: ReducerTester<State> = { thenStateShouldEqual, givenReducer, whenActionIsDispatched };

  return instance;
};

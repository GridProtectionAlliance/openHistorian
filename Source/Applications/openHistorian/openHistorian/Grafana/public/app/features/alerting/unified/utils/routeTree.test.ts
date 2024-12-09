import { RouteWithID } from 'app/plugins/datasource/alertmanager/types';

import { FormAmRoute } from '../types/amroutes';

import { GRAFANA_DATASOURCE_NAME } from './datasource';
import { addRouteToReferenceRoute, cleanRouteIDs, findRouteInTree, omitRouteFromRouteTree } from './routeTree';

describe('findRouteInTree', () => {
  it('should find the correct route', () => {
    const needle: RouteWithID = { id: 'route-2' };

    const root: RouteWithID = {
      id: 'route-0',
      routes: [{ id: 'route-1' }, needle, { id: 'route-3', routes: [{ id: 'route-4' }] }],
    };

    expect(findRouteInTree(root, { id: 'route-2' })).toStrictEqual([needle, root, 1]);
  });

  it('should return undefined for unknown route', () => {
    const root: RouteWithID = {
      id: 'route-0',
      routes: [{ id: 'route-1' }],
    };

    expect(findRouteInTree(root, { id: 'none' })).toStrictEqual([undefined, undefined, undefined]);
  });
});

describe('addRouteToReferenceRoute', () => {
  const targetRoute = { id: 'route-3' };
  const root: RouteWithID = {
    id: 'route-1',
    routes: [{ id: 'route-2' }, targetRoute],
  };

  const newRoute: Partial<FormAmRoute> = {
    id: 'new-route',
    receiver: 'new-route',
  };

  it('should be able to add above', () => {
    expect(addRouteToReferenceRoute(GRAFANA_DATASOURCE_NAME, newRoute, targetRoute, root, 'above')).toMatchSnapshot();
  });

  it('should be able to add below', () => {
    expect(addRouteToReferenceRoute(GRAFANA_DATASOURCE_NAME, newRoute, targetRoute, root, 'below')).toMatchSnapshot();
  });

  it('should be able to add as child', () => {
    expect(addRouteToReferenceRoute(GRAFANA_DATASOURCE_NAME, newRoute, targetRoute, root, 'child')).toMatchSnapshot();
  });

  it('should throw if target route does not exist', () => {
    expect(() =>
      addRouteToReferenceRoute(GRAFANA_DATASOURCE_NAME, newRoute, { id: 'unknown' }, root, 'child')
    ).toThrow();
  });
});

describe('omitRouteFromRouteTree', () => {
  it('should omit route from tree', () => {
    const tree: RouteWithID = {
      id: 'route-1',
      receiver: 'root',
      routes: [
        { id: 'route-2', receiver: 'receiver-2' },
        { id: 'route-3', receiver: 'receiver-3', routes: [{ id: 'route-4', receiver: 'receiver-4' }] },
      ],
    };

    expect(omitRouteFromRouteTree({ id: 'route-4' }, tree)).toEqual({
      id: 'route-1',
      receiver: 'root',
      routes: [
        { id: 'route-2', receiver: 'receiver-2' },
        { id: 'route-3', receiver: 'receiver-3', routes: [] },
      ],
    });
  });

  it('should throw when removing root route from tree', () => {
    const tree: RouteWithID = {
      id: 'route-1',
    };

    expect(() => {
      omitRouteFromRouteTree(tree, { id: 'route-1' });
    }).toThrow();
  });
});

describe('cleanRouteIDs', () => {
  it('should remove IDs from routesr recursively', () => {
    expect(
      cleanRouteIDs({
        id: '1',
        receiver: '1',
        routes: [
          { id: '2', receiver: '2' },
          { id: '3', receiver: '3' },
        ],
      })
    ).toEqual({ receiver: '1', routes: [{ receiver: '2' }, { receiver: '3' }] });
  });

  it('should also accept regular routes', () => {
    expect(cleanRouteIDs({ receiver: 'test' })).toEqual({ receiver: 'test' });
  });
});

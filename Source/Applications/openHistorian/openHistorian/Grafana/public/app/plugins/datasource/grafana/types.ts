import { DataQuery } from '@grafana/data';
import { LiveDataFilter } from '@grafana/runtime';
import { SearchQuery } from 'app/features/search/service';

//----------------------------------------------
// Query
//----------------------------------------------

export enum GrafanaQueryType {
  LiveMeasurements = 'measurements',
  Annotations = 'annotations',

  // backend
  RandomWalk = 'randomWalk',
  List = 'list',
  Read = 'read',
  Search = 'search',
}

export interface GrafanaQuery extends DataQuery {
  queryType: GrafanaQueryType; // RandomWalk by default
  channel?: string;
  filter?: LiveDataFilter;
  buffer?: number;
  path?: string; // for list and read
  search?: SearchQuery;
}

export const defaultQuery: GrafanaQuery = {
  refId: 'A',
  queryType: GrafanaQueryType.RandomWalk,
};

//----------------------------------------------
// Annotations
//----------------------------------------------

export enum GrafanaAnnotationType {
  Dashboard = 'dashboard',
  Tags = 'tags',
}

export interface GrafanaAnnotationQuery extends GrafanaQuery {
  type: GrafanaAnnotationType; // tags
  limit: number; // 100
  tags?: string[];
  matchAny?: boolean; // By default Grafana only shows annotations that match all tags in the query. Enabling this returns annotations that match any of the tags in the query.
}

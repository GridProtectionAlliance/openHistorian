import React, { PureComponent } from 'react';
import { DataSourceSettings } from '@grafana/data';
import { selectors } from '@grafana/e2e-selectors';

export interface Props {
  dataSource: DataSourceSettings;
}

export class DataSourcesListItem extends PureComponent<Props> {
  render() {
    const { dataSource } = this.props;
    return (
      <li className="card-item-wrapper">
        <a className="card-item" href={`datasources/edit/${dataSource.id}`}>
          <div className="card-item-header">
            <div className="card-item-type">{dataSource.type}</div>
          </div>
          <div className="card-item-body">
            <figure className="card-item-figure">
              <img src={dataSource.typeLogoUrl} alt={dataSource.name} />
            </figure>
            <div className="card-item-details">
              <div className="card-item-name" aria-label={selectors.pages.DataSources.dataSources(dataSource.name)}>
                {dataSource.name}
                {dataSource.isDefault && <span className="btn btn-secondary btn-small card-item-label">default</span>}
              </div>
              <div className="card-item-sub-name">{dataSource.url}</div>
            </div>
          </div>
        </a>
      </li>
    );
  }
}

export default DataSourcesListItem;

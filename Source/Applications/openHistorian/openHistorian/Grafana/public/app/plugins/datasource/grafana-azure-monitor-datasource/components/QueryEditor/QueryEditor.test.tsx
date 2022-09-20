import { render, screen, waitFor } from '@testing-library/react';
import React from 'react';
import { selectOptionInTest } from 'test/helpers/selectOptionInTest';

import * as ui from '@grafana/ui';

import createMockDatasource from '../../__mocks__/datasource';
import { invalidNamespaceError } from '../../__mocks__/errors';
import createMockQuery from '../../__mocks__/query';
import { AzureQueryType } from '../../types';

import QueryEditor from './QueryEditor';

// Have to mock CodeEditor because it doesnt seem to work in tests???
jest.mock('@grafana/ui', () => ({
  ...jest.requireActual<typeof ui>('@grafana/ui'),
  CodeEditor: function CodeEditor({ value }: { value: string }) {
    return <pre>{value}</pre>;
  },
}));

describe('Azure Monitor QueryEditor', () => {
  it('renders the Metrics query editor when the query type is Metrics', async () => {
    const mockDatasource = createMockDatasource();
    const mockQuery = {
      ...createMockQuery(),
      queryType: AzureQueryType.AzureMonitor,
    };

    render(<QueryEditor query={mockQuery} datasource={mockDatasource} onChange={() => {}} onRunQuery={() => {}} />);
    await waitFor(() =>
      expect(screen.getByTestId('azure-monitor-metrics-query-editor-with-experimental-ui')).toBeInTheDocument()
    );
  });

  it('renders the Logs query editor when the query type is Logs', async () => {
    const mockDatasource = createMockDatasource();
    const mockQuery = {
      ...createMockQuery(),
      queryType: AzureQueryType.LogAnalytics,
    };

    render(<QueryEditor query={mockQuery} datasource={mockDatasource} onChange={() => {}} onRunQuery={() => {}} />);
    await waitFor(() =>
      expect(screen.queryByTestId('azure-monitor-logs-query-editor-with-experimental-ui')).toBeInTheDocument()
    );
  });

  it('changes the query type when selected', async () => {
    const mockDatasource = createMockDatasource();
    const mockQuery = createMockQuery();
    const onChange = jest.fn();
    render(<QueryEditor query={mockQuery} datasource={mockDatasource} onChange={onChange} onRunQuery={() => {}} />);
    await waitFor(() => expect(screen.getByTestId('azure-monitor-query-editor')).toBeInTheDocument());

    const metrics = await screen.findByLabelText(/Service/);
    await selectOptionInTest(metrics, 'Logs');

    expect(onChange).toHaveBeenCalledWith({
      ...mockQuery,
      queryType: AzureQueryType.LogAnalytics,
    });
  });

  it('displays error messages from frontend Azure calls', async () => {
    const mockDatasource = createMockDatasource();
    mockDatasource.azureMonitorDatasource.getMetricNamespaces = jest.fn().mockRejectedValue(invalidNamespaceError());
    render(
      <QueryEditor query={createMockQuery()} datasource={mockDatasource} onChange={() => {}} onRunQuery={() => {}} />
    );
    await waitFor(() =>
      expect(screen.getByTestId('azure-monitor-metrics-query-editor-with-experimental-ui')).toBeInTheDocument()
    );
    expect(screen.getByText('An error occurred while requesting metadata from Azure Monitor')).toBeInTheDocument();
  });

  it('should render the experimental QueryHeader when feature toggle is enabled', async () => {
    const mockDatasource = createMockDatasource();
    const mockQuery = {
      ...createMockQuery(),
      queryType: AzureQueryType.AzureMonitor,
    };

    render(<QueryEditor query={mockQuery} datasource={mockDatasource} onChange={() => {}} onRunQuery={() => {}} />);

    await waitFor(() => expect(screen.getByTestId('azure-monitor-experimental-header')).toBeInTheDocument());
  });
});

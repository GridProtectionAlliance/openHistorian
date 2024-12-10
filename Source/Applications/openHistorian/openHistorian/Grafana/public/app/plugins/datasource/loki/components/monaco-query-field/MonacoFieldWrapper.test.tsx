import { render, screen, waitFor } from '@testing-library/react';

import { selectors } from '@grafana/e2e-selectors';

import { createLokiDatasource } from '../../__mocks__/datasource';

import { MonacoQueryFieldWrapper, Props } from './MonacoQueryFieldWrapper';

function renderComponent({ initialValue = '', onChange = jest.fn(), onRunQuery = jest.fn() }: Partial<Props> = {}) {
  const datasource = createLokiDatasource();

  render(
    <MonacoQueryFieldWrapper
      datasource={datasource}
      history={[]}
      initialValue={initialValue}
      onChange={onChange}
      onRunQuery={onRunQuery}
      placeholder="Enter a Loki query (run with Shift+Enter)"
    />
  );
}

describe('MonacoFieldWrapper', () => {
  test('Renders with no errors', async () => {
    renderComponent();

    await waitFor(async () => {
      const monacoEditor = await screen.findByTestId(selectors.components.ReactMonacoEditor.editorLazy);
      expect(monacoEditor).toBeInTheDocument();
    });
  });
});

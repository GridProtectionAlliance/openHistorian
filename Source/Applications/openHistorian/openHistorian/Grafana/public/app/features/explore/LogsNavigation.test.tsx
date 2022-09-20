import { render, screen, fireEvent } from '@testing-library/react';
import React, { ComponentProps } from 'react';

import { LogsSortOrder } from '@grafana/data';

import LogsNavigation from './LogsNavigation';

// we have to mock out reportInteraction, otherwise it crashes the test.
jest.mock('@grafana/runtime', () => ({
  ...jest.requireActual('@grafana/runtime'),
  reportInteraction: () => null,
}));

type LogsNavigationProps = ComponentProps<typeof LogsNavigation>;
const defaultProps: LogsNavigationProps = {
  absoluteRange: { from: 1637319381811, to: 1637322981811 },
  timeZone: 'local',
  queries: [],
  loading: false,
  logsSortOrder: undefined,
  visibleRange: { from: 1637322959000, to: 1637322981811 },
  onChangeTime: jest.fn(),
  scrollToTopLogs: jest.fn(),
  addResultsToCache: jest.fn(),
  clearCache: jest.fn(),
};

const setup = (propOverrides?: object) => {
  const props = {
    ...defaultProps,
    ...propOverrides,
  };

  return render(<LogsNavigation {...props} />);
};

describe('LogsNavigation', () => {
  it('should always render 3 navigation buttons', () => {
    setup();
    expect(screen.getByTestId('newerLogsButton')).toBeInTheDocument();
    expect(screen.getByTestId('olderLogsButton')).toBeInTheDocument();
    expect(screen.getByTestId('scrollToTop')).toBeInTheDocument();
  });

  it('should render 3 navigation buttons in correct order when default logs order', () => {
    const { container } = setup();
    const expectedOrder = ['newerLogsButton', 'olderLogsButton', 'scrollToTop'];
    const elements = container.querySelectorAll(
      '[data-testid=newerLogsButton],[data-testid=olderLogsButton],[data-testid=scrollToTop]'
    );
    expect(Array.from(elements).map((el) => el.getAttribute('data-testid'))).toMatchObject(expectedOrder);
  });

  it('should render 3 navigation buttons in correct order when flipped logs order', () => {
    const { container } = setup({ logsSortOrder: LogsSortOrder.Ascending });
    const expectedOrder = ['olderLogsButton', 'newerLogsButton', 'scrollToTop'];
    const elements = container.querySelectorAll(
      '[data-testid=newerLogsButton],[data-testid=olderLogsButton],[data-testid=scrollToTop]'
    );
    expect(Array.from(elements).map((el) => el.getAttribute('data-testid'))).toMatchObject(expectedOrder);
  });

  it('should disable fetch buttons when logs are loading', () => {
    setup({ loading: true });
    const olderLogsButton = screen.getByTestId('olderLogsButton');
    const newerLogsButton = screen.getByTestId('newerLogsButton');
    expect(olderLogsButton).toBeDisabled();
    expect(newerLogsButton).toBeDisabled();
  });

  it('should render logs navigation pages section', () => {
    setup();
    expect(screen.getByTestId('logsNavigationPages')).toBeInTheDocument();
  });

  it('should correctly request older logs when flipped order', () => {
    const onChangeTimeMock = jest.fn();
    const { rerender } = setup({ onChangeTime: onChangeTimeMock });
    fireEvent.click(screen.getByTestId('olderLogsButton'));
    expect(onChangeTimeMock).toHaveBeenCalledWith({ from: 1637319359000, to: 1637322959000 });

    rerender(
      <LogsNavigation
        {...defaultProps}
        absoluteRange={{ from: 1637319359000, to: 1637322959000 }}
        visibleRange={{ from: 1637322938000, to: 1637322959000 }}
        onChangeTime={onChangeTimeMock}
        logsSortOrder={LogsSortOrder.Ascending}
      />
    );
    fireEvent.click(screen.getByTestId('olderLogsButton'));
    expect(onChangeTimeMock).toHaveBeenCalledWith({ from: 1637319338000, to: 1637322938000 });
  });
});

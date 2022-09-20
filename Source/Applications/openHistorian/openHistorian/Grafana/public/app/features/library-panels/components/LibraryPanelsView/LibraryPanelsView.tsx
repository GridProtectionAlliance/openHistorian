import { css, cx } from '@emotion/css';
import React, { useMemo, useReducer } from 'react';
import { useDebounce } from 'react-use';

import { GrafanaTheme, LoadingState } from '@grafana/data';
import { Pagination, useStyles } from '@grafana/ui';

import { LibraryElementDTO } from '../../types';
import { LibraryPanelCard } from '../LibraryPanelCard/LibraryPanelCard';

import { asyncDispatcher, deleteLibraryPanel, searchForLibraryPanels } from './actions';
import { changePage, initialLibraryPanelsViewState, libraryPanelsViewReducer } from './reducer';

interface LibraryPanelViewProps {
  className?: string;
  onClickCard: (panel: LibraryElementDTO) => void;
  showSecondaryActions?: boolean;
  currentPanelId?: string;
  searchString: string;
  sortDirection?: string;
  panelFilter?: string[];
  folderFilter?: string[];
  perPage?: number;
}

export const LibraryPanelsView: React.FC<LibraryPanelViewProps> = ({
  className,
  onClickCard,
  searchString,
  sortDirection,
  panelFilter,
  folderFilter,
  showSecondaryActions,
  currentPanelId: currentPanel,
  perPage: propsPerPage = 40,
}) => {
  const styles = useStyles(getPanelViewStyles);
  const [{ libraryPanels, page, perPage, numberOfPages, loadingState, currentPanelId }, dispatch] = useReducer(
    libraryPanelsViewReducer,
    {
      ...initialLibraryPanelsViewState,
      currentPanelId: currentPanel,
      perPage: propsPerPage,
    }
  );
  const asyncDispatch = useMemo(() => asyncDispatcher(dispatch), [dispatch]);
  useDebounce(
    () =>
      asyncDispatch(
        searchForLibraryPanels({
          searchString,
          sortDirection,
          panelFilter,
          folderFilter,
          page,
          perPage,
          currentPanelId,
        })
      ),
    300,
    [searchString, sortDirection, panelFilter, folderFilter, page, asyncDispatch]
  );
  const onDelete = ({ uid }: LibraryElementDTO) =>
    asyncDispatch(deleteLibraryPanel(uid, { searchString, page, perPage }));
  const onPageChange = (page: number) => asyncDispatch(changePage({ page }));

  return (
    <div className={cx(styles.container, className)}>
      <div className={styles.libraryPanelList}>
        {loadingState === LoadingState.Loading ? (
          <p>Loading library panels...</p>
        ) : libraryPanels.length < 1 ? (
          <p className={styles.noPanelsFound}>No library panels found.</p>
        ) : (
          libraryPanels?.map((item, i) => (
            <LibraryPanelCard
              key={`library-panel=${i}`}
              libraryPanel={item}
              onDelete={onDelete}
              onClick={onClickCard}
              showSecondaryActions={showSecondaryActions}
            />
          ))
        )}
      </div>
      {libraryPanels.length ? (
        <div className={styles.pagination}>
          <Pagination
            currentPage={page}
            numberOfPages={numberOfPages}
            onNavigate={onPageChange}
            hideWhenSinglePage={true}
          />
        </div>
      ) : null}
    </div>
  );
};

const getPanelViewStyles = (theme: GrafanaTheme) => {
  return {
    container: css`
      display: flex;
      flex-direction: column;
      flex-wrap: nowrap;
    `,
    libraryPanelList: css`
      max-width: 100%;
      display: grid;
      grid-gap: ${theme.spacing.sm};
    `,
    searchHeader: css`
      display: flex;
    `,
    newPanelButton: css`
      margin-top: 10px;
      align-self: flex-start;
    `,
    pagination: css`
      align-self: center;
      margin-top: ${theme.spacing.sm};
    `,
    noPanelsFound: css`
      label: noPanelsFound;
      min-height: 200px;
    `,
  };
};

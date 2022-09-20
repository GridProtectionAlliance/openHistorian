import React, { useState } from 'react';
import { useDebounce } from 'react-use';

import { ConfirmModal } from '@grafana/ui';
import { Page } from 'app/core/components/Page/Page';
import PageActionBar from 'app/core/components/PageActionBar/PageActionBar';
import { contextSrv } from 'app/core/services/context_srv';

import EmptyListCTA from '../../core/components/EmptyListCTA/EmptyListCTA';

import { EmptyQueryListBanner } from './EmptyQueryListBanner';
import { PlaylistPageList } from './PlaylistPageList';
import { StartModal } from './StartModal';
import { deletePlaylist, getAllPlaylist } from './api';
import { PlaylistDTO } from './types';

export const PlaylistPage = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [debouncedSearchQuery, setDebouncedSearchQuery] = useState(searchQuery);
  const [hasFetched, setHasFetched] = useState(false);
  const [startPlaylist, setStartPlaylist] = useState<PlaylistDTO | undefined>();
  const [playlistToDelete, setPlaylistToDelete] = useState<PlaylistDTO | undefined>();
  const [forcePlaylistsFetch, setForcePlaylistsFetch] = useState(0);

  const [playlists, setPlaylists] = useState<PlaylistDTO[]>([]);

  useDebounce(
    async () => {
      const playlists = await getAllPlaylist(searchQuery);
      if (!hasFetched) {
        setHasFetched(true);
      }
      setPlaylists(playlists);
      setDebouncedSearchQuery(searchQuery);
    },
    350,
    [forcePlaylistsFetch, searchQuery]
  );

  const hasPlaylists = playlists && playlists.length > 0;
  const onDismissDelete = () => setPlaylistToDelete(undefined);
  const onDeletePlaylist = () => {
    if (!playlistToDelete) {
      return;
    }
    deletePlaylist(playlistToDelete.uid).finally(() => {
      setForcePlaylistsFetch(forcePlaylistsFetch + 1);
      setPlaylistToDelete(undefined);
    });
  };

  const emptyListBanner = (
    <EmptyListCTA
      title="There are no playlists created yet"
      buttonIcon="plus"
      buttonLink="playlists/new"
      buttonTitle="Create Playlist"
      buttonDisabled={!contextSrv.isEditor}
      proTip="You can use playlists to cycle dashboards on TVs without user control"
      proTipLink="http://docs.grafana.org/reference/playlist/"
      proTipLinkTitle="Learn more"
      proTipTarget="_blank"
    />
  );

  const showSearch = playlists.length > 0 || searchQuery.length > 0 || debouncedSearchQuery.length > 0;

  return (
    <Page navId="dashboards/playlists">
      <Page.Contents isLoading={!hasFetched}>
        {showSearch && (
          <PageActionBar
            searchQuery={searchQuery}
            linkButton={contextSrv.isEditor && { title: 'New playlist', href: '/playlists/new' }}
            setSearchQuery={setSearchQuery}
          />
        )}

        {!hasPlaylists && searchQuery ? (
          <EmptyQueryListBanner />
        ) : (
          <PlaylistPageList
            playlists={playlists}
            setStartPlaylist={setStartPlaylist}
            setPlaylistToDelete={setPlaylistToDelete}
          />
        )}
        {!showSearch && emptyListBanner}
        {playlistToDelete && (
          <ConfirmModal
            title={playlistToDelete.name}
            confirmText="Delete"
            body={`Are you sure you want to delete '${playlistToDelete.name}' playlist?`}
            onConfirm={onDeletePlaylist}
            isOpen={Boolean(playlistToDelete)}
            onDismiss={onDismissDelete}
          />
        )}
        {startPlaylist && <StartModal playlist={startPlaylist} onDismiss={() => setStartPlaylist(undefined)} />}
      </Page.Contents>
    </Page>
  );
};

export default PlaylistPage;

import _ from 'lodash';
import coreModule from '../../core/core_module';
import { ILocationService, IScope } from 'angular';
import { getBackendSrv } from '@grafana/runtime';
import { NavModelSrv } from 'app/core/nav_model_srv';
import { AppEventEmitter } from 'app/types';
import { AppEvents } from '@grafana/data';
import { promiseToDigest } from '../../core/utils/promiseToDigest';

export interface PlaylistItem {
  value: any;
  id: any;
  type: string;
  order: any;
}
export class PlaylistEditCtrl {
  filteredDashboards: any = [];
  filteredTags: any = [];
  searchQuery = '';
  loading = false;
  playlist: any = {
    interval: '5m',
  };

  playlistItems: any = [];
  dashboardresult: any = [];
  tagresult: any = [];
  navModel: any;
  isNew: boolean;

  /** @ngInject */
  constructor(
    private $scope: IScope & AppEventEmitter,
    private $location: ILocationService,
    $route: any,
    navModelSrv: NavModelSrv
  ) {
    this.navModel = navModelSrv.getNav('dashboards', 'playlists', 0);
    this.isNew = !$route.current.params.id;

    if ($route.current.params.id) {
      const playlistId = $route.current.params.id;

      promiseToDigest(this.$scope)(
        getBackendSrv()
          .get('/api/playlists/' + playlistId)
          .then((result: any) => {
            this.playlist = result;
            this.playlistItems = result.items;
          })
      );
    }
  }

  filterFoundPlaylistItems() {
    this.filteredDashboards = _.reject(this.dashboardresult, playlistItem => {
      return _.find(this.playlistItems, listPlaylistItem => {
        return parseInt(listPlaylistItem.value, 10) === playlistItem.id;
      });
    });

    this.filteredTags = _.reject(this.tagresult, tag => {
      return _.find(this.playlistItems, listPlaylistItem => {
        return listPlaylistItem.value === tag.term;
      });
    });
  }

  addPlaylistItem(playlistItem: PlaylistItem) {
    playlistItem.value = playlistItem.id.toString();
    playlistItem.type = 'dashboard_by_id';
    playlistItem.order = this.playlistItems.length + 1;

    this.playlistItems.push(playlistItem);
    this.filterFoundPlaylistItems();
  }

  addTagPlaylistItem(tag: { term: any }) {
    const playlistItem: any = {
      value: tag.term,
      type: 'dashboard_by_tag',
      order: this.playlistItems.length + 1,
      title: tag.term,
    };

    this.playlistItems.push(playlistItem);
    this.filterFoundPlaylistItems();
  }

  removePlaylistItem(playlistItem: PlaylistItem) {
    _.remove(this.playlistItems, listedPlaylistItem => {
      return playlistItem === listedPlaylistItem;
    });
    this.filterFoundPlaylistItems();
  }

  savePlaylist(playlist: any, playlistItems: PlaylistItem[]) {
    let savePromise;

    playlist.items = playlistItems;

    savePromise = playlist.id
      ? promiseToDigest(this.$scope)(getBackendSrv().put('/api/playlists/' + playlist.id, playlist))
      : promiseToDigest(this.$scope)(getBackendSrv().post('/api/playlists', playlist));

    savePromise.then(
      () => {
        this.$scope.appEvent(AppEvents.alertSuccess, ['Playlist saved']);
        this.$location.path('/playlists');
      },
      () => {
        this.$scope.appEvent(AppEvents.alertError, ['Unable to save playlist']);
      }
    );
  }

  isPlaylistEmpty() {
    return !this.playlistItems.length;
  }

  backToList() {
    this.$location.path('/playlists');
  }

  searchStarted(promise: Promise<any>) {
    promise.then((data: any) => {
      this.dashboardresult = data.dashboardResult;
      this.tagresult = data.tagResult;
      this.filterFoundPlaylistItems();
    });
  }

  movePlaylistItem(playlistItem: PlaylistItem, offset: number) {
    const currentPosition = this.playlistItems.indexOf(playlistItem);
    const newPosition = currentPosition + offset;

    if (newPosition >= 0 && newPosition < this.playlistItems.length) {
      this.playlistItems.splice(currentPosition, 1);
      this.playlistItems.splice(newPosition, 0, playlistItem);
    }
  }

  movePlaylistItemUp(playlistItem: PlaylistItem) {
    this.movePlaylistItem(playlistItem, -1);
  }

  movePlaylistItemDown(playlistItem: PlaylistItem) {
    this.movePlaylistItem(playlistItem, 1);
  }
}

coreModule.controller('PlaylistEditCtrl', PlaylistEditCtrl);

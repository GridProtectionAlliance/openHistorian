import '../playlist_edit_ctrl';
import { PlaylistEditCtrl } from '../playlist_edit_ctrl';
import { ILocationService, IScope } from 'angular';
import { AppEventEmitter } from '../../../types';

describe('PlaylistEditCtrl', () => {
  let ctx: any;
  beforeEach(() => {
    const navModelSrv: any = {
      getNav: () => {
        return { breadcrumbs: [], node: {} } as any;
      },
    };

    ctx = new PlaylistEditCtrl(
      (null as unknown) as IScope & AppEventEmitter,
      (null as unknown) as ILocationService,
      { current: { params: {} } },
      navModelSrv
    );

    ctx.dashboardresult = [
      { id: 2, title: 'dashboard: 2' },
      { id: 3, title: 'dashboard: 3' },
    ];

    ctx.tagresult = [
      { term: 'graphite', count: 1 },
      { term: 'nyc', count: 2 },
    ];
  });

  describe('searchresult returns 2 dashboards, ', () => {
    it('found dashboard should be 2', () => {
      expect(ctx.dashboardresult.length).toBe(2);
    });

    it('filtred result should be 2', () => {
      ctx.filterFoundPlaylistItems();
      expect(ctx.filteredDashboards.length).toBe(2);
      expect(ctx.filteredTags.length).toBe(2);
    });

    describe('adds one dashboard to playlist, ', () => {
      beforeEach(() => {
        ctx.addPlaylistItem({ id: 2, title: 'dashboard: 2' });
        ctx.addTagPlaylistItem({ term: 'graphite' });
        ctx.filterFoundPlaylistItems();
      });

      it('playlistitems should be increased by one', () => {
        expect(ctx.playlistItems.length).toBe(2);
      });

      it('filtred playlistitems should be reduced by one', () => {
        expect(ctx.filteredDashboards.length).toBe(1);
        expect(ctx.filteredTags.length).toBe(1);
      });

      it('found dashboard should be 2', () => {
        expect(ctx.dashboardresult.length).toBe(2);
      });

      describe('removes one dashboard from playlist, ', () => {
        beforeEach(() => {
          ctx.removePlaylistItem(ctx.playlistItems[0]);
          ctx.removePlaylistItem(ctx.playlistItems[0]);
          ctx.filterFoundPlaylistItems();
        });

        it('playlistitems should be increased by one', () => {
          expect(ctx.playlistItems.length).toBe(0);
        });

        it('found dashboard should be 2', () => {
          expect(ctx.dashboardresult.length).toBe(2);
          expect(ctx.filteredDashboards.length).toBe(2);
          expect(ctx.filteredTags.length).toBe(2);
          expect(ctx.tagresult.length).toBe(2);
        });
      });
    });
  });
});

"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[4074],{48548:(H,C,t)=>{t.r(C),t.d(C,{DashboardListPage:()=>M,default:()=>Q});var r=t(9892),e=t(68404),A=t(22350),I=t(83383),x=t(37932),F=t(79396),P=t(29930),w=t(60492);const T=a=>P.ae.getFolderByUid(a,{withAccessControl:!0}).then(o=>{const s=(0,w.B)(o);return s.children[0].active=!0,{folder:o,folderNav:s}});var L=t(72648),$=t(46967),i=t(35287),v=t(82002),p=t(81168),B=t(54350),U=t(50343),V=t(66757),b=t(35645),E=t(41818),f=t(38484),z=t(52081),N=t(31403),W=t(71698),O=t(39904),K=t(62079);const Z=({folder:a,canCreateFolders:o=!1,canCreateDashboards:s=!1})=>{const[l,c]=(0,e.useState)(!1),m=b.v.featureToggles.nestedFolders&&(a?.canSave??!1),h=(0,e.useMemo)(()=>new Map([["folder",new Set(a?.uid?[a.uid]:[])]]),[a]),n=g=>{let u=`dashboard/${g}`;return g==="new_folder"&&(u="dashboards/folder/new/"),a?.uid&&(u+=`?folderUid=${a.uid}`),u},d=()=>e.createElement(f.v,null,s&&e.createElement(f.v.Item,{url:n("new"),label:(0,i.t)("search.dashboard-actions.new-dashboard","New Dashboard"),onClick:()=>(0,E.ff)("grafana_menu_item_clicked",{url:n("new"),from:"/dashboards"})}),o&&(b.v.featureToggles.nestedFolders||!a?.uid)&&e.createElement(f.v.Item,{url:n("new_folder"),label:(0,i.t)("search.dashboard-actions.new-folder","New Folder"),onClick:()=>(0,E.ff)("grafana_menu_item_clicked",{url:n("new_folder"),from:"/dashboards"})}),s&&e.createElement(f.v.Item,{url:n("import"),label:(0,i.t)("search.dashboard-actions.import","Import"),onClick:()=>(0,E.ff)("grafana_menu_item_clicked",{url:n("import"),from:"/dashboards"})}));return e.createElement(e.Fragment,null,e.createElement("div",null,e.createElement(z.Lh,null,m&&e.createElement(N.zx,{onClick:()=>c(!0),icon:"exchange-alt",variant:"secondary"},"Move"),e.createElement(W.L,{overlay:d,placement:"bottom-start"},e.createElement(N.zx,{variant:"primary"},(0,i.t)("search.dashboard-actions.new","New"),e.createElement(O.J,{name:"angle-down"}))))),m&&l&&e.createElement(K.i,{onMoveItems:()=>{},results:h,onDismiss:()=>c(!1)}))},D=e.memo(({folder:a})=>{const o=(0,L.wW)(J),s=(0,V.hD)(),l=s.useState(),{onKeyDown:c,keyboardEvents:m}=(0,B.A)(),h=a?.uid,n=a?.canSave,{isEditor:d}=v.Vt,g=a?n:v.Vt.hasEditPermissionInFolders,u=v.Vt.hasAccess(p.AccessControlAction.FoldersCreate,d),y=g||!!n,S=h?v.Vt.hasAccessInMetadata(p.AccessControlAction.DashboardsCreate,a,y):v.Vt.hasAccess(p.AccessControlAction.DashboardsCreate,y),R=a===void 0&&u||S;return(0,e.useEffect)(()=>s.initStateFromUrl(a?.uid),[a?.uid,s]),e.createElement(e.Fragment,null,e.createElement("div",{className:(0,r.cx)(o.actionBar,"page-action-bar")},e.createElement("div",{className:(0,r.cx)(o.inputWrapper,"gf-form gf-form--grow m-r-2")},e.createElement($.I,{value:l.query??"",onChange:G=>s.onQueryChange(G.currentTarget.value),onKeyDown:c,autoFocus:!0,spellCheck:!1,placeholder:l.includePanels?(0,i.t)("search.search-input.include-panels-placeholder","Search for dashboards and panels"):(0,i.t)("search.search-input.placeholder","Search for dashboards"),className:o.searchInput,suffix:null})),R&&e.createElement(Z,{folder:a,canCreateFolders:u,canCreateDashboards:S})),e.createElement(U.Z,{showManage:Boolean(d||g||n),folderDTO:a,hidePseudoFolders:!0,keyboardEvents:m}))});D.displayName="ManageDashboardsNew";const j=D,J=a=>({actionBar:r.css`
    ${a.breakpoints.down("sm")} {
      flex-wrap: wrap;
    }
  `,inputWrapper:r.css`
    ${a.breakpoints.down("sm")} {
      margin-right: 0 !important;
    }
  `,searchInput:r.css`
    margin-bottom: 6px;
    min-height: ${a.spacing(4)};
  `,unsupported:r.css`
    padding: 10px;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
    font-size: 18px;
  `,noResults:r.css`
    padding: ${a.v1.spacing.md};
    background: ${a.v1.colors.bg2};
    font-style: italic;
    margin-top: ${a.v1.spacing.md};
  `}),M=(0,e.memo)(({match:a,location:o})=>{const{loading:s,value:l}=(0,A.Z)(()=>{const c=a.params.uid,m=o.pathname;return!c||!m.startsWith("/dashboards")?Promise.resolve({}):T(c).then(({folder:h,folderNav:n})=>{const d=I.u.stripBaseFromUrl(h.url);return d!==o.pathname&&x.E1.replace(d),{folder:h,pageNav:n}})},[a.params.uid]);return e.createElement(F.T,{navId:"dashboards/browse",pageNav:l?.pageNav},e.createElement(F.T.Contents,{isLoading:s,className:r.css`
          display: flex;
          flex-direction: column;
          height: 100%;
        `},e.createElement(j,{folder:l?.folder})))});M.displayName="DashboardListPage";const Q=M}}]);

//# sourceMappingURL=DashboardListPage.0609b0261325d16a1def.js.map
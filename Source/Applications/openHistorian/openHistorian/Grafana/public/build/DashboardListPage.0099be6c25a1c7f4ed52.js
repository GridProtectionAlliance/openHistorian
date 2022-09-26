"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[4074],{72794:(e,a,r)=>{r.r(a),r.d(a,{DashboardListPage:()=>k,default:()=>F});var s=r(36636),n=r(68404),o=r(17614),t=r(43215),l=r(90923),d=r(69371),i=r(28659),c=r(49545);var u,h,p=r(92115),m=r(93368),g=r(3490),f=r(61959),v=r(47570),b=r(97621),x=r(51189),w=r(71625),y=r(12568),j=r(45916);const N=e=>{let{folderId:a,canCreateFolders:r=!1,canCreateDashboards:s=!1}=e;const n=e=>{let r=`dashboard/${e}`;return a&&(r+=`?folderId=${a}`),r};return(0,j.jsx)("div",{children:(0,j.jsx)(g.Dropdown,{overlay:()=>(0,j.jsxs)(g.Menu,{children:[s&&(0,j.jsx)(g.Menu.Item,{url:n("new"),label:"New Dashboard"}),!a&&r&&(u||(u=(0,j.jsx)(g.Menu.Item,{url:"dashboards/folder/new",label:"New Folder"}))),s&&(0,j.jsx)(g.Menu.Item,{url:n("import"),label:"Import"})]}),placement:"bottom-start",children:h||(h=(0,j.jsxs)(g.Button,{variant:"primary",children:["New",(0,j.jsx)(g.Icon,{name:"angle-down"})]}))})})};const I=n.memo((e=>{var a;let{folder:r}=e;const o=(0,g.useStyles2)(D),{query:t,onQueryChange:d}=(0,w.A)({}),{onKeyDown:i,keyboardEvents:c}=(0,x.A)(),u=null==r?void 0:r.id,h=null==r?void 0:r.canSave,I=r?h:f.Vt.hasEditPermissionInFolders;let[C,k]=(0,p.Z)(b.to,!0);l.config.featureToggles.panelTitleSearch||(C=!1);const{isEditor:F}=f.Vt,[T,$]=(0,n.useState)(null!==(a=t.query)&&void 0!==a?a:"");return(0,m.Z)((()=>d(T)),200,[T]),(0,j.jsxs)(j.Fragment,{children:[(0,j.jsxs)("div",{className:(0,s.cx)(o.actionBar,"page-action-bar"),children:[(0,j.jsx)("div",{className:(0,s.cx)(o.inputWrapper,"gf-form gf-form--grow m-r-2"),children:(0,j.jsx)(g.Input,{value:T,onChange:e=>{e.preventDefault(),$(e.currentTarget.value)},onKeyDown:i,autoFocus:!0,spellCheck:!1,placeholder:C?"Search for dashboards and panels":"Search for dashboards",className:o.searchInput,suffix:null})}),(0,j.jsx)(N,{folderId:u,canCreateFolders:f.Vt.hasAccess(v.bW.FoldersCreate,F),canCreateDashboards:f.Vt.hasAccess(v.bW.DashboardsCreate,I||!!h)})]}),(0,j.jsx)(y.Z,{showManage:F||I||h,folderDTO:r,queryText:t.query,onQueryTextChange:e=>{$(e)},hidePseudoFolders:!0,includePanels:C,setIncludePanels:k,keyboardEvents:c})]})}));I.displayName="ManageDashboardsNew";const C=I,D=e=>({actionBar:s.css`
    ${e.breakpoints.down("sm")} {
      flex-wrap: wrap;
    }
  `,inputWrapper:s.css`
    ${e.breakpoints.down("sm")} {
      margin-right: 0 !important;
    }
  `,searchInput:s.css`
    margin-bottom: 6px;
    min-height: ${e.spacing(4)};
  `,unsupported:s.css`
    padding: 10px;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
    font-size: 18px;
  `,noResults:s.css`
    padding: ${e.v1.spacing.md};
    background: ${e.v1.colors.bg2};
    font-style: italic;
    margin-top: ${e.v1.spacing.md};
  `}),k=(0,n.memo)((e=>{let{match:a,location:r}=e;const{loading:n,value:u}=(0,o.Z)((()=>{const e=a.params.uid,s=r.pathname;return e&&s.startsWith("/dashboards")?(e=>i.ae.getFolderByUid(e).then((e=>{const a=(0,c.B)(e);return a.children[0].active=!0,{folder:e,folderNav:a}})))(e).then((e=>{let{folder:a,folderNav:s}=e;const n=t.locationUtil.stripBaseFromUrl(a.url);return n!==r.pathname&&l.locationService.replace(n),{folder:a,pageNav:s}})):Promise.resolve({})}),[a.params.uid]);return(0,j.jsx)(d.T,{navId:"dashboards/browse",pageNav:null==u?void 0:u.pageNav,children:(0,j.jsx)(d.T.Contents,{isLoading:n,className:s.css`
          display: flex;
          flex-direction: column;
          overflow: hidden;
        `,children:(0,j.jsx)(C,{folder:null==u?void 0:u.folder})})})}));k.displayName="DashboardListPage";const F=k}}]);
//# sourceMappingURL=DashboardListPage.0099be6c25a1c7f4ed52.js.map
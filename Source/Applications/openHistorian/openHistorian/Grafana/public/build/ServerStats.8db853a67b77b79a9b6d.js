"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[7641],{83331:(e,s,a)=>{a.r(s),a.d(s,{ServerStats:()=>k});var t=a(36636),n=a(68404),r=a(90923),i=a(3490),c=a(47570),o=a(61959),l=a(56008),d=a(43215),u=a(45916);const h=()=>{var e;const s=m((0,i.useTheme2)()),[a,t]=(0,n.useState)(!1),[c,o]=(0,n.useState)({mode:"thumbs",theme:r.config.theme2.isLight?"light":"dark"}),l=()=>t(!1);return(0,u.jsxs)(u.Fragment,{children:[(0,u.jsxs)(i.Modal,{title:"Start crawler",isOpen:a,onDismiss:l,children:[(0,u.jsx)("div",{className:s.wrap,children:(0,u.jsx)(i.CodeEditor,{height:200,value:null!==(e=JSON.stringify(c,null,2))&&void 0!==e?e:"",showLineNumbers:!1,readOnly:!1,language:"json",showMiniMap:!1,onBlur:e=>{o(JSON.parse(e))}})}),(0,u.jsxs)(i.Modal.ButtonRow,{children:[(0,u.jsx)(i.Button,{type:"submit",onClick:()=>{(0,r.getBackendSrv)().post("/api/admin/crawler/start",c).then((e=>{console.log("GOT",e),l()}))},children:"Start"}),(0,u.jsx)(i.Button,{variant:"secondary",onClick:l,children:"Cancel"})]})]}),(0,u.jsx)(i.Button,{onClick:()=>t(!0),variant:"primary",children:"Start"})]})},m=e=>({wrap:t.css`
      border: 2px solid #111;
    `});var v,g,p;const x=()=>{const e=f((0,i.useTheme2)()),[s,a]=(0,n.useState)();return(0,n.useEffect)((()=>{const e=(0,r.getGrafanaLiveSrv)().getStream({scope:d.LiveChannelScope.Grafana,namespace:"broadcast",path:"crawler"}).subscribe({next:e=>{((0,d.isLiveChannelMessageEvent)(e)||(0,d.isLiveChannelStatusEvent)(e))&&a(e.message)}});return()=>{e.unsubscribe()}}),[]),s?(0,u.jsxs)("div",{className:e.wrap,children:[(0,u.jsx)("pre",{children:JSON.stringify(s,null,2)}),"running"!==s.state&&(p||(p=(0,u.jsx)(h,{}))),"stopped"!==s.state&&(0,u.jsx)(i.Button,{variant:"secondary",onClick:()=>{(0,r.getBackendSrv)().post("/api/admin/crawler/stop")},children:"Stop"})]}):(0,u.jsxs)("div",{className:e.wrap,children:["No status (never run)",v||(v=(0,u.jsx)("br",{})),g||(g=(0,u.jsx)(h,{}))]})},f=e=>({wrap:t.css`
      border: 4px solid red;
    `,running:t.css`
      border: 4px solid green;
    `});var j,b,S,w,y,N;const k=()=>{const[e,s]=(0,n.useState)(null),[a,t]=(0,n.useState)(!1),d=(0,i.useStyles2)(A),h=o.Vt.hasAccess(c.bW.DataSourcesRead,o.Vt.isGrafanaAdmin),m=o.Vt.hasAccess(c.bW.UsersRead,o.Vt.isGrafanaAdmin);return(0,n.useEffect)((()=>{o.Vt.hasAccess(c.bW.ActionServerStatsRead,o.Vt.isGrafanaAdmin)&&(t(!0),(async()=>(0,r.getBackendSrv)().get("api/admin/stats").catch((e=>(console.error(e),null))))().then((e=>{s(e),t(!1)})))}),[]),o.Vt.hasAccess(c.bW.ActionServerStatsRead,o.Vt.isGrafanaAdmin)?(0,u.jsxs)(u.Fragment,{children:[(0,u.jsx)("h2",{className:d.title,children:"Instance statistics"}),a?(0,u.jsx)("div",{className:d.loader,children:j||(j=(0,u.jsx)(l.a,{text:"Loading instance stats..."}))}):e?(0,u.jsxs)("div",{className:d.row,children:[(0,u.jsx)(B,{content:[{name:"Dashboards (starred)",value:`${e.dashboards} (${e.stars})`},{name:"Tags",value:e.tags},{name:"Playlists",value:e.playlists},{name:"Snapshots",value:e.snapshots}],footer:b||(b=(0,u.jsx)(i.LinkButton,{href:"/dashboards",variant:"secondary",children:"Manage dashboards"}))}),(0,u.jsxs)("div",{className:d.doubleRow,children:[(0,u.jsx)(B,{content:[{name:"Data sources",value:e.datasources}],footer:h&&(S||(S=(0,u.jsx)(i.LinkButton,{href:"/datasources",variant:"secondary",children:"Manage data sources"})))}),(0,u.jsx)(B,{content:[{name:"Alerts",value:e.alerts}],footer:w||(w=(0,u.jsx)(i.LinkButton,{href:"/alerting/list",variant:"secondary",children:"Alerts"}))})]}),(0,u.jsx)(B,{content:[{name:"Organisations",value:e.orgs},{name:"Users total",value:e.users},{name:"Active users in last 30 days",value:e.activeUsers},{name:"Active sessions",value:e.activeSessions}],footer:m&&(y||(y=(0,u.jsx)(i.LinkButton,{href:"/admin/users",variant:"secondary",children:"Manage users"})))})]}):(0,u.jsx)("p",{className:d.notFound,children:"No stats found."}),r.config.featureToggles.dashboardPreviews&&r.config.featureToggles.dashboardPreviewsAdmin&&(N||(N=(0,u.jsx)(x,{})))]}):null},A=e=>({title:t.css`
      margin-bottom: ${e.spacing(4)};
    `,row:t.css`
      display: flex;
      justify-content: space-between;
      width: 100%;

      & > div:not(:last-of-type) {
        margin-right: ${e.spacing(2)};
      }

      & > div {
        width: 33.3%;
      }
    `,doubleRow:t.css`
      display: flex;
      flex-direction: column;

      & > div:first-of-type {
        margin-bottom: ${e.spacing(2)};
      }
    `,loader:t.css`
      height: 290px;
    `,notFound:t.css`
      font-size: ${e.typography.h6.fontSize};
      text-align: center;
      height: 290px;
    `}),B=e=>{let{content:s,footer:a}=e;const t=(0,i.useStyles2)(C);return(0,u.jsx)(i.CardContainer,{className:t.container,disableHover:!0,children:(0,u.jsxs)("div",{className:t.inner,children:[(0,u.jsx)("div",{className:t.content,children:s.map((e=>(0,u.jsxs)("div",{className:t.row,children:[(0,u.jsx)("span",{children:e.name}),(0,u.jsx)("span",{children:e.value})]},e.name)))}),a&&(0,u.jsx)("div",{children:a})]})})},C=e=>({container:t.css`
      padding: ${e.spacing(2)};
    `,inner:t.css`
      display: flex;
      flex-direction: column;
      width: 100%;
    `,content:t.css`
      flex: 1 0 auto;
    `,row:t.css`
      display: flex;
      justify-content: space-between;
      width: 100%;
      margin-bottom: ${e.spacing(2)};
      align-items: center;
    `})},56008:(e,s,a)=>{a.d(s,{a:()=>r});a(68404);var t=a(3490),n=a(45916);const r=e=>{let{text:s="Loading..."}=e;return(0,n.jsx)("div",{className:"page-loader-wrapper",children:(0,n.jsx)(t.LoadingPlaceholder,{text:s})})}}}]);
//# sourceMappingURL=ServerStats.8db853a67b77b79a9b6d.js.map
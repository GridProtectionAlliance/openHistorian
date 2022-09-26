"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[3675,7641],{83331:(e,s,t)=>{t.r(s),t.d(s,{ServerStats:()=>k});var i=t(36636),n=t(68404),a=t(90923),r=t(3490),l=t(47570),c=t(61959),o=t(56008),d=t(43215),h=t(45916);const x=()=>{var e;const s=p((0,r.useTheme2)()),[t,i]=(0,n.useState)(!1),[l,c]=(0,n.useState)({mode:"thumbs",theme:a.config.theme2.isLight?"light":"dark"}),o=()=>i(!1);return(0,h.jsxs)(h.Fragment,{children:[(0,h.jsxs)(r.Modal,{title:"Start crawler",isOpen:t,onDismiss:o,children:[(0,h.jsx)("div",{className:s.wrap,children:(0,h.jsx)(r.CodeEditor,{height:200,value:null!==(e=JSON.stringify(l,null,2))&&void 0!==e?e:"",showLineNumbers:!1,readOnly:!1,language:"json",showMiniMap:!1,onBlur:e=>{c(JSON.parse(e))}})}),(0,h.jsxs)(r.Modal.ButtonRow,{children:[(0,h.jsx)(r.Button,{type:"submit",onClick:()=>{(0,a.getBackendSrv)().post("/api/admin/crawler/start",l).then((e=>{console.log("GOT",e),o()}))},children:"Start"}),(0,h.jsx)(r.Button,{variant:"secondary",onClick:o,children:"Cancel"})]})]}),(0,h.jsx)(r.Button,{onClick:()=>i(!0),variant:"primary",children:"Start"})]})},p=e=>({wrap:i.css`
      border: 2px solid #111;
    `});var u,g,j;const m=()=>{const e=v((0,r.useTheme2)()),[s,t]=(0,n.useState)();return(0,n.useEffect)((()=>{const e=(0,a.getGrafanaLiveSrv)().getStream({scope:d.LiveChannelScope.Grafana,namespace:"broadcast",path:"crawler"}).subscribe({next:e=>{((0,d.isLiveChannelMessageEvent)(e)||(0,d.isLiveChannelStatusEvent)(e))&&t(e.message)}});return()=>{e.unsubscribe()}}),[]),s?(0,h.jsxs)("div",{className:e.wrap,children:[(0,h.jsx)("pre",{children:JSON.stringify(s,null,2)}),"running"!==s.state&&(j||(j=(0,h.jsx)(x,{}))),"stopped"!==s.state&&(0,h.jsx)(r.Button,{variant:"secondary",onClick:()=>{(0,a.getBackendSrv)().post("/api/admin/crawler/stop")},children:"Stop"})]}):(0,h.jsxs)("div",{className:e.wrap,children:["No status (never run)",u||(u=(0,h.jsx)("br",{})),g||(g=(0,h.jsx)(x,{}))]})},v=e=>({wrap:i.css`
      border: 4px solid red;
    `,running:i.css`
      border: 4px solid green;
    `});var f,b,y,S,w,N;const k=()=>{const[e,s]=(0,n.useState)(null),[t,i]=(0,n.useState)(!1),d=(0,r.useStyles2)(A),x=c.Vt.hasAccess(l.bW.DataSourcesRead,c.Vt.isGrafanaAdmin),p=c.Vt.hasAccess(l.bW.UsersRead,c.Vt.isGrafanaAdmin);return(0,n.useEffect)((()=>{c.Vt.hasAccess(l.bW.ActionServerStatsRead,c.Vt.isGrafanaAdmin)&&(i(!0),(async()=>(0,a.getBackendSrv)().get("api/admin/stats").catch((e=>(console.error(e),null))))().then((e=>{s(e),i(!1)})))}),[]),c.Vt.hasAccess(l.bW.ActionServerStatsRead,c.Vt.isGrafanaAdmin)?(0,h.jsxs)(h.Fragment,{children:[(0,h.jsx)("h2",{className:d.title,children:"Instance statistics"}),t?(0,h.jsx)("div",{className:d.loader,children:f||(f=(0,h.jsx)(o.a,{text:"Loading instance stats..."}))}):e?(0,h.jsxs)("div",{className:d.row,children:[(0,h.jsx)(L,{content:[{name:"Dashboards (starred)",value:`${e.dashboards} (${e.stars})`},{name:"Tags",value:e.tags},{name:"Playlists",value:e.playlists},{name:"Snapshots",value:e.snapshots}],footer:b||(b=(0,h.jsx)(r.LinkButton,{href:"/dashboards",variant:"secondary",children:"Manage dashboards"}))}),(0,h.jsxs)("div",{className:d.doubleRow,children:[(0,h.jsx)(L,{content:[{name:"Data sources",value:e.datasources}],footer:x&&(y||(y=(0,h.jsx)(r.LinkButton,{href:"/datasources",variant:"secondary",children:"Manage data sources"})))}),(0,h.jsx)(L,{content:[{name:"Alerts",value:e.alerts}],footer:S||(S=(0,h.jsx)(r.LinkButton,{href:"/alerting/list",variant:"secondary",children:"Alerts"}))})]}),(0,h.jsx)(L,{content:[{name:"Organisations",value:e.orgs},{name:"Users total",value:e.users},{name:"Active users in last 30 days",value:e.activeUsers},{name:"Active sessions",value:e.activeSessions}],footer:p&&(w||(w=(0,h.jsx)(r.LinkButton,{href:"/admin/users",variant:"secondary",children:"Manage users"})))})]}):(0,h.jsx)("p",{className:d.notFound,children:"No stats found."}),a.config.featureToggles.dashboardPreviews&&a.config.featureToggles.dashboardPreviewsAdmin&&(N||(N=(0,h.jsx)(m,{})))]}):null},A=e=>({title:i.css`
      margin-bottom: ${e.spacing(4)};
    `,row:i.css`
      display: flex;
      justify-content: space-between;
      width: 100%;

      & > div:not(:last-of-type) {
        margin-right: ${e.spacing(2)};
      }

      & > div {
        width: 33.3%;
      }
    `,doubleRow:i.css`
      display: flex;
      flex-direction: column;

      & > div:first-of-type {
        margin-bottom: ${e.spacing(2)};
      }
    `,loader:i.css`
      height: 290px;
    `,notFound:i.css`
      font-size: ${e.typography.h6.fontSize};
      text-align: center;
      height: 290px;
    `}),L=e=>{let{content:s,footer:t}=e;const i=(0,r.useStyles2)(G);return(0,h.jsx)(r.CardContainer,{className:i.container,disableHover:!0,children:(0,h.jsxs)("div",{className:i.inner,children:[(0,h.jsx)("div",{className:i.content,children:s.map((e=>(0,h.jsxs)("div",{className:i.row,children:[(0,h.jsx)("span",{children:e.name}),(0,h.jsx)("span",{children:e.value})]},e.name)))}),t&&(0,h.jsx)("div",{children:t})]})})},G=e=>({container:i.css`
      padding: ${e.spacing(2)};
    `,inner:i.css`
      display: flex;
      flex-direction: column;
      width: 100%;
    `,content:i.css`
      flex: 1 0 auto;
    `,row:i.css`
      display: flex;
      justify-content: space-between;
      width: 100%;
      margin-bottom: ${e.spacing(2)};
      align-items: center;
    `})},23675:(e,s,t)=>{t.r(s),t.d(s,{UpgradeInfo:()=>C,UpgradePage:()=>G,default:()=>W});var i=t(36636),n=(t(68404),t(18745)),a=t(3490),r=t(69371),l=t(8674),c=t(45916);const o={fontWeight:500,fontSize:"26px",lineHeight:"123%"},d=(0,a.stylesFactory)((e=>{const s=e.isDark?"public/img/licensing/header_dark.svg":"public/img/licensing/header_light.svg",t=e.isDark?e.palette.dark9:e.palette.gray6;return{container:i.css`
      padding: 36px 79px;
      background: ${e.colors.panelBg};
    `,footer:i.css`
      text-align: center;
      padding: 16px;
      background: ${t};
    `,header:i.css`
      height: 137px;
      padding: 40px 0 0 79px;
      position: relative;
      background: url('${s}') right;
    `}}));function h(e){let{header:s,editionNotice:t,subheader:i,children:n}=e;const r=(0,a.useTheme)(),l=d(r);return(0,c.jsxs)(c.Fragment,{children:[(0,c.jsxs)("div",{className:l.header,children:[(0,c.jsx)("h2",{style:o,children:s}),i&&(0,c.jsx)("h3",{children:i}),(0,c.jsx)(x,{size:"128px",style:{boxShadow:"0px 0px 24px rgba(24, 58, 110, 0.45)",background:"#0A1C36",position:"absolute",top:"19px",left:"71%"},children:(0,c.jsx)("img",{src:"public/img/grafana_icon.svg",alt:"Grafana",width:"80px",style:{position:"absolute",left:"23px",top:"20px"}})})]}),(0,c.jsx)("div",{className:l.container,children:n}),t&&(0,c.jsx)("div",{className:l.footer,children:t})]})}const x=e=>{let{size:s,style:t,children:i}=e;return(0,c.jsx)("div",{style:Object.assign({width:s,height:s,position:"absolute",bottom:0,right:0,borderRadius:"50%"},t),children:i})};var p,u,g,j,m,v,f,b,y,S,w,N,k,A,L=t(83331);function G(e){let{navModel:s}=e;return(0,c.jsx)(r.T,{navModel:s,children:p||(p=(0,c.jsxs)(r.T.Contents,{children:[(0,c.jsx)(L.ServerStats,{}),(0,c.jsx)(C,{editionNotice:"You are running the open-source version of Grafana. You have to install the Enterprise edition in order enable Enterprise features."})]}))})}const B={fontWeight:500,fontSize:"26px",lineHeight:"123%"},C=e=>{let{editionNotice:s}=e;const t=(0,a.useStyles2)(D);return(0,c.jsxs)(c.Fragment,{children:[(0,c.jsx)("h2",{className:t.title,children:"Enterprise license"}),(0,c.jsx)(h,{header:"Grafana Enterprise",subheader:"Get your free trial",editionNotice:s,children:(0,c.jsxs)("div",{className:t.column,children:[u||(u=(0,c.jsx)($,{})),g||(g=(0,c.jsx)(T,{}))]})})]})},D=e=>({column:i.css`
      display: grid;
      grid-template-columns: 100%;
      column-gap: 20px;
      row-gap: 40px;

      @media (min-width: 1050px) {
        grid-template-columns: 50% 50%;
      }
    `,title:i.css`
      margin: ${e.spacing(4)} 0;
    `}),E=()=>(0,c.jsxs)("div",{style:{marginTop:"40px",marginBottom:"30px"},children:[j||(j=(0,c.jsx)("h2",{style:B,children:"Get Grafana Enterprise"})),m||(m=(0,c.jsx)(M,{})),(0,c.jsx)("p",{style:{paddingTop:"12px"},children:"You can use the trial version for free for 30 days. We will remind you about it five days before the trial period ends."})]}),M=()=>v||(v=(0,c.jsx)(a.LinkButton,{variant:"primary",size:"lg",href:"https://grafana.com/contact?about=grafana-enterprise&utm_source=grafana-upgrade-page",children:"Contact us and get a free trial"})),T=()=>(0,c.jsxs)("div",{children:[f||(f=(0,c.jsx)("h4",{children:"At your service"})),b||(b=(0,c.jsxs)(O,{children:[(0,c.jsx)(R,{title:"Enterprise Plugins",image:"public/img/licensing/plugin_enterprise.svg"}),(0,c.jsx)(R,{title:"Critical SLA: 2 hours",image:"public/img/licensing/sla.svg"}),(0,c.jsxs)(R,{title:"Unlimited Expert Support",image:"public/img/licensing/customer_support.svg",children:["24 x 7 x 365 support via",(0,c.jsxs)(O,{nested:!0,children:[(0,c.jsx)(R,{title:"Email"}),(0,c.jsx)(R,{title:"Private Slack channel"}),(0,c.jsx)(R,{title:"Phone"})]})]}),(0,c.jsx)(R,{title:"Hand-in-hand support",image:"public/img/licensing/handinhand_support.svg",children:"in the upgrade process"})]})),(0,c.jsxs)("div",{style:{marginTop:"20px"},children:[y||(y=(0,c.jsx)("strong",{children:"Also included:"})),S||(S=(0,c.jsx)("br",{})),"Indemnification, working with Grafana Labs on future prioritization, and training from the core Grafana team."]}),w||(w=(0,c.jsx)(E,{}))]}),$=()=>(0,c.jsxs)("div",{style:{paddingRight:"11px"},children:[N||(N=(0,c.jsx)("h4",{children:"Enhanced functionality"})),k||(k=(0,c.jsx)(P,{}))]}),P=()=>A||(A=(0,c.jsxs)(O,{children:[(0,c.jsx)(R,{title:"Data source permissions"}),(0,c.jsx)(R,{title:"Reporting"}),(0,c.jsx)(R,{title:"SAML authentication"}),(0,c.jsx)(R,{title:"Enhanced LDAP integration"}),(0,c.jsx)(R,{title:"Team Sync",children:"LDAP, GitHub OAuth, Auth Proxy, Okta"}),(0,c.jsx)(R,{title:"White labeling"}),(0,c.jsx)(R,{title:"Auditing"}),(0,c.jsx)(R,{title:"Settings updates at runtime"}),(0,c.jsx)(R,{title:"Grafana usage insights",children:(0,c.jsxs)(O,{nested:!0,children:[(0,c.jsx)(R,{title:"Sort dashboards by popularity in search"}),(0,c.jsx)(R,{title:"Find unused dashboards"}),(0,c.jsx)(R,{title:"Dashboard usage stats drawer"}),(0,c.jsx)(R,{title:"Dashboard presence indicators"})]})}),(0,c.jsx)(R,{title:"Enterprise plugins",children:(0,c.jsxs)(O,{nested:!0,children:[(0,c.jsx)(R,{title:"Oracle"}),(0,c.jsx)(R,{title:"Splunk"}),(0,c.jsx)(R,{title:"Service Now"}),(0,c.jsx)(R,{title:"Dynatrace"}),(0,c.jsx)(R,{title:"New Relic"}),(0,c.jsx)(R,{title:"DataDog"}),(0,c.jsx)(R,{title:"AppDynamics"}),(0,c.jsx)(R,{title:"SAP HANA®"}),(0,c.jsx)(R,{title:"Gitlab"}),(0,c.jsx)(R,{title:"Honeycomb"}),(0,c.jsx)(R,{title:"Jira"}),(0,c.jsx)(R,{title:"MongoDB"}),(0,c.jsx)(R,{title:"Salesforce"}),(0,c.jsx)(R,{title:"Snowflake"}),(0,c.jsx)(R,{title:"Wavefront"})]})})]})),O=e=>{let{children:s,nested:t}=e;const n=i.css`
    display: flex;
    flex-direction: column;
    padding-top: 8px;

    > div {
      margin-bottom: ${t?0:8}px;
    }
  `;return(0,c.jsx)("div",{className:n,children:s})},R=e=>{let{children:s,title:t,image:n}=e;const a=n||"public/img/licensing/checkmark.svg",r=i.css`
    display: flex;

    > img {
      display: block;
      height: 22px;
      flex-grow: 0;
      padding-right: 12px;
    }
  `,l=i.css`
    font-weight: 500;
    line-height: 1.7;
  `;return(0,c.jsxs)("div",{className:r,children:[(0,c.jsx)("img",{src:a,alt:""}),(0,c.jsxs)("div",{children:[(0,c.jsx)("div",{className:l,children:t}),s]})]})},W=(0,n.connect)((e=>({navModel:(0,l.h)(e.navIndex,"upgrading")})))(G)},56008:(e,s,t)=>{t.d(s,{a:()=>a});t(68404);var i=t(3490),n=t(45916);const a=e=>{let{text:s="Loading..."}=e;return(0,n.jsx)("div",{className:"page-loader-wrapper",children:(0,n.jsx)(i.LoadingPlaceholder,{text:s})})}}}]);
//# sourceMappingURL=3675.0cde91be6bd460c6528d.js.map
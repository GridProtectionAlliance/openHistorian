"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[2446,7641],{93976:(T,u,n)=>{n.r(u),n.d(u,{ServerStats:()=>N});var s=n(9892),e=n(68404),h=n(72648),o=n(31403),x=n(35633),g=n(81168),c=n(82002),S=n(43714),A=n(54899);const b=async()=>(0,A.i)().get("api/admin/stats").catch(a=>(console.error(a),null)),N=()=>{const[a,f]=(0,e.useState)(null),[m,d]=(0,e.useState)(!1),E=(0,h.wW)(C),P=c.Vt.hasAccess(g.AccessControlAction.DataSourcesRead,c.Vt.isGrafanaAdmin),G=c.Vt.hasAccess(g.AccessControlAction.UsersRead,c.Vt.isGrafanaAdmin);return(0,e.useEffect)(()=>{c.Vt.hasAccess(g.AccessControlAction.ActionServerStatsRead,c.Vt.isGrafanaAdmin)&&(d(!0),b().then(v=>{f(v),d(!1)}))},[]),c.Vt.hasAccess(g.AccessControlAction.ActionServerStatsRead,c.Vt.isGrafanaAdmin)?e.createElement(e.Fragment,null,e.createElement("h2",{className:E.title},"Instance statistics"),m?e.createElement("div",{className:E.loader},e.createElement(S.a,{text:"Loading instance stats..."})):a?e.createElement("div",{className:E.row},e.createElement(p,{content:[{name:"Dashboards (starred)",value:`${a.dashboards} (${a.stars})`},{name:"Tags",value:a.tags},{name:"Playlists",value:a.playlists},{name:"Snapshots",value:a.snapshots}],footer:e.createElement(o.Qj,{href:"/dashboards",variant:"secondary"},"Manage dashboards")}),e.createElement("div",{className:E.doubleRow},e.createElement(p,{content:[{name:"Data sources",value:a.datasources}],footer:P&&e.createElement(o.Qj,{href:"/datasources",variant:"secondary"},"Manage data sources")}),e.createElement(p,{content:[{name:"Alerts",value:a.alerts}],footer:e.createElement(o.Qj,{href:"/alerting/list",variant:"secondary"},"Alerts")})),e.createElement(p,{content:[{name:"Organisations",value:a.orgs},{name:"Users total",value:a.users},{name:"Active users in last 30 days",value:a.activeUsers},{name:"Active sessions",value:a.activeSessions}],footer:G&&e.createElement(o.Qj,{href:"/admin/users",variant:"secondary"},"Manage users")})):e.createElement("p",{className:E.notFound},"No stats found.")):null},C=a=>({title:s.css`
      margin-bottom: ${a.spacing(4)};
    `,row:s.css`
      display: flex;
      justify-content: space-between;
      width: 100%;

      & > div:not(:last-of-type) {
        margin-right: ${a.spacing(2)};
      }

      & > div {
        width: 33.3%;
      }
    `,doubleRow:s.css`
      display: flex;
      flex-direction: column;

      & > div:first-of-type {
        margin-bottom: ${a.spacing(2)};
      }
    `,loader:s.css`
      height: 290px;
    `,notFound:s.css`
      font-size: ${a.typography.h6.fontSize};
      text-align: center;
      height: 290px;
    `}),p=({content:a,footer:f})=>{const m=(0,h.wW)(D);return e.createElement(x._,{className:m.container,disableHover:!0},e.createElement("div",{className:m.inner},e.createElement("div",{className:m.content},a.map(d=>e.createElement("div",{key:d.name,className:m.row},e.createElement("span",null,d.name),e.createElement("span",null,d.value)))),f&&e.createElement("div",null,f)))},D=a=>({container:s.css`
      padding: ${a.spacing(2)};
    `,inner:s.css`
      display: flex;
      flex-direction: column;
      width: 100%;
    `,content:s.css`
      flex: 1 0 auto;
    `,row:s.css`
      display: flex;
      justify-content: space-between;
      width: 100%;
      margin-bottom: ${a.spacing(2)};
      align-items: center;
    `})},62446:(T,u,n)=>{n.r(u),n.d(u,{UpgradeInfo:()=>a,UpgradePage:()=>p,default:()=>w});var s=n(9892),e=n(68404),h=n(36635),o=n(72648),x=n(31403),g=n(79396),c=n(86245);const S={fontWeight:500,fontSize:"26px",lineHeight:"123%"},A=r=>{const l=r.isDark?"public/img/licensing/header_dark.svg":"public/img/licensing/header_light.svg",i=r.isDark?r.v1.palette.dark9:r.v1.palette.gray6;return{container:s.css`
      padding: 36px 79px;
      background: ${r.components.panel.background};
    `,footer:s.css`
      text-align: center;
      padding: 16px;
      background: ${i};
    `,header:s.css`
      height: 137px;
      padding: 40px 0 0 79px;
      position: relative;
      background: url('${l}') right;
    `}};function b({header:r,editionNotice:l,subheader:i,children:L}){const y=(0,o.wW)(A);return e.createElement(e.Fragment,null,e.createElement("div",{className:y.header},e.createElement("h2",{style:S},r),i&&e.createElement("h3",null,i),e.createElement(N,{size:"128px",style:{boxShadow:"0px 0px 24px rgba(24, 58, 110, 0.45)",background:"#0A1C36",position:"absolute",top:"19px",left:"71%"}},e.createElement("img",{src:"public/img/grafana_icon.svg",alt:"Grafana",width:"80px",style:{position:"absolute",left:"23px",top:"20px"}}))),e.createElement("div",{className:y.container},L),l&&e.createElement("div",{className:y.footer},l))}const N=({size:r,style:l,children:i})=>e.createElement("div",{style:{width:r,height:r,position:"absolute",bottom:0,right:0,borderRadius:"50%",...l}},i);var C=n(93976);function p({navModel:r}){return e.createElement(g.T,{navModel:r},e.createElement(g.T.Contents,null,e.createElement(C.ServerStats,null),e.createElement(a,{editionNotice:`You are running the open-source version of Grafana.
        You have to install the Enterprise edition in order enable Enterprise features.`})))}const D={fontWeight:500,fontSize:"26px",lineHeight:"123%"},a=({editionNotice:r})=>{const l=(0,o.wW)(f);return e.createElement(e.Fragment,null,e.createElement("h2",{className:l.title},"Enterprise license"),e.createElement(b,{header:"Grafana Enterprise",subheader:"Get your free trial",editionNotice:r},e.createElement("div",{className:l.column},e.createElement(P,null),e.createElement(E,null))))},f=r=>({column:s.css`
      display: grid;
      grid-template-columns: 100%;
      column-gap: 20px;
      row-gap: 40px;

      @media (min-width: 1050px) {
        grid-template-columns: 50% 50%;
      }
    `,title:s.css`
      margin: ${r.spacing(4)} 0;
    `}),m=()=>e.createElement("div",{style:{marginTop:"40px",marginBottom:"30px"}},e.createElement("h2",{style:D},"Get Grafana Enterprise"),e.createElement(d,null),e.createElement("p",{style:{paddingTop:"12px"}},"You can use the trial version for free for 30 days. We will remind you about it five days before the trial period ends.")),d=()=>e.createElement(x.Qj,{variant:"primary",size:"lg",href:"https://grafana.com/contact?about=grafana-enterprise&utm_source=grafana-upgrade-page"},"Contact us and get a free trial"),E=()=>e.createElement("div",null,e.createElement("h4",null,"At your service"),e.createElement(v,null,e.createElement(t,{title:"Enterprise Plugins",image:"public/img/licensing/plugin_enterprise.svg"}),e.createElement(t,{title:"Critical SLA: 2 hours",image:"public/img/licensing/sla.svg"}),e.createElement(t,{title:"Unlimited Expert Support",image:"public/img/licensing/customer_support.svg"},"24 x 7 x 365 support via",e.createElement(v,{nested:!0},e.createElement(t,{title:"Email"}),e.createElement(t,{title:"Private Slack channel"}),e.createElement(t,{title:"Phone"}))),e.createElement(t,{title:"Hand-in-hand support",image:"public/img/licensing/handinhand_support.svg"},"in the upgrade process")),e.createElement("div",{style:{marginTop:"20px"}},e.createElement("strong",null,"Also included:"),e.createElement("br",null),"Indemnification, working with Grafana Labs on future prioritization, and training from the core Grafana team."),e.createElement(m,null)),P=()=>e.createElement("div",{style:{paddingRight:"11px"}},e.createElement("h4",null,"Enhanced functionality"),e.createElement(G,null)),G=()=>e.createElement(v,null,e.createElement(t,{title:"Data source permissions"}),e.createElement(t,{title:"Reporting"}),e.createElement(t,{title:"SAML authentication"}),e.createElement(t,{title:"Enhanced LDAP integration"}),e.createElement(t,{title:"Team Sync"},"LDAP, GitHub OAuth, Auth Proxy, Okta"),e.createElement(t,{title:"White labeling"}),e.createElement(t,{title:"Auditing"}),e.createElement(t,{title:"Settings updates at runtime"}),e.createElement(t,{title:"Grafana usage insights"},e.createElement(v,{nested:!0},e.createElement(t,{title:"Sort dashboards by popularity in search"}),e.createElement(t,{title:"Find unused dashboards"}),e.createElement(t,{title:"Dashboard usage stats drawer"}),e.createElement(t,{title:"Dashboard presence indicators"}))),e.createElement(t,{title:"Enterprise plugins"},e.createElement(v,{nested:!0},e.createElement(t,{title:"Oracle"}),e.createElement(t,{title:"Splunk"}),e.createElement(t,{title:"Service Now"}),e.createElement(t,{title:"Dynatrace"}),e.createElement(t,{title:"New Relic"}),e.createElement(t,{title:"DataDog"}),e.createElement(t,{title:"AppDynamics"}),e.createElement(t,{title:"SAP HANA\xAE"}),e.createElement(t,{title:"Gitlab"}),e.createElement(t,{title:"Honeycomb"}),e.createElement(t,{title:"Jira"}),e.createElement(t,{title:"MongoDB"}),e.createElement(t,{title:"Salesforce"}),e.createElement(t,{title:"Snowflake"}),e.createElement(t,{title:"Wavefront"})))),v=({children:r,nested:l})=>{const i=s.css`
    display: flex;
    flex-direction: column;
    padding-top: 8px;

    > div {
      margin-bottom: ${l?0:8}px;
    }
  `;return e.createElement("div",{className:i},r)},t=({children:r,title:l,image:i})=>{const L=i||"public/img/licensing/checkmark.svg",y=s.css`
    display: flex;

    > img {
      display: block;
      height: 22px;
      flex-grow: 0;
      padding-right: 12px;
    }
  `,$=s.css`
    font-weight: 500;
    line-height: 1.7;
  `;return e.createElement("div",{className:y},e.createElement("img",{src:L,alt:""}),e.createElement("div",null,e.createElement("div",{className:$},l),r))},U=r=>({navModel:(0,c.ht)(r.navIndex,"upgrading")}),w=(0,h.connect)(U)(p)},43714:(T,u,n)=>{n.d(u,{a:()=>h});var s=n(68404),e=n(61744);const h=({text:o="Loading..."})=>s.createElement("div",{className:"page-loader-wrapper"},s.createElement(e.u,{text:o}))}}]);

//# sourceMappingURL=2446.95802c7499f012cb9f57.js.map
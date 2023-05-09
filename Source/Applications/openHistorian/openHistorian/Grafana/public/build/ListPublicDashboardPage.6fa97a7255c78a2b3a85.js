"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[1344],{10429:(V,m,t)=>{t.r(m),t.d(m,{ListPublicDashboardPage:()=>f,default:()=>C});var e=t(68404),h=t(79396),s=t(9892),v=t(98101),g=t(68183),u=t(72648),b=t(67487),p=t(6554),x=t(29460),r=t(39904),P=t(34719),T=t(26202),E=t(31403),y=t(47694),D=t(82002),$=t(10655),I=t(84532),N=t(81168),L=t(80600);const z=n=>`${(0,y.iE)().appUrl}public-dashboards/${n}`,S=()=>{const{width:n}=(0,v.Z)(),c=n<=480,J=(0,u.l4)(),l=(0,u.wW)(()=>B(J,c)),{data:U,isLoading:W,isFetching:j}=(0,$.WJ)(),d=g.wl.pages.PublicDashboards,A=D.Vt.hasAccess(N.AccessControlAction.DashboardsPublicWrite,(0,I.RN)()),i=c?"sm":"md";return e.createElement(h.T.Contents,{isLoading:W},e.createElement("table",{className:"filter-table"},e.createElement("thead",null,e.createElement("tr",null,e.createElement("th",{className:l.nameTh},"Name"),e.createElement("th",null,"Status"),e.createElement("th",{className:l.fetchingSpinner},j&&e.createElement(b.$,null)))),e.createElement("tbody",null,U?.map(a=>{const o=!a.dashboardUid;return e.createElement("tr",{key:a.uid},e.createElement("td",{className:l.titleTd},e.createElement(p.u,{content:o?"The linked dashboard has already been deleted":a.title,placement:"top"},o?e.createElement("div",{className:l.orphanedTitle},e.createElement("p",null,"Orphaned public dashboard"),e.createElement(r.J,{name:"info-circle",className:l.orphanedInfoIcon})):e.createElement(x.r,{className:l.link,href:`/d/${a.dashboardUid}`},a.title))),e.createElement("td",null,e.createElement(P.V,{name:a.isEnabled?"enabled":"paused",colorIndex:o?9:a.isEnabled?20:15})),e.createElement("td",null,e.createElement(T.h,{className:l.buttonGroup},e.createElement(E.Qj,{href:z(a.accessToken),fill:"text",size:i,title:a.isEnabled?"View public dashboard":"Public dashboard is disabled",target:"_blank",disabled:!a.isEnabled||o,"data-testid":d.ListItem.linkButton},e.createElement(r.J,{size:i,name:"external-link-alt"})),e.createElement(E.Qj,{fill:"text",size:i,href:`/d/${a.dashboardUid}?shareView=share`,title:"Configure public dashboard",disabled:o,"data-testid":d.ListItem.configButton},e.createElement(r.J,{size:i,name:"cog"})),A&&e.createElement(L.H,{variant:"primary",fill:"text","data-testid":d.ListItem.trashcanButton,publicDashboard:a,loader:e.createElement(b.$,null)},e.createElement(r.J,{size:i,name:"trash-alt"})))))}))))};function B(n,c){return{fetchingSpinner:s.css`
      display: flex;
      justify-content: end;
    `,link:s.css`
      color: ${n.colors.primary.text};
      text-decoration: underline;
      margin-right: ${n.spacing()};
    `,nameTh:s.css`
      width: 20%;
    `,titleTd:s.css`
      max-width: 0;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    `,buttonGroup:s.css`
      justify-content: ${c?"space-between":"end"};
    `,orphanedTitle:s.css`
      display: flex;
      align-items: center;
      gap: ${n.spacing(1)};

      p {
        margin: ${n.spacing(0)};
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }
    `,orphanedInfoIcon:s.css`
      color: ${n.colors.text.link};
    `}}const f=({})=>e.createElement(h.T,{navId:"dashboards/public"},e.createElement(S,null)),C=f}}]);

//# sourceMappingURL=ListPublicDashboardPage.6fa97a7255c78a2b3a85.js.map
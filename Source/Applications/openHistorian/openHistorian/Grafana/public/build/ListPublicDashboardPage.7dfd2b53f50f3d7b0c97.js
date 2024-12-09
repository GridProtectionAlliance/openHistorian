"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[5761],{53371:(O,P,s)=>{s.r(P),s.d(P,{ListPublicDashboardPage:()=>N,default:()=>F});var a=s(74848),c=s(32196),D=s(96540),U=s(49045),R=s(13544),x=s(32264),A=s(14110),m=s(40845),p=s(10860),B=s(15292),v=s(55852),Y=s(62930),C=s(69529),S=s(72109),z=s(66864),E=s(19384),L=s(21780),o=s(8484),I=s(16233),f=s(72686),$=s(72254),W=s(31678),M=s(71678),Z=s(90591);const k=({dashboard:t,publicDashboard:d,loader:r,children:n,onDismiss:l,...b})=>{const[e,{isLoading:y}]=(0,f.bN)(),j=(h,i)=>{e({dashboard:t,uid:h.uid,dashboardUid:h.dashboardUid}),i()};return(0,a.jsx)(M.$s,{children:({showModal:h,hideModal:i})=>{const g=x.$.featureToggles.newDashboardSharingComponent?(0,o.t)("shared-dashboard-list.button.revoke-button-text","Revoke access"):(0,o.t)("public-dashboard-list.button.revoke-button-text","Revoke public URL");return(0,a.jsx)(v.$n,{"aria-label":g,title:g,onClick:()=>h(Z.m,{onConfirm:()=>j(d,i),onDismiss:()=>{l?l():i()}}),...b,children:y&&r?r:n})}})},K=({pd:t})=>{const d=(0,m.of)(T),r=(0,m.$j)(),n=(0,U.A)(`(max-width: ${r.breakpoints.values.sm}px)`),[l,{isLoading:b}]=(0,f.T2)(),e=R.Tp.pages.PublicDashboards,y=I.TP.hasPermission(W.AccessControlAction.DashboardsPublicWrite),j=(u,G)=>{const H={dashboard:{uid:u.dashboardUid},payload:{uid:u.uid,isEnabled:!G}};l(H)},h=(0,D.useMemo)(()=>n?p.Z.Actions:p.Z.SecondaryActions,[n]),i=x.$.featureToggles.newDashboardSharingComponent,g=i?(0,o.t)("shared-dashboard-list.toggle.pause-sharing-toggle-text","Pause access"):(0,o.t)("public-dashboard-list.toggle.pause-sharing-toggle-text","Pause sharing");return(0,a.jsxs)(p.Z,{className:d.card,href:`/d/${t.dashboardUid}`,children:[(0,a.jsx)(p.Z.Heading,{className:d.heading,children:(0,a.jsx)("span",{children:t.title})}),(0,a.jsxs)(h,{className:d.actions,children:[(0,a.jsxs)("div",{className:d.pauseSwitch,children:[(0,a.jsx)(B.d,{value:!t.isEnabled,label:g,disabled:b,onChange:u=>{(0,A.rR)("grafana_dashboards_public_enable_clicked",{action:u.currentTarget.checked?"disable":"enable"}),j(t,u.currentTarget.checked)},"data-testid":e.ListItem.pauseSwitch}),(0,a.jsx)("span",{children:g})]}),(0,a.jsx)(v.z9,{fill:"text",icon:"external-link-alt",variant:"secondary",target:"_blank",color:r.colors.warning.text,href:(0,$.mL)(t.accessToken),tooltip:i?(0,o.t)("shared-dashboard-list.button.view-button-tooltip","View shared dashboard"):(0,o.t)("public-dashboard-list.button.view-button-tooltip","View public dashboard"),"data-testid":e.ListItem.linkButton},"public-dashboard-url"),(0,a.jsx)(v.z9,{fill:"text",icon:"cog",variant:"secondary",color:r.colors.warning.text,href:(0,$.gN)(t.dashboardUid,t.slug),tooltip:i?(0,o.t)("shared-dashboard-list.button.config-button-tooltip","Configure shared dashboard"):(0,o.t)("public-dashboard-list.button.config-button-tooltip","Configure public dashboard"),"data-testid":e.ListItem.configButton},"public-dashboard-config-url"),y&&(0,a.jsx)(k,{fill:"text",icon:"trash-alt",variant:"secondary",publicDashboard:t,tooltip:i?(0,o.t)("shared-dashboard-list.button.revoke-button-tooltip","Revoke access"):(0,o.t)("public-dashboard-list.button.revoke-button-tooltip","Revoke public dashboard URL"),loader:(0,a.jsx)(Y.y,{}),"data-testid":e.ListItem.trashcanButton})]})]})},V=()=>{const[t,d]=(0,D.useState)(1),r=(0,m.of)(T),{data:n,isLoading:l,isError:b}=(0,f._e)(t);return(0,a.jsx)(L.YW,{navId:"dashboards/public",children:(0,a.jsx)(L.YW.Contents,{isLoading:l,children:!l&&!b&&!!n&&(0,a.jsx)("div",{children:n.publicDashboards.length===0?x.$.featureToggles.newDashboardSharingComponent?(0,a.jsx)(C.p,{variant:"call-to-action",message:(0,o.t)("shared-dashboard-list.empty-state.message","You haven't created any shared dashboards yet"),children:(0,a.jsxs)(o.x6,{i18nKey:"shared-dashboard-list.empty-state.more-info",children:["Create a shared dashboard from any existing dashboard through the ",(0,a.jsx)("b",{children:"Share"})," modal."," ",(0,a.jsx)(S.Y,{external:!0,href:"https://grafana.com/docs/grafana/latest/dashboards/share-dashboards-panels/shared-dashboards",children:"Learn more"})]})}):(0,a.jsx)(C.p,{variant:"call-to-action",message:(0,o.t)("public-dashboard-list.empty-state.message","You haven't created any public dashboards yet"),children:(0,a.jsxs)(o.x6,{i18nKey:"public-dashboard-list.empty-state.more-info",children:["Create a public dashboard from any existing dashboard through the ",(0,a.jsx)("b",{children:"Share"})," modal."," ",(0,a.jsx)(S.Y,{external:!0,href:"https://grafana.com/docs/grafana/latest/dashboards/dashboard-public/#make-a-dashboard-public",children:"Learn more"})]})}):(0,a.jsxs)(a.Fragment,{children:[(0,a.jsx)("ul",{className:r.list,children:n.publicDashboards.map(e=>(0,a.jsx)("li",{children:(0,a.jsx)(K,{pd:e})},e.uid))}),(0,a.jsx)(z.Gy,{justify:"flex-end",children:(0,a.jsx)(E.d,{onNavigate:d,currentPage:n.page,numberOfPages:n.totalPages,hideWhenSinglePage:!0})})]})})})})},T=t=>({list:(0,c.css)`
    list-style-type: none;
    margin-bottom: ${t.spacing(2)};
  `,card:(0,c.css)`
    ${t.breakpoints.up("sm")} {
      display: flex;
    }
  `,heading:(0,c.css)`
    display: flex;
    align-items: center;
    gap: ${t.spacing(1)};
    flex: 1;
  `,orphanedTitle:(0,c.css)`
    display: flex;
    align-items: center;
    gap: ${t.spacing(1)};
  `,actions:(0,c.css)`
    display: flex;
    align-items: center;
    position: relative;

    gap: ${t.spacing(.5)};
    ${t.breakpoints.up("sm")} {
      gap: ${t.spacing(1)};
    }
  `,pauseSwitch:(0,c.css)`
    display: flex;
    gap: ${t.spacing(1)};
    align-items: center;
    font-size: ${t.typography.bodySmall.fontSize};
    margin-bottom: 0;
    flex: 1;

    ${t.breakpoints.up("sm")} {
      padding-right: ${t.spacing(2)};
    }
  `}),N=({})=>(0,a.jsx)(V,{}),F=N}}]);

//# sourceMappingURL=ListPublicDashboardPage.7dfd2b53f50f3d7b0c97.js.map
"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[6034],{90237:(b,d,l)=>{l.r(d),l.d(d,{default:()=>E});var n=l(9892),e=l(68404),m=l(61549),g=l(26418),u=l(35645),i=l(72648),p=l(39904),f=l(45524);function E(){const t=(0,i.l4)(),a=(0,i.wW)(h);return e.createElement(f.J,{pageId:u.v.featureToggles.topnav?"alerting":"alert-home"},e.createElement("div",{className:a.grid},e.createElement(w,{className:a.ctaContainer}),e.createElement(s,{className:a.flowBlock},e.createElement("div",null,e.createElement("h3",null,"How it works"),e.createElement("ul",{className:a.list},e.createElement("li",null,"Grafana alerting periodically queries data sources and evaluates the condition defined in the alert rule"),e.createElement("li",null,"If the condition is breached, an alert instance fires"),e.createElement("li",null,"Firing instances are routed to notification policies based on matching labels"),e.createElement("li",null,"Notifications are sent out to the contact points specified in the notification policy"))),e.createElement(m.Z,{src:`public/img/alerting/at_a_glance_${t.name.toLowerCase()}.svg`,width:void 0,height:void 0})),e.createElement(s,{className:a.gettingStartedBlock},e.createElement("h3",null,"Get started"),e.createElement(g.Stack,{direction:"column",alignItems:"space-between"},e.createElement("ul",{className:a.list},e.createElement("li",null,e.createElement("strong",null,"Create an alert rule")," by adding queries and expressions from multiple data sources."),e.createElement("li",null,e.createElement("strong",null,"Add labels")," to your alert rules"," ",e.createElement("strong",null,"to connect them to notification policies")),e.createElement("li",null,e.createElement("strong",null,"Configure contact points")," to define where to send your notifications to."),e.createElement("li",null,e.createElement("strong",null,"Configure notification policies")," to route your alert instances to contact points.")),e.createElement("div",null,e.createElement(_,{href:"https://grafana.com/docs/grafana/latest/alerting/",title:"Read more in the Docs"})))),e.createElement(s,{className:a.videoBlock},e.createElement("iframe",{title:"Alerting - Introductory video",src:"https://player.vimeo.com/video/720001629?h=c6c1732f92",width:"960",height:"540",allow:"autoplay; fullscreen",allowFullScreen:!0,frameBorder:"0",style:{colorScheme:"light dark"}}))))}const h=t=>({grid:n.css`
    display: grid;
    grid-template-rows: min-content auto auto;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
    gap: ${t.spacing(2)};
  `,ctaContainer:n.css`
    grid-column: 1 / span 5;
  `,flowBlock:n.css`
    grid-column: 1 / span 5;

    display: flex;
    flex-wrap: wrap;
    gap: ${t.spacing(1)};

    & > div {
      flex: 2;
      min-width: 350px;
    }
    & > svg {
      flex: 3;
      min-width: 500px;
    }
  `,videoBlock:n.css`
    grid-column: 3 / span 3;
    grid-row: 3 / span 1;

    // Video required
    position: relative;
    padding: 56.25% 0 0 0; /* 16:9 */

    iframe {
      position: absolute;
      top: ${t.spacing(2)};
      left: ${t.spacing(2)};
      width: calc(100% - ${t.spacing(4)});
      height: calc(100% - ${t.spacing(4)});
      border: none;
    }
  `,gettingStartedBlock:n.css`
    grid-column: 1 / span 2;
    justify-content: space-between;
  `,list:n.css`
    margin: ${t.spacing(0,2)};
    & > li {
      margin-bottom: ${t.spacing(1)};
    }
  `});function w({className:t}){const a=(0,i.wW)(v);return e.createElement(s,{className:(0,n.cx)(a.ctaContainer,t)},e.createElement(c,{title:"Alert rules",description:"Define the condition that must be met before an alert rule fires",href:"/alerting/list",hrefText:"Manage alert rules"}),e.createElement("div",{className:a.separator}),e.createElement(c,{title:"Contact points",description:"Configure who receives notifications and how they are sent",href:"/alerting/notifications",hrefText:"Manage contact points"}),e.createElement("div",{className:a.separator}),e.createElement(c,{title:"Notification policies",description:"Configure how firing alert instances are routed to contact points",href:"/alerting/routes",hrefText:"Manage notification policies"}))}const v=t=>({ctaContainer:n.css`
    padding: ${t.spacing(4,2)};
    display: flex;
    gap: ${t.spacing(4)};
    justify-content: space-between;
    flex-wrap: wrap;

    ${t.breakpoints.down("lg")} {
      flex-direction: column;
    }
  `,separator:n.css`
    width: 1px;
    background-color: ${t.colors.border.medium};

    ${t.breakpoints.down("lg")} {
      display: none;
    }
  `});function c({title:t,description:a,href:r,hrefText:$}){const o=(0,i.wW)(y);return e.createElement("div",{className:o.container},e.createElement("h3",{className:o.title},t),e.createElement("div",{className:o.desc},a),e.createElement("div",{className:o.actionRow},e.createElement("a",{href:r,className:o.link},$)))}const y=t=>({container:n.css`
    flex: 1;
    min-width: 240px;
    display: grid;
    gap: ${t.spacing(1)};
    grid-template-columns: min-content 1fr 1fr 1fr;
    grid-template-rows: min-content auto min-content;
  `,title:n.css`
    margin-bottom: 0;
    grid-column: 2 / span 3;
    grid-row: 1;
  `,desc:n.css`
    grid-column: 2 / span 3;
    grid-row: 2;
  `,actionRow:n.css`
    grid-column: 2 / span 3;
    grid-row: 3;
    max-width: 240px;
  `,link:n.css`
    color: ${t.colors.text.link};
  `});function s({children:t,className:a}){const r=(0,i.wW)(x);return e.createElement("div",{className:(0,n.cx)(r.box,a)},t)}const x=t=>({box:n.css`
    padding: ${t.spacing(2)};
    background-color: ${t.colors.background.secondary};
    border-radius: ${t.shape.borderRadius()};
  `});function _({href:t,title:a}){const r=(0,i.wW)(C);return e.createElement("a",{href:t,className:r.link,rel:"noreferrer"},a," ",e.createElement(p.J,{name:"angle-right",size:"xl"}))}const C=t=>({link:n.css`
    display: block;
    color: ${t.colors.text.link};
  `})}}]);

//# sourceMappingURL=AlertingHome.a1557dac9864289b91aa.js.map
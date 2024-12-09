"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[8463],{55005:(f,i,a)=>{a.r(i),a.d(i,{plugin:()=>g});var l=a(65158),s=a(74848),e=a(32196),o=a(40845);const r=[{value:0,label:"Documentation",href:"https://grafana.com/docs/grafana/latest"},{value:1,label:"Tutorials",href:"https://grafana.com/tutorials"},{value:2,label:"Community",href:"https://community.grafana.com"},{value:3,label:"Public Slack",href:"http://slack.grafana.com"}],c=()=>{const n=(0,o.of)(p);return(0,s.jsxs)("div",{className:n.container,children:[(0,s.jsx)("h1",{className:n.title,children:"Welcome to Grafana"}),(0,s.jsxs)("div",{className:n.help,children:[(0,s.jsx)("h3",{className:n.helpText,children:"Need help?"}),(0,s.jsx)("div",{className:n.helpLinks,children:r.map((t,d)=>(0,s.jsx)("a",{className:n.helpLink,href:`${t.href}?utm_source=grafana_gettingstarted`,children:t.label},`${t.label}-${d}`))})]})]})},p=n=>({container:(0,e.css)`
      display: flex;
      /// background: url(public/img/g8_home_v2.svg) no-repeat;
      background-size: cover;
      height: 100%;
      align-items: center;
      padding: 0 16px;
      justify-content: space-between;
      padding: 0 ${n.spacing(3)};

      ${n.breakpoints.down("lg")} {
        background-position: 0px;
        flex-direction: column;
        align-items: flex-start;
        justify-content: center;
      }

      ${n.breakpoints.down("sm")} {
        padding: 0 ${n.spacing(1)};
      }
    `,title:(0,e.css)`
      margin-bottom: 0;

      ${n.breakpoints.down("lg")} {
        margin-bottom: ${n.spacing(1)};
      }

      ${n.breakpoints.down("md")} {
        font-size: ${n.typography.h2.fontSize};
      }
      ${n.breakpoints.down("sm")} {
        font-size: ${n.typography.h3.fontSize};
      }
    `,help:(0,e.css)`
      display: flex;
      align-items: baseline;
    `,helpText:(0,e.css)`
      margin-right: ${n.spacing(2)};
      margin-bottom: 0;

      ${n.breakpoints.down("md")} {
        font-size: ${n.typography.h4.fontSize};
      }

      ${n.breakpoints.down("sm")} {
        display: none;
      }
    `,helpLinks:(0,e.css)`
      display: flex;
      flex-wrap: wrap;
    `,helpLink:(0,e.css)`
      margin-right: ${n.spacing(2)};
      text-decoration: underline;
      text-wrap: no-wrap;

      ${n.breakpoints.down("sm")} {
        margin-right: 8px;
      }
    `}),g=new l.m(c).setNoPadding()}}]);

//# sourceMappingURL=welcomeBanner.50166ab0d8976013cfec.js.map
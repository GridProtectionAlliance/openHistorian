"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[9142],{96485:(X,f,i)=>{i.r(f),i.d(f,{plugin:()=>Y});var $=i(65158),s=i(74848),a=i(32196),j=i(96540),l=i(14110),w=i(32264),S=i(62930),u=i(55852),N=i(3911),T=i(10096),p=i(27677),P=i(14792),h=i(40845),D=i(14578);const m=(t,e)=>{const n="linear-gradient(to right, #5182CC 0%, #245BAF 100%)",o=e?n:"linear-gradient(to right, #f05a28 0%, #fbca0a 100%)",r=e?n:"linear-gradient(to right, #FBCA0A 0%, #F05A28 100%)",c=t.isDark?o:r;return`
      background-color: ${t.colors.background.secondary};
      margin-right: ${t.spacing(4)};
      border: 1px solid ${t.colors.border.weak};
      border-bottom-left-radius: ${t.shape.borderRadius(2)};
      border-bottom-right-radius: ${t.shape.borderRadius(2)};
      position: relative;
      max-height: 230px;

      ${t.breakpoints.down("xxl")} {
        margin-right: ${t.spacing(2)};
      }
      &::before {
        display: block;
        content: ' ';
        position: absolute;
        left: 0;
        right: 0;
        height: 2px;
        top: 0;
        background-image: ${c};
      }
`},x=(0,a.css)`
  padding: 16px;
`,A=({card:t})=>{const e=(0,h.of)(F,t.done);return(0,s.jsxs)("div",{className:e.card,children:[(0,s.jsx)("div",{className:x,children:(0,s.jsxs)("a",{href:`${t.href}?utm_source=grafana_gettingstarted`,className:e.url,onClick:()=>(0,l.rR)("grafana_getting_started_docs",{title:t.title,link:t.href}),children:[(0,s.jsx)("div",{className:e.heading,children:t.done?"complete":t.heading}),(0,s.jsx)("h4",{className:e.title,children:t.title})]})}),(0,s.jsxs)("a",{href:`${t.learnHref}?utm_source=grafana_gettingstarted`,className:e.learnUrl,target:"_blank",rel:"noreferrer",onClick:()=>(0,l.rR)("grafana_getting_started_docs",{title:t.title,link:t.learnHref}),children:["Learn how in the docs ",(0,s.jsx)(D.I,{name:"external-link-alt"})]})]})},F=(t,e)=>({card:(0,a.css)`
      ${m(t,e)}

      min-width: 230px;

      ${t.breakpoints.down("md")} {
        min-width: 192px;
      }
    `,heading:(0,a.css)`
      text-transform: uppercase;
      color: ${e?t.v1.palette.blue95:"#FFB357"};
      margin-bottom: ${t.spacing(2)};
    `,title:(0,a.css)`
      margin-bottom: ${t.spacing(2)};
    `,url:(0,a.css)`
      display: inline-block;
    `,learnUrl:(0,a.css)`
      border-top: 1px solid ${t.colors.border.weak};
      position: absolute;
      bottom: 0;
      padding: 8px 16px;
      width: 100%;
    `});var d=i(33390);const R=({card:t})=>{const e=(0,h.of)(G,t.done);return(0,s.jsx)("a",{className:e.card,target:"_blank",rel:"noreferrer",href:`${t.href}?utm_source=grafana_gettingstarted`,onClick:n=>B(n,t),children:(0,s.jsxs)("div",{className:x,children:[(0,s.jsx)("div",{className:e.type,children:t.type}),(0,s.jsx)("div",{className:e.heading,children:t.done?"complete":t.heading}),(0,s.jsx)("h4",{className:e.cardTitle,children:t.title}),(0,s.jsx)("div",{className:e.info,children:t.info})]})})},B=(t,e)=>{d.A.get(e.key)||d.A.set(e.key,!0),(0,l.rR)("grafana_getting_started_tutorial",{title:e.title})},G=(t,e)=>({card:(0,a.css)`
      ${m(t,e)}
      width: 460px;
      min-width: 460px;

      ${t.breakpoints.down("xl")} {
        min-width: 368px;
      }

      ${t.breakpoints.down("lg")} {
        min-width: 272px;
      }
    `,type:(0,a.css)`
      color: ${t.colors.primary.text};
      text-transform: uppercase;
    `,heading:(0,a.css)`
      text-transform: uppercase;
      color: ${t.colors.primary.text};
      margin-bottom: ${t.spacing(1)};
    `,cardTitle:(0,a.css)`
      margin-bottom: ${t.spacing(2)};
    `,info:(0,a.css)`
      margin-bottom: ${t.spacing(2)};
    `,status:(0,a.css)`
      display: flex;
      justify-content: flex-end;
    `}),I=({step:t})=>{const e=(0,h.of)(L);return(0,s.jsxs)("div",{className:e.setup,children:[(0,s.jsxs)("div",{className:e.info,children:[(0,s.jsx)("h2",{className:e.title,children:t.title}),(0,s.jsx)("p",{children:t.info})]}),(0,s.jsx)("div",{className:e.cards,children:t.cards.map((n,o)=>{const r=`${n.title}-${o}`;return n.type==="tutorial"?(0,s.jsx)(R,{card:n},r):(0,s.jsx)(A,{card:n},r)})})]})},L=t=>({setup:(0,a.css)({display:"flex",width:"95%"}),info:(0,a.css)({width:"172px",marginRight:"5%",[t.breakpoints.down("xxl")]:{marginRight:t.spacing(4)},[t.breakpoints.down("sm")]:{display:"none"}}),title:(0,a.css)({color:t.v1.palette.blue95}),cards:(0,a.css)({overflowX:"auto",overflowY:"hidden",width:"100%",display:"flex",justifyContent:"flex-start"})});var H=i(31193);const y="Grafana fundamentals",z="Create users and teams",b="getting.started.",v=`${b}${y.replace(" ","-").trim().toLowerCase()}`,k=`${b}${z.replace(" ","-").trim().toLowerCase()}`,U=()=>[{heading:"Welcome to Grafana",subheading:"The steps below will guide you to quickly finish setting up your Grafana installation.",title:"Basic",info:"The steps below will guide you to quickly finish setting up your Grafana installation.",done:!1,cards:[{type:"tutorial",heading:"Data source and dashboards",title:y,info:"Set up and understand Grafana if you have no prior experience. This tutorial guides you through the entire process and covers the \u201CData source\u201D and \u201CDashboards\u201D steps to the right.",href:"https://grafana.com/tutorials/grafana-fundamentals",icon:"grafana",check:()=>Promise.resolve(d.A.get(v)),key:v,done:!1},{type:"docs",title:"Add your first data source",heading:"data sources",icon:"database",learnHref:"https://grafana.com/docs/grafana/latest/features/datasources/add-a-data-source",href:"datasources/new",check:()=>new Promise(t=>{t((0,H.tR)().getMetricSources().filter(e=>e.meta.builtIn!==!0).length>0)}),done:!1},{type:"docs",heading:"dashboards",title:"Create your first dashboard",icon:"apps",href:"dashboard/new",learnHref:"https://grafana.com/docs/grafana/latest/guides/getting_started/#create-a-dashboard",check:async()=>(await(0,p.AI)().search({limit:1})).length>0,done:!1}]},{heading:"Setup complete!",subheading:"All necessary steps to use Grafana are done. Now tackle advanced steps or make the best use of this home dashboard \u2013 it is, after all, a fully customizable dashboard \u2013 and remove this panel.",title:"Advanced",info:" Manage your users and teams and add plugins. These steps are optional",done:!1,cards:[{type:"tutorial",heading:"Users",title:"Create users and teams",info:"Learn to organize your users in teams and manage resource access and roles.",href:"https://grafana.com/tutorials/create-users-and-teams",icon:"users-alt",key:k,check:()=>Promise.resolve(d.A.get(k)),done:!1},{type:"docs",heading:"plugins",title:"Find and install plugins",learnHref:"https://grafana.com/docs/grafana/latest/plugins/installation",href:"plugins",icon:"plug",check:async()=>{const t=await(0,p.AI)().get("/api/plugins",{embedded:0,core:0});return Promise.resolve(t.length>0)},done:!1}]}];class M extends j.PureComponent{constructor(){super(...arguments),this.state={checksDone:!1,currentStep:0,steps:U()},this.onForwardClick=()=>{(0,l.rR)("grafana_getting_started_button_to_advanced_tutorials"),this.setState(e=>({currentStep:e.currentStep+1}))},this.onPreviousClick=()=>{(0,l.rR)("grafana_getting_started_button_to_basic_tutorials"),this.setState(e=>({currentStep:e.currentStep-1}))},this.dismiss=()=>{const{id:e}=this.props,n=(0,P.UA)().getCurrent(),o=n?.getPanelById(e);(0,l.rR)("grafana_getting_started_remove_panel"),n?.removePanel(o),p.IB.put("/api/user/helpflags/1",void 0,{showSuccessAlert:!1}).then(r=>{T.TP.user.helpFlags1=r.helpFlags1})}}async componentDidMount(){const{steps:e}=this.state,n=e.map(async r=>{const c=r.cards.map(async g=>g.check().then(W=>({...g,done:W}))),C=await Promise.all(c);return{...r,done:C.every(g=>g.done),cards:C}}),o=await Promise.all(n);this.setState({currentStep:o[0].done?1:0,steps:o,checksDone:!0})}render(){const{checksDone:e,currentStep:n,steps:o}=this.state,r=K(),c=o[n];return(0,s.jsx)("div",{className:r.container,children:e?(0,s.jsxs)(s.Fragment,{children:[(0,s.jsx)(u.$n,{variant:"secondary",fill:"text",className:r.dismiss,onClick:this.dismiss,children:"Remove this panel"}),n===o.length-1&&(0,s.jsx)(u.$n,{className:(0,a.cx)(r.backForwardButtons,r.previous),onClick:this.onPreviousClick,"aria-label":"To basic tutorials",icon:"angle-left",variant:"secondary"}),(0,s.jsx)("div",{className:r.content,children:(0,s.jsx)(I,{step:c})}),n<o.length-1&&(0,s.jsx)(u.$n,{className:(0,a.cx)(r.backForwardButtons,r.forward),onClick:this.onForwardClick,"aria-label":"To advanced tutorials",icon:"angle-right",variant:"secondary"})]}):(0,s.jsxs)("div",{className:r.loading,children:[(0,s.jsx)("div",{className:r.loadingText,children:"Checking completed setup steps"}),(0,s.jsx)(S.y,{size:"xl",inline:!0})]})})}}const K=(0,N.N)(()=>{const t=w.$.theme2;return{container:(0,a.css)({display:"flex",flexDirection:"column",height:"100%",backgroundSize:"cover",padding:`${t.spacing(4)} ${t.spacing(2)} 0`}),content:(0,a.css)({label:"content",display:"flex",justifyContent:"center",[t.breakpoints.down("xxl")]:{marginLeft:t.spacing(3),justifyContent:"flex-start"}}),header:(0,a.css)({label:"header",marginBottom:t.spacing(3),display:"flex",flexDirection:"column",[t.breakpoints.down("lg")]:{flexDirection:"row"}}),headerLogo:(0,a.css)({height:"58px",paddingRight:t.spacing(2),display:"none",[t.breakpoints.up("md")]:{display:"block"}}),heading:(0,a.css)({label:"heading",marginRight:t.spacing(3),marginBottom:t.spacing(3),flexGrow:1,display:"flex",[t.breakpoints.up("md")]:{marginBottom:0}}),backForwardButtons:(0,a.css)({position:"absolute",top:"50%",transform:"translateY(-50%)"}),previous:(0,a.css)({left:"10px",[t.breakpoints.down("md")]:{left:0}}),forward:(0,a.css)({right:"10px",[t.breakpoints.down("md")]:{right:0}}),dismiss:(0,a.css)({alignSelf:"flex-end",textDecoration:"underline",marginBottom:t.spacing(1)}),loading:(0,a.css)({display:"flex",justifyContent:"center",alignItems:"center",height:"100%"}),loadingText:(0,a.css)({marginRight:t.spacing(1)})}}),Y=new $.m(M).setNoPadding()}}]);

//# sourceMappingURL=gettingStartedPanel.830005e471ef0dce7e2a.js.map
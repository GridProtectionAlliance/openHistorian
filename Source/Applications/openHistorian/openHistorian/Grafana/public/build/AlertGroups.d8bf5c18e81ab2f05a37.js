"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[2415],{48401:(N,d,e)=>{e.r(d),e.d(d,{default:()=>se});var a=e(9892),t=e(68404),o=e(72648),n=e(61744),E=e(45253),f=e(96044),p=e(81168),A=e(39031),m=e(30173),y=e(45524),u=e(88331),h=e(94647),G=e(31403),T=e(29614),C=e(23403),L=e(10505),z=e(94984),K=e(64353),b=e(2594);const Q=({onStateFilterChange:c,stateFilter:O})=>{const v=(0,o.wW)(J),g=Object.entries(A.Z9).sort(([r],[l])=>r<l?-1:1).map(([r,l])=>({label:r,value:l}));return t.createElement("div",{className:v.wrapper},t.createElement(K._,null,"State"),t.createElement(b.S,{options:g,value:O,onChange:c}))},J=c=>({wrapper:a.css`
    margin-left: ${c.spacing(1)};
  `});var F=e(82897),X=e(53217),Y=e(39904);const s=({className:c,groups:O,groupBy:v,onGroupingChange:g})=>{const r=(0,F.uniq)(O.flatMap(l=>l.alerts).flatMap(({labels:l})=>Object.keys(l))).filter(l=>!(l.startsWith("__")&&l.endsWith("__"))).map(l=>({label:l,value:l}));return t.createElement("div",{"data-testid":"group-by-container",className:c},t.createElement(K._,null,"Custom group by"),t.createElement(X.NU,{"aria-label":"group by label keys",value:v,placeholder:"Group by",prefix:t.createElement(Y.J,{name:"tag-alt"}),onChange:l=>{g(l.map(({value:i})=>i))},options:r}))};var W=e(30151);const P=({groups:c})=>{const[O,v]=(0,t.useState)(Math.floor(Math.random()*100)),[g,r]=(0,f.K)(),{groupBy:l=[],queryString:i,alertState:S}=(0,L.lC)(g),I=`matcher-${O}`,B=(0,C.k)("instance"),[j,Z]=(0,T.k)(B),$=(0,o.wW)(R),V=()=>{r({groupBy:null,queryString:null,alertState:null}),setTimeout(()=>v(O+1),100)},w=!!(l.length>0||i||S);return t.createElement("div",{className:$.wrapper},t.createElement(z.P,{current:j,onChange:Z,dataSources:B}),t.createElement("div",{className:$.filterSection},t.createElement(W.F,{className:$.filterInput,key:I,defaultQueryString:i,onFilterChange:M=>r({queryString:M||null})}),t.createElement(s,{className:$.filterInput,groups:c,groupBy:l,onGroupingChange:M=>r({groupBy:M.length?M.join(","):null})}),t.createElement(Q,{stateFilter:S,onStateFilterChange:M=>r({alertState:M||null})}),w&&t.createElement(G.zx,{className:$.clearButton,variant:"secondary",icon:"times",onClick:V},"Clear filters")))},R=c=>({wrapper:a.css`
    border-bottom: 1px solid ${c.colors.border.medium};
    margin-bottom: ${c.spacing(3)};
  `,filterSection:a.css`
    display: flex;
    flex-direction: row;
    margin-bottom: ${c.spacing(3)};
  `,filterInput:a.css`
    width: 340px;
    & + & {
      margin-left: ${c.spacing(1)};
    }
  `,clearButton:a.css`
    margin-left: ${c.spacing(1)};
    margin-top: 19px;
  `});var x=e(20194);const D=c=>{const[O]=(0,f.K)(),v=(0,L.lC)(O),g=(0,x.Zh)(v.queryString||"");return(0,t.useMemo)(()=>c.reduce((r,l)=>{const i=l.alerts.filter(({labels:S,status:I})=>{const B=(0,x.eD)(S,g),j=v.alertState?I.state===v.alertState:!0;return B&&j});return i.length>0&&(Object.keys(l.labels).length===0?r.unshift({...l,alerts:i}):r.push({...l,alerts:i})),r},[]),[c,v,g])},U=(c,O)=>(0,t.useMemo)(()=>O.length===0?c.filter(r=>Object.keys(r.labels).length===0).length>1?c.reduce((r,l)=>{if(Object.keys(l.labels).length===0){const i=r.find(({labels:S})=>Object.keys(S));i?i.alerts=(0,F.uniqBy)([...i.alerts,...l.alerts],"labels"):r.push({alerts:l.alerts,labels:{},receiver:{name:"NONE"}})}else r.push(l);return r},[]):c:c.flatMap(({alerts:g})=>g).reduce((g,r)=>{if(O.every(i=>Object.keys(r.labels).includes(i))){const i=g.find(S=>O.every(I=>S.labels[I]===r.labels[I]));if(i)i.alerts.push(r);else{const S=O.reduce((I,B)=>(I={...I,[B]:r.labels[B]},I),{});g.push({alerts:[r],labels:S,receiver:{name:"NONE"}})}}else{const i=g.find(S=>Object.keys(S.labels).length===0);i?i.alerts.push(r):g.push({alerts:[r],labels:{},receiver:{name:"NONE"}})}return g},[]),[c,O]);var k=e(69945),q=e(72004),_=e(37190),ee=e(45849),te=e(46818);const ae=()=>{const{useGetAlertmanagerChoiceStatusQuery:c}=m.h,O=(0,C.k)("instance"),[v]=(0,T.k)(O),g=(0,p.useDispatch)(),[r]=(0,f.K)(),{groupBy:l=[]}=(0,L.lC)(r),i=(0,o.wW)(ne),{currentData:S}=c(),I=(0,k._)(M=>M.amAlertGroups),{loading:B,error:j,result:Z=[]}=I[v||""]??te.oq,$=U(Z,l),V=D($),w=v===ee.GC&&S?.alertmanagersChoice===A.TE.External;return(0,t.useEffect)(()=>{function M(){v&&g((0,q.mS)(v))}M();const H=setInterval(M,_.iF);return()=>{clearInterval(H)}},[g,v]),v?t.createElement(y.J,{pageId:"groups"},t.createElement(P,{groups:Z}),B&&t.createElement(n.u,{text:"Loading notifications"}),j&&!B&&t.createElement(E.b,{title:"Error loading notifications",severity:"error"},j.message||"Unknown error"),w&&t.createElement(E.b,{title:"Grafana alerts are not delivered to Grafana Alertmanager"},"Grafana is configured to send alerts to external alertmanagers only. No alerts are expected to be available here for the selected Alertmanager."),Z&&V.map((M,H)=>t.createElement(t.Fragment,{key:`${JSON.stringify(M.labels)}-group-${H}`},(H===1&&Object.keys(V[0].labels).length===0||H===0&&Object.keys(M.labels).length>0)&&t.createElement("p",{className:i.groupingBanner},"Grouped by: ",Object.keys(M.labels).join(", ")),t.createElement(h.y,{alertManagerSourceName:v||"",group:M}))),Z&&!V.length&&t.createElement("p",null,"No results.")):t.createElement(y.J,{pageId:"groups"},t.createElement(u.I,{availableAlertManagers:O}))},ne=c=>({groupingBanner:a.css`
    margin: ${c.spacing(2,0)};
  `}),se=ae},30173:(N,d,e)=>{e.d(d,{h:()=>t});var a=e(29427);const t=a.C.injectEndpoints({endpoints:o=>({getAlertmanagerChoiceStatus:o.query({query:()=>({url:"/api/v1/ngalert"}),providesTags:["AlertmanagerChoice"]}),getExternalAlertmanagerConfig:o.query({query:()=>({url:"/api/v1/ngalert/admin_config"}),providesTags:["AlertmanagerChoice"]}),getExternalAlertmanagers:o.query({query:()=>({url:"/api/v1/ngalert/alertmanagers"}),transformResponse:n=>n.data}),saveExternalAlertmanagersConfig:o.mutation({query:n=>({url:"/api/v1/ngalert/admin_config",method:"POST",data:n}),invalidatesTags:["AlertmanagerChoice"]})})})},51981:(N,d,e)=>{e.d(d,{q:()=>o});var a=e(68404),t=e(82002);const o=({actions:n,children:E,fallback:f=!0})=>n.some(p=>t.Vt.hasAccess(p,f))?a.createElement(a.Fragment,null,E):null},94762:(N,d,e)=>{e.d(d,{F:()=>E});var a=e(9892),t=e(68404),o=e(72648),n=e(53731);const E=({renderExpandedContent:p,...A})=>{const m=(0,o.wW)(f);return t.createElement(n.t,{renderExpandedContent:p?(y,u,h)=>t.createElement(t.Fragment,null,u!==h.length-1&&t.createElement("div",{className:(0,a.cx)(m.contentGuideline,m.guideline)}),p(y,u,h)):void 0,renderPrefixHeader:()=>t.createElement("div",{className:m.relative},t.createElement("div",{className:(0,a.cx)(m.headerGuideline,m.guideline)})),renderPrefixCell:(y,u,h)=>t.createElement("div",{className:m.relative},t.createElement("div",{className:(0,a.cx)(m.topGuideline,m.guideline)}),u!==h.length-1&&t.createElement("div",{className:(0,a.cx)(m.bottomGuideline,m.guideline)})),...A})},f=p=>({relative:a.css`
    position: relative;
    height: 100%;
  `,guideline:a.css`
    left: -19px;
    border-left: 1px solid ${p.colors.border.medium};
    position: absolute;

    ${p.breakpoints.down("md")} {
      display: none;
    }
  `,topGuideline:a.css`
    width: 18px;
    border-bottom: 1px solid ${p.colors.border.medium};
    top: 0;
    bottom: 50%;
  `,bottomGuideline:a.css`
    top: 50%;
    bottom: 0;
  `,contentGuideline:a.css`
    top: 0;
    bottom: 0;
    left: -49px !important;
  `,headerGuideline:a.css`
    top: -25px;
    bottom: 0;
  `})},88331:(N,d,e)=>{e.d(d,{I:()=>p});var a=e(68404),t=e(45253),o=e(29614),n=e(94984);const E=()=>a.createElement(t.b,{title:"No Alertmanager found",severity:"warning"},"We could not find any external Alertmanagers and you may not have access to the built-in Grafana Alertmanager."),f=()=>a.createElement(t.b,{title:"Selected Alertmanager not found. Select a different Alertmanager.",severity:"warning"},"Selected Alertmanager no longer exists or you may not have permission to access it."),p=({availableAlertManagers:A})=>{const[m,y]=(0,o.k)(A),u=A.length>0;return a.createElement("div",null,u?a.createElement(a.Fragment,null,a.createElement(n.P,{onChange:y,dataSources:A}),a.createElement(f,null)):a.createElement(E,null))}},94647:(N,d,e)=>{e.d(d,{y:()=>X});var a=e(9892),t=e(68404),o=e(72648),n=e(39031),E=e(52694),f=e(33950),p=e(37556),A=e(94762),m=e(90072),y=e(31403),u=e(82002),h=e(81168),G=e(97953),T=e(45849),C=e(10505),L=e(78543),z=e(51981);const K=({alert:s,alertManagerSourceName:W})=>{const P=(0,o.wW)(b),R=(0,G.QX)(W),D=(0,T.HY)(W)?u.Vt.hasPermission(h.AccessControlAction.AlertingRuleRead):!0;return t.createElement(t.Fragment,null,t.createElement("div",{className:P.actionsRow},t.createElement(z.q,{actions:[R.update,R.create],fallback:u.Vt.isEditor},s.status.state===n.Z9.Suppressed&&t.createElement(y.Qj,{href:`${(0,C.eQ)("/alerting/silences",W)}&silenceIds=${s.status.silencedBy.join(",")}`,className:P.button,icon:"bell",size:"sm"},"Manage silences"),s.status.state===n.Z9.Active&&t.createElement(y.Qj,{href:(0,C.VN)(W,s.labels),className:P.button,icon:"bell-slash",size:"sm"},"Silence")),D&&s.generatorURL&&t.createElement(y.Qj,{className:P.button,href:s.generatorURL,icon:"chart-line",size:"sm"},"See source")),Object.entries(s.annotations).map(([U,k])=>t.createElement(L.a,{key:U,annotationKey:U,value:k})),t.createElement("div",{className:P.receivers},"Receivers:"," ",s.receivers.map(({name:U})=>U).filter(U=>!!U).join(", ")))},b=s=>({button:a.css`
    & + & {
      margin-left: ${s.spacing(1)};
    }
  `,actionsRow:a.css`
    padding: ${s.spacing(2,0)} !important;
    border-bottom: 1px solid ${s.colors.border.medium};
  `,receivers:a.css`
    padding: ${s.spacing(1,0)};
  `}),Q=({alerts:s,alertManagerSourceName:W})=>{const P=(0,o.wW)(J),R=(0,t.useMemo)(()=>[{id:"state",label:"State",renderCell:({data:D})=>t.createElement(t.Fragment,null,t.createElement(m.G,{state:D.status.state}),t.createElement("span",{className:P.duration},"for"," ",(0,p.vT)({start:new Date(D.startsAt),end:new Date(D.endsAt)}))),size:"220px"},{id:"labels",label:"Labels",renderCell:({data:{labels:D}})=>t.createElement(E.s,{labels:D}),size:1}],[P]),x=(0,t.useMemo)(()=>s.map(D=>({id:D.fingerprint,data:D})),[s]);return t.createElement("div",{className:P.tableWrapper,"data-testid":"alert-group-table"},t.createElement(A.F,{cols:R,items:x,isExpandable:!0,renderExpandedContent:({data:D})=>t.createElement(K,{alert:D,alertManagerSourceName:W})}))},J=s=>({tableWrapper:a.css`
    margin-top: ${s.spacing(3)};
    ${s.breakpoints.up("md")} {
      margin-left: ${s.spacing(4.5)};
    }
  `,duration:a.css`
    margin-left: ${s.spacing(1)};
    font-size: ${s.typography.bodySmall.fontSize};
  `});var F=e(96308);const X=({alertManagerSourceName:s,group:W})=>{const[P,R]=(0,t.useState)(!0),x=(0,o.wW)(Y);return t.createElement("div",{className:x.wrapper},t.createElement("div",{className:x.header},t.createElement("div",{className:x.group,"data-testid":"alert-group"},t.createElement(f.U,{size:"sm",isCollapsed:P,onToggle:()=>R(!P),"data-testid":"alert-group-collapse-toggle"}),Object.keys(W.labels).length?t.createElement(E.s,{labels:W.labels}):t.createElement("span",null,"No grouping")),t.createElement(F.Z,{group:W})),!P&&t.createElement(Q,{alertManagerSourceName:s,alerts:W.alerts}))},Y=s=>({wrapper:a.css`
    & + & {
      margin-top: ${s.spacing(2)};
    }
  `,header:a.css`
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
    align-items: center;
    justify-content: space-between;
    padding: ${s.spacing(1,1,1,0)};
    background-color: ${s.colors.background.secondary};
    width: 100%;
  `,group:a.css`
    display: flex;
    flex-direction: row;
    align-items: center;
  `,summary:a.css``,spanElement:a.css`
    margin-left: ${s.spacing(.5)};
  `,[n.Z9.Active]:a.css`
    color: ${s.colors.error.main};
  `,[n.Z9.Suppressed]:a.css`
    color: ${s.colors.primary.main};
  `,[n.Z9.Unprocessed]:a.css`
    color: ${s.colors.secondary.main};
  `})},30151:(N,d,e)=>{e.d(d,{F:()=>G});var a=e(9892),t=e(82897),o=e.n(t),n=e(68404),E=e(26418),f=e(54408),p=e(72648),A=e(39904),m=e(64353),y=e(6554),u=e(46967),h=e(41904);const G=({className:C,onFilterChange:L,defaultQueryString:z})=>{const K=(0,p.wW)(T),b=(0,n.useMemo)(()=>(0,t.debounce)(J=>{(0,f.PN)(h.z7.filterByLabel);const F=J.target;L(F.value)},600),[L]);(0,n.useEffect)(()=>b.cancel(),[b]);const Q=n.createElement(A.J,{name:"search"});return n.createElement("div",{className:C},n.createElement(m._,null,n.createElement(E.Stack,{gap:.5},n.createElement("span",null,"Search by label"),n.createElement(y.u,{content:n.createElement("div",null,"Filter alerts using label querying, ex:",n.createElement("pre",null,'{severity="critical", instance=~"cluster-us-.+"}'))},n.createElement(A.J,{className:K.icon,name:"info-circle",size:"sm"})))),n.createElement(u.I,{placeholder:"Search",defaultValue:z,onChange:b,"data-testid":"search-query-input",prefix:Q,className:K.inputWidth}))},T=C=>({icon:a.css`
    margin-right: ${C.spacing(.5)};
  `,inputWidth:a.css`
    width: 340px;
    flex-grow: 0;
  `})},90072:(N,d,e)=>{e.d(d,{G:()=>E});var a=e(68404),t=e(39031),o=e(79453);const n={[t.Z9.Active]:"bad",[t.Z9.Unprocessed]:"neutral",[t.Z9.Suppressed]:"info"},E=({state:f})=>a.createElement(o.i,{state:n[f]},f)},29614:(N,d,e)=>{e.d(d,{k:()=>p});var a=e(68404),t=e(96044),o=e(58379),n=e(37190),E=e(45849);function f(A){return(0,a.useCallback)(m=>A.map(u=>u.name).includes(m),[A])}function p(A){const[m,y]=(0,t.K)(),u=f(A),h=(0,a.useCallback)(C=>{u(C)&&(C===E.GC?(o.Z.delete(n.de),y({[n.c4]:null})):(o.Z.set(n.de,C),y({[n.c4]:C})))},[y,u]),G=m[n.c4];if(G&&typeof G=="string")return u(G)?[G,h]:[void 0,h];const T=o.Z.get(n.de);return T&&typeof T=="string"&&u(T)?(h(T),[T,h]):u(E.GC)?[E.GC,h]:[void 0,h]}},23403:(N,d,e)=>{e.d(d,{k:()=>o});var a=e(68404),t=e(45849);function o(n){return(0,a.useMemo)(()=>(0,t.LE)(n),[n])}}}]);

//# sourceMappingURL=AlertGroups.d8bf5c18e81ab2f05a37.js.map
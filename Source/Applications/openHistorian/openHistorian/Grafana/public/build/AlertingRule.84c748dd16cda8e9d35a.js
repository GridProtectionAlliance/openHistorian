"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[4679],{77110:(te,w,n)=>{n.r(w),n.d(w,{RuleViewer:()=>pe,default:()=>rt});var r=n(9892),N=n(39493),e=n(68404),L=n(86253),V=n(3499),Q=n(49922),M=n(35645),p=n(72648),b=n(45253),x=n(61744),h=n(52081),W=n(31403),K=n(39904),m=n(2352),E=n(8180),T=n(53546),C=n(84952),j=n(20091),y=n(82897),A=n(26418),O=n(19985),Z=n(25482),H=n(75e3),D=n(97573),Y=n(67162),ve=n(97201),fe=n(89050),ae=n(46519),ye=n(27677),Re=n(86647),xe=n(98825),he=n(9405),Ce=n(35575),$e=n(81168),re=n(37190),Ne=n(51981),De=n(74542);const le=4;function Pe({data:t,refId:a,model:l,datasourceUid:c,relativeTimeRange:o,onTimeRangeChange:i,className:g}){const u=(0,p.l4)(),s=(0,p.wW)(be),d=(0,H.j)(l)?re.Fe:re.GC,[v,f]=(0,e.useState)(d),B=(0,Re.F)().getInstanceSettings(c),[$,F]=(0,e.useState)({frameIndex:0,showHeader:!0}),U=(0,e.useCallback)(I=>{const S=(0,ae.CQ)().unix()-I.unix();if(o){const z=o.from-o.to;i({from:S+z,to:S})}},[i,o]),_=(0,e.useCallback)(I=>I===0?(0,ae.CQ)():(0,ae.CQ)().subtract(I,"seconds"),[]);return t?B?e.createElement("div",{className:(0,r.cx)(s.content,g)},e.createElement(fe.Z,null,({width:I,height:S})=>e.createElement("div",{style:{width:I,height:S}},e.createElement("div",{className:s.header},e.createElement("div",{className:s.actions},!(0,H.j)(l)&&o?e.createElement(Ce.x,{date:_(o.to),onChange:U,maxDate:new Date}):null,e.createElement(De.j,{onChange:f,value:v,size:"md"}),e.createElement(Ne.q,{actions:[$e.AccessControlAction.DataSourcesExplore]},!(0,H.j)(l)&&e.createElement(e.Fragment,null,e.createElement("div",{className:s.spacing}),e.createElement(W.Qj,{size:"md",variant:"secondary",icon:"compass",target:"_blank",href:Me(B,l)},"View in Explore"))))),e.createElement(xe.$,{height:S-u.spacing.gridSize*le,width:I,data:t,pluginId:v,title:"",onOptionsChange:F,options:$})))):e.createElement("div",{className:(0,r.cx)(s.content,g)},e.createElement(b.b,{title:"Could not find datasource for query"}),e.createElement(he.p,{width:"100%",height:"250px",language:"json",showLineNumbers:!1,showMiniMap:!1,value:JSON.stringify(l,null,"	"),readOnly:!0})):null}function Me(t,a){const{name:l}=t,{refId:c,...o}=a;return ye.Cj.renderUrl(`${M.v.appSubUrl}/explore`,{left:JSON.stringify({datasource:l,queries:[{refId:"A",...o}],range:{from:"now-1h",to:"now"}})})}const be=t=>({content:r.css`
      width: 100%;
      height: 250px;
    `,header:r.css`
      height: ${t.spacing(le)};
      display: flex;
      align-items: center;
      justify-content: flex-end;
      white-space: nowrap;
    `,refId:r.css`
      font-weight: ${t.typography.fontWeightMedium};
      color: ${t.colors.text.link};
      overflow: hidden;
    `,dataSource:r.css`
      margin-left: ${t.spacing(1)};
      font-style: italic;
      color: ${t.colors.text.secondary};
    `,actions:r.css`
      display: flex;
      align-items: center;
    `,spacing:r.css`
      padding: ${t.spacing(0,1,0,0)};
    `,errorMessage:r.css`
      white-space: pre-wrap;
    `});function Te({queries:t,condition:a,evalDataByQuery:l={},evalTimeRanges:c={},onTimeRangeChange:o}){const i=(0,y.keyBy)(Object.values(M.v.datasources),d=>d.uid),g=t.filter(d=>!(0,H.j)(d.model)),u=t.filter(d=>(0,H.j)(d.model)),s=(0,p.wW)(ne);return e.createElement(A.Stack,{gap:2,direction:"column"},e.createElement("div",{className:s.maxWidthContainer},e.createElement(A.Stack,{gap:2},g.map(({model:d,relativeTimeRange:v,refId:f,datasourceUid:B},$)=>{const F=i[B];return e.createElement(oe,{key:$,refId:f,isAlertCondition:a===f,model:d,relativeTimeRange:v,evalTimeRange:c[f],dataSource:F,queryData:l[f],onEvalTimeRangeChange:U=>o(f,U)})}))),e.createElement("div",{className:s.maxWidthContainer},e.createElement(A.Stack,{gap:1},u.map(({model:d,refId:v,datasourceUid:f},B)=>{const $=i[f];return(0,H.j)(d)&&e.createElement(Ie,{key:B,refId:v,isAlertCondition:a===v,model:d,dataSource:$,evalData:l[v]})}))))}function oe({refId:t,relativeTimeRange:a,model:l,dataSource:c,queryData:o,evalTimeRange:i,onEvalTimeRangeChange:g}){const u=(0,p.wW)(Oe),s=[c?.name??"[[Data source not found]]"];return a&&s.push((0,Z.C_)(a).display),e.createElement(ce,{refId:t,headerItems:s,className:u.contentBox},e.createElement("pre",{className:u.code},e.createElement("code",null,(0,j.$w)(l))),c&&e.createElement(Pe,{refId:t,datasourceUid:c.uid,model:l,data:o,relativeTimeRange:i,onTimeRangeChange:g,className:u.visualization}))}const Oe=t=>({code:r.css`
    margin: ${t.spacing(1)};
  `,contentBox:r.css`
    flex: 1 0 100%; // RuleViewerVisualization uses AutoSizer which doesn't expand the box
  `,visualization:r.css`
    padding: ${t.spacing(1)};
  `});function Ie({refId:t,model:a,evalData:l,isAlertCondition:c}){function o(){switch(a.type){case D.Us.math:return e.createElement(Qe,{model:a});case D.Us.reduce:return e.createElement(Be,{model:a});case D.Us.resample:return e.createElement(we,{model:a});case D.Us.classic:return e.createElement(We,{model:a});case D.Us.threshold:return e.createElement(Ve,{model:a});default:return e.createElement(e.Fragment,null,"Expression not supported: ",a.type)}}return e.createElement(ce,{refId:t,headerItems:[(0,y.startCase)(a.type)],isAlertCondition:c},o(),l&&e.createElement(ve.bw,{series:l.series,isAlertCondition:c}))}function ce({refId:t,headerItems:a=[],children:l,isAlertCondition:c,className:o}){const i=(0,p.wW)(Se);return e.createElement("div",{className:(0,r.cx)(i.container,o)},e.createElement("header",{className:i.header},e.createElement("span",{className:i.refId},t),a.map((g,u)=>e.createElement("span",{key:u,className:i.textBlock},g)),c&&e.createElement("div",{className:i.conditionIndicator},e.createElement(O.C,{color:"green",icon:"check",text:"Alert condition"}))),l)}const Se=t=>({container:r.css`
    flex: 1 0 25%;
    border: 1px solid ${t.colors.border.strong};
    max-width: 100%;
  `,header:r.css`
    display: flex;
    align-items: center;
    gap: ${t.spacing(1)};
    padding: ${t.spacing(1)};
    background-color: ${t.colors.background.secondary};
  `,textBlock:r.css`
    border: 1px solid ${t.colors.border.weak};
    padding: ${t.spacing(.5,1)};
    background-color: ${t.colors.background.primary};
  `,refId:r.css`
    color: ${t.colors.text.link};
    padding: ${t.spacing(.5,1)};
    border: 1px solid ${t.colors.border.weak};
  `,conditionIndicator:r.css`
    margin-left: auto;
  `});function We({model:t}){const a=(0,p.wW)(Ae),l=(0,y.keyBy)(Y.Z.reducerTypes,i=>i.value),c=(0,y.keyBy)(Y.Z.evalOperators,i=>i.value),o=(0,y.keyBy)(Y.Z.evalFunctions,i=>i.value);return e.createElement("div",{className:a.container},t.conditions?.map(({query:i,operator:g,reducer:u,evaluator:s},d)=>{const v=ie(s);return e.createElement(e.Fragment,{key:d},e.createElement("div",{className:a.blue},d===0?"WHEN":!!g?.type&&c[g?.type]?.text),e.createElement("div",{className:a.bold},u?.type&&l[u.type]?.text),e.createElement("div",{className:a.blue},"OF"),e.createElement("div",{className:a.bold},i.params[0]),e.createElement("div",{className:a.blue},o[s.type].text),e.createElement("div",{className:a.bold},v?`(${s.params[0]}; ${s.params[1]})`:s.params[0]))}))}const Ae=t=>({container:r.css`
    padding: ${t.spacing(1)};
    display: grid;
    grid-template-columns: max-content max-content max-content max-content max-content max-content;
    gap: ${t.spacing(0,1)};
  `,...q(t)});function Be({model:t}){const a=(0,p.wW)(Ue),{reducer:l,expression:c,settings:o}=t,i=D.SQ.find(s=>s.value===l),g=o?.mode??D.kN.Strict,u=D.YM.find(s=>s.value===g);return e.createElement("div",{className:a.container},e.createElement("div",{className:a.label},"Function"),e.createElement("div",{className:a.value},i?.label),e.createElement("div",{className:a.label},"Input"),e.createElement("div",{className:a.value},c),e.createElement("div",{className:a.label},"Mode"),e.createElement("div",{className:a.value},u?.label))}const Ue=t=>({container:r.css`
    padding: ${t.spacing(1)};
    display: grid;
    gap: ${t.spacing(1)};
    grid-template-rows: 1fr 1fr;
    grid-template-columns: 1fr 1fr 1fr 1fr;

    > :nth-child(6) {
      grid-column: span 3;
    }
  `,...q(t)});function we({model:t}){const a=(0,p.wW)(Le),{expression:l,window:c,downsampler:o,upsampler:i}=t,g=D.Fr.find(s=>s.value===o),u=D.r8.find(s=>s.value===i);return e.createElement("div",{className:a.container},e.createElement("div",{className:a.label},"Input"),e.createElement("div",{className:a.value},l),e.createElement("div",{className:a.label},"Resample to"),e.createElement("div",{className:a.value},c),e.createElement("div",{className:a.label},"Downsample"),e.createElement("div",{className:a.value},g?.label),e.createElement("div",{className:a.label},"Upsample"),e.createElement("div",{className:a.value},u?.label))}const Le=t=>({container:r.css`
    padding: ${t.spacing(1)};
    display: grid;
    gap: ${t.spacing(1)};
    grid-template-columns: 1fr 1fr 1fr 1fr;
    grid-template-rows: 1fr 1fr;
  `,...q(t)});function Ve({model:t}){const a=(0,p.wW)(ne),{expression:l,conditions:c}=t,o=c&&c[0]?.evaluator,i=D.Mi.find(u=>u.value===o?.type),g=o?ie(o):!1;return e.createElement("div",{className:a.container},e.createElement("div",{className:a.label},"Input"),e.createElement("div",{className:a.value},l),o&&e.createElement(e.Fragment,null,e.createElement("div",{className:a.blue},i?.label),e.createElement("div",{className:a.bold},g?`(${o.params[0]}; ${o.params[1]})`:o.params[0])))}const ne=t=>{const{blue:a,bold:l,...c}=q(t);return{...c,maxWidthContainer:r.css`
      max-width: 100%;
    `,container:r.css`
      padding: ${t.spacing(1)};
      display: flex;
      gap: ${t.spacing(1)};
    `,blue:r.css`
      ${a};
      margin: auto 0;
    `,bold:r.css`
      ${l};
      margin: auto 0;
    `}};function Qe({model:t}){const a=(0,p.wW)(ne),{expression:l}=t;return e.createElement("div",{className:a.container},e.createElement("div",{className:a.label},"Input"),e.createElement("div",{className:a.value},l))}const q=t=>({blue:r.css`
    color: ${t.colors.text.link};
  `,bold:r.css`
    font-weight: ${t.typography.fontWeightBold};
  `,label:r.css`
    display: flex;
    align-items: center;
    padding: ${t.spacing(.5,1)};
    background-color: ${t.colors.background.secondary};
    font-size: ${t.typography.bodySmall.fontSize};
    line-height: ${t.typography.bodySmall.lineHeight};
    font-weight: ${t.typography.fontWeightBold};
  `,value:r.css`
    padding: ${t.spacing(.5,1)};
    border: 1px solid ${t.colors.border.weak};
  `});function ie(t){return t.type===Y.$.IsWithinRange||t.type===Y.$.IsOutsideRange}var je=n(52694),X=n(80498),ue=n(40106),G=n(28104),Fe=n(49279),ze=n(68854),Ke=n(24990),Ze=n(8674);const He=({group:t})=>{const a=t.source_tenants??[];return e.createElement(X.C,{label:"Tenant sources"},e.createElement(e.Fragment,null,a.map(l=>e.createElement("div",{key:l},l))))};var Ge=n(78443),Je=n(80399),Ye=n(48208),Xe=n(69369),ke=n(76277),qe=n(81001),_e=n(99085),se=n(45849),J=n(79662);function de(t){if(!t)return[];const{namespace:a,rulerRule:l}=t,{rulesSource:c}=a;if((0,se.HY)(c)&&(0,J.Pc)(l))return l.grafana_alert.data;if((0,se.jq)(c)){const o=tt(c,t);return[et(o,c.uid)]}return[]}function et(t,a){return{refId:t.refId,datasourceUid:a,queryType:"",model:t,relativeTimeRange:{from:360,to:0}}}function tt(t,a){const l="A";switch(t.type){case"prometheus":return{refId:l,expr:a.query};case"loki":return{refId:l,expr:a.query};default:throw new Error(`Query for datasource type ${t.type} is currently not supported by cloud alert rules.`)}}var at=n(60048);const me="Could not find data source for rule",ge="Could not view rule",k="View rule";function pe({match:t}){const a=(0,p.wW)(Ee),[l,c]=(0,L.Z)(!1),{id:o}=t.params,i=at.OA(o,!0),{loading:g,error:u,result:s}=(0,ke.H)(i,i?.ruleSourceName),d=(0,e.useMemo)(()=>new qe.v,[]),v=(0,V.Z)(d.get()),f=(0,e.useMemo)(()=>de(s),[s]),B=(0,_e.$9)(s?.annotations||{}),[$,F]=(0,e.useState)({}),{allDataSourcesAvailable:U}=(0,Xe.S)(f),_=(0,e.useCallback)(()=>{if(f.length>0&&U){const R=f.map(P=>({...P,relativeTimeRange:$[P.refId]??P.relativeTimeRange}));d.run(R)}},[f,$,d,U]);(0,e.useEffect)(()=>{const R=de(s),P=Object.fromEntries(R.map(ee=>[ee.refId,ee.relativeTimeRange??{from:0,to:0}]));F(P)},[s]),(0,e.useEffect)(()=>{U&&l&&_()},[_,U,l]),(0,e.useEffect)(()=>()=>d.destroy(),[d]);const I=(0,e.useCallback)((R,P)=>{const ee=(0,N.ZP)($,ot=>{ot[R]=P});F(ee)},[$,F]);if(!i?.ruleSourceName)return e.createElement(G.$,{title:k},e.createElement(b.b,{title:ge},e.createElement("details",{className:a.errorMessage},me)));const S=(0,se.o_)(i.ruleSourceName);if(g)return e.createElement(G.$,{title:k},e.createElement(x.u,{text:"Loading rule..."}));if(u||!S)return e.createElement(G.$,{title:k},e.createElement(b.b,{title:ge},e.createElement("details",{className:a.errorMessage},u?.message??me,e.createElement("br",null),!!u?.stack&&u.stack)));if(!s)return e.createElement(G.$,{title:k},e.createElement("span",null,"Rule could not be found."));const z=(0,J.Jq)(s.group),lt=(0,J.Pc)(s.rulerRule)&&Boolean(s.rulerRule.grafana_alert.provenance);return e.createElement(G.$,{wrapInContent:!1,title:k},z&&e.createElement(b.b,{severity:"info",title:"This rule is part of a federated rule group."},e.createElement(h.wc,null,"Federated rule groups are currently an experimental feature.",e.createElement(W.zx,{fill:"text",icon:"book"},e.createElement("a",{href:"https://grafana.com/docs/metrics-enterprise/latest/tenant-management/tenant-federation/#cross-tenant-alerting-and-recording-rule-federation"},"Read documentation")))),lt&&e.createElement(ue.Xq,{resource:ue.Uv.AlertRule}),e.createElement(G.l,null,e.createElement("div",null,e.createElement("h4",null,e.createElement(K.J,{name:"bell",size:"lg"})," ",s.name),e.createElement(Ye.p,{rule:s,isCreating:!1,isDeleting:!1}),e.createElement(Fe.f,{rule:s,rulesSource:S,isViewMode:!0})),e.createElement("div",{className:a.details},e.createElement("div",{className:a.leftSide},s.promRule&&e.createElement(X.C,{label:"Health",horizontal:!0},e.createElement(Je.V,{rule:s.promRule})),!!s.labels&&!!Object.keys(s.labels).length&&e.createElement(X.C,{label:"Labels",horizontal:!0},e.createElement(je.s,{labels:s.labels,className:a.labels})),e.createElement(Ze.C,{rulesSource:S,rule:s,annotations:B}),e.createElement(ze.J,{annotations:B})),e.createElement("div",{className:a.rightSide},e.createElement(Ke.C,{rule:s,rulesSource:S}),z&&e.createElement(He,{group:s.group}),e.createElement(X.C,{label:"Namespace / Group",className:a.rightSideDetails},s.namespace.name," / ",s.group.name),(0,J.Pc)(s.rulerRule)&&e.createElement(nt,{rule:s.rulerRule.grafana_alert}))),e.createElement("div",null,e.createElement(Ge.M,{rule:s,pagination:{itemsPerPage:C.gN}}))),e.createElement(m.U,{label:"Query & Results",isOpen:l,onToggle:c,loading:v&&st(v),collapsible:!0,className:a.collapse},(0,J.Pc)(s.rulerRule)&&!z&&e.createElement(Te,{condition:s.rulerRule.grafana_alert.condition,queries:f,evalDataByQuery:v,evalTimeRanges:$,onTimeRangeChange:I}),!(0,J.Pc)(s.rulerRule)&&!z&&v&&Object.keys(v).length>0&&e.createElement("div",{className:a.queries},f.map(R=>e.createElement(oe,{key:R.refId,refId:R.refId,model:R.model,dataSource:Object.values(M.v.datasources).find(P=>P.uid===R.datasourceUid),queryData:v[R.refId],relativeTimeRange:R.relativeTimeRange,evalTimeRange:$[R.refId],onEvalTimeRangeChange:P=>I(R.refId,P),isAlertCondition:!1}))),!z&&!U&&e.createElement(b.b,{title:"Query not available",severity:"warning",className:a.queryWarning},"Cannot display the query preview. Some of the data sources used in the queries are not available.")))}function nt({rule:t}){const a=(0,p.wW)(Ee),l=()=>navigator.clipboard&&navigator.clipboard.writeText(t.uid);return e.createElement(X.C,{label:"Rule UID",childrenWrapperClassName:a.ruleUid},t.uid," ",e.createElement(E.h,{name:"copy",onClick:l}))}function st(t){return!!Object.values(t).find(a=>a.state===Q.Gu.Loading)}const Ee=t=>({errorMessage:r.css`
      white-space: pre-wrap;
    `,queries:r.css`
      height: 100%;
      width: 100%;
    `,collapse:r.css`
      margin-top: ${t.spacing(2)};
      border-color: ${t.colors.border.weak};
      border-radius: ${t.shape.borderRadius()};
    `,queriesTitle:r.css`
      padding: ${t.spacing(2,.5)};
      font-size: ${t.typography.h5.fontSize};
      font-weight: ${t.typography.fontWeightBold};
      font-family: ${t.typography.h5.fontFamily};
    `,query:r.css`
      border-bottom: 1px solid ${t.colors.border.medium};
      padding: ${t.spacing(2)};
    `,queryWarning:r.css`
      margin: ${t.spacing(4,0)};
    `,details:r.css`
      display: flex;
      flex-direction: row;
      gap: ${t.spacing(4)};
    `,leftSide:r.css`
      flex: 1;
    `,rightSide:r.css`
      padding-right: ${t.spacing(3)};
    `,rightSideDetails:r.css`
      & > div:first-child {
        width: auto;
      }
    `,labels:r.css`
      justify-content: flex-start;
    `,ruleUid:r.css`
      display: flex;
      align-items: center;
      gap: ${t.spacing(1)};
    `}),rt=(0,T.Pf)(pe,{style:"page"})},51981:(te,w,n)=>{n.d(w,{q:()=>e});var r=n(68404),N=n(82002);const e=({actions:L,children:V,fallback:Q=!0})=>L.some(M=>N.Vt.hasAccess(M,Q))?r.createElement(r.Fragment,null,V):null},28104:(te,w,n)=>{n.d(w,{$:()=>Q,l:()=>M});var r=n(9892),N=n(68404),e=n(72648),L=n(79396);const V={icon:"bell",id:"alert-rule-view",breadcrumbs:[{title:"Alert rules",url:"alerting/list"}]};function Q(x){const{wrapInContent:h=!0,children:W,title:K}=x,m=(0,e.wW)(p);return N.createElement(L.T,{pageNav:{...V,text:K},navId:"alert-list"},N.createElement(L.T.Contents,null,N.createElement("div",{className:m.content},h?N.createElement(M,{...x}):W)))}function M({children:x,padding:h=2}){const W=(0,e.wW)(b(h));return N.createElement("div",{className:W.wrapper},x)}const p=x=>({content:r.css`
      max-width: ${x.breakpoints.values.xxl}px;
    `}),b=x=>h=>({wrapper:r.css`
      background: ${h.colors.background.primary};
      border: 1px solid ${h.colors.border.weak};
      border-radius: ${h.shape.borderRadius()};
      padding: ${h.spacing(x)};
    `})},76277:(te,w,n)=>{n.d(w,{H:()=>x,X:()=>h});var r=n(68404),N=n(22350),e=n(81168),L=n(72004),V=n(46818),Q=n(60048),M=n(79662),p=n(61627),b=n(69945);function x(m,E){const T=W(E),C=(0,p.Zo)(E),j=(0,r.useMemo)(()=>{if(!(!m||!E||C.length===0))for(const y of C)for(const A of y.groups)for(const O of A.rules){const Z=Q.Yd(E,O);if(Q.Dg(Z,m))return O}},[m,E,C]);return{...T,result:j}}function h(m,E){const T=W(E),C=(0,p.Zo)(E),j=(0,r.useMemo)(()=>{if(!m||!E||C.length===0)return[];const y=[];for(const A of C)for(const O of A.groups)for(const Z of O.rules)Z.name===m&&y.push(Z);return y},[m,E,C]);return{...T,result:j}}function W(m){const E=(0,e.useDispatch)(),T=(0,b._)(O=>O.promRules),C=K(m,T),j=(0,b._)(O=>O.rulerRules),y=K(m,j),{loading:A}=(0,N.Z)(async()=>{m&&await E((0,L.dn)({rulesSourceName:m}))},[E,m]);return{loading:A,error:C.error??(0,M.m$)(y)?void 0:y.error,dispatched:C.dispatched&&y.dispatched}}function K(m,E){if(!m)return V.oq;const T=E[m];return T||V.oq}}}]);

//# sourceMappingURL=AlertingRule.84c748dd16cda8e9d35a.js.map
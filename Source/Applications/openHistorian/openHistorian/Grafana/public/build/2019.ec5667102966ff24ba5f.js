"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[2019],{94762:(z,T,e)=>{e.d(T,{F:()=>g});var a=e(9892),u=e(68404),N=e(72648),M=e(53731);const g=({renderExpandedContent:v,...f})=>{const m=(0,N.wW)(t);return u.createElement(M.t,{renderExpandedContent:v?(x,p,$)=>u.createElement(u.Fragment,null,p!==$.length-1&&u.createElement("div",{className:(0,a.cx)(m.contentGuideline,m.guideline)}),v(x,p,$)):void 0,renderPrefixHeader:()=>u.createElement("div",{className:m.relative},u.createElement("div",{className:(0,a.cx)(m.headerGuideline,m.guideline)})),renderPrefixCell:(x,p,$)=>u.createElement("div",{className:m.relative},u.createElement("div",{className:(0,a.cx)(m.topGuideline,m.guideline)}),p!==$.length-1&&u.createElement("div",{className:(0,a.cx)(m.bottomGuideline,m.guideline)})),...f})},t=v=>({relative:a.css`
    position: relative;
    height: 100%;
  `,guideline:a.css`
    left: -19px;
    border-left: 1px solid ${v.colors.border.medium};
    position: absolute;

    ${v.breakpoints.down("md")} {
      display: none;
    }
  `,topGuideline:a.css`
    width: 18px;
    border-bottom: 1px solid ${v.colors.border.medium};
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
  `})},29721:(z,T,e)=>{e.d(T,{V:()=>N});var a=e(68404),u=e(39904);const N=({namespace:M,group:g})=>g?a.createElement(a.Fragment,null,M," ",a.createElement(u.J,{name:"angle-right"})," ",g):a.createElement(a.Fragment,null,M)},62019:(z,T,e)=>{e.d(T,{i:()=>Ne});var a=e(9892),u=e(85719),N=e(29122);function M(n,s){(0,N.Z)(2,arguments);var c=(0,u.Z)(n),i=(0,u.Z)(s);return c.getTime()<i.getTime()}var g=e(14016),t=e(68404),v=e(37556),f=e(46519),m=e(38849),x=e(72648),p=e(6554),$=e(84952),H=e(8055),K=e(37190),P=e(79662),Y=e(53731),Z=e(94762),j=e(40106),W=e(29721),J=e(46587),Q=e(70356),X=e(26418),A=e(31403),w=e(94599),k=e(98102),q=e(60499),_=e(81168),ee=e(44391),te=e(72004),I=e(45849),V=e(10505),O=e(60048),ne=e(93411),ae=e(41273);const Oe=n=>window.matchMedia(`(max-width: ${n}px)`).matches,le=({rule:n,rulesSource:s})=>{const c=(0,_.useDispatch)(),i=(0,Q.TH)(),D=(0,q.iG)(),C=(0,x.wW)(oe),{namespace:d,group:l,rulerRule:o}=n,[r,h]=(0,t.useState)(),R=(0,I.EG)(s),E=(0,P.Pc)(n.rulerRule)&&Boolean(n.rulerRule.grafana_alert.provenance),y=[],L=(0,P.Jq)(l),{isEditable:be,isRemovable:Se}=(0,ee.M)(R,o),F=i.pathname+i.search,B=se(i.pathname),Le=()=>{if(r&&r.rulerRule){const b=O.Zk((0,I.EG)(r.namespace.rulesSource),r.namespace.name,r.group.name,r.rulerRule);c((0,te.hS)(b,{navigateTo:B?"/alerting/list":void 0})),h(void 0)}},ze=()=>(0,V.t6)(s,n),Ae=(0,I.EG)(s);if(B||y.push(t.createElement(p.u,{placement:"top",content:"View"},t.createElement(A.Qj,{className:C.button,title:"View",size:"sm",key:"view",variant:"secondary",icon:"eye",href:(0,V.V2)(s,n,F)}))),be&&o&&!L){const b=O.Zk(Ae,d.name,l.name,o);if(!E){const G=(0,ne.u)(`/alerting/${encodeURIComponent(O.$V(b))}/edit`,{returnTo:F});B&&y.push(t.createElement(w.m,{key:"copy",icon:"copy",onClipboardError:Ie=>{D.error("Error while copying URL",Ie)},className:C.button,size:"sm",getText:ze},"Copy link to rule")),y.push(t.createElement(p.u,{placement:"top",content:"Edit"},t.createElement(A.Qj,{title:"Edit",className:C.button,size:"sm",key:"edit",variant:"secondary",icon:"pen",href:G})))}y.push(t.createElement(p.u,{placement:"top",content:"Copy"},t.createElement(ae.E,{ruleIdentifier:b,isProvisioned:E,className:C.button})))}return Se&&o&&!L&&!E&&y.push(t.createElement(p.u,{placement:"top",content:"Delete"},t.createElement(A.zx,{title:"Delete",className:C.button,size:"sm",type:"button",key:"delete",variant:"secondary",icon:"trash-alt",onClick:()=>h(n)}))),y.length?t.createElement(t.Fragment,null,t.createElement(X.Stack,{gap:1},y.map((b,G)=>t.createElement(t.Fragment,{key:G},b))),!!r&&t.createElement(k.s,{isOpen:!0,title:"Delete rule",body:"Deleting this rule will permanently remove it from your alert rule list. Are you sure you want to delete this rule?",confirmText:"Yes, delete",icon:"exclamation-triangle",onConfirm:Le,onDismiss:()=>h(void 0)})):null};function se(n){return n.endsWith("/view")}const oe=n=>({button:a.css`
    padding: 0 ${n.spacing(2)};
  `});var re=e(35645),ie=e(39904),ce=e(82963);function me({rule:n}){const s=(0,x.wW)(ue),{exceedsLimit:c}=(0,t.useMemo)(()=>(0,ce.f)(n.group.interval),[n.group.interval]);return c?t.createElement(p.u,{theme:"error",content:t.createElement("div",null,"A minimum evaluation interval of"," ",t.createElement("span",{className:s.globalLimitValue},re.v.unifiedAlerting.minInterval)," has been configured in Grafana and will be used instead of the ",n.group.interval," interval configured for the Rule Group.")},t.createElement(ie.J,{name:"stopwatch-slash",className:s.icon})):null}function ue(n){return{globalLimitValue:a.css`
      font-weight: ${n.typography.fontWeightBold};
    `,icon:a.css`
      fill: ${n.colors.warning.text};
    `}}var de=e(53739),ve=e(99085),U=e(20383),pe=e(52694),S=e(80498),Ee=e(49279),ge=e(68854),fe=e(24990),he=e(8674),De=e(78443);const Re=15,ye=({rule:n})=>{const s=(0,x.wW)(xe),{namespace:{rulesSource:c}}=n,i=(0,ve.$9)(n.annotations);return t.createElement("div",null,t.createElement(Ee.f,{rule:n,rulesSource:c,isViewMode:!1}),t.createElement("div",{className:s.wrapper},t.createElement("div",{className:s.leftSide},t.createElement(Ce,{rule:n}),!!n.labels&&!!Object.keys(n.labels).length&&t.createElement(S.C,{label:"Labels",horizontal:!0},t.createElement(pe.s,{labels:n.labels})),t.createElement(he.C,{rulesSource:c,rule:n,annotations:i}),t.createElement(ge.J,{annotations:i})),t.createElement("div",{className:s.rightSide},t.createElement(fe.C,{rulesSource:c,rule:n}))),t.createElement(De.M,{rule:n,itemsDisplayLimit:Re}))},Ce=({rule:n})=>{let s,c=n.group.interval,i=n.promRule?.lastEvaluation,D=n.promRule?.evaluationTime;return(0,P.yF)(n.rulerRule)||(s=n.rulerRule?.for),t.createElement(t.Fragment,null,c&&t.createElement(S.C,{label:"Evaluate",horizontal:!0},"Every ",c),s&&t.createElement(S.C,{label:"For",horizontal:!0},s),i&&!(0,U.gV)(i)&&t.createElement(S.C,{label:"Last evaluation",horizontal:!0},t.createElement(p.u,{placement:"top",content:`${(0,m.dq)(i,{format:"YYYY-MM-DD HH:mm:ss"})}`,theme:"info"},t.createElement("span",null,`${(0,f.CQ)(i).locale("en").fromNow(!0)} ago`))),i&&!(0,U.gV)(i)&&D!==void 0&&t.createElement(S.C,{label:"Evaluation time",horizontal:!0},t.createElement(p.u,{placement:"top",content:`${D}s`,theme:"info"},t.createElement("span",null,(0,de.q)({timeInMs:D*1e3,humanize:!0})))))},xe=n=>({wrapper:a.css`
    display: flex;
    flex-direction: row;

    ${n.breakpoints.down("md")} {
      flex-direction: column;
    }
  `,leftSide:a.css`
    flex: 1;
  `,rightSide:a.css`
    ${n.breakpoints.up("md")} {
      padding-left: 90px;
      width: 300px;
    }
  `});var Me=e(80399),Te=e(48208);const Ne=({rules:n,className:s,showGuidelines:c=!1,emptyMessage:i="No rules found.",showGroupColumn:D=!1,showSummaryColumn:C=!1,showNextEvaluationColumn:d=!1})=>{const l=(0,x.wW)($e),o=(0,a.cx)(l.wrapper,s,{[l.wrapperMargin]:c}),r=(0,t.useMemo)(()=>n.map((E,y)=>({id:`${E.namespace.name}-${E.group.name}-${E.name}-${y}`,data:E})),[n]),h=Pe(C,D,d);if(!n.length)return t.createElement("div",{className:(0,a.cx)(o,l.emptyMessage)},i);const R=c?Z.F:Y.t;return t.createElement("div",{className:o,"data-testid":"rules-table"},t.createElement(R,{cols:h,isExpandable:!0,items:r,renderExpandedContent:({data:E})=>t.createElement(ye,{rule:E}),pagination:{itemsPerPage:$.gN},paginationStyles:l.pagination}))},$e=n=>({wrapperMargin:a.css`
    ${n.breakpoints.up("md")} {
      margin-left: 36px;
    }
  `,emptyMessage:a.css`
    padding: ${n.spacing(1)};
  `,wrapper:a.css`
    width: auto;
    border-radius: ${n.shape.borderRadius()};
  `,pagination:a.css`
    display: flex;
    margin: 0;
    padding-top: ${n.spacing(1)};
    padding-bottom: ${n.spacing(.25)};
    justify-content: center;
    border-left: 1px solid ${n.colors.border.strong};
    border-right: 1px solid ${n.colors.border.strong};
    border-bottom: 1px solid ${n.colors.border.strong};
  `});function Pe(n,s,c){const{hasRuler:i,rulerRulesLoaded:D}=(0,H.h)(),C=(0,t.useCallback)(d=>{const l=d.promRule?.lastEvaluation&&(0,v.qb)(d.promRule.lastEvaluation),o=d.group.interval&&(0,v.jO)(d.group.interval);if(!l||!o||(0,P.E)(d))return;const r=(0,v.RA)(d.group.interval),h=Date.parse(d.promRule?.lastEvaluation||""),R=(0,v.Ks)(h,r);return M(R,new Date)?{humanized:`within ${(0,g.Z)(r)}`,fullDate:`within ${(0,g.Z)(r)}`}:{humanized:`in ${(0,f.CQ)(R).locale("en").fromNow(!0)}`,fullDate:(0,m.dq)(R,{format:"YYYY-MM-DD HH:mm:ss"})}},[]);return(0,t.useMemo)(()=>{const d=[{id:"state",label:"State",renderCell:({data:l})=>{const{namespace:o}=l,{rulesSource:r}=o,{promRule:h,rulerRule:R}=l,E=!!(i(r)&&D(r)&&h&&!R),y=!!(i(r)&&D(r)&&R&&!h),L=(0,P.E)(l);return t.createElement(Te.p,{rule:l,isDeleting:E,isCreating:y,isPaused:L})},size:"165px"},{id:"name",label:"Name",renderCell:({data:l})=>l.name,size:c?4:5},{id:"provisioned",label:"",renderCell:({data:l})=>{const o=l.rulerRule;return(0,P.Pc)(o)&&o.grafana_alert.provenance?t.createElement(j.C0,null):null},size:"100px"},{id:"warnings",label:"",renderCell:({data:l})=>t.createElement(me,{rule:l}),size:"45px"},{id:"health",label:"Health",renderCell:({data:{promRule:l,group:o}})=>l?t.createElement(Me.V,{rule:l}):null,size:"75px"}];return n&&d.push({id:"summary",label:"Summary",renderCell:({data:l})=>t.createElement(J.Z,{input:l.annotations[K.q6.summary]??""}),size:c?4:5}),c&&d.push({id:"nextEvaluation",label:"Next evaluation",renderCell:({data:l})=>{const o=C(l);return o&&t.createElement(p.u,{placement:"top",content:`${o?.fullDate}`,theme:"info"},t.createElement("span",null,o?.humanized))},size:2}),s&&d.push({id:"group",label:"Group",renderCell:({data:l})=>{const{namespace:o,group:r}=l;return r.name==="default"?t.createElement(W.V,{namespace:o.name}):t.createElement(W.V,{namespace:o.name,group:r.name})},size:5}),d.push({id:"actions",label:"Actions",renderCell:({data:l})=>t.createElement(le,{rule:l,rulesSource:l.namespace.rulesSource}),size:"200px"}),d},[n,s,c,i,D,C])}},8055:(z,T,e)=>{e.d(T,{h:()=>M});var a=e(68404),u=e(45849),N=e(69945);function M(){const g=(0,N._)(f=>f.rulerRules),t=(0,a.useCallback)(f=>{const m=typeof f=="string"?f:f.name;return m===u.GC||!!g[m]?.result},[g]),v=(0,a.useCallback)(f=>{const m=(0,u.EG)(f),x=g[m]?.result;return Boolean(x)},[g]);return{hasRuler:t,rulerRulesLoaded:v}}}}]);

//# sourceMappingURL=2019.ec5667102966ff24ba5f.js.map
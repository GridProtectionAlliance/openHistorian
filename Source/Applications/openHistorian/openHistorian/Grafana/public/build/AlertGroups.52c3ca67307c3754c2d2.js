"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[2415],{2842:(e,s,t)=>{t.r(s),t.d(s,{default:()=>Y});var r=t(36636),a=t(68404),n=t(18745),l=t(3490),i=t(26011),o=t(40256),c=t(71308),d=t(10331),u=t(94322),p=t(91045),g=t(43215),m=t(29721),h=t(82344),b=t(61959),x=t(47570),j=t(82969),f=t(19462),v=t(39357),y=t(212),S=t(53262),N=t(45916);const k=e=>{let{alert:s,alertManagerSourceName:t}=e;const r=(0,l.useStyles2)(C),a=(0,j.QX)(t),n=!(0,f.HY)(t)||b.Vt.hasPermission(x.bW.AlertingRuleRead);return(0,N.jsxs)(N.Fragment,{children:[(0,N.jsxs)("div",{className:r.actionsRow,children:[(0,N.jsxs)(S.q,{actions:[a.update,a.create],fallback:b.Vt.isEditor,children:[s.status.state===d.Z9.Suppressed&&(0,N.jsx)(l.LinkButton,{href:`${(0,v.eQ)("/alerting/silences",t)}&silenceIds=${s.status.silencedBy.join(",")}`,className:r.button,icon:"bell",size:"sm",children:"Manage silences"}),s.status.state===d.Z9.Active&&(0,N.jsx)(l.LinkButton,{href:(0,v.VN)(t,s.labels),className:r.button,icon:"bell-slash",size:"sm",children:"Silence"})]}),n&&s.generatorURL&&(0,N.jsx)(l.LinkButton,{className:r.button,href:s.generatorURL,icon:"chart-line",size:"sm",children:"See source"})]}),Object.entries(s.annotations).map((e=>{let[s,t]=e;return(0,N.jsx)(y.a,{annotationKey:s,value:t},s)})),(0,N.jsxs)("div",{className:r.receivers,children:["Receivers:"," ",s.receivers.map((e=>{let{name:s}=e;return s})).filter((e=>!!e)).join(", ")]})]})},C=e=>({button:r.css`
    & + & {
      margin-left: ${e.spacing(1)};
    }
  `,actionsRow:r.css`
    padding: ${e.spacing(2,0)} !important;
    border-bottom: 1px solid ${e.colors.border.medium};
  `,receivers:r.css`
    padding: ${e.spacing(1,0)};
  `}),$=e=>{let{alerts:s,alertManagerSourceName:t}=e;const r=(0,l.useStyles2)(w),n=(0,a.useMemo)((()=>[{id:"state",label:"State",renderCell:e=>{let{data:s}=e;return(0,N.jsxs)(N.Fragment,{children:[(0,N.jsx)(h.G,{state:s.status.state}),(0,N.jsxs)("span",{className:r.duration,children:["for"," ",(0,g.intervalToAbbreviatedDurationString)({start:new Date(s.startsAt),end:new Date(s.endsAt)})]})]})},size:"220px"},{id:"labels",label:"Labels",renderCell:e=>{let{data:{labels:s}}=e;return(0,N.jsx)(u.s,{className:r.labels,labels:s})},size:1}]),[r]),i=(0,a.useMemo)((()=>s.map((e=>({id:e.fingerprint,data:e})))),[s]);return(0,N.jsx)("div",{className:r.tableWrapper,"data-testid":"alert-group-table",children:(0,N.jsx)(m.F,{cols:n,items:i,isExpandable:!0,renderExpandedContent:e=>{let{data:s}=e;return(0,N.jsx)(k,{alert:s,alertManagerSourceName:t})}})})},w=e=>({tableWrapper:r.css`
    margin-top: ${e.spacing(3)};
    ${e.breakpoints.up("md")} {
      margin-left: ${e.spacing(4.5)};
    }
  `,duration:r.css`
    margin-left: ${e.spacing(1)};
    font-size: ${e.typography.bodySmall.fontSize};
  `,labels:r.css`
    padding-bottom: 0;
  `});var O,M=t(8861);const A=e=>{let{alertManagerSourceName:s,group:t}=e;const[r,n]=(0,a.useState)(!0),i=(0,l.useStyles2)(G);return(0,N.jsxs)("div",{className:i.wrapper,children:[(0,N.jsxs)("div",{className:i.header,children:[(0,N.jsxs)("div",{className:i.group,"data-testid":"alert-group",children:[(0,N.jsx)(p.U,{isCollapsed:r,onToggle:()=>n(!r),"data-testid":"alert-group-collapse-toggle"}),Object.keys(t.labels).length?(0,N.jsx)(u.s,{className:i.headerLabels,labels:t.labels}):O||(O=(0,N.jsx)("span",{children:"No grouping"}))]}),(0,N.jsx)(M.Z,{group:t})]}),!r&&(0,N.jsx)($,{alertManagerSourceName:s,alerts:t.alerts})]})},G=e=>({wrapper:r.css`
    & + & {
      margin-top: ${e.spacing(2)};
    }
  `,headerLabels:r.css`
    padding-bottom: 0 !important;
    margin-bottom: -${e.spacing(.5)};
  `,header:r.css`
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
    align-items: center;
    justify-content: space-between;
    padding: ${e.spacing(1,1,1,0)};
    background-color: ${e.colors.background.secondary};
    width: 100%;
  `,group:r.css`
    display: flex;
    flex-direction: row;
    align-items: center;
  `,summary:r.css``,spanElement:r.css`
    margin-left: ${e.spacing(.5)};
  `,[d.Z9.Active]:r.css`
    color: ${e.colors.error.main};
  `,[d.Z9.Suppressed]:r.css`
    color: ${e.colors.primary.main};
  `,[d.Z9.Unprocessed]:r.css`
    color: ${e.colors.secondary.main};
  `});var F,B=t(82498),I=t(1698),L=t(50489);const Z=e=>{let{onStateFilterChange:s,stateFilter:t}=e;const r=(0,l.useStyles2)(E),a=Object.entries(d.Z9).sort(((e,s)=>{let[t]=e,[r]=s;return t<r?-1:1})).map((e=>{let[s,t]=e;return{label:s,value:t}}));return(0,N.jsxs)("div",{className:r.wrapper,children:[F||(F=(0,N.jsx)(l.Label,{children:"State"})),(0,N.jsx)(l.RadioButtonGroup,{options:a,value:t,onChange:s})]})},E=e=>({wrapper:r.css`
    margin-left: ${e.spacing(1)};
  `});var q,z,R=t(82897);const W=e=>{let{className:s,groups:t,groupBy:r,onGroupingChange:a}=e;const n=(0,R.uniq)(t.flatMap((e=>e.alerts)).flatMap((e=>{let{labels:s}=e;return Object.keys(s)}))).filter((e=>!(e.startsWith("__")&&e.endsWith("__")))).map((e=>({label:e,value:e})));return(0,N.jsxs)("div",{"data-testid":"group-by-container",className:s,children:[q||(q=(0,N.jsx)(l.Label,{children:"Custom group by"})),(0,N.jsx)(l.MultiSelect,{"aria-label":"group by label keys",value:r,placeholder:"Group by",prefix:z||(z=(0,N.jsx)(l.Icon,{name:"tag-alt"})),onChange:e=>{a(e.map((e=>{let{value:s}=e;return s})))},options:n})]})};var P=t(16905);const T=e=>{let{groups:s}=e;const[t,r]=(0,a.useState)(Math.floor(100*Math.random())),[n,o]=(0,i.K)(),{groupBy:c=[],queryString:d,alertState:u}=(0,v.lC)(n),p=`matcher-${t}`,g=(0,I.k)("instance"),[m,h]=(0,B.k)(g),b=(0,l.useStyles2)(U),x=!!(c.length>0||d||u);return(0,N.jsxs)("div",{className:b.wrapper,children:[(0,N.jsx)(L.P,{current:m,onChange:h,dataSources:g}),(0,N.jsxs)("div",{className:b.filterSection,children:[(0,N.jsx)(P.F,{className:b.filterInput,defaultQueryString:d,onFilterChange:e=>o({queryString:e||null})},p),(0,N.jsx)(W,{className:b.filterInput,groups:s,groupBy:c,onGroupingChange:e=>o({groupBy:e.length?e.join(","):null})}),(0,N.jsx)(Z,{stateFilter:u,onStateFilterChange:e=>o({alertState:e||null})}),x&&(0,N.jsx)(l.Button,{className:b.clearButton,variant:"secondary",icon:"times",onClick:()=>{o({groupBy:null,queryString:null,alertState:null}),setTimeout((()=>r(t+1)),100)},children:"Clear filters"})]})]})},U=e=>({wrapper:r.css`
    border-bottom: 1px solid ${e.colors.border.medium};
    margin-bottom: ${e.spacing(3)};
  `,filterSection:r.css`
    display: flex;
    flex-direction: row;
    margin-bottom: ${e.spacing(3)};
  `,filterInput:r.css`
    width: 340px;
    & + & {
      margin-left: ${e.spacing(1)};
    }
  `,clearButton:r.css`
    margin-left: ${e.spacing(1)};
    margin-top: 19px;
  `});var D=t(82139);var K,V,_=t(33899),J=t(83809),Q=t(85464),H=t(8455);const X=e=>({groupingBanner:r.css`
    margin: ${e.spacing(2,0)};
  `}),Y=()=>{var e;const s=(0,I.k)("instance"),[t]=(0,B.k)(s),r=(0,n.useDispatch)(),[d]=(0,i.K)(),{groupBy:u=[]}=(0,v.lC)(d),p=(0,l.useStyles2)(X),g=(0,_._)((e=>e.amAlertGroups)),{loading:m,error:h,result:b=[]}=null!==(e=g[t||""])&&void 0!==e?e:H.oq,x=((e,s)=>(0,a.useMemo)((()=>0===s.length?e.filter((e=>0===Object.keys(e.labels).length)).length>1?e.reduce(((e,s)=>{if(0===Object.keys(s.labels).length){const t=e.find((e=>{let{labels:s}=e;return Object.keys(s)}));t?t.alerts=(0,R.uniqBy)([...t.alerts,...s.alerts],"labels"):e.push({alerts:s.alerts,labels:{},receiver:{name:"NONE"}})}else e.push(s);return e}),[]):e:e.flatMap((e=>{let{alerts:s}=e;return s})).reduce(((e,t)=>{if(s.every((e=>Object.keys(t.labels).includes(e)))){const r=e.find((e=>s.every((s=>e.labels[s]===t.labels[s]))));if(r)r.alerts.push(t);else{const r=s.reduce(((e,s)=>Object.assign({},e,{[s]:t.labels[s]})),{});e.push({alerts:[t],labels:r,receiver:{name:"NONE"}})}}else{const s=e.find((e=>0===Object.keys(e.labels).length));s?s.alerts.push(t):e.push({alerts:[t],labels:{},receiver:{name:"NONE"}})}return e}),[])),[e,s]))(b,u),j=(e=>{const[s]=(0,i.K)(),t=(0,v.lC)(s),r=(0,D.Zh)(t.queryString||"");return(0,a.useMemo)((()=>e.reduce(((e,s)=>{const a=s.alerts.filter((e=>{let{labels:s,status:a}=e;const n=(0,D.eD)(s,r),l=!t.alertState||a.state===t.alertState;return n&&l}));return a.length>0&&(0===Object.keys(s.labels).length?e.unshift(Object.assign({},s,{alerts:a})):e.push(Object.assign({},s,{alerts:a}))),e}),[])),[e,t,r])})(x);return(0,a.useEffect)((()=>{function e(){t&&r((0,J.mS)(t))}e();const s=setInterval(e,Q.iF);return()=>{clearInterval(s)}}),[r,t]),t?(0,N.jsxs)(o.J,{pageId:"groups",children:[(0,N.jsx)(T,{groups:b}),m&&(K||(K=(0,N.jsx)(l.LoadingPlaceholder,{text:"Loading notifications"}))),h&&!m&&(0,N.jsx)(l.Alert,{title:"Error loading notifications",severity:"error",children:h.message||"Unknown error"}),b&&j.map(((e,s)=>(0,N.jsxs)(a.Fragment,{children:[(1===s&&0===Object.keys(j[0].labels).length||0===s&&Object.keys(e.labels).length>0)&&(0,N.jsxs)("p",{className:p.groupingBanner,children:["Grouped by: ",Object.keys(e.labels).join(", ")]}),(0,N.jsx)(A,{alertManagerSourceName:t||"",group:e})]},`${JSON.stringify(e.labels)}-group-${s}`))),b&&!j.length&&(V||(V=(0,N.jsx)("p",{children:"No results."})))]}):(0,N.jsx)(o.J,{pageId:"groups",children:(0,N.jsx)(c.I,{availableAlertManagers:s})})}},40256:(e,s,t)=>{t.d(s,{J:()=>i});t(68404);var r=t(18745),a=t(69371),n=t(8674),l=t(45916);const i=e=>{let{children:s,pageId:t,isLoading:i}=e;const o=(0,n.h)((0,r.useSelector)((e=>e.navIndex)),t);return(0,l.jsx)(a.T,{navModel:o,children:(0,l.jsx)(a.T.Contents,{isLoading:i,children:s})})}},53262:(e,s,t)=>{t.d(s,{q:()=>n});t(68404);var r=t(61959),a=t(45916);const n=e=>{let{actions:s,children:t,fallback:n=!0}=e;return s.some((e=>r.Vt.hasAccess(e,n)))?(0,a.jsx)(a.Fragment,{children:t}):null}},29721:(e,s,t)=>{t.d(s,{F:()=>o});var r=t(36636),a=(t(68404),t(3490)),n=t(9019),l=t(45916);const i=["renderExpandedContent"];const o=e=>{let{renderExpandedContent:s}=e,t=function(e,s){if(null==e)return{};var t,r,a={},n=Object.keys(e);for(r=0;r<n.length;r++)t=n[r],s.indexOf(t)>=0||(a[t]=e[t]);return a}(e,i);const o=(0,a.useStyles2)(c);return(0,l.jsx)(n.t,Object.assign({renderExpandedContent:s?(e,t,a)=>(0,l.jsxs)(l.Fragment,{children:[!(t===a.length-1)&&(0,l.jsx)("div",{className:(0,r.cx)(o.contentGuideline,o.guideline)}),s(e,t,a)]}):void 0,renderPrefixHeader:()=>(0,l.jsx)("div",{className:o.relative,children:(0,l.jsx)("div",{className:(0,r.cx)(o.headerGuideline,o.guideline)})}),renderPrefixCell:(e,s,t)=>(0,l.jsxs)("div",{className:o.relative,children:[(0,l.jsx)("div",{className:(0,r.cx)(o.topGuideline,o.guideline)}),!(s===t.length-1)&&(0,l.jsx)("div",{className:(0,r.cx)(o.bottomGuideline,o.guideline)})]})},t))},c=e=>({relative:r.css`
    position: relative;
    height: 100%;
  `,guideline:r.css`
    left: -19px;
    border-left: 1px solid ${e.colors.border.medium};
    position: absolute;

    ${e.breakpoints.down("md")} {
      display: none;
    }
  `,topGuideline:r.css`
    width: 18px;
    border-bottom: 1px solid ${e.colors.border.medium};
    top: 0;
    bottom: 50%;
  `,bottomGuideline:r.css`
    top: 50%;
    bottom: 0;
  `,contentGuideline:r.css`
    top: 0;
    bottom: 0;
    left: -49px !important;
  `,headerGuideline:r.css`
    top: -25px;
    bottom: 0;
  `})},71308:(e,s,t)=>{t.d(s,{I:()=>g});t(68404);var r,a,n,l,i=t(3490),o=t(82498),c=t(50489),d=t(45916);const u=()=>r||(r=(0,d.jsx)(i.Alert,{title:"No Alertmanager found",severity:"warning",children:"We could not find any external Alertmanagers and you may not have access to the built-in Grafana Alertmanager."})),p=()=>a||(a=(0,d.jsx)(i.Alert,{title:"Selected Alertmanager not found. Select a different Alertmanager.",severity:"warning",children:"Selected Alertmanager no longer exists or you may not have permission to access it."})),g=e=>{let{availableAlertManagers:s}=e;const[t,r]=(0,o.k)(s),a=s.length>0;return(0,d.jsx)("div",{children:a?(0,d.jsxs)(d.Fragment,{children:[(0,d.jsx)(c.P,{onChange:r,dataSources:s}),n||(n=(0,d.jsx)(p,{}))]}):l||(l=(0,d.jsx)(u,{}))})}},16905:(e,s,t)=>{t.d(s,{F:()=>d});var r,a,n,l=t(36636),i=(t(68404),t(8072)),o=t(3490),c=t(45916);const d=e=>{let{className:s,onFilterChange:t,defaultQueryString:l,queryString:d}=e;const p=(0,o.useStyles2)(u),g=r||(r=(0,c.jsx)(o.Icon,{name:"search"}));return(0,c.jsxs)("div",{className:s,children:[(0,c.jsx)(o.Label,{children:(0,c.jsxs)(i.Stack,{gap:.5,children:[a||(a=(0,c.jsx)("span",{children:"Search by label"})),(0,c.jsx)(o.Tooltip,{content:n||(n=(0,c.jsxs)("div",{children:["Filter alerts using label querying, ex:",(0,c.jsx)("pre",{children:'{severity="critical", instance=~"cluster-us-.+"}'})]})),children:(0,c.jsx)(o.Icon,{className:p.icon,name:"info-circle",size:"sm"})})]})}),(0,c.jsx)(o.Input,{placeholder:"Search",defaultValue:l,value:d,onChange:e=>{const s=e.target;t(s.value)},"data-testid":"search-query-input",prefix:g,className:p.inputWidth})]})},u=e=>({icon:l.css`
    margin-right: ${e.spacing(.5)};
  `,inputWidth:l.css`
    width: 340px;
    flex-grow: 0;
  `})},82344:(e,s,t)=>{t.d(s,{G:()=>i});t(68404);var r=t(10331),a=t(49179),n=t(45916);const l={[r.Z9.Active]:"bad",[r.Z9.Unprocessed]:"neutral",[r.Z9.Suppressed]:"info"},i=e=>{let{state:s}=e;return(0,n.jsx)(a.i,{state:l[s],children:s})}},82498:(e,s,t)=>{t.d(s,{k:()=>o});var r=t(68404),a=t(26011),n=t(17421),l=t(85464),i=t(19462);function o(e){const[s,t]=(0,a.K)(),o=function(e){return(0,r.useCallback)((s=>e.map((e=>e.name)).includes(s)),[e])}(e),c=(0,r.useCallback)((e=>{o(e)&&(e===i.GC?(n.Z.delete(l.de),t({[l.c4]:null})):(n.Z.set(l.de,e),t({[l.c4]:e})))}),[t,o]),d=s[l.c4];if(d&&"string"==typeof d)return o(d)?[d,c]:[void 0,c];const u=n.Z.get(l.de);return u&&"string"==typeof u&&o(u)?(c(u),[u,c]):o(i.GC)?[i.GC,c]:[void 0,c]}},1698:(e,s,t)=>{t.d(s,{k:()=>n});var r=t(68404),a=t(19462);function n(e){return(0,r.useMemo)((()=>(0,a.LE)(e)),[e])}}}]);
//# sourceMappingURL=AlertGroups.52c3ca67307c3754c2d2.js.map
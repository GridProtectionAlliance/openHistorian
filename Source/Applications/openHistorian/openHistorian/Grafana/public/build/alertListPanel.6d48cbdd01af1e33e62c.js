"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[3375],{15458:(pe,G,s)=>{s.d(G,{R:()=>k});var n=s(74848),X=s(32196),T=s(40845),O=s(29158);const k=({labelKey:A,value:j,operator:F="=",onRemoveLabel:y})=>{const _=(0,T.of)(q);return(0,n.jsxs)("div",{className:_.wrapper,children:[A,F,j,!!y&&(0,n.jsx)(O.K,{name:"times",size:"xs",onClick:y,tooltip:"Remove label"})]})},q=A=>({wrapper:(0,X.css)({padding:A.spacing(.5,1),borderRadius:A.shape.radius.default,border:`solid 1px ${A.colors.border.medium}`,fontSize:A.typography.bodySmall.fontSize,backgroundColor:A.colors.background.secondary,fontWeight:A.typography.fontWeightBold,color:A.colors.text.primary,display:"inline-block",lineHeight:"1.2"})})},73235:(pe,G,s)=>{s.r(G),s.d(G,{plugin:()=>tt});var n=s(74848),X=s(65158),T=s(67061),O=s(55852),k=s(85242),q=s(51612),A=s(31678),j=s(57220),F=s(2543),y=s(96540),_=s(90182),z=s(14578),ee=s(61410),Y=s(26627),L=s(88467),S=s(94822),ve=s(75214);const ye=e=>{const{onChange:a,id:t,defaultValue:o,dataSource:l}=e,r=(0,A.useDispatch)();(0,y.useEffect)(()=>{l?l&&r((0,Y.mB)({rulesSourceName:l})):r((0,Y.nV)())},[r,l]);const i=(0,ee.$)(m=>m.promRules),p=(0,L.p0)(i),x=(0,L.Vf)(i),c=(0,y.useMemo)(()=>{if((0,F.isEmpty)(i))return[];if(!p)return[];const m=Object.keys(i).flatMap(u=>i[u].result??[]).flatMap(u=>u.groups).flatMap(u=>u.rules.filter(v=>v.type===S.JS.Alerting)).flatMap(u=>u.alerts??[]).map(u=>Object.keys(u.labels??{})).flatMap(u=>u.filter(v=>!(0,ve.F2)(v)));return(0,F.uniq)(m)},[p,i]);return(0,n.jsx)(_.KF,{id:t,isLoading:x,defaultValue:o,"aria-label":"group by label keys",placeholder:"Group by",prefix:(0,n.jsx)(z.I,{name:"tag-alt"}),onChange:m=>{a(m.map(u=>u.value??""))},options:c.map(m=>({label:m,value:m}))})};var d=s(32196),xe=s(42941),Re=s(94701),Ae=s(3591),E=s(40845),Se=s(40276),K=s(37494),je=s(39558),Fe=s(42418),be=s(2913),ie=s(55907),Ne=s(79938),M=s(75648),Ie=s(9518),$e=s(55740),ce=s(32642),R=s(23662),de=s(14792),V=s(19073),f=s(64861),De=s(15458),Me=s(55127),we=s.n(Me),Ce=s(2426),Be=s(78742),Le=s(15054),Ee=s(98164),$=s(21460);function Ve(e,a){const t=(0,ce.Zc)(e);return(0,Ee.Av)(a,t)}function te(e,a){const{stateFilter:t,alertInstanceLabelFilter:o}=e;return(0,F.isEmpty)(t)?a:a.filter(l=>(t.firing&&((0,$.S)(l,S.Gi.Alerting)||(0,$.S)(l,S.cF.Firing))||t.pending&&((0,$.S)(l,S.Gi.Pending)||(0,$.S)(l,S.cF.Pending))||t.noData&&(0,$.S)(l,S.Gi.NoData)||t.normal&&(0,$.S)(l,S.Gi.Normal)||t.error&&(0,$.S)(l,S.Gi.Error)||t.inactive&&(0,$.S)(l,S.cF.Inactive))&&(o?Ve(e.alertInstanceLabelFilter,l.labels):!0))}const ue=({rule:e,alerts:a,options:t,grafanaTotalInstances:o,handleInstancesLimit:l,limitInstances:r,grafanaFilteredInstancesTotal:i})=>{const p=t.groupMode===f.r6.Custom?!0:t.showInstances,[x,c]=(0,y.useState)(p),m=(0,E.of)(Pe),u=(0,E.of)(O.my),v=(0,y.useCallback)(()=>{c(B=>!B)},[]),h=(0,y.useMemo)(()=>te(t,(0,Be.Cp)(t.sortOrder,a))??[],[a,t]),g=o!==void 0,N=o&&i?o-i:0,b=a.length-h.length,D=g?N:b,w=h.length>0,U=w?v:F.noop;(0,y.useEffect)(()=>{h.length===0&&c(!1)},[h]);const C=async()=>{l&&(l(!1),c(!0))},se=async()=>{l&&(l(!0),c(!0))},ne=r?i:h.length,le=h.length,Z=g?ne:le,re=r?`Showing ${M.Ys} of ${o} instances`:`Showing all ${o} instances`,oe=r?"View all instances":`Limit the result to ${M.Ys} instances`,W=o&&M.Ys===h.length&&o>h.length,J=o&&M.Ys<h.length&&!r,Q=W||J?(0,n.jsxs)("div",{className:m.footerRow,children:[(0,n.jsx)("div",{children:re}),(0,n.jsx)(O.$n,{size:"sm",variant:"secondary",onClick:r?C:se,children:oe})]}):void 0;return(0,n.jsxs)("div",{children:[t.groupMode===f.r6.Default&&(0,n.jsxs)("button",{className:(0,d.cx)(u,w?m.clickable:""),onClick:()=>U(),children:[w&&(0,n.jsx)(z.I,{name:x?"angle-down":"angle-right",size:"md"}),(0,n.jsx)("span",{children:`${Z} ${we()("instance",Z)}`}),D>0&&(0,n.jsxs)("span",{children:[", ",`${D} hidden by filters`]})]}),x&&(0,n.jsx)(Ce.D,{rule:e,instances:h,pagination:{itemsPerPage:2*Le.FG},footerRow:Q})]})},Pe=e=>({clickable:(0,d.css)`
    cursor: pointer;
  `,footerRow:(0,d.css)`
    display: flex;
    flex-direction: column;
    gap: ${e.spacing(1)};
    justify-content: space-between;
    align-items: center;
    width: 100%;
  `}),P="__ungrouped__",Oe=({rules:e,options:a})=>{const t=(0,E.of)(ae),o=a.groupBy,l=(0,y.useMemo)(()=>{const r=new Map,i=c=>o?Te(c,o):!0;e.forEach(c=>{const m=(0,R.TU)(c),u=i(c);(m?.alerts??[]).forEach(v=>{const h=u?Ue(o,v.labels):P,g=r.get(h)?.alerts??[];r.set(h,{rule:c,alerts:[...g,v]})})});const p=r.get(P)?.alerts??[];return r.delete(P),r.set(P,{alerts:p}),Array.from(r.entries()).reduce((c,[m,{rule:u,alerts:v}])=>{const h=te(a,v);return h.length>0&&c.set(m,{rule:u,alerts:h}),c},new Map)},[o,e,a]);return(0,n.jsx)(n.Fragment,{children:Array.from(l).map(([r,{rule:i,alerts:p}])=>(0,n.jsx)("li",{className:t.alertRuleItem,"data-testid":r,children:(0,n.jsxs)("div",{children:[(0,n.jsx)("div",{className:t.customGroupDetails,children:(0,n.jsxs)("div",{className:t.alertLabels,children:[r!==P&&Ge(r).map(([x,c])=>(0,n.jsx)(De.R,{labelKey:x,value:c},x)),r===P&&"No grouping"]})}),(0,n.jsx)(ue,{rule:i,alerts:p,options:a})]})},r))})};function Ue(e,a){return new URLSearchParams(e.map(t=>[t,a[t]])).toString()}function Ge(e){return[...new URLSearchParams(e)]}function Te(e,a){const t=(0,R.TU)(e);return a.every(o=>(t?.alerts??[]).some(l=>l.labels[o]))}const ze=Oe;var Ye=s(87586),Ke=s(70416),He=s(26058),fe=s(18461),Ze=s(3704);function ge(e){return Object.values(e).filter(a=>a!==void 0).reduce((a,t)=>a+t,0)}const We=({rules:e,options:a,handleInstancesLimit:t,limitInstances:o,hideViewRuleLinkText:l})=>{const r=(0,E.of)(ae),i=(0,E.of)(Je),{href:p}=(0,Ye.A)(),x=e.length<=a.maxItems?e:e.slice(0,a.maxItems);return(0,n.jsx)(n.Fragment,{children:(0,n.jsx)("ol",{className:r.alertRuleList,children:x.map((c,m)=>{const{namespaceName:u,groupName:v,dataSourceName:h}=c,g=(0,R.Z8)(c.promRule)?c.promRule:void 0,N=(0,R.Om)(g),b=(0,fe.UP)(c.dataSourceName,c),D=(0,fe.$9)(b),w=c.dataSourceName===j.hY?ge(c.instanceTotals):void 0,U=c.dataSourceName===j.hY?ge(c.filteredInstanceTotals):void 0,C=(0,Ze.G)(`/alerting/${encodeURIComponent(h)}/${encodeURIComponent(D)}/view`,{returnTo:p??""});return g?(0,n.jsxs)("li",{className:r.alertRuleItem,children:[(0,n.jsx)("div",{className:i.icon,children:(0,n.jsx)(z.I,{name:ie.A.getStateDisplayModel(g.state).iconClass,className:i[(0,R.Wy)(g.state)],size:"lg"})}),(0,n.jsxs)("div",{className:r.alertNameWrapper,children:[(0,n.jsxs)("div",{className:r.instanceDetails,children:[(0,n.jsxs)(T.B,{direction:"row",gap:1,children:[(0,n.jsx)("div",{className:r.alertName,title:c.name,children:c.name}),(0,n.jsx)(He.h,{}),C&&(0,n.jsxs)("a",{href:C,target:"__blank",className:r.link,rel:"noopener","aria-label":"View alert rule",children:[(0,n.jsx)("span",{className:(0,d.cx)({[r.hidden]:l}),children:"View alert rule"}),(0,n.jsx)(z.I,{name:"external-link-alt",size:"sm"})]})]}),(0,n.jsxs)("div",{className:r.alertDuration,children:[(0,n.jsx)("span",{className:i[(0,R.Wy)(g.state)],children:(0,R.XI)(g.state)})," ",N&&g.state!==S.cF.Inactive&&(0,n.jsxs)(n.Fragment,{children:["for"," ",(0,n.jsx)("span",{children:(0,Ke.dU)({start:N,end:Date.now()})})]})]})]}),(0,n.jsx)(ue,{rule:c,alerts:g.alerts??[],options:a,grafanaTotalInstances:w,grafanaFilteredInstancesTotal:U,handleInstancesLimit:t,limitInstances:o})]})]},`alert-${u}-${v}-${c.name}-${m}`):null})})})},Je=e=>({common:(0,d.css)`
    width: 70px;
    text-align: center;
    align-self: stretch;

    display: inline-block;
    color: white;
    border-radius: ${e.shape.radius.default};
    font-size: ${e.typography.bodySmall.fontSize};
    text-transform: capitalize;
    line-height: 1.2;
    flex-shrink: 0;

    display: flex;
    flex-direction: column;
    justify-content: center;
  `,icon:(0,d.css)`
    margin-top: ${e.spacing(2.5)};
    align-self: flex-start;
  `,good:(0,d.css)`
    color: ${e.colors.success.main};
  `,bad:(0,d.css)`
    color: ${e.colors.error.main};
  `,warning:(0,d.css)`
    color: ${e.colors.warning.main};
  `,neutral:(0,d.css)`
    color: ${e.colors.secondary.main};
  `,info:(0,d.css)`
    color: ${e.colors.primary.main};
  `}),Qe=We;function Xe(e){const a=(t,[o,l])=>l?[...t,o]:t;return Object.entries(e).reduce(a,[])}const H=({dispatch:e,limitInstances:a,matcherList:t,dataSourceName:o,stateList:l})=>{e(o?(0,Y.Lc)({rulesSourceName:o,limitAlerts:a?M.Ys:void 0,matcher:t,state:l}):(0,Y.yo)(!1,{limitAlerts:a?M.Ys:void 0,matcher:t,state:l}))};function ke(e){const a=(0,A.useDispatch)(),[t,o]=(0,xe.A)(!0),[,l]=(0,V.Ej)(V.RY.ViewAlertRule),{usePrometheusRulesByNamespaceQuery:r}=Ne.hK,i=(0,ee.$)(I=>I.promRules),p=(0,ee.$)(I=>I.rulerRules),x=(0,L.t4)(i),c=e.width<320;(0,y.useEffect)(()=>{e.options.stateFilter.inactive===!0&&(e.options.stateFilter.normal=!0),e.options.stateFilter.inactive=void 0},[e.options.stateFilter]);let m;(0,Re.A)(()=>{m=(0,de.UA)().getCurrent()});const u=(0,y.useMemo)(()=>Xe(e.options.stateFilter),[e.options.stateFilter]),{options:v,replaceVariables:h}=e,g=v.datasource===j.vv?j.hY:v.datasource,N={...e.options,alertName:h(v.alertName),alertInstanceLabelFilter:h(v.alertInstanceLabelFilter)},b=(0,y.useMemo)(()=>(0,ce.Zc)(N.alertInstanceLabelFilter),[N.alertInstanceLabelFilter]),D=(!g||g===j.hY)&&l,{currentData:w=[],isLoading:U,refetch:C}=r({limitAlerts:t?M.Ys:void 0,matcher:b,state:u},{skip:!D});(0,y.useEffect)(()=>{i.loading||H({dispatch:a,limitInstances:t,matcherList:b,dataSourceName:g,stateList:u});const I=m?.events.subscribe(Ae.sR,()=>{D&&C(),(!g||g!==j.hY)&&H({dispatch:a,limitInstances:t,matcherList:b,dataSourceName:g,stateList:u})});return()=>{I?.unsubscribe()}},[a,m,b,u,t,g,C,D,i.loading]);const se=I=>{I?(H({dispatch:a,limitInstances:t,matcherList:b,dataSourceName:g,stateList:u}),o(!0)):(H({dispatch:a,limitInstances:!1,matcherList:b,dataSourceName:g,stateList:u}),o(!1))},ne=(0,Ie.dy)(void 0,w),le=(0,L.t4)(p),Z=(0,L.BU)(i),re=x||le,oe=(0,L.Vf)(i),W=(0,E.of)(ae),J=(0,R.dS)(ne),Q=e.options.sortOrder,B=(0,y.useMemo)(()=>_e(e,qe(Q,J)),[J,Q,e]),me=B.length===0?"No alerts matching filters":void 0,at=U||re&&oe&&!Z,he=Object.values(i).some(I=>I.result);return(0,n.jsx)(Se.E,{autoHeightMin:"100%",autoHeightMax:"100%",children:(0,n.jsxs)("div",{className:W.container,children:[he&&me&&(0,n.jsx)("div",{className:W.noAlertsMessage,children:me}),he&&(0,n.jsxs)("section",{children:[e.options.viewMode===f.nE.Stat&&(0,n.jsx)(K.yV,{width:e.width,height:e.height,graphMode:K.$p.None,textMode:K.SV.Auto,justifyMode:K.F8.Auto,theme:be.$W.theme2,value:{text:`${B.length}`,numeric:B.length}}),e.options.viewMode===f.nE.List&&e.options.groupMode===f.r6.Custom&&(0,n.jsx)(ze,{rules:B,options:N}),e.options.viewMode===f.nE.List&&e.options.groupMode===f.r6.Default&&(0,n.jsx)(Qe,{rules:B,options:N,handleInstancesLimit:se,limitInstances:t,hideViewRuleLinkText:c})]}),at&&(0,n.jsx)(je._,{text:"Loading..."})]})})}function qe(e,a){if(e===f.xB.Importance)return(0,F.sortBy)(a,o=>ie.A.alertStateSortScore[o.state]);if(e===f.xB.TimeAsc)return(0,F.sortBy)(a,o=>{const l=(0,R.TU)(o)??void 0;return(0,R.Om)(l)||new Date});if(e===f.xB.TimeDesc)return(0,F.sortBy)(a,o=>{const l=(0,R.TU)(o)??void 0;return(0,R.Om)(l)||new Date}).reverse();const t=(0,F.sortBy)(a,o=>o.name.toLowerCase());return e===f.xB.AlphaDesc&&t.reverse(),t}function _e(e,a){const{options:t,replaceVariables:o}=e;let l=[...a];if(t.dashboardAlerts){const r=(0,de.UA)().getCurrent()?.uid;l=l.filter(({annotations:i={}})=>Object.entries(i).some(([p,x])=>p===$e.YH.dashboardUID&&x===r))}if(t.alertName){const r=o(t.alertName);l=l.filter(({name:i})=>i.toLocaleLowerCase().includes(r.toLocaleLowerCase()))}if(l=l.filter(r=>{const i=(0,R.TU)(r);return i?t.stateFilter.firing&&i.state===S.cF.Firing||t.stateFilter.pending&&i.state===S.cF.Pending||t.stateFilter.normal&&i.state===S.cF.Inactive:!1}),t.folder&&(l=l.filter(r=>r.namespaceName===t.folder.title)),t.datasource){const r=t.datasource===j.vv;l=l.filter(r?({dataSourceName:i})=>i===j.hY:({dataSourceName:i})=>i===t.datasource)}return l=l.reduce((r,i)=>{const p=(0,R.TU)(i);return(p?te({stateFilter:t.stateFilter,alertInstanceLabelFilter:o(t.alertInstanceLabelFilter)},p.alerts??[]):[]).length&&r.push(i),r},[]),l}const ae=e=>({cardContainer:(0,d.css)`
    padding: ${e.spacing(.5)} 0 ${e.spacing(.25)} 0;
    line-height: ${e.typography.body.lineHeight};
    margin-bottom: 0px;
  `,container:(0,d.css)`
    overflow-y: auto;
    height: 100%;
  `,alertRuleList:(0,d.css)`
    display: flex;
    flex-wrap: wrap;
    justify-content: space-between;
    list-style-type: none;
  `,alertRuleItem:(0,d.css)`
    display: flex;
    align-items: center;
    width: 100%;
    height: 100%;
    background: ${e.colors.background.secondary};
    padding: ${e.spacing(.5)} ${e.spacing(1)};
    border-radius: ${e.shape.radius.default};
    margin-bottom: ${e.spacing(.5)};

    gap: ${e.spacing(2)};
  `,alertName:(0,d.css)`
    font-size: ${e.typography.h6.fontSize};
    font-weight: ${e.typography.fontWeightBold};

    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  `,alertNameWrapper:(0,d.css)`
    display: flex;
    flex: 1;
    flex-wrap: nowrap;
    flex-direction: column;

    min-width: 100px;
  `,alertLabels:(0,d.css)`
    > * {
      margin-right: ${e.spacing(.5)};
    }
  `,alertDuration:(0,d.css)`
    font-size: ${e.typography.bodySmall.fontSize};
  `,alertRuleItemText:(0,d.css)`
    font-weight: ${e.typography.fontWeightBold};
    font-size: ${e.typography.bodySmall.fontSize};
    margin: 0;
  `,alertRuleItemTime:(0,d.css)`
    color: ${e.colors.text.secondary};
    font-weight: normal;
    white-space: nowrap;
  `,alertRuleItemInfo:(0,d.css)`
    font-weight: normal;
    flex-grow: 2;
    display: flex;
    align-items: flex-end;
  `,noAlertsMessage:(0,d.css)`
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 100%;
  `,alertIcon:(0,d.css)`
    margin-right: ${e.spacing(.5)};
  `,instanceDetails:(0,d.css)`
    min-width: 1px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  `,customGroupDetails:(0,d.css)`
    margin-bottom: ${e.spacing(.5)};
  `,link:(0,d.css)`
    word-break: break-all;
    color: ${e.colors.primary.text};
    display: flex;
    align-items: center;
    gap: ${e.spacing(1)};
  `,hidden:(0,d.css)`
    display: none;
  `});function et(e){const[,a]=(0,V.Ej)(V.RY.ViewAlertRule),[,t]=(0,V.Ej)(V.RY.ViewExternalAlertRule);return!a&&!t?(0,n.jsx)(Fe.F,{title:"Permission required",children:"Sorry, you do not have the required permissions to read alert rules"}):(0,n.jsx)(ke,{...e})}const tt=new X.m(et).setPanelOptions(e=>{e.addRadio({path:"viewMode",name:"View mode",description:"Toggle between list view and stat view",defaultValue:f.nE.List,settings:{options:[{label:"List",value:f.nE.List},{label:"Stat",value:f.nE.Stat}]},category:["Options"]}).addRadio({path:"groupMode",name:"Group mode",description:"How alert instances should be grouped",defaultValue:f.r6.Default,settings:{options:[{value:f.r6.Default,label:"Default grouping"},{value:f.r6.Custom,label:"Custom grouping"}]},category:["Options"]}).addCustomEditor({path:"groupBy",name:"Group by",description:"Filter alerts using label querying",id:"groupBy",defaultValue:[],showIf:a=>a.groupMode===f.r6.Custom,category:["Options"],editor:a=>(0,n.jsx)(ye,{id:a.id??"groupBy",defaultValue:a.value.map(t=>({label:t,value:t})),onChange:a.onChange,dataSource:a.context.options.datasource})}).addNumberInput({name:"Max items",path:"maxItems",description:"Maximum alerts to display",defaultValue:20,category:["Options"]}).addSelect({name:"Sort order",path:"sortOrder",description:"Sort order of alerts and alert instances",settings:{options:[{label:"Alphabetical (asc)",value:f.xB.AlphaAsc},{label:"Alphabetical (desc)",value:f.xB.AlphaDesc},{label:"Importance",value:f.xB.Importance},{label:"Time (asc)",value:f.xB.TimeAsc},{label:"Time (desc)",value:f.xB.TimeDesc}]},defaultValue:f.xB.AlphaAsc,category:["Options"]}).addBooleanSwitch({path:"dashboardAlerts",name:"Alerts linked to this dashboard",description:"Only show alerts linked to this dashboard",defaultValue:!1,category:["Options"]}).addTextInput({path:"alertName",name:"Alert name",description:"Filter for alerts containing this text",defaultValue:"",category:["Filter"]}).addTextInput({path:"alertInstanceLabelFilter",name:"Alert instance label",description:'Filter alert instances using label querying, ex: {severity="critical", instance=~"cluster-us-.+"}',defaultValue:"",category:["Filter"]}).addCustomEditor({path:"datasource",name:"Datasource",description:"Filter from alert source",id:"datasource",defaultValue:null,editor:function(t){return(0,n.jsxs)(T.B,{gap:1,children:[(0,n.jsx)(q.s,{...t,type:["prometheus","loki","grafana"],noDefault:!0,current:t.value,onChange:o=>t.onChange(o.name)}),(0,n.jsx)(O.$n,{variant:"secondary",onClick:()=>t.onChange(null),children:"Clear"})]})},category:["Filter"]}).addCustomEditor({showIf:a=>a.datasource===j.vv||!a.datasource,path:"folder",name:"Folder",description:"Filter for alerts in the selected folder (only for Grafana alerts)",id:"folder",defaultValue:null,editor:function(t){return(0,n.jsx)(k.sR,{enableReset:!0,showRoot:!1,allowEmpty:!0,initialTitle:t.value?.title,initialFolderUid:t.value?.uid,permissionLevel:A.PermissionLevelString.View,onClear:()=>t.onChange(""),...t})},category:["Filter"]}).addBooleanSwitch({path:"stateFilter.firing",name:"Alerting / Firing",defaultValue:!0,category:["Alert state filter"]}).addBooleanSwitch({path:"stateFilter.pending",name:"Pending",defaultValue:!0,category:["Alert state filter"]}).addBooleanSwitch({path:"stateFilter.noData",name:"No Data",defaultValue:!1,category:["Alert state filter"]}).addBooleanSwitch({path:"stateFilter.normal",name:"Normal",defaultValue:!1,category:["Alert state filter"]}).addBooleanSwitch({path:"stateFilter.error",name:"Error",defaultValue:!0,category:["Alert state filter"]})})}}]);

//# sourceMappingURL=alertListPanel.6d48cbdd01af1e33e62c.js.map
"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[1783],{80163:(b,S,t)=>{t.r(S),t.d(S,{default:()=>Ct});var e=t(68404),u=t(70356),m=t(45253),f=t(53546),p=t(81168),D=t(79572),I=t(94984),M=t(45524),R=t(731),T=t(88331),s=t(9892),O=t(82897),y=t(59052),W=t(70197),x=t(37556),j=t(69311),z=t(46519),X=t(35645),B=t(72648),fe=t(13393),N=t(24799),Y=t(46967),he=t(97379),F=t(31403),Ae=t(40014),V=t(39031),pe=t(31808),k=t(69945),Z=t(72004),K=t(20194),ae=t(66367),U=t(10505),q=t(46818),ye=t(19985),Se=t(61627),Ce=t(79662),ne=t(52694),se=t(53731),De=t(45608);const Me=()=>{const[a,n]=(0,e.useState)([]),r=(0,y.Gc)(),c=(0,p.useDispatch)(),{watch:i}=r,g=i("matchers"),l=(0,B.wW)(Pe),E=xe();(0,e.useEffect)(()=>{c((0,Z.ei)())},[c]);const o=(0,Se.Zo)();return(0,W.Z)(()=>{const h=o.flatMap(d=>d.groups.flatMap(A=>A.rules.map(v=>v.promRule).filter(v=>(0,Ce.x_)(v)).flatMap(v=>(0,ae.TZ)(v.alerts??[],g))));n(h)},500,[o,g]),e.createElement("div",null,e.createElement("h4",{className:l.title},"Affected alert instances",a.length>0?e.createElement(ye.C,{className:l.badge,color:"blue",text:a.length}):null),e.createElement("div",{className:l.table},g.every(h=>!h.value&&!h.name)?e.createElement("span",null,"Add a valid matcher to see affected alerts"):e.createElement(se.t,{items:a,isExpandable:!1,cols:E,pagination:{itemsPerPage:5}})))};function xe(){return[{id:"state",label:"State",renderCell:function({data:{matchedInstance:n}}){return e.createElement(De.l,{state:n.state})},size:"160px"},{id:"labels",label:"Labels",renderCell:function({data:{matchedInstance:n}}){return e.createElement(ne.s,{labels:n.labels})},size:"auto"},{id:"created",label:"Created",renderCell:function({data:{matchedInstance:n}}){return e.createElement(e.Fragment,null,n.activeAt.startsWith("0001")?"-":(0,z.CQ)(n.activeAt).format("YYYY-MM-DD HH:mm:ss"))},size:"180px"}]}const Pe=a=>({table:s.css`
    max-width: ${a.breakpoints.values.lg}px;
  `,moreMatches:s.css`
    margin-top: ${a.spacing(1)};
  `,title:s.css`
    display: flex;
    align-items: center;
  `,badge:s.css`
    margin-left: ${a.spacing(1)};
  `});var Te=t(63619),Oe=t(53217),Ie=t(8180);const Re=({className:a})=>{const n=(0,B.wW)(Be),r=(0,y.Gc)(),{control:c,register:i,formState:{errors:g}}=r,{fields:l=[],append:E,remove:o}=(0,y.Dq)({name:"matchers"});return e.createElement("div",{className:(0,s.cx)(a,n.wrapper)},e.createElement(N.g,{label:"Matching labels",required:!0},e.createElement("div",null,e.createElement("div",{className:n.matchers},l.map((h,d)=>e.createElement("div",{className:n.row,key:`${h.id}`,"data-testid":"matcher"},e.createElement(N.g,{label:"Label",invalid:!!g?.matchers?.[d]?.name,error:g?.matchers?.[d]?.name?.message},e.createElement(Y.I,{...i(`matchers.${d}.name`,{required:{value:!0,message:"Required."}}),defaultValue:h.name,placeholder:"label"})),e.createElement(N.g,{label:"Operator"},e.createElement(Te.g,{control:c,render:({field:{onChange:A,ref:v,...C}})=>e.createElement(Oe.Ph,{...C,onChange:H=>A(H.value),className:n.matcherOptions,options:K.tA,"aria-label":"operator"}),defaultValue:h.operator||K.tA[0].value,name:`matchers.${d}.operator`,rules:{required:{value:!0,message:"Required."}}})),e.createElement(N.g,{label:"Value",invalid:!!g?.matchers?.[d]?.value,error:g?.matchers?.[d]?.value?.message},e.createElement(Y.I,{...i(`matchers.${d}.value`,{required:{value:!0,message:"Required."}}),defaultValue:h.value,placeholder:"value"})),l.length>1&&e.createElement(Ie.h,{className:n.removeButton,tooltip:"Remove matcher",name:"trash-alt",onClick:()=>o(d)},"Remove")))),e.createElement(F.zx,{type:"button",icon:"plus",variant:"secondary",onClick:()=>{const h={name:"",value:"",operator:V._M.equal};E(h)}},"Add matcher"))))},Be=a=>({wrapper:s.css`
      margin-top: ${a.spacing(2)};
    `,row:s.css`
      display: flex;
      align-items: flex-start;
      flex-direction: row;
      background-color: ${a.colors.background.secondary};
      padding: ${a.spacing(1)} ${a.spacing(1)} 0 ${a.spacing(1)};
      & > * + * {
        margin-left: ${a.spacing(2)};
      }
    `,removeButton:s.css`
      margin-left: ${a.spacing(1)};
      margin-top: ${a.spacing(2.5)};
    `,matcherOptions:s.css`
      min-width: 140px;
    `,matchers:s.css`
      max-width: ${a.breakpoints.values.sm}px;
      margin: ${a.spacing(1)} 0;
      padding-top: ${a.spacing(.5)};
    `}),be=Re;var We=t(95794);const Ne=()=>{const{control:a,getValues:n}=(0,y.Gc)(),{field:{onChange:r,value:c},fieldState:{invalid:i}}=(0,y.bc)({name:"startsAt",control:a,rules:{validate:C=>n().endsAt>C}}),{field:{onChange:g,value:l},fieldState:{invalid:E}}=(0,y.bc)({name:"endsAt",control:a,rules:{validate:C=>n().startsAt<C}}),{field:{onChange:o,value:h}}=(0,y.bc)({name:"timeZone",control:a}),d=i||E,A=(0,z.CQ)(c),v=(0,z.CQ)(l);return e.createElement(N.g,{className:$e.timeRange,label:"Silence start and end",error:d?"To is before or the same as from":"",invalid:d},e.createElement(We.K,{value:{from:A,to:v,raw:{from:A,to:v}},timeZone:h,onChange:C=>{r((0,z.CQ)(C.from)),g((0,z.CQ)(C.to))},onChangeTimeZone:C=>o(C),hideTimeZone:!1,hideQuickRanges:!0,placeholder:"Select time range"}))},$e={timeRange:s.css`
    width: 400px;
  `},Le=a=>{const n={},r=a.get("comment"),c=a.getAll("matcher"),i=(0,ae.RT)(c);return i.length&&(n.matchers=i.map(K.cm)),r&&(n.comment=r),n},Ke=(a,n)=>{const r=new Date;if(n){const i=Date.parse(n.endsAt)<Date.now()?{start:r,end:(0,x.Ks)(r,{hours:2})}:{start:new Date(n.startsAt),end:new Date(n.endsAt)};return{id:n.id,startsAt:i.start.toISOString(),endsAt:i.end.toISOString(),comment:n.comment,createdBy:n.createdBy,duration:(0,x.vT)(i),isRegex:!1,matchers:n.matchers?.map(K.cm)||[],matcherName:"",matcherValue:"",timeZone:j.Ys}}else{const c=(0,x.Ks)(r,{hours:2});return{id:"",startsAt:r.toISOString(),endsAt:c.toISOString(),comment:`created ${(0,z.CQ)().format("YYYY-MM-DD HH:mm")}`,createdBy:X.v.bootData.user.name,duration:"2h",isRegex:!1,matchers:[{name:"",value:"",operator:V._M.equal}],matcherName:"",matcherValue:"",timeZone:j.Ys,...Le(a)}}},Ue=({silence:a,alertManagerSourceName:n})=>{const[r]=(0,pe.j)(),c=(0,e.useMemo)(()=>Ke(r,a),[a,r]),i=(0,y.cI)({defaultValues:c}),g=(0,p.useDispatch)(),l=(0,B.wW)(Qe),{loading:E}=(0,k._)(L=>L.updateSilence);(0,Ae.x)(L=>L.unifiedAlerting.updateSilence=q.oq);const{register:o,handleSubmit:h,formState:d,watch:A,setValue:v,clearErrors:C}=i,H=L=>{const{id:Ee,startsAt:J,endsAt:Mt,comment:xt,createdBy:Pt,matchers:Tt}=L,Ot=Tt.map(K._J),ve=(0,O.pickBy)({id:Ee,startsAt:J,endsAt:Mt,comment:xt,createdBy:Pt,matchers:Ot},It=>!!It);g((0,Z.QY)({alertManagerSourceName:n,payload:ve,exitOnSave:!0,successMessage:`Silence ${ve.id?"updated":"created"}`}))},$=A("duration"),P=A("startsAt"),Q=A("endsAt"),[ue,ge]=(0,e.useState)($);(0,W.Z)(()=>{if((0,x.qb)(P)&&(0,x.qb)(Q))if($!==ue)v("endsAt",(0,z.CQ)((0,x.Ks)(new Date(P),(0,x.RA)($))).toISOString()),ge($);else{const L=new Date(P).valueOf();if(new Date(Q).valueOf()>L){const J=(0,x.vT)({start:new Date(P),end:new Date(Q)});v("duration",J),ge(J)}}},700,[C,$,Q,ue,v,P]);const Dt=Boolean(X.v.bootData.user.isSignedIn&&X.v.bootData.user.name);return e.createElement(y.RV,{...i},e.createElement("form",{onSubmit:h(H)},e.createElement(fe.C,{label:`${a?"Recreate silence":"Create silence"}`},e.createElement("div",{className:(0,s.cx)(l.flexRow,l.silencePeriod)},e.createElement(Ne,null),e.createElement(N.g,{label:"Duration",invalid:!!d.errors.duration,error:d.errors.duration&&(d.errors.duration.type==="required"?"Required field":d.errors.duration.message)},e.createElement(Y.I,{className:l.createdBy,...o("duration",{validate:L=>Object.keys((0,x.RA)(L)).length===0?"Invalid duration. Valid example: 1d 4h (Available units: y, M, w, d, h, m, s)":void 0}),id:"duration"}))),e.createElement(be,null),e.createElement(N.g,{className:(0,s.cx)(l.field,l.textArea),label:"Comment",required:!0,error:d.errors.comment?.message,invalid:!!d.errors.comment},e.createElement(he.K,{...o("comment",{required:{value:!0,message:"Required."}}),rows:5,placeholder:"Details about the silence"})),!Dt&&e.createElement(N.g,{className:(0,s.cx)(l.field,l.createdBy),label:"Created By",required:!0,error:d.errors.createdBy?.message,invalid:!!d.errors.createdBy},e.createElement(Y.I,{...o("createdBy",{required:{value:!0,message:"Required."}}),placeholder:"Who's creating the silence"})),e.createElement(Me,null)),e.createElement("div",{className:l.flexRow},E&&e.createElement(F.zx,{disabled:!0,icon:"fa fa-spinner",variant:"primary"},"Saving..."),!E&&e.createElement(F.zx,{type:"submit"},"Submit"),e.createElement(F.Qj,{href:(0,U.eQ)("alerting/silences",n),variant:"secondary",fill:"outline"},"Cancel"))))},Qe=a=>({field:s.css`
    margin: ${a.spacing(1,0)};
  `,textArea:s.css`
    max-width: ${a.breakpoints.values.sm}px;
  `,createdBy:s.css`
    width: 200px;
  `,flexRow:s.css`
    display: flex;
    flex-direction: row;
    justify-content: flex-start;

    & > * {
      margin-right: ${a.spacing(1)};
    }
  `,silencePeriod:s.css`
    max-width: ${a.breakpoints.values.sm}px;
  `}),re=Ue;var w=t(21053),le=t(26418),ce=t(29460),_=t(39904),ee=t(96044),G=t(82002),te=t(97953),ze=t(51981);const oe=({className:a,...n})=>{const r=(0,B.wW)(Fe);return e.createElement(F.zx,{variant:"secondary",size:"xs",className:(0,s.cx)(r.wrapper,a),...n})},Fe=a=>({wrapper:s.css`
    height: 24px;
    font-size: ${a.typography.bodySmall.fontSize};
  `});var Ve=t(65750),Ye=t(34807);const Ze=({matchers:a})=>{const n=(0,B.wW)(Ge);return e.createElement("div",null,e.createElement(Ye.P,{className:n.tags,tags:a.map(r=>`${r.name}${(0,K.zy)(r)}${r.value}`)}))},Ge=()=>({tags:s.css`
    justify-content: flex-start;
  `});var He=t(97097),je=t(69142);const we=({alertManagerSourceName:a})=>{const n=(0,te.QX)(a);return G.Vt.hasAccess(n.create,G.Vt.isEditor)?e.createElement(je.Z,{title:"You haven't created any silences yet",buttonIcon:"bell-slash",buttonLink:(0,U.eQ)("alerting/silence/new",a),buttonTitle:"Create silence"}):e.createElement(He._,{callToActionElement:e.createElement("div",null),message:"No silences found."})};var Je=t(85655),Xe=t(33950),ke=t(90072);const qe=({alert:a,className:n})=>{const[r,c]=(0,e.useState)(!0),i=(0,x.vT)({start:new Date(a.startsAt),end:new Date(a.endsAt)}),g=Object.entries(a.labels).reduce((l,[E,o])=>((E==="alertname"||E==="__alert_rule_title__")&&(l=o),l),"");return e.createElement(e.Fragment,null,e.createElement("tr",{className:n},e.createElement("td",null,e.createElement(Xe.U,{isCollapsed:r,onToggle:l=>c(l)})),e.createElement("td",null,e.createElement(ke.G,{state:a.status.state})),e.createElement("td",null,"for ",i," seconds"),e.createElement("td",null,g)),!r&&e.createElement("tr",{className:n},e.createElement("td",null),e.createElement("td",{colSpan:5},e.createElement(ne.s,{labels:a.labels}))))},_e=({silencedAlerts:a})=>{const n=(0,B.wW)(Je.D),r=(0,B.wW)(et);return a.length?e.createElement("table",{className:(0,s.cx)(n.table,r.tableMargin)},e.createElement("colgroup",null,e.createElement("col",{className:n.colExpand}),e.createElement("col",{className:r.colState}),e.createElement("col",null),e.createElement("col",{className:r.colName})),e.createElement("thead",null,e.createElement("tr",null,e.createElement("th",null),e.createElement("th",null,"State"),e.createElement("th",null),e.createElement("th",null,"Alert name"))),e.createElement("tbody",null,a.map((c,i)=>e.createElement(qe,{key:c.fingerprint,alert:c,className:i%2===0?n.evenRow:""})))):null},et=a=>({tableMargin:s.css`
    margin-bottom: ${a.spacing(1)};
  `,colState:s.css`
    width: 110px;
  `,colName:s.css`
    width: 65%;
  `}),tt=_e,at=({silence:a})=>{const{startsAt:n,endsAt:r,comment:c,createdBy:i,silencedAlerts:g}=a,l=(0,B.wW)(nt),E="YYYY-MM-DD HH:mm",o=w.parse(n),h=w.parse(r),d=(0,x.vT)({start:new Date(n),end:new Date(r)});return e.createElement("div",{className:l.container},e.createElement("div",{className:l.title},"Comment"),e.createElement("div",null,c),e.createElement("div",{className:l.title},"Schedule"),e.createElement("div",null,`${o?.format(E)} - ${h?.format(E)}`),e.createElement("div",{className:l.title},"Duration"),e.createElement("div",null," ",d),e.createElement("div",{className:l.title},"Created by"),e.createElement("div",null," ",i),e.createElement("div",{className:l.title},"Affected alerts"),e.createElement(tt,{silencedAlerts:g}))},nt=a=>({container:s.css`
    display: grid;
    grid-template-columns: 1fr 9fr;
    grid-row-gap: 1rem;
  `,title:s.css`
    color: ${a.colors.text.primary};
  `,row:s.css`
    margin: ${a.spacing(1,0)};
  `});var st=t(79453);const rt={[V.As.Active]:"good",[V.As.Expired]:"neutral",[V.As.Pending]:"neutral"},lt=({state:a})=>e.createElement(st.i,{state:rt[a]},a);var ct=t(64353),ot=t(6554),it=t(2594);const dt=Object.entries(V.As).map(([a,n])=>({label:a,value:n})),ie=()=>(0,O.uniqueId)("query-string-"),mt=()=>{const[a,n]=(0,e.useState)(ie()),[r,c]=(0,ee.K)(),{queryString:i,silenceState:g}=(0,U.pF)(r),l=(0,B.wW)(ut),E=(0,O.debounce)(A=>{const v=A.target;c({queryString:v.value||null})},400),o=A=>{c({silenceState:A})},h=()=>{c({queryString:null,silenceState:null}),setTimeout(()=>n(ie()))},d=i&&i.length>3?(0,K.Zh)(i).length===0:!1;return e.createElement("div",{className:l.flexRow},e.createElement(N.g,{className:l.rowChild,label:e.createElement(ct._,null,e.createElement(le.Stack,{gap:.5},e.createElement("span",null,"Search by matchers"),e.createElement(ot.u,{content:e.createElement("div",null,"Filter silences by matchers using a comma separated list of matchers, ie:",e.createElement("pre",null,"severity=critical, instance=~cluster-us-.+"))},e.createElement(_.J,{name:"info-circle",size:"sm"})))),invalid:d,error:d?"Query must use valid matcher syntax":null},e.createElement(Y.I,{key:a,className:l.searchInput,prefix:e.createElement(_.J,{name:"search"}),onChange:E,defaultValue:i??"",placeholder:"Search","data-testid":"search-query-input"})),e.createElement(N.g,{className:l.rowChild,label:"State"},e.createElement(it.S,{options:dt,value:g,onChange:o})),(i||g)&&e.createElement("div",{className:l.rowChild},e.createElement(F.zx,{variant:"secondary",icon:"times",onClick:h},"Clear filters")))},ut=a=>({searchInput:s.css`
    width: 360px;
  `,flexRow:s.css`
    display: flex;
    flex-direction: row;
    align-items: flex-end;
    padding-bottom: ${a.spacing(2)};
    border-bottom: 1px solid ${a.colors.border.strong};
  `,rowChild:s.css`
    margin-right: ${a.spacing(1)};
    margin-bottom: 0;
    max-height: 52px;
  `,fieldLabel:s.css`
    font-size: 12px;
    font-weight: 500;
  `}),gt=({silences:a,alertManagerAlerts:n,alertManagerSourceName:r})=>{const c=(0,B.wW)(de),[i]=(0,ee.K)(),g=Et(a),l=(0,te.QX)(r),{silenceState:E}=(0,U.pF)(i),o=!!g.length&&(E===void 0||E===V.As.Expired),h=vt(r),d=(0,e.useMemo)(()=>{const A=v=>n.filter(C=>C.status.silencedBy.includes(v));return g.map(v=>{const C=A(v.id);return{id:v.id,data:{...v,silencedAlerts:C}}})},[g,n]);return e.createElement("div",{"data-testid":"silences-table"},!!a.length&&e.createElement(e.Fragment,null,e.createElement(mt,null),e.createElement(ze.q,{actions:[l.create],fallback:G.Vt.isEditor},e.createElement("div",{className:c.topButtonContainer},e.createElement(ce.r,{href:(0,U.eQ)("/alerting/silence/new",r)},e.createElement(F.zx,{className:c.addNewSilence,icon:"plus"},"Add Silence")))),d.length?e.createElement(e.Fragment,null,e.createElement(se.t,{items:d,cols:h,isExpandable:!0,renderExpandedContent:({data:A})=>e.createElement(at,{silence:A})}),o&&e.createElement("div",{className:c.callout},e.createElement(_.J,{className:c.calloutIcon,name:"info-circle"}),e.createElement("span",null,"Expired silences are automatically deleted after 5 days."))):"No matching silences found"),!a.length&&e.createElement(we,{alertManagerSourceName:r}))},Et=a=>{const[n]=(0,ee.K)();return(0,e.useMemo)(()=>{const{queryString:r,silenceState:c}=(0,U.pF)(n),i=n?.silenceIds;return a.filter(g=>!(typeof i=="string"&&!i.split(",").includes(g.id)||r&&!(0,K.Zh)(r).every(o=>g.matchers?.some(({name:h,value:d,isEqual:A,isRegex:v})=>o.name===h&&o.value===d&&o.isEqual===A&&o.isRegex===v))||c&&!(g.status.state===c)))},[n,a])},de=a=>({topButtonContainer:s.css`
    display: flex;
    flex-direction: row;
    justify-content: flex-end;
  `,addNewSilence:s.css`
    margin: ${a.spacing(2,0)};
  `,callout:s.css`
    background-color: ${a.colors.background.secondary};
    border-top: 3px solid ${a.colors.info.border};
    border-radius: ${a.shape.borderRadius()};
    height: 62px;
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-top: ${a.spacing(2)};

    & > * {
      margin-left: ${a.spacing(1)};
    }
  `,calloutIcon:s.css`
    color: ${a.colors.info.text};
  `,editButton:s.css`
    margin-left: ${a.spacing(.5)};
  `});function vt(a){const n=(0,p.useDispatch)(),r=(0,B.wW)(de),c=(0,te.QX)(a);return(0,e.useMemo)(()=>{const i=E=>{n((0,Z.yO)(a,E))},g=G.Vt.hasAccess(c.update,G.Vt.isEditor),l=[{id:"state",label:"State",renderCell:function({data:{status:o}}){return e.createElement(lt,{state:o.state})},size:4},{id:"matchers",label:"Matching labels",renderCell:function({data:{matchers:o}}){return e.createElement(Ze,{matchers:o||[]})},size:10},{id:"alerts",label:"Alerts",renderCell:function({data:{silencedAlerts:o}}){return e.createElement("span",{"data-testid":"alerts"},o.length)},size:4},{id:"schedule",label:"Schedule",renderCell:function({data:{startsAt:o,endsAt:h}}){const d=w.parse(o),A=w.parse(h),v="YYYY-MM-DD HH:mm";return e.createElement(e.Fragment,null," ",d?.format(v)," ","-",A?.format(v))},size:7}];return g&&l.push({id:"actions",label:"Actions",renderCell:function({data:o}){return e.createElement(le.Stack,{gap:.5},o.status.state==="expired"?e.createElement(ce.r,{href:(0,U.eQ)(`/alerting/silence/${o.id}/edit`,a)},e.createElement(oe,{icon:"sync"},"Recreate")):e.createElement(oe,{icon:"bell",onClick:()=>i(o.id)},"Unsilence"),o.status.state!=="expired"&&e.createElement(Ve.A,{className:r.editButton,to:(0,U.eQ)(`/alerting/silence/${o.id}/edit`,a),icon:"pen",tooltip:"edit"}))},size:5}),l},[a,n,r,c])}const ft=gt;var ht=t(29614),At=t(23403);const me={icon:"bell-slash",breadcrumbs:[{title:"Silences",url:"alerting/silences"}]};function pt(){const{isExact:a,path:n}=(0,u.$B)(),[r,c]=(0,e.useState)();return(0,e.useEffect)(()=>{n==="/alerting/silence/new"?c({...me,id:"silence-new",text:"Add silence"}):n==="/alerting/silence/:id/edit"&&c({...me,id:"silence-edit",text:"Edit silence"})},[n,a]),r}var yt=t(37190);const St=()=>{const a=(0,At.k)("instance"),[n,r]=(0,ht.k)(a),c=(0,p.useDispatch)(),i=(0,k._)(P=>P.silences),g=(0,k._)(P=>P.amAlerts),l=n?g[n]||q.oq:void 0,E=(0,u.TH)(),o=pt(),h=E.pathname.endsWith("/alerting/silences"),{currentData:d}=D.T.useDiscoverAmFeaturesQuery({amSourceName:n??""},{skip:!n});(0,e.useEffect)(()=>{function P(){n&&(c((0,Z.je)(n)),c((0,Z.dB)(n)))}P();const Q=setInterval(()=>P,yt.cm);return()=>{clearInterval(Q)}},[n,c]);const{result:A,loading:v,error:C}=n&&i[n]||q.oq,H=(0,e.useCallback)(P=>A&&A.find(Q=>Q.id===P),[A]),$=C?.message?.includes("the Alertmanager is not configured")&&d?.lazyConfigInit;return n?e.createElement(M.J,{pageId:"silences",isLoading:v,pageNav:o},e.createElement(I.P,{disabled:!h,current:n,onChange:r,dataSources:a}),e.createElement(R.u,{currentAlertmanager:n}),$&&e.createElement(m.b,{title:"The selected Alertmanager has no configuration",severity:"warning"},"Create a new contact point to create a configuration using the default values or contact your administrator to set up the Alertmanager."),C&&!v&&!$&&e.createElement(m.b,{severity:"error",title:"Error loading silences"},C.message||"Unknown error."),l?.error&&!l?.loading&&!$&&e.createElement(m.b,{severity:"error",title:"Error loading Alertmanager alerts"},l.error?.message||"Unknown error."),A&&!C&&e.createElement(u.rs,null,e.createElement(u.AW,{exact:!0,path:"/alerting/silences"},e.createElement(ft,{silences:A,alertManagerAlerts:l?.result??[],alertManagerSourceName:n})),e.createElement(u.AW,{exact:!0,path:"/alerting/silence/new"},e.createElement(re,{alertManagerSourceName:n})),e.createElement(u.AW,{exact:!0,path:"/alerting/silence/:id/edit"},({match:P})=>P?.params.id&&e.createElement(re,{silence:H(P.params.id),alertManagerSourceName:n})))):h?e.createElement(M.J,{pageId:"silences",pageNav:o},e.createElement(T.I,{availableAlertManagers:a})):e.createElement(u.l_,{to:"/alerting/silences"})},Ct=(0,f.Pf)(St,{style:"page"})},30173:(b,S,t)=>{t.d(S,{h:()=>u});var e=t(29427);const u=e.C.injectEndpoints({endpoints:m=>({getAlertmanagerChoiceStatus:m.query({query:()=>({url:"/api/v1/ngalert"}),providesTags:["AlertmanagerChoice"]}),getExternalAlertmanagerConfig:m.query({query:()=>({url:"/api/v1/ngalert/admin_config"}),providesTags:["AlertmanagerChoice"]}),getExternalAlertmanagers:m.query({query:()=>({url:"/api/v1/ngalert/alertmanagers"}),transformResponse:f=>f.data}),saveExternalAlertmanagersConfig:m.mutation({query:f=>({url:"/api/v1/ngalert/admin_config",method:"POST",data:f}),invalidatesTags:["AlertmanagerChoice"]})})})},51981:(b,S,t)=>{t.d(S,{q:()=>m});var e=t(68404),u=t(82002);const m=({actions:f,children:p,fallback:D=!0})=>f.some(I=>u.Vt.hasAccess(I,D))?e.createElement(e.Fragment,null,p):null},731:(b,S,t)=>{t.d(S,{u:()=>M});var e=t(9892),u=t(68404),m=t(72648),f=t(45253),p=t(39031),D=t(30173),I=t(45849);function M({currentAlertmanager:T}){const s=(0,m.wW)(R),{useGetAlertmanagerChoiceStatusQuery:O}=D.h,{currentData:y}=O(),W=T===I.GC;if(!(y?.alertmanagersChoice&&[p.TE.External,p.TE.All].includes(y?.alertmanagersChoice))||!W)return null;const j=y.numExternalAlertmanagers>0;return y.alertmanagersChoice===p.TE.External?u.createElement(f.b,{title:"Grafana alerts are not delivered to Grafana Alertmanager"},"Grafana is configured to send alerts to external Alertmanagers only. Changing Grafana Alertmanager configuration will not affect delivery of your alerts.",u.createElement("div",{className:s.adminHint},"To change your Alertmanager setup, go to the Alerting Admin page. If you do not have access, contact your Administrator.")):y.alertmanagersChoice===p.TE.All&&j?u.createElement(f.b,{title:"You have additional Alertmanagers to configure",severity:"warning"},"Ensure you make configuration changes in the correct Alertmanagers; both internal and external. Changing one will not affect the others.",u.createElement("div",{className:s.adminHint},"To change your Alertmanager setup, go to the Alerting Admin page. If you do not have access, contact your Administrator.")):null}const R=T=>({adminHint:e.css`
    font-size: ${T.typography.bodySmall.fontSize};
    font-weight: ${T.typography.bodySmall.fontWeight};
  `})},88331:(b,S,t)=>{t.d(S,{I:()=>I});var e=t(68404),u=t(45253),m=t(29614),f=t(94984);const p=()=>e.createElement(u.b,{title:"No Alertmanager found",severity:"warning"},"We could not find any external Alertmanagers and you may not have access to the built-in Grafana Alertmanager."),D=()=>e.createElement(u.b,{title:"Selected Alertmanager not found. Select a different Alertmanager.",severity:"warning"},"Selected Alertmanager no longer exists or you may not have permission to access it."),I=({availableAlertManagers:M})=>{const[R,T]=(0,m.k)(M),s=M.length>0;return e.createElement("div",null,s?e.createElement(e.Fragment,null,e.createElement(f.P,{onChange:T,dataSources:M}),e.createElement(D,null)):e.createElement(p,null))}},65750:(b,S,t)=>{t.d(S,{A:()=>f});var e=t(68404),u=t(6554),m=t(31403);const f=({tooltip:p,icon:D,to:I,target:M,onClick:R,className:T,tooltipPlacement:s="top",...O})=>{const y=typeof p=="string"?p:void 0;return e.createElement(u.u,{content:p,placement:s},I?e.createElement(m.Qj,{variant:"secondary",fill:"text",icon:D,href:I,size:"sm",target:M,...O,"aria-label":y}):e.createElement(m.zx,{className:T,variant:"secondary",fill:"text",size:"sm",icon:D,type:"button",onClick:R,...O,"aria-label":y}))}},90072:(b,S,t)=>{t.d(S,{G:()=>p});var e=t(68404),u=t(39031),m=t(79453);const f={[u.Z9.Active]:"bad",[u.Z9.Unprocessed]:"neutral",[u.Z9.Suppressed]:"info"},p=({state:D})=>e.createElement(m.i,{state:f[D]},D)},29614:(b,S,t)=>{t.d(S,{k:()=>I});var e=t(68404),u=t(96044),m=t(58379),f=t(37190),p=t(45849);function D(M){return(0,e.useCallback)(R=>M.map(s=>s.name).includes(R),[M])}function I(M){const[R,T]=(0,u.K)(),s=D(M),O=(0,e.useCallback)(x=>{s(x)&&(x===p.GC?(m.Z.delete(f.de),T({[f.c4]:null})):(m.Z.set(f.de,x),T({[f.c4]:x})))},[T,s]),y=R[f.c4];if(y&&typeof y=="string")return s(y)?[y,O]:[void 0,O];const W=m.Z.get(f.de);return W&&typeof W=="string"&&s(W)?(O(W),[W,O]):s(p.GC)?[p.GC,O]:[void 0,O]}},23403:(b,S,t)=>{t.d(S,{k:()=>m});var e=t(68404),u=t(45849);function m(f){return(0,e.useMemo)(()=>(0,u.LE)(f),[f])}},31808:(b,S,t)=>{t.d(S,{j:()=>f});var e=t(68404),u=t(70356),m=t(37932);function f(){const{search:p}=(0,u.TH)(),D=(0,e.useMemo)(()=>new URLSearchParams(p),[p]),I=(0,e.useCallback)((M,R)=>{m.E1.partial(M,R)},[]);return[D,I]}},85655:(b,S,t)=>{t.d(S,{D:()=>u});var e=t(9892);const u=m=>({table:e.css`
    width: 100%;
    border-radius: ${m.shape.borderRadius()};
    border: solid 1px ${m.colors.border.weak};
    background-color: ${m.colors.background.secondary};

    th {
      padding: ${m.spacing(1)};
    }

    td {
      padding: 0 ${m.spacing(1)};
    }

    tr {
      height: 38px;
    }
  `,evenRow:e.css`
    background-color: ${m.colors.background.primary};
  `,colExpand:e.css`
    width: 36px;
  `,actionsCell:e.css`
    text-align: right;
    width: 1%;
    white-space: nowrap;

    & > * + * {
      margin-left: ${m.spacing(.5)};
    }
  `})}}]);

//# sourceMappingURL=AlertSilences.adc12576e4433f99a73b.js.map
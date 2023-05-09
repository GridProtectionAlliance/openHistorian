"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[8808],{40404:(J,I,t)=>{t.d(I,{p:()=>W});var s=t(9892),e=t(68404),l=t(35645),P=t(72648),b=t(39904),f=t(29460),p=t(35974),S=t(89710),D=t(49922),O=t(35029),m=t(31403),h=t(74391),n=t(54605),L=t(86648),x=t(62992);const A={loadingState:D.Gu.Loading,dashboardTitles:[]},i=(0,x.PH)("libraryPanels/delete/searchCompleted"),C=(o=A,v)=>i.match(v)?{...o,dashboardTitles:v.payload.dashboards.map(T=>T.title),loadingState:D.Gu.Done}:o;function $(o){return async function(v){const T=await(0,L.E8)(o.uid);v(i({dashboards:T}))}}const M=({libraryPanel:o,onDismiss:v,onConfirm:T})=>{const G=(0,P.wW)(h.J),[{dashboardTitles:F,loadingState:B},j]=(0,e.useReducer)(C,A),V=(0,e.useMemo)(()=>(0,n.tb)(j),[j]);(0,e.useEffect)(()=>{V($(o))},[V,o]);const H=Boolean(F.length),Q=B===D.Gu.Done;return e.createElement(O.u,{className:G.modal,title:"Delete library panel",icon:"trash-alt",onDismiss:v,isOpen:!0},Q?null:e.createElement(R,null),Q?e.createElement("div",null,H?e.createElement(U,{dashboardTitles:F}):null,H?null:e.createElement(g,null),e.createElement(O.u.ButtonRow,null,e.createElement(m.zx,{variant:"secondary",onClick:v,fill:"outline"},"Cancel"),e.createElement(m.zx,{variant:"destructive",onClick:T,disabled:H},"Delete"))):null)},R=()=>e.createElement("span",null,"Loading library panel..."),g=()=>{const o=(0,P.wW)(h.J);return e.createElement("div",{className:o.modalText},"Do you want to delete this panel?")},U=({dashboardTitles:o})=>{const v=(0,P.wW)(h.J),T=o.length===1?"dashboard.":"dashboards.",G=`${o.length} ${T}`;return o.length===0?null:e.createElement("div",null,e.createElement("p",{className:v.textInfo},"This library panel can not be deleted because it is connected to ",e.createElement("strong",null,G)," Remove the library panel from the dashboards listed below and retry."),e.createElement("table",{className:v.myTable},e.createElement("thead",null,e.createElement("tr",null,e.createElement("th",null,"Dashboard name"))),e.createElement("tbody",null,o.map((F,B)=>e.createElement("tr",{key:`dash-title-${B}`},e.createElement("td",null,F))))))},W=({libraryPanel:o,onClick:v,onDelete:T,showSecondaryActions:G})=>{const[F,B]=(0,e.useState)(!1),j=()=>{T?.(o),B(!1)},V=l.v.panels[o.model.type]??(0,p.X)(o.model.type).meta;return e.createElement(e.Fragment,null,e.createElement(S.X,{isCurrent:!1,title:o.name,description:o.description,plugin:V,onClick:()=>v?.(o),onDelete:G?()=>B(!0):void 0},e.createElement(K,{libraryPanel:o})),F&&e.createElement(M,{libraryPanel:o,onConfirm:j,onDismiss:()=>B(!1)}))};function K({libraryPanel:o}){const v=(0,P.wW)(z);return!o.meta?.folderUid&&!o.meta?.folderName?null:o.meta.folderUid?e.createElement("span",{className:v.metaContainer},e.createElement(f.r,{href:`/dashboards/f/${o.meta.folderUid}`},e.createElement(b.J,{name:"folder-upload",size:"sm"}),e.createElement("span",null,o.meta.folderName))):e.createElement("span",{className:v.metaContainer},e.createElement(b.J,{name:"folder",size:"sm"}),e.createElement("span",null,o.meta.folderName))}function z(o){return{metaContainer:s.css`
      display: flex;
      align-items: center;
      color: ${o.colors.text.secondary};
      font-size: ${o.typography.bodySmall.fontSize};
      padding-top: ${o.spacing(.5)};

      svg {
        margin-right: ${o.spacing(.5)};
        margin-bottom: 3px;
      }
    `}}},54605:(J,I,t)=>{t.d(I,{UO:()=>x,Xu:()=>L,tb:()=>A});var s=t(53376),e=t(49372),l=t(59980),P=t(12877),b=t(25740),f=t(39859),p=t(59724),S=t(86318),D=t(57027),O=t(46978),m=t(82615),h=t(86648),n=t(4377);function L(i){return function(C){const $=new s.w0,M=(0,e.D)((0,h.Pq)({searchString:i.searchString,perPage:i.perPage,page:i.page,excludeUid:i.currentPanelId,sortDirection:i.sortDirection,typeFilter:i.panelFilter,folderFilterUIDs:i.folderFilterUIDs})).pipe((0,f.z)(({perPage:R,elements:g,page:U,totalCount:W})=>(0,l.of)((0,n.zK)({libraryPanels:g,page:U,perPage:R,totalCount:W}))),(0,p.K)(R=>(console.error(R),(0,l.of)((0,n.zK)({...n.p$,page:i.page,perPage:i.perPage})))),(0,S.x)(()=>$.unsubscribe()),(0,D.B)());$.add((0,P.T)((0,b.H)(50).pipe((0,O.h)((0,n.xU)()),(0,m.R)(M)),M).subscribe(C))}}function x(i,C){return async function($){try{await(0,h.UO)(i),L(C)($)}catch(M){console.error(M)}}}function A(i){return function(C){return C instanceof Function?C(i):i(C)}}},4377:(J,I,t)=>{t.d(I,{_p:()=>p,oO:()=>f,p$:()=>l,xU:()=>P,zK:()=>b});var s=t(62992),e=t(49922);const l={loadingState:e.Gu.Loading,libraryPanels:[],totalCount:0,perPage:40,page:1,numberOfPages:0,currentPanelId:void 0},P=(0,s.PH)("libraryPanels/view/initSearch"),b=(0,s.PH)("libraryPanels/view/searchCompleted"),f=(0,s.PH)("libraryPanels/view/changePage"),p=(S,D)=>{if(P.match(D))return{...S,loadingState:e.Gu.Loading};if(b.match(D)){const{libraryPanels:O,page:m,perPage:h,totalCount:n}=D.payload,L=Math.ceil(n/h);return{...S,libraryPanels:O,perPage:h,totalCount:n,loadingState:e.Gu.Done,numberOfPages:L,page:m>L?m-1:m}}return f.match(D)?{...S,page:D.payload.page}:S}},74391:(J,I,t)=>{t.d(I,{J:()=>e});var s=t(9892);function e(l){return{myTable:s.css`
      max-height: 204px;
      overflow-y: auto;
      margin-top: 11px;
      margin-bottom: 28px;
      border-radius: ${l.shape.borderRadius(1)};
      border: 1px solid ${l.colors.action.hover};
      background: ${l.colors.background.primary};
      color: ${l.colors.text.secondary};
      font-size: ${l.typography.h6.fontSize};
      width: 100%;

      thead {
        color: #538ade;
        font-size: ${l.typography.bodySmall.fontSize};
      }

      th,
      td {
        padding: 6px 13px;
        height: ${l.spacing(4)};
      }

      tbody > tr:nth-child(odd) {
        background: ${l.colors.background.secondary};
      }
    `,noteTextbox:s.css`
      margin-bottom: ${l.spacing(4)};
    `,textInfo:s.css`
      color: ${l.colors.text.secondary};
      font-size: ${l.typography.size.sm};
    `,dashboardSearch:s.css`
      margin-top: ${l.spacing(2)};
    `,modal:s.css`
      width: 500px;
    `,modalText:s.css`
      font-size: ${l.typography.h4.fontSize};
      color: ${l.colors.text.primary};
      margin-bottom: ${l.spacing(4)};
      padding-top: ${l.spacing(2)};
    `}}},26440:(J,I,t)=>{t.r(I),t.d(I,{default:()=>De});var s=t(9892),e=t(68404),l=t(36635),P=t(25318),b=t(49922),f=t(68183),p=t(41818),S=t(35645),D=t(30784),O=t(94270),m=t(24799),h=t(46967),n=t(31403),L=t(97379),x=t(52081),A=t(67487),i=t(72648),C=t(7804),$=t(18271),M=t(79396),R=t(70431),g=t(38849),U=t(37932),W=t(19825),K=t(50946),z=t(24693),o=t(53117),v=t(22069),T=t(63619),G=t(79658),F=t(54899),B=t(85385);const j=r=>{let a;try{a=JSON.parse(r)}catch{return"Not valid JSON"}if(a&&a.hasOwnProperty("tags"))if(Array.isArray(a.tags)){if(a.tags.some(d=>typeof d!="string"))return"tags expected array of strings"}else return"tags expected array";return!0},V=r=>{const a=/(^\d+$)|dashboards\/(\d+)/.exec(r);return a&&(a[1]||a[2])?!0:"Could not find a valid Grafana.com ID"},H=(r,a)=>B.t.validateNewDashboardName(a,r).then(()=>!0).catch(E=>{if(E.type==="EXISTING")return E.message}),Q=r=>(0,F.i)().get(`/api/dashboards/uid/${r}`).then(a=>`Dashboard named '${a?.dashboard.title}' in folder '${a?.meta.folderTitle}' has the same UID`).catch(a=>(a.isHandled=!0,!0));var re=t(40404);function te({inputs:r,label:a,description:E,folderName:d}){const y=(0,i.wW)(se);return Boolean(r?.length)?e.createElement("div",{className:y.spacer},e.createElement(m.g,{label:a,description:E},e.createElement(e.Fragment,null,r.map((c,X)=>{const w=`elements[${X}]`,Y=c.state===z.KQ.New?{...c.model,meta:{...c.model.meta,folderName:d??"General"}}:{...c.model};return e.createElement("div",{className:y.item,key:w},e.createElement(re.p,{libraryPanel:Y,onClick:()=>{}}))})))):null}function se(r){return{spacer:s.css`
      margin-bottom: ${r.spacing(2)};
    `,item:s.css`
      margin-bottom: ${r.spacing(1)};
    `}}const le=({register:r,errors:a,control:E,getValues:d,uidReset:y,inputs:c,initialFolderUid:X,onUidReset:w,onCancel:Y,onSubmit:k,watch:q})=>{const[_,Pe]=(0,e.useState)(!1),Ie=q("dataSources"),oe=q("folder");(0,e.useEffect)(()=>{_&&(a.title||a.uid)&&k(d(),{})},[a,d,_,k]);const xe=c?.libraryPanels?.filter(u=>u.state===z.KQ.New)??[],Oe=c?.libraryPanels?.filter(u=>u.state===z.KQ.Exists)??[];return e.createElement(e.Fragment,null,e.createElement(W.D,null,"Options"),e.createElement(m.g,{label:"Name",invalid:!!a.title,error:a.title&&a.title.message},e.createElement(h.I,{...r("title",{required:"Name is required",validate:async u=>await H(u,d().folder.uid)}),type:"text","data-testid":f.wl.components.ImportDashboardForm.name})),e.createElement(m.g,{label:"Folder"},e.createElement(T.g,{render:({field:{ref:u,...N}})=>e.createElement(G.Es,{...N,enableCreateNew:!0,initialFolderUid:X}),name:"folder",control:E})),e.createElement(m.g,{label:"Unique identifier (UID)",description:`The unique identifier (UID) of a dashboard can be used for uniquely identify a dashboard between multiple Grafana installs.
                The UID allows having consistent URLs for accessing dashboards so changing the title of a dashboard will not break any
                bookmarked links to that dashboard.`,invalid:!!a.uid,error:a.uid&&a.uid.message},e.createElement(e.Fragment,null,y?e.createElement(h.I,{...r("uid",{required:!0,validate:async u=>await Q(u)})}):e.createElement(h.I,{disabled:!0,...r("uid",{validate:async u=>await Q(u)}),addonAfter:!y&&e.createElement(n.zx,{onClick:w},"Change uid")}))),c.dataSources&&c.dataSources.map((u,N)=>{if(u.pluginId===v.hr.type)return null;const Z=`dataSources[${N}]`,Ce=Ie??[];return e.createElement(m.g,{label:u.label,key:Z,invalid:a.dataSources&&!!a.dataSources[N],error:a.dataSources&&a.dataSources[N]&&"A data source is required"},e.createElement(T.g,{name:Z,render:({field:{ref:Me,...Se}})=>e.createElement(o.q,{...Se,noDefault:!0,placeholder:u.info,pluginId:u.pluginId,current:Ce[N]?.uid}),control:E,rules:{required:!0}}))}),c.constants&&c.constants.map((u,N)=>{const Z=`constants[${N}]`;return e.createElement(m.g,{label:u.label,error:a.constants&&a.constants[N]&&`${u.label} needs a value`,invalid:a.constants&&!!a.constants[N],key:Z},e.createElement(h.I,{...r(Z,{required:!0}),defaultValue:u.value}))}),e.createElement(te,{inputs:xe,label:"New library panels",description:"List of new library panels that will get imported.",folderName:oe.title}),e.createElement(te,{inputs:Oe,label:"Existing library panels",description:"List of existing library panels. These panels are not affected by the import.",folderName:oe.title}),e.createElement(x.Lh,null,e.createElement(n.zx,{type:"submit","data-testid":f.wl.components.ImportDashboardForm.submit,variant:de(a),onClick:()=>{Pe(!0)}},ie(a)),e.createElement(n.zx,{type:"reset",variant:"secondary",onClick:Y},"Cancel")))};function de(r){return r&&(r.title||r.uid)?"destructive":"primary"}function ie(r){return r&&(r.title||r.uid)?"Import (Overwrite)":"Import"}const ce="dashboard_import_imported",me=r=>{const a=U.E1.getSearchObject();return{dashboard:r.importDashboard.dashboard,meta:r.importDashboard.meta,source:r.importDashboard.source,inputs:r.importDashboard.inputs,folder:a.folderUid?{uid:String(a.folderUid)}:{uid:""}}},ue={clearLoadedDashboard:K.ys,importDashboard:K.$j},pe=(0,l.connect)(me,ue);class he extends e.PureComponent{constructor(){super(...arguments),this.state={uidReset:!1},this.onSubmit=a=>{(0,p.ff)(ce),this.props.importDashboard(a)},this.onCancel=()=>{this.props.clearLoadedDashboard()},this.onUidReset=()=>{this.setState({uidReset:!0})}}render(){const{dashboard:a,inputs:E,meta:d,source:y,folder:c}=this.props,{uidReset:X}=this.state;return e.createElement(e.Fragment,null,y===z.G7.Gcom&&e.createElement("div",{style:{marginBottom:"24px"}},e.createElement("div",null,e.createElement(W.D,null,"Importing dashboard from"," ",e.createElement("a",{href:`https://grafana.com/dashboards/${a.gnetId}`,className:"external-link",target:"_blank",rel:"noreferrer"},"Grafana.com"))),e.createElement("table",{className:"filter-table form-inline"},e.createElement("tbody",null,e.createElement("tr",null,e.createElement("td",null,"Published by"),e.createElement("td",null,d.orgName)),e.createElement("tr",null,e.createElement("td",null,"Updated on"),e.createElement("td",null,(0,g.dq)(d.updatedAt)))))),e.createElement(O.l,{onSubmit:this.onSubmit,defaultValues:{...a,constants:[],dataSources:[],elements:[],folder:c},validateOnMount:!0,validateFieldsOnMount:["title","uid"],validateOn:"onChange"},({register:w,errors:Y,control:k,watch:q,getValues:_})=>e.createElement(le,{register:w,errors:Y,control:k,getValues:_,uidReset:X,inputs:E,onCancel:this.onCancel,onUidReset:this.onUidReset,onSubmit:this.onSubmit,watch:q,initialFolderUid:c.uid})))}}const ae=pe(he);ae.displayName="ImportDashboardOverview";const ee="dashboard_import_loaded",ge=r=>({loadingState:r.importDashboard.state}),Ee={fetchGcomDashboard:K.q_,importDashboardJson:K.nQ,cleanUpAction:R.e},fe=(0,l.connect)(ge,Ee);class ve extends e.PureComponent{constructor(a){super(a),this.fileListRenderer=(d,y)=>null,this.onFileUpload=d=>{(0,p.ff)(ee,{import_source:"json_uploaded"});try{this.props.importDashboardJson(JSON.parse(String(d)))}catch(y){y instanceof Error&&$.Z.emit(P.SI.alertError,["Import failed","JSON -> JS Serialization failed: "+y.message]);return}},this.getDashboardFromJson=d=>{(0,p.ff)(ee,{import_source:"json_pasted"}),this.props.importDashboardJson(JSON.parse(d.dashboardJson))},this.getGcomDashboard=d=>{(0,p.ff)(ee,{import_source:"gcom"});let y;const c=/(^\d+$)|dashboards\/(\d+)/.exec(d.gcomDashboard);c&&c[1]?y=c[1]:c&&c[2]&&(y=c[2]),y&&this.props.fetchGcomDashboard(y)},this.pageNav={text:"Import dashboard",subTitle:"Import dashboard from file or Grafana.com",breadcrumbs:[{title:"Dashboards",url:"dashboards"}]};const{gcomDashboardId:E}=this.props.queryParams;if(E){this.getGcomDashboard({gcomDashboard:E});return}}componentWillUnmount(){this.props.cleanUpAction({cleanupAction:a=>a.importDashboard=z.wg})}renderImportForm(){const a=ye(this.props.theme);return e.createElement(e.Fragment,null,e.createElement("div",{className:a.option},e.createElement(D.Yo,{options:{multiple:!1,accept:[".json",".txt"]},readAs:"readAsText",fileListRenderer:this.fileListRenderer,onLoad:this.onFileUpload},e.createElement(D.A_,{primaryText:"Upload dashboard JSON file",secondaryText:"Drag and drop here or click to browse"}))),e.createElement("div",{className:a.option},e.createElement(O.l,{onSubmit:this.getGcomDashboard,defaultValues:{gcomDashboard:""}},({register:E,errors:d})=>e.createElement(m.g,{label:"Import via grafana.com",invalid:!!d.gcomDashboard,error:d.gcomDashboard&&d.gcomDashboard.message},e.createElement(h.I,{id:"url-input",placeholder:"Grafana.com dashboard URL or ID",type:"text",...E("gcomDashboard",{required:"A Grafana dashboard URL or ID is required",validate:V}),addonAfter:e.createElement(n.zx,{type:"submit"},"Load")})))),e.createElement("div",{className:a.option},e.createElement(O.l,{onSubmit:this.getDashboardFromJson,defaultValues:{dashboardJson:""}},({register:E,errors:d})=>e.createElement(e.Fragment,null,e.createElement(m.g,{label:"Import via panel json",invalid:!!d.dashboardJson,error:d.dashboardJson&&d.dashboardJson.message},e.createElement(L.K,{...E("dashboardJson",{required:"Need a dashboard JSON model",validate:j}),"data-testid":f.wl.components.DashboardImportPage.textarea,id:"dashboard-json-textarea",rows:10})),e.createElement(x.Lh,null,e.createElement(n.zx,{type:"submit","data-testid":f.wl.components.DashboardImportPage.submit},"Load"),e.createElement(n.Qj,{variant:"secondary",href:`${S.v.appSubUrl}/dashboards`},"Cancel"))))))}render(){const{loadingState:a}=this.props;return e.createElement(M.T,{navId:"dashboards/browse",pageNav:this.pageNav},e.createElement(M.T.Contents,null,a===b.Gu.Loading&&e.createElement(x.wc,{justify:"center"},e.createElement(x.Lh,{justify:"center"},e.createElement(A.$,{size:32}))),[b.Gu.Error,b.Gu.NotStarted].includes(a)&&this.renderImportForm(),a===b.Gu.Done&&e.createElement(ae,null)))}}const be=(0,i.HE)(ve),ne=fe(be);ne.displayName="DashboardImport";const De=ne,ye=(0,C.B)(r=>({option:s.css`
      margin-bottom: ${r.spacing(4)};
      max-width: 600px;
    `}))},89710:(J,I,t)=>{t.d(I,{X:()=>O});var s=t(9892),e=t(68404),l=t(24699),P=t(81850),b=t(68183),f=t(72648),p=t(8180),S=t(51613),D=t(81731);const O=({isCurrent:n,title:L,plugin:x,onClick:A,onDelete:i,disabled:C,showBadge:$,description:M,children:R})=>{const g=(0,f.wW)(m),U=C||x.state===l.BV.deprecated,W=(0,s.cx)({[g.item]:!0,[g.itemDisabled]:U,[g.current]:n});return e.createElement("div",{className:W,"aria-label":b.wl.components.PluginVisualization.item(x.name),onClick:U?void 0:A,title:n?"Click again to close this section":x.name},e.createElement("img",{className:(0,s.cx)(g.img,{[g.disabled]:U}),src:x.info.logos.small,alt:""}),e.createElement("div",{className:(0,s.cx)(g.itemContent,{[g.disabled]:U})},e.createElement("div",{className:g.name},L),M?e.createElement("span",{className:g.description},M):null,R),$&&e.createElement("div",{className:(0,s.cx)(g.badge,{[g.disabled]:U})},e.createElement(h,{plugin:x})),i&&e.createElement(p.h,{name:"trash-alt",onClick:K=>{K.stopPropagation(),i()},className:g.deleteButton,"aria-label":"Delete button on panel type card"}))};O.displayName="PanelTypeCard";const m=n=>({item:s.css`
      position: relative;
      display: flex;
      flex-shrink: 0;
      cursor: pointer;
      background: ${n.colors.background.secondary};
      border-radius: ${n.shape.borderRadius()};
      box-shadow: ${n.shadows.z1};
      border: 1px solid ${n.colors.background.secondary};
      align-items: center;
      padding: 8px;
      width: 100%;
      position: relative;
      overflow: hidden;
      transition: ${n.transitions.create(["background"],{duration:n.transitions.duration.short})};

      &:hover {
        background: ${n.colors.emphasize(n.colors.background.secondary,.03)};
      }
    `,itemContent:s.css`
      overflow: hidden;
      position: relative;
      padding: ${n.spacing(0,1)};
    `,itemDisabled:s.css`
      cursor: default;

      &,
      &:hover {
        background: ${n.colors.action.disabledBackground};
      }
    `,current:s.css`
      label: currentVisualizationItem;
      border: 1px solid ${n.colors.primary.border};
      background: ${n.colors.action.selected};
    `,disabled:s.css`
      opacity: ${n.colors.action.disabledOpacity};
      filter: grayscale(1);
      cursor: default;
      pointer-events: none;
    `,name:s.css`
      text-overflow: ellipsis;
      overflow: hidden;
      font-size: ${n.typography.size.sm};
      font-weight: ${n.typography.fontWeightMedium};
      width: 100%;
    `,description:s.css`
      display: block;
      text-overflow: ellipsis;
      overflow: hidden;
      color: ${n.colors.text.secondary};
      font-size: ${n.typography.bodySmall.fontSize};
      font-weight: ${n.typography.fontWeightLight};
      width: 100%;
      max-height: 4.5em;
    `,img:s.css`
      max-height: 38px;
      width: 38px;
      display: flex;
      align-items: center;
    `,badge:s.css`
      background: ${n.colors.background.primary};
    `,deleteButton:s.css`
      cursor: pointer;
      margin-left: auto;
    `}),h=({plugin:n})=>(0,P.x)(n.signature)?e.createElement(S.o,{status:n.signature}):e.createElement(D.u,{state:n.state});h.displayName="PanelPluginBadge"},81731:(J,I,t)=>{t.d(I,{u:()=>P});var s=t(68404),e=t(24699),l=t(19985);const P=f=>{const p=b(f.state);return p?s.createElement(l.C,{color:p.color,title:p.tooltip,text:p.text,icon:p.icon}):null};function b(f){switch(f){case e.BV.deprecated:return{text:"Deprecated",color:"red",tooltip:"This feature is deprecated and will be removed in a future release"};case e.BV.alpha:return{text:"Alpha",color:"blue",tooltip:"This feature is experimental and future updates might not be backward compatible"};case e.BV.beta:return{text:"Beta",color:"blue",tooltip:"This feature is close to complete but not fully tested"};default:return null}}}}]);

//# sourceMappingURL=DashboardImport.bb0188ab4bc0d4c05107.js.map
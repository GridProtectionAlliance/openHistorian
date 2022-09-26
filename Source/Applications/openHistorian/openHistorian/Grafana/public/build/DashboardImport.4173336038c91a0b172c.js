"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[8808],{71585:(e,t,a)=>{a.d(t,{p:()=>N});var r=a(36636),s=a(68404),n=a(90923),o=a(3490),i=a(80672),l=a(87092),d=a(43215),c=a(34751),u=a(74669),h=a(23078),p=a(35140);const m={loadingState:d.LoadingState.Loading,dashboardTitles:[]},g=(0,p.PH)("libraryPanels/delete/searchCompleted"),b=function(){let e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:m,t=arguments.length>1?arguments[1]:void 0;return g.match(t)?Object.assign({},e,{dashboardTitles:t.payload.dashboards.map((e=>e.title)),loadingState:d.LoadingState.Done}):e};var x,f,j,y,v=a(45916);const S=e=>{let{libraryPanel:t,onDismiss:a,onConfirm:r}=e;const n=(0,o.useStyles)(c.J),[{dashboardTitles:i,loadingState:l},p]=(0,s.useReducer)(b,m),j=(0,s.useMemo)((()=>(0,u.tb)(p)),[p]);(0,s.useEffect)((()=>{j(function(e){return async function(t){const a=await(0,h.E8)(e.uid);t(g({dashboards:a}))}}(t))}),[j,t]);const y=Boolean(i.length),S=l===d.LoadingState.Done;return(0,v.jsxs)(o.Modal,{className:n.modal,title:"Delete library panel",icon:"trash-alt",onDismiss:a,isOpen:!0,children:[S?null:x||(x=(0,v.jsx)(D,{})),S?(0,v.jsxs)("div",{children:[y?(0,v.jsx)(P,{dashboardTitles:i}):null,y?null:f||(f=(0,v.jsx)(w,{})),(0,v.jsxs)(o.Modal.ButtonRow,{children:[(0,v.jsx)(o.Button,{variant:"secondary",onClick:a,fill:"outline",children:"Cancel"}),(0,v.jsx)(o.Button,{variant:"destructive",onClick:r,disabled:y,children:"Delete"})]})]}):null]})},D=()=>j||(j=(0,v.jsx)("span",{children:"Loading library panel..."})),w=()=>{const e=(0,o.useStyles)(c.J);return(0,v.jsx)("div",{className:e.modalText,children:"Do you want to delete this panel?"})},P=e=>{let{dashboardTitles:t}=e;const a=(0,o.useStyles)(c.J),r=1===t.length?"dashboard.":"dashboards.",s=`${t.length} ${r}`;return 0===t.length?null:(0,v.jsxs)("div",{children:[(0,v.jsxs)("p",{className:a.textInfo,children:["This library panel can not be deleted because it is connected to ",(0,v.jsx)("strong",{children:s})," Remove the library panel from the dashboards listed below and retry."]}),(0,v.jsxs)("table",{className:a.myTable,children:[y||(y=(0,v.jsx)("thead",{children:(0,v.jsx)("tr",{children:(0,v.jsx)("th",{children:"Dashboard name"})})})),(0,v.jsx)("tbody",{children:t.map(((e,t)=>(0,v.jsx)("tr",{children:(0,v.jsx)("td",{children:e})},`dash-title-${t}`)))})]})]})};var $,I;const N=e=>{var t;let{libraryPanel:a,onClick:r,onDelete:o,showSecondaryActions:d}=e;const[c,u]=(0,s.useState)(!1),h=null!==(t=n.config.panels[a.model.type])&&void 0!==t?t:(0,i.X)(a.model.type).meta;return(0,v.jsxs)(v.Fragment,{children:[(0,v.jsx)(l.X,{isCurrent:!1,title:a.name,description:a.description,plugin:h,onClick:()=>null==r?void 0:r(a),onDelete:d?()=>u(!0):void 0,children:(0,v.jsx)(C,{libraryPanel:a})}),c&&(0,v.jsx)(S,{libraryPanel:a,onConfirm:()=>{null==o||o(a),u(!1)},onDismiss:()=>u(!1)})]})};function C(e){let{libraryPanel:t}=e;const a=(0,o.useStyles2)(k);return t.meta.folderUid||t.meta.folderName?t.meta.folderUid?(0,v.jsx)("span",{className:a.metaContainer,children:(0,v.jsxs)(o.Link,{href:`/dashboards/f/${t.meta.folderUid}`,children:[I||(I=(0,v.jsx)(o.Icon,{name:"folder-upload",size:"sm"})),(0,v.jsx)("span",{children:t.meta.folderName})]})}):(0,v.jsxs)("span",{className:a.metaContainer,children:[$||($=(0,v.jsx)(o.Icon,{name:"folder",size:"sm"})),(0,v.jsx)("span",{children:t.meta.folderName})]}):null}function k(e){return{metaContainer:r.css`
      display: flex;
      align-items: center;
      color: ${e.colors.text.secondary};
      font-size: ${e.typography.bodySmall.fontSize};
      padding-top: ${e.spacing(.5)};

      svg {
        margin-right: ${e.spacing(.5)};
        margin-bottom: 3px;
      }
    `}}},74669:(e,t,a)=>{a.d(t,{UO:()=>x,Xu:()=>b,tb:()=>f});var r=a(4203),s=a(94396),n=a(71808),o=a(3321),i=a(46089),l=a(71114),d=a(14444),c=a(24298),u=a(2027),h=a(48099),p=a(40818),m=a(23078),g=a(4401);function b(e){return function(t){const a=new r.w0,b=(0,s.D)((0,m.Pq)({searchString:e.searchString,perPage:e.perPage,page:e.page,excludeUid:e.currentPanelId,sortDirection:e.sortDirection,typeFilter:e.panelFilter,folderFilter:e.folderFilter})).pipe((0,l.z)((e=>{let{perPage:t,elements:a,page:r,totalCount:s}=e;return(0,n.of)((0,g.zK)({libraryPanels:a,page:r,perPage:t,totalCount:s}))})),(0,d.K)((t=>(console.error(t),(0,n.of)((0,g.zK)(Object.assign({},g.p$,{page:e.page,perPage:e.perPage})))))),(0,c.x)((()=>a.unsubscribe())),(0,u.B)());a.add((0,o.T)((0,i.H)(50).pipe((0,h.h)((0,g.xU)()),(0,p.R)(b)),b).subscribe(t))}}function x(e,t){return async function(a){try{await(0,m.UO)(e),b(t)(a)}catch(e){console.error(e)}}}function f(e){return function(t){return t instanceof Function?t(e):e(t)}}},4401:(e,t,a)=>{a.d(t,{_p:()=>d,oO:()=>l,p$:()=>n,xU:()=>o,zK:()=>i});var r=a(35140),s=a(43215);const n={loadingState:s.LoadingState.Loading,libraryPanels:[],totalCount:0,perPage:40,page:1,numberOfPages:0,currentPanelId:void 0},o=(0,r.PH)("libraryPanels/view/initSearch"),i=(0,r.PH)("libraryPanels/view/searchCompleted"),l=(0,r.PH)("libraryPanels/view/changePage"),d=(e,t)=>{if(o.match(t))return Object.assign({},e,{loadingState:s.LoadingState.Loading});if(i.match(t)){const{libraryPanels:a,page:r,perPage:n,totalCount:o}=t.payload,i=Math.ceil(o/n);return Object.assign({},e,{libraryPanels:a,perPage:n,totalCount:o,loadingState:s.LoadingState.Done,numberOfPages:i,page:r>i?r-1:r})}return l.match(t)?Object.assign({},e,{page:t.payload.page}):e}},34751:(e,t,a)=>{a.d(t,{J:()=>s});var r=a(36636);function s(e){return{myTable:r.css`
      max-height: 204px;
      overflow-y: auto;
      margin-top: 11px;
      margin-bottom: 28px;
      border-radius: ${e.border.radius.sm};
      border: 1px solid ${e.colors.bg3};
      background: ${e.colors.bg1};
      color: ${e.colors.textSemiWeak};
      font-size: ${e.typography.size.md};
      width: 100%;

      thead {
        color: #538ade;
        font-size: ${e.typography.size.sm};
      }

      th,
      td {
        padding: 6px 13px;
        height: ${e.spacing.xl};
      }

      tbody > tr:nth-child(odd) {
        background: ${e.colors.bg2};
      }
    `,noteTextbox:r.css`
      margin-bottom: ${e.spacing.xl};
    `,textInfo:r.css`
      color: ${e.colors.textSemiWeak};
      font-size: ${e.typography.size.sm};
    `,dashboardSearch:r.css`
      margin-top: ${e.spacing.md};
    `,modal:r.css`
      width: 500px;
    `,modalText:r.css`
      font-size: ${e.typography.heading.h4};
      color: ${e.colors.link};
      margin-bottom: calc(${e.spacing.d} * 2);
      padding-top: ${e.spacing.d};
    `}}},54948:(e,t,a)=>{a.r(t),a.d(t,{default:()=>M});var r=a(36636),s=a(68404),n=a(18745),o=a(43215),i=a(16695),l=a(90923),d=a(3490),c=a(5831),u=a(69371),h=a(45193),p=a(85189),m=a(77239),g=a(20467),b=a(95513),x=a(42235);const f=e=>{try{return JSON.parse(e),!0}catch(e){return"Not valid JSON"}},j=e=>{const t=/(^\d+$)|dashboards\/(\d+)/.exec(e);return!(!t||!t[1]&&!t[2])||"Could not find a valid Grafana.com ID"},y=e=>(0,l.getBackendSrv)().get(`/api/dashboards/uid/${e}`).then((e=>`Dashboard named '${null==e?void 0:e.dashboard.title}' in folder '${null==e?void 0:e.meta.folderTitle}' has the same UID`)).catch((e=>(e.isHandled=!0,!0)));var v=a(71585),S=a(45916);function D(e){let{inputs:t,label:a,description:r,folderName:s}=e;const n=(0,d.useStyles2)(w);return Boolean(null==t?void 0:t.length)?(0,S.jsx)("div",{className:n.spacer,children:(0,S.jsx)(d.Field,{label:a,description:r,children:(0,S.jsx)(S.Fragment,{children:t.map(((e,t)=>{const a=`elements[${t}]`,r=e.state===m.KQ.New?Object.assign({},e.model,{meta:Object.assign({},e.model.meta,{folderName:null!=s?s:"General"})}):Object.assign({},e.model);return(0,S.jsx)("div",{className:n.item,children:(0,S.jsx)(v.p,{libraryPanel:r,onClick:()=>{}})},a)}))})})}):null}function w(e){return{spacer:r.css`
      margin-bottom: ${e.spacing(2)};
    `,item:r.css`
      margin-bottom: ${e.spacing(1)};
    `}}const P=["ref"],$=["ref"];var I;function N(e,t){if(null==e)return{};var a,r,s={},n=Object.keys(e);for(r=0;r<n.length;r++)a=n[r],t.indexOf(a)>=0||(s[a]=e[a]);return s}const C=e=>{var t,a,r,n;let{register:o,errors:c,control:u,getValues:h,uidReset:p,inputs:f,initialFolderId:j,onUidReset:v,onCancel:w,onSubmit:C,watch:O}=e;const[T,U]=(0,s.useState)(!1),L=O("dataSources"),z=O("folder");(0,s.useEffect)((()=>{T&&(c.title||c.uid)&&C(h(),{})}),[c,h,T,C]);const J=null!==(t=null==f||null===(a=f.libraryPanels)||void 0===a?void 0:a.filter((e=>e.state===m.KQ.New)))&&void 0!==t?t:[],B=null!==(r=null==f||null===(n=f.libraryPanels)||void 0===n?void 0:n.filter((e=>e.state===m.KQ.Exists)))&&void 0!==r?r:[];return(0,S.jsxs)(S.Fragment,{children:[I||(I=(0,S.jsx)(d.Legend,{children:"Options"})),(0,S.jsx)(d.Field,{label:"Name",invalid:!!c.title,error:c.title&&c.title.message,children:(0,S.jsx)(d.Input,Object.assign({},o("title",{required:"Name is required",validate:async e=>{return await(t=e,a=h().folder.id,x.t.validateNewDashboardName(a,t).then((()=>!0)).catch((e=>{if("EXISTING"===e.type)return e.message})));var t,a}}),{type:"text","data-testid":i.wl.components.ImportDashboardForm.name}))}),(0,S.jsx)(d.Field,{label:"Folder",children:(0,S.jsx)(d.InputControl,{render:e=>{let{}=e,t=N(e.field,P);return(0,S.jsx)(b.E,Object.assign({},t,{enableCreateNew:!0,initialFolderId:j}))},name:"folder",control:u})}),(0,S.jsx)(d.Field,{label:"Unique identifier (UID)",description:"The unique identifier (UID) of a dashboard can be used for uniquely identify a dashboard between multiple Grafana installs. The UID allows having consistent URLs for accessing dashboards so changing the title of a dashboard will not break any bookmarked links to that dashboard.",invalid:!!c.uid,error:c.uid&&c.uid.message,children:(0,S.jsx)(S.Fragment,{children:p?(0,S.jsx)(d.Input,Object.assign({},o("uid",{required:!0,validate:async e=>await y(e)}))):(0,S.jsx)(d.Input,Object.assign({disabled:!0},o("uid",{validate:async e=>await y(e)}),{addonAfter:!p&&(0,S.jsx)(d.Button,{onClick:v,children:"Change uid"})}))})}),f.dataSources&&f.dataSources.map(((e,t)=>{if(e.pluginId===g.hr.type)return null;const a=`dataSources[${t}]`,r=null!=L?L:[];return(0,S.jsx)(d.Field,{label:e.label,invalid:c.dataSources&&!!c.dataSources[t],error:c.dataSources&&c.dataSources[t]&&"A data source is required",children:(0,S.jsx)(d.InputControl,{name:a,render:a=>{var s;let{}=a,n=N(a.field,$);return(0,S.jsx)(l.DataSourcePicker,Object.assign({},n,{noDefault:!0,placeholder:e.info,pluginId:e.pluginId,current:null===(s=r[t])||void 0===s?void 0:s.uid}))},control:u,rules:{required:!0}})},a)})),f.constants&&f.constants.map(((e,t)=>{const a=`constants[${t}]`;return(0,S.jsx)(d.Field,{label:e.label,error:c.constants&&c.constants[t]&&`${e.label} needs a value`,invalid:c.constants&&!!c.constants[t],children:(0,S.jsx)(d.Input,Object.assign({},o(a,{required:!0}),{defaultValue:e.value}))},a)})),(0,S.jsx)(D,{inputs:J,label:"New library panels",description:"List of new library panels that will get imported.",folderName:z.title}),(0,S.jsx)(D,{inputs:B,label:"Existing library panels",description:"List of existing library panels. These panels are not affected by the import.",folderName:z.title}),(0,S.jsxs)(d.HorizontalGroup,{children:[(0,S.jsx)(d.Button,{type:"submit","data-testid":i.wl.components.ImportDashboardForm.submit,variant:k(c),onClick:()=>{U(!0)},children:F(c)}),(0,S.jsx)(d.Button,{type:"reset",variant:"secondary",onClick:w,children:"Cancel"})]})]})};function k(e){return e&&(e.title||e.uid)?"destructive":"primary"}function F(e){return e&&(e.title||e.uid)?"Import (Overwrite)":"Import"}var O,T;function U(e,t,a){return t in e?Object.defineProperty(e,t,{value:a,enumerable:!0,configurable:!0,writable:!0}):e[t]=a,e}const L={clearLoadedDashboard:p.ys,importDashboard:p.$j},z=(0,n.connect)((e=>{const t=l.locationService.getSearchObject();return{dashboard:e.importDashboard.dashboard,meta:e.importDashboard.meta,source:e.importDashboard.source,inputs:e.importDashboard.inputs,folder:t.folderId?{id:Number(t.folderId)}:{id:0}}}),L);class J extends s.PureComponent{constructor(){super(...arguments),U(this,"state",{uidReset:!1}),U(this,"onSubmit",(e=>{(0,l.reportInteraction)("dashboard_import_imported"),this.props.importDashboard(e)})),U(this,"onCancel",(()=>{this.props.clearLoadedDashboard()})),U(this,"onUidReset",(()=>{this.setState({uidReset:!0})}))}render(){const{dashboard:e,inputs:t,meta:a,source:r,folder:s}=this.props,{uidReset:n}=this.state;return(0,S.jsxs)(S.Fragment,{children:[r===m.G7.Gcom&&(0,S.jsxs)("div",{style:{marginBottom:"24px"},children:[(0,S.jsx)("div",{children:(0,S.jsxs)(d.Legend,{children:["Importing dashboard from"," ",(0,S.jsx)("a",{href:`https://grafana.com/dashboards/${e.gnetId}`,className:"external-link",target:"_blank",rel:"noreferrer",children:"Grafana.com"})]})}),(0,S.jsx)("table",{className:"filter-table form-inline",children:(0,S.jsxs)("tbody",{children:[(0,S.jsxs)("tr",{children:[O||(O=(0,S.jsx)("td",{children:"Published by"})),(0,S.jsx)("td",{children:a.orgName})]}),(0,S.jsxs)("tr",{children:[T||(T=(0,S.jsx)("td",{children:"Updated on"})),(0,S.jsx)("td",{children:(0,o.dateTimeFormat)(a.updatedAt)})]})]})})]}),(0,S.jsx)(d.Form,{onSubmit:this.onSubmit,defaultValues:Object.assign({},e,{constants:[],dataSources:[],elements:[],folder:s}),validateOnMount:!0,validateFieldsOnMount:["title","uid"],validateOn:"onChange",children:e=>{let{register:a,errors:r,control:o,watch:i,getValues:l}=e;return(0,S.jsx)(C,{register:a,errors:r,control:o,getValues:l,uidReset:n,inputs:t,onCancel:this.onCancel,onUidReset:this.onUidReset,onSubmit:this.onSubmit,watch:i,initialFolderId:s.id})}})]})}}const B=z(J);var R,G,q;function A(e,t,a){return t in e?Object.defineProperty(e,t,{value:a,enumerable:!0,configurable:!0,writable:!0}):e[t]=a,e}B.displayName="ImportDashboardOverview";const _="dashboard_import_loaded",E={fetchGcomDashboard:p.q_,importDashboardJson:p.nQ,cleanUpAction:h.e},V=(0,n.connect)((e=>({loadingState:e.importDashboard.state})),E);class H extends s.PureComponent{constructor(e){super(e),A(this,"onFileUpload",(e=>{(0,l.reportInteraction)(_,{import_source:"json_uploaded"});const{importDashboardJson:t}=this.props,a=e.currentTarget.files&&e.currentTarget.files.length>0&&e.currentTarget.files[0];if(a){const e=new FileReader,r=()=>e=>{let a;try{a=JSON.parse(e.target.result)}catch(e){return void(e instanceof Error&&c.Z.emit(o.AppEvents.alertError,["Import failed","JSON -> JS Serialization failed: "+e.message]))}t(a)};e.onload=r(),e.readAsText(a)}})),A(this,"getDashboardFromJson",(e=>{(0,l.reportInteraction)(_,{import_source:"json_pasted"}),this.props.importDashboardJson(JSON.parse(e.dashboardJson))})),A(this,"getGcomDashboard",(e=>{let t;(0,l.reportInteraction)(_,{import_source:"gcom"});const a=/(^\d+$)|dashboards\/(\d+)/.exec(e.gcomDashboard);a&&a[1]?t=a[1]:a&&a[2]&&(t=a[2]),t&&this.props.fetchGcomDashboard(t)}));const{gcomDashboardId:t}=this.props.queryParams;t&&this.getGcomDashboard({gcomDashboard:t})}componentWillUnmount(){this.props.cleanUpAction({stateSelector:e=>e.importDashboard})}renderImportForm(){const e=W(this.props.theme);return(0,S.jsxs)(S.Fragment,{children:[(0,S.jsx)("div",{className:e.option,children:(0,S.jsx)(d.FileUpload,{accept:"application/json",onFileUpload:this.onFileUpload,children:"Upload JSON file"})}),(0,S.jsx)("div",{className:e.option,children:(0,S.jsx)(d.Form,{onSubmit:this.getGcomDashboard,defaultValues:{gcomDashboard:""},children:e=>{let{register:t,errors:a}=e;return(0,S.jsx)(d.Field,{label:"Import via grafana.com",invalid:!!a.gcomDashboard,error:a.gcomDashboard&&a.gcomDashboard.message,children:(0,S.jsx)(d.Input,Object.assign({id:"url-input",placeholder:"Grafana.com dashboard URL or ID",type:"text"},t("gcomDashboard",{required:"A Grafana dashboard URL or ID is required",validate:j}),{addonAfter:R||(R=(0,S.jsx)(d.Button,{type:"submit",children:"Load"}))}))})}})}),(0,S.jsx)("div",{className:e.option,children:(0,S.jsx)(d.Form,{onSubmit:this.getDashboardFromJson,defaultValues:{dashboardJson:""},children:e=>{let{register:t,errors:a}=e;return(0,S.jsxs)(S.Fragment,{children:[(0,S.jsx)(d.Field,{label:"Import via panel json",invalid:!!a.dashboardJson,error:a.dashboardJson&&a.dashboardJson.message,children:(0,S.jsx)(d.TextArea,Object.assign({},t("dashboardJson",{required:"Need a dashboard JSON model",validate:f}),{"data-testid":i.wl.components.DashboardImportPage.textarea,id:"dashboard-json-textarea",rows:10}))}),(0,S.jsx)(d.Button,{type:"submit","data-testid":i.wl.components.DashboardImportPage.submit,children:"Load"})]})}})})]})}render(){const{loadingState:e}=this.props;return(0,S.jsx)(u.T,{navId:"dashboards/import",children:(0,S.jsxs)(u.T.Contents,{children:[e===o.LoadingState.Loading&&(G||(G=(0,S.jsx)(d.VerticalGroup,{justify:"center",children:(0,S.jsx)(d.HorizontalGroup,{justify:"center",children:(0,S.jsx)(d.Spinner,{size:32})})}))),[o.LoadingState.Error,o.LoadingState.NotStarted].includes(e)&&this.renderImportForm(),e===o.LoadingState.Done&&(q||(q=(0,S.jsx)(B,{})))]})})}}const K=V((0,d.withTheme2)(H));K.displayName="DashboardImport";const M=K,W=(0,d.stylesFactory)((e=>({option:r.css`
      margin-bottom: ${e.spacing(4)};
    `})))},87092:(e,t,a)=>{a.d(t,{X:()=>d});var r=a(36636),s=(a(68404),a(43215)),n=a(16695),o=a(3490),i=a(30978),l=a(45916);const d=e=>{let{isCurrent:t,title:a,plugin:i,onClick:d,onDelete:h,disabled:p,showBadge:m,description:g,children:b}=e;const x=(0,o.useStyles2)(c),f=(0,r.cx)({[x.item]:!0,[x.disabled]:p||i.state===s.PluginState.deprecated,[x.current]:t});return(0,l.jsxs)("div",{className:f,"aria-label":n.wl.components.PluginVisualization.item(i.name),onClick:p?void 0:d,title:t?"Click again to close this section":i.name,children:[(0,l.jsx)("img",{className:x.img,src:i.info.logos.small,alt:""}),(0,l.jsxs)("div",{className:x.itemContent,children:[(0,l.jsx)("div",{className:x.name,children:a}),g?(0,l.jsx)("span",{className:x.description,children:g}):null,b]}),m&&(0,l.jsx)("div",{className:(0,r.cx)(x.badge,p&&x.disabled),children:(0,l.jsx)(u,{plugin:i})}),h&&(0,l.jsx)(o.IconButton,{name:"trash-alt",onClick:e=>{e.stopPropagation(),h()},className:x.deleteButton,"aria-label":"Delete button on panel type card"})]})};d.displayName="PanelTypeCard";const c=e=>({item:r.css`
      position: relative;
      display: flex;
      flex-shrink: 0;
      cursor: pointer;
      background: ${e.colors.background.secondary};
      border-radius: ${e.shape.borderRadius()};
      box-shadow: ${e.shadows.z1};
      border: 1px solid ${e.colors.background.secondary};
      align-items: center;
      padding: 8px;
      width: 100%;
      position: relative;
      overflow: hidden;
      transition: ${e.transitions.create(["background"],{duration:e.transitions.duration.short})};

      &:hover {
        background: ${e.colors.emphasize(e.colors.background.secondary,.03)};
      }
    `,itemContent:r.css`
      overflow: hidden;
      position: relative;
      padding: ${e.spacing(0,1)};
    `,current:r.css`
      label: currentVisualizationItem;
      border: 1px solid ${e.colors.primary.border};
      background: ${e.colors.action.selected};
    `,disabled:r.css`
      opacity: 0.2;
      filter: grayscale(1);
      cursor: default;
      pointer-events: none;
    `,name:r.css`
      text-overflow: ellipsis;
      overflow: hidden;
      font-size: ${e.typography.size.sm};
      font-weight: ${e.typography.fontWeightMedium};
      width: 100%;
    `,description:r.css`
      display: block;
      text-overflow: ellipsis;
      overflow: hidden;
      color: ${e.colors.text.secondary};
      font-size: ${e.typography.bodySmall.fontSize};
      font-weight: ${e.typography.fontWeightLight};
      width: 100%;
      max-height: 4.5em;
    `,img:r.css`
      max-height: 38px;
      width: 38px;
      display: flex;
      align-items: center;
    `,badge:r.css`
      background: ${e.colors.background.primary};
    `,deleteButton:r.css`
      margin-left: auto;
    `}),u=e=>{let{plugin:t}=e;return(0,s.isUnsignedPluginSignature)(t.signature)?(0,l.jsx)(o.PluginSignatureBadge,{status:t.signature}):(0,l.jsx)(i.u,{state:t.state})};u.displayName="PanelPluginBadge"},30978:(e,t,a)=>{a.d(t,{u:()=>o});a(68404);var r=a(43215),s=a(3490),n=a(45916);const o=e=>{const t=function(e){switch(e){case r.PluginState.deprecated:return{text:"Deprecated",color:"red",tooltip:"This feature is deprecated and will be removed in a future release"};case r.PluginState.alpha:return{text:"Alpha",color:"blue",tooltip:"This feature is experimental and future updates might not be backward compatible"};case r.PluginState.beta:return{text:"Beta",color:"blue",tooltip:"This feature is close to complete but not fully tested"};default:return null}}(e.state);return t?(0,n.jsx)(s.Badge,{color:t.color,title:t.tooltip,text:t.text,icon:t.icon}):null}}}]);
//# sourceMappingURL=DashboardImport.4173336038c91a0b172c.js.map
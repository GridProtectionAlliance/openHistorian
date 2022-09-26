"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[9586],{62431:(e,a,t)=>{t.d(a,{j:()=>c});var n,s=t(36636),i=t(68404),r=t(3490),l=t(7874),o=t(45916);const c=e=>{let{onChange:a,maxMenuHeight:t}=e;const s=(0,i.useMemo)((()=>(0,l.x)()),[]),c=(0,i.useMemo)((()=>s.map((e=>({label:e.name,imgUrl:e.info.logos.small,value:e}))).sort(((e,a)=>{var t;return null===(t=e.label)||void 0===t?void 0:t.localeCompare(a.label)}))),[s]),[u,p]=(0,i.useState)([]),g=(0,i.useCallback)((e=>{const t=[];for(const a of e)a.value&&t.push(a.value);a(t),p(e)}),[a]),h=(0,r.useStyles2)(d),m={defaultOptions:!0,getOptionLabel:e=>e.label,getOptionValue:e=>e.value,noOptionsMessage:"No Panel types found",placeholder:"Filter by type",maxMenuHeight:t,options:c,value:u,onChange:g};return(0,o.jsxs)("div",{className:h.container,children:[u.length>0&&(0,o.jsx)(r.Button,{size:"xs",icon:"trash-alt",fill:"text",className:h.clear,onClick:()=>g([]),"aria-label":"Clear types",children:"Clear types"}),(0,o.jsx)(r.MultiSelect,Object.assign({},m,{prefix:n||(n=(0,o.jsx)(r.Icon,{name:"filter"})),"aria-label":"Panel Type filter"}))]})};function d(e){return{container:s.css`
      label: container;
      position: relative;
      min-width: 180px;
      flex-grow: 1;
    `,clear:s.css`
      label: clear;
      font-size: ${e.spacing(1.5)};
      position: absolute;
      top: -${e.spacing(4.5)};
      right: 0;
    `}}},71585:(e,a,t)=>{t.d(a,{p:()=>F});var n=t(36636),s=t(68404),i=t(90923),r=t(3490),l=t(80672),o=t(87092),c=t(43215),d=t(34751),u=t(74669),p=t(23078),g=t(35140);const h={loadingState:c.LoadingState.Loading,dashboardTitles:[]},m=(0,g.PH)("libraryPanels/delete/searchCompleted"),x=function(){let e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:h,a=arguments.length>1?arguments[1]:void 0;return m.match(a)?Object.assign({},e,{dashboardTitles:a.payload.dashboards.map((e=>e.title)),loadingState:c.LoadingState.Done}):e};var b,f,y,j,P=t(45916);const v=e=>{let{libraryPanel:a,onDismiss:t,onConfirm:n}=e;const i=(0,r.useStyles)(d.J),[{dashboardTitles:l,loadingState:o},g]=(0,s.useReducer)(x,h),y=(0,s.useMemo)((()=>(0,u.tb)(g)),[g]);(0,s.useEffect)((()=>{y(function(e){return async function(a){const t=await(0,p.E8)(e.uid);a(m({dashboards:t}))}}(a))}),[y,a]);const j=Boolean(l.length),v=o===c.LoadingState.Done;return(0,P.jsxs)(r.Modal,{className:i.modal,title:"Delete library panel",icon:"trash-alt",onDismiss:t,isOpen:!0,children:[v?null:b||(b=(0,P.jsx)(w,{})),v?(0,P.jsxs)("div",{children:[j?(0,P.jsx)(C,{dashboardTitles:l}):null,j?null:f||(f=(0,P.jsx)(S,{})),(0,P.jsxs)(r.Modal.ButtonRow,{children:[(0,P.jsx)(r.Button,{variant:"secondary",onClick:t,fill:"outline",children:"Cancel"}),(0,P.jsx)(r.Button,{variant:"destructive",onClick:n,disabled:j,children:"Delete"})]})]}):null]})},w=()=>y||(y=(0,P.jsx)("span",{children:"Loading library panel..."})),S=()=>{const e=(0,r.useStyles)(d.J);return(0,P.jsx)("div",{className:e.modalText,children:"Do you want to delete this panel?"})},C=e=>{let{dashboardTitles:a}=e;const t=(0,r.useStyles)(d.J),n=1===a.length?"dashboard.":"dashboards.",s=`${a.length} ${n}`;return 0===a.length?null:(0,P.jsxs)("div",{children:[(0,P.jsxs)("p",{className:t.textInfo,children:["This library panel can not be deleted because it is connected to ",(0,P.jsx)("strong",{children:s})," Remove the library panel from the dashboards listed below and retry."]}),(0,P.jsxs)("table",{className:t.myTable,children:[j||(j=(0,P.jsx)("thead",{children:(0,P.jsx)("tr",{children:(0,P.jsx)("th",{children:"Dashboard name"})})})),(0,P.jsx)("tbody",{children:a.map(((e,a)=>(0,P.jsx)("tr",{children:(0,P.jsx)("td",{children:e})},`dash-title-${a}`)))})]})]})};var $,k;const F=e=>{var a;let{libraryPanel:t,onClick:n,onDelete:r,showSecondaryActions:c}=e;const[d,u]=(0,s.useState)(!1),p=null!==(a=i.config.panels[t.model.type])&&void 0!==a?a:(0,l.X)(t.model.type).meta;return(0,P.jsxs)(P.Fragment,{children:[(0,P.jsx)(o.X,{isCurrent:!1,title:t.name,description:t.description,plugin:p,onClick:()=>null==n?void 0:n(t),onDelete:c?()=>u(!0):void 0,children:(0,P.jsx)(N,{libraryPanel:t})}),d&&(0,P.jsx)(v,{libraryPanel:t,onConfirm:()=>{null==r||r(t),u(!1)},onDismiss:()=>u(!1)})]})};function N(e){let{libraryPanel:a}=e;const t=(0,r.useStyles2)(O);return a.meta.folderUid||a.meta.folderName?a.meta.folderUid?(0,P.jsx)("span",{className:t.metaContainer,children:(0,P.jsxs)(r.Link,{href:`/dashboards/f/${a.meta.folderUid}`,children:[k||(k=(0,P.jsx)(r.Icon,{name:"folder-upload",size:"sm"})),(0,P.jsx)("span",{children:a.meta.folderName})]})}):(0,P.jsxs)("span",{className:t.metaContainer,children:[$||($=(0,P.jsx)(r.Icon,{name:"folder",size:"sm"})),(0,P.jsx)("span",{children:a.meta.folderName})]}):null}function O(e){return{metaContainer:n.css`
      display: flex;
      align-items: center;
      color: ${e.colors.text.secondary};
      font-size: ${e.typography.bodySmall.fontSize};
      padding-top: ${e.spacing(.5)};

      svg {
        margin-right: ${e.spacing(.5)};
        margin-bottom: 3px;
      }
    `}}},19586:(e,a,t)=>{t.d(a,{N:()=>$,e:()=>C});var n,s=t(36636),i=t(68404),r=t(3490),l=t(99151),o=t.n(l),c=t(28659),d=t(47570),u=t(45916);function p(e){let{onChange:a,maxMenuHeight:t}=e;const s=(0,r.useStyles2)(g),[l,p]=(0,i.useState)(!1),h=(0,i.useCallback)((e=>async function(e,a){a(!0);const t={query:e,type:"dash-folder",permission:d.bf.View},n=(await(0,c.i)().search(t)).map((e=>({label:e.title,value:{id:e.id,title:e.title}})));e&&!"general".includes(e.toLowerCase())||n.unshift({label:"General",value:{id:0,title:"General"}});return a(!1),n}(e,p)),[]),m=(0,i.useMemo)((()=>o()(h,300)),[h]),[x,b]=(0,i.useState)([]),f=(0,i.useCallback)((e=>{const t=[];for(const a of e)a.value&&t.push(a.value);a(t),b(e)}),[a]),y={defaultOptions:!0,isMulti:!0,noOptionsMessage:"No folders found",placeholder:"Filter by folder",maxMenuHeight:t,value:x,onChange:f};return(0,u.jsxs)("div",{className:s.container,children:[x.length>0&&(0,u.jsx)(r.Button,{size:"xs",icon:"trash-alt",fill:"text",className:s.clear,onClick:()=>f([]),"aria-label":"Clear folders",children:"Clear folders"}),(0,u.jsx)(r.AsyncMultiSelect,Object.assign({},y,{isLoading:l,loadOptions:m,prefix:n||(n=(0,u.jsx)(r.Icon,{name:"filter"})),"aria-label":"Folder filter"}))]})}function g(e){return{container:s.css`
      label: container;
      position: relative;
      min-width: 180px;
      flex-grow: 1;
    `,clear:s.css`
      label: clear;
      font-size: ${e.spacing(1.5)};
      position: absolute;
      top: -${e.spacing(4.5)};
      right: 0;
    `}}var h=t(62431),m=t(78603),x=t(65747),b=t(23195),f=t(35140);const y={searchQuery:"",panelFilter:[],folderFilter:[],sortDirection:void 0},j=(0,f.PH)("libraryPanels/search/searchChanged"),P=(0,f.PH)("libraryPanels/search/sortChanged"),v=(0,f.PH)("libraryPanels/search/panelFilterChanged"),w=(0,f.PH)("libraryPanels/search/folderFilterChanged"),S=(e,a)=>j.match(a)?Object.assign({},e,{searchQuery:a.payload}):P.match(a)?Object.assign({},e,{sortDirection:a.payload.value}):v.match(a)?Object.assign({},e,{panelFilter:a.payload.map((e=>e.id))}):w.match(a)?Object.assign({},e,{folderFilter:a.payload.map((e=>String(e.id)))}):e;let C;!function(e){e.Tight="tight",e.Spacious="spacious"}(C||(C={}));const $=e=>{let{onClick:a,variant:t=C.Spacious,currentPanelId:n,currentFolderId:s,perPage:l=x.gN,showPanelFilter:o=!1,showFolderFilter:c=!1,showSort:d=!1,showSecondaryActions:g=!1}=e;const f=(0,r.useStyles2)(k),[{sortDirection:$,panelFilter:F,folderFilter:N,searchQuery:O},z]=(0,i.useReducer)(S,Object.assign({},y,{folderFilter:s?[s.toString(10)]:[]})),D=e=>z(j(e)),L=e=>z(P(e)),I=e=>z(w(e)),M=e=>z(v(e));return t===C.Spacious?(0,u.jsx)("div",{className:f.container,children:(0,u.jsxs)(r.VerticalGroup,{spacing:"lg",children:[(0,u.jsx)(r.FilterInput,{value:O,onChange:D,placeholder:"Search by name or description",width:0}),(0,u.jsx)("div",{className:f.buttonRow,children:(0,u.jsxs)(r.HorizontalGroup,{spacing:"sm",justify:d&&o||c?"space-between":"flex-end",children:[d&&(0,u.jsx)(m.P,{value:$,onChange:L,filter:["alpha-asc","alpha-desc"]}),(0,u.jsxs)(r.HorizontalGroup,{spacing:"sm",justify:c&&o?"space-between":"flex-end",children:[c&&(0,u.jsx)(p,{onChange:I}),o&&(0,u.jsx)(h.j,{onChange:M})]})]})}),(0,u.jsx)("div",{className:f.libraryPanelsView,children:(0,u.jsx)(b.u,{onClickCard:a,searchString:O,sortDirection:$,panelFilter:F,folderFilter:N,currentPanelId:n,showSecondaryActions:g,perPage:l})})]})}):(0,u.jsx)("div",{className:f.container,children:(0,u.jsxs)(r.VerticalGroup,{spacing:"xs",children:[(0,u.jsxs)("div",{className:f.tightButtonRow,children:[(0,u.jsx)("div",{className:f.tightFilter,children:(0,u.jsx)(r.FilterInput,{value:O,onChange:D,placeholder:"Search by name",width:0})}),(0,u.jsxs)("div",{className:f.tightSortFilter,children:[d&&(0,u.jsx)(m.P,{value:$,onChange:L}),c&&(0,u.jsx)(p,{onChange:I,maxMenuHeight:200}),o&&(0,u.jsx)(h.j,{onChange:M,maxMenuHeight:200})]})]}),(0,u.jsx)("div",{className:f.libraryPanelsView,children:(0,u.jsx)(b.u,{onClickCard:a,searchString:O,sortDirection:$,panelFilter:F,folderFilter:N,currentPanelId:n,showSecondaryActions:g,perPage:l})})]})})};function k(e){return{container:s.css`
      width: 100%;
      overflow-y: auto;
      padding: ${e.spacing(1)};
    `,buttonRow:s.css`
      display: flex;
      justify-content: space-between;
      width: 100%;
      margin-top: ${e.spacing(2)}; // Clear types link
    `,tightButtonRow:s.css`
      display: flex;
      justify-content: space-between;
      width: 100%;
      margin-top: ${e.spacing(4)}; // Clear types link
    `,tightFilter:s.css`
      flex-grow: 1;
    `,tightSortFilter:s.css`
      flex-grow: 1;
      padding: ${e.spacing(0,0,0,.5)};
    `,libraryPanelsView:s.css`
      width: 100%;
    `}}},23195:(e,a,t)=>{t.d(a,{u:()=>g});var n,s=t(36636),i=t(68404),r=t(93368),l=t(43215),o=t(3490),c=t(71585),d=t(74669),u=t(4401),p=t(45916);const g=e=>{let{className:a,onClickCard:t,searchString:g,sortDirection:m,panelFilter:x,folderFilter:b,showSecondaryActions:f,currentPanelId:y,perPage:j=40}=e;const P=(0,o.useStyles)(h),[{libraryPanels:v,page:w,perPage:S,numberOfPages:C,loadingState:$,currentPanelId:k},F]=(0,i.useReducer)(u._p,Object.assign({},u.p$,{currentPanelId:y,perPage:j})),N=(0,i.useMemo)((()=>(0,d.tb)(F)),[F]);(0,r.Z)((()=>N((0,d.Xu)({searchString:g,sortDirection:m,panelFilter:x,folderFilter:b,page:w,perPage:S,currentPanelId:k}))),300,[g,m,x,b,w,N]);const O=e=>{let{uid:a}=e;return N((0,d.UO)(a,{searchString:g,page:w,perPage:S}))};return(0,p.jsxs)("div",{className:(0,s.cx)(P.container,a),children:[(0,p.jsx)("div",{className:P.libraryPanelList,children:$===l.LoadingState.Loading?n||(n=(0,p.jsx)("p",{children:"Loading library panels..."})):v.length<1?(0,p.jsx)("p",{className:P.noPanelsFound,children:"No library panels found."}):null==v?void 0:v.map(((e,a)=>(0,p.jsx)(c.p,{libraryPanel:e,onDelete:O,onClick:t,showSecondaryActions:f},`library-panel=${a}`)))}),v.length?(0,p.jsx)("div",{className:P.pagination,children:(0,p.jsx)(o.Pagination,{currentPage:w,numberOfPages:C,onNavigate:e=>N((0,u.oO)({page:e})),hideWhenSinglePage:!0})}):null]})},h=e=>({container:s.css`
      display: flex;
      flex-direction: column;
      flex-wrap: nowrap;
    `,libraryPanelList:s.css`
      max-width: 100%;
      display: grid;
      grid-gap: ${e.spacing.sm};
    `,searchHeader:s.css`
      display: flex;
    `,newPanelButton:s.css`
      margin-top: 10px;
      align-self: flex-start;
    `,pagination:s.css`
      align-self: center;
      margin-top: ${e.spacing.sm};
    `,noPanelsFound:s.css`
      label: noPanelsFound;
      min-height: 200px;
    `})},74669:(e,a,t)=>{t.d(a,{UO:()=>b,Xu:()=>x,tb:()=>f});var n=t(4203),s=t(94396),i=t(71808),r=t(3321),l=t(46089),o=t(71114),c=t(14444),d=t(24298),u=t(2027),p=t(48099),g=t(40818),h=t(23078),m=t(4401);function x(e){return function(a){const t=new n.w0,x=(0,s.D)((0,h.Pq)({searchString:e.searchString,perPage:e.perPage,page:e.page,excludeUid:e.currentPanelId,sortDirection:e.sortDirection,typeFilter:e.panelFilter,folderFilter:e.folderFilter})).pipe((0,o.z)((e=>{let{perPage:a,elements:t,page:n,totalCount:s}=e;return(0,i.of)((0,m.zK)({libraryPanels:t,page:n,perPage:a,totalCount:s}))})),(0,c.K)((a=>(console.error(a),(0,i.of)((0,m.zK)(Object.assign({},m.p$,{page:e.page,perPage:e.perPage})))))),(0,d.x)((()=>t.unsubscribe())),(0,u.B)());t.add((0,r.T)((0,l.H)(50).pipe((0,p.h)((0,m.xU)()),(0,g.R)(x)),x).subscribe(a))}}function b(e,a){return async function(t){try{await(0,h.UO)(e),x(a)(t)}catch(e){console.error(e)}}}function f(e){return function(a){return a instanceof Function?a(e):e(a)}}},4401:(e,a,t)=>{t.d(a,{_p:()=>c,oO:()=>o,p$:()=>i,xU:()=>r,zK:()=>l});var n=t(35140),s=t(43215);const i={loadingState:s.LoadingState.Loading,libraryPanels:[],totalCount:0,perPage:40,page:1,numberOfPages:0,currentPanelId:void 0},r=(0,n.PH)("libraryPanels/view/initSearch"),l=(0,n.PH)("libraryPanels/view/searchCompleted"),o=(0,n.PH)("libraryPanels/view/changePage"),c=(e,a)=>{if(r.match(a))return Object.assign({},e,{loadingState:s.LoadingState.Loading});if(l.match(a)){const{libraryPanels:t,page:n,perPage:i,totalCount:r}=a.payload,l=Math.ceil(r/i);return Object.assign({},e,{libraryPanels:t,perPage:i,totalCount:r,loadingState:s.LoadingState.Done,numberOfPages:l,page:n>l?n-1:n})}return o.match(a)?Object.assign({},e,{page:a.payload.page}):e}},34751:(e,a,t)=>{t.d(a,{J:()=>s});var n=t(36636);function s(e){return{myTable:n.css`
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
    `,noteTextbox:n.css`
      margin-bottom: ${e.spacing.xl};
    `,textInfo:n.css`
      color: ${e.colors.textSemiWeak};
      font-size: ${e.typography.size.sm};
    `,dashboardSearch:n.css`
      margin-top: ${e.spacing.md};
    `,modal:n.css`
      width: 500px;
    `,modalText:n.css`
      font-size: ${e.typography.heading.h4};
      color: ${e.colors.link};
      margin-bottom: calc(${e.spacing.d} * 2);
      padding-top: ${e.spacing.d};
    `}}},87092:(e,a,t)=>{t.d(a,{X:()=>c});var n=t(36636),s=(t(68404),t(43215)),i=t(16695),r=t(3490),l=t(30978),o=t(45916);const c=e=>{let{isCurrent:a,title:t,plugin:l,onClick:c,onDelete:p,disabled:g,showBadge:h,description:m,children:x}=e;const b=(0,r.useStyles2)(d),f=(0,n.cx)({[b.item]:!0,[b.disabled]:g||l.state===s.PluginState.deprecated,[b.current]:a});return(0,o.jsxs)("div",{className:f,"aria-label":i.wl.components.PluginVisualization.item(l.name),onClick:g?void 0:c,title:a?"Click again to close this section":l.name,children:[(0,o.jsx)("img",{className:b.img,src:l.info.logos.small,alt:""}),(0,o.jsxs)("div",{className:b.itemContent,children:[(0,o.jsx)("div",{className:b.name,children:t}),m?(0,o.jsx)("span",{className:b.description,children:m}):null,x]}),h&&(0,o.jsx)("div",{className:(0,n.cx)(b.badge,g&&b.disabled),children:(0,o.jsx)(u,{plugin:l})}),p&&(0,o.jsx)(r.IconButton,{name:"trash-alt",onClick:e=>{e.stopPropagation(),p()},className:b.deleteButton,"aria-label":"Delete button on panel type card"})]})};c.displayName="PanelTypeCard";const d=e=>({item:n.css`
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
    `,itemContent:n.css`
      overflow: hidden;
      position: relative;
      padding: ${e.spacing(0,1)};
    `,current:n.css`
      label: currentVisualizationItem;
      border: 1px solid ${e.colors.primary.border};
      background: ${e.colors.action.selected};
    `,disabled:n.css`
      opacity: 0.2;
      filter: grayscale(1);
      cursor: default;
      pointer-events: none;
    `,name:n.css`
      text-overflow: ellipsis;
      overflow: hidden;
      font-size: ${e.typography.size.sm};
      font-weight: ${e.typography.fontWeightMedium};
      width: 100%;
    `,description:n.css`
      display: block;
      text-overflow: ellipsis;
      overflow: hidden;
      color: ${e.colors.text.secondary};
      font-size: ${e.typography.bodySmall.fontSize};
      font-weight: ${e.typography.fontWeightLight};
      width: 100%;
      max-height: 4.5em;
    `,img:n.css`
      max-height: 38px;
      width: 38px;
      display: flex;
      align-items: center;
    `,badge:n.css`
      background: ${e.colors.background.primary};
    `,deleteButton:n.css`
      margin-left: auto;
    `}),u=e=>{let{plugin:a}=e;return(0,s.isUnsignedPluginSignature)(a.signature)?(0,o.jsx)(r.PluginSignatureBadge,{status:a.signature}):(0,o.jsx)(l.u,{state:a.state})};u.displayName="PanelPluginBadge"},7874:(e,a,t)=>{t.d(a,{r:()=>r,x:()=>i});var n=t(43215),s=t(78837);function i(){const e=s.vc.panels;return Object.keys(e).filter((a=>!1===e[a].hideFromList)).map((a=>e[a])).sort(((e,a)=>e.sort-a.sort))}function r(e,a,t){if(!a.length)return e.filter((e=>e.state!==n.PluginState.deprecated||t.id===e.id));const s=(0,n.unEscapeStringFromRegex)(a).toLowerCase(),i=[],r=[],l="graph".startsWith(s);for(const a of e){if(a.state===n.PluginState.deprecated&&t.id!==a.id)continue;const e=a.name.toLowerCase().indexOf(s);0===e?i.push(a):e>0?r.push(a):l&&"timeseries"===a.id&&i.push(a)}return i.concat(r)}},30978:(e,a,t)=>{t.d(a,{u:()=>r});t(68404);var n=t(43215),s=t(3490),i=t(45916);const r=e=>{const a=function(e){switch(e){case n.PluginState.deprecated:return{text:"Deprecated",color:"red",tooltip:"This feature is deprecated and will be removed in a future release"};case n.PluginState.alpha:return{text:"Alpha",color:"blue",tooltip:"This feature is experimental and future updates might not be backward compatible"};case n.PluginState.beta:return{text:"Beta",color:"blue",tooltip:"This feature is close to complete but not fully tested"};default:return null}}(e.state);return a?(0,i.jsx)(s.Badge,{color:a.color,title:a.tooltip,text:a.text,icon:a.icon}):null}}}]);
//# sourceMappingURL=9586.38943810912de4805b87.js.map
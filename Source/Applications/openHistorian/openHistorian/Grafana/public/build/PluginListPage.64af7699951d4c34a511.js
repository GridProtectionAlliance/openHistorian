"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[94],{13548:(e,r,s)=>{s.d(r,{SX:()=>n,IF:()=>p,oZ:()=>c,xh:()=>g});s(68404);var a=s(43215),l=s(3490),t=s(45916);function n(e){let{error:r}=e;const s=function(e){switch(e){case a.PluginErrorCode.modifiedSignature:return"Plugin disabled due to modified content";case a.PluginErrorCode.invalidSignature:return"Plugin disabled due to invalid plugin signature";case a.PluginErrorCode.missingSignature:return"Plugin disabled due to missing plugin signature";case null:case void 0:return"Plugin disabled";default:return"Plugin disabled due to unknown error"+(e?`: ${e}`:"")}}(r);return(0,t.jsx)(l.Badge,{icon:"exclamation-triangle",text:"Disabled",color:"red",tooltip:s})}var i=s(36636);const o=e=>i.css`
  background: ${e.colors.background.primary};
  border-color: ${e.colors.border.strong};
  color: ${e.colors.text.secondary};
`;function c(){const e=(0,l.useStyles2)(o);return(0,t.jsx)(l.Badge,{text:"Installed",color:"orange",className:e})}var u,d=s(90923);function p(e){let{plugin:r}=e;const s=(0,l.useStyles2)(o);return(0,d.featureEnabled)("enterprise.plugins")?u||(u=(0,t.jsx)(l.Badge,{text:"Enterprise",color:"blue"})):(0,t.jsxs)(l.HorizontalGroup,{children:[(0,t.jsx)(l.PluginSignatureBadge,{status:r.signature}),(0,t.jsx)(l.Badge,{icon:"lock","aria-label":"lock icon",text:"Enterprise",color:"blue",className:s}),(0,t.jsx)(l.Button,{size:"sm",fill:"text",icon:"external-link-alt",onClick:e=>{e.preventDefault(),window.open(`https://grafana.com/grafana/plugins/${r.id}?utm_source=grafana_catalog_learn_more`,"_blank","noopener,noreferrer")},children:"Learn more"})]})}function g(e){let{plugin:r}=e;const s=(0,l.useStyles2)(h);return r.hasUpdate&&!r.isCore&&r.type!==a.PluginType.renderer?(0,t.jsx)("p",{className:s.hasUpdate,children:"Update available!"}):null}const h=e=>({hasUpdate:i.css`
      color: ${e.colors.text.secondary};
      font-size: ${e.typography.bodySmall.fontSize};
      margin-bottom: 0;
    `})},845:(e,r,s)=>{s.d(r,{E:()=>l});s(68404);var a=s(45916);function l(e){let{alt:r,className:s,src:l,height:t}=e;return(0,a.jsx)("img",{src:l,className:s,alt:r,loading:"lazy",height:t})}},62591:(e,r,s)=>{s.r(r),s.d(r,{default:()=>D});var a=s(36636),l=s(68404),t=s(18745),n=s(42326),i=s(90923),o=s(3490),c=s(69371),u=s(8674),d=s(45916);const p=e=>{let{children:r,wrap:s,className:l}=e;const t=(0,o.useTheme2)(),n=g(t,s);return(0,d.jsx)("div",{className:(0,a.cx)(n.container,l),children:r})},g=(e,r)=>({container:a.css`
    display: flex;
    flex-direction: row;
    flex-wrap: ${r?"wrap":"no-wrap"};
    & > * {
      margin-bottom: ${e.spacing()};
      margin-right: ${e.spacing()};
    }
    & > *:last-child {
      margin-right: 0;
    }
  `});var h,x=s(79729),y=s(13548);function m(e){let{plugin:r}=e;return r.isEnterprise?(0,d.jsxs)(o.HorizontalGroup,{height:"auto",wrap:!0,children:[(0,d.jsx)(y.IF,{plugin:r}),r.isDisabled&&(0,d.jsx)(y.SX,{error:r.error}),(0,d.jsx)(y.xh,{plugin:r})]}):(0,d.jsxs)(o.HorizontalGroup,{height:"auto",wrap:!0,children:[(0,d.jsx)(o.PluginSignatureBadge,{status:r.signature}),r.isDisabled&&(0,d.jsx)(y.SX,{error:r.error}),r.isInstalled&&(h||(h=(0,d.jsx)(y.oZ,{}))),(0,d.jsx)(y.xh,{plugin:r})]})}var f=s(845);const b="48px";function v(e){let{plugin:r,pathName:s,displayMode:l=x.lL.Grid}=e;const t=(0,o.useStyles2)(j),n=l===x.lL.List;return(0,d.jsxs)("a",{href:`${s}/${r.id}`,className:(0,a.cx)(t.container,{[t.list]:n}),children:[(0,d.jsx)(f.E,{src:r.info.logos.small,className:t.pluginLogo,height:b,alt:""}),(0,d.jsx)("h2",{className:(0,a.cx)(t.name,"plugin-name"),children:r.name}),(0,d.jsxs)("div",{className:(0,a.cx)(t.content,"plugin-content"),children:[(0,d.jsxs)("p",{children:["By ",r.orgName]}),(0,d.jsx)(m,{plugin:r})]}),(0,d.jsx)("div",{className:t.pluginType,children:r.type&&(0,d.jsx)(o.Icon,{name:x.Co[r.type],title:`${r.type} plugin`})})]})}const j=e=>({container:a.css`
      display: grid;
      grid-template-columns: ${b} 1fr ${e.spacing(3)};
      grid-template-rows: auto;
      gap: ${e.spacing(2)};
      grid-auto-flow: row;
      background: ${e.colors.background.secondary};
      border-radius: ${e.shape.borderRadius()};
      padding: ${e.spacing(3)};
      transition: ${e.transitions.create(["background-color","box-shadow","border-color","color"],{duration:e.transitions.duration.short})};

      &:hover {
        background: ${e.colors.emphasize(e.colors.background.secondary,.03)};
      }
    `,list:a.css`
      row-gap: 0px;

      > img {
        align-self: start;
      }

      > .plugin-content {
        min-height: 0px;
        grid-area: 2 / 2 / 4 / 3;

        > p {
          margin: ${e.spacing(0,0,.5,0)};
        }
      }

      > .plugin-name {
        align-self: center;
        grid-area: 1 / 2 / 2 / 3;
      }
    `,pluginType:a.css`
      grid-area: 1 / 3 / 2 / 4;
      color: ${e.colors.text.secondary};
    `,pluginLogo:a.css`
      grid-area: 1 / 1 / 3 / 2;
      max-width: 100%;
      align-self: center;
      object-fit: contain;
    `,content:a.css`
      grid-area: 3 / 1 / 4 / 3;
      color: ${e.colors.text.secondary};
    `,name:a.css`
      grid-area: 1 / 2 / 3 / 3;
      align-self: center;
      font-size: ${e.typography.h4.fontSize};
      color: ${e.colors.text.primary};
      margin: 0;
    `}),S=e=>{let{plugins:r,displayMode:s}=e;const l=s===x.lL.List,t=(0,o.useStyles2)(P),c=(0,n.TH)(),u=i.config.appSubUrl+c.pathname;return(0,d.jsx)("div",{className:(0,a.cx)(t.container,{[t.list]:l}),"data-testid":"plugin-list",children:r.map((e=>(0,d.jsx)(v,{plugin:e,pathName:u,displayMode:s},e.id)))})},P=e=>({container:a.css`
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(288px, 1fr));
      gap: ${e.spacing(3)};
    `,list:a.css`
      grid-template-columns: 1fr;
    `});var w=s(93368);const B=e=>{let{value:r,onSearch:s}=e;const[a,t]=(0,l.useState)(r);return function(e){let r=arguments.length>1&&void 0!==arguments[1]?arguments[1]:0,s=arguments.length>2&&void 0!==arguments[2]?arguments[2]:[];const a=(0,l.useRef)(!0),t=[...s,a];(0,w.Z)((()=>{if(!a.current)return e();a.current=!1}),r,t)}((()=>s(null!=a?a:"")),500,[a]),(0,d.jsx)(o.FilterInput,{value:a,onKeyDown:e=>{"Enter"!==e.key&&13!==e.keyCode||s(e.currentTarget.value)},placeholder:"Search Grafana plugins",onChange:e=>{t(e)},width:46})};var $=s(4936);var N=s(30110);function D(e){let{route:r}=e;const s=(0,n.TH)(),l=(0,i.locationSearchToObject)(s.search),g=L(r.routeName),h=(0,t.useSelector)((e=>(0,u.h)(e.navIndex,g))),{displayMode:y,setDisplayMode:m}=(0,N.iY)(),f=(0,o.useStyles2)(C),b={push:e=>{let{query:r}=e;i.locationService.partial(r)}},v=(0,N.y9)(),j=l.q||"",P=l.filterBy||"installed",w=l.filterByType||"all",D=l.sortBy||$.Nh.nameAsc,{isLoading:k,error:T,plugins:E}=(0,N.GE)({query:j,filterBy:P,filterByType:w,sortBy:D}),A=[{value:"all",label:"All"},{value:"installed",label:"Installed"}],z=e=>{b.push({query:{filterBy:e}})};return T?(console.error(T.message),null):(0,d.jsx)(c.T,{navModel:h,children:(0,d.jsxs)(c.T.Contents,{children:[(0,d.jsxs)(p,{wrap:!0,children:[(0,d.jsx)(B,{value:j,onSearch:e=>{b.push({query:{filterBy:"all",filterByType:"all",q:e}})}}),(0,d.jsxs)(p,{wrap:!0,className:f.actionBar,children:[(0,d.jsx)("div",{children:(0,d.jsx)(o.RadioButtonGroup,{value:w,onChange:e=>{b.push({query:{filterByType:e}})},options:[{value:"all",label:"All"},{value:"datasource",label:"Data sources"},{value:"panel",label:"Panels"},{value:"app",label:"Applications"}]})}),v?(0,d.jsx)("div",{children:(0,d.jsx)(o.RadioButtonGroup,{value:P,onChange:z,options:A})}):(0,d.jsx)(o.Tooltip,{content:"This filter has been disabled because the Grafana server cannot access grafana.com",placement:"top",children:(0,d.jsx)("div",{children:(0,d.jsx)(o.RadioButtonGroup,{disabled:!0,value:P,onChange:z,options:A})})}),(0,d.jsx)("div",{children:(0,d.jsx)(o.Select,{"aria-label":"Sort Plugins List",width:24,value:D,onChange:e=>{b.push({query:{sortBy:e.value}})},options:[{value:"nameAsc",label:"Sort by name (A-Z)"},{value:"nameDesc",label:"Sort by name (Z-A)"},{value:"updated",label:"Sort by updated date"},{value:"published",label:"Sort by published date"},{value:"downloads",label:"Sort by downloads"}]})}),(0,d.jsx)("div",{children:(0,d.jsx)(o.RadioButtonGroup,{className:f.displayAs,value:y,onChange:m,options:[{value:x.lL.Grid,icon:"table",description:"Display plugins in a grid layout"},{value:x.lL.List,icon:"list-ul",description:"Display plugins in list"}]})})]})]}),(0,d.jsx)("div",{className:f.listWrap,children:k?(0,d.jsx)(o.LoadingPlaceholder,{className:a.css`
                margin-bottom: 0;
              `,text:"Loading results"}):(0,d.jsx)(S,{plugins:E,displayMode:y})})]})})}const C=e=>({actionBar:a.css`
    ${e.breakpoints.up("xl")} {
      margin-left: auto;
    }
  `,listWrap:a.css`
    margin-top: ${e.spacing(2)};
  `,displayAs:a.css`
    svg {
      margin-right: 0;
    }
  `}),L=e=>e===x.cd.HomeAdmin||e===x.cd.BrowseAdmin?"admin-plugins":"plugins"},30110:(e,r,s)=>{s.d(r,{iY:()=>A,bt:()=>C,ZV:()=>D,GE:()=>S,UQ:()=>w,bJ:()=>P,x3:()=>B,IS:()=>L,y9:()=>N,S1:()=>$,wq:()=>k});var a=s(68404),l=s(18745),t=s(4936),n=s(72192),i=s(1250),o=s(98335),c=s(43215),u=s(79729);const d=e=>e.plugins,p=(0,o.P1)(d,(e=>{let{items:r}=e;return r})),g=(0,o.P1)(d,(e=>{let{settings:r}=e;return r.displayMode})),{selectAll:h,selectById:x}=i.CD.getSelectors(p),y=(e,r)=>(0,o.P1)((e=>(0,o.P1)(h,(r=>r.filter((r=>"installed"===e?r.isInstalled:!r.isCore)))))(e),(e=>e.filter((e=>"all"===r||e.type===r)))),m=(e,r,s)=>(0,o.P1)(y(r,s),(e=>(0,o.P1)(h,(r=>""===e?[]:r.filter((r=>{const s=[];return r.name&&s.push(r.name.toLowerCase()),r.orgName&&s.push(r.orgName.toLowerCase()),s.some((r=>r.includes((0,c.unEscapeStringFromRegex)(e).toLowerCase())))})))))(e),((r,s)=>""===e?r:s)),f=(0,o.P1)(h,(e=>e?e.filter((e=>Boolean(e.error))).map((e=>({pluginId:e.id,errorCode:e.error}))):[])),b=e=>(0,o.P1)(d,(r=>{let{requests:s={}}=r;return s[e]})),v=e=>(0,o.P1)(b(e),(e=>(null==e?void 0:e.status)===u.eE.Pending)),j=e=>(0,o.P1)(b(e),(e=>(null==e?void 0:e.status)===u.eE.Rejected?null==e?void 0:e.error:null)),S=e=>{let{query:r="",filterBy:s="installed",filterByType:a="all",sortBy:n=t.Nh.nameAsc}=e;T();const i=(0,l.useSelector)(m(r,s,a)),{isLoading:o,error:c}=D();return{isLoading:o,error:c,plugins:(0,t.AA)(i,n)}},P=e=>(T(),E(e),(0,l.useSelector)((r=>x(r,e)))),w=()=>(T(),(0,l.useSelector)(f)),B=()=>{const e=(0,l.useDispatch)();return(r,s,a)=>e((0,n.N9)({id:r,version:s,isUpdating:a}))},$=()=>{const e=(0,l.useDispatch)();return r=>e((0,n.Tz)(r))},N=()=>null===(0,l.useSelector)(j(n.tQ.typePrefix)),D=()=>({isLoading:(0,l.useSelector)(v(n.Qd.typePrefix)),error:(0,l.useSelector)(j(n.Qd.typePrefix))}),C=()=>({isLoading:(0,l.useSelector)(v(n.DD.typePrefix)),error:(0,l.useSelector)(j(n.DD.typePrefix))}),L=()=>({isInstalling:(0,l.useSelector)(v(n.N9.typePrefix)),error:(0,l.useSelector)(j(n.N9.typePrefix))}),k=()=>({isUninstalling:(0,l.useSelector)(v(n.Tz.typePrefix)),error:(0,l.useSelector)(j(n.Tz.typePrefix))}),T=()=>{const e=(0,l.useDispatch)(),r=(0,l.useSelector)((s=n.Qd.typePrefix,(0,o.P1)(b(s),(e=>void 0===e))));var s;(0,a.useEffect)((()=>{r&&e((0,n.Qd)())}),[])},E=e=>{const r=(0,l.useDispatch)(),s=(0,l.useSelector)((r=>x(r,e))),t=!(0,l.useSelector)(v(n.DD.typePrefix))&&s&&!s.details;(0,a.useEffect)((()=>{t&&r((0,n.DD)(e))}),[s])},A=()=>{const e=(0,l.useDispatch)();return{displayMode:(0,l.useSelector)(g),setDisplayMode:r=>e((0,i.UC)(r))}}}}]);
//# sourceMappingURL=PluginListPage.64af7699951d4c34a511.js.map
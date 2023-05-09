"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[94],{75260:(w,D,a)=>{a.d(D,{SX:()=>c,IF:()=>A,oZ:()=>f,xh:()=>T});var r=a(68404),e=a(24699),y=a(19985);function c({error:o}){const h=L(o);return r.createElement(y.C,{icon:"exclamation-triangle",text:"Disabled",color:"red",tooltip:h})}function L(o){switch(o){case e.w2.modifiedSignature:return"Plugin disabled due to modified content";case e.w2.invalidSignature:return"Plugin disabled due to invalid plugin signature";case e.w2.missingSignature:return"Plugin disabled due to missing plugin signature";case null:case void 0:return"Plugin disabled";default:return`Plugin disabled due to unknown error${o?`: ${o}`:""}`}}var i=a(72648),p=a(9892);const S=o=>p.css`
  background: ${o.colors.background.primary};
  border-color: ${o.colors.border.strong};
  color: ${o.colors.text.secondary};
`;function f(){const o=(0,i.wW)(S);return r.createElement(y.C,{text:"Installed",color:"orange",className:o})}var N=a(66121),$=a(52081),P=a(51613),C=a(31403);function A({plugin:o}){const h=(0,i.wW)(S),x=v=>{v.preventDefault(),window.open(`https://grafana.com/grafana/plugins/${o.id}?utm_source=grafana_catalog_learn_more`,"_blank","noopener,noreferrer")};return(0,N.v)("enterprise.plugins")?r.createElement(y.C,{text:"Enterprise",color:"blue"}):r.createElement($.Lh,null,r.createElement(P.o,{status:o.signature}),r.createElement(y.C,{icon:"lock","aria-label":"lock icon",text:"Enterprise",color:"blue",className:h}),r.createElement(C.zx,{size:"sm",fill:"text",icon:"external-link-alt",onClick:x},"Learn more"))}function T({plugin:o}){const h=(0,i.wW)(b);return o.hasUpdate&&!o.isCore&&o.type!==e.zV.renderer?r.createElement("p",{className:h.hasUpdate},"Update available!"):null}const b=o=>({hasUpdate:p.css`
      color: ${o.colors.text.secondary};
      font-size: ${o.typography.bodySmall.fontSize};
      margin-bottom: 0;
    `})},21701:(w,D,a)=>{a.r(D),a.d(D,{default:()=>I});var r=a(9892),e=a(68404),y=a(70356),c=a(37932),L=a(35645),i=a(72648),p=a(24799),S=a(53217),f=a(2594),N=a(6554),$=a(61744),P=a(79396),C=a(86245),A=a(86475),T=a(81168),b=a(21048),o=a(48996),h=a(39904),x=a(52081),v=a(51613),m=a(75260);function R({plugin:n}){return n.isEnterprise?e.createElement(x.Lh,{height:"auto",wrap:!0},e.createElement(m.IF,{plugin:n}),n.isDisabled&&e.createElement(m.SX,{error:n.error}),e.createElement(m.xh,{plugin:n})):e.createElement(x.Lh,{height:"auto",wrap:!0},e.createElement(v.o,{status:n.signature}),n.isDisabled&&e.createElement(m.SX,{error:n.error}),n.isInstalled&&e.createElement(m.oZ,null),e.createElement(m.xh,{plugin:n}))}function U({alt:n,className:d,src:t,height:s}){return e.createElement("img",{src:t,className:d,alt:n,loading:"lazy",height:s})}const W="48px";function M({plugin:n,pathName:d,displayMode:t=o.lL.Grid}){const s=(0,i.wW)(k),l=t===o.lL.List;return e.createElement("a",{href:`${d}/${n.id}`,className:(0,r.cx)(s.container,{[s.list]:l})},e.createElement(U,{src:n.info.logos.small,className:s.pluginLogo,height:W,alt:""}),e.createElement("h2",{className:(0,r.cx)(s.name,"plugin-name")},n.name),e.createElement("div",{className:(0,r.cx)(s.content,"plugin-content")},e.createElement("p",null,"By ",n.orgName),e.createElement(R,{plugin:n})),e.createElement("div",{className:s.pluginType},n.type&&e.createElement(h.J,{name:o.Co[n.type],title:`${n.type} plugin`})))}const k=n=>({container:r.css`
      display: grid;
      grid-template-columns: ${W} 1fr ${n.spacing(3)};
      grid-template-rows: auto;
      gap: ${n.spacing(2)};
      grid-auto-flow: row;
      background: ${n.colors.background.secondary};
      border-radius: ${n.shape.borderRadius()};
      padding: ${n.spacing(3)};
      transition: ${n.transitions.create(["background-color","box-shadow","border-color","color"],{duration:n.transitions.duration.short})};

      &:hover {
        background: ${n.colors.emphasize(n.colors.background.secondary,.03)};
      }
    `,list:r.css`
      row-gap: 0px;

      > img {
        align-self: start;
      }

      > .plugin-content {
        min-height: 0px;
        grid-area: 2 / 2 / 4 / 3;

        > p {
          margin: ${n.spacing(0,0,.5,0)};
        }
      }

      > .plugin-name {
        align-self: center;
        grid-area: 1 / 2 / 2 / 3;
      }
    `,pluginType:r.css`
      grid-area: 1 / 3 / 2 / 4;
      color: ${n.colors.text.secondary};
    `,pluginLogo:r.css`
      grid-area: 1 / 1 / 3 / 2;
      max-width: 100%;
      align-self: center;
      object-fit: contain;
    `,content:r.css`
      grid-area: 3 / 1 / 4 / 3;
      color: ${n.colors.text.secondary};
    `,name:r.css`
      grid-area: 1 / 2 / 3 / 3;
      align-self: center;
      font-size: ${n.typography.h4.fontSize};
      color: ${n.colors.text.primary};
      margin: 0;
    `}),Q=({plugins:n,displayMode:d})=>{const t=d===o.lL.List,s=(0,i.wW)(Z),{pathname:l}=(0,y.TH)(),u=L.v.appSubUrl+(l.endsWith("/")?l.slice(0,-1):l);return e.createElement("div",{className:(0,r.cx)(s.container,{[s.list]:t}),"data-testid":"plugin-list"},n.map(g=>e.createElement(M,{key:g.id,plugin:g,pathName:u,displayMode:d})))},Z=n=>({container:r.css`
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(288px, 1fr));
      gap: ${n.spacing(3)};
    `,list:r.css`
      grid-template-columns: 1fr;
    `});var H=a(70197),O=a(14747);const V=(n,d=0,t=[])=>{const s=(0,e.useRef)(!0),l=[...t,s];return(0,H.Z)(()=>{if(s.current){s.current=!1;return}return n()},d,l)},G=({value:n,onSearch:d})=>{const[t,s]=(0,e.useState)(n);return V(()=>d(t??""),500,[t]),e.createElement(O.H,{value:t,onKeyDown:l=>{(l.key==="Enter"||l.keyCode===13)&&d(l.currentTarget.value)},placeholder:"Search Grafana plugins",onChange:l=>{s(l)},width:46})};var X=a(59210);const j=()=>({push:({query:n})=>{c.E1.partial(n)}});var F=a(74509);function I({route:n}){const d=(0,y.TH)(),t=(0,c.Ox)(d.search),s=(0,T.useSelector)(E=>(0,C.ht)(E.navIndex,"plugins")),{displayMode:l,setDisplayMode:u}=(0,F.iY)(),g=(0,i.wW)(J),B=j(),K=(0,F.y9)(),z=t.q||"",Y=t.filterBy||"installed",q=t.filterByType||"all",_=t.sortBy||X.Nh.nameAsc,{isLoading:ne,error:ee,plugins:ae}=(0,F.GE)({query:z,filterBy:Y,filterByType:q,sortBy:_}),te=[{value:"all",label:"All"},{value:"installed",label:"Installed"}],le=E=>{B.push({query:{sortBy:E.value}})},se=E=>{B.push({query:{filterBy:E}})},oe=E=>{B.push({query:{filterByType:E.value}})},re=E=>{B.push({query:{filterBy:"all",filterByType:"all",q:E}})};if(ee)return console.error(ee.message),null;const ce=L.v.featureToggles.dataConnectionsConsole?e.createElement("p",null,"Extend the Grafana experience with panel plugins and apps. To find more data sources go to"," ",e.createElement("a",{href:`${A.Z.ConnectData}?cat=data-source`},"Connections"),"."):e.createElement("p",null,"Extend the Grafana experience with panel plugins and apps.");return e.createElement(P.T,{navModel:s,subTitle:ce},e.createElement(P.T.Contents,null,e.createElement(b.L,{wrap:!0},e.createElement(p.g,{label:"Search"},e.createElement(G,{value:z,onSearch:re})),e.createElement(b.L,{wrap:!0,className:g.actionBar},e.createElement(p.g,{label:"Type"},e.createElement(S.Ph,{"aria-label":"Plugin type filter",value:q,onChange:oe,width:18,options:[{value:"all",label:"All"},{value:"datasource",label:"Data sources"},{value:"panel",label:"Panels"},{value:"app",label:"Applications"}]})),K?e.createElement(p.g,{label:"State"},e.createElement(f.S,{value:Y,onChange:se,options:te})):e.createElement(N.u,{content:"This filter has been disabled because the Grafana server cannot access grafana.com",placement:"top"},e.createElement("div",null,e.createElement(p.g,{label:"State"},e.createElement(f.S,{disabled:!0,value:Y,onChange:se,options:te})))),e.createElement(p.g,{label:"Sort"},e.createElement(S.Ph,{"aria-label":"Sort Plugins List",width:24,value:_,onChange:le,options:[{value:"nameAsc",label:"By name (A-Z)"},{value:"nameDesc",label:"By name (Z-A)"},{value:"updated",label:"By updated date"},{value:"published",label:"By published date"},{value:"downloads",label:"By downloads"}]})),e.createElement(p.g,{label:"View"},e.createElement(f.S,{className:g.displayAs,value:l,onChange:u,options:[{value:o.lL.Grid,icon:"table",description:"Display plugins in a grid layout"},{value:o.lL.List,icon:"list-ul",description:"Display plugins in list"}]})))),e.createElement("div",{className:g.listWrap},ne?e.createElement($.u,{className:r.css`
                margin-bottom: 0;
              `,text:"Loading results"}):e.createElement(Q,{plugins:ae,displayMode:l}))))}const J=n=>({actionBar:r.css`
    ${n.breakpoints.up("xl")} {
      margin-left: auto;
    }
  `,listWrap:r.css`
    margin-top: ${n.spacing(2)};
  `,displayAs:r.css`
    svg {
      margin-right: 0;
    }
  `})},74509:(w,D,a)=>{a.d(D,{iY:()=>d,bt:()=>X,ZV:()=>G,GE:()=>U,UQ:()=>Q,bJ:()=>M,x3:()=>Z,IS:()=>j,y9:()=>V,S1:()=>O,wq:()=>F,kH:()=>H});var r=a(68404),e=a(81168),y=a(59210),c=a(85805),L=a(66552),i=a(90158),p=a(29076),S=a(48996);const f=t=>t.plugins,N=(0,i.P1)(f,({items:t})=>t),$=(0,i.P1)(f,({settings:t})=>t.displayMode),{selectAll:P,selectById:C}=L.CD.getSelectors(N),A=t=>(0,i.P1)(P,s=>s.filter(l=>t==="installed"?l.isInstalled:!l.isCore)),T=(t,s)=>(0,i.P1)(A(t),l=>l.filter(u=>s==="all"||u.type===s)),b=t=>(0,i.P1)(P,s=>t===""?[]:s.filter(l=>{const u=[];return l.name&&u.push(l.name.toLowerCase()),l.orgName&&u.push(l.orgName.toLowerCase()),u.some(g=>g.includes((0,p.x6)(t).toLowerCase()))})),o=(t,s,l)=>(0,i.P1)(T(s,l),b(t),(u,g)=>t===""?u:g),h=(0,i.P1)(P,t=>t?t.filter(s=>Boolean(s.error)).map(s=>({pluginId:s.id,errorCode:s.error})):[]),x=t=>(0,i.P1)(f,({requests:s={}})=>s[t]),v=t=>(0,i.P1)(x(t),s=>s?.status===S.eE.Pending),m=t=>(0,i.P1)(x(t),s=>s?.status===S.eE.Rejected?s?.error:null),R=t=>(0,i.P1)(x(t),s=>s===void 0),U=({query:t="",filterBy:s="installed",filterByType:l="all",sortBy:u=y.Nh.nameAsc})=>{I();const g=(0,e.useSelector)(o(t,s,l)),{isLoading:B,error:K}=G(),z=(0,y.AA)(g,u);return{isLoading:B,error:K,plugins:z}},W=()=>(I(),useSelector(selectAll)),M=t=>(I(),n(t),(0,e.useSelector)(s=>C(s,t))),k=t=>(J(),useSelector(s=>selectById(s,t))),Q=()=>(I(),(0,e.useSelector)(h)),Z=()=>{const t=(0,e.useDispatch)();return(s,l,u)=>t((0,c.N9)({id:s,version:l,isUpdating:u}))},H=()=>{const t=(0,e.useDispatch)();return()=>t((0,c.bQ)())},O=()=>{const t=(0,e.useDispatch)();return s=>t((0,c.Tz)(s))},V=()=>(0,e.useSelector)(m(c.tQ.typePrefix))===null,G=()=>{const t=(0,e.useSelector)(v(c.Qd.typePrefix)),s=(0,e.useSelector)(m(c.Qd.typePrefix));return{isLoading:t,error:s}},X=()=>{const t=(0,e.useSelector)(v(c.DD.typePrefix)),s=(0,e.useSelector)(m(c.DD.typePrefix));return{isLoading:t,error:s}},j=()=>{const t=(0,e.useSelector)(v(c.N9.typePrefix)),s=(0,e.useSelector)(m(c.N9.typePrefix));return{isInstalling:t,error:s}},F=()=>{const t=(0,e.useSelector)(v(c.Tz.typePrefix)),s=(0,e.useSelector)(m(c.Tz.typePrefix));return{isUninstalling:t,error:s}},I=()=>{const t=(0,e.useDispatch)(),s=(0,e.useSelector)(R(c.Qd.typePrefix));(0,r.useEffect)(()=>{s&&t((0,c.Qd)())},[])},J=()=>{const t=useDispatch(),s=useSelector(selectIsRequestNotFetched(fetchAllLocal.typePrefix));useEffect(()=>{s&&t(fetchAllLocal())},[])},n=t=>{const s=(0,e.useDispatch)(),l=(0,e.useSelector)(B=>C(B,t)),g=!(0,e.useSelector)(v(c.DD.typePrefix))&&l&&!l.details;(0,r.useEffect)(()=>{g&&s((0,c.DD)(t))},[l])},d=()=>{const t=(0,e.useDispatch)();return{displayMode:(0,e.useSelector)($),setDisplayMode:l=>t((0,L.UC)(l))}}}}]);

//# sourceMappingURL=PluginListPage.64fe2ffc8392dab48b4b.js.map
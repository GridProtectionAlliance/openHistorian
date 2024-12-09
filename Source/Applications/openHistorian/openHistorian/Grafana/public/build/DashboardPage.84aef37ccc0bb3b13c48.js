"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[2976,9136,2859],{50051:(B,_,e)=>{e.d(_,{A:()=>i});var t=e(71354),r=e.n(t),n=e(76314),d=e.n(n),s=d()(r());s.push([B.id,`.react-grid-layout {
  position: relative;
  transition: height 200ms ease;
}
.react-grid-item {
  transition: all 200ms ease;
  transition-property: left, top, width, height;
}
.react-grid-item img {
  pointer-events: none;
  user-select: none;
}
.react-grid-item.cssTransforms {
  transition-property: transform, width, height;
}
.react-grid-item.resizing {
  transition: none;
  z-index: 1;
  will-change: width, height;
}

.react-grid-item.react-draggable-dragging {
  transition: none;
  z-index: 3;
  will-change: transform;
}

.react-grid-item.dropping {
  visibility: hidden;
}

.react-grid-item.react-grid-placeholder {
  background: red;
  opacity: 0.2;
  transition-duration: 100ms;
  z-index: 2;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  -o-user-select: none;
  user-select: none;
}

.react-grid-item.react-grid-placeholder.placeholder-resizing {
  transition: none;
}

.react-grid-item > .react-resizable-handle {
  position: absolute;
  width: 20px;
  height: 20px;
}

.react-grid-item > .react-resizable-handle::after {
  content: "";
  position: absolute;
  right: 3px;
  bottom: 3px;
  width: 5px;
  height: 5px;
  border-right: 2px solid rgba(0, 0, 0, 0.4);
  border-bottom: 2px solid rgba(0, 0, 0, 0.4);
}

.react-resizable-hide > .react-resizable-handle {
  display: none;
}

.react-grid-item > .react-resizable-handle.react-resizable-handle-sw {
  bottom: 0;
  left: 0;
  cursor: sw-resize;
  transform: rotate(90deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-se {
  bottom: 0;
  right: 0;
  cursor: se-resize;
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-nw {
  top: 0;
  left: 0;
  cursor: nw-resize;
  transform: rotate(180deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-ne {
  top: 0;
  right: 0;
  cursor: ne-resize;
  transform: rotate(270deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-w,
.react-grid-item > .react-resizable-handle.react-resizable-handle-e {
  top: 50%;
  margin-top: -10px;
  cursor: ew-resize;
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-w {
  left: 0;
  transform: rotate(135deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-e {
  right: 0;
  transform: rotate(315deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-n,
.react-grid-item > .react-resizable-handle.react-resizable-handle-s {
  left: 50%;
  margin-left: -10px;
  cursor: ns-resize;
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-n {
  top: 0;
  transform: rotate(225deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-s {
  bottom: 0;
  transform: rotate(45deg);
}
`,"",{version:3,sources:["webpack://./node_modules/react-grid-layout/css/styles.css"],names:[],mappings:"AAAA;EACE,kBAAkB;EAClB,6BAA6B;AAC/B;AACA;EACE,0BAA0B;EAC1B,6CAA6C;AAC/C;AACA;EACE,oBAAoB;EACpB,iBAAiB;AACnB;AACA;EACE,6CAA6C;AAC/C;AACA;EACE,gBAAgB;EAChB,UAAU;EACV,0BAA0B;AAC5B;;AAEA;EACE,gBAAgB;EAChB,UAAU;EACV,sBAAsB;AACxB;;AAEA;EACE,kBAAkB;AACpB;;AAEA;EACE,eAAe;EACf,YAAY;EACZ,0BAA0B;EAC1B,UAAU;EACV,yBAAyB;EACzB,sBAAsB;EACtB,qBAAqB;EACrB,oBAAoB;EACpB,iBAAiB;AACnB;;AAEA;EACE,gBAAgB;AAClB;;AAEA;EACE,kBAAkB;EAClB,WAAW;EACX,YAAY;AACd;;AAEA;EACE,WAAW;EACX,kBAAkB;EAClB,UAAU;EACV,WAAW;EACX,UAAU;EACV,WAAW;EACX,0CAA0C;EAC1C,2CAA2C;AAC7C;;AAEA;EACE,aAAa;AACf;;AAEA;EACE,SAAS;EACT,OAAO;EACP,iBAAiB;EACjB,wBAAwB;AAC1B;AACA;EACE,SAAS;EACT,QAAQ;EACR,iBAAiB;AACnB;AACA;EACE,MAAM;EACN,OAAO;EACP,iBAAiB;EACjB,yBAAyB;AAC3B;AACA;EACE,MAAM;EACN,QAAQ;EACR,iBAAiB;EACjB,yBAAyB;AAC3B;AACA;;EAEE,QAAQ;EACR,iBAAiB;EACjB,iBAAiB;AACnB;AACA;EACE,OAAO;EACP,yBAAyB;AAC3B;AACA;EACE,QAAQ;EACR,yBAAyB;AAC3B;AACA;;EAEE,SAAS;EACT,kBAAkB;EAClB,iBAAiB;AACnB;AACA;EACE,MAAM;EACN,yBAAyB;AAC3B;AACA;EACE,SAAS;EACT,wBAAwB;AAC1B",sourcesContent:[`.react-grid-layout {
  position: relative;
  transition: height 200ms ease;
}
.react-grid-item {
  transition: all 200ms ease;
  transition-property: left, top, width, height;
}
.react-grid-item img {
  pointer-events: none;
  user-select: none;
}
.react-grid-item.cssTransforms {
  transition-property: transform, width, height;
}
.react-grid-item.resizing {
  transition: none;
  z-index: 1;
  will-change: width, height;
}

.react-grid-item.react-draggable-dragging {
  transition: none;
  z-index: 3;
  will-change: transform;
}

.react-grid-item.dropping {
  visibility: hidden;
}

.react-grid-item.react-grid-placeholder {
  background: red;
  opacity: 0.2;
  transition-duration: 100ms;
  z-index: 2;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  -o-user-select: none;
  user-select: none;
}

.react-grid-item.react-grid-placeholder.placeholder-resizing {
  transition: none;
}

.react-grid-item > .react-resizable-handle {
  position: absolute;
  width: 20px;
  height: 20px;
}

.react-grid-item > .react-resizable-handle::after {
  content: "";
  position: absolute;
  right: 3px;
  bottom: 3px;
  width: 5px;
  height: 5px;
  border-right: 2px solid rgba(0, 0, 0, 0.4);
  border-bottom: 2px solid rgba(0, 0, 0, 0.4);
}

.react-resizable-hide > .react-resizable-handle {
  display: none;
}

.react-grid-item > .react-resizable-handle.react-resizable-handle-sw {
  bottom: 0;
  left: 0;
  cursor: sw-resize;
  transform: rotate(90deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-se {
  bottom: 0;
  right: 0;
  cursor: se-resize;
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-nw {
  top: 0;
  left: 0;
  cursor: nw-resize;
  transform: rotate(180deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-ne {
  top: 0;
  right: 0;
  cursor: ne-resize;
  transform: rotate(270deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-w,
.react-grid-item > .react-resizable-handle.react-resizable-handle-e {
  top: 50%;
  margin-top: -10px;
  cursor: ew-resize;
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-w {
  left: 0;
  transform: rotate(135deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-e {
  right: 0;
  transform: rotate(315deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-n,
.react-grid-item > .react-resizable-handle.react-resizable-handle-s {
  left: 50%;
  margin-left: -10px;
  cursor: ns-resize;
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-n {
  top: 0;
  transform: rotate(225deg);
}
.react-grid-item > .react-resizable-handle.react-resizable-handle-s {
  bottom: 0;
  transform: rotate(45deg);
}
`],sourceRoot:""}]);const i=s},30291:(B,_,e)=>{e.d(_,{A:()=>g});var t=e(71354),r=e.n(t),n=e(76314),d=e.n(n),s=e(4417),i=e.n(s),E=new URL(e(59224),e.b),c=d()(r()),C=i()(E);c.push([B.id,`.react-resizable {
  position: relative;
}
.react-resizable-handle {
  position: absolute;
  width: 20px;
  height: 20px;
  background-repeat: no-repeat;
  background-origin: content-box;
  box-sizing: border-box;
  background-image: url(${C});
  background-position: bottom right;
  padding: 0 3px 3px 0;
}
.react-resizable-handle-sw {
  bottom: 0;
  left: 0;
  cursor: sw-resize;
  transform: rotate(90deg);
}
.react-resizable-handle-se {
  bottom: 0;
  right: 0;
  cursor: se-resize;
}
.react-resizable-handle-nw {
  top: 0;
  left: 0;
  cursor: nw-resize;
  transform: rotate(180deg);
}
.react-resizable-handle-ne {
  top: 0;
  right: 0;
  cursor: ne-resize;
  transform: rotate(270deg);
}
.react-resizable-handle-w,
.react-resizable-handle-e {
  top: 50%;
  margin-top: -10px;
  cursor: ew-resize;
}
.react-resizable-handle-w {
  left: 0;
  transform: rotate(135deg);
}
.react-resizable-handle-e {
  right: 0;
  transform: rotate(315deg);
}
.react-resizable-handle-n,
.react-resizable-handle-s {
  left: 50%;
  margin-left: -10px;
  cursor: ns-resize;
}
.react-resizable-handle-n {
  top: 0;
  transform: rotate(225deg);
}
.react-resizable-handle-s {
  bottom: 0;
  transform: rotate(45deg);
}`,"",{version:3,sources:["webpack://./node_modules/react-resizable/css/styles.css"],names:[],mappings:"AAAA;EACE,kBAAkB;AACpB;AACA;EACE,kBAAkB;EAClB,WAAW;EACX,YAAY;EACZ,4BAA4B;EAC5B,8BAA8B;EAC9B,sBAAsB;EACtB,yDAAuY;EACvY,iCAAiC;EACjC,oBAAoB;AACtB;AACA;EACE,SAAS;EACT,OAAO;EACP,iBAAiB;EACjB,wBAAwB;AAC1B;AACA;EACE,SAAS;EACT,QAAQ;EACR,iBAAiB;AACnB;AACA;EACE,MAAM;EACN,OAAO;EACP,iBAAiB;EACjB,yBAAyB;AAC3B;AACA;EACE,MAAM;EACN,QAAQ;EACR,iBAAiB;EACjB,yBAAyB;AAC3B;AACA;;EAEE,QAAQ;EACR,iBAAiB;EACjB,iBAAiB;AACnB;AACA;EACE,OAAO;EACP,yBAAyB;AAC3B;AACA;EACE,QAAQ;EACR,yBAAyB;AAC3B;AACA;;EAEE,SAAS;EACT,kBAAkB;EAClB,iBAAiB;AACnB;AACA;EACE,MAAM;EACN,yBAAyB;AAC3B;AACA;EACE,SAAS;EACT,wBAAwB;AAC1B",sourcesContent:[`.react-resizable {
  position: relative;
}
.react-resizable-handle {
  position: absolute;
  width: 20px;
  height: 20px;
  background-repeat: no-repeat;
  background-origin: content-box;
  box-sizing: border-box;
  background-image: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA2IDYiIHN0eWxlPSJiYWNrZ3JvdW5kLWNvbG9yOiNmZmZmZmYwMCIgeD0iMHB4IiB5PSIwcHgiIHdpZHRoPSI2cHgiIGhlaWdodD0iNnB4Ij48ZyBvcGFjaXR5PSIwLjMwMiI+PHBhdGggZD0iTSA2IDYgTCAwIDYgTCAwIDQuMiBMIDQgNC4yIEwgNC4yIDQuMiBMIDQuMiAwIEwgNiAwIEwgNiA2IEwgNiA2IFoiIGZpbGw9IiMwMDAwMDAiLz48L2c+PC9zdmc+');
  background-position: bottom right;
  padding: 0 3px 3px 0;
}
.react-resizable-handle-sw {
  bottom: 0;
  left: 0;
  cursor: sw-resize;
  transform: rotate(90deg);
}
.react-resizable-handle-se {
  bottom: 0;
  right: 0;
  cursor: se-resize;
}
.react-resizable-handle-nw {
  top: 0;
  left: 0;
  cursor: nw-resize;
  transform: rotate(180deg);
}
.react-resizable-handle-ne {
  top: 0;
  right: 0;
  cursor: ne-resize;
  transform: rotate(270deg);
}
.react-resizable-handle-w,
.react-resizable-handle-e {
  top: 50%;
  margin-top: -10px;
  cursor: ew-resize;
}
.react-resizable-handle-w {
  left: 0;
  transform: rotate(135deg);
}
.react-resizable-handle-e {
  right: 0;
  transform: rotate(315deg);
}
.react-resizable-handle-n,
.react-resizable-handle-s {
  left: 50%;
  margin-left: -10px;
  cursor: ns-resize;
}
.react-resizable-handle-n {
  top: 0;
  transform: rotate(225deg);
}
.react-resizable-handle-s {
  bottom: 0;
  transform: rotate(45deg);
}`],sourceRoot:""}]);const g=c},13251:(B,_,e)=>{e.d(_,{EmbeddedDashboard:()=>h});var t=e(74848),r=e(32196),n=e(96540),d=e(76885),s=e(81887),i=e(42418),E=e(62930),c=e(40845),C=e(31678),g=e(72255);function h(a){const l=(0,g.sP)(),{dashboard:u,loadError:D}=l.useState();return(0,n.useEffect)(()=>(l.loadDashboard({uid:a.uid,route:C.DashboardRoutes.Embedded}),()=>{l.clearState()}),[l,a.uid]),D?(0,t.jsx)(i.F,{severity:"error",title:"Failed to load dashboard",children:D}):u?(0,t.jsx)(m,{model:u,...a}):(0,t.jsx)(E.y,{})}function m({model:a,initialState:l,onStateChange:u}){const[D,O]=(0,n.useState)(!1),{controls:M,body:P}=a.useState(),b=(0,c.of)(A);return(0,n.useEffect)(()=>{if(O(!0),l){const f=new URLSearchParams(l);s.Go.syncStateFromSearchParams(a,f)}return a.activate()},[a]),o(u,a),D?(0,t.jsxs)("div",{className:(0,r.cx)(b.canvas,M&&b.canvasWithControls),children:[M&&(0,t.jsx)("div",{className:b.controlsWrapper,children:(0,t.jsx)(M.Component,{model:M})}),(0,t.jsx)("div",{className:b.body,children:(0,t.jsx)(P.Component,{model:P})})]}):null}function o(a,l){(0,n.useEffect)(()=>{if(!a)return;let u="";const D=l.subscribeToEvent(s.bZ,O=>{if(O.payload.changedObject.urlSync){const M=s.Go.getUrlState(l),P=d.kM.renderUrl("",M);u!==P&&(u=P,a(P))}});return()=>D.unsubscribe()},[l,a])}function A(a){return{canvas:(0,r.css)({label:"canvas-content",display:"grid",gridTemplateAreas:`
        "panels"`,gridTemplateColumns:"1fr",gridTemplateRows:"1fr",flexBasis:"100%",flexGrow:1}),canvasWithControls:(0,r.css)({gridTemplateAreas:`
        "controls"
        "panels"`,gridTemplateRows:"auto 1fr"}),body:(0,r.css)({label:"body",flexGrow:1,display:"flex",gap:"8px",gridArea:"panels",marginBottom:a.spacing(2)}),controlsWrapper:(0,r.css)({display:"flex",flexDirection:"column",flexGrow:0,gridArea:"controls",padding:a.spacing(2,0,2,2)})}}},14962:(B,_,e)=>{e.r(_),e.d(_,{EmbeddedDashboardTestPage:()=>E,default:()=>c});var t=e(74848),r=e(96540),n=e(64388),d=e(90613),s=e(21780),i=e(13251);function E(){const[C,g]=(0,r.useState)("?from=now-5m&to=now");return(0,t.jsxs)(s.YW,{navId:"dashboards/browse",pageNav:{text:"Embedding dashboard",subTitle:"Showing dashboard: Panel Tests - Pie chart"},layout:n.k.Canvas,children:[(0,t.jsxs)(d.a,{paddingY:2,children:["Internal url state: ",C]}),(0,t.jsx)(i.EmbeddedDashboard,{uid:"lVE-2YFMz",initialState:C,onStateChange:g})]})}const c=E},30798:(B,_,e)=>{e.r(_),e.d(_,{default:()=>C});var t=e(74848),r=e(96540),n=e(54148),d=e(19347),s=e(39601),i=e(21780),E=e(31678),c=e(28601);function C(){const[g,h]=(0,r.useState)(null),{datasourceUid:m}=(0,n.g)(),o=(0,E.useDispatch)();return(0,r.useEffect)(()=>{if(!(0,d.l)().getInstanceSettings(m)){h("Data source not found");return}o((0,c.Ub)(m)),s.Ny.replace("/dashboard/new")},[m,o]),g?(0,t.jsx)(i.YW,{navId:"dashboards",children:(0,t.jsx)(i.YW.Contents,{children:(0,t.jsxs)("div",{children:['Data source with UID "',m,'" not found.']})})}):null}},65572:(B,_,e)=>{var t=e(85072),r=e.n(t),n=e(97825),d=e.n(n),s=e(77659),i=e.n(s),E=e(55056),c=e.n(E),C=e(10540),g=e.n(C),h=e(41113),m=e.n(h),o=e(50051),A={};A.styleTagTransform=m(),A.setAttributes=c(),A.insert=i().bind(null,"head"),A.domAPI=d(),A.insertStyleElement=g();var a=r()(o.A,A),l=o.A&&o.A.locals?o.A.locals:void 0},9964:(B,_,e)=>{var t=e(85072),r=e.n(t),n=e(97825),d=e.n(n),s=e(77659),i=e.n(s),E=e(55056),c=e.n(E),C=e(10540),g=e.n(C),h=e(41113),m=e.n(h),o=e(30291),A={};A.styleTagTransform=m(),A.setAttributes=c(),A.insert=i().bind(null,"head"),A.domAPI=d(),A.insertStyleElement=g();var a=r()(o.A,A),l=o.A&&o.A.locals?o.A.locals:void 0},59224:B=>{B.exports="data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA2IDYiIHN0eWxlPSJiYWNrZ3JvdW5kLWNvbG9yOiNmZmZmZmYwMCIgeD0iMHB4IiB5PSIwcHgiIHdpZHRoPSI2cHgiIGhlaWdodD0iNnB4Ij48ZyBvcGFjaXR5PSIwLjMwMiI+PHBhdGggZD0iTSA2IDYgTCAwIDYgTCAwIDQuMiBMIDQgNC4yIEwgNC4yIDQuMiBMIDQuMiAwIEwgNiAwIEwgNiA2IEwgNiA2IFoiIGZpbGw9IiMwMDAwMDAiLz48L2c+PC9zdmc+"}}]);

//# sourceMappingURL=DashboardPage.84aef37ccc0bb3b13c48.js.map
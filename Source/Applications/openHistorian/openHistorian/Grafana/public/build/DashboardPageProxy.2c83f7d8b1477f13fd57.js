"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[9136],{50051:(a,d,e)=>{e.d(d,{A:()=>_});var A=e(71354),i=e.n(A),s=e(76314),l=e.n(s),n=l()(i());n.push([a.id,`.react-grid-layout {
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
`],sourceRoot:""}]);const _=n},30291:(a,d,e)=>{e.d(d,{A:()=>C});var A=e(71354),i=e.n(A),s=e(76314),l=e.n(s),n=e(4417),_=e.n(n),E=new URL(e(59224),e.b),o=l()(i()),c=_()(E);o.push([a.id,`.react-resizable {
  position: relative;
}
.react-resizable-handle {
  position: absolute;
  width: 20px;
  height: 20px;
  background-repeat: no-repeat;
  background-origin: content-box;
  box-sizing: border-box;
  background-image: url(${c});
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
}`],sourceRoot:""}]);const C=o},65572:(a,d,e)=>{var A=e(85072),i=e.n(A),s=e(97825),l=e.n(s),n=e(77659),_=e.n(n),E=e(55056),o=e.n(E),c=e(10540),C=e.n(c),B=e(41113),g=e.n(B),r=e(50051),t={};t.styleTagTransform=g(),t.setAttributes=o(),t.insert=_().bind(null,"head"),t.domAPI=l(),t.insertStyleElement=C();var m=i()(r.A,t),h=r.A&&r.A.locals?r.A.locals:void 0},9964:(a,d,e)=>{var A=e(85072),i=e.n(A),s=e(97825),l=e.n(s),n=e(77659),_=e.n(n),E=e(55056),o=e.n(E),c=e(10540),C=e.n(c),B=e(41113),g=e.n(B),r=e(30291),t={};t.styleTagTransform=g(),t.setAttributes=o(),t.insert=_().bind(null,"head"),t.domAPI=l(),t.insertStyleElement=C();var m=i()(r.A,t),h=r.A&&r.A.locals?r.A.locals:void 0},59224:a=>{a.exports="data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA2IDYiIHN0eWxlPSJiYWNrZ3JvdW5kLWNvbG9yOiNmZmZmZmYwMCIgeD0iMHB4IiB5PSIwcHgiIHdpZHRoPSI2cHgiIGhlaWdodD0iNnB4Ij48ZyBvcGFjaXR5PSIwLjMwMiI+PHBhdGggZD0iTSA2IDYgTCAwIDYgTCAwIDQuMiBMIDQgNC4yIEwgNC4yIDQuMiBMIDQuMiAwIEwgNiAwIEwgNiA2IEwgNiA2IFoiIGZpbGw9IiMwMDAwMDAiLz48L2c+PC9zdmc+"}}]);

//# sourceMappingURL=DashboardPageProxy.2c83f7d8b1477f13fd57.js.map
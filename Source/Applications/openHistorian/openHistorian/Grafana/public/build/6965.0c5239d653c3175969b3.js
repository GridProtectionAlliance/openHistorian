"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[6965],{86889:(Q,W,n)=>{n.d(W,{z:()=>x,e:()=>M});var l=n(74848),h=n(96540),P=n(11261),V=n(52811),z=n(78226),p=n(32196),N=n(34743),S=n(83505),Y=n(46885),D=n(83195),I=n(13544),Z=n(40845),G=n(17464),H=n(74007),O=n(25469);function d(t){const o={position:"relative",top:"auto",right:"auto",marginRight:0};return(0,l.jsx)("div",{style:{width:"100%",display:"flex",justifyContent:"flex-end",paddingBottom:"6px"},children:(0,l.jsx)(O.J,{onClick:t.onClick,style:t.style??o})})}const g=({timeZone:t,dataFrame:o,dataFrameFieldIndex:s,config:r,exemplarColor:e,clickedExemplarFieldIndex:a,setClickedExemplarFieldIndex:c})=>{const i=(0,Z.of)(f),[u,m]=(0,h.useState)(!1),[v,b]=(0,h.useState)(!1),y=[(0,N.UU)({fallbackAxisSideDirection:"end",crossAxis:!1,boundary:document.body}),(0,N.BN)()],{context:$,refs:j,floatingStyles:E}=(0,S.we)({open:u,placement:"bottom",onOpenChange:m,middleware:y,whileElementsMounted:Y.ll,strategy:"fixed"}),R=(0,S.s9)($),T=(0,S.Mk)($,{handleClose:(0,S.iB)(),enabled:a===void 0}),{getReferenceProps:U,getFloatingProps:A}=(0,S.bv)([R,T]);(0,h.useEffect)(()=>{a?.fieldIndex===s.fieldIndex&&a?.frameIndex===s.frameIndex||b(!1)},[a,s]);const F=()=>{const C=[(0,l.jsx)("rect",{fill:e,x:"3.38672",width:"4.78985",height:"4.78985",transform:"rotate(45 3.38672 0)"},"diamond"),(0,l.jsx)("path",{fill:e,d:"M1.94444 3.49988L0 5.44432L1.55552 6.99984L3.49996 5.05539L5.4444 6.99983L6.99992 5.44431L5.05548 3.49988L6.99983 1.55552L5.44431 0L3.49996 1.94436L1.5556 0L8.42584e-05 1.55552L1.94444 3.49988Z"},"x"),(0,l.jsx)("path",{fill:e,d:"M4 0L7.4641 6H0.535898L4 0Z"},"triangle"),(0,l.jsx)("rect",{fill:e,width:"5",height:"5"},"rectangle"),(0,l.jsx)("path",{fill:e,d:"M3 0.5L5.85317 2.57295L4.76336 5.92705H1.23664L0.146831 2.57295L3 0.5Z"},"pentagon"),(0,l.jsx)("path",{fill:e,d:"m2.35672,4.2425l0,2.357l1.88558,0l0,-2.357l2.3572,0l0,-1.88558l-2.3572,0l0,-2.35692l-1.88558,0l0,2.35692l-2.35672,0l0,1.88558l2.35672,0z"},"plus")];return C[s.frameIndex%C.length]},q=()=>{b(!0)},_=(0,h.useCallback)(()=>{const C=o.fields.filter(k=>k.config.links?.length&&k.config.links?.length>0)||[],et=[...C,...o.fields.filter(k=>!C.includes(k))],st=()=>{b(!1),m(!1),c(void 0)};let X=[],J=[];et.map((k,lt)=>{const B=k.values[s.fieldIndex];k.config.links?.length&&J?.push(...k.getLinks?.({valueRowIndex:s.fieldIndex})||[]);const nt=k.display?k.display(B):{text:`${B}`,numeric:+B};X.push({name:k.name,value:B,valueString:(0,D.cN)(nt),highlight:!1})});const ot={position:"relative",top:"35px",right:"5px",marginRight:0};return(0,l.jsxs)("div",{className:i.tooltip,ref:j.setFloating,style:E,...A(),children:[v&&(0,l.jsx)(d,{onClick:st,style:ot}),(0,l.jsx)(H.i,{displayValues:X,links:J})]})},[o.fields,s,i,v,c,E,A,j.setFloating]),tt=r.getSeries().find(C=>C.props.dataFrameFieldIndex?.frameIndex===s.frameIndex)?.props.lineColor,K=()=>{c(s),q()};return(0,l.jsxs)(l.Fragment,{children:[(0,l.jsx)("div",{ref:j.setReference,className:i.markerWrapper,"data-testid":I.Tp.components.DataSource.Prometheus.exemplarMarker,role:"button",tabIndex:0,...U(),onClick:K,onKeyDown:C=>{C.key==="Enter"&&K()},children:(0,l.jsx)("svg",{viewBox:"0 0 7 7",width:"7",height:"7",style:{fill:tt},className:(0,p.cx)(i.marble,(u||v)&&i.activeMarble),children:F()})}),(u||v)&&(0,l.jsx)(G.ZL,{children:_()})]})},f=t=>{const o=t.isDark?t.v1.palette.dark2:t.v1.palette.white,s=t.isDark?t.v1.palette.dark9:t.v1.palette.gray5,r=t.isDark?t.v1.palette.black:t.v1.palette.white,e=t.isDark?t.v1.palette.dark3:t.v1.palette.gray6;return{markerWrapper:(0,p.css)({padding:"0 4px 4px 4px",width:"8px",height:"8px",boxSizing:"content-box",transform:"translate3d(-50%, 0, 0)","&:hover":{"> svg":{transform:"scale(1.3)",opacity:1,filter:"drop-shadow(0 0 8px rgba(0, 0, 0, 0.5))"}}}),marker:(0,p.css)({width:0,height:0,borderLeft:"4px solid transparent",borderRight:"4px solid transparent",borderBottom:`4px solid ${t.v1.palette.red}`,pointerEvents:"none"}),wrapper:(0,p.css)({background:o,border:`1px solid ${s}`,borderRadius:t.shape.borderRadius(2),boxShadow:`0 0 20px ${r}`,padding:t.spacing(1)}),exemplarsTable:(0,p.css)({width:"100%","tr td":{padding:"5px 10px",whiteSpace:"nowrap",borderBottom:`4px solid ${t.components.panel.background}`},tr:{backgroundColor:t.colors.background.primary,"&:nth-child(even)":{backgroundColor:e}}}),valueWrapper:(0,p.css)({display:"flex",flexDirection:"row",flexWrap:"wrap",columnGap:t.spacing(1),"> span":{flexGrow:0},"> *":{flex:"1 1",alignSelf:"center"}}),tooltip:(0,p.css)({background:"none",padding:0,overflowY:"auto",maxHeight:"95vh"}),header:(0,p.css)({background:s,padding:"6px 10px",display:"flex"}),title:(0,p.css)({fontWeight:t.typography.fontWeightMedium,paddingRight:t.spacing(2),overflow:"hidden",display:"inline-block",whiteSpace:"nowrap",textOverflow:"ellipsis",flexGrow:1}),body:(0,p.css)({fontWeight:t.typography.fontWeightMedium,borderRadius:t.shape.borderRadius(2),overflow:"hidden"}),marble:(0,p.css)({display:"block",opacity:.5,[t.transitions.handleMotion("no-preference")]:{transition:"transform 0.15s ease-out"}}),activeMarble:(0,p.css)({transform:"scale(1.3)",opacity:1,filter:"drop-shadow(0 0 8px rgba(0, 0, 0, 0.5))"})}},x=({exemplars:t,timeZone:o,config:s,visibleSeries:r})=>{const e=(0,h.useRef)(),[a,c]=(0,h.useState)();(0,h.useLayoutEffect)(()=>{s.addHook("init",m=>{e.current=m})},[s]);const i=(0,h.useCallback)((m,v)=>{const b=m.fields.find(T=>T.name===P.LE),y=m.fields.find(T=>T.name===P.Bc);if(!b||!y||!e.current)return;const $=Object.keys(e.current.scales).find(T=>!["x","y"].some(U=>U===T))??V.s,j=e.current.scales[$].min,E=e.current.scales[$].max;let R=y.values[v.fieldIndex];return j!=null&&R<j&&(R=j),E!=null&&R>E&&(R=E),{x:e.current.valToPos(b.values[v.fieldIndex],"x"),y:e.current.valToPos(R,$)}},[]),u=(0,h.useCallback)((m,v)=>{const b=r!==void 0?w(r,m,v):!0,y=r!==void 0?L(m,v,r):void 0;return b?(0,l.jsx)(g,{setClickedExemplarFieldIndex:c,clickedExemplarFieldIndex:a,timeZone:o,dataFrame:m,dataFrameFieldIndex:v,config:s,exemplarColor:y}):(0,l.jsx)(l.Fragment,{})},[s,o,r,c,a]);return(0,l.jsx)(z.a,{config:s,id:"exemplars",events:t,renderEventMarker:u,mapEventToXYCoords:i})},M=(t,o)=>{const s=t.series.filter(e=>e.props.show),r=[];return o?.length&&s.forEach(e=>{const a=e.props?.dataFrameFieldIndex?.frameIndex,c=e.props?.dataFrameFieldIndex?.fieldIndex;if(a!==void 0&&c!==void 0){const i=o[a]?.fields[c];i?.labels&&r.push({labels:i.labels,color:e.props?.lineColor??""})}}),{labels:r,totalSeriesCount:t.series.length}},L=(t,o,s)=>{let r;return s.labels.some(e=>{const a=Object.keys(e.labels),c=t.fields.filter(i=>a.find(u=>u===i.name));return c.length&&c.every((u,m,v)=>{const b=u.values[o.fieldIndex];return e.labels[u.name]===b})?(r=e.color,!0):!1}),r},w=(t,o,s)=>{let r=!1;return t.labels.length===t.totalSeriesCount?r=!0:t.labels.some(e=>{const a=Object.keys(e.labels);if(Object.keys(e.labels).length===0)r=!0;else{const c=o.fields.filter(i=>a.find(u=>u===i.name));c.length&&(r=t.labels.some(i=>Object.keys(i.labels).every(u=>{const m=i.labels[u];return c.find(v=>v.values[s.fieldIndex]===m)})))}return r}),r}},59965:(Q,W,n)=>{n.d(W,{p:()=>O});var l=n(74848),h=n(96540),P=n(11261),V=n(83195),z=n(55129),p=n(32196),N=n(2543),S=n(55794),Y=n.n(S),D=n(40845);const I=({step:d,y:g,dragBounds:f,mapPositionToValue:x,formatValue:M,onChange:L})=>{const w=(0,D.$j)();let t=g,o="none";g<(f.top??0)&&(o="top"),g>(f.bottom??0)+22&&(o="bottom"),o==="bottom"&&(t=f.bottom??g),o==="top"&&(t=f.top??g);const s=typeof L!="function",r=(0,D.of)(Z,d,o,s),[e,a]=(0,h.useState)(d.value),c=(0,h.useMemo)(()=>w.colors.getContrastText(w.visualization.getColorByName(d.color)),[d.color,w]);return(0,l.jsx)(Y(),{axis:"y",grid:[1,1],disabled:s,onStop:s?N.noop:(i,u)=>(L(x(u.lastY)),!1),onDrag:(i,u)=>a(x(u.lastY)),position:{x:0,y:t},bounds:f,children:(0,l.jsx)("div",{className:r.handle,style:{color:c},children:(0,l.jsx)("span",{className:r.handleText,children:M(e)})})})};I.displayName="ThresholdDragHandle";const Z=(d,g,f,x)=>{const M=d.visualization.getColorByName(g.color),L=G(f),w=f!=="none";return{handle:(0,p.css)`
      display: flex;
      align-items: center;
      position: absolute;
      left: 0;
      width: calc(100% - 9px);
      height: 18px;
      margin-top: -9px;
      border-color: ${M};
      cursor: ${x?"initial":"grab"};
      border-top-right-radius: ${d.shape.radius.default};
      border-bottom-right-radius: ${d.shape.radius.default};
      ${w&&(0,p.css)`
        margin-top: 0;
        border-radius: ${d.shape.radius.default};
      `}
      background: ${M};
      font-size: ${d.typography.bodySmall.fontSize};
      &:before {
        ${L};
      }
    `,handleText:(0,p.css)`
      text-align: center;
      width: 100%;
      display: block;
      text-overflow: ellipsis;
      white-space: nowrap;
      overflow: hidden;
    `}};function G(d){const g=d==="none",f=x=>(0,p.css)`
    content: '';
    position: absolute;

    bottom: 0;
    top: 0;
    width: 0;
    height: 0;
    left: 0;

    border-right-style: solid;
    border-right-width: ${x}px;
    border-right-color: inherit;
    border-top: ${x}px solid transparent;
    border-bottom: ${x}px solid transparent;
  `;return g?(0,p.css)`
      ${f(9)};
      left: -9px;
    `:d==="top"?(0,p.css)`
      ${f(5)};
      left: calc(50% - 2.5px);
      top: -7px;
      transform: rotate(90deg);
    `:d==="bottom"?(0,p.css)`
      ${f(5)};
      left: calc(50% - 2.5px);
      top: calc(100% - 2.5px);
      transform: rotate(-90deg);
    `:""}const H=60,O=({config:d,fieldConfig:g,onThresholdsChange:f})=>{const x=(0,h.useRef)(),[M,L]=(0,h.useState)(0);(0,h.useLayoutEffect)(()=>{d.setPadding([0,H,0,0]),d.addHook("init",t=>{x.current=t}),d.addHook("draw",()=>{L(t=>t+1)})},[d]);const w=(0,h.useMemo)(()=>{const t=x.current;if(!t)return null;const o=g.defaults.thresholds;if(!o)return null;const s=(0,z.M)(g.defaults,P.PU.number),r=g.defaults.decimals,e=[];for(let a=0;a<o.steps.length;a++){const c=o.steps[a],i=t.valToPos(c.value,s);if(Number.isNaN(i)||!Number.isFinite(i))continue;const u=t.bbox.height/window.devicePixelRatio,v=typeof f=="function"?y=>{const $=[...o.steps.slice(0,a),...o.steps.slice(a+1),{...o.steps[a],value:y}].sort((j,E)=>j.value-E.value);f({...o,steps:$})}:void 0,b=(0,l.jsx)(I,{step:c,y:i,dragBounds:{top:0,bottom:u},mapPositionToValue:y=>t.posToVal(y,s),formatValue:y=>(0,V.j_)(s)(y,r).text,onChange:v},`${c.value}-${a}`);e.push(b)}return e},[M,g,f]);return x.current?(0,l.jsx)("div",{style:{position:"absolute",overflow:"visible",left:`${(x.current.bbox.left+x.current.bbox.width)/window.devicePixelRatio}px`,top:`${x.current.bbox.top/window.devicePixelRatio}px`,width:`${H}px`,height:`${x.current.bbox.height/window.devicePixelRatio}px`},children:w}):null};O.displayName="ThresholdControlsPlugin"}}]);

//# sourceMappingURL=6965.0c5239d653c3175969b3.js.map
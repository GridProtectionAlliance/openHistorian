"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[1166],{97186:(fe,V,s)=>{s.d(V,{$:()=>e,r:()=>l});const e={showLabels:"grafana.explore.logs.showLabels",showTime:"grafana.explore.logs.showTime",wrapLogMessage:"grafana.explore.logs.wrapLogMessage",prettifyLogMessage:"grafana.explore.logs.prettifyLogMessage",logsSortOrder:"grafana.explore.logs.sortOrder",logContextWrapLogMessage:"grafana.explore.logs.logContext.wrapLogMessage"},l="grafana.explore.logs.visualisationType"},81073:(fe,V,s)=>{s.d(V,{I:()=>r});var e=s(74848),l=s(32196),B=s(62930);const r=({adjective:w="newer"})=>{const $=`Loading ${w} logs...`;return(0,e.jsx)("div",{className:Z,children:(0,e.jsxs)("div",{children:[$," ",(0,e.jsx)(B.y,{inline:!0})]})})},Z=(0,l.css)`
  display: flex;
  justify-content: center;
`},93463:(fe,V,s)=>{s.d(V,{A:()=>N,h:()=>$});var e=s(74848),l=s(32196),B=s(96540),r=s(40845),Z=s(56034);const w=["detected_level","level","lvl","filename"],$=(0,B.memo)(({labels:f,emptyMessage:L})=>{const h=(0,r.of)(E),b=(0,B.useMemo)(()=>Object.keys(f).filter(g=>!g.startsWith("_")&&!w.includes(g)).sort(),[f]);return b.length===0&&L?(0,e.jsx)("span",{className:(0,l.cx)([h.logsLabels]),children:(0,e.jsx)("span",{className:(0,l.cx)([h.logsLabel]),children:L})}):(0,e.jsx)("span",{className:(0,l.cx)([h.logsLabels]),children:b.map(g=>{const T=f[g];if(!T)return;const j=`${g}=${T}`;return(0,e.jsx)(Z.m,{content:j,placement:"top",children:(0,e.jsx)(n,{styles:h,children:j})},g)})})});$.displayName="LogLabels";const N=(0,B.memo)(({labels:f})=>{const L=(0,r.of)(E);return(0,e.jsx)("span",{className:(0,l.cx)([L.logsLabels]),children:f.map(h=>(0,e.jsx)(n,{styles:L,tooltip:h,children:h},h))})});N.displayName="LogLabelsList";const n=(0,B.forwardRef)(({styles:f,tooltip:L,children:h},b)=>(0,e.jsx)("span",{className:(0,l.cx)([f.logsLabel]),ref:b,children:(0,e.jsx)("span",{className:(0,l.cx)([f.logsLabelValue]),title:L,children:h})}));n.displayName="LogLabel";const E=f=>({logsLabels:(0,l.css)`
      display: flex;
      flex-wrap: wrap;
      font-size: ${f.typography.size.xs};
    `,logsLabel:(0,l.css)`
      label: logs-label;
      display: flex;
      padding: ${f.spacing(0,.25)};
      background-color: ${f.colors.background.secondary};
      border-radius: ${f.shape.radius.default};
      margin: ${f.spacing(.125,.5,0,0)};
      text-overflow: ellipsis;
      white-space: nowrap;
      overflow: hidden;
    `,logsLabelValue:(0,l.css)`
      label: logs-label__value;
      display: inline-block;
      max-width: ${f.spacing(25)};
      text-overflow: ellipsis;
      overflow: hidden;
    `})},85492:(fe,V,s)=>{s.d(V,{$:()=>E});var e=s(74848),l=s(88838),B=s(96540),r=s(23257),Z=s.n(r),w=s(21),$=s(40845);function N(f,L){return L.split(/;\s*/).reduce((h,b)=>{if(b==="color:rgba(0,0,0,0.5)")return{color:f.colors.text.secondary};const g=b.match(/([^:\s]+)\s*:\s*(.+)/);if(g&&g[1]&&g[2]){const T=g[1].replace(/-([a-z])/g,(j,q)=>q.toUpperCase());h[T]=g[2]}return h},{})}class n extends B.PureComponent{constructor(){super(...arguments),this.state={chunks:[],prevValue:""}}static getDerivedStateFromProps(L,h){return L.value===h.prevValue?null:{chunks:l.Ay.parse(L.value).spans.map(g=>g.css?{style:N(L.theme,g.css),text:g.text}:{text:g.text}),prevValue:L.value}}render(){const{chunks:L}=this.state;return L.map((h,b)=>{const g=this.props.highlight?.searchWords?(0,e.jsx)(Z(),{textToHighlight:h.text,searchWords:this.props.highlight.searchWords,findChunks:w.Oq,highlightClassName:this.props.highlight.highlightClassName},b):h.text;return h.style?(0,e.jsx)("span",{style:h.style,"data-testid":"ansiLogLine",children:g},b):g})}}const E=(0,$.cV)(n);E.displayName="LogMessageAnsi"},42785:(fe,V,s)=>{s.d(V,{Q:()=>n});var e=s(74848),l=s(96540),B=s(76885),r=s(71733),Z=s(43127),w=s(29158),$=s(10534),N=s(2913);const n=(0,l.memo)(({logText:E,onOpenContext:f,onPermalinkClick:L,onPinLine:h,onUnpinLine:b,pinLineButtonTooltipTitle:g,pinned:T,row:j,showContextToggle:q,styles:ae,mouseIsOver:ce,onBlur:de,getRowContextQuery:Q})=>{const he=q?q(j):!1,ge=(0,l.useCallback)(X=>{X.stopPropagation()},[]),be=(0,l.useCallback)(async X=>{if(X.stopPropagation(),Q&&(X.nativeEvent.ctrlKey||X.nativeEvent.metaKey||X.nativeEvent.shiftKey)){const pe=window.open("about:blank"),ue=await Q(j,void 0,!1);if(ue&&pe){const me=B.kM.renderUrl(r.I.assureBaseUrl(`${(0,N.zj)().appSubUrl}explore`),{left:JSON.stringify({datasource:ue.datasource,queries:[ue],range:(0,Z.E2)()})});pe.location=me;return}pe?.close()}f(j)},[f,Q,j]),ye=(0,l.useCallback)(X=>{!X.currentTarget.contains(X.relatedTarget)&&de&&de()},[de]),le=(0,l.useCallback)(()=>E,[E]);return(0,e.jsxs)("span",{className:`log-row-menu ${ae.rowMenu}`,onClick:ge,onBlur:ye,children:[T&&!ce&&(0,e.jsx)(w.K,{className:ae.unPinButton,size:"md",name:"gf-pin",onClick:()=>b&&b(j),tooltip:"Unpin line",tooltipPlacement:"top","aria-label":"Unpin line",tabIndex:0}),ce&&(0,e.jsxs)(e.Fragment,{children:[he&&(0,e.jsx)(w.K,{size:"md",name:"gf-show-context",onClick:be,tooltip:"Show context",tooltipPlacement:"top","aria-label":"Show context",tabIndex:0}),(0,e.jsx)($.b,{className:ae.copyLogButton,icon:"copy",variant:"secondary",fill:"text",size:"md",getText:le,tooltip:"Copy to clipboard",tooltipPlacement:"top",tabIndex:0}),T&&b&&(0,e.jsx)(w.K,{className:ae.unPinButton,size:"md",name:"gf-pin",onClick:()=>b&&b(j),tooltip:"Unpin line",tooltipPlacement:"top","aria-label":"Unpin line",tabIndex:0}),!T&&h&&(0,e.jsx)(w.K,{className:ae.unPinButton,size:"md",name:"gf-pin",onClick:()=>h&&h(j),tooltip:g??"Pin line",tooltipPlacement:"top","aria-label":"Pin line",tabIndex:0}),L&&j.rowId!==void 0&&j.uid&&(0,e.jsx)(w.K,{tooltip:"Copy shortlink","aria-label":"Copy shortlink",tooltipPlacement:"top",size:"md",name:"share-alt",onClick:()=>L(j),tabIndex:0})]})]})});n.displayName="LogRowMenuCell"},40455:(fe,V,s)=>{s.d(V,{S:()=>N,a:()=>f});var e=s(74848),l=s(96540),B=s(23257),r=s.n(B),Z=s(21),w=s(85492),$=s(42785);const N=1e5,n=({hasAnsi:L,entry:h,highlights:b,styles:g})=>{const T=b&&b.length>0&&b[0]&&b[0].length>0&&h.length<N,j=b??[];if(L){const q=T?{searchWords:j,highlightClassName:g.logsRowMatchHighLight}:void 0;return(0,e.jsx)(w.$,{value:h,highlight:q})}else if(T)return(0,e.jsx)(r(),{textToHighlight:h,searchWords:j,findChunks:Z.Oq,highlightClassName:g.logsRowMatchHighLight});return(0,e.jsx)(e.Fragment,{children:h})},E=(L,h,b,g)=>{if(h)try{return JSON.stringify(JSON.parse(L),void 0,2)}catch{}return!b&&!g&&(L=L.replace(/(\r\n|\n|\r)/g,"")),L},f=(0,l.memo)(L=>{const{row:h,wrapLogMessage:b,prettifyLogMessage:g,showContextToggle:T,styles:j,onOpenContext:q,onPermalinkClick:ae,onUnpinLine:ce,onPinLine:de,pinLineButtonTooltipTitle:Q,pinned:he,mouseIsOver:ge,onBlur:be,getRowContextQuery:ye,expanded:le}=L,{hasAnsi:X,raw:pe}=h,ue=(0,l.useMemo)(()=>E(pe,g,b,!!le),[pe,g,b,le]),me=(0,l.useMemo)(()=>ge||he,[ge,he]);return(0,e.jsxs)(e.Fragment,{children:[(0,e.jsx)("td",{className:j.logsRowMessage,children:(0,e.jsx)("div",{className:b?j.positionRelative:j.horizontalScroll,children:(0,e.jsx)("button",{className:`${j.logLine} ${j.positionRelative}`,children:(0,e.jsx)(n,{hasAnsi:X,entry:ue,highlights:h.searchWords,styles:j})})})}),(0,e.jsx)("td",{className:`log-row-menu-cell ${j.logRowMenuCell}`,children:me&&(0,e.jsx)($.Q,{logText:ue,row:h,showContextToggle:T,getRowContextQuery:ye,onOpenContext:q,onPermalinkClick:ae,onPinLine:de,onUnpinLine:ce,pinLineButtonTooltipTitle:Q,pinned:he,styles:j,mouseIsOver:ge,onBlur:be})})]})});f.displayName="LogRowMessage"},14647:(fe,V,s)=>{s.d(V,{k:()=>Pe});var e=s(74848),l=s(32196),B=s(41811),r=s(96540),Z=s(52622),w=s(32264),$=s(40845),N=s(14110),n=s(38138),E=s(91002);const f=({x:d,y:o,onClickFilterString:t,onClickFilterOutString:i,selection:a,row:x,close:v})=>{const y=(0,r.useRef)(null),A=(0,$.of)(h);return(0,r.useEffect)(()=>{function S(k){k.key==="Escape"&&v()}return document.addEventListener("keyup",S),()=>{document.removeEventListener("keyup",S)}},[v]),t||i?(0,e.jsx)("div",{className:A.menu,style:{top:o,left:d},children:(0,e.jsxs)(n.W,{ref:y,children:[(0,e.jsx)(n.W.Item,{label:"Copy selection",onClick:()=>{(0,E.Dk)(a,y),v(),L("copy",a.length,x.datasourceType)}}),t&&(0,e.jsx)(n.W.Item,{label:"Add as line contains filter",onClick:()=>{t(a,x.dataFrame.refId),v(),L("line_contains",a.length,x.datasourceType)}}),i&&(0,e.jsx)(n.W.Item,{label:"Add as line does not contain filter",onClick:()=>{i(a,x.dataFrame.refId),v(),L("line_does_not_contain",a.length,x.datasourceType)}})]})}):null};function L(d,o,t){(0,N.rR)("grafana_explore_logs_popover_menu",{action:d,selectionLength:o,datasourceType:t||"unknown"})}const h=d=>({menu:(0,l.css)({position:"fixed",zIndex:d.zIndex.modal})});class b{constructor(){this.seen=new Set,this.count=0}getKey(o){this.count+=1;const t=`k_${o}`;return this.seen.has(t)?`i_${this.count}`:(this.seen.add(t),t)}}var g=s(2543),T=s(72724),j=s(56034),q=s(14578),ae=s(41260),ce=s(41987),de=s(10534),Q=s(29158),he=s(14689),ge=s(69147),be=s(3911);const ye=d=>({logsStatsRow:(0,l.css)`
    label: logs-stats-row;
    margin: ${parseInt(d.spacing(2),10)/1.75}px 0;
  `,logsStatsRowActive:(0,l.css)`
    label: logs-stats-row--active;
    color: ${d.colors.primary.text};
    position: relative;
  `,logsStatsRowLabel:(0,l.css)`
    label: logs-stats-row__label;
    display: flex;
    margin-bottom: 1px;
  `,logsStatsRowValue:(0,l.css)`
    label: logs-stats-row__value;
    flex: 1;
    text-overflow: ellipsis;
    overflow: hidden;
  `,logsStatsRowCount:(0,l.css)`
    label: logs-stats-row__count;
    text-align: right;
    margin-left: ${d.spacing(.75)};
  `,logsStatsRowPercent:(0,l.css)`
    label: logs-stats-row__percent;
    text-align: right;
    margin-left: ${d.spacing(.75)};
    width: ${d.spacing(4.5)};
  `,logsStatsRowBar:(0,l.css)`
    label: logs-stats-row__bar;
    height: ${d.spacing(.5)};
    overflow: hidden;
    background: ${d.colors.text.disabled};
  `,logsStatsRowInnerBar:(0,l.css)`
    label: logs-stats-row__innerbar;
    height: ${d.spacing(.5)};
    overflow: hidden;
    background: ${d.colors.primary.main};
  `}),le=({active:d,count:o,proportion:t,value:i})=>{const a=(0,$.of)(ye),x=`${Math.round(t*100)}%`,v={width:x},y=d?(0,l.cx)([a.logsStatsRow,a.logsStatsRowActive]):(0,l.cx)([a.logsStatsRow]);return(0,e.jsxs)("div",{className:y,children:[(0,e.jsxs)("div",{className:(0,l.cx)([a.logsStatsRowLabel]),children:[(0,e.jsx)("div",{className:(0,l.cx)([a.logsStatsRowValue]),title:i,children:i}),(0,e.jsx)("div",{className:(0,l.cx)([a.logsStatsRowCount]),children:o}),(0,e.jsx)("div",{className:(0,l.cx)([a.logsStatsRowPercent]),children:x})]}),(0,e.jsx)("div",{className:(0,l.cx)([a.logsStatsRowBar]),children:(0,e.jsx)("div",{className:(0,l.cx)([a.logsStatsRowInnerBar]),style:v})})]})};le.displayName="LogLabelStatsRow";const X=5,pe=(0,be.N)(d=>({logsStats:(0,l.css)`
      label: logs-stats;
      background: inherit;
      color: ${d.colors.text.primary};
      word-break: break-all;
      width: fit-content;
      max-width: 100%;
    `,logsStatsHeader:(0,l.css)`
      label: logs-stats__header;
      border-bottom: 1px solid ${d.colors.border.medium};
      display: flex;
    `,logsStatsTitle:(0,l.css)`
      label: logs-stats__title;
      font-weight: ${d.typography.fontWeightMedium};
      padding-right: ${d.spacing(2)};
      display: inline-block;
      white-space: nowrap;
      text-overflow: ellipsis;
      flex-grow: 1;
    `,logsStatsClose:(0,l.css)`
      label: logs-stats__close;
      cursor: pointer;
    `,logsStatsBody:(0,l.css)`
      label: logs-stats__body;
      padding: 5px 0px;
    `}));class ue extends r.PureComponent{render(){const{label:o,rowCount:t,stats:i,value:a,theme:x,isLabel:v}=this.props,y=pe(x),A=i.slice(0,X);let _=A.find(p=>p.value===a),S=i.slice(X);const k=!_;k&&(_=S.find(p=>p.value===a),S=S.filter(p=>p.value!==a));const O=S.reduce((p,z)=>p+z.count,0),W=A.reduce((p,z)=>p+z.count,0)+O,F=O/W;return(0,e.jsxs)("div",{className:y.logsStats,"data-testid":"logLabelStats",children:[(0,e.jsx)("div",{className:y.logsStatsHeader,children:(0,e.jsxs)("div",{className:y.logsStatsTitle,children:[o,": ",W," of ",t," rows have that ",v?"label":"field"]})}),(0,e.jsxs)("div",{className:y.logsStatsBody,children:[A.map(p=>(0,e.jsx)(le,{...p,active:p.value===a},p.value)),k&&_&&(0,e.jsx)(le,{..._,active:!0},_.value),O>0&&(0,e.jsx)(le,{count:O,value:"Other",proportion:F},"__OTHERS__")]})]})}}const me=(0,$.cV)(ue);me.displayName="LogLabelStats";var Se=s(77020);const c=(0,B.A)(d=>({wordBreakAll:(0,l.css)`
      label: wordBreakAll;
      word-break: break-all;
    `,copyButton:(0,l.css)`
      & > button {
        color: ${d.colors.text.secondary};
        padding: 0;
        justify-content: center;
        border-radius: ${d.shape.radius.circle};
        height: ${d.spacing(d.components.height.sm)};
        width: ${d.spacing(d.components.height.sm)};
        svg {
          margin: 0;
        }

        span > div {
          top: -5px;
          & button {
            color: ${d.colors.success.main};
          }
        }
      }
    `,adjoiningLinkButton:(0,l.css)`
      margin-left: ${d.spacing(1)};
    `,wrapLine:(0,l.css)`
      label: wrapLine;
      white-space: pre-wrap;
    `,logDetailsStats:(0,l.css)`
      padding: 0 ${d.spacing(1)};
    `,logDetailsValue:(0,l.css)`
      display: flex;
      align-items: center;
      line-height: 22px;

      .log-details-value-copy {
        visibility: hidden;
      }
      &:hover {
        .log-details-value-copy {
          visibility: visible;
        }
      }
    `,buttonRow:(0,l.css)`
      display: flex;
      flex-direction: row;
      gap: ${d.spacing(.5)};
      margin-left: ${d.spacing(.5)};
    `}));class J extends r.PureComponent{constructor(){super(...arguments),this.state={showFieldsStats:!1,fieldCount:0,fieldStats:null},this.showField=()=>{const{onClickShowField:o,parsedKeys:t,row:i}=this.props;o&&o(t[0]),(0,N.rR)("grafana_explore_logs_log_details_replace_line_clicked",{datasourceType:i.datasourceType,logRowUid:i.uid,type:"enable"})},this.hideField=()=>{const{onClickHideField:o,parsedKeys:t,row:i}=this.props;o&&o(t[0]),(0,N.rR)("grafana_explore_logs_log_details_replace_line_clicked",{datasourceType:i.datasourceType,logRowUid:i.uid,type:"disable"})},this.isFilterLabelActive=async()=>{const{isFilterLabelActive:o,parsedKeys:t,parsedValues:i,row:a}=this.props;return o?await o(t[0],i[0],a.dataFrame?.refId):!1},this.filterLabel=()=>{const{onClickFilterLabel:o,parsedKeys:t,parsedValues:i,row:a}=this.props;o&&o(t[0],i[0],(0,ge.f8)(a)||void 0),(0,N.rR)("grafana_explore_logs_log_details_filter_clicked",{datasourceType:a.datasourceType,filterType:"include",logRowUid:a.uid})},this.filterOutLabel=()=>{const{onClickFilterOutLabel:o,parsedKeys:t,parsedValues:i,row:a}=this.props;o&&o(t[0],i[0],(0,ge.f8)(a)||void 0),(0,N.rR)("grafana_explore_logs_log_details_filter_clicked",{datasourceType:a.datasourceType,filterType:"exclude",logRowUid:a.uid})},this.updateStats=()=>{const{getStats:o}=this.props,t=o(),i=t?t.reduce((a,x)=>a+x.count,0):0;(!(0,g.isEqual)(this.state.fieldStats,t)||i!==this.state.fieldCount)&&this.setState({fieldStats:t,fieldCount:i})},this.showStats=()=>{const{isLabel:o,row:t,app:i}=this.props,{showFieldsStats:a}=this.state;a||this.updateStats(),this.toggleFieldsStats(),(0,N.rR)("grafana_explore_logs_log_details_stats_clicked",{dataSourceType:t.datasourceType,fieldType:o?"label":"detectedField",type:a?"close":"open",logRowUid:t.uid,app:i})}}componentDidUpdate(){this.state.showFieldsStats&&this.updateStats()}toggleFieldsStats(){this.setState(o=>({showFieldsStats:!o.showFieldsStats}))}generateClipboardButton(o){const{theme:t}=this.props,i=c(t);return(0,e.jsx)("div",{className:`log-details-value-copy ${i.copyButton}`,children:(0,e.jsx)(de.b,{getText:()=>o,title:"Copy value to clipboard",fill:"text",variant:"secondary",icon:"copy",size:"md"})})}generateMultiVal(o,t){return(0,e.jsx)("table",{children:(0,e.jsx)("tbody",{children:o?.map((i,a)=>(0,e.jsx)("tr",{children:(0,e.jsxs)("td",{children:[i,t&&i!==""&&this.generateClipboardButton(i)]})},`${i}-${a}`))})})}render(){const{theme:o,parsedKeys:t,parsedValues:i,isLabel:a,links:x,displayedFields:v,wrapLogMessage:y,onClickFilterLabel:A,onClickFilterOutLabel:_,disableActions:S,row:k,app:O,onPinLine:U,pinLineButtonTooltipTitle:W}=this.props,{showFieldsStats:F,fieldStats:p,fieldCount:z}=this.state,K=c(o),R=(0,Se.D)(o),ee=t==null?!1:t.length===1,I=i==null?!1:i.length===1,H=!S&&A&&_,te=O===ce.Jk.Explore&&k.dataFrame?.refId?` in query ${k.dataFrame?.refId}`:"",ie=!I&&i!=null&&!i.every(M=>M===""),u=v&&t!=null&&v.includes(t[0])?(0,e.jsx)(Q.K,{variant:"primary",tooltip:"Hide this field",name:"eye",onClick:this.hideField}):(0,e.jsx)(Q.K,{tooltip:"Show this field instead of the message",name:"eye",onClick:this.showField});return(0,e.jsxs)(e.Fragment,{children:[(0,e.jsxs)("tr",{className:R.logDetailsValue,children:[(0,e.jsx)("td",{className:R.logsDetailsIcon,children:(0,e.jsxs)("div",{className:K.buttonRow,children:[H&&(0,e.jsxs)(e.Fragment,{children:[(0,e.jsx)(se,{name:"search-plus",onClick:this.filterLabel,isActive:()=>this.isFilterLabelActive(),tooltipSuffix:te}),(0,e.jsx)(Q.K,{name:"search-minus",tooltip:`Filter out value${te}`,onClick:this.filterOutLabel})]}),!S&&v&&u,!S&&(0,e.jsx)(Q.K,{variant:F?"primary":"secondary",name:"signal",tooltip:"Ad-hoc statistics",className:"stats-button",disabled:!ee,onClick:this.showStats})]})}),(0,e.jsx)("td",{className:R.logDetailsLabel,children:ee?t[0]:this.generateMultiVal(t)}),(0,e.jsx)("td",{className:(0,l.cx)(K.wordBreakAll,y&&K.wrapLine),children:(0,e.jsxs)("div",{className:K.logDetailsValue,children:[I?i[0]:this.generateMultiVal(i,!0),I&&this.generateClipboardButton(i[0]),(0,e.jsx)("div",{className:(0,l.cx)((I||ie)&&K.adjoiningLinkButton),children:x?.map((M,C)=>{if(M.onClick&&U){const P=M.onClick;M.onClick=(G,m)=>{U(k,!1),P(G,m)}}return(0,e.jsx)("span",{children:(0,e.jsx)(he.R,{buttonProps:{tooltip:typeof W=="object"&&M.onClick?W:void 0},link:M})},`${M.title}-${C}`)})})]})})]}),F&&ee&&I&&(0,e.jsxs)("tr",{children:[(0,e.jsx)("td",{children:(0,e.jsx)(Q.K,{variant:F?"primary":"secondary",name:"signal",tooltip:"Hide ad-hoc statistics",onClick:this.showStats})}),(0,e.jsx)("td",{colSpan:2,children:(0,e.jsx)("div",{className:K.logDetailsStats,children:(0,e.jsx)(me,{stats:p,label:t[0],value:i[0],rowCount:z,isLabel:a})})})]})]})}}const se=({isActive:d,tooltipSuffix:o,...t})=>{const[i,a]=(0,r.useState)(!1),x=i?"Remove filter":"Filter for value";return(0,r.useEffect)(()=>{d().then(a)},[d]),(0,e.jsx)(Q.K,{...t,variant:i?"primary":void 0,tooltip:x+o})},ne=(0,$.cV)(J);ne.displayName="LogDetailsRow";var we=s(70377);class ve extends r.PureComponent{render(){const{app:o,row:t,theme:i,hasError:a,onClickFilterOutLabel:x,onClickFilterLabel:v,getRows:y,showDuplicates:A,className:_,onClickShowField:S,onClickHideField:k,displayedFields:O,getFieldLinks:U,wrapLogMessage:W,onPinLine:F,styles:p,pinLineButtonTooltipTitle:z}=this.props,K=(0,Se.L)(i,t.logLevel),R=t.labels?t.labels:{},ee=Object.keys(R).length>0,I=(0,we.jE)(t,U);let H=I.filter(m=>m.links?.length);const te=H.filter(m=>m.fieldIndex!==t.entryFieldIndex).sort(),ie=H.filter(m=>m.fieldIndex===t.entryFieldIndex).sort(),u=(0,we.qE)(ie),M=te&&te.length>0||u&&u.length>0,C=t.dataFrame.meta?.type===ae.m.LogLines?[]:I.filter(m=>m.links?.length===0&&m.fieldIndex!==t.entryFieldIndex).sort(),P=C&&C.length>0,G=a?"":`${K.logsRowLevelColor} ${p.logsRowLevel} ${p.logsRowLevelDetails}`;return(0,e.jsxs)("tr",{className:(0,l.cx)(_,p.logDetails),children:[A&&(0,e.jsx)("td",{}),(0,e.jsx)("td",{className:G,"aria-label":"Log level"}),(0,e.jsx)("td",{colSpan:4,children:(0,e.jsx)("div",{className:p.logDetailsContainer,children:(0,e.jsx)("table",{className:p.logDetailsTable,children:(0,e.jsxs)("tbody",{children:[(ee||P)&&(0,e.jsx)("tr",{children:(0,e.jsx)("td",{colSpan:100,className:p.logDetailsHeading,"aria-label":"Fields",children:"Fields"})}),Object.keys(R).sort().map((m,Y)=>{const D=R[m];return(0,e.jsx)(ne,{parsedKeys:[m],parsedValues:[D],isLabel:!0,getStats:()=>(0,E.Tj)(y(),m),onClickFilterOutLabel:x,onClickFilterLabel:v,onClickShowField:S,onClickHideField:k,row:t,app:o,wrapLogMessage:W,displayedFields:O,disableActions:!1,isFilterLabelActive:this.props.isFilterLabelActive},`${m}=${D}-${Y}`)}),C.map((m,Y)=>{const{keys:D,values:re,fieldIndex:Le}=m;return(0,e.jsx)(ne,{parsedKeys:D,parsedValues:re,onClickShowField:S,onClickHideField:k,onClickFilterOutLabel:x,onClickFilterLabel:v,getStats:()=>(0,E.s2)(t.dataFrame.fields[Le].values),displayedFields:O,wrapLogMessage:W,row:t,app:o,disableActions:!1,isFilterLabelActive:this.props.isFilterLabelActive},`${D[0]}=${re[0]}-${Y}`)}),M&&(0,e.jsx)("tr",{children:(0,e.jsx)("td",{colSpan:100,className:p.logDetailsHeading,"aria-label":"Data Links",children:"Links"})}),te.map((m,Y)=>{const{keys:D,values:re,links:Le,fieldIndex:je}=m;return(0,e.jsx)(ne,{parsedKeys:D,parsedValues:re,links:Le,onClickShowField:S,onClickHideField:k,onPinLine:F,pinLineButtonTooltipTitle:z,getStats:()=>(0,E.s2)(t.dataFrame.fields[je].values),displayedFields:O,wrapLogMessage:W,row:t,app:o,disableActions:!1},`${D[0]}=${re[0]}-${Y}`)}),u?.map((m,Y)=>{const{keys:D,values:re,links:Le,fieldIndex:je}=m;return(0,e.jsx)(ne,{parsedKeys:D,parsedValues:re,links:Le,onClickShowField:S,onClickHideField:k,onPinLine:F,pinLineButtonTooltipTitle:z,getStats:()=>(0,E.s2)(t.dataFrame.fields[je].values),displayedFields:O,wrapLogMessage:W,row:t,app:o,disableActions:!0},`${D[0]}=${re[0]}-${Y}`)}),!P&&!ee&&!M&&(0,e.jsx)("tr",{children:(0,e.jsx)("td",{colSpan:100,"aria-label":"No details",children:"No details available"})})]})})})})]})}}const Re=(0,$.cV)(ve);Re.displayName="LogDetails";var Me=s(93463),$e=s(40455),Te=s(42785);const Ee=(0,r.memo)(d=>{const{row:o,detectedFields:t,getFieldLinks:i,wrapLogMessage:a,styles:x,mouseIsOver:v,pinned:y,...A}=d,_=a?"":ke.noWrap,S=(0,r.useMemo)(()=>(0,we.jE)(o,i),[i,o]),k=(0,r.useMemo)(()=>{let U="";for(let W=0;W<t.length;W++){const F=t[W],p=S.find(z=>{const{keys:K}=z;return K[0]===F});p&&(U+=` ${F}=${p.values}`),o.labels[F]!==void 0&&o.labels[F]!==null&&(U+=` ${F}=${o.labels[F]}`)}return U.trimStart()},[t,S,o.labels]),O=(0,r.useMemo)(()=>v||y,[v,y]);return(0,e.jsxs)(e.Fragment,{children:[(0,e.jsx)("td",{className:x.logsRowMessage,children:(0,e.jsx)("div",{className:_,children:k})}),(0,e.jsx)("td",{className:`log-row-menu-cell ${x.logRowMenuCell}`,children:O&&(0,e.jsx)(Te.Q,{logText:k,row:o,styles:x,pinned:y,mouseIsOver:v,...A})})]})}),ke={noWrap:(0,l.css)`
    white-space: nowrap;
  `};Ee.displayName="LogRowMessageDisplayedFields";class Ie extends r.PureComponent{constructor(o){super(o),this.state={permalinked:!1,showingContext:!1,showDetails:!1,mouseIsOver:!1},this.debouncedContextClose=(0,g.debounce)(()=>{this.setState({showingContext:!1})},3e3),this.onOpenContext=t=>{this.setState({showingContext:!0}),this.props.onOpenContext(t,this.debouncedContextClose)},this.onRowClick=t=>{this.props.handleTextSelection?.(t,this.props.row)||this.props.enableLogDetails&&this.setState(i=>({showDetails:!i.showDetails}))},this.onMouseEnter=()=>{this.setState({mouseIsOver:!0}),this.props.onLogRowHover&&this.props.onLogRowHover(this.props.row)},this.onMouseMove=t=>{this.props.handleTextSelection&&document.getSelection()?.toString()&&t.buttons>0&&this.setState({mouseIsOver:!1})},this.onMouseLeave=()=>{this.setState({mouseIsOver:!1})},this.scrollToLogRow=(t,i=!1)=>{const{row:a,permalinkedRowId:x,scrollIntoView:v,containerRendered:y}=this.props;if(x!==a.uid){(t.permalinked||i)&&this.setState({permalinked:!1});return}!this.state.permalinked&&y&&this.logLineRef.current&&v&&(v(this.logLineRef.current),(0,N.rR)("grafana_explore_logs_permalink_opened",{datasourceType:a.datasourceType??"unknown",logRowUid:a.uid}),this.setState({permalinked:!0}))},this.escapeRow=(0,B.A)((t,i)=>t.hasUnescapedContent&&i?{...t,entry:(0,E.Hw)(t.entry),raw:(0,E.Hw)(t.raw)}:t),this.logLineRef=r.createRef()}renderTimeStamp(o){return(0,T.LE)(o,{timeZone:this.props.timeZone,defaultWithMS:!0})}componentDidMount(){this.scrollToLogRow(this.state,!0)}componentDidUpdate(o,t){this.scrollToLogRow(t)}render(){const{getRows:o,onClickFilterLabel:t,onClickFilterOutLabel:i,onClickShowField:a,onClickHideField:x,enableLogDetails:v,row:y,showDuplicates:A,showContextToggle:_,showLabels:S,showTime:k,displayedFields:O,wrapLogMessage:U,prettifyLogMessage:W,theme:F,getFieldLinks:p,forceEscape:z,app:K,styles:R,getRowContextQuery:ee,pinned:I}=this.props,{showDetails:H,showingContext:te,permalinked:ie}=this.state,u=(0,Se.L)(F,y.logLevel),{errorMessage:M,hasError:C}=(0,E.Ei)(y),{sampleMessage:P,isSampled:G}=(0,E.qR)(y),m=(0,l.cx)(R.logsRow,{[R.errorLogRow]:C,[R.highlightBackground]:te||ie||I}),Y=(0,l.cx)(R.logsRow,{[R.errorLogRow]:C,[R.highlightBackground]:ie&&!this.state.showDetails}),D=this.escapeRow(y,z);return(0,e.jsxs)(e.Fragment,{children:[(0,e.jsxs)("tr",{ref:this.logLineRef,className:m,onClick:this.onRowClick,onMouseEnter:this.onMouseEnter,onMouseLeave:this.onMouseLeave,onMouseMove:this.onMouseMove,onFocus:this.onMouseEnter,children:[A&&(0,e.jsx)("td",{className:R.logsRowDuplicates,children:D.duplicates&&D.duplicates>0?`${D.duplicates+1}x`:null}),(0,e.jsxs)("td",{className:C||G?R.logsRowWithError:`${u.logsRowLevelColor} ${R.logsRowLevel}`,children:[C&&(0,e.jsx)(j.m,{content:`Error: ${M}`,placement:"right",theme:"error",children:(0,e.jsx)(q.I,{className:R.logIconError,name:"exclamation-triangle",size:"xs"})}),G&&(0,e.jsx)(j.m,{content:`${P}`,placement:"right",theme:"info",children:(0,e.jsx)(q.I,{className:R.logIconInfo,name:"info-circle",size:"xs"})})]}),(0,e.jsx)("td",{title:v?H?"Hide log details":"See log details":"",className:v?R.logsRowToggleDetails:"",children:v&&(0,e.jsx)(q.I,{className:R.topVerticalAlign,name:H?"angle-down":"angle-right"})}),k&&(0,e.jsx)("td",{className:R.logsRowLocalTime,children:this.renderTimeStamp(y.timeEpochMs)}),S&&D.uniqueLabels&&(0,e.jsx)("td",{className:R.logsRowLabels,children:(0,e.jsx)(Me.h,{labels:D.uniqueLabels})}),O&&O.length>0?(0,e.jsx)(Ee,{row:D,showContextToggle:_,detectedFields:O,getFieldLinks:p,wrapLogMessage:U,onOpenContext:this.onOpenContext,onPermalinkClick:this.props.onPermalinkClick,styles:R,onPinLine:this.props.onPinLine,onUnpinLine:this.props.onUnpinLine,pinned:this.props.pinned,mouseIsOver:this.state.mouseIsOver,onBlur:this.onMouseLeave}):(0,e.jsx)($e.a,{row:D,showContextToggle:_,getRowContextQuery:ee,wrapLogMessage:U,prettifyLogMessage:W,onOpenContext:this.onOpenContext,onPermalinkClick:this.props.onPermalinkClick,app:K,styles:R,onPinLine:this.props.onPinLine,onUnpinLine:this.props.onUnpinLine,pinLineButtonTooltipTitle:this.props.pinLineButtonTooltipTitle,pinned:this.props.pinned,mouseIsOver:this.state.mouseIsOver,onBlur:this.onMouseLeave,expanded:this.state.showDetails})]}),this.state.showDetails&&(0,e.jsx)(Re,{onPinLine:this.props.onPinLine,className:Y,showDuplicates:A,getFieldLinks:p,onClickFilterLabel:t,onClickFilterOutLabel:i,onClickShowField:a,onClickHideField:x,getRows:o,row:D,wrapLogMessage:U,hasError:C,displayedFields:O,app:K,styles:R,isFilterLabelActive:this.props.isFilterLabelActive,pinLineButtonTooltipTitle:this.props.pinLineButtonTooltipTitle})]})}}const Ce=(0,$.cV)(Ie);Ce.displayName="LogRow";const De=100;class xe extends r.PureComponent{constructor(){super(...arguments),this.renderAllTimer=null,this.logRowsRef=(0,r.createRef)(),this.state={renderAll:!1,selection:"",selectedRow:null,popoverMenuCoordinates:{x:0,y:0}},this.openContext=(o,t)=>{this.props.onOpenContext&&this.props.onOpenContext(o,t)},this.handleSelection=(o,t)=>{if(this.popoverMenuSupported()===!1)return!1;const i=document.getSelection()?.toString();if(!i||!this.logRowsRef.current)return!1;const a=270,x=105,v=o.clientX+a>window.innerWidth?window.innerWidth-a:o.clientX,y=o.clientY+x>window.innerHeight?window.innerHeight-x:o.clientY;return this.setState({selection:i,popoverMenuCoordinates:{x:v,y},selectedRow:t}),document.addEventListener("click",this.handleDeselection),document.addEventListener("contextmenu",this.handleDeselection),!0},this.handleDeselection=o=>{if((0,E.a8)(o.target)&&!this.logRowsRef.current?.contains(o.target)){this.closePopoverMenu();return}document.getSelection()?.toString()||this.closePopoverMenu()},this.closePopoverMenu=()=>{document.removeEventListener("click",this.handleDeselection),document.removeEventListener("contextmenu",this.handleDeselection),this.setState({selection:"",popoverMenuCoordinates:{x:0,y:0},selectedRow:null})},this.makeGetRows=(0,B.A)(o=>()=>o),this.sortLogs=(0,B.A)((o,t)=>(0,E.oR)(o,t))}static{this.defaultProps={previewLimit:De}}popoverMenuSupported(){return w.$.featureToggles.logRowsPopoverMenu?!!(this.props.onClickFilterOutString||this.props.onClickFilterString):!1}componentDidMount(){const{logRows:o,previewLimit:t}=this.props,a=(o?o.length:0)<=t*2;a?this.setState({renderAll:a}):this.renderAllTimer=window.setTimeout(()=>this.setState({renderAll:!0}),2e3)}componentWillUnmount(){document.removeEventListener("click",this.handleDeselection),document.removeEventListener("contextmenu",this.handleDeselection),document.removeEventListener("selectionchange",this.handleDeselection),this.renderAllTimer&&clearTimeout(this.renderAllTimer)}render(){const{deduplicatedRows:o,logRows:t,dedupStrategy:i,theme:a,logsSortOrder:x,previewLimit:v,pinnedLogs:y,...A}=this.props,{renderAll:_}=this.state,S=(0,Se.D)(a),k=o||t,O=t&&t.length>0,U=k?k.reduce((I,H)=>H.duplicates?I+H.duplicates:I,0):0,W=i!==Z.fY.none&&U>0,F=k||[],p=x?this.sortLogs(F,x):F,z=p.slice(0,v),K=p.slice(v,p.length),R=this.makeGetRows(p),ee=new b;return(0,e.jsxs)("div",{className:S.logRows,ref:this.logRowsRef,children:[this.state.selection&&this.state.selectedRow&&(0,e.jsx)(f,{close:this.closePopoverMenu,row:this.state.selectedRow,selection:this.state.selection,...this.state.popoverMenuCoordinates,onClickFilterString:A.onClickFilterString,onClickFilterOutString:A.onClickFilterOutString}),(0,e.jsx)("table",{className:(0,l.cx)(S.logsRowsTable,this.props.overflowingContent?"":S.logsRowsTableContain),children:(0,e.jsxs)("tbody",{children:[O&&z.map(I=>(0,e.jsx)(Ce,{getRows:R,row:I,showDuplicates:W,logsSortOrder:x,onOpenContext:this.openContext,styles:S,onPermalinkClick:this.props.onPermalinkClick,scrollIntoView:this.props.scrollIntoView,permalinkedRowId:this.props.permalinkedRowId,onPinLine:this.props.onPinLine,onUnpinLine:this.props.onUnpinLine,pinLineButtonTooltipTitle:this.props.pinLineButtonTooltipTitle,pinned:this.props.pinnedRowId===I.uid||y?.some(H=>H===I.rowId),isFilterLabelActive:this.props.isFilterLabelActive,handleTextSelection:this.popoverMenuSupported()?this.handleSelection:void 0,...A},ee.getKey(I.uid))),O&&_&&K.map(I=>(0,e.jsx)(Ce,{getRows:R,row:I,showDuplicates:W,logsSortOrder:x,onOpenContext:this.openContext,styles:S,onPermalinkClick:this.props.onPermalinkClick,scrollIntoView:this.props.scrollIntoView,permalinkedRowId:this.props.permalinkedRowId,onPinLine:this.props.onPinLine,onUnpinLine:this.props.onUnpinLine,pinLineButtonTooltipTitle:this.props.pinLineButtonTooltipTitle,pinned:this.props.pinnedRowId===I.uid||y?.some(H=>H===I.rowId),isFilterLabelActive:this.props.isFilterLabelActive,handleTextSelection:this.popoverMenuSupported()?this.handleSelection:void 0,...A},ee.getKey(I.uid))),O&&!_&&(0,e.jsx)("tr",{children:(0,e.jsxs)("td",{colSpan:5,children:["Rendering ",p.length-v," rows..."]})})]})})]})}}const Pe=(0,$.cV)(xe);Pe.displayName="LogsRows"},77020:(fe,V,s)=>{s.d(V,{D:()=>N,L:()=>$});var e=s(32196),l=s(41811),B=s(84140),r=s(9557),Z=s(23596),w=s(16797);const $=(n,E)=>{let f=n.isLight?n.v1.palette.gray5:n.v1.palette.gray2;switch(E){case r.$b.crit:case r.$b.critical:f="#705da0";break;case r.$b.error:case r.$b.err:f="#e24d42";break;case r.$b.warning:case r.$b.warn:f=n.colors.warning.main;break;case r.$b.info:f="#7eb26d";break;case r.$b.debug:f="#1f78c1";break;case r.$b.trace:f="#6ed0e0";break}return{logsRowLevelColor:(0,e.css)`
      &::after {
        background-color: ${f};
      }
    `}},N=(0,l.A)(n=>{const E=w.hoverColor(n.colors.background.secondary,n),f=(0,B.A)(n.components.dashboard.background).setAlpha(.7).toRgbString();return{logsRowLevel:(0,e.css)`
      label: logs-row__level;
      max-width: ${n.spacing(1.25)};
      cursor: default;
      &::after {
        content: '';
        display: block;
        position: absolute;
        top: 1px;
        bottom: 1px;
        width: 3px;
        left: ${n.spacing(.5)};
      }
    `,logsRowWithError:(0,e.css)({maxWidth:`${n.spacing(1.5)}`}),logsRowMatchHighLight:(0,e.css)`
      label: logs-row__match-highlight;
      background: inherit;
      padding: inherit;
      color: ${n.components.textHighlight.text}
      background-color: ${n.components.textHighlight};
    `,logRows:(0,e.css)({position:"relative"}),logsRowsTable:(0,e.css)`
      label: logs-rows;
      font-family: ${n.typography.fontFamilyMonospace};
      font-size: ${n.typography.bodySmall.fontSize};
      width: 100%;
      position: relative;
    `,logsRowsTableContain:(0,e.css)`
      contain: strict;
    `,highlightBackground:(0,e.css)`
      background-color: ${(0,B.A)(n.colors.info.transparent).setAlpha(.25).toString()};
    `,logsRow:(0,e.css)`
      label: logs-row;
      width: 100%;
      cursor: pointer;
      vertical-align: top;

      &:hover {
        .log-row-menu {
          z-index: 1;
        }

        background: ${E};
      }

      td:not(.log-row-menu-cell):last-child {
        width: 100%;
      }

      > td:not(.log-row-menu-cell) {
        position: relative;
        padding-right: ${n.spacing(1)};
        border-top: 1px solid transparent;
        border-bottom: 1px solid transparent;
        height: 100%;
      }
    `,logsRowDuplicates:(0,e.css)`
      label: logs-row__duplicates;
      text-align: right;
      width: 4em;
      cursor: default;
    `,logIconError:(0,e.css)({color:n.colors.warning.main,position:"relative",top:"-2px"}),logIconInfo:(0,e.css)({color:n.colors.info.main,position:"relative",top:"-2px"}),logsRowToggleDetails:(0,e.css)`
      label: logs-row-toggle-details__level;
      font-size: 9px;
      padding-top: 5px;
      max-width: 15px;
    `,logsRowLocalTime:(0,e.css)`
      label: logs-row__localtime;
      white-space: nowrap;
    `,logsRowLabels:(0,e.css)`
      label: logs-row__labels;
      white-space: nowrap;
      max-width: 22em;

      /* This is to make the labels vertical align */
      > span {
        margin-top: 0.75px;
      }
    `,logsRowMessage:(0,e.css)`
      label: logs-row__message;
      white-space: pre-wrap;
      word-break: break-all;
      overflow-wrap: anywhere;
      width: 100%;
      text-align: left;
    `,copyLogButton:(0,e.css)`
      padding: 0 0 0 ${n.spacing(.5)};
      height: ${n.spacing(3)};
      width: ${n.spacing(3.25)};
      line-height: ${n.spacing(2.5)};
      overflow: hidden;
      &:hover {
          background-color: ${Z.MV.alpha(n.colors.text.primary,.12)};
        }
      }
    `,logDetailsContainer:(0,e.css)`
      label: logs-row-details-table;
      border: 1px solid ${n.colors.border.medium};
      padding: 0 ${n.spacing(1)} ${n.spacing(1)};
      border-radius: ${n.shape.radius.default};
      margin: ${n.spacing(2.5)} ${n.spacing(1)} ${n.spacing(2.5)} ${n.spacing(2)};
      cursor: default;
    `,logDetailsTable:(0,e.css)`
      label: logs-row-details-table;
      line-height: 18px;
      width: 100%;
      td:last-child {
        width: 100%;
      }
    `,logsDetailsIcon:(0,e.css)`
      label: logs-row-details__icon;
      position: relative;
      color: ${n.v1.palette.gray3};
      padding-top: 1px;
      padding-bottom: 1px;
      padding-right: ${n.spacing(.75)};
    `,logDetailsLabel:(0,e.css)`
      label: logs-row-details__label;
      max-width: 30em;
      min-width: 20em;
      padding: 0 ${n.spacing(1)};
      overflow-wrap: break-word;
    `,logDetailsHeading:(0,e.css)`
      label: logs-row-details__heading;
      font-weight: ${n.typography.fontWeightBold};
      padding: ${n.spacing(1)} 0 ${n.spacing(.5)};
    `,logDetailsValue:(0,e.css)`
      label: logs-row-details__row;
      position: relative;
      vertical-align: middle;
      cursor: default;

      &:hover {
        background-color: ${E};
      }
    `,topVerticalAlign:(0,e.css)`
      label: topVerticalAlign;
      margin-top: -${n.spacing(.9)};
      margin-left: -${n.spacing(.25)};
    `,detailsOpen:(0,e.css)`
      &:hover {
        background-color: ${w.hoverColor(n.colors.background.primary,n)};
      }
    `,errorLogRow:(0,e.css)`
      label: erroredLogRow;
      color: ${n.colors.text.secondary};
    `,positionRelative:(0,e.css)`
      label: positionRelative;
      position: relative;
    `,rowWithContext:(0,e.css)`
      label: rowWithContext;
      z-index: 1;
      outline: 9999px solid ${f};
      display: inherit;
    `,horizontalScroll:(0,e.css)`
      label: horizontalScroll;
      white-space: pre;
    `,contextNewline:(0,e.css)`
      display: block;
      margin-left: 0px;
    `,rowMenu:(0,e.css)`
      label: rowMenu;
      display: flex;
      flex-wrap: nowrap;
      flex-direction: row;
      align-content: flex-end;
      justify-content: space-evenly;
      align-items: center;
      position: absolute;
      top: 0;
      bottom: auto;
      background: ${n.colors.background.primary};
      box-shadow: ${n.shadows.z3};
      padding: ${n.spacing(.5,1,.5,1)};
      z-index: 100;
      gap: ${n.spacing(.5)};

      & > button {
        margin: 0;
      }
    `,logRowMenuCell:(0,e.css)`
      position: sticky;
      z-index: ${n.zIndex.dropdown};
      margin-top: -${n.spacing(.125)};
      right: 0px;

      & > span {
        transform: translateX(-100%);
      }
    `,logLine:(0,e.css)`
      background-color: transparent;
      border: none;
      diplay: inline;
      font-family: ${n.typography.fontFamilyMonospace};
      font-size: ${n.typography.bodySmall.fontSize};
      letter-spacing: ${n.typography.bodySmall.letterSpacing};
      text-align: left;
      padding: 0;
      user-select: text;
    `,logsRowLevelDetails:(0,e.css)`
      label: logs-row__level_details;
      &::after {
        top: -3px;
      }
    `,logDetails:(0,e.css)`
      label: logDetailsDefaultCursor;
      cursor: default;

      &:hover {
        background-color: ${n.colors.background.primary};
      }
    `,visibleRowMenu:(0,e.css)`
      label: visibleRowMenu;
      aspect-ratio: 1/1;
      z-index: 90;
    `,linkButton:(0,e.css)`
      label: linkButton;
      > button {
        padding-top: ${n.spacing(.5)};
      }
    `,hidden:(0,e.css)`
      label: hidden;
      visibility: hidden;
    `,unPinButton:(0,e.css)`
      height: ${n.spacing(3)};
      line-height: ${n.spacing(2.5)};
    `}})},81166:(fe,V,s)=>{s.d(V,{V:()=>Se});var e=s(74848),l=s(32196),B=s(2543),r=s(96540),Z=s(16817),w=s(39070),$=s(52622),N=s(9557),n=s(47232),E=s(14110),f=s(32264),L=s(40845),h=s(37390),b=s(55852),g=s(33390),T=s(97186),j=s(29436),q=s(31678),ae=s(69147),ce=s(91002),de=s(81073),Q=s(14647),he=s(15292);function ge(c){return{buttons:(0,l.css)({display:"flex",gap:c.spacing(1)})}}const be=c=>{const J=(0,L.of)(ge),{wrapLines:se,onChangeWrapLines:ne,onScrollCenterClick:we}=c,ve=(0,r.useCallback)(Re=>{const Me=Re.currentTarget.checked;(0,E.rR)("grafana_explore_logs_log_context_toggle_lines_clicked",{state:Me}),ne(Me)},[ne]);return(0,e.jsxs)("div",{className:J.buttons,children:[(0,e.jsx)(he.K,{showLabel:!0,value:se,onChange:ve,label:"Wrap lines"}),(0,e.jsx)(b.$n,{variant:"secondary",onClick:we,children:"Center matched line"})]})},ye=c=>({modal:(0,l.css)`
      width: 85vw;
      ${c.breakpoints.down("md")} {
        width: 100%;
      }
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
    `,sticky:(0,l.css)`
      position: sticky;
      z-index: 1;
      top: -1px;
      bottom: -1px;
    `,entry:(0,l.css)`
      & > td {
        padding: ${c.spacing(1)} 0 ${c.spacing(1)} 0;
      }
      background: ${c.colors.emphasize(c.colors.background.secondary)};

      & > table {
        margin-bottom: 0;
      }

      & .log-row-menu {
        margin-top: -6px;
      }
    `,datasourceUi:(0,l.css)`
      padding-bottom: ${c.spacing(1.25)};
      display: flex;
      align-items: center;
    `,logRowGroups:(0,l.css)`
      overflow: auto;
      max-height: 75%;
      align-self: stretch;
      display: inline-block;
      border: 1px solid ${c.colors.border.weak};
      border-radius: ${c.shape.radius.default};
      & > table {
        min-width: 100%;
      }
    `,flexColumn:(0,l.css)`
      display: flex;
      flex-direction: column;
      padding: 0 ${c.spacing(3)} ${c.spacing(3)} ${c.spacing(3)};
    `,flexRow:(0,l.css)`
      display: flex;
      flex-direction: row;
      align-items: center;
      & > div:last-child {
        margin-left: auto;
      }
    `,noMarginBottom:(0,l.css)`
      & > table {
        margin-bottom: 0;
      }
    `,hidden:(0,l.css)`
      display: none;
    `,paddingTop:(0,l.css)`
      padding-top: ${c.spacing(1)};
    `,paddingBottom:(0,l.css)`
      padding-bottom: ${c.spacing(1)};
    `,link:(0,l.css)`
      color: ${c.colors.text.secondary};
      font-size: ${c.typography.bodySmall.fontSize};
      :hover {
        color: ${c.colors.text.link};
      }
    `,loadingCell:(0,l.css)`
      position: sticky;
      left: 50%;
      display: inline-block;
      transform: translateX(-50%);
    `}),le=()=>({above:{loadingState:w.Gu.NotStarted,rows:[]},below:{loadingState:w.Gu.NotStarted,rows:[]}}),X=(c,J)=>c==="above"&&J===$.uH.Descending||c==="below"&&J===$.uH.Ascending?N.ZF.Forward:N.ZF.Backward,pe=(c,J)=>({...c,dataFrame:{...c.dataFrame,refId:`context_${J.above}_${J.below}`}}),ue=(c,J)=>c.some(se=>se.entry===J.entry&&se.timeEpochNs===J.timeEpochNs),me=100,Se=({row:c,open:J,logsSortOrder:se,timeZone:ne,getLogRowContextUi:we,getRowContextQuery:ve,onClose:Re,getRowContext:Me})=>{const $e=(0,r.useRef)(null),Te=(0,r.useRef)(null),Ee=(0,r.useRef)(null),ke=(0,r.useRef)(null),Ie=(0,r.useRef)(null),Ce=(0,r.useRef)(null),De=(0,r.useRef)(null),xe=(0,r.useRef)({above:0,below:0}),Pe=(0,q.useDispatch)(),d=(0,L.$j)(),o=ye(d),[t,i]=(0,r.useState)(!0),[a,x]=(0,r.useState)(le()),v=(u,M)=>{x(C=>{const P={...C};return P[u]=M(C[u]),P})},y=(0,r.useRef)(1),[A,_]=(0,r.useState)(null),[S,k]=(0,r.useState)(g.A.getBool(T.$.logContextWrapLogMessage,g.A.getBool(T.$.wrapLogMessage,!0))),O=(0,r.useCallback)(()=>{const{below:u,above:M}=a,C=(0,ce.oR)([...u.rows,c,...M.rows],$.uH.Ascending),P=C[0].timeEpochMs;let G=C[C.length-1].timeEpochMs;P===G&&(G+=1);const m=(0,n.KQ)(P),Y=(0,n.KQ)(G);return{from:m,to:Y,raw:{from:m,to:Y}}},[a,c]),U=(0,r.useCallback)(async()=>{const u=ve?await ve(c):null;_(u)},[c,ve]),W=async()=>{await U(),x(le()),xe.current={above:0,below:0},y.current+=1},F=async(u,M)=>{xe.current[u]+=1;const C=M.at(u==="above"?0:-1);if(C==null)throw new Error("should never happen. the array always contains at least 1 item (the middle row)");(0,E.rR)("grafana_explore_logs_log_context_load_more_called",{datasourceType:C.datasourceType,above:xe.current.above,below:xe.current.below});const P=X(u,se),G=await Me(pe(C,xe.current),{limit:me,direction:P}),m=(0,ae.HT)(G.data).rows;return se===$.uH.Ascending&&m.reverse(),m.filter(D=>!ue(M,D))};(0,r.useEffect)(()=>{J&&U()},[U,J]);const[p,z]=(0,r.useState)([]),K=u=>{p.indexOf(u)===-1&&z([...p,u])},R=u=>{const M=p.indexOf(u);M>-1&&(p.splice(M,1),z([...p]))},ee=async u=>{const{below:M,above:C}=a;if(a[u].loadingState===w.Gu.Loading)return;v(u,m=>({...m,loadingState:w.Gu.Loading}));const G=y.current;try{const m=[...C.rows,c,...M.rows],Y=(await F(u,m)).map(oe=>!oe.searchWords||!oe.searchWords?.length?{...oe,searchWords:c.searchWords}:oe),[D,re]=(0,B.partition)(Y,oe=>oe.timeEpochNs>c.timeEpochNs),Le=se===$.uH.Ascending?re:D,je=se===$.uH.Ascending?D:re;G===y.current&&x(oe=>{const Ae=Le.length>0?(0,ce.oR)([...Le,...oe.above.rows],se):oe.above.rows,Oe=je.length>0?(0,ce.oR)([...oe.below.rows,...je],se):oe.below.rows;return{above:{rows:Ae,loadingState:u==="above"?Y.length===0?w.Gu.Done:w.Gu.NotStarted:oe.above.loadingState},below:{rows:Oe,loadingState:u==="below"?Y.length===0?w.Gu.Done:w.Gu.NotStarted:oe.below.loadingState}}})}catch{v(u,m=>({rows:m.rows,loadingState:w.Gu.Error}))}},I=async(u,M)=>{for(const C of u){if(!C.isIntersecting)continue;const P=C.target;P===Ce.current?ee("above"):P===De.current&&ee("below")}};(0,r.useEffect)(()=>{const u=$e.current,M=Ce.current,C=De.current;if(u==null)return;const P=new IntersectionObserver(I,{root:u});return M!=null&&P.observe(M),C!=null&&P.observe(C),()=>{P.disconnect()}});const H=(0,r.useCallback)(()=>{Ee.current?.scrollIntoView({block:"center"}),Te.current?.scrollIntoView({block:"center"})},[Ee,Te]);(0,r.useLayoutEffect)(()=>{const u=$e.current;if(u==null)return;const M=Ie.current,C=u.clientHeight;if(Ie.current=C,M!==C){H();return}if(xe.current.above<=1&&xe.current.below<=1){H();return}const P=ke.current,G=u.scrollHeight;if(ke.current=G,P!=null){const m=u.scrollTop+(G-P);u.scrollTop=m}},[a.above.rows,H]),(0,Z.A)(U,[ve,c]);const te=a.above.loadingState,ie=a.below.loadingState;return(0,e.jsxs)(h.a,{isOpen:J,title:"Log context",contentClassName:o.flexColumn,className:o.modal,onDismiss:Re,children:[f.$.featureToggles.logsContextDatasourceUi&&we&&(0,e.jsx)("div",{className:o.datasourceUi,children:we(c,W)}),(0,e.jsx)("div",{className:(0,l.cx)(o.flexRow,o.paddingBottom),children:(0,e.jsx)("div",{children:(0,e.jsx)(be,{wrapLines:S,onChangeWrapLines:k,onScrollCenterClick:H})})}),(0,e.jsx)("div",{ref:$e,className:o.logRowGroups,children:(0,e.jsx)("table",{children:(0,e.jsxs)("tbody",{children:[(0,e.jsx)("tr",{children:(0,e.jsxs)("td",{className:o.loadingCell,children:[te!==w.Gu.Done&&te!==w.Gu.Error&&(0,e.jsx)("div",{ref:Ce,children:(0,e.jsx)(de.I,{adjective:"newer"})}),te===w.Gu.Error&&(0,e.jsx)("div",{children:"Error loading log more logs."}),te===w.Gu.Done&&(0,e.jsx)("div",{children:"No more logs available."})]})}),(0,e.jsx)("tr",{children:(0,e.jsx)("td",{className:o.noMarginBottom,children:(0,e.jsx)(Q.k,{logRows:a.above.rows,dedupStrategy:$.fY.none,showLabels:g.A.getBool(T.$.showLabels,!1),showTime:g.A.getBool(T.$.showTime,!0),wrapLogMessage:S,prettifyLogMessage:g.A.getBool(T.$.prettifyLogMessage,!1),enableLogDetails:!0,timeZone:ne,displayedFields:p,onClickShowField:K,onClickHideField:R})})}),(0,e.jsx)("tr",{ref:Ee}),(0,e.jsx)("tr",{ref:Te,className:(0,l.cx)(o.entry,t?o.sticky:null),"data-testid":"entry-row",children:(0,e.jsx)("td",{className:o.noMarginBottom,children:(0,e.jsx)(Q.k,{logRows:[c],dedupStrategy:$.fY.none,showLabels:g.A.getBool(T.$.showLabels,!1),showTime:g.A.getBool(T.$.showTime,!0),wrapLogMessage:S,prettifyLogMessage:g.A.getBool(T.$.prettifyLogMessage,!1),enableLogDetails:!0,timeZone:ne,displayedFields:p,onClickShowField:K,onClickHideField:R,onUnpinLine:()=>i(!1),onPinLine:()=>i(!0),pinnedRowId:t?c.uid:void 0,overflowingContent:!0})})}),(0,e.jsx)("tr",{children:(0,e.jsx)("td",{children:(0,e.jsx)(e.Fragment,{children:(0,e.jsx)(Q.k,{logRows:a.below.rows,dedupStrategy:$.fY.none,showLabels:g.A.getBool(T.$.showLabels,!1),showTime:g.A.getBool(T.$.showTime,!0),wrapLogMessage:S,prettifyLogMessage:g.A.getBool(T.$.prettifyLogMessage,!1),enableLogDetails:!0,timeZone:ne,displayedFields:p,onClickShowField:K,onClickHideField:R})})})}),(0,e.jsx)("tr",{children:(0,e.jsxs)("td",{className:o.loadingCell,children:[ie!==w.Gu.Done&&ie!==w.Gu.Error&&(0,e.jsx)("div",{ref:De,children:(0,e.jsx)(de.I,{adjective:"older"})}),ie===w.Gu.Error&&(0,e.jsx)("div",{children:"Error loading log more logs."}),ie===w.Gu.Done&&(0,e.jsx)("div",{children:"No more logs available."})]})})]})})}),(0,e.jsx)(h.a.ButtonRow,{children:A?.datasource?.uid&&(0,e.jsx)(b.$n,{variant:"secondary",onClick:async()=>{let u=c.uid;c.dataFrame.refId&&(u=c.uid.replace(c.dataFrame.refId,A.refId)),Pe((0,j.ve)({queries:[A],range:O(),datasourceUid:A.datasource.uid,panelsState:{logs:{id:u}}})),Re(),(0,E.rR)("grafana_explore_logs_log_context_open_split_view_clicked",{datasourceType:c.datasourceType,logRowUid:c.uid})},children:"Open in split view"})})]})}}}]);

//# sourceMappingURL=1166.247cc706617cc6970cae.js.map
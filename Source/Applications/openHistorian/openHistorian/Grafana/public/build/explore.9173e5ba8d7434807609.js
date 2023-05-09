(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[9782],{80414:(ge,Z,i)=>{"use strict";i.d(Z,{K:()=>se});var t=i(671),T=i(65583),ee=i(86647),R=i(34177);const te=({sourceUID:D,targetUID:ne,...J})=>({...J,source:(0,ee.F)().getInstanceSettings(D),target:(0,ee.F)().getInstanceSettings(ne)}),p=D=>D.map(te);function I(D){return D.data}const se=()=>{const{backend:D}=(0,R.p)(),[ne,J]=(0,t.Z)(()=>(0,T.n)(D.fetch({url:"/api/datasources/correlations",method:"GET",showErrorAlert:!1})).then(I).then(p),[D]),[A,U]=(0,t.Z)(({sourceUID:B,...W})=>D.post(`/api/datasources/uid/${B}/correlations`,W).then(te),[D]),[K,oe]=(0,t.Z)(({sourceUID:B,uid:W})=>D.delete(`/api/datasources/uid/${B}/correlations/${W}`),[D]),[Y,F]=(0,t.Z)(({sourceUID:B,uid:W,..._})=>D.patch(`/api/datasources/uid/${B}/correlations/${W}`,_).then(te),[D]);return{create:{execute:U,...A},update:{execute:F,...Y},get:{execute:J,...ne},remove:{execute:oe,...K}}}},91551:(ge,Z,i)=>{"use strict";i.d(Z,{N:()=>ne});var t=i(68061),T=i(74188),ee=i(85854),R=i(14582),te=i(89890),p=i(3249),I=i(17882);const se="hideSeriesFrom",D=(0,t.Y4)(se);function ne(F,B,W,_){const{overrides:P}=W,L=F,he=P.findIndex(D);if(he<0){if(B===I.R.ToggleSelection){const H=J([L,...Y(P,_)]);return{...W,overrides:[...W.overrides,H]}}const Se=oe(_,L),fe=J(Se);return{...W,overrides:[...W.overrides,fe]}}const me=Array.from(P),[ue]=me.splice(he,1);if(B===I.R.ToggleSelection){let Se=U(ue);const fe=Y(me,_);if(fe.length>0&&(Se=Se.filter(z=>fe.indexOf(z)<0)),Se[0]===L&&Se.length===1)return{...W,overrides:me};const H=J([L,...fe]);return{...W,overrides:[...me,H]}}const ce=A(ue,L);return K(ce,_)?{...W,overrides:me}:{...W,overrides:[...me,ce]}}function J(F,B=T.Ys.exclude,W){return W=W??{id:"custom.hideFrom",value:{viz:!0,legend:!1,tooltip:!1}},{__systemRef:se,matcher:{id:ee.mi.byNames,options:{mode:B,names:F,prefix:B===T.Ys.exclude?"All except:":void 0,readOnly:!0}},properties:[{...W,value:{viz:!0,legend:!1,tooltip:!1}}]}}const A=(F,B,W=T.Ys.exclude)=>{const _=F.properties.find(he=>he.id==="custom.hideFrom"),P=U(F),L=P.findIndex(he=>he===B);return L<0?P.push(B):P.splice(L,1),J(P,W,_)},U=F=>{const B=F.matcher.options?.names;return Array.isArray(B)?[...B]:[]},K=(F,B)=>U(F).length===oe(B).length,oe=(F,B)=>{const W=new Set;for(const _ of F)for(const P of _.fields){if(P.type!==R.fS.number)continue;const L=(0,te.C)(P,_,F);L!==B&&W.add(L)}return Array.from(W)},Y=(F,B)=>{let W=[];for(const _ of F){const P=_.properties.find(L=>L.id==="custom.hideFrom");if(P!==void 0&&P.value?.legend===!0){const he=p.Ls.get(_.matcher.id).get(_.matcher.options);for(const me of B)for(const ue of me.fields){if(ue.type!==R.fS.number)continue;const ce=(0,te.C)(ue,me,B);he(ue,me,B)&&W.push(ce)}}}return W}},21258:(ge,Z,i)=>{"use strict";i.r(Z),i.d(Z,{default:()=>or});var t=i(68404),T=i(35645),ee=i(87513),R=i(81168),te=i(25e3),p=i(9892),I=i(82897),se=i(98101),D=i(49922),ne=i(37932),J=i(60325),A=i(53546),U=i(80177),K=i(34177),oe=i(60499),Y=i(76770),F=i(3032),B=i(36578),W=i(80414),_=i(58166),P=i(48197),L=i(73257),he=i(75090);const me=({exploreIdLeft:e,exploreIdRight:n})=>{const[s,o]=(0,t.useState)([]),{query:a}=(0,_.useKBar)(),r=(0,R.useDispatch)(),l=(0,R.useSelector)(he.p);return(0,t.useEffect)(()=>{const c={name:"Explore",priority:_.Priority.HIGH+1},d=[];l?(d.push({id:"explore/run-query-left",name:"Run query (left)",keywords:"query left",perform:()=>{r((0,L.aA)(e))},section:c}),n&&(d.push({id:"explore/run-query-right",name:"Run query (right)",keywords:"query right",perform:()=>{r((0,L.aA)(n))},section:c}),d.push({id:"explore/split-view-close-left",name:"Close split view left",keywords:"split",perform:()=>{r((0,P.YV)(e))},section:c}),d.push({id:"explore/split-view-close-right",name:"Close split view right",keywords:"split",perform:()=>{r((0,P.YV)(n))},section:c}))):(d.push({id:"explore/run-query",name:"Run query",keywords:"query",perform:()=>{r((0,L.aA)(e))},section:c}),d.push({id:"explore/split-view-open",name:"Open split view",keywords:"split",perform:()=>{r((0,P.bW)())},section:c})),o(d)},[e,n,l,a,r]),(0,_.useRegisterActions)(a?s:[],[s,a]),null};var ue=i(58635),ce=i(36635),Se=i(85081),fe=i(68183),H=i(41818),z=i(72648),Pe=i(47694),le=i(58379),Ee=i(55935),We=i(63496),ke=i(91162),Ie=i(25405),gs=i(89050),G=i(90595),Ue=i(86647),Qe=i(45253),xt=i(68226),St=i(72080),bt=i(13580),tt=i(18271),fs=i(41141);const wt=e=>{const n={transition:`opacity ${e.duration}ms linear`,opacity:0},s={exited:{opacity:0,display:"none"},entering:{opacity:0},entered:{opacity:1},exiting:{opacity:0}};return t.createElement(fs.ZP,{in:e.in,timeout:e.duration,unmountOnExit:e.unmountOnExit||!1,onExited:e.onExited},o=>t.createElement("div",{style:{...n,...s[o]}},e.children))};var He=i(40967),ys=i(68545),st=i(45984),Oe=i(40400),Ct=i(18607),vs=i(97394),Es=function(){var e=function(n,s){return e=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(o,a){o.__proto__=a}||function(o,a){for(var r in a)Object.prototype.hasOwnProperty.call(a,r)&&(o[r]=a[r])},e(n,s)};return function(n,s){e(n,s);function o(){this.constructor=n}n.prototype=s===null?Object.create(s):(o.prototype=s.prototype,new o)}}(),X=function(){return X=Object.assign||function(e){for(var n,s=1,o=arguments.length;s<o;s++){n=arguments[s];for(var a in n)Object.prototype.hasOwnProperty.call(n,a)&&(e[a]=n[a])}return e},X.apply(this,arguments)},Rt={width:"100%",height:"10px",top:"0px",left:"0px",cursor:"row-resize"},Tt={width:"10px",height:"100%",top:"0px",left:"0px",cursor:"col-resize"},Ve={width:"20px",height:"20px",position:"absolute"},xs={top:X(X({},Rt),{top:"-5px"}),right:X(X({},Tt),{left:void 0,right:"-5px"}),bottom:X(X({},Rt),{top:void 0,bottom:"-5px"}),left:X(X({},Tt),{left:"-5px"}),topRight:X(X({},Ve),{right:"-10px",top:"-10px",cursor:"ne-resize"}),bottomRight:X(X({},Ve),{right:"-10px",bottom:"-10px",cursor:"se-resize"}),bottomLeft:X(X({},Ve),{left:"-10px",bottom:"-10px",cursor:"sw-resize"}),topLeft:X(X({},Ve),{left:"-10px",top:"-10px",cursor:"nw-resize"})},Ss=function(e){Es(n,e);function n(){var s=e!==null&&e.apply(this,arguments)||this;return s.onMouseDown=function(o){s.props.onResizeStart(o,s.props.direction)},s.onTouchStart=function(o){s.props.onResizeStart(o,s.props.direction)},s}return n.prototype.render=function(){return t.createElement("div",{className:this.props.className||"",style:X(X({position:"absolute",userSelect:"none"},xs[this.props.direction]),this.props.replaceStyles||{}),onMouseDown:this.onMouseDown,onTouchStart:this.onTouchStart},this.props.children)},n}(t.PureComponent),bs=function(){var e=function(n,s){return e=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(o,a){o.__proto__=a}||function(o,a){for(var r in a)Object.prototype.hasOwnProperty.call(a,r)&&(o[r]=a[r])},e(n,s)};return function(n,s){e(n,s);function o(){this.constructor=n}n.prototype=s===null?Object.create(s):(o.prototype=s.prototype,new o)}}(),ye=function(){return ye=Object.assign||function(e){for(var n,s=1,o=arguments.length;s<o;s++){n=arguments[s];for(var a in n)Object.prototype.hasOwnProperty.call(n,a)&&(e[a]=n[a])}return e},ye.apply(this,arguments)},ws={width:"auto",height:"auto"},Ze=function(e,n,s){return Math.max(Math.min(e,s),n)},Lt=function(e,n){return Math.round(e/n)*n},Fe=function(e,n){return new RegExp(e,"i").test(n)},Ge=function(e){return Boolean(e.touches&&e.touches.length)},Cs=function(e){return Boolean((e.clientX||e.clientX===0)&&(e.clientY||e.clientY===0))},zt=function(e,n,s){s===void 0&&(s=0);var o=n.reduce(function(r,l,c){return Math.abs(l-e)<Math.abs(n[r]-e)?c:r},0),a=Math.abs(n[o]-e);return s===0||a<s?n[o]:e},ot=function(e){return e=e.toString(),e==="auto"||e.endsWith("px")||e.endsWith("%")||e.endsWith("vh")||e.endsWith("vw")||e.endsWith("vmax")||e.endsWith("vmin")?e:e+"px"},je=function(e,n,s,o){if(e&&typeof e=="string"){if(e.endsWith("px"))return Number(e.replace("px",""));if(e.endsWith("%")){var a=Number(e.replace("%",""))/100;return n*a}if(e.endsWith("vw")){var a=Number(e.replace("vw",""))/100;return s*a}if(e.endsWith("vh")){var a=Number(e.replace("vh",""))/100;return o*a}}return e},Rs=function(e,n,s,o,a,r,l){return o=je(o,e.width,n,s),a=je(a,e.height,n,s),r=je(r,e.width,n,s),l=je(l,e.height,n,s),{maxWidth:typeof o>"u"?void 0:Number(o),maxHeight:typeof a>"u"?void 0:Number(a),minWidth:typeof r>"u"?void 0:Number(r),minHeight:typeof l>"u"?void 0:Number(l)}},Ts=["as","style","className","grid","snap","bounds","boundsByDirection","size","defaultSize","minWidth","minHeight","maxWidth","maxHeight","lockAspectRatio","lockAspectRatioExtraWidth","lockAspectRatioExtraHeight","enable","handleStyles","handleClasses","handleWrapperStyle","handleWrapperClass","children","onResizeStart","onResize","onResizeStop","handleComponent","scale","resizeRatio","snapGap"],It="__resizable_base__",Ls=function(e){bs(n,e);function n(s){var o=e.call(this,s)||this;return o.ratio=1,o.resizable=null,o.parentLeft=0,o.parentTop=0,o.resizableLeft=0,o.resizableRight=0,o.resizableTop=0,o.resizableBottom=0,o.targetLeft=0,o.targetTop=0,o.appendBase=function(){if(!o.resizable||!o.window)return null;var a=o.parentNode;if(!a)return null;var r=o.window.document.createElement("div");return r.style.width="100%",r.style.height="100%",r.style.position="absolute",r.style.transform="scale(0, 0)",r.style.left="0",r.style.flex="0 0 100%",r.classList?r.classList.add(It):r.className+=It,a.appendChild(r),r},o.removeBase=function(a){var r=o.parentNode;r&&r.removeChild(a)},o.ref=function(a){a&&(o.resizable=a)},o.state={isResizing:!1,width:typeof(o.propsSize&&o.propsSize.width)>"u"?"auto":o.propsSize&&o.propsSize.width,height:typeof(o.propsSize&&o.propsSize.height)>"u"?"auto":o.propsSize&&o.propsSize.height,direction:"right",original:{x:0,y:0,width:0,height:0},backgroundStyle:{height:"100%",width:"100%",backgroundColor:"rgba(0,0,0,0)",cursor:"auto",opacity:0,position:"fixed",zIndex:9999,top:"0",left:"0",bottom:"0",right:"0"},flexBasis:void 0},o.onResizeStart=o.onResizeStart.bind(o),o.onMouseMove=o.onMouseMove.bind(o),o.onMouseUp=o.onMouseUp.bind(o),o}return Object.defineProperty(n.prototype,"parentNode",{get:function(){return this.resizable?this.resizable.parentNode:null},enumerable:!1,configurable:!0}),Object.defineProperty(n.prototype,"window",{get:function(){return!this.resizable||!this.resizable.ownerDocument?null:this.resizable.ownerDocument.defaultView},enumerable:!1,configurable:!0}),Object.defineProperty(n.prototype,"propsSize",{get:function(){return this.props.size||this.props.defaultSize||ws},enumerable:!1,configurable:!0}),Object.defineProperty(n.prototype,"size",{get:function(){var s=0,o=0;if(this.resizable&&this.window){var a=this.resizable.offsetWidth,r=this.resizable.offsetHeight,l=this.resizable.style.position;l!=="relative"&&(this.resizable.style.position="relative"),s=this.resizable.style.width!=="auto"?this.resizable.offsetWidth:a,o=this.resizable.style.height!=="auto"?this.resizable.offsetHeight:r,this.resizable.style.position=l}return{width:s,height:o}},enumerable:!1,configurable:!0}),Object.defineProperty(n.prototype,"sizeStyle",{get:function(){var s=this,o=this.props.size,a=function(c){if(typeof s.state[c]>"u"||s.state[c]==="auto")return"auto";if(s.propsSize&&s.propsSize[c]&&s.propsSize[c].toString().endsWith("%")){if(s.state[c].toString().endsWith("%"))return s.state[c].toString();var d=s.getParentSize(),h=Number(s.state[c].toString().replace("px","")),u=h/d[c]*100;return u+"%"}return ot(s.state[c])},r=o&&typeof o.width<"u"&&!this.state.isResizing?ot(o.width):a("width"),l=o&&typeof o.height<"u"&&!this.state.isResizing?ot(o.height):a("height");return{width:r,height:l}},enumerable:!1,configurable:!0}),n.prototype.getParentSize=function(){if(!this.parentNode)return this.window?{width:this.window.innerWidth,height:this.window.innerHeight}:{width:0,height:0};var s=this.appendBase();if(!s)return{width:0,height:0};var o=!1,a=this.parentNode.style.flexWrap;a!=="wrap"&&(o=!0,this.parentNode.style.flexWrap="wrap"),s.style.position="relative",s.style.minWidth="100%",s.style.minHeight="100%";var r={width:s.offsetWidth,height:s.offsetHeight};return o&&(this.parentNode.style.flexWrap=a),this.removeBase(s),r},n.prototype.bindEvents=function(){this.window&&(this.window.addEventListener("mouseup",this.onMouseUp),this.window.addEventListener("mousemove",this.onMouseMove),this.window.addEventListener("mouseleave",this.onMouseUp),this.window.addEventListener("touchmove",this.onMouseMove,{capture:!0,passive:!1}),this.window.addEventListener("touchend",this.onMouseUp))},n.prototype.unbindEvents=function(){this.window&&(this.window.removeEventListener("mouseup",this.onMouseUp),this.window.removeEventListener("mousemove",this.onMouseMove),this.window.removeEventListener("mouseleave",this.onMouseUp),this.window.removeEventListener("touchmove",this.onMouseMove,!0),this.window.removeEventListener("touchend",this.onMouseUp))},n.prototype.componentDidMount=function(){if(!(!this.resizable||!this.window)){var s=this.window.getComputedStyle(this.resizable);this.setState({width:this.state.width||this.size.width,height:this.state.height||this.size.height,flexBasis:s.flexBasis!=="auto"?s.flexBasis:void 0})}},n.prototype.componentWillUnmount=function(){this.window&&this.unbindEvents()},n.prototype.createSizeForCssProperty=function(s,o){var a=this.propsSize&&this.propsSize[o];return this.state[o]==="auto"&&this.state.original[o]===s&&(typeof a>"u"||a==="auto")?"auto":s},n.prototype.calculateNewMaxFromBoundary=function(s,o){var a=this.props.boundsByDirection,r=this.state.direction,l=a&&Fe("left",r),c=a&&Fe("top",r),d,h;if(this.props.bounds==="parent"){var u=this.parentNode;u&&(d=l?this.resizableRight-this.parentLeft:u.offsetWidth+(this.parentLeft-this.resizableLeft),h=c?this.resizableBottom-this.parentTop:u.offsetHeight+(this.parentTop-this.resizableTop))}else this.props.bounds==="window"?this.window&&(d=l?this.resizableRight:this.window.innerWidth-this.resizableLeft,h=c?this.resizableBottom:this.window.innerHeight-this.resizableTop):this.props.bounds&&(d=l?this.resizableRight-this.targetLeft:this.props.bounds.offsetWidth+(this.targetLeft-this.resizableLeft),h=c?this.resizableBottom-this.targetTop:this.props.bounds.offsetHeight+(this.targetTop-this.resizableTop));return d&&Number.isFinite(d)&&(s=s&&s<d?s:d),h&&Number.isFinite(h)&&(o=o&&o<h?o:h),{maxWidth:s,maxHeight:o}},n.prototype.calculateNewSizeFromDirection=function(s,o){var a=this.props.scale||1,r=this.props.resizeRatio||1,l=this.state,c=l.direction,d=l.original,h=this.props,u=h.lockAspectRatio,m=h.lockAspectRatioExtraHeight,g=h.lockAspectRatioExtraWidth,v=d.width,f=d.height,y=m||0,E=g||0;return Fe("right",c)&&(v=d.width+(s-d.x)*r/a,u&&(f=(v-E)/this.ratio+y)),Fe("left",c)&&(v=d.width-(s-d.x)*r/a,u&&(f=(v-E)/this.ratio+y)),Fe("bottom",c)&&(f=d.height+(o-d.y)*r/a,u&&(v=(f-y)*this.ratio+E)),Fe("top",c)&&(f=d.height-(o-d.y)*r/a,u&&(v=(f-y)*this.ratio+E)),{newWidth:v,newHeight:f}},n.prototype.calculateNewSizeFromAspectRatio=function(s,o,a,r){var l=this.props,c=l.lockAspectRatio,d=l.lockAspectRatioExtraHeight,h=l.lockAspectRatioExtraWidth,u=typeof r.width>"u"?10:r.width,m=typeof a.width>"u"||a.width<0?s:a.width,g=typeof r.height>"u"?10:r.height,v=typeof a.height>"u"||a.height<0?o:a.height,f=d||0,y=h||0;if(c){var E=(g-f)*this.ratio+y,x=(v-f)*this.ratio+y,w=(u-y)/this.ratio+f,S=(m-y)/this.ratio+f,C=Math.max(u,E),N=Math.min(m,x),Q=Math.max(g,w),j=Math.min(v,S);s=Ze(s,C,N),o=Ze(o,Q,j)}else s=Ze(s,u,m),o=Ze(o,g,v);return{newWidth:s,newHeight:o}},n.prototype.setBoundingClientRect=function(){if(this.props.bounds==="parent"){var s=this.parentNode;if(s){var o=s.getBoundingClientRect();this.parentLeft=o.left,this.parentTop=o.top}}if(this.props.bounds&&typeof this.props.bounds!="string"){var a=this.props.bounds.getBoundingClientRect();this.targetLeft=a.left,this.targetTop=a.top}if(this.resizable){var r=this.resizable.getBoundingClientRect(),l=r.left,c=r.top,d=r.right,h=r.bottom;this.resizableLeft=l,this.resizableRight=d,this.resizableTop=c,this.resizableBottom=h}},n.prototype.onResizeStart=function(s,o){if(!(!this.resizable||!this.window)){var a=0,r=0;if(s.nativeEvent&&Cs(s.nativeEvent)?(a=s.nativeEvent.clientX,r=s.nativeEvent.clientY):s.nativeEvent&&Ge(s.nativeEvent)&&(a=s.nativeEvent.touches[0].clientX,r=s.nativeEvent.touches[0].clientY),this.props.onResizeStart&&this.resizable){var l=this.props.onResizeStart(s,o,this.resizable);if(l===!1)return}this.props.size&&(typeof this.props.size.height<"u"&&this.props.size.height!==this.state.height&&this.setState({height:this.props.size.height}),typeof this.props.size.width<"u"&&this.props.size.width!==this.state.width&&this.setState({width:this.props.size.width})),this.ratio=typeof this.props.lockAspectRatio=="number"?this.props.lockAspectRatio:this.size.width/this.size.height;var c,d=this.window.getComputedStyle(this.resizable);if(d.flexBasis!=="auto"){var h=this.parentNode;if(h){var u=this.window.getComputedStyle(h).flexDirection;this.flexDir=u.startsWith("row")?"row":"column",c=d.flexBasis}}this.setBoundingClientRect(),this.bindEvents();var m={original:{x:a,y:r,width:this.size.width,height:this.size.height},isResizing:!0,backgroundStyle:ye(ye({},this.state.backgroundStyle),{cursor:this.window.getComputedStyle(s.target).cursor||"auto"}),direction:o,flexBasis:c};this.setState(m)}},n.prototype.onMouseMove=function(s){var o=this;if(!(!this.state.isResizing||!this.resizable||!this.window)){if(this.window.TouchEvent&&Ge(s))try{s.preventDefault(),s.stopPropagation()}catch{}var a=this.props,r=a.maxWidth,l=a.maxHeight,c=a.minWidth,d=a.minHeight,h=Ge(s)?s.touches[0].clientX:s.clientX,u=Ge(s)?s.touches[0].clientY:s.clientY,m=this.state,g=m.direction,v=m.original,f=m.width,y=m.height,E=this.getParentSize(),x=Rs(E,this.window.innerWidth,this.window.innerHeight,r,l,c,d);r=x.maxWidth,l=x.maxHeight,c=x.minWidth,d=x.minHeight;var w=this.calculateNewSizeFromDirection(h,u),S=w.newHeight,C=w.newWidth,N=this.calculateNewMaxFromBoundary(r,l);this.props.snap&&this.props.snap.x&&(C=zt(C,this.props.snap.x,this.props.snapGap)),this.props.snap&&this.props.snap.y&&(S=zt(S,this.props.snap.y,this.props.snapGap));var Q=this.calculateNewSizeFromAspectRatio(C,S,{width:N.maxWidth,height:N.maxHeight},{width:c,height:d});if(C=Q.newWidth,S=Q.newHeight,this.props.grid){var j=Lt(C,this.props.grid[0]),ae=Lt(S,this.props.grid[1]),M=this.props.snapGap||0;C=M===0||Math.abs(j-C)<=M?j:C,S=M===0||Math.abs(ae-S)<=M?ae:S}var O={width:C-v.width,height:S-v.height};if(f&&typeof f=="string"){if(f.endsWith("%")){var b=C/E.width*100;C=b+"%"}else if(f.endsWith("vw")){var k=C/this.window.innerWidth*100;C=k+"vw"}else if(f.endsWith("vh")){var q=C/this.window.innerHeight*100;C=q+"vh"}}if(y&&typeof y=="string"){if(y.endsWith("%")){var b=S/E.height*100;S=b+"%"}else if(y.endsWith("vw")){var k=S/this.window.innerWidth*100;S=k+"vw"}else if(y.endsWith("vh")){var q=S/this.window.innerHeight*100;S=q+"vh"}}var re={width:this.createSizeForCssProperty(C,"width"),height:this.createSizeForCssProperty(S,"height")};this.flexDir==="row"?re.flexBasis=re.width:this.flexDir==="column"&&(re.flexBasis=re.height),(0,vs.flushSync)(function(){o.setState(re)}),this.props.onResize&&this.props.onResize(s,g,this.resizable,O)}},n.prototype.onMouseUp=function(s){var o=this.state,a=o.isResizing,r=o.direction,l=o.original;if(!(!a||!this.resizable)){var c={width:this.size.width-l.width,height:this.size.height-l.height};this.props.onResizeStop&&this.props.onResizeStop(s,r,this.resizable,c),this.props.size&&this.setState(this.props.size),this.unbindEvents(),this.setState({isResizing:!1,backgroundStyle:ye(ye({},this.state.backgroundStyle),{cursor:"auto"})})}},n.prototype.updateSize=function(s){this.setState({width:s.width,height:s.height})},n.prototype.renderResizer=function(){var s=this,o=this.props,a=o.enable,r=o.handleStyles,l=o.handleClasses,c=o.handleWrapperStyle,d=o.handleWrapperClass,h=o.handleComponent;if(!a)return null;var u=Object.keys(a).map(function(m){return a[m]!==!1?t.createElement(Ss,{key:m,direction:m,onResizeStart:s.onResizeStart,replaceStyles:r&&r[m],className:l&&l[m]},h&&h[m]?h[m]:null):null});return t.createElement("div",{className:d,style:c},u)},n.prototype.render=function(){var s=this,o=Object.keys(this.props).reduce(function(l,c){return Ts.indexOf(c)!==-1||(l[c]=s.props[c]),l},{}),a=ye(ye(ye({position:"relative",userSelect:this.state.isResizing?"none":"auto"},this.props.style),this.sizeStyle),{maxWidth:this.props.maxWidth,maxHeight:this.props.maxHeight,minWidth:this.props.minWidth,minHeight:this.props.minHeight,boxSizing:"border-box",flexShrink:0});this.state.flexBasis&&(a.flexBasis=this.state.flexBasis);var r=this.props.as||"div";return t.createElement(r,ye({ref:this.ref,style:a,className:this.props.className},o),this.state.isResizing&&t.createElement("div",{style:this.state.backgroundStyle}),this.props.children,this.renderResizer())},n.defaultProps={as:"div",onResizeStart:function(){},onResize:function(){},onResizeStop:function(){},enable:{top:!0,right:!0,bottom:!0,left:!0,topRight:!0,bottomRight:!0,bottomLeft:!0,topLeft:!0},style:{},grid:[1,1],lockAspectRatio:!1,lockAspectRatioExtraWidth:0,lockAspectRatioExtraHeight:0,scale:1,resizeRatio:1,snapGap:0},n}(t.PureComponent),zs=i(7804);const Is=e=>p.keyframes`
  0% {
    transform: translateY(${e.components.horizontalDrawer.defaultHeight}px);
  }

  100% {
    transform: translateY(0px);
  }
`,Fs=(0,zs.B)(e=>({container:p.css`
      position: fixed !important;
      bottom: 0;
      background: ${e.colors.background.primary};
      border-top: 1px solid ${e.colors.border.weak};
      margin: ${e.spacing(0,-2,0,-2)};
      box-shadow: ${e.shadows.z3};
      z-index: ${e.zIndex.navbarFixed};
    `,drawerActive:p.css`
      opacity: 1;
      animation: 0.5s ease-out ${Is(e)};
    `,rzHandle:p.css`
      background: ${e.colors.secondary.main};
      transition: 0.3s background ease-in-out;
      position: relative;
      width: 200px !important;
      height: 7px !important;
      left: calc(50% - 100px) !important;
      top: -4px !important;
      cursor: grab;
      border-radius: ${e.shape.radius.pill};
      &:hover {
        background: ${e.colors.secondary.shade};
      }
    `}));function Ft(e){const{width:n,children:s,onResize:o}=e,a=(0,z.l4)(),r=Fs(a),l=`${n+31.5}px`;return t.createElement(Ls,{className:(0,p.cx)(r.container,r.drawerActive),defaultSize:{width:l,height:`${a.components.horizontalDrawer.defaultHeight}px`},handleClasses:{top:r.rzHandle},enable:{top:!0,right:!1,bottom:!1,left:!1,topRight:!1,bottomRight:!1,bottomLeft:!1,topLeft:!1},maxHeight:"100vh",maxWidth:l,minWidth:l,onResize:o},s)}var Ns=i(21957),Ds=i(46835),Ps=i(18893),Hs=i(82090),Os=i(72062);function Ms(e){const{loading:n,width:s,onClose:o,queryResponse:a,timeZone:r}=e,l=a?.series||[];let c=a?.errors;!c?.length&&a?.error&&(c=[a.error]),(0,t.useEffect)(()=>{(0,H.ff)("grafana_explore_query_inspector_opened")},[]);const d={label:"Stats",value:"stats",icon:"chart-line",content:t.createElement(Hs.f,{data:a,timeZone:a?.request?.timezone})},h={label:"JSON",value:"json",icon:"brackets-curly",content:t.createElement(Ps.W,{data:a,onClose:o})},u={label:"Data",value:"data",icon:"database",content:t.createElement(Ns.E,{data:l,isLoading:n,options:{withTransforms:!1,withFieldConfig:!1},timeZone:r,app:Oe.zj.Explore})},m={label:"Query",value:"query",icon:"info-circle",content:t.createElement(Os.D,{data:l,onRefreshQuery:()=>e.runQueries(e.exploreId)})},g=[d,m,h,u];if(c?.length){const v={label:"Error",value:"error",icon:"exclamation-triangle",content:t.createElement(Ds.l,{errors:c})};g.push(v)}return t.createElement(Ft,{width:s},t.createElement(Ct.W,{tabs:g,onClose:o,closeIconTooltip:"Close query inspector"}))}function As(e,{exploreId:n}){const o=e.explore[n],{loading:a,queryResponse:r}=o;return{loading:a,queryResponse:r}}const $s={runQueries:L.aA},Bs=(0,ce.connect)(As,$s)(Ms);var Ws=i(53117),Ye=i(5562),Re=i(64319),Nt=i(26202),ks=i(41490),Us=i(35224),Qs=i(95379),Ne=i(77582),Dt=i(57465),Vs=i(64503),Zs=i(19349),Pt=i(54294),be=i(46519),Ht=i(21053),Gs=i(32873),Ot=i(5242),Me=i(6554);function js(e){const{onClick:n,isSynced:s}=e,o=()=>{const{isSynced:a}=e,r=a?"Unsync all views":"Sync all views to this time range";return t.createElement(t.Fragment,null,r)};return t.createElement(Me.u,{content:o,placement:"bottom"},t.createElement(Re.h,{icon:"link",variant:s?"active":"canvas","aria-label":s?"Synced times":"Unsynced times",onClick:n}))}class Ys extends t.Component{constructor(){super(...arguments),this.onMoveTimePicker=n=>{const{range:s,onChangeTime:o,timeZone:a}=this.props,{from:r,to:l}=(0,Ot.e)(n,s),c={from:(0,be.GY)(a,r),to:(0,be.GY)(a,l)};o(c)},this.onMoveForward=()=>this.onMoveTimePicker(1),this.onMoveBack=()=>this.onMoveTimePicker(-1),this.onChangeTimePicker=n=>{const s=Ht.isMathString(n.raw.from)?n.raw.from:n.from,o=Ht.isMathString(n.raw.to)?n.raw.to:n.to;this.props.onChangeTime({from:s,to:o}),(0,H.ff)("grafana_explore_time_picker_time_range_changed",{timeRangeFrom:s,timeRangeTo:o})},this.onZoom=()=>{const{range:n,onChangeTime:s,timeZone:o}=this.props,{from:a,to:r}=(0,Ot.h)(n,2),l={from:(0,be.GY)(o,a),to:(0,be.GY)(o,r)};s(l)}}render(){const{range:n,timeZone:s,fiscalYearStartMonth:o,splitted:a,syncedTimes:r,onChangeTimeSync:l,hideText:c,onChangeTimeZone:d,onChangeFiscalYearStartMonth:h}=this.props,u=a?t.createElement(js,{onClick:l,isSynced:r}):void 0,m={value:n,timeZone:s,fiscalYearStartMonth:o,onMoveBackward:this.onMoveBack,onMoveForward:this.onMoveForward,onZoom:this.onZoom,hideText:c};return t.createElement(Gs.a,{isOnCanvas:!0,...m,timeSyncButton:u,isSynced:r,widthOverride:a?window.innerWidth/2:void 0,onChange:this.onChangeTimePicker,onChangeTimeZone:d,onChangeFiscalYearStartMonth:h})}}var Mt=i(22163);function Ks(e){const{start:n,pause:s,resume:o,isLive:a,isPaused:r,stop:l,splitted:c}=e,d=a&&!r?"active":"canvas",h=a?r?o:s:n;return t.createElement(Nt.h,null,t.createElement(Me.u,{content:a&&!r?t.createElement(t.Fragment,null,"Pause the live stream"):t.createElement(t.Fragment,null,"Start live stream your logs"),placement:"bottom"},t.createElement(Re.h,{iconOnly:c,variant:d,icon:!a||r?"play":"pause",onClick:h},a&&r?"Paused":"Live")),t.createElement(Mt.Z,{mountOnEnter:!0,unmountOnExit:!0,timeout:100,in:a,classNames:{enter:Ke.stopButtonEnter,enterActive:Ke.stopButtonEnterActive,exit:Ke.stopButtonExit,exitActive:Ke.stopButtonExitActive}},t.createElement(Me.u,{content:t.createElement(t.Fragment,null,"Stop and exit the live stream"),placement:"bottom"},t.createElement(Re.h,{variant:d,onClick:l,icon:"square-shape"}))))}const Ke={stopButtonEnter:p.css`
    label: stopButtonEnter;
    width: 0;
    opacity: 0;
    overflow: hidden;
  `,stopButtonEnterActive:p.css`
    label: stopButtonEnterActive;
    opacity: 1;
    width: 32px;
  `,stopButtonExit:p.css`
    label: stopButtonExit;
    width: 32px;
    opacity: 1;
    overflow: hidden;
  `,stopButtonExitActive:p.css`
    label: stopButtonExitActive;
    opacity: 0;
    width: 0;
  `};var At=i(46526),Te=i(86297);function Js(e){const n=(0,R.useDispatch)(),s=(0,t.useCallback)(()=>{n((0,L.sQ)({exploreId:e,isPaused:!0}))},[e,n]),o=(0,t.useCallback)(()=>{n((0,L.sQ)({exploreId:e,isPaused:!1}))},[e,n]),a=(0,t.useCallback)(()=>{s(),n((0,Te.oz)(e,Ye.dP.offOption.value)),n((0,L.aA)(e))},[e,n,s]),r=(0,t.useCallback)(()=>{n((0,Te.oz)(e,Ye.dP.liveOption.value))},[e,n]);return{pause:s,resume:o,stop:a,start:r}}function $t(e){const n=Js(e.exploreId);return e.children(n)}const Xs=(0,t.lazy)(()=>i.e(2319).then(i.bind(i,2319)).then(({AddToDashboard:e})=>({default:e}))),qs=(e,n)=>({rotateIcon:(0,p.css)({"> div > svg":{transform:e==="left"&&n||e==="right"&&!n?"rotate(180deg)":"none"}})});class _s extends t.PureComponent{constructor(){super(...arguments),this.onChangeDatasource=async n=>{const{changeDatasource:s,exploreId:o}=this.props;s(o,n.uid,{importQueries:!0})},this.onRunQuery=(n=!1)=>{const{runQueries:s,cancelQueries:o,exploreId:a}=this.props;return n?o(a):s(a)},this.onChangeRefreshInterval=n=>{const{changeRefreshInterval:s,exploreId:o}=this.props;s(o,n)},this.onChangeTimeSync=()=>{const{syncTimes:n,exploreId:s}=this.props;n(s)},this.onCopyShortLink=async()=>{await(0,Dt.L)(window.location.href),(0,H.ff)("grafana_explore_shortened_link_clicked")},this.onOpenSplitView=()=>{const{split:n}=this.props;n(),(0,H.ff)("grafana_explore_split_view_opened",{origin:"menu"})},this.onCloseSplitView=()=>{const{closeSplit:n,exploreId:s}=this.props;n(s),(0,H.ff)("grafana_explore_split_view_closed")},this.renderRefreshPicker=n=>{const{loading:s,refreshInterval:o,isLive:a}=this.props;let r=s?"Cancel":"Run query",l,c="108px";return n&&(l=r,r=void 0,c="35px"),t.createElement(Ye.dP,{key:"refreshPicker",onIntervalChanged:this.onChangeRefreshInterval,value:o,isLoading:s,text:r,tooltip:l,intervals:(0,Zs.$t)().getValidIntervals(Ye.o5),isLive:a,onRefresh:()=>this.onRunQuery(s),noIntervalPicker:a,primary:!0,width:c})},this.renderActions=()=>{const{splitted:n,isLive:s,exploreId:o,range:a,timeZone:r,fiscalYearStartMonth:l,onChangeTime:c,syncedTimes:d,onChangeTimeZone:h,onChangeFiscalYearStartMonth:u,isPaused:m,hasLiveOption:g,containerWidth:v,largerExploreId:f}=this.props,y=n||v<1210,E=f===o,x=qs(o,E),w=Ne.Vt.hasAccess(R.AccessControlAction.DashboardsCreate,Ne.Vt.isEditor)||Ne.Vt.hasAccess(R.AccessControlAction.DashboardsWrite,Ne.Vt.isEditor),S=()=>{E?this.props.evenPaneResizeAction():this.props.maximizePaneAction({exploreId:o})};return[n?t.createElement(Nt.h,{key:"split-controls"},t.createElement(Re.h,{variant:"canvas",tooltip:`${E?"Narrow":"Widen"} pane`,onClick:S,icon:E?"gf-movepane-left":"gf-movepane-right",iconOnly:!0,className:x.rotateIcon}),t.createElement(Re.h,{tooltip:"Close split pane",onClick:this.onCloseSplitView,icon:"times",variant:"canvas"},"Close")):t.createElement(Re.h,{variant:"canvas",key:"split",tooltip:"Split the pane",onClick:this.onOpenSplitView,icon:"columns",disabled:s},"Split"),w&&t.createElement(t.Suspense,{key:"addToDashboard",fallback:null},t.createElement(Xs,{exploreId:o})),!s&&t.createElement(Ys,{key:"timeControls",exploreId:o,range:a,timeZone:r,fiscalYearStartMonth:l,onChangeTime:c,splitted:n,syncedTimes:d,onChangeTimeSync:this.onChangeTimeSync,hideText:y,onChangeTimeZone:h,onChangeFiscalYearStartMonth:u}),this.renderRefreshPicker(y),g&&t.createElement($t,{key:"liveControls",exploreId:o},C=>{const N={...C,start:()=>{(0,H.ff)("grafana_explore_logs_live_tailing_clicked",{datasourceType:this.props.datasourceType}),C.start()}};return t.createElement(Ks,{splitted:n,isLive:s,isPaused:m,start:N.start,pause:N.pause,resume:N.resume,stop:N.stop})})].filter(Boolean)}}render(){const{datasourceMissing:n,exploreId:s,splitted:o,containerWidth:a,topOfViewRef:r,refreshInterval:l,loading:c}=this.props,d=(o?a<700:a<800)||!1,h=T.v.featureToggles.topnav,u=t.createElement(Vs.u,{key:"share",tooltip:"Copy shortened link",icon:"share-alt",onClick:this.onCopyShortLink,"aria-label":"Copy shortened link"}),m=()=>!n&&t.createElement(Ws.q,{key:`${s}-ds-picker`,mixed:T.v.featureToggles.exploreMixedDatasource===!0,onChange:this.onChangeDatasource,current:this.props.datasourceRef,hideTextValue:d,width:d?8:void 0}),g=[!h&&s===F.Kd.left&&u,m()].filter(Boolean);return t.createElement("div",{ref:r},l&&t.createElement(ks.F,{func:this.onRunQuery,interval:l,loading:c}),h&&t.createElement("div",{ref:r},t.createElement(Qs.A,{actions:[u,t.createElement("div",{style:{flex:1},key:"spacer"})]})),t.createElement(Us.X,{"aria-label":"Explore toolbar",title:s===F.Kd.left&&!h?"Explore":void 0,pageIcon:s===F.Kd.left&&!h?"compass":void 0,leftItems:g,forceShowLeftItems:!0},this.renderActions()))}}const eo=(e,{exploreId:n})=>{const{syncedTimes:s,largerExploreId:o}=e.explore,a=e.explore[n],{datasourceInstance:r,datasourceMissing:l,range:c,refreshInterval:d,loading:h,isLive:u,isPaused:m,containerWidth:g}=a,v=!!r?.meta?.streaming;return{datasourceMissing:l,datasourceRef:r?.getRef(),datasourceType:r?.type,loading:h,range:c,timeZone:(0,Ie.Z)(e.user),fiscalYearStartMonth:(0,Ie.i)(e.user),splitted:(0,he.p)(e),refreshInterval:d,hasLiveOption:v,isLive:u,isPaused:m,syncedTimes:s,containerWidth:g,largerExploreId:o}},to={changeDatasource:At.zU,changeRefreshInterval:Te.oz,cancelQueries:L.ci,runQueries:L.aA,closeSplit:P.YV,split:P.bW,syncTimes:Te.mG,onChangeTimeZone:Pt.YT,onChangeFiscalYearStartMonth:Pt.rf,maximizePaneAction:P.nL,evenPaneResizeAction:P.AP},so=(0,ce.connect)(eo,to)(_s);var oo=i(8433);const no=e=>{const n=(0,z.wW)(s=>ao(s));return t.createElement("div",{className:n.container},t.createElement(oo.Z,{data:e.dataFrames[0],app:Oe.zj.Explore}))},ao=e=>({container:p.css`
    background: ${e.colors.background.primary};
    display: flow-root;
    padding: 0 ${e.spacing(1)} ${e.spacing(1)} ${e.spacing(1)};
    border: 1px solid ${e.components.panel.borderColor};
    border-radius: ${e.shape.borderRadius(1)};
  `});var we=i(2352),Bt=i(16983),ro=i(39778),io=i(66926),Je=i(36998),lo=i(89890),co=i(29431),uo=i(98825),$=i(40538),Ce=i(39904),V=i(31403),Wt=i(56703),po=i(91551),ho=i(39493);function mo(e,n){return(0,ho.ZP)(e,s=>{s.defaults.custom===void 0&&(s.defaults.custom={});const{custom:o}=s.defaults;switch(o.stacking===void 0&&(o.stacking={group:"A"}),n){case"lines":o.drawStyle=$.l8.Line,o.stacking.mode=$.o0.None,o.fillOpacity=0;break;case"bars":o.drawStyle=$.l8.Bars,o.stacking.mode=$.o0.None,o.fillOpacity=100;break;case"points":o.drawStyle=$.l8.Points,o.stacking.mode=$.o0.None,o.fillOpacity=0;break;case"stacked_lines":o.drawStyle=$.l8.Line,o.stacking.mode=$.o0.Normal,o.fillOpacity=100;break;case"stacked_bars":o.drawStyle=$.l8.Bars,o.stacking.mode=$.o0.Normal,o.fillOpacity=100;break;default:{const a=n;throw new Error(`Invalid graph-style: ${a}`)}}})}var go=function(e){return(e+1)%1e6};function fo(){var e=(0,t.useReducer)(go,0),n=e[1];return n}function Le(e,n){return typeof e=="function"?e.length?e(n):e():e}function yo(e){var n=(0,t.useRef)(Le(e)),s=fo();return(0,t.useMemo)(function(){return[function(){return n.current},function(o){n.current=Le(o,n.current),s()}]},[])}function vo(e,n,s){e===void 0&&(e=0),n===void 0&&(n=null),s===void 0&&(s=null);var o=Le(e);typeof o!="number"&&console.error("initialValue has to be a number, got "+typeof e),typeof s=="number"?o=Math.max(o,s):s!==null&&console.error("min has to be a number, got "+typeof s),typeof n=="number"?o=Math.min(o,n):n!==null&&console.error("max has to be a number, got "+typeof n);var a=yo(o),r=a[0],l=a[1];return[r(),(0,t.useMemo)(function(){var c=function(d){var h=r(),u=Le(d,h);h!==u&&(typeof s=="number"&&(u=Math.max(u,s)),typeof n=="number"&&(u=Math.min(u,n)),h!==u&&l(u))};return{get:r,set:c,inc:function(d){d===void 0&&(d=1);var h=Le(d,r());typeof h!="number"&&console.error("delta has to be a number or function returning a number, got "+typeof h),c(function(u){return u+h})},dec:function(d){d===void 0&&(d=1);var h=Le(d,r());typeof h!="number"&&console.error("delta has to be a number or function returning a number, got "+typeof h),c(function(u){return u-h})},reset:function(d){d===void 0&&(d=o);var h=Le(d,r());typeof h!="number"&&console.error("value has to be a number or function returning a number, got "+typeof h),o=h,c(h)}}},[o,s,n])]}var Eo=i(54291),kt=i(38067);function xo(e){const[n,{inc:s}]=vo(0),o=(0,Eo.Z)(e);return(0,t.useMemo)(()=>{o&&!(0,kt.nl)(e,o,kt.Ch)&&s()},[e,o,s]),n}const nt=20;function Ut({data:e,height:n,width:s,timeZone:o,absoluteRange:a,onChangeTime:r,loadingState:l,annotations:c,onHiddenSeriesChanged:d,splitOpenFn:h,graphStyle:u,tooltipDisplayMode:m=$.f3.Single,anchorToZero:g=!1,eventBus:v}){const f=(0,z.l4)(),y=(0,z.wW)(So),[E,x]=(0,t.useState)(!1),w={from:(0,be.CQ)(a.from),to:(0,be.CQ)(a.to),raw:{from:(0,be.CQ)(a.from),to:(0,be.CQ)(a.to)}},S=(0,t.useMemo)(()=>(0,ro.j)((0,Wt.F)(Wt.r),"Explore"),[]),[C,N]=(0,t.useState)({defaults:{min:g?0:void 0,color:{mode:io.S.PaletteClassic},custom:{drawStyle:$.l8.Line,fillOpacity:0,pointSize:5}},overrides:[]}),Q=(0,t.useMemo)(()=>mo(C,u),[C,u]),j=(0,t.useMemo)(()=>(0,Je.SM)({fieldConfig:Q,data:E?e:e.slice(0,nt),timeZone:o,replaceVariables:b=>b,theme:f,fieldConfigRegistry:S}),[S,e,o,f,Q,E]),ae=xo(j);(0,t.useEffect)(()=>{if(d){const b=[];j.forEach(k=>{k.fields.map(re=>re.config?.custom?.hideFrom?.viz).every(I.identity)&&b.push((0,lo.n)(k))}),d(b)}},[j,d]);const M={eventBus:v,sync:()=>co.m.Crosshair,onSplitOpen:h,onToggleSeriesVisibility(b,k){N((0,po.N)(b,k,C,e))}},O=(0,t.useMemo)(()=>({tooltip:{mode:m,sort:$.As.None},legend:{displayMode:$.jK.List,showLegend:!0,placement:"bottom",calcs:[]}}),[m]);return t.createElement(J._w,{value:M},e.length>nt&&!E&&t.createElement("div",{className:y.timeSeriesDisclaimer},t.createElement(Ce.J,{className:y.disclaimerIcon,name:"exclamation-triangle"}),"Showing only ",nt," time series.",t.createElement(V.zx,{variant:"primary",fill:"text",onClick:()=>x(!0),className:y.showAllButton},"Show all ",e.length)),t.createElement(uo.$,{data:{series:j,timeRange:w,state:l,annotations:c,structureRev:ae},pluginId:"timeseries",title:"",width:s,height:n,onChangeTimeRange:r,timeZone:o,options:O}))}const So=e=>({timeSeriesDisclaimer:p.css`
    label: time-series-disclaimer;
    margin: ${e.spacing(1)} auto;
    padding: 10px 0;
    text-align: center;
  `,disclaimerIcon:p.css`
    label: disclaimer-icon;
    color: ${e.colors.warning.main};
    margin-right: ${e.spacing(.5)};
  `,showAllButton:p.css`
    margin-left: ${e.spacing(.5)};
  `});var Qt=i(52081),Xe=i(2594);const bo=R.EXPLORE_GRAPH_STYLES.map(e=>({value:e,label:e[0].toUpperCase()+e.slice(1).replace(/_/," ")}));function wo(e){const{graphStyle:n,onChangeGraphStyle:s}=e;return t.createElement(Qt.Lh,{justify:"space-between",wrap:!0},"Graph",t.createElement(Xe.S,{size:"sm",options:bo,value:n,onChange:s}))}const Vt="grafana.explore.style.graph",mr=e=>{store.set(Vt,e)},Co=()=>To(le.Z.get(Vt)),Ro="lines",To=e=>R.EXPLORE_GRAPH_STYLES.find(s=>s===e)??Ro,Lo=({loading:e,data:n,eventBus:s,height:o,width:a,absoluteRange:r,timeZone:l,annotations:c,onChangeTime:d,splitOpenFn:h,loadingState:u})=>{const[m,g]=(0,t.useState)(Co),v=(0,z.l4)(),f=parseInt(v.spacing(2).slice(0,-2),10),y=(0,t.useCallback)(E=>{(0,Bt.FG)(E),g(E)},[]);return t.createElement(we.U,{label:t.createElement(wo,{graphStyle:m,onChangeGraphStyle:y}),loading:e,isOpen:!0},t.createElement(Ut,{graphStyle:m,data:n,height:o,width:a-f,absoluteRange:r,onChangeTime:d,timeZone:l,annotations:c,splitOpenFn:h,loadingState:u,eventBus:s}))};var Zt=i(28550),at=i(38849),zo=i(25287),Io=i(34282),Fo=i(78337),No=i(53739);const Gt=150,Do=({resetKey:e,humanize:n,className:s})=>{const[o,a]=(0,t.useState)(0);return(0,Fo.Z)(()=>a(o+Gt),Gt),(0,t.useEffect)(()=>a(0),[e]),t.createElement(No.q,{timeInMs:o,className:s,humanize:n})},Po=e=>({logsRowsLive:p.css`
    label: logs-rows-live;
    font-family: ${e.typography.fontFamilyMonospace};
    font-size: ${e.typography.bodySmall.fontSize};
    display: flex;
    flex-flow: column nowrap;
    height: 60vh;
    overflow-y: scroll;
    :first-child {
      margin-top: auto !important;
    }
  `,logsRowFade:p.css`
    label: logs-row-fresh;
    color: ${e.colors.text};
    background-color: ${(0,Zt.Z)(e.colors.info.transparent).setAlpha(.25).toString()};
    animation: fade 1s ease-out 1s 1 normal forwards;
    @keyframes fade {
      from {
        background-color: ${(0,Zt.Z)(e.colors.info.transparent).setAlpha(.25).toString()};
      }
      to {
        background-color: transparent;
      }
    }
  `,logsRowsIndicator:p.css`
    font-size: ${e.typography.h6.fontSize};
    padding-top: ${e.spacing(1)};
    display: flex;
    align-items: center;
  `,button:p.css`
    margin-right: ${e.spacing(1)};
  `,fullWidth:p.css`
    width: 100%;
  `});class Ho extends t.PureComponent{constructor(n){super(n),this.liveEndDiv=null,this.scrollContainerRef=t.createRef(),this.onScroll=s=>{const{isPaused:o,onPause:a}=this.props,{scrollTop:r,clientHeight:l,scrollHeight:c}=s.currentTarget;c-(r+l)>=5&&!o&&a()},this.rowsToRender=()=>{const{isPaused:s}=this.props;let{logRowsToRender:o=[]}=this.state;return s||(o=o.slice(-100)),o},this.state={logRowsToRender:n.logRows}}static getDerivedStateFromProps(n,s){return n.isPaused?null:{logRowsToRender:n.logRows}}render(){const{theme:n,timeZone:s,onPause:o,onResume:a,isPaused:r}=this.props,l=Po(n),{logsRow:c,logsRowLocalTime:d,logsRowMessage:h}=(0,Io.h)(n);return t.createElement("div",null,t.createElement("table",{className:l.fullWidth},t.createElement("tbody",{onScroll:r?void 0:this.onScroll,className:l.logsRowsLive,ref:this.scrollContainerRef},this.rowsToRender().map(u=>t.createElement("tr",{className:(0,p.cx)(c,l.logsRowFade),key:u.uid},t.createElement("td",{className:d},(0,at.dq)(u.timeEpochMs,{timeZone:s})),t.createElement("td",{className:h},u.hasAnsi?t.createElement(zo.Q,{value:u.raw}):u.entry))),t.createElement("tr",{ref:u=>{this.liveEndDiv=u,this.liveEndDiv&&this.scrollContainerRef.current?.scrollTo&&!r&&this.scrollContainerRef.current?.scrollTo(0,this.scrollContainerRef.current.scrollHeight)}}))),t.createElement("div",{className:l.logsRowsIndicator},t.createElement(V.zx,{variant:"secondary",onClick:r?a:o,className:l.button},t.createElement(Ce.J,{name:r?"play":"pause"}),"\xA0",r?"Resume":"Pause"),t.createElement(V.zx,{variant:"secondary",onClick:this.props.stopLive,className:l.button},t.createElement(Ce.J,{name:"square-shape",size:"lg",type:"mono"}),"\xA0 Exit live mode"),r||t.createElement("span",null,"Last line received: ",t.createElement(Do,{resetKey:this.props.logRows,humanize:!0})," ago")))}}const Oo=(0,z.HE)(Ho);var jt=i(55294),Mo=i(33180),Ao=i(11543),xe=i(81764),ze=i(8944),rt=i(37959),Yt=i(28947),$o=i(3574),Bo=i.n($o),it=i(38484),Wo=i(71698),ko=i(65066),Uo=i(54972),Qo=i(48955),Kt=i(81042);const Jt=e=>({metaContainer:p.css`
    flex: 1;
    color: ${e.colors.text.secondary};
    margin-bottom: ${e.spacing(2)};
    min-width: 30%;
    display: flex;
    flex-wrap: wrap;
  `,metaItem:p.css`
    margin-right: ${e.spacing(2)};
    margin-top: ${e.spacing(.5)};
    display: flex;
    align-items: center;

    .logs-meta-item__error {
      color: ${e.colors.error.text};
    }
  `,metaLabel:p.css`
    margin-right: calc(${e.spacing(2)} / 2);
    font-size: ${e.typography.bodySmall.fontSize};
    font-weight: ${e.typography.fontWeightMedium};
  `,metaValue:p.css`
    font-family: ${e.typography.fontFamilyMonospace};
    font-size: ${e.typography.bodySmall.fontSize};
  `}),Vo=(0,t.memo)(function(n){const s=(0,z.wW)(Jt),{label:o,value:a}=n;return t.createElement("div",{"data-testid":"meta-info-text-item",className:s.metaItem},o&&t.createElement("span",{className:s.metaLabel},o,":"),t.createElement("span",{className:s.metaValue},a))}),lt=(0,t.memo)(function(n){const s=(0,z.wW)(Jt),{metaItems:o}=n;return t.createElement("div",{className:s.metaContainer,"data-testid":"meta-info-text"},o.map((a,r)=>t.createElement(Vo,{key:`${r}-${a.label}`,label:a.label,value:a.value})))}),Zo=()=>({metaContainer:p.css`
    flex: 1;
    display: flex;
    flex-wrap: wrap;
  `});var Go=(e=>(e.Text="text",e.Json="json",e))(Go||{});const Xt=t.memo(({meta:e,dedupStrategy:n,dedupCount:s,displayedFields:o,clearDetectedFields:a,hasUnescapedContent:r,forceEscape:l,onEscapeNewlines:c,logRows:d})=>{const h=(0,z.wW)(Zo),u=v=>{switch((0,H.ff)("grafana_logs_download_logs_clicked",{app:Oe.zj.Explore,format:v,area:"logs-meta-row"}),v){case"text":(0,ko.Fc)({meta:e,rows:d},"Explore");break;case"json":const f=(0,Kt.Di)(d),y=new Blob([JSON.stringify(f)],{type:"application/json;charset=utf-8"}),E=`Explore-logs-${(0,at.dq)(new Date)}.json`;Bo()(y,E);break}},m=[...e];n!==$.Y4.none&&m.push({label:"Deduplication count",value:s,kind:G.Ku.Number}),d.some(v=>v.entry.length>Qo.n)&&m.push({label:"Info",value:"Logs with more than 100,000 characters could not be parsed and highlighted",kind:G.Ku.String}),o?.length>0&&m.push({label:"Showing only selected fields",value:qt(o,G.Ku.LabelsMap)},{label:"",value:t.createElement(V.zx,{variant:"secondary",size:"sm",onClick:a},"Show original line")}),r&&m.push({label:"Your logs might have incorrectly escaped content",value:t.createElement(Me.u,{content:"Fix incorrectly escaped newline and tab sequences in log lines. Manually review the results to confirm that the replacements are correct.",placement:"right"},t.createElement(V.zx,{variant:"secondary",size:"sm",onClick:c},l?"Remove escaping":"Escape newlines"))});const g=t.createElement(it.v,null,t.createElement(it.v.Item,{label:"txt",onClick:()=>u("text")}),t.createElement(it.v.Item,{label:"json",onClick:()=>u("json")}));return t.createElement(t.Fragment,null,m&&t.createElement("div",{className:h.metaContainer},t.createElement(lt,{metaItems:m.map(v=>({label:v.label,value:"kind"in v?qt(v.value,v.kind):v.value}))}),t.createElement(Wo.L,{overlay:g},t.createElement(Re.h,{isOpen:!1,variant:"canvas",icon:"download-alt"},"Download"))))});Xt.displayName="LogsMetaRow";function qt(e,n){return n===G.Ku.LabelsMap?t.createElement(Uo.j,{labels:e}):n===G.Ku.Error?t.createElement("span",{className:"logs-meta-item__error"},e):e}var ct=i(67487),jo=i(13211),Yo=i(47513);function Ko({pages:e,currentPageIndex:n,oldestLogsFirst:s,timeZone:o,loading:a,changeTime:r}){const l=u=>`${(0,at.dq)(u,{format:Yo.U6.interval.second,timeZone:o})}`,c=(u,m)=>{if(n===m&&a)return t.createElement(ct.$,null);const g=l(s?u.logsRange.from:u.logsRange.to),v=l(s?u.logsRange.to:u.logsRange.from);return`${g} \u2014 ${v}`},d=(0,z.l4)(),h=Jo(d,a);return t.createElement(xt.$,{autoHide:!0},t.createElement("div",{className:h.pagesWrapper,"data-testid":"logsNavigationPages"},t.createElement("div",{className:h.pagesContainer},e.map((u,m)=>t.createElement("button",{type:"button","data-testid":`page${m+1}`,className:(0,p.cx)((0,V.gN)(d),h.page),key:u.queryRange.to,onClick:()=>{(0,H.ff)("grafana_explore_logs_pagination_clicked",{pageType:"page",pageNumber:m+1}),!a&&r({from:u.queryRange.from,to:u.queryRange.to})}},t.createElement("div",{className:(0,p.cx)(h.line,{selectedBg:n===m})}),t.createElement("div",{className:(0,p.cx)(h.time,{selectedText:n===m})},c(u,m)))))))}const Jo=(e,n)=>({pagesWrapper:p.css`
      height: 100%;
      padding-left: ${e.spacing(.5)};
      display: flex;
      flex-direction: column;
      overflow-y: scroll;
      &::after {
        content: '';
        display: block;
        background: repeating-linear-gradient(
          135deg,
          ${e.colors.background.primary},
          ${e.colors.background.primary} 5px,
          ${e.colors.background.secondary} 5px,
          ${e.colors.background.secondary} 15px
        );
        width: 3px;
        height: inherit;
        margin-bottom: 8px;
      }
    `,pagesContainer:p.css`
      display: flex;
      padding: 0;
      flex-direction: column;
    `,page:p.css`
      display: flex;
      margin: ${e.spacing(2)} 0;
      cursor: ${n?"auto":"pointer"};
      white-space: normal;
      .selectedBg {
        background: ${e.colors.primary.main};
      }
      .selectedText {
        color: ${e.colors.primary.main};
      }
    `,line:p.css`
      width: 3px;
      height: 100%;
      align-items: center;
      background: ${e.colors.text.secondary};
    `,time:p.css`
      width: 60px;
      min-height: 80px;
      font-size: ${e.v1.typography.size.sm};
      padding-left: ${e.spacing(.5)};
      display: flex;
      align-items: center;
    `});function Xo({absoluteRange:e,logsSortOrder:n,timeZone:s,loading:o,onChangeTime:a,scrollToTopLogs:r,visibleRange:l,queries:c,clearCache:d,addResultsToCache:h}){const[u,m]=(0,t.useState)([]),[g,v]=(0,t.useState)(0),f=(0,t.useRef)(),y=(0,t.useRef)(),E=(0,t.useRef)(0),x=n===$.UV.Ascending,w=x?g===u.length-1:g===0,S=x?g===0:g===u.length-1,C=(0,z.l4)(),N=_o(C,x,o);(0,t.useEffect)(()=>{const O={logsRange:l,queryRange:e};let b=[];if(!(0,I.isEqual)(y.current,e)||!(0,I.isEqual)(f.current,c))d(),m([O]),v(0),f.current=c,E.current=e.to-e.from;else{m(q=>(b=q.filter(re=>!(0,I.isEqual)(O.queryRange,re.queryRange)),b=[...b,O].sort((re,et)=>j(re,et,n)),b));const k=b.findIndex(q=>q.queryRange.to===e.to);v(k)}h()},[l,e,n,c,d,h]),(0,t.useEffect)(()=>{d()},[]);const Q=({from:O,to:b})=>{y.current={from:O,to:b},a({from:O,to:b})},j=(O,b,k)=>k===$.UV.Ascending?O.queryRange.to>b.queryRange.to?1:-1:O.queryRange.to>b.queryRange.to?-1:1,ae=t.createElement(V.zx,{"data-testid":"olderLogsButton",className:N.navButton,variant:"secondary",onClick:()=>{if((0,H.ff)("grafana_explore_logs_pagination_clicked",{pageType:"olderLogsButton"}),S)Q({from:l.from-E.current,to:l.from});else{const O=x?-1:1;Q({from:u[g+O].queryRange.from,to:u[g+O].queryRange.to})}},disabled:o},t.createElement("div",{className:N.navButtonContent},o?t.createElement(ct.$,null):t.createElement(Ce.J,{name:x?"angle-up":"angle-down",size:"lg"}),"Older logs")),M=t.createElement(V.zx,{"data-testid":"newerLogsButton",className:N.navButton,variant:"secondary",onClick:()=>{if((0,H.ff)("grafana_explore_logs_pagination_clicked",{pageType:"newerLogsButton"}),!w){const O=x?1:-1;Q({from:u[g+O].queryRange.from,to:u[g+O].queryRange.to})}},disabled:o||w},t.createElement("div",{className:N.navButtonContent},o&&t.createElement(ct.$,null),w||o?null:t.createElement(Ce.J,{name:x?"angle-down":"angle-up",size:"lg"}),w?"Start of range":"Newer logs"));return t.createElement("div",{className:N.navContainer},x?ae:M,t.createElement(Ko,{pages:u,currentPageIndex:g,oldestLogsFirst:x,timeZone:s,loading:o,changeTime:Q}),x?M:ae,t.createElement(V.zx,{"data-testid":"scrollToTop",className:N.scrollToTopButton,variant:"secondary",onClick:r,title:"Scroll to top"},t.createElement(Ce.J,{name:"arrow-up",size:"lg"})))}const qo=(0,t.memo)(Xo),_o=(e,n,s)=>{const o=e.flags.topnav?`calc(100vh - 2*${e.spacing(2)} - 2*${jo.$}px)`:"95vh";return{navContainer:p.css`
      max-height: ${o};
      display: flex;
      flex-direction: column;
      justify-content: ${n?"flex-start":"space-between"};
      position: sticky;
      top: ${e.spacing(2)};
      right: 0;
    `,navButton:p.css`
      width: 58px;
      height: 68px;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      line-height: 1;
    `,navButtonContent:p.css`
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      width: 100%;
      height: 100%;
      white-space: normal;
    `,scrollToTopButton:p.css`
      width: 40px;
      height: 40px;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      margin-top: ${e.spacing(1)};
    `}};function en(e){const{width:n,timeZone:s,splitOpen:o,onUpdateTimeRange:a,onHiddenSeriesChanged:r}=e,l=(0,z.l4)(),c=(0,z.wW)(tn),d=parseInt(l.spacing(2).slice(0,-2),10),h=150;if(e.logsVolumeData===void 0)return null;const u=e.logsVolumeData,m=(0,G.Js)(u?.data);let g=m?`${m.name}`:"";(0,G.FP)(u.data)&&(g=[g,"This datasource does not support full-range histograms. The graph below is based on the logs seen in the response."].filter(I.identity).join(". "));const v=(0,G.FP)(u.data)?(0,G.a6)(u.data,e.absoluteRange):e.absoluteRange;let f;u?.data&&(u.data.length>0?f=t.createElement(Ut,{graphStyle:"lines",loadingState:u.state??D.Gu.Done,data:u.data,height:h,width:n-d*2,absoluteRange:v,onChangeTime:a,timeZone:s,splitOpenFn:o,tooltipDisplayMode:$.f3.Multi,onHiddenSeriesChanged:r,anchorToZero:!0,eventBus:e.eventBus}):f=t.createElement("span",null,"No volume data."));let y=t.createElement("span",null,g);return u.state===D.Gu.Streaming&&(y=t.createElement(t.Fragment,null,y,t.createElement(Me.u,{content:"Streaming"},t.createElement(Ce.J,{name:"circle-mono",size:"md",className:c.streaming,"data-testid":"logs-volume-streaming"})))),t.createElement("div",{style:{height:h},className:c.contentContainer},f,y&&t.createElement("div",{className:c.extraInfoContainer},y))}const tn=e=>({extraInfoContainer:p.css`
      display: flex;
      justify-content: end;
      position: absolute;
      right: 5px;
      top: -10px;
      font-size: ${e.typography.bodySmall.fontSize};
      color: ${e.colors.text.secondary};
    `,contentContainer:p.css`
      display: flex;
      align-items: center;
      justify-content: center;
      position: relative;
    `,streaming:p.css`
      color: ${e.colors.success.text};
    `});function dt(e){const[n,s]=(0,t.useState)(!1),o=100,{error:a,title:r,suggestedAction:l,onSuggestedAction:c,onRemove:d,severity:h="warning"}=e,u=a?.message||a?.data?.message||"",m=!n&&u.length>o;return t.createElement(Qe.b,{title:r,severity:h,onRemove:d},m?t.createElement(V.zx,{variant:"secondary",size:"xs",onClick:()=>{s(!0)}},"Show details"):u,l&&c&&t.createElement(V.zx,{variant:"primary",size:"xs",onClick:c},l))}function sn(e){return!e||!e.error&&!e.errors?!1:(e.error?[e.error]:e.errors||[]).some(s=>(`${s.message||s.data?.message}`?.toLowerCase()).includes("timeout"))}const on=({logsVolumeData:e,absoluteRange:n,onUpdateTimeRange:s,width:o,onLoadLogsVolume:a,onHiddenSeriesChanged:r,eventBus:l,splitOpen:c,timeZone:d,onClose:h})=>{const u=(0,t.useMemo)(()=>{const y=(0,I.groupBy)(e?.data||[],"meta.custom.datasourceName");return(0,I.mapValues)(y,E=>(0,Kt._c)(E))},[e]),m=(0,z.wW)(nn),g=Object.keys(u).length,v=Object.values(u).some(y=>{const E=an(y,n);return!(0,G.FP)(y)&&E&&E<1}),f=sn(e);return e?.state===D.Gu.Loading?t.createElement("span",null,"Loading..."):f?t.createElement(dt,{title:"The logs volume query is taking too long and has timed out",severity:"info",suggestedAction:"Retry",onSuggestedAction:a,onRemove:h}):e?.error!==void 0?t.createElement(dt,{error:e.error,title:"Failed to load log volume for this query"}):t.createElement("div",{className:m.listContainer},Object.keys(u).map((y,E)=>{const x={data:u[y]};return t.createElement(en,{key:E,absoluteRange:n,width:o,logsVolumeData:x,onUpdateTimeRange:s,timeZone:d,splitOpen:c,onLoadLogsVolume:a,onHiddenSeriesChanged:g>1?()=>{}:r,eventBus:l})}),v&&t.createElement("div",{className:m.extraInfoContainer},t.createElement(xe._,{label:"Reload log volume",transparent:!0},t.createElement(V.zx,{size:"xs",icon:"sync",variant:"secondary",onClick:a,id:"reload-volume"}))))},nn=e=>({listContainer:p.css`
      padding-top: 10px;
    `,extraInfoContainer:p.css`
      display: flex;
      justify-content: end;
      position: absolute;
      right: 5px;
      top: 5px;
    `,oldInfoText:p.css`
      font-size: ${e.typography.bodySmall.fontSize};
      color: ${e.colors.text.secondary};
    `});function an(e,n){const s=e&&e[0]&&e[0].meta?.custom?.absoluteRange;return s?(n.from-n.to)/(s.from-s.to):void 0}const pe={showLabels:"grafana.explore.logs.showLabels",showTime:"grafana.explore.logs.showTime",wrapLogMessage:"grafana.explore.logs.wrapLogMessage",prettifyLogMessage:"grafana.explore.logs.prettifyLogMessage",logsSortOrder:"grafana.explore.logs.sortOrder"},rn=p.css`
  & > div {
    overflow: visible;
    & > div {
      overflow: visible;
    }
  }
`,ln=[$.Y4.none,$.Y4.exact,$.Y4.numbers,$.Y4.signature];class cn extends t.PureComponent{constructor(n){super(n),this.topLogsRef=(0,t.createRef)(),this.state={showLabels:le.Z.getBool(pe.showLabels,!1),showTime:le.Z.getBool(pe.showTime,!0),wrapLogMessage:le.Z.getBool(pe.wrapLogMessage,!0),prettifyLogMessage:le.Z.getBool(pe.prettifyLogMessage,!1),dedupStrategy:$.Y4.none,hiddenLogLevels:[],logsSortOrder:le.Z.get(pe.logsSortOrder)||$.UV.Descending,isFlipping:!1,displayedFields:[],forceEscape:!1},this.onLogRowHover=s=>{s?this.props.eventBus.publish(new jt.es({point:{time:s.timeEpochMs}})):this.props.eventBus.publish(new jt.xH)},this.onChangeLogsSortOrder=()=>{this.setState({isFlipping:!0}),this.flipOrderTimer=window.setTimeout(()=>{this.setState(s=>{const o=s.logsSortOrder===$.UV.Descending?$.UV.Ascending:$.UV.Descending;return le.Z.set(pe.logsSortOrder,o),{logsSortOrder:o}})},0),this.cancelFlippingTimer=window.setTimeout(()=>this.setState({isFlipping:!1}),1e3)},this.onEscapeNewlines=()=>{this.setState(s=>({forceEscape:!s.forceEscape}))},this.onChangeDedup=s=>{(0,H.ff)("grafana_explore_logs_deduplication_clicked",{deduplicationType:s,datasourceType:this.props.datasourceType}),this.setState({dedupStrategy:s})},this.onChangeLabels=s=>{const{target:o}=s;if(o){const a=o.checked;this.setState({showLabels:a}),le.Z.set(pe.showLabels,a)}},this.onChangeTime=s=>{const{target:o}=s;if(o){const a=o.checked;this.setState({showTime:a}),le.Z.set(pe.showTime,a)}},this.onChangeWrapLogMessage=s=>{const{target:o}=s;if(o){const a=o.checked;this.setState({wrapLogMessage:a}),le.Z.set(pe.wrapLogMessage,a)}},this.onChangePrettifyLogMessage=s=>{const{target:o}=s;if(o){const a=o.checked;this.setState({prettifyLogMessage:a}),le.Z.set(pe.prettifyLogMessage,a)}},this.onToggleLogLevel=s=>{const o=s.map(a=>G.in[a]);this.setState({hiddenLogLevels:o})},this.onToggleLogsVolumeCollapse=s=>{this.props.onSetLogsVolumeEnabled(s),(0,H.ff)("grafana_explore_logs_histogram_toggle_clicked",{datasourceType:this.props.datasourceType,type:s?"open":"close"})},this.onClickScan=s=>{s.preventDefault(),this.props.onStartScanning&&this.props.onStartScanning()},this.onClickStopScan=s=>{s.preventDefault(),this.props.onStopScanning&&this.props.onStopScanning()},this.showField=s=>{this.state.displayedFields.indexOf(s)===-1&&this.setState(a=>({displayedFields:a.displayedFields.concat(s)}))},this.hideField=s=>{this.state.displayedFields.indexOf(s)>-1&&this.setState(a=>({displayedFields:a.displayedFields.filter(r=>s!==r)}))},this.clearDetectedFields=()=>{this.setState(s=>({displayedFields:[]}))},this.checkUnescapedContent=(0,ue.Z)(s=>!!s.some(o=>o.hasUnescapedContent)),this.dedupRows=(0,ue.Z)((s,o)=>{const a=(0,rt.UW)(s,o),r=a.reduce((l,c)=>c.duplicates?l+c.duplicates:l,0);return{dedupedRows:a,dedupCount:r}}),this.filterRows=(0,ue.Z)((s,o)=>(0,rt.nu)(s,new Set(o))),this.createNavigationRange=(0,ue.Z)(s=>{if(!s||s.length===0)return;const o=s[0].timeEpochMs,a=s[s.length-1].timeEpochMs;return a<o?{from:a,to:o}:{from:o,to:a}}),this.scrollToTopLogs=()=>this.topLogsRef.current?.scrollIntoView(),this.logsVolumeEventBus=n.eventBus.newScopedBus("logsvolume",{onlyLocal:!1})}componentWillUnmount(){this.flipOrderTimer&&window.clearTimeout(this.flipOrderTimer),this.cancelFlippingTimer&&window.clearTimeout(this.cancelFlippingTimer)}render(){const{width:n,splitOpen:s,logRows:o,logsMeta:a,logsVolumeEnabled:r,logsVolumeData:l,loadLogsVolumeData:c,loading:d=!1,onClickFilterLabel:h,onClickFilterOutLabel:u,timeZone:m,scanning:g,scanRange:v,showContextToggle:f,absoluteRange:y,onChangeTime:E,getFieldLinks:x,theme:w,logsQueries:S,clearCache:C,addResultsToCache:N,exploreId:Q,scrollElement:j,getRowContext:ae,getLogRowContextUi:M}=this.props,{showLabels:O,showTime:b,wrapLogMessage:k,prettifyLogMessage:q,dedupStrategy:re,hiddenLogLevels:et,logsSortOrder:vt,isFlipping:nr,displayedFields:hs,forceEscape:ms}=this.state,ie=un(w,k),ar=o&&o.length>0,rr=this.checkUnescapedContent(o),ir=this.filterRows(o,et),{dedupedRows:lr,dedupCount:cr}=this.dedupRows(ir,re),dr=this.createNavigationRange(o),ur=v?`Scanning ${Mo.describeTimeRange(v)}`:"Scanning...";return t.createElement(t.Fragment,null,t.createElement(we.U,{label:"Logs volume",collapsible:!0,isOpen:r,onToggle:this.onToggleLogsVolumeCollapse},r&&t.createElement(on,{absoluteRange:y,width:n,logsVolumeData:l,onUpdateTimeRange:E,timeZone:m,splitOpen:s,onLoadLogsVolume:c,onHiddenSeriesChanged:this.onToggleLogLevel,eventBus:this.logsVolumeEventBus,onClose:()=>this.onToggleLogsVolumeCollapse(!1)})),t.createElement(we.U,{label:"Logs",loading:d,isOpen:!0,className:rn},t.createElement("div",{className:ie.logOptions,ref:this.topLogsRef},t.createElement(Ao.Z,null,t.createElement(xe._,{label:"Time",className:ie.horizontalInlineLabel,transparent:!0},t.createElement(ze.x,{value:b,onChange:this.onChangeTime,className:ie.horizontalInlineSwitch,transparent:!0,id:`show-time_${Q}`})),t.createElement(xe._,{label:"Unique labels",className:ie.horizontalInlineLabel,transparent:!0},t.createElement(ze.x,{value:O,onChange:this.onChangeLabels,className:ie.horizontalInlineSwitch,transparent:!0,id:`unique-labels_${Q}`})),t.createElement(xe._,{label:"Wrap lines",className:ie.horizontalInlineLabel,transparent:!0},t.createElement(ze.x,{value:k,onChange:this.onChangeWrapLogMessage,className:ie.horizontalInlineSwitch,transparent:!0,id:`wrap-lines_${Q}`})),t.createElement(xe._,{label:"Prettify JSON",className:ie.horizontalInlineLabel,transparent:!0},t.createElement(ze.x,{value:q,onChange:this.onChangePrettifyLogMessage,className:ie.horizontalInlineSwitch,transparent:!0,id:`prettify_${Q}`})),t.createElement(xe._,{label:"Deduplication",className:ie.horizontalInlineLabel,transparent:!0},t.createElement(Xe.S,{options:ln.map(Et=>({label:(0,I.capitalize)(Et),value:Et,description:G.Uv[Et]})),value:re,onChange:this.onChangeDedup,className:ie.radioButtons}))),t.createElement("div",null,t.createElement(xe._,{label:"Display results",className:ie.horizontalInlineLabel,transparent:!0},t.createElement(Xe.S,{disabled:nr,options:[{label:"Newest first",value:$.UV.Descending,description:"Show results newest to oldest"},{label:"Oldest first",value:$.UV.Ascending,description:"Show results oldest to newest"}],value:vt,onChange:this.onChangeLogsSortOrder,className:ie.radioButtons})))),t.createElement(Xt,{logRows:o,meta:a||[],dedupStrategy:re,dedupCount:cr,hasUnescapedContent:rr,forceEscape:ms,displayedFields:hs,onEscapeNewlines:this.onEscapeNewlines,clearDetectedFields:this.clearDetectedFields}),t.createElement("div",{className:ie.logsSection},t.createElement("div",{className:ie.logRows,"data-testid":"logRows"},t.createElement(Yt.w,{logRows:o,deduplicatedRows:lr,dedupStrategy:re,getRowContext:ae,getLogRowContextUi:M,onClickFilterLabel:h,onClickFilterOutLabel:u,showContextToggle:f,showLabels:O,showTime:b,enableLogDetails:!0,forceEscape:ms,wrapLogMessage:k,prettifyLogMessage:q,timeZone:m,getFieldLinks:x,logsSortOrder:vt,displayedFields:hs,onClickShowField:this.showField,onClickHideField:this.hideField,app:Oe.zj.Explore,scrollElement:j,onLogRowHover:this.onLogRowHover}),!d&&!ar&&!g&&t.createElement("div",{className:ie.noData},"No logs found.",t.createElement(V.zx,{size:"sm",variant:"secondary",onClick:this.onClickScan},"Scan for older logs")),g&&t.createElement("div",{className:ie.noData},t.createElement("span",null,ur),t.createElement(V.zx,{size:"sm",variant:"secondary",onClick:this.onClickStopScan},"Stop scan"))),t.createElement(qo,{logsSortOrder:vt,visibleRange:dr??y,absoluteRange:y,timeZone:m,onChangeTime:E,loading:d,queries:S??[],scrollToTopLogs:this.scrollToTopLogs,addResultsToCache:N,clearCache:C}))))}}const dn=(0,z.HE)(cn),un=(e,n)=>({noData:p.css`
      > * {
        margin-left: 0.5em;
      }
    `,logOptions:p.css`
      display: flex;
      justify-content: space-between;
      align-items: baseline;
      flex-wrap: wrap;
      background-color: ${e.colors.background.primary};
      padding: ${e.spacing(1,2)};
      border-radius: ${e.shape.borderRadius()};
      margin: ${e.spacing(0,0,1)};
      border: 1px solid ${e.colors.border.medium};
    `,headerButton:p.css`
      margin: ${e.spacing(.5,0,0,1)};
    `,horizontalInlineLabel:p.css`
      > label {
        margin-right: 0;
      }
    `,horizontalInlineSwitch:p.css`
      padding: 0 ${e.spacing(1)} 0 0;
    `,radioButtons:p.css`
      margin: 0;
    `,logsSection:p.css`
      display: flex;
      flex-direction: row;
      justify-content: space-between;
    `,logRows:p.css`
      overflow-x: ${n?"unset":"scroll"};
      overflow-y: visible;
      width: 100%;
    `}),ut=500,pt=100,pn=(0,ue.Z)(()=>({logsEnter:p.css`
      label: logsEnter;
      position: absolute;
      opacity: 0;
      height: auto;
      width: 100%;
    `,logsEnterActive:p.css`
      label: logsEnterActive;
      opacity: 1;
      transition: opacity ${ut}ms ease-out ${pt}ms;
    `,logsExit:p.css`
      label: logsExit;
      position: absolute;
      opacity: 1;
      height: auto;
      width: 100%;
    `,logsExitActive:p.css`
      label: logsExitActive;
      opacity: 0;
      transition: opacity ${ut}ms ease-out ${pt}ms;
    `}));function _t(e){const{visible:n,children:s}=e,o=pn();return t.createElement(Mt.Z,{in:n,mountOnEnter:!0,unmountOnExit:!0,timeout:ut+pt,classNames:{enter:o.logsEnter,enterActive:o.logsEnterActive,exit:o.logsExit,exitActive:o.logsExitActive}},s)}var qe=i(41611);class hn extends t.PureComponent{constructor(){super(...arguments),this.onChangeTime=n=>{const{exploreId:s,updateTimeRange:o}=this.props;o({exploreId:s,absoluteRange:n})},this.getLogRowContext=async(n,s)=>{const{datasourceInstance:o,logsQueries:a}=this.props;if((0,G.Q4)(o)){const r=(a??[]).find(l=>l.refId===n.dataFrame.refId&&l.datasource!=null&&l.datasource.type===o.type);return o.getLogRowContext(n,s,r)}return[]},this.getLogRowContextUi=(n,s)=>{const{datasourceInstance:o}=this.props;return(0,G.xW)(o)&&o.getLogRowContextUi?o.getLogRowContextUi(n,s):t.createElement(t.Fragment,null)},this.showContextToggle=n=>{const{datasourceInstance:s}=this.props;return(0,G.Q4)(s)?s.showContextToggle(n):!1},this.getFieldLinks=(n,s,o)=>{const{splitOpenFn:a,range:r}=this.props;return(0,qe.a_)({field:n,rowIndex:s,splitOpenFn:a,range:r,dataFrame:o})}}render(){const{loading:n,loadingState:s,logRows:o,logsMeta:a,logsSeries:r,logsQueries:l,loadSupplementaryQueryData:c,setSupplementaryQueryEnabled:d,onClickFilterLabel:h,onClickFilterOutLabel:u,onStartScanning:m,onStopScanning:g,absoluteRange:v,timeZone:f,visibleRange:y,scanning:E,range:x,width:w,splitOpenFn:S,isLive:C,exploreId:N,addResultsToCache:Q,clearCache:j,scrollElement:ae,logsVolume:M}=this.props;return o?t.createElement(t.Fragment,null,t.createElement(_t,{visible:C},t.createElement(we.U,{label:"Logs",loading:!1,isOpen:!0},t.createElement($t,{exploreId:N},O=>t.createElement(Oo,{logRows:o,timeZone:f,stopLive:O.stop,isPaused:this.props.isPaused,onPause:O.pause,onResume:O.resume})))),t.createElement(_t,{visible:!C},t.createElement(dn,{exploreId:N,datasourceType:this.props.datasourceInstance?.type,logRows:o,logsMeta:a,logsSeries:r,logsVolumeEnabled:M.enabled,onSetLogsVolumeEnabled:O=>d(N,O,G.v8.LogsVolume),logsVolumeData:M.data,logsQueries:l,width:w,splitOpen:S,loading:n,loadingState:s,loadLogsVolumeData:()=>c(N,G.v8.LogsVolume),onChangeTime:this.onChangeTime,onClickFilterLabel:h,onClickFilterOutLabel:u,onStartScanning:m,onStopScanning:g,absoluteRange:v,visibleRange:y,timeZone:f,scanning:E,scanRange:x.raw,showContextToggle:this.showContextToggle,getRowContext:this.getLogRowContext,getLogRowContextUi:this.getLogRowContextUi,getFieldLinks:this.getFieldLinks,addResultsToCache:()=>Q(N),clearCache:()=>j(N),scrollElement:ae,eventBus:this.props.eventBus}))):null}}function mn(e,{exploreId:n}){const o=e.explore[n],{logsResult:a,loading:r,scanning:l,datasourceInstance:c,isLive:d,isPaused:h,range:u,absoluteRange:m,supplementaryQueries:g}=o,v=(0,Ie.Z)(e.user),f=g[G.v8.LogsVolume];return{loading:r,logRows:a?.rows,logsMeta:a?.meta,logsSeries:a?.series,logsQueries:a?.queries,visibleRange:a?.visibleRange,scanning:l,timeZone:v,datasourceInstance:c,isLive:d,isPaused:h,range:u,absoluteRange:m,logsVolume:f}}const gn={updateTimeRange:Te.cD,addResultsToCache:L.K8,clearCache:L.LK,loadSupplementaryQueryData:L.W1,setSupplementaryQueryEnabled:L.z0},fn=(0,ce.connect)(mn,gn)(hn);function yn(e){const{queryResponse:n,timeZone:s,enabled:o,setLogsSampleEnabled:a,datasourceInstance:r,queries:l,splitOpen:c}=e,d=(0,z.wW)(vn),h=g=>{a(g),(0,H.ff)("grafana_explore_logs_sample_toggle_clicked",{datasourceType:r?.type??"unknown",type:g?"open":"close"})},u=()=>{if(!(0,G.mN)(r,G.v8.LogsSample))return null;const g=l.map(f=>r.getSupplementaryQuery(G.v8.LogsSample,f)).filter(f=>!!f);if(!g.length)return null;const v=()=>{c({queries:g,datasourceUid:r.uid}),(0,H.ff)("grafana_explore_logs_sample_split_button_clicked",{datasourceType:r?.type??"unknown",queriesCount:g.length})};return t.createElement(V.zx,{size:"sm",className:d.logSamplesButton,onClick:v},"Open logs in split view")};let m;if(n===void 0)m=null;else if(n.error!==void 0)m=t.createElement(dt,{error:n.error,title:"Failed to load logs sample for this query"});else if(n.state===D.Gu.Loading)m=t.createElement("span",null,"Logs sample is loading...");else if(n.data.length===0||n.data[0].length===0)m=t.createElement("span",null,"No logs sample data.");else{const g=(0,rt.aB)(n.data);m=t.createElement(t.Fragment,null,t.createElement(u,null),t.createElement("div",{className:d.logContainer},t.createElement(Yt.w,{logRows:g.rows,dedupStrategy:$.Y4.none,showLabels:le.Z.getBool(pe.showLabels,!1),showTime:le.Z.getBool(pe.showTime,!0),wrapLogMessage:le.Z.getBool(pe.wrapLogMessage,!0),prettifyLogMessage:le.Z.getBool(pe.prettifyLogMessage,!1),timeZone:s,enableLogDetails:!0})))}return n?.state!==D.Gu.NotStarted?t.createElement(we.U,{label:"Logs sample",isOpen:o,collapsible:!0,onToggle:h},m):null}const vn=e=>({logSamplesButton:p.css`
    position: absolute;
    top: ${e.spacing(1)};
    right: ${e.spacing(1)};
  `,logContainer:p.css`
    overflow-x: scroll;
  `}),En=()=>{const e=(0,z.wW)(xn);return t.createElement(t.Fragment,null,t.createElement(St.l,{"data-testid":"explore-no-data",className:e.wrapper},t.createElement("span",{className:e.message},"No data")))},xn=e=>({wrapper:p.css`
    label: no-data-card;
    padding: ${e.spacing(3)};
    background: ${e.colors.background.primary};
    border-radius: ${e.shape.radius.default};
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    flex-grow: 1;
  `,message:p.css`
    font-size: ${e.typography.h2.fontSize};
    padding: ${e.spacing(4)};
    color: ${e.colors.text.disabled};
  `});var Sn=i(97097);const bn=()=>{const e=(0,z.l4)(),n=Ne.Vt.hasPermission(R.AccessControlAction.DataSourcesCreate)&&Ne.Vt.hasPermission(R.AccessControlAction.DataSourcesWrite),s="Explore requires at least one data source. Once you have added a data source, you can query it here.",o=t.createElement(t.Fragment,null,t.createElement(Ce.J,{name:"rocket"}),t.createElement(t.Fragment,null," ProTip: You can also define data sources through configuration files. "),t.createElement("a",{href:"http://docs.grafana.org/administration/provisioning/#datasources?utm_source=explore",target:"_blank",rel:"noreferrer",className:"text-link"},"Learn more")),a=t.createElement(V.Qj,{size:"lg",href:"datasources/new",icon:"database",disabled:!n},"Add data source"),r=p.css`
    max-width: ${e.breakpoints.values.lg}px;
    margin-top: ${e.spacing(2)};
    align-self: center;
  `;return t.createElement(Sn._,{callToActionElement:a,className:r,footer:o,message:s})};var wn=i(86253),Cn=i(74538),Rn=i(85506);const Tn=e=>({warningText:p.css`
    label: warningText;
    font-size: ${e.typography.bodySmall.fontSize};
    color: ${e.colors.text.secondary};
  `});function Ln(e){const{dataFrames:n,range:s,splitOpenFn:o,withTraceView:a,datasourceType:r}=e,l=(0,qe.u8)(s,o),c=(0,z.l4)(),d=(0,z.wW)(Tn),h=(0,Je.SM)({fieldConfig:{defaults:{},overrides:[]},data:n,replaceVariables:C=>C,theme:c}),{nodes:u}=(0,Rn.Y)(h),[m,g]=(0,wn.Z)(!1),v=()=>{g(),(0,H.ff)("grafana_traces_node_graph_panel_clicked",{datasourceType:r,grafana_version:T.v.buildInfo.version,isExpanded:!m})},{height:f}=(0,se.Z)(),y=(0,t.useRef)(null),[E,x]=(0,t.useState)(250);(0,t.useEffect)(()=>{if(y.current){const{top:C}=y.current.getBoundingClientRect();x(C)}},[y]);const w=f-E-32,S=a&&u[0]?.length>1e3?t.createElement("span",{className:d.warningText}," (",u[0].length," nodes, can be slow to load)"):null;return t.createElement(we.U,{label:t.createElement("span",null,"Node graph",S," "),collapsible:a,isOpen:a?m:!0,onToggle:a?()=>v():void 0},t.createElement("div",{ref:y,style:a?{height:500}:{minHeight:600,height:w}},t.createElement(Cn.E,{dataFrames:h,getLinks:l})))}function zn(e,{exploreId:n}){return{range:e.explore[n].range}}const In=(0,ce.connect)(zn,{})(Ln);var Ae=i(90158),Fn=i(99822),Nn=i(84457);const Dn=e=>{const n=(0,he.F)(e);return{getQueries:(0,Ae.P1)(n,s=>s.queries),getQueryResponse:(0,Ae.P1)(n,s=>s.queryResponse),getHistory:(0,Ae.P1)(n,s=>s.history),getEventBridge:(0,Ae.P1)(n,s=>s.eventBridge),getDatasourceInstanceSettings:(0,Ae.P1)(n,s=>(0,ke.ak)().getInstanceSettings(s.datasourceInstance?.uid))}},Pn=({exploreId:e})=>{const n=(0,R.useDispatch)(),{getQueries:s,getDatasourceInstanceSettings:o,getQueryResponse:a,getHistory:r,getEventBridge:l}=(0,t.useMemo)(()=>Dn(e),[e]),c=(0,R.useSelector)(s),d=(0,R.useSelector)(o),h=(0,R.useSelector)(a),u=(0,R.useSelector)(r),m=(0,R.useSelector)(l),g=(0,t.useCallback)(()=>{n((0,L.aA)(e))},[n,e]),v=(0,t.useCallback)(w=>{n((0,L.qV)({exploreId:e,queries:w}))},[n,e]),f=(0,t.useCallback)(w=>{v([...c,{...w,refId:(0,Fn.Hs)(c)}])},[v,c]),y=()=>{(0,H.ff)("grafana_explore_query_row_copy")},E=()=>{(0,H.ff)("grafana_explore_query_row_remove")},x=w=>{(0,H.ff)("grafana_query_row_toggle",w===void 0?{}:{queryEnabled:w})};return t.createElement(Nn.l,{dsSettings:d,queries:c,onQueriesChange:v,onAddQuery:f,onRunQueries:g,onQueryCopied:y,onQueryRemoved:E,onQueryToggled:x,data:h,app:Oe.zj.Explore,history:u,eventBus:m})};var es=i(88144),ts=i(84952),Hn=i(75478),ss=i(24799),On=i(20112);const ht=" ",Mn=e=>{const n={},s=[],o=e.fields.filter(l=>!["Time"].includes(l.name));let a=o.find(l=>l.name==="__name__")?.values.toArray()??[];!a.length&&o.length&&o[0].values.length&&(a=Array(o[0].values.length).fill(""));const r=e.fields.filter(l=>!["__name__"].includes(l.name));return a.forEach(function(l,c){n[l]={};const d=n[l][c]??{};for(const h of r){const u=h.name;if(u!=="Time")if(typeof h?.display=="function"){const m=(0,On.zc)(h?.display(h.values.get(c)));m?d[u]=m:u.includes("Value #")&&(d[u]=ht)}else console.warn("Field display method is missing!")}s.push({...d,__name__:l})}),s};var os=i(8366),An=i(3823),De=i(8180);const $n=(e,n)=>({rowWrapper:p.css`
    position: relative;
    min-width: ${_e};
    padding-right: 5px;
  `,rowValue:p.css`
    white-space: nowrap;
    overflow-x: auto;
    -ms-overflow-style: none; /* IE and Edge */
    scrollbar-width: none; /* Firefox */
    display: block;
    padding-right: 10px;

    &::-webkit-scrollbar {
      display: none; /* Chrome, Safari and Opera */
    }

    &:before {
      pointer-events: none;
      content: '';
      width: 100%;
      height: 100%;
      position: absolute;
      left: 0;
      top: 0;
      background: linear-gradient(to right, transparent calc(100% - 25px), ${e.colors.background.primary});
    }
  `,rowValuesWrap:p.css`
    padding-left: ${as};
    width: calc(${n} * ${_e});
    display: flex;
  `}),Bn=({totalNumberOfValues:e,values:n,hideFieldsWithoutValues:s})=>{const o=(0,z.wW)(a=>$n(a,e));return t.createElement("div",{role:"cell",className:o.rowValuesWrap},n?.map(a=>s&&(a.value===void 0||a.value===ht)?null:t.createElement("span",{key:a.key,className:o.rowWrapper},t.createElement("span",{className:o.rowValue},a.value))))},Wn=e=>{const n=e.isDark?"#ce9178":"#a31515",s=e.isDark?"#73bf69":"#56a64b";return{metricName:p.css`
      color: ${s};
    `,metricValue:p.css`
      color: ${n};
    `,expanded:p.css`
      display: block;
      text-indent: 1em;
    `}},kn=({value:e,index:n,length:s,isExpandedView:o})=>{const a=(0,z.wW)(Wn),r=e.key,l=e.value;return t.createElement("span",{className:o?a.expanded:"",key:n},t.createElement("span",{className:a.metricName},r),t.createElement("span",null,"="),t.createElement("span",null,'"'),t.createElement("span",{className:a.metricValue},l),t.createElement("span",null,'"'),n<s-1&&t.createElement("span",null,", "))},ns="20px",_e="80px",as="25px",Un=(e,n,s)=>({rowWrapper:p.css`
    border-bottom: 1px solid ${e.colors.border.medium};
    display: flex;
    position: relative;
    padding-left: 22px;
    ${s?"":"align-items: center;"}
    ${s?"":"height: 100%;"}
  `,copyToClipboardWrapper:p.css`
    position: absolute;
    left: 0;
    ${s?"":"bottom: 0;"}
    ${s?"top: 4px;":"top: 0;"}
    margin: auto;
    z-index: 1;
    height: 16px;
    width: 16px;
  `,rowLabelWrapWrap:p.css`
    position: relative;
    width: calc(100% - (${n} * ${_e}) - ${as});
  `,rowLabelWrap:p.css`
    white-space: nowrap;
    overflow-x: auto;
    -ms-overflow-style: none; /* IE and Edge */
    scrollbar-width: none; /* Firefox */
    padding-right: ${ns};

    &::-webkit-scrollbar {
      display: none; /* Chrome, Safari and Opera */
    }

    &:after {
      pointer-events: none;
      content: '';
      width: 100%;
      height: 100%;
      position: absolute;
      left: 0;
      top: 0;
      background: linear-gradient(
        to right,
        transparent calc(100% - ${ns}),
        ${e.colors.background.primary}
      );
    }
  `});function Qn(e){let n=[],s=[];for(const o in e)o in e&&e[o]&&!o.includes("Value")?n.push({key:o,value:e[o]}):o in e&&e[o]&&o.includes("Value")&&s.push({key:o,value:e[o]});return{values:s,attributeValues:n}}const Vn=({listItemData:e,listKey:n,totalNumberOfValues:s,valueLabels:o,isExpandedView:a})=>{const{__name__:r,...l}=e,[c,d]=(0,An.Z)(),h=o?.length??s,u=(0,z.wW)(E=>Un(E,h,a)),{values:m,attributeValues:g}=Qn(l),v=E=>E==="\u221E"?"+Inf":E,f=`${r}{${g.map(E=>E.key!=="le"?`${E.key}="${v(E.value)}"`:"")}}`,y=Boolean(o&&o?.length);return t.createElement(t.Fragment,null,o!==void 0&&a&&t.createElement(rs,{valueLabels:o,expanded:a}),t.createElement("div",{key:n,className:u.rowWrapper},t.createElement("span",{className:u.copyToClipboardWrapper},t.createElement(De.h,{tooltip:"Copy to clipboard",onClick:()=>{(0,H.ff)("grafana_explore_prometheus_instant_query_ui_raw_toggle_expand"),d(f)},name:"copy"})),t.createElement("span",{role:"cell",className:u.rowLabelWrapWrap},t.createElement("div",{className:u.rowLabelWrap},t.createElement("span",null,r),t.createElement("span",null,"{"),t.createElement("span",null,g.map((E,x)=>t.createElement(kn,{isExpandedView:a,value:E,key:x,index:x,length:g.length}))),t.createElement("span",null,"}"))),t.createElement(Bn,{hideFieldsWithoutValues:y,totalNumberOfValues:h,values:m})))},Zn=(e,n)=>({valueNavigation:p.css`
      width: ${_e};
      font-weight: bold;
    `,valueNavigationWrapper:p.css`
      display: flex;
      justify-content: flex-end;
    `,itemLabelsWrap:p.css`
      ${n?"":`border-bottom: 1px solid ${e.colors.border.medium}`};
    `}),Gn=e=>e.includes(os.QG)?e.replace(os.QG,""):e,rs=({valueLabels:e,expanded:n})=>{const s=(0,z.wW)(o=>Zn(o,n));return t.createElement("div",{className:s.itemLabelsWrap},t.createElement("div",{className:s.valueNavigationWrapper},e.map((o,a)=>t.createElement("span",{className:s.valueNavigation,key:o.name},Gn(o.name)))))},$e={wrapper:p.css`
    height: 100%;
    overflow: scroll;
  `,switchWrapper:p.css`
    display: flex;
    flex-direction: row;
    margin-bottom: 0;
  `,switchLabel:p.css`
    margin-left: 15px;
    margin-bottom: 0;
  `,switch:p.css`
    margin-left: 10px;
  `,resultCount:p.css`
    margin-bottom: 4px;
  `,header:p.css`
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 10px 0;
    font-size: 12px;
    line-height: 1.25;
  `},jn=480,Yn=2,Kn=e=>{const{tableResult:n}=e,s=(0,I.cloneDeep)(n),o=(0,t.useRef)(null),a=s.fields.filter(g=>g.name.includes("Value")),r=Mn(s),{width:l}=(0,se.Z)(),[c,d]=(0,t.useState)(l<=jn||a.length>Yn),h=()=>{d(!c);const g={isExpanded:!c};(0,H.ff)("grafana_explore_prometheus_instant_query_ui_raw_toggle_expand",g)};(0,t.useEffect)(()=>{o.current?.resetAfterIndex(0,!0)},[c]);const u=g=>{if(g<10){let y=0;for(let E=0;E<g;E++)y+=m(E,!0);return Math.min(600,y)}return 600},m=(g,v)=>{if(!v)return 32;const E=r[g];return 1.5*32+(Object.keys(E).length-a.length)*22};return t.createElement("section",null,t.createElement("header",{className:$e.header},t.createElement(ss.g,{className:$e.switchWrapper,label:"Expand results",htmlFor:"isExpandedView"},t.createElement("div",{className:$e.switch},t.createElement(ze.r,{onChange:h,id:"isExpandedView",value:c,label:"Expand results"}))),t.createElement("div",{className:$e.resultCount},"Result series: ",r.length)),t.createElement("div",{role:"table"},t.createElement(t.Fragment,null,a.length>1&&!c&&t.createElement(rs,{valueLabels:a,expanded:c}),t.createElement(Hn.S_,{ref:o,itemCount:r.length,className:$e.wrapper,itemSize:g=>m(g,c),height:u(r.length),width:"100%"},({index:g,style:v})=>{let f;return c&&(f=a.filter(y=>{const E=r[g][y.name];return E&&E!==ht})),t.createElement("div",{role:"row",style:{...v,overflow:"hidden"}},t.createElement(Vn,{isExpandedView:c,valueLabels:f,totalNumberOfValues:a.length,listKey:r[g].__name__,listItemData:r[g]}))}))))};function Jn(e,{exploreId:n}){const o=e.explore[n],{loading:a,tableResult:r,rawPrometheusResult:l,range:c}=o,d=l?[l]:[],h=(r?.length??!1)>0&&l?r:d;return{loading:h&&h.length>0?!1:a,tableResult:h,range:c}}const Xn=(0,ce.connect)(Jn,{});class qn extends t.PureComponent{constructor(n){super(n),this.onChangeResultsStyle=s=>{this.setState({resultsStyle:s})},this.renderLabel=()=>{const s=(0,p.css)({display:"flex",justifyContent:"space-between"}),o=F.zN.map(a=>({value:a,label:a[0].toUpperCase()+a.slice(1).replace(/_/," ")}));return t.createElement("div",{className:s},this.state.resultsStyle===R.TABLE_RESULTS_STYLE.raw?"Raw":"Table",t.createElement(Xe.S,{onClick:()=>{const a={state:this.state.resultsStyle===R.TABLE_RESULTS_STYLE.table?R.TABLE_RESULTS_STYLE.raw:R.TABLE_RESULTS_STYLE.table};(0,H.ff)("grafana_explore_prometheus_instant_query_ui_toggle_clicked",a)},size:"sm",options:o,value:this.state?.resultsStyle,onChange:this.onChangeResultsStyle}))},n.showRawPrometheus&&(this.state={resultsStyle:R.TABLE_RESULTS_STYLE.raw})}getMainFrame(n){return n?.find(s=>s.meta?.custom?.parentRowIndex===void 0)||n?.[0]}getTableHeight(){const{tableResult:n}=this.props,s=this.getMainFrame(n);return!s||s.length===0?200:Math.max(Math.min(600,s.length*35)+35)}render(){const{loading:n,onCellFilterAdded:s,tableResult:o,width:a,splitOpenFn:r,range:l,ariaLabel:c,timeZone:d}=this.props,h=this.getTableHeight(),u=a-Pe.vc.theme.panelPadding*2-ts.QO;let m=o;if(m?.length){m=(0,Je.SM)({data:m,timeZone:d,theme:Pe.vc.theme2,replaceVariables:E=>E,fieldConfig:{defaults:{},overrides:[]}});for(const E of m)for(const x of E.fields)x.getLinks=w=>(0,qe.a_)({field:x,rowIndex:w.valueRowIndex,splitOpenFn:r,range:l,dataFrame:E})}const g=this.getMainFrame(m),v=m?.filter(E=>E.meta?.custom?.parentRowIndex!==void 0),f=this.state?.resultsStyle!==void 0?this.renderLabel():"Table",y=!this.state?.resultsStyle||this.state?.resultsStyle===R.TABLE_RESULTS_STYLE.table;return t.createElement(we.U,{label:f,loading:n,isOpen:!0},g?.length&&t.createElement(t.Fragment,null,y&&t.createElement(es.i,{ariaLabel:c,data:g,subData:v,width:u,height:h,onCellFilterAdded:s}),this.state?.resultsStyle===R.TABLE_RESULTS_STYLE.raw&&t.createElement(Kn,{tableResult:g})),!g?.length&&t.createElement(lt,{metaItems:[{value:"0 series returned"}]}))}}const _n=Xn(qn),ea=e=>{const{queryError:n}=e,s=!!n,o=s?100:10,a=n?"Query error":"Unknown error",r=n?.message||n?.data?.message||null;return t.createElement(wt,{in:s,duration:o},t.createElement(Qe.b,{severity:"error",title:a,topSpacing:2},r))};function ta(e){const n=(0,R.useSelector)(o=>o.explore[e.exploreId]?.queryResponse),s=n?.state===D.Gu.Error?n?.error:void 0;return s?.refId?null:t.createElement(ea,{queryError:s})}var ve=i(13582),de=i(51424),sa=i(97759),Be=i(53217),is=i(14747),oa=i(22350),na=i(97379),aa=i(61744),mt=i(659),gt=i(3153);function ra(e,{exploreId:n}){const s=e.explore,{datasourceInstance:o}=s[n];return{exploreId:n,datasourceInstance:o}}const ia={changeDatasource:At.zU,deleteHistoryItem:ve.NV,commentHistoryItem:ve.Ff,starHistoryItem:ve.ev,setQueries:L.KO},la=(0,ce.connect)(ra,ia),ca=e=>{const n="240px",s="170px",o=e.colors.background.secondary;return{queryCard:p.css`
      position: relative;
      display: flex;
      flex-direction: column;
      border: 1px solid ${e.colors.border.weak};
      margin: ${e.spacing(1)} 0;
      background-color: ${o};
      border-radius: ${e.shape.borderRadius(1)};
      .starred {
        color: ${e.v1.palette.orange};
      }
    `,cardRow:p.css`
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: ${e.spacing(1)};
      border-bottom: none;
      :first-of-type {
        border-bottom: 1px solid ${e.colors.border.weak};
        padding: ${e.spacing(.5,1)};
      }
      img {
        height: ${e.typography.fontSize}px;
        max-width: ${e.typography.fontSize}px;
        margin-right: ${e.spacing(1)};
      }
    `,queryActionButtons:p.css`
      max-width: ${s};
      display: flex;
      justify-content: flex-end;
      font-size: ${e.typography.size.base};
      button {
        margin-left: ${e.spacing(1)};
      }
    `,queryContainer:p.css`
      font-weight: ${e.typography.fontWeightMedium};
      width: calc(100% - ${n});
    `,updateCommentContainer:p.css`
      width: calc(100% + ${n});
      margin-top: ${e.spacing(1)};
    `,comment:p.css`
      overflow-wrap: break-word;
      font-size: ${e.typography.bodySmall.fontSize};
      font-weight: ${e.typography.fontWeightRegular};
      margin-top: ${e.spacing(.5)};
    `,commentButtonRow:p.css`
      > * {
        margin-right: ${e.spacing(1)};
      }
    `,textArea:p.css`
      width: 100%;
    `,runButton:p.css`
      max-width: ${s};
      display: flex;
      justify-content: flex-end;
      button {
        height: auto;
        padding: ${e.spacing(.5,2)};
        line-height: 1.4;
        span {
          white-space: normal !important;
        }
      }
    `,loader:p.css`
      position: absolute;
      width: 100%;
      height: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      background-color: ${e.colors.background.secondary};
    `}};function da(e){const{query:n,commentHistoryItem:s,starHistoryItem:o,deleteHistoryItem:a,changeDatasource:r,exploreId:l,datasourceInstance:c,setQueries:d}=e,[h,u]=(0,t.useState)(!1),[m,g]=(0,t.useState)(n.comment),{value:v,loading:f}=(0,oa.Z)(async()=>{let b;try{b=await(0,Ue.F)().get(n.datasourceUid)}catch{}return{dsInstance:b,queries:await Promise.all(n.queries.map(async k=>{let q;if(b?.meta.mixed)try{q=await(0,Ue.F)().get(k.datasource)}catch{}else q=b;return{query:k,datasource:q}}))}},[n.datasourceUid,n.queries]),y=(0,z.wW)(ca),E=async()=>{const b=n.queries,k=n.datasourceUid!==c?.uid;k&&await r(l,n.datasourceUid),d(l,b),(0,H.ff)("grafana_explore_query_history_run",{queryHistoryEnabled:T.v.queryHistoryEnabled,differentDataSource:k})},x=async()=>{const b=[...n.queries.map(q=>q.datasource?.type||"unknown")];if((0,H.ff)("grafana_explore_query_history_copy_query",{datasources:b,mixed:Boolean(v?.dsInstance?.meta.mixed)}),f||!v)return;const k=v.queries.map(q=>(0,de.OH)(q.query,q.datasource)).join(`
`);(0,Ee.n9)(k),(0,gt.WI)((0,mt.$l)((0,oe.AT)("Query copied to clipboard")))},w=async()=>{const b=(0,de.t2)(n);await(0,Dt.L)(b)},S=()=>{const b=k=>{a(k),(0,gt.WI)((0,mt.$l)((0,oe.AT)("Query deleted"))),(0,H.ff)("grafana_explore_query_history_deleted",{queryHistoryEnabled:T.v.queryHistoryEnabled})};n.starred?tt.Z.publish(new st.VJ({title:"Delete",text:"Are you sure you want to permanently delete your starred query?",yesText:"Delete",icon:"trash-alt",onConfirm:()=>b(n.id)})):b(n.id)},C=()=>{o(n.id,!n.starred),(0,H.ff)("grafana_explore_query_history_starred",{queryHistoryEnabled:T.v.queryHistoryEnabled,newValue:!n.starred})},N=()=>u(!h),Q=()=>{s(n.id,m),u(!1),(0,H.ff)("grafana_explore_query_history_commented",{queryHistoryEnabled:T.v.queryHistoryEnabled})},j=()=>{u(!1),g(n.comment)},ae=b=>{b.key==="Enter"&&(b.shiftKey||b.ctrlKey)&&Q(),b.key==="Escape"&&j()},M=t.createElement("div",{className:y.updateCommentContainer,"aria-label":m?"Update comment form":"Add comment form"},t.createElement(na.K,{onKeyDown:ae,value:m,placeholder:m?void 0:"An optional description of what the query does.",onChange:b=>g(b.currentTarget.value),className:y.textArea}),t.createElement("div",{className:y.commentButtonRow},t.createElement(V.zx,{onClick:Q},"Save comment"),t.createElement(V.zx,{variant:"secondary",onClick:j},"Cancel"))),O=t.createElement("div",{className:y.queryActionButtons},t.createElement(De.h,{name:"comment-alt",onClick:N,title:n.comment?.length>0?"Edit comment":"Add comment"}),t.createElement(De.h,{name:"copy",onClick:x,title:"Copy query to clipboard"}),v?.dsInstance&&t.createElement(De.h,{name:"share-alt",onClick:w,title:"Copy shortened link to clipboard"}),t.createElement(De.h,{name:"trash-alt",title:"Delete query",onClick:S}),t.createElement(De.h,{name:n.starred?"favorite":"star",iconType:n.starred?"mono":"default",onClick:C,title:n.starred?"Unstar query":"Star query"}));return t.createElement("div",{className:y.queryCard},t.createElement("div",{className:y.cardRow},t.createElement(ls,{dsApi:v?.dsInstance,size:"sm"}),O),t.createElement("div",{className:(0,p.cx)(y.cardRow)},t.createElement("div",{className:y.queryContainer},v?.queries.map((b,k)=>t.createElement(pa,{query:b,key:`${b}-${k}`,showDsInfo:v?.dsInstance?.meta.mixed})),!h&&n.comment&&t.createElement("div",{"aria-label":"Query comment",className:y.comment},n.comment),h&&M),!h&&t.createElement("div",{className:y.runButton},t.createElement(V.zx,{variant:"secondary",onClick:E,disabled:!v?.dsInstance||v.queries.some(b=>!b.datasource)},c?.uid===n.datasourceUid?"Run query":"Switch data source and run query"))),f&&t.createElement(aa.u,{text:"loading...",className:y.loader}))}const ua=e=>({queryRow:p.css`
    border-top: 1px solid ${e.colors.border.weak};
    display: flex;
    flex-direction: row;
    padding: 4px 0px;
    gap: 4px;
    :first-child {
      border-top: none;
    }
  `,dsInfoContainer:p.css`
    display: flex;
    align-items: center;
  `,queryText:p.css`
    word-break: break-all;
  `}),pa=({query:e,showDsInfo:n=!1})=>{const s=(0,z.wW)(ua);return t.createElement("div",{className:s.queryRow},n&&t.createElement("div",{className:s.dsInfoContainer},t.createElement(ls,{dsApi:e.datasource,size:"md"}),": "),t.createElement("span",{"aria-label":"Query text",className:s.queryText},(0,de.OH)(e.query,e.datasource)))},ha=e=>n=>p.css`
    display: flex;
    align-items: center;
    font-size: ${n.typography[e==="sm"?"bodySmall":"body"].fontSize};
    font-weight: ${n.typography.fontWeightMedium};
    white-space: nowrap;
  `;function ls({dsApi:e,size:n}){const s=(0,t.useCallback)(a=>ha(n)(a),[n]),o=(0,z.wW)(s);return t.createElement("div",{className:o},t.createElement("img",{src:e?.meta.info.logos.small||"public/img/icn-datasource.svg",alt:e?.type||"Data source does not exist anymore","aria-label":"Data source icon"}),t.createElement("div",{"aria-label":"Data source name"},e?.name||"Data source does not exist anymore"))}const cs=la(da),ma=(e,n)=>{const s=e.isLight?e.v1.palette.gray5:e.v1.palette.dark4;return{container:p.css`
      display: flex;
    `,labelSlider:p.css`
      font-size: ${e.typography.bodySmall.fontSize};
      &:last-of-type {
        margin-top: ${e.spacing(3)};
      }
      &:first-of-type {
        font-weight: ${e.typography.fontWeightMedium};
        margin-bottom: ${e.spacing(2)};
      }
    `,containerContent:p.css`
      /* 134px is based on the width of the Query history tabs bar, so the content is aligned to right side of the tab */
      width: calc(100% - 134px);
    `,containerSlider:p.css`
      width: 129px;
      margin-right: ${e.spacing(1)};
    `,fixedSlider:p.css`
      position: fixed;
    `,slider:p.css`
      bottom: 10px;
      height: ${n-180}px;
      width: 129px;
      padding: ${e.spacing(1)} 0;
    `,selectors:p.css`
      display: flex;
      justify-content: space-between;
      flex-wrap: wrap;
    `,filterInput:p.css`
      margin-bottom: ${e.spacing(1)};
    `,multiselect:p.css`
      width: 100%;
      margin-bottom: ${e.spacing(1)};
      .gf-form-select-box__multi-value {
        background-color: ${s};
        padding: ${e.spacing(.25,.5,.25,1)};
        border-radius: ${e.shape.borderRadius(1)};
      }
    `,sort:p.css`
      width: 170px;
    `,sessionName:p.css`
      display: flex;
      align-items: flex-start;
      justify-content: flex-start;
      margin-top: ${e.spacing(3)};
      h4 {
        margin: 0 10px 0 0;
      }
    `,heading:p.css`
      font-size: ${e.typography.h4.fontSize};
      margin: ${e.spacing(2,.25,1,.25)};
    `,footer:p.css`
      height: 60px;
      margin: ${e.spacing(3)} auto;
      display: flex;
      justify-content: center;
      font-weight: ${e.typography.fontWeightLight};
      font-size: ${e.typography.bodySmall.fontSize};
      a {
        font-weight: ${e.typography.fontWeightMedium};
        margin-left: ${e.spacing(.25)};
      }
    `,queries:p.css`
      font-size: ${e.typography.bodySmall.fontSize};
      font-weight: ${e.typography.fontWeightRegular};
      margin-left: ${e.spacing(.5)};
    `}};function ga(e){const{queries:n,totalQueries:s,loading:o,richHistorySearchFilters:a,updateFilters:r,clearRichHistoryResults:l,loadMoreRichHistory:c,richHistorySettings:d,exploreId:h,height:u,activeDatasourceInstance:m}=e,g=(0,z.wW)((0,t.useCallback)(x=>ma(x,u),[u])),v=(0,de.DR)();if((0,t.useEffect)(()=>{const x=!d.activeDatasourceOnly&&d.lastUsedDatasourceFilters?d.lastUsedDatasourceFilters:[m],w={search:"",sortOrder:de.As.Descending,datasourceFilters:x,from:0,to:d.retentionPeriod,starred:!1};return r(w),()=>{l()}},[]),!a)return t.createElement("span",null,"Loading...");const f=(0,de.k4)(n,a.sortOrder),y=us(),E=n.length&&n.length!==s;return t.createElement("div",{className:g.container},t.createElement("div",{className:g.containerSlider},t.createElement("div",{className:g.fixedSlider},t.createElement("div",{className:g.labelSlider},"Filter history"),t.createElement("div",{className:g.labelSlider},(0,de.bQ)(a.from)),t.createElement("div",{className:g.slider},t.createElement(sa.U,{tooltipAlwaysVisible:!1,min:0,max:d.retentionPeriod,value:[a.from,a.to],orientation:"vertical",formatTooltipResult:de.bQ,reverse:!0,onAfterChange:x=>{r({from:x[0],to:x[1]})}})),t.createElement("div",{className:g.labelSlider},(0,de.bQ)(a.to)))),t.createElement("div",{className:g.containerContent},t.createElement("div",{className:g.selectors},!d.activeDatasourceOnly&&t.createElement(Be.NU,{className:g.multiselect,options:v.map(x=>({value:x.name,label:x.name})),value:a.datasourceFilters,placeholder:"Filter queries for data sources(s)","aria-label":"Filter queries for data sources(s)",onChange:x=>{r({datasourceFilters:x.map(w=>w.value)})}}),t.createElement("div",{className:g.filterInput},t.createElement(is.H,{escapeRegex:!1,placeholder:"Search queries",value:a.search,onChange:x=>r({search:x})})),t.createElement("div",{"aria-label":"Sort queries",className:g.sort},t.createElement(Be.Ph,{value:y.filter(x=>x.value===a.sortOrder),options:y,placeholder:"Sort queries by",onChange:x=>r({sortOrder:x.value})}))),o&&t.createElement("span",null,"Loading results..."),!o&&Object.keys(f).map(x=>t.createElement("div",{key:x},t.createElement("div",{className:g.heading},x," ",t.createElement("span",{className:g.queries},E?"Displaying ":"",f[x].length," queries")),f[x].map(w=>t.createElement(cs,{query:w,key:w.id,exploreId:h})))),E?t.createElement("div",null,"Showing ",n.length," of ",s," ",t.createElement(V.zx,{onClick:c},"Load more")):null,t.createElement("div",{className:g.footer},T.v.queryHistoryEnabled?"":"The history is local to your browser and is not shared with others.")))}var fa=i(66719);const ya=e=>({container:p.css`
      font-size: ${e.typography.bodySmall.fontSize};
    `,spaceBetween:p.css`
      margin-bottom: ${e.spacing(3)};
    `,input:p.css`
      max-width: 200px;
    `,bold:p.css`
      font-weight: ${e.typography.fontWeightBold};
    `,bottomMargin:p.css`
      margin-bottom: ${e.spacing(1)};
    `}),ds=[{value:2,label:"2 days"},{value:5,label:"5 days"},{value:7,label:"1 week"},{value:14,label:"2 weeks"}];function va(e){const{retentionPeriod:n,starredTabAsFirstTab:s,activeDatasourceOnly:o,onChangeRetentionPeriod:a,toggleStarredTabAsFirstTab:r,toggleactiveDatasourceOnly:l,deleteRichHistory:c}=e,d=(0,z.wW)(ya),h=ds.find(m=>m.value===n),u=()=>{tt.Z.publish(new st.VJ({title:"Delete",text:"Are you sure you want to permanently delete your query history?",yesText:"Delete",icon:"trash-alt",onConfirm:()=>{c(),(0,gt.WI)((0,mt.$l)((0,oe.AT)("Query history deleted")))}}))};return t.createElement("div",{className:d.container},(0,He.X)().changeRetention?t.createElement(ss.g,{label:"History time span",description:`Select the period of time for which Grafana will save your query history. Up to ${fa.H6} entries will be stored.`},t.createElement("div",{className:d.input},t.createElement(Be.Ph,{value:h,options:ds,onChange:a}))):t.createElement(Qe.b,{severity:"info",title:"History time span"},"Grafana will keep entries up to ",h?.label,"."),t.createElement(xe._,{label:"Change the default active tab from \u201CQuery history\u201D to \u201CStarred\u201D",className:d.spaceBetween},t.createElement(ze.x,{id:"explore-query-history-settings-default-active-tab",value:s,onChange:r})),(0,He.X)().onlyActiveDataSource&&t.createElement(xe._,{label:"Only show queries for data source currently active in Explore",className:d.spaceBetween},t.createElement(ze.x,{id:"explore-query-history-settings-data-source-behavior",value:o,onChange:l})),(0,He.X)().clearHistory&&t.createElement("div",null,t.createElement("div",{className:d.bold},"Clear query history"),t.createElement("div",{className:d.bottomMargin},"Delete all of your query history, permanently."),t.createElement(V.zx,{variant:"destructive",onClick:u},"Clear query history")))}const Ea=e=>{const n=e.isLight?e.v1.palette.gray5:e.v1.palette.dark4;return{container:p.css`
      display: flex;
    `,containerContent:p.css`
      width: 100%;
    `,selectors:p.css`
      display: flex;
      justify-content: space-between;
      flex-wrap: wrap;
    `,multiselect:p.css`
      width: 100%;
      margin-bottom: ${e.spacing(1)};
      .gf-form-select-box__multi-value {
        background-color: ${n};
        padding: ${e.spacing(.25,.5,.25,1)};
        border-radius: ${e.shape.borderRadius(1)};
      }
    `,filterInput:p.css`
      margin-bottom: ${e.spacing(1)};
    `,sort:p.css`
      width: 170px;
    `,footer:p.css`
      height: 60px;
      margin-top: ${e.spacing(3)};
      display: flex;
      justify-content: center;
      font-weight: ${e.typography.fontWeightLight};
      font-size: ${e.typography.bodySmall.fontSize};
      a {
        font-weight: ${e.typography.fontWeightMedium};
        margin-left: ${e.spacing(.25)};
      }
    `}};function xa(e){const{updateFilters:n,clearRichHistoryResults:s,loadMoreRichHistory:o,activeDatasourceInstance:a,richHistorySettings:r,queries:l,totalQueries:c,loading:d,richHistorySearchFilters:h,exploreId:u}=e,m=(0,z.wW)(Ea),g=(0,de.DR)();if((0,t.useEffect)(()=>{const f=r.activeDatasourceOnly&&r.lastUsedDatasourceFilters?r.lastUsedDatasourceFilters:[a],y={search:"",sortOrder:de.As.Descending,datasourceFilters:f,from:0,to:r.retentionPeriod,starred:!0};return n(y),()=>{s()}},[]),!h)return t.createElement("span",null,"Loading...");const v=us();return t.createElement("div",{className:m.container},t.createElement("div",{className:m.containerContent},t.createElement("div",{className:m.selectors},!r.activeDatasourceOnly&&t.createElement(Be.NU,{className:m.multiselect,options:g.map(f=>({value:f.name,label:f.name})),value:h.datasourceFilters,placeholder:"Filter queries for data sources(s)","aria-label":"Filter queries for data sources(s)",onChange:f=>{n({datasourceFilters:f.map(y=>y.value)})}}),t.createElement("div",{className:m.filterInput},t.createElement(is.H,{escapeRegex:!1,placeholder:"Search queries",value:h.search,onChange:f=>n({search:f})})),t.createElement("div",{"aria-label":"Sort queries",className:m.sort},t.createElement(Be.Ph,{value:v.filter(f=>f.value===h.sortOrder),options:v,placeholder:"Sort queries by",onChange:f=>n({sortOrder:f.value})}))),d&&t.createElement("span",null,"Loading results..."),!d&&l.map(f=>t.createElement(cs,{query:f,key:f.id,exploreId:u})),l.length&&l.length!==c?t.createElement("div",null,"Showing ",l.length," of ",c," ",t.createElement(V.zx,{onClick:o},"Load more")):null,t.createElement("div",{className:m.footer},T.v.queryHistoryEnabled?"":"The history is local to your browser and is not shared with others.")))}var ft=(e=>(e.RichHistory="Query history",e.Starred="Starred",e.Settings="Settings",e))(ft||{});const us=()=>[{label:"Newest first",value:de.As.Descending},{label:"Oldest first",value:de.As.Ascending},{label:"Data source A-Z",value:de.As.DatasourceAZ},{label:"Data source Z-A",value:de.As.DatasourceZA}].filter(e=>(0,He.X)().availableFilters.includes(e.value));class Sa extends t.PureComponent{constructor(){super(...arguments),this.state={loading:!1},this.updateSettings=n=>{this.props.updateHistorySettings({...this.props.richHistorySettings,...n})},this.updateFilters=n=>{const s={...this.props.richHistorySearchFilters,...n,page:1};this.props.updateHistorySearchFilters(this.props.exploreId,s),this.loadRichHistory()},this.clearResults=()=>{this.props.clearRichHistoryResults(this.props.exploreId)},this.loadRichHistory=(0,I.debounce)(()=>{this.props.loadRichHistory(this.props.exploreId),this.setState({loading:!0})},300),this.onChangeRetentionPeriod=n=>{n.value!==void 0&&this.updateSettings({retentionPeriod:n.value})},this.toggleStarredTabAsFirstTab=()=>this.updateSettings({starredTabAsFirstTab:!this.props.richHistorySettings.starredTabAsFirstTab}),this.toggleActiveDatasourceOnly=()=>this.updateSettings({activeDatasourceOnly:!this.props.richHistorySettings.activeDatasourceOnly})}componentDidUpdate(n){n.richHistory!==this.props.richHistory&&this.setState({loading:!1})}render(){const{richHistory:n,richHistoryTotal:s,height:o,exploreId:a,deleteRichHistory:r,onClose:l,firstTab:c,activeDatasourceInstance:d}=this.props,{loading:h}=this.state,u={label:"Query history",value:"Query history",content:t.createElement(ga,{queries:n,totalQueries:s||0,loading:h,updateFilters:this.updateFilters,clearRichHistoryResults:()=>this.props.clearRichHistoryResults(this.props.exploreId),loadMoreRichHistory:()=>this.props.loadMoreRichHistory(this.props.exploreId),activeDatasourceInstance:d,richHistorySettings:this.props.richHistorySettings,richHistorySearchFilters:this.props.richHistorySearchFilters,exploreId:a,height:o}),icon:"history"},m={label:"Starred",value:"Starred",content:t.createElement(xa,{queries:n,totalQueries:s||0,loading:h,activeDatasourceInstance:d,updateFilters:this.updateFilters,clearRichHistoryResults:()=>this.props.clearRichHistoryResults(this.props.exploreId),loadMoreRichHistory:()=>this.props.loadMoreRichHistory(this.props.exploreId),richHistorySettings:this.props.richHistorySettings,richHistorySearchFilters:this.props.richHistorySearchFilters,exploreId:a}),icon:"star"},g={label:"Settings",value:"Settings",content:t.createElement(va,{retentionPeriod:this.props.richHistorySettings.retentionPeriod,starredTabAsFirstTab:this.props.richHistorySettings.starredTabAsFirstTab,activeDatasourceOnly:this.props.richHistorySettings.activeDatasourceOnly,onChangeRetentionPeriod:this.onChangeRetentionPeriod,toggleStarredTabAsFirstTab:this.toggleStarredTabAsFirstTab,toggleactiveDatasourceOnly:this.toggleActiveDatasourceOnly,deleteRichHistory:r}),icon:"sliders-v-alt"};let v=[u,m,g];return t.createElement(Ct.W,{tabs:v,onClose:l,defaultTab:c,closeIconTooltip:"Close query history"})}}const ba=(0,z.HE)(Sa);function wa(e,{exploreId:n}){const s=e.explore,o=s[n],a=o.richHistorySearchFilters,r=s.richHistorySettings,{datasourceInstance:l}=o,c=r?.starredTabAsFirstTab?ft.Starred:ft.RichHistory,{richHistory:d,richHistoryTotal:h}=o;return{richHistory:d,richHistoryTotal:h,firstTab:c,activeDatasourceInstance:l.name,richHistorySettings:r,richHistorySearchFilters:a}}const Ca={initRichHistory:ve.sO,loadRichHistory:ve.TV,loadMoreRichHistory:ve.io,clearRichHistoryResults:ve.Cs,updateHistorySettings:ve.ch,updateHistorySearchFilters:ve.KZ,deleteRichHistory:ve.ik},Ra=(0,ce.connect)(wa,Ca);function Ta(e){const n=(0,z.l4)(),[s,o]=(0,t.useState)(n.components.horizontalDrawer.defaultHeight),{richHistory:a,richHistoryTotal:r,width:l,firstTab:c,activeDatasourceInstance:d,exploreId:h,deleteRichHistory:u,initRichHistory:m,loadRichHistory:g,loadMoreRichHistory:v,clearRichHistoryResults:f,richHistorySettings:y,updateHistorySettings:E,richHistorySearchFilters:x,updateHistorySearchFilters:w,onClose:S}=e;return(0,t.useEffect)(()=>{m(),(0,H.ff)("grafana_explore_query_history_opened",{queryHistoryEnabled:T.v.queryHistoryEnabled})},[m]),y?t.createElement(Ft,{width:l,onResize:(C,N,Q)=>{o(Number(Q.style.height.slice(0,-2)))}},t.createElement(ba,{richHistory:a,richHistoryTotal:r,firstTab:c,activeDatasourceInstance:d,exploreId:h,onClose:S,height:s,deleteRichHistory:u,richHistorySettings:y,richHistorySearchFilters:x,updateHistorySettings:E,updateHistorySearchFilters:w,loadRichHistory:g,loadMoreRichHistory:v,clearRichHistoryResults:f})):t.createElement("span",null,"Loading...")}const La=Ra(Ta),za=e=>({containerMargin:p.css`
      margin-top: ${e.spacing(2)};
    `});function Ia(e){const n=(0,z.l4)(),s=za(n);return t.createElement("div",{className:s.containerMargin},t.createElement(Qt.Lh,null,!e.addQueryRowButtonHidden&&t.createElement(V.zx,{variant:"secondary","aria-label":"Add row button",onClick:e.onClickAddQueryRowButton,disabled:e.addQueryRowButtonDisabled,icon:"plus"},"Add query"),!e.richHistoryRowButtonHidden&&t.createElement(V.zx,{variant:"secondary","aria-label":"Rich history button",className:(0,p.cx)({["explore-active-button"]:e.richHistoryButtonActive}),onClick:e.onClickRichHistoryButton,icon:"history"},"Query history"),t.createElement(V.zx,{variant:"secondary","aria-label":"Query inspector button",className:(0,p.cx)({["explore-active-button"]:e.queryInspectorButtonActive}),onClick:e.onClickQueryInspectorButton,icon:"info-circle"},"Inspector")))}function Fa(e,{exploreId:n}){const o=e.explore[n],{loading:a,tableResult:r,range:l}=o;return{loading:r&&r.length>0?!1:a,tableResult:r,range:l}}const Na=(0,ce.connect)(Fa,{});class Da extends t.PureComponent{getMainFrame(n){return n?.find(s=>s.meta?.custom?.parentRowIndex===void 0)||n?.[0]}getTableHeight(){const{tableResult:n}=this.props,s=this.getMainFrame(n);return!s||s.length===0?200:Math.max(Math.min(600,s.length*35)+35)}render(){const{loading:n,onCellFilterAdded:s,tableResult:o,width:a,splitOpenFn:r,range:l,ariaLabel:c,timeZone:d}=this.props,h=this.getTableHeight(),u=a-Pe.vc.theme.panelPadding*2-ts.QO;let m=o;if(m?.length){m=(0,Je.SM)({data:m,timeZone:d,theme:Pe.vc.theme2,replaceVariables:f=>f,fieldConfig:{defaults:{},overrides:[]}});for(const f of m)for(const y of f.fields)y.getLinks=E=>(0,qe.a_)({field:y,rowIndex:E.valueRowIndex,splitOpenFn:r,range:l,dataFrame:f})}const g=this.getMainFrame(m),v=m?.filter(f=>f.meta?.custom?.parentRowIndex!==void 0);return t.createElement(we.U,{label:"Table",loading:n,isOpen:!0},g?.length?t.createElement(es.i,{ariaLabel:c,data:g,subData:v,width:u,height:h,maxHeight:600,onCellFilterAdded:s}):t.createElement(lt,{metaItems:[{value:"0 series returned"}]}))}}const Pa=Na(Da);var Ha=i(94619),Oa=i(72169),Ma=i(93183),Aa=i(75151),$a=i(335);const Ba=e=>({container:p.css`
    label: container;
    margin-bottom: ${e.spacing(1)};
    background-color: ${e.colors.background.primary};
    border: 1px solid ${e.colors.border.medium};
    position: relative;
    border-radius: ${e.shape.radius.default};
    width: 100%;
    display: flex;
    flex-direction: column;
    flex: 1 1 0;
    padding: ${T.v.featureToggles.newTraceView?0:e.spacing(e.components.panel.padding)};
  `});function Wa(e){const n=e.dataFrames[0],s=(0,z.wW)(Ba),{dataFrames:o,splitOpenFn:a,exploreId:r,scrollElement:l,topOfViewRef:c,queryResponse:d}=e,h=(0,t.useMemo)(()=>(0,$a.N)(n),[n]),{search:u,setSearch:m,spanFindMatches:g}=(0,Aa.R)(h?.spans),[v,f]=(0,t.useState)(""),[y,E]=(0,t.useState)(""),x=(0,R.useSelector)(S=>S.explore[e.exploreId]?.datasourceInstance??void 0),w=x?x?.type:"unknown";return h?t.createElement("div",{className:s.container},!T.v.featureToggles.newTraceView&&t.createElement(Oa.Z,{navigable:!0,searchValue:u,setSearch:m,spanFindMatches:g,searchBarSuffix:y,setSearchBarSuffix:E,focusedSpanIdForSearch:v,setFocusedSpanIdForSearch:f,datasourceType:w}),t.createElement(Ha.m,{exploreId:r,dataFrames:o,splitOpenFn:a,scrollElement:l,traceProp:h,spanFindMatches:g,search:u,focusedSpanIdForSearch:v,queryResponse:d,datasource:x,topOfViewRef:c,topOfViewRefType:Ma.l4.Explore})):null}var yt=i(93713);const ka=e=>({exploreMain:p.css`
      label: exploreMain;
      // Is needed for some transition animations to work.
      position: relative;
      margin-top: 21px;
    `,button:p.css`
      label: button;
      margin: 1em 4px 0 0;
    `,queryContainer:p.css`
      label: queryContainer;
      // Need to override normal css class and don't want to count on ordering of the classes in html.
      height: auto !important;
      flex: unset !important;
      display: unset !important;
      padding: ${e.spacing(1)};
    `,exploreContainer:p.css`
      display: flex;
      flex: 1 1 auto;
      flex-direction: column;
      padding: ${e.spacing(2)};
      padding-top: 0;
    `});var Ua=(e=>(e[e.RichHistory=0]="RichHistory",e[e.QueryInspector=1]="QueryInspector",e))(Ua||{});class Qa extends t.PureComponent{constructor(n){super(n),this.topOfViewRef=(0,t.createRef)(),this.onChangeTime=s=>{const{updateTimeRange:o,exploreId:a}=this.props;o({exploreId:a,rawRange:s})},this.onClickExample=s=>{this.props.setQueries(this.props.exploreId,[s])},this.onCellFilterAdded=s=>{const{value:o,key:a,operator:r}=s;r===bt.PT&&this.onClickFilterLabel(a,o),r===bt.tE&&this.onClickFilterOutLabel(a,o)},this.onClickFilterLabel=(s,o)=>{this.onModifyQueries({type:"ADD_FILTER",options:{key:s,value:o}})},this.onClickFilterOutLabel=(s,o)=>{this.onModifyQueries({type:"ADD_FILTER_OUT",options:{key:s,value:o}})},this.onClickAddQueryRowButton=()=>{const{exploreId:s,queryKeys:o}=this.props;this.props.addQueryRow(s,o.length)},this.onMakeAbsoluteTime=()=>{const{makeAbsoluteTime:s}=this.props;s()},this.onModifyQueries=s=>{const o=async(a,r)=>{const{datasource:l}=a;if(l==null)return a;const c=await(0,Ue.F)().get(l);return c.modifyQuery?c.modifyQuery(a,r):a};this.props.modifyQueries(this.props.exploreId,s,o)},this.onResize=s=>{this.props.changeSize(this.props.exploreId,s)},this.onStartScanning=()=>{this.props.scanStart(this.props.exploreId)},this.onStopScanning=()=>{this.props.scanStopAction({exploreId:this.props.exploreId})},this.onUpdateTimeRange=s=>{const{exploreId:o,updateTimeRange:a}=this.props;a({exploreId:o,absoluteRange:s})},this.toggleShowRichHistory=()=>{this.setState(s=>({openDrawer:s.openDrawer===0?void 0:0}))},this.toggleShowQueryInspector=()=>{this.setState(s=>({openDrawer:s.openDrawer===1?void 0:1}))},this.onSplitOpen=s=>async o=>{if(this.props.splitOpen(o),o&&this.props.datasourceInstance){const a=(await(0,Ue.F)().get(o.datasourceUid)).type,r=this.props.datasourceInstance.uid===We.D?(0,I.get)(this.props.queries,"0.datasource.type"):this.props.datasourceInstance.type,l={origin:"panel",panelType:s,source:r,target:a,exploreId:this.props.exploreId};(0,H.ff)("grafana_explore_split_view_opened",l)}},this.memoizedGetNodeGraphDataFrames=(0,ue.Z)(ys.Ee),this.state={openDrawer:void 0},this.graphEventBus=n.eventBus.newScopedBus("graph",{onlyLocal:!1}),this.logsEventBus=n.eventBus.newScopedBus("logs",{onlyLocal:!1})}componentDidMount(){this.absoluteTimeUnsubsciber=tt.Z.subscribe(st.QI,this.onMakeAbsoluteTime)}componentWillUnmount(){this.absoluteTimeUnsubsciber?.unsubscribe()}renderEmptyState(n){return t.createElement("div",{className:(0,p.cx)(n)},t.createElement(bn,null))}renderNoData(){return t.createElement(En,null)}renderCompactUrlWarning(){return t.createElement(wt,{in:!0,duration:100},t.createElement(Qe.b,{severity:"warning",title:"Compact URL Deprecation Notice",topSpacing:2},"The URL that brought you here was a compact URL - this format will soon be deprecated. Please replace the URL previously saved with the URL available now."))}renderGraphPanel(n){const{graphResult:s,absoluteRange:o,timeZone:a,queryResponse:r,loading:l,showFlameGraph:c}=this.props;return t.createElement(Lo,{loading:l,data:s,height:c?180:400,width:n,absoluteRange:o,timeZone:a,onChangeTime:this.onUpdateTimeRange,annotations:r.annotations,splitOpenFn:this.onSplitOpen("graph"),loadingState:r.state,eventBus:this.graphEventBus})}renderTablePanel(n){const{exploreId:s,timeZone:o}=this.props;return t.createElement(Pa,{ariaLabel:fe.wl.pages.Explore.General.table,width:n,exploreId:s,onCellFilterAdded:this.onCellFilterAdded,timeZone:o,splitOpenFn:this.onSplitOpen("table")})}renderRawPrometheus(n){const{exploreId:s,datasourceInstance:o,timeZone:a}=this.props;return t.createElement(_n,{showRawPrometheus:!0,ariaLabel:fe.wl.pages.Explore.General.table,width:n,exploreId:s,onCellFilterAdded:o?.modifyQuery?this.onCellFilterAdded:void 0,timeZone:a,splitOpenFn:this.onSplitOpen("table")})}renderLogsPanel(n){const{exploreId:s,syncedTimes:o,theme:a,queryResponse:r}=this.props,l=parseInt(a.spacing(2).slice(0,-2),10);return t.createElement(fn,{exploreId:s,loadingState:r.state,syncedTimes:o,width:n-l,onClickFilterLabel:this.onClickFilterLabel,onClickFilterOutLabel:this.onClickFilterOutLabel,onStartScanning:this.onStartScanning,onStopScanning:this.onStopScanning,scrollElement:this.scrollElement,eventBus:this.logsEventBus,splitOpenFn:this.onSplitOpen("logs")})}renderLogsSamplePanel(){const{logsSample:n,timeZone:s,setSupplementaryQueryEnabled:o,exploreId:a,datasourceInstance:r,queries:l}=this.props;return t.createElement(yn,{queryResponse:n.data,timeZone:s,enabled:n.enabled,queries:l,datasourceInstance:r,splitOpen:this.onSplitOpen("logsSample"),setLogsSampleEnabled:c=>o(a,c,G.v8.LogsSample)})}renderNodeGraphPanel(){const{exploreId:n,showTrace:s,queryResponse:o,datasourceInstance:a}=this.props,r=a?a?.type:"unknown";return t.createElement(In,{dataFrames:this.memoizedGetNodeGraphDataFrames(o.series),exploreId:n,withTraceView:s,datasourceType:r,splitOpenFn:this.onSplitOpen("nodeGraph")})}renderFlameGraphPanel(){const{queryResponse:n}=this.props;return t.createElement(no,{dataFrames:n.flameGraphFrames})}renderTraceViewPanel(){const{queryResponse:n,exploreId:s}=this.props,o=n.series.filter(a=>a.meta?.preferredVisualisationType==="trace");return o.length&&t.createElement(Wa,{exploreId:s,dataFrames:o,splitOpenFn:this.onSplitOpen("traceView"),scrollElement:this.scrollElement,queryResponse:n,topOfViewRef:this.topOfViewRef})}render(){const{datasourceInstance:n,datasourceMissing:s,exploreId:o,graphResult:a,queryResponse:r,isLive:l,theme:c,showMetrics:d,showTable:h,showRawPrometheus:u,showLogs:m,showTrace:g,showNodeGraph:v,showFlameGraph:f,timeZone:y,isFromCompactUrl:E,showLogsSample:x}=this.props,{openDrawer:w}=this.state,S=ka(c),C=r&&r.state!==D.Gu.NotStarted,N=w===0,Q=!(0,He.X)().queryHistoryAvailable,j=w===1,ae=r.state===D.Gu.Done&&[r.logsFrames,r.graphFrames,r.nodeGraphFrames,r.flameGraphFrames,r.tableFrames,r.rawPrometheusFrames,r.traceFrames].every(M=>M.length===0);return t.createElement(xt.$,{testId:fe.wl.pages.Explore.General.scrollView,autoHeightMin:"100%",scrollRefCallback:M=>this.scrollElement=M||void 0},t.createElement(so,{exploreId:o,onChangeTime:this.onChangeTime,topOfViewRef:this.topOfViewRef}),E?this.renderCompactUrlWarning():null,s?this.renderEmptyState(S.exploreContainer):null,n&&t.createElement("div",{className:S.exploreContainer},t.createElement(St.l,{className:S.queryContainer},t.createElement(Pn,{exploreId:o}),t.createElement(Ia,{addQueryRowButtonDisabled:l,addQueryRowButtonHidden:!1,richHistoryRowButtonHidden:Q,richHistoryButtonActive:N,queryInspectorButtonActive:j,onClickAddQueryRowButton:this.onClickAddQueryRowButton,onClickRichHistoryButton:this.toggleShowRichHistory,onClickQueryInspectorButton:this.toggleShowQueryInspector}),t.createElement(ta,{exploreId:o})),t.createElement(gs.Z,{onResize:this.onResize,disableHeight:!0},({width:M})=>M===0?null:t.createElement("main",{className:(0,p.cx)(S.exploreMain),style:{width:M}},t.createElement(A.z4,null,C&&t.createElement(t.Fragment,null,d&&a&&t.createElement(A.z4,null,this.renderGraphPanel(M)),u&&t.createElement(A.z4,null,this.renderRawPrometheus(M)),h&&t.createElement(A.z4,null,this.renderTablePanel(M)),m&&t.createElement(A.z4,null,this.renderLogsPanel(M)),v&&t.createElement(A.z4,null,this.renderNodeGraphPanel()),f&&t.createElement(A.z4,null,this.renderFlameGraphPanel()),g&&t.createElement(A.z4,null,this.renderTraceViewPanel()),T.v.featureToggles.logsSampleInExplore&&x&&t.createElement(A.z4,null,this.renderLogsSamplePanel()),ae&&t.createElement(A.z4,null,this.renderNoData())),N&&t.createElement(La,{width:M,exploreId:o,onClose:this.toggleShowRichHistory}),j&&t.createElement(Bs,{exploreId:o,width:M,onClose:this.toggleShowQueryInspector,timeZone:y}))))))}}function Va(e,{exploreId:n}){const s=e.explore,{syncedTimes:o}=s,a=s[n],r=(0,Ie.Z)(e.user),{datasourceInstance:l,datasourceMissing:c,queryKeys:d,queries:h,isLive:u,graphResult:m,tableResult:g,logsResult:v,showLogs:f,showMetrics:y,showTable:E,showTrace:x,absoluteRange:w,queryResponse:S,showNodeGraph:C,showFlameGraph:N,loading:Q,isFromCompactUrl:j,showRawPrometheus:ae,supplementaryQueries:M}=a,O=M[G.v8.LogsSample],b=!!(O.dataProvider!==void 0&&!v&&(m||g));return{datasourceInstance:l,datasourceMissing:c,queryKeys:d,queries:h,isLive:u,graphResult:m,logsResult:v??void 0,absoluteRange:w,queryResponse:S,syncedTimes:o,timeZone:r,showLogs:f,showMetrics:y,showTable:E,showTrace:x,showNodeGraph:C,showRawPrometheus:ae,showFlameGraph:N,splitted:(0,he.p)(e),loading:Q,isFromCompactUrl:j||!1,logsSample:O,showLogsSample:b}}const Za={changeSize:yt.qN,modifyQueries:L.sK,scanStart:L.EA,scanStopAction:L.P4,setQueries:L.KO,updateTimeRange:Te.cD,makeAbsoluteTime:Te.F9,addQueryRow:L.CS,splitOpen:P.bW,setSupplementaryQueryEnabled:L.z0},Ga=(0,ce.connect)(Va,Za),ja=(0,z.HE)(Ga(Qa)),Ya=e=>({explore:p.css`
      display: flex;
      flex: 1 1 auto;
      flex-direction: column;
      overflow: hidden;
      min-width: 600px;
      & + & {
        border-left: 1px dotted ${e.colors.border.medium};
      }
    `});class Ka extends t.PureComponent{constructor(n){super(n),this.el=null,this.refreshExplore=s=>{const{exploreId:o,urlQuery:a}=this.props;a!==s&&a!==P.IS[o]&&this.props.refreshExplore(o,a)},this.getRef=s=>{this.el=s},this.exploreEvents=new Se.F,this.state={openDrawer:void 0}}async componentDidMount(){const{initialized:n,exploreId:s,initialDatasource:o,initialQueries:a,initialRange:r,panelsState:l,orgId:c,isFromCompactUrl:d}=this.props,h=this.el?.offsetWidth??0;if(!n){let u,m;if((!a||a.length===0)&&o&&!(o===We.D||o.uid===We.D)){const{instance:y}=await(0,Bt.r_)(c,o);u=y.getRef()}let g=await(0,Ee.Z8)(a,u);if(!(0,Ee._o)(g).noneHaveDatasource&&!(0,Ee._o)(g).allDatasourceSame)if(Pe.vc.featureToggles.exploreMixedDatasource)m=await(0,ke.ak)().get(We.D);else{const f=g.find(y=>y.datasource?.uid).datasource.uid;if(f){m=f;const y=await(0,ke.ak)().get(f),E=await(0,ke.ak)().get(o);await this.props.importQueries(s,g,E,y),await this.props.stateSave({replace:!0}),g=this.props.initialQueries}}d&&(0,H.ff)("grafana_explore_compact_notice"),this.props.initializeExplore(s,m||g[0]?.datasource||o,g,r,h,this.exploreEvents,l,d)}}componentWillUnmount(){this.exploreEvents.removeAllListeners()}componentDidUpdate(n){this.refreshExplore(n.urlQuery)}render(){const{theme:n,exploreId:s,initialized:o,eventBus:a}=this.props,r=Ya(n);return t.createElement("div",{className:r.explore,ref:this.getRef,"data-testid":fe.wl.pages.Explore.General.container},o&&t.createElement(ja,{exploreId:s,eventBus:a}))}}const Ja=(0,ue.Z)(Ee.vP);function Xa(e,n){const s=(0,Ee.J5)(n.urlQuery),o=(0,Ie.Z)(e.user),a=(0,Ie.i)(e.user),{datasource:r,queries:l,range:c,panelsState:d}=s||{},h=r||le.Z.get((0,Ee.I$)(e.user.orgId)),u=c?Ja(c,o,a):(0,Ee.OQ)(o,Ee.UI,a);return{initialized:e.explore[n.exploreId]?.initialized,initialDatasource:h,initialQueries:l,initialRange:u,panelsState:d,orgId:e.user.orgId,isFromCompactUrl:s.isFromCompactUrl||!1}}const qa={initializeExplore:yt.CK,refreshExplore:yt.Om,importQueries:L.GJ,stateSave:P.og},_a=(0,ce.connect)(Xa,qa),ps=(0,z.HE)(_a(Ka)),er={pageScrollbarWrapper:p.css`
    width: 100%;
    flex-grow: 1;
    min-height: 0;
    height: 100%;
    position: relative;
  `};function tr(e){sr();const n=(0,R.useDispatch)(),s=e.queryParams,{keybindings:o,chrome:a,config:r}=(0,K.p)(),l=(0,Y.q)("explore"),{get:c}=(0,W.K)(),{warning:d}=(0,oe.iG)(),h=(0,J.R9)(),u=(0,t.useRef)(h.eventBus.newScopedBus("explore",{onlyLocal:!1})),[m,g]=(0,t.useState)(.5),{width:v}=(0,se.Z)(),f=200,y=(0,R.useSelector)(S=>S.explore);(0,t.useEffect)(()=>{a.update({sectionNav:l.node})},[a,l]),(0,t.useEffect)(()=>{o.setupTimeRangeBindings(!1)},[o]),(0,t.useEffect)(()=>{r.featureToggles.correlations?c.execute():n((0,P.CL)([]))},[]),(0,t.useEffect)(()=>{c.value?n((0,P.CL)(c.value)):c.error&&(n((0,P.CL)([])),d("Could not load correlations.","Correlations data could not be loaded, DataLinks may have partial data."))},[c.value,c.error,n,d]),(0,t.useEffect)(()=>{P.IS.left=void 0,P.IS.right=void 0;const S=ne.E1.getSearchObject();return(S.from||S.to)&&ne.E1.partial({from:void 0,to:void 0},!0),()=>{n((0,P.WK)())}},[]);const E=S=>{const C=v/2,N=(0,I.inRange)(S,C-100,C+100);n(N?(0,P.Sx)({largerExploreId:void 0}):(0,P.Sx)({largerExploreId:S>C?F.Kd.right:F.Kd.left})),g(S/v)},x=Boolean(s.left)&&Boolean(s.right);let w=0;return x&&(!y.evenSplitPanes&&y.maxedExploreId?w=y.maxedExploreId===F.Kd.right?v-f:f:y.evenSplitPanes?w=Math.floor(v/2):m!==void 0&&(w=v*m)),t.createElement("div",{className:er.pageScrollbarWrapper},t.createElement(me,{exploreIdLeft:F.Kd.left,exploreIdRight:F.Kd.right}),t.createElement(U.U,{splitOrientation:"vertical",paneSize:w,minSize:f,maxSize:f*-1,primary:"second",splitVisible:x,paneStyle:{overflow:"auto",display:"flex",flexDirection:"column"},onDragFinished:S=>{S&&E(S)}},t.createElement(A.z4,{style:"page"},t.createElement(ps,{exploreId:F.Kd.left,urlQuery:s.left,eventBus:u.current})),x&&t.createElement(A.z4,{style:"page"},t.createElement(ps,{exploreId:F.Kd.right,urlQuery:s.right,eventBus:u.current}))))}const sr=()=>{const e=(0,Y.q)("explore"),n=(0,R.useSelector)(s=>[s.explore.left.datasourceInstance?.name,s.explore.right?.datasourceInstance?.name].filter(D.fQ));document.title=`${e.main.text} - ${n.join(" | ")} - ${B.c.AppTitle}`};function or(e){const{isLoading:n}=(0,te.zJ)(),{hasDatasource:s}=(0,R.useSelector)(l=>({hasDatasource:l.dataSources.dataSourcesCount>0})),[o,a]=(0,t.useState)(T.v.featureToggles.datasourceOnboarding);return s||!o?t.createElement(tr,{...e}):t.createElement(ee.O,{onCTAClick:()=>a(!1),loading:n,title:"Welcome to Grafana Explore!",CTAText:"Or explore sample data",navId:"explore"})}},75090:(ge,Z,i)=>{"use strict";i.d(Z,{F:()=>ee,p:()=>T});var t=i(81168);const T=R=>Boolean(R.explore[t.ExploreId.left]&&R.explore[t.ExploreId.right]),ee=R=>te=>te.explore[R]},39653:(ge,Z,i)=>{"use strict";i.d(Z,{q:()=>t});var t=(T=>(T.Data="data",T.Meta="meta",T.Error="error",T.Stats="stats",T.JSON="json",T.Query="query",T.Help="help",T))(t||{})},3823:(ge,Z,i)=>{"use strict";i.d(Z,{Z:()=>se});var t=i(81582),T=i.n(t),ee=i(68404),R=i(24015),te=function(D){D===void 0&&(D={});var ne=(0,ee.useState)(D),J=ne[0],A=ne[1],U=(0,ee.useCallback)(function(K){A(function(oe){return Object.assign({},oe,K instanceof Function?K(oe):K)})},[]);return[J,U]};const p=te;var I=function(){var D=(0,R.Z)(),ne=p({value:void 0,error:void 0,noUserInteraction:!0}),J=ne[0],A=ne[1],U=(0,ee.useCallback)(function(K){if(D()){var oe,Y;try{if(typeof K!="string"&&typeof K!="number"){var F=new Error("Cannot copy typeof "+typeof K+" to clipboard, must be a string");A({value:K,error:F,noUserInteraction:!0});return}else if(K===""){var F=new Error("Cannot copy empty string to clipboard.");A({value:K,error:F,noUserInteraction:!0});return}Y=K.toString(),oe=T()(Y),A({value:Y,error:void 0,noUserInteraction:oe})}catch(B){A({value:Y,error:B,noUserInteraction:oe})}}},[]);return[J,U]};const se=I},78337:(ge,Z,i)=>{"use strict";i.d(Z,{Z:()=>ee});var t=i(68404),T=function(R,te){var p=(0,t.useRef)(function(){});(0,t.useEffect)(function(){p.current=R}),(0,t.useEffect)(function(){if(te!==null){var I=setInterval(function(){return p.current()},te||0);return function(){return clearInterval(I)}}},[te])};const ee=T},81582:(ge,Z,i)=>{"use strict";var t=i(66259),T={"text/plain":"Text","text/html":"Url",default:"Text"},ee="Copy to clipboard: #{key}, Enter";function R(p){var I=(/mac os x/i.test(navigator.userAgent)?"\u2318":"Ctrl")+"+C";return p.replace(/#{\s*key\s*}/g,I)}function te(p,I){var se,D,ne,J,A,U,K=!1;I||(I={}),se=I.debug||!1;try{ne=t(),J=document.createRange(),A=document.getSelection(),U=document.createElement("span"),U.textContent=p,U.style.all="unset",U.style.position="fixed",U.style.top=0,U.style.clip="rect(0, 0, 0, 0)",U.style.whiteSpace="pre",U.style.webkitUserSelect="text",U.style.MozUserSelect="text",U.style.msUserSelect="text",U.style.userSelect="text",U.addEventListener("copy",function(Y){if(Y.stopPropagation(),I.format)if(Y.preventDefault(),typeof Y.clipboardData>"u"){se&&console.warn("unable to use e.clipboardData"),se&&console.warn("trying IE specific stuff"),window.clipboardData.clearData();var F=T[I.format]||T.default;window.clipboardData.setData(F,p)}else Y.clipboardData.clearData(),Y.clipboardData.setData(I.format,p);I.onCopy&&(Y.preventDefault(),I.onCopy(Y.clipboardData))}),document.body.appendChild(U),J.selectNodeContents(U),A.addRange(J);var oe=document.execCommand("copy");if(!oe)throw new Error("copy command was unsuccessful");K=!0}catch(Y){se&&console.error("unable to copy using execCommand: ",Y),se&&console.warn("trying IE specific stuff");try{window.clipboardData.setData(I.format||"text",p),I.onCopy&&I.onCopy(window.clipboardData),K=!0}catch(F){se&&console.error("unable to copy using clipboardData: ",F),se&&console.error("falling back to prompt"),D=R("message"in I?I.message:ee),window.prompt(D,p)}}finally{A&&(typeof A.removeRange=="function"?A.removeRange(J):A.removeAllRanges()),U&&document.body.removeChild(U),ne()}return K}ge.exports=te},66259:ge=>{ge.exports=function(){var Z=document.getSelection();if(!Z.rangeCount)return function(){};for(var i=document.activeElement,t=[],T=0;T<Z.rangeCount;T++)t.push(Z.getRangeAt(T));switch(i.tagName.toUpperCase()){case"INPUT":case"TEXTAREA":i.blur();break;default:i=null;break}return Z.removeAllRanges(),function(){Z.type==="Caret"&&Z.removeAllRanges(),Z.rangeCount||t.forEach(function(ee){Z.addRange(ee)}),i&&i.focus()}}}}]);

//# sourceMappingURL=explore.9173e5ba8d7434807609.js.map
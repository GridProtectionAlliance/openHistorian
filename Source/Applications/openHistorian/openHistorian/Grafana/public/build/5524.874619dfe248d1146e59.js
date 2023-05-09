"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[5524],{45524:(xe,de,B)=>{B.d(de,{J:()=>fe});var v=B(35582),ge=B.n(v),O=B(68404),A=B(37802),U=B(79396),R=B(27050);const se="ctrl+1",ye=new(ge())(document.body),fe=({children:te,pageId:ae,pageNav:W,isLoading:Y})=>{const[Q,ue]=(0,O.useState)(!1);return(0,O.useEffect)(()=>(ye.bind(se,()=>{ue(Z=>!Z)}),()=>{ye.unbind(se)}),[]),O.createElement(A.AN,{features:R.Z},O.createElement(U.T,{pageNav:W,navId:ae},O.createElement(U.T.Contents,{isLoading:Y},te)),Q?O.createElement(A.zJ,{defaultOpen:!0}):null)}},27050:(xe,de,B)=>{B.d(de,{Z:()=>O,v:()=>v});var v=(A=>(A.NotificationPoliciesV2MatchingInstances="notification-policies.v2.matching-instances",A))(v||{});const O=[{name:"notification-policies.v2.matching-instances",defaultValue:!1}]},37802:(xe,de,B)=>{B.d(de,{AN:()=>Ki,zJ:()=>ta,Qb:()=>Kr});/*! *****************************************************************************
Copyright (c) Microsoft Corporation.

Permission to use, copy, modify, and/or distribute this software for any
purpose with or without fee is hereby granted.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM
LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
PERFORMANCE OF THIS SOFTWARE.
***************************************************************************** */var v=function(){return v=Object.assign||function(t){for(var r,n=1,i=arguments.length;n<i;n++){r=arguments[n];for(var a in r)Object.prototype.hasOwnProperty.call(r,a)&&(t[a]=r[a])}return t},v.apply(this,arguments)};function ge(e,t){var r={};for(var n in e)Object.prototype.hasOwnProperty.call(e,n)&&t.indexOf(n)<0&&(r[n]=e[n]);if(e!=null&&typeof Object.getOwnPropertySymbols=="function")for(var i=0,n=Object.getOwnPropertySymbols(e);i<n.length;i++)t.indexOf(n[i])<0&&Object.prototype.propertyIsEnumerable.call(e,n[i])&&(r[n[i]]=e[n[i]]);return r}function O(e){var t=typeof Symbol=="function"&&Symbol.iterator,r=t&&e[t],n=0;if(r)return r.call(e);if(e&&typeof e.length=="number")return{next:function(){return e&&n>=e.length&&(e=void 0),{value:e&&e[n++],done:!e}}};throw new TypeError(t?"Object is not iterable.":"Symbol.iterator is not defined.")}function A(e,t){var r=typeof Symbol=="function"&&e[Symbol.iterator];if(!r)return e;var n=r.call(e),i,a=[],o;try{for(;(t===void 0||t-- >0)&&!(i=n.next()).done;)a.push(i.value)}catch(u){o={error:u}}finally{try{i&&!i.done&&(r=n.return)&&r.call(n)}finally{if(o)throw o.error}}return a}function U(e,t,r){if(r||arguments.length===2)for(var n=0,i=t.length,a;n<i;n++)(a||!(n in t))&&(a||(a=Array.prototype.slice.call(t,0,n)),a[n]=t[n]);return e.concat(a||Array.prototype.slice.call(t))}var R;(function(e){e.Start="xstate.start",e.Stop="xstate.stop",e.Raise="xstate.raise",e.Send="xstate.send",e.Cancel="xstate.cancel",e.NullEvent="",e.Assign="xstate.assign",e.After="xstate.after",e.DoneState="done.state",e.DoneInvoke="done.invoke",e.Log="xstate.log",e.Init="xstate.init",e.Invoke="xstate.invoke",e.ErrorExecution="error.execution",e.ErrorCommunication="error.communication",e.ErrorPlatform="error.platform",e.ErrorCustom="xstate.error",e.Update="xstate.update",e.Pure="xstate.pure",e.Choose="xstate.choose"})(R||(R={}));var se;(function(e){e.Parent="#_parent",e.Internal="#_internal"})(se||(se={}));var ye=R.Start,fe=R.Stop,te=R.Raise,ae=R.Send,W=R.Cancel,Y=R.NullEvent,Q=R.Assign,ue=R.After,Z=R.DoneState,he=R.Log,nt=R.Init,De=R.Invoke,it=R.ErrorExecution,Ne=R.ErrorPlatform,at=R.ErrorCustom,le=R.Update,Se=R.Choose,ot=R.Pure,Qt=".",qt={},Nt="xstate.guard",Xr="",J=!0,st;function na(e){return Object.keys(e)}function _t(e,t,r){r===void 0&&(r=Qt);var n=Ve(e,r),i=Ve(t,r);return L(i)?L(n)?i===n:!1:L(n)?n in i:Object.keys(n).every(function(a){return a in i?_t(n[a],i[a]):!1})}function Zt(e){try{return L(e)||typeof e=="number"?"".concat(e):e.type}catch{throw new Error("Events must be strings or objects with a string event.type property.")}}function ia(e){try{return L(e)||typeof e=="number"?"".concat(e):G(e)?e.name:e.type}catch{throw new Error("Actions must be strings or objects with a string action.type property.")}}function Tt(e,t){try{return Pe(e)?e:e.toString().split(t)}catch{throw new Error("'".concat(e,"' is not a valid state path."))}}function Yr(e){return typeof e=="object"&&"value"in e&&"context"in e&&"event"in e&&"_event"in e}function Ve(e,t){if(Yr(e))return e.value;if(Pe(e))return ut(e);if(typeof e!="string")return e;var r=Tt(e,t);return ut(r)}function ut(e){if(e.length===1)return e[0];for(var t={},r=t,n=0;n<e.length-1;n++)n===e.length-2?r[e[n]]=e[n+1]:(r[e[n]]={},r=r[e[n]]);return t}function We(e,t){for(var r={},n=Object.keys(e),i=0;i<n.length;i++){var a=n[i];r[a]=t(e[a],a,e,i)}return r}function er(e,t,r){var n,i,a={};try{for(var o=O(Object.keys(e)),u=o.next();!u.done;u=o.next()){var s=u.value,l=e[s];r(l)&&(a[s]=t(l,s,e))}}catch(d){n={error:d}}finally{try{u&&!u.done&&(i=o.return)&&i.call(o)}finally{if(n)throw n.error}}return a}var Qr=function(e){return function(t){var r,n,i=t;try{for(var a=O(e),o=a.next();!o.done;o=a.next()){var u=o.value;i=i[u]}}catch(s){r={error:s}}finally{try{o&&!o.done&&(n=a.return)&&n.call(a)}finally{if(r)throw r.error}}return i}};function qr(e,t){return function(r){var n,i,a=r;try{for(var o=O(e),u=o.next();!u.done;u=o.next()){var s=u.value;a=a[t][s]}}catch(l){n={error:l}}finally{try{u&&!u.done&&(i=o.return)&&i.call(o)}finally{if(n)throw n.error}}return a}}function lt(e){if(!e)return[[]];if(L(e))return[[e]];var t=H(Object.keys(e).map(function(r){var n=e[r];return typeof n!="string"&&(!n||!Object.keys(n).length)?[[r]]:lt(e[r]).map(function(i){return[r].concat(i)})}));return t}function aa(e){var t,r,n={};if(e&&e.length===1&&e[0].length===1)return e[0][0];try{for(var i=__values(e),a=i.next();!a.done;a=i.next())for(var o=a.value,u=n,s=0;s<o.length;s++){var l=o[s];if(s===o.length-2){u[l]=o[s+1];break}u[l]=u[l]||{},u=u[l]}}catch(d){t={error:d}}finally{try{a&&!a.done&&(r=i.return)&&r.call(i)}finally{if(t)throw t.error}}return n}function H(e){var t;return(t=[]).concat.apply(t,U([],A(e),!1))}function tr(e){return Pe(e)?e:[e]}function pe(e){return e===void 0?[]:tr(e)}function ct(e,t,r){var n,i;if(G(e))return e(t,r.data);var a={};try{for(var o=O(Object.keys(e)),u=o.next();!u.done;u=o.next()){var s=u.value,l=e[s];G(l)?a[s]=l(t,r.data):a[s]=l}}catch(d){n={error:d}}finally{try{u&&!u.done&&(i=o.return)&&i.call(o)}finally{if(n)throw n.error}}return a}function Zr(e){return/^(done|error)\./.test(e)}function rr(e){return!!(e instanceof Promise||e!==null&&(G(e)||typeof e=="object")&&G(e.then))}function en(e){return e!==null&&typeof e=="object"&&"transition"in e&&typeof e.transition=="function"}function tn(e,t){var r,n,i=A([[],[]],2),a=i[0],o=i[1];try{for(var u=O(e),s=u.next();!s.done;s=u.next()){var l=s.value;t(l)?a.push(l):o.push(l)}}catch(d){r={error:d}}finally{try{s&&!s.done&&(n=u.return)&&n.call(u)}finally{if(r)throw r.error}}return[a,o]}function nr(e,t){return We(e.states,function(r,n){if(r){var i=(L(t)?void 0:t[n])||(r?r.current:void 0);if(i)return{current:i,states:nr(r,i)}}})}function rn(e,t){return{current:t,states:nr(e,t)}}function ir(e,t,r,n){J||ee(!!e,"Attempting to update undefined context");var i=e&&r.reduce(function(a,o){var u,s,l=o.assignment,d={state:n,action:o,_event:t},h={};if(G(l))h=l(a,t.data,d);else try{for(var c=O(Object.keys(l)),p=c.next();!p.done;p=c.next()){var g=p.value,y=l[g];h[g]=G(y)?y(a,t.data,d):y}}catch(k){u={error:k}}finally{try{p&&!p.done&&(s=c.return)&&s.call(c)}finally{if(u)throw u.error}}return Object.assign({},a,h)},e);return i}var ee=function(){};J||(ee=function(e,t){var r=e instanceof Error?e:void 0;if(!(!r&&e)&&console!==void 0){var n=["Warning: ".concat(t)];r&&n.push(r),console.warn.apply(console,n)}});function Pe(e){return Array.isArray(e)}function G(e){return typeof e=="function"}function L(e){return typeof e=="string"}function ar(e,t){if(e)return L(e)?{type:Nt,name:e,predicate:t?t[e]:void 0}:G(e)?{type:Nt,name:e.name,predicate:e}:e}function nn(e){try{return"subscribe"in e&&G(e.subscribe)}catch{return!1}}var Ee=function(){return typeof Symbol=="function"&&Symbol.observable||"@@observable"}(),oa=(st={},st[Ee]=function(){return this},st[Symbol.observable]=function(){return this},st);function we(e){return!!e&&"__xstatenode"in e}function an(e){return!!e&&typeof e.send=="function"}var on=function(){var e=0;return function(){return e++,e.toString(16)}}();function dt(e,t){return L(e)||typeof e=="number"?v({type:e},t):e}function re(e,t){if(!L(e)&&"$$type"in e&&e.$$type==="scxml")return e;var r=dt(e);return v({name:r.type,data:r,$$type:"scxml",type:"external"},t)}function Ce(e,t){var r=tr(t).map(function(n){return typeof n>"u"||typeof n=="string"||we(n)?{target:n,event:e}:v(v({},n),{event:e})});return r}function sn(e){if(!(e===void 0||e===Xr))return pe(e)}function un(e,t,r){if(!J){var n=e.stack?" Stacktrace was '".concat(e.stack,"'"):"";if(e===t)console.error("Missing onError handler for invocation '".concat(r,"', error was '").concat(e,"'.").concat(n));else{var i=t.stack?" Stacktrace was '".concat(t.stack,"'"):"";console.error("Missing onError handler and/or unhandled exception/promise rejection for invocation '".concat(r,"'. ")+"Original error: '".concat(e,"'. ").concat(n," Current error is '").concat(t,"'.").concat(i))}}}function or(e,t,r,n,i){var a=e.options.guards,o={state:i,cond:t,_event:n};if(t.type===Nt)return(a?.[t.name]||t.predicate)(r,n.data,o);var u=a?.[t.type];if(!u)throw new Error("Guard '".concat(t.type,"' is not implemented on machine '").concat(e.id,"'."));return u(r,n.data,o)}function sr(e){return typeof e=="string"?{type:e}:e}function ft(e,t,r){var n=function(){},i=typeof e=="object",a=i?e:null;return{next:((i?e.next:e)||n).bind(a),error:((i?e.error:t)||n).bind(a),complete:((i?e.complete:r)||n).bind(a)}}function ht(e,t){return"".concat(e,":invocation[").concat(t,"]")}function At(e){return(e.type===te||e.type===ae&&e.to===se.Internal)&&typeof e.delay!="number"}var pt=function(e){return e.type==="atomic"||e.type==="final"};function ur(e){return Object.keys(e.states).map(function(t){return e.states[t]})}function $e(e){return ur(e).filter(function(t){return t.type!=="history"})}function lr(e){var t=[e];return pt(e)?t:t.concat(H($e(e).map(lr)))}function Je(e,t){var r,n,i,a,o,u,s,l,d=new Set(e),h=jt(d),c=new Set(t);try{for(var p=O(c),g=p.next();!g.done;g=p.next())for(var y=g.value,k=y.parent;k&&!c.has(k);)c.add(k),k=k.parent}catch(N){r={error:N}}finally{try{g&&!g.done&&(n=p.return)&&n.call(p)}finally{if(r)throw r.error}}var x=jt(c);try{for(var w=O(c),S=w.next();!S.done;S=w.next()){var y=S.value;if(y.type==="compound"&&(!x.get(y)||!x.get(y).length))h.get(y)?h.get(y).forEach(function($){return c.add($)}):y.initialStateNodes.forEach(function($){return c.add($)});else if(y.type==="parallel")try{for(var _=(o=void 0,O($e(y))),P=_.next();!P.done;P=_.next()){var F=P.value;c.has(F)||(c.add(F),h.get(F)?h.get(F).forEach(function($){return c.add($)}):F.initialStateNodes.forEach(function($){return c.add($)}))}}catch($){o={error:$}}finally{try{P&&!P.done&&(u=_.return)&&u.call(_)}finally{if(o)throw o.error}}}}catch(N){i={error:N}}finally{try{S&&!S.done&&(a=w.return)&&a.call(w)}finally{if(i)throw i.error}}try{for(var I=O(c),C=I.next();!C.done;C=I.next())for(var y=C.value,k=y.parent;k&&!c.has(k);)c.add(k),k=k.parent}catch(N){s={error:N}}finally{try{C&&!C.done&&(l=I.return)&&l.call(I)}finally{if(s)throw s.error}}return c}function cr(e,t){var r=t.get(e);if(!r)return{};if(e.type==="compound"){var n=r[0];if(n){if(pt(n))return n.key}else return{}}var i={};return r.forEach(function(a){i[a.key]=cr(a,t)}),i}function jt(e){var t,r,n=new Map;try{for(var i=O(e),a=i.next();!a.done;a=i.next()){var o=a.value;n.has(o)||n.set(o,[]),o.parent&&(n.has(o.parent)||n.set(o.parent,[]),n.get(o.parent).push(o))}}catch(u){t={error:u}}finally{try{a&&!a.done&&(r=i.return)&&r.call(i)}finally{if(t)throw t.error}}return n}function ln(e,t){var r=Je([e],t);return cr(e,jt(r))}function He(e,t){return Array.isArray(e)?e.some(function(r){return r===t}):e instanceof Set?e.has(t):!1}function cn(e){return U([],A(new Set(H(U([],A(e.map(function(t){return t.ownEvents})),!1)))),!1)}function vt(e,t){return t.type==="compound"?$e(t).some(function(r){return r.type==="final"&&He(e,r)}):t.type==="parallel"?$e(t).every(function(r){return vt(e,r)}):!1}function dn(e){return e===void 0&&(e=[]),e.reduce(function(t,r){return r.meta!==void 0&&(t[r.id]=r.meta),t},{})}function dr(e){return new Set(H(e.map(function(t){return t.tags})))}var Ie=re({type:nt});function Dt(e,t){return t&&t[e]||void 0}function Ke(e,t){var r;if(L(e)||typeof e=="number"){var n=Dt(e,t);G(n)?r={type:e,exec:n}:n?r=n:r={type:e,exec:void 0}}else if(G(e))r={type:e.name||e.toString(),exec:e};else{var n=Dt(e.type,t);if(G(n))r=v(v({},e),{exec:n});else if(n){var i=n.type||e.type;r=v(v(v({},n),e),{type:i})}else r=e}return r}var ke=function(e,t){if(!e)return[];var r=Pe(e)?e:[e];return r.map(function(n){return Ke(n,t)})};function Pt(e){var t=Ke(e);return v(v({id:L(e)?e:t.id},t),{type:t.type})}function fr(e,t){return{type:te,event:typeof e=="function"?e:dt(e),delay:t?t.delay:void 0,id:t?.id}}function fn(e,t,r,n){var i={_event:r},a=re(G(e.event)?e.event(t,r.data,i):e.event),o;if(L(e.delay)){var u=n&&n[e.delay];o=G(u)?u(t,r.data,i):u}else o=G(e.delay)?e.delay(t,r.data,i):e.delay;return v(v({},e),{type:te,_event:a,delay:o})}function Re(e,t){return{to:t?t.to:void 0,type:ae,event:G(e)?e:dt(e),delay:t?t.delay:void 0,id:t&&t.id!==void 0?t.id:G(e)?e.name:Zt(e)}}function hn(e,t,r,n){var i={_event:r},a=re(G(e.event)?e.event(t,r.data,i):e.event),o;if(L(e.delay)){var u=n&&n[e.delay];o=G(u)?u(t,r.data,i):u}else o=G(e.delay)?e.delay(t,r.data,i):e.delay;var s=G(e.to)?e.to(t,r.data,i):e.to;return v(v({},e),{to:s,_event:a,event:a.data,delay:o})}function Ct(e,t){return Re(e,v(v({},t),{to:se.Parent}))}function pn(e,t,r){return Re(t,v(v({},r),{to:e}))}function vn(){return Ct(le)}function sa(e,t){return Re(e,__assign(__assign({},t),{to:function(r,n,i){var a=i._event;return a.origin}}))}var mn=function(e,t){return{context:e,event:t}};function ua(e,t){return e===void 0&&(e=mn),{type:log$1,label:t,expr:e}}var gn=function(e,t,r){return v(v({},e),{value:L(e.expr)?e.expr:e.expr(t,r.data,{_event:r})})},yn=function(e){return{type:W,sendId:e}};function wn(e){var t=Pt(e);return{type:R.Start,activity:t,exec:void 0}}function bn(e){var t=G(e)?e:Pt(e);return{type:R.Stop,activity:t,exec:void 0}}function xn(e,t,r){var n=G(e.activity)?e.activity(t,r.data):e.activity,i=typeof n=="string"?{id:n}:n,a={type:R.Stop,activity:i};return a}var Sn=function(e){return{type:Q,assignment:e}};function la(e){return typeof e=="object"&&"type"in e}function En(e,t){var r=t?"#".concat(t):"";return"".concat(R.After,"(").concat(e,")").concat(r)}function mt(e,t){var r="".concat(R.DoneState,".").concat(e),n={type:r,data:t};return n.toString=function(){return r},n}function Xe(e,t){var r="".concat(R.DoneInvoke,".").concat(e),n={type:r,data:t};return n.toString=function(){return r},n}function Ye(e,t){var r="".concat(R.ErrorPlatform,".").concat(e),n={type:r,data:t};return n.toString=function(){return r},n}function ca(e){return{type:ActionTypes.Pure,get:e}}function kn(e,t){if(!J&&(!e||typeof e=="function")){var r=e;e=function(){for(var n=[],i=0;i<arguments.length;i++)n[i]=arguments[i];var a=typeof r=="function"?r.apply(void 0,U([],A(n),!1)):r;if(!a)throw new Error("Attempted to forward event to undefined actor. This risks an infinite loop in the sender.");return a}}return Re(function(n,i){return i},v(v({},t),{to:e}))}function da(e,t){return Ct(function(r,n,i){return{type:error$1,data:isFunction(e)?e(r,n,i):e}},__assign(__assign({},t),{to:SpecialTargets.Parent}))}function fa(e){return{type:ActionTypes.Choose,conds:e}}var On=function(e){var t,r,n=[];try{for(var i=O(e),a=i.next();!a.done;a=i.next())for(var o=a.value,u=0;u<o.actions.length;){if(o.actions[u].type===Q){n.push(o.actions[u]),o.actions.splice(u,1);continue}u++}}catch(s){t={error:s}}finally{try{a&&!a.done&&(r=i.return)&&r.call(i)}finally{if(t)throw t.error}}return n};function gt(e,t,r,n,i,a,o){o===void 0&&(o=!1);var u=o?[]:On(i),s=u.length?ir(r,n,u,t):r,l=o?[r]:void 0,d=[];function h(g,y){var k;switch(y.type){case te:{var x=fn(y,s,n,e.options.delays);return a&&typeof x.delay=="number"&&a(x,s,n),x}case ae:var w=hn(y,s,n,e.options.delays);if(!J){var S=y.delay;ee(!L(S)||typeof w.delay=="number","No delay reference for delay expression '".concat(S,"' was found on machine '").concat(e.id,"'"))}return a&&w.to!==se.Internal&&(g==="entry"?d.push(w):a(w,s,n)),w;case he:{var _=gn(y,s,n);return a?.(_,s,n),_}case Se:{var P=y,F=(k=P.conds.find(function(be){var j=ar(be.cond,e.options.guards);return!j||or(e,j,s,n,a?void 0:t)}))===null||k===void 0?void 0:k.actions;if(!F)return[];var I=A(gt(e,t,s,n,[{type:g,actions:ke(pe(F),e.options.actions)}],a,o),2),C=I[0],N=I[1];return s=N,l?.push(s),C}case ot:{var F=y.get(s,n.data);if(!F)return[];var $=A(gt(e,t,s,n,[{type:g,actions:ke(pe(F),e.options.actions)}],a,o),2),T=$[0],z=$[1];return s=z,l?.push(s),T}case fe:{var _=xn(y,s,n);return a?.(_,r,n),_}case Q:{s=ir(s,n,[y],a?void 0:t),l?.push(s);break}default:var K=Ke(y,e.options.actions),oe=K.exec;if(a)a(K,s,n);else if(oe&&l){var Ae=l.length-1,Oe=v(v({},K),{exec:function(be){for(var j=[],V=1;V<arguments.length;V++)j[V-1]=arguments[V];oe.apply(void 0,U([l[Ae]],A(j),!1))}});K=Oe}return K}}function c(g){var y,k,x=[];try{for(var w=O(g.actions),S=w.next();!S.done;S=w.next()){var _=S.value,P=h(g.type,_);P&&(x=x.concat(P))}}catch(F){y={error:F}}finally{try{S&&!S.done&&(k=w.return)&&k.call(w)}finally{if(y)throw y.error}}return d.forEach(function(F){a(F,s,n)}),d.length=0,x}var p=H(i.map(c));return[p,s]}function hr(e,t){if(e===t)return!0;if(e===void 0||t===void 0)return!1;if(L(e)||L(t))return e===t;var r=Object.keys(e),n=Object.keys(t);return r.length===n.length&&r.every(function(i){return hr(e[i],t[i])})}function Nn(e){return typeof e!="object"||e===null?!1:"value"in e&&"_event"in e}var ha=null;function _n(e,t){var r=e.exec,n=v(v({},e),{exec:r!==void 0?function(){return r(t.context,t.event,{action:e,state:t,_event:t._event})}:void 0});return n}var ce=function(){function e(t){var r=this,n;this.actions=[],this.activities=qt,this.meta={},this.events=[],this.value=t.value,this.context=t.context,this._event=t._event,this._sessionid=t._sessionid,this.event=this._event.data,this.historyValue=t.historyValue,this.history=t.history,this.actions=t.actions||[],this.activities=t.activities||qt,this.meta=dn(t.configuration),this.events=t.events||[],this.matches=this.matches.bind(this),this.toStrings=this.toStrings.bind(this),this.configuration=t.configuration,this.transitions=t.transitions,this.children=t.children,this.done=!!t.done,this.tags=(n=Array.isArray(t.tags)?new Set(t.tags):t.tags)!==null&&n!==void 0?n:new Set,this.machine=t.machine,Object.defineProperty(this,"nextEvents",{get:function(){return cn(r.configuration)}})}return e.from=function(t,r){if(t instanceof e)return t.context!==r?new e({value:t.value,context:r,_event:t._event,_sessionid:null,historyValue:t.historyValue,history:t.history,actions:[],activities:t.activities,meta:{},events:[],configuration:[],transitions:[],children:{}}):t;var n=Ie;return new e({value:t,context:r,_event:n,_sessionid:null,historyValue:void 0,history:void 0,actions:[],activities:void 0,meta:void 0,events:[],configuration:[],transitions:[],children:{}})},e.create=function(t){return new e(t)},e.inert=function(t,r){if(t instanceof e){if(!t.actions.length)return t;var n=Ie;return new e({value:t.value,context:r,_event:n,_sessionid:null,historyValue:t.historyValue,history:t.history,activities:t.activities,configuration:t.configuration,transitions:[],children:{}})}return e.from(t,r)},e.prototype.toStrings=function(t,r){var n=this;if(t===void 0&&(t=this.value),r===void 0&&(r="."),L(t))return[t];var i=Object.keys(t);return i.concat.apply(i,U([],A(i.map(function(a){return n.toStrings(t[a],r).map(function(o){return a+r+o})})),!1))},e.prototype.toJSON=function(){var t=this;t.configuration,t.transitions;var r=t.tags;t.machine;var n=ge(t,["configuration","transitions","tags","machine"]);return v(v({},n),{tags:Array.from(r)})},e.prototype.matches=function(t){return _t(t,this.value)},e.prototype.hasTag=function(t){return this.tags.has(t)},e.prototype.can=function(t){var r;J&&ee(!!this.machine,"state.can(...) used outside of a machine-created State object; this will always return false.");var n=(r=this.machine)===null||r===void 0?void 0:r.getTransitionData(this,t);return!!n?.transitions.length&&n.transitions.some(function(i){return i.target!==void 0||i.actions.length})},e}(),yt=[],Le=function(e,t){yt.push(e);var r=t(e);return yt.pop(),r},Tn=function(e){return e(yt[yt.length-1])};function pr(e){var t;return t={id:e,send:function(){},subscribe:function(){return{unsubscribe:function(){}}},getSnapshot:function(){},toJSON:function(){return{id:e}}},t[Ee]=function(){return this},t}function An(e,t,r,n){var i,a=sr(e.src),o=(i=t?.options.services)===null||i===void 0?void 0:i[a.type],u=e.data?ct(e.data,r,n):void 0,s=o?It(o,e.id,u):pr(e.id);return s.meta=e,s}function It(e,t,r){var n=pr(t);if(n.deferred=!0,we(e)){var i=n.state=Le(void 0,function(){return(r?e.withContext(r):e).initialState});n.getSnapshot=function(){return i}}return n}function jn(e){try{return typeof e.send=="function"}catch{return!1}}function Dn(e){return jn(e)&&"id"in e}function Pn(e){var t;return v((t={subscribe:function(){return{unsubscribe:function(){}}},id:"anonymous",getSnapshot:function(){}},t[Ee]=function(){return this},t),e)}function Cn(e){if(typeof e=="string"){var t={type:e};return t.toString=function(){return e},t}return e}function wt(e){return v(v({type:De},e),{toJSON:function(){e.onDone,e.onError;var t=ge(e,["onDone","onError"]);return v(v({},t),{type:De,src:Cn(e.src)})}})}var Fe="",Rt="#",Qe="*",Me={},ze=function(e){return e[0]===Rt},In=function(){return{actions:{},guards:{},services:{},activities:{},delays:{}}},Rn=function(e,t,r){var n=r.slice(0,-1).some(function(a){return!("cond"in a)&&!("in"in a)&&(L(a.target)||we(a.target))}),i=t===Fe?"the transient event":"event '".concat(t,"'");ee(!n,"One or more transitions for ".concat(i," on state '").concat(e.id,"' are unreachable. ")+"Make sure that the default transition is the last one defined.")},Ln=function(){function e(t,r,n,i){n===void 0&&(n="context"in t?t.context:void 0);var a=this,o;this.config=t,this._context=n,this.order=-1,this.__xstatenode=!0,this.__cache={events:void 0,relativeValue:new Map,initialStateValue:void 0,initialState:void 0,on:void 0,transitions:void 0,candidates:{},delayedTransitions:void 0},this.idMap={},this.tags=[],this.options=Object.assign(In(),r),this.parent=i?.parent,this.key=this.config.key||i?.key||this.config.id||"(machine)",this.machine=this.parent?this.parent.machine:this,this.path=this.parent?this.parent.path.concat(this.key):[],this.delimiter=this.config.delimiter||(this.parent?this.parent.delimiter:Qt),this.id=this.config.id||U([this.machine.key],A(this.path),!1).join(this.delimiter),this.version=this.parent?this.parent.version:this.config.version,this.type=this.config.type||(this.config.parallel?"parallel":this.config.states&&Object.keys(this.config.states).length?"compound":this.config.history?"history":"atomic"),this.schema=this.parent?this.machine.schema:(o=this.config.schema)!==null&&o!==void 0?o:{},this.description=this.config.description,J||ee(!("parallel"in this.config),'The "parallel" property is deprecated and will be removed in version 4.1. '.concat(this.config.parallel?"Replace with `type: 'parallel'`":"Use `type: '".concat(this.type,"'`")," in the config for state node '").concat(this.id,"' instead.")),this.initial=this.config.initial,this.states=this.config.states?We(this.config.states,function(l,d){var h,c=new e(l,{},void 0,{parent:a,key:d});return Object.assign(a.idMap,v((h={},h[c.id]=c,h),c.idMap)),c}):Me;var u=0;function s(l){var d,h;l.order=u++;try{for(var c=O(ur(l)),p=c.next();!p.done;p=c.next()){var g=p.value;s(g)}}catch(y){d={error:y}}finally{try{p&&!p.done&&(h=c.return)&&h.call(c)}finally{if(d)throw d.error}}}s(this),this.history=this.config.history===!0?"shallow":this.config.history||!1,this._transient=!!this.config.always||(this.config.on?Array.isArray(this.config.on)?this.config.on.some(function(l){var d=l.event;return d===Fe}):Fe in this.config.on:!1),this.strict=!!this.config.strict,this.onEntry=pe(this.config.entry||this.config.onEntry).map(function(l){return Ke(l)}),this.onExit=pe(this.config.exit||this.config.onExit).map(function(l){return Ke(l)}),this.meta=this.config.meta,this.doneData=this.type==="final"?this.config.data:void 0,this.invoke=pe(this.config.invoke).map(function(l,d){var h,c;if(we(l)){var p=ht(a.id,d);return a.machine.options.services=v((h={},h[p]=l,h),a.machine.options.services),wt({src:p,id:p})}else if(L(l.src)){var p=l.id||ht(a.id,d);return wt(v(v({},l),{id:p,src:l.src}))}else if(we(l.src)||G(l.src)){var p=l.id||ht(a.id,d);return a.machine.options.services=v((c={},c[p]=l.src,c),a.machine.options.services),wt(v(v({id:p},l),{src:p}))}else{var g=l.src;return wt(v(v({id:ht(a.id,d)},l),{src:g}))}}),this.activities=pe(this.config.activities).concat(this.invoke).map(function(l){return Pt(l)}),this.transition=this.transition.bind(this),this.tags=pe(this.config.tags)}return e.prototype._init=function(){this.__cache.transitions||lr(this).forEach(function(t){return t.on})},e.prototype.withConfig=function(t,r){var n=this.options,i=n.actions,a=n.activities,o=n.guards,u=n.services,s=n.delays;return new e(this.config,{actions:v(v({},i),t.actions),activities:v(v({},a),t.activities),guards:v(v({},o),t.guards),services:v(v({},u),t.services),delays:v(v({},s),t.delays)},r??this.context)},e.prototype.withContext=function(t){return new e(this.config,this.options,t)},Object.defineProperty(e.prototype,"context",{get:function(){return G(this._context)?this._context():this._context},enumerable:!1,configurable:!0}),Object.defineProperty(e.prototype,"definition",{get:function(){return{id:this.id,key:this.key,version:this.version,context:this.context,type:this.type,initial:this.initial,history:this.history,states:We(this.states,function(t){return t.definition}),on:this.on,transitions:this.transitions,entry:this.onEntry,exit:this.onExit,activities:this.activities||[],meta:this.meta,order:this.order||-1,data:this.doneData,invoke:this.invoke,description:this.description,tags:this.tags}},enumerable:!1,configurable:!0}),e.prototype.toJSON=function(){return this.definition},Object.defineProperty(e.prototype,"on",{get:function(){if(this.__cache.on)return this.__cache.on;var t=this.transitions;return this.__cache.on=t.reduce(function(r,n){return r[n.eventType]=r[n.eventType]||[],r[n.eventType].push(n),r},{})},enumerable:!1,configurable:!0}),Object.defineProperty(e.prototype,"after",{get:function(){return this.__cache.delayedTransitions||(this.__cache.delayedTransitions=this.getDelayedTransitions(),this.__cache.delayedTransitions)},enumerable:!1,configurable:!0}),Object.defineProperty(e.prototype,"transitions",{get:function(){return this.__cache.transitions||(this.__cache.transitions=this.formatTransitions(),this.__cache.transitions)},enumerable:!1,configurable:!0}),e.prototype.getCandidates=function(t){if(this.__cache.candidates[t])return this.__cache.candidates[t];var r=t===Fe,n=this.transitions.filter(function(i){var a=i.eventType===t;return r?a:a||i.eventType===Qe});return this.__cache.candidates[t]=n,n},e.prototype.getDelayedTransitions=function(){var t=this,r=this.config.after;if(!r)return[];var n=function(a,o){var u=G(a)?"".concat(t.id,":delay[").concat(o,"]"):a,s=En(u,t.id);return t.onEntry.push(Re(s,{delay:a})),t.onExit.push(yn(s)),s},i=Pe(r)?r.map(function(a,o){var u=n(a.delay,o);return v(v({},a),{event:u})}):H(Object.keys(r).map(function(a,o){var u=r[a],s=L(u)?{target:u}:u,l=isNaN(+a)?a:+a,d=n(l,o);return pe(s).map(function(h){return v(v({},h),{event:d,delay:l})})}));return i.map(function(a){var o=a.delay;return v(v({},t.formatTransition(a)),{delay:o})})},e.prototype.getStateNodes=function(t){var r,n=this;if(!t)return[];var i=t instanceof ce?t.value:Ve(t,this.delimiter);if(L(i)){var a=this.getStateNode(i).initial;return a!==void 0?this.getStateNodes((r={},r[i]=a,r)):[this,this.states[i]]}var o=Object.keys(i),u=[this];return u.push.apply(u,U([],A(H(o.map(function(s){return n.getStateNode(s).getStateNodes(i[s])}))),!1)),u},e.prototype.handles=function(t){var r=Zt(t);return this.events.includes(r)},e.prototype.resolveState=function(t){var r=t instanceof ce?t:ce.create(t),n=Array.from(Je([],this.getStateNodes(r.value)));return new ce(v(v({},r),{value:this.resolve(r.value),configuration:n,done:vt(n,this),tags:dr(n),machine:this.machine}))},e.prototype.transitionLeafNode=function(t,r,n){var i=this.getStateNode(t),a=i.next(r,n);return!a||!a.transitions.length?this.next(r,n):a},e.prototype.transitionCompoundNode=function(t,r,n){var i=Object.keys(t),a=this.getStateNode(i[0]),o=a._transition(t[i[0]],r,n);return!o||!o.transitions.length?this.next(r,n):o},e.prototype.transitionParallelNode=function(t,r,n){var i,a,o={};try{for(var u=O(Object.keys(t)),s=u.next();!s.done;s=u.next()){var l=s.value,d=t[l];if(d){var h=this.getStateNode(l),c=h._transition(d,r,n);c&&(o[l]=c)}}}catch(x){i={error:x}}finally{try{s&&!s.done&&(a=u.return)&&a.call(u)}finally{if(i)throw i.error}}var p=Object.keys(o).map(function(x){return o[x]}),g=H(p.map(function(x){return x.transitions})),y=p.some(function(x){return x.transitions.length>0});if(!y)return this.next(r,n);var k=H(Object.keys(o).map(function(x){return o[x].configuration}));return{transitions:g,exitSet:H(p.map(function(x){return x.exitSet})),configuration:k,source:r,actions:H(Object.keys(o).map(function(x){return o[x].actions}))}},e.prototype._transition=function(t,r,n){return L(t)?this.transitionLeafNode(t,r,n):Object.keys(t).length===1?this.transitionCompoundNode(t,r,n):this.transitionParallelNode(t,r,n)},e.prototype.getTransitionData=function(t,r){return this._transition(t.value,t,re(r))},e.prototype.next=function(t,r){var n,i,a=this,o=r.name,u=[],s=[],l;try{for(var d=O(this.getCandidates(o)),h=d.next();!h.done;h=d.next()){var c=h.value,p=c.cond,g=c.in,y=t.context,k=g?L(g)&&ze(g)?t.matches(Ve(this.getStateNodeById(g).path,this.delimiter)):_t(Ve(g,this.delimiter),Qr(this.path.slice(0,-2))(t.value)):!0,x=!1;try{x=!p||or(this.machine,p,y,r,t)}catch(_){throw new Error("Unable to evaluate guard '".concat(p.name||p.type,"' in transition for event '").concat(o,"' in state node '").concat(this.id,`':
`).concat(_.message))}if(x&&k){c.target!==void 0&&(s=c.target),u.push.apply(u,U([],A(c.actions),!1)),l=c;break}}}catch(_){n={error:_}}finally{try{h&&!h.done&&(i=d.return)&&i.call(d)}finally{if(n)throw n.error}}if(l){if(!s.length)return{transitions:[l],exitSet:[],configuration:t.value?[this]:[],source:t,actions:u};var w=H(s.map(function(_){return a.getRelativeStateNodes(_,t.historyValue)})),S=!!l.internal;return{transitions:[l],exitSet:S?[]:H(s.map(function(_){return a.getPotentiallyReenteringNodes(_)})),configuration:w,source:t,actions:u}}},e.prototype.getPotentiallyReenteringNodes=function(t){if(this.order<t.order)return[this];for(var r=[],n=this,i=t;n&&n!==i;)r.push(n),n=n.parent;return n!==i?[]:(r.push(i),r)},e.prototype.getActions=function(t,r,n,i,a,o,u){var s,l,d,h,c=this,p=o?Je([],this.getStateNodes(o.value)):[],g=new Set;try{for(var y=O(Array.from(t).sort(function(T,z){return T.order-z.order})),k=y.next();!k.done;k=y.next()){var x=k.value;(!He(p,x)||He(n.exitSet,x)||x.parent&&g.has(x.parent))&&g.add(x)}}catch(T){s={error:T}}finally{try{k&&!k.done&&(l=y.return)&&l.call(y)}finally{if(s)throw s.error}}try{for(var w=O(p),S=w.next();!S.done;S=w.next()){var x=S.value;(!He(t,x)||He(n.exitSet,x.parent))&&n.exitSet.push(x)}}catch(T){d={error:T}}finally{try{S&&!S.done&&(h=w.return)&&h.call(w)}finally{if(d)throw d.error}}n.exitSet.sort(function(T,z){return z.order-T.order});var _=Array.from(g).sort(function(T,z){return T.order-z.order}),P=new Set(n.exitSet),F=H(_.map(function(T){var z=[];if(T.type!=="final")return z;var K=T.parent;if(!K.parent)return z;z.push(mt(T.id,T.doneData),mt(K.id,T.doneData?ct(T.doneData,i,a):void 0));var oe=K.parent;return oe.type==="parallel"&&$e(oe).every(function(Ae){return vt(n.configuration,Ae)})&&z.push(mt(oe.id)),z})),I=_.map(function(T){var z=T.onEntry,K=T.activities.map(function(oe){return wn(oe)});return{type:"entry",actions:ke(u?U(U([],A(z),!1),A(K),!1):U(U([],A(K),!1),A(z),!1),c.machine.options.actions)}}).concat({type:"state_done",actions:F.map(function(T){return fr(T)})}),C=Array.from(P).map(function(T){return{type:"exit",actions:ke(U(U([],A(T.onExit),!1),A(T.activities.map(function(z){return bn(z)})),!1),c.machine.options.actions)}}),N=C.concat({type:"transition",actions:ke(n.actions,this.machine.options.actions)}).concat(I);if(r){var $=ke(H(U([],A(t),!1).sort(function(T,z){return z.order-T.order}).map(function(T){return T.onExit})),this.machine.options.actions).filter(function(T){return!At(T)});return N.concat({type:"stop",actions:$})}return N},e.prototype.transition=function(t,r,n,i){t===void 0&&(t=this.initialState);var a=re(r),o;if(t instanceof ce)o=n===void 0?t:this.resolveState(ce.from(t,n));else{var u=L(t)?this.resolve(ut(this.getResolvedPath(t))):this.resolve(t),s=n??this.machine.context;o=this.resolveState(ce.from(u,s))}if(!J&&a.name===Qe)throw new Error("An event cannot have the wildcard type ('".concat(Qe,"')"));if(this.strict&&!this.events.includes(a.name)&&!Zr(a.name))throw new Error("Machine '".concat(this.id,"' does not accept event '").concat(a.name,"'"));var l=this._transition(o.value,o,a)||{transitions:[],configuration:[],exitSet:[],source:o,actions:[]},d=Je([],this.getStateNodes(o.value)),h=l.configuration.length?Je(d,l.configuration):d;return l.configuration=U([],A(h),!1),this.resolveTransition(l,o,o.context,i,a)},e.prototype.resolveRaisedTransition=function(t,r,n,i){var a,o=t.actions;return t=this.transition(t,r,void 0,i),t._event=n,t.event=n.data,(a=t.actions).unshift.apply(a,U([],A(o),!1)),t},e.prototype.resolveTransition=function(t,r,n,i,a){var o,u,s,l,d=this;a===void 0&&(a=Ie);var h=t.configuration,c=!r||t.transitions.length>0,p=c?t.configuration:r?r.configuration:[],g=vt(p,this),y=c?ln(this.machine,h):void 0,k=r?r.historyValue?r.historyValue:t.source?this.machine.historyValue(r.value):void 0:void 0,x=this.getActions(new Set(p),g,t,n,a,r,i),w=r?v({},r.activities):{};try{for(var S=O(x),_=S.next();!_.done;_=S.next()){var P=_.value;try{for(var F=(s=void 0,O(P.actions)),I=F.next();!I.done;I=F.next()){var C=I.value;C.type===ye?w[C.activity.id||C.activity.type]=C:C.type===fe&&(w[C.activity.id||C.activity.type]=!1)}}catch(me){s={error:me}}finally{try{I&&!I.done&&(l=F.return)&&l.call(F)}finally{if(s)throw s.error}}}}catch(me){o={error:me}}finally{try{_&&!_.done&&(u=S.return)&&u.call(S)}finally{if(o)throw o.error}}var N=A(gt(this,r,n,a,x,i,this.machine.config.predictableActionArguments||this.machine.config.preserveActionOrder),2),$=N[0],T=N[1],z=A(tn($,At),2),K=z[0],oe=z[1],Ae=$.filter(function(me){var Be;return me.type===ye&&((Be=me.activity)===null||Be===void 0?void 0:Be.type)===De}),Oe=Ae.reduce(function(me,Be){return me[Be.activity.id]=An(Be.activity,d.machine,T,a),me},r?v({},r.children):{}),be=new ce({value:y||r.value,context:T,_event:a,_sessionid:r?r._sessionid:null,historyValue:y?k?rn(k,y):void 0:r?r.historyValue:void 0,history:!y||t.source?r:void 0,actions:y?oe:[],activities:y?w:r?r.activities:{},events:[],configuration:p,transitions:t.transitions,children:Oe,done:g,tags:dr(p),machine:this}),j=n!==T;be.changed=a.name===le||j;var V=be.history;V&&delete V.history;var ne=!g&&(this._transient||h.some(function(me){return me._transient}));if(!c&&(!ne||a.name===Fe))return be;var q=be;if(!g)for(ne&&(q=this.resolveRaisedTransition(q,{type:Y},a,i));K.length;){var ie=K.shift();q=this.resolveRaisedTransition(q,ie._event,a,i)}var je=q.changed||(V?!!q.actions.length||j||typeof V.value!=typeof q.value||!hr(q.value,V.value):void 0);return q.changed=je,q.history=V,q},e.prototype.getStateNode=function(t){if(ze(t))return this.machine.getStateNodeById(t);if(!this.states)throw new Error("Unable to retrieve child state '".concat(t,"' from '").concat(this.id,"'; no child states exist."));var r=this.states[t];if(!r)throw new Error("Child state '".concat(t,"' does not exist on '").concat(this.id,"'"));return r},e.prototype.getStateNodeById=function(t){var r=ze(t)?t.slice(Rt.length):t;if(r===this.id)return this;var n=this.machine.idMap[r];if(!n)throw new Error("Child state node '#".concat(r,"' does not exist on machine '").concat(this.id,"'"));return n},e.prototype.getStateNodeByPath=function(t){if(typeof t=="string"&&ze(t))try{return this.getStateNodeById(t.slice(1))}catch{}for(var r=Tt(t,this.delimiter).slice(),n=this;r.length;){var i=r.shift();if(!i.length)break;n=n.getStateNode(i)}return n},e.prototype.resolve=function(t){var r,n=this;if(!t)return this.initialStateValue||Me;switch(this.type){case"parallel":return We(this.initialStateValue,function(a,o){return a?n.getStateNode(o).resolve(t[o]||a):Me});case"compound":if(L(t)){var i=this.getStateNode(t);return i.type==="parallel"||i.type==="compound"?(r={},r[t]=i.initialStateValue,r):t}return Object.keys(t).length?We(t,function(a,o){return a?n.getStateNode(o).resolve(a):Me}):this.initialStateValue||{};default:return t||Me}},e.prototype.getResolvedPath=function(t){if(ze(t)){var r=this.machine.idMap[t.slice(Rt.length)];if(!r)throw new Error("Unable to find state node '".concat(t,"'"));return r.path}return Tt(t,this.delimiter)},Object.defineProperty(e.prototype,"initialStateValue",{get:function(){var t;if(this.__cache.initialStateValue)return this.__cache.initialStateValue;var r;if(this.type==="parallel")r=er(this.states,function(n){return n.initialStateValue||Me},function(n){return n.type!=="history"});else if(this.initial!==void 0){if(!this.states[this.initial])throw new Error("Initial state '".concat(this.initial,"' not found on '").concat(this.key,"'"));r=pt(this.states[this.initial])?this.initial:(t={},t[this.initial]=this.states[this.initial].initialStateValue,t)}else r={};return this.__cache.initialStateValue=r,this.__cache.initialStateValue},enumerable:!1,configurable:!0}),e.prototype.getInitialState=function(t,r){this._init();var n=this.getStateNodes(t);return this.resolveTransition({configuration:n,exitSet:[],transitions:[],source:void 0,actions:[]},void 0,r??this.machine.context,void 0)},Object.defineProperty(e.prototype,"initialState",{get:function(){var t=this.initialStateValue;if(!t)throw new Error("Cannot retrieve initial state from simple state '".concat(this.id,"'."));return this.getInitialState(t)},enumerable:!1,configurable:!0}),Object.defineProperty(e.prototype,"target",{get:function(){var t;if(this.type==="history"){var r=this.config;L(r.target)?t=ze(r.target)?ut(this.machine.getStateNodeById(r.target).path.slice(this.path.length-1)):r.target:t=r.target}return t},enumerable:!1,configurable:!0}),e.prototype.getRelativeStateNodes=function(t,r,n){return n===void 0&&(n=!0),n?t.type==="history"?t.resolveHistory(r):t.initialStateNodes:[t]},Object.defineProperty(e.prototype,"initialStateNodes",{get:function(){var t=this;if(pt(this))return[this];if(this.type==="compound"&&!this.initial)return J||ee(!1,"Compound state node '".concat(this.id,"' has no initial state.")),[this];var r=lt(this.initialStateValue);return H(r.map(function(n){return t.getFromRelativePath(n)}))},enumerable:!1,configurable:!0}),e.prototype.getFromRelativePath=function(t){if(!t.length)return[this];var r=A(t),n=r[0],i=r.slice(1);if(!this.states)throw new Error("Cannot retrieve subPath '".concat(n,"' from node with no states"));var a=this.getStateNode(n);if(a.type==="history")return a.resolveHistory();if(!this.states[n])throw new Error("Child state '".concat(n,"' does not exist on '").concat(this.id,"'"));return this.states[n].getFromRelativePath(i)},e.prototype.historyValue=function(t){if(Object.keys(this.states).length)return{current:t||this.initialStateValue,states:er(this.states,function(r,n){if(!t)return r.historyValue();var i=L(t)?void 0:t[n];return r.historyValue(i||r.initialStateValue)},function(r){return!r.history})}},e.prototype.resolveHistory=function(t){var r=this;if(this.type!=="history")return[this];var n=this.parent;if(!t){var i=this.target;return i?H(lt(i).map(function(o){return n.getFromRelativePath(o)})):n.initialStateNodes}var a=qr(n.path,"states")(t).current;return L(a)?[n.getStateNode(a)]:H(lt(a).map(function(o){return r.history==="deep"?n.getFromRelativePath(o):[n.states[o[0]]]}))},Object.defineProperty(e.prototype,"stateIds",{get:function(){var t=this,r=H(Object.keys(this.states).map(function(n){return t.states[n].stateIds}));return[this.id].concat(r)},enumerable:!1,configurable:!0}),Object.defineProperty(e.prototype,"events",{get:function(){var t,r,n,i;if(this.__cache.events)return this.__cache.events;var a=this.states,o=new Set(this.ownEvents);if(a)try{for(var u=O(Object.keys(a)),s=u.next();!s.done;s=u.next()){var l=s.value,d=a[l];if(d.states)try{for(var h=(n=void 0,O(d.events)),c=h.next();!c.done;c=h.next()){var p=c.value;o.add("".concat(p))}}catch(g){n={error:g}}finally{try{c&&!c.done&&(i=h.return)&&i.call(h)}finally{if(n)throw n.error}}}}catch(g){t={error:g}}finally{try{s&&!s.done&&(r=u.return)&&r.call(u)}finally{if(t)throw t.error}}return this.__cache.events=Array.from(o)},enumerable:!1,configurable:!0}),Object.defineProperty(e.prototype,"ownEvents",{get:function(){var t=new Set(this.transitions.filter(function(r){return!(!r.target&&!r.actions.length&&r.internal)}).map(function(r){return r.eventType}));return Array.from(t)},enumerable:!1,configurable:!0}),e.prototype.resolveTarget=function(t){var r=this;if(t!==void 0)return t.map(function(n){if(!L(n))return n;var i=n[0]===r.delimiter;if(i&&!r.parent)return r.getStateNodeByPath(n.slice(1));var a=i?r.key+n:n;if(r.parent)try{var o=r.parent.getStateNodeByPath(a);return o}catch(u){throw new Error("Invalid transition definition for state node '".concat(r.id,`':
`).concat(u.message))}else return r.getStateNodeByPath(a)})},e.prototype.formatTransition=function(t){var r=this,n=sn(t.target),i="internal"in t?t.internal:n?n.some(function(s){return L(s)&&s[0]===r.delimiter}):!0,a=this.machine.options.guards,o=this.resolveTarget(n),u=v(v({},t),{actions:ke(pe(t.actions)),cond:ar(t.cond,a),target:o,source:this,internal:i,eventType:t.event,toJSON:function(){return v(v({},u),{target:u.target?u.target.map(function(s){return"#".concat(s.id)}):void 0,source:"#".concat(r.id)})}});return u},e.prototype.formatTransitions=function(){var t,r,n=this,i;if(!this.config.on)i=[];else if(Array.isArray(this.config.on))i=this.config.on;else{var a=this.config.on,o=Qe,u=a[o],s=u===void 0?[]:u,l=ge(a,[typeof o=="symbol"?o:o+""]);i=H(Object.keys(l).map(function(w){!J&&w===Fe&&ee(!1,"Empty string transition configs (e.g., `{ on: { '': ... }}`) for transient transitions are deprecated. Specify the transition in the `{ always: ... }` property instead. "+'Please check the `on` configuration for "#'.concat(n.id,'".'));var S=Ce(w,l[w]);return J||Rn(n,w,S),S}).concat(Ce(Qe,s)))}var d=this.config.always?Ce("",this.config.always):[],h=this.config.onDone?Ce(String(mt(this.id)),this.config.onDone):[];J||ee(!(this.config.onDone&&!this.parent),'Root nodes cannot have an ".onDone" transition. Please check the config of "'.concat(this.id,'".'));var c=H(this.invoke.map(function(w){var S=[];return w.onDone&&S.push.apply(S,U([],A(Ce(String(Xe(w.id)),w.onDone)),!1)),w.onError&&S.push.apply(S,U([],A(Ce(String(Ye(w.id)),w.onError)),!1)),S})),p=this.after,g=H(U(U(U(U([],A(h),!1),A(c),!1),A(i),!1),A(d),!1).map(function(w){return pe(w).map(function(S){return n.formatTransition(S)})}));try{for(var y=O(p),k=y.next();!k.done;k=y.next()){var x=k.value;g.push(x)}}catch(w){t={error:w}}finally{try{k&&!k.done&&(r=y.return)&&r.call(y)}finally{if(t)throw t.error}}return g},e}(),vr=!1;function pa(e,t,r){return r===void 0&&(r=e.context),new StateNode(e,t,r)}function mr(e,t){return!J&&!("predictableActionArguments"in e)&&!vr&&(vr=!0,console.warn("It is highly recommended to set `predictableActionArguments` to `true` when using `createMachine`. https://xstate.js.org/docs/guides/actions.html")),new Ln(e,t)}var Ue=Sn,va=Re,ma=pn,ga=Ct,ya=vn,wa=kn,ba=Xe,xa=fr,Fn={deferEvents:!1},gr=function(){function e(t){this.processingEvent=!1,this.queue=[],this.initialized=!1,this.options=v(v({},Fn),t)}return e.prototype.initialize=function(t){if(this.initialized=!0,t){if(!this.options.deferEvents){this.schedule(t);return}this.process(t)}this.flushEvents()},e.prototype.schedule=function(t){if(!this.initialized||this.processingEvent){this.queue.push(t);return}if(this.queue.length!==0)throw new Error("Event queue should be empty when it is not processing events");this.process(t),this.flushEvents()},e.prototype.clear=function(){this.queue=[]},e.prototype.flushEvents=function(){for(var t=this.queue.shift();t;)this.process(t),t=this.queue.shift()},e.prototype.process=function(t){this.processingEvent=!0;try{t()}catch(r){throw this.clear(),r}finally{this.processingEvent=!1}},e}(),Lt=new Map,Mn=0,qe={bookId:function(){return"x:".concat(Mn++)},register:function(e,t){return Lt.set(e,t),e},get:function(e){return Lt.get(e)},free:function(e){Lt.delete(e)}};function Ft(){if(typeof globalThis<"u")return globalThis;if(typeof self<"u")return self;if(typeof window<"u")return window;if(typeof B.g<"u")return B.g;J||console.warn("XState could not find a global object in this environment. Please let the maintainers know and raise an issue here: https://github.com/statelyai/xstate/issues")}function zn(){var e=Ft();if(e&&"__xstate__"in e)return e.__xstate__}function Un(e){if(Ft()){var t=zn();t&&t.register(e)}}function Sa(e,t){return{transition:e,initialState:t}}function Ea(e){var t={error:void 0,data:void 0,status:"pending"};return{transition:function(r,n,i){var a=i.parent,o=i.id,u=i.observers;switch(n.type){case"fulfill":return a?.send(doneInvoke(o,n.data)),{error:void 0,data:n.data,status:"fulfilled"};case"reject":return a?.send(error(o,n.error)),u.forEach(function(s){s.error(n.error)}),{error:n.error,data:void 0,status:"rejected"};default:return r}},initialState:t,start:function(r){var n=r.self;return e().then(function(i){n.send({type:"fulfill",data:i})},function(i){n.send({type:"reject",error:i})}),t}}}function Gn(e,t){t===void 0&&(t={});var r=e.initialState,n=new Set,i=[],a=!1,o=function(){if(!a){for(a=!0;i.length>0;){var l=i.shift();r=e.transition(r,l,s),n.forEach(function(d){return d.next(r)})}a=!1}},u=Pn({id:t.id,send:function(l){i.push(l),o()},getSnapshot:function(){return r},subscribe:function(l,d,h){var c=ft(l,d,h);return n.add(c),c.next(r),{unsubscribe:function(){n.delete(c)}}}}),s={parent:t.parent,self:u,id:t.id||"anonymous",observers:n};return r=e.start?e.start(s):r,u}var Mt={sync:!1,autoForward:!1},X;(function(e){e[e.NotStarted=0]="NotStarted",e[e.Running=1]="Running",e[e.Stopped=2]="Stopped"})(X||(X={}));var Bn=function(){function e(t,r){r===void 0&&(r=e.defaultOptions);var n=this;this.machine=t,this.delayedEventsMap={},this.listeners=new Set,this.contextListeners=new Set,this.stopListeners=new Set,this.doneListeners=new Set,this.eventListeners=new Set,this.sendListeners=new Set,this.initialized=!1,this.status=X.NotStarted,this.children=new Map,this.forwardTo=new Set,this._outgoingQueue=[],this.init=this.start,this.send=function(d,h){if(Pe(d))return n.batch(d),n.state;var c=re(dt(d,h));if(n.status===X.Stopped)return J||ee(!1,'Event "'.concat(c.name,'" was sent to stopped service "').concat(n.machine.id,`". This service has already reached its final state, and will not transition.
Event: `).concat(JSON.stringify(c.data))),n.state;if(n.status!==X.Running&&!n.options.deferEvents)throw new Error('Event "'.concat(c.name,'" was sent to uninitialized service "').concat(n.machine.id,`". Make sure .start() is called for this service, or set { deferEvents: true } in the service options.
Event: `).concat(JSON.stringify(c.data)));return n.scheduler.schedule(function(){n.forward(c);var p=n._nextState(c);n.update(p,c)}),n._state},this.sendTo=function(d,h,c){var p=n.parent&&(h===se.Parent||n.parent.id===h),g=p?n.parent:L(h)?h===se.Internal?n:n.children.get(h)||qe.get(h):an(h)?h:void 0;if(!g){if(!p)throw new Error("Unable to send event to child '".concat(h,"' from service '").concat(n.id,"'."));J||ee(!1,"Service '".concat(n.id,"' has no parent: unable to send event ").concat(d.type));return}if("machine"in g){if(n.status!==X.Stopped||n.parent!==g||n.state.done){var y=v(v({},d),{name:d.name===at?"".concat(Ye(n.id)):d.name,origin:n.sessionId});!c&&n.machine.config.predictableActionArguments?n._outgoingQueue.push([g,y]):g.send(y)}}else!c&&n.machine.config.predictableActionArguments?n._outgoingQueue.push([g,d.data]):g.send(d.data)},this._exec=function(d,h,c,p){p===void 0&&(p=n.machine.options.actions);var g=d.exec||Dt(d.type,p),y=G(g)?g:g?g.exec:d.exec;if(y)try{return y(h,c.data,n.machine.config.predictableActionArguments?{action:d,_event:c}:{action:d,state:n.state,_event:c})}catch(oe){throw n.parent&&n.parent.send({type:"xstate.error",data:oe}),oe}switch(d.type){case te:{var k=d;n.defer(k);break}case ae:var x=d;if(typeof x.delay=="number"){n.defer(x);return}else x.to?n.sendTo(x._event,x.to,c===Ie):n.send(x._event);break;case W:n.cancel(d.sendId);break;case ye:{if(n.status!==X.Running)return;var w=d.activity;if(!n.machine.config.predictableActionArguments&&!n.state.activities[w.id||w.type])break;if(w.type===R.Invoke){var S=sr(w.src),_=n.machine.options.services?n.machine.options.services[S.type]:void 0,P=w.id,F=w.data;J||ee(!("forward"in w),"`forward` property is deprecated (found in invocation of '".concat(w.src,"' in in machine '").concat(n.machine.id,"'). ")+"Please use `autoForward` instead.");var I="autoForward"in w?w.autoForward:!!w.forward;if(!_){J||ee(!1,"No service found for invocation '".concat(w.src,"' in machine '").concat(n.machine.id,"'."));return}var C=F?ct(F,h,c):void 0;if(typeof _=="string")return;var N=G(_)?_(h,c.data,{data:C,src:S,meta:w.meta}):_;if(!N)return;var $=void 0;we(N)&&(N=C?N.withContext(C):N,$={autoForward:I}),n.spawn(N,P,$)}else n.spawnActivity(w);break}case fe:{n.stopChild(d.activity.id);break}case he:var T=d,z=T.label,K=T.value;z?n.logger(z,K):n.logger(K);break;default:J||ee(!1,"No implementation found for action type '".concat(d.type,"'"));break}};var i=v(v({},e.defaultOptions),r),a=i.clock,o=i.logger,u=i.parent,s=i.id,l=s!==void 0?s:t.id;this.id=l,this.logger=o,this.clock=a,this.parent=u,this.options=i,this.scheduler=new gr({deferEvents:this.options.deferEvents}),this.sessionId=qe.bookId()}return Object.defineProperty(e.prototype,"initialState",{get:function(){var t=this;return this._initialState?this._initialState:Le(this,function(){return t._initialState=t.machine.initialState,t._initialState})},enumerable:!1,configurable:!0}),Object.defineProperty(e.prototype,"state",{get:function(){return J||ee(this.status!==X.NotStarted,"Attempted to read state from uninitialized service '".concat(this.id,"'. Make sure the service is started first.")),this._state},enumerable:!1,configurable:!0}),e.prototype.execute=function(t,r){var n,i;try{for(var a=O(t.actions),o=a.next();!o.done;o=a.next()){var u=o.value;this.exec(u,t,r)}}catch(s){n={error:s}}finally{try{o&&!o.done&&(i=a.return)&&i.call(a)}finally{if(n)throw n.error}}},e.prototype.update=function(t,r){var n,i,a,o,u,s,l,d,h=this;if(t._sessionid=this.sessionId,this._state=t,(!this.machine.config.predictableActionArguments||r===Ie)&&this.options.execute)this.execute(this.state);else for(var c=void 0;c=this._outgoingQueue.shift();)c[0].send(c[1]);if(this.children.forEach(function(N){h.state.children[N.id]=N}),this.devTools&&this.devTools.send(r.data,t),t.event)try{for(var p=O(this.eventListeners),g=p.next();!g.done;g=p.next()){var y=g.value;y(t.event)}}catch(N){n={error:N}}finally{try{g&&!g.done&&(i=p.return)&&i.call(p)}finally{if(n)throw n.error}}try{for(var k=O(this.listeners),x=k.next();!x.done;x=k.next()){var y=x.value;y(t,t.event)}}catch(N){a={error:N}}finally{try{x&&!x.done&&(o=k.return)&&o.call(k)}finally{if(a)throw a.error}}try{for(var w=O(this.contextListeners),S=w.next();!S.done;S=w.next()){var _=S.value;_(this.state.context,this.state.history?this.state.history.context:void 0)}}catch(N){u={error:N}}finally{try{S&&!S.done&&(s=w.return)&&s.call(w)}finally{if(u)throw u.error}}if(this.state.done){var P=t.configuration.find(function(N){return N.type==="final"&&N.parent===h.machine}),F=P&&P.doneData?ct(P.doneData,t.context,r):void 0;this._doneEvent=Xe(this.id,F);try{for(var I=O(this.doneListeners),C=I.next();!C.done;C=I.next()){var y=C.value;y(this._doneEvent)}}catch(N){l={error:N}}finally{try{C&&!C.done&&(d=I.return)&&d.call(I)}finally{if(l)throw l.error}}this._stop(),this._stopChildren(),qe.free(this.sessionId)}},e.prototype.onTransition=function(t){return this.listeners.add(t),this.status===X.Running&&t(this.state,this.state.event),this},e.prototype.subscribe=function(t,r,n){var i=this,a=ft(t,r,n);this.listeners.add(a.next),this.status!==X.NotStarted&&a.next(this.state);var o=function(){i.doneListeners.delete(o),i.stopListeners.delete(o),a.complete()};return this.status===X.Stopped?a.complete():(this.onDone(o),this.onStop(o)),{unsubscribe:function(){i.listeners.delete(a.next),i.doneListeners.delete(o),i.stopListeners.delete(o)}}},e.prototype.onEvent=function(t){return this.eventListeners.add(t),this},e.prototype.onSend=function(t){return this.sendListeners.add(t),this},e.prototype.onChange=function(t){return this.contextListeners.add(t),this},e.prototype.onStop=function(t){return this.stopListeners.add(t),this},e.prototype.onDone=function(t){return this.status===X.Stopped&&this._doneEvent?t(this._doneEvent):this.doneListeners.add(t),this},e.prototype.off=function(t){return this.listeners.delete(t),this.eventListeners.delete(t),this.sendListeners.delete(t),this.stopListeners.delete(t),this.doneListeners.delete(t),this.contextListeners.delete(t),this},e.prototype.start=function(t){var r=this;if(this.status===X.Running)return this;this.machine._init(),qe.register(this.sessionId,this),this.initialized=!0,this.status=X.Running;var n=t===void 0?this.initialState:Le(this,function(){return Nn(t)?r.machine.resolveState(t):r.machine.resolveState(ce.from(t,r.machine.context))});return this.options.devTools&&this.attachDev(),this.scheduler.initialize(function(){r.update(n,Ie)}),this},e.prototype._stopChildren=function(){this.children.forEach(function(t){G(t.stop)&&t.stop()}),this.children.clear()},e.prototype._stop=function(){var t,r,n,i,a,o,u,s,l,d;try{for(var h=O(this.listeners),c=h.next();!c.done;c=h.next()){var p=c.value;this.listeners.delete(p)}}catch(I){t={error:I}}finally{try{c&&!c.done&&(r=h.return)&&r.call(h)}finally{if(t)throw t.error}}try{for(var g=O(this.stopListeners),y=g.next();!y.done;y=g.next()){var p=y.value;p(),this.stopListeners.delete(p)}}catch(I){n={error:I}}finally{try{y&&!y.done&&(i=g.return)&&i.call(g)}finally{if(n)throw n.error}}try{for(var k=O(this.contextListeners),x=k.next();!x.done;x=k.next()){var p=x.value;this.contextListeners.delete(p)}}catch(I){a={error:I}}finally{try{x&&!x.done&&(o=k.return)&&o.call(k)}finally{if(a)throw a.error}}try{for(var w=O(this.doneListeners),S=w.next();!S.done;S=w.next()){var p=S.value;this.doneListeners.delete(p)}}catch(I){u={error:I}}finally{try{S&&!S.done&&(s=w.return)&&s.call(w)}finally{if(u)throw u.error}}if(!this.initialized)return this;this.initialized=!1,this.status=X.Stopped,this._initialState=void 0;try{for(var _=O(Object.keys(this.delayedEventsMap)),P=_.next();!P.done;P=_.next()){var F=P.value;this.clock.clearTimeout(this.delayedEventsMap[F])}}catch(I){l={error:I}}finally{try{P&&!P.done&&(d=_.return)&&d.call(_)}finally{if(l)throw l.error}}this.scheduler.clear(),this.scheduler=new gr({deferEvents:this.options.deferEvents})},e.prototype.stop=function(){var t=this,r=this.scheduler;return this._stop(),r.schedule(function(){var n=re({type:"xstate.stop"}),i=Le(t,function(){var a=H(U([],A(t.state.configuration),!1).sort(function(d,h){return h.order-d.order}).map(function(d){return ke(d.onExit,t.machine.options.actions)})),o=A(gt(t.machine,t.state,t.state.context,n,[{type:"exit",actions:a}],t.machine.config.predictableActionArguments?t._exec:void 0,t.machine.config.predictableActionArguments||t.machine.config.preserveActionOrder),2),u=o[0],s=o[1],l=new ce({value:t.state.value,context:s,_event:n,_sessionid:t.sessionId,historyValue:void 0,history:t.state,actions:u.filter(function(d){return!At(d)}),activities:{},events:[],configuration:[],transitions:[],children:{},done:t.state.done,tags:t.state.tags,machine:t.machine});return l.changed=!0,l});t.update(i,n),t._stopChildren(),qe.free(t.sessionId)}),this},e.prototype.batch=function(t){var r=this;if(this.status===X.NotStarted&&this.options.deferEvents)J||ee(!1,"".concat(t.length,' event(s) were sent to uninitialized service "').concat(this.machine.id,`" and are deferred. Make sure .start() is called for this service.
Event: `).concat(JSON.stringify(event)));else if(this.status!==X.Running)throw new Error("".concat(t.length,' event(s) were sent to uninitialized service "').concat(this.machine.id,'". Make sure .start() is called for this service, or set { deferEvents: true } in the service options.'));if(t.length){var n=!!this.machine.config.predictableActionArguments&&this._exec;this.scheduler.schedule(function(){var i,a,o=r.state,u=!1,s=[],l=function(p){var g=re(p);r.forward(g),o=Le(r,function(){return r.machine.transition(o,g,void 0,n||void 0)}),s.push.apply(s,U([],A(r.machine.config.predictableActionArguments?o.actions:o.actions.map(function(y){return _n(y,o)})),!1)),u=u||!!o.changed};try{for(var d=O(t),h=d.next();!h.done;h=d.next()){var c=h.value;l(c)}}catch(p){i={error:p}}finally{try{h&&!h.done&&(a=d.return)&&a.call(d)}finally{if(i)throw i.error}}o.changed=u,o.actions=s,r.update(o,re(t[t.length-1]))})}},e.prototype.sender=function(t){return this.send.bind(this,t)},e.prototype._nextState=function(t,r){var n=this;r===void 0&&(r=!!this.machine.config.predictableActionArguments&&this._exec);var i=re(t);if(i.name.indexOf(Ne)===0&&!this.state.nextEvents.some(function(o){return o.indexOf(Ne)===0}))throw i.data.data;var a=Le(this,function(){return n.machine.transition(n.state,i,void 0,r||void 0)});return a},e.prototype.nextState=function(t){return this._nextState(t,!1)},e.prototype.forward=function(t){var r,n;try{for(var i=O(this.forwardTo),a=i.next();!a.done;a=i.next()){var o=a.value,u=this.children.get(o);if(!u)throw new Error("Unable to forward event '".concat(t,"' from interpreter '").concat(this.id,"' to nonexistant child '").concat(o,"'."));u.send(t)}}catch(s){r={error:s}}finally{try{a&&!a.done&&(n=i.return)&&n.call(i)}finally{if(r)throw r.error}}},e.prototype.defer=function(t){var r=this,n=this.clock.setTimeout(function(){"to"in t&&t.to?r.sendTo(t._event,t.to,!0):r.send(t._event)},t.delay);t.id&&(this.delayedEventsMap[t.id]=n)},e.prototype.cancel=function(t){this.clock.clearTimeout(this.delayedEventsMap[t]),delete this.delayedEventsMap[t]},e.prototype.exec=function(t,r,n){n===void 0&&(n=this.machine.options.actions),this._exec(t,r.context,r._event,n)},e.prototype.removeChild=function(t){var r;this.children.delete(t),this.forwardTo.delete(t),(r=this.state)===null||r===void 0||delete r.children[t]},e.prototype.stopChild=function(t){var r=this.children.get(t);r&&(this.removeChild(t),G(r.stop)&&r.stop())},e.prototype.spawn=function(t,r,n){if(this.status!==X.Running)return It(t,r);if(rr(t))return this.spawnPromise(Promise.resolve(t),r);if(G(t))return this.spawnCallback(t,r);if(Dn(t))return this.spawnActor(t,r);if(nn(t))return this.spawnObservable(t,r);if(we(t))return this.spawnMachine(t,v(v({},n),{id:r}));if(en(t))return this.spawnBehavior(t,r);throw new Error('Unable to spawn entity "'.concat(r,'" of type "').concat(typeof t,'".'))},e.prototype.spawnMachine=function(t,r){var n=this;r===void 0&&(r={});var i=new e(t,v(v({},this.options),{parent:this,id:r.id||t.id})),a=v(v({},Mt),r);a.sync&&i.onTransition(function(u){n.send(le,{state:u,id:i.id})});var o=i;return this.children.set(i.id,o),a.autoForward&&this.forwardTo.add(i.id),i.onDone(function(u){n.removeChild(i.id),n.send(re(u,{origin:i.id}))}).start(),o},e.prototype.spawnBehavior=function(t,r){var n=Gn(t,{id:r,parent:this});return this.children.set(r,n),n},e.prototype.spawnPromise=function(t,r){var n,i=this,a=!1,o;t.then(function(s){a||(o=s,i.removeChild(r),i.send(re(Xe(r,s),{origin:r})))},function(s){if(!a){i.removeChild(r);var l=Ye(r,s);try{i.send(re(l,{origin:r}))}catch(d){un(s,d,r),i.devTools&&i.devTools.send(l,i.state),i.machine.strict&&i.stop()}}});var u=(n={id:r,send:function(){},subscribe:function(s,l,d){var h=ft(s,l,d),c=!1;return t.then(function(p){c||(h.next(p),!c&&h.complete())},function(p){c||h.error(p)}),{unsubscribe:function(){return c=!0}}},stop:function(){a=!0},toJSON:function(){return{id:r}},getSnapshot:function(){return o}},n[Ee]=function(){return this},n);return this.children.set(r,u),u},e.prototype.spawnCallback=function(t,r){var n,i=this,a=!1,o=new Set,u=new Set,s,l=function(c){s=c,u.forEach(function(p){return p(c)}),!a&&i.send(re(c,{origin:r}))},d;try{d=t(l,function(c){o.add(c)})}catch(c){this.send(Ye(r,c))}if(rr(d))return this.spawnPromise(d,r);var h=(n={id:r,send:function(c){return o.forEach(function(p){return p(c)})},subscribe:function(c){var p=ft(c);return u.add(p.next),{unsubscribe:function(){u.delete(p.next)}}},stop:function(){a=!0,G(d)&&d()},toJSON:function(){return{id:r}},getSnapshot:function(){return s}},n[Ee]=function(){return this},n);return this.children.set(r,h),h},e.prototype.spawnObservable=function(t,r){var n,i=this,a,o=t.subscribe(function(s){a=s,i.send(re(s,{origin:r}))},function(s){i.removeChild(r),i.send(re(Ye(r,s),{origin:r}))},function(){i.removeChild(r),i.send(re(Xe(r),{origin:r}))}),u=(n={id:r,send:function(){},subscribe:function(s,l,d){return t.subscribe(s,l,d)},stop:function(){return o.unsubscribe()},getSnapshot:function(){return a},toJSON:function(){return{id:r}}},n[Ee]=function(){return this},n);return this.children.set(r,u),u},e.prototype.spawnActor=function(t,r){return this.children.set(r,t),t},e.prototype.spawnActivity=function(t){var r=this.machine.options&&this.machine.options.activities?this.machine.options.activities[t.type]:void 0;if(!r){J||ee(!1,"No implementation found for activity '".concat(t.type,"'"));return}var n=r(this.state.context,t);this.spawnEffect(t.id,n)},e.prototype.spawnEffect=function(t,r){var n;this.children.set(t,(n={id:t,send:function(){},subscribe:function(){return{unsubscribe:function(){}}},stop:r||void 0,getSnapshot:function(){},toJSON:function(){return{id:t}}},n[Ee]=function(){return this},n))},e.prototype.attachDev=function(){var t=Ft();if(this.options.devTools&&t){if(t.__REDUX_DEVTOOLS_EXTENSION__){var r=typeof this.options.devTools=="object"?this.options.devTools:void 0;this.devTools=t.__REDUX_DEVTOOLS_EXTENSION__.connect(v(v({name:this.id,autoPause:!0,stateSanitizer:function(n){return{value:n.value,context:n.context,actions:n.actions}}},r),{features:v({jump:!1,skip:!1},r?r.features:void 0)}),this.machine),this.devTools.init(this.state)}Un(this)}},e.prototype.toJSON=function(){return{id:this.id}},e.prototype[Ee]=function(){return this},e.prototype.getSnapshot=function(){return this.status===X.NotStarted?this.initialState:this._state},e.defaultOptions={execute:!0,deferEvents:!0,clock:{setTimeout:function(t,r){return setTimeout(t,r)},clearTimeout:function(t){return clearTimeout(t)}},logger:console.log.bind(console),devTools:!1},e.interpret=yr,e}(),Vn=function(e){return L(e)?v(v({},Mt),{name:e}):v(v(v({},Mt),{name:on()}),e)};function Wn(e,t){var r=Vn(t);return Tn(function(n){if(!J){var i=we(e)||G(e);ee(!!n||i,'Attempted to spawn an Actor (ID: "'.concat(we(e)?e.id:"undefined",'") outside of a service. This will have no effect.'))}return n?n.spawn(e,r.name,r):It(e,r.name)})}function yr(e,t){var r=new Bn(e,t);return r}var f=B(68404),$n=B(43297),Jn=B(1376);function wr(e){var t=f.useRef();return t.current||(t.current={v:e()}),t.current.v}var bt=function(){return bt=Object.assign||function(e){for(var t,r=1,n=arguments.length;r<n;r++){t=arguments[r];for(var i in t)Object.prototype.hasOwnProperty.call(t,i)&&(e[i]=t[i])}return e},bt.apply(this,arguments)},Hn=function(e,t){var r={};for(var n in e)Object.prototype.hasOwnProperty.call(e,n)&&t.indexOf(n)<0&&(r[n]=e[n]);if(e!=null&&typeof Object.getOwnPropertySymbols=="function")for(var i=0,n=Object.getOwnPropertySymbols(e);i<n.length;i++)t.indexOf(n[i])<0&&Object.prototype.propertyIsEnumerable.call(e,n[i])&&(r[n[i]]=e[n[i]]);return r},Kn=function(e,t){var r=typeof Symbol=="function"&&e[Symbol.iterator];if(!r)return e;var n=r.call(e),i,a=[],o;try{for(;(t===void 0||t-- >0)&&!(i=n.next()).done;)a.push(i.value)}catch(u){o={error:u}}finally{try{i&&!i.done&&(r=n.return)&&r.call(n)}finally{if(o)throw o.error}}return a};function br(e,t){var r=wr(function(){return typeof e=="function"?e():e});if(!1)var n,i;var a=t.context,o=t.guards,u=t.actions,s=t.activities,l=t.services,d=t.delays,h=t.state,c=Hn(t,["context","guards","actions","activities","services","delays","state"]),p=wr(function(){var g={context:a,guards:o,actions:u,activities:s,services:l,delays:d},y=r.withConfig(g,function(){return bt(bt({},r.context),a)});return yr(y,c)});return(0,Jn.Z)(function(){Object.assign(p.machine.options.actions,u),Object.assign(p.machine.options.guards,o),Object.assign(p.machine.options.activities,s),Object.assign(p.machine.options.services,l),Object.assign(p.machine.options.delays,d)},[u,o,s,l,d]),p}function ka(e){for(var t=[],r=1;r<arguments.length;r++)t[r-1]=arguments[r];var n=Kn(t,2),i=n[0],a=i===void 0?{}:i,o=n[1],u=br(e,a);return useEffect(function(){if(o){var s=u.subscribe(toObserver(o));return function(){s.unsubscribe()}}},[o]),useEffect(function(){var s=a.state;return u.start(s?State.create(s):void 0),function(){u.stop(),u.status=InterpreterStatus.NotStarted}},[]),u}var Xn=function(e,t){var r=typeof Symbol=="function"&&e[Symbol.iterator];if(!r)return e;var n=r.call(e),i,a=[],o;try{for(;(t===void 0||t-- >0)&&!(i=n.next()).done;)a.push(i.value)}catch(u){o={error:u}}finally{try{i&&!i.done&&(r=n.return)&&r.call(n)}finally{if(o)throw o.error}}return a},Yn=function(e){var t=typeof Symbol=="function"&&Symbol.iterator,r=t&&e[t],n=0;if(r)return r.call(e);if(e&&typeof e.length=="number")return{next:function(){return e&&n>=e.length&&(e=void 0),{value:e&&e[n++],done:!e}}};throw new TypeError(t?"Object is not iterable.":"Symbol.iterator is not defined.")};function Oa(e,t){var r,n,i=Xn([[],[]],2),a=i[0],o=i[1];try{for(var u=Yn(e),s=u.next();!s.done;s=u.next()){var l=s.value;t(l)?a.push(l):o.push(l)}}catch(d){r={error:d}}finally{try{s&&!s.done&&(n=u.return)&&n.call(u)}finally{if(r)throw r.error}}return[a,o]}function Na(e){return e.status!==0?e.getSnapshot():e.machine.initialState}function xr(e,t){return e===t?e!==0||t!==0||1/e===1/t:e!==e&&t!==t}function _a(e,t){if(xr(e,t))return!0;if(typeof e!="object"||e===null||typeof t!="object"||t===null)return!1;var r=Object.keys(e),n=Object.keys(t);if(r.length!==n.length)return!1;for(var i=0;i<r.length;i++)if(!Object.prototype.hasOwnProperty.call(t,r[i])||!xr(e[r[i]],t[r[i]]))return!1;return!0}function Ta(e){return"state"in e&&"machine"in e}function Qn(e,t,r){if(e.status===X.NotStarted)return!0;var n=r.changed===void 0&&(Object.keys(r.children).length>0||typeof t.changed=="boolean");return!(r.changed||n)}var qn=function(e,t){var r=typeof Symbol=="function"&&e[Symbol.iterator];if(!r)return e;var n=r.call(e),i,a=[],o;try{for(;(t===void 0||t-- >0)&&!(i=n.next()).done;)a.push(i.value)}catch(u){o={error:u}}finally{try{i&&!i.done&&(r=n.return)&&r.call(n)}finally{if(o)throw o.error}}return a};function Zn(e){return e}function Sr(e){for(var t=[],r=1;r<arguments.length;r++)t[r-1]=arguments[r];var n=qn(t,1),i=n[0],a=i===void 0?{}:i,o=br(e,a),u=(0,f.useCallback)(function(){return o.status===X.NotStarted?a.state?ce.create(a.state):o.machine.initialState:o.getSnapshot()},[o]),s=(0,f.useCallback)(function(h,c){return Qn(o,h,c)},[o]),l=(0,f.useCallback)(function(h){var c=o.subscribe(h).unsubscribe;return c},[o]),d=(0,$n.useSyncExternalStoreWithSelector)(l,u,u,Zn,s);return(0,f.useEffect)(function(){var h=a.state;return o.start(h?ce.create(h):void 0),function(){o.stop(),o.status=X.NotStarted}},[]),[d,o.send,o]}var ei=B(97394);function ti(...e){return e.filter(Boolean).join(" ")}function zt(e,t,...r){if(e in t){let i=t[e];return typeof i=="function"?i(...r):i}let n=new Error(`Tried to handle "${e}" but there is no handler defined. Only defined handlers are: ${Object.keys(t).map(i=>`"${i}"`).join(", ")}.`);throw Error.captureStackTrace&&Error.captureStackTrace(n,zt),n}var ri=(e=>(e[e.None=0]="None",e[e.RenderStrategy=1]="RenderStrategy",e[e.Static=2]="Static",e))(ri||{}),ni=(e=>(e[e.Unmount=0]="Unmount",e[e.Hidden=1]="Hidden",e))(ni||{});function Ze({ourProps:e,theirProps:t,slot:r,defaultTag:n,features:i,visible:a=!0,name:o}){let u=Er(t,e);if(a)return xt(u,r,n,o);let s=i??0;if(s&2){let{static:l=!1,...d}=u;if(l)return xt(d,r,n,o)}if(s&1){let{unmount:l=!0,...d}=u;return zt(l?0:1,{[0](){return null},[1](){return xt({...d,hidden:!0,style:{display:"none"}},r,n,o)}})}return xt(u,r,n,o)}function xt(e,t={},r,n){var i;let{as:a=r,children:o,refName:u="ref",...s}=Gt(e,["unmount","static"]),l=e.ref!==void 0?{[u]:e.ref}:{},d=typeof o=="function"?o(t):o;s.className&&typeof s.className=="function"&&(s.className=s.className(t));let h={};if(t){let c=!1,p=[];for(let[g,y]of Object.entries(t))typeof y=="boolean"&&(c=!0),y===!0&&p.push(g);c&&(h["data-headlessui-state"]=p.join(" "))}if(a===f.Fragment&&Object.keys(Ut(s)).length>0){if(!(0,f.isValidElement)(d)||Array.isArray(d)&&d.length>1)throw new Error(['Passing props on "Fragment"!',"",`The current component <${n} /> is rendering a "Fragment".`,"However we need to passthrough the following props:",Object.keys(s).map(g=>`  - ${g}`).join(`
`),"","You can apply a few solutions:",['Add an `as="..."` prop, to ensure that we render an actual element instead of a "Fragment".',"Render a single element as the child so that we can forward the props onto that element."].map(g=>`  - ${g}`).join(`
`)].join(`
`));let c=ti((i=d.props)==null?void 0:i.className,s.className),p=c?{className:c}:{};return(0,f.cloneElement)(d,Object.assign({},Er(d.props,Ut(Gt(s,["ref"]))),h,l,ii(d.ref,l.ref),p))}return(0,f.createElement)(a,Object.assign({},Gt(s,["ref"]),a!==f.Fragment&&l,a!==f.Fragment&&h),d)}function ii(...e){return{ref:e.every(t=>t==null)?void 0:t=>{for(let r of e)r!=null&&(typeof r=="function"?r(t):r.current=t)}}}function Er(...e){var t;if(e.length===0)return{};if(e.length===1)return e[0];let r={},n={};for(let i of e)for(let a in i)a.startsWith("on")&&typeof i[a]=="function"?((t=n[a])!=null||(n[a]=[]),n[a].push(i[a])):r[a]=i[a];if(r.disabled||r["aria-disabled"])return Object.assign(r,Object.fromEntries(Object.keys(n).map(i=>[i,void 0])));for(let i in n)Object.assign(r,{[i](a,...o){let u=n[i];for(let s of u){if((a instanceof Event||a?.nativeEvent instanceof Event)&&a.defaultPrevented)return;s(a,...o)}}});return r}function et(e){var t;return Object.assign((0,f.forwardRef)(e),{displayName:(t=e.displayName)!=null?t:e.name})}function Ut(e){let t=Object.assign({},e);for(let r in t)t[r]===void 0&&delete t[r];return t}function Gt(e,t=[]){let r=Object.assign({},e);for(let n of t)n in r&&delete r[n];return r}var ai=Object.defineProperty,oi=(e,t,r)=>t in e?ai(e,t,{enumerable:!0,configurable:!0,writable:!0,value:r}):e[t]=r,Bt=(e,t,r)=>(oi(e,typeof t!="symbol"?t+"":t,r),r);class si{constructor(){Bt(this,"current",this.detect()),Bt(this,"handoffState","pending"),Bt(this,"currentId",0)}set(t){this.current!==t&&(this.handoffState="pending",this.currentId=0,this.current=t)}reset(){this.set(this.detect())}nextId(){return++this.currentId}get isServer(){return this.current==="server"}get isClient(){return this.current==="client"}detect(){return typeof window>"u"||typeof document>"u"?"server":"client"}handoff(){this.handoffState==="pending"&&(this.handoffState="complete")}get isHandoffComplete(){return this.handoffState==="complete"}}let _e=new si,Ge=(e,t)=>{_e.isServer?(0,f.useEffect)(e,t):(0,f.useLayoutEffect)(e,t)};function ui(){let[e,t]=(0,f.useState)(_e.isHandoffComplete);return e&&_e.isHandoffComplete===!1&&t(!1),(0,f.useEffect)(()=>{e!==!0&&t(!0)},[e]),(0,f.useEffect)(()=>_e.handoff(),[]),e}var kr;let St=(kr=f.useId)!=null?kr:function(){let e=ui(),[t,r]=f.useState(e?()=>_e.nextId():null);return Ge(()=>{t===null&&r(_e.nextId())},[t]),t!=null?""+t:void 0};var Te=(e=>(e.Space=" ",e.Enter="Enter",e.Escape="Escape",e.Backspace="Backspace",e.Delete="Delete",e.ArrowLeft="ArrowLeft",e.ArrowUp="ArrowUp",e.ArrowRight="ArrowRight",e.ArrowDown="ArrowDown",e.Home="Home",e.End="End",e.PageUp="PageUp",e.PageDown="PageDown",e.Tab="Tab",e))(Te||{});let Vt=["[contentEditable=true]","[tabindex]","a[href]","area[href]","button:not([disabled])","iframe","input:not([disabled])","select:not([disabled])","textarea:not([disabled])"].map(e=>`${e}:not([tabindex='-1'])`).join(",");var tt=(e=>(e[e.First=1]="First",e[e.Previous=2]="Previous",e[e.Next=4]="Next",e[e.Last=8]="Last",e[e.WrapAround=16]="WrapAround",e[e.NoScroll=32]="NoScroll",e))(tt||{}),Wt=(e=>(e[e.Error=0]="Error",e[e.Overflow=1]="Overflow",e[e.Success=2]="Success",e[e.Underflow=3]="Underflow",e))(Wt||{}),li=(e=>(e[e.Previous=-1]="Previous",e[e.Next=1]="Next",e))(li||{});function Or(e=document.body){return e==null?[]:Array.from(e.querySelectorAll(Vt)).sort((t,r)=>Math.sign((t.tabIndex||Number.MAX_SAFE_INTEGER)-(r.tabIndex||Number.MAX_SAFE_INTEGER)))}var ci=(e=>(e[e.Strict=0]="Strict",e[e.Loose=1]="Loose",e))(ci||{});function di(e,t=0){var r;return e===((r=m(e))==null?void 0:r.body)?!1:M(t,{[0](){return e.matches(Vt)},[1](){let n=e;for(;n!==null;){if(n.matches(Vt))return!0;n=n.parentElement}return!1}})}function Aa(e){let t=m(e);b().nextFrame(()=>{t&&!di(t.activeElement,0)&&fi(e)})}function fi(e){e?.focus({preventScroll:!0})}let hi=["textarea","input"].join(",");function pi(e){var t,r;return(r=(t=e?.matches)==null?void 0:t.call(e,hi))!=null?r:!1}function Nr(e,t=r=>r){return e.slice().sort((r,n)=>{let i=t(r),a=t(n);if(i===null||a===null)return 0;let o=i.compareDocumentPosition(a);return o&Node.DOCUMENT_POSITION_FOLLOWING?-1:o&Node.DOCUMENT_POSITION_PRECEDING?1:0})}function ja(e,t){return $t(Or(),t,{relativeTo:e})}function $t(e,t,{sorted:r=!0,relativeTo:n=null,skipElements:i=[]}={}){let a=Array.isArray(e)?e.length>0?e[0].ownerDocument:document:e.ownerDocument,o=Array.isArray(e)?r?Nr(e):e:Or(e);i.length>0&&o.length>1&&(o=o.filter(p=>!i.includes(p))),n=n??a.activeElement;let u=(()=>{if(t&5)return 1;if(t&10)return-1;throw new Error("Missing Focus.First, Focus.Previous, Focus.Next or Focus.Last")})(),s=(()=>{if(t&1)return 0;if(t&2)return Math.max(0,o.indexOf(n))-1;if(t&4)return Math.max(0,o.indexOf(n))+1;if(t&8)return o.length-1;throw new Error("Missing Focus.First, Focus.Previous, Focus.Next or Focus.Last")})(),l=t&32?{preventScroll:!0}:{},d=0,h=o.length,c;do{if(d>=h||d+h<=0)return 0;let p=s+d;if(t&16)p=(p+h)%h;else{if(p<0)return 3;if(p>=h)return 1}c=o[p],c?.focus(l),d+=u}while(c!==a.activeElement);return t&6&&pi(c)&&c.select(),c.hasAttribute("tabindex")||c.setAttribute("tabindex","0"),2}function vi(e=0){let[t,r]=(0,f.useState)(e),n=(0,f.useCallback)(u=>r(s=>s|u),[t]),i=(0,f.useCallback)(u=>Boolean(t&u),[t]),a=(0,f.useCallback)(u=>r(s=>s&~u),[r]),o=(0,f.useCallback)(u=>r(s=>s^u),[r]);return{flags:t,addFlag:n,hasFlag:i,removeFlag:a,toggleFlag:o}}function _r(e){let t=(0,f.useRef)(e);return Ge(()=>{t.current=e},[e]),t}let ve=function(e){let t=_r(e);return f.useCallback((...r)=>t.current(...r),[t])},Tr=Symbol();function Da(e,t=!0){return Object.assign(e,{[Tr]:t})}function Et(...e){let t=(0,f.useRef)(e);(0,f.useEffect)(()=>{t.current=e},[e]);let r=ve(n=>{for(let i of t.current)i!=null&&(typeof i=="function"?i(n):i.current=n)});return e.every(n=>n==null||n?.[Tr])?void 0:r}let Ar=(0,f.createContext)(null);function jr(){let e=(0,f.useContext)(Ar);if(e===null){let t=new Error("You used a <Label /> component, but it is not inside a relevant parent.");throw Error.captureStackTrace&&Error.captureStackTrace(t,jr),t}return e}function Dr(){let[e,t]=(0,f.useState)([]);return[e.length>0?e.join(" "):void 0,(0,f.useMemo)(()=>function(r){let n=ve(a=>(t(o=>[...o,a]),()=>t(o=>{let u=o.slice(),s=u.indexOf(a);return s!==-1&&u.splice(s,1),u}))),i=(0,f.useMemo)(()=>({register:n,slot:r.slot,name:r.name,props:r.props}),[n,r.slot,r.name,r.props]);return f.createElement(Ar.Provider,{value:i},r.children)},[t])]}let mi="label",gi=et(function(e,t){let r=St(),{id:n=`headlessui-label-${r}`,passive:i=!1,...a}=e,o=jr(),u=Et(t);Ge(()=>o.register(n),[n,o.register]);let s={ref:u,...o.props,id:n};return i&&("onClick"in s&&delete s.onClick,"onClick"in a&&delete a.onClick),Ze({ourProps:s,theirProps:a,slot:o.slot||{},defaultTag:mi,name:o.name||"Label"})}),Pr=(0,f.createContext)(null);function Cr(){let e=(0,f.useContext)(Pr);if(e===null){let t=new Error("You used a <Description /> component, but it is not inside a relevant parent.");throw Error.captureStackTrace&&Error.captureStackTrace(t,Cr),t}return e}function Ir(){let[e,t]=(0,f.useState)([]);return[e.length>0?e.join(" "):void 0,(0,f.useMemo)(()=>function(r){let n=ve(a=>(t(o=>[...o,a]),()=>t(o=>{let u=o.slice(),s=u.indexOf(a);return s!==-1&&u.splice(s,1),u}))),i=(0,f.useMemo)(()=>({register:n,slot:r.slot,name:r.name,props:r.props}),[n,r.slot,r.name,r.props]);return f.createElement(Pr.Provider,{value:i},r.children)},[t])]}let yi="p",wi=et(function(e,t){let r=St(),{id:n=`headlessui-description-${r}`,...i}=e,a=Cr(),o=Et(t);Ge(()=>a.register(n),[n,a.register]);let u={ref:o,...a.props,id:n};return Ze({ourProps:u,theirProps:i,slot:a.slot||{},defaultTag:yi,name:a.name||"Description"})});function Rr(e){return _e.isServer?null:e instanceof Node?e.ownerDocument:e!=null&&e.hasOwnProperty("current")&&e.current instanceof Node?e.current.ownerDocument:document}function bi({container:e,accept:t,walk:r,enabled:n=!0}){let i=(0,f.useRef)(t),a=(0,f.useRef)(r);(0,f.useEffect)(()=>{i.current=t,a.current=r},[t,r]),Ge(()=>{if(!e||!n)return;let o=Rr(e);if(!o)return;let u=i.current,s=a.current,l=Object.assign(h=>u(h),{acceptNode:u}),d=o.createTreeWalker(e,NodeFilter.SHOW_ELEMENT,l,!1);for(;d.nextNode();)s(d.currentNode)},[e,n,i,a])}let xi="div";var Lr=(e=>(e[e.None=1]="None",e[e.Focusable=2]="Focusable",e[e.Hidden=4]="Hidden",e))(Lr||{});let Si=et(function(e,t){let{features:r=1,...n}=e,i={ref:t,"aria-hidden":(r&2)===2?!0:void 0,style:{position:"fixed",top:1,left:1,width:1,height:0,padding:0,margin:-1,overflow:"hidden",clip:"rect(0, 0, 0, 0)",whiteSpace:"nowrap",borderWidth:"0",...(r&4)===4&&(r&2)!==2&&{display:"none"}}};return Ze({ourProps:i,theirProps:n,slot:{},defaultTag:xi,name:"Hidden"})});function Fr(e={},t=null,r=[]){for(let[n,i]of Object.entries(e))zr(r,Mr(t,n),i);return r}function Mr(e,t){return e?e+"["+t+"]":t}function zr(e,t,r){if(Array.isArray(r))for(let[n,i]of r.entries())zr(e,Mr(t,n.toString()),i);else r instanceof Date?e.push([t,r.toISOString()]):typeof r=="boolean"?e.push([t,r?"1":"0"]):typeof r=="string"?e.push([t,r]):typeof r=="number"?e.push([t,`${r}`]):r==null?e.push([t,""]):Fr(r,t,e)}function Ei(e){var t;let r=(t=e?.form)!=null?t:e.closest("form");if(r){for(let n of r.elements)if(n.tagName==="INPUT"&&n.type==="submit"||n.tagName==="BUTTON"&&n.type==="submit"||n.nodeName==="INPUT"&&n.type==="image"){n.click();return}}}function ki(e,t,r){let[n,i]=(0,f.useState)(r),a=e!==void 0,o=(0,f.useRef)(a),u=(0,f.useRef)(!1),s=(0,f.useRef)(!1);return a&&!o.current&&!u.current?(u.current=!0,o.current=a,console.error("A component is changing from uncontrolled to controlled. This may be caused by the value changing from undefined to a defined value, which should not happen.")):!a&&o.current&&!s.current&&(s.current=!0,o.current=a,console.error("A component is changing from controlled to uncontrolled. This may be caused by the value changing from a defined value to undefined, which should not happen.")),[a?e:n,ve(l=>(a||i(l),t?.(l)))]}function Ur(e){let t=e.parentElement,r=null;for(;t&&!(t instanceof HTMLFieldSetElement);)t instanceof HTMLLegendElement&&(r=t),t=t.parentElement;let n=t?.getAttribute("disabled")==="";return n&&Oi(r)?!1:n}function Oi(e){if(!e)return!1;let t=e.previousElementSibling;for(;t!==null;){if(t instanceof HTMLLegendElement)return!1;t=t.previousElementSibling}return!0}function Ni(e){typeof queueMicrotask=="function"?queueMicrotask(e):Promise.resolve().then(e).catch(t=>setTimeout(()=>{throw t}))}function _i(){let e=[],t=[],r={enqueue(n){t.push(n)},addEventListener(n,i,a,o){return n.addEventListener(i,a,o),r.add(()=>n.removeEventListener(i,a,o))},requestAnimationFrame(...n){let i=requestAnimationFrame(...n);return r.add(()=>cancelAnimationFrame(i))},nextFrame(...n){return r.requestAnimationFrame(()=>r.requestAnimationFrame(...n))},setTimeout(...n){let i=setTimeout(...n);return r.add(()=>clearTimeout(i))},microTask(...n){let i={current:!0};return Ni(()=>{i.current&&n[0]()}),r.add(()=>{i.current=!1})},add(n){return e.push(n),()=>{let i=e.indexOf(n);if(i>=0){let[a]=e.splice(i,1);a()}}},dispose(){for(let n of e.splice(0))n()},async workQueue(){for(let n of t.splice(0))await n()},style(n,i,a){let o=n.style.getPropertyValue(i);return Object.assign(n.style,{[i]:a}),this.add(()=>{Object.assign(n.style,{[i]:o})})}};return r}function Ti(){let[e]=(0,f.useState)(_i);return(0,f.useEffect)(()=>()=>e.dispose(),[e]),e}var Ai=(e=>(e[e.RegisterOption=0]="RegisterOption",e[e.UnregisterOption=1]="UnregisterOption",e))(Ai||{});let ji={[0](e,t){let r=[...e.options,{id:t.id,element:t.element,propsRef:t.propsRef}];return{...e,options:Nr(r,n=>n.element.current)}},[1](e,t){let r=e.options.slice(),n=e.options.findIndex(i=>i.id===t.id);return n===-1?e:(r.splice(n,1),{...e,options:r})}},Jt=(0,f.createContext)(null);Jt.displayName="RadioGroupDataContext";function Gr(e){let t=(0,f.useContext)(Jt);if(t===null){let r=new Error(`<${e} /> is missing a parent <RadioGroup /> component.`);throw Error.captureStackTrace&&Error.captureStackTrace(r,Gr),r}return t}let Ht=(0,f.createContext)(null);Ht.displayName="RadioGroupActionsContext";function Br(e){let t=(0,f.useContext)(Ht);if(t===null){let r=new Error(`<${e} /> is missing a parent <RadioGroup /> component.`);throw Error.captureStackTrace&&Error.captureStackTrace(r,Br),r}return t}function Di(e,t){return zt(t.type,ji,e,t)}let Pi="div",Ci=et(function(e,t){let r=St(),{id:n=`headlessui-radiogroup-${r}`,value:i,defaultValue:a,name:o,onChange:u,by:s=(j,V)=>j===V,disabled:l=!1,...d}=e,h=ve(typeof s=="string"?(j,V)=>{let ne=s;return j?.[ne]===V?.[ne]}:s),[c,p]=(0,f.useReducer)(Di,{options:[]}),g=c.options,[y,k]=Dr(),[x,w]=Ir(),S=(0,f.useRef)(null),_=Et(S,t),[P,F]=ki(i,u,a),I=(0,f.useMemo)(()=>g.find(j=>!j.propsRef.current.disabled),[g]),C=(0,f.useMemo)(()=>g.some(j=>h(j.propsRef.current.value,P)),[g,P]),N=ve(j=>{var V;if(l||h(j,P))return!1;let ne=(V=g.find(q=>h(q.propsRef.current.value,j)))==null?void 0:V.propsRef.current;return ne!=null&&ne.disabled?!1:(F?.(j),!0)});bi({container:S.current,accept(j){return j.getAttribute("role")==="radio"?NodeFilter.FILTER_REJECT:j.hasAttribute("role")?NodeFilter.FILTER_SKIP:NodeFilter.FILTER_ACCEPT},walk(j){j.setAttribute("role","none")}});let $=ve(j=>{let V=S.current;if(!V)return;let ne=Rr(V),q=g.filter(ie=>ie.propsRef.current.disabled===!1).map(ie=>ie.element.current);switch(j.key){case Te.Enter:Ei(j.currentTarget);break;case Te.ArrowLeft:case Te.ArrowUp:if(j.preventDefault(),j.stopPropagation(),$t(q,tt.Previous|tt.WrapAround)===Wt.Success){let ie=g.find(je=>je.element.current===ne?.activeElement);ie&&N(ie.propsRef.current.value)}break;case Te.ArrowRight:case Te.ArrowDown:if(j.preventDefault(),j.stopPropagation(),$t(q,tt.Next|tt.WrapAround)===Wt.Success){let ie=g.find(je=>je.element.current===ne?.activeElement);ie&&N(ie.propsRef.current.value)}break;case Te.Space:{j.preventDefault(),j.stopPropagation();let ie=g.find(je=>je.element.current===ne?.activeElement);ie&&N(ie.propsRef.current.value)}break}}),T=ve(j=>(p({type:0,...j}),()=>p({type:1,id:j.id}))),z=(0,f.useMemo)(()=>({value:P,firstOption:I,containsCheckedOption:C,disabled:l,compare:h,...c}),[P,I,C,l,h,c]),K=(0,f.useMemo)(()=>({registerOption:T,change:N}),[T,N]),oe={ref:_,id:n,role:"radiogroup","aria-labelledby":y,"aria-describedby":x,onKeyDown:$},Ae=(0,f.useMemo)(()=>({value:P}),[P]),Oe=(0,f.useRef)(null),be=Ti();return(0,f.useEffect)(()=>{!Oe.current||a!==void 0&&be.addEventListener(Oe.current,"reset",()=>{N(a)})},[Oe,N]),f.createElement(w,{name:"RadioGroup.Description"},f.createElement(k,{name:"RadioGroup.Label"},f.createElement(Ht.Provider,{value:K},f.createElement(Jt.Provider,{value:z},o!=null&&P!=null&&Fr({[o]:P}).map(([j,V],ne)=>f.createElement(Si,{features:Lr.Hidden,ref:ne===0?q=>{var ie;Oe.current=(ie=q?.closest("form"))!=null?ie:null}:void 0,...Ut({key:j,as:"input",type:"radio",checked:V!=null,hidden:!0,readOnly:!0,name:j,value:V})})),Ze({ourProps:oe,theirProps:d,slot:Ae,defaultTag:Pi,name:"RadioGroup"})))))});var Ii=(e=>(e[e.Empty=1]="Empty",e[e.Active=2]="Active",e))(Ii||{});let Ri="div",Li=et(function(e,t){var r;let n=St(),{id:i=`headlessui-radiogroup-option-${n}`,value:a,disabled:o=!1,...u}=e,s=(0,f.useRef)(null),l=Et(s,t),[d,h]=Dr(),[c,p]=Ir(),{addFlag:g,removeFlag:y,hasFlag:k}=vi(1),x=_r({value:a,disabled:o}),w=Gr("RadioGroup.Option"),S=Br("RadioGroup.Option");Ge(()=>S.registerOption({id:i,element:s,propsRef:x}),[i,S,s,e]);let _=ve(z=>{var K;if(Ur(z.currentTarget))return z.preventDefault();!S.change(a)||(g(2),(K=s.current)==null||K.focus())}),P=ve(z=>{if(Ur(z.currentTarget))return z.preventDefault();g(2)}),F=ve(()=>y(2)),I=((r=w.firstOption)==null?void 0:r.id)===i,C=w.disabled||o,N=w.compare(w.value,a),$={ref:l,id:i,role:"radio","aria-checked":N?"true":"false","aria-labelledby":d,"aria-describedby":c,"aria-disabled":C?!0:void 0,tabIndex:(()=>C?-1:N||!w.containsCheckedOption&&I?0:-1)(),onClick:C?void 0:_,onFocus:C?void 0:P,onBlur:C?void 0:F},T=(0,f.useMemo)(()=>({checked:N,disabled:C,active:k(2)}),[N,C,k]);return f.createElement(p,{name:"RadioGroup.Description"},f.createElement(h,{name:"RadioGroup.Label"},Ze({ourProps:$,theirProps:u,slot:T,defaultTag:Ri,name:"RadioGroup.Option"})))}),rt=Object.assign(Ci,{Option:Li,Label:gi,Description:wi});var Fi=Object.defineProperty,Vr=Object.getOwnPropertySymbols,Mi=Object.prototype.hasOwnProperty,zi=Object.prototype.propertyIsEnumerable,Wr=(e,t,r)=>t in e?Fi(e,t,{enumerable:!0,configurable:!0,writable:!0,value:r}):e[t]=r,Ui=(e,t)=>{for(var r in t||(t={}))Mi.call(t,r)&&Wr(e,r,t[r]);if(Vr)for(var r of Vr(t))zi.call(t,r)&&Wr(e,r,t[r]);return e},Kt=(e,t,r)=>new Promise((n,i)=>{var a=s=>{try{u(r.next(s))}catch(l){i(l)}},o=s=>{try{u(r.throw(s))}catch(l){i(l)}},u=s=>s.done?n(s.value):Promise.resolve(s.value).then(a,o);u((r=r.apply(e,t)).next())});function Gi(e){var t,r;return[e.matches("enabled")?!0:e.matches("disabled")?!1:void 0,(r=(t=e.context.featureDesc)==null?void 0:t.force)!=null?r:!1]}var Bi=mr({id:"feature",initial:"initial",context:{},predictableActionArguments:!0,on:{ENABLE:[{target:"asyncEnabled",cond:e=>{var t;return((t=e.featureDesc)==null?void 0:t.onChangeDefault)!=null}},{target:"enabled"}],TOGGLE:[{target:"asyncEnabled",cond:e=>{var t;return((t=e.featureDesc)==null?void 0:t.onChangeDefault)!=null}},{target:"enabled"}],DISABLE:[{target:"asyncDisabled",cond:e=>{var t;return((t=e.featureDesc)==null?void 0:t.onChangeDefault)!=null}},{target:"disabled"}],UNSET:[{target:"asyncUnspecied",cond:e=>{var t;return((t=e.featureDesc)==null?void 0:t.onChangeDefault)!=null}},{target:"unspecified"}],SET:[{target:"asyncEnabled",cond:(e,t)=>{var r;return t.value===!0&&((r=e.featureDesc)==null?void 0:r.onChangeDefault)!=null}},{target:"asyncDisabled",cond:(e,t)=>{var r;return t.value===!1&&((r=e.featureDesc)==null?void 0:r.onChangeDefault)!=null}},{target:"asyncUnspecied",cond:(e,t)=>{var r;return((r=e.featureDesc)==null?void 0:r.onChangeDefault)!=null}},{target:"enabled",cond:(e,t)=>t.value===!0},{target:"disabled",cond:(e,t)=>t.value===!1},{target:"unspecified"}]},states:{initial:{on:{INIT:[{actions:Ue({featureDesc:(e,t)=>t.feature}),target:"enabled",cond:(e,t)=>t.feature.defaultValue===!0},{actions:Ue({featureDesc:(e,t)=>t.feature}),target:"unspecified",cond:(e,t)=>t.feature.defaultValue===void 0},{actions:Ue({featureDesc:(e,t)=>t.feature}),target:"disabled",cond:(e,t)=>t.feature.defaultValue===!1}]}},unspecified:{},disabled:{},enabled:{},asyncDisabled:{invoke:{id:"set-off-upstream",src:e=>Kt(void 0,null,function*(){var t;let r=(t=e.featureDesc)==null?void 0:t.onChangeDefault;if(r!=null&&e.featureDesc!=null)return r(e.featureDesc.name,!1)}),onDone:[{target:"enabled",cond:(e,t)=>t.data===!0},{target:"disabled",cond:(e,t)=>t.data===!1},{target:"unspecified"}],onError:"unspecified"}},asyncUnspecied:{invoke:{id:"set-unset-upstream",src:e=>Kt(void 0,null,function*(){var t;let r=(t=e.featureDesc)==null?void 0:t.onChangeDefault;if(r!=null&&e.featureDesc!=null)return r(e.featureDesc.name,void 0)}),onDone:[{target:"enabled",cond:(e,t)=>t.data===!0},{target:"disabled",cond:(e,t)=>t.data===!1},{target:"unspecified"}],onError:"unspecified"}},asyncEnabled:{invoke:{id:"set-on-upstream",src:e=>Kt(void 0,null,function*(){var t;let r=(t=e.featureDesc)==null?void 0:t.onChangeDefault;if(r!=null&&e.featureDesc!=null)return r(e.featureDesc.name,!0)}),onDone:[{target:"enabled",cond:(e,t)=>t.data===!0},{target:"disabled",cond:(e,t)=>t.data===!1},{target:"unspecified"}],onError:"unspecified"}}}});function kt(e,t){if(e.context.features[t]==null)return[void 0,!1];let r=e.context.features[t].getSnapshot();return r!=null?Gi(r):[void 0,!1]}var $r=mr({id:"features",initial:"idle",predictableActionArguments:!0,context:{features:{}},states:{idle:{on:{INIT:{target:"ready",cond:(e,t)=>t.features.length>0,actions:Ue({features:(e,t)=>{let r={};for(let n of t.features)r[n.name]=Wn(Bi,{name:n.name,sync:!0}),r[n.name].send({type:"INIT",feature:n});return r}})}}},ready:{on:{DE_INIT:{target:"idle",actions:Ue({features:(e,t)=>({})})},SET_ALL:{actions:Ue({features:(e,t)=>{let r=Ui({},e.features);return Object.keys(r).forEach(n=>{var i;r[n].send({type:"SET",value:(i=t.features[n])!=null?i:void 0})}),r}})},SET:{actions:(e,t)=>{let r=e.features[t.name];r?.send({type:"SET",value:t.value})}},TOGGLE:{actions:(e,t)=>{let r=e.features[t.name];r?.send({type:"TOGGLE"})}},ENABLE:{actions:(e,t)=>{let r=e.features[t.name];r?.send({type:"ENABLE"})}},DISABLE:{actions:(e,t)=>{let r=e.features[t.name];r?.send({type:"DISABLE"})}},UNSET:{actions:(e,t)=>{let r=e.features[t.name];r?.send({type:"UNSET"})}}}}}}),Jr=(0,f.createContext)(e=>!1),Xt=(0,f.createContext)(null),Vi=class{constructor(e,t,r){this.featureDesc=r,this.dispatch=e,this.testFeature=t}toggle(e){this.dispatch({type:"TOGGLE",name:e})}enable(e){this.dispatch({type:"ENABLE",name:e})}unset(e){this.dispatch({type:"UNSET",name:e})}disable(e){this.dispatch({type:"DISABLE",name:e})}setAll(e){this.dispatch({type:"SET_ALL",features:e})}listFeatures(){return this.featureDesc.map(e=>[e.name,this.testFeature(e.name)])}};function Wi(e,t,r,n){(0,f.useEffect)(()=>e?(window.feature=new Vi(n,r,t),()=>{window.feature!=null&&delete window.feature}):()=>{},[t,n,e,r])}var Hr="react-enable:feature-values";function $i(e,t,r){let n=(0,f.useMemo)(()=>{let a={};if(r.matches("ready"))for(let o of t){let[u]=kt(r,o.name);u!=null&&(a[o.name]=u)}return a},[t,r]),i=Object.keys(n).length===0||e==null?"{}":JSON.stringify({overrides:n});(0,f.useEffect)(()=>{try{e!=null&&r.matches("ready")&&e.setItem(Hr,i)}catch{}},[r,e,i])}function Ji(e,t){let r=t.map(n=>kt(n,e));for(let[n,i]of r)if(n!=null&&i)return n;for(let[n]of r)if(n!=null)return n}function Hi(e,t){return(0,f.useCallback)(r=>Ji(r,[e,t]),[e,t])}function Ki({children:e,features:t,disableConsole:r=!1,storage:n=window.sessionStorage}){let i=(0,f.useRef)(t),[a,o]=Sr($r),[u,s]=Sr($r);(0,f.useEffect)(()=>(s({type:"INIT",features:t}),()=>{s({type:"DE_INIT"})}),[s,t]),(0,f.useEffect)(()=>{let h={};if(n!=null)try{let c=n.getItem(Hr);c!=null&&(h=JSON.parse(c).overrides)}catch(c){console.error("error in localStorage",c)}return o({type:"INIT",features:i.current.filter(c=>c.noOverride!==!0).map(c=>{var p;return{name:c.name,description:c.description,defaultValue:(p=h?.[c.name])!=null?p:void 0}})}),()=>{o({type:"DE_INIT"})}},[i,o,n]),$i(n,i.current,a);let l=Hi(a,u);Wi(!r,i.current,l,s);let d=(0,f.useMemo)(()=>({overridesSend:o,defaultsSend:s,featuresDescription:i.current,overridesState:a,defaultsState:u,test:l}),[o,s,a,u,l]);return f.createElement(Xt.Provider,{value:d},f.createElement(Jr.Provider,{value:l},e))}function Ot(e){let t=(0,f.useContext)(Jr),r=(0,f.useMemo)(()=>e==null?[]:Array.isArray(e)?e:[e],[e]);return[t,r]}function Xi(e){let[t,r]=Ot(e);return r.length>0&&r.every(t)}function Kr(e){let[t,r]=Ot(e);return r.some(t)}function Pa({feature:e=[],allFeatures:t=[],children:r}){let n=Kr(e),i=Xi(t);return n||i?D.createElement(D.Fragment,null,r):null}function Yi(e){let[t,r]=Ot(e);return e.length>0&&r.every(n=>{var i;return!((i=t(n))!=null&&i)})}function Qi(e){let[t,r]=Ot(e);return r.some(n=>{var i;return!((i=t(n))!=null&&i)})}var Ca=({feature:e=[],allFeatures:t=[],children:r})=>{let n=Qi(e),i=Yi(t);return n||i?E.createElement(E.Fragment,null,r):null},qi=`/*
! tailwindcss v3.0.24 | MIT License | https://tailwindcss.com
*/

/*
1. Prevent padding and border from affecting element width. (https://github.com/mozdevs/cssremedy/issues/4)
2. Allow adding a border to an element by just adding a border-width. (https://github.com/tailwindcss/tailwindcss/pull/116)
*/

*,
::before,
::after {
  box-sizing: border-box;
  /* 1 */
  border-width: 0;
  /* 2 */
  border-style: solid;
  /* 2 */
  border-color: #e5e7eb;
  /* 2 */
}

::before,
::after {
  --tw-content: '';
}

/*
1. Use a consistent sensible line-height in all browsers.
2. Prevent adjustments of font size after orientation changes in iOS.
3. Use a more readable tab size.
4. Use the user's configured \`sans\` font-family by default.
*/

html {
  line-height: 1.5;
  /* 1 */
  -webkit-text-size-adjust: 100%;
  /* 2 */
  -moz-tab-size: 4;
  /* 3 */
  -o-tab-size: 4;
     tab-size: 4;
  /* 3 */
  font-family: ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, "Noto Sans", sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji";
  /* 4 */
}

/*
1. Remove the margin in all browsers.
2. Inherit line-height from \`html\` so users can set them as a class directly on the \`html\` element.
*/

body {
  margin: 0;
  /* 1 */
  line-height: inherit;
  /* 2 */
}

/*
1. Add the correct height in Firefox.
2. Correct the inheritance of border color in Firefox. (https://bugzilla.mozilla.org/show_bug.cgi?id=190655)
3. Ensure horizontal rules are visible by default.
*/

hr {
  height: 0;
  /* 1 */
  color: inherit;
  /* 2 */
  border-top-width: 1px;
  /* 3 */
}

/*
Add the correct text decoration in Chrome, Edge, and Safari.
*/

abbr:where([title]) {
  -webkit-text-decoration: underline dotted;
          text-decoration: underline dotted;
}

/*
Remove the default font size and weight for headings.
*/

h1,
h2,
h3,
h4,
h5,
h6 {
  font-size: inherit;
  font-weight: inherit;
}

/*
Reset links to optimize for opt-in styling instead of opt-out.
*/

a {
  color: inherit;
  text-decoration: inherit;
}

/*
Add the correct font weight in Edge and Safari.
*/

b,
strong {
  font-weight: bolder;
}

/*
1. Use the user's configured \`mono\` font family by default.
2. Correct the odd \`em\` font sizing in all browsers.
*/

code,
kbd,
samp,
pre {
  font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", "Courier New", monospace;
  /* 1 */
  font-size: 1em;
  /* 2 */
}

/*
Add the correct font size in all browsers.
*/

small {
  font-size: 80%;
}

/*
Prevent \`sub\` and \`sup\` elements from affecting the line height in all browsers.
*/

sub,
sup {
  font-size: 75%;
  line-height: 0;
  position: relative;
  vertical-align: baseline;
}

sub {
  bottom: -0.25em;
}

sup {
  top: -0.5em;
}

/*
1. Remove text indentation from table contents in Chrome and Safari. (https://bugs.chromium.org/p/chromium/issues/detail?id=999088, https://bugs.webkit.org/show_bug.cgi?id=201297)
2. Correct table border color inheritance in all Chrome and Safari. (https://bugs.chromium.org/p/chromium/issues/detail?id=935729, https://bugs.webkit.org/show_bug.cgi?id=195016)
3. Remove gaps between table borders by default.
*/

table {
  text-indent: 0;
  /* 1 */
  border-color: inherit;
  /* 2 */
  border-collapse: collapse;
  /* 3 */
}

/*
1. Change the font styles in all browsers.
2. Remove the margin in Firefox and Safari.
3. Remove default padding in all browsers.
*/

button,
input,
optgroup,
select,
textarea {
  font-family: inherit;
  /* 1 */
  font-size: 100%;
  /* 1 */
  line-height: inherit;
  /* 1 */
  color: inherit;
  /* 1 */
  margin: 0;
  /* 2 */
  padding: 0;
  /* 3 */
}

/*
Remove the inheritance of text transform in Edge and Firefox.
*/

button,
select {
  text-transform: none;
}

/*
1. Correct the inability to style clickable types in iOS and Safari.
2. Remove default button styles.
*/

button,
[type='button'],
[type='reset'],
[type='submit'] {
  -webkit-appearance: button;
  /* 1 */
  background-color: transparent;
  /* 2 */
  background-image: none;
  /* 2 */
}

/*
Use the modern Firefox focus style for all focusable elements.
*/

:-moz-focusring {
  outline: auto;
}

/*
Remove the additional \`:invalid\` styles in Firefox. (https://github.com/mozilla/gecko-dev/blob/2f9eacd9d3d995c937b4251a5557d95d494c9be1/layout/style/res/forms.css#L728-L737)
*/

:-moz-ui-invalid {
  box-shadow: none;
}

/*
Add the correct vertical alignment in Chrome and Firefox.
*/

progress {
  vertical-align: baseline;
}

/*
Correct the cursor style of increment and decrement buttons in Safari.
*/

::-webkit-inner-spin-button,
::-webkit-outer-spin-button {
  height: auto;
}

/*
1. Correct the odd appearance in Chrome and Safari.
2. Correct the outline style in Safari.
*/

[type='search'] {
  -webkit-appearance: textfield;
  /* 1 */
  outline-offset: -2px;
  /* 2 */
}

/*
Remove the inner padding in Chrome and Safari on macOS.
*/

::-webkit-search-decoration {
  -webkit-appearance: none;
}

/*
1. Correct the inability to style clickable types in iOS and Safari.
2. Change font properties to \`inherit\` in Safari.
*/

::-webkit-file-upload-button {
  -webkit-appearance: button;
  /* 1 */
  font: inherit;
  /* 2 */
}

/*
Add the correct display in Chrome and Safari.
*/

summary {
  display: list-item;
}

/*
Removes the default spacing and border for appropriate elements.
*/

blockquote,
dl,
dd,
h1,
h2,
h3,
h4,
h5,
h6,
hr,
figure,
p,
pre {
  margin: 0;
}

fieldset {
  margin: 0;
  padding: 0;
}

legend {
  padding: 0;
}

ol,
ul,
menu {
  list-style: none;
  margin: 0;
  padding: 0;
}

/*
Prevent resizing textareas horizontally by default.
*/

textarea {
  resize: vertical;
}

/*
1. Reset the default placeholder opacity in Firefox. (https://github.com/tailwindlabs/tailwindcss/issues/3300)
2. Set the default placeholder color to the user's configured gray 400 color.
*/

input::-moz-placeholder, textarea::-moz-placeholder {
  opacity: 1;
  /* 1 */
  color: #9ca3af;
  /* 2 */
}

input:-ms-input-placeholder, textarea:-ms-input-placeholder {
  opacity: 1;
  /* 1 */
  color: #9ca3af;
  /* 2 */
}

input::placeholder,
textarea::placeholder {
  opacity: 1;
  /* 1 */
  color: #9ca3af;
  /* 2 */
}

/*
Set the default cursor for buttons.
*/

button,
[role="button"] {
  cursor: pointer;
}

/*
Make sure disabled buttons don't get the pointer cursor.
*/

:disabled {
  cursor: default;
}

/*
1. Make replaced elements \`display: block\` by default. (https://github.com/mozdevs/cssremedy/issues/14)
2. Add \`vertical-align: middle\` to align replaced elements more sensibly by default. (https://github.com/jensimmons/cssremedy/issues/14#issuecomment-634934210)
   This can trigger a poorly considered lint error in some tools but is included by design.
*/

img,
svg,
video,
canvas,
audio,
iframe,
embed,
object {
  display: block;
  /* 1 */
  vertical-align: middle;
  /* 2 */
}

/*
Constrain images and videos to the parent width and preserve their intrinsic aspect ratio. (https://github.com/mozdevs/cssremedy/issues/14)
*/

img,
video {
  max-width: 100%;
  height: auto;
}

/*
Ensure the default browser behavior of the \`hidden\` attribute.
*/

[hidden] {
  display: none;
}

[type='text'],[type='email'],[type='url'],[type='password'],[type='number'],[type='date'],[type='datetime-local'],[type='month'],[type='search'],[type='tel'],[type='time'],[type='week'],[multiple],textarea,select {
  -webkit-appearance: none;
     -moz-appearance: none;
          appearance: none;
  background-color: #fff;
  border-color: #6b7280;
  border-width: 1px;
  border-radius: 0px;
  padding-top: 0.5rem;
  padding-right: 0.75rem;
  padding-bottom: 0.5rem;
  padding-left: 0.75rem;
  font-size: 1rem;
  line-height: 1.5rem;
  --tw-shadow: 0 0 #0000;
}

[type='text']:focus, [type='email']:focus, [type='url']:focus, [type='password']:focus, [type='number']:focus, [type='date']:focus, [type='datetime-local']:focus, [type='month']:focus, [type='search']:focus, [type='tel']:focus, [type='time']:focus, [type='week']:focus, [multiple]:focus, textarea:focus, select:focus {
  outline: 2px solid transparent;
  outline-offset: 2px;
  --tw-ring-inset: var(--tw-empty,/*!*/ /*!*/);
  --tw-ring-offset-width: 0px;
  --tw-ring-offset-color: #fff;
  --tw-ring-color: #2563eb;
  --tw-ring-offset-shadow: var(--tw-ring-inset) 0 0 0 var(--tw-ring-offset-width) var(--tw-ring-offset-color);
  --tw-ring-shadow: var(--tw-ring-inset) 0 0 0 calc(1px + var(--tw-ring-offset-width)) var(--tw-ring-color);
  box-shadow: var(--tw-ring-offset-shadow), var(--tw-ring-shadow), var(--tw-shadow);
  border-color: #2563eb;
}

input::-moz-placeholder, textarea::-moz-placeholder {
  color: #6b7280;
  opacity: 1;
}

input:-ms-input-placeholder, textarea:-ms-input-placeholder {
  color: #6b7280;
  opacity: 1;
}

input::placeholder,textarea::placeholder {
  color: #6b7280;
  opacity: 1;
}

::-webkit-datetime-edit-fields-wrapper {
  padding: 0;
}

::-webkit-date-and-time-value {
  min-height: 1.5em;
}

::-webkit-datetime-edit,::-webkit-datetime-edit-year-field,::-webkit-datetime-edit-month-field,::-webkit-datetime-edit-day-field,::-webkit-datetime-edit-hour-field,::-webkit-datetime-edit-minute-field,::-webkit-datetime-edit-second-field,::-webkit-datetime-edit-millisecond-field,::-webkit-datetime-edit-meridiem-field {
  padding-top: 0;
  padding-bottom: 0;
}

select {
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3e%3c/svg%3e");
  background-position: right 0.5rem center;
  background-repeat: no-repeat;
  background-size: 1.5em 1.5em;
  padding-right: 2.5rem;
  -webkit-print-color-adjust: exact;
          color-adjust: exact;
}

[multiple] {
  background-image: initial;
  background-position: initial;
  background-repeat: unset;
  background-size: initial;
  padding-right: 0.75rem;
  -webkit-print-color-adjust: unset;
          color-adjust: unset;
}

[type='checkbox'],[type='radio'] {
  -webkit-appearance: none;
     -moz-appearance: none;
          appearance: none;
  padding: 0;
  -webkit-print-color-adjust: exact;
          color-adjust: exact;
  display: inline-block;
  vertical-align: middle;
  background-origin: border-box;
  -webkit-user-select: none;
     -moz-user-select: none;
      -ms-user-select: none;
          user-select: none;
  flex-shrink: 0;
  height: 1rem;
  width: 1rem;
  color: #2563eb;
  background-color: #fff;
  border-color: #6b7280;
  border-width: 1px;
  --tw-shadow: 0 0 #0000;
}

[type='checkbox'] {
  border-radius: 0px;
}

[type='radio'] {
  border-radius: 100%;
}

[type='checkbox']:focus,[type='radio']:focus {
  outline: 2px solid transparent;
  outline-offset: 2px;
  --tw-ring-inset: var(--tw-empty,/*!*/ /*!*/);
  --tw-ring-offset-width: 2px;
  --tw-ring-offset-color: #fff;
  --tw-ring-color: #2563eb;
  --tw-ring-offset-shadow: var(--tw-ring-inset) 0 0 0 var(--tw-ring-offset-width) var(--tw-ring-offset-color);
  --tw-ring-shadow: var(--tw-ring-inset) 0 0 0 calc(2px + var(--tw-ring-offset-width)) var(--tw-ring-color);
  box-shadow: var(--tw-ring-offset-shadow), var(--tw-ring-shadow), var(--tw-shadow);
}

[type='checkbox']:checked,[type='radio']:checked {
  border-color: transparent;
  background-color: currentColor;
  background-size: 100% 100%;
  background-position: center;
  background-repeat: no-repeat;
}

[type='checkbox']:checked {
  background-image: url("data:image/svg+xml,%3csvg viewBox='0 0 16 16' fill='white' xmlns='http://www.w3.org/2000/svg'%3e%3cpath d='M12.207 4.793a1 1 0 010 1.414l-5 5a1 1 0 01-1.414 0l-2-2a1 1 0 011.414-1.414L6.5 9.086l4.293-4.293a1 1 0 011.414 0z'/%3e%3c/svg%3e");
}

[type='radio']:checked {
  background-image: url("data:image/svg+xml,%3csvg viewBox='0 0 16 16' fill='white' xmlns='http://www.w3.org/2000/svg'%3e%3ccircle cx='8' cy='8' r='3'/%3e%3c/svg%3e");
}

[type='checkbox']:checked:hover,[type='checkbox']:checked:focus,[type='radio']:checked:hover,[type='radio']:checked:focus {
  border-color: transparent;
  background-color: currentColor;
}

[type='checkbox']:indeterminate {
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 16 16'%3e%3cpath stroke='white' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M4 8h8'/%3e%3c/svg%3e");
  border-color: transparent;
  background-color: currentColor;
  background-size: 100% 100%;
  background-position: center;
  background-repeat: no-repeat;
}

[type='checkbox']:indeterminate:hover,[type='checkbox']:indeterminate:focus {
  border-color: transparent;
  background-color: currentColor;
}

[type='file'] {
  background: unset;
  border-color: inherit;
  border-width: 0;
  border-radius: 0;
  padding: 0;
  font-size: unset;
  line-height: inherit;
}

[type='file']:focus {
  outline: 1px auto -webkit-focus-ring-color;
}

*, ::before, ::after {
  --tw-translate-x: 0;
  --tw-translate-y: 0;
  --tw-rotate: 0;
  --tw-skew-x: 0;
  --tw-skew-y: 0;
  --tw-scale-x: 1;
  --tw-scale-y: 1;
  --tw-pan-x:  ;
  --tw-pan-y:  ;
  --tw-pinch-zoom:  ;
  --tw-scroll-snap-strictness: proximity;
  --tw-ordinal:  ;
  --tw-slashed-zero:  ;
  --tw-numeric-figure:  ;
  --tw-numeric-spacing:  ;
  --tw-numeric-fraction:  ;
  --tw-ring-inset:  ;
  --tw-ring-offset-width: 0px;
  --tw-ring-offset-color: #fff;
  --tw-ring-color: rgb(59 130 246 / 0.5);
  --tw-ring-offset-shadow: 0 0 #0000;
  --tw-ring-shadow: 0 0 #0000;
  --tw-shadow: 0 0 #0000;
  --tw-shadow-colored: 0 0 #0000;
  --tw-blur:  ;
  --tw-brightness:  ;
  --tw-contrast:  ;
  --tw-grayscale:  ;
  --tw-hue-rotate:  ;
  --tw-invert:  ;
  --tw-saturate:  ;
  --tw-sepia:  ;
  --tw-drop-shadow:  ;
  --tw-backdrop-blur:  ;
  --tw-backdrop-brightness:  ;
  --tw-backdrop-contrast:  ;
  --tw-backdrop-grayscale:  ;
  --tw-backdrop-hue-rotate:  ;
  --tw-backdrop-invert:  ;
  --tw-backdrop-opacity:  ;
  --tw-backdrop-saturate:  ;
  --tw-backdrop-sepia:  ;
}

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border-width: 0;
}

.pointer-events-none {
  pointer-events: none;
}

.invisible {
  visibility: hidden;
}

.fixed {
  position: fixed;
}

.absolute {
  position: absolute;
}

.relative {
  position: relative;
}

.-inset-px {
  top: -1px;
  right: -1px;
  bottom: -1px;
  left: -1px;
}

.inset-0 {
  top: 0px;
  right: 0px;
  bottom: 0px;
  left: 0px;
}

.bottom-0 {
  bottom: 0px;
}

.left-0 {
  left: 0px;
}

.z-10 {
  z-index: 10;
}

.mx-4 {
  margin-left: 1rem;
  margin-right: 1rem;
}

.my-4 {
  margin-top: 1rem;
  margin-bottom: 1rem;
}

.mx-8 {
  margin-left: 2rem;
  margin-right: 2rem;
}

.mt-4 {
  margin-top: 1rem;
}

.mt-1 {
  margin-top: 0.25rem;
}

.mt-6 {
  margin-top: 1.5rem;
}

.mt-5 {
  margin-top: 1.25rem;
}

.inline-block {
  display: inline-block;
}

.flex {
  display: flex;
}

.inline-flex {
  display: inline-flex;
}

.grid {
  display: grid;
}

.h-7 {
  height: 1.75rem;
}

.h-4 {
  height: 1rem;
}

.h-5 {
  height: 1.25rem;
}

.h-8 {
  height: 2rem;
}

.h-6 {
  height: 1.5rem;
}

.min-h-screen {
  min-height: 100vh;
}

.w-4 {
  width: 1rem;
}

.w-5 {
  width: 1.25rem;
}

.w-8 {
  width: 2rem;
}

.w-6 {
  width: 1.5rem;
}

.max-w-full {
  max-width: 100%;
}

.shrink {
  flex-shrink: 1;
}

.grow {
  flex-grow: 1;
}

.transform {
  transform: translate(var(--tw-translate-x), var(--tw-translate-y)) rotate(var(--tw-rotate)) skewX(var(--tw-skew-x)) skewY(var(--tw-skew-y)) scaleX(var(--tw-scale-x)) scaleY(var(--tw-scale-y));
}

.cursor-not-allowed {
  cursor: not-allowed;
}

.cursor-pointer {
  cursor: pointer;
}

.grid-cols-1 {
  grid-template-columns: repeat(1, minmax(0, 1fr));
}

.flex-row {
  flex-direction: row;
}

.flex-col {
  flex-direction: column;
}

.flex-nowrap {
  flex-wrap: nowrap;
}

.items-end {
  align-items: flex-end;
}

.items-center {
  align-items: center;
}

.justify-center {
  justify-content: center;
}

.gap-2 {
  gap: 0.5rem;
}

.gap-1 {
  gap: 0.25rem;
}

.gap-4 {
  gap: 1rem;
}

.gap-9 {
  gap: 2.25rem;
}

.gap-y-6 {
  row-gap: 1.5rem;
}

.overflow-hidden {
  overflow: hidden;
}

.overflow-y-auto {
  overflow-y: auto;
}

.rounded-sm {
  border-radius: 0.125rem;
}

.rounded-lg {
  border-radius: 0.5rem;
}

.rounded-full {
  border-radius: 9999px;
}

.border {
  border-width: 1px;
}

.border-2 {
  border-width: 2px;
}

.border-orange-500 {
  --tw-border-opacity: 1;
  border-color: rgb(249 115 22 / var(--tw-border-opacity));
}

.border-green-500 {
  --tw-border-opacity: 1;
  border-color: rgb(34 197 94 / var(--tw-border-opacity));
}

.border-red-500 {
  --tw-border-opacity: 1;
  border-color: rgb(239 68 68 / var(--tw-border-opacity));
}

.border-transparent {
  border-color: transparent;
}

.border-gray-300 {
  --tw-border-opacity: 1;
  border-color: rgb(209 213 219 / var(--tw-border-opacity));
}

.border-blue-500 {
  --tw-border-opacity: 1;
  border-color: rgb(59 130 246 / var(--tw-border-opacity));
}

.border-gray-500 {
  --tw-border-opacity: 1;
  border-color: rgb(107 114 128 / var(--tw-border-opacity));
}

.bg-white {
  --tw-bg-opacity: 1;
  background-color: rgb(255 255 255 / var(--tw-bg-opacity));
}

.bg-blue-600 {
  --tw-bg-opacity: 1;
  background-color: rgb(37 99 235 / var(--tw-bg-opacity));
}

.p-3 {
  padding: 0.75rem;
}

.p-1 {
  padding: 0.25rem;
}

.px-2 {
  padding-left: 0.5rem;
  padding-right: 0.5rem;
}

.py-1 {
  padding-top: 0.25rem;
  padding-bottom: 0.25rem;
}

.px-4 {
  padding-left: 1rem;
  padding-right: 1rem;
}

.pt-4 {
  padding-top: 1rem;
}

.pb-10 {
  padding-bottom: 2.5rem;
}

.pt-5 {
  padding-top: 1.25rem;
}

.pb-4 {
  padding-bottom: 1rem;
}

.pt-0 {
  padding-top: 0px;
}

.pb-0 {
  padding-bottom: 0px;
}

.pr-4 {
  padding-right: 1rem;
}

.pl-4 {
  padding-left: 1rem;
}

.text-left {
  text-align: left;
}

.align-middle {
  vertical-align: middle;
}

.align-bottom {
  vertical-align: bottom;
}

.text-xs {
  font-size: 0.75rem;
  line-height: 1rem;
}

.text-base {
  font-size: 1rem;
  line-height: 1.5rem;
}

.text-sm {
  font-size: 0.875rem;
  line-height: 1.25rem;
}

.text-lg {
  font-size: 1.125rem;
  line-height: 1.75rem;
}

.font-medium {
  font-weight: 500;
}

.leading-6 {
  line-height: 1.5rem;
}

.leading-7 {
  line-height: 1.75rem;
}

.text-gray-900 {
  --tw-text-opacity: 1;
  color: rgb(17 24 39 / var(--tw-text-opacity));
}

.text-orange-500 {
  --tw-text-opacity: 1;
  color: rgb(249 115 22 / var(--tw-text-opacity));
}

.text-green-500 {
  --tw-text-opacity: 1;
  color: rgb(34 197 94 / var(--tw-text-opacity));
}

.text-gray-500 {
  --tw-text-opacity: 1;
  color: rgb(107 114 128 / var(--tw-text-opacity));
}

.text-red-500 {
  --tw-text-opacity: 1;
  color: rgb(239 68 68 / var(--tw-text-opacity));
}

.text-blue-500 {
  --tw-text-opacity: 1;
  color: rgb(59 130 246 / var(--tw-text-opacity));
}

.text-white {
  --tw-text-opacity: 1;
  color: rgb(255 255 255 / var(--tw-text-opacity));
}

.shadow-sm {
  --tw-shadow: 0 1px 2px 0 rgb(0 0 0 / 0.05);
  --tw-shadow-colored: 0 1px 2px 0 var(--tw-shadow-color);
  box-shadow: var(--tw-ring-offset-shadow, 0 0 #0000), var(--tw-ring-shadow, 0 0 #0000), var(--tw-shadow);
}

.shadow {
  --tw-shadow: 0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1);
  --tw-shadow-colored: 0 1px 3px 0 var(--tw-shadow-color), 0 1px 2px -1px var(--tw-shadow-color);
  box-shadow: var(--tw-ring-offset-shadow, 0 0 #0000), var(--tw-ring-shadow, 0 0 #0000), var(--tw-shadow);
}

.shadow-xl {
  --tw-shadow: 0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1);
  --tw-shadow-colored: 0 20px 25px -5px var(--tw-shadow-color), 0 8px 10px -6px var(--tw-shadow-color);
  box-shadow: var(--tw-ring-offset-shadow, 0 0 #0000), var(--tw-ring-shadow, 0 0 #0000), var(--tw-shadow);
}

.ring-2 {
  --tw-ring-offset-shadow: var(--tw-ring-inset) 0 0 0 var(--tw-ring-offset-width) var(--tw-ring-offset-color);
  --tw-ring-shadow: var(--tw-ring-inset) 0 0 0 calc(2px + var(--tw-ring-offset-width)) var(--tw-ring-color);
  box-shadow: var(--tw-ring-offset-shadow), var(--tw-ring-shadow), var(--tw-shadow, 0 0 #0000);
}

.ring-blue-500 {
  --tw-ring-opacity: 1;
  --tw-ring-color: rgb(59 130 246 / var(--tw-ring-opacity));
}

.ring-gray-500 {
  --tw-ring-opacity: 1;
  --tw-ring-color: rgb(107 114 128 / var(--tw-ring-opacity));
}

.invert {
  --tw-invert: invert(100%);
  filter: var(--tw-blur) var(--tw-brightness) var(--tw-contrast) var(--tw-grayscale) var(--tw-hue-rotate) var(--tw-invert) var(--tw-saturate) var(--tw-sepia) var(--tw-drop-shadow);
}

.filter {
  filter: var(--tw-blur) var(--tw-brightness) var(--tw-contrast) var(--tw-grayscale) var(--tw-hue-rotate) var(--tw-invert) var(--tw-saturate) var(--tw-sepia) var(--tw-drop-shadow);
}

.transition-all {
  transition-property: all;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 150ms;
}

.focus\\:outline-none:focus {
  outline: 2px solid transparent;
  outline-offset: 2px;
}

.focus\\:ring-2:focus {
  --tw-ring-offset-shadow: var(--tw-ring-inset) 0 0 0 var(--tw-ring-offset-width) var(--tw-ring-offset-color);
  --tw-ring-shadow: var(--tw-ring-inset) 0 0 0 calc(2px + var(--tw-ring-offset-width)) var(--tw-ring-color);
  box-shadow: var(--tw-ring-offset-shadow), var(--tw-ring-shadow), var(--tw-shadow, 0 0 #0000);
}

.focus\\:ring-blue-600:focus {
  --tw-ring-opacity: 1;
  --tw-ring-color: rgb(37 99 235 / var(--tw-ring-opacity));
}

.focus\\:ring-offset-2:focus {
  --tw-ring-offset-width: 2px;
}

@media (min-width: 640px) {
  .sm\\:my-8 {
    margin-top: 2rem;
    margin-bottom: 2rem;
  }

  .sm\\:mt-3 {
    margin-top: 0.75rem;
  }

  .sm\\:mt-6 {
    margin-top: 1.5rem;
  }

  .sm\\:block {
    display: block;
  }

  .sm\\:grid-cols-3 {
    grid-template-columns: repeat(3, minmax(0, 1fr));
  }

  .sm\\:gap-x-4 {
    -moz-column-gap: 1rem;
         column-gap: 1rem;
  }

  .sm\\:p-0 {
    padding: 0px;
  }

  .sm\\:p-6 {
    padding: 1.5rem;
  }

  .sm\\:align-middle {
    vertical-align: middle;
  }

  .sm\\:text-sm {
    font-size: 0.875rem;
    line-height: 1.25rem;
  }
}

@media (min-width: 1024px) {
  .lg\\:max-w-\\[80\\%\\] {
    max-width: 80%;
  }

  .lg\\:gap-4 {
    gap: 1rem;
  }
}
`;function Yt(...e){return e.filter(Boolean).join(" ")}function Zi({feature:e}){var t,r,n;let i=(0,f.useContext)(Xt),a=(0,f.useCallback)(c=>{if(i?.overridesSend!=null)switch(c){case"true":{i.overridesSend({type:"ENABLE",name:e.name});break}case"false":{i.overridesSend({type:"DISABLE",name:e.name});break}case"unset":{i.overridesSend({type:"UNSET",name:e.name});break}}},[e.name,i]);if(i==null)return null;let{overridesState:o,test:u,defaultsState:s}=i,l=((t=kt(s,e.name)[0])!=null?t:"unset").toString(),d=((r=kt(o,e.name)[0])!=null?r:"unset").toString(),h=u(e.name);return f.createElement(rt,{disabled:e.noOverride,onChange:a,value:d},f.createElement(rt.Label,null,f.createElement("h6",{className:"text-gray-900 align-center flex flex-row flex-nowrap items-center gap-2 lg:gap-4 h-7"},f.createElement("span",{className:"font-medium"},"Feature: ",f.createElement("code",null,e.name)),e.noOverride===!0?f.createElement("div",{className:"border-orange-500 text-orange-500 flex flex-nowrap text-xs flex-row gap-1 rounded-sm border items-center justify-center px-2 py-1"},f.createElement("svg",{"aria-hidden":"true",className:"h-4 w-4 min-w-4",fill:"currentColor",viewBox:"0 0 20 20",xmlns:"http://www.w3.org/2000/svg"},f.createElement("path",{clipRule:"evenodd",d:"M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z",fillRule:"evenodd"})),f.createElement("div",null,"No Overrides")):null,h===!0?f.createElement("div",{className:"flex flex-nowrap text-xs text-green-500 flex-row gap-1 rounded-sm border items-center justify-center border-green-500 px-2 py-1"},f.createElement("svg",{"aria-hidden":"true",className:"h-4 w-4 min-w-4",fill:"currentColor",viewBox:"0 0 20 20",xmlns:"http://www.w3.org/2000/svg"},f.createElement("path",{clipRule:"evenodd",d:"M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z",fillRule:"evenodd"})),f.createElement("div",null,h?"Enabled":"Disabled")):null),e.description==null?null:f.createElement("p",{className:"text-base text-gray-500 text-sm"},e.description)),f.createElement("div",{className:"mt-4 grid grid-cols-1 gap-y-6 sm:grid-cols-3 sm:gap-x-4"},[{id:"false",title:`Disable ${e.name}`,description:"Override the feature to be disabled"},{id:"unset",title:"Default",description:"Inherit enabled state from defaults",disabled:((n=e.noOverride)!=null?n:!1)||e.force,defaultValue:l==="true"?f.createElement("div",{className:"text-green-500 border-green-500 flex flex-nowrap text-xs flex-row gap-1 rounded-sm border items-center justify-center px-2 py-1"},f.createElement("span",null,"Enabled")):f.createElement("div",{className:"text-red-500 border-red-500 flex flex-nowrap text-xs flex-row gap-1 rounded-sm border items-center justify-center px-2 py-1"},f.createElement("span",null,"Disabled"))},{id:"true",title:`Enable ${e.name}`,description:"Override the feature to be enabled"}].map(c=>f.createElement(rt.Option,{className:({checked:p,active:g,disabled:y})=>Yt(p?"border-transparent":"border-gray-300",!y&&g?"border-blue-500 ring-2 ring-blue-500":"",y?"border-transparent ring-gray-500 cursor-not-allowed":"cursor-pointer","relative bg-white border rounded-lg shadow-sm p-3 flex focus:outline-none"),disabled:c.disabled,key:c.id,value:c.id},({checked:p,active:g,disabled:y})=>f.createElement(f.Fragment,null,f.createElement("div",{className:"flex flex-col grow"},f.createElement(rt.Label,{as:"span",className:"flex flex-nowrap flex-row gap-1 items-center space-between"},f.createElement("span",{className:"text-sm font-medium text-gray-900 grow shrink"},c.title),c.defaultValue!=null?c.defaultValue:null,f.createElement("svg",{"aria-hidden":"true",className:Yt(p?"":"invisible","h-5 w-5 text-blue-500 min-w-4"),fill:"currentColor",viewBox:"0 0 20 20",xmlns:"http://www.w3.org/2000/svg"},f.createElement("path",{clipRule:"evenodd",d:"M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z",fillRule:"evenodd"}))),f.createElement(rt.Description,{as:"span",className:"mt-1 flex items-center text-sm text-gray-500"},c.description)),f.createElement("div",{"aria-hidden":"true",className:Yt(!y&&g?"border":"border-2",p?y?"border-gray-500":"border-blue-500":"border-transparent","absolute -inset-px rounded-lg pointer-events-none")}))))))}function ea({root:e,children:t}){return ei.createPortal(t,e)}function ta({defaultOpen:e=!1}){let[t,r]=(0,f.useState)(null);return f.createElement("div",{ref:n=>{if(n==null||t!=null)return;let i=n?.attachShadow({mode:"open"}),a=document.createElement("style"),o=document.createElement("div");a.textContent=qi,i.appendChild(a),i.appendChild(o),r(o)},style:{zIndex:99999,position:"fixed",width:"0",height:"0",bottom:0}},t!=null?f.createElement(ea,{root:t},f.createElement(ra,{defaultOpen:e})):null)}function ra({defaultOpen:e=!1}){let[t,r]=(0,f.useState)(e),n=(0,f.useContext)(Xt);if(n==null)return null;let{featuresDescription:i}=n;return i.length===0?null:f.createElement("div",{className:"relative"},f.createElement("div",{className:"absolute bottom-0 left-0 mx-4 my-4"},f.createElement("button",{className:"inline-flex items-center text-sm font-medium p-1 h-8 w-8 align-middle cursor-pointer rounded-full bg-blue-600 text-white  border border-transparent justify-center text-base font-medium focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-600 sm:text-sm",onClick:()=>r(!0),title:"Toggle features",type:"button"},f.createElement("svg",{className:"w-6 h-6 min-h-6 min-w-6",fill:"currentColor",viewBox:"0 0 20 20",xmlns:"http://www.w3.org/2000/svg"},f.createElement("path",{clipRule:"evenodd",d:"M3 6a3 3 0 013-3h10a1 1 0 01.8 1.6L14.25 8l2.55 3.4A1 1 0 0116 13H6a1 1 0 00-1 1v3a1 1 0 11-2 0V6z",fillRule:"evenodd"})))),t?f.createElement("div",{className:"fixed z-10 inset-0 overflow-y-auto"},f.createElement("div",{className:"flex items-end justify-flex-start mx-8 my-4 min-h-screen pt-4 px-4 pb-10 sm:block sm:p-0"},f.createElement("div",{className:"relative inline-block align-bottom bg-white rounded-lg px-4 pt-5 pb-4 text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:p-6 lg:max-w-[80%] max-w-full"},f.createElement("div",null,f.createElement("div",{className:"mt-1 sm:mt-3"},f.createElement("h3",{className:"flex flex-row gap-4 flex-nowrap items-center space-between"},f.createElement("div",{className:"grow text-lg leading-6 font-medium text-gray-900"},"Feature Flag Overrides")),f.createElement("p",{className:"text-sm text-gray-500"},"Features can be enabled or disabled unless they are forced upstream. You can also revert to default."),f.createElement("div",{className:"mt-6"},f.createElement("fieldset",{className:"flex flex-col gap-9"},f.createElement("legend",{className:"sr-only"},"Feature Flags"),i.map(a=>f.createElement(Zi,{feature:a,key:a.name})))),f.createElement("div",{className:"flex justify-center items-center mt-5 sm:mt-6"},f.createElement("button",{className:"inline-flex items-center text-sm font-medium pt-0 pb-0 pr-4 pl-4 h-8 leading-7 align-middle cursor-pointer rounded-sm bg-blue-600 text-white border border-transparent justify-center text-base font-medium focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-600 sm:text-sm",onClick:()=>r(!1),type:"button"},"Done"))))))):null)}},73315:(xe,de,B)=>{/**
 * @license React
 * use-sync-external-store-shim.production.min.js
 *
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */var v=B(68404);function ge(W,Y){return W===Y&&(W!==0||1/W===1/Y)||W!==W&&Y!==Y}var O=typeof Object.is=="function"?Object.is:ge,A=v.useState,U=v.useEffect,R=v.useLayoutEffect,se=v.useDebugValue;function ye(W,Y){var Q=Y(),ue=A({inst:{value:Q,getSnapshot:Y}}),Z=ue[0].inst,he=ue[1];return R(function(){Z.value=Q,Z.getSnapshot=Y,fe(Z)&&he({inst:Z})},[W,Q,Y]),U(function(){return fe(Z)&&he({inst:Z}),W(function(){fe(Z)&&he({inst:Z})})},[W]),se(Q),Q}function fe(W){var Y=W.getSnapshot;W=W.value;try{var Q=Y();return!O(W,Q)}catch{return!0}}function te(W,Y){return Y()}var ae=typeof window>"u"||typeof window.document>"u"||typeof window.document.createElement>"u"?te:ye;de.useSyncExternalStore=v.useSyncExternalStore!==void 0?v.useSyncExternalStore:ae},74414:(xe,de,B)=>{/**
 * @license React
 * use-sync-external-store-shim/with-selector.production.min.js
 *
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */var v=B(68404),ge=B(87024);function O(te,ae){return te===ae&&(te!==0||1/te===1/ae)||te!==te&&ae!==ae}var A=typeof Object.is=="function"?Object.is:O,U=ge.useSyncExternalStore,R=v.useRef,se=v.useEffect,ye=v.useMemo,fe=v.useDebugValue;de.useSyncExternalStoreWithSelector=function(te,ae,W,Y,Q){var ue=R(null);if(ue.current===null){var Z={hasValue:!1,value:null};ue.current=Z}else Z=ue.current;ue=ye(function(){function nt(le){if(!De){if(De=!0,it=le,le=Y(le),Q!==void 0&&Z.hasValue){var Se=Z.value;if(Q(Se,le))return Ne=Se}return Ne=le}if(Se=Ne,A(it,le))return Se;var ot=Y(le);return Q!==void 0&&Q(Se,ot)?Se:(it=le,Ne=ot)}var De=!1,it,Ne,at=W===void 0?null:W;return[function(){return nt(ae())},at===null?void 0:function(){return nt(at())}]},[ae,W,Y,Q]);var he=U(te,ue[0],ue[1]);return se(function(){Z.hasValue=!0,Z.value=he},[he]),fe(he),he}},87024:(xe,de,B)=>{xe.exports=B(73315)},43297:(xe,de,B)=>{xe.exports=B(74414)}}]);

//# sourceMappingURL=5524.874619dfe248d1146e59.js.map
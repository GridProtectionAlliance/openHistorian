"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[1337],{50951:(Y,R,a)=>{a.r(R),a.d(R,{ServiceAccountsListPageUnconnected:()=>L,default:()=>H,getStyles:()=>w});var n=a(9892),o=a(64681),C=a.n(o),e=a(68404),S=a(36635),g=a(72648),d=a(6554),D=a(39904),F=a(14747),v=a(2594),y=a(31403),P=a(98102),V=a(69142),x=a(79396),z=a(2555),i=a(77582),c=a(81168),f=a(14377),s=a(52081),l=a(8180),I=a(66226),m=a(65135);const u=t=>`Edit service account's ${t} details`,M=(0,e.memo)(({serviceAccount:t,onRoleChange:h,roleOptions:X,onRemoveButtonClick:T,onDisable:B,onEnable:A,onAddTokenClick:W})=>{const O=`org/serviceaccounts/${t.id}`,k=(0,g.wW)(N),U=i.Vt.hasPermissionInMetadata(c.AccessControlAction.ServiceAccountsWrite,t),J=i.Vt.hasPermission(c.AccessControlAction.ActionRolesList)&&i.Vt.hasPermission(c.AccessControlAction.ActionUserRolesList);return e.createElement("tr",{key:t.id,className:(0,n.cx)({[k.disabled]:t.isDisabled})},e.createElement("td",{className:"width-4 text-center link-td"},e.createElement("a",{href:O,"aria-label":u(t.name)},e.createElement("img",{className:"filter-table__avatar",src:t.avatarUrl,alt:`Avatar for user ${t.name}`}))),e.createElement("td",{className:"link-td max-width-10"},e.createElement("a",{className:"ellipsis",href:O,title:t.name,"aria-label":u(t.name)},t.name)),e.createElement("td",{className:"link-td max-width-10"},e.createElement("a",{className:k.accountId,href:O,title:t.login,"aria-label":u(t.name)},t.login)),i.Vt.licensedAccessControlEnabled()?e.createElement("td",null,J&&e.createElement(I.R,{userId:t.id,orgId:t.orgId,basicRole:t.role,onBasicRoleChange:Q=>h(Q,t),roleOptions:X,basicRoleDisabled:!U,disabled:t.isDisabled})):e.createElement("td",null,e.createElement(m.A,{"aria-label":"Role",value:t.role,disabled:!U||t.isDisabled,onChange:Q=>h(Q,t)})),e.createElement("td",{className:"link-td max-width-10"},e.createElement("a",{className:"ellipsis",href:O,title:"Tokens","aria-label":u(t.name)},e.createElement("div",{className:(0,n.cx)(k.tokensInfo,{[k.tokensInfoSecondary]:!t.tokens})},e.createElement("span",null,e.createElement(D.J,{name:"key-skeleton-alt"})),t.tokens||"No tokens"))),e.createElement("td",null,e.createElement(s.Lh,{justify:"flex-end"},i.Vt.hasPermission(c.AccessControlAction.ServiceAccountsWrite)&&!t.tokens&&e.createElement(y.zx,{onClick:()=>W(t),disabled:t.isDisabled},"Add token"),i.Vt.hasPermissionInMetadata(c.AccessControlAction.ServiceAccountsWrite,t)&&(t.isDisabled?e.createElement(y.zx,{variant:"primary",onClick:()=>A(t)},"Enable"):e.createElement(y.zx,{variant:"secondary",onClick:()=>B(t)},"Disable")),i.Vt.hasPermissionInMetadata(c.AccessControlAction.ServiceAccountsDelete,t)&&e.createElement(l.h,{className:k.deleteButton,name:"trash-alt",size:"md",onClick:()=>T(t),"aria-label":`Delete service account ${t.name}`}))))});M.displayName="ServiceAccountListItem";const N=t=>({iconRow:n.css`
      svg {
        margin-left: ${t.spacing(.5)};
      }
    `,accountId:(0,n.cx)("ellipsis",n.css`
        color: ${t.colors.text.secondary};
      `),deleteButton:n.css`
      color: ${t.colors.text.secondary};
    `,tokensInfo:n.css`
      span {
        margin-right: ${t.spacing(1)};
      }
    `,tokensInfoSecondary:n.css`
      color: ${t.colors.text.secondary};
    `,disabled:n.css`
      td a {
        color: ${t.colors.text.secondary};
      }
    `}),$=M;var p=a(50655);function G(t){return{...t.serviceAccounts}}const b={changeQuery:p.R5,fetchACOptions:p.bX,fetchServiceAccounts:p.Xd,deleteServiceAccount:p.yN,updateServiceAccount:p.TL,changeStateFilter:p.XE,createServiceAccountToken:p.fT},j=(0,S.connect)(G,b),L=({serviceAccounts:t,isLoading:h,roleOptions:X,query:T,serviceAccountStateFilter:B,changeQuery:A,fetchACOptions:W,fetchServiceAccounts:O,deleteServiceAccount:k,updateServiceAccount:U,changeStateFilter:J,createServiceAccountToken:Q})=>{const Z=(0,g.wW)(w),[ce,q]=(0,e.useState)(!1),[re,ee]=(0,e.useState)(!1),[ie,te]=(0,e.useState)(!1),[de,ae]=(0,e.useState)(""),[E,K]=(0,e.useState)(null);(0,e.useEffect)(()=>{O({withLoadingIndicator:!0}),i.Vt.licensedAccessControlEnabled()&&W()},[W,O]);const ne=t.length===0&&B===c.ServiceAccountStateFilter.All&&!T,me=async(r,Ae)=>{const Se={...Ae,role:r};U(Se),i.Vt.licensedAccessControlEnabled()&&W()},ue=r=>{A(r)},Ee=r=>{J(r)},ge=r=>{K(r),ee(!0)},ve=async()=>{E&&k(E.id),oe()},fe=r=>{K(r),te(!0)},pe=()=>{E&&U({...E,isDisabled:!0}),se()},De=r=>{U({...r,isDisabled:!1})},ye=r=>{K(r),q(!0)},Ce=async r=>{E&&Q(E.id,r,ae)},he=()=>{q(!1),K(null),ae("")},oe=()=>{ee(!1),K(null)},se=()=>{te(!1),K(null)},le=e.createElement("a",{className:"external-link",href:"https://grafana.com/docs/grafana/latest/administration/service-accounts/",target:"_blank",rel:"noopener noreferrer"},"here."),Te=e.createElement("span",null,"Service accounts and their tokens can be used to authenticate against the Grafana API. Find out more ",le);return e.createElement(x.T,{navId:"serviceaccounts",subTitle:Te},e.createElement(x.T.Contents,null,e.createElement(x.T.OldNavOnly,null,e.createElement("div",{className:Z.pageHeader},e.createElement("h2",null,"Service accounts"),e.createElement("div",{className:Z.apiKeyInfoLabel},e.createElement(d.u,{placement:"bottom",interactive:!0,content:e.createElement(e.Fragment,null,"API keys are now service accounts with tokens. Find out more ",le)},e.createElement(D.J,{name:"question-circle"})),e.createElement("span",null,"Looking for API keys?")))),e.createElement("div",{className:"page-action-bar"},e.createElement("div",{className:"gf-form gf-form--grow"},e.createElement(F.H,{placeholder:"Search service account by name",value:T,onChange:ue,width:50})),e.createElement(v.S,{options:[{label:"All",value:c.ServiceAccountStateFilter.All},{label:"With expired tokens",value:c.ServiceAccountStateFilter.WithExpiredTokens},{label:"Disabled",value:c.ServiceAccountStateFilter.Disabled}],onChange:Ee,value:B,className:Z.filter}),!ne&&i.Vt.hasPermission(c.AccessControlAction.ServiceAccountsCreate)&&e.createElement(y.Qj,{href:"org/serviceaccounts/create",variant:"primary"},"Add service account")),h&&e.createElement(z.Z,null),!h&&ne&&e.createElement(e.Fragment,null,e.createElement(V.Z,{title:"You haven't created any service accounts yet.",buttonIcon:"key-skeleton-alt",buttonLink:"org/serviceaccounts/create",buttonTitle:"Add service account",buttonDisabled:!i.Vt.hasPermission(c.AccessControlAction.ServiceAccountsCreate),proTip:"Remember, you can provide specific permissions for API access to other applications.",proTipLink:"",proTipLinkTitle:"",proTipTarget:"_blank"})),!h&&t.length!==0&&e.createElement(e.Fragment,null,e.createElement("div",{className:(0,n.cx)(Z.table,"admin-list-table")},e.createElement("table",{className:"filter-table filter-table--hover"},e.createElement("thead",null,e.createElement("tr",null,e.createElement("th",null),e.createElement("th",null,"Account"),e.createElement("th",null,"ID"),e.createElement("th",null,"Roles"),e.createElement("th",null,"Tokens"),e.createElement("th",{style:{width:"34px"}}))),e.createElement("tbody",null,t.map(r=>e.createElement($,{serviceAccount:r,key:r.id,roleOptions:X,onRoleChange:me,onRemoveButtonClick:ge,onDisable:fe,onEnable:De,onAddTokenClick:ye})))))),E&&e.createElement(e.Fragment,null,e.createElement(P.s,{isOpen:re,body:`Are you sure you want to delete '${E.name}'${E.tokens?` and ${E.tokens} accompanying ${C()("token",E.tokens)}`:""}?`,confirmText:"Delete",title:"Delete service account",onConfirm:ve,onDismiss:oe}),e.createElement(P.s,{isOpen:ie,title:"Disable service account",body:`Are you sure you want to disable '${E.name}'?`,confirmText:"Disable service account",onConfirm:pe,onDismiss:se}),e.createElement(f.m,{isOpen:ce,token:de,serviceAccountLogin:E.login,onCreateToken:Ce,onClose:he}))))},w=t=>({table:n.css`
      margin-top: ${t.spacing(3)};
    `,filter:n.css`
      margin: 0 ${t.spacing(1)};
    `,row:n.css`
      display: flex;
      align-items: center;
      height: 100% !important;

      a {
        padding: ${t.spacing(.5)} 0 !important;
      }
    `,unitTooltip:n.css`
      display: flex;
      flex-direction: column;
    `,unitItem:n.css`
      cursor: pointer;
      padding: ${t.spacing(.5)} 0;
      margin-right: ${t.spacing(1)};
    `,disabled:n.css`
      color: ${t.colors.text.disabled};
    `,link:n.css`
      color: inherit;
      cursor: pointer;
      text-decoration: underline;
    `,pageHeader:n.css`
      display: flex;
      margin-bottom: ${t.spacing(2)};
    `,apiKeyInfoLabel:n.css`
      margin-left: ${t.spacing(1)};
      line-height: 2.2;
      flex-grow: 1;
      color: ${t.colors.text.secondary};

      span {
        padding: ${t.spacing(.5)};
      }
    `,filterDelimiter:n.css`
      flex-grow: 1;
    `}),H=j(L)},14377:(Y,R,a)=>{a.d(R,{m:()=>x});var n=a(9892),o=a(68404),C=a(44288),e=a(35645),S=a(72648),g=a(35029),d=a(24799),D=a(46967),F=a(2594),v=a(86348),y=a(31403),P=a(94599);const V=[{label:"No expiration",value:!1},{label:"Set expiration date",value:!0}],x=({isOpen:c,token:f,serviceAccountLogin:s,onCreateToken:l,onClose:I})=>{const m=new Date;m.setDate(m.getDate()+1);const u=new Date;e.v.tokenExpirationDayLimit!==void 0&&e.v.tokenExpirationDayLimit>-1?u.setDate(u.getDate()+e.v.tokenExpirationDayLimit+1):u.setDate(864e13);const M=e.v.tokenExpirationDayLimit!==void 0&&e.v.tokenExpirationDayLimit>0,[N,$]=(0,o.useState)(""),[p,G]=(0,o.useState)(""),[b,j]=(0,o.useState)(M),[L,w]=(0,o.useState)(m),[_,H]=(0,o.useState)(L!==""),t=(0,S.wW)(i);(0,o.useEffect)(()=>{c&&$(`${s}-${(0,C.Z)()}`)},[s,c]);const h=A=>{H(A!==""),w(A)},X=()=>{l({name:p||N,secondsToLive:b?z(L):void 0})},T=()=>{G(""),$(""),j(M),w(m),H(L!==""),I()},B=f?"Service account token created":"Add service account token";return o.createElement(g.u,{isOpen:c,title:B,onDismiss:T,className:t.modal,contentClassName:t.modalContent},f?o.createElement(o.Fragment,null,o.createElement(d.g,{label:"Token",description:"Copy the token now as you will not be able to see it again. Losing a token requires creating a new one."},o.createElement("div",{className:t.modalTokenRow},o.createElement(D.I,{name:"tokenValue",value:f,readOnly:!0}),o.createElement(P.m,{className:t.modalCopyToClipboardButton,variant:"primary",size:"md",icon:"copy",getText:()=>f},"Copy clipboard"))),o.createElement(g.u.ButtonRow,null,o.createElement(P.m,{variant:"primary",getText:()=>f,onClipboardCopy:T},"Copy to clipboard and close"),o.createElement(y.zx,{variant:"secondary",onClick:T},"Close"))):o.createElement("div",null,o.createElement(d.g,{label:"Display name",description:"Name to easily identify the token",required:!0},o.createElement(D.I,{name:"tokenName",value:p,placeholder:N,onChange:A=>{G(A.currentTarget.value)}})),!b&&o.createElement(d.g,{label:"Expiration"},o.createElement(F.S,{options:V,value:b,onChange:j,size:"md"})),b&&o.createElement(d.g,{label:"Expiration date"},o.createElement(v.d,{onChange:h,value:L,placeholder:"",minDate:m,maxDate:u})),o.createElement(g.u.ButtonRow,null,o.createElement(y.zx,{onClick:X,disabled:b&&!_},"Generate token"))))},z=c=>{const f=new Date(c),s=new Date;return Math.ceil((f.getTime()-s.getTime())/1e3)},i=c=>({modal:n.css`
      width: 550px;
    `,modalContent:n.css`
      overflow: visible;
    `,modalTokenRow:n.css`
      display: flex;
    `,modalCopyToClipboardButton:n.css`
      margin-left: ${c.spacing(.5)};
    `})},50655:(Y,R,a)=>{a.d(R,{R5:()=>i,TL:()=>P,XE:()=>c,Xd:()=>v,bX:()=>F,fT:()=>x,yN:()=>V});var n=a(82897),o=a.n(n),C=a(54899),e=a(11630),S=a(82002),g=a(81168),d=a(45003);const D="/api/serviceaccounts";function F(){return async s=>{try{if(S.Vt.licensedAccessControlEnabled()&&S.Vt.hasPermission(g.AccessControlAction.ActionRolesList)){const l=await(0,e.ul)();s((0,d.Dn)(l))}}catch(l){console.error(l)}}}function v({withLoadingIndicator:s}={withLoadingIndicator:!1}){return async(l,I)=>{try{if(S.Vt.hasPermission(g.AccessControlAction.ServiceAccountsRead)){s&&l((0,d.pN)());const{perPage:m,page:u,query:M,serviceAccountStateFilter:N}=I().serviceAccounts,$=await(0,C.i)().get(`/api/serviceaccounts/search?perpage=${m}&page=${u}&query=${M}${z(N)}&accesscontrol=true`);l((0,d.Ub)($))}}catch(m){console.error(m)}finally{l((0,d.dt)())}}}const y=(0,n.debounce)(s=>s(v()),500,{leading:!0});function P(s){return async l=>{await(0,C.i)().patch(`${D}/${s.id}?accesscontrol=true`,{...s}),l(v())}}function V(s){return async l=>{await(0,C.i)().delete(`${D}/${s}`),l(v())}}function x(s,l,I){return async m=>{const u=await(0,C.i)().post(`${D}/${s}/tokens`,l);I(u.key),m(v())}}const z=s=>{switch(s){case g.ServiceAccountStateFilter.WithExpiredTokens:return"&expiredTokens=true";case g.ServiceAccountStateFilter.Disabled:return"&disabled=true";default:return""}};function i(s){return async l=>{l((0,d.aj)(s)),y(l)}}function c(s){return async l=>{l((0,d.M4)(s)),l(v())}}function f(s){return async l=>{l(pageChanged(s)),l(v())}}}}]);

//# sourceMappingURL=ServiceAccountsPage.0e301cb03b1496e30241.js.map
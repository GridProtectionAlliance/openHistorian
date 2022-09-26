"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[1337],{7352:(e,s,n)=>{n.r(s),n.d(s,{ServiceAccountsListPageUnconnected:()=>M,default:()=>V,getStyles:()=>B});var t,i=n(36636),a=n(64681),c=n.n(a),o=n(68404),l=n(18745),r=n(3490),d=n(28436),u=n(69371),h=n(22584),m=n(98163),g=n(47570),x=n(26343),p=n(57497),b=n(8936),v=n(45916);const j=e=>`Edit service account's ${e} details`,f=(0,o.memo)((e=>{let{serviceAccount:s,onRoleChange:n,roleOptions:a,builtInRoles:c,onRemoveButtonClick:o,onDisable:l,onEnable:d,onAddTokenClick:u}=e;const h=`org/serviceaccounts/${s.id}`,x=(0,r.useStyles2)(k),f=m.Vt.hasPermissionInMetadata(g.bW.ServiceAccountsWrite,s),y=m.Vt.hasPermission(g.bW.ActionRolesList)&&m.Vt.hasPermission(g.bW.ActionUserRolesList);return(0,v.jsxs)("tr",{className:(0,i.cx)({[x.disabled]:s.isDisabled}),children:[(0,v.jsx)("td",{className:"width-4 text-center link-td",children:(0,v.jsx)("a",{href:h,"aria-label":j(s.name),children:(0,v.jsx)("img",{className:"filter-table__avatar",src:s.avatarUrl,alt:`Avatar for user ${s.name}`})})}),(0,v.jsx)("td",{className:"link-td max-width-10",children:(0,v.jsx)("a",{className:"ellipsis",href:h,title:s.name,"aria-label":j(s.name),children:s.name})}),(0,v.jsx)("td",{className:"link-td max-width-10",children:(0,v.jsx)("a",{className:x.accountId,href:h,title:s.login,"aria-label":j(s.name),children:s.login})}),m.Vt.licensedAccessControlEnabled()?(0,v.jsx)("td",{children:y&&(0,v.jsx)(p.R,{userId:s.id,orgId:s.orgId,builtInRole:s.role,onBuiltinRoleChange:e=>n(e,s),roleOptions:a,builtInRoles:c,builtinRolesDisabled:!f,disabled:s.isDisabled})}):(0,v.jsx)("td",{children:(0,v.jsx)(b.A,{"aria-label":"Role",value:s.role,disabled:!f||s.isDisabled,onChange:e=>n(e,s)})}),(0,v.jsx)("td",{className:"link-td max-width-10",children:(0,v.jsx)("a",{className:"ellipsis",href:h,title:"Tokens","aria-label":j(s.name),children:(0,v.jsxs)("div",{className:(0,i.cx)(x.tokensInfo,{[x.tokensInfoSecondary]:!s.tokens}),children:[t||(t=(0,v.jsx)("span",{children:(0,v.jsx)(r.Icon,{name:"key-skeleton-alt"})})),s.tokens||"No tokens"]})})}),(0,v.jsx)("td",{children:(0,v.jsxs)(r.HorizontalGroup,{justify:"flex-end",children:[m.Vt.hasPermission(g.bW.ServiceAccountsWrite)&&!s.tokens&&(0,v.jsx)(r.Button,{onClick:()=>u(s),disabled:s.isDisabled,children:"Add token"}),m.Vt.hasPermissionInMetadata(g.bW.ServiceAccountsWrite,s)&&(s.isDisabled?(0,v.jsx)(r.Button,{variant:"primary",onClick:()=>d(s),children:"Enable"}):(0,v.jsx)(r.Button,{variant:"secondary",onClick:()=>l(s),children:"Disable"})),m.Vt.hasPermissionInMetadata(g.bW.ServiceAccountsDelete,s)&&(0,v.jsx)(r.IconButton,{className:x.deleteButton,name:"trash-alt",size:"md",onClick:()=>o(s),"aria-label":`Delete service account ${s.name}`})]})})]},s.id)}));f.displayName="ServiceAccountListItem";const k=e=>({iconRow:i.css`
      svg {
        margin-left: ${e.spacing(.5)};
      }
    `,accountId:(0,i.cx)("ellipsis",i.css`
        color: ${e.colors.text.secondary};
      `),deleteButton:i.css`
      color: ${e.colors.text.secondary};
    `,tokensInfo:i.css`
      span {
        margin-right: ${e.spacing(1)};
      }
    `,tokensInfoSecondary:i.css`
      color: ${e.colors.text.secondary};
    `,disabled:i.css`
      td a {
        color: ${e.colors.text.secondary};
      }
    `}),y=f;var A,I,S,C,T,$,D,N,R,w,W,L=n(79710);const O={changeQuery:L.R5,fetchACOptions:L.bX,fetchServiceAccounts:L.Xd,deleteServiceAccount:L.yN,updateServiceAccount:L.TL,changeStateFilter:L.XE,createServiceAccountToken:L.fT,getApiKeysMigrationStatus:L.hv,getApiKeysMigrationInfo:L.xi,closeApiKeysMigrationInfo:L.f3},P=(0,l.connect)((function(e){return Object.assign({},e.serviceAccounts)}),O),M=e=>{let{serviceAccounts:s,isLoading:n,roleOptions:t,builtInRoles:a,query:l,serviceAccountStateFilter:p,apiKeysMigrated:b,showApiKeysMigrationInfo:j,changeQuery:f,fetchACOptions:k,fetchServiceAccounts:L,deleteServiceAccount:O,updateServiceAccount:P,changeStateFilter:M,createServiceAccountToken:V,getApiKeysMigrationStatus:E,getApiKeysMigrationInfo:F,closeApiKeysMigrationInfo:K}=e;const _=(0,r.useStyles2)(B),[G,H]=(0,o.useState)(!1),[U,X]=(0,o.useState)(!1),[q,z]=(0,o.useState)(!1),[Q,Y]=(0,o.useState)(""),[Z,J]=(0,o.useState)(null);(0,o.useEffect)((()=>{L({withLoadingIndicator:!0}),E(),F(),m.Vt.licensedAccessControlEnabled()&&k()}),[k,L,E,F]);const ee=0===s.length&&p===g.Wc.All&&!l,se=async(e,s)=>{const n=Object.assign({},s,{role:e});P(n),m.Vt.licensedAccessControlEnabled()&&k()},ne=e=>{J(e),X(!0)},te=e=>{J(e),z(!0)},ie=e=>{P(Object.assign({},e,{isDisabled:!1}))},ae=e=>{J(e),H(!0)},ce=()=>{X(!1),J(null)},oe=()=>{z(!1),J(null)},le=A||(A=(0,v.jsx)("a",{className:"external-link",href:"https://grafana.com/docs/grafana/latest/administration/service-accounts/",target:"_blank",rel:"noopener noreferrer",children:"here."})),re=(0,v.jsxs)("span",{children:["Service accounts and their tokens can be used to authenticate against the Grafana API. Find out more ",le]});return(0,v.jsx)(u.T,{navId:"serviceaccounts",subTitle:re,children:(0,v.jsxs)(u.T.Contents,{children:[b&&j&&(0,v.jsx)(r.Alert,{title:"API keys migrated to Service accounts. Your keys are now called tokens and live inside respective service accounts. Learn more.",severity:"success",onRemove:()=>{K()}}),(0,v.jsx)(u.T.OldNavOnly,{children:(0,v.jsxs)("div",{className:_.pageHeader,children:[I||(I=(0,v.jsx)("h2",{children:"Service accounts"})),(0,v.jsxs)("div",{className:_.apiKeyInfoLabel,children:[(0,v.jsx)(r.Tooltip,{placement:"bottom",interactive:!0,content:(0,v.jsxs)(v.Fragment,{children:["API keys are now service accounts with tokens. Find out more ",le]}),children:S||(S=(0,v.jsx)(r.Icon,{name:"question-circle"}))}),C||(C=(0,v.jsx)("span",{children:"Looking for API keys?"}))]})]})}),(0,v.jsxs)("div",{className:"page-action-bar",children:[(0,v.jsx)("div",{className:"gf-form gf-form--grow",children:(0,v.jsx)(r.FilterInput,{placeholder:"Search service account by name",value:l,onChange:e=>{f(e)},width:50})}),(0,v.jsx)(r.RadioButtonGroup,{options:[{label:"All",value:g.Wc.All},{label:"With expired tokens",value:g.Wc.WithExpiredTokens},{label:"Disabled",value:g.Wc.Disabled}],onChange:e=>{M(e)},value:p,className:_.filter}),!ee&&m.Vt.hasPermission(g.bW.ServiceAccountsCreate)&&(T||(T=(0,v.jsx)(r.LinkButton,{href:"org/serviceaccounts/create",variant:"primary",children:"Add service account"})))]}),n&&($||($=(0,v.jsx)(h.Z,{}))),!n&&ee&&(0,v.jsx)(v.Fragment,{children:(0,v.jsx)(d.Z,{title:"You haven't created any service accounts yet.",buttonIcon:"key-skeleton-alt",buttonLink:"org/serviceaccounts/create",buttonTitle:"Add service account",buttonDisabled:!m.Vt.hasPermission(g.bW.ServiceAccountsCreate),proTip:"Remember, you can provide specific permissions for API access to other applications.",proTipLink:"",proTipLinkTitle:"",proTipTarget:"_blank"})}),!n&&0!==s.length&&(0,v.jsx)(v.Fragment,{children:(0,v.jsx)("div",{className:(0,i.cx)(_.table,"admin-list-table"),children:(0,v.jsxs)("table",{className:"filter-table filter-table--hover",children:[(0,v.jsx)("thead",{children:(0,v.jsxs)("tr",{children:[D||(D=(0,v.jsx)("th",{})),N||(N=(0,v.jsx)("th",{children:"Account"})),R||(R=(0,v.jsx)("th",{children:"ID"})),w||(w=(0,v.jsx)("th",{children:"Roles"})),W||(W=(0,v.jsx)("th",{children:"Tokens"})),(0,v.jsx)("th",{style:{width:"34px"}})]})}),(0,v.jsx)("tbody",{children:s.map((e=>(0,v.jsx)(y,{serviceAccount:e,builtInRoles:a,roleOptions:t,onRoleChange:se,onRemoveButtonClick:ne,onDisable:te,onEnable:ie,onAddTokenClick:ae},e.id)))})]})})}),Z&&(0,v.jsxs)(v.Fragment,{children:[(0,v.jsx)(r.ConfirmModal,{isOpen:U,body:`Are you sure you want to delete '${Z.name}'${Z.tokens?` and ${Z.tokens} accompanying ${c()("token",Z.tokens)}`:""}?`,confirmText:"Delete",title:"Delete service account",onConfirm:async()=>{Z&&O(Z.id),ce()},onDismiss:ce}),(0,v.jsx)(r.ConfirmModal,{isOpen:q,title:"Disable service account",body:`Are you sure you want to disable '${Z.name}'?`,confirmText:"Disable service account",onConfirm:()=>{Z&&P(Object.assign({},Z,{isDisabled:!0})),oe()},onDismiss:oe}),(0,v.jsx)(x.m,{isOpen:G,token:Q,serviceAccountLogin:Z.login,onCreateToken:async e=>{Z&&V(Z.id,e,Y)},onClose:()=>{H(!1),J(null),Y("")}})]})]})})},B=e=>({table:i.css`
      margin-top: ${e.spacing(3)};
    `,filter:i.css`
      margin: 0 ${e.spacing(1)};
    `,row:i.css`
      display: flex;
      align-items: center;
      height: 100% !important;

      a {
        padding: ${e.spacing(.5)} 0 !important;
      }
    `,unitTooltip:i.css`
      display: flex;
      flex-direction: column;
    `,unitItem:i.css`
      cursor: pointer;
      padding: ${e.spacing(.5)} 0;
      margin-right: ${e.spacing(1)};
    `,disabled:i.css`
      color: ${e.colors.text.disabled};
    `,link:i.css`
      color: inherit;
      cursor: pointer;
      text-decoration: underline;
    `,pageHeader:i.css`
      display: flex;
      margin-bottom: ${e.spacing(2)};
    `,apiKeyInfoLabel:i.css`
      margin-left: ${e.spacing(1)};
      line-height: 2.2;
      flex-grow: 1;
      color: ${e.colors.text.secondary};

      span {
        padding: ${e.spacing(.5)};
      }
    `,filterDelimiter:i.css`
      flex-grow: 1;
    `}),V=P(M)}}]);
//# sourceMappingURL=ServiceAccountsPage.48eb8659c24d17486122.js.map
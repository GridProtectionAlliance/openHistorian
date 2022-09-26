"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[7675],{93970:(e,t,n)=>{n.d(t,{q:()=>s});var r=n(18745),i=n(8674);const s=e=>{const t=(0,r.useSelector)((e=>e.navIndex));return(0,i.h)(t,e)}},19891:(e,t,n)=>{n.r(t),n.d(t,{default:()=>te});var r,i=n(36636),s=n(68404),a=n(17614),l=n(43215),o=n(90923),c=n(3490),d=n(5831),u=n(69371),h=n(93970),g=n(21169),x=n(45916);function p(e){let{onPathChange:t}=e;return(0,x.jsxs)("div",{children:[r||(r=(0,x.jsx)("div",{children:"TODO... Add ROOT"})),(0,x.jsx)(c.Button,{variant:"secondary",onClick:()=>t("/"),children:"Cancel"})]})}var m,f=n(82897);function v(e){let{pathName:t,onPathChange:n,rootIcon:r}=e;const i=(0,c.useStyles2)(j),s=t.split("/").filter(Boolean);return(0,x.jsxs)("ul",{className:i.breadCrumb,children:[r&&(0,x.jsx)("li",{onClick:()=>n(""),children:(0,x.jsx)(c.Icon,{name:r})}),s.map(((e,t)=>{let r="/"+s.slice(0,t+1).join("/");const i=t===s.length-1;return(0,x.jsx)("li",{onClick:i?void 0:()=>n(r),children:e},(0,f.uniqueId)(e))}))]})}function j(e){return{breadCrumb:i.css`
      list-style: none;
      padding: ${e.spacing(2,1)};

      li {
        display: inline;

        :not(:last-child) {
          color: ${e.colors.text.link};
          cursor: pointer;
        }
        + li:before {
          content: '>';
          padding: ${e.spacing(1)};
          color: ${e.colors.text.secondary};
        }
      }
    `}}const b={folderName:""};function y(e){let{validate:t,onDismiss:n,onSubmit:r}=e;return(0,x.jsx)(c.Modal,{onDismiss:n,isOpen:!0,title:"New Folder",children:(0,x.jsx)(c.Form,{defaultValues:b,onSubmit:r,maxWidth:"none",children:e=>{let{register:n,errors:r}=e;return(0,x.jsxs)(x.Fragment,{children:[(0,x.jsx)(c.Field,{label:"Folder name",invalid:!!r.folderName,error:r.folderName&&r.folderName.message,children:(0,x.jsx)(c.Input,Object.assign({id:"folder-name-input"},n("folderName",{required:"Folder name is required.",validate:{validate:t}})))}),m||(m=(0,x.jsx)(c.Modal.ButtonRow,{children:(0,x.jsx)(c.Button,{type:"submit",children:"Create"})}))]})}})})}var w,C,S,N,$,k=n(92115);const F={format:"git",generalFolderPath:"general",history:!0,exclude:{},git:{}},O=[{label:"GIT",value:"git",description:"Exports a fresh git repository"}],T=e=>{var t,n,r;let{onPathChange:i}=e;const[d,u]=(0,s.useState)(),[h,g]=(0,k.Z)("grafana.export.config",F),[p,m]=(0,s.useState)(!1),f=(0,a.Z)((()=>(0,o.getBackendSrv)().get("/api/admin/export/options")),[]),v=(0,s.useCallback)(((e,t)=>{if(!f.value||!h)return;const n={};if("*"!==e){for(let i of f.value.exporters){var r;let s=null===(r=h.exclude)||void 0===r?void 0:r[i.key];e===i.key&&(s=!t),s&&(n[i.key]=s)}g(Object.assign({},h,{exclude:n}))}else{if(!t)for(let e of f.value.exporters)n[e.key]=!0;g(Object.assign({},h,{exclude:n}))}}),[h,g,f]);return(0,s.useEffect)((()=>{const e=(0,o.getGrafanaLiveSrv)().getStream({scope:l.LiveChannelScope.Grafana,namespace:"broadcast",path:"export"}).subscribe({next:e=>{((0,l.isLiveChannelMessageEvent)(e)||(0,l.isLiveChannelStatusEvent)(e))&&u(e.message)}});return()=>{e.unsubscribe()}}),[]),(0,x.jsxs)("div",{children:[d&&(0,x.jsxs)("div",{children:[w||(w=(0,x.jsx)("h3",{children:"Status"})),(0,x.jsx)("pre",{children:JSON.stringify(d,null,2)}),d.running&&(0,x.jsx)("div",{children:(0,x.jsx)(c.Button,{variant:"secondary",onClick:()=>{(0,o.getBackendSrv)().post("/api/admin/export/stop")},children:"Stop"})})]}),!Boolean(null==d?void 0:d.running)&&(0,x.jsxs)("div",{children:[C||(C=(0,x.jsx)("h3",{children:"Export grafana instance"})),(0,x.jsx)(c.Field,{label:"Format",children:(0,x.jsx)(c.Select,{options:O,width:40,value:O.find((e=>e.value===(null==h?void 0:h.format))),onChange:e=>g(Object.assign({},h,{format:e.value}))})}),(0,x.jsx)(c.Field,{label:"Keep history",children:(0,x.jsx)(c.Switch,{value:null==h?void 0:h.history,onChange:e=>g(Object.assign({},h,{history:e.currentTarget.checked}))})}),(0,x.jsx)(c.Field,{label:"Include",children:(0,x.jsxs)(x.Fragment,{children:[(0,x.jsx)(c.InlineFieldRow,{children:(0,x.jsx)(c.InlineField,{label:"Toggle all",labelWidth:18,children:(0,x.jsx)(c.InlineSwitch,{value:0===Object.keys(null!==(t=null==h?void 0:h.exclude)&&void 0!==t?t:{}).length,onChange:e=>v("*",e.currentTarget.checked)})})}),f.value&&(0,x.jsx)("div",{children:f.value.exporters.map((e=>{var t;return(0,x.jsx)(c.InlineFieldRow,{children:(0,x.jsx)(c.InlineField,{label:e.name,labelWidth:18,tooltip:e.description,children:(0,x.jsx)(c.InlineSwitch,{value:!0!==(null==h||null===(t=h.exclude)||void 0===t?void 0:t[e.key]),onChange:t=>v(e.key,t.currentTarget.checked)})})},e.key)}))})]})}),(0,x.jsx)(c.Field,{label:"General folder",description:"Set the folder name for items without a real folder",children:(0,x.jsx)(c.Input,{width:40,value:null!==(n=null==h?void 0:h.generalFolderPath)&&void 0!==n?n:"",onChange:e=>g(Object.assign({},h,{generalFolderPath:e.currentTarget.value})),placeholder:"root folder path"})}),(0,x.jsxs)(c.HorizontalGroup,{children:[(0,x.jsx)(c.Button,{onClick:()=>{(0,o.getBackendSrv)().post("/api/admin/export",h).then((e=>{e.cfg&&e.status.running&&g(e.cfg)}))},variant:"primary",children:"Export"}),S||(S=(0,x.jsx)(c.LinkButton,{href:"admin/storage/",variant:"secondary",children:"Cancel"}))]})]}),N||(N=(0,x.jsx)("br",{})),$||($=(0,x.jsx)("br",{})),(0,x.jsx)(c.Collapse,{label:"Request details",isOpen:p,onToggle:m,collapsible:!0,children:(0,x.jsx)(c.CodeEditor,{height:275,value:null!==(r=JSON.stringify(h,null,2))&&void 0!==r?r:"",showLineNumbers:!1,readOnly:!1,language:"json",showMiniMap:!1,onBlur:e=>{g(JSON.parse(e))}})})]})};var I,B,P,E=n(17377),W=n(87723),L=n(13868),R=n(57545);function D(e){let{listing:t,path:n,onPathChange:r,view:i}=e;const l=(0,c.useStyles2)(M),o=(0,s.useMemo)((()=>function(e){const t=e.lastIndexOf(".");if(t<0)return{};switch(e.substring(t+1).toLowerCase()){case"svg":return{category:"svg"};case"jpg":case"jpeg":case"png":case"webp":case"gif":return{category:"image"};case"geojson":case"json":return{category:"text",language:"json"};case"text":case"go":case"md":return{category:"text"}}return{}}(n)),[n]),d=(0,a.Z)((async()=>{if("text"===o.category){const e=await(0,L.$)().get(n);return(0,f.isString)(e)?e:JSON.stringify(e,null,2)}return null}),[o,n]);switch(i){case R.i.Config:return I||(I=(0,x.jsx)("div",{children:"CONFIGURE?"}));case R.i.Perms:return B||(B=(0,x.jsx)("div",{children:"Permissions"}));case R.i.History:return P||(P=(0,x.jsx)("div",{children:"TODO... history"}))}let u=`api/storage/read/${n}`;switch(u.endsWith("/")&&(u=u.substring(0,u.length-1)),o.category){case"svg":return(0,x.jsx)("div",{children:(0,x.jsx)(E.Z,{src:u,className:l.icon})});case"image":return(0,x.jsx)("div",{children:(0,x.jsx)("a",{target:"_self",href:u,children:(0,x.jsx)("img",{src:u,className:l.img})})});case"text":return(0,x.jsx)("div",{className:l.tableWrapper,children:(0,x.jsx)(W.Z,{children:e=>{var t,n;let{width:r,height:i}=e;return(0,x.jsx)(c.CodeEditor,{width:r,height:i,value:null!==(t=d.value)&&void 0!==t?t:"",showLineNumbers:!1,readOnly:!0,language:null!==(n=o.language)&&void 0!==n?n:"text",showMiniMap:!1,onBlur:e=>{console.log("CHANGED!",e)}})}})})}return(0,x.jsxs)("div",{children:["FILE: ",(0,x.jsx)("a",{href:u,children:n})]})}const M=e=>({wrapper:i.css`
    display: flex;
    flex-direction: column;
    height: 100%;
  `,tableControlRowWrapper:i.css`
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: ${e.spacing(2)};
  `,tableWrapper:i.css`
    border: 1px solid ${e.colors.border.medium};
    height: 100%;
  `,uploadSpot:i.css`
    margin-left: ${e.spacing(2)};
  `,border:i.css`
    border: 1px solid ${e.colors.border.medium};
    padding: ${e.spacing(2)};
  `,img:i.css`
    max-width: 100%;
    // max-height: 147px;
    // fill: ${e.colors.text.primary};
  `,icon:i.css`
    // max-width: 100%;
    // max-height: 147px;
    // fill: ${e.colors.text.primary};
  `});var A,G;function H(e){let{listing:t,view:n}=e;const r=(0,c.useStyles2)(Z);switch(n){case R.i.Config:return A||(A=(0,x.jsx)("div",{children:"CONFIGURE?"}));case R.i.Perms:return G||(G=(0,x.jsx)("div",{children:"Permissions"}))}return(0,x.jsx)("div",{className:r.tableWrapper,children:(0,x.jsx)(W.Z,{children:e=>{let{width:n,height:r}=e;return(0,x.jsx)("div",{style:{width:`${n}px`,height:`${r}px`},children:(0,x.jsx)(c.Table,{height:r,width:n,data:t,noHeader:!1,showTypeIcons:!1,resizable:!1})})}})})}const Z=e=>({wrapper:i.css`
    display: flex;
    flex-direction: column;
    height: 100%;
  `,tableControlRowWrapper:i.css`
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: ${e.spacing(2)};
  `,tableWrapper:i.css`
    border: 1px solid ${e.colors.border.medium};
    height: 100%;
  `,uploadSpot:i.css`
    margin-left: ${e.spacing(2)};
  `,border:i.css`
    border: 1px solid ${e.colors.border.medium};
    padding: ${e.spacing(2)};
  `});function q(e){let{root:t,onPathChange:n}=e;const r=(0,c.useStyles2)(z),i=(0,a.Z)((0,L.$)().getConfig),[l,d]=(0,s.useState)("");let u=location.pathname;u.endsWith("/")||(u+="/");const h=(0,s.useMemo)((()=>{const e=i.value;if(null!=l&&l.length&&e){const t=l.toLowerCase();return e.filter((e=>{const n=e.config;return!!(n.name.toLowerCase().indexOf(t)>=0||n.description.toLowerCase().indexOf(t)>=0)}))}return null!=e?e:[]}),[l,i]);return(0,x.jsxs)("div",{children:[(0,x.jsxs)("div",{className:"page-action-bar",children:[(0,x.jsx)("div",{className:"gf-form gf-form--grow",children:(0,x.jsx)(c.FilterInput,{placeholder:"Search Storage",value:l,onChange:d})}),(0,x.jsx)(c.Button,{className:"pull-right",onClick:()=>n("",R.i.AddRoot),children:"Add Root"}),o.config.featureToggles.export&&(0,x.jsx)(c.Button,{className:"pull-right",onClick:()=>n("",R.i.Export),children:"Export"})]}),(0,x.jsx)(c.VerticalGroup,{children:h.map((e=>{var t,n,i,s;const a=e.ready;return(0,x.jsxs)(c.Card,{href:a?`admin/storage/${e.config.prefix}/`:void 0,children:[(0,x.jsx)(c.Card.Heading,{children:e.config.name}),(0,x.jsxs)(c.Card.Meta,{className:r.clickable,children:[e.config.description,(null===(t=e.config.git)||void 0===t?void 0:t.remote)&&(0,x.jsx)("a",{href:null===(n=e.config.git)||void 0===n?void 0:n.remote,children:null===(i=e.config.git)||void 0===i?void 0:i.remote})]}),null===(s=e.notice)||void 0===s?void 0:s.map((e=>(0,x.jsx)(c.Alert,{severity:e.severity,title:e.text},e.text))),(0,x.jsx)(c.Card.Tags,{className:r.clickable,children:(0,x.jsx)(c.HorizontalGroup,{children:(0,x.jsx)(c.TagList,{tags:J(e)})})}),(0,x.jsx)(c.Card.Figure,{className:r.clickable,children:(0,x.jsx)(c.Icon,{name:U(e.config.type),size:"xxxl",className:r.secondaryTextColor})})]},e.config.prefix)}))})]})}function z(e){return{secondaryTextColor:i.css`
      color: ${e.colors.text.secondary};
    `,clickable:i.css`
      pointer-events: none;
    `}}function J(e){const t=[];return e.builtin&&t.push("Builtin"),e.editable||t.push("Read only"),e.ready||t.push("Not ready"),t}function U(e){switch(e){case"git":return"code-branch";default:return"folder-open";case"sql":return"database"}}var V,_;function K(e){let{setErrorMessages:t,setPath:n,path:r,fileNames:i}=e;const a=(0,c.useStyles2)(Q),[l,o]=(0,s.useState)(void 0),[d,u]=(0,s.useState)(!1),[h,g]=(0,s.useState)(1),[p,m]=(0,s.useState)(!0);(0,s.useEffect)((()=>{g((e=>e+1))}),[l]);const f=async(e,i)=>{if(!e)return void t(["Please select a file."]);const s=await(0,L.$)().upload(r,e,i);200!==s.status?t([s.message]):(e=>{console.log("Uploaded: "+r),e.path?n(e.path):n(r)})(s)};return(0,x.jsxs)(x.Fragment,{children:[(0,x.jsx)(c.FileUpload,{accept:"image/jpg, image/jpeg, image/png, image/gif, image/webp",onFileUpload:e=>{t([]);const n=e.currentTarget.files&&e.currentTarget.files.length>0&&e.currentTarget.files[0]?e.currentTarget.files[0]:void 0;if(n){o(n);(0,L.J)(n.name,i)?(u(!0),m(!0)):(u(!1),f(n,!1).then((e=>{})))}},className:a.uploadButton,children:"Upload"},h),l&&d&&(0,x.jsx)(c.ConfirmModal,{isOpen:p,body:(0,x.jsxs)("div",{children:[(0,x.jsx)("p",{children:null==l?void 0:l.name}),V||(V=(0,x.jsx)("p",{children:"A file with this name already exists."})),_||(_=(0,x.jsx)("p",{children:"What would you like to do?"}))]}),title:"This file already exists",confirmText:"Replace",onConfirm:()=>{l&&(f(l,!0).then((e=>{})),m(!1))},onDismiss:()=>{o(void 0),u(!1),m(!1)}})]})}const Q=e=>({uploadButton:i.css`
    margin-right: ${e.spacing(2)};
  `});var X,Y;const ee=/^[a-z\d!\-_.*'() ]+$/;function te(e){var t,n,r,i,m;const f=(0,c.useStyles2)(ne),j=(0,h.q)("storage"),b=null!==(t=e.match.params.path)&&void 0!==t?t:"",w=null!==(n=e.queryParams.view)&&void 0!==n?n:R.i.Data,C=(e,t)=>{let n=("/admin/storage/"+e).replace("//","/");t&&t!==R.i.Data&&(n+="?view="+t),o.locationService.push(n)},[S,N]=(0,s.useState)(!1),[$,k]=(0,s.useState)([]),F=(0,a.Z)((()=>(0,L.$)().list(b).then((e=>{if(e){const t=e.fields[0];e.fields[0]=Object.assign({},t,{getLinks:e=>{var n;const r=t.values.get(null!==(n=e.valueRowIndex)&&void 0!==n?n:0),i=b+"/"+r;return[{title:`Open ${r}`,href:`/admin/storage/${i}`,target:"_self",origin:t,onClick:()=>{C(i)}}]}})}return e}))),[b]),O=(0,s.useMemo)((()=>{let e=(null==b?void 0:b.indexOf("/"))<0;if(F.value){if(1===F.value.length){const t=F.value.fields[0].values.get(0);e=!b.endsWith(t)}else e=!0}return e}),[b,F]),I=(0,s.useMemo)((()=>{var e,t,n,r,i,s;return null!==(e=null===(t=F.value)||void 0===t||null===(n=t.fields)||void 0===n||null===(r=n.find((e=>"name"===e.name)))||void 0===r||null===(i=r.values)||void 0===i||null===(s=i.toArray())||void 0===s?void 0:s.filter((e=>"string"==typeof e)))&&void 0!==e?e:[]}),[F]);return(0,x.jsx)(u.T,{navModel:j,children:(0,x.jsx)(u.T.Contents,{isLoading:F.loading,children:(()=>{const e=!(null!=b&&b.length)||"/"===b;switch(w){case R.i.Export:return e?r||(r=(0,x.jsx)(T,{onPathChange:C})):(C(""),X||(X=(0,x.jsx)(c.Spinner,{})));case R.i.AddRoot:return e?i||(i=(0,x.jsx)(p,{onPathChange:C})):(C(""),Y||(Y=(0,x.jsx)(c.Spinner,{})))}const t=F.value;if(!(0,l.isDataFrame)(t))return(0,x.jsx)(x.Fragment,{});if(e)return(0,x.jsx)(q,{root:t,onPathChange:C});const n=[{what:R.i.Data,text:"Data"}];b.indexOf("/")<0&&n.push({what:R.i.Config,text:"Configure"}),O?n.push({what:R.i.Perms,text:"Permissions"}):n.push({what:R.i.History,text:"History"});const s=O&&b.startsWith("resources"),a=b.startsWith("resources/"),u=o.config.featureToggles.dashboardsFromStorage&&(O||b.endsWith(".json")),h=()=>{k([])};return(0,x.jsxs)("div",{className:f.wrapper,children:[(0,x.jsxs)(c.HorizontalGroup,{width:"100%",justify:"space-between",spacing:"md",height:25,children:[(0,x.jsx)(v,{pathName:b,onPathChange:C,rootIcon:j.node.icon}),(0,x.jsxs)(c.HorizontalGroup,{children:[u&&(0,x.jsx)(c.LinkButton,{icon:"dashboard",href:`g/${b}`,children:"Dashboard"}),s&&(0,x.jsxs)(x.Fragment,{children:[m||(m=(0,x.jsx)(K,{path:b,setErrorMessages:k,fileNames:I,setPath:C})),(0,x.jsx)(c.Button,{onClick:()=>N(!0),children:"New Folder"})]}),a&&(0,x.jsx)(c.Button,{variant:"destructive",onClick:()=>{const e=O?"Are you sure you want to delete this folder and all its contents?":"Are you sure you want to delete this file?",t=(e=>{const t=e.lastIndexOf("/");return t<1?"":e.substring(0,t)})(b);d.Z.publish(new g.VJ({title:"Delete "+(O?"folder":"file"),text:e,icon:"trash-alt",yesText:"Delete",onConfirm:()=>(0,L.$)().delete({path:b,isFolder:O}).then((()=>{C(t)}))}))},children:"Delete"})]})]}),$.length>0&&(0,x.jsx)("div",{className:f.errorAlert,children:(0,x.jsx)(c.Alert,{title:"Upload failed",severity:"error",onRemove:h,children:$.map((e=>(0,x.jsx)("div",{children:e},e)))})}),(0,x.jsx)(c.TabsBar,{children:n.map((e=>(0,x.jsx)(c.Tab,{label:e.text,active:e.what===w,onChangeTab:()=>C(b,e.what)},e.what)))}),O?(0,x.jsx)(H,{listing:t,view:w}):(0,x.jsx)(D,{path:b,listing:t,onPathChange:C,view:w}),S&&(0,x.jsx)(y,{onSubmit:async e=>{let{folderName:t}=e;const n=`${b}/${t}`,r=await(0,L.$)().createFolder(n);"string"!=typeof(null==r?void 0:r.error)&&(C(n),N(!1))},onDismiss:()=>{N(!1)},validate:e=>{const t=e.toLowerCase();return(0,L.J)(e,I)?"A file or a folder with the same name already exists":ee.test(t)?!(e.length>256)||"Name is too long, maximum length: 256 characters":"Name contains illegal characters"}})]})})()})})}const ne=e=>({wrapper:i.css`
    display: flex;
    flex-direction: column;
    height: 100%;
  `,tableControlRowWrapper:i.css`
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: ${e.spacing(2)};
  `,tableWrapper:i.css`
    border: 1px solid ${e.colors.border.medium};
    height: 100%;
  `,border:i.css`
    border: 1px solid ${e.colors.border.medium};
    padding: ${e.spacing(2)};
  `,errorAlert:i.css`
    padding-top: 20px;
  `,uploadButton:i.css`
    margin-right: ${e.spacing(2)};
  `})}}]);
//# sourceMappingURL=StoragePage.f3a7d4879e1b6c4e8877.js.map
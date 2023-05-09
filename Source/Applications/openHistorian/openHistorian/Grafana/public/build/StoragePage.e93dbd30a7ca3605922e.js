"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[7675],{14527:(Y,U,n)=>{n.r(U),n.d(U,{default:()=>be});var i=n(9892),e=n(68404),N=n(22350),j=n(16911),J=n(37932),T=n(72648),z=n(67487),M=n(45253),R=n(52081),Z=n(40419),s=n(31403),u=n(72142),S=n(79841),O=n(18271),w=n(79396),b=n(76770),X=n(45984);function k({onPathChange:t}){return e.createElement("div",null,e.createElement("div",null,"TODO... Add ROOT"),e.createElement(s.zx,{variant:"secondary",onClick:()=>t("/")},"Cancel"))}var H=n(82897),V=n(39904);function q({pathName:t,onPathChange:r,rootIcon:m}){const a=(0,T.wW)(_),l=t.split("/").filter(Boolean);return e.createElement("ul",{className:a.breadCrumb},m&&e.createElement("li",null,e.createElement(V.J,{name:m,onClick:()=>r("")})),l.map((o,$)=>{let g="/"+l.slice(0,$+1).join("/");const p=()=>r(g),x=$===l.length-1;return e.createElement("li",{key:(0,H.uniqueId)(o),onClick:x?void 0:p},o)}))}function _(t){return{breadCrumb:i.css`
      list-style: none;
      padding: ${t.spacing(2,1)};

      li {
        display: inline;

        :not(:last-child) {
          color: ${t.colors.text.link};
          cursor: pointer;
        }
        + li:before {
          content: '>';
          padding: ${t.spacing(1)};
          color: ${t.colors.text.secondary};
        }
      }
    `}}var K=n(35029),ee=n(94270),te=n(24799),ne=n(46967);const ae={folderName:""};function re({validate:t,onDismiss:r,onSubmit:m}){return e.createElement(K.u,{onDismiss:r,isOpen:!0,title:"New Folder"},e.createElement(ee.l,{defaultValues:ae,onSubmit:m,maxWidth:"none"},({register:a,errors:l})=>e.createElement(e.Fragment,null,e.createElement(te.g,{label:"Folder name",invalid:!!l.folderName,error:l.folderName&&l.folderName.message},e.createElement(ne.I,{id:"folder-name-input",...a("folderName",{required:"Folder name is required.",validate:{validate:t}})})),e.createElement(K.u.ButtonRow,null,e.createElement(s.zx,{type:"submit"},"Create")))))}var G=n(89050),se=n(9405),le=n(16964),D=n(63163),C=(t=>(t.Data="data",t.Config="config",t.Perms="perms",t.History="history",t.AddRoot="add",t))(C||{}),oe=(t=>(t.Save="save",t.PR="pr",t.Push="push",t))(oe||{});function ie({listing:t,path:r,onPathChange:m,view:a}){const l=(0,T.wW)(de),o=(0,e.useMemo)(()=>ce(r),[r]),$=(0,N.Z)(async()=>{if(o.category==="text"){const p=await(0,D.$)().get(r);return(0,H.isString)(p)?p:JSON.stringify(p,null,2)}return null},[o,r]);switch(a){case C.Config:return e.createElement("div",null,"CONFIGURE?");case C.Perms:return e.createElement("div",null,"Permissions");case C.History:return e.createElement("div",null,"TODO... history")}let g=`api/storage/read/${r}`;switch(g.endsWith("/")&&(g=g.substring(0,g.length-1)),o.category){case"svg":return e.createElement("div",null,e.createElement(le.V,{src:g,className:l.icon}));case"image":return e.createElement("div",null,e.createElement("a",{target:"_self",href:g},e.createElement("img",{src:g,alt:"File preview",className:l.img})));case"text":return e.createElement("div",{className:l.tableWrapper},e.createElement(G.Z,null,({width:p,height:x})=>e.createElement(se.p,{width:p,height:x,value:$.value??"",showLineNumbers:!1,readOnly:!0,language:o.language??"text",showMiniMap:!1,onBlur:h=>{console.log("CHANGED!",h)}})))}return e.createElement("div",null,"FILE: ",e.createElement("a",{href:g},r))}function ce(t){const r=t.lastIndexOf(".");if(r<0)return{};switch(t.substring(r+1).toLowerCase()){case"svg":return{category:"svg"};case"jpg":case"jpeg":case"png":case"webp":case"gif":return{category:"image"};case"geojson":case"json":return{category:"text",language:"json"};case"text":case"go":case"md":return{category:"text"}}return{}}const de=t=>({wrapper:i.css`
    display: flex;
    flex-direction: column;
    height: 100%;
  `,tableControlRowWrapper:i.css`
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: ${t.spacing(2)};
  `,tableWrapper:i.css`
    border: 1px solid ${t.colors.border.medium};
    height: 100%;
  `,uploadSpot:i.css`
    margin-left: ${t.spacing(2)};
  `,border:i.css`
    border: 1px solid ${t.colors.border.medium};
    padding: ${t.spacing(2)};
  `,img:i.css`
    max-width: 100%;
    // max-height: 147px;
    // fill: ${t.colors.text.primary};
  `,icon:i.css`
    // max-width: 100%;
    // max-height: 147px;
    // fill: ${t.colors.text.primary};
  `});var ue=n(88144);function me({listing:t,view:r}){const m=(0,T.wW)(ge);switch(r){case C.Config:return e.createElement("div",null,"CONFIGURE?");case C.Perms:return e.createElement("div",null,"Permissions")}return e.createElement("div",{className:m.tableWrapper},e.createElement(G.Z,null,({width:a,height:l})=>e.createElement("div",{style:{width:`${a}px`,height:`${l}px`}},e.createElement(ue.i,{height:l,width:a,data:t,noHeader:!1,showTypeIcons:!1,resizable:!1}))))}const ge=t=>({wrapper:i.css`
    display: flex;
    flex-direction: column;
    height: 100%;
  `,tableControlRowWrapper:i.css`
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: ${t.spacing(2)};
  `,tableWrapper:i.css`
    border: 1px solid ${t.colors.border.medium};
    height: 100%;
  `,uploadSpot:i.css`
    margin-left: ${t.spacing(2)};
  `,border:i.css`
    border: 1px solid ${t.colors.border.medium};
    padding: ${t.spacing(2)};
  `});var B=n(72948),fe=n(34807),pe=n(14747);function he({root:t,onPathChange:r}){const m=(0,T.wW)(ve),a=(0,N.Z)((0,D.$)().getConfig),[l,o]=(0,e.useState)("");let $=location.pathname;$.endsWith("/")||($+="/");const g=(0,e.useMemo)(()=>{let x=a.value??[];if(l?.length){const v=l.toLowerCase();x=x.filter(P=>{const d=P.config;return d.name.toLowerCase().indexOf(v)>=0||d.description.toLowerCase().indexOf(v)>=0})}const h=[],c=[];for(const v of x??[])v.config.underContentRoot?c.push(v):v.config.prefix!=="content"&&h.push(v);return{base:h,content:c}},[l,a]),p=(x,h)=>e.createElement(R.wc,null,h.map(c=>{const v=c.ready;return e.createElement(B.Z,{key:c.config.prefix,href:v?`admin/storage/${x}${c.config.prefix}/`:void 0},e.createElement(B.Z.Heading,null,c.config.name),e.createElement(B.Z.Meta,{className:m.clickable},c.config.description,c.config.git?.remote&&e.createElement("a",{href:c.config.git?.remote},c.config.git?.remote)),c.notice?.map(P=>e.createElement(M.b,{key:P.text,severity:P.severity,title:P.text})),e.createElement(B.Z.Tags,{className:m.clickable},e.createElement(R.Lh,null,e.createElement(fe.P,{tags:Ee(c)}))),e.createElement(B.Z.Figure,{className:m.clickable},e.createElement(V.J,{name:ye(c.config.type),size:"xxxl",className:m.secondaryTextColor})))}));return e.createElement("div",null,e.createElement("div",{className:"page-action-bar"},e.createElement("div",{className:"gf-form gf-form--grow"},e.createElement(pe.H,{placeholder:"Search Storage",value:l,onChange:o})),e.createElement(s.zx,{className:"pull-right",onClick:()=>r("",C.AddRoot)},"Add Root")),e.createElement("div",null,p("",g.base)),e.createElement("div",null,e.createElement("h3",null,"Content"),p("content/",g.content)))}function ve(t){return{secondaryTextColor:i.css`
      color: ${t.colors.text.secondary};
    `,clickable:i.css`
      pointer-events: none;
    `}}function Ee(t){const r=[];return t.builtin&&r.push("Builtin"),t.ready||r.push("Not ready"),r}function ye(t){switch(t){case"git":return"code-branch";case"disk":return"folder-open";case"sql":return"database";default:return"folder-open"}}var xe=n(82381),Ce=n(98102);const we="image/jpg, image/jpeg, image/png, image/gif, image/webp";function $e({setErrorMessages:t,setPath:r,path:m,fileNames:a}){const l=(0,T.wW)(Fe),[o,$]=(0,e.useState)(void 0),[g,p]=(0,e.useState)(!1),[x,h]=(0,e.useState)(1),[c,v]=(0,e.useState)(!0);(0,e.useEffect)(()=>{h(f=>f+1)},[o]);const P=f=>{console.log("Uploaded: "+m),f.path?r(f.path):r(m)},d=async(f,L)=>{if(!f){t(["Please select a file."]);return}const W=await(0,D.$)().upload(m,f,L);W.status!==200?t([W.message]):P(W)},E=f=>{t([]);const L=f.currentTarget.files&&f.currentTarget.files.length>0&&f.currentTarget.files[0]?f.currentTarget.files[0]:void 0;L&&($(L),(0,D.J)(L.name,a)?(p(!0),v(!0)):(p(!1),d(L,!1).then(y=>{})))},F=()=>{o&&(d(o,!0).then(f=>{}),v(!1))},A=()=>{$(void 0),p(!1),v(!1)};return e.createElement(e.Fragment,null,e.createElement(xe.p,{accept:we,onFileUpload:E,key:x,className:l.uploadButton},"Upload"),o&&g&&e.createElement(Ce.s,{isOpen:c,body:e.createElement("div",null,e.createElement("p",null,o?.name),e.createElement("p",null,"A file with this name already exists."),e.createElement("p",null,"What would you like to do?")),title:"This file already exists",confirmText:"Replace",onConfirm:F,onDismiss:A}))}const Fe=t=>({uploadButton:i.css`
    margin-right: ${t.spacing(2)};
  `}),Ne=/^[a-z\d!\-_.*'() ]+$/,Q=256,Oe=t=>{const r=t.lastIndexOf("/");return r<1?"":t.substring(0,r)};function be(t){const r=(0,T.wW)(Pe),m=(0,b.q)("storage"),a=t.match.params.path??"",l=t.queryParams.view??C.Data,o=(d,E)=>{let F=("/admin/storage/"+d).replace("//","/");E&&E!==C.Data&&(F+="?view="+E),J.E1.push(F)},[$,g]=(0,e.useState)(!1),[p,x]=(0,e.useState)([]),h=(0,N.Z)(()=>(0,D.$)().list(a).then(d=>{if(d){const E=d.fields[0];d.fields[0]={...E,getLinks:F=>{const A=E.values.get(F.valueRowIndex??0),f=a+"/"+A;return[{title:`Open ${A}`,href:`/admin/storage/${f}`,target:"_self",origin:E,onClick:()=>{o(f)}}]}}}return d}),[a]),c=(0,e.useMemo)(()=>{let d=a?.indexOf("/")<0;if(h.value)if(h.value.length===1){const F=h.value.fields[0].values.get(0);d=!a.endsWith(F)}else d=!0;return d},[a,h]),v=(0,e.useMemo)(()=>h.value?.fields?.find(d=>d.name==="name")?.values?.toArray()?.filter(d=>typeof d=="string")??[],[h]),P=()=>{const d=!a?.length||a==="/";switch(l){case C.AddRoot:return d?e.createElement(k,{onPathChange:o}):(o(""),e.createElement(z.$,null))}const E=h.value;if(!(0,j.aY)(E))return e.createElement(e.Fragment,null);if(d)return e.createElement(he,{root:E,onPathChange:o});const F=[{what:C.Data,text:"Data"}];a.indexOf("/")<0&&F.push({what:C.Config,text:"Configure"}),c||F.push({what:C.History,text:"History"});const A=c&&(a.startsWith("resources")||a.startsWith("content")),f=a.startsWith("resources/")||a.startsWith("content/"),L=()=>e.createElement("div",{className:r.errorAlert},e.createElement(M.b,{title:"Upload failed",severity:"error",onRemove:W},p.map(y=>e.createElement("div",{key:y},y)))),W=()=>{x([])};return e.createElement("div",{className:r.wrapper},e.createElement(R.Lh,{width:"100%",justify:"space-between",spacing:"md",height:25},e.createElement(q,{pathName:a,onPathChange:o,rootIcon:(0,Z.toIconName)(m.node.icon??"")}),e.createElement(R.Lh,null,A&&e.createElement(e.Fragment,null,e.createElement($e,{path:a,setErrorMessages:x,fileNames:v,setPath:o}),e.createElement(s.zx,{onClick:()=>g(!0)},"New Folder")),f&&e.createElement(s.zx,{variant:"destructive",onClick:()=>{const y=c?"Are you sure you want to delete this folder and all its contents?":"Are you sure you want to delete this file?",I=Oe(a);O.Z.publish(new X.VJ({title:`Delete ${c?"folder":"file"}`,text:y,icon:"trash-alt",yesText:"Delete",onConfirm:()=>(0,D.$)().delete({path:a,isFolder:c}).then(()=>{o(I)})}))}},"Delete"))),p.length>0&&L(),e.createElement(u.J,null,F.map(y=>e.createElement(S.O,{key:y.what,label:y.text,active:y.what===l,onChangeTab:()=>o(a,y.what)}))),c?e.createElement(me,{listing:E,view:l}):e.createElement(ie,{path:a,listing:E,onPathChange:o,view:l}),$&&e.createElement(re,{onSubmit:async({folderName:y})=>{const I=`${a}/${y}`;typeof(await(0,D.$)().createFolder(I))?.error!="string"&&(o(I),g(!1))},onDismiss:()=>{g(!1)},validate:y=>{const I=y.toLowerCase();return(0,D.J)(y,v)?"A file or a folder with the same name already exists":Ne.test(I)?y.length>Q?`Name is too long, maximum length: ${Q} characters`:!0:"Name contains illegal characters"}}))};return e.createElement(w.T,{navModel:m},e.createElement(w.T.Contents,{isLoading:h.loading},P()))}const Pe=t=>({wrapper:i.css`
    display: flex;
    flex-direction: column;
    height: 100%;
  `,tableControlRowWrapper:i.css`
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: ${t.spacing(2)};
  `,tableWrapper:i.css`
    border: 1px solid ${t.colors.border.medium};
    height: 100%;
  `,border:i.css`
    border: 1px solid ${t.colors.border.medium};
    padding: ${t.spacing(2)};
  `,errorAlert:i.css`
    padding-top: 20px;
  `,uploadButton:i.css`
    margin-right: ${t.spacing(2)};
  `})},63163:(Y,U,n)=>{n.d(U,{$:()=>R,J:()=>z});var i=n(70006),e=n(36512),N=n(54899),j=n(35645),J=n(29930);class T{constructor(){}async get(s){const u=`api/storage/read/${s}`.replace("//","/");return(0,N.i)().get(u)}async list(s){let u="api/storage/list/";s&&(u+=s+"/");const S=await(0,N.i)().get(u);if(S?.data){const O=(0,i.vP)(S);for(const w of O.fields)w.display=(0,e.U_)({field:w,theme:j.v.theme2});return O}}async createFolder(s){const u=await(0,N.i)().post("/api/storage/createFolder",JSON.stringify({path:s}));return u.success?{}:{error:u.message??"unknown error"}}async deleteFolder(s){const u=await(0,N.i)().post("/api/storage/deleteFolder",JSON.stringify(s));return u.success?{}:{error:u.message??"unknown error"}}async deleteFile(s){const u=await(0,N.i)().post(`/api/storage/delete/${s.path}`);return u.success?{}:{error:u.message??"unknown error"}}async delete(s){return s.isFolder?this.deleteFolder({path:s.path,force:!0}):this.deleteFile({path:s.path})}async upload(s,u,S){const O=new FormData;O.append("folder",s),O.append("file",u),O.append("overwriteExistingFile",String(S));const w=await fetch("/api/storage/upload",{method:"POST",body:O});let b=await w.json();return b||(b={}),b.status=w.status,b.statusText=w.statusText,w.status!==200&&!b.err&&(b.err=!0),b}async write(s,u){return J.ae.post(`/api/storage/write/${s}`,u)}async getConfig(){return(0,N.i)().get("/api/storage/config")}async getOptions(s){return(0,N.i)().get(`/api/storage/options/${s}`)}}function z(Z,s){const S=Z.toLowerCase().trim();return s.map(w=>w.trim().toLowerCase()).includes(S)}let M;function R(){return M||(M=new T),M}}}]);

//# sourceMappingURL=StoragePage.e93dbd30a7ca3605922e.js.map
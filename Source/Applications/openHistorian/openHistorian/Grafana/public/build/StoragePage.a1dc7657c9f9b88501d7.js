"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[1927],{72235:(Z,R,s)=>{s.d(R,{l:()=>T});var e=s(74848),a=s(32196),f=s(96540),U=s(49785);function T({defaultValues:K,onSubmit:H,validateOnMount:$=!1,validateFieldsOnMount:b,children:M,validateOn:o="onSubmit",maxWidth:c=600,...O}){const{handleSubmit:v,trigger:F,formState:A,...V}=(0,U.mN)({mode:o,defaultValues:K});return(0,f.useEffect)(()=>{$&&F(b)},[F,b,$]),(0,e.jsx)("form",{className:(0,a.css)({maxWidth:c!=="none"?c+"px":c,width:"100%"}),onSubmit:v(H),...O,children:M({errors:A.errors,formState:A,trigger:F,...V})})}},68932:(Z,R,s)=>{s.r(R),s.d(R,{default:()=>De});var e=s(74848),a=s(32196),f=s(96540),U=s(54148),T=s(16817),K=s(14236),H=s(39601),$=s(40845),b=s(62930),M=s(42418),o=s(90613),c=s(8887),O=s(67061),v=s(55852),F=s(63021),A=s(40675),V=s(28138),G=s(21780),q=s(25249),_=s(28444);function ee({onPathChange:t}){return(0,e.jsxs)("div",{children:[(0,e.jsx)("div",{children:"TODO... Add ROOT"}),(0,e.jsx)(v.$n,{variant:"secondary",onClick:()=>t("/"),children:"Cancel"})]})}var J=s(2543),Y=s(14578);function te({pathName:t,onPathChange:n,rootIcon:u}){const r=(0,$.of)(se),i=t.split("/").filter(Boolean);return(0,e.jsxs)("ul",{className:r.breadCrumb,children:[u&&(0,e.jsx)("li",{children:(0,e.jsx)(Y.I,{name:u,onClick:()=>n("")})}),i.map((l,w)=>{let h="/"+i.slice(0,w+1).join("/");const m=()=>n(h),E=w===i.length-1;return(0,e.jsx)("li",{onClick:E?void 0:m,children:l},(0,J.uniqueId)(l))})]})}function se(t){return{breadCrumb:(0,a.css)`
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
    `}}var Q=s(37390),re=s(88575),ne=s(10354),ae=s(72235);const oe={folderName:""};function ie({validate:t,onDismiss:n,onSubmit:u}){return(0,e.jsx)(Q.a,{onDismiss:n,isOpen:!0,title:"New Folder",children:(0,e.jsx)(ae.l,{defaultValues:oe,onSubmit:u,maxWidth:"none",children:({register:r,errors:i})=>(0,e.jsxs)(e.Fragment,{children:[(0,e.jsx)(re.D,{label:"Folder name",invalid:!!i.folderName,error:i.folderName&&i.folderName.message,children:(0,e.jsx)(ne.p,{id:"folder-name-input",...r("folderName",{required:"Folder name is required.",validate:{validate:t}})})}),(0,e.jsx)(Q.a.ButtonRow,{children:(0,e.jsx)(v.$n,{type:"submit",children:"Create"})})]})})})}var X=s(70713),le=s(32372),ce=s(47694),D=s(53652),N=(t=>(t.Data="data",t.Config="config",t.Perms="perms",t.History="history",t.AddRoot="add",t))(N||{}),de=(t=>(t.Save="save",t.PR="pr",t.Push="push",t))(de||{});function ge({listing:t,path:n,onPathChange:u,view:r}){const i=(0,$.of)(ue),l=(0,f.useMemo)(()=>fe(n),[n]),w=(0,T.A)(async()=>{if(l.category==="text"){const m=await(0,D.o)().get(n);return(0,J.isString)(m)?m:JSON.stringify(m,null,2)}return null},[l,n]);switch(r){case N.Config:return(0,e.jsx)("div",{children:"CONFIGURE?"});case N.Perms:return(0,e.jsx)("div",{children:"Permissions"});case N.History:return(0,e.jsx)("div",{children:"TODO... history"})}let h=`api/storage/read/${n}`;switch(h.endsWith("/")&&(h=h.substring(0,h.length-1)),l.category){case"svg":return(0,e.jsx)("div",{children:(0,e.jsx)(ce.y,{src:h,className:i.icon})});case"image":return(0,e.jsx)("div",{children:(0,e.jsx)("a",{target:"_self",href:h,children:(0,e.jsx)("img",{src:h,alt:"File preview",className:i.img})})});case"text":return(0,e.jsx)("div",{className:i.tableWrapper,children:(0,e.jsx)(X.Ay,{children:({width:m,height:E})=>(0,e.jsx)(le.B,{width:m,height:E,value:w.value??"",showLineNumbers:!1,readOnly:!0,language:l.language??"text",showMiniMap:!1,onBlur:p=>{console.log("CHANGED!",p)}})})})}return(0,e.jsxs)("div",{children:["FILE: ",(0,e.jsx)("a",{href:h,children:n})]})}function fe(t){const n=t.lastIndexOf(".");if(n<0)return{};switch(t.substring(n+1).toLowerCase()){case"svg":return{category:"svg"};case"jpg":case"jpeg":case"png":case"webp":case"gif":return{category:"image"};case"geojson":case"json":return{category:"text",language:"json"};case"text":case"go":case"md":return{category:"text"}}return{}}const ue=t=>({wrapper:(0,a.css)`
    display: flex;
    flex-direction: column;
    height: 100%;
  `,tableControlRowWrapper:(0,a.css)`
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: ${t.spacing(2)};
  `,tableWrapper:(0,a.css)`
    border: 1px solid ${t.colors.border.medium};
    height: 100%;
  `,uploadSpot:(0,a.css)`
    margin-left: ${t.spacing(2)};
  `,border:(0,a.css)`
    border: 1px solid ${t.colors.border.medium};
    padding: ${t.spacing(2)};
  `,img:(0,a.css)`
    max-width: 100%;
    // max-height: 147px;
    // fill: ${t.colors.text.primary};
  `,icon:(0,a.css)`
    // max-width: 100%;
    // max-height: 147px;
    // fill: ${t.colors.text.primary};
  `});var he=s(77093);function xe({listing:t,view:n}){const u=(0,$.of)(me);switch(n){case N.Config:return(0,e.jsx)("div",{children:"CONFIGURE?"});case N.Perms:return(0,e.jsx)("div",{children:"Permissions"})}return(0,e.jsx)("div",{className:u.tableWrapper,children:(0,e.jsx)(X.Ay,{children:({width:r,height:i})=>(0,e.jsx)("div",{style:{width:`${r}px`,height:`${i}px`},children:(0,e.jsx)(he.X,{height:i,width:r,data:t,noHeader:!1,showTypeIcons:!1,resizable:!1})})})})}const me=t=>({wrapper:(0,a.css)`
    display: flex;
    flex-direction: column;
    height: 100%;
  `,tableControlRowWrapper:(0,a.css)`
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: ${t.spacing(2)};
  `,tableWrapper:(0,a.css)`
    border: 1px solid ${t.colors.border.medium};
    height: 100%;
  `,uploadSpot:(0,a.css)`
    margin-left: ${t.spacing(2)};
  `,border:(0,a.css)`
    border: 1px solid ${t.colors.border.medium};
    padding: ${t.spacing(2)};
  `});var z=s(10860),pe=s(64149),ve=s(14186),ye=s(67647);function je({root:t,onPathChange:n}){const u=(0,$.of)(Ce),r=(0,T.A)((0,D.o)().getConfig),[i,l]=(0,f.useState)("");let w=location.pathname;w.endsWith("/")||(w+="/");const h=(0,f.useMemo)(()=>{let E=r.value??[];if(i?.length){const y=i.toLowerCase();E=E.filter(S=>{const g=S.config;return g.name.toLowerCase().indexOf(y)>=0||g.description.toLowerCase().indexOf(y)>=0})}const p=[],d=[];for(const y of E??[])y.config.underContentRoot?d.push(y):y.config.prefix!=="content"&&p.push(y);return{base:p,content:d}},[i,r]),m=(E,p)=>(0,e.jsx)(O.B,{direction:"column",children:p.map(d=>{const y=d.ready;return(0,e.jsxs)(z.Z,{href:y?`admin/storage/${E}${d.config.prefix}/`:void 0,children:[(0,e.jsx)(z.Z.Heading,{children:d.config.name}),(0,e.jsxs)(z.Z.Meta,{className:u.clickable,children:[d.config.description,d.config.git?.remote&&(0,e.jsx)("a",{href:d.config.git?.remote,children:d.config.git?.remote})]}),d.notice?.map(S=>(0,e.jsx)(M.F,{severity:S.severity,title:S.text},S.text)),(0,e.jsx)(z.Z.Tags,{className:u.clickable,children:(0,e.jsx)(O.B,{children:(0,e.jsx)(pe.L,{tags:Fe(d)})})}),(0,e.jsx)(z.Z.Figure,{className:u.clickable,children:(0,e.jsx)(Y.I,{name:Ee(d.config.type),size:"xxxl",className:u.secondaryTextColor})})]},d.config.prefix)})});return(0,e.jsxs)("div",{children:[(0,e.jsxs)("div",{className:"page-action-bar",children:[(0,e.jsx)(ve.I,{grow:!0,children:(0,e.jsx)(ye.Z,{placeholder:"Search Storage",value:i,onChange:l})}),(0,e.jsx)("div",{className:"page-action-bar__spacer"}),(0,e.jsx)(v.$n,{onClick:()=>n("",N.AddRoot),children:"Add Root"})]}),(0,e.jsx)("div",{children:m("",h.base)}),(0,e.jsxs)("div",{children:[(0,e.jsx)("h3",{children:"Content"}),m("content/",h.content)]})]})}function Ce(t){return{secondaryTextColor:(0,a.css)({color:t.colors.text.secondary}),clickable:(0,a.css)({pointerEvents:"none"})}}function Fe(t){const n=[];return t.builtin&&n.push("Builtin"),t.ready||n.push("Not ready"),n}function Ee(t){switch(t){case"git":return"code-branch";case"disk":return"folder-open";case"sql":return"database";default:return"folder-open"}}var $e=s(73546),Ne=s(96374);const Oe="image/jpg, image/jpeg, image/png, image/gif, image/webp";function Ae({setErrorMessages:t,setPath:n,path:u,fileNames:r}){const i=(0,$.of)(we),[l,w]=(0,f.useState)(void 0),[h,m]=(0,f.useState)(!1),[E,p]=(0,f.useState)(1),[d,y]=(0,f.useState)(!0);(0,f.useEffect)(()=>{p(x=>x+1)},[l]);const S=x=>{console.log("Uploaded: "+u),x.path?n(x.path):n(u)},g=async(x,I)=>{if(!x){t(["Please select a file."]);return}const L=await(0,D.o)().upload(u,x,I);L.status!==200?t([L.message]):S(L)},j=x=>{t([]);const I=x.currentTarget.files&&x.currentTarget.files.length>0&&x.currentTarget.files[0]?x.currentTarget.files[0]:void 0;I&&(w(I),(0,D.E)(I.name,r)?(m(!0),y(!0)):(m(!1),g(I,!1).then(C=>{})))},P=()=>{l&&(g(l,!0).then(x=>{}),y(!1))},B=()=>{w(void 0),m(!1),y(!1)};return(0,e.jsxs)(e.Fragment,{children:[(0,e.jsx)($e.e,{accept:Oe,onFileUpload:j,className:i.uploadButton,children:"Upload"},E),l&&h&&(0,e.jsx)(Ne.u,{isOpen:d,body:(0,e.jsxs)("div",{children:[(0,e.jsx)("p",{children:l?.name}),(0,e.jsx)("p",{children:"A file with this name already exists."}),(0,e.jsx)("p",{children:"What would you like to do?"})]}),title:"This file already exists",confirmText:"Replace",onConfirm:P,onDismiss:B})]})}const we=t=>({uploadButton:(0,a.css)`
    margin-right: ${t.spacing(2)};
  `}),Pe=/^[a-z\d!\-_.*'() ]+$/,k=256,Se=t=>{const n=t.lastIndexOf("/");return n<1?"":t.substring(0,n)};function De(t){const n=(0,$.of)(Ie),u=(0,q.C)("storage"),{path:r=""}=(0,U.g)(),i=t.queryParams.view??N.Data,l=(g,j)=>{let P=("/admin/storage/"+g).replace("//","/");j&&j!==N.Data&&(P+="?view="+j),H.Ny.push(P)},[w,h]=(0,f.useState)(!1),[m,E]=(0,f.useState)([]),p=(0,T.A)(()=>(0,D.o)().list(r).then(g=>{if(g){const j=g.fields[0];g.fields[0]={...j,getLinks:P=>{const B=j.values[P.valueRowIndex??0],x=r+"/"+B;return[{title:`Open ${B}`,href:`/admin/storage/${x}`,target:"_self",origin:j,onClick:()=>{l(x)}}]}}}return g}),[r]),d=(0,f.useMemo)(()=>{let g=r?.indexOf("/")<0;if(p.value)if(p.value.length===1){const P=p.value.fields[0].values[0];g=!r.endsWith(P)}else g=!0;return g},[r,p]),y=(0,f.useMemo)(()=>p.value?.fields?.find(g=>g.name==="name")?.values.filter(g=>typeof g=="string")??[],[p]),S=()=>{const g=!r?.length||r==="/";switch(i){case N.AddRoot:return g?(0,e.jsx)(ee,{onPathChange:l}):(l(""),(0,e.jsx)(b.y,{}))}const j=p.value;if(!(0,K.ci)(j))return(0,e.jsx)(e.Fragment,{});if(g)return(0,e.jsx)(je,{root:j,onPathChange:l});const P=[{what:N.Data,text:"Data"}];r.indexOf("/")<0&&P.push({what:N.Config,text:"Configure"}),d||P.push({what:N.History,text:"History"});const B=d&&(r.startsWith("resources")||r.startsWith("content")),x=r.startsWith("resources/")||r.startsWith("content/"),I=()=>(0,e.jsx)("div",{className:n.errorAlert,children:(0,e.jsx)(M.F,{title:"Upload failed",severity:"error",onRemove:L,children:m.map(C=>(0,e.jsx)("div",{children:C},C))})}),L=()=>{E([])};return(0,e.jsxs)("div",{className:n.wrapper,children:[(0,e.jsxs)(o.a,{display:"flex",justifyContent:"space-between",width:"100%",height:3,children:[(0,e.jsx)(te,{pathName:r,onPathChange:l,rootIcon:(0,c.Uo)(u.node.icon??"")}),(0,e.jsxs)(O.B,{children:[B&&(0,e.jsxs)(e.Fragment,{children:[(0,e.jsx)(Ae,{path:r,setErrorMessages:E,fileNames:y,setPath:l}),(0,e.jsx)(v.$n,{onClick:()=>h(!0),children:"New Folder"})]}),x&&(0,e.jsx)(v.$n,{variant:"destructive",onClick:()=>{const C=d?"Are you sure you want to delete this folder and all its contents?":"Are you sure you want to delete this file?",W=Se(r);V.A.publish(new _.bY({title:`Delete ${d?"folder":"file"}`,text:C,icon:"trash-alt",yesText:"Delete",onConfirm:()=>(0,D.o)().delete({path:r,isFolder:d}).then(()=>{l(W)})}))},children:"Delete"})]})]}),m.length>0&&I(),(0,e.jsx)(F.U,{children:P.map(C=>(0,e.jsx)(A.o,{label:C.text,active:C.what===i,onChangeTab:()=>l(r,C.what)},C.what))}),d?(0,e.jsx)(xe,{listing:j,view:i}):(0,e.jsx)(ge,{path:r,listing:j,onPathChange:l,view:i}),w&&(0,e.jsx)(ie,{onSubmit:async({folderName:C})=>{const W=`${r}/${C}`;typeof(await(0,D.o)().createFolder(W))?.error!="string"&&(l(W),h(!1))},onDismiss:()=>{h(!1)},validate:C=>{const W=C.toLowerCase();return(0,D.E)(C,y)?"A file or a folder with the same name already exists":Pe.test(W)?C.length>k?`Name is too long, maximum length: ${k} characters`:!0:"Name contains illegal characters"}})]})};return(0,e.jsx)(G.YW,{navModel:u,children:(0,e.jsx)(G.YW.Contents,{isLoading:p.loading,children:S()})})}const Ie=t=>({wrapper:(0,a.css)`
    display: flex;
    flex-direction: column;
    height: 100%;
  `,tableControlRowWrapper:(0,a.css)`
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: ${t.spacing(2)};
  `,tableWrapper:(0,a.css)`
    border: 1px solid ${t.colors.border.medium};
    height: 100%;
  `,border:(0,a.css)`
    border: 1px solid ${t.colors.border.medium};
    padding: ${t.spacing(2)};
  `,errorAlert:(0,a.css)`
    padding-top: 20px;
  `,uploadButton:(0,a.css)`
    margin-right: ${t.spacing(2)};
  `})},53652:(Z,R,s)=>{s.d(R,{E:()=>H,o:()=>b});var e=s(89667),a=s(57875),f=s(17172),U=s(32264),T=s(27677);class K{constructor(){}async get(o){const c=`api/storage/read/${o}`.replace("//","/");return(0,f.AI)().get(c)}async list(o){let c="api/storage/list/";o&&(c+=o+"/");const O=await(0,f.AI)().get(c);if(O?.data){const v=(0,e.or)(O);for(const F of v.fields)F.display=(0,a.J)({field:F,theme:U.$.theme2});return v}}async createFolder(o){const c=await(0,f.AI)().post("/api/storage/createFolder",JSON.stringify({path:o}));return c.success?{}:{error:c.message??"unknown error"}}async deleteFolder(o){const c=await(0,f.AI)().post("/api/storage/deleteFolder",JSON.stringify(o));return c.success?{}:{error:c.message??"unknown error"}}async deleteFile(o){const c=await(0,f.AI)().post(`/api/storage/delete/${o.path}`);return c.success?{}:{error:c.message??"unknown error"}}async delete(o){return o.isFolder?this.deleteFolder({path:o.path,force:!0}):this.deleteFile({path:o.path})}async upload(o,c,O){const v=new FormData;v.append("folder",o),v.append("file",c),v.append("overwriteExistingFile",String(O));const F=await fetch("/api/storage/upload",{method:"POST",body:v});let A=await F.json();return A||(A={}),A.status=F.status,A.statusText=F.statusText,F.status!==200&&!A.err&&(A.err=!0),A}async write(o,c){return T.IB.post(`/api/storage/write/${o}`,c)}async getConfig(){return(0,f.AI)().get("/api/storage/config")}async getOptions(o){return(0,f.AI)().get(`/api/storage/options/${o}`)}}function H(M,o){const O=M.toLowerCase().trim();return o.map(F=>F.trim().toLowerCase()).includes(O)}let $;function b(){return $||($=new K),$}}}]);

//# sourceMappingURL=StoragePage.a1dc7657c9f9b88501d7.js.map
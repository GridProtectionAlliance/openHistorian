"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[6938],{20582:(l,A,t)=>{t.r(A),t.d(A,{getCallName:()=>E,getCslTypeNameFromClrType:()=>d,getCurrentCommandRange:()=>s,getEntityDataTypeFromCslType:()=>u,getExpression:()=>o,getInputParametersAsCslString:()=>n,getKustoWorker:()=>T,kustoDefaults:()=>y,themeNames:()=>P});var i=t(97396);function s(p,B){for(var f=B.lineNumber-1,v=p.getModel().getLinesContent(),O=0,M=[],D=0;D<v.length;D++){var U=v[D].trim()==="";if(U?M.push({commandOrdinal:O++,lineNumber:D}):M.push({commandOrdinal:O,lineNumber:D}),D>f&&O>M[f].commandOrdinal)break}var K=M[f].commandOrdinal,b=M.filter(function(w){return w.commandOrdinal===K}),S=b[0].lineNumber+1,L=b[b.length-1].lineNumber+1,k=v[L-1].length+1;return new i.Range(S,1,L,k)}function r(p){var B=Object.getPrototypeOf(p);B.getCurrentCommandRange=function(f){s(this,f)}}var a=function(){function p(B){var f=this;this.editor=B,this.disposables=[],this.decorations=[],this.editor.onDidChangeCursorSelection(function(v){f.editor.getModel().getLanguageId()==="kusto"&&f.highlightCommandUnderCursor(v)})}return p.prototype.getId=function(){return p.ID},p.prototype.dispose=function(){this.disposables.forEach(function(B){return B.dispose()})},p.prototype.highlightCommandUnderCursor=function(B){if(!B.selection.isEmpty()){this.decorations=this.editor.deltaDecorations(this.decorations,[]);return}var f=s(this.editor,B.selection.getStartPosition()),v=[{range:f,options:p.CURRENT_COMMAND_HIGHLIGHT}];this.decorations=this.editor.deltaDecorations(this.decorations,v)},p.ID="editor.contrib.kustoCommandHighlighter",p.CURRENT_COMMAND_HIGHLIGHT={className:"selectionHighlight"},p}();const e=a;var _=function(){function p(B){var f=this;this.editor=B,this.actionAdded=!1,B.onDidChangeCursorSelection(function(v){f.editor.getModel().getLanguageId()==="kusto"&&(f.actionAdded||(B.addAction({id:"editor.action.kusto.formatCurrentCommand",label:"Format Command Under Cursor",keybindings:[i.KeyMod.chord(i.KeyMod.CtrlCmd|i.KeyCode.KeyK,i.KeyMod.CtrlCmd|i.KeyCode.KeyF)],run:function(O){B.trigger("KustoCommandFormatter","editor.action.formatSelection",null)},contextMenuGroupId:"1_modification"}),f.actionAdded=!0))})}return p}();const C=_;var c={"System.SByte":"bool","System.Byte":"uint8","System.Int16":"int16","System.UInt16":"uint16","System.Int32":"int","System.UInt32":"uint","System.Int64":"long","System.UInt64":"ulong","System.String":"string","System.Single":"float","System.Double":"real","System.DateTime":"datetime","System.TimeSpan":"timespan","System.Guid":"guid","System.Boolean":"bool","Newtonsoft.Json.Linq.JArray":"dynamic","Newtonsoft.Json.Linq.JObject":"dynamic","Newtonsoft.Json.Linq.JToken":"dynamic","System.Object":"dynamic","System.Data.SqlTypes.SqlDecimal":"decimal"},d=function(p){return c[p]||p},m={object:"Object",bool:"Boolean",uint8:"Byte",int16:"Int16",uint16:"UInt16",int:"Int32",uint:"UInt32",long:"Int64",ulong:"UInt64",float:"Single",real:"Double",decimal:"Decimal",datetime:"DateTime",string:"String",dynamic:"Dynamic",timespan:"TimeSpan"},u=function(p){return m[p]||p},E=function(p){return"".concat(p.name,"(").concat(p.inputParameters.map(function(B){return"{".concat(B.name,"}")}).join(","),")")},o=function(p){return"let ".concat(p.name," = ").concat(n(p.inputParameters)," ").concat(p.body)},n=function(p){return"(".concat(p.map(function(B){return g(B)}).join(","),")")},g=function(p){if(p.columns&&p.columns.length>0){var B=p.columns.map(function(f){return"".concat(f.name,":").concat(f.cslType||d(f.type))}).join(",");return"".concat(p.name,":").concat(B===""?"*":B)}else return"".concat(p.name,":").concat(p.cslType||d(p.type))},h=function(){function p(B){this._onDidChange=new i.Emitter,this.setLanguageSettings(B),this._workerMaxIdleTime=0}return Object.defineProperty(p.prototype,"onDidChange",{get:function(){return this._onDidChange.event},enumerable:!1,configurable:!0}),Object.defineProperty(p.prototype,"languageSettings",{get:function(){return this._languageSettings},enumerable:!1,configurable:!0}),p.prototype.setLanguageSettings=function(B){this._languageSettings=B||Object.create(null),this._onDidChange.fire(this)},p.prototype.setMaximumWorkerIdleTime=function(B){this._workerMaxIdleTime=B},p.prototype.getWorkerMaxIdleTime=function(){return this._workerMaxIdleTime},p}(),x={includeControlCommands:!0,newlineAfterPipe:!0,openSuggestionDialogAfterPreviousSuggestionAccepted:!0,useIntellisenseV2:!0,useSemanticColorization:!0,useTokenColorization:!1,enableHover:!0,formatter:{indentationSize:4,pipeOperatorStyle:"Smart"},syntaxErrorAsMarkDown:{enableSyntaxErrorAsMarkDown:!1},enableQueryWarnings:!1,enableQuerySuggestions:!1,disabledDiagnosticCodes:[],quickFixCodeActions:["Change to","FixAll"],enableQuickFixes:!1,completionOptions:{includeExtendedSyntax:!1}};function T(){return new Promise(function(p,B){I(function(f){f.getKustoWorker().then(p,B)})})}function I(p){t.e(2405).then(t.bind(t,92405)).then(p)}var y=new h(x),P={light:"kusto-light",dark:"kusto-dark",dark2:"kusto-dark2"};i.languages.onLanguage("kusto",function(){I(function(p){return p.setupMode(y,i)})}),i.languages.register({id:"kusto",extensions:[".csl",".kql"]}),i.editor.defineTheme(P.light,{base:"vs",inherit:!0,rules:[{token:"comment",foreground:"008000"},{token:"variable.predefined",foreground:"800080"},{token:"function",foreground:"0000FF"},{token:"operator.sql",foreground:"CC3700"},{token:"string",foreground:"B22222"},{token:"operator.scss",foreground:"0000FF"},{token:"variable",foreground:"C71585"},{token:"variable.parameter",foreground:"9932CC"},{token:"",foreground:"000000"},{token:"type",foreground:"0000FF"},{token:"tag",foreground:"0000FF"},{token:"annotation",foreground:"2B91AF"},{token:"keyword",foreground:"0000FF"},{token:"number",foreground:"191970"},{token:"annotation",foreground:"9400D3"},{token:"invalid",background:"cd3131"}],colors:{}}),i.editor.defineTheme(P.dark,{base:"vs-dark",inherit:!0,rules:[{token:"comment",foreground:"608B4E"},{token:"variable.predefined",foreground:"4ec9b0"},{token:"function",foreground:"dcdcaa"},{token:"operator.sql",foreground:"9cdcfe"},{token:"string",foreground:"ce9178"},{token:"operator.scss",foreground:"569cd6"},{token:"variable",foreground:"4ec9b0"},{token:"variable.parameter",foreground:"c586c0"},{token:"",foreground:"d4d4d4"},{token:"type",foreground:"569cd6"},{token:"tag",foreground:"569cd6"},{token:"annotation",foreground:"9cdcfe"},{token:"keyword",foreground:"569cd6"},{token:"number",foreground:"d7ba7d"},{token:"annotation",foreground:"b5cea8"},{token:"invalid",background:"cd3131"}],colors:{}}),i.editor.defineTheme(P.dark2,{base:"vs-dark",inherit:!0,rules:[],colors:{"editor.background":"#1B1A19","editorSuggestWidget.selectedBackground":"#004E8C"}}),i.editor.onDidCreateEditor(function(p){var B;!((B=window.MonacoEnvironment)===null||B===void 0)&&B.globalAPI&&r(p),new e(p),W(p)&&new C(p),j(p)});function j(p){p.onDidChangeCursorSelection(function(B){if(y&&y.languageSettings&&y.languageSettings.openSuggestionDialogAfterPreviousSuggestionAccepted){var f=B.source==="snippet"&&B.reason===i.editor.CursorChangeReason.NotSet;if(!f||p.getModel().getWordAtPosition(B.selection.getPosition())!==null)return;B.selection,setTimeout(function(){return p.trigger("monaco-kusto","editor.action.triggerSuggest",{})},10)}})}function W(p){return p.addAction!==void 0}var R={getCslTypeNameFromClrType:d,getCallName:E,getExpression:o,getInputParametersAsCslString:n,getEntityDataTypeFromCslType:u,kustoDefaults:y,getKustoWorker:T,getCurrentCommandRange:s,themeNames:P};i.languages.kusto=R},94566:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-action-bar {
	white-space: nowrap;
	height: 100%;
}

.monaco-action-bar .actions-container {
	display: flex;
	margin: 0 auto;
	padding: 0;
	height: 100%;
	width: 100%;
	align-items: center;
}

.monaco-action-bar.vertical .actions-container {
	display: inline-block;
}

.monaco-action-bar .action-item {
	display: block;
	align-items: center;
	justify-content: center;
	cursor: pointer;
	position: relative;  /* DO NOT REMOVE - this is the key to preventing the ghosting icon bug in Chrome 42 */
}

.monaco-action-bar .action-item.disabled {
	cursor: default;
}

.monaco-action-bar .action-item .icon,
.monaco-action-bar .action-item .codicon {
	display: block;
}

.monaco-action-bar .action-item .codicon {
	display: flex;
	align-items: center;
	width: 16px;
	height: 16px;
}

.monaco-action-bar .action-label {
	font-size: 11px;
	padding: 3px;
	border-radius: 5px;
}

.monaco-action-bar .action-item.disabled .action-label,
.monaco-action-bar .action-item.disabled .action-label::before,
.monaco-action-bar .action-item.disabled .action-label:hover {
	opacity: 0.6;
}

/* Vertical actions */

.monaco-action-bar.vertical {
	text-align: left;
}

.monaco-action-bar.vertical .action-item {
	display: block;
}

.monaco-action-bar.vertical .action-label.separator {
	display: block;
	border-bottom: 1px solid #bbb;
	padding-top: 1px;
	margin-left: .8em;
	margin-right: .8em;
}

.monaco-action-bar .action-item .action-label.separator {
	width: 1px;
	height: 16px;
	margin: 5px 4px !important;
	cursor: default;
	min-width: 1px;
	padding: 0;
	background-color: #bbb;
}

.secondary-actions .monaco-action-bar .action-label {
	margin-left: 6px;
}

/* Action Items */
.monaco-action-bar .action-item.select-container {
	overflow: hidden; /* somehow the dropdown overflows its container, we prevent it here to not push */
	flex: 1;
	max-width: 170px;
	min-width: 60px;
	display: flex;
	align-items: center;
	justify-content: center;
	margin-right: 10px;
}

.monaco-action-bar .action-item.action-dropdown-item {
	display: flex;
}

.monaco-action-bar .action-item.action-dropdown-item > .action-label {
	margin-right: 1px;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/actionbar/actionbar.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,mBAAmB;CACnB,YAAY;AACb;;AAEA;CACC,aAAa;CACb,cAAc;CACd,UAAU;CACV,YAAY;CACZ,WAAW;CACX,mBAAmB;AACpB;;AAEA;CACC,qBAAqB;AACtB;;AAEA;CACC,cAAc;CACd,mBAAmB;CACnB,uBAAuB;CACvB,eAAe;CACf,kBAAkB,GAAG,qFAAqF;AAC3G;;AAEA;CACC,eAAe;AAChB;;AAEA;;CAEC,cAAc;AACf;;AAEA;CACC,aAAa;CACb,mBAAmB;CACnB,WAAW;CACX,YAAY;AACb;;AAEA;CACC,eAAe;CACf,YAAY;CACZ,kBAAkB;AACnB;;AAEA;;;CAGC,YAAY;AACb;;AAEA,qBAAqB;;AAErB;CACC,gBAAgB;AACjB;;AAEA;CACC,cAAc;AACf;;AAEA;CACC,cAAc;CACd,6BAA6B;CAC7B,gBAAgB;CAChB,iBAAiB;CACjB,kBAAkB;AACnB;;AAEA;CACC,UAAU;CACV,YAAY;CACZ,0BAA0B;CAC1B,eAAe;CACf,cAAc;CACd,UAAU;CACV,sBAAsB;AACvB;;AAEA;CACC,gBAAgB;AACjB;;AAEA,iBAAiB;AACjB;CACC,gBAAgB,EAAE,iFAAiF;CACnG,OAAO;CACP,gBAAgB;CAChB,eAAe;CACf,aAAa;CACb,mBAAmB;CACnB,uBAAuB;CACvB,kBAAkB;AACnB;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,iBAAiB;AAClB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-action-bar {
	white-space: nowrap;
	height: 100%;
}

.monaco-action-bar .actions-container {
	display: flex;
	margin: 0 auto;
	padding: 0;
	height: 100%;
	width: 100%;
	align-items: center;
}

.monaco-action-bar.vertical .actions-container {
	display: inline-block;
}

.monaco-action-bar .action-item {
	display: block;
	align-items: center;
	justify-content: center;
	cursor: pointer;
	position: relative;  /* DO NOT REMOVE - this is the key to preventing the ghosting icon bug in Chrome 42 */
}

.monaco-action-bar .action-item.disabled {
	cursor: default;
}

.monaco-action-bar .action-item .icon,
.monaco-action-bar .action-item .codicon {
	display: block;
}

.monaco-action-bar .action-item .codicon {
	display: flex;
	align-items: center;
	width: 16px;
	height: 16px;
}

.monaco-action-bar .action-label {
	font-size: 11px;
	padding: 3px;
	border-radius: 5px;
}

.monaco-action-bar .action-item.disabled .action-label,
.monaco-action-bar .action-item.disabled .action-label::before,
.monaco-action-bar .action-item.disabled .action-label:hover {
	opacity: 0.6;
}

/* Vertical actions */

.monaco-action-bar.vertical {
	text-align: left;
}

.monaco-action-bar.vertical .action-item {
	display: block;
}

.monaco-action-bar.vertical .action-label.separator {
	display: block;
	border-bottom: 1px solid #bbb;
	padding-top: 1px;
	margin-left: .8em;
	margin-right: .8em;
}

.monaco-action-bar .action-item .action-label.separator {
	width: 1px;
	height: 16px;
	margin: 5px 4px !important;
	cursor: default;
	min-width: 1px;
	padding: 0;
	background-color: #bbb;
}

.secondary-actions .monaco-action-bar .action-label {
	margin-left: 6px;
}

/* Action Items */
.monaco-action-bar .action-item.select-container {
	overflow: hidden; /* somehow the dropdown overflows its container, we prevent it here to not push */
	flex: 1;
	max-width: 170px;
	min-width: 60px;
	display: flex;
	align-items: center;
	justify-content: center;
	margin-right: 10px;
}

.monaco-action-bar .action-item.action-dropdown-item {
	display: flex;
}

.monaco-action-bar .action-item.action-dropdown-item > .action-label {
	margin-right: 1px;
}
`],sourceRoot:""}]);const _=e},35038:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-aria-container {
	position: absolute; /* try to hide from window but not from screen readers */
	left:-999em;
}`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/aria/aria.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kBAAkB,EAAE,wDAAwD;CAC5E,WAAW;AACZ",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-aria-container {
	position: absolute; /* try to hide from window but not from screen readers */
	left:-999em;
}`],sourceRoot:""}]);const _=e},96499:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-text-button {
	box-sizing: border-box;
	display: flex;
	width: 100%;
	padding: 4px;
	text-align: center;
	cursor: pointer;
	justify-content: center;
	align-items: center;
}

.monaco-text-button:focus {
	outline-offset: 2px !important;
}

.monaco-text-button:hover {
	text-decoration: none !important;
}

.monaco-button.disabled:focus,
.monaco-button.disabled {
	opacity: 0.4 !important;
	cursor: default;
}

.monaco-text-button > .codicon {
	margin: 0 0.2em;
	color: inherit !important;
}

.monaco-button-dropdown {
	display: flex;
	cursor: pointer;
}

.monaco-button-dropdown.disabled {
	cursor: default;
}

.monaco-button-dropdown > .monaco-button:focus {
	outline-offset: -1px !important;
}

.monaco-button-dropdown.disabled > .monaco-button.disabled,
.monaco-button-dropdown.disabled > .monaco-button.disabled:focus,
.monaco-button-dropdown.disabled > .monaco-button-dropdown-separator {
	opacity: 0.4 !important;
}

.monaco-button-dropdown > .monaco-button.monaco-text-button {
	border-right-width: 0 !important;
}

.monaco-button-dropdown .monaco-button-dropdown-separator {
	padding: 4px 0;
	cursor: default;
}

.monaco-button-dropdown .monaco-button-dropdown-separator > div {
	height: 100%;
	width: 1px;
}

.monaco-button-dropdown > .monaco-button.monaco-dropdown-button {
	border-left-width: 0 !important;
}

.monaco-description-button {
	flex-direction: column;
}

.monaco-description-button .monaco-button-label {
	font-weight: 500;
}

.monaco-description-button .monaco-button-description {
	font-style: italic;
}

.monaco-description-button .monaco-button-label,
.monaco-description-button .monaco-button-description
{
	display: flex;
	justify-content: center;
	align-items: center;
}

.monaco-description-button .monaco-button-label > .codicon,
.monaco-description-button .monaco-button-description > .codicon
{
	margin: 0 0.2em;
	color: inherit !important;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/button/button.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,sBAAsB;CACtB,aAAa;CACb,WAAW;CACX,YAAY;CACZ,kBAAkB;CAClB,eAAe;CACf,uBAAuB;CACvB,mBAAmB;AACpB;;AAEA;CACC,8BAA8B;AAC/B;;AAEA;CACC,gCAAgC;AACjC;;AAEA;;CAEC,uBAAuB;CACvB,eAAe;AAChB;;AAEA;CACC,eAAe;CACf,yBAAyB;AAC1B;;AAEA;CACC,aAAa;CACb,eAAe;AAChB;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,+BAA+B;AAChC;;AAEA;;;CAGC,uBAAuB;AACxB;;AAEA;CACC,gCAAgC;AACjC;;AAEA;CACC,cAAc;CACd,eAAe;AAChB;;AAEA;CACC,YAAY;CACZ,UAAU;AACX;;AAEA;CACC,+BAA+B;AAChC;;AAEA;CACC,sBAAsB;AACvB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,kBAAkB;AACnB;;AAEA;;;CAGC,aAAa;CACb,uBAAuB;CACvB,mBAAmB;AACpB;;AAEA;;;CAGC,eAAe;CACf,yBAAyB;AAC1B",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-text-button {
	box-sizing: border-box;
	display: flex;
	width: 100%;
	padding: 4px;
	text-align: center;
	cursor: pointer;
	justify-content: center;
	align-items: center;
}

.monaco-text-button:focus {
	outline-offset: 2px !important;
}

.monaco-text-button:hover {
	text-decoration: none !important;
}

.monaco-button.disabled:focus,
.monaco-button.disabled {
	opacity: 0.4 !important;
	cursor: default;
}

.monaco-text-button > .codicon {
	margin: 0 0.2em;
	color: inherit !important;
}

.monaco-button-dropdown {
	display: flex;
	cursor: pointer;
}

.monaco-button-dropdown.disabled {
	cursor: default;
}

.monaco-button-dropdown > .monaco-button:focus {
	outline-offset: -1px !important;
}

.monaco-button-dropdown.disabled > .monaco-button.disabled,
.monaco-button-dropdown.disabled > .monaco-button.disabled:focus,
.monaco-button-dropdown.disabled > .monaco-button-dropdown-separator {
	opacity: 0.4 !important;
}

.monaco-button-dropdown > .monaco-button.monaco-text-button {
	border-right-width: 0 !important;
}

.monaco-button-dropdown .monaco-button-dropdown-separator {
	padding: 4px 0;
	cursor: default;
}

.monaco-button-dropdown .monaco-button-dropdown-separator > div {
	height: 100%;
	width: 1px;
}

.monaco-button-dropdown > .monaco-button.monaco-dropdown-button {
	border-left-width: 0 !important;
}

.monaco-description-button {
	flex-direction: column;
}

.monaco-description-button .monaco-button-label {
	font-weight: 500;
}

.monaco-description-button .monaco-button-description {
	font-style: italic;
}

.monaco-description-button .monaco-button-label,
.monaco-description-button .monaco-button-description
{
	display: flex;
	justify-content: center;
	align-items: center;
}

.monaco-description-button .monaco-button-label > .codicon,
.monaco-description-button .monaco-button-description > .codicon
{
	margin: 0 0.2em;
	color: inherit !important;
}
`],sourceRoot:""}]);const _=e},714:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.codicon-wrench-subaction {
	opacity: 0.5;
}

@keyframes codicon-spin {
	100% {
		transform:rotate(360deg);
	}
}

.codicon-sync.codicon-modifier-spin,
.codicon-loading.codicon-modifier-spin,
.codicon-gear.codicon-modifier-spin,
.codicon-notebook-state-executing.codicon-modifier-spin {
	/* Use steps to throttle FPS to reduce CPU usage */
	animation: codicon-spin 1.5s steps(30) infinite;
}

.codicon-modifier-disabled {
	opacity: 0.4;
}

/* custom speed & easing for loading icon */
.codicon-loading,
.codicon-tree-item-loading::before {
	animation-duration: 1s !important;
	animation-timing-function: cubic-bezier(0.53, 0.21, 0.29, 0.67) !important;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/codicons/codicon/codicon-modifiers.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,YAAY;AACb;;AAEA;CACC;EACC,wBAAwB;CACzB;AACD;;AAEA;;;;CAIC,kDAAkD;CAClD,+CAA+C;AAChD;;AAEA;CACC,YAAY;AACb;;AAEA,2CAA2C;AAC3C;;CAEC,iCAAiC;CACjC,0EAA0E;AAC3E",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.codicon-wrench-subaction {
	opacity: 0.5;
}

@keyframes codicon-spin {
	100% {
		transform:rotate(360deg);
	}
}

.codicon-sync.codicon-modifier-spin,
.codicon-loading.codicon-modifier-spin,
.codicon-gear.codicon-modifier-spin,
.codicon-notebook-state-executing.codicon-modifier-spin {
	/* Use steps to throttle FPS to reduce CPU usage */
	animation: codicon-spin 1.5s steps(30) infinite;
}

.codicon-modifier-disabled {
	opacity: 0.4;
}

/* custom speed & easing for loading icon */
.codicon-loading,
.codicon-tree-item-loading::before {
	animation-duration: 1s !important;
	animation-timing-function: cubic-bezier(0.53, 0.21, 0.29, 0.67) !important;
}
`],sourceRoot:""}]);const _=e},12171:(l,A,t)=>{t.d(A,{A:()=>m});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=t(4417),_=t.n(e),C=new URL(t(18880),t.b),c=a()(s()),d=_()(C);c.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

@font-face {
	font-family: "codicon";
	font-display: block;
	src: url(${d}) format("truetype");
}

.codicon[class*='codicon-'] {
	font: normal normal normal 16px/1 codicon;
	display: inline-block;
	text-decoration: none;
	text-rendering: auto;
	text-align: center;
	text-transform: none;
	-webkit-font-smoothing: antialiased;
	-moz-osx-font-smoothing: grayscale;
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

/* icon rules are dynamically created by the platform theme service (see iconsStyleSheet.ts) */
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/codicons/codicon/codicon.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,sBAAsB;CACtB,mBAAmB;CACnB,+DAA0C;AAC3C;;AAEA;CACC,yCAAyC;CACzC,qBAAqB;CACrB,qBAAqB;CACrB,oBAAoB;CACpB,kBAAkB;CAClB,oBAAoB;CACpB,mCAAmC;CACnC,kCAAkC;CAClC,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;AACtB;;AAEA,8FAA8F",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

@font-face {
	font-family: "codicon";
	font-display: block;
	src: url(./codicon.ttf) format("truetype");
}

.codicon[class*='codicon-'] {
	font: normal normal normal 16px/1 codicon;
	display: inline-block;
	text-decoration: none;
	text-rendering: auto;
	text-align: center;
	text-transform: none;
	-webkit-font-smoothing: antialiased;
	-moz-osx-font-smoothing: grayscale;
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

/* icon rules are dynamically created by the platform theme service (see iconsStyleSheet.ts) */
`],sourceRoot:""}]);const m=c},8970:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.context-view {
	position: absolute;
}

.context-view.fixed {
	all: initial;
	font-family: inherit;
	font-size: 13px;
	position: fixed;
	color: inherit;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/contextview/contextview.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kBAAkB;AACnB;;AAEA;CACC,YAAY;CACZ,oBAAoB;CACpB,eAAe;CACf,eAAe;CACf,cAAc;AACf",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.context-view {
	position: absolute;
}

.context-view.fixed {
	all: initial;
	font-family: inherit;
	font-size: 13px;
	position: fixed;
	color: inherit;
}
`],sourceRoot:""}]);const _=e},81684:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-count-badge {
	padding: 3px 6px;
	border-radius: 11px;
	font-size: 11px;
	min-width: 18px;
	min-height: 18px;
	line-height: 11px;
	font-weight: normal;
	text-align: center;
	display: inline-block;
	box-sizing: border-box;
}

.monaco-count-badge.long {
	padding: 2px 3px;
	border-radius: 2px;
	min-height: auto;
	line-height: normal;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/countBadge/countBadge.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,gBAAgB;CAChB,mBAAmB;CACnB,eAAe;CACf,eAAe;CACf,gBAAgB;CAChB,iBAAiB;CACjB,mBAAmB;CACnB,kBAAkB;CAClB,qBAAqB;CACrB,sBAAsB;AACvB;;AAEA;CACC,gBAAgB;CAChB,kBAAkB;CAClB,gBAAgB;CAChB,mBAAmB;AACpB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-count-badge {
	padding: 3px 6px;
	border-radius: 11px;
	font-size: 11px;
	min-width: 18px;
	min-height: 18px;
	line-height: 11px;
	font-weight: normal;
	text-align: center;
	display: inline-block;
	box-sizing: border-box;
}

.monaco-count-badge.long {
	padding: 2px 3px;
	border-radius: 2px;
	min-height: auto;
	line-height: normal;
}
`],sourceRoot:""}]);const _=e},8474:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
/* ---------- Find input ---------- */

.monaco-findInput {
	position: relative;
}

.monaco-findInput .monaco-inputbox {
	font-size: 13px;
	width: 100%;
}

.monaco-findInput > .controls {
	position: absolute;
	top: 3px;
	right: 2px;
}

.vs .monaco-findInput.disabled {
	background-color: #E1E1E1;
}

/* Theming */
.vs-dark .monaco-findInput.disabled {
	background-color: #333;
}

/* Highlighting */
.monaco-findInput.highlight-0 .controls,
.hc-light .monaco-findInput.highlight-0 .controls {
	animation: monaco-findInput-highlight-0 100ms linear 0s;
}

.monaco-findInput.highlight-1 .controls,
.hc-light .monaco-findInput.highlight-1 .controls {
	animation: monaco-findInput-highlight-1 100ms linear 0s;
}

.hc-black .monaco-findInput.highlight-0 .controls,
.vs-dark  .monaco-findInput.highlight-0 .controls {
	animation: monaco-findInput-highlight-dark-0 100ms linear 0s;
}

.hc-black .monaco-findInput.highlight-1 .controls,
.vs-dark  .monaco-findInput.highlight-1 .controls {
	animation: monaco-findInput-highlight-dark-1 100ms linear 0s;
}

@keyframes monaco-findInput-highlight-0 {
	0% { background: rgba(253, 255, 0, 0.8); }
	100% { background: transparent; }
}
@keyframes monaco-findInput-highlight-1 {
	0% { background: rgba(253, 255, 0, 0.8); }
	/* Made intentionally different such that the CSS minifier does not collapse the two animations into a single one*/
	99% { background: transparent; }
}

@keyframes monaco-findInput-highlight-dark-0 {
	0% { background: rgba(255, 255, 255, 0.44); }
	100% { background: transparent; }
}
@keyframes monaco-findInput-highlight-dark-1 {
	0% { background: rgba(255, 255, 255, 0.44); }
	/* Made intentionally different such that the CSS minifier does not collapse the two animations into a single one*/
	99% { background: transparent; }
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/findinput/findInput.css"],names:[],mappings:"AAAA;;;+FAG+F;AAC/F,qCAAqC;;AAErC;CACC,kBAAkB;AACnB;;AAEA;CACC,eAAe;CACf,WAAW;AACZ;;AAEA;CACC,kBAAkB;CAClB,QAAQ;CACR,UAAU;AACX;;AAEA;CACC,yBAAyB;AAC1B;;AAEA,YAAY;AACZ;CACC,sBAAsB;AACvB;;AAEA,iBAAiB;AACjB;;CAEC,uDAAuD;AACxD;;AAEA;;CAEC,uDAAuD;AACxD;;AAEA;;CAEC,4DAA4D;AAC7D;;AAEA;;CAEC,4DAA4D;AAC7D;;AAEA;CACC,KAAK,kCAAkC,EAAE;CACzC,OAAO,uBAAuB,EAAE;AACjC;AACA;CACC,KAAK,kCAAkC,EAAE;CACzC,kHAAkH;CAClH,MAAM,uBAAuB,EAAE;AAChC;;AAEA;CACC,KAAK,qCAAqC,EAAE;CAC5C,OAAO,uBAAuB,EAAE;AACjC;AACA;CACC,KAAK,qCAAqC,EAAE;CAC5C,kHAAkH;CAClH,MAAM,uBAAuB,EAAE;AAChC",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
/* ---------- Find input ---------- */

.monaco-findInput {
	position: relative;
}

.monaco-findInput .monaco-inputbox {
	font-size: 13px;
	width: 100%;
}

.monaco-findInput > .controls {
	position: absolute;
	top: 3px;
	right: 2px;
}

.vs .monaco-findInput.disabled {
	background-color: #E1E1E1;
}

/* Theming */
.vs-dark .monaco-findInput.disabled {
	background-color: #333;
}

/* Highlighting */
.monaco-findInput.highlight-0 .controls,
.hc-light .monaco-findInput.highlight-0 .controls {
	animation: monaco-findInput-highlight-0 100ms linear 0s;
}

.monaco-findInput.highlight-1 .controls,
.hc-light .monaco-findInput.highlight-1 .controls {
	animation: monaco-findInput-highlight-1 100ms linear 0s;
}

.hc-black .monaco-findInput.highlight-0 .controls,
.vs-dark  .monaco-findInput.highlight-0 .controls {
	animation: monaco-findInput-highlight-dark-0 100ms linear 0s;
}

.hc-black .monaco-findInput.highlight-1 .controls,
.vs-dark  .monaco-findInput.highlight-1 .controls {
	animation: monaco-findInput-highlight-dark-1 100ms linear 0s;
}

@keyframes monaco-findInput-highlight-0 {
	0% { background: rgba(253, 255, 0, 0.8); }
	100% { background: transparent; }
}
@keyframes monaco-findInput-highlight-1 {
	0% { background: rgba(253, 255, 0, 0.8); }
	/* Made intentionally different such that the CSS minifier does not collapse the two animations into a single one*/
	99% { background: transparent; }
}

@keyframes monaco-findInput-highlight-dark-0 {
	0% { background: rgba(255, 255, 255, 0.44); }
	100% { background: transparent; }
}
@keyframes monaco-findInput-highlight-dark-1 {
	0% { background: rgba(255, 255, 255, 0.44); }
	/* Made intentionally different such that the CSS minifier does not collapse the two animations into a single one*/
	99% { background: transparent; }
}
`],sourceRoot:""}]);const _=e},48134:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* ---------- Icon label ---------- */

.monaco-icon-label {
	display: flex; /* required for icons support :before rule */
	overflow: hidden;
	text-overflow: ellipsis;
}

.monaco-icon-label::before {

	/* svg icons rendered as background image */
	background-size: 16px;
	background-position: left center;
	background-repeat: no-repeat;
	padding-right: 6px;
	width: 16px;
	height: 22px;
	line-height: inherit !important;
	display: inline-block;

	/* fonts icons */
	-webkit-font-smoothing: antialiased;
	-moz-osx-font-smoothing: grayscale;
	vertical-align: top;

	flex-shrink: 0; /* fix for https://github.com/microsoft/vscode/issues/13787 */
}

.monaco-icon-label > .monaco-icon-label-container {
	min-width: 0;
	overflow: hidden;
	text-overflow: ellipsis;
	flex: 1;
}

.monaco-icon-label > .monaco-icon-label-container > .monaco-icon-name-container > .label-name {
	color: inherit;
	white-space: pre; /* enable to show labels that include multiple whitespaces */
}

.monaco-icon-label > .monaco-icon-label-container > .monaco-icon-name-container > .label-name > .label-separator {
	margin: 0 2px;
	opacity: 0.5;
}

.monaco-icon-label > .monaco-icon-label-container > .monaco-icon-description-container > .label-description {
	opacity: .7;
	margin-left: 0.5em;
	font-size: 0.9em;
	white-space: pre; /* enable to show labels that include multiple whitespaces */
}

.monaco-icon-label.nowrap > .monaco-icon-label-container > .monaco-icon-description-container > .label-description{
	white-space: nowrap
}

.vs .monaco-icon-label > .monaco-icon-label-container > .monaco-icon-description-container > .label-description {
	opacity: .95;
}

.monaco-icon-label.italic > .monaco-icon-label-container > .monaco-icon-name-container > .label-name,
.monaco-icon-label.italic > .monaco-icon-label-container > .monaco-icon-description-container > .label-description {
	font-style: italic;
}

.monaco-icon-label.deprecated {
	text-decoration: line-through;
	opacity: 0.66;
}

/* make sure apply italic font style to decorations as well */
.monaco-icon-label.italic::after {
	font-style: italic;
}

.monaco-icon-label.strikethrough > .monaco-icon-label-container > .monaco-icon-name-container > .label-name,
.monaco-icon-label.strikethrough > .monaco-icon-label-container > .monaco-icon-description-container > .label-description {
	text-decoration: line-through;
}

.monaco-icon-label::after {
	opacity: 0.75;
	font-size: 90%;
	font-weight: 600;
	margin: auto 16px 0 5px; /* https://github.com/microsoft/vscode/issues/113223 */
	text-align: center;
}

/* make sure selection color wins when a label is being selected */
.monaco-list:focus .selected .monaco-icon-label, /* list */
.monaco-list:focus .selected .monaco-icon-label::after
{
	color: inherit !important;
}

.monaco-list-row.focused.selected .label-description,
.monaco-list-row.selected .label-description {
	opacity: .8;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/iconLabel/iconlabel.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F,qCAAqC;;AAErC;CACC,aAAa,EAAE,4CAA4C;CAC3D,gBAAgB;CAChB,uBAAuB;AACxB;;AAEA;;CAEC,2CAA2C;CAC3C,qBAAqB;CACrB,gCAAgC;CAChC,4BAA4B;CAC5B,kBAAkB;CAClB,WAAW;CACX,YAAY;CACZ,+BAA+B;CAC/B,qBAAqB;;CAErB,gBAAgB;CAChB,mCAAmC;CACnC,kCAAkC;CAClC,mBAAmB;;CAEnB,cAAc,EAAE,6DAA6D;AAC9E;;AAEA;CACC,YAAY;CACZ,gBAAgB;CAChB,uBAAuB;CACvB,OAAO;AACR;;AAEA;CACC,cAAc;CACd,gBAAgB,EAAE,4DAA4D;AAC/E;;AAEA;CACC,aAAa;CACb,YAAY;AACb;;AAEA;CACC,WAAW;CACX,kBAAkB;CAClB,gBAAgB;CAChB,gBAAgB,EAAE,4DAA4D;AAC/E;;AAEA;CACC;AACD;;AAEA;CACC,YAAY;AACb;;AAEA;;CAEC,kBAAkB;AACnB;;AAEA;CACC,6BAA6B;CAC7B,aAAa;AACd;;AAEA,6DAA6D;AAC7D;CACC,kBAAkB;AACnB;;AAEA;;CAEC,6BAA6B;AAC9B;;AAEA;CACC,aAAa;CACb,cAAc;CACd,gBAAgB;CAChB,uBAAuB,EAAE,sDAAsD;CAC/E,kBAAkB;AACnB;;AAEA,kEAAkE;AAClE;;;CAGC,yBAAyB;AAC1B;;AAEA;;CAEC,WAAW;AACZ",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* ---------- Icon label ---------- */

.monaco-icon-label {
	display: flex; /* required for icons support :before rule */
	overflow: hidden;
	text-overflow: ellipsis;
}

.monaco-icon-label::before {

	/* svg icons rendered as background image */
	background-size: 16px;
	background-position: left center;
	background-repeat: no-repeat;
	padding-right: 6px;
	width: 16px;
	height: 22px;
	line-height: inherit !important;
	display: inline-block;

	/* fonts icons */
	-webkit-font-smoothing: antialiased;
	-moz-osx-font-smoothing: grayscale;
	vertical-align: top;

	flex-shrink: 0; /* fix for https://github.com/microsoft/vscode/issues/13787 */
}

.monaco-icon-label > .monaco-icon-label-container {
	min-width: 0;
	overflow: hidden;
	text-overflow: ellipsis;
	flex: 1;
}

.monaco-icon-label > .monaco-icon-label-container > .monaco-icon-name-container > .label-name {
	color: inherit;
	white-space: pre; /* enable to show labels that include multiple whitespaces */
}

.monaco-icon-label > .monaco-icon-label-container > .monaco-icon-name-container > .label-name > .label-separator {
	margin: 0 2px;
	opacity: 0.5;
}

.monaco-icon-label > .monaco-icon-label-container > .monaco-icon-description-container > .label-description {
	opacity: .7;
	margin-left: 0.5em;
	font-size: 0.9em;
	white-space: pre; /* enable to show labels that include multiple whitespaces */
}

.monaco-icon-label.nowrap > .monaco-icon-label-container > .monaco-icon-description-container > .label-description{
	white-space: nowrap
}

.vs .monaco-icon-label > .monaco-icon-label-container > .monaco-icon-description-container > .label-description {
	opacity: .95;
}

.monaco-icon-label.italic > .monaco-icon-label-container > .monaco-icon-name-container > .label-name,
.monaco-icon-label.italic > .monaco-icon-label-container > .monaco-icon-description-container > .label-description {
	font-style: italic;
}

.monaco-icon-label.deprecated {
	text-decoration: line-through;
	opacity: 0.66;
}

/* make sure apply italic font style to decorations as well */
.monaco-icon-label.italic::after {
	font-style: italic;
}

.monaco-icon-label.strikethrough > .monaco-icon-label-container > .monaco-icon-name-container > .label-name,
.monaco-icon-label.strikethrough > .monaco-icon-label-container > .monaco-icon-description-container > .label-description {
	text-decoration: line-through;
}

.monaco-icon-label::after {
	opacity: 0.75;
	font-size: 90%;
	font-weight: 600;
	margin: auto 16px 0 5px; /* https://github.com/microsoft/vscode/issues/113223 */
	text-align: center;
}

/* make sure selection color wins when a label is being selected */
.monaco-list:focus .selected .monaco-icon-label, /* list */
.monaco-list:focus .selected .monaco-icon-label::after
{
	color: inherit !important;
}

.monaco-list-row.focused.selected .label-description,
.monaco-list-row.selected .label-description {
	opacity: .8;
}
`],sourceRoot:""}]);const _=e},1366:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-inputbox {
	position: relative;
	display: block;
	padding: 0;
	box-sizing:	border-box;

	/* Customizable */
	font-size: inherit;
}

.monaco-inputbox.idle {
	border: 1px solid transparent;
}

.monaco-inputbox > .ibwrapper > .input,
.monaco-inputbox > .ibwrapper > .mirror {

	/* Customizable */
	padding: 4px;
}

.monaco-inputbox > .ibwrapper {
	position: relative;
	width: 100%;
	height: 100%;
}

.monaco-inputbox > .ibwrapper > .input {
	display: inline-block;
	box-sizing:	border-box;
	width: 100%;
	height: 100%;
	line-height: inherit;
	border: none;
	font-family: inherit;
	font-size: inherit;
	resize: none;
	color: inherit;
}

.monaco-inputbox > .ibwrapper > input {
	text-overflow: ellipsis;
}

.monaco-inputbox > .ibwrapper > textarea.input {
	display: block;
	-ms-overflow-style: none; /* IE 10+: hide scrollbars */
	scrollbar-width: none; /* Firefox: hide scrollbars */
	outline: none;
}

.monaco-inputbox > .ibwrapper > textarea.input::-webkit-scrollbar {
	display: none; /* Chrome + Safari: hide scrollbar */
}

.monaco-inputbox > .ibwrapper > textarea.input.empty {
	white-space: nowrap;
}

.monaco-inputbox > .ibwrapper > .mirror {
	position: absolute;
	display: inline-block;
	width: 100%;
	top: 0;
	left: 0;
	box-sizing: border-box;
	white-space: pre-wrap;
	visibility: hidden;
	word-wrap: break-word;
}

/* Context view */

.monaco-inputbox-container {
	text-align: right;
}

.monaco-inputbox-container .monaco-inputbox-message {
	display: inline-block;
	overflow: hidden;
	text-align: left;
	width: 100%;
	box-sizing:	border-box;
	padding: 0.4em;
	font-size: 12px;
	line-height: 17px;
	margin-top: -1px;
	word-wrap: break-word;
}

/* Action bar support */
.monaco-inputbox .monaco-action-bar {
	position: absolute;
	right: 2px;
	top: 4px;
}

.monaco-inputbox .monaco-action-bar .action-item {
	margin-left: 2px;
}

.monaco-inputbox .monaco-action-bar .action-item .codicon {
	background-repeat: no-repeat;
	width: 16px;
	height: 16px;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/inputbox/inputBox.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kBAAkB;CAClB,cAAc;CACd,UAAU;CACV,sBAAsB;;CAEtB,iBAAiB;CACjB,kBAAkB;AACnB;;AAEA;CACC,6BAA6B;AAC9B;;AAEA;;;CAGC,iBAAiB;CACjB,YAAY;AACb;;AAEA;CACC,kBAAkB;CAClB,WAAW;CACX,YAAY;AACb;;AAEA;CACC,qBAAqB;CACrB,sBAAsB;CACtB,WAAW;CACX,YAAY;CACZ,oBAAoB;CACpB,YAAY;CACZ,oBAAoB;CACpB,kBAAkB;CAClB,YAAY;CACZ,cAAc;AACf;;AAEA;CACC,uBAAuB;AACxB;;AAEA;CACC,cAAc;CACd,wBAAwB,EAAE,4BAA4B;CACtD,qBAAqB,EAAE,6BAA6B;CACpD,aAAa;AACd;;AAEA;CACC,aAAa,EAAE,oCAAoC;AACpD;;AAEA;CACC,mBAAmB;AACpB;;AAEA;CACC,kBAAkB;CAClB,qBAAqB;CACrB,WAAW;CACX,MAAM;CACN,OAAO;CACP,sBAAsB;CACtB,qBAAqB;CACrB,kBAAkB;CAClB,qBAAqB;AACtB;;AAEA,iBAAiB;;AAEjB;CACC,iBAAiB;AAClB;;AAEA;CACC,qBAAqB;CACrB,gBAAgB;CAChB,gBAAgB;CAChB,WAAW;CACX,sBAAsB;CACtB,cAAc;CACd,eAAe;CACf,iBAAiB;CACjB,gBAAgB;CAChB,qBAAqB;AACtB;;AAEA,uBAAuB;AACvB;CACC,kBAAkB;CAClB,UAAU;CACV,QAAQ;AACT;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,4BAA4B;CAC5B,WAAW;CACX,YAAY;AACb",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-inputbox {
	position: relative;
	display: block;
	padding: 0;
	box-sizing:	border-box;

	/* Customizable */
	font-size: inherit;
}

.monaco-inputbox.idle {
	border: 1px solid transparent;
}

.monaco-inputbox > .ibwrapper > .input,
.monaco-inputbox > .ibwrapper > .mirror {

	/* Customizable */
	padding: 4px;
}

.monaco-inputbox > .ibwrapper {
	position: relative;
	width: 100%;
	height: 100%;
}

.monaco-inputbox > .ibwrapper > .input {
	display: inline-block;
	box-sizing:	border-box;
	width: 100%;
	height: 100%;
	line-height: inherit;
	border: none;
	font-family: inherit;
	font-size: inherit;
	resize: none;
	color: inherit;
}

.monaco-inputbox > .ibwrapper > input {
	text-overflow: ellipsis;
}

.monaco-inputbox > .ibwrapper > textarea.input {
	display: block;
	-ms-overflow-style: none; /* IE 10+: hide scrollbars */
	scrollbar-width: none; /* Firefox: hide scrollbars */
	outline: none;
}

.monaco-inputbox > .ibwrapper > textarea.input::-webkit-scrollbar {
	display: none; /* Chrome + Safari: hide scrollbar */
}

.monaco-inputbox > .ibwrapper > textarea.input.empty {
	white-space: nowrap;
}

.monaco-inputbox > .ibwrapper > .mirror {
	position: absolute;
	display: inline-block;
	width: 100%;
	top: 0;
	left: 0;
	box-sizing: border-box;
	white-space: pre-wrap;
	visibility: hidden;
	word-wrap: break-word;
}

/* Context view */

.monaco-inputbox-container {
	text-align: right;
}

.monaco-inputbox-container .monaco-inputbox-message {
	display: inline-block;
	overflow: hidden;
	text-align: left;
	width: 100%;
	box-sizing:	border-box;
	padding: 0.4em;
	font-size: 12px;
	line-height: 17px;
	margin-top: -1px;
	word-wrap: break-word;
}

/* Action bar support */
.monaco-inputbox .monaco-action-bar {
	position: absolute;
	right: 2px;
	top: 4px;
}

.monaco-inputbox .monaco-action-bar .action-item {
	margin-left: 2px;
}

.monaco-inputbox .monaco-action-bar .action-item .codicon {
	background-repeat: no-repeat;
	width: 16px;
	height: 16px;
}
`],sourceRoot:""}]);const _=e},95422:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-keybinding {
	display: flex;
	align-items: center;
	line-height: 10px;
}

.monaco-keybinding > .monaco-keybinding-key {
	display: inline-block;
	border-style: solid;
	border-width: 1px;
	border-radius: 3px;
	vertical-align: middle;
	font-size: 11px;
	padding: 3px 5px;
	margin: 0 2px;
}

.monaco-keybinding > .monaco-keybinding-key:first-child {
	margin-left: 0;
}

.monaco-keybinding > .monaco-keybinding-key:last-child {
	margin-right: 0;
}

.monaco-keybinding > .monaco-keybinding-key-separator {
	display: inline-block;
}

.monaco-keybinding > .monaco-keybinding-key-chord-separator {
	width: 6px;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/keybindingLabel/keybindingLabel.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,aAAa;CACb,mBAAmB;CACnB,iBAAiB;AAClB;;AAEA;CACC,qBAAqB;CACrB,mBAAmB;CACnB,iBAAiB;CACjB,kBAAkB;CAClB,sBAAsB;CACtB,eAAe;CACf,gBAAgB;CAChB,aAAa;AACd;;AAEA;CACC,cAAc;AACf;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,qBAAqB;AACtB;;AAEA;CACC,UAAU;AACX",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-keybinding {
	display: flex;
	align-items: center;
	line-height: 10px;
}

.monaco-keybinding > .monaco-keybinding-key {
	display: inline-block;
	border-style: solid;
	border-width: 1px;
	border-radius: 3px;
	vertical-align: middle;
	font-size: 11px;
	padding: 3px 5px;
	margin: 0 2px;
}

.monaco-keybinding > .monaco-keybinding-key:first-child {
	margin-left: 0;
}

.monaco-keybinding > .monaco-keybinding-key:last-child {
	margin-right: 0;
}

.monaco-keybinding > .monaco-keybinding-key-separator {
	display: inline-block;
}

.monaco-keybinding > .monaco-keybinding-key-chord-separator {
	width: 6px;
}
`],sourceRoot:""}]);const _=e},44959:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-list {
	position: relative;
	height: 100%;
	width: 100%;
	white-space: nowrap;
}

.monaco-list.mouse-support {
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

.monaco-list > .monaco-scrollable-element {
	height: 100%;
}

.monaco-list-rows {
	position: relative;
	width: 100%;
	height: 100%;
}

.monaco-list.horizontal-scrolling .monaco-list-rows {
	width: auto;
	min-width: 100%;
}

.monaco-list-row {
	position: absolute;
	box-sizing: border-box;
	overflow: hidden;
	width: 100%;
}

.monaco-list.mouse-support .monaco-list-row {
	cursor: pointer;
	touch-action: none;
}

/* for OS X ballistic scrolling */
.monaco-list-row.scrolling {
	display: none !important;
}

/* Focus */
.monaco-list.element-focused,
.monaco-list.selection-single,
.monaco-list.selection-multiple {
	outline: 0 !important;
}

/* Dnd */
.monaco-drag-image {
	display: inline-block;
	padding: 1px 7px;
	border-radius: 10px;
	font-size: 12px;
	position: absolute;
	z-index: 1000;
}

/* Filter */

.monaco-list-type-filter-message {
	position: absolute;
	box-sizing: border-box;
	width: 100%;
	height: 100%;
	top: 0;
	left: 0;
	padding: 40px 1em 1em 1em;
	text-align: center;
	white-space: normal;
	opacity: 0.7;
	pointer-events: none;
}

.monaco-list-type-filter-message:empty {
	display: none;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/list/list.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kBAAkB;CAClB,YAAY;CACZ,WAAW;CACX,mBAAmB;AACpB;;AAEA;CACC,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;AACtB;;AAEA;CACC,YAAY;AACb;;AAEA;CACC,kBAAkB;CAClB,WAAW;CACX,YAAY;AACb;;AAEA;CACC,WAAW;CACX,eAAe;AAChB;;AAEA;CACC,kBAAkB;CAClB,sBAAsB;CACtB,gBAAgB;CAChB,WAAW;AACZ;;AAEA;CACC,eAAe;CACf,kBAAkB;AACnB;;AAEA,iCAAiC;AACjC;CACC,wBAAwB;AACzB;;AAEA,UAAU;AACV;;;CAGC,qBAAqB;AACtB;;AAEA,QAAQ;AACR;CACC,qBAAqB;CACrB,gBAAgB;CAChB,mBAAmB;CACnB,eAAe;CACf,kBAAkB;CAClB,aAAa;AACd;;AAEA,WAAW;;AAEX;CACC,kBAAkB;CAClB,sBAAsB;CACtB,WAAW;CACX,YAAY;CACZ,MAAM;CACN,OAAO;CACP,yBAAyB;CACzB,kBAAkB;CAClB,mBAAmB;CACnB,YAAY;CACZ,oBAAoB;AACrB;;AAEA;CACC,aAAa;AACd",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-list {
	position: relative;
	height: 100%;
	width: 100%;
	white-space: nowrap;
}

.monaco-list.mouse-support {
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

.monaco-list > .monaco-scrollable-element {
	height: 100%;
}

.monaco-list-rows {
	position: relative;
	width: 100%;
	height: 100%;
}

.monaco-list.horizontal-scrolling .monaco-list-rows {
	width: auto;
	min-width: 100%;
}

.monaco-list-row {
	position: absolute;
	box-sizing: border-box;
	overflow: hidden;
	width: 100%;
}

.monaco-list.mouse-support .monaco-list-row {
	cursor: pointer;
	touch-action: none;
}

/* for OS X ballistic scrolling */
.monaco-list-row.scrolling {
	display: none !important;
}

/* Focus */
.monaco-list.element-focused,
.monaco-list.selection-single,
.monaco-list.selection-multiple {
	outline: 0 !important;
}

/* Dnd */
.monaco-drag-image {
	display: inline-block;
	padding: 1px 7px;
	border-radius: 10px;
	font-size: 12px;
	position: absolute;
	z-index: 1000;
}

/* Filter */

.monaco-list-type-filter-message {
	position: absolute;
	box-sizing: border-box;
	width: 100%;
	height: 100%;
	top: 0;
	left: 0;
	padding: 40px 1em 1em 1em;
	text-align: center;
	white-space: normal;
	opacity: 0.7;
	pointer-events: none;
}

.monaco-list-type-filter-message:empty {
	display: none;
}
`],sourceRoot:""}]);const _=e},266:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-mouse-cursor-text {
	cursor: text;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/mouseCursor/mouseCursor.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,YAAY;AACb",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-mouse-cursor-text {
	cursor: text;
}
`],sourceRoot:""}]);const _=e},44978:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-progress-container {
	width: 100%;
	height: 5px;
	overflow: hidden; /* keep progress bit in bounds */
}

.monaco-progress-container .progress-bit {
	width: 2%;
	height: 5px;
	position: absolute;
	left: 0;
	display: none;
}

.monaco-progress-container.active .progress-bit {
	display: inherit;
}

.monaco-progress-container.discrete .progress-bit {
	left: 0;
	transition: width 100ms linear;
}

.monaco-progress-container.discrete.done .progress-bit {
	width: 100%;
}

.monaco-progress-container.infinite .progress-bit {
	animation-name: progress;
	animation-duration: 4s;
	animation-iteration-count: infinite;
	transform: translate3d(0px, 0px, 0px);
	animation-timing-function: linear;
}

.monaco-progress-container.infinite.infinite-long-running .progress-bit {
	/*
		The more smooth \`linear\` timing function can cause
		higher GPU consumption as indicated in
		https://github.com/microsoft/vscode/issues/97900 &
		https://github.com/microsoft/vscode/issues/138396
	*/
	animation-timing-function: steps(100);
}

/**
 * The progress bit has a width: 2% (1/50) of the parent container. The animation moves it from 0% to 100% of
 * that container. Since translateX is relative to the progress bit size, we have to multiple it with
 * its relative size to the parent container:
 * parent width: 5000%
 *    bit width: 100%
 * translateX should be as follow:
 *  50%: 5000% * 50% - 50% (set to center) = 2450%
 * 100%: 5000% * 100% - 100% (do not overflow) = 4900%
 */
@keyframes progress { from { transform: translateX(0%) scaleX(1) } 50% { transform: translateX(2500%) scaleX(3) } to { transform: translateX(4900%) scaleX(1) } }
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/progressbar/progressbar.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,WAAW;CACX,WAAW;CACX,gBAAgB,EAAE,gCAAgC;AACnD;;AAEA;CACC,SAAS;CACT,WAAW;CACX,kBAAkB;CAClB,OAAO;CACP,aAAa;AACd;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,OAAO;CACP,8BAA8B;AAC/B;;AAEA;CACC,WAAW;AACZ;;AAEA;CACC,wBAAwB;CACxB,sBAAsB;CACtB,mCAAmC;CACnC,qCAAqC;CACrC,iCAAiC;AAClC;;AAEA;CACC;;;;;EAKC;CACD,qCAAqC;AACtC;;AAEA;;;;;;;;;EASE;AACF,sBAAsB,OAAO,oCAAoC,EAAE,MAAM,uCAAuC,EAAE,KAAK,uCAAuC,EAAE",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-progress-container {
	width: 100%;
	height: 5px;
	overflow: hidden; /* keep progress bit in bounds */
}

.monaco-progress-container .progress-bit {
	width: 2%;
	height: 5px;
	position: absolute;
	left: 0;
	display: none;
}

.monaco-progress-container.active .progress-bit {
	display: inherit;
}

.monaco-progress-container.discrete .progress-bit {
	left: 0;
	transition: width 100ms linear;
}

.monaco-progress-container.discrete.done .progress-bit {
	width: 100%;
}

.monaco-progress-container.infinite .progress-bit {
	animation-name: progress;
	animation-duration: 4s;
	animation-iteration-count: infinite;
	transform: translate3d(0px, 0px, 0px);
	animation-timing-function: linear;
}

.monaco-progress-container.infinite.infinite-long-running .progress-bit {
	/*
		The more smooth \`linear\` timing function can cause
		higher GPU consumption as indicated in
		https://github.com/microsoft/vscode/issues/97900 &
		https://github.com/microsoft/vscode/issues/138396
	*/
	animation-timing-function: steps(100);
}

/**
 * The progress bit has a width: 2% (1/50) of the parent container. The animation moves it from 0% to 100% of
 * that container. Since translateX is relative to the progress bit size, we have to multiple it with
 * its relative size to the parent container:
 * parent width: 5000%
 *    bit width: 100%
 * translateX should be as follow:
 *  50%: 5000% * 50% - 50% (set to center) = 2450%
 * 100%: 5000% * 100% - 100% (do not overflow) = 4900%
 */
@keyframes progress { from { transform: translateX(0%) scaleX(1) } 50% { transform: translateX(2500%) scaleX(3) } to { transform: translateX(4900%) scaleX(1) } }
`],sourceRoot:""}]);const _=e},14166:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

:root {
	--sash-size: 4px;
}

.monaco-sash {
	position: absolute;
	z-index: 35;
	touch-action: none;
}

.monaco-sash.disabled {
	pointer-events: none;
}

.monaco-sash.mac.vertical {
	cursor: col-resize;
}

.monaco-sash.vertical.minimum {
	cursor: e-resize;
}

.monaco-sash.vertical.maximum {
	cursor: w-resize;
}

.monaco-sash.mac.horizontal {
	cursor: row-resize;
}

.monaco-sash.horizontal.minimum {
	cursor: s-resize;
}

.monaco-sash.horizontal.maximum {
	cursor: n-resize;
}

.monaco-sash.disabled {
	cursor: default !important;
	pointer-events: none !important;
}

.monaco-sash.vertical {
	cursor: ew-resize;
	top: 0;
	width: var(--sash-size);
	height: 100%;
}

.monaco-sash.horizontal {
	cursor: ns-resize;
	left: 0;
	width: 100%;
	height: var(--sash-size);
}

.monaco-sash:not(.disabled) > .orthogonal-drag-handle {
	content: " ";
	height: calc(var(--sash-size) * 2);
	width: calc(var(--sash-size) * 2);
	z-index: 100;
	display: block;
	cursor: all-scroll;
	position: absolute;
}

.monaco-sash.horizontal.orthogonal-edge-north:not(.disabled)
	> .orthogonal-drag-handle.start,
.monaco-sash.horizontal.orthogonal-edge-south:not(.disabled)
	> .orthogonal-drag-handle.end {
	cursor: nwse-resize;
}

.monaco-sash.horizontal.orthogonal-edge-north:not(.disabled)
	> .orthogonal-drag-handle.end,
.monaco-sash.horizontal.orthogonal-edge-south:not(.disabled)
	> .orthogonal-drag-handle.start {
	cursor: nesw-resize;
}

.monaco-sash.vertical > .orthogonal-drag-handle.start {
	left: calc(var(--sash-size) * -0.5);
	top: calc(var(--sash-size) * -1);
}
.monaco-sash.vertical > .orthogonal-drag-handle.end {
	left: calc(var(--sash-size) * -0.5);
	bottom: calc(var(--sash-size) * -1);
}
.monaco-sash.horizontal > .orthogonal-drag-handle.start {
	top: calc(var(--sash-size) * -0.5);
	left: calc(var(--sash-size) * -1);
}
.monaco-sash.horizontal > .orthogonal-drag-handle.end {
	top: calc(var(--sash-size) * -0.5);
	right: calc(var(--sash-size) * -1);
}

.monaco-sash:before {
	content: '';
	pointer-events: none;
	position: absolute;
	width: 100%;
	height: 100%;
	transition: background-color 0.1s ease-out;
	background: transparent;
}

.monaco-sash.vertical:before {
	width: var(--sash-hover-size);
	left: calc(50% - (var(--sash-hover-size) / 2));
}

.monaco-sash.horizontal:before {
	height: var(--sash-hover-size);
	top: calc(50% - (var(--sash-hover-size) / 2));
}

.pointer-events-disabled {
	pointer-events: none !important;
}

/** Debug **/

.monaco-sash.debug {
	background: cyan;
}

.monaco-sash.debug.disabled {
	background: rgba(0, 255, 255, 0.2);
}

.monaco-sash.debug:not(.disabled) > .orthogonal-drag-handle {
	background: red;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/sash/sash.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,gBAAgB;AACjB;;AAEA;CACC,kBAAkB;CAClB,WAAW;CACX,kBAAkB;AACnB;;AAEA;CACC,oBAAoB;AACrB;;AAEA;CACC,kBAAkB;AACnB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,kBAAkB;AACnB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,0BAA0B;CAC1B,+BAA+B;AAChC;;AAEA;CACC,iBAAiB;CACjB,MAAM;CACN,uBAAuB;CACvB,YAAY;AACb;;AAEA;CACC,iBAAiB;CACjB,OAAO;CACP,WAAW;CACX,wBAAwB;AACzB;;AAEA;CACC,YAAY;CACZ,kCAAkC;CAClC,iCAAiC;CACjC,YAAY;CACZ,cAAc;CACd,kBAAkB;CAClB,kBAAkB;AACnB;;AAEA;;;;CAIC,mBAAmB;AACpB;;AAEA;;;;CAIC,mBAAmB;AACpB;;AAEA;CACC,mCAAmC;CACnC,gCAAgC;AACjC;AACA;CACC,mCAAmC;CACnC,mCAAmC;AACpC;AACA;CACC,kCAAkC;CAClC,iCAAiC;AAClC;AACA;CACC,kCAAkC;CAClC,kCAAkC;AACnC;;AAEA;CACC,WAAW;CACX,oBAAoB;CACpB,kBAAkB;CAClB,WAAW;CACX,YAAY;CACZ,0CAA0C;CAC1C,uBAAuB;AACxB;;AAEA;CACC,6BAA6B;CAC7B,8CAA8C;AAC/C;;AAEA;CACC,8BAA8B;CAC9B,6CAA6C;AAC9C;;AAEA;CACC,+BAA+B;AAChC;;AAEA,YAAY;;AAEZ;CACC,gBAAgB;AACjB;;AAEA;CACC,kCAAkC;AACnC;;AAEA;CACC,eAAe;AAChB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

:root {
	--sash-size: 4px;
}

.monaco-sash {
	position: absolute;
	z-index: 35;
	touch-action: none;
}

.monaco-sash.disabled {
	pointer-events: none;
}

.monaco-sash.mac.vertical {
	cursor: col-resize;
}

.monaco-sash.vertical.minimum {
	cursor: e-resize;
}

.monaco-sash.vertical.maximum {
	cursor: w-resize;
}

.monaco-sash.mac.horizontal {
	cursor: row-resize;
}

.monaco-sash.horizontal.minimum {
	cursor: s-resize;
}

.monaco-sash.horizontal.maximum {
	cursor: n-resize;
}

.monaco-sash.disabled {
	cursor: default !important;
	pointer-events: none !important;
}

.monaco-sash.vertical {
	cursor: ew-resize;
	top: 0;
	width: var(--sash-size);
	height: 100%;
}

.monaco-sash.horizontal {
	cursor: ns-resize;
	left: 0;
	width: 100%;
	height: var(--sash-size);
}

.monaco-sash:not(.disabled) > .orthogonal-drag-handle {
	content: " ";
	height: calc(var(--sash-size) * 2);
	width: calc(var(--sash-size) * 2);
	z-index: 100;
	display: block;
	cursor: all-scroll;
	position: absolute;
}

.monaco-sash.horizontal.orthogonal-edge-north:not(.disabled)
	> .orthogonal-drag-handle.start,
.monaco-sash.horizontal.orthogonal-edge-south:not(.disabled)
	> .orthogonal-drag-handle.end {
	cursor: nwse-resize;
}

.monaco-sash.horizontal.orthogonal-edge-north:not(.disabled)
	> .orthogonal-drag-handle.end,
.monaco-sash.horizontal.orthogonal-edge-south:not(.disabled)
	> .orthogonal-drag-handle.start {
	cursor: nesw-resize;
}

.monaco-sash.vertical > .orthogonal-drag-handle.start {
	left: calc(var(--sash-size) * -0.5);
	top: calc(var(--sash-size) * -1);
}
.monaco-sash.vertical > .orthogonal-drag-handle.end {
	left: calc(var(--sash-size) * -0.5);
	bottom: calc(var(--sash-size) * -1);
}
.monaco-sash.horizontal > .orthogonal-drag-handle.start {
	top: calc(var(--sash-size) * -0.5);
	left: calc(var(--sash-size) * -1);
}
.monaco-sash.horizontal > .orthogonal-drag-handle.end {
	top: calc(var(--sash-size) * -0.5);
	right: calc(var(--sash-size) * -1);
}

.monaco-sash:before {
	content: '';
	pointer-events: none;
	position: absolute;
	width: 100%;
	height: 100%;
	transition: background-color 0.1s ease-out;
	background: transparent;
}

.monaco-sash.vertical:before {
	width: var(--sash-hover-size);
	left: calc(50% - (var(--sash-hover-size) / 2));
}

.monaco-sash.horizontal:before {
	height: var(--sash-hover-size);
	top: calc(50% - (var(--sash-hover-size) / 2));
}

.pointer-events-disabled {
	pointer-events: none !important;
}

/** Debug **/

.monaco-sash.debug {
	background: cyan;
}

.monaco-sash.debug.disabled {
	background: rgba(0, 255, 255, 0.2);
}

.monaco-sash.debug:not(.disabled) > .orthogonal-drag-handle {
	background: red;
}
`],sourceRoot:""}]);const _=e},80140:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* Arrows */
.monaco-scrollable-element > .scrollbar > .scra {
	cursor: pointer;
	font-size: 11px !important;
}

.monaco-scrollable-element > .visible {
	opacity: 1;

	/* Background rule added for IE9 - to allow clicks on dom node */
	background:rgba(0,0,0,0);

	transition: opacity 100ms linear;
}
.monaco-scrollable-element > .invisible {
	opacity: 0;
	pointer-events: none;
}
.monaco-scrollable-element > .invisible.fade {
	transition: opacity 800ms linear;
}

/* Scrollable Content Inset Shadow */
.monaco-scrollable-element > .shadow {
	position: absolute;
	display: none;
}
.monaco-scrollable-element > .shadow.top {
	display: block;
	top: 0;
	left: 3px;
	height: 3px;
	width: 100%;
}
.monaco-scrollable-element > .shadow.left {
	display: block;
	top: 3px;
	left: 0;
	height: 100%;
	width: 3px;
}
.monaco-scrollable-element > .shadow.top-left-corner {
	display: block;
	top: 0;
	left: 0;
	height: 3px;
	width: 3px;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/scrollbar/media/scrollbars.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F,WAAW;AACX;CACC,eAAe;CACf,0BAA0B;AAC3B;;AAEA;CACC,UAAU;;CAEV,gEAAgE;CAChE,wBAAwB;;CAExB,gCAAgC;AACjC;AACA;CACC,UAAU;CACV,oBAAoB;AACrB;AACA;CACC,gCAAgC;AACjC;;AAEA,oCAAoC;AACpC;CACC,kBAAkB;CAClB,aAAa;AACd;AACA;CACC,cAAc;CACd,MAAM;CACN,SAAS;CACT,WAAW;CACX,WAAW;AACZ;AACA;CACC,cAAc;CACd,QAAQ;CACR,OAAO;CACP,YAAY;CACZ,UAAU;AACX;AACA;CACC,cAAc;CACd,MAAM;CACN,OAAO;CACP,WAAW;CACX,UAAU;AACX",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* Arrows */
.monaco-scrollable-element > .scrollbar > .scra {
	cursor: pointer;
	font-size: 11px !important;
}

.monaco-scrollable-element > .visible {
	opacity: 1;

	/* Background rule added for IE9 - to allow clicks on dom node */
	background:rgba(0,0,0,0);

	transition: opacity 100ms linear;
}
.monaco-scrollable-element > .invisible {
	opacity: 0;
	pointer-events: none;
}
.monaco-scrollable-element > .invisible.fade {
	transition: opacity 800ms linear;
}

/* Scrollable Content Inset Shadow */
.monaco-scrollable-element > .shadow {
	position: absolute;
	display: none;
}
.monaco-scrollable-element > .shadow.top {
	display: block;
	top: 0;
	left: 3px;
	height: 3px;
	width: 100%;
}
.monaco-scrollable-element > .shadow.left {
	display: block;
	top: 3px;
	left: 0;
	height: 100%;
	width: 3px;
}
.monaco-scrollable-element > .shadow.top-left-corner {
	display: block;
	top: 0;
	left: 0;
	height: 3px;
	width: 3px;
}
`],sourceRoot:""}]);const _=e},3474:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-split-view2 {
	position: relative;
	width: 100%;
	height: 100%;
}

.monaco-split-view2 > .sash-container {
	position: absolute;
	width: 100%;
	height: 100%;
	pointer-events: none;
}

.monaco-split-view2 > .sash-container > .monaco-sash {
	pointer-events: initial;
}

.monaco-split-view2 > .monaco-scrollable-element {
	width: 100%;
	height: 100%;
}

.monaco-split-view2 > .monaco-scrollable-element > .split-view-container {
	width: 100%;
	height: 100%;
	white-space: nowrap;
	position: relative;
}

.monaco-split-view2 > .monaco-scrollable-element > .split-view-container > .split-view-view {
	white-space: initial;
	position: absolute;
}

.monaco-split-view2 > .monaco-scrollable-element > .split-view-container > .split-view-view:not(.visible) {
	display: none;
}

.monaco-split-view2.vertical > .monaco-scrollable-element > .split-view-container > .split-view-view {
	width: 100%;
}

.monaco-split-view2.horizontal > .monaco-scrollable-element > .split-view-container > .split-view-view {
	height: 100%;
}

.monaco-split-view2.separator-border > .monaco-scrollable-element > .split-view-container > .split-view-view:not(:first-child)::before {
	content: ' ';
	position: absolute;
	top: 0;
	left: 0;
	z-index: 5;
	pointer-events: none;
	background-color: var(--separator-border);
}

.monaco-split-view2.separator-border.horizontal > .monaco-scrollable-element > .split-view-container > .split-view-view:not(:first-child)::before {
	height: 100%;
	width: 1px;
}

.monaco-split-view2.separator-border.vertical > .monaco-scrollable-element > .split-view-container > .split-view-view:not(:first-child)::before {
	height: 1px;
	width: 100%;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/splitview/splitview.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kBAAkB;CAClB,WAAW;CACX,YAAY;AACb;;AAEA;CACC,kBAAkB;CAClB,WAAW;CACX,YAAY;CACZ,oBAAoB;AACrB;;AAEA;CACC,uBAAuB;AACxB;;AAEA;CACC,WAAW;CACX,YAAY;AACb;;AAEA;CACC,WAAW;CACX,YAAY;CACZ,mBAAmB;CACnB,kBAAkB;AACnB;;AAEA;CACC,oBAAoB;CACpB,kBAAkB;AACnB;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,WAAW;AACZ;;AAEA;CACC,YAAY;AACb;;AAEA;CACC,YAAY;CACZ,kBAAkB;CAClB,MAAM;CACN,OAAO;CACP,UAAU;CACV,oBAAoB;CACpB,yCAAyC;AAC1C;;AAEA;CACC,YAAY;CACZ,UAAU;AACX;;AAEA;CACC,WAAW;CACX,WAAW;AACZ",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-split-view2 {
	position: relative;
	width: 100%;
	height: 100%;
}

.monaco-split-view2 > .sash-container {
	position: absolute;
	width: 100%;
	height: 100%;
	pointer-events: none;
}

.monaco-split-view2 > .sash-container > .monaco-sash {
	pointer-events: initial;
}

.monaco-split-view2 > .monaco-scrollable-element {
	width: 100%;
	height: 100%;
}

.monaco-split-view2 > .monaco-scrollable-element > .split-view-container {
	width: 100%;
	height: 100%;
	white-space: nowrap;
	position: relative;
}

.monaco-split-view2 > .monaco-scrollable-element > .split-view-container > .split-view-view {
	white-space: initial;
	position: absolute;
}

.monaco-split-view2 > .monaco-scrollable-element > .split-view-container > .split-view-view:not(.visible) {
	display: none;
}

.monaco-split-view2.vertical > .monaco-scrollable-element > .split-view-container > .split-view-view {
	width: 100%;
}

.monaco-split-view2.horizontal > .monaco-scrollable-element > .split-view-container > .split-view-view {
	height: 100%;
}

.monaco-split-view2.separator-border > .monaco-scrollable-element > .split-view-container > .split-view-view:not(:first-child)::before {
	content: ' ';
	position: absolute;
	top: 0;
	left: 0;
	z-index: 5;
	pointer-events: none;
	background-color: var(--separator-border);
}

.monaco-split-view2.separator-border.horizontal > .monaco-scrollable-element > .split-view-container > .split-view-view:not(:first-child)::before {
	height: 100%;
	width: 1px;
}

.monaco-split-view2.separator-border.vertical > .monaco-scrollable-element > .split-view-container > .split-view-view:not(:first-child)::before {
	height: 1px;
	width: 100%;
}
`],sourceRoot:""}]);const _=e},94234:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-table {
	display: flex;
	flex-direction: column;
	position: relative;
	height: 100%;
	width: 100%;
	white-space: nowrap;
}

.monaco-table > .monaco-split-view2 {
	border-bottom: 1px solid transparent;
}

.monaco-table > .monaco-list {
	flex: 1;
}

.monaco-table-tr {
	display: flex;
	height: 100%;
}

.monaco-table-th {
	width: 100%;
	height: 100%;
	font-weight: bold;
	overflow: hidden;
	text-overflow: ellipsis;
}

.monaco-table-th,
.monaco-table-td {
	box-sizing: border-box;
	flex-shrink: 0;
	overflow: hidden;
	white-space: nowrap;
	text-overflow: ellipsis;
}

.monaco-table > .monaco-split-view2 .monaco-sash.vertical::before {
	content: "";
	position: absolute;
	left: calc(var(--sash-size) / 2);
	width: 0;
	border-left: 1px solid transparent;
}

.monaco-table > .monaco-split-view2,
.monaco-table > .monaco-split-view2 .monaco-sash.vertical::before {
	transition: border-color 0.2s ease-out;
}
/*
.monaco-table:hover > .monaco-split-view2,
.monaco-table:hover > .monaco-split-view2 .monaco-sash.vertical::before {
	border-color: rgba(204, 204, 204, 0.2);
} */
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/table/table.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,aAAa;CACb,sBAAsB;CACtB,kBAAkB;CAClB,YAAY;CACZ,WAAW;CACX,mBAAmB;AACpB;;AAEA;CACC,oCAAoC;AACrC;;AAEA;CACC,OAAO;AACR;;AAEA;CACC,aAAa;CACb,YAAY;AACb;;AAEA;CACC,WAAW;CACX,YAAY;CACZ,iBAAiB;CACjB,gBAAgB;CAChB,uBAAuB;AACxB;;AAEA;;CAEC,sBAAsB;CACtB,cAAc;CACd,gBAAgB;CAChB,mBAAmB;CACnB,uBAAuB;AACxB;;AAEA;CACC,WAAW;CACX,kBAAkB;CAClB,gCAAgC;CAChC,QAAQ;CACR,kCAAkC;AACnC;;AAEA;;CAEC,sCAAsC;AACvC;AACA;;;;GAIG",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-table {
	display: flex;
	flex-direction: column;
	position: relative;
	height: 100%;
	width: 100%;
	white-space: nowrap;
}

.monaco-table > .monaco-split-view2 {
	border-bottom: 1px solid transparent;
}

.monaco-table > .monaco-list {
	flex: 1;
}

.monaco-table-tr {
	display: flex;
	height: 100%;
}

.monaco-table-th {
	width: 100%;
	height: 100%;
	font-weight: bold;
	overflow: hidden;
	text-overflow: ellipsis;
}

.monaco-table-th,
.monaco-table-td {
	box-sizing: border-box;
	flex-shrink: 0;
	overflow: hidden;
	white-space: nowrap;
	text-overflow: ellipsis;
}

.monaco-table > .monaco-split-view2 .monaco-sash.vertical::before {
	content: "";
	position: absolute;
	left: calc(var(--sash-size) / 2);
	width: 0;
	border-left: 1px solid transparent;
}

.monaco-table > .monaco-split-view2,
.monaco-table > .monaco-split-view2 .monaco-sash.vertical::before {
	transition: border-color 0.2s ease-out;
}
/*
.monaco-table:hover > .monaco-split-view2,
.monaco-table:hover > .monaco-split-view2 .monaco-sash.vertical::before {
	border-color: rgba(204, 204, 204, 0.2);
} */
`],sourceRoot:""}]);const _=e},62516:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-custom-toggle {
	margin-left: 2px;
	float: left;
	cursor: pointer;
	overflow: hidden;
	width: 20px;
	height: 20px;
	border-radius: 3px;
	border: 1px solid transparent;
	padding: 1px;
	box-sizing:	border-box;
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

.monaco-custom-toggle:hover {
	background-color: var(--vscode-inputOption-hoverBackground);
}

.hc-black .monaco-custom-toggle:hover,
.hc-light .monaco-custom-toggle:hover {
	border: 1px dashed var(--vscode-focusBorder);
}

.hc-black .monaco-custom-toggle,
.hc-light .monaco-custom-toggle {
	background: none;
}

.hc-black .monaco-custom-toggle:hover,
.hc-light .monaco-custom-toggle:hover {
	background: none;
}

.monaco-custom-toggle.monaco-checkbox {
	height: 18px;
	width: 18px;
	border: 1px solid transparent;
	border-radius: 3px;
	margin-right: 9px;
	margin-left: 0px;
	padding: 0px;
	opacity: 1;
	background-size: 16px !important;
}

/* hide check when unchecked */
.monaco-custom-toggle.monaco-checkbox:not(.checked)::before {
	visibility: hidden;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/toggle/toggle.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,gBAAgB;CAChB,WAAW;CACX,eAAe;CACf,gBAAgB;CAChB,WAAW;CACX,YAAY;CACZ,kBAAkB;CAClB,6BAA6B;CAC7B,YAAY;CACZ,sBAAsB;CACtB,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;AACtB;;AAEA;CACC,2DAA2D;AAC5D;;AAEA;;CAEC,4CAA4C;AAC7C;;AAEA;;CAEC,gBAAgB;AACjB;;AAEA;;CAEC,gBAAgB;AACjB;;AAEA;CACC,YAAY;CACZ,WAAW;CACX,6BAA6B;CAC7B,kBAAkB;CAClB,iBAAiB;CACjB,gBAAgB;CAChB,YAAY;CACZ,UAAU;CACV,gCAAgC;AACjC;;AAEA,8BAA8B;AAC9B;CACC,kBAAkB;AACnB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-custom-toggle {
	margin-left: 2px;
	float: left;
	cursor: pointer;
	overflow: hidden;
	width: 20px;
	height: 20px;
	border-radius: 3px;
	border: 1px solid transparent;
	padding: 1px;
	box-sizing:	border-box;
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

.monaco-custom-toggle:hover {
	background-color: var(--vscode-inputOption-hoverBackground);
}

.hc-black .monaco-custom-toggle:hover,
.hc-light .monaco-custom-toggle:hover {
	border: 1px dashed var(--vscode-focusBorder);
}

.hc-black .monaco-custom-toggle,
.hc-light .monaco-custom-toggle {
	background: none;
}

.hc-black .monaco-custom-toggle:hover,
.hc-light .monaco-custom-toggle:hover {
	background: none;
}

.monaco-custom-toggle.monaco-checkbox {
	height: 18px;
	width: 18px;
	border: 1px solid transparent;
	border-radius: 3px;
	margin-right: 9px;
	margin-left: 0px;
	padding: 0px;
	opacity: 1;
	background-size: 16px !important;
}

/* hide check when unchecked */
.monaco-custom-toggle.monaco-checkbox:not(.checked)::before {
	visibility: hidden;
}
`],sourceRoot:""}]);const _=e},71963:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-tl-row {
	display: flex;
	height: 100%;
	align-items: center;
	position: relative;
}

.monaco-tl-indent {
	height: 100%;
	position: absolute;
	top: 0;
	left: 16px;
	pointer-events: none;
}

.hide-arrows .monaco-tl-indent {
	left: 12px;
}

.monaco-tl-indent > .indent-guide {
	display: inline-block;
	box-sizing: border-box;
	height: 100%;
	border-left: 1px solid transparent;
}

.monaco-tl-indent > .indent-guide {
	transition: border-color 0.1s linear;
}

.monaco-tl-twistie,
.monaco-tl-contents {
	height: 100%;
}

.monaco-tl-twistie {
	font-size: 10px;
	text-align: right;
	padding-right: 6px;
	flex-shrink: 0;
	width: 16px;
	display: flex !important;
	align-items: center;
	justify-content: center;
	transform: translateX(3px);
}

.monaco-tl-contents {
	flex: 1;
	overflow: hidden;
}

.monaco-tl-twistie::before {
	border-radius: 20px;
}

.monaco-tl-twistie.collapsed::before {
	transform: rotate(-90deg);
}

.monaco-tl-twistie.codicon-tree-item-loading::before {
	/* Use steps to throttle FPS to reduce CPU usage */
	animation: codicon-spin 1.25s steps(30) infinite;
}

.monaco-tree-type-filter {
	position: absolute;
	top: 0;
	display: flex;
	padding: 3px;
	transition: top 0.3s;
	max-width: 200px;
	z-index: 100;
	margin: 0 6px;
}

.monaco-tree-type-filter.disabled {
	top: -40px;
}

.monaco-tree-type-filter-grab {
	display: flex !important;
	align-items: center;
	justify-content: center;
	cursor: grab;
	margin-right: 2px;
}

.monaco-tree-type-filter-grab.grabbing {
	cursor: grabbing;
}

.monaco-tree-type-filter-input {
	flex: 1;
}

.monaco-tree-type-filter-input .monaco-inputbox {
	height: 23px;
}

.monaco-tree-type-filter-input .monaco-inputbox > .ibwrapper > .input,
.monaco-tree-type-filter-input .monaco-inputbox > .ibwrapper > .mirror {
	padding: 2px 4px;
}

.monaco-tree-type-filter-input .monaco-findInput > .controls {
	top: 2px;
}

.monaco-tree-type-filter-actionbar {
	margin-left: 4px;
}

.monaco-tree-type-filter-actionbar .monaco-action-bar .action-label {
	padding: 2px;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/tree/media/tree.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,aAAa;CACb,YAAY;CACZ,mBAAmB;CACnB,kBAAkB;AACnB;;AAEA;CACC,YAAY;CACZ,kBAAkB;CAClB,MAAM;CACN,UAAU;CACV,oBAAoB;AACrB;;AAEA;CACC,UAAU;AACX;;AAEA;CACC,qBAAqB;CACrB,sBAAsB;CACtB,YAAY;CACZ,kCAAkC;AACnC;;AAEA;CACC,oCAAoC;AACrC;;AAEA;;CAEC,YAAY;AACb;;AAEA;CACC,eAAe;CACf,iBAAiB;CACjB,kBAAkB;CAClB,cAAc;CACd,WAAW;CACX,wBAAwB;CACxB,mBAAmB;CACnB,uBAAuB;CACvB,0BAA0B;AAC3B;;AAEA;CACC,OAAO;CACP,gBAAgB;AACjB;;AAEA;CACC,mBAAmB;AACpB;;AAEA;CACC,yBAAyB;AAC1B;;AAEA;CACC,kDAAkD;CAClD,gDAAgD;AACjD;;AAEA;CACC,kBAAkB;CAClB,MAAM;CACN,aAAa;CACb,YAAY;CACZ,oBAAoB;CACpB,gBAAgB;CAChB,YAAY;CACZ,aAAa;AACd;;AAEA;CACC,UAAU;AACX;;AAEA;CACC,wBAAwB;CACxB,mBAAmB;CACnB,uBAAuB;CACvB,YAAY;CACZ,iBAAiB;AAClB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,OAAO;AACR;;AAEA;CACC,YAAY;AACb;;AAEA;;CAEC,gBAAgB;AACjB;;AAEA;CACC,QAAQ;AACT;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,YAAY;AACb",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-tl-row {
	display: flex;
	height: 100%;
	align-items: center;
	position: relative;
}

.monaco-tl-indent {
	height: 100%;
	position: absolute;
	top: 0;
	left: 16px;
	pointer-events: none;
}

.hide-arrows .monaco-tl-indent {
	left: 12px;
}

.monaco-tl-indent > .indent-guide {
	display: inline-block;
	box-sizing: border-box;
	height: 100%;
	border-left: 1px solid transparent;
}

.monaco-tl-indent > .indent-guide {
	transition: border-color 0.1s linear;
}

.monaco-tl-twistie,
.monaco-tl-contents {
	height: 100%;
}

.monaco-tl-twistie {
	font-size: 10px;
	text-align: right;
	padding-right: 6px;
	flex-shrink: 0;
	width: 16px;
	display: flex !important;
	align-items: center;
	justify-content: center;
	transform: translateX(3px);
}

.monaco-tl-contents {
	flex: 1;
	overflow: hidden;
}

.monaco-tl-twistie::before {
	border-radius: 20px;
}

.monaco-tl-twistie.collapsed::before {
	transform: rotate(-90deg);
}

.monaco-tl-twistie.codicon-tree-item-loading::before {
	/* Use steps to throttle FPS to reduce CPU usage */
	animation: codicon-spin 1.25s steps(30) infinite;
}

.monaco-tree-type-filter {
	position: absolute;
	top: 0;
	display: flex;
	padding: 3px;
	transition: top 0.3s;
	max-width: 200px;
	z-index: 100;
	margin: 0 6px;
}

.monaco-tree-type-filter.disabled {
	top: -40px;
}

.monaco-tree-type-filter-grab {
	display: flex !important;
	align-items: center;
	justify-content: center;
	cursor: grab;
	margin-right: 2px;
}

.monaco-tree-type-filter-grab.grabbing {
	cursor: grabbing;
}

.monaco-tree-type-filter-input {
	flex: 1;
}

.monaco-tree-type-filter-input .monaco-inputbox {
	height: 23px;
}

.monaco-tree-type-filter-input .monaco-inputbox > .ibwrapper > .input,
.monaco-tree-type-filter-input .monaco-inputbox > .ibwrapper > .mirror {
	padding: 2px 4px;
}

.monaco-tree-type-filter-input .monaco-findInput > .controls {
	top: 2px;
}

.monaco-tree-type-filter-actionbar {
	margin-left: 4px;
}

.monaco-tree-type-filter-actionbar .monaco-action-bar .action-label {
	padding: 2px;
}
`],sourceRoot:""}]);const _=e},74333:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.quick-input-widget {
	position: absolute;
	width: 600px;
	z-index: 2550;
	left: 50%;
	margin-left: -300px;
	-webkit-app-region: no-drag;
}

.quick-input-titlebar {
	display: flex;
	align-items: center;
}

.quick-input-left-action-bar {
	display: flex;
	margin-left: 4px;
	flex: 1;
}

.quick-input-title {
	padding: 3px 0px;
	text-align: center;
	text-overflow: ellipsis;
	overflow: hidden;
}

.quick-input-right-action-bar {
	display: flex;
	margin-right: 4px;
	flex: 1;
}

.quick-input-right-action-bar > .actions-container {
	justify-content: flex-end;
}

.quick-input-titlebar .monaco-action-bar .action-label.codicon {
	background-position: center;
	background-repeat: no-repeat;
	padding: 2px;
}

.quick-input-description {
	margin: 6px;
}

.quick-input-header .quick-input-description {
	margin: 4px 2px;
}

.quick-input-header {
	display: flex;
	padding: 6px 6px 0px 6px;
	margin-bottom: -2px;
}

.quick-input-widget.hidden-input .quick-input-header {
	/* reduce margins and paddings when input box hidden */
	padding: 0;
	margin-bottom: 0;
}

.quick-input-and-message {
	display: flex;
	flex-direction: column;
	flex-grow: 1;
	min-width: 0;
	position: relative;
}

.quick-input-check-all {
	align-self: center;
	margin: 0;
}

.quick-input-filter {
	flex-grow: 1;
	display: flex;
	position: relative;
}

.quick-input-box {
	flex-grow: 1;
}

.quick-input-widget.show-checkboxes .quick-input-box,
.quick-input-widget.show-checkboxes .quick-input-message {
	margin-left: 5px;
}

.quick-input-visible-count {
	position: absolute;
	left: -10000px;
}

.quick-input-count {
	align-self: center;
	position: absolute;
	right: 4px;
	display: flex;
	align-items: center;
}

.quick-input-count .monaco-count-badge {
	vertical-align: middle;
	padding: 2px 4px;
	border-radius: 2px;
	min-height: auto;
	line-height: normal;
}

.quick-input-action {
	margin-left: 6px;
}

.quick-input-action .monaco-text-button {
	font-size: 11px;
	padding: 0 6px;
	display: flex;
	height: 27.5px;
	align-items: center;
}

.quick-input-message {
	margin-top: -1px;
	padding: 5px;
	overflow-wrap: break-word;
}

.quick-input-message > .codicon {
	margin: 0 0.2em;
	vertical-align: text-bottom;
}

.quick-input-progress.monaco-progress-container {
	position: relative;
}

.quick-input-progress.monaco-progress-container,
.quick-input-progress.monaco-progress-container .progress-bit {
	height: 2px;
}

.quick-input-list {
	line-height: 22px;
	margin-top: 6px;
	padding: 0px 1px 1px 1px;
}

.quick-input-widget.hidden-input .quick-input-list {
	margin-top: 0; /* reduce margins when input box hidden */
}

.quick-input-list .monaco-list {
	overflow: hidden;
	max-height: calc(20 * 22px);
}

.quick-input-list .quick-input-list-entry {
	box-sizing: border-box;
	overflow: hidden;
	display: flex;
	height: 100%;
	padding: 0 6px;
}

.quick-input-list .quick-input-list-entry.quick-input-list-separator-border {
	border-top-width: 1px;
	border-top-style: solid;
}

.quick-input-list .monaco-list-row[data-index="0"] .quick-input-list-entry.quick-input-list-separator-border {
	border-top-style: none;
}

.quick-input-list .quick-input-list-label {
	overflow: hidden;
	display: flex;
	height: 100%;
	flex: 1;
}

.quick-input-list .quick-input-list-checkbox {
	align-self: center;
	margin: 0;
}

.quick-input-list .quick-input-list-rows {
	overflow: hidden;
	text-overflow: ellipsis;
	display: flex;
	flex-direction: column;
	height: 100%;
	flex: 1;
	margin-left: 5px;
}

.quick-input-widget.show-checkboxes .quick-input-list .quick-input-list-rows {
	margin-left: 10px;
}

.quick-input-widget .quick-input-list .quick-input-list-checkbox {
	display: none;
}
.quick-input-widget.show-checkboxes .quick-input-list .quick-input-list-checkbox {
	display: inline;
}

.quick-input-list .quick-input-list-rows > .quick-input-list-row {
	display: flex;
	align-items: center;
}

.quick-input-list .quick-input-list-rows > .quick-input-list-row .monaco-icon-label,
.quick-input-list .quick-input-list-rows > .quick-input-list-row .monaco-icon-label .monaco-icon-label-container > .monaco-icon-name-container {
	flex: 1; /* make sure the icon label grows within the row */
}

.quick-input-list .quick-input-list-rows > .quick-input-list-row .codicon[class*='codicon-'] {
	vertical-align: text-bottom;
}

.quick-input-list .quick-input-list-rows .monaco-highlighted-label span {
	opacity: 1;
}

.quick-input-list .quick-input-list-entry .quick-input-list-entry-keybinding {
	margin-right: 8px; /* separate from the separator label or scrollbar if any */
}

.quick-input-list .quick-input-list-label-meta {
	opacity: 0.7;
	line-height: normal;
	text-overflow: ellipsis;
	overflow: hidden;
}

.quick-input-list .monaco-highlighted-label .highlight {
	font-weight: bold;
}

.quick-input-list .quick-input-list-entry .quick-input-list-separator {
	margin-right: 8px; /* separate from keybindings or actions */
}

.quick-input-list .quick-input-list-entry-action-bar {
	display: flex;
	flex: 0;
	overflow: visible;
}

.quick-input-list .quick-input-list-entry-action-bar .action-label {
	/*
	 * By default, actions in the quick input action bar are hidden
	 * until hovered over them or selected.
	 */
	display: none;
}

.quick-input-list .quick-input-list-entry-action-bar .action-label.codicon {
	margin-right: 4px;
	padding: 0px 2px 2px 2px;
}

.quick-input-list .quick-input-list-entry-action-bar {
	margin-top: 1px;
}

.quick-input-list .quick-input-list-entry-action-bar {
	margin-right: 4px; /* separate from scrollbar */
}

.quick-input-list .quick-input-list-entry .quick-input-list-entry-action-bar .action-label.always-visible,
.quick-input-list .quick-input-list-entry:hover .quick-input-list-entry-action-bar .action-label,
.quick-input-list .monaco-list-row.focused .quick-input-list-entry-action-bar .action-label {
	display: flex;
}

/* focused items in quick pick */
.quick-input-list .monaco-list-row.focused .monaco-keybinding-key,
.quick-input-list .monaco-list-row.focused .quick-input-list-entry .quick-input-list-separator {
	color: inherit
}
.quick-input-list .monaco-list-row.focused .monaco-keybinding-key {
	background: none;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/parts/quickinput/browser/media/quickInput.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kBAAkB;CAClB,YAAY;CACZ,aAAa;CACb,SAAS;CACT,mBAAmB;CACnB,2BAA2B;AAC5B;;AAEA;CACC,aAAa;CACb,mBAAmB;AACpB;;AAEA;CACC,aAAa;CACb,gBAAgB;CAChB,OAAO;AACR;;AAEA;CACC,gBAAgB;CAChB,kBAAkB;CAClB,uBAAuB;CACvB,gBAAgB;AACjB;;AAEA;CACC,aAAa;CACb,iBAAiB;CACjB,OAAO;AACR;;AAEA;CACC,yBAAyB;AAC1B;;AAEA;CACC,2BAA2B;CAC3B,4BAA4B;CAC5B,YAAY;AACb;;AAEA;CACC,WAAW;AACZ;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,aAAa;CACb,wBAAwB;CACxB,mBAAmB;AACpB;;AAEA;CACC,sDAAsD;CACtD,UAAU;CACV,gBAAgB;AACjB;;AAEA;CACC,aAAa;CACb,sBAAsB;CACtB,YAAY;CACZ,YAAY;CACZ,kBAAkB;AACnB;;AAEA;CACC,kBAAkB;CAClB,SAAS;AACV;;AAEA;CACC,YAAY;CACZ,aAAa;CACb,kBAAkB;AACnB;;AAEA;CACC,YAAY;AACb;;AAEA;;CAEC,gBAAgB;AACjB;;AAEA;CACC,kBAAkB;CAClB,cAAc;AACf;;AAEA;CACC,kBAAkB;CAClB,kBAAkB;CAClB,UAAU;CACV,aAAa;CACb,mBAAmB;AACpB;;AAEA;CACC,sBAAsB;CACtB,gBAAgB;CAChB,kBAAkB;CAClB,gBAAgB;CAChB,mBAAmB;AACpB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,eAAe;CACf,cAAc;CACd,aAAa;CACb,cAAc;CACd,mBAAmB;AACpB;;AAEA;CACC,gBAAgB;CAChB,YAAY;CACZ,yBAAyB;AAC1B;;AAEA;CACC,eAAe;CACf,2BAA2B;AAC5B;;AAEA;CACC,kBAAkB;AACnB;;AAEA;;CAEC,WAAW;AACZ;;AAEA;CACC,iBAAiB;CACjB,eAAe;CACf,wBAAwB;AACzB;;AAEA;CACC,aAAa,EAAE,yCAAyC;AACzD;;AAEA;CACC,gBAAgB;CAChB,2BAA2B;AAC5B;;AAEA;CACC,sBAAsB;CACtB,gBAAgB;CAChB,aAAa;CACb,YAAY;CACZ,cAAc;AACf;;AAEA;CACC,qBAAqB;CACrB,uBAAuB;AACxB;;AAEA;CACC,sBAAsB;AACvB;;AAEA;CACC,gBAAgB;CAChB,aAAa;CACb,YAAY;CACZ,OAAO;AACR;;AAEA;CACC,kBAAkB;CAClB,SAAS;AACV;;AAEA;CACC,gBAAgB;CAChB,uBAAuB;CACvB,aAAa;CACb,sBAAsB;CACtB,YAAY;CACZ,OAAO;CACP,gBAAgB;AACjB;;AAEA;CACC,iBAAiB;AAClB;;AAEA;CACC,aAAa;AACd;AACA;CACC,eAAe;AAChB;;AAEA;CACC,aAAa;CACb,mBAAmB;AACpB;;AAEA;;CAEC,OAAO,EAAE,kDAAkD;AAC5D;;AAEA;CACC,2BAA2B;AAC5B;;AAEA;CACC,UAAU;AACX;;AAEA;CACC,iBAAiB,EAAE,0DAA0D;AAC9E;;AAEA;CACC,YAAY;CACZ,mBAAmB;CACnB,uBAAuB;CACvB,gBAAgB;AACjB;;AAEA;CACC,iBAAiB;AAClB;;AAEA;CACC,iBAAiB,EAAE,yCAAyC;AAC7D;;AAEA;CACC,aAAa;CACb,OAAO;CACP,iBAAiB;AAClB;;AAEA;CACC;;;GAGE;CACF,aAAa;AACd;;AAEA;CACC,iBAAiB;CACjB,wBAAwB;AACzB;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,iBAAiB,EAAE,4BAA4B;AAChD;;AAEA;;;CAGC,aAAa;AACd;;AAEA,gCAAgC;AAChC;;CAEC;AACD;AACA;CACC,gBAAgB;AACjB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.quick-input-widget {
	position: absolute;
	width: 600px;
	z-index: 2550;
	left: 50%;
	margin-left: -300px;
	-webkit-app-region: no-drag;
}

.quick-input-titlebar {
	display: flex;
	align-items: center;
}

.quick-input-left-action-bar {
	display: flex;
	margin-left: 4px;
	flex: 1;
}

.quick-input-title {
	padding: 3px 0px;
	text-align: center;
	text-overflow: ellipsis;
	overflow: hidden;
}

.quick-input-right-action-bar {
	display: flex;
	margin-right: 4px;
	flex: 1;
}

.quick-input-right-action-bar > .actions-container {
	justify-content: flex-end;
}

.quick-input-titlebar .monaco-action-bar .action-label.codicon {
	background-position: center;
	background-repeat: no-repeat;
	padding: 2px;
}

.quick-input-description {
	margin: 6px;
}

.quick-input-header .quick-input-description {
	margin: 4px 2px;
}

.quick-input-header {
	display: flex;
	padding: 6px 6px 0px 6px;
	margin-bottom: -2px;
}

.quick-input-widget.hidden-input .quick-input-header {
	/* reduce margins and paddings when input box hidden */
	padding: 0;
	margin-bottom: 0;
}

.quick-input-and-message {
	display: flex;
	flex-direction: column;
	flex-grow: 1;
	min-width: 0;
	position: relative;
}

.quick-input-check-all {
	align-self: center;
	margin: 0;
}

.quick-input-filter {
	flex-grow: 1;
	display: flex;
	position: relative;
}

.quick-input-box {
	flex-grow: 1;
}

.quick-input-widget.show-checkboxes .quick-input-box,
.quick-input-widget.show-checkboxes .quick-input-message {
	margin-left: 5px;
}

.quick-input-visible-count {
	position: absolute;
	left: -10000px;
}

.quick-input-count {
	align-self: center;
	position: absolute;
	right: 4px;
	display: flex;
	align-items: center;
}

.quick-input-count .monaco-count-badge {
	vertical-align: middle;
	padding: 2px 4px;
	border-radius: 2px;
	min-height: auto;
	line-height: normal;
}

.quick-input-action {
	margin-left: 6px;
}

.quick-input-action .monaco-text-button {
	font-size: 11px;
	padding: 0 6px;
	display: flex;
	height: 27.5px;
	align-items: center;
}

.quick-input-message {
	margin-top: -1px;
	padding: 5px;
	overflow-wrap: break-word;
}

.quick-input-message > .codicon {
	margin: 0 0.2em;
	vertical-align: text-bottom;
}

.quick-input-progress.monaco-progress-container {
	position: relative;
}

.quick-input-progress.monaco-progress-container,
.quick-input-progress.monaco-progress-container .progress-bit {
	height: 2px;
}

.quick-input-list {
	line-height: 22px;
	margin-top: 6px;
	padding: 0px 1px 1px 1px;
}

.quick-input-widget.hidden-input .quick-input-list {
	margin-top: 0; /* reduce margins when input box hidden */
}

.quick-input-list .monaco-list {
	overflow: hidden;
	max-height: calc(20 * 22px);
}

.quick-input-list .quick-input-list-entry {
	box-sizing: border-box;
	overflow: hidden;
	display: flex;
	height: 100%;
	padding: 0 6px;
}

.quick-input-list .quick-input-list-entry.quick-input-list-separator-border {
	border-top-width: 1px;
	border-top-style: solid;
}

.quick-input-list .monaco-list-row[data-index="0"] .quick-input-list-entry.quick-input-list-separator-border {
	border-top-style: none;
}

.quick-input-list .quick-input-list-label {
	overflow: hidden;
	display: flex;
	height: 100%;
	flex: 1;
}

.quick-input-list .quick-input-list-checkbox {
	align-self: center;
	margin: 0;
}

.quick-input-list .quick-input-list-rows {
	overflow: hidden;
	text-overflow: ellipsis;
	display: flex;
	flex-direction: column;
	height: 100%;
	flex: 1;
	margin-left: 5px;
}

.quick-input-widget.show-checkboxes .quick-input-list .quick-input-list-rows {
	margin-left: 10px;
}

.quick-input-widget .quick-input-list .quick-input-list-checkbox {
	display: none;
}
.quick-input-widget.show-checkboxes .quick-input-list .quick-input-list-checkbox {
	display: inline;
}

.quick-input-list .quick-input-list-rows > .quick-input-list-row {
	display: flex;
	align-items: center;
}

.quick-input-list .quick-input-list-rows > .quick-input-list-row .monaco-icon-label,
.quick-input-list .quick-input-list-rows > .quick-input-list-row .monaco-icon-label .monaco-icon-label-container > .monaco-icon-name-container {
	flex: 1; /* make sure the icon label grows within the row */
}

.quick-input-list .quick-input-list-rows > .quick-input-list-row .codicon[class*='codicon-'] {
	vertical-align: text-bottom;
}

.quick-input-list .quick-input-list-rows .monaco-highlighted-label span {
	opacity: 1;
}

.quick-input-list .quick-input-list-entry .quick-input-list-entry-keybinding {
	margin-right: 8px; /* separate from the separator label or scrollbar if any */
}

.quick-input-list .quick-input-list-label-meta {
	opacity: 0.7;
	line-height: normal;
	text-overflow: ellipsis;
	overflow: hidden;
}

.quick-input-list .monaco-highlighted-label .highlight {
	font-weight: bold;
}

.quick-input-list .quick-input-list-entry .quick-input-list-separator {
	margin-right: 8px; /* separate from keybindings or actions */
}

.quick-input-list .quick-input-list-entry-action-bar {
	display: flex;
	flex: 0;
	overflow: visible;
}

.quick-input-list .quick-input-list-entry-action-bar .action-label {
	/*
	 * By default, actions in the quick input action bar are hidden
	 * until hovered over them or selected.
	 */
	display: none;
}

.quick-input-list .quick-input-list-entry-action-bar .action-label.codicon {
	margin-right: 4px;
	padding: 0px 2px 2px 2px;
}

.quick-input-list .quick-input-list-entry-action-bar {
	margin-top: 1px;
}

.quick-input-list .quick-input-list-entry-action-bar {
	margin-right: 4px; /* separate from scrollbar */
}

.quick-input-list .quick-input-list-entry .quick-input-list-entry-action-bar .action-label.always-visible,
.quick-input-list .quick-input-list-entry:hover .quick-input-list-entry-action-bar .action-label,
.quick-input-list .monaco-list-row.focused .quick-input-list-entry-action-bar .action-label {
	display: flex;
}

/* focused items in quick pick */
.quick-input-list .monaco-list-row.focused .monaco-keybinding-key,
.quick-input-list .monaco-list-row.focused .quick-input-list-entry .quick-input-list-separator {
	color: inherit
}
.quick-input-list .monaco-list-row.focused .monaco-keybinding-key {
	background: none;
}
`],sourceRoot:""}]);const _=e},86307:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .inputarea {
	min-width: 0;
	min-height: 0;
	margin: 0;
	padding: 0;
	position: absolute;
	outline: none !important;
	resize: none;
	border: none;
	overflow: hidden;
	color: transparent;
	background-color: transparent;
}
/*.monaco-editor .inputarea {
	position: fixed !important;
	width: 800px !important;
	height: 500px !important;
	top: initial !important;
	left: initial !important;
	bottom: 0 !important;
	right: 0 !important;
	color: black !important;
	background: white !important;
	line-height: 15px !important;
	font-size: 14px !important;
}*/
.monaco-editor .inputarea.ime-input {
	z-index: 10;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/controller/textAreaHandler.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,YAAY;CACZ,aAAa;CACb,SAAS;CACT,UAAU;CACV,kBAAkB;CAClB,wBAAwB;CACxB,YAAY;CACZ,YAAY;CACZ,gBAAgB;CAChB,kBAAkB;CAClB,6BAA6B;AAC9B;AACA;;;;;;;;;;;;EAYE;AACF;CACC,WAAW;AACZ",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .inputarea {
	min-width: 0;
	min-height: 0;
	margin: 0;
	padding: 0;
	position: absolute;
	outline: none !important;
	resize: none;
	border: none;
	overflow: hidden;
	color: transparent;
	background-color: transparent;
}
/*.monaco-editor .inputarea {
	position: fixed !important;
	width: 800px !important;
	height: 500px !important;
	top: initial !important;
	left: initial !important;
	bottom: 0 !important;
	right: 0 !important;
	color: black !important;
	background: white !important;
	line-height: 15px !important;
	font-size: 14px !important;
}*/
.monaco-editor .inputarea.ime-input {
	z-index: 10;
}
`],sourceRoot:""}]);const _=e},72035:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .blockDecorations-container {
	position: absolute;
	top: 0;
}

.monaco-editor .blockDecorations-block {
	position: absolute;
	box-sizing: border-box;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/blockDecorations/blockDecorations.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kBAAkB;CAClB,MAAM;AACP;;AAEA;CACC,kBAAkB;CAClB,sBAAsB;AACvB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .blockDecorations-container {
	position: absolute;
	top: 0;
}

.monaco-editor .blockDecorations-block {
	position: absolute;
	box-sizing: border-box;
}
`],sourceRoot:""}]);const _=e},28405:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .view-overlays .current-line {
	display: block;
	position: absolute;
	left: 0;
	top: 0;
	box-sizing: border-box;
}

.monaco-editor .margin-view-overlays .current-line {
	display: block;
	position: absolute;
	left: 0;
	top: 0;
	box-sizing: border-box;
}

.monaco-editor .margin-view-overlays .current-line.current-line-margin.current-line-margin-both {
	border-right: 0;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/currentLineHighlight/currentLineHighlight.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,cAAc;CACd,kBAAkB;CAClB,OAAO;CACP,MAAM;CACN,sBAAsB;AACvB;;AAEA;CACC,cAAc;CACd,kBAAkB;CAClB,OAAO;CACP,MAAM;CACN,sBAAsB;AACvB;;AAEA;CACC,eAAe;AAChB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .view-overlays .current-line {
	display: block;
	position: absolute;
	left: 0;
	top: 0;
	box-sizing: border-box;
}

.monaco-editor .margin-view-overlays .current-line {
	display: block;
	position: absolute;
	left: 0;
	top: 0;
	box-sizing: border-box;
}

.monaco-editor .margin-view-overlays .current-line.current-line-margin.current-line-margin-both {
	border-right: 0;
}
`],sourceRoot:""}]);const _=e},83093:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/*
	Keeping name short for faster parsing.
	cdr = core decorations rendering (div)
*/
.monaco-editor .lines-content .cdr {
	position: absolute;
}`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/decorations/decorations.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;;;CAGC;AACD;CACC,kBAAkB;AACnB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/*
	Keeping name short for faster parsing.
	cdr = core decorations rendering (div)
*/
.monaco-editor .lines-content .cdr {
	position: absolute;
}`],sourceRoot:""}]);const _=e},98081:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .glyph-margin {
	position: absolute;
	top: 0;
}

/*
	Keeping name short for faster parsing.
	cgmr = core glyph margin rendering (div)
*/
.monaco-editor .margin-view-overlays .cgmr {
	position: absolute;
	display: flex;
	align-items: center;
	justify-content: center;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/glyphMargin/glyphMargin.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kBAAkB;CAClB,MAAM;AACP;;AAEA;;;CAGC;AACD;CACC,kBAAkB;CAClB,aAAa;CACb,mBAAmB;CACnB,uBAAuB;AACxB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .glyph-margin {
	position: absolute;
	top: 0;
}

/*
	Keeping name short for faster parsing.
	cgmr = core glyph margin rendering (div)
*/
.monaco-editor .margin-view-overlays .cgmr {
	position: absolute;
	display: flex;
	align-items: center;
	justify-content: center;
}
`],sourceRoot:""}]);const _=e},93777:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .lines-content .core-guide {
	position: absolute;
	box-sizing: border-box;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/indentGuides/indentGuides.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kBAAkB;CAClB,sBAAsB;AACvB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .lines-content .core-guide {
	position: absolute;
	box-sizing: border-box;
}
`],sourceRoot:""}]);const _=e},6953:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .margin-view-overlays .line-numbers {
	font-variant-numeric: tabular-nums;
	position: absolute;
	text-align: right;
	display: inline-block;
	vertical-align: middle;
	box-sizing: border-box;
	cursor: default;
	height: 100%;
}

.monaco-editor .relative-current-line-number {
	text-align: left;
	display: inline-block;
	width: 100%;
}

.monaco-editor .margin-view-overlays .line-numbers.lh-odd {
	margin-top: 1px;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/lineNumbers/lineNumbers.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kCAAkC;CAClC,kBAAkB;CAClB,iBAAiB;CACjB,qBAAqB;CACrB,sBAAsB;CACtB,sBAAsB;CACtB,eAAe;CACf,YAAY;AACb;;AAEA;CACC,gBAAgB;CAChB,qBAAqB;CACrB,WAAW;AACZ;;AAEA;CACC,eAAe;AAChB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .margin-view-overlays .line-numbers {
	font-variant-numeric: tabular-nums;
	position: absolute;
	text-align: right;
	display: inline-block;
	vertical-align: middle;
	box-sizing: border-box;
	cursor: default;
	height: 100%;
}

.monaco-editor .relative-current-line-number {
	text-align: left;
	display: inline-block;
	width: 100%;
}

.monaco-editor .margin-view-overlays .line-numbers.lh-odd {
	margin-top: 1px;
}
`],sourceRoot:""}]);const _=e},65876:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* Uncomment to see lines flashing when they're painted */
/*.monaco-editor .view-lines > .view-line {
	background-color: none;
	animation-name: flash-background;
	animation-duration: 800ms;
}
@keyframes flash-background {
	0%   { background-color: lightgreen; }
	100% { background-color: none }
}*/

.mtkcontrol {
	color: rgb(255, 255, 255) !important;
	background: rgb(150, 0, 0) !important;
}

.monaco-editor.no-user-select .lines-content,
.monaco-editor.no-user-select .view-line,
.monaco-editor.no-user-select .view-lines {
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

.monaco-editor.enable-user-select {
	user-select: initial;
	-webkit-user-select: initial;
	-ms-user-select: initial;
}

.monaco-editor .view-lines {
	white-space: nowrap;
}

.monaco-editor .view-line {
	position: absolute;
	width: 100%;
}

.monaco-editor .mtkz {
	display: inline-block;
}

/* TODO@tokenization bootstrap fix */
/*.monaco-editor .view-line > span > span {
	float: none;
	min-height: inherit;
	margin-left: inherit;
}*/
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/lines/viewLines.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F,yDAAyD;AACzD;;;;;;;;EAQE;;AAEF;CACC,oCAAoC;CACpC,qCAAqC;AACtC;;AAEA;;;CAGC,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;AACtB;;AAEA;CACC,oBAAoB;CACpB,4BAA4B;CAC5B,wBAAwB;AACzB;;AAEA;CACC,mBAAmB;AACpB;;AAEA;CACC,kBAAkB;CAClB,WAAW;AACZ;;AAEA;CACC,qBAAqB;AACtB;;AAEA,oCAAoC;AACpC;;;;EAIE",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* Uncomment to see lines flashing when they're painted */
/*.monaco-editor .view-lines > .view-line {
	background-color: none;
	animation-name: flash-background;
	animation-duration: 800ms;
}
@keyframes flash-background {
	0%   { background-color: lightgreen; }
	100% { background-color: none }
}*/

.mtkcontrol {
	color: rgb(255, 255, 255) !important;
	background: rgb(150, 0, 0) !important;
}

.monaco-editor.no-user-select .lines-content,
.monaco-editor.no-user-select .view-line,
.monaco-editor.no-user-select .view-lines {
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

.monaco-editor.enable-user-select {
	user-select: initial;
	-webkit-user-select: initial;
	-ms-user-select: initial;
}

.monaco-editor .view-lines {
	white-space: nowrap;
}

.monaco-editor .view-line {
	position: absolute;
	width: 100%;
}

.monaco-editor .mtkz {
	display: inline-block;
}

/* TODO@tokenization bootstrap fix */
/*.monaco-editor .view-line > span > span {
	float: none;
	min-height: inherit;
	margin-left: inherit;
}*/
`],sourceRoot:""}]);const _=e},57375:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .lines-decorations {
	position: absolute;
	top: 0;
	background: white;
}

/*
	Keeping name short for faster parsing.
	cldr = core lines decorations rendering (div)
*/
.monaco-editor .margin-view-overlays .cldr {
	position: absolute;
	height: 100%;
}`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/linesDecorations/linesDecorations.css"],names:[],mappings:"AAAA;;;+FAG+F;AAC/F;CACC,kBAAkB;CAClB,MAAM;CACN,iBAAiB;AAClB;;AAEA;;;CAGC;AACD;CACC,kBAAkB;CAClB,YAAY;AACb",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .lines-decorations {
	position: absolute;
	top: 0;
	background: white;
}

/*
	Keeping name short for faster parsing.
	cldr = core lines decorations rendering (div)
*/
.monaco-editor .margin-view-overlays .cldr {
	position: absolute;
	height: 100%;
}`],sourceRoot:""}]);const _=e},73313:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/*
	Keeping name short for faster parsing.
	cmdr = core margin decorations rendering (div)
*/
.monaco-editor .margin-view-overlays .cmdr {
	position: absolute;
	left: 0;
	width: 100%;
	height: 100%;
}`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/marginDecorations/marginDecorations.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;;;CAGC;AACD;CACC,kBAAkB;CAClB,OAAO;CACP,WAAW;CACX,YAAY;AACb",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/*
	Keeping name short for faster parsing.
	cmdr = core margin decorations rendering (div)
*/
.monaco-editor .margin-view-overlays .cmdr {
	position: absolute;
	left: 0;
	width: 100%;
	height: 100%;
}`],sourceRoot:""}]);const _=e},36493:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* START cover the case that slider is visible on mouseover */
.monaco-editor .minimap.slider-mouseover .minimap-slider {
	opacity: 0;
	transition: opacity 100ms linear;
}
.monaco-editor .minimap.slider-mouseover:hover .minimap-slider {
	opacity: 1;
}
.monaco-editor .minimap.slider-mouseover .minimap-slider.active {
	opacity: 1;
}
/* END cover the case that slider is visible on mouseover */

.monaco-editor .minimap-shadow-hidden {
	position: absolute;
	width: 0;
}
.monaco-editor .minimap-shadow-visible {
	position: absolute;
	left: -6px;
	width: 6px;
}
.monaco-editor.no-minimap-shadow .minimap-shadow-visible {
	position: absolute;
	left: -1px;
	width: 1px;
}

/* 0.5s fade in/out for the minimap */
.minimap.autohide {
	opacity: 0.0;
	transition: opacity 0.5s;
}
.minimap.autohide:hover {
	opacity: 1.0;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/minimap/minimap.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F,6DAA6D;AAC7D;CACC,UAAU;CACV,gCAAgC;AACjC;AACA;CACC,UAAU;AACX;AACA;CACC,UAAU;AACX;AACA,2DAA2D;;AAE3D;CACC,kBAAkB;CAClB,QAAQ;AACT;AACA;CACC,kBAAkB;CAClB,UAAU;CACV,UAAU;AACX;AACA;CACC,kBAAkB;CAClB,UAAU;CACV,UAAU;AACX;;AAEA,qCAAqC;AACrC;CACC,YAAY;CACZ,wBAAwB;AACzB;AACA;CACC,YAAY;AACb",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* START cover the case that slider is visible on mouseover */
.monaco-editor .minimap.slider-mouseover .minimap-slider {
	opacity: 0;
	transition: opacity 100ms linear;
}
.monaco-editor .minimap.slider-mouseover:hover .minimap-slider {
	opacity: 1;
}
.monaco-editor .minimap.slider-mouseover .minimap-slider.active {
	opacity: 1;
}
/* END cover the case that slider is visible on mouseover */

.monaco-editor .minimap-shadow-hidden {
	position: absolute;
	width: 0;
}
.monaco-editor .minimap-shadow-visible {
	position: absolute;
	left: -6px;
	width: 6px;
}
.monaco-editor.no-minimap-shadow .minimap-shadow-visible {
	position: absolute;
	left: -1px;
	width: 1px;
}

/* 0.5s fade in/out for the minimap */
.minimap.autohide {
	opacity: 0.0;
	transition: opacity 0.5s;
}
.minimap.autohide:hover {
	opacity: 1.0;
}
`],sourceRoot:""}]);const _=e},80213:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .overlayWidgets {
	position: absolute;
	top: 0;
	left:0;
}`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/overlayWidgets/overlayWidgets.css"],names:[],mappings:"AAAA;;;+FAG+F;AAC/F;CACC,kBAAkB;CAClB,MAAM;CACN,MAAM;AACP",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .overlayWidgets {
	position: absolute;
	top: 0;
	left:0;
}`],sourceRoot:""}]);const _=e},81637:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .view-ruler {
	position: absolute;
	top: 0;
}`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/rulers/rulers.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kBAAkB;CAClB,MAAM;AACP",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .view-ruler {
	position: absolute;
	top: 0;
}`],sourceRoot:""}]);const _=e},29133:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .scroll-decoration {
	position: absolute;
	top: 0;
	left: 0;
	height: 6px;
}`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/scrollDecoration/scrollDecoration.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,kBAAkB;CAClB,MAAM;CACN,OAAO;CACP,WAAW;AACZ",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .scroll-decoration {
	position: absolute;
	top: 0;
	left: 0;
	height: 6px;
}`],sourceRoot:""}]);const _=e},48829:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/*
	Keeping name short for faster parsing.
	cslr = core selections layer rendering (div)
*/
.monaco-editor .lines-content .cslr {
	position: absolute;
}

.monaco-editor			.top-left-radius		{ border-top-left-radius: 3px; }
.monaco-editor			.bottom-left-radius		{ border-bottom-left-radius: 3px; }
.monaco-editor			.top-right-radius		{ border-top-right-radius: 3px; }
.monaco-editor			.bottom-right-radius	{ border-bottom-right-radius: 3px; }

.monaco-editor.hc-black .top-left-radius		{ border-top-left-radius: 0; }
.monaco-editor.hc-black .bottom-left-radius		{ border-bottom-left-radius: 0; }
.monaco-editor.hc-black .top-right-radius		{ border-top-right-radius: 0; }
.monaco-editor.hc-black .bottom-right-radius	{ border-bottom-right-radius: 0; }

.monaco-editor.hc-light .top-left-radius		{ border-top-left-radius: 0; }
.monaco-editor.hc-light .bottom-left-radius		{ border-bottom-left-radius: 0; }
.monaco-editor.hc-light .top-right-radius		{ border-top-right-radius: 0; }
.monaco-editor.hc-light .bottom-right-radius	{ border-bottom-right-radius: 0; }
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/selections/selections.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;;;CAGC;AACD;CACC,kBAAkB;AACnB;;AAEA,qCAAqC,2BAA2B,EAAE;AAClE,wCAAwC,8BAA8B,EAAE;AACxE,sCAAsC,4BAA4B,EAAE;AACpE,wCAAwC,+BAA+B,EAAE;;AAEzE,4CAA4C,yBAAyB,EAAE;AACvE,+CAA+C,4BAA4B,EAAE;AAC7E,6CAA6C,0BAA0B,EAAE;AACzE,+CAA+C,6BAA6B,EAAE;;AAE9E,4CAA4C,yBAAyB,EAAE;AACvE,+CAA+C,4BAA4B,EAAE;AAC7E,6CAA6C,0BAA0B,EAAE;AACzE,+CAA+C,6BAA6B,EAAE",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/*
	Keeping name short for faster parsing.
	cslr = core selections layer rendering (div)
*/
.monaco-editor .lines-content .cslr {
	position: absolute;
}

.monaco-editor			.top-left-radius		{ border-top-left-radius: 3px; }
.monaco-editor			.bottom-left-radius		{ border-bottom-left-radius: 3px; }
.monaco-editor			.top-right-radius		{ border-top-right-radius: 3px; }
.monaco-editor			.bottom-right-radius	{ border-bottom-right-radius: 3px; }

.monaco-editor.hc-black .top-left-radius		{ border-top-left-radius: 0; }
.monaco-editor.hc-black .bottom-left-radius		{ border-bottom-left-radius: 0; }
.monaco-editor.hc-black .top-right-radius		{ border-top-right-radius: 0; }
.monaco-editor.hc-black .bottom-right-radius	{ border-bottom-right-radius: 0; }

.monaco-editor.hc-light .top-left-radius		{ border-top-left-radius: 0; }
.monaco-editor.hc-light .bottom-left-radius		{ border-bottom-left-radius: 0; }
.monaco-editor.hc-light .top-right-radius		{ border-top-right-radius: 0; }
.monaco-editor.hc-light .bottom-right-radius	{ border-bottom-right-radius: 0; }
`],sourceRoot:""}]);const _=e},2289:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .cursors-layer {
	position: absolute;
	top: 0;
}

.monaco-editor .cursors-layer > .cursor {
	position: absolute;
	overflow: hidden;
}

/* -- smooth-caret-animation -- */
.monaco-editor .cursors-layer.cursor-smooth-caret-animation > .cursor {
	transition: all 80ms;
}

/* -- block-outline-style -- */
.monaco-editor .cursors-layer.cursor-block-outline-style > .cursor {
	box-sizing: border-box;
	background: transparent !important;
	border-style: solid;
	border-width: 1px;
}

/* -- underline-style -- */
.monaco-editor .cursors-layer.cursor-underline-style > .cursor {
	border-bottom-width: 2px;
	border-bottom-style: solid;
	background: transparent !important;
	box-sizing: border-box;
}

/* -- underline-thin-style -- */
.monaco-editor .cursors-layer.cursor-underline-thin-style > .cursor {
	border-bottom-width: 1px;
	border-bottom-style: solid;
	background: transparent !important;
	box-sizing: border-box;
}

@keyframes monaco-cursor-smooth {
	0%,
	20% {
		opacity: 1;
	}
	60%,
	100% {
		opacity: 0;
	}
}

@keyframes monaco-cursor-phase {
	0%,
	20% {
		opacity: 1;
	}
	90%,
	100% {
		opacity: 0;
	}
}

@keyframes monaco-cursor-expand {
	0%,
	20% {
		transform: scaleY(1);
	}
	80%,
	100% {
		transform: scaleY(0);
	}
}

.cursor-smooth {
	animation: monaco-cursor-smooth 0.5s ease-in-out 0s 20 alternate;
}

.cursor-phase {
	animation: monaco-cursor-phase 0.5s ease-in-out 0s 20 alternate;
}

.cursor-expand > .cursor {
	animation: monaco-cursor-expand 0.5s ease-in-out 0s 20 alternate;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/viewParts/viewCursors/viewCursors.css"],names:[],mappings:"AAAA;;;+FAG+F;AAC/F;CACC,kBAAkB;CAClB,MAAM;AACP;;AAEA;CACC,kBAAkB;CAClB,gBAAgB;AACjB;;AAEA,iCAAiC;AACjC;CACC,oBAAoB;AACrB;;AAEA,8BAA8B;AAC9B;CACC,sBAAsB;CACtB,kCAAkC;CAClC,mBAAmB;CACnB,iBAAiB;AAClB;;AAEA,0BAA0B;AAC1B;CACC,wBAAwB;CACxB,0BAA0B;CAC1B,kCAAkC;CAClC,sBAAsB;AACvB;;AAEA,+BAA+B;AAC/B;CACC,wBAAwB;CACxB,0BAA0B;CAC1B,kCAAkC;CAClC,sBAAsB;AACvB;;AAEA;CACC;;EAEC,UAAU;CACX;CACA;;EAEC,UAAU;CACX;AACD;;AAEA;CACC;;EAEC,UAAU;CACX;CACA;;EAEC,UAAU;CACX;AACD;;AAEA;CACC;;EAEC,oBAAoB;CACrB;CACA;;EAEC,oBAAoB;CACrB;AACD;;AAEA;CACC,gEAAgE;AACjE;;AAEA;CACC,+DAA+D;AAChE;;AAEA;CACC,gEAAgE;AACjE",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .cursors-layer {
	position: absolute;
	top: 0;
}

.monaco-editor .cursors-layer > .cursor {
	position: absolute;
	overflow: hidden;
}

/* -- smooth-caret-animation -- */
.monaco-editor .cursors-layer.cursor-smooth-caret-animation > .cursor {
	transition: all 80ms;
}

/* -- block-outline-style -- */
.monaco-editor .cursors-layer.cursor-block-outline-style > .cursor {
	box-sizing: border-box;
	background: transparent !important;
	border-style: solid;
	border-width: 1px;
}

/* -- underline-style -- */
.monaco-editor .cursors-layer.cursor-underline-style > .cursor {
	border-bottom-width: 2px;
	border-bottom-style: solid;
	background: transparent !important;
	box-sizing: border-box;
}

/* -- underline-thin-style -- */
.monaco-editor .cursors-layer.cursor-underline-thin-style > .cursor {
	border-bottom-width: 1px;
	border-bottom-style: solid;
	background: transparent !important;
	box-sizing: border-box;
}

@keyframes monaco-cursor-smooth {
	0%,
	20% {
		opacity: 1;
	}
	60%,
	100% {
		opacity: 0;
	}
}

@keyframes monaco-cursor-phase {
	0%,
	20% {
		opacity: 1;
	}
	90%,
	100% {
		opacity: 0;
	}
}

@keyframes monaco-cursor-expand {
	0%,
	20% {
		transform: scaleY(1);
	}
	80%,
	100% {
		transform: scaleY(0);
	}
}

.cursor-smooth {
	animation: monaco-cursor-smooth 0.5s ease-in-out 0s 20 alternate;
}

.cursor-phase {
	animation: monaco-cursor-phase 0.5s ease-in-out 0s 20 alternate;
}

.cursor-expand > .cursor {
	animation: monaco-cursor-expand 0.5s ease-in-out 0s 20 alternate;
}
`],sourceRoot:""}]);const _=e},95388:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
/* ---------- DiffEditor ---------- */

.monaco-diff-editor .diffOverview {
	z-index: 9;
}

.monaco-diff-editor .diffOverview .diffViewport {
	z-index: 10;
}

/* colors not externalized: using transparancy on background */
.monaco-diff-editor.vs			.diffOverview { background: rgba(0, 0, 0, 0.03); }
.monaco-diff-editor.vs-dark		.diffOverview { background: rgba(255, 255, 255, 0.01); }

.monaco-scrollable-element.modified-in-monaco-diff-editor.vs		.scrollbar { background: rgba(0,0,0,0); }
.monaco-scrollable-element.modified-in-monaco-diff-editor.vs-dark	.scrollbar { background: rgba(0,0,0,0); }
.monaco-scrollable-element.modified-in-monaco-diff-editor.hc-black	.scrollbar { background: none; }
.monaco-scrollable-element.modified-in-monaco-diff-editor.hc-light	.scrollbar { background: none; }

.monaco-scrollable-element.modified-in-monaco-diff-editor .slider {
	z-index: 10;
}
.modified-in-monaco-diff-editor				.slider.active { background: rgba(171, 171, 171, .4); }
.modified-in-monaco-diff-editor.hc-black	.slider.active { background: none; }
.modified-in-monaco-diff-editor.hc-light	.slider.active { background: none; }

/* ---------- Diff ---------- */

.monaco-editor .insert-sign,
.monaco-diff-editor .insert-sign,
.monaco-editor .delete-sign,
.monaco-diff-editor .delete-sign {
	font-size: 11px !important;
	opacity: 0.7 !important;
	display: flex !important;
	align-items: center;
}
.monaco-editor.hc-black .insert-sign,
.monaco-diff-editor.hc-black .insert-sign,
.monaco-editor.hc-black .delete-sign,
.monaco-diff-editor.hc-black .delete-sign,
.monaco-editor.hc-light .insert-sign,
.monaco-diff-editor.hc-light .insert-sign,
.monaco-editor.hc-light .delete-sign,
.monaco-diff-editor.hc-light .delete-sign {
	opacity: 1;
}

.monaco-editor .inline-deleted-margin-view-zone {
	text-align: right;
}
.monaco-editor .inline-added-margin-view-zone {
	text-align: right;
}

.monaco-editor .arrow-revert-change {
	z-index: 10;
	position: absolute;
}

.monaco-editor .arrow-revert-change:hover {
	cursor: pointer;
}

/* ---------- Inline Diff ---------- */

.monaco-editor .view-zones .view-lines .view-line span {
	display: inline-block;
}

.monaco-editor .margin-view-zones .lightbulb-glyph:hover {
	cursor: pointer;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/widget/media/diffEditor.css"],names:[],mappings:"AAAA;;;+FAG+F;AAC/F,qCAAqC;;AAErC;CACC,UAAU;AACX;;AAEA;CACC,WAAW;AACZ;;AAEA,8DAA8D;AAC9D,yCAAyC,+BAA+B,EAAE;AAC1E,6CAA6C,qCAAqC,EAAE;;AAEpF,2EAA2E,yBAAyB,EAAE;AACtG,+EAA+E,yBAAyB,EAAE;AAC1G,gFAAgF,gBAAgB,EAAE;AAClG,gFAAgF,gBAAgB,EAAE;;AAElG;CACC,WAAW;AACZ;AACA,oDAAoD,mCAAmC,EAAE;AACzF,0DAA0D,gBAAgB,EAAE;AAC5E,0DAA0D,gBAAgB,EAAE;;AAE5E,+BAA+B;;AAE/B;;;;CAIC,0BAA0B;CAC1B,uBAAuB;CACvB,wBAAwB;CACxB,mBAAmB;AACpB;AACA;;;;;;;;CAQC,UAAU;AACX;;AAEA;CACC,iBAAiB;AAClB;AACA;CACC,iBAAiB;AAClB;;AAEA;CACC,WAAW;CACX,kBAAkB;AACnB;;AAEA;CACC,eAAe;AAChB;;AAEA,sCAAsC;;AAEtC;CACC,qBAAqB;AACtB;;AAEA;CACC,eAAe;AAChB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
/* ---------- DiffEditor ---------- */

.monaco-diff-editor .diffOverview {
	z-index: 9;
}

.monaco-diff-editor .diffOverview .diffViewport {
	z-index: 10;
}

/* colors not externalized: using transparancy on background */
.monaco-diff-editor.vs			.diffOverview { background: rgba(0, 0, 0, 0.03); }
.monaco-diff-editor.vs-dark		.diffOverview { background: rgba(255, 255, 255, 0.01); }

.monaco-scrollable-element.modified-in-monaco-diff-editor.vs		.scrollbar { background: rgba(0,0,0,0); }
.monaco-scrollable-element.modified-in-monaco-diff-editor.vs-dark	.scrollbar { background: rgba(0,0,0,0); }
.monaco-scrollable-element.modified-in-monaco-diff-editor.hc-black	.scrollbar { background: none; }
.monaco-scrollable-element.modified-in-monaco-diff-editor.hc-light	.scrollbar { background: none; }

.monaco-scrollable-element.modified-in-monaco-diff-editor .slider {
	z-index: 10;
}
.modified-in-monaco-diff-editor				.slider.active { background: rgba(171, 171, 171, .4); }
.modified-in-monaco-diff-editor.hc-black	.slider.active { background: none; }
.modified-in-monaco-diff-editor.hc-light	.slider.active { background: none; }

/* ---------- Diff ---------- */

.monaco-editor .insert-sign,
.monaco-diff-editor .insert-sign,
.monaco-editor .delete-sign,
.monaco-diff-editor .delete-sign {
	font-size: 11px !important;
	opacity: 0.7 !important;
	display: flex !important;
	align-items: center;
}
.monaco-editor.hc-black .insert-sign,
.monaco-diff-editor.hc-black .insert-sign,
.monaco-editor.hc-black .delete-sign,
.monaco-diff-editor.hc-black .delete-sign,
.monaco-editor.hc-light .insert-sign,
.monaco-diff-editor.hc-light .insert-sign,
.monaco-editor.hc-light .delete-sign,
.monaco-diff-editor.hc-light .delete-sign {
	opacity: 1;
}

.monaco-editor .inline-deleted-margin-view-zone {
	text-align: right;
}
.monaco-editor .inline-added-margin-view-zone {
	text-align: right;
}

.monaco-editor .arrow-revert-change {
	z-index: 10;
	position: absolute;
}

.monaco-editor .arrow-revert-change:hover {
	cursor: pointer;
}

/* ---------- Inline Diff ---------- */

.monaco-editor .view-zones .view-lines .view-line span {
	display: inline-block;
}

.monaco-editor .margin-view-zones .lightbulb-glyph:hover {
	cursor: pointer;
}
`],sourceRoot:""}]);const _=e},66931:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-diff-editor .diff-review-line-number {
	text-align: right;
	display: inline-block;
}

.monaco-diff-editor .diff-review {
	position: absolute;
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

.monaco-diff-editor .diff-review-summary {
	padding-left: 10px;
}

.monaco-diff-editor .diff-review-shadow {
	position: absolute;
}

.monaco-diff-editor .diff-review-row {
	white-space: pre;
}

.monaco-diff-editor .diff-review-table {
	display: table;
	min-width: 100%;
}

.monaco-diff-editor .diff-review-row {
	display: table-row;
	width: 100%;
}

.monaco-diff-editor .diff-review-spacer {
	display: inline-block;
	width: 10px;
	vertical-align: middle;
}

.monaco-diff-editor .diff-review-spacer > .codicon {
	font-size: 9px !important;
}

.monaco-diff-editor .diff-review-actions {
	display: inline-block;
	position: absolute;
	right: 10px;
	top: 2px;
}

.monaco-diff-editor .diff-review-actions .action-label {
	width: 16px;
	height: 16px;
	margin: 2px 0;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/widget/media/diffReview.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,iBAAiB;CACjB,qBAAqB;AACtB;;AAEA;CACC,kBAAkB;CAClB,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;AACtB;;AAEA;CACC,kBAAkB;AACnB;;AAEA;CACC,kBAAkB;AACnB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,cAAc;CACd,eAAe;AAChB;;AAEA;CACC,kBAAkB;CAClB,WAAW;AACZ;;AAEA;CACC,qBAAqB;CACrB,WAAW;CACX,sBAAsB;AACvB;;AAEA;CACC,yBAAyB;AAC1B;;AAEA;CACC,qBAAqB;CACrB,kBAAkB;CAClB,WAAW;CACX,QAAQ;AACT;;AAEA;CACC,WAAW;CACX,YAAY;CACZ,aAAa;AACd",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-diff-editor .diff-review-line-number {
	text-align: right;
	display: inline-block;
}

.monaco-diff-editor .diff-review {
	position: absolute;
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

.monaco-diff-editor .diff-review-summary {
	padding-left: 10px;
}

.monaco-diff-editor .diff-review-shadow {
	position: absolute;
}

.monaco-diff-editor .diff-review-row {
	white-space: pre;
}

.monaco-diff-editor .diff-review-table {
	display: table;
	min-width: 100%;
}

.monaco-diff-editor .diff-review-row {
	display: table-row;
	width: 100%;
}

.monaco-diff-editor .diff-review-spacer {
	display: inline-block;
	width: 10px;
	vertical-align: middle;
}

.monaco-diff-editor .diff-review-spacer > .codicon {
	font-size: 9px !important;
}

.monaco-diff-editor .diff-review-actions {
	display: inline-block;
	position: absolute;
	right: 10px;
	top: 2px;
}

.monaco-diff-editor .diff-review-actions .action-label {
	width: 16px;
	height: 16px;
	margin: 2px 0;
}
`],sourceRoot:""}]);const _=e},99585:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* -------------------- IE10 remove auto clear button -------------------- */

::-ms-clear {
	display: none;
}

/* All widgets */
/* I am not a big fan of this rule */
.monaco-editor .editor-widget input {
	color: inherit;
}

/* -------------------- Editor -------------------- */

.monaco-editor {
	position: relative;
	overflow: visible;
	-webkit-text-size-adjust: 100%;
}

/* -------------------- Misc -------------------- */

.monaco-editor .overflow-guard {
	position: relative;
	overflow: hidden;
}

.monaco-editor .view-overlays {
	position: absolute;
	top: 0;
}

/*
.monaco-editor .auto-closed-character {
	opacity: 0.3;
}
*/
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/browser/widget/media/editor.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F,4EAA4E;;AAE5E;CACC,aAAa;AACd;;AAEA,gBAAgB;AAChB,oCAAoC;AACpC;CACC,cAAc;AACf;;AAEA,qDAAqD;;AAErD;CACC,kBAAkB;CAClB,iBAAiB;CACjB,8BAA8B;AAC/B;;AAEA,mDAAmD;;AAEnD;CACC,kBAAkB;CAClB,gBAAgB;AACjB;;AAEA;CACC,kBAAkB;CAClB,MAAM;AACP;;AAEA;;;;CAIC",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* -------------------- IE10 remove auto clear button -------------------- */

::-ms-clear {
	display: none;
}

/* All widgets */
/* I am not a big fan of this rule */
.monaco-editor .editor-widget input {
	color: inherit;
}

/* -------------------- Editor -------------------- */

.monaco-editor {
	position: relative;
	overflow: visible;
	-webkit-text-size-adjust: 100%;
}

/* -------------------- Misc -------------------- */

.monaco-editor .overflow-guard {
	position: relative;
	overflow: hidden;
}

.monaco-editor .view-overlays {
	position: absolute;
	top: 0;
}

/*
.monaco-editor .auto-closed-character {
	opacity: 0.3;
}
*/
`],sourceRoot:""}]);const _=e},71446:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.quick-input-widget {
	font-size: 13px;
}

.quick-input-widget .monaco-highlighted-label .highlight,
.quick-input-widget .monaco-highlighted-label .highlight {
	color: #0066BF;
}

.vs .quick-input-widget .monaco-list-row.focused .monaco-highlighted-label .highlight,
.vs .quick-input-widget .monaco-list-row.focused .monaco-highlighted-label .highlight {
	color: #9DDDFF;
}

.vs-dark .quick-input-widget .monaco-highlighted-label .highlight,
.vs-dark .quick-input-widget .monaco-highlighted-label .highlight {
	color: #0097fb;
}

.hc-black .quick-input-widget .monaco-highlighted-label .highlight,
.hc-black .quick-input-widget .monaco-highlighted-label .highlight {
	color: #F38518;
}

.hc-light .quick-input-widget .monaco-highlighted-label .highlight,
.hc-light .quick-input-widget .monaco-highlighted-label .highlight {
	color: #0F4A85;
}

.monaco-keybinding > .monaco-keybinding-key {
	background-color: rgba(221, 221, 221, 0.4);
	border: solid 1px rgba(204, 204, 204, 0.4);
	border-bottom-color: rgba(187, 187, 187, 0.4);
	box-shadow: inset 0 -1px 0 rgba(187, 187, 187, 0.4);
	color: #555;
}

.hc-black .monaco-keybinding > .monaco-keybinding-key {
	background-color: transparent;
	border: solid 1px rgb(111, 195, 223);
	box-shadow: none;
	color: #fff;
}

.hc-light .monaco-keybinding > .monaco-keybinding-key {
	background-color: transparent;
	border: solid 1px #0F4A85;
	box-shadow: none;
	color: #292929;
}

.vs-dark .monaco-keybinding > .monaco-keybinding-key {
	background-color: rgba(128, 128, 128, 0.17);
	border: solid 1px rgba(51, 51, 51, 0.6);
	border-bottom-color: rgba(68, 68, 68, 0.6);
	box-shadow: inset 0 -1px 0 rgba(68, 68, 68, 0.6);
	color: #ccc;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/standalone/browser/quickInput/standaloneQuickInput.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,eAAe;AAChB;;AAEA;;CAEC,cAAc;AACf;;AAEA;;CAEC,cAAc;AACf;;AAEA;;CAEC,cAAc;AACf;;AAEA;;CAEC,cAAc;AACf;;AAEA;;CAEC,cAAc;AACf;;AAEA;CACC,0CAA0C;CAC1C,0CAA0C;CAC1C,6CAA6C;CAC7C,mDAAmD;CACnD,WAAW;AACZ;;AAEA;CACC,6BAA6B;CAC7B,oCAAoC;CACpC,gBAAgB;CAChB,WAAW;AACZ;;AAEA;CACC,6BAA6B;CAC7B,yBAAyB;CACzB,gBAAgB;CAChB,cAAc;AACf;;AAEA;CACC,2CAA2C;CAC3C,uCAAuC;CACvC,0CAA0C;CAC1C,gDAAgD;CAChD,WAAW;AACZ",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.quick-input-widget {
	font-size: 13px;
}

.quick-input-widget .monaco-highlighted-label .highlight,
.quick-input-widget .monaco-highlighted-label .highlight {
	color: #0066BF;
}

.vs .quick-input-widget .monaco-list-row.focused .monaco-highlighted-label .highlight,
.vs .quick-input-widget .monaco-list-row.focused .monaco-highlighted-label .highlight {
	color: #9DDDFF;
}

.vs-dark .quick-input-widget .monaco-highlighted-label .highlight,
.vs-dark .quick-input-widget .monaco-highlighted-label .highlight {
	color: #0097fb;
}

.hc-black .quick-input-widget .monaco-highlighted-label .highlight,
.hc-black .quick-input-widget .monaco-highlighted-label .highlight {
	color: #F38518;
}

.hc-light .quick-input-widget .monaco-highlighted-label .highlight,
.hc-light .quick-input-widget .monaco-highlighted-label .highlight {
	color: #0F4A85;
}

.monaco-keybinding > .monaco-keybinding-key {
	background-color: rgba(221, 221, 221, 0.4);
	border: solid 1px rgba(204, 204, 204, 0.4);
	border-bottom-color: rgba(187, 187, 187, 0.4);
	box-shadow: inset 0 -1px 0 rgba(187, 187, 187, 0.4);
	color: #555;
}

.hc-black .monaco-keybinding > .monaco-keybinding-key {
	background-color: transparent;
	border: solid 1px rgb(111, 195, 223);
	box-shadow: none;
	color: #fff;
}

.hc-light .monaco-keybinding > .monaco-keybinding-key {
	background-color: transparent;
	border: solid 1px #0F4A85;
	box-shadow: none;
	color: #292929;
}

.vs-dark .monaco-keybinding > .monaco-keybinding-key {
	background-color: rgba(128, 128, 128, 0.17);
	border: solid 1px rgba(51, 51, 51, 0.6);
	border-bottom-color: rgba(68, 68, 68, 0.6);
	box-shadow: inset 0 -1px 0 rgba(68, 68, 68, 0.6);
	color: #ccc;
}
`],sourceRoot:""}]);const _=e},3614:(l,A,t)=>{t.d(A,{A:()=>_});var i=t(71354),s=t.n(i),r=t(76314),a=t.n(r),e=a()(s());e.push([l.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/


/* Default standalone editor fonts */
.monaco-editor {
	font-family: -apple-system, BlinkMacSystemFont, "Segoe WPC", "Segoe UI", "HelveticaNeue-Light", system-ui, "Ubuntu", "Droid Sans", sans-serif;
	--monaco-monospace-font: "SF Mono", Monaco, Menlo, Consolas, "Ubuntu Mono", "Liberation Mono", "DejaVu Sans Mono", "Courier New", monospace;
}

.monaco-menu .monaco-action-bar.vertical .action-item .action-menu-item:focus .action-label {
	stroke-width: 1.2px;
}

.monaco-editor.vs-dark .monaco-menu .monaco-action-bar.vertical .action-menu-item:focus .action-label,
.monaco-editor.hc-black .monaco-menu .monaco-action-bar.vertical .action-menu-item:focus .action-label,
.monaco-editor.hc-light .monaco-menu .monaco-action-bar.vertical .action-menu-item:focus .action-label {
	stroke-width: 1.2px;
}

.monaco-hover p {
	margin: 0;
}

/* See https://github.com/microsoft/monaco-editor/issues/2168#issuecomment-780078600 */
.monaco-aria-container {
	position: absolute !important;
	top: 0; /* avoid being placed underneath a sibling element */
	height: 1px;
	width: 1px;
	margin: -1px;
	overflow: hidden;
	padding: 0;
	clip: rect(1px, 1px, 1px, 1px);
	clip-path: inset(50%);
}

/* The hc-black theme is already high contrast optimized */
.monaco-editor.hc-black,
.monaco-editor.hc-light {
	-ms-high-contrast-adjust: none;
}
/* In case the browser goes into high contrast mode and the editor is not configured with the hc-black theme */
@media screen and (-ms-high-contrast:active) {

	/* current line highlight */
	.monaco-editor.vs .view-overlays .current-line,
	.monaco-editor.vs-dark .view-overlays .current-line {
		border-color: windowtext !important;
		border-left: 0;
		border-right: 0;
	}

	/* view cursors */
	.monaco-editor.vs .cursor,
	.monaco-editor.vs-dark .cursor {
		background-color: windowtext !important;
	}
	/* dnd target */
	.monaco-editor.vs .dnd-target,
	.monaco-editor.vs-dark .dnd-target {
		border-color: windowtext !important;
	}

	/* selected text background */
	.monaco-editor.vs .selected-text,
	.monaco-editor.vs-dark .selected-text {
		background-color: highlight !important;
	}

	/* allow the text to have a transparent background. */
	.monaco-editor.vs .view-line,
	.monaco-editor.vs-dark .view-line {
		-ms-high-contrast-adjust: none;
	}

	/* text color */
	.monaco-editor.vs .view-line span,
	.monaco-editor.vs-dark .view-line span {
		color: windowtext !important;
	}
	/* selected text color */
	.monaco-editor.vs .view-line span.inline-selected-text,
	.monaco-editor.vs-dark .view-line span.inline-selected-text {
		color: highlighttext !important;
	}

	/* allow decorations */
	.monaco-editor.vs .view-overlays,
	.monaco-editor.vs-dark .view-overlays {
		-ms-high-contrast-adjust: none;
	}

	/* various decorations */
	.monaco-editor.vs .selectionHighlight,
	.monaco-editor.vs-dark .selectionHighlight,
	.monaco-editor.vs .wordHighlight,
	.monaco-editor.vs-dark .wordHighlight,
	.monaco-editor.vs .wordHighlightStrong,
	.monaco-editor.vs-dark .wordHighlightStrong,
	.monaco-editor.vs .reference-decoration,
	.monaco-editor.vs-dark .reference-decoration {
		border: 2px dotted highlight !important;
		background: transparent !important;
		box-sizing: border-box;
	}
	.monaco-editor.vs .rangeHighlight,
	.monaco-editor.vs-dark .rangeHighlight {
		background: transparent !important;
		border: 1px dotted activeborder !important;
		box-sizing: border-box;
	}
	.monaco-editor.vs .bracket-match,
	.monaco-editor.vs-dark .bracket-match {
		border-color: windowtext !important;
		background: transparent !important;
	}

	/* find widget */
	.monaco-editor.vs .findMatch,
	.monaco-editor.vs-dark .findMatch,
	.monaco-editor.vs .currentFindMatch,
	.monaco-editor.vs-dark .currentFindMatch {
		border: 2px dotted activeborder !important;
		background: transparent !important;
		box-sizing: border-box;
	}
	.monaco-editor.vs .find-widget,
	.monaco-editor.vs-dark .find-widget {
		border: 1px solid windowtext;
	}

	/* list - used by suggest widget */
	.monaco-editor.vs .monaco-list .monaco-list-row,
	.monaco-editor.vs-dark .monaco-list .monaco-list-row {
		-ms-high-contrast-adjust: none;
		color: windowtext !important;
	}
	.monaco-editor.vs .monaco-list .monaco-list-row.focused,
	.monaco-editor.vs-dark .monaco-list .monaco-list-row.focused {
		color: highlighttext !important;
		background-color: highlight !important;
	}
	.monaco-editor.vs .monaco-list .monaco-list-row:hover,
	.monaco-editor.vs-dark .monaco-list .monaco-list-row:hover {
		background: transparent !important;
		border: 1px solid highlight;
		box-sizing: border-box;
	}

	/* scrollbars */
	.monaco-editor.vs .monaco-scrollable-element > .scrollbar,
	.monaco-editor.vs-dark .monaco-scrollable-element > .scrollbar {
		-ms-high-contrast-adjust: none;
		background: background !important;
		border: 1px solid windowtext;
		box-sizing: border-box;
	}
	.monaco-editor.vs .monaco-scrollable-element > .scrollbar > .slider,
	.monaco-editor.vs-dark .monaco-scrollable-element > .scrollbar > .slider {
		background: windowtext !important;
	}
	.monaco-editor.vs .monaco-scrollable-element > .scrollbar > .slider:hover,
	.monaco-editor.vs-dark .monaco-scrollable-element > .scrollbar > .slider:hover {
		background: highlight !important;
	}
	.monaco-editor.vs .monaco-scrollable-element > .scrollbar > .slider.active,
	.monaco-editor.vs-dark .monaco-scrollable-element > .scrollbar > .slider.active {
		background: highlight !important;
	}

	/* overview ruler */
	.monaco-editor.vs .decorationsOverviewRuler,
	.monaco-editor.vs-dark .decorationsOverviewRuler {
		opacity: 0;
	}

	/* minimap */
	.monaco-editor.vs .minimap,
	.monaco-editor.vs-dark .minimap {
		display: none;
	}

	/* squiggles */
	.monaco-editor.vs .squiggly-d-error,
	.monaco-editor.vs-dark .squiggly-d-error {
		background: transparent !important;
		border-bottom: 4px double #E47777;
	}
	.monaco-editor.vs .squiggly-c-warning,
	.monaco-editor.vs-dark .squiggly-c-warning {
		border-bottom: 4px double #71B771;
	}
	.monaco-editor.vs .squiggly-b-info,
	.monaco-editor.vs-dark .squiggly-b-info {
		border-bottom: 4px double #71B771;
	}
	.monaco-editor.vs .squiggly-a-hint,
	.monaco-editor.vs-dark .squiggly-a-hint {
		border-bottom: 4px double #6c6c6c;
	}

	/* contextmenu */
	.monaco-editor.vs .monaco-menu .monaco-action-bar.vertical .action-menu-item:focus .action-label,
	.monaco-editor.vs-dark .monaco-menu .monaco-action-bar.vertical .action-menu-item:focus .action-label {
		-ms-high-contrast-adjust: none;
		color: highlighttext !important;
		background-color: highlight !important;
	}
	.monaco-editor.vs .monaco-menu .monaco-action-bar.vertical .action-menu-item:hover .action-label,
	.monaco-editor.vs-dark .monaco-menu .monaco-action-bar.vertical .action-menu-item:hover .action-label {
		-ms-high-contrast-adjust: none;
		background: transparent !important;
		border: 1px solid highlight;
		box-sizing: border-box;
	}

	/* diff editor */
	.monaco-diff-editor.vs .diffOverviewRuler,
	.monaco-diff-editor.vs-dark .diffOverviewRuler {
		display: none;
	}
	.monaco-editor.vs .line-insert,
	.monaco-editor.vs-dark .line-insert,
	.monaco-editor.vs .line-delete,
	.monaco-editor.vs-dark .line-delete {
		background: transparent !important;
		border: 1px solid highlight !important;
		box-sizing: border-box;
	}
	.monaco-editor.vs .char-insert,
	.monaco-editor.vs-dark .char-insert,
	.monaco-editor.vs .char-delete,
	.monaco-editor.vs-dark .char-delete {
		background: transparent !important;
	}
}

/*.monaco-editor.vs [tabindex="0"]:focus {
	outline: 1px solid rgba(0, 122, 204, 0.4);
	outline-offset: -1px;
	opacity: 1 !important;
}

.monaco-editor.vs-dark [tabindex="0"]:focus {
	outline: 1px solid rgba(14, 99, 156, 0.6);
	outline-offset: -1px;
	opacity: 1 !important;
}*/
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/standalone/browser/standalone-tokens.css"],names:[],mappings:"AAAA;;;+FAG+F;;;AAG/F,oCAAoC;AACpC;CACC,6IAA6I;CAC7I,2IAA2I;AAC5I;;AAEA;CACC,mBAAmB;AACpB;;AAEA;;;CAGC,mBAAmB;AACpB;;AAEA;CACC,SAAS;AACV;;AAEA,sFAAsF;AACtF;CACC,6BAA6B;CAC7B,MAAM,EAAE,oDAAoD;CAC5D,WAAW;CACX,UAAU;CACV,YAAY;CACZ,gBAAgB;CAChB,UAAU;CACV,8BAA8B;CAC9B,qBAAqB;AACtB;;AAEA,0DAA0D;AAC1D;;CAEC,8BAA8B;AAC/B;AACA,8GAA8G;AAC9G;;CAEC,2BAA2B;CAC3B;;EAEC,mCAAmC;EACnC,cAAc;EACd,eAAe;CAChB;;CAEA,iBAAiB;CACjB;;EAEC,uCAAuC;CACxC;CACA,eAAe;CACf;;EAEC,mCAAmC;CACpC;;CAEA,6BAA6B;CAC7B;;EAEC,sCAAsC;CACvC;;CAEA,qDAAqD;CACrD;;EAEC,8BAA8B;CAC/B;;CAEA,eAAe;CACf;;EAEC,4BAA4B;CAC7B;CACA,wBAAwB;CACxB;;EAEC,+BAA+B;CAChC;;CAEA,sBAAsB;CACtB;;EAEC,8BAA8B;CAC/B;;CAEA,wBAAwB;CACxB;;;;;;;;EAQC,uCAAuC;EACvC,kCAAkC;EAClC,sBAAsB;CACvB;CACA;;EAEC,kCAAkC;EAClC,0CAA0C;EAC1C,sBAAsB;CACvB;CACA;;EAEC,mCAAmC;EACnC,kCAAkC;CACnC;;CAEA,gBAAgB;CAChB;;;;EAIC,0CAA0C;EAC1C,kCAAkC;EAClC,sBAAsB;CACvB;CACA;;EAEC,4BAA4B;CAC7B;;CAEA,kCAAkC;CAClC;;EAEC,8BAA8B;EAC9B,4BAA4B;CAC7B;CACA;;EAEC,+BAA+B;EAC/B,sCAAsC;CACvC;CACA;;EAEC,kCAAkC;EAClC,2BAA2B;EAC3B,sBAAsB;CACvB;;CAEA,eAAe;CACf;;EAEC,8BAA8B;EAC9B,iCAAiC;EACjC,4BAA4B;EAC5B,sBAAsB;CACvB;CACA;;EAEC,iCAAiC;CAClC;CACA;;EAEC,gCAAgC;CACjC;CACA;;EAEC,gCAAgC;CACjC;;CAEA,mBAAmB;CACnB;;EAEC,UAAU;CACX;;CAEA,YAAY;CACZ;;EAEC,aAAa;CACd;;CAEA,cAAc;CACd;;EAEC,kCAAkC;EAClC,iCAAiC;CAClC;CACA;;EAEC,iCAAiC;CAClC;CACA;;EAEC,iCAAiC;CAClC;CACA;;EAEC,iCAAiC;CAClC;;CAEA,gBAAgB;CAChB;;EAEC,8BAA8B;EAC9B,+BAA+B;EAC/B,sCAAsC;CACvC;CACA;;EAEC,8BAA8B;EAC9B,kCAAkC;EAClC,2BAA2B;EAC3B,sBAAsB;CACvB;;CAEA,gBAAgB;CAChB;;EAEC,aAAa;CACd;CACA;;;;EAIC,kCAAkC;EAClC,sCAAsC;EACtC,sBAAsB;CACvB;CACA;;;;EAIC,kCAAkC;CACnC;AACD;;AAEA;;;;;;;;;;EAUE",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/


/* Default standalone editor fonts */
.monaco-editor {
	font-family: -apple-system, BlinkMacSystemFont, "Segoe WPC", "Segoe UI", "HelveticaNeue-Light", system-ui, "Ubuntu", "Droid Sans", sans-serif;
	--monaco-monospace-font: "SF Mono", Monaco, Menlo, Consolas, "Ubuntu Mono", "Liberation Mono", "DejaVu Sans Mono", "Courier New", monospace;
}

.monaco-menu .monaco-action-bar.vertical .action-item .action-menu-item:focus .action-label {
	stroke-width: 1.2px;
}

.monaco-editor.vs-dark .monaco-menu .monaco-action-bar.vertical .action-menu-item:focus .action-label,
.monaco-editor.hc-black .monaco-menu .monaco-action-bar.vertical .action-menu-item:focus .action-label,
.monaco-editor.hc-light .monaco-menu .monaco-action-bar.vertical .action-menu-item:focus .action-label {
	stroke-width: 1.2px;
}

.monaco-hover p {
	margin: 0;
}

/* See https://github.com/microsoft/monaco-editor/issues/2168#issuecomment-780078600 */
.monaco-aria-container {
	position: absolute !important;
	top: 0; /* avoid being placed underneath a sibling element */
	height: 1px;
	width: 1px;
	margin: -1px;
	overflow: hidden;
	padding: 0;
	clip: rect(1px, 1px, 1px, 1px);
	clip-path: inset(50%);
}

/* The hc-black theme is already high contrast optimized */
.monaco-editor.hc-black,
.monaco-editor.hc-light {
	-ms-high-contrast-adjust: none;
}
/* In case the browser goes into high contrast mode and the editor is not configured with the hc-black theme */
@media screen and (-ms-high-contrast:active) {

	/* current line highlight */
	.monaco-editor.vs .view-overlays .current-line,
	.monaco-editor.vs-dark .view-overlays .current-line {
		border-color: windowtext !important;
		border-left: 0;
		border-right: 0;
	}

	/* view cursors */
	.monaco-editor.vs .cursor,
	.monaco-editor.vs-dark .cursor {
		background-color: windowtext !important;
	}
	/* dnd target */
	.monaco-editor.vs .dnd-target,
	.monaco-editor.vs-dark .dnd-target {
		border-color: windowtext !important;
	}

	/* selected text background */
	.monaco-editor.vs .selected-text,
	.monaco-editor.vs-dark .selected-text {
		background-color: highlight !important;
	}

	/* allow the text to have a transparent background. */
	.monaco-editor.vs .view-line,
	.monaco-editor.vs-dark .view-line {
		-ms-high-contrast-adjust: none;
	}

	/* text color */
	.monaco-editor.vs .view-line span,
	.monaco-editor.vs-dark .view-line span {
		color: windowtext !important;
	}
	/* selected text color */
	.monaco-editor.vs .view-line span.inline-selected-text,
	.monaco-editor.vs-dark .view-line span.inline-selected-text {
		color: highlighttext !important;
	}

	/* allow decorations */
	.monaco-editor.vs .view-overlays,
	.monaco-editor.vs-dark .view-overlays {
		-ms-high-contrast-adjust: none;
	}

	/* various decorations */
	.monaco-editor.vs .selectionHighlight,
	.monaco-editor.vs-dark .selectionHighlight,
	.monaco-editor.vs .wordHighlight,
	.monaco-editor.vs-dark .wordHighlight,
	.monaco-editor.vs .wordHighlightStrong,
	.monaco-editor.vs-dark .wordHighlightStrong,
	.monaco-editor.vs .reference-decoration,
	.monaco-editor.vs-dark .reference-decoration {
		border: 2px dotted highlight !important;
		background: transparent !important;
		box-sizing: border-box;
	}
	.monaco-editor.vs .rangeHighlight,
	.monaco-editor.vs-dark .rangeHighlight {
		background: transparent !important;
		border: 1px dotted activeborder !important;
		box-sizing: border-box;
	}
	.monaco-editor.vs .bracket-match,
	.monaco-editor.vs-dark .bracket-match {
		border-color: windowtext !important;
		background: transparent !important;
	}

	/* find widget */
	.monaco-editor.vs .findMatch,
	.monaco-editor.vs-dark .findMatch,
	.monaco-editor.vs .currentFindMatch,
	.monaco-editor.vs-dark .currentFindMatch {
		border: 2px dotted activeborder !important;
		background: transparent !important;
		box-sizing: border-box;
	}
	.monaco-editor.vs .find-widget,
	.monaco-editor.vs-dark .find-widget {
		border: 1px solid windowtext;
	}

	/* list - used by suggest widget */
	.monaco-editor.vs .monaco-list .monaco-list-row,
	.monaco-editor.vs-dark .monaco-list .monaco-list-row {
		-ms-high-contrast-adjust: none;
		color: windowtext !important;
	}
	.monaco-editor.vs .monaco-list .monaco-list-row.focused,
	.monaco-editor.vs-dark .monaco-list .monaco-list-row.focused {
		color: highlighttext !important;
		background-color: highlight !important;
	}
	.monaco-editor.vs .monaco-list .monaco-list-row:hover,
	.monaco-editor.vs-dark .monaco-list .monaco-list-row:hover {
		background: transparent !important;
		border: 1px solid highlight;
		box-sizing: border-box;
	}

	/* scrollbars */
	.monaco-editor.vs .monaco-scrollable-element > .scrollbar,
	.monaco-editor.vs-dark .monaco-scrollable-element > .scrollbar {
		-ms-high-contrast-adjust: none;
		background: background !important;
		border: 1px solid windowtext;
		box-sizing: border-box;
	}
	.monaco-editor.vs .monaco-scrollable-element > .scrollbar > .slider,
	.monaco-editor.vs-dark .monaco-scrollable-element > .scrollbar > .slider {
		background: windowtext !important;
	}
	.monaco-editor.vs .monaco-scrollable-element > .scrollbar > .slider:hover,
	.monaco-editor.vs-dark .monaco-scrollable-element > .scrollbar > .slider:hover {
		background: highlight !important;
	}
	.monaco-editor.vs .monaco-scrollable-element > .scrollbar > .slider.active,
	.monaco-editor.vs-dark .monaco-scrollable-element > .scrollbar > .slider.active {
		background: highlight !important;
	}

	/* overview ruler */
	.monaco-editor.vs .decorationsOverviewRuler,
	.monaco-editor.vs-dark .decorationsOverviewRuler {
		opacity: 0;
	}

	/* minimap */
	.monaco-editor.vs .minimap,
	.monaco-editor.vs-dark .minimap {
		display: none;
	}

	/* squiggles */
	.monaco-editor.vs .squiggly-d-error,
	.monaco-editor.vs-dark .squiggly-d-error {
		background: transparent !important;
		border-bottom: 4px double #E47777;
	}
	.monaco-editor.vs .squiggly-c-warning,
	.monaco-editor.vs-dark .squiggly-c-warning {
		border-bottom: 4px double #71B771;
	}
	.monaco-editor.vs .squiggly-b-info,
	.monaco-editor.vs-dark .squiggly-b-info {
		border-bottom: 4px double #71B771;
	}
	.monaco-editor.vs .squiggly-a-hint,
	.monaco-editor.vs-dark .squiggly-a-hint {
		border-bottom: 4px double #6c6c6c;
	}

	/* contextmenu */
	.monaco-editor.vs .monaco-menu .monaco-action-bar.vertical .action-menu-item:focus .action-label,
	.monaco-editor.vs-dark .monaco-menu .monaco-action-bar.vertical .action-menu-item:focus .action-label {
		-ms-high-contrast-adjust: none;
		color: highlighttext !important;
		background-color: highlight !important;
	}
	.monaco-editor.vs .monaco-menu .monaco-action-bar.vertical .action-menu-item:hover .action-label,
	.monaco-editor.vs-dark .monaco-menu .monaco-action-bar.vertical .action-menu-item:hover .action-label {
		-ms-high-contrast-adjust: none;
		background: transparent !important;
		border: 1px solid highlight;
		box-sizing: border-box;
	}

	/* diff editor */
	.monaco-diff-editor.vs .diffOverviewRuler,
	.monaco-diff-editor.vs-dark .diffOverviewRuler {
		display: none;
	}
	.monaco-editor.vs .line-insert,
	.monaco-editor.vs-dark .line-insert,
	.monaco-editor.vs .line-delete,
	.monaco-editor.vs-dark .line-delete {
		background: transparent !important;
		border: 1px solid highlight !important;
		box-sizing: border-box;
	}
	.monaco-editor.vs .char-insert,
	.monaco-editor.vs-dark .char-insert,
	.monaco-editor.vs .char-delete,
	.monaco-editor.vs-dark .char-delete {
		background: transparent !important;
	}
}

/*.monaco-editor.vs [tabindex="0"]:focus {
	outline: 1px solid rgba(0, 122, 204, 0.4);
	outline-offset: -1px;
	opacity: 1 !important;
}

.monaco-editor.vs-dark [tabindex="0"]:focus {
	outline: 1px solid rgba(14, 99, 156, 0.6);
	outline-offset: -1px;
	opacity: 1 !important;
}*/
`],sourceRoot:""}]);const _=e},96861:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(94566),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},75301:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(35038),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},88723:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(96499),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},74639:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(714),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},63470:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(12171),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},17713:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(8970),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},90551:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(81684),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},37905:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(8474),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},48285:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(48134),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},16285:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(1366),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},83005:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(95422),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},67119:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(44959),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},65865:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(266),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},86529:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(44978),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},85329:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(14166),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},917:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(80140),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},41697:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(3474),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},98977:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(94234),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},43839:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(62516),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},66136:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(71963),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},1620:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(74333),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},95592:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(86307),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},402:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(72035),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},6028:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(28405),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},95524:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(83093),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},72544:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(98081),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},27988:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(93777),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},94104:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(6953),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},94921:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(65876),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},4806:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(57375),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},2808:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(73313),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},46820:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(36493),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},48304:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(80213),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},28252:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(81637),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},40884:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(29133),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},67340:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(48829),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},27168:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(2289),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},13837:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(95388),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},32662:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(66931),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},30116:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(99585),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},20917:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(71446),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},38561:(l,A,t)=>{var i=t(85072),s=t.n(i),r=t(97825),a=t.n(r),e=t(77659),_=t.n(e),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),u=t(41113),E=t.n(u),o=t(3614),n={};n.styleTagTransform=E(),n.setAttributes=c(),n.insert=_().bind(null,"head"),n.domAPI=a(),n.insertStyleElement=m();var g=s()(o.A,n),h=o.A&&o.A.locals?o.A.locals:void 0},18880:(l,A,t)=>{l.exports=t.p+"static/img/codicon.b797181c.ttf"}}]);

//# sourceMappingURL=6938.5c518d2c601b4707b633.js.map
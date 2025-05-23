"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[8164,8926],{94566:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},35038:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
}`],sourceRoot:""}]);const i=o},96499:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},714:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},12171:(A,l,t)=>{t.d(l,{A:()=>m});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=t(4417),i=t.n(o),C=new URL(t(18880),t.b),c=_()(a()),d=i()(C);c.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const m=c},8970:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},81684:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},79862:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-dropdown {
	height: 100%;
	padding: 0;
}

.monaco-dropdown > .dropdown-label {
	cursor: pointer;
	height: 100%;
	display: flex;
	align-items: center;
	justify-content: center;
}

.monaco-dropdown > .dropdown-label > .action-label.disabled {
	cursor: default;
}

.monaco-dropdown-with-primary {
	display: flex !important;
	flex-direction: row;
	border-radius: 5px;
}

.monaco-dropdown-with-primary > .action-container > .action-label {
	margin-right: 0;
}

.monaco-dropdown-with-primary > .dropdown-action-container > .monaco-dropdown > .dropdown-label .codicon[class*='codicon-'] {
	font-size: 12px;
	padding-left: 0px;
	padding-right: 0px;
	line-height: 16px;
	margin-left: -3px;
}

.monaco-dropdown-with-primary > .dropdown-action-container > .monaco-dropdown > .dropdown-label > .action-label {
	display: block;
	background-size: 16px;
	background-position: center center;
	background-repeat: no-repeat;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/dropdown/dropdown.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,YAAY;CACZ,UAAU;AACX;;AAEA;CACC,eAAe;CACf,YAAY;CACZ,aAAa;CACb,mBAAmB;CACnB,uBAAuB;AACxB;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,wBAAwB;CACxB,mBAAmB;CACnB,kBAAkB;AACnB;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,eAAe;CACf,iBAAiB;CACjB,kBAAkB;CAClB,iBAAiB;CACjB,iBAAiB;AAClB;;AAEA;CACC,cAAc;CACd,qBAAqB;CACrB,kCAAkC;CAClC,4BAA4B;AAC7B",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-dropdown {
	height: 100%;
	padding: 0;
}

.monaco-dropdown > .dropdown-label {
	cursor: pointer;
	height: 100%;
	display: flex;
	align-items: center;
	justify-content: center;
}

.monaco-dropdown > .dropdown-label > .action-label.disabled {
	cursor: default;
}

.monaco-dropdown-with-primary {
	display: flex !important;
	flex-direction: row;
	border-radius: 5px;
}

.monaco-dropdown-with-primary > .action-container > .action-label {
	margin-right: 0;
}

.monaco-dropdown-with-primary > .dropdown-action-container > .monaco-dropdown > .dropdown-label .codicon[class*='codicon-'] {
	font-size: 12px;
	padding-left: 0px;
	padding-right: 0px;
	line-height: 16px;
	margin-left: -3px;
}

.monaco-dropdown-with-primary > .dropdown-action-container > .monaco-dropdown > .dropdown-label > .action-label {
	display: block;
	background-size: 16px;
	background-position: center center;
	background-repeat: no-repeat;
}
`],sourceRoot:""}]);const i=o},8474:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},96854:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-hover {
	cursor: default;
	position: absolute;
	overflow: hidden;
	z-index: 50;
	user-select: text;
	-webkit-user-select: text;
	-ms-user-select: text;
	box-sizing: initial;
	animation: fadein 100ms linear;
	line-height: 1.5em;
}

.monaco-hover.hidden {
	display: none;
}

.monaco-hover a:hover {
	cursor: pointer;
}

.monaco-hover .hover-contents:not(.html-hover-contents) {
	padding: 4px 8px;
}

.monaco-hover .markdown-hover > .hover-contents:not(.code-hover-contents) {
	max-width: 500px;
	word-wrap: break-word;
}

.monaco-hover .markdown-hover > .hover-contents:not(.code-hover-contents) hr {
	min-width: 100%;
}

.monaco-hover p,
.monaco-hover .code,
.monaco-hover ul {
	margin: 8px 0;
}

.monaco-hover code {
	font-family: var(--monaco-monospace-font);
}

.monaco-hover hr {
	box-sizing: border-box;
	border-left: 0px;
	border-right: 0px;
	margin-top: 4px;
	margin-bottom: -4px;
	margin-left: -8px;
	margin-right: -8px;
	height: 1px;
}

.monaco-hover p:first-child,
.monaco-hover .code:first-child,
.monaco-hover ul:first-child {
	margin-top: 0;
}

.monaco-hover p:last-child,
.monaco-hover .code:last-child,
.monaco-hover ul:last-child {
	margin-bottom: 0;
}

/* MarkupContent Layout */
.monaco-hover ul {
	padding-left: 20px;
}
.monaco-hover ol {
	padding-left: 20px;
}

.monaco-hover li > p {
	margin-bottom: 0;
}

.monaco-hover li > ul {
	margin-top: 0;
}

.monaco-hover code {
	border-radius: 3px;
	padding: 0 0.4em;
}

.monaco-hover .monaco-tokenized-source {
	white-space: pre-wrap;
}

.monaco-hover .hover-row.status-bar {
	font-size: 12px;
	line-height: 22px;
}

.monaco-hover .hover-row.status-bar .actions {
	display: flex;
	padding: 0px 8px;
}

.monaco-hover .hover-row.status-bar .actions .action-container {
	margin-right: 16px;
	cursor: pointer;
}

.monaco-hover .hover-row.status-bar .actions .action-container .action .icon {
	padding-right: 4px;
}

.monaco-hover .markdown-hover .hover-contents .codicon {
	color: inherit;
	font-size: inherit;
	vertical-align: middle;
}

.monaco-hover .hover-contents a.code-link:hover,
.monaco-hover .hover-contents a.code-link {
	color: inherit;
}

.monaco-hover .hover-contents a.code-link:before {
	content: '(';
}

.monaco-hover .hover-contents a.code-link:after {
	content: ')';
}

.monaco-hover .hover-contents a.code-link > span {
	text-decoration: underline;
	/** Hack to force underline to show **/
	border-bottom: 1px solid transparent;
	text-underline-position: under;
}

/** Spans in markdown hovers need a margin-bottom to avoid looking cramped: https://github.com/microsoft/vscode/issues/101496 **/
.monaco-hover .markdown-hover .hover-contents:not(.code-hover-contents):not(.html-hover-contents) span {
	margin-bottom: 4px;
	display: inline-block;
}

.monaco-hover-content .action-container a {
	-webkit-user-select: none;
	user-select: none;
}

.monaco-hover-content .action-container.disabled {
	pointer-events: none;
	opacity: 0.4;
	cursor: default;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/base/browser/ui/hover/hover.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,eAAe;CACf,kBAAkB;CAClB,gBAAgB;CAChB,WAAW;CACX,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;CACrB,mBAAmB;CACnB,8BAA8B;CAC9B,kBAAkB;AACnB;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,gBAAgB;CAChB,qBAAqB;AACtB;;AAEA;CACC,eAAe;AAChB;;AAEA;;;CAGC,aAAa;AACd;;AAEA;CACC,yCAAyC;AAC1C;;AAEA;CACC,sBAAsB;CACtB,gBAAgB;CAChB,iBAAiB;CACjB,eAAe;CACf,mBAAmB;CACnB,iBAAiB;CACjB,kBAAkB;CAClB,WAAW;AACZ;;AAEA;;;CAGC,aAAa;AACd;;AAEA;;;CAGC,gBAAgB;AACjB;;AAEA,yBAAyB;AACzB;CACC,kBAAkB;AACnB;AACA;CACC,kBAAkB;AACnB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,kBAAkB;CAClB,gBAAgB;AACjB;;AAEA;CACC,qBAAqB;AACtB;;AAEA;CACC,eAAe;CACf,iBAAiB;AAClB;;AAEA;CACC,aAAa;CACb,gBAAgB;AACjB;;AAEA;CACC,kBAAkB;CAClB,eAAe;AAChB;;AAEA;CACC,kBAAkB;AACnB;;AAEA;CACC,cAAc;CACd,kBAAkB;CAClB,sBAAsB;AACvB;;AAEA;;CAEC,cAAc;AACf;;AAEA;CACC,YAAY;AACb;;AAEA;CACC,YAAY;AACb;;AAEA;CACC,0BAA0B;CAC1B,sCAAsC;CACtC,oCAAoC;CACpC,8BAA8B;AAC/B;;AAEA,gIAAgI;AAChI;CACC,kBAAkB;CAClB,qBAAqB;AACtB;;AAEA;CACC,yBAAyB;CACzB,iBAAiB;AAClB;;AAEA;CACC,oBAAoB;CACpB,YAAY;CACZ,eAAe;AAChB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-hover {
	cursor: default;
	position: absolute;
	overflow: hidden;
	z-index: 50;
	user-select: text;
	-webkit-user-select: text;
	-ms-user-select: text;
	box-sizing: initial;
	animation: fadein 100ms linear;
	line-height: 1.5em;
}

.monaco-hover.hidden {
	display: none;
}

.monaco-hover a:hover {
	cursor: pointer;
}

.monaco-hover .hover-contents:not(.html-hover-contents) {
	padding: 4px 8px;
}

.monaco-hover .markdown-hover > .hover-contents:not(.code-hover-contents) {
	max-width: 500px;
	word-wrap: break-word;
}

.monaco-hover .markdown-hover > .hover-contents:not(.code-hover-contents) hr {
	min-width: 100%;
}

.monaco-hover p,
.monaco-hover .code,
.monaco-hover ul {
	margin: 8px 0;
}

.monaco-hover code {
	font-family: var(--monaco-monospace-font);
}

.monaco-hover hr {
	box-sizing: border-box;
	border-left: 0px;
	border-right: 0px;
	margin-top: 4px;
	margin-bottom: -4px;
	margin-left: -8px;
	margin-right: -8px;
	height: 1px;
}

.monaco-hover p:first-child,
.monaco-hover .code:first-child,
.monaco-hover ul:first-child {
	margin-top: 0;
}

.monaco-hover p:last-child,
.monaco-hover .code:last-child,
.monaco-hover ul:last-child {
	margin-bottom: 0;
}

/* MarkupContent Layout */
.monaco-hover ul {
	padding-left: 20px;
}
.monaco-hover ol {
	padding-left: 20px;
}

.monaco-hover li > p {
	margin-bottom: 0;
}

.monaco-hover li > ul {
	margin-top: 0;
}

.monaco-hover code {
	border-radius: 3px;
	padding: 0 0.4em;
}

.monaco-hover .monaco-tokenized-source {
	white-space: pre-wrap;
}

.monaco-hover .hover-row.status-bar {
	font-size: 12px;
	line-height: 22px;
}

.monaco-hover .hover-row.status-bar .actions {
	display: flex;
	padding: 0px 8px;
}

.monaco-hover .hover-row.status-bar .actions .action-container {
	margin-right: 16px;
	cursor: pointer;
}

.monaco-hover .hover-row.status-bar .actions .action-container .action .icon {
	padding-right: 4px;
}

.monaco-hover .markdown-hover .hover-contents .codicon {
	color: inherit;
	font-size: inherit;
	vertical-align: middle;
}

.monaco-hover .hover-contents a.code-link:hover,
.monaco-hover .hover-contents a.code-link {
	color: inherit;
}

.monaco-hover .hover-contents a.code-link:before {
	content: '(';
}

.monaco-hover .hover-contents a.code-link:after {
	content: ')';
}

.monaco-hover .hover-contents a.code-link > span {
	text-decoration: underline;
	/** Hack to force underline to show **/
	border-bottom: 1px solid transparent;
	text-underline-position: under;
}

/** Spans in markdown hovers need a margin-bottom to avoid looking cramped: https://github.com/microsoft/vscode/issues/101496 **/
.monaco-hover .markdown-hover .hover-contents:not(.code-hover-contents):not(.html-hover-contents) span {
	margin-bottom: 4px;
	display: inline-block;
}

.monaco-hover-content .action-container a {
	-webkit-user-select: none;
	user-select: none;
}

.monaco-hover-content .action-container.disabled {
	pointer-events: none;
	opacity: 0.4;
	cursor: default;
}
`],sourceRoot:""}]);const i=o},48134:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},1366:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},95422:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},44959:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},266:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},44978:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},14166:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},80140:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},3474:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},94234:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},62516:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},71963:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},74333:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},86307:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},72035:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},28405:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},83093:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
}`],sourceRoot:""}]);const i=o},98081:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},93777:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},6953:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},65876:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},57375:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
}`],sourceRoot:""}]);const i=o},73313:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
}`],sourceRoot:""}]);const i=o},36493:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},80213:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
}`],sourceRoot:""}]);const i=o},81637:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
}`],sourceRoot:""}]);const i=o},29133:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
}`],sourceRoot:""}]);const i=o},48829:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},2289:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},95388:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},66931:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},99585:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},42755:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .selection-anchor {
	background-color: #007ACC;
	width: 2px !important;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/anchorSelect/browser/anchorSelect.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,yBAAyB;CACzB,qBAAqB;AACtB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .selection-anchor {
	background-color: #007ACC;
	width: 2px !important;
}
`],sourceRoot:""}]);const i=o},7997:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .bracket-match {
	box-sizing: border-box;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/bracketMatching/browser/bracketMatching.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,sBAAsB;AACvB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .bracket-match {
	box-sizing: border-box;
}
`],sourceRoot:""}]);const i=o},26550:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .contentWidgets .codicon-light-bulb,
.monaco-editor .contentWidgets .codicon-lightbulb-autofix {
	display: flex;
	align-items: center;
	justify-content: center;
}

.monaco-editor .contentWidgets .codicon-light-bulb:hover,
.monaco-editor .contentWidgets .codicon-lightbulb-autofix:hover {
	cursor: pointer;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/codeAction/browser/lightBulbWidget.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;;CAEC,aAAa;CACb,mBAAmB;CACnB,uBAAuB;AACxB;;AAEA;;CAEC,eAAe;AAChB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .contentWidgets .codicon-light-bulb,
.monaco-editor .contentWidgets .codicon-lightbulb-autofix {
	display: flex;
	align-items: center;
	justify-content: center;
}

.monaco-editor .contentWidgets .codicon-light-bulb:hover,
.monaco-editor .contentWidgets .codicon-lightbulb-autofix:hover {
	cursor: pointer;
}
`],sourceRoot:""}]);const i=o},24259:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.codeActionMenuWidget {
	padding: 8px 0px 8px 0px;
	overflow: auto;
	font-size: 13px;
	border-radius: 5px;
	min-width: 160px;
	z-index: 40;
	display: block;
	/* flex-direction: column;
	flex: 0 1 auto; */
	width: 100%;
	border-width: 0px;
	border-color: none;
	background-color: var(--vscode-menu-background);
	color: var(--vscode-menu-foreground);
	box-shadow: rgb(0,0,0, 16%) 0px 2px 8px;
}

.codeActionMenuWidget .monaco-list:not(.element-focused):focus:before {
	position: absolute;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
	z-index: 5; /* make sure we are on top of the tree items */
	content: "";
	pointer-events: none; /* enable click through */
	outline: 0px solid !important; /* we still need to handle the empty tree or no focus item case */
	outline-width: 0px !important;
	outline-style: none !important;
	outline-offset: 0px !important;
}

.codeActionMenuWidget .monaco-list {
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
	border: none !important;
	border-width: 0px !important;
}

/* .codeActionMenuWidget .monaco-list:not(.element-focus) {
	border: none !important;
	border-width: 0px !important;
} */

.codeActionMenuWidget .monaco-list .monaco-scrollable-element .monaco-list-rows {
	height: 100% !important;
}

.codeActionMenuWidget .monaco-list .monaco-scrollable-element {
	overflow: visible;
}
/** Styles for each row in the list element **/

.codeActionMenuWidget .monaco-list .monaco-list-row:not(.separator) {
	display: flex;
	-mox-box-sizing: border-box;
	box-sizing: border-box;
	padding: 0px 26px 0px 26px;
	background-repeat: no-repeat;
	background-position: 2px 2px;
	white-space: nowrap;
	cursor: pointer;
	touch-action: none;
	width: 100%;
}


.codeActionMenuWidget .monaco-list .monaco-list-row:hover:not(.option-disabled),
.codeActionMenuWidget .monaco-list .moncao-list-row.focused:not(.option-disabled) {
	color: var(--vscode-menu-selectionForeground) !important;
	background-color: var(--vscode-menu-selectionBackground) !important;
}

.codeActionMenuWidget .monaco-list .option-disabled,
.codeActionMenuWidget .monaco-list .option-disabled .focused {
	pointer-events: none;
	-webkit-touch-callout: none;
	-webkit-user-select: none;
	-khtml-user-select: none;
	-moz-user-select: none;
	-ms-user-select: none;
	user-select: none;
	color: var(--vscode-disabledForeground) !important;
}

.codeActionMenuWidget .monaco-list .separator {
	border-bottom: 1px solid var(--vscode-menu-separatorBackground);
	padding-top: 0px !important;
	/* padding: 30px; */
	width: 100%;
	height: 0px !important;
	opacity: 1;
	font-size: inherit;
	margin: 5px 0 !important;
	border-radius: 0;
	display: flex;
	-mox-box-sizing: border-box;
	box-sizing: border-box;
	background-repeat: no-repeat;
	background-position: 2px 2px;
	white-space: nowrap;
	cursor: pointer;
	touch-action: none;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/codeAction/browser/media/action.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,wBAAwB;CACxB,cAAc;CACd,eAAe;CACf,kBAAkB;CAClB,gBAAgB;CAChB,WAAW;CACX,cAAc;CACd;kBACiB;CACjB,WAAW;CACX,iBAAiB;CACjB,kBAAkB;CAClB,+CAA+C;CAC/C,oCAAoC;CACpC,uCAAuC;AACxC;;AAEA;CACC,kBAAkB;CAClB,MAAM;CACN,OAAO;CACP,WAAW;CACX,YAAY;CACZ,UAAU,EAAE,8CAA8C;CAC1D,WAAW;CACX,oBAAoB,EAAE,yBAAyB;CAC/C,6BAA6B,EAAE,iEAAiE;CAChG,6BAA6B;CAC7B,8BAA8B;CAC9B,8BAA8B;AAC/B;;AAEA;CACC,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;CACrB,uBAAuB;CACvB,4BAA4B;AAC7B;;AAEA;;;GAGG;;AAEH;CACC,uBAAuB;AACxB;;AAEA;CACC,iBAAiB;AAClB;AACA,8CAA8C;;AAE9C;CACC,aAAa;CACb,2BAA2B;CAC3B,sBAAsB;CACtB,0BAA0B;CAC1B,4BAA4B;CAC5B,4BAA4B;CAC5B,mBAAmB;CACnB,eAAe;CACf,kBAAkB;CAClB,WAAW;AACZ;;;AAGA;;CAEC,wDAAwD;CACxD,mEAAmE;AACpE;;AAEA;;CAEC,oBAAoB;CACpB,2BAA2B;CAC3B,yBAAyB;CACzB,wBAAwB;CACxB,sBAAsB;CACtB,qBAAqB;CACrB,iBAAiB;CACjB,kDAAkD;AACnD;;AAEA;CACC,+DAA+D;CAC/D,2BAA2B;CAC3B,mBAAmB;CACnB,WAAW;CACX,sBAAsB;CACtB,UAAU;CACV,kBAAkB;CAClB,wBAAwB;CACxB,gBAAgB;CAChB,aAAa;CACb,2BAA2B;CAC3B,sBAAsB;CACtB,4BAA4B;CAC5B,4BAA4B;CAC5B,mBAAmB;CACnB,eAAe;CACf,kBAAkB;AACnB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.codeActionMenuWidget {
	padding: 8px 0px 8px 0px;
	overflow: auto;
	font-size: 13px;
	border-radius: 5px;
	min-width: 160px;
	z-index: 40;
	display: block;
	/* flex-direction: column;
	flex: 0 1 auto; */
	width: 100%;
	border-width: 0px;
	border-color: none;
	background-color: var(--vscode-menu-background);
	color: var(--vscode-menu-foreground);
	box-shadow: rgb(0,0,0, 16%) 0px 2px 8px;
}

.codeActionMenuWidget .monaco-list:not(.element-focused):focus:before {
	position: absolute;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
	z-index: 5; /* make sure we are on top of the tree items */
	content: "";
	pointer-events: none; /* enable click through */
	outline: 0px solid !important; /* we still need to handle the empty tree or no focus item case */
	outline-width: 0px !important;
	outline-style: none !important;
	outline-offset: 0px !important;
}

.codeActionMenuWidget .monaco-list {
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
	border: none !important;
	border-width: 0px !important;
}

/* .codeActionMenuWidget .monaco-list:not(.element-focus) {
	border: none !important;
	border-width: 0px !important;
} */

.codeActionMenuWidget .monaco-list .monaco-scrollable-element .monaco-list-rows {
	height: 100% !important;
}

.codeActionMenuWidget .monaco-list .monaco-scrollable-element {
	overflow: visible;
}
/** Styles for each row in the list element **/

.codeActionMenuWidget .monaco-list .monaco-list-row:not(.separator) {
	display: flex;
	-mox-box-sizing: border-box;
	box-sizing: border-box;
	padding: 0px 26px 0px 26px;
	background-repeat: no-repeat;
	background-position: 2px 2px;
	white-space: nowrap;
	cursor: pointer;
	touch-action: none;
	width: 100%;
}


.codeActionMenuWidget .monaco-list .monaco-list-row:hover:not(.option-disabled),
.codeActionMenuWidget .monaco-list .moncao-list-row.focused:not(.option-disabled) {
	color: var(--vscode-menu-selectionForeground) !important;
	background-color: var(--vscode-menu-selectionBackground) !important;
}

.codeActionMenuWidget .monaco-list .option-disabled,
.codeActionMenuWidget .monaco-list .option-disabled .focused {
	pointer-events: none;
	-webkit-touch-callout: none;
	-webkit-user-select: none;
	-khtml-user-select: none;
	-moz-user-select: none;
	-ms-user-select: none;
	user-select: none;
	color: var(--vscode-disabledForeground) !important;
}

.codeActionMenuWidget .monaco-list .separator {
	border-bottom: 1px solid var(--vscode-menu-separatorBackground);
	padding-top: 0px !important;
	/* padding: 30px; */
	width: 100%;
	height: 0px !important;
	opacity: 1;
	font-size: inherit;
	margin: 5px 0 !important;
	border-radius: 0;
	display: flex;
	-mox-box-sizing: border-box;
	box-sizing: border-box;
	background-repeat: no-repeat;
	background-position: 2px 2px;
	white-space: nowrap;
	cursor: pointer;
	touch-action: none;
}
`],sourceRoot:""}]);const i=o},61727:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .codelens-decoration {
	overflow: hidden;
	display: inline-block;
	text-overflow: ellipsis;
	white-space: nowrap;
	color: var(--vscode-editorCodeLens-foreground)
}

.monaco-editor .codelens-decoration>span,
.monaco-editor .codelens-decoration>a {
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
	white-space: nowrap;
	vertical-align: sub;
}

.monaco-editor .codelens-decoration>a {
	text-decoration: none;
}

.monaco-editor .codelens-decoration>a:hover {
	cursor: pointer;
	color: var(--vscode-editorLink-activeForeground) !important;
}

.monaco-editor .codelens-decoration>a:hover .codicon {
	color: var(--vscode-editorLink-activeForeground) !important;
}

.monaco-editor .codelens-decoration .codicon {
	vertical-align: middle;
	color: currentColor !important;
	color: var(--vscode-editorCodeLens-foreground);
}

.monaco-editor .codelens-decoration>a:hover .codicon::before {
	cursor: pointer;
}

@keyframes fadein {
	0% {
		opacity: 0;
		visibility: visible;
	}

	100% {
		opacity: 1;
	}
}

.monaco-editor .codelens-decoration.fadein {
	animation: fadein 0.1s linear;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/codelens/browser/codelensWidget.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,gBAAgB;CAChB,qBAAqB;CACrB,uBAAuB;CACvB,mBAAmB;CACnB;AACD;;AAEA;;CAEC,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;CACrB,mBAAmB;CACnB,mBAAmB;AACpB;;AAEA;CACC,qBAAqB;AACtB;;AAEA;CACC,eAAe;CACf,2DAA2D;AAC5D;;AAEA;CACC,2DAA2D;AAC5D;;AAEA;CACC,sBAAsB;CACtB,8BAA8B;CAC9B,8CAA8C;AAC/C;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC;EACC,UAAU;EACV,mBAAmB;CACpB;;CAEA;EACC,UAAU;CACX;AACD;;AAEA;CACC,6BAA6B;AAC9B",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .codelens-decoration {
	overflow: hidden;
	display: inline-block;
	text-overflow: ellipsis;
	white-space: nowrap;
	color: var(--vscode-editorCodeLens-foreground)
}

.monaco-editor .codelens-decoration>span,
.monaco-editor .codelens-decoration>a {
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
	white-space: nowrap;
	vertical-align: sub;
}

.monaco-editor .codelens-decoration>a {
	text-decoration: none;
}

.monaco-editor .codelens-decoration>a:hover {
	cursor: pointer;
	color: var(--vscode-editorLink-activeForeground) !important;
}

.monaco-editor .codelens-decoration>a:hover .codicon {
	color: var(--vscode-editorLink-activeForeground) !important;
}

.monaco-editor .codelens-decoration .codicon {
	vertical-align: middle;
	color: currentColor !important;
	color: var(--vscode-editorCodeLens-foreground);
}

.monaco-editor .codelens-decoration>a:hover .codicon::before {
	cursor: pointer;
}

@keyframes fadein {
	0% {
		opacity: 0;
		visibility: visible;
	}

	100% {
		opacity: 1;
	}
}

.monaco-editor .codelens-decoration.fadein {
	animation: fadein 0.1s linear;
}
`],sourceRoot:""}]);const i=o},53345:(A,l,t)=>{t.d(l,{A:()=>m});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=t(4417),i=t.n(o),C=new URL(t(68968),t.b),c=_()(a()),d=i()(C);c.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.colorpicker-widget {
	height: 190px;
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

/* Decoration */

.colorpicker-color-decoration,
.hc-light .colorpicker-color-decoration {
	border: solid 0.1em #000;
	box-sizing: border-box;
	margin: 0.1em 0.2em 0 0.2em;
	width: 0.8em;
	height: 0.8em;
	line-height: 0.8em;
	display: inline-block;
	cursor: pointer;
}

.hc-black .colorpicker-color-decoration,
.vs-dark .colorpicker-color-decoration {
	border: solid 0.1em #eee;
}

/* Header */

.colorpicker-header {
	display: flex;
	height: 24px;
	position: relative;
	background: url(${d});
	background-size: 9px 9px;
	image-rendering: pixelated;
}

.colorpicker-header .picked-color {
	width: 216px;
	display: flex;
	align-items: center;
	justify-content: center;
	line-height: 24px;
	cursor: pointer;
	color: white;
	flex: 1;
}

.colorpicker-header .picked-color .codicon {
	color: inherit;
	font-size: 14px;
	position: absolute;
	left: 8px;
}

.colorpicker-header .picked-color.light {
	color: black;
}

.colorpicker-header .original-color {
	width: 74px;
	z-index: inherit;
	cursor: pointer;
}


/* Body */

.colorpicker-body {
	display: flex;
	padding: 8px;
	position: relative;
}

.colorpicker-body .saturation-wrap {
	overflow: hidden;
	height: 150px;
	position: relative;
	min-width: 220px;
	flex: 1;
}

.colorpicker-body .saturation-box {
	height: 150px;
	position: absolute;
}

.colorpicker-body .saturation-selection {
	width: 9px;
	height: 9px;
	margin: -5px 0 0 -5px;
	border: 1px solid rgb(255, 255, 255);
	border-radius: 100%;
	box-shadow: 0px 0px 2px rgba(0, 0, 0, 0.8);
	position: absolute;
}

.colorpicker-body .strip {
	width: 25px;
	height: 150px;
}

.colorpicker-body .hue-strip {
	position: relative;
	margin-left: 8px;
	cursor: grab;
	background: linear-gradient(to bottom, #ff0000 0%, #ffff00 17%, #00ff00 33%, #00ffff 50%, #0000ff 67%, #ff00ff 83%, #ff0000 100%);
}

.colorpicker-body .opacity-strip {
	position: relative;
	margin-left: 8px;
	cursor: grab;
	background: url(${d});
	background-size: 9px 9px;
	image-rendering: pixelated;
}

.colorpicker-body .strip.grabbing {
	cursor: grabbing;
}

.colorpicker-body .slider {
	position: absolute;
	top: 0;
	left: -2px;
	width: calc(100% + 4px);
	height: 4px;
	box-sizing: border-box;
	border: 1px solid rgba(255, 255, 255, 0.71);
	box-shadow: 0px 0px 1px rgba(0, 0, 0, 0.85);
}

.colorpicker-body .strip .overlay {
	height: 150px;
	pointer-events: none;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/colorPicker/browser/colorPicker.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,aAAa;CACb,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;AACtB;;AAEA,eAAe;;AAEf;;CAEC,wBAAwB;CACxB,sBAAsB;CACtB,2BAA2B;CAC3B,YAAY;CACZ,aAAa;CACb,kBAAkB;CAClB,qBAAqB;CACrB,eAAe;AAChB;;AAEA;;CAEC,wBAAwB;AACzB;;AAEA,WAAW;;AAEX;CACC,aAAa;CACb,YAAY;CACZ,kBAAkB;CAClB,mDAAiR;CACjR,wBAAwB;CACxB,0BAA0B;AAC3B;;AAEA;CACC,YAAY;CACZ,aAAa;CACb,mBAAmB;CACnB,uBAAuB;CACvB,iBAAiB;CACjB,eAAe;CACf,YAAY;CACZ,OAAO;AACR;;AAEA;CACC,cAAc;CACd,eAAe;CACf,kBAAkB;CAClB,SAAS;AACV;;AAEA;CACC,YAAY;AACb;;AAEA;CACC,WAAW;CACX,gBAAgB;CAChB,eAAe;AAChB;;;AAGA,SAAS;;AAET;CACC,aAAa;CACb,YAAY;CACZ,kBAAkB;AACnB;;AAEA;CACC,gBAAgB;CAChB,aAAa;CACb,kBAAkB;CAClB,gBAAgB;CAChB,OAAO;AACR;;AAEA;CACC,aAAa;CACb,kBAAkB;AACnB;;AAEA;CACC,UAAU;CACV,WAAW;CACX,qBAAqB;CACrB,oCAAoC;CACpC,mBAAmB;CACnB,0CAA0C;CAC1C,kBAAkB;AACnB;;AAEA;CACC,WAAW;CACX,aAAa;AACd;;AAEA;CACC,kBAAkB;CAClB,gBAAgB;CAChB,YAAY;CACZ,iIAAiI;AAClI;;AAEA;CACC,kBAAkB;CAClB,gBAAgB;CAChB,YAAY;CACZ,mDAAiR;CACjR,wBAAwB;CACxB,0BAA0B;AAC3B;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,kBAAkB;CAClB,MAAM;CACN,UAAU;CACV,uBAAuB;CACvB,WAAW;CACX,sBAAsB;CACtB,2CAA2C;CAC3C,2CAA2C;AAC5C;;AAEA;CACC,aAAa;CACb,oBAAoB;AACrB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.colorpicker-widget {
	height: 190px;
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

/* Decoration */

.colorpicker-color-decoration,
.hc-light .colorpicker-color-decoration {
	border: solid 0.1em #000;
	box-sizing: border-box;
	margin: 0.1em 0.2em 0 0.2em;
	width: 0.8em;
	height: 0.8em;
	line-height: 0.8em;
	display: inline-block;
	cursor: pointer;
}

.hc-black .colorpicker-color-decoration,
.vs-dark .colorpicker-color-decoration {
	border: solid 0.1em #eee;
}

/* Header */

.colorpicker-header {
	display: flex;
	height: 24px;
	position: relative;
	background: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAYAAACp8Z5+AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMTZEaa/1AAAAHUlEQVQYV2PYvXu3JAi7uLiAMaYAjAGTQBPYLQkAa/0Zef3qRswAAAAASUVORK5CYII=");
	background-size: 9px 9px;
	image-rendering: pixelated;
}

.colorpicker-header .picked-color {
	width: 216px;
	display: flex;
	align-items: center;
	justify-content: center;
	line-height: 24px;
	cursor: pointer;
	color: white;
	flex: 1;
}

.colorpicker-header .picked-color .codicon {
	color: inherit;
	font-size: 14px;
	position: absolute;
	left: 8px;
}

.colorpicker-header .picked-color.light {
	color: black;
}

.colorpicker-header .original-color {
	width: 74px;
	z-index: inherit;
	cursor: pointer;
}


/* Body */

.colorpicker-body {
	display: flex;
	padding: 8px;
	position: relative;
}

.colorpicker-body .saturation-wrap {
	overflow: hidden;
	height: 150px;
	position: relative;
	min-width: 220px;
	flex: 1;
}

.colorpicker-body .saturation-box {
	height: 150px;
	position: absolute;
}

.colorpicker-body .saturation-selection {
	width: 9px;
	height: 9px;
	margin: -5px 0 0 -5px;
	border: 1px solid rgb(255, 255, 255);
	border-radius: 100%;
	box-shadow: 0px 0px 2px rgba(0, 0, 0, 0.8);
	position: absolute;
}

.colorpicker-body .strip {
	width: 25px;
	height: 150px;
}

.colorpicker-body .hue-strip {
	position: relative;
	margin-left: 8px;
	cursor: grab;
	background: linear-gradient(to bottom, #ff0000 0%, #ffff00 17%, #00ff00 33%, #00ffff 50%, #0000ff 67%, #ff00ff 83%, #ff0000 100%);
}

.colorpicker-body .opacity-strip {
	position: relative;
	margin-left: 8px;
	cursor: grab;
	background: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAYAAACp8Z5+AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMTZEaa/1AAAAHUlEQVQYV2PYvXu3JAi7uLiAMaYAjAGTQBPYLQkAa/0Zef3qRswAAAAASUVORK5CYII=");
	background-size: 9px 9px;
	image-rendering: pixelated;
}

.colorpicker-body .strip.grabbing {
	cursor: grabbing;
}

.colorpicker-body .slider {
	position: absolute;
	top: 0;
	left: -2px;
	width: calc(100% + 4px);
	height: 4px;
	box-sizing: border-box;
	border: 1px solid rgba(255, 255, 255, 0.71);
	box-shadow: 0px 0px 1px rgba(0, 0, 0, 0.85);
}

.colorpicker-body .strip .overlay {
	height: 150px;
	pointer-events: none;
}
`],sourceRoot:""}]);const m=c},88357:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor.vs .dnd-target,
.monaco-editor.hc-light .dnd-target {
	border-right: 2px dotted black;
	color: white; /* opposite of black */
}
.monaco-editor.vs-dark .dnd-target {
	border-right: 2px dotted #AEAFAD;
	color: #51504f; /* opposite of #AEAFAD */
}
.monaco-editor.hc-black .dnd-target {
	border-right: 2px dotted #fff;
	color: #000; /* opposite of #fff */
}

.monaco-editor.mouse-default .view-lines,
.monaco-editor.vs-dark.mac.mouse-default .view-lines,
.monaco-editor.hc-black.mac.mouse-default .view-lines,
.monaco-editor.hc-light.mac.mouse-default .view-lines {
	cursor: default;
}
.monaco-editor.mouse-copy .view-lines,
.monaco-editor.vs-dark.mac.mouse-copy .view-lines,
.monaco-editor.hc-black.mac.mouse-copy .view-lines,
.monaco-editor.hc-light.mac.mouse-copy .view-lines {
	cursor: copy;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/dnd/browser/dnd.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;;CAEC,8BAA8B;CAC9B,YAAY,EAAE,sBAAsB;AACrC;AACA;CACC,gCAAgC;CAChC,cAAc,EAAE,wBAAwB;AACzC;AACA;CACC,6BAA6B;CAC7B,WAAW,EAAE,qBAAqB;AACnC;;AAEA;;;;CAIC,eAAe;AAChB;AACA;;;;CAIC,YAAY;AACb",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor.vs .dnd-target,
.monaco-editor.hc-light .dnd-target {
	border-right: 2px dotted black;
	color: white; /* opposite of black */
}
.monaco-editor.vs-dark .dnd-target {
	border-right: 2px dotted #AEAFAD;
	color: #51504f; /* opposite of #AEAFAD */
}
.monaco-editor.hc-black .dnd-target {
	border-right: 2px dotted #fff;
	color: #000; /* opposite of #fff */
}

.monaco-editor.mouse-default .view-lines,
.monaco-editor.vs-dark.mac.mouse-default .view-lines,
.monaco-editor.hc-black.mac.mouse-default .view-lines,
.monaco-editor.hc-light.mac.mouse-default .view-lines {
	cursor: default;
}
.monaco-editor.mouse-copy .view-lines,
.monaco-editor.vs-dark.mac.mouse-copy .view-lines,
.monaco-editor.hc-black.mac.mouse-copy .view-lines,
.monaco-editor.hc-light.mac.mouse-copy .view-lines {
	cursor: copy;
}
`],sourceRoot:""}]);const i=o},45395:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* Find widget */
.monaco-editor .find-widget {
	position: absolute;
	z-index: 35;
	height: 33px;
	overflow: hidden;
	line-height: 19px;
	transition: transform 200ms linear;
	padding: 0 4px;
	box-sizing: border-box;
	transform: translateY(calc(-100% - 10px)); /* shadow (10px) */
}

.monaco-workbench.reduce-motion .monaco-editor .find-widget {
	transition: transform 0ms linear;
}

.monaco-editor .find-widget textarea {
	margin: 0px;
}

.monaco-editor .find-widget.hiddenEditor {
	display: none;
}

/* Find widget when replace is toggled on */
.monaco-editor .find-widget.replaceToggled > .replace-part {
	display: flex;
}

.monaco-editor .find-widget.visible  {
	transform: translateY(0);
}

.monaco-editor .find-widget .monaco-inputbox.synthetic-focus {
	outline: 1px solid -webkit-focus-ring-color;
	outline-offset: -1px;
}

.monaco-editor .find-widget .monaco-inputbox .input {
	background-color: transparent;
	min-height: 0;
}

.monaco-editor .find-widget .monaco-findInput .input {
	font-size: 13px;
}

.monaco-editor .find-widget > .find-part,
.monaco-editor .find-widget > .replace-part {
	margin: 4px 0 0 17px;
	font-size: 12px;
	display: flex;
}

.monaco-editor .find-widget > .find-part .monaco-inputbox,
.monaco-editor .find-widget > .replace-part .monaco-inputbox {
	min-height: 25px;
}


.monaco-editor .find-widget > .replace-part .monaco-inputbox > .ibwrapper > .mirror {
	padding-right: 22px;
}

.monaco-editor .find-widget > .find-part .monaco-inputbox > .ibwrapper > .input,
.monaco-editor .find-widget > .find-part .monaco-inputbox > .ibwrapper > .mirror,
.monaco-editor .find-widget > .replace-part .monaco-inputbox > .ibwrapper > .input,
.monaco-editor .find-widget > .replace-part .monaco-inputbox > .ibwrapper > .mirror {
	padding-top: 2px;
	padding-bottom: 2px;
}

.monaco-editor .find-widget > .find-part .find-actions {
	height: 25px;
	display: flex;
	align-items: center;
}

.monaco-editor .find-widget > .replace-part .replace-actions {
	height: 25px;
	display: flex;
	align-items: center;
}

.monaco-editor .find-widget .monaco-findInput {
	vertical-align: middle;
	display: flex;
	flex:1;
}

.monaco-editor .find-widget .monaco-findInput .monaco-scrollable-element {
	/* Make sure textarea inherits the width correctly */
	width: 100%;
}

.monaco-editor .find-widget .monaco-findInput .monaco-scrollable-element .scrollbar.vertical {
	/* Hide vertical scrollbar */
	opacity: 0;
}

.monaco-editor .find-widget .matchesCount {
	display: flex;
	flex: initial;
	margin: 0 0 0 3px;
	padding: 2px 0 0 2px;
	height: 25px;
	vertical-align: middle;
	box-sizing: border-box;
	text-align: center;
	line-height: 23px;
}

.monaco-editor .find-widget .button {
	width: 16px;
	height: 16px;
	padding: 3px;
	border-radius: 5px;
	display: flex;
	flex: initial;
	margin-left: 3px;
	background-position: center center;
	background-repeat: no-repeat;
	cursor: pointer;
	display: flex;
	align-items: center;
	justify-content: center;
}

/* find in selection button */
.monaco-editor .find-widget .codicon-find-selection {
	width: 22px;
	height: 22px;
	padding: 3px;
	border-radius: 5px;
}

.monaco-editor .find-widget .button.left {
	margin-left: 0;
	margin-right: 3px;
}

.monaco-editor .find-widget .button.wide {
	width: auto;
	padding: 1px 6px;
	top: -1px;
}

.monaco-editor .find-widget .button.toggle {
	position: absolute;
	top: 0;
	left: 3px;
	width: 18px;
	height: 100%;
	border-radius: 0;
	box-sizing: border-box;
}

.monaco-editor .find-widget .button.toggle.disabled {
	display: none;
}

.monaco-editor .find-widget .disabled {
	color: var(--vscode-disabledForeground);
	cursor: default;
}

.monaco-editor .find-widget > .replace-part {
	display: none;
}

.monaco-editor .find-widget > .replace-part > .monaco-findInput {
	position: relative;
	display: flex;
	vertical-align: middle;
	flex: auto;
	flex-grow: 0;
	flex-shrink: 0;
}

.monaco-editor .find-widget > .replace-part > .monaco-findInput > .controls {
	position: absolute;
	top: 3px;
	right: 2px;
}

/* REDUCED */
.monaco-editor .find-widget.reduced-find-widget .matchesCount {
	display:none;
}

/* NARROW (SMALLER THAN REDUCED) */
.monaco-editor .find-widget.narrow-find-widget {
	max-width: 257px !important;
}

/* COLLAPSED (SMALLER THAN NARROW) */
.monaco-editor .find-widget.collapsed-find-widget {
	max-width: 170px !important;
}

.monaco-editor .find-widget.collapsed-find-widget .button.previous,
.monaco-editor .find-widget.collapsed-find-widget .button.next,
.monaco-editor .find-widget.collapsed-find-widget .button.replace,
.monaco-editor .find-widget.collapsed-find-widget .button.replace-all,
.monaco-editor .find-widget.collapsed-find-widget > .find-part .monaco-findInput .controls {
	display:none;
}

.monaco-editor .findMatch {
	animation-duration: 0;
	animation-name: inherit !important;
}

.monaco-editor .find-widget .monaco-sash {
	left: 0 !important;
}

.monaco-editor.hc-black .find-widget .button:before {
	position: relative;
	top: 1px;
	left: 2px;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/find/browser/findWidget.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F,gBAAgB;AAChB;CACC,kBAAkB;CAClB,WAAW;CACX,YAAY;CACZ,gBAAgB;CAChB,iBAAiB;CACjB,kCAAkC;CAClC,cAAc;CACd,sBAAsB;CACtB,yCAAyC,EAAE,kBAAkB;AAC9D;;AAEA;CACC,gCAAgC;AACjC;;AAEA;CACC,WAAW;AACZ;;AAEA;CACC,aAAa;AACd;;AAEA,2CAA2C;AAC3C;CACC,aAAa;AACd;;AAEA;CACC,wBAAwB;AACzB;;AAEA;CACC,2CAA2C;CAC3C,oBAAoB;AACrB;;AAEA;CACC,6BAA6B;CAC7B,aAAa;AACd;;AAEA;CACC,eAAe;AAChB;;AAEA;;CAEC,oBAAoB;CACpB,eAAe;CACf,aAAa;AACd;;AAEA;;CAEC,gBAAgB;AACjB;;;AAGA;CACC,mBAAmB;AACpB;;AAEA;;;;CAIC,gBAAgB;CAChB,mBAAmB;AACpB;;AAEA;CACC,YAAY;CACZ,aAAa;CACb,mBAAmB;AACpB;;AAEA;CACC,YAAY;CACZ,aAAa;CACb,mBAAmB;AACpB;;AAEA;CACC,sBAAsB;CACtB,aAAa;CACb,MAAM;AACP;;AAEA;CACC,oDAAoD;CACpD,WAAW;AACZ;;AAEA;CACC,4BAA4B;CAC5B,UAAU;AACX;;AAEA;CACC,aAAa;CACb,aAAa;CACb,iBAAiB;CACjB,oBAAoB;CACpB,YAAY;CACZ,sBAAsB;CACtB,sBAAsB;CACtB,kBAAkB;CAClB,iBAAiB;AAClB;;AAEA;CACC,WAAW;CACX,YAAY;CACZ,YAAY;CACZ,kBAAkB;CAClB,aAAa;CACb,aAAa;CACb,gBAAgB;CAChB,kCAAkC;CAClC,4BAA4B;CAC5B,eAAe;CACf,aAAa;CACb,mBAAmB;CACnB,uBAAuB;AACxB;;AAEA,6BAA6B;AAC7B;CACC,WAAW;CACX,YAAY;CACZ,YAAY;CACZ,kBAAkB;AACnB;;AAEA;CACC,cAAc;CACd,iBAAiB;AAClB;;AAEA;CACC,WAAW;CACX,gBAAgB;CAChB,SAAS;AACV;;AAEA;CACC,kBAAkB;CAClB,MAAM;CACN,SAAS;CACT,WAAW;CACX,YAAY;CACZ,gBAAgB;CAChB,sBAAsB;AACvB;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,uCAAuC;CACvC,eAAe;AAChB;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,kBAAkB;CAClB,aAAa;CACb,sBAAsB;CACtB,UAAU;CACV,YAAY;CACZ,cAAc;AACf;;AAEA;CACC,kBAAkB;CAClB,QAAQ;CACR,UAAU;AACX;;AAEA,YAAY;AACZ;CACC,YAAY;AACb;;AAEA,kCAAkC;AAClC;CACC,2BAA2B;AAC5B;;AAEA,oCAAoC;AACpC;CACC,2BAA2B;AAC5B;;AAEA;;;;;CAKC,YAAY;AACb;;AAEA;CACC,qBAAqB;CACrB,kCAAkC;AACnC;;AAEA;CACC,kBAAkB;AACnB;;AAEA;CACC,kBAAkB;CAClB,QAAQ;CACR,SAAS;AACV",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* Find widget */
.monaco-editor .find-widget {
	position: absolute;
	z-index: 35;
	height: 33px;
	overflow: hidden;
	line-height: 19px;
	transition: transform 200ms linear;
	padding: 0 4px;
	box-sizing: border-box;
	transform: translateY(calc(-100% - 10px)); /* shadow (10px) */
}

.monaco-workbench.reduce-motion .monaco-editor .find-widget {
	transition: transform 0ms linear;
}

.monaco-editor .find-widget textarea {
	margin: 0px;
}

.monaco-editor .find-widget.hiddenEditor {
	display: none;
}

/* Find widget when replace is toggled on */
.monaco-editor .find-widget.replaceToggled > .replace-part {
	display: flex;
}

.monaco-editor .find-widget.visible  {
	transform: translateY(0);
}

.monaco-editor .find-widget .monaco-inputbox.synthetic-focus {
	outline: 1px solid -webkit-focus-ring-color;
	outline-offset: -1px;
}

.monaco-editor .find-widget .monaco-inputbox .input {
	background-color: transparent;
	min-height: 0;
}

.monaco-editor .find-widget .monaco-findInput .input {
	font-size: 13px;
}

.monaco-editor .find-widget > .find-part,
.monaco-editor .find-widget > .replace-part {
	margin: 4px 0 0 17px;
	font-size: 12px;
	display: flex;
}

.monaco-editor .find-widget > .find-part .monaco-inputbox,
.monaco-editor .find-widget > .replace-part .monaco-inputbox {
	min-height: 25px;
}


.monaco-editor .find-widget > .replace-part .monaco-inputbox > .ibwrapper > .mirror {
	padding-right: 22px;
}

.monaco-editor .find-widget > .find-part .monaco-inputbox > .ibwrapper > .input,
.monaco-editor .find-widget > .find-part .monaco-inputbox > .ibwrapper > .mirror,
.monaco-editor .find-widget > .replace-part .monaco-inputbox > .ibwrapper > .input,
.monaco-editor .find-widget > .replace-part .monaco-inputbox > .ibwrapper > .mirror {
	padding-top: 2px;
	padding-bottom: 2px;
}

.monaco-editor .find-widget > .find-part .find-actions {
	height: 25px;
	display: flex;
	align-items: center;
}

.monaco-editor .find-widget > .replace-part .replace-actions {
	height: 25px;
	display: flex;
	align-items: center;
}

.monaco-editor .find-widget .monaco-findInput {
	vertical-align: middle;
	display: flex;
	flex:1;
}

.monaco-editor .find-widget .monaco-findInput .monaco-scrollable-element {
	/* Make sure textarea inherits the width correctly */
	width: 100%;
}

.monaco-editor .find-widget .monaco-findInput .monaco-scrollable-element .scrollbar.vertical {
	/* Hide vertical scrollbar */
	opacity: 0;
}

.monaco-editor .find-widget .matchesCount {
	display: flex;
	flex: initial;
	margin: 0 0 0 3px;
	padding: 2px 0 0 2px;
	height: 25px;
	vertical-align: middle;
	box-sizing: border-box;
	text-align: center;
	line-height: 23px;
}

.monaco-editor .find-widget .button {
	width: 16px;
	height: 16px;
	padding: 3px;
	border-radius: 5px;
	display: flex;
	flex: initial;
	margin-left: 3px;
	background-position: center center;
	background-repeat: no-repeat;
	cursor: pointer;
	display: flex;
	align-items: center;
	justify-content: center;
}

/* find in selection button */
.monaco-editor .find-widget .codicon-find-selection {
	width: 22px;
	height: 22px;
	padding: 3px;
	border-radius: 5px;
}

.monaco-editor .find-widget .button.left {
	margin-left: 0;
	margin-right: 3px;
}

.monaco-editor .find-widget .button.wide {
	width: auto;
	padding: 1px 6px;
	top: -1px;
}

.monaco-editor .find-widget .button.toggle {
	position: absolute;
	top: 0;
	left: 3px;
	width: 18px;
	height: 100%;
	border-radius: 0;
	box-sizing: border-box;
}

.monaco-editor .find-widget .button.toggle.disabled {
	display: none;
}

.monaco-editor .find-widget .disabled {
	color: var(--vscode-disabledForeground);
	cursor: default;
}

.monaco-editor .find-widget > .replace-part {
	display: none;
}

.monaco-editor .find-widget > .replace-part > .monaco-findInput {
	position: relative;
	display: flex;
	vertical-align: middle;
	flex: auto;
	flex-grow: 0;
	flex-shrink: 0;
}

.monaco-editor .find-widget > .replace-part > .monaco-findInput > .controls {
	position: absolute;
	top: 3px;
	right: 2px;
}

/* REDUCED */
.monaco-editor .find-widget.reduced-find-widget .matchesCount {
	display:none;
}

/* NARROW (SMALLER THAN REDUCED) */
.monaco-editor .find-widget.narrow-find-widget {
	max-width: 257px !important;
}

/* COLLAPSED (SMALLER THAN NARROW) */
.monaco-editor .find-widget.collapsed-find-widget {
	max-width: 170px !important;
}

.monaco-editor .find-widget.collapsed-find-widget .button.previous,
.monaco-editor .find-widget.collapsed-find-widget .button.next,
.monaco-editor .find-widget.collapsed-find-widget .button.replace,
.monaco-editor .find-widget.collapsed-find-widget .button.replace-all,
.monaco-editor .find-widget.collapsed-find-widget > .find-part .monaco-findInput .controls {
	display:none;
}

.monaco-editor .findMatch {
	animation-duration: 0;
	animation-name: inherit !important;
}

.monaco-editor .find-widget .monaco-sash {
	left: 0 !important;
}

.monaco-editor.hc-black .find-widget .button:before {
	position: relative;
	top: 1px;
	left: 2px;
}
`],sourceRoot:""}]);const i=o},55405:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .margin-view-overlays .codicon-folding-manual-collapsed,
.monaco-editor .margin-view-overlays .codicon-folding-manual-expanded,
.monaco-editor .margin-view-overlays .codicon-folding-expanded,
.monaco-editor .margin-view-overlays .codicon-folding-collapsed {
	cursor: pointer;
	opacity: 0;
	transition: opacity 0.5s;
	display: flex;
	align-items: center;
	justify-content: center;
	font-size: 140%;
	margin-left: 2px;
}

.monaco-editor .margin-view-overlays:hover .codicon,
.monaco-editor .margin-view-overlays .codicon.codicon-folding-collapsed,
.monaco-editor .margin-view-overlays .codicon.codicon-folding-manual-collapsed,
.monaco-editor .margin-view-overlays .codicon.alwaysShowFoldIcons {
	opacity: 1;
}

.monaco-editor .inline-folded:after {
	color: grey;
	margin: 0.1em 0.2em 0 0.2em;
	content: "\u22EF";
	display: inline;
	line-height: 1em;
	cursor: pointer;
}

`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/folding/browser/folding.css"],names:[],mappings:"AAAA;;;+FAG+F;AAC/F;;;;CAIC,eAAe;CACf,UAAU;CACV,wBAAwB;CACxB,aAAa;CACb,mBAAmB;CACnB,uBAAuB;CACvB,eAAe;CACf,gBAAgB;AACjB;;AAEA;;;;CAIC,UAAU;AACX;;AAEA;CACC,WAAW;CACX,2BAA2B;CAC3B,YAAY;CACZ,eAAe;CACf,gBAAgB;CAChB,eAAe;AAChB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .margin-view-overlays .codicon-folding-manual-collapsed,
.monaco-editor .margin-view-overlays .codicon-folding-manual-expanded,
.monaco-editor .margin-view-overlays .codicon-folding-expanded,
.monaco-editor .margin-view-overlays .codicon-folding-collapsed {
	cursor: pointer;
	opacity: 0;
	transition: opacity 0.5s;
	display: flex;
	align-items: center;
	justify-content: center;
	font-size: 140%;
	margin-left: 2px;
}

.monaco-editor .margin-view-overlays:hover .codicon,
.monaco-editor .margin-view-overlays .codicon.codicon-folding-collapsed,
.monaco-editor .margin-view-overlays .codicon.codicon-folding-manual-collapsed,
.monaco-editor .margin-view-overlays .codicon.alwaysShowFoldIcons {
	opacity: 1;
}

.monaco-editor .inline-folded:after {
	color: grey;
	margin: 0.1em 0.2em 0 0.2em;
	content: "\u22EF";
	display: inline;
	line-height: 1em;
	cursor: pointer;
}

`],sourceRoot:""}]);const i=o},81788:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* marker zone */

.monaco-editor .peekview-widget .head .peekview-title .severity-icon {
	display: inline-block;
	vertical-align: text-top;
	margin-right: 4px;
}

.monaco-editor .marker-widget {
	text-overflow: ellipsis;
	white-space: nowrap;
}

.monaco-editor .marker-widget > .stale {
	opacity: 0.6;
	font-style: italic;
}

.monaco-editor .marker-widget .title {
	display: inline-block;
	padding-right: 5px;
}

.monaco-editor .marker-widget .descriptioncontainer {
	position: absolute;
	white-space: pre;
	user-select: text;
	-webkit-user-select: text;
	-ms-user-select: text;
	padding: 8px 12px 0 20px;
}

.monaco-editor .marker-widget .descriptioncontainer .message {
	display: flex;
	flex-direction: column;
}

.monaco-editor .marker-widget .descriptioncontainer .message .details {
	padding-left: 6px;
}

.monaco-editor .marker-widget .descriptioncontainer .message .source,
.monaco-editor .marker-widget .descriptioncontainer .message span.code {
	opacity: 0.6;
}

.monaco-editor .marker-widget .descriptioncontainer .message a.code-link {
	opacity: 0.6;
	color: inherit;
}

.monaco-editor .marker-widget .descriptioncontainer .message a.code-link:before {
	content: '(';
}

.monaco-editor .marker-widget .descriptioncontainer .message a.code-link:after {
	content: ')';
}

.monaco-editor .marker-widget .descriptioncontainer .message a.code-link > span {
	text-decoration: underline;
	/** Hack to force underline to show **/
	border-bottom: 1px solid transparent;
	text-underline-position: under;
	color: var(--vscode-textLink-foreground);
}

.monaco-editor .marker-widget .descriptioncontainer .message a.code-link > span {
	color: var(--vscode-textLink-activeForeground);
}

.monaco-editor .marker-widget .descriptioncontainer .filename {
	cursor: pointer;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/gotoError/browser/media/gotoErrorWidget.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F,gBAAgB;;AAEhB;CACC,qBAAqB;CACrB,wBAAwB;CACxB,iBAAiB;AAClB;;AAEA;CACC,uBAAuB;CACvB,mBAAmB;AACpB;;AAEA;CACC,YAAY;CACZ,kBAAkB;AACnB;;AAEA;CACC,qBAAqB;CACrB,kBAAkB;AACnB;;AAEA;CACC,kBAAkB;CAClB,gBAAgB;CAChB,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;CACrB,wBAAwB;AACzB;;AAEA;CACC,aAAa;CACb,sBAAsB;AACvB;;AAEA;CACC,iBAAiB;AAClB;;AAEA;;CAEC,YAAY;AACb;;AAEA;CACC,YAAY;CACZ,cAAc;AACf;;AAEA;CACC,YAAY;AACb;;AAEA;CACC,YAAY;AACb;;AAEA;CACC,0BAA0B;CAC1B,sCAAsC;CACtC,oCAAoC;CACpC,8BAA8B;CAC9B,wCAAwC;AACzC;;AAEA;CACC,8CAA8C;AAC/C;;AAEA;CACC,eAAe;AAChB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* marker zone */

.monaco-editor .peekview-widget .head .peekview-title .severity-icon {
	display: inline-block;
	vertical-align: text-top;
	margin-right: 4px;
}

.monaco-editor .marker-widget {
	text-overflow: ellipsis;
	white-space: nowrap;
}

.monaco-editor .marker-widget > .stale {
	opacity: 0.6;
	font-style: italic;
}

.monaco-editor .marker-widget .title {
	display: inline-block;
	padding-right: 5px;
}

.monaco-editor .marker-widget .descriptioncontainer {
	position: absolute;
	white-space: pre;
	user-select: text;
	-webkit-user-select: text;
	-ms-user-select: text;
	padding: 8px 12px 0 20px;
}

.monaco-editor .marker-widget .descriptioncontainer .message {
	display: flex;
	flex-direction: column;
}

.monaco-editor .marker-widget .descriptioncontainer .message .details {
	padding-left: 6px;
}

.monaco-editor .marker-widget .descriptioncontainer .message .source,
.monaco-editor .marker-widget .descriptioncontainer .message span.code {
	opacity: 0.6;
}

.monaco-editor .marker-widget .descriptioncontainer .message a.code-link {
	opacity: 0.6;
	color: inherit;
}

.monaco-editor .marker-widget .descriptioncontainer .message a.code-link:before {
	content: '(';
}

.monaco-editor .marker-widget .descriptioncontainer .message a.code-link:after {
	content: ')';
}

.monaco-editor .marker-widget .descriptioncontainer .message a.code-link > span {
	text-decoration: underline;
	/** Hack to force underline to show **/
	border-bottom: 1px solid transparent;
	text-underline-position: under;
	color: var(--vscode-textLink-foreground);
}

.monaco-editor .marker-widget .descriptioncontainer .message a.code-link > span {
	color: var(--vscode-textLink-activeForeground);
}

.monaco-editor .marker-widget .descriptioncontainer .filename {
	cursor: pointer;
}
`],sourceRoot:""}]);const i=o},31503:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .goto-definition-link {
	text-decoration: underline;
	cursor: pointer;
}`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/gotoSymbol/browser/link/goToDefinitionAtPosition.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,0BAA0B;CAC1B,eAAe;AAChB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .goto-definition-link {
	text-decoration: underline;
	cursor: pointer;
}`],sourceRoot:""}]);const i=o},26378:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* -- zone widget */
.monaco-editor .zone-widget .zone-widget-container.reference-zone-widget {
	border-top-width: 1px;
	border-bottom-width: 1px;
}

.monaco-editor .reference-zone-widget .inline {
	display: inline-block;
	vertical-align: top;
}

.monaco-editor .reference-zone-widget .messages {
	height: 100%;
	width: 100%;
	text-align: center;
	padding: 3em 0;
}

.monaco-editor .reference-zone-widget .ref-tree {
	line-height: 23px;
	background-color: var(--vscode-peekViewResult-background);
	color: var(--vscode-peekViewResult-lineForeground);
}

.monaco-editor .reference-zone-widget .ref-tree .reference {
	text-overflow: ellipsis;
	overflow: hidden;
}

.monaco-editor .reference-zone-widget .ref-tree .reference-file {
	display: inline-flex;
	width: 100%;
	height: 100%;
	color: var(--vscode-peekViewResult-fileForeground);
}

.monaco-editor .reference-zone-widget .ref-tree .monaco-list:focus .selected .reference-file {
	color: inherit !important;
}

.monaco-editor .reference-zone-widget .ref-tree .monaco-list:focus .monaco-list-rows > .monaco-list-row.selected:not(.highlighted) {
	background-color: var(--vscode-peekViewResult-selectionBackground);
	color: var(--vscode-peekViewResult-selectionForeground) !important;
}

.monaco-editor .reference-zone-widget .ref-tree .reference-file .count {
	margin-right: 12px;
	margin-left: auto;
}

.monaco-editor .reference-zone-widget .ref-tree .referenceMatch .highlight {
	background-color: var(--vscode-peekViewResult-matchHighlightBackground);
}

.monaco-editor .reference-zone-widget .preview .reference-decoration {
	background-color: var(--vscode-peekViewEditor-matchHighlightBackground);
	border: 2px solid var(--vscode-peekViewEditor-matchHighlightBorder);
	box-sizing: border-box;
}

.monaco-editor .reference-zone-widget .preview .monaco-editor .monaco-editor-background,
.monaco-editor .reference-zone-widget .preview .monaco-editor .inputarea.ime-input {
	background-color: var(--vscode-peekViewEditor-background);
}

.monaco-editor .reference-zone-widget .preview .monaco-editor .margin {
	background-color: var(--vscode-peekViewEditorGutter-background);
}

/* High Contrast Theming */

.monaco-editor.hc-black .reference-zone-widget .ref-tree .reference-file,
.monaco-editor.hc-light .reference-zone-widget .ref-tree .reference-file {
	font-weight: bold;
}

.monaco-editor.hc-black .reference-zone-widget .ref-tree .referenceMatch .highlight,
.monaco-editor.hc-light .reference-zone-widget .ref-tree .referenceMatch .highlight {
	border: 1px dotted var(--vscode-contrastActiveBorder, transparent);
	box-sizing: border-box;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/gotoSymbol/browser/peek/referencesWidget.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F,mBAAmB;AACnB;CACC,qBAAqB;CACrB,wBAAwB;AACzB;;AAEA;CACC,qBAAqB;CACrB,mBAAmB;AACpB;;AAEA;CACC,YAAY;CACZ,WAAW;CACX,kBAAkB;CAClB,cAAc;AACf;;AAEA;CACC,iBAAiB;CACjB,yDAAyD;CACzD,kDAAkD;AACnD;;AAEA;CACC,uBAAuB;CACvB,gBAAgB;AACjB;;AAEA;CACC,oBAAoB;CACpB,WAAW;CACX,YAAY;CACZ,kDAAkD;AACnD;;AAEA;CACC,yBAAyB;AAC1B;;AAEA;CACC,kEAAkE;CAClE,kEAAkE;AACnE;;AAEA;CACC,kBAAkB;CAClB,iBAAiB;AAClB;;AAEA;CACC,uEAAuE;AACxE;;AAEA;CACC,uEAAuE;CACvE,mEAAmE;CACnE,sBAAsB;AACvB;;AAEA;;CAEC,yDAAyD;AAC1D;;AAEA;CACC,+DAA+D;AAChE;;AAEA,0BAA0B;;AAE1B;;CAEC,iBAAiB;AAClB;;AAEA;;CAEC,kEAAkE;CAClE,sBAAsB;AACvB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* -- zone widget */
.monaco-editor .zone-widget .zone-widget-container.reference-zone-widget {
	border-top-width: 1px;
	border-bottom-width: 1px;
}

.monaco-editor .reference-zone-widget .inline {
	display: inline-block;
	vertical-align: top;
}

.monaco-editor .reference-zone-widget .messages {
	height: 100%;
	width: 100%;
	text-align: center;
	padding: 3em 0;
}

.monaco-editor .reference-zone-widget .ref-tree {
	line-height: 23px;
	background-color: var(--vscode-peekViewResult-background);
	color: var(--vscode-peekViewResult-lineForeground);
}

.monaco-editor .reference-zone-widget .ref-tree .reference {
	text-overflow: ellipsis;
	overflow: hidden;
}

.monaco-editor .reference-zone-widget .ref-tree .reference-file {
	display: inline-flex;
	width: 100%;
	height: 100%;
	color: var(--vscode-peekViewResult-fileForeground);
}

.monaco-editor .reference-zone-widget .ref-tree .monaco-list:focus .selected .reference-file {
	color: inherit !important;
}

.monaco-editor .reference-zone-widget .ref-tree .monaco-list:focus .monaco-list-rows > .monaco-list-row.selected:not(.highlighted) {
	background-color: var(--vscode-peekViewResult-selectionBackground);
	color: var(--vscode-peekViewResult-selectionForeground) !important;
}

.monaco-editor .reference-zone-widget .ref-tree .reference-file .count {
	margin-right: 12px;
	margin-left: auto;
}

.monaco-editor .reference-zone-widget .ref-tree .referenceMatch .highlight {
	background-color: var(--vscode-peekViewResult-matchHighlightBackground);
}

.monaco-editor .reference-zone-widget .preview .reference-decoration {
	background-color: var(--vscode-peekViewEditor-matchHighlightBackground);
	border: 2px solid var(--vscode-peekViewEditor-matchHighlightBorder);
	box-sizing: border-box;
}

.monaco-editor .reference-zone-widget .preview .monaco-editor .monaco-editor-background,
.monaco-editor .reference-zone-widget .preview .monaco-editor .inputarea.ime-input {
	background-color: var(--vscode-peekViewEditor-background);
}

.monaco-editor .reference-zone-widget .preview .monaco-editor .margin {
	background-color: var(--vscode-peekViewEditorGutter-background);
}

/* High Contrast Theming */

.monaco-editor.hc-black .reference-zone-widget .ref-tree .reference-file,
.monaco-editor.hc-light .reference-zone-widget .ref-tree .reference-file {
	font-weight: bold;
}

.monaco-editor.hc-black .reference-zone-widget .ref-tree .referenceMatch .highlight,
.monaco-editor.hc-light .reference-zone-widget .ref-tree .referenceMatch .highlight {
	border: 1px dotted var(--vscode-contrastActiveBorder, transparent);
	box-sizing: border-box;
}
`],sourceRoot:""}]);const i=o},58169:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .suggest-preview-additional-widget {
	white-space: nowrap;
}

.monaco-editor .suggest-preview-additional-widget .content-spacer {
	color: transparent;
	white-space: pre;
}

.monaco-editor .suggest-preview-additional-widget .button {
	display: inline-block;
	cursor: pointer;
	text-decoration: underline;
	text-underline-position: under;
}

.monaco-editor .ghost-text-hidden {
	opacity: 0;
	font-size: 0;
}

.monaco-editor .ghost-text-decoration {
	font-style: italic;
}

.monaco-editor .suggest-preview-text {
	font-style: italic;
}

.monaco-editor .inline-completion-text-to-replace {
	text-decoration: underline;
	text-underline-position: under;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/inlineCompletions/browser/ghostText.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,mBAAmB;AACpB;;AAEA;CACC,kBAAkB;CAClB,gBAAgB;AACjB;;AAEA;CACC,qBAAqB;CACrB,eAAe;CACf,0BAA0B;CAC1B,8BAA8B;AAC/B;;AAEA;CACC,UAAU;CACV,YAAY;AACb;;AAEA;CACC,kBAAkB;AACnB;;AAEA;CACC,kBAAkB;AACnB;;AAEA;CACC,0BAA0B;CAC1B,8BAA8B;AAC/B",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .suggest-preview-additional-widget {
	white-space: nowrap;
}

.monaco-editor .suggest-preview-additional-widget .content-spacer {
	color: transparent;
	white-space: pre;
}

.monaco-editor .suggest-preview-additional-widget .button {
	display: inline-block;
	cursor: pointer;
	text-decoration: underline;
	text-underline-position: under;
}

.monaco-editor .ghost-text-hidden {
	opacity: 0;
	font-size: 0;
}

.monaco-editor .ghost-text-decoration {
	font-style: italic;
}

.monaco-editor .suggest-preview-text {
	font-style: italic;
}

.monaco-editor .inline-completion-text-to-replace {
	text-decoration: underline;
	text-underline-position: under;
}
`],sourceRoot:""}]);const i=o},1177:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .detected-link,
.monaco-editor .detected-link-active {
	text-decoration: underline;
	text-underline-position: under;
}

.monaco-editor .detected-link-active {
	cursor: pointer;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/links/browser/links.css"],names:[],mappings:"AAAA;;;+FAG+F;AAC/F;;CAEC,0BAA0B;CAC1B,8BAA8B;AAC/B;;AAEA;CACC,eAAe;AAChB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .detected-link,
.monaco-editor .detected-link-active {
	text-decoration: underline;
	text-underline-position: under;
}

.monaco-editor .detected-link-active {
	cursor: pointer;
}
`],sourceRoot:""}]);const i=o},7201:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .monaco-editor-overlaymessage {
	padding-bottom: 8px;
	z-index: 10000;
}

.monaco-editor .monaco-editor-overlaymessage.below {
	padding-bottom: 0;
	padding-top: 8px;
	z-index: 10000;
}

@keyframes fadeIn {
	from { opacity: 0; }
	to { opacity: 1; }
}
.monaco-editor .monaco-editor-overlaymessage.fadeIn {
	animation: fadeIn 150ms ease-out;
}

@keyframes fadeOut {
	from { opacity: 1; }
	to { opacity: 0; }
}
.monaco-editor .monaco-editor-overlaymessage.fadeOut {
	animation: fadeOut 100ms ease-out;
}

.monaco-editor .monaco-editor-overlaymessage .message {
	padding: 1px 4px;
	color: var(--vscode-inputValidation-infoForeground);
	background-color: var(--vscode-inputValidation-infoBackground);
	border: 1px solid var(--vscode-inputValidation-infoBorder);
}

.monaco-editor.hc-black .monaco-editor-overlaymessage .message,
.monaco-editor.hc-light .monaco-editor-overlaymessage .message {
	border-width: 2px;
}

.monaco-editor .monaco-editor-overlaymessage .anchor {
	width: 0 !important;
	height: 0 !important;
	border-color: transparent;
	border-style: solid;
	z-index: 1000;
	border-width: 8px;
	position: absolute;
}

.monaco-editor .monaco-editor-overlaymessage .anchor.top {
	border-bottom-color: var(--vscode-inputValidation-infoBorder);
}

.monaco-editor .monaco-editor-overlaymessage .anchor.below {
	border-top-color: var(--vscode-inputValidation-infoBorder);
}

.monaco-editor .monaco-editor-overlaymessage:not(.below) .anchor.top,
.monaco-editor .monaco-editor-overlaymessage.below .anchor.below {
	display: none;
}

.monaco-editor .monaco-editor-overlaymessage.below .anchor.top {
	display: inherit;
	top: -8px;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/message/browser/messageController.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,mBAAmB;CACnB,cAAc;AACf;;AAEA;CACC,iBAAiB;CACjB,gBAAgB;CAChB,cAAc;AACf;;AAEA;CACC,OAAO,UAAU,EAAE;CACnB,KAAK,UAAU,EAAE;AAClB;AACA;CACC,gCAAgC;AACjC;;AAEA;CACC,OAAO,UAAU,EAAE;CACnB,KAAK,UAAU,EAAE;AAClB;AACA;CACC,iCAAiC;AAClC;;AAEA;CACC,gBAAgB;CAChB,mDAAmD;CACnD,8DAA8D;CAC9D,0DAA0D;AAC3D;;AAEA;;CAEC,iBAAiB;AAClB;;AAEA;CACC,mBAAmB;CACnB,oBAAoB;CACpB,yBAAyB;CACzB,mBAAmB;CACnB,aAAa;CACb,iBAAiB;CACjB,kBAAkB;AACnB;;AAEA;CACC,6DAA6D;AAC9D;;AAEA;CACC,0DAA0D;AAC3D;;AAEA;;CAEC,aAAa;AACd;;AAEA;CACC,gBAAgB;CAChB,SAAS;AACV",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .monaco-editor-overlaymessage {
	padding-bottom: 8px;
	z-index: 10000;
}

.monaco-editor .monaco-editor-overlaymessage.below {
	padding-bottom: 0;
	padding-top: 8px;
	z-index: 10000;
}

@keyframes fadeIn {
	from { opacity: 0; }
	to { opacity: 1; }
}
.monaco-editor .monaco-editor-overlaymessage.fadeIn {
	animation: fadeIn 150ms ease-out;
}

@keyframes fadeOut {
	from { opacity: 1; }
	to { opacity: 0; }
}
.monaco-editor .monaco-editor-overlaymessage.fadeOut {
	animation: fadeOut 100ms ease-out;
}

.monaco-editor .monaco-editor-overlaymessage .message {
	padding: 1px 4px;
	color: var(--vscode-inputValidation-infoForeground);
	background-color: var(--vscode-inputValidation-infoBackground);
	border: 1px solid var(--vscode-inputValidation-infoBorder);
}

.monaco-editor.hc-black .monaco-editor-overlaymessage .message,
.monaco-editor.hc-light .monaco-editor-overlaymessage .message {
	border-width: 2px;
}

.monaco-editor .monaco-editor-overlaymessage .anchor {
	width: 0 !important;
	height: 0 !important;
	border-color: transparent;
	border-style: solid;
	z-index: 1000;
	border-width: 8px;
	position: absolute;
}

.monaco-editor .monaco-editor-overlaymessage .anchor.top {
	border-bottom-color: var(--vscode-inputValidation-infoBorder);
}

.monaco-editor .monaco-editor-overlaymessage .anchor.below {
	border-top-color: var(--vscode-inputValidation-infoBorder);
}

.monaco-editor .monaco-editor-overlaymessage:not(.below) .anchor.top,
.monaco-editor .monaco-editor-overlaymessage.below .anchor.below {
	display: none;
}

.monaco-editor .monaco-editor-overlaymessage.below .anchor.top {
	display: inherit;
	top: -8px;
}
`],sourceRoot:""}]);const i=o},20991:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .parameter-hints-widget {
	/* Must be higher than the sash's z-index and terminal canvases but lower than the suggest widget */
	z-index: 39;
	display: flex;
	flex-direction: column;
	line-height: 1.5em;
	cursor: default;
}

.monaco-editor .parameter-hints-widget > .phwrapper {
	max-width: 440px;
	display: flex;
	flex-direction: row;
}

.monaco-editor .parameter-hints-widget.multiple {
	min-height: 3.3em;
	padding: 0;
}

.monaco-editor .parameter-hints-widget.visible {
	transition: left .05s ease-in-out;
}

.monaco-editor .parameter-hints-widget p,
.monaco-editor .parameter-hints-widget ul {
	margin: 8px 0;
}

.monaco-editor .parameter-hints-widget .monaco-scrollable-element,
.monaco-editor .parameter-hints-widget .body {
	display: flex;
	flex: 1;
	flex-direction: column;
	min-height: 100%;
}

.monaco-editor .parameter-hints-widget .signature {
	padding: 4px 5px;
}

.monaco-editor .parameter-hints-widget .docs {
	padding: 0 10px 0 5px;
	white-space: pre-wrap;
}

.monaco-editor .parameter-hints-widget .docs.empty {
	display: none;
}

.monaco-editor .parameter-hints-widget .docs .markdown-docs {
	white-space: initial;
}

.monaco-editor .parameter-hints-widget .docs .markdown-docs a:hover {
	cursor: pointer;
}

.monaco-editor .parameter-hints-widget .docs .markdown-docs code {
	font-family: var(--monaco-monospace-font);
}

.monaco-editor .parameter-hints-widget .docs  .monaco-tokenized-source,
.monaco-editor .parameter-hints-widget .docs .code {
	white-space: pre-wrap;
}

.monaco-editor .parameter-hints-widget .docs code {
	border-radius: 3px;
	padding: 0 0.4em;
}

.monaco-editor .parameter-hints-widget .controls {
	display: none;
	flex-direction: column;
	align-items: center;
	min-width: 22px;
	justify-content: flex-end;
}

.monaco-editor .parameter-hints-widget.multiple .controls {
	display: flex;
	padding: 0 2px;
}

.monaco-editor .parameter-hints-widget.multiple .button {
	width: 16px;
	height: 16px;
	background-repeat: no-repeat;
	cursor: pointer;
}

.monaco-editor .parameter-hints-widget .button.previous {
	bottom: 24px;
}

.monaco-editor .parameter-hints-widget .overloads {
	text-align: center;
	height: 12px;
	line-height: 12px;
	font-family: var(--monaco-monospace-font);
}

.monaco-editor .parameter-hints-widget .signature .parameter.active {
	font-weight: bold;
}

.monaco-editor .parameter-hints-widget .documentation-parameter > .parameter {
	font-weight: bold;
	margin-right: 0.5em;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/parameterHints/browser/parameterHints.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,mGAAmG;CACnG,WAAW;CACX,aAAa;CACb,sBAAsB;CACtB,kBAAkB;CAClB,eAAe;AAChB;;AAEA;CACC,gBAAgB;CAChB,aAAa;CACb,mBAAmB;AACpB;;AAEA;CACC,iBAAiB;CACjB,UAAU;AACX;;AAEA;CACC,iCAAiC;AAClC;;AAEA;;CAEC,aAAa;AACd;;AAEA;;CAEC,aAAa;CACb,OAAO;CACP,sBAAsB;CACtB,gBAAgB;AACjB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,qBAAqB;CACrB,qBAAqB;AACtB;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,oBAAoB;AACrB;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,yCAAyC;AAC1C;;AAEA;;CAEC,qBAAqB;AACtB;;AAEA;CACC,kBAAkB;CAClB,gBAAgB;AACjB;;AAEA;CACC,aAAa;CACb,sBAAsB;CACtB,mBAAmB;CACnB,eAAe;CACf,yBAAyB;AAC1B;;AAEA;CACC,aAAa;CACb,cAAc;AACf;;AAEA;CACC,WAAW;CACX,YAAY;CACZ,4BAA4B;CAC5B,eAAe;AAChB;;AAEA;CACC,YAAY;AACb;;AAEA;CACC,kBAAkB;CAClB,YAAY;CACZ,iBAAiB;CACjB,yCAAyC;AAC1C;;AAEA;CACC,iBAAiB;AAClB;;AAEA;CACC,iBAAiB;CACjB,mBAAmB;AACpB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .parameter-hints-widget {
	/* Must be higher than the sash's z-index and terminal canvases but lower than the suggest widget */
	z-index: 39;
	display: flex;
	flex-direction: column;
	line-height: 1.5em;
	cursor: default;
}

.monaco-editor .parameter-hints-widget > .phwrapper {
	max-width: 440px;
	display: flex;
	flex-direction: row;
}

.monaco-editor .parameter-hints-widget.multiple {
	min-height: 3.3em;
	padding: 0;
}

.monaco-editor .parameter-hints-widget.visible {
	transition: left .05s ease-in-out;
}

.monaco-editor .parameter-hints-widget p,
.monaco-editor .parameter-hints-widget ul {
	margin: 8px 0;
}

.monaco-editor .parameter-hints-widget .monaco-scrollable-element,
.monaco-editor .parameter-hints-widget .body {
	display: flex;
	flex: 1;
	flex-direction: column;
	min-height: 100%;
}

.monaco-editor .parameter-hints-widget .signature {
	padding: 4px 5px;
}

.monaco-editor .parameter-hints-widget .docs {
	padding: 0 10px 0 5px;
	white-space: pre-wrap;
}

.monaco-editor .parameter-hints-widget .docs.empty {
	display: none;
}

.monaco-editor .parameter-hints-widget .docs .markdown-docs {
	white-space: initial;
}

.monaco-editor .parameter-hints-widget .docs .markdown-docs a:hover {
	cursor: pointer;
}

.monaco-editor .parameter-hints-widget .docs .markdown-docs code {
	font-family: var(--monaco-monospace-font);
}

.monaco-editor .parameter-hints-widget .docs  .monaco-tokenized-source,
.monaco-editor .parameter-hints-widget .docs .code {
	white-space: pre-wrap;
}

.monaco-editor .parameter-hints-widget .docs code {
	border-radius: 3px;
	padding: 0 0.4em;
}

.monaco-editor .parameter-hints-widget .controls {
	display: none;
	flex-direction: column;
	align-items: center;
	min-width: 22px;
	justify-content: flex-end;
}

.monaco-editor .parameter-hints-widget.multiple .controls {
	display: flex;
	padding: 0 2px;
}

.monaco-editor .parameter-hints-widget.multiple .button {
	width: 16px;
	height: 16px;
	background-repeat: no-repeat;
	cursor: pointer;
}

.monaco-editor .parameter-hints-widget .button.previous {
	bottom: 24px;
}

.monaco-editor .parameter-hints-widget .overloads {
	text-align: center;
	height: 12px;
	line-height: 12px;
	font-family: var(--monaco-monospace-font);
}

.monaco-editor .parameter-hints-widget .signature .parameter.active {
	font-weight: bold;
}

.monaco-editor .parameter-hints-widget .documentation-parameter > .parameter {
	font-weight: bold;
	margin-right: 0.5em;
}
`],sourceRoot:""}]);const i=o},69734:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .peekview-widget .head {
	box-sizing: border-box;
	display: flex;
	justify-content: space-between;
	flex-wrap: nowrap;
}

.monaco-editor .peekview-widget .head .peekview-title {
	display: flex;
	align-items: center;
	font-size: 13px;
	margin-left: 20px;
	min-width: 0;
	text-overflow: ellipsis;
	overflow: hidden;
}

.monaco-editor .peekview-widget .head .peekview-title.clickable {
	cursor: pointer;
}

.monaco-editor .peekview-widget .head .peekview-title .dirname:not(:empty) {
	font-size: 0.9em;
	margin-left: 0.5em;
	text-overflow: ellipsis;
	overflow: hidden;
}

.monaco-editor .peekview-widget .head .peekview-title .meta {
	white-space: nowrap;
	overflow: hidden;
	text-overflow: ellipsis;
}

.monaco-editor .peekview-widget .head .peekview-title .dirname {
	white-space: nowrap;
}

.monaco-editor .peekview-widget .head .peekview-title .filename {
	overflow: hidden;
	text-overflow: ellipsis;
	white-space: nowrap;
}

.monaco-editor .peekview-widget .head .peekview-title .meta:not(:empty)::before {
	content: '-';
	padding: 0 0.3em;
}

.monaco-editor .peekview-widget .head .peekview-actions {
	flex: 1;
	text-align: right;
	padding-right: 2px;
}

.monaco-editor .peekview-widget .head .peekview-actions > .monaco-action-bar {
	display: inline-block;
}

.monaco-editor .peekview-widget .head .peekview-actions > .monaco-action-bar,
.monaco-editor .peekview-widget .head .peekview-actions > .monaco-action-bar > .actions-container {
	height: 100%;
}

.monaco-editor .peekview-widget > .body {
	border-top: 1px solid;
	position: relative;
}

.monaco-editor .peekview-widget .head .peekview-title .codicon {
	margin-right: 4px;
}

.monaco-editor .peekview-widget .monaco-list .monaco-list-row.focused .codicon {
	color: inherit !important;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/peekView/browser/media/peekViewWidget.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,sBAAsB;CACtB,aAAa;CACb,8BAA8B;CAC9B,iBAAiB;AAClB;;AAEA;CACC,aAAa;CACb,mBAAmB;CACnB,eAAe;CACf,iBAAiB;CACjB,YAAY;CACZ,uBAAuB;CACvB,gBAAgB;AACjB;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,gBAAgB;CAChB,kBAAkB;CAClB,uBAAuB;CACvB,gBAAgB;AACjB;;AAEA;CACC,mBAAmB;CACnB,gBAAgB;CAChB,uBAAuB;AACxB;;AAEA;CACC,mBAAmB;AACpB;;AAEA;CACC,gBAAgB;CAChB,uBAAuB;CACvB,mBAAmB;AACpB;;AAEA;CACC,YAAY;CACZ,gBAAgB;AACjB;;AAEA;CACC,OAAO;CACP,iBAAiB;CACjB,kBAAkB;AACnB;;AAEA;CACC,qBAAqB;AACtB;;AAEA;;CAEC,YAAY;AACb;;AAEA;CACC,qBAAqB;CACrB,kBAAkB;AACnB;;AAEA;CACC,iBAAiB;AAClB;;AAEA;CACC,yBAAyB;AAC1B",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .peekview-widget .head {
	box-sizing: border-box;
	display: flex;
	justify-content: space-between;
	flex-wrap: nowrap;
}

.monaco-editor .peekview-widget .head .peekview-title {
	display: flex;
	align-items: center;
	font-size: 13px;
	margin-left: 20px;
	min-width: 0;
	text-overflow: ellipsis;
	overflow: hidden;
}

.monaco-editor .peekview-widget .head .peekview-title.clickable {
	cursor: pointer;
}

.monaco-editor .peekview-widget .head .peekview-title .dirname:not(:empty) {
	font-size: 0.9em;
	margin-left: 0.5em;
	text-overflow: ellipsis;
	overflow: hidden;
}

.monaco-editor .peekview-widget .head .peekview-title .meta {
	white-space: nowrap;
	overflow: hidden;
	text-overflow: ellipsis;
}

.monaco-editor .peekview-widget .head .peekview-title .dirname {
	white-space: nowrap;
}

.monaco-editor .peekview-widget .head .peekview-title .filename {
	overflow: hidden;
	text-overflow: ellipsis;
	white-space: nowrap;
}

.monaco-editor .peekview-widget .head .peekview-title .meta:not(:empty)::before {
	content: '-';
	padding: 0 0.3em;
}

.monaco-editor .peekview-widget .head .peekview-actions {
	flex: 1;
	text-align: right;
	padding-right: 2px;
}

.monaco-editor .peekview-widget .head .peekview-actions > .monaco-action-bar {
	display: inline-block;
}

.monaco-editor .peekview-widget .head .peekview-actions > .monaco-action-bar,
.monaco-editor .peekview-widget .head .peekview-actions > .monaco-action-bar > .actions-container {
	height: 100%;
}

.monaco-editor .peekview-widget > .body {
	border-top: 1px solid;
	position: relative;
}

.monaco-editor .peekview-widget .head .peekview-title .codicon {
	margin-right: 4px;
}

.monaco-editor .peekview-widget .monaco-list .monaco-list-row.focused .codicon {
	color: inherit !important;
}
`],sourceRoot:""}]);const i=o},38869:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .rename-box {
	z-index: 100;
	color: inherit;
}

.monaco-editor .rename-box.preview {
	padding: 3px 3px 0 3px;
}

.monaco-editor .rename-box .rename-input {
	padding: 3px;
	width: calc(100% - 6px);
}

.monaco-editor .rename-box .rename-label {
	display: none;
	opacity: .8;
}

.monaco-editor .rename-box.preview .rename-label {
	display: inherit;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/rename/browser/renameInputField.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,YAAY;CACZ,cAAc;AACf;;AAEA;CACC,sBAAsB;AACvB;;AAEA;CACC,YAAY;CACZ,uBAAuB;AACxB;;AAEA;CACC,aAAa;CACb,WAAW;AACZ;;AAEA;CACC,gBAAgB;AACjB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .rename-box {
	z-index: 100;
	color: inherit;
}

.monaco-editor .rename-box.preview {
	padding: 3px 3px 0 3px;
}

.monaco-editor .rename-box .rename-input {
	padding: 3px;
	width: calc(100% - 6px);
}

.monaco-editor .rename-box .rename-label {
	display: none;
	opacity: .8;
}

.monaco-editor .rename-box.preview .rename-label {
	display: inherit;
}
`],sourceRoot:""}]);const i=o},90069:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .snippet-placeholder {
	min-width: 2px;
	outline-style: solid;
	outline-width: 1px;
	background-color: var(--vscode-editor-snippetTabstopHighlightBackground, transparent);
	outline-color: var(--vscode-editor-snippetTabstopHighlightBorder, transparent);
}

.monaco-editor .finish-snippet-placeholder {
	outline-style: solid;
	outline-width: 1px;
	background-color: var(--vscode-editor-snippetFinalTabstopHighlightBackground, transparent);
	outline-color: var(--vscode-editor-snippetFinalTabstopHighlightBorder, transparent);
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/snippet/browser/snippetSession.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,cAAc;CACd,oBAAoB;CACpB,kBAAkB;CAClB,qFAAqF;CACrF,8EAA8E;AAC/E;;AAEA;CACC,oBAAoB;CACpB,kBAAkB;CAClB,0FAA0F;CAC1F,mFAAmF;AACpF",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .snippet-placeholder {
	min-width: 2px;
	outline-style: solid;
	outline-width: 1px;
	background-color: var(--vscode-editor-snippetTabstopHighlightBackground, transparent);
	outline-color: var(--vscode-editor-snippetTabstopHighlightBorder, transparent);
}

.monaco-editor .finish-snippet-placeholder {
	outline-style: solid;
	outline-width: 1px;
	background-color: var(--vscode-editor-snippetFinalTabstopHighlightBackground, transparent);
	outline-color: var(--vscode-editor-snippetFinalTabstopHighlightBorder, transparent);
}
`],sourceRoot:""}]);const i=o},87160:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* Suggest widget*/

.monaco-editor .suggest-widget {
	width: 430px;
	z-index: 40;
	display: flex;
	flex-direction: column;
}

.monaco-editor .suggest-widget.message {
	flex-direction: row;
	align-items: center;
}

.monaco-editor .suggest-widget,
.monaco-editor .suggest-details {
	flex: 0 1 auto;
	width: 100%;
	border-style: solid;
	border-width: 1px;
	border-color: var(--vscode-editorSuggestWidget-border);
	background-color: var(--vscode-editorSuggestWidget-background);
}

.monaco-editor.hc-black .suggest-widget,
.monaco-editor.hc-black .suggest-details,
.monaco-editor.hc-light .suggest-widget,
.monaco-editor.hc-light .suggest-details {
	border-width: 2px;
}

/* Styles for status bar part */


.monaco-editor .suggest-widget .suggest-status-bar {
	box-sizing: border-box;
	display: none;
	flex-flow: row nowrap;
	justify-content: space-between;
	width: 100%;
	font-size: 80%;
	padding: 0 4px 0 4px;
	border-top: 1px solid var(--vscode-editorSuggestWidget-border);
	overflow: hidden;
}

.monaco-editor .suggest-widget.with-status-bar .suggest-status-bar {
	display: flex;
}

.monaco-editor .suggest-widget .suggest-status-bar .left {
	padding-right: 8px;
}

.monaco-editor .suggest-widget.with-status-bar .suggest-status-bar .action-label {
	color: var(--vscode-editorSuggestWidgetStatus-foreground);
}

.monaco-editor .suggest-widget.with-status-bar .suggest-status-bar .action-item:not(:last-of-type) .action-label {
	margin-right: 0;
}

.monaco-editor .suggest-widget.with-status-bar .suggest-status-bar .action-item:not(:last-of-type) .action-label::after {
	content: ', ';
	margin-right: 0.3em;
}

.monaco-editor .suggest-widget.with-status-bar .monaco-list .monaco-list-row>.contents>.main>.right>.readMore,
.monaco-editor .suggest-widget.with-status-bar .monaco-list .monaco-list-row.focused.string-label>.contents>.main>.right>.readMore {
	display: none;
}

.monaco-editor .suggest-widget.with-status-bar:not(.docs-side) .monaco-list .monaco-list-row:hover>.contents>.main>.right.can-expand-details>.details-label {
	width: 100%;
}

/* Styles for Message element for when widget is loading or is empty */

.monaco-editor .suggest-widget>.message {
	padding-left: 22px;
}

/** Styles for the list element **/

.monaco-editor .suggest-widget>.tree {
	height: 100%;
	width: 100%;
}

.monaco-editor .suggest-widget .monaco-list {
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

/** Styles for each row in the list element **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row {
	display: flex;
	-mox-box-sizing: border-box;
	box-sizing: border-box;
	padding-right: 10px;
	background-repeat: no-repeat;
	background-position: 2px 2px;
	white-space: nowrap;
	cursor: pointer;
	touch-action: none;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.focused {
	color: var(--vscode-editorSuggestWidget-selectedForeground);
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.focused .codicon {
	color: var(--vscode-editorSuggestWidget-selectedIconForeground);
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents {
	flex: 1;
	height: 100%;
	overflow: hidden;
	padding-left: 2px;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main {
	display: flex;
	overflow: hidden;
	text-overflow: ellipsis;
	white-space: pre;
	justify-content: space-between;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.left,
.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right {
	display: flex;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row:not(.focused)>.contents>.main .monaco-icon-label {
	color: var(--vscode-editorSuggestWidget-foreground);
}

.monaco-editor .suggest-widget:not(.frozen) .monaco-highlighted-label .highlight {
	font-weight: bold;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main .monaco-highlighted-label .highlight {
	color: var(--vscode-editorSuggestWidget-highlightForeground);
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.focused>.contents>.main .monaco-highlighted-label .highlight {
	color: var(--vscode-editorSuggestWidget-focusHighlightForeground);
}

/** ReadMore Icon styles **/

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.header>.codicon-close,
.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.readMore::before {
	color: inherit;
	opacity: 1;
	font-size: 14px;
	cursor: pointer;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.header>.codicon-close {
	position: absolute;
	top: 6px;
	right: 2px;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.header>.codicon-close:hover,
.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.readMore:hover {
	opacity: 1;
}

/** signature, qualifier, type/details opacity **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.details-label {
	opacity: 0.7;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.left>.signature-label {
	overflow: hidden;
	text-overflow: ellipsis;
	opacity: 0.6;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.left>.qualifier-label {
	margin-left: 12px;
	opacity: 0.4;
	font-size: 85%;
	line-height: initial;
	text-overflow: ellipsis;
	overflow: hidden;
	align-self: center;
}

/** Type Info and icon next to the label in the focused completion item **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.details-label {
	font-size: 85%;
	margin-left: 1.1em;
	overflow: hidden;
	text-overflow: ellipsis;
	white-space: nowrap;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.details-label>.monaco-tokenized-source {
	display: inline;
}

/** Details: if using CompletionItem#details, show on focus **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.details-label {
	display: none;
}

.monaco-editor .suggest-widget:not(.shows-details) .monaco-list .monaco-list-row.focused>.contents>.main>.right>.details-label {
	display: inline;
}

/** Details: if using CompletionItemLabel#details, always show **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row:not(.string-label)>.contents>.main>.right>.details-label,
.monaco-editor .suggest-widget.docs-side .monaco-list .monaco-list-row.focused:not(.string-label)>.contents>.main>.right>.details-label {
	display: inline;
}

/** Ellipsis on hover **/

.monaco-editor .suggest-widget:not(.docs-side) .monaco-list .monaco-list-row.focused:hover>.contents>.main>.right.can-expand-details>.details-label {
	width: calc(100% - 26px);
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.left {
	flex-shrink: 1;
	flex-grow: 1;
	overflow: hidden;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.left>.monaco-icon-label {
	flex-shrink: 0;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row:not(.string-label)>.contents>.main>.left>.monaco-icon-label {
	max-width: 100%;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.string-label>.contents>.main>.left>.monaco-icon-label {
	flex-shrink: 1;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right {
	overflow: hidden;
	flex-shrink: 4;
	max-width: 70%;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.readMore {
	display: inline-block;
	position: absolute;
	right: 10px;
	width: 18px;
	height: 18px;
	visibility: hidden;
}

/** Do NOT display ReadMore when docs is side/below **/

.monaco-editor .suggest-widget.docs-side .monaco-list .monaco-list-row>.contents>.main>.right>.readMore {
	display: none !important;
}

/** Do NOT display ReadMore when using plain CompletionItemLabel (details/documentation might not be resolved) **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.string-label>.contents>.main>.right>.readMore {
	display: none;
}

/** Focused item can show ReadMore, but can't when docs is side/below **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.focused.string-label>.contents>.main>.right>.readMore {
	display: inline-block;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.focused:hover>.contents>.main>.right>.readMore {
	visibility: visible;
}

/** Styles for each row in the list **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .monaco-icon-label.deprecated {
	opacity: 0.66;
	text-decoration: unset;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .monaco-icon-label.deprecated>.monaco-icon-label-container>.monaco-icon-name-container {
	text-decoration: line-through;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .monaco-icon-label::before {
	height: 100%;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .icon {
	display: block;
	height: 16px;
	width: 16px;
	margin-left: 2px;
	background-repeat: no-repeat;
	background-size: 80%;
	background-position: center;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .icon.hide {
	display: none;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .suggest-icon {
	display: flex;
	align-items: center;
	margin-right: 4px;
}

.monaco-editor .suggest-widget.no-icons .monaco-list .monaco-list-row .icon,
.monaco-editor .suggest-widget.no-icons .monaco-list .monaco-list-row .suggest-icon::before {
	display: none;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .icon.customcolor .colorspan {
	margin: 0 0 0 0.3em;
	border: 0.1em solid #000;
	width: 0.7em;
	height: 0.7em;
	display: inline-block;
}

/** Styles for the docs of the completion item in focus **/

.monaco-editor .suggest-details-container {
	z-index: 41;
}

.monaco-editor .suggest-details {
	display: flex;
	flex-direction: column;
	cursor: default;
	color: var(--vscode-editorSuggestWidget-foreground);
}

.monaco-editor .suggest-details.focused {
	border-color: var(--vscode-focusBorder);
}

.monaco-editor .suggest-details a {
	color: var(--vscode-textLink-foreground);
}

.monaco-editor .suggest-details a:hover {
	color: var(--vscode-textLink-activeForeground);
}

.monaco-editor .suggest-details code {
	background-color: var(--vscode-textCodeBlock-background);
}

.monaco-editor .suggest-details.no-docs {
	display: none;
}

.monaco-editor .suggest-details>.monaco-scrollable-element {
	flex: 1;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body {
	box-sizing: border-box;
	height: 100%;
	width: 100%;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.header>.type {
	flex: 2;
	overflow: hidden;
	text-overflow: ellipsis;
	opacity: 0.7;
	white-space: pre;
	margin: 0 24px 0 0;
	padding: 4px 0 12px 5px;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.header>.type.auto-wrap {
	white-space: normal;
	word-break: break-all;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs {
	margin: 0;
	padding: 4px 5px;
	white-space: pre-wrap;
}

.monaco-editor .suggest-details.no-type>.monaco-scrollable-element>.body>.docs {
	margin-right: 24px;
	overflow: hidden;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs {
	padding: 0;
	white-space: initial;
	min-height: calc(1rem + 8px);
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs>div,
.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs>span:not(:empty) {
	padding: 4px 5px;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs>div>p:first-child {
	margin-top: 0;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs>div>p:last-child {
	margin-bottom: 0;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs .monaco-tokenized-source {
	white-space: pre;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs .code {
	white-space: pre-wrap;
	word-wrap: break-word;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs .codicon {
	vertical-align: sub;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>p:empty {
	display: none;
}

.monaco-editor .suggest-details code {
	border-radius: 3px;
	padding: 0 0.4em;
}

.monaco-editor .suggest-details ul {
	padding-left: 20px;
}

.monaco-editor .suggest-details ol {
	padding-left: 20px;
}

.monaco-editor .suggest-details p code {
	font-family: var(--monaco-monospace-font);
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/suggest/browser/media/suggest.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F,kBAAkB;;AAElB;CACC,YAAY;CACZ,WAAW;CACX,aAAa;CACb,sBAAsB;AACvB;;AAEA;CACC,mBAAmB;CACnB,mBAAmB;AACpB;;AAEA;;CAEC,cAAc;CACd,WAAW;CACX,mBAAmB;CACnB,iBAAiB;CACjB,sDAAsD;CACtD,8DAA8D;AAC/D;;AAEA;;;;CAIC,iBAAiB;AAClB;;AAEA,+BAA+B;;;AAG/B;CACC,sBAAsB;CACtB,aAAa;CACb,qBAAqB;CACrB,8BAA8B;CAC9B,WAAW;CACX,cAAc;CACd,oBAAoB;CACpB,8DAA8D;CAC9D,gBAAgB;AACjB;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,kBAAkB;AACnB;;AAEA;CACC,yDAAyD;AAC1D;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,aAAa;CACb,mBAAmB;AACpB;;AAEA;;CAEC,aAAa;AACd;;AAEA;CACC,WAAW;AACZ;;AAEA,sEAAsE;;AAEtE;CACC,kBAAkB;AACnB;;AAEA,kCAAkC;;AAElC;CACC,YAAY;CACZ,WAAW;AACZ;;AAEA;CACC,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;AACtB;;AAEA,8CAA8C;;AAE9C;CACC,aAAa;CACb,2BAA2B;CAC3B,sBAAsB;CACtB,mBAAmB;CACnB,4BAA4B;CAC5B,4BAA4B;CAC5B,mBAAmB;CACnB,eAAe;CACf,kBAAkB;AACnB;;AAEA;CACC,2DAA2D;AAC5D;;AAEA;CACC,+DAA+D;AAChE;;AAEA;CACC,OAAO;CACP,YAAY;CACZ,gBAAgB;CAChB,iBAAiB;AAClB;;AAEA;CACC,aAAa;CACb,gBAAgB;CAChB,uBAAuB;CACvB,gBAAgB;CAChB,8BAA8B;AAC/B;;AAEA;;CAEC,aAAa;AACd;;AAEA;CACC,mDAAmD;AACpD;;AAEA;CACC,iBAAiB;AAClB;;AAEA;CACC,4DAA4D;AAC7D;;AAEA;CACC,iEAAiE;AAClE;;AAEA,2BAA2B;;AAE3B;;CAEC,cAAc;CACd,UAAU;CACV,eAAe;CACf,eAAe;AAChB;;AAEA;CACC,kBAAkB;CAClB,QAAQ;CACR,UAAU;AACX;;AAEA;;CAEC,UAAU;AACX;;AAEA,iDAAiD;;AAEjD;CACC,YAAY;AACb;;AAEA;CACC,gBAAgB;CAChB,uBAAuB;CACvB,YAAY;AACb;;AAEA;CACC,iBAAiB;CACjB,YAAY;CACZ,cAAc;CACd,oBAAoB;CACpB,uBAAuB;CACvB,gBAAgB;CAChB,kBAAkB;AACnB;;AAEA,0EAA0E;;AAE1E;CACC,cAAc;CACd,kBAAkB;CAClB,gBAAgB;CAChB,uBAAuB;CACvB,mBAAmB;AACpB;;AAEA;CACC,eAAe;AAChB;;AAEA,8DAA8D;;AAE9D;CACC,aAAa;AACd;;AAEA;CACC,eAAe;AAChB;;AAEA,iEAAiE;;AAEjE;;CAEC,eAAe;AAChB;;AAEA,wBAAwB;;AAExB;CACC,wBAAwB;AACzB;;AAEA;CACC,cAAc;CACd,YAAY;CACZ,gBAAgB;AACjB;;AAEA;CACC,cAAc;AACf;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,cAAc;AACf;;AAEA;CACC,gBAAgB;CAChB,cAAc;CACd,cAAc;AACf;;AAEA;CACC,qBAAqB;CACrB,kBAAkB;CAClB,WAAW;CACX,WAAW;CACX,YAAY;CACZ,kBAAkB;AACnB;;AAEA,sDAAsD;;AAEtD;CACC,wBAAwB;AACzB;;AAEA,iHAAiH;;AAEjH;CACC,aAAa;AACd;;AAEA,wEAAwE;;AAExE;CACC,qBAAqB;AACtB;;AAEA;CACC,mBAAmB;AACpB;;AAEA,sCAAsC;;AAEtC;CACC,aAAa;CACb,sBAAsB;AACvB;;AAEA;CACC,6BAA6B;AAC9B;;AAEA;CACC,YAAY;AACb;;AAEA;CACC,cAAc;CACd,YAAY;CACZ,WAAW;CACX,gBAAgB;CAChB,4BAA4B;CAC5B,oBAAoB;CACpB,2BAA2B;AAC5B;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,aAAa;CACb,mBAAmB;CACnB,iBAAiB;AAClB;;AAEA;;CAEC,aAAa;AACd;;AAEA;CACC,mBAAmB;CACnB,wBAAwB;CACxB,YAAY;CACZ,aAAa;CACb,qBAAqB;AACtB;;AAEA,0DAA0D;;AAE1D;CACC,WAAW;AACZ;;AAEA;CACC,aAAa;CACb,sBAAsB;CACtB,eAAe;CACf,mDAAmD;AACpD;;AAEA;CACC,uCAAuC;AACxC;;AAEA;CACC,wCAAwC;AACzC;;AAEA;CACC,8CAA8C;AAC/C;;AAEA;CACC,wDAAwD;AACzD;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,OAAO;AACR;;AAEA;CACC,sBAAsB;CACtB,YAAY;CACZ,WAAW;AACZ;;AAEA;CACC,OAAO;CACP,gBAAgB;CAChB,uBAAuB;CACvB,YAAY;CACZ,gBAAgB;CAChB,kBAAkB;CAClB,uBAAuB;AACxB;;AAEA;CACC,mBAAmB;CACnB,qBAAqB;AACtB;;AAEA;CACC,SAAS;CACT,gBAAgB;CAChB,qBAAqB;AACtB;;AAEA;CACC,kBAAkB;CAClB,gBAAgB;AACjB;;AAEA;CACC,UAAU;CACV,oBAAoB;CACpB,4BAA4B;AAC7B;;AAEA;;CAEC,gBAAgB;AACjB;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,gBAAgB;AACjB;;AAEA;CACC,qBAAqB;CACrB,qBAAqB;AACtB;;AAEA;CACC,mBAAmB;AACpB;;AAEA;CACC,aAAa;AACd;;AAEA;CACC,kBAAkB;CAClB,gBAAgB;AACjB;;AAEA;CACC,kBAAkB;AACnB;;AAEA;CACC,kBAAkB;AACnB;;AAEA;CACC,yCAAyC;AAC1C",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* Suggest widget*/

.monaco-editor .suggest-widget {
	width: 430px;
	z-index: 40;
	display: flex;
	flex-direction: column;
}

.monaco-editor .suggest-widget.message {
	flex-direction: row;
	align-items: center;
}

.monaco-editor .suggest-widget,
.monaco-editor .suggest-details {
	flex: 0 1 auto;
	width: 100%;
	border-style: solid;
	border-width: 1px;
	border-color: var(--vscode-editorSuggestWidget-border);
	background-color: var(--vscode-editorSuggestWidget-background);
}

.monaco-editor.hc-black .suggest-widget,
.monaco-editor.hc-black .suggest-details,
.monaco-editor.hc-light .suggest-widget,
.monaco-editor.hc-light .suggest-details {
	border-width: 2px;
}

/* Styles for status bar part */


.monaco-editor .suggest-widget .suggest-status-bar {
	box-sizing: border-box;
	display: none;
	flex-flow: row nowrap;
	justify-content: space-between;
	width: 100%;
	font-size: 80%;
	padding: 0 4px 0 4px;
	border-top: 1px solid var(--vscode-editorSuggestWidget-border);
	overflow: hidden;
}

.monaco-editor .suggest-widget.with-status-bar .suggest-status-bar {
	display: flex;
}

.monaco-editor .suggest-widget .suggest-status-bar .left {
	padding-right: 8px;
}

.monaco-editor .suggest-widget.with-status-bar .suggest-status-bar .action-label {
	color: var(--vscode-editorSuggestWidgetStatus-foreground);
}

.monaco-editor .suggest-widget.with-status-bar .suggest-status-bar .action-item:not(:last-of-type) .action-label {
	margin-right: 0;
}

.monaco-editor .suggest-widget.with-status-bar .suggest-status-bar .action-item:not(:last-of-type) .action-label::after {
	content: ', ';
	margin-right: 0.3em;
}

.monaco-editor .suggest-widget.with-status-bar .monaco-list .monaco-list-row>.contents>.main>.right>.readMore,
.monaco-editor .suggest-widget.with-status-bar .monaco-list .monaco-list-row.focused.string-label>.contents>.main>.right>.readMore {
	display: none;
}

.monaco-editor .suggest-widget.with-status-bar:not(.docs-side) .monaco-list .monaco-list-row:hover>.contents>.main>.right.can-expand-details>.details-label {
	width: 100%;
}

/* Styles for Message element for when widget is loading or is empty */

.monaco-editor .suggest-widget>.message {
	padding-left: 22px;
}

/** Styles for the list element **/

.monaco-editor .suggest-widget>.tree {
	height: 100%;
	width: 100%;
}

.monaco-editor .suggest-widget .monaco-list {
	user-select: none;
	-webkit-user-select: none;
	-ms-user-select: none;
}

/** Styles for each row in the list element **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row {
	display: flex;
	-mox-box-sizing: border-box;
	box-sizing: border-box;
	padding-right: 10px;
	background-repeat: no-repeat;
	background-position: 2px 2px;
	white-space: nowrap;
	cursor: pointer;
	touch-action: none;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.focused {
	color: var(--vscode-editorSuggestWidget-selectedForeground);
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.focused .codicon {
	color: var(--vscode-editorSuggestWidget-selectedIconForeground);
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents {
	flex: 1;
	height: 100%;
	overflow: hidden;
	padding-left: 2px;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main {
	display: flex;
	overflow: hidden;
	text-overflow: ellipsis;
	white-space: pre;
	justify-content: space-between;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.left,
.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right {
	display: flex;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row:not(.focused)>.contents>.main .monaco-icon-label {
	color: var(--vscode-editorSuggestWidget-foreground);
}

.monaco-editor .suggest-widget:not(.frozen) .monaco-highlighted-label .highlight {
	font-weight: bold;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main .monaco-highlighted-label .highlight {
	color: var(--vscode-editorSuggestWidget-highlightForeground);
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.focused>.contents>.main .monaco-highlighted-label .highlight {
	color: var(--vscode-editorSuggestWidget-focusHighlightForeground);
}

/** ReadMore Icon styles **/

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.header>.codicon-close,
.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.readMore::before {
	color: inherit;
	opacity: 1;
	font-size: 14px;
	cursor: pointer;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.header>.codicon-close {
	position: absolute;
	top: 6px;
	right: 2px;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.header>.codicon-close:hover,
.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.readMore:hover {
	opacity: 1;
}

/** signature, qualifier, type/details opacity **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.details-label {
	opacity: 0.7;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.left>.signature-label {
	overflow: hidden;
	text-overflow: ellipsis;
	opacity: 0.6;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.left>.qualifier-label {
	margin-left: 12px;
	opacity: 0.4;
	font-size: 85%;
	line-height: initial;
	text-overflow: ellipsis;
	overflow: hidden;
	align-self: center;
}

/** Type Info and icon next to the label in the focused completion item **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.details-label {
	font-size: 85%;
	margin-left: 1.1em;
	overflow: hidden;
	text-overflow: ellipsis;
	white-space: nowrap;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.details-label>.monaco-tokenized-source {
	display: inline;
}

/** Details: if using CompletionItem#details, show on focus **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.details-label {
	display: none;
}

.monaco-editor .suggest-widget:not(.shows-details) .monaco-list .monaco-list-row.focused>.contents>.main>.right>.details-label {
	display: inline;
}

/** Details: if using CompletionItemLabel#details, always show **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row:not(.string-label)>.contents>.main>.right>.details-label,
.monaco-editor .suggest-widget.docs-side .monaco-list .monaco-list-row.focused:not(.string-label)>.contents>.main>.right>.details-label {
	display: inline;
}

/** Ellipsis on hover **/

.monaco-editor .suggest-widget:not(.docs-side) .monaco-list .monaco-list-row.focused:hover>.contents>.main>.right.can-expand-details>.details-label {
	width: calc(100% - 26px);
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.left {
	flex-shrink: 1;
	flex-grow: 1;
	overflow: hidden;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.left>.monaco-icon-label {
	flex-shrink: 0;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row:not(.string-label)>.contents>.main>.left>.monaco-icon-label {
	max-width: 100%;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.string-label>.contents>.main>.left>.monaco-icon-label {
	flex-shrink: 1;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right {
	overflow: hidden;
	flex-shrink: 4;
	max-width: 70%;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row>.contents>.main>.right>.readMore {
	display: inline-block;
	position: absolute;
	right: 10px;
	width: 18px;
	height: 18px;
	visibility: hidden;
}

/** Do NOT display ReadMore when docs is side/below **/

.monaco-editor .suggest-widget.docs-side .monaco-list .monaco-list-row>.contents>.main>.right>.readMore {
	display: none !important;
}

/** Do NOT display ReadMore when using plain CompletionItemLabel (details/documentation might not be resolved) **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.string-label>.contents>.main>.right>.readMore {
	display: none;
}

/** Focused item can show ReadMore, but can't when docs is side/below **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.focused.string-label>.contents>.main>.right>.readMore {
	display: inline-block;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row.focused:hover>.contents>.main>.right>.readMore {
	visibility: visible;
}

/** Styles for each row in the list **/

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .monaco-icon-label.deprecated {
	opacity: 0.66;
	text-decoration: unset;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .monaco-icon-label.deprecated>.monaco-icon-label-container>.monaco-icon-name-container {
	text-decoration: line-through;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .monaco-icon-label::before {
	height: 100%;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .icon {
	display: block;
	height: 16px;
	width: 16px;
	margin-left: 2px;
	background-repeat: no-repeat;
	background-size: 80%;
	background-position: center;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .icon.hide {
	display: none;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .suggest-icon {
	display: flex;
	align-items: center;
	margin-right: 4px;
}

.monaco-editor .suggest-widget.no-icons .monaco-list .monaco-list-row .icon,
.monaco-editor .suggest-widget.no-icons .monaco-list .monaco-list-row .suggest-icon::before {
	display: none;
}

.monaco-editor .suggest-widget .monaco-list .monaco-list-row .icon.customcolor .colorspan {
	margin: 0 0 0 0.3em;
	border: 0.1em solid #000;
	width: 0.7em;
	height: 0.7em;
	display: inline-block;
}

/** Styles for the docs of the completion item in focus **/

.monaco-editor .suggest-details-container {
	z-index: 41;
}

.monaco-editor .suggest-details {
	display: flex;
	flex-direction: column;
	cursor: default;
	color: var(--vscode-editorSuggestWidget-foreground);
}

.monaco-editor .suggest-details.focused {
	border-color: var(--vscode-focusBorder);
}

.monaco-editor .suggest-details a {
	color: var(--vscode-textLink-foreground);
}

.monaco-editor .suggest-details a:hover {
	color: var(--vscode-textLink-activeForeground);
}

.monaco-editor .suggest-details code {
	background-color: var(--vscode-textCodeBlock-background);
}

.monaco-editor .suggest-details.no-docs {
	display: none;
}

.monaco-editor .suggest-details>.monaco-scrollable-element {
	flex: 1;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body {
	box-sizing: border-box;
	height: 100%;
	width: 100%;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.header>.type {
	flex: 2;
	overflow: hidden;
	text-overflow: ellipsis;
	opacity: 0.7;
	white-space: pre;
	margin: 0 24px 0 0;
	padding: 4px 0 12px 5px;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.header>.type.auto-wrap {
	white-space: normal;
	word-break: break-all;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs {
	margin: 0;
	padding: 4px 5px;
	white-space: pre-wrap;
}

.monaco-editor .suggest-details.no-type>.monaco-scrollable-element>.body>.docs {
	margin-right: 24px;
	overflow: hidden;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs {
	padding: 0;
	white-space: initial;
	min-height: calc(1rem + 8px);
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs>div,
.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs>span:not(:empty) {
	padding: 4px 5px;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs>div>p:first-child {
	margin-top: 0;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs>div>p:last-child {
	margin-bottom: 0;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs .monaco-tokenized-source {
	white-space: pre;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs .code {
	white-space: pre-wrap;
	word-wrap: break-word;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>.docs.markdown-docs .codicon {
	vertical-align: sub;
}

.monaco-editor .suggest-details>.monaco-scrollable-element>.body>p:empty {
	display: none;
}

.monaco-editor .suggest-details code {
	border-radius: 3px;
	padding: 0 0.4em;
}

.monaco-editor .suggest-details ul {
	padding-left: 20px;
}

.monaco-editor .suggest-details ol {
	padding-left: 20px;
}

.monaco-editor .suggest-details p code {
	font-family: var(--monaco-monospace-font);
}
`],sourceRoot:""}]);const i=o},6065:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.editor-banner {
	box-sizing: border-box;
	cursor: default;
	width: 100%;
	font-size: 12px;
	display: flex;
	overflow: visible;

	height: 26px;

	background: var(--vscode-banner-background);
}


.editor-banner .icon-container {
	display: flex;
	flex-shrink: 0;
	align-items: center;
	padding: 0 6px 0 10px;
}

.editor-banner .icon-container.custom-icon {
	background-repeat: no-repeat;
	background-position: center center;
	background-size: 16px;
	width: 16px;
	padding: 0;
	margin: 0 6px 0 10px;
}

.editor-banner .message-container {
	display: flex;
	align-items: center;
	line-height: 26px;
	text-overflow: ellipsis;
	white-space: nowrap;
	overflow: hidden;
}

.editor-banner .message-container p {
	margin-block-start: 0;
	margin-block-end: 0;
}

.editor-banner .message-actions-container {
	flex-grow: 1;
	flex-shrink: 0;
	line-height: 26px;
	margin: 0 4px;
}

.editor-banner .message-actions-container a.monaco-button {
	width: inherit;
	margin: 2px 8px;
	padding: 0px 12px;
}

.editor-banner .message-actions-container a {
	padding: 3px;
	margin-left: 12px;
	text-decoration: underline;
}

.editor-banner .action-container {
	padding: 0 10px 0 6px;
}

.editor-banner {
	background-color: var(--vscode-banner-background);
}

.editor-banner,
.editor-banner .action-container .codicon,
.editor-banner .message-actions-container .monaco-link {
	color: var(--vscode-banner-foreground);
}

.editor-banner .icon-container .codicon {
	color: var(--vscode-banner-iconForeground);
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/unicodeHighlighter/browser/bannerController.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,sBAAsB;CACtB,eAAe;CACf,WAAW;CACX,eAAe;CACf,aAAa;CACb,iBAAiB;;CAEjB,YAAY;;CAEZ,2CAA2C;AAC5C;;;AAGA;CACC,aAAa;CACb,cAAc;CACd,mBAAmB;CACnB,qBAAqB;AACtB;;AAEA;CACC,4BAA4B;CAC5B,kCAAkC;CAClC,qBAAqB;CACrB,WAAW;CACX,UAAU;CACV,oBAAoB;AACrB;;AAEA;CACC,aAAa;CACb,mBAAmB;CACnB,iBAAiB;CACjB,uBAAuB;CACvB,mBAAmB;CACnB,gBAAgB;AACjB;;AAEA;CACC,qBAAqB;CACrB,mBAAmB;AACpB;;AAEA;CACC,YAAY;CACZ,cAAc;CACd,iBAAiB;CACjB,aAAa;AACd;;AAEA;CACC,cAAc;CACd,eAAe;CACf,iBAAiB;AAClB;;AAEA;CACC,YAAY;CACZ,iBAAiB;CACjB,0BAA0B;AAC3B;;AAEA;CACC,qBAAqB;AACtB;;AAEA;CACC,iDAAiD;AAClD;;AAEA;;;CAGC,sCAAsC;AACvC;;AAEA;CACC,0CAA0C;AAC3C",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.editor-banner {
	box-sizing: border-box;
	cursor: default;
	width: 100%;
	font-size: 12px;
	display: flex;
	overflow: visible;

	height: 26px;

	background: var(--vscode-banner-background);
}


.editor-banner .icon-container {
	display: flex;
	flex-shrink: 0;
	align-items: center;
	padding: 0 6px 0 10px;
}

.editor-banner .icon-container.custom-icon {
	background-repeat: no-repeat;
	background-position: center center;
	background-size: 16px;
	width: 16px;
	padding: 0;
	margin: 0 6px 0 10px;
}

.editor-banner .message-container {
	display: flex;
	align-items: center;
	line-height: 26px;
	text-overflow: ellipsis;
	white-space: nowrap;
	overflow: hidden;
}

.editor-banner .message-container p {
	margin-block-start: 0;
	margin-block-end: 0;
}

.editor-banner .message-actions-container {
	flex-grow: 1;
	flex-shrink: 0;
	line-height: 26px;
	margin: 0 4px;
}

.editor-banner .message-actions-container a.monaco-button {
	width: inherit;
	margin: 2px 8px;
	padding: 0px 12px;
}

.editor-banner .message-actions-container a {
	padding: 3px;
	margin-left: 12px;
	text-decoration: underline;
}

.editor-banner .action-container {
	padding: 0 10px 0 6px;
}

.editor-banner {
	background-color: var(--vscode-banner-background);
}

.editor-banner,
.editor-banner .action-container .codicon,
.editor-banner .message-actions-container .monaco-link {
	color: var(--vscode-banner-foreground);
}

.editor-banner .icon-container .codicon {
	color: var(--vscode-banner-iconForeground);
}
`],sourceRoot:""}]);const i=o},18245:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .unicode-highlight {
	border: 1px solid var(--vscode-editorUnicodeHighlight-border);
	background-color: var(--vscode-editorUnicodeHighlight-background);
	box-sizing: border-box;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/unicodeHighlighter/browser/unicodeHighlighter.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,6DAA6D;CAC7D,iEAAiE;CACjE,sBAAsB;AACvB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .unicode-highlight {
	border: 1px solid var(--vscode-editorUnicodeHighlight-border);
	background-color: var(--vscode-editorUnicodeHighlight-background);
	box-sizing: border-box;
}
`],sourceRoot:""}]);const i=o},12889:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .zone-widget {
	position: absolute;
	z-index: 10;
}


.monaco-editor .zone-widget .zone-widget-container {
	border-top-style: solid;
	border-bottom-style: solid;
	border-top-width: 0;
	border-bottom-width: 0;
	position: relative;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/contrib/zoneWidget/browser/zoneWidget.css"],names:[],mappings:"AAAA;;;+FAG+F;AAC/F;CACC,kBAAkB;CAClB,WAAW;AACZ;;;AAGA;CACC,uBAAuB;CACvB,0BAA0B;CAC1B,mBAAmB;CACnB,sBAAsB;CACtB,kBAAkB;AACnB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
.monaco-editor .zone-widget {
	position: absolute;
	z-index: 10;
}


.monaco-editor .zone-widget .zone-widget-container {
	border-top-style: solid;
	border-bottom-style: solid;
	border-top-width: 0;
	border-bottom-width: 0;
	position: relative;
}
`],sourceRoot:""}]);const i=o},3343:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .accessibilityHelpWidget {
	padding: 10px;
	vertical-align: middle;
	overflow: scroll;
}`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/standalone/browser/accessibilityHelp/accessibilityHelp.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,aAAa;CACb,sBAAsB;CACtB,gBAAgB;AACjB",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .accessibilityHelpWidget {
	padding: 10px;
	vertical-align: middle;
	overflow: scroll;
}`],sourceRoot:""}]);const i=o},59337:(A,l,t)=>{t.d(l,{A:()=>u});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=t(4417),i=t.n(o),C=new URL(t(37584),t.b),c=new URL(t(86060),t.b),d=_()(a()),m=i()(C),E=i()(c);d.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .iPadShowKeyboard {
	width: 58px;
	min-width: 0;
	height: 36px;
	min-height: 0;
	margin: 0;
	padding: 0;
	position: absolute;
	resize: none;
	overflow: hidden;
	background: url(${m}) center center no-repeat;
	border: 4px solid #F6F6F6;
	border-radius: 4px;
}

.monaco-editor.vs-dark .iPadShowKeyboard {
	background: url(${E}) center center no-repeat;
	border: 4px solid #252526;
}`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/standalone/browser/iPadShowKeyboard/iPadShowKeyboard.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,WAAW;CACX,YAAY;CACZ,YAAY;CACZ,aAAa;CACb,SAAS;CACT,UAAU;CACV,kBAAkB;CAClB,YAAY;CACZ,gBAAgB;CAChB,2EAAqnD;CACrnD,yBAAyB;CACzB,kBAAkB;AACnB;;AAEA;CACC,2EAAqnD;CACrnD,yBAAyB;AAC1B",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .iPadShowKeyboard {
	width: 58px;
	min-width: 0;
	height: 36px;
	min-height: 0;
	margin: 0;
	padding: 0;
	position: absolute;
	resize: none;
	overflow: hidden;
	background: url("data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNTMiIGhlaWdodD0iMzYiIHZpZXdCb3g9IjAgMCA1MyAzNiIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGcgY2xpcC1wYXRoPSJ1cmwoI2NsaXAwKSI+CjxwYXRoIGZpbGwtcnVsZT0iZXZlbm9kZCIgY2xpcC1ydWxlPSJldmVub2RkIiBkPSJNNDguMDM2NCA0LjAxMDQySDQuMDA3NzlMNC4wMDc3OSAzMi4wMjg2SDQ4LjAzNjRWNC4wMTA0MlpNNC4wMDc3OSAwLjAwNzgxMjVDMS43OTcyMSAwLjAwNzgxMjUgMC4wMDUxODc5OSAxLjc5OTg0IDAuMDA1MTg3OTkgNC4wMTA0MlYzMi4wMjg2QzAuMDA1MTg3OTkgMzQuMjM5MiAxLjc5NzIxIDM2LjAzMTIgNC4wMDc3OSAzNi4wMzEySDQ4LjAzNjRDNTAuMjQ3IDM2LjAzMTIgNTIuMDM5IDM0LjIzOTIgNTIuMDM5IDMyLjAyODZWNC4wMTA0MkM1Mi4wMzkgMS43OTk4NCA1MC4yNDcgMC4wMDc4MTI1IDQ4LjAzNjQgMC4wMDc4MTI1SDQuMDA3NzlaTTguMDEwNDIgOC4wMTMwMkgxMi4wMTNWMTIuMDE1Nkg4LjAxMDQyVjguMDEzMDJaTTIwLjAxODIgOC4wMTMwMkgxNi4wMTU2VjEyLjAxNTZIMjAuMDE4MlY4LjAxMzAyWk0yNC4wMjA4IDguMDEzMDJIMjguMDIzNFYxMi4wMTU2SDI0LjAyMDhWOC4wMTMwMlpNMzYuMDI4NiA4LjAxMzAySDMyLjAyNlYxMi4wMTU2SDM2LjAyODZWOC4wMTMwMlpNNDAuMDMxMiA4LjAxMzAySDQ0LjAzMzlWMTIuMDE1Nkg0MC4wMzEyVjguMDEzMDJaTTE2LjAxNTYgMTYuMDE4Mkg4LjAxMDQyVjIwLjAyMDhIMTYuMDE1NlYxNi4wMTgyWk0yMC4wMTgyIDE2LjAxODJIMjQuMDIwOFYyMC4wMjA4SDIwLjAxODJWMTYuMDE4MlpNMzIuMDI2IDE2LjAxODJIMjguMDIzNFYyMC4wMjA4SDMyLjAyNlYxNi4wMTgyWk00NC4wMzM5IDE2LjAxODJWMjAuMDIwOEgzNi4wMjg2VjE2LjAxODJINDQuMDMzOVpNMTIuMDEzIDI0LjAyMzRIOC4wMTA0MlYyOC4wMjZIMTIuMDEzVjI0LjAyMzRaTTE2LjAxNTYgMjQuMDIzNEgzNi4wMjg2VjI4LjAyNkgxNi4wMTU2VjI0LjAyMzRaTTQ0LjAzMzkgMjQuMDIzNEg0MC4wMzEyVjI4LjAyNkg0NC4wMzM5VjI0LjAyMzRaIiBmaWxsPSIjNDI0MjQyIi8+CjwvZz4KPGRlZnM+CjxjbGlwUGF0aCBpZD0iY2xpcDAiPgo8cmVjdCB3aWR0aD0iNTMiIGhlaWdodD0iMzYiIGZpbGw9IndoaXRlIi8+CjwvY2xpcFBhdGg+CjwvZGVmcz4KPC9zdmc+Cg==") center center no-repeat;
	border: 4px solid #F6F6F6;
	border-radius: 4px;
}

.monaco-editor.vs-dark .iPadShowKeyboard {
	background: url("data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNTMiIGhlaWdodD0iMzYiIHZpZXdCb3g9IjAgMCA1MyAzNiIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGcgY2xpcC1wYXRoPSJ1cmwoI2NsaXAwKSI+CjxwYXRoIGZpbGwtcnVsZT0iZXZlbm9kZCIgY2xpcC1ydWxlPSJldmVub2RkIiBkPSJNNDguMDM2NCA0LjAxMDQySDQuMDA3NzlMNC4wMDc3OSAzMi4wMjg2SDQ4LjAzNjRWNC4wMTA0MlpNNC4wMDc3OSAwLjAwNzgxMjVDMS43OTcyMSAwLjAwNzgxMjUgMC4wMDUxODc5OSAxLjc5OTg0IDAuMDA1MTg3OTkgNC4wMTA0MlYzMi4wMjg2QzAuMDA1MTg3OTkgMzQuMjM5MiAxLjc5NzIxIDM2LjAzMTIgNC4wMDc3OSAzNi4wMzEySDQ4LjAzNjRDNTAuMjQ3IDM2LjAzMTIgNTIuMDM5IDM0LjIzOTIgNTIuMDM5IDMyLjAyODZWNC4wMTA0MkM1Mi4wMzkgMS43OTk4NCA1MC4yNDcgMC4wMDc4MTI1IDQ4LjAzNjQgMC4wMDc4MTI1SDQuMDA3NzlaTTguMDEwNDIgOC4wMTMwMkgxMi4wMTNWMTIuMDE1Nkg4LjAxMDQyVjguMDEzMDJaTTIwLjAxODIgOC4wMTMwMkgxNi4wMTU2VjEyLjAxNTZIMjAuMDE4MlY4LjAxMzAyWk0yNC4wMjA4IDguMDEzMDJIMjguMDIzNFYxMi4wMTU2SDI0LjAyMDhWOC4wMTMwMlpNMzYuMDI4NiA4LjAxMzAySDMyLjAyNlYxMi4wMTU2SDM2LjAyODZWOC4wMTMwMlpNNDAuMDMxMiA4LjAxMzAySDQ0LjAzMzlWMTIuMDE1Nkg0MC4wMzEyVjguMDEzMDJaTTE2LjAxNTYgMTYuMDE4Mkg4LjAxMDQyVjIwLjAyMDhIMTYuMDE1NlYxNi4wMTgyWk0yMC4wMTgyIDE2LjAxODJIMjQuMDIwOFYyMC4wMjA4SDIwLjAxODJWMTYuMDE4MlpNMzIuMDI2IDE2LjAxODJIMjguMDIzNFYyMC4wMjA4SDMyLjAyNlYxNi4wMTgyWk00NC4wMzM5IDE2LjAxODJWMjAuMDIwOEgzNi4wMjg2VjE2LjAxODJINDQuMDMzOVpNMTIuMDEzIDI0LjAyMzRIOC4wMTA0MlYyOC4wMjZIMTIuMDEzVjI0LjAyMzRaTTE2LjAxNTYgMjQuMDIzNEgzNi4wMjg2VjI4LjAyNkgxNi4wMTU2VjI0LjAyMzRaTTQ0LjAzMzkgMjQuMDIzNEg0MC4wMzEyVjI4LjAyNkg0NC4wMzM5VjI0LjAyMzRaIiBmaWxsPSIjQzVDNUM1Ii8+CjwvZz4KPGRlZnM+CjxjbGlwUGF0aCBpZD0iY2xpcDAiPgo8cmVjdCB3aWR0aD0iNTMiIGhlaWdodD0iMzYiIGZpbGw9IndoaXRlIi8+CjwvY2xpcFBhdGg+CjwvZGVmcz4KPC9zdmc+Cg==") center center no-repeat;
	border: 4px solid #252526;
}`],sourceRoot:""}]);const u=d},72931:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .tokens-inspect-widget {
	z-index: 50;
	user-select: text;
	-webkit-user-select: text;
	-ms-user-select: text;
	padding: 10px;
}

.tokens-inspect-separator {
	height: 1px;
	border: 0;
}

.monaco-editor .tokens-inspect-widget .tm-token {
	font-family: var(--monaco-monospace-font);
}

.monaco-editor .tokens-inspect-widget .tm-token-length {
	font-weight: normal;
	font-size: 60%;
	float: right;
}

.monaco-editor .tokens-inspect-widget .tm-metadata-table {
	width: 100%;
}

.monaco-editor .tokens-inspect-widget .tm-metadata-value {
	font-family: var(--monaco-monospace-font);
	text-align: right;
}

.monaco-editor .tokens-inspect-widget .tm-token-type {
	font-family: var(--monaco-monospace-font);
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/editor/standalone/browser/inspectTokens/inspectTokens.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,WAAW;CACX,iBAAiB;CACjB,yBAAyB;CACzB,qBAAqB;CACrB,aAAa;AACd;;AAEA;CACC,WAAW;CACX,SAAS;AACV;;AAEA;CACC,yCAAyC;AAC1C;;AAEA;CACC,mBAAmB;CACnB,cAAc;CACd,YAAY;AACb;;AAEA;CACC,WAAW;AACZ;;AAEA;CACC,yCAAyC;CACzC,iBAAiB;AAClB;;AAEA;CACC,yCAAyC;AAC1C",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-editor .tokens-inspect-widget {
	z-index: 50;
	user-select: text;
	-webkit-user-select: text;
	-ms-user-select: text;
	padding: 10px;
}

.tokens-inspect-separator {
	height: 1px;
	border: 0;
}

.monaco-editor .tokens-inspect-widget .tm-token {
	font-family: var(--monaco-monospace-font);
}

.monaco-editor .tokens-inspect-widget .tm-token-length {
	font-weight: normal;
	font-size: 60%;
	float: right;
}

.monaco-editor .tokens-inspect-widget .tm-metadata-table {
	width: 100%;
}

.monaco-editor .tokens-inspect-widget .tm-metadata-value {
	font-family: var(--monaco-monospace-font);
	text-align: right;
}

.monaco-editor .tokens-inspect-widget .tm-token-type {
	font-family: var(--monaco-monospace-font);
}
`],sourceRoot:""}]);const i=o},71446:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},3614:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
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
`],sourceRoot:""}]);const i=o},19055:(A,l,t)=>{t.d(l,{A:()=>i});var s=t(71354),a=t.n(s),r=t(76314),_=t.n(r),o=_()(a());o.push([A.id,`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-action-bar .action-item.menu-entry .action-label.icon {
	width: 16px;
	height: 16px;
	background-repeat: no-repeat;
	background-position: 50%;
	background-size: 16px;
}


.monaco-dropdown-with-default {
	display: flex !important;
	flex-direction: row;
	border-radius: 5px;
}

.monaco-dropdown-with-default > .action-container > .action-label {
	margin-right: 0;
}

.monaco-dropdown-with-default > .action-container.menu-entry > .action-label.icon {
	width: 16px;
	height: 16px;
	background-repeat: no-repeat;
	background-position: 50%;
	background-size: 16px;
}

.monaco-dropdown-with-default > .dropdown-action-container > .monaco-dropdown > .dropdown-label .codicon[class*='codicon-'] {
	font-size: 12px;
	padding-left: 0px;
	padding-right: 0px;
	line-height: 16px;
	margin-left: -3px;
}

.monaco-dropdown-with-default > .dropdown-action-container > .monaco-dropdown > .dropdown-label > .action-label {
	display: block;
	background-size: 16px;
	background-position: center center;
	background-repeat: no-repeat;
}
`,"",{version:3,sources:["webpack://./node_modules/monaco-editor/esm/vs/platform/actions/browser/menuEntryActionViewItem.css"],names:[],mappings:"AAAA;;;+FAG+F;;AAE/F;CACC,WAAW;CACX,YAAY;CACZ,4BAA4B;CAC5B,wBAAwB;CACxB,qBAAqB;AACtB;;;AAGA;CACC,wBAAwB;CACxB,mBAAmB;CACnB,kBAAkB;AACnB;;AAEA;CACC,eAAe;AAChB;;AAEA;CACC,WAAW;CACX,YAAY;CACZ,4BAA4B;CAC5B,wBAAwB;CACxB,qBAAqB;AACtB;;AAEA;CACC,eAAe;CACf,iBAAiB;CACjB,kBAAkB;CAClB,iBAAiB;CACjB,iBAAiB;AAClB;;AAEA;CACC,cAAc;CACd,qBAAqB;CACrB,kCAAkC;CAClC,4BAA4B;AAC7B",sourcesContent:[`/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

.monaco-action-bar .action-item.menu-entry .action-label.icon {
	width: 16px;
	height: 16px;
	background-repeat: no-repeat;
	background-position: 50%;
	background-size: 16px;
}


.monaco-dropdown-with-default {
	display: flex !important;
	flex-direction: row;
	border-radius: 5px;
}

.monaco-dropdown-with-default > .action-container > .action-label {
	margin-right: 0;
}

.monaco-dropdown-with-default > .action-container.menu-entry > .action-label.icon {
	width: 16px;
	height: 16px;
	background-repeat: no-repeat;
	background-position: 50%;
	background-size: 16px;
}

.monaco-dropdown-with-default > .dropdown-action-container > .monaco-dropdown > .dropdown-label .codicon[class*='codicon-'] {
	font-size: 12px;
	padding-left: 0px;
	padding-right: 0px;
	line-height: 16px;
	margin-left: -3px;
}

.monaco-dropdown-with-default > .dropdown-action-container > .monaco-dropdown > .dropdown-label > .action-label {
	display: block;
	background-size: 16px;
	background-position: center center;
	background-repeat: no-repeat;
}
`],sourceRoot:""}]);const i=o},75521:(A,l,t)=>{t.r(l),t.d(l,{completionItemProvider:()=>p,language:()=>e,languageConfiguration:()=>a});for(var s=t(10858),a={wordPattern:/(-?\d*\.\d\w*)|([^`~!#%^&*()\-=+\[{\]}\\|;:'",.<>\/?\s]+)/g,comments:{lineComment:"#"},brackets:[["{","}"],["[","]"],["(",")"]],autoClosingPairs:[{open:"{",close:"}"},{open:"[",close:"]"},{open:"(",close:")"},{open:'"',close:'"'},{open:"'",close:"'"}],surroundingPairs:[{open:"{",close:"}"},{open:"[",close:"]"},{open:"(",close:")"},{open:'"',close:'"'},{open:"'",close:"'"},{open:"<",close:">"}],folding:{}},r=["sum","min","max","avg","group","stddev","stdvar","count","count_values","bottomk","topk","quantile"],_=["abs","absent","ceil","changes","clamp_max","clamp_min","day_of_month","day_of_week","days_in_month","delta","deriv","exp","floor","histogram_quantile","holt_winters","hour","idelta","increase","irate","label_join","label_replace","ln","log2","log10","minute","month","predict_linear","rate","resets","round","scalar","sort","sort_desc","sqrt","time","timestamp","vector","year"],o=[],i=0,C=r;i<C.length;i++){var c=C[i];o.push(c+"_over_time")}var d=["on","ignoring","group_right","group_left","by","without"],m="("+d.reduce(function(g,B){return g+"|"+B})+")",E=["+","-","*","/","%","^","==","!=",">","<",">=","<=","and","or","unless"],u=["offset"],n=r.concat(_).concat(o).concat(d).concat(u),e={ignoreCase:!1,defaultToken:"",tokenPostfix:".promql",keywords:n,operators:E,vectorMatching:m,symbols:/[=><!~?:&|+\-*\/^%]+/,escapes:/\\(?:[abfnrtv\\"']|x[0-9A-Fa-f]{1,4}|u[0-9A-Fa-f]{4}|U[0-9A-Fa-f]{8})/,digits:/\d+(_+\d+)*/,octaldigits:/[0-7]+(_+[0-7]+)*/,binarydigits:/[0-1]+(_+[0-1]+)*/,hexdigits:/[[0-9a-fA-F]+(_+[0-9a-fA-F]+)*/,integersuffix:/(ll|LL|u|U|l|L)?(ll|LL|u|U|l|L)?/,floatsuffix:/[fFlL]?/,tokenizer:{root:[[/@vectorMatching\s*(?=\()/,"type","@clauses"],[/[a-z_]\w*(?=\s*(=|!=|=~|!~))/,"tag"],[/(^#.*$)/,"comment"],[/[a-zA-Z_]\w*/,{cases:{"@keywords":"type","@default":"identifier"}}],[/"([^"\\]|\\.)*$/,"string.invalid"],[/'([^'\\]|\\.)*$/,"string.invalid"],[/"/,"string","@string_double"],[/'/,"string","@string_single"],[/`/,"string","@string_backtick"],{include:"@whitespace"},[/[{}()\[\]]/,"@brackets"],[/[<>](?!@symbols)/,"@brackets"],[/@symbols/,{cases:{"@operators":"delimiter","@default":""}}],[/\d+[smhdwy]/,"number"],[/\d*\d+[eE]([\-+]?\d+)?(@floatsuffix)/,"number.float"],[/\d*\.\d+([eE][\-+]?\d+)?(@floatsuffix)/,"number.float"],[/0[xX][0-9a-fA-F']*[0-9a-fA-F](@integersuffix)/,"number.hex"],[/0[0-7']*[0-7](@integersuffix)/,"number.octal"],[/0[bB][0-1']*[0-1](@integersuffix)/,"number.binary"],[/\d[\d']*\d(@integersuffix)/,"number"],[/\d(@integersuffix)/,"number"]],string_double:[[/[^\\"]+/,"string"],[/@escapes/,"string.escape"],[/\\./,"string.escape.invalid"],[/"/,"string","@pop"]],string_single:[[/[^\\']+/,"string"],[/@escapes/,"string.escape"],[/\\./,"string.escape.invalid"],[/'/,"string","@pop"]],string_backtick:[[/[^\\`$]+/,"string"],[/@escapes/,"string.escape"],[/\\./,"string.escape.invalid"],[/`/,"string","@pop"]],clauses:[[/[^(,)]/,"tag"],[/\)/,"identifier","@pop"]],whitespace:[[/[ \t\r\n]+/,"white"]]}},p={provideCompletionItems:function(){var g=n.map(function(B){return{label:B,kind:s.languages.CompletionItemKind.Keyword,insertText:B,insertTextRules:s.languages.CompletionItemInsertTextRule.InsertAsSnippet}});return{suggestions:g}}}},96861:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(94566),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},75301:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(35038),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},88723:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(96499),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},74639:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(714),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},63470:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(12171),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},17713:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(8970),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},90551:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(81684),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},43749:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(79862),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},37905:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(8474),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},25925:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(96854),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},48285:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(48134),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},16285:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(1366),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},83005:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(95422),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},67119:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(44959),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},65865:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(266),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},86529:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(44978),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},85329:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(14166),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},917:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(80140),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},41697:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(3474),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},98977:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(94234),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},43839:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(62516),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},66136:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(71963),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},1620:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(74333),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},95592:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(86307),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},402:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(72035),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},6028:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(28405),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},95524:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(83093),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},72544:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(98081),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},27988:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(93777),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},94104:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(6953),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},94921:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(65876),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},4806:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(57375),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},2808:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(73313),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},46820:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(36493),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},48304:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(80213),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},28252:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(81637),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},40884:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(29133),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},67340:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(48829),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},27168:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(2289),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},13837:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(95388),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},32662:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(66931),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},30116:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(99585),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},80978:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(42755),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},45088:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(7997),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},28838:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(26550),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},79518:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(24259),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},8322:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(61727),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},51580:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(53345),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},79008:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(88357),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},68926:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(45395),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},73152:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(55405),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},44029:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(81788),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},30568:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(31503),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},65577:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(26378),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},56588:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(58169),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},89532:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(1177),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},81424:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(7201),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},21862:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(20991),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},16803:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(69734),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},57136:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(38869),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},28130:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(90069),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},69269:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(87160),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},82788:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(6065),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},91856:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(18245),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},13336:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(12889),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},34908:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(3343),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},778:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(59337),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},43152:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(72931),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},20917:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(71446),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},38561:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(3614),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},83910:(A,l,t)=>{var s=t(85072),a=t.n(s),r=t(97825),_=t.n(r),o=t(77659),i=t.n(o),C=t(55056),c=t.n(C),d=t(10540),m=t.n(d),E=t(41113),u=t.n(E),n=t(19055),e={};e.styleTagTransform=u(),e.setAttributes=c(),e.insert=i().bind(null,"head"),e.domAPI=_(),e.insertStyleElement=m();var p=a()(n.A,e),g=n.A&&n.A.locals?n.A.locals:void 0},68968:A=>{A.exports="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAYAAACp8Z5+AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMTZEaa/1AAAAHUlEQVQYV2PYvXu3JAi7uLiAMaYAjAGTQBPYLQkAa/0Zef3qRswAAAAASUVORK5CYII="},37584:A=>{A.exports="data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNTMiIGhlaWdodD0iMzYiIHZpZXdCb3g9IjAgMCA1MyAzNiIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGcgY2xpcC1wYXRoPSJ1cmwoI2NsaXAwKSI+CjxwYXRoIGZpbGwtcnVsZT0iZXZlbm9kZCIgY2xpcC1ydWxlPSJldmVub2RkIiBkPSJNNDguMDM2NCA0LjAxMDQySDQuMDA3NzlMNC4wMDc3OSAzMi4wMjg2SDQ4LjAzNjRWNC4wMTA0MlpNNC4wMDc3OSAwLjAwNzgxMjVDMS43OTcyMSAwLjAwNzgxMjUgMC4wMDUxODc5OSAxLjc5OTg0IDAuMDA1MTg3OTkgNC4wMTA0MlYzMi4wMjg2QzAuMDA1MTg3OTkgMzQuMjM5MiAxLjc5NzIxIDM2LjAzMTIgNC4wMDc3OSAzNi4wMzEySDQ4LjAzNjRDNTAuMjQ3IDM2LjAzMTIgNTIuMDM5IDM0LjIzOTIgNTIuMDM5IDMyLjAyODZWNC4wMTA0MkM1Mi4wMzkgMS43OTk4NCA1MC4yNDcgMC4wMDc4MTI1IDQ4LjAzNjQgMC4wMDc4MTI1SDQuMDA3NzlaTTguMDEwNDIgOC4wMTMwMkgxMi4wMTNWMTIuMDE1Nkg4LjAxMDQyVjguMDEzMDJaTTIwLjAxODIgOC4wMTMwMkgxNi4wMTU2VjEyLjAxNTZIMjAuMDE4MlY4LjAxMzAyWk0yNC4wMjA4IDguMDEzMDJIMjguMDIzNFYxMi4wMTU2SDI0LjAyMDhWOC4wMTMwMlpNMzYuMDI4NiA4LjAxMzAySDMyLjAyNlYxMi4wMTU2SDM2LjAyODZWOC4wMTMwMlpNNDAuMDMxMiA4LjAxMzAySDQ0LjAzMzlWMTIuMDE1Nkg0MC4wMzEyVjguMDEzMDJaTTE2LjAxNTYgMTYuMDE4Mkg4LjAxMDQyVjIwLjAyMDhIMTYuMDE1NlYxNi4wMTgyWk0yMC4wMTgyIDE2LjAxODJIMjQuMDIwOFYyMC4wMjA4SDIwLjAxODJWMTYuMDE4MlpNMzIuMDI2IDE2LjAxODJIMjguMDIzNFYyMC4wMjA4SDMyLjAyNlYxNi4wMTgyWk00NC4wMzM5IDE2LjAxODJWMjAuMDIwOEgzNi4wMjg2VjE2LjAxODJINDQuMDMzOVpNMTIuMDEzIDI0LjAyMzRIOC4wMTA0MlYyOC4wMjZIMTIuMDEzVjI0LjAyMzRaTTE2LjAxNTYgMjQuMDIzNEgzNi4wMjg2VjI4LjAyNkgxNi4wMTU2VjI0LjAyMzRaTTQ0LjAzMzkgMjQuMDIzNEg0MC4wMzEyVjI4LjAyNkg0NC4wMzM5VjI0LjAyMzRaIiBmaWxsPSIjNDI0MjQyIi8+CjwvZz4KPGRlZnM+CjxjbGlwUGF0aCBpZD0iY2xpcDAiPgo8cmVjdCB3aWR0aD0iNTMiIGhlaWdodD0iMzYiIGZpbGw9IndoaXRlIi8+CjwvY2xpcFBhdGg+CjwvZGVmcz4KPC9zdmc+Cg=="},86060:A=>{A.exports="data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNTMiIGhlaWdodD0iMzYiIHZpZXdCb3g9IjAgMCA1MyAzNiIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGcgY2xpcC1wYXRoPSJ1cmwoI2NsaXAwKSI+CjxwYXRoIGZpbGwtcnVsZT0iZXZlbm9kZCIgY2xpcC1ydWxlPSJldmVub2RkIiBkPSJNNDguMDM2NCA0LjAxMDQySDQuMDA3NzlMNC4wMDc3OSAzMi4wMjg2SDQ4LjAzNjRWNC4wMTA0MlpNNC4wMDc3OSAwLjAwNzgxMjVDMS43OTcyMSAwLjAwNzgxMjUgMC4wMDUxODc5OSAxLjc5OTg0IDAuMDA1MTg3OTkgNC4wMTA0MlYzMi4wMjg2QzAuMDA1MTg3OTkgMzQuMjM5MiAxLjc5NzIxIDM2LjAzMTIgNC4wMDc3OSAzNi4wMzEySDQ4LjAzNjRDNTAuMjQ3IDM2LjAzMTIgNTIuMDM5IDM0LjIzOTIgNTIuMDM5IDMyLjAyODZWNC4wMTA0MkM1Mi4wMzkgMS43OTk4NCA1MC4yNDcgMC4wMDc4MTI1IDQ4LjAzNjQgMC4wMDc4MTI1SDQuMDA3NzlaTTguMDEwNDIgOC4wMTMwMkgxMi4wMTNWMTIuMDE1Nkg4LjAxMDQyVjguMDEzMDJaTTIwLjAxODIgOC4wMTMwMkgxNi4wMTU2VjEyLjAxNTZIMjAuMDE4MlY4LjAxMzAyWk0yNC4wMjA4IDguMDEzMDJIMjguMDIzNFYxMi4wMTU2SDI0LjAyMDhWOC4wMTMwMlpNMzYuMDI4NiA4LjAxMzAySDMyLjAyNlYxMi4wMTU2SDM2LjAyODZWOC4wMTMwMlpNNDAuMDMxMiA4LjAxMzAySDQ0LjAzMzlWMTIuMDE1Nkg0MC4wMzEyVjguMDEzMDJaTTE2LjAxNTYgMTYuMDE4Mkg4LjAxMDQyVjIwLjAyMDhIMTYuMDE1NlYxNi4wMTgyWk0yMC4wMTgyIDE2LjAxODJIMjQuMDIwOFYyMC4wMjA4SDIwLjAxODJWMTYuMDE4MlpNMzIuMDI2IDE2LjAxODJIMjguMDIzNFYyMC4wMjA4SDMyLjAyNlYxNi4wMTgyWk00NC4wMzM5IDE2LjAxODJWMjAuMDIwOEgzNi4wMjg2VjE2LjAxODJINDQuMDMzOVpNMTIuMDEzIDI0LjAyMzRIOC4wMTA0MlYyOC4wMjZIMTIuMDEzVjI0LjAyMzRaTTE2LjAxNTYgMjQuMDIzNEgzNi4wMjg2VjI4LjAyNkgxNi4wMTU2VjI0LjAyMzRaTTQ0LjAzMzkgMjQuMDIzNEg0MC4wMzEyVjI4LjAyNkg0NC4wMzM5VjI0LjAyMzRaIiBmaWxsPSIjQzVDNUM1Ii8+CjwvZz4KPGRlZnM+CjxjbGlwUGF0aCBpZD0iY2xpcDAiPgo8cmVjdCB3aWR0aD0iNTMiIGhlaWdodD0iMzYiIGZpbGw9IndoaXRlIi8+CjwvY2xpcFBhdGg+CjwvZGVmcz4KPC9zdmc+Cg=="},18880:(A,l,t)=>{A.exports=t.p+"static/img/codicon.b797181c.ttf"}}]);

//# sourceMappingURL=8164.cc510b92780293a4489e.js.map
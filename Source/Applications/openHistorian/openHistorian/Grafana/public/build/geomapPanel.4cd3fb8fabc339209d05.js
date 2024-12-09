"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[190],{33011:(t,C,o)=>{o.d(C,{A:()=>b});var e=o(71354),l=o.n(e),i=o(76314),a=o.n(i),A=o(4417),s=o.n(A),c=new URL(o(69029),o.b),p=a()(l()),d=s()(c);p.push([t.id,`.ol-control i {
  cursor: default;
}

/* Bar style */
.ol-control.ol-bar {
  left: 50%;
  min-height: 1em;
  min-width: 1em;
  position: absolute;
  top: 0.5em;
  transform: translate(-50%,0);
  -webkit-transform: translate(-50%,0);
  white-space: nowrap;
}

/* Hide subbar when not inserted in a parent bar */
.ol-control.ol-toggle .ol-option-bar {
  display: none;
}

/* Default position for controls */
.ol-control.ol-bar .ol-bar {
  position: static;
}
.ol-control.ol-bar .ol-control {
  position: relative;
  top: auto;
  left:auto;
  right:auto;
  bottom: auto;
  display: inline-block;
  vertical-align: middle;
  background-color: transparent;
  padding: 0;
  margin: 0;
  transform: none;
  -webkit-transform: none;
}
.ol-control.ol-bar .ol-bar {
  position: static;
}
.ol-control.ol-bar .ol-control button {
  margin:2px 1px;
  outline: none;
}

/* Positionning */
.ol-control.ol-bar.ol-left {
  left: 0.5em;
  top: 50%;
  -webkit-transform: translate(0px, -50%);
          transform: translate(0px, -50%);
}
.ol-control.ol-bar.ol-left .ol-control {
  display: block;
}

.ol-control.ol-bar.ol-right {
  left: auto;
  right: 0.5em;
  top: 50%;
  -webkit-transform: translate(0px, -50%);
          transform: translate(0px, -50%);
}
.ol-control.ol-bar.ol-right .ol-control {
  display: block;
}

.ol-control.ol-bar.ol-bottom {
  top: auto;
  bottom: 0.5em;
}

.ol-control.ol-bar.ol-top.ol-left,
.ol-control.ol-bar.ol-top.ol-right {
  top: 4.5em;
  -webkit-transform:none;
          transform:none;
}
.ol-touch .ol-control.ol-bar.ol-top.ol-left,
.ol-touch .ol-control.ol-bar.ol-top.ol-right {
  top: 5.5em;
}
.ol-control.ol-bar.ol-bottom.ol-left,
.ol-control.ol-bar.ol-bottom.ol-right {
  top: auto;
  bottom: 0.5em;
  -webkit-transform:none;
          transform:none;
}

/* Group buttons */
.ol-control.ol-bar.ol-group {
  margin: 1px 1px 1px 0;
}
.ol-control.ol-bar.ol-right .ol-group,
.ol-control.ol-bar.ol-left .ol-group {
  margin: 1px 1px 0 1px;
}

.ol-control.ol-bar.ol-group button {
  border-radius:0;
  margin: 0 0 0 1px;
}
.ol-control.ol-bar.ol-right.ol-group button,
.ol-control.ol-bar.ol-left.ol-group button,
.ol-control.ol-bar.ol-right .ol-group button,
.ol-control.ol-bar.ol-left .ol-group button {
  margin: 0 0 1px 0;
}
.ol-control.ol-bar.ol-group .ol-control:first-child > button {
  border-radius: 5px 0 0 5px;
}
.ol-control.ol-bar.ol-group .ol-control:last-child > button {
  border-radius: 0 5px 5px 0;
}
.ol-control.ol-bar.ol-left.ol-group .ol-control:first-child > button,
.ol-control.ol-bar.ol-right.ol-group .ol-control:first-child > button,
.ol-control.ol-bar.ol-left .ol-group .ol-control:first-child > button,
.ol-control.ol-bar.ol-right .ol-group .ol-control:first-child > button {
  border-radius: 5px 5px 0 0;
}
.ol-control.ol-bar.ol-left.ol-group .ol-control:last-child > button,
.ol-control.ol-bar.ol-right.ol-group .ol-control:last-child > button,
.ol-control.ol-bar.ol-left .ol-group .ol-control:last-child > button,
.ol-control.ol-bar.ol-right .ol-group .ol-control:last-child > button {
  border-radius: 0 0 5px 5px;
}

/* */
.ol-control.ol-bar .ol-rotate {
  opacity:1;
  visibility: visible;
}
.ol-control.ol-bar .ol-rotate button {
  display: block
}

/* Active buttons */
.ol-control.ol-bar .ol-toggle.ol-active > button,
.ol-control.ol-bar .ol-toggle.ol-active button:hover {
  background-color: #00AAFF;
  color: #fff;
}
.ol-control.ol-toggle button:disabled {
  background-color: #ccc;
}

/* Subbar toolbar */
.ol-control.ol-bar .ol-control.ol-option-bar {
  display: none;
  position:absolute;
  top:100%;
  left:0;
  margin: 5px 0;
  border-radius: 0;
  background-color: rgba(255,255,255, 0.8);
  /* border: 1px solid rgba(0, 60, 136, 0.5); */
  -webkit-box-shadow: 0 0 0 1px rgba(0, 60, 136, 0.5), 1px 1px 2px rgba(0, 0, 0, 0.5);
          box-shadow: 0 0 0 1px rgba(0, 60, 136, 0.5), 1px 1px 2px rgba(0, 0, 0, 0.5);
}

.ol-control.ol-bar .ol-option-bar:before {
  content: "";
  border: 0.5em solid transparent;
  border-color: transparent transparent rgba(0, 60, 136, 0.5);
  position: absolute;
  bottom: 100%;
  left: 0.3em;
  pointer-events: none;
}

.ol-control.ol-bar .ol-option-bar .ol-control {
  display: table-cell;
}
.ol-control.ol-bar .ol-control .ol-bar
{	display: none;
}
.ol-control.ol-bar .ol-control.ol-active > .ol-option-bar {
  display: block;
}

.ol-control.ol-bar .ol-control.ol-collapsed ul {
  display: none;
}

.ol-control.ol-bar .ol-control.ol-text-button > div:hover,
.ol-control.ol-bar .ol-control.ol-text-button > div {
  background-color: transparent;
  color: rgba(0, 60, 136, 0.5);
  width: auto;
  min-width: 1.375em;
  margin: 0;
}

.ol-control.ol-bar .ol-control.ol-text-button {
  font-size:0.9em;
  border-left: 1px solid rgba(0, 60, 136, 0.8);
  border-radius: 0;
}
.ol-control.ol-bar .ol-control.ol-text-button:first-child {
  border-left:0;
}
.ol-control.ol-bar .ol-control.ol-text-button > div {
  padding: .11em 0.3em;
  font-weight: normal;
  font-size: 1.14em;
  font-family: Arial,Helvetica,sans-serif;
}
.ol-control.ol-bar .ol-control.ol-text-button div:hover {
  color: rgba(0, 60, 136, 1);
}

.ol-control.ol-bar.ol-bottom .ol-option-bar {
  top: auto;
  bottom: 100%;
}
.ol-control.ol-bar.ol-bottom .ol-option-bar:before {
  border-color: rgba(0, 60, 136, 0.5) transparent transparent ;
  bottom: auto;
  top: 100%;
}

.ol-control.ol-bar.ol-left .ol-option-bar {
  left:100%;
  top: 0;
  bottom: auto;
  margin: 0 5px;
}
.ol-control.ol-bar.ol-left .ol-option-bar:before {
  border-color: transparent rgba(0, 60, 136, 0.5) transparent transparent;
  bottom: auto;
  right: 100%;
  left: auto;
  top: 0.3em;
}
.ol-control.ol-bar.ol-right .ol-option-bar {
  right:100%;
  left:auto;
  top: 0;
  bottom: auto;
  margin: 0 5px;
}
.ol-control.ol-bar.ol-right .ol-option-bar:before {
  border-color: transparent transparent transparent rgba(0, 60, 136, 0.5);
  bottom: auto;
  left: 100%;
  top: 0.3em;
}

.ol-control.ol-bar.ol-left .ol-option-bar .ol-option-bar,
.ol-control.ol-bar.ol-right .ol-option-bar .ol-option-bar {
  top: 100%;
  bottom: auto;
  left: 0.3em;
  right: auto;
  margin: 5px 0;
}
.ol-control.ol-bar.ol-right .ol-option-bar .ol-option-bar {
  right: 0.3em;
  left: auto;
}
.ol-control.ol-bar.ol-left .ol-option-bar .ol-option-bar:before,
.ol-control.ol-bar.ol-right .ol-option-bar .ol-option-bar:before {
  border-color: transparent transparent rgba(0, 60, 136, 0.5);
  bottom: 100%;
  top: auto;
  left: 0.3em;
  right: auto;
}
.ol-control.ol-bar.ol-right .ol-option-bar .ol-option-bar:before {
  right: 0.3em;
  left: auto;
}

.ol-control-title {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
}

.ol-center-position {
  position: absolute;
  bottom: 0;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
  background-color: rgba(255,255,255,.8);
  padding: .1em 1em;
}

.ol-compassctrl {
  display: none;
  top: 1em;
  left: auto;
  right: 1em;
}
.ol-compassctrl.ol-visible {
  display: block!important;
}
.ol-ext-dialog {
  position: fixed;
  top: -100%;
  left: 0;
  width: 150%;
  height: 100%;
  opacity: 0;
  background-color: rgba(0,0,0,.5);
  z-index: 1000;
  pointer-events: none!important;
  -webkit-transition: opacity .2s, top 0s .2s;
  transition: opacity .2s, top 0s .2s;
}
.ol-ext-dialog.ol-visible {
  opacity: 1;
  top: 0;
  pointer-events: all!important;
  -webkit-transition: opacity .2s, top 0s;
  transition: opacity .2s, top 0s;
}

.ol-viewport .ol-ext-dialog {
  position: absolute;
}
.ol-ext-dialog > form > h2 {
  margin: 0 .5em .5em 0;
  display: none;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.ol-ext-dialog > form.ol-title > h2 {
  display: block;
}
.ol-ext-dialog > form {
  position: absolute;
  top: 0;
  left: 33.33%;
  min-width: 5em;
  max-width: 60%;
  min-height: 3em;
  max-height: 100%;
  background-color: #fff;
  border: 1px solid #333;
  -webkit-box-shadow: 3px 3px 4px rgba(0,0,0, 0.5);
          box-shadow: 3px 3px 4px rgba(0,0,0, 0.5);
  -webkit-transform: translate(-50%, -30%);
          transform: translate(-50%, -30%);
  -webkit-transition: top .2s, -webkit-transform .2s;
  transition: top .2s, -webkit-transform .2s;
  transition: top .2s, transform .2s;
  transition: top .2s, transform .2s, -webkit-transform .2s;
  padding: 1em;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  overflow-x: hidden;
  overflow-y: auto;
}
.ol-ext-dialog > form.ol-closebox {
  padding-top: 1.5em;
}
.ol-ext-dialog > form.ol-title {
  padding-top: 1em;
}
.ol-ext-dialog > form.ol-button {
  padding-bottom: .5em;
}

.ol-ext-dialog.ol-zoom > form {
  top: 30%;
  -webkit-transform: translate(-50%, -30%) scale(0);
          transform: translate(-50%, -30%) scale(0);
}
.ol-ext-dialog.ol-visible > form {
  top: 30%;
}
.ol-ext-dialog.ol-zoom.ol-visible > form {
  -webkit-transform: translate(-50%, -30%) scale(1);
          transform: translate(-50%, -30%) scale(1);
}

.ol-ext-dialog > form .ol-content {
  overflow-x: hidden;
  overflow-y: auto;
}

.ol-ext-dialog > form .ol-closebox {
  position: absolute;
  top: .5em;
  right: .5em;
  width: 1em;
  height: 1em;
  cursor: pointer;
  display: none;
}
.ol-ext-dialog > form.ol-closebox .ol-closebox {
  display: block;
}
.ol-ext-dialog > form .ol-closebox:before,
.ol-ext-dialog > form .ol-closebox:after {
  content: "";
  position: absolute;
  background-color: currentColor;
  top: 50%;
  left: 50%;
  width: 1em;
  height: .1em;
  border-radius: .1em;
  -webkit-transform: translate(-50%, -50%) rotate(45deg);
          transform: translate(-50%, -50%) rotate(45deg);
}
.ol-ext-dialog > form .ol-closebox:before {
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);
          transform: translate(-50%, -50%) rotate(-45deg);
}

.ol-ext-dialog > form .ol-buttons {
  text-align: right;
  overflow-x: hidden;
}
.ol-ext-dialog > form .ol-buttons input {
  margin-top: .5em;
  padding: .5em;
  background: none;
  border: 0;
  font-size: 1em;
  color: rgba(0,60,136,1);
  cursor: pointer;
  border-radius: .25em;
  outline-width: 0;
}
.ol-ext-dialog > form .ol-buttons input:hover {
  background-color:  rgba(0,60,136,.1);
}
.ol-ext-dialog > form .ol-buttons input[type=submit] {
  font-weight: bold;
}

.ol-ext-dialog .ol-progress-message {
  font-size: .9em;
  text-align: center;
  padding-bottom: .5em;
}
.ol-ext-dialog .ol-progress-bar {
  border: 1px solid #369;
  width: 20em;
  height: 1em;
  max-width: 100%;
  padding: 2px;
  margin: .5em auto 0;
  overflow: hidden;
}
.ol-ext-dialog .ol-progress-bar > div {
  background: #369;
  height: 100%;
  width: 50%;
  -webkit-transition: width .3s;
  transition: width .3s;
}
.ol-ext-dialog .ol-progress-bar > div.notransition {
  -webkit-transition: unset;
  transition: unset;
}

/* full screen */
.ol-ext-dialog.ol-fullscreen-dialog form {
  top: 1em;
  -webkit-transform: none;
          transform: none;
  left: 1em;
  bottom: 1em;
  right: 1em;
  max-width: calc(66.6% - 2em);
  text-align: center;
  background: transparent;
  -webkit-box-shadow: none;
          box-shadow: none;
  border: none;
  color: #fff;
}
.ol-ext-dialog.ol-fullscreen-dialog form .ol-closebox {
  top: 0;
  right: 0;
  font-size: 2em;
}
.ol-ext-dialog.ol-fullscreen-dialog .ol-closebox:before,
.ol-ext-dialog.ol-fullscreen-dialog .ol-closebox:after {
  border: .1em solid currentColor;
}
.ol-ext-dialog.ol-fullscreen-dialog img,
.ol-ext-dialog.ol-fullscreen-dialog video {
  max-width: 100%;
}

/* Fullscreen dialog */
body > .ol-ext-dialog .ol-content {
  max-height: calc(100vh - 10em);
}

body > .ol-ext-dialog > form {
  overflow: visible;
}
.ol-editbar .ol-button button {
  position: relative;
  display: inline-block;
  font-style: normal;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  vertical-align: middle;
}
.ol-editbar .ol-button button:before, 
.ol-editbar .ol-button button:after {
  content: "";
  border-width: 0;
  position: absolute;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  background-color: currentColor;
}
.ol-editbar .ol-button button:focus {
  outline: none;
}

.ol-editbar .ol-selection > button:before {
  width: .6em;
  height: 1em;
  background-color: transparent;
  border: .5em solid currentColor;
  border-width: 0 .25em .65em;
  border-color: currentColor transparent;
  -webkit-box-shadow:0 0.6em 0 -0.23em;
          box-shadow:0 0.6em 0 -0.23em;
  top: .35em;
  left: .5em;
  -webkit-transform: translate(-50%, -50%) rotate(-30deg);
          transform: translate(-50%, -50%) rotate(-30deg);
}
.ol-editbar .ol-selection0 > button:after {
  width: .28em;
  height: .6em;
  background-color: transparent;
  border: .5em solid currentColor;
  border-width: 0 .05em .7em;
  border-color: currentColor transparent;
  top: .5em;
  left: .7em;
  -webkit-transform: rotate(-45deg);
          transform: rotate(-45deg);
}

.ol-editbar .ol-delete button:after,
.ol-editbar .ol-delete button:before {
  width: 1em;
  height: .2em;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%) rotate(45deg);
          transform: translate(-50%, -50%) rotate(45deg);
}
.ol-editbar .ol-delete button:after {
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);
          transform: translate(-50%, -50%) rotate(-45deg);
}

.ol-editbar .ol-info button:before {
  width: .25em;
  height: .6em;
  border-radius: .03em;
  top: .47em;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
}
.ol-editbar .ol-info button:after {
  width: .25em;
  height: .2em;
  border-radius: .03em;
  -webkit-box-shadow: -0.1em 0.35em, -0.1em 0.82em, 0.1em 0.82em;
          box-shadow: -0.1em 0.35em, -0.1em 0.82em, 0.1em 0.82em;
  top: .12em;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
}

.ol-editbar .ol-drawpoint button:before {
  width: .7em;
  height: .7em;
  border-radius: 50%;
  border: .15em solid currentColor;
  background-color: transparent;
  top: .2em;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
}
.ol-editbar .ol-drawpoint button:after {
  width: .4em;
  height: .4em;
  border: .15em solid currentColor;
  border-color: currentColor transparent;
  border-width: .4em .2em 0;
  background-color: transparent;
  top: .8em;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
}

.ol-editbar .ol-drawline > button:before,
.ol-editbar .ol-drawpolygon > button:before,
.ol-editbar .ol-drawhole > button:before {
  width: .8em;
  height: .8em;
  border: .13em solid currentColor;
  background-color: transparent;
  border-width: .2em .13em .09em;
  top: .2em;
  left: .25em;
  -webkit-transform: rotate(10deg) perspective(1em) rotateX(40deg);
          transform: rotate(10deg) perspective(1em) rotateX(40deg);
}
.ol-editbar .ol-drawline > button:before {
  border-bottom: 0;
}
.ol-editbar .ol-drawline > button:after,
.ol-editbar .ol-drawhole > button:after,
.ol-editbar .ol-drawpolygon > button:after {
  width: .3em;
  height: .3em;
  top: 0.2em;
  left: .25em;
  -webkit-box-shadow: -0.2em 0.55em, 0.6em 0.1em, 0.65em 0.7em;
          box-shadow: -0.2em 0.55em, 0.6em 0.1em, 0.65em 0.7em;
}
.ol-editbar .ol-drawhole > button:after {
  -webkit-box-shadow: -0.2em 0.55em, 0.6em 0.1em, 0.65em 0.7em, 0.25em 0.35em;
          box-shadow: -0.2em 0.55em, 0.6em 0.1em, 0.65em 0.7em, 0.25em 0.35em;
}


.ol-editbar .ol-offset > button i,
.ol-editbar .ol-transform > button i {
  position: absolute;
  width: .9em;
  height: .9em;
  overflow: hidden;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}
.ol-editbar .ol-offset > button i{
  width: .8em;
  height: .8em;
}

.ol-editbar .ol-offset > button i:before,
.ol-editbar .ol-transform > button i:before,
.ol-editbar .ol-transform > button i:after {
  content: "";
  height: 1em;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%) rotate(45deg);
          transform: translate(-50%, -50%) rotate(45deg);
  -webkit-box-shadow: 0.5em 0 0 0.1em, -0.5em 0 0 0.1em;
          box-shadow: 0.5em 0 0 0.1em, -0.5em 0 0 0.1em;
  width: .1em;
  position: absolute;
  background-color: currentColor;
}
.ol-editbar .ol-offset > button i:before{
  -webkit-box-shadow: 0.45em 0 0 0.1em, -0.45em 0 0 0.1em;
          box-shadow: 0.45em 0 0 0.1em, -0.45em 0 0 0.1em;
}
.ol-editbar .ol-transform > button i:after {
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);
          transform: translate(-50%, -50%) rotate(-45deg);
}

.ol-editbar .ol-split > button:before {
  width: .3em;
  height: .3em;
  top: .81em;
  left: .75em;
  border-radius: 50%;
  -webkit-box-shadow: 0.1em -0.4em, -0.15em -0.25em;
          box-shadow: 0.1em -0.4em, -0.15em -0.25em;
}
.ol-editbar .ol-split > button:after {
  width: .8em;
  height: .8em;
  top: .15em;
  left: -.1em;
  border: .1em solid currentColor;
  border-width: 0 .2em .2em 0;
  background-color: transparent;
  border-radius: .1em;
  -webkit-transform: rotate(20deg) scaleY(.6) rotate(-45deg);
          transform: rotate(20deg) scaleY(.6) rotate(-45deg);
}

.ol-editbar .ol-drawregular > button:before {
  width: .9em;
  height: .9em;
  top: 50%;
  left: 50%;
  border: .1em solid currentColor;
  background-color: transparent;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}
.ol-editbar .ol-drawregular .ol-bar .ol-text-button > div > div > div {
  border: .5em solid currentColor;
  border-color: transparent currentColor;
  display: inline-block;
  cursor: pointer;
  vertical-align: text-bottom;
}
.ol-editbar .ol-drawregular .ol-bar:before,
.ol-control.ol-bar.ol-editbar .ol-drawregular .ol-bar {
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
}
.ol-editbar .ol-drawregular .ol-bar .ol-text-button {
  min-width: 6em;
  text-align: center;
}
.ol-editbar .ol-drawregular .ol-bar .ol-text-button > div > div > div:first-child {
  border-width: .5em .5em .5em 0;
  margin: 0 .5em 0 0;
}
.ol-editbar .ol-drawregular .ol-bar .ol-text-button > div > div > div:last-child {
  border-width: .5em 0 .5em .5em;
  margin: 0 0 0 .5em;
}

.ol-feature-list {
  position: relative;
}
.ol-control.ol-feature-list {
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
}
.ol-control.ol-feature-list > button:before {
  content: "";
  position: absolute;
  width: 60%;
  height: 50%;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  -webkit-box-shadow: inset 0 2.5px;
          box-shadow: inset 0 2.5px;  
  background-image: -webkit-gradient(linear, left top, right top, from(currentColor), to(transparent));  
  background-image: linear-gradient(90deg, currentColor, transparent);
}

.ol-control.ol-feature-list.ol-collapsed {
  bottom: .5em;
  left: .5em;
  width: auto;
}
.ol-feature-list .ol-content {
  background-color: #f4f6f8;
  clear: both;
}
.ol-feature-list.ol-collapsed .ol-content > * {
  display: none;
}
.ol-feature-list .ol-header {
  position: relative;
  overflow: hidden;
}
.ol-feature-list.ol-collapsed .ol-header {
  display: block;
}

.ol-control.ol-feature-list > button,
.ol-control.ol-feature-list.ol-collapsed .ol-content {
  display: none;
}
.ol-control.ol-feature-list.ol-collapsed > button {
  display: block;
}

.ol-feature-list .ol-sizer {
  width: 100%;
  height: 1px;
  cursor: row-resize;
  background-color: #ccc;
  position: relative;
  border: 1px solid #f4f6f8;
  border-width: 1px 0;
}
.ol-feature-list.ol-bottom .ol-sizer {
  position: absolute;
  bottom: 0;
}
.ol-feature-list .ol-sizer:before {
  content: "";
  width: 1.5em;
  height: 4px;
  background-color: #ddd;
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  z-index: 1;
  border: 3px solid transparent;
  border-width: 3px 1.5em;
  -webkit-box-shadow: inset 0 -1px #aaa, inset 0 1px #aaa;
          box-shadow: inset 0 -1px #aaa, inset 0 1px #aaa;
  border-radius: 2px;
}
.ol-feature-list .ol-sizer:hover:before {
  background-color: #ccc;
}

.ol-feature-list .ol-content .ol-header p {
  display: inline-block;
  margin: 0.1em .25em;
  vertical-align: middle;
  white-space: nowrap;
  width: calc(100% - 2em);
  text-overflow: ellipsis;
  overflow: hidden;
}
.ol-feature-list .ol-content .ol-header .ol-buttons {
  position: absolute;
  right: 0;
  top: 50%;
  -webkit-transform: translateY(-50%);
          transform: translateY(-50%);
}

.ol-feature-list .ol-content .ol-header button {
  position: relative;
  border: 0;
  background: transparent;
  vertical-align: middle;
  width: 1em;
  height: 1em;
  margin: 0 .2em;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}
.ol-feature-list .ol-content .ol-header button.ol-closebox:before,
.ol-feature-list .ol-content .ol-header button.ol-closebox:after {
  content: "";
  position: absolute;
  left: 50%;
  top: 50%;
  -webkit-transform: translate(-50%, -50%) rotate(45deg);
          transform: translate(-50%, -50%) rotate(45deg);
  background: currentColor;
  width: .1em;
  height: 1em;
  display: block;
  font-style: normal;
}
.ol-feature-list .ol-content .ol-header button.ol-closebox:after {
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);
          transform: translate(-50%, -50%) rotate(-45deg);
}

.ol-feature-list.ol-collapsed .ol-content .ol-header button.ol-closebox {
  border: 1px solid currentColor;
}
.ol-feature-list.ol-collapsed .ol-content .ol-header button.ol-closebox:before {
  -webkit-transform: translate(-50%, -50%) rotate(90deg);
          transform: translate(-50%, -50%) rotate(90deg);
}
.ol-feature-list.ol-collapsed .ol-content .ol-header button.ol-closebox:after {
  display: none;
}

.ol-feature-list .ol-scroll-container {
  overflow: auto;
  height: 10em;
  min-height: 3em;
}
.ol-feature-list .ol-scroll-container:focus {
  outline: unset;
}

.ol-feature-list table {
  background-color: #fff;
  border-collapse: collapse;
  white-space: nowrap;
  position: relative;
}
.ol-feature-list table thead {
  background-color: #f4f6f8;
  position: -webkit-sticky;
  position: sticky;
  top: 0;
  overflow: hidden;
}
/* Fixed height */
.ol-feature-list table tr {
  height: 1.4em;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}
.ol-feature-list table tbody tr:hover {
  background-color: #eee;
  cursor: pointer;
}

.ol-feature-list table tbody tr.ol-selected {
  background-color: #eee;
}

.ol-feature-list table td {
  border: 0 solid #ddd;
  border-width: 0 0 1px 1px;
  overflow: hidden;
  width: 100px;
}
.ol-feature-list table td.number {
  text-align: right;
}
.ol-feature-list table td.undefined {
  text-align: center;
  background-color: #fafafa;
}
.ol-feature-list table tr:hover td,
.ol-feature-list table .ol-selected td {
  background-color: transparent;
}
.ol-feature-list thead td {
  position: relative;
}
.ol-feature-list thead td.sort {
  padding-right: 1.5em;
}

.ol-feature-list thead td p {
  margin: 0;
  display: inline-block;
}
.ol-feature-list table td button {
  position: absolute;
  width: 1.2em;
  height: 1.4em;
  top: 0;
  right: 0;
  margin: .1em 0;
  padding: 0;
  background-color: transparent;
  border: 0;
}
.ol-feature-list thead td button.sort:before {
  content: '\u21C5';
  font-style: normal;
  font-weight: normal;
  cursor: pointer;
  font-size: 1em;
  line-height: 1em;
}
.ol-feature-list thead td button.sortup:before {
  content: '\u21A7';
}
.ol-feature-list thead td button.sortdown:before {
  content: '\u21A5';
}

.ol-gauge {
  top: 0.5em;
  left: 50%;
  -webkit-transform: translateX(-50%);
  transform: translateX(-50%);
  background-color: rgba(255,255,255,.5);
  padding: 0.1em 0.5em;
}

.ol-gauge > * {
  display: inline-block;
  vertical-align: middle;
}
.ol-gauge > span {
  margin: 0 0.5em 0 0;
}
.ol-gauge > div {
  display: inline-block;
  width: 200px;
  border: 1px solid rgba(0,60,136,.5);
  border-radius: 3px;
  padding:1px;
}
.ol-gauge button {
  height: 0.8em;
  margin:0;
  max-width:100%;
}

.ol-control.ol-bookmark {
  top: 0.5em;
  left: 3em;
  background-color: rgba(255,255,255,.5);
}
.ol-control.ol-bookmark button {
  position: relative;
}
.ol-control.ol-bookmark > button::before {
  content: "";
  position: absolute;
  border-width: 10px 5px 4px;
  border-style: solid;
  border-color: currentColor;
  border-bottom-color: transparent;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
  transform: translate(-50%, -50%);
  height: 0;
}


.ol-control.ol-bookmark > div {
  display: block;
  min-width: 5em;
}
.ol-control.ol-bookmark.ol-collapsed > div {
  display: none;
}
.ol-control.ol-bookmark input {
  font-size: 0.9em;
  margin: 0 0.5em 0.5em;
  padding: 0 0.5em;
}
.ol-control.ol-bookmark ul {
  margin: 0 0 0.5em;
  padding: 0;
  list-style: none;
  min-width: 10em;
}
.ol-control.ol-bookmark li {
  color: rgba(0,60,136,0.8);
  font-size: 0.9em;
  padding: 0 0.2em 0 0.5em;
  cursor: default;
  clear:both;
}

.ol-control.ol-bookmark li:hover {
  background-color: rgba(0,60,136,.5);
  color: #fff;
}

.ol-control.ol-bookmark > div button {
  width: 1em;
  height: 0.8em;
  float: right;
  background-color: transparent;
  cursor: pointer;
  border-radius: 0;
}
.ol-control.ol-bookmark > div button:before {
  content: "\\2A2F";
  color: #936;
  font-size: 1.2em;
  line-height: 1em;
  border-radius: 0;
    position: absolute;
    top: 50%;
    left: 50%;
    -webkit-transform: translate(-50%, -50%);
    transform: translate(-50%, -50%);
}

.ol-bookmark ul li button,
.ol-bookmark input {
  display: none;
}
.ol-bookmark.ol-editable ul li button,
.ol-bookmark.ol-editable input {
  display: block;
}


.ol-control.ol-geobt {
  top: auto;
  left: auto;
  right: .5em;
  bottom: 3em;
}
.ol-touch .ol-control.ol-geobt {
  bottom: 3.5em;
}
.ol-control.ol-geobt button:before {
  content: "";
  position: absolute;
  background: transparent;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  border: .16em solid currentColor;
  width: .4em;
  height: .4em;
  border-radius: 50%;
}
.ol-control.ol-geobt button:after {
  content: "";
  position: absolute;
  width: .2em;
  height: .2em;
  background: transparent;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  -webkit-box-shadow: .42em 0, -.42em 0, 0 .42em, 0 -.42em;
          box-shadow: .42em 0, -.42em 0, 0 .42em, 0 -.42em;
}

.ol-control.ol-bar.ol-geobar .ol-control {
	display: inline-block;
	vertical-align: middle;
}

.ol-control.ol-bar.ol-geobar .ol-bar {
  display: none;
}
.ol-bar.ol-geobar.ol-active .ol-bar {
  display: inline-block;
}

.ol-bar.ol-geobar .geolocBt button:before,
.ol-bar.ol-geobar .geolocBt button:after {
  content: "";
  display: block;
  position: absolute;
  border: 1px solid transparent;
  border-width: 0.3em 0.8em 0 0.2em;
  border-color: currentColor transparent transparent;
  -webkit-transform: rotate(-30deg);
  transform: rotate(-30deg);
  top: .45em;
  left: 0.15em;
  font-size: 1.2em;
}
.ol-bar.ol-geobar .geolocBt button:after {
  border-width: 0 0.8em .3em 0.2em;
  border-color: transparent transparent currentColor;
	-webkit-transform: rotate(-61deg);
	transform: rotate(-61deg);
}

.ol-bar.ol-geobar .startBt button:before {
  content: "";
  display: block;
  position: absolute;
  width: 1em;
  height: 1em;
  background-color: #800;
  border-radius: 50%;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%,-50%);
  transform: translate(-50%,-50%);
}
.ol-bar.ol-geobar .pauseBt button:before,
.ol-bar.ol-geobar .pauseBt button:after {
  content: "";
  display: block;
  position: absolute;
  width: .25em;
  height: 1em;
  background-color: currentColor;
  top: 50%;
  left: 35%;
  -webkit-transform: translate(-50%,-50%);
  transform: translate(-50%,-50%);
}
.ol-bar.ol-geobar .pauseBt button:after {
  left: 65%;
}

.ol-control.ol-bar.ol-geobar .centerBt,
.ol-control.ol-bar.ol-geobar .pauseBt,
.ol-bar.ol-geobar.pauseTrack .startBt,
.ol-bar.ol-geobar.centerTrack .startBt,
.ol-bar.ol-geobar.centerTrack.pauseTrack .pauseBt,
.ol-bar.ol-geobar.centerTrack .pauseBt {
  display: none;
}
.ol-bar.ol-geobar.pauseTrack .pauseBt,
.ol-bar.ol-geobar.centerTrack .centerBt{
  display: inline-block;
}

.ol-control.ol-globe
{	position: absolute;
	left: 0.5em;
	bottom: 0.5em;
	border-radius: 50%;
	opacity: 0.7;
	transform: scale(0.5);
	transform-origin: 0 100%;
	-webkit-transform: scale(0.5);
	-webkit-transform-origin: 0 100%;
}
.ol-control.ol-globe:hover
{	opacity: 0.9;
}

.ol-control.ol-globe .panel
{	display:block;
	width:170px;
	height:170px;
	background-color:#fff;
	cursor: pointer;
	border-radius: 50%;
	overflow: hidden;
	-webkit-box-shadow: 0 0 10px 5px rgba(255, 255, 255, 0.5);
	        box-shadow: 0 0 10px 5px rgba(255, 255, 255, 0.5);
}
.ol-control.ol-globe .panel .ol-viewport
{	border-radius: 50%;
}

.ol-control.ol-globe .ol-pointer
{	display: block;
	background-color: #fff;
	width:10px;
	height: 10px;
	border:10px solid red;
	position: absolute;
	top: 50%;
	left:50%;
	transform: translate(-15px, -40px);
	-webkit-transform: translate(-15px, -40px);
	border-radius: 50%;
	z-index:1;
	transition: opacity 0.15s, top 0s, left 0s;
	-webkit-transition: opacity 0.15s, top 0s, left 0s;
}
.ol-control.ol-globe .ol-pointer.hidden
{	opacity:0;
	transition: opacity 0.15s, top 3s, left 5s;
	-webkit-transition: opacity 0.15s, top 3s, left 5s;
}

.ol-control.ol-globe .ol-pointer::before
{	border-radius: 50%;
	-webkit-box-shadow: 6px 6px 10px 5px #000;
	        box-shadow: 6px 6px 10px 5px #000;
	content: "";
	display: block;
	height: 0;
	left: 0;
	position: absolute;
	top: 23px;
	width: 0;
}
.ol-control.ol-globe .ol-pointer::after
{	content:"";
	width:0;
	height:0;
	display: block;
	position: absolute;
	border-width: 20px 10px 0;
	border-color: red transparent;
	border-style: solid;
	left: -50%;
	top: 100%;
}

.ol-control.ol-globe .panel::before {
  border-radius: 50%;
  -webkit-box-shadow: -20px -20px 80px 2px rgba(0, 0, 0, 0.7) inset;
          box-shadow: -20px -20px 80px 2px rgba(0, 0, 0, 0.7) inset;
  content: "";
  display: block;
  height: 100%;
  left: 0;
  position: absolute;
  top: 0;
  width: 100%;
  z-index: 1;
}
.ol-control.ol-globe .panel::after {
  border-radius: 50%;
  -webkit-box-shadow: 0 0 20px 7px rgba(255, 255, 255, 1);
          box-shadow: 0 0 20px 7px rgba(255, 255, 255, 1);
  content: "";
  display: block;
  height: 0;
  left: 23%;
  position: absolute;
  top: 20%;
  -webkit-transform: rotate(-40deg);
          transform: rotate(-40deg);
  width: 20%;
  z-index: 1;
}


.ol-control.ol-globe.ol-collapsed .panel
{	display:none;
}

.ol-control-top.ol-globe
{	bottom: auto;
	top: 5em;
	transform-origin: 0 0;
	-webkit-transform-origin: 0 0;
}
.ol-control-right.ol-globe
{	left: auto;
	right: 0.5em;
	transform-origin: 100% 100%;
	-webkit-transform-origin: 100% 100%;
}
.ol-control-right.ol-control-top.ol-globe
{	left: auto;
	right: 0.5em;
	transform-origin: 100% 0;
	-webkit-transform-origin: 100% 0;
}

.ol-gridreference
{	background: #fff;
	border: 1px solid #000;
	overflow: auto;
	max-height: 100%;
	top:0;
	right:0;
}
.ol-gridreference input
{	width:100%;
}
.ol-gridreference ul
{	margin:0;
	padding:0;
	list-style: none;
} 
.ol-gridreference li
{	padding: 0 0.5em;
	cursor: pointer;
}
.ol-gridreference ul li:hover 
{	background-color: #ccc;
}
.ol-gridreference li.ol-title,
.ol-gridreference li.ol-title:hover
{	background:rgba(0,60,136,.5);
	color:#fff;
	cursor:default;
}
.ol-gridreference ul li .ol-ref
{	margin-left: 0.5em;
}
.ol-gridreference ul li .ol-ref:before
{	content:"(";
}
.ol-gridreference ul li .ol-ref:after
{	content:")";
}

.ol-control.ol-imageline {
  bottom:0;
  left: 0;
  right: 0;
  padding: 0;
  overflow: visible;
  -webkit-transition: .3s;
  transition: .3s;
  border-radius: 0;
}
.ol-control.ol-imageline.ol-collapsed {
  -webkit-transform: translateY(100%);
          transform: translateY(100%);
}
.ol-imageline > div {
  height: 4em;
  position: relative;
  white-space: nowrap;
  scroll-behavior: smooth;
  overflow: hidden;
  width: 100%;
}
.ol-imageline > div.ol-move {
  scroll-behavior: unset;
}

.ol-control.ol-imageline button {
  position: absolute;
  top: -1em;
  -webkit-transform: translateY(-100%);
          transform: translateY(-100%);
  margin: .65em;
  -webkit-box-shadow: 0 0 0 0.15em rgba(255,255,255,.4);
          box-shadow: 0 0 0 0.15em rgba(255,255,255,.4);
}
.ol-control.ol-imageline button:before {
  content: '';
  position: absolute;
  -webkit-transform: translate(-50%, -50%) rotate(135deg);
          transform: translate(-50%, -50%) rotate(135deg);
  top: 40%;
  left: 50%;
  width: .4em;
  height: .4em;
  border: .1em solid currentColor;
  border-width: .15em .15em 0 0;
}
.ol-control.ol-imageline.ol-collapsed button:before {
  top: 60%;
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);
          transform: translate(-50%, -50%) rotate(-45deg);
}

.ol-imageline,
.ol-imageline:hover {
  background-color: rgba(0,0,0,.75);
}

.ol-imageline.ol-arrow:after,
.ol-imageline.ol-arrow:before {
  content: "";
  position: absolute;
  top: 50%;
  left: .2em;
  border-color: #fff #000;
  border-width: 1em .6em 1em 0;
  border-style: solid;
  -webkit-transform: translateY(-50%);
          transform: translateY(-50%);
  z-index: 1;
  opacity: .8;
  pointer-events: none;
  -webkit-box-shadow: -0.6em 0 0 1em #fff;
          box-shadow: -0.6em 0 0 1em #fff;
}
.ol-imageline.ol-arrow:after {
  border-width: 1em 0 1em .6em;
  left: auto;
  right: .2em;
  -webkit-box-shadow: 0.6em 0 0 1em #fff;
          box-shadow: 0.6em 0 0 1em #fff;
}
.ol-imageline.ol-scroll0.ol-arrow:before {
  display: none;
}
.ol-imageline.ol-scroll1.ol-arrow:after {
  display: none;
}


.ol-imageline .ol-image {
  position: relative;
  height: 100%;
  display: inline-block;
  cursor: pointer;
}
.ol-imageline img {
  max-height: 100%;
  border: .25em solid transparent;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  opacity: 0;
  -webkit-transition: 1s;
  transition: 1s;
}
.ol-imageline img.ol-loaded {
  opacity:1;
}

.ol-imageline .ol-image.select {
  background-color: #fff;
}
.ol-imageline .ol-image span {
  position: absolute;
  width: 125%;
  max-height: 2.4em;
  left: 50%;
  bottom: 0;
  display: none;
  color: #fff;
  background-color: rgba(0,0,0,.5);
  font-size: .8em;
  overflow: hidden;
  white-space: normal;
  text-align: center;
  line-height: 1.2em;
  -webkit-transform: translateX(-50%) scaleX(.8);
          transform: translateX(-50%) scaleX(.8);
}
/*
.ol-imageline .ol-image.select span,
*/
.ol-imageline .ol-image:hover span {
  display: block;
}

.ol-control.ol-routing.ol-isochrone .ol-method-time,
.ol-control.ol-routing.ol-isochrone .ol-method-distance,
.ol-control.ol-routing.ol-isochrone > button {
  position: relative;
}
.ol-control.ol-routing.ol-isochrone .ol-method-time:before,
.ol-control.ol-routing.ol-isochrone > button:before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  border: .1em solid currentColor;
  width: .8em;
  height: .8em;
  border-radius: 50%;
  -webkit-box-shadow: 0 -0.5em 0 -0.35em, 0.4em -0.35em 0 -0.35em;
          box-shadow: 0 -0.5em 0 -0.35em, 0.4em -0.35em 0 -0.35em;
  clip: unset;
}
.ol-control.ol-routing.ol-isochrone .ol-method-time:after,
.ol-control.ol-routing.ol-isochrone > button:after {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%) rotate(-60deg);
          transform: translate(-50%, -50%) rotate(-60deg);
  border-radius: 50%;
  border: .3em solid transparent;
  border-right-color: currentColor;
  -webkit-box-shadow: none;
          box-shadow: none;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  clip: unset;
}

.ol-control.ol-routing.ol-isochrone .ol-method-distance:before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%) rotate(-30deg);
          transform: translate(-50%, -50%) rotate(-30deg);
  width: 1em;
  height: .5em;
  border: .1em solid currentColor;
  -webkit-box-sizing: border-box;
          box-sizing: border-box
}
.ol-control.ol-routing.ol-isochrone .ol-method-distance:after {
  content: '';
  position: absolute;
  width: .1em;
  height: .15em;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%) rotate(-30deg);
          transform: translate(-50%, -50%) rotate(-30deg);
  -webkit-box-shadow: inset 0 -0.15em, 0 0.1em, 0.25em 0.1em, -0.25em 0.1em;
          box-shadow: inset 0 -0.15em, 0 0.1em, 0.25em 0.1em, -0.25em 0.1em;
}

.ol-control.ol-routing.ol-isochrone .ol-direction-direct:before,
.ol-control.ol-routing.ol-isochrone .ol-direction-reverse:before {
  content: '';
  position: absolute;
  top: 50%;
  left: 30%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  width: .3em;
  height: .3em;
  border-radius: 50%;
  border: .1em solid currentColor;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  -webkit-box-shadow: 0.25em 0 0 -0.05em;
          box-shadow: 0.25em 0 0 -0.05em;
}
.ol-control.ol-routing.ol-isochrone .ol-direction-direct:after,
.ol-control.ol-routing.ol-isochrone .ol-direction-reverse:after {
  content: '';
  position: absolute;
  top: 50%;
  left: 70%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  border: .4em solid transparent;
  border-width: .4em 0 .4em .4em;
  border-color: transparent currentColor;
}
.ol-control.ol-routing.ol-isochrone .ol-direction-reverse:after {
  border-width: .4em .4em .4em 0;
}

.ol-control.ol-isochrone.ol-collapsed .content {
  display: none;
}
.ol-control.ol-isochrone input[type="number"] {
  width: 3em;
  text-align: right;
  margin: 0 .1em;
}
.ol-control.ol-isochrone .ol-distance input[type="number"] {
  width: 5em;
}

.ol-isochrone .ol-time,
.ol-isochrone .ol-distance {
  display: none;
}
.ol-isochrone .ol-time.selected,
.ol-isochrone .ol-distance.selected {
  display: block;
}

.ol-control.ol-layerswitcher-popup {
  position: absolute;
  right: 0.5em;
  text-align: left;
  top: 3em;
}
.ol-control.ol-layerswitcher-popup .panel {
  clear:both;
  background:#fff;
}

.ol-layerswitcher-popup .panel {
  list-style: none;
  padding: 0.25em;
  margin:0;
  overflow: hidden;
}

.ol-layerswitcher-popup .panel ul {
  list-style: none;
  padding: 0 0 0 20px;
  overflow: hidden;
}

.ol-layerswitcher-popup.ol-collapsed .panel {
  display:none;
}
.ol-layerswitcher-popup.ol-forceopen .panel {
  display:block;
}

.ol-layerswitcher-popup button  {
  background-color: white;
  background-image: url(${d});
  background-position: center;
  background-repeat: no-repeat;
  float: right;
  height: 38px;
  width: 38px;
}

.ol-layerswitcher-popup li {
  color:#369;
  padding:0.25em 1em;
  font-family:"Trebuchet MS",Helvetica,sans-serif;
  cursor:pointer;
}
.ol-layerswitcher-popup li.ol-header {
  display: none;
}
.ol-layerswitcher-popup li.select,
.ol-layerswitcher-popup li.ol-visible {
  background:rgba(0, 60, 136, 0.7);
  color:#fff;
}
.ol-layerswitcher-popup li:hover {
  background:rgba(0, 60, 136, 0.9);
  color:#fff;
}

.ol-control.ol-layerswitcher.ol-layer-shop {
  height: calc(100% - 4em);
  max-height: unset;
  max-width: 16em;
  background-color: transparent;
  pointer-events: none!important;
  overflow: visible;
}
.ol-control.ol-layerswitcher > * {
  pointer-events: auto;
}

.ol-control.ol-layer-shop > button,
.ol-control.ol-layer-shop .panel-container {
  -webkit-box-shadow: 0 0 0 3px rgba(255,255,255,.5);
          box-shadow: 0 0 0 3px rgba(255,255,255,.5);
}
.ol-control.ol-layerswitcher.ol-layer-shop .panel-container {
  overflow-y: scroll;
  max-height: calc(100% - 6.5em);
  border: 2px solid #369;
  border-width: 2px 0;
  padding: 0;
}
.ol-control.ol-layer-shop .panel {
  padding: 0;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  margin: .25em .5em;
}
.ol-control.ol-layerswitcher.ol-layer-shop .panel-container.ol-scrolldiv {
  overflow: hidden;
}
.ol-control.ol-layer-shop .ol-scroll {
  background-color: rgba(0,0,0,.3);
  opacity: .5;
}
.ol-layerswitcher.ol-layer-shop ul.panel li.ol-header {
  display: none;
}
.ol-layerswitcher.ol-layer-shop ul.panel li {
  margin-right: 0;
  padding-right: 0;
}
.ol-layerswitcher.ol-layer-shop .layerup {
  height: 1.5em;
  width: 1.4em;
  margin: 0;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  border-radius: 3px;
  background-color: transparent;
  color: rgba(0,60,136,1);
}
.ol-layerswitcher.ol-layer-shop .layerup:hover {
  background-color: rgba(0,60,136,.3);
}
.ol-layerswitcher.ol-layer-shop .layerup:before {
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  border: 0;
  background-color: currentColor;
  width: 1em;
  height: 2px;
  -webkit-box-shadow: 0 -4px, 0 4px;
          box-shadow: 0 -4px, 0 4px;
}
.ol-layerswitcher.ol-layer-shop .layerup:after {
  content: unset;
}

.ol-control.ol-layer-shop .ol-title-bar {
  background-color: rgba(255,255,255,.5);
  font-size: .9em;
  height: calc(2.8em - 4px);
  max-width: 14.6em;
  padding: .7em .5em;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  text-align: right;
  -webkit-transform: scaleY(1.1);
          transform: scaleY(1.1);
  -webkit-transition: width 0s, -webkit-transform .1s;
  transition: width 0s, -webkit-transform .1s;
  transition: transform .1s, width 0s;
  transition: transform .1s, width 0s, -webkit-transform .1s;
  -webkit-transform-origin: 100% 0;
          transform-origin: 100% 0;
}
.ol-control.ol-layer-shop:hover .ol-title-bar {
  background-color: rgba(255,255,255,.7);
}
.ol-control.ol-layer-shop.ol-collapsed .ol-title-bar {
  max-width: 10em;
  -webkit-transform: scale(.9, 1.1);
          transform: scale(.9, 1.1);
}
.ol-control.ol-layer-shop.ol-forceopen .ol-title-bar {
  max-width: 14.6em;
  -webkit-transform: scaleY(1.1);
          transform: scaleY(1.1);
}

.ol-control.ol-layer-shop .ol-bar {
  position: relative;
  height: 1.75em;
  clear: both;
  -webkit-box-shadow: 0 0 0 3px rgba(255,255,255,.5);
          box-shadow: 0 0 0 3px rgba(255,255,255,.5);
  background-color: #fff;
  text-align: right;
  z-index: 10;
}
.ol-control.ol-layer-shop.ol-collapsed .ol-scroll,
.ol-control.ol-layer-shop.ol-collapsed .ol-bar {
  border-width: 2px 0 0;
  display: none;
}
.ol-control.ol-layer-shop.ol-forceopen .ol-scroll,
.ol-control.ol-layer-shop.ol-forceopen .ol-bar  {
  display: block;
}
.ol-control.ol-layer-shop .ol-bar > * {
  font-size: .9em;
  display: inline-block;
  vertical-align: middle;
  margin-top: .25em;
  background-color: transparent;
}

.ol-layer-shop .ol-bar .ol-button,
.ol-touch .ol-layer-shop .ol-bar .ol-button {
  position: relative;
  top: unset;
  left: unset;
  bottom: unset;
  right: unset;
  margin: 0;
}
.ol-layer-shop .ol-bar button {
  background-color: #fff;
  color: rgba(0,60,136,1)
}
.ol-layer-shop .ol-bar button:hover {
  background-color: rgba(0,60,136,.2);
}

/* Touch device */
.ol-touch .ol-layerswitcher.ol-layer-shop > button {
  font-size: 1.7em;
}
.ol-touch .ol-layer-shop .ol-bar {
  height: 2em;
}
.ol-touch .ol-layer-shop .ol-control button {
  font-size: 1.4em;
}
.ol-touch .ol-control.ol-layer-shop .panel {
  max-height: calc(100% - 7em);
}
.ol-touch .ol-control.ol-layer-shop .panel label {
  height: 1.8em;
}
.ol-touch .ol-control.ol-layer-shop .panel label span {
  margin-left: .5em;
  padding-top: .25em;
}
.ol-touch .ol-control.ol-layer-shop .panel label:before,
.ol-touch .ol-control.ol-layer-shop .panel label:after {
  font-size: 1.3em;
  z-index: 1;
}

.ol-control.ol-layerswitcher {
  position: absolute;
  right: 0.5em;
  text-align: left;
  top: 3em;
  max-height: calc(100% - 6em);
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  overflow: hidden;
}
.ol-control.ol-layerswitcher .ol-switchertopdiv,
.ol-control.ol-layerswitcher .ol-switcherbottomdiv {
  display: block
}
.ol-control.ol-layerswitcher.ol-collapsed .ol-switchertopdiv,
.ol-control.ol-layerswitcher.ol-collapsed .ol-switcherbottomdiv {
  display: none;
}
.ol-layerswitcher.ol-forceopen.ol-collapsed .ol-switchertopdiv,
.ol-layerswitcher.ol-forceopen.ol-collapsed .ol-switcherbottomdiv {
  display: block;
}

.ol-control.ol-layerswitcher .ol-switchertopdiv,
.ol-control.ol-layerswitcher .ol-switcherbottomdiv {
  position: absolute;
  top:0;
  left:0;
  right:0;
  height: 45px;
  background: #fff; 
  z-index:2;
  opacity:1;
  cursor: pointer;
  border-top:2px solid transparent;
  border-bottom:2px solid #369;
  margin:0 2px;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}
.ol-control.ol-layerswitcher .ol-switcherbottomdiv {
  top: auto;
  bottom: 0;
  height: 2em;
  border-top:2px solid #369;
  border-bottom:2px solid transparent;
}
.ol-control.ol-layerswitcher .ol-switchertopdiv:before,
.ol-control.ol-layerswitcher .ol-switcherbottomdiv:before {
  content:"";
  position: absolute;
  left:50%;
  top:50%;
  border:10px solid transparent;
  width:0;
  height:0;
  transform: translate(-50%, -50%);
  -webkit-transform: translate(-50%, -50%);
  opacity:0.8;
}

.ol-control.ol-layerswitcher .ol-switchertopdiv:hover:before,
.ol-control.ol-layerswitcher .ol-switcherbottomdiv:hover:before {
  opacity:1;
}
.ol-control.ol-layerswitcher .ol-switchertopdiv:before {
  border-bottom-color: #369;
  border-top: 0;
}
.ol-control.ol-layerswitcher .ol-switcherbottomdiv:before {
  border-top-color: #369;
  border-bottom: 0;
}

.ol-control.ol-layerswitcher .panel-container {
  background-color: #fff;
  border-radius: 0 0 2px 2px;
  clear: both;
  display: block; /* display:block to show panel on over */
  padding: 0.5em 0.5em 0;
}

.ol-layerswitcher .panel {
  list-style: none;
  padding: 0;
  margin: 0;
  overflow: hidden;
  font-family: Tahoma,Geneva,sans-serif;
  font-size:0.9em;
  -webkit-transition: top 0.3s;
  transition: top 0.3s;
  position: relative;
  top:0;
}

.ol-layerswitcher .panel ul {
  list-style: none;
  padding: 0 0 0 20px;
  overflow: hidden;
  clear: both;
}

/** Customize checkbox
*/
.ol-layerswitcher input[type="radio"],
.ol-layerswitcher input[type="checkbox"] {
  display:none;
}

.ol-layerswitcher .panel li {
  -weblit-transition: -webkit-transform 0.2s linear;
  -webkit-transition: -webkit-transform 0.2s linear;
  transition: -webkit-transform 0.2s linear;
  transition: transform 0.2s linear;
  transition: transform 0.2s linear, -webkit-transform 0.2s linear;
  clear: both;
  display: block;
  border:1px solid transparent;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}
.ol-layerswitcher .panel li.ol-layer-select {
  background-color: rgba(0,60,136,.2);
  margin: 0 -.5em;
  padding: 0 .5em
}
/* drag and drop */
.ol-layerswitcher .panel li.drag {
  opacity: 0.5;
  transform:scale(0.8);
  -webkit-transform:scale(0.8);
}
.ol-dragover {
  background:rgba(51,102,153,0.5);
  opacity:0.8;
}
.ol-layerswitcher .panel li.forbidden,
.forbidden .ol-layerswitcher-buttons div,
.forbidden .layerswitcher-opacity div {
  background:rgba(255,0,0,0.5);
  color:#f00!important;
}

/* cursor management */
.ol-layerswitcher.drag,
.ol-layerswitcher.drag * {
  cursor:not-allowed!important;
  cursor:no-drop!important;
}
.ol-layerswitcher.drag .panel li.dropover,
.ol-layerswitcher.drag .panel li.dropover * {
  cursor: pointer!important;
  cursor: n-resize!important;
  cursor: ns-resize!important;
  cursor: -webkit-grab!important;
  cursor: grab!important;
  cursor: -webkit-grabbing!important;
  cursor: grabbing!important;
}

.ol-layerswitcher .panel li.dropover {
  background: rgba(51, 102, 153, 0.5);
}

.ol-layerswitcher .panel li label {
  display: inline-block;
  height: 1.4em;
  max-width: 12em;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
  padding: 0 0 0 1.7em;
  position: relative;
}

.ol-layerswitcher .panel li label span {
  display: inline-block;
  width: 100%;
  height: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  padding-right: .2em;
}
.ol-layerswitcher [type="radio"] + label:before,
.ol-layerswitcher [type="checkbox"] + label:before,
.ol-layerswitcher [type="radio"]:checked + label:after,
.ol-layerswitcher [type="checkbox"]:checked + label:after {
  content: '';
  position: absolute;
  left: 0.1em; top: 0.1em;
  width: 1.2em; height: 1.2em; 
  border: 2px solid #369;
  background: #fff;
  -webkit-box-sizing:border-box;
          box-sizing:border-box;
}

.ol-layerswitcher [type="radio"] + label:before,
.ol-layerswitcher [type="radio"] + label:after {
  border-radius: 50%;
}

.ol-layerswitcher [type="radio"]:checked + label:after {
  background: #369 none repeat scroll 0 0;
  margin: 0.3em;
  width: 0.6em;
  height: 0.6em;
}

.ol-layerswitcher [type="checkbox"]:checked + label:after {
  background: transparent;
  border-width: 0 3px 3px 0;
  border-style: solid;
  border-color: #369;
    width: 0.7em;
    height: 1em;
    -webkit-transform: rotate(45deg);
    transform: rotate(45deg);
    left: 0.55em;
    top: -0.05em;
    -webkit-box-shadow: 1px 0px 1px 1px #fff;
            box-shadow: 1px 0px 1px 1px #fff;
}

.ol-layerswitcher .panel li.ol-layer-hidden {
  opacity: 0.6;
}

.ol-layerswitcher.ol-collapsed .panel-container {
  display:none;
}
.ol-layerswitcher.ol-forceopen .panel-container {
  display:block;
}

.ol-layerswitcher-image > button,
.ol-layerswitcher > button {
  background-color: white;
  float: right;
  z-index: 10;
  position: relative;
  font-size: 1.7em;
}
.ol-touch .ol-layerswitcher-image > button,
.ol-touch .ol-layerswitcher > button {
  font-size: 2.5em;
}
.ol-layerswitcher-image > button:before,
.ol-layerswitcher-image > button:after,
.ol-layerswitcher > button:before,
.ol-layerswitcher > button:after {
  content: "";
  position:absolute;
  width: .75em;
  height: .75em;
  border-radius: 0.15em;
  -webkit-transform: scaleY(.8) rotate(45deg);
  transform: scaleY(.8) rotate(45deg);
}
.ol-layerswitcher-image > button:before,
.ol-layerswitcher > button:before {
  background: #e2e4e1;
  top: .32em;
  left: .34em;
  -webkit-box-shadow: 0.1em 0.1em #325158;
  box-shadow: 0.1em 0.1em #325158;
}
.ol-layerswitcher-image > button:after,
.ol-layerswitcher > button:after {
  top: .22em;
  left: .34em;
  background: #83bcc5;
  background-image: radial-gradient( circle at .85em .6em, #70b3be 0, #70b3be .65em, #83bcc5 .65em);
}
.ol-layerswitcher-buttons {
  display:block;
  float: right;
  text-align:right;
}
.ol-layerswitcher-buttons > div {
  display: inline-block;
  position: relative;
  cursor: pointer;
  height:1em;
  width:1em;
  margin:2px;
  line-height: 1em;
    text-align: center;
    background: #369;
    vertical-align: middle;
    color: #fff;
}

.ol-layerswitcher .panel li > div {
  display: inline-block;
  position: relative;
}

/* line break */
.ol-layerswitcher .ol-separator {
  display:block;
  width:0;
  height:0;
  padding:0;
  margin:0;
}

.ol-layerswitcher .layerup {
  float: right;
  height:2.5em;
  background-color: #369;
  opacity: 0.5;
  cursor: move;
  cursor: ns-resize;
}

.ol-layerswitcher .layerup:before,
.ol-layerswitcher .layerup:after {
  border-color: #fff transparent;
  border-style: solid;
  border-width: 0.4em 0.4em 0;
  content: "";
  height: 0;
  position: absolute;
  bottom: 3px;
  left: 0.1em;
  width: 0;
}
.ol-layerswitcher .layerup:after {
  border-width: 0 0.4em 0.4em;
  top:3px;
  bottom: auto;
}

.ol-layerswitcher .layerInfo {
  background: #369;
  border-radius: 100%;
}
.ol-layerswitcher .layerInfo:before {
  color: #fff;
  content: "i";
  display: block;
  font-size: 0.8em;
  font-weight: bold;
  text-align: center;
  width: 1.25em;
  position:absolute;
  left: 0;
  top: 0;
}

.ol-layerswitcher .layerTrash {
  background: #369;
}
.ol-layerswitcher .layerTrash:before {
  color: #fff;
  content: "\\00d7";
  font-size:1em;
  top: 50%;
  left: 0;
  right: 0;
  text-align: center;
  line-height: 1em;
  margin: -0.5em 0;
  position: absolute;
}

.ol-layerswitcher .layerExtent {
  background: #369;
}
.ol-layerswitcher .layerExtent:before {
  border-right: 1px solid #fff;
  border-bottom: 1px solid #fff;
  content: "";
  display: block;
  position: absolute;
  left: 6px;
  right: 2px;
  top: 6px;
  bottom: 3px;
}
.ol-layerswitcher .layerExtent:after {
  border-left: 1px solid #fff;
  border-top: 1px solid #fff;
  content: "";
  display: block;
  position: absolute;
  bottom: 6px;
  left: 2px;
  right: 6px;
  top: 3px;
}

.ol-layerswitcher .expend-layers,
.ol-layerswitcher .collapse-layers {
  margin: 0 2px;
  background-color: transparent;
}
.ol-layerswitcher .expend-layers:before,
.ol-layerswitcher .collapse-layers:before {
  content:"";
  position:absolute;
  top:50%;
  left:0;
  margin-top:-2px;
  height:4px;
  width:100%;
  background:#369;
}
.ol-layerswitcher .expend-layers:after {
  content:"";
  position:absolute;
  left:50%;
  top:0;
  margin-left:-2px;
  width:4px;
  height:100%;
  background:#369;
}
/*
.ol-layerswitcher .collapse-layers:before {
  content:"";
  position:absolute;
  border:0.5em solid #369;
  border-color: #369 transparent transparent;
  margin-top:0.25em;
}
.ol-layerswitcher .expend-layers:before {
  content:"";
  position:absolute;
  border:0.5em solid #369;
  border-color: transparent transparent transparent #369 ;
  margin-left:0.25em;
}
*/

.ol-layerswitcher .layerswitcher-opacity {
  position:relative;
  border: 1px solid #369;
  height: 3px;
  width: 120px;
  margin:5px 1em 10px 7px;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  border-radius: 3px;
  background: #69c;
  background: -webkit-gradient(linear, left top, right top, from(rgba(0,60,136,0)), to(rgba(0,60,136,0.6)));
  background: linear-gradient(to right, rgba(0,60,136,0), rgba(0,60,136,0.6));
  cursor: pointer;
  -webkit-box-shadow: 1px 1px 1px rgba(0,0,0,0.5);
          box-shadow: 1px 1px 1px rgba(0,0,0,0.5);
}

.ol-layerswitcher .layerswitcher-opacity .layerswitcher-opacity-cursor,
.ol-layerswitcher .layerswitcher-opacity .layerswitcher-opacity-cursor:before {
  position: absolute;
  width: 20px;
  height: 20px;
  top: 50%;
  left: 50%;
  background: rgba(0,60,136,0.5);
  border-radius: 50%;
  -webkit-transform: translate(-50%, -50%);
  transform: translate(-50%, -50%);
  z-index: 1;
}
.ol-layerswitcher .layerswitcher-opacity .layerswitcher-opacity-cursor:before {
  content: "";
  position: absolute;
  width: 50%;
  height: 50%;
}
.ol-touch .ol-layerswitcher .layerswitcher-opacity .layerswitcher-opacity-cursor {
  width: 26px;
  height: 26px;
}

.ol-layerswitcher .layerswitcher-opacity-label { 
  display:none;
  position: absolute;
  right: -2.5em;
  bottom: 5px;
  font-size: 0.8em;
}
.ol-layerswitcher .layerswitcher-opacity-label::after {
  content:"%";
}

.ol-layerswitcher .layerswitcher-progress {
  display:block;
  margin:-4px 1em 2px 7px;
  width: 120px;
}
.ol-layerswitcher .layerswitcher-progress div {
  background-color: #369;
  height:2px;
  display:block;
  width:0;
}

.ol-control.ol-layerswitcher-image {
  position: absolute;
  right: 0.5em;
  text-align: left;
  top: 1em;
  transition: all 0.2s ease 0s;
  -webkit-transition: all 0.2s ease 0s;
}
.ol-control.ol-layerswitcher-image.ol-collapsed {
  top:3em;
  -webkit-transition: none;
  transition: none;
}

.ol-layerswitcher-image .panel {
  list-style: none;
  padding: 0.25em;
  margin:0;
  overflow: hidden;
}

.ol-layerswitcher-image .panel ul {
  list-style: none;
  padding: 0 0 0 20px;
  overflow: hidden;
}

.ol-layerswitcher-image.ol-collapsed .panel {
  display:none;
}
.ol-layerswitcher-image.ol-forceopen .panel {
  display:block;
  clear:both;
}

.ol-layerswitcher-image button {
  float: right;
  display:none;
}

.ol-layerswitcher-image.ol-collapsed button {
  display:block;
  position:relative;
}

.ol-layerswitcher-image li {
  border-radius: 4px;
  border: 3px solid transparent;
  -webkit-box-shadow: 1px 1px 4px rgba(0, 0, 0, 0.5);
          box-shadow: 1px 1px 4px rgba(0, 0, 0, 0.5);
  display: inline-block;
  width: 64px;
  height: 64px;
  margin:2px;
  position: relative;
  background-color: #fff;
  overflow: hidden;
  vertical-align: middle;
  cursor:pointer;
}
.ol-layerswitcher-image li.ol-layer-hidden {
  opacity: 0.5;
  border-color:#555;
}
.ol-layerswitcher-image li.ol-header {
  display: none;
}

.ol-layerswitcher-image li img {
  position:absolute;
  max-width:100%;
}
.ol-layerswitcher-image li.select,
.ol-layerswitcher-image li.ol-visible {
  border: 3px solid red;
}

.ol-layerswitcher-image li p {
  display:none;
}
.ol-layerswitcher-image li:hover p {
  background-color: rgba(0, 0, 0, 0.5);
  color: #fff;
  bottom: 0;
  display: block;
  left: 0;
  margin: 0;
  overflow: hidden;
  position: absolute;
  right: 0;
  text-align: center;
  height:1.2em;
  font-family:Verdana, Geneva, sans-serif;
  font-size:0.8em;
}
.ol-control.ol-legend {
  bottom: .5em;
  left: .5em;
  z-index: 1;
  max-height: 90%;
  max-width: 90%;
  overflow-x: hidden;
  overflow-y: auto;
  background-color: rgba(255,255,255,.6);
}
.ol-control.ol-legend:hover {
  background-color: rgba(255,255,255,.8);
}
.ol-control.ol-legend.ol-empty,
.ol-control.ol-legend.ol-collapsed {
  overflow: hidden;
}
.ol-control.ol-legend button {
  position: relative;
  display: none;
}
.ol-control.ol-legend.ol-empty button,
.ol-control.ol-legend.ol-collapsed button {
  display: block;
}
.ol-control.ol-legend.ol-uncollapsible button {
  display: none;
}

.ol-control.ol-legend > ul,
.ol-control.ol-legend > canvas {
  margin: 2px;
}

.ol-control.ol-legend button.ol-closebox {
  display: block;
  position: absolute;
  top: 0;
  right: 0;
  background: none;
  cursor: pointer;
  z-index: 1;
}
.ol-control.ol-legend.ol-empty button.ol-closebox,
.ol-control.ol-legend.ol-uncollapsible button.ol-closebox,
.ol-control.ol-legend.ol-collapsed button.ol-closebox {
  display: none;
}
.ol-control.ol-legend button.ol-closebox:before {
  content: "\\D7";
  background: none;
  color: rgba(0,60,136,.5);
  font-size: 1.3em;
}
.ol-control.ol-legend button.ol-closebox:hover:before {
  color: rgba(0,60,136,1);
}
.ol-control.ol-legend .ol-legendImg {
  display: block;
}
.ol-control.ol-legend.ol-empty .ol-legendImg,
.ol-control.ol-legend.ol-collapsed .ol-legendImg {
  display: none;
}
.ol-control.ol-legend.ol-uncollapsible .ol-legendImg {
  display: block;
}

.ol-control.ol-legend > button:first-child:before,
.ol-control.ol-legend > button:first-child:after {
  content: "";
  position: absolute;
  top: .25em;
  left: .2em;
  width: .2em;
  height: .2em;
  background-color: currentColor;
  -webkit-box-shadow: 0 0.35em, 0 0.7em;
          box-shadow: 0 0.35em, 0 0.7em;
}
.ol-control.ol-legend button:first-child:after {
  top: .27em;
  left: .55em;
  height: .15em;
  width: .6em;
}

ul.ol-legend {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  margin: 0;
  padding: 0;
  list-style: none;
}
.ol-control.ol-legend.ol-empty ul,
.ol-control.ol-legend.ol-collapsed ul {
  display: none;
}
.ol-control.ol-legend.ol-uncollapsible ul {
  display: block;
}
ul.ol-legend li.ol-title {
  text-align: center;
  font-weight: bold;
}
ul.ol-legend li.ol-title > div:first-child {
  width: 0!important;
}
ul.ol-legend li {
  overflow: hidden;
  padding: 0;
  white-space: nowrap;
}
ul.ol-legend li div {
  display: inline-block;
  vertical-align: top;
}

.ol-control.ol-legend .ol-legend {
  display: inline-block;
}
.ol-control.ol-legend.ol-empty .ol-legend,
.ol-control.ol-legend.ol-collapsed .ol-legend {
  display: none;
}
.ol-control.ol-legend.ol-empty button {
  opacity: .4;
}
.ol-control.ol-mapzone {
  position: absolute;
  right: 0.5em;
  text-align: left;
  top: .5em;
  max-height: calc(100% - 6em);
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  overflow: hidden;
}

.ol-control.ol-mapzone.ol-collapsed {
  top: 3em;
}

.ol-control.ol-mapzone button {
  position: relative;
  float: right;
  margin-top: 2.5em;
}
.ol-touch .ol-control.ol-mapzone button {
  margin-top: 1.67em;
}
.ol-control.ol-mapzone.ol-collapsed button {
  margin-top: 0;
}

.ol-control.ol-mapzone button i {
  border: .1em solid currentColor;
  border-radius: 50%;
  width: .9em;
  height: .9em; 
  overflow: hidden;
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}
.ol-control.ol-mapzone button i:before {
  content: "";
  background-color: currentColor;
  width: 0.4em;
  height: .4em;
  position: absolute;
  left: .5em;
  top: 0.3em;
  border-radius: 50%;
  -webkit-box-shadow: .05em .3em 0 -.051em currentColor,
  	-.05em -.35em 0 -.1em currentColor,
  	-.5em -.35em 0 0em currentColor,
  	-.65em .1em 0 -.03em currentColor,
  	-.65em -.05em 0 -.05em currentColor;
          box-shadow: .05em .3em 0 -.051em currentColor,
  	-.05em -.35em 0 -.1em currentColor,
  	-.5em -.35em 0 0em currentColor,
  	-.65em .1em 0 -.03em currentColor,
  	-.65em -.05em 0 -.05em currentColor
}

.ol-mapzone > div {
  position: relative;
  display: inline-block;
  width: 5em;
  height: 5em;
  margin: 0 .2em 0 0;
}
.ol-control.ol-mapzone.ol-collapsed > div {
  display: none;
}
.ol-mapzone > div p {
  margin: 0;
  position: absolute;
  bottom: 0;
  /* background: rgba(255,255,255,.5); */
  color: #fff;
  font-weight: bold;
  text-align: center;
  width: 160%;
  overflow: hidden;
  font-family: 'Lucida Grande',Verdana,Geneva,Lucida,Arial,Helvetica,sans-serif;
  -webkit-transform: scaleX(.625);
          transform: scaleX(.625);
  -webkit-transform-origin: 0 0;
          transform-origin: 0 0;
  cursor: default;
}

.ol-notification {
  width: 150%;
  bottom: 0;
  border: 0;
  background: none;
  margin: 0;
  padding: 0;
}
.ol-notification > div,
.ol-notification > div:hover {
  position: absolute;
  background-color: rgba(0,0,0,.8);
  color: #fff;
  bottom: 0;
  left: 33.33%;
  max-width: calc(66% - 4em);
  min-width: 5em;
  max-height: 5em;
  min-height: 1em;
  border-radius: 4px 4px 0 0;
  padding: .2em .5em;
  text-align: center;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
  -webkit-transition: .3s;
  transition: .3s;
  opacity: 1;
}
.ol-notification.ol-collapsed > div {
  bottom: -5em;
  opacity: 0;
}

.ol-notification a {
  color: #9cf;
  cursor: pointer;
}

.ol-notification .ol-close,
.ol-notification .ol-close:hover {
  padding-right: 1.5em;
}

.ol-notification .closeBox {
  position: absolute;
  top: 0;
  right: 0.3em;
}
.ol-notification .closeBox:before {
  content: '\\00d7';
}
.ol-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width:100%;
  height: 100%;
  background-color: rgba(0,0,0,0.4);
  padding: 1em;
  color: #fff;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  z-index: 1;
  opacity: 0;
  display: none;
  cursor: default;
  overflow: hidden;
  -webkit-transition: all 0.5s;
  transition: all 0.5s;
  pointer-events: none;
  z-index: 9;
}

.ol-overlay.slide-up {
  transform: translateY(100%);
  -webkit-transform: translateY(100%);
}
.ol-overlay.slide-down {
  -webkit-transform: translateY(-100%);
  transform: translateY(-100%);
}
.ol-overlay.slide-left
{	-webkit-transform: translateX(-100%);
  transform: translateX(-100%);
}
.ol-overlay.slide-right {
  -webkit-transform: translateX(100%);
  transform: translateX(100%);
}
.ol-overlay.zoom {
  top: 50%;
  left: 50%;
  opacity:0.5;
  -webkit-transform: translate(-50%,-50%) scale(0);
  transform: translate(-50%,-50%) scale(0);
}
.ol-overlay.zoomout {
  -webkit-transform: scale(3);
  transform: scale(3);
}
.ol-overlay.zoomrotate {
  top: 50%;
  left: 50%;
  opacity:0.5;
  -webkit-transform: translate(-50%,-50%) scale(0) rotate(360deg);
  transform: translate(-50%,-50%) scale(0) rotate(360deg);
}
.ol-overlay.stretch {
  top: 50%;
  left: 50%;
  opacity:0.5;
  -webkit-transform: translate(-50%,-50%) scaleX(0);
  transform: translate(-50%,-50%) scaleX(0) ;
}
.ol-overlay.stretchy {
  top: 50%;
  left: 50%;
  opacity:0.5;
  -webkit-transform: translate(-50%,-50%) scaleY(0);
  transform: translate(-50%,-50%) scaleY(0) ;
}
.ol-overlay.wipe {
  opacity: 1;
  /* clip: must be set programmatically */
  /* clip-path: use % but not crossplatform (IE) */
}
.ol-overlay.flip {
  -webkit-transform: perspective(600px) rotateY(180deg);
  transform: perspective(600px) rotateY(180deg);
}
.ol-overlay.card {
  opacity: 0.5;
  -webkit-transform: translate(-80%, 100%) rotate(-0.5turn);
  transform: translate(-80%, 100%) rotate(-0.5turn);
}
.ol-overlay.book {
  -webkit-transform: perspective(600px) rotateY(-180deg) scaleX(0.6);
  transform: perspective(600px) rotateY(-180deg) scaleX(0.6) ;
  -webkit-transform-origin: 10% 50%;
  transform-origin: 10% 50%;
}
.ol-overlay.book.visible {
  -webkit-transform-origin: 10% 50%;
  transform-origin: 10% 50%;
}

.ol-overlay.ol-visible {
  opacity:1;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  -webkit-transform: none;
  transform: none;
  pointer-events: all;  
}

.ol-overlay .ol-closebox {
  position: absolute;
  top: 1em;
  right: 1em;
  width: 1em;
  height: 1em;
  cursor: pointer;
  z-index:1;
}
.ol-overlay .ol-closebox:before {
  content: "\\274c";
  display: block;
  text-align: center;
  vertical-align: middle;
}

.ol-overlay .ol-fullscreen-image {
  position: absolute;
  top: 0;
  left: 0;
  bottom: 0;
  right: 0;
}
.ol-overlay .ol-fullscreen-image img {
  position: absolute;
  max-width: 100%;
  max-height: 100%;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  padding: 1em;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
  transform: translate(-50%, -50%);
}
.ol-overlay .ol-fullscreen-image.ol-has-title img {
  padding-bottom: 3em;
}
.ol-overlay .ol-fullscreen-image p {
  background-color: rgba(0,0,0,.5);
  padding: .5em;
  position: absolute;
  left: 0;
  right: 0;
  bottom: 0;
  margin: 0;
  text-align: center;
}
.ol-control.ol-overview
{	position: absolute;
	left: 0.5em;
	text-align: left;
	bottom: 0.5em;
}

.ol-control.ol-overview .panel
{	display:block;
	width:150px;
	height:150px;
	margin:2px;
	background-color:#fff;
	border:1px solid #369;
	cursor: pointer;
}

.ol-overview:not(.ol-collapsed) button
{	position:absolute;
	bottom:2px;
	left:2px;
	z-index:2;
}

.ol-control.ol-overview.ol-collapsed .panel
{	display:none;
}

.ol-overview.ol-collapsed button:before
{	content:'\\00bb';
}
.ol-overview button:before
{	content:'\\00ab';
}


.ol-control-right.ol-overview
{	left: auto;
	right: 0.5em;
}
.ol-control-right.ol-overview:not(.ol-collapsed) button
{	left:auto;
	right:2px;
}
.ol-control-right.ol-overview.ol-collapsed button:before
{	content:'\\00ab';
}
.ol-control-right.ol-overview button:before
{	content:'\\00bb';
}

.ol-control-top.ol-overview
{	bottom: auto;
	top: 5em;
}
.ol-control-top.ol-overview:not(.ol-collapsed) button
{	bottom:auto;
	top:2px;
}

.ol-permalink {
  position: absolute;
  top:0.5em;
  right: 2.5em;
}
.ol-touch .ol-permalink {
  right: 3em;
}

.ol-permalink button i {
  position: absolute;
  width: 1em;
  height: 1em;
  display: block;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}
.ol-permalink button i:before {
  content: '\\2197';
  position: absolute;
  border: 1px solid currentColor;
  left: 0;
  top: 0;
  width: 0.3em;
  height: 1em;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  border-width: 1px 0 0 1px;
  padding: 0 0.2em;
}
.ol-permalink button i:after {
  content: '';
  position: absolute;
  border: 1px solid currentColor;
  right: 0;
  bottom: 0;
  width: 1em;
  height: 0.3em;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  border-width: 0 1px 1px 0;
  padding: 0.2em;
}
.ol-control.ol-print {
  top:.5em;
  left: 3em;
}
.ol-control.ol-print button:before {
  content: "";
  width: .9em;
  height: .35em;
  position: absolute;
  left: 50%;
  top: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
  -webkit-box-shadow: inset 0 0 0 0.1em, inset 0.55em 0, 0 0.2em 0 -0.1em;
          box-shadow: inset 0 0 0 0.1em, inset 0.55em 0, 0 0.2em 0 -0.1em;
}
.ol-control.ol-print button:after {
  content: "";
  width: .7em;
  height: .6em;
  position: absolute;
  left: 50%;
  top: 25%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
  -webkit-box-shadow: inset 0 0 0 0.15em;
          box-shadow: inset 0 0 0 0.15em;
}
.ol-ext-print-dialog {
  width: 100%;
  height: 100%;
}
.ol-ext-print-dialog > form .ol-closebox {
  right: auto;
  left: 16.5em;
  z-index: 1;
  color: #999;
}
.ol-ext-print-dialog .ol-content[data-status="printing"] {
  opacity: .5;
}
.ol-ext-print-dialog .ol-content .ol-error {
  display: none;
  background: #b00;
  color: yellow;
  text-align: center;
  padding: 1em .5em;
  font-weight: bold;
  margin: 0 -1em;
}
.ol-ext-print-dialog .ol-content[data-status="error"] .ol-error {
  display: block;
}


.ol-ext-print-dialog > form,
.ol-ext-print-dialog.ol-visible > form {
  -webkit-transition: none;
  transition: none;
  top: 1em;
  left: 1em;
  bottom: 1em;
  right: 1em;
  -webkit-transform: none;
          transform: none;
  max-width: 100%;
  max-height: 100%;
  background-color: #eee;
  padding: 0;
}
.ol-ext-print-dialog .ol-print-map {
  position: absolute;
  top: 0;
  bottom: 0;
  right: 0;
  width: calc(100% - 18em);
  overflow: hidden;
}
.ol-ext-print-dialog .ol-print-map .ol-page {
  position: absolute;
  left: 50%;
  top: 50%;
  background: #fff;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}
.ol-ext-print-dialog .ol-print-map .ol-page.margin {
  -webkit-box-sizing: content-box;
          box-sizing: content-box;
}
.ol-ext-print-dialog .ol-map {
  width: 100%;
  height: 100%;
}
.ol-ext-print-dialog .ol-print-map .ol-control {
  display: none!important;
}

.ol-ext-print-dialog .ol-print-param {
  position: absolute;
  overflow-x: hidden;
  top: 0;
  bottom: 0;
  left: 0;
  width: 18em;
  background-color: #fff;
  padding: 1em;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}

.ol-ext-print-dialog .ol-print-param h2 {
  display: block;
  color: rgba(0,60,136,.7);
  font-size: 1.1em;
}
.ol-ext-print-dialog .ol-print-param ul {
  padding: 0;
  list-style: none;
}
.ol-ext-print-dialog .ol-print-param li {
  position: relative;
  margin: .5em 0;
  font-size: .9em;
}
.ol-ext-print-dialog .ol-print-param li.hidden {
  display: none;
}
.ol-ext-print-dialog .ol-print-param label {
  width: 8em;
  display: inline-block;
  vertical-align: middle;
}

.ol-ext-print-dialog select {
  outline: none;
  vertical-align: middle;
}

.ol-ext-print-dialog .ol-orientation {
  text-align: center;
}
.ol-ext-print-dialog .ol-orientation label {
  position: relative;
  width: 7em;
  cursor: pointer;
}
.ol-ext-print-dialog .ol-orientation input {
  position: absolute;
  opacity: 0;
  width: 0;
  height: 0;
}
.ol-ext-print-dialog .ol-orientation span {
  position: relative;
  width: 80%;
  display: block;
  padding: 3.5em 0 .2em;
}
.ol-ext-print-dialog .ol-orientation span:before {
  content: "";
  position: absolute;
  width: 2em;
  height: 2.6em;
  bottom: 1.5em;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
  color: #333;
  background-color: currentColor;
  border: 1px solid currentColor;
  border-radius: 0 1em 0 0;
  opacity: .5;
  overflow: hidden;
  -webkit-box-shadow: inset 1.3em -1.91em #ddd;
          box-shadow: inset 1.3em -1.91em #ddd;
}

.ol-ext-print-dialog .ol-orientation .landscape span:before {
  width: 2.6em;
  height: 2em;
  margin: .2em 0;
  -webkit-box-shadow: inset 1.91em -1.3em #ddd;
          box-shadow: inset 1.91em -1.3em #ddd;
}
.ol-ext-print-dialog .ol-orientation input:checked + span {
  opacity: 1;
  -webkit-box-shadow: 0 0 .2em rgba(0,0,0,.5);
          box-shadow: 0 0 .2em rgba(0,0,0,.5);
}

.ol-ext-print-dialog .ol-ext-toggle-switch span {
  position: absolute;
  right: -2em;
  top: 50%;
  -webkit-transform: translateY(-50%);
          transform: translateY(-50%);
}

.ol-print-title input[type=text] {
  margin-top: .5em;
  width: calc(100% - 6em);
  margin-left: 6em;
}

.ol-ext-print-dialog .ol-size option:first-child {
  font-style: italic;
}

.ol-ext-print-dialog .ol-saveas,
.ol-ext-print-dialog .ol-savelegend {
  text-align: center;
}
.ol-ext-print-dialog .ol-saveas select,
.ol-ext-print-dialog .ol-savelegend select {
  background-color: rgba(0,60,136,.7);
  color: #fff;
  padding: .5em;
  margin: 1em 0 0;
  font-size: 1em;
  border: 0;
  font-weight: bold;
  max-width: 12em;
}
.ol-ext-print-dialog .ol-saveas select option,
.ol-ext-print-dialog .ol-savelegend select option {
  background-color: #fff;
  color: #666;
}
.ol-ext-print-dialog .ol-savelegend select {
  margin-top: 0;
}

.ol-ext-print-dialog .ol-ext-buttons {
  text-align: right;
  border-top: 1px solid #ccc;
  padding: .8em .5em;
  margin: 0 -1em;
}
.ol-ext-print-dialog button {
  font-size: 1em;
  margin: 0 .2em;
  border: 1px solid #999;
  background: none;
  padding: .3em 1em;
  color: #333;
}
.ol-ext-print-dialog button[type="submit"] {
  background-color: rgba(0,60,136,.7);
  color: #fff;
  font-weight: bold;
}

.ol-ext-print-dialog .ol-clipboard-copy {
  position: absolute;
  pointer-events: none;
  top: 0;
  background-color: rgba(0,0,0,.5);
  color: #fff;
  padding: .5em 1em;
  border-radius: 1em;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
  width: -webkit-fit-content;
  width: -moz-fit-content;
  width: fit-content;
  -webkit-transition: 0s;
  transition: 0s;
  opacity: 0;
}
.ol-ext-print-dialog .ol-clipboard-copy.visible {
  -webkit-animation: 1s ol-clipboard-copy;
          animation: 1s ol-clipboard-copy;
}
.ol-ext-print-dialog .ol-print-map .ol-control.ol-canvas-control {
  display: block!important;
}
.ol-ext-print-dialog .ol-print-map .ol-control.ol-print-compass {
  display: block!important;
}
.ol-ext-print-dialog .ol-print-map .ol-control.olext-print-compass {
  top: 0;
  right: 0;
  width: 60px;
  height: 60px;
  margin: 20px;
}

@-webkit-keyframes ol-clipboard-copy { 
  0% { opacity: 0; top: 0; }
  80% { opacity: 1; top: -3em; }
  100% { opacity: 0; top: -3em; }  
}

@keyframes ol-clipboard-copy { 
  0% { opacity: 0; top: 0; }
  80% { opacity: 1; top: -3em; }
  100% { opacity: 0; top: -3em; }  
}

@media print {
  body.ol-print-document {
    margin: 0!important;
    padding: 0!important;
  }
  body.ol-print-document > * {
    display: none!important;
  }
  body.ol-print-document > .ol-ext-print-dialog {
    display: block!important;
  }
  body.ol-print-document > .ol-ext-print-dialog .ol-content {
    max-height: unset!important;
    max-width: unset!important;
    width: unset!important;
    height: unset!important;
  }
  .ol-ext-print-dialog > form,
  .ol-ext-print-dialog {
    position: unset;
    -webkit-box-shadow: none;
            box-shadow: none;
    background: none!important;
    border: 0;
  }
  .ol-ext-print-dialog > form > *,
  .ol-ext-print-dialog .ol-print-param {
    display: none!important;
    background: none;
  } 
  .ol-ext-print-dialog .ol-content {
    display: block!important;
    border: 0;
    background: none;
  }
  .ol-ext-print-dialog .ol-print-map {
    position: unset; 
    background: none;
    width: auto;
    overflow: visible;
  }
  .ol-ext-print-dialog .ol-print-map .ol-page {
    -webkit-transform: none!important;
            transform: none!important;
    -webkit-box-shadow: none!important;
            box-shadow: none!important;
    position: unset;
  }
}

@media (max-width: 25em) {
  .ol-ext-print-dialog .ol-print-param {
    width: 13em;
  }
  .ol-ext-print-dialog .ol-print-map {
    width: calc(100% - 13em);
  }
  .ol-ext-print-dialog .ol-print-param .ol-print-title input[type="text"] {
    width: 100%;
    margin: 0;
  }
}
.ol-ext-print-dialog2x .ol-map,
.ol-ext-print-dialog2x .ol-map2 {
  width: 100%;
  height: 50%;
}
.ol-ext-print-dialog2x[data-orientation="landscape"] .ol-map,
.ol-ext-print-dialog2x[data-orientation="landscape"] .ol-map2 {
  display: inline-block;
  width: 50%;
  height: 100%;
}

.ol-ext-print-dialog2x[data-clip-map] .ol-map,
.ol-ext-print-dialog2x[data-swipe-orientation] .ol-map {
  width: 100%!important;
  height: 100%!important;
}
.ol-ext-print-dialog2x[data-clip-map] .ol-map2,
.ol-ext-print-dialog2x[data-swipe-orientation] .ol-map2 {
  width: 100%!important;
  height: 100%!important;
  position: relative!important;
  left: 0;
  top: -100%;
}
.ol-profile {
  position: relative;
  -webkit-user-select: none;
     -moz-user-select: none;
      -ms-user-select: none;
          user-select: none;
}
.ol-control.ol-profile {
  position: absolute;
  top: 0.5em;
  right: 3em;
  text-align: right;
  overflow: hidden;
}
.ol-profile .ol-zoom-out {
  position: absolute;
  top: 10px;
  right: 10px;
  width: 1em;
  height: 1em;
  padding: 0;
  border: 1px solid #000;
  border-radius: 2px;
  cursor: pointer;
}
.ol-profile .ol-zoom-out:before {
  content: '';
  height: 2px;
  width: 60%;
  background: currentColor;
  position: absolute;
  left: 50%;
  top: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}

.ol-profile .ol-inner  {
  position: relative;
  padding: 0.5em;
  font-size: 0.8em;
}
.ol-control.ol-profile .ol-inner {
  display: block;
  background-color: rgba(255,255,255,0.7);
  margin: 2.3em 2px 2px;
}
.ol-control.ol-profile.ol-collapsed .ol-inner {
  display: none;
}

.ol-profile canvas {
  display: block;
}
.ol-profile button {
  display: block;
  position: absolute;
  right: 0;
  overflow: hidden;
}
.ol-profile button i {
  position: absolute;
  left: 50%;
  top: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  width: 1em;
  height: 1em;
  overflow: hidden;
}
.ol-profile button i:before,
.ol-profile button i:after {
  content: "";
  position: absolute;
  display: block;
  background-color: currentColor;
  width: 1em;
  height: .9em;
  -webkit-transform: scaleX(.8) translate(-.25em, .5em) rotate(45deg);
          transform: scaleX(.8) translate(-.25em, .5em) rotate(45deg);
}
.ol-profile button i:after {
  -webkit-transform: scaleX(.8) translate(.35em, .7em) rotate(45deg);
          transform: scaleX(.8) translate(.35em, .7em) rotate(45deg);
}

.ol-profile.ol-collapsed button {
  position: static;
}

.ol-profile .ol-profilebar,
.ol-profile .ol-profilecursor {
  position:absolute;
  pointer-events: none;
  width: 1px;
  display: none;
}
.ol-profile .ol-profilecursor {
  width: 0;
  height: 0;
}
.ol-profile .ol-profilecursor:before {
  content:"";
  pointer-events: none;
  display: block;
  margin: -2px;
  width:5px;
  height:5px;
}
.ol-profile .ol-profilebar,
.ol-profile .ol-profilecursor:before {
  background: red;
}

.ol-profile table {
  text-align: center;
  width: 100%;
}

.ol-profile table span {
  display: block;
}

.ol-profilepopup {
  background-color: rgba(255, 255, 255, 0.5);
  margin: 0.5em;
  padding: 0 0.5em;
  position: absolute;
  top:-1em;
  white-space: nowrap;
}
.ol-profilepopup.ol-left {
  right:0;
}


.ol-profile table td {
  padding: 0 2px;
}

.ol-profile table .track-info {
  display: table-row;
}
.ol-profile table .point-info {
  display: none;
}
.ol-profile .over table .track-info {
  display: none;
}
.ol-profile .over table .point-info {
  display: table-row;
}

.ol-profile p {
  text-align: center;
  margin:0;
}

.ol-control.ol-progress-bar {
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 0;
  pointer-events: none!important;
  background-color: transparent;
}

.ol-control.ol-progress-bar > .ol-bar {
  position: absolute;
  background-color: rgba(0,60,136,.5);
  left: 0;
  bottom: 0;
  height: .5em;
  width: 0;
  -webkit-transition: width .2s;
  transition: width .2s;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}

.ol-progress-bar > .ol-waiting {
  display: none;
}

.ol-viewport .ol-control.ol-progress-bar > .ol-waiting {
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  color: #fff;
  font-size: 2em;
  display: block;
  -webkit-animation: 1s linear infinite ol-progress-bar-blink;
          animation: 1s linear infinite ol-progress-bar-blink;
}

@-webkit-keyframes ol-progress-bar-blink {
  0%, 30% {
    visibility: hidden;
  }
  100% {
    visibility: visible;
  }
}

@keyframes ol-progress-bar-blink {
  0%, 30% {
    visibility: hidden;
  }
  100% {
    visibility: visible;
  }
}

.ol-control.ol-routing {
  top: 0.5em;
  left: 3em;
  max-height: 90%;
  overflow-y: auto;
}
.ol-touch .ol-control.ol-routing {
  left: 3.5em;
}
.ol-control.ol-routing.ol-searching {
  opacity: .5;
}

.ol-control.ol-routing .ol-car,
.ol-control.ol-routing > button {
  position: relative;
}
.ol-control.ol-routing .ol-car:after,
.ol-control.ol-routing > button:after {
  content: "";
  position: absolute;
  width: .78em;
  height: 0.6em;
  border-radius: 40% 50% 0 0 / 50% 70% 0 0;
  -webkit-box-shadow: inset 0 0 0 0.065em, -0.35em 0.14em 0 -0.09em, inset 0 -0.37em, inset -0.14em 0.005em;
          box-shadow: inset 0 0 0 0.065em, -0.35em 0.14em 0 -0.09em, inset 0 -0.37em, inset -0.14em 0.005em;
  clip: rect(0 1em .5em -1em);
  top: .35em;
  left: .4em;
}
.ol-control.ol-routing .ol-car:before,
.ol-control.ol-routing > button:before {
  content: "";
  position: absolute;
  width: .28em;
  height: .28em;
  border-radius: 50%;
  -webkit-box-shadow: inset 0 0 0 1em, 0.65em 0;
          box-shadow: inset 0 0 0 1em, 0.65em 0;
  top: 0.73em;
  left: .20em;
}
.ol-control.ol-routing .ol-pedestrian:after {
  content: "";
  position: absolute;
  width: .3em;
  height: .4em;
  top: .25em;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
  -webkit-box-shadow: inset 0.3em 0, 0.1em 0.5em 0 -0.1em, -0.1em 0.5em 0 -0.1em, 0.25em 0.1em 0 -0.1em, -0.25em 0.1em 0 -0.1em;
          box-shadow: inset 0.3em 0, 0.1em 0.5em 0 -0.1em, -0.1em 0.5em 0 -0.1em, 0.25em 0.1em 0 -0.1em, -0.25em 0.1em 0 -0.1em;
  border-top: .2em solid transparent;
}
.ol-control.ol-routing .ol-pedestrian:before {
  content: "";
  position: absolute;
  width: .3em;
  height: .3em;
  top: .1em;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
  border-radius: 50%;
  background-color: currentColor;
}

.ol-control.ol-routing .content {
  margin: .5em;
}

.ol-control.ol-routing.ol-collapsed .content {
  display: none;
}

.ol-routing .ol-search.ol-collapsed ul {
	display: none;
}
.ol-routing .ol-search ul .copy {
  display: none;
}
.ol-routing .ol-search ul.history {
  /* display: none; */
}
.ol-routing .content .search-input > div > * {
  display: inline-block;
  vertical-align: top;
}
.ol-routing .ol-result ul {
  list-style: none;
  display: block;
}
.ol-routing .ol-result li {
  position: relative;
  min-height: 1.65em;
}
.ol-routing .ol-result li i {
  display: block;
  font-size: .8em;
  font-weight: bold;
}

.ol-routing .ol-result li:before {
  content: "";
  border: 5px solid transparent;
  position: absolute;
  left: -1.75em;
  border-bottom-color: #369;
  border-width: .6em .4em .6em;
  -webkit-transform-origin: 50% 125%;
          transform-origin: 50% 125%;
  -webkit-box-shadow: 0 0.65em 0 -0.25em #369;
          box-shadow: 0 0.65em 0 -0.25em #369;
  top: -.8em;
}
.ol-routing .ol-result li:after {
  content: "";
  position: absolute;
  width: 0.25em;
  height: .6em;
  left: -1.5em;
  background: #369;
  top: .6em;
  border-radius: 0.1em;
}
.ol-routing .ol-result li.R:before {
  -webkit-transform: rotate(90deg);
          transform: rotate(90deg);
}
.ol-routing .ol-result li.FR:before {
  -webkit-transform: rotate(45deg);
          transform: rotate(45deg);
}
.ol-routing .ol-result li.L:before {
  -webkit-transform: rotate(-90deg);
          transform: rotate(-90deg);
}
.ol-routing .ol-result li.FL:before {
  -webkit-transform: rotate(-45deg);
          transform: rotate(-45deg);
}

.ol-routing .content > i {
  vertical-align: middle;
  margin: 0 .3em 0 .1em;
  font-style: normal;
}
.ol-routing .ol-button,
.ol-routing .ol-button:focus,
.ol-routing .ol-pedestrian,
.ol-routing .ol-car {
  font-size: 1.1em;
  position: relative;
  display: inline-block;
  width: 1.4em;
  height: 1.4em;
  color: rgba(0,60,136,1);
  background-color: transparent;
  margin: 0 .1em;
  opacity: .5;
  vertical-align: middle;
  outline: none;
  cursor: pointer;
}
.ol-routing .ol-button:hover,
.ol-routing .ol-button.selected,
.ol-routing i.selected {
  opacity: 1;
  background: transparent;
}

.ol-control.ol-routing {
  background-color: rgba(255,255,255,.25);
}
.ol-control.ol-routing:hover {
  background-color: rgba(255,255,255,.75);
}

.search-input > div > button:before {
  content: '\\00b1';
}
.ol-viewport .ol-scale {
	left: .5em;
	bottom: 2.5em;
	text-align: center;
	-webkit-transform: scaleX(.8);
	-webkit-transform-origin: 0 0;
	transform: scaleX(.8);
	transform-origin: 0 0;
	background-color: rgba(255, 255, 255, 0.75);
}
.ol-viewport .ol-scale input {
	background: none;
    border: 0;
    width: 8em;
    text-align: center;
}

.ol-search{
  top: 0.5em;
  left: 3em;
}
.ol-touch .ol-search {
  left: 3.5em;
}
.ol-search button {
  top: 2px;
  left: 2px;
  float: left;
}
.ol-control.ol-search > button:before {
  content: "";
  position: absolute;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  width: .7em;
  height: .7em;
  background-color: transparent;
  border: .12em solid currentColor;
  border-radius: 100%;
  top: .35em;
  left: .35em;
}
.ol-control.ol-search > button:after {
  content: "";
  position: absolute;
  top: 1.1em;
  left: .95em;
  width: .45em;
  height: .15em;
  background-color: currentColor;
  border-radius: .05em;
  -webkit-transform: rotate(45deg);
          transform: rotate(45deg);
  -webkit-box-shadow: -0.18em 0 0 -0.03em;
          box-shadow: -0.18em 0 0 -0.03em;
}

.ol-search button.ol-revers {
  float: none;
  background-image: none;
  display: inline-block;
  vertical-align: bottom;
  position: relative;
  top: 0;
  left: 0;
}
.ol-search.ol-revers button.ol-revers {
  background-color: rgba(0,136,60,.5)
}

.ol-control.ol-search.ol-collapsed button.ol-revers {
  display: none;
}
.ol-search button.ol-revers:before {
  content: "";
  border: .1em solid currentColor;
  position: absolute;
  left: 50%;
  top: 50%;
  -webkit-transform: translate(-50%,-50%);
          transform: translate(-50%,-50%);
  border-radius: 50%;
  width: .55em;
  height: .55em;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}
.ol-search button.ol-revers:after {
  content: "";
  position: absolute;
  left: 50%;
  top: 50%;
  -webkit-transform: translate(-50%,-50%);
          transform: translate(-50%,-50%);
  width: .2em;
  height: .2em;
  background-color: transparent;
  -webkit-box-shadow: .35em 0 currentColor, 0 .35em currentColor, -.35em 0 currentColor, 0 -.35em currentColor;
          box-shadow: .35em 0 currentColor, 0 .35em currentColor, -.35em 0 currentColor, 0 -.35em currentColor;
}

.ol-search input {
  display: inline-block;
  border: 0;
  margin: 1px 1px 1px 2px;
  font-size: 1.14em;
  padding-left: 0.3em;
  height: 1.375em;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  -webkit-transition: all 0.1s;
  transition: all 0.1s;
}
.ol-touch .ol-search input,
.ol-touch .ol-search ul {
  font-size: 1.5em;
}
.ol-search.ol-revers > ul,
.ol-control.ol-search.ol-collapsed > * {
  display: none;
}
.ol-control.ol-search.ol-collapsed > button {
  display: block;
}

.ol-search ul {
  list-style: none;
  padding: 0;
  margin: 0;
  display: block;
  clear: both;
  cursor: pointer;
  max-width: 17em;
  overflow-x: hidden;
  z-index: 1;
  background: #fff;
}
/*
.ol-control.ol-search ul {
  position: absolute;
  box-shadow: 5px 5px 5px rgba(0,0,0,0.5);
}
*/
.ol-search ul li {
  padding: 0.1em 0.5em;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.ol-search ul li.select,
.ol-search ul li:hover {
  background-color: rgba(0,60,136,.5);
  color: #fff;
}
.ol-search ul li img {
  float: left;
  max-height: 2em;
}
.ol-search li.copy {
    background: rgba(0,0,0,.5);
  color: #fff;
}
.ol-search li.copy a {
  color: #fff;
  text-decoration: none;
}

.ol-search.searching:before {
  content: '';
  position: absolute;
  height: 3px;
  left: 0;
  top: 1.6em;
  -webkit-animation: pulse .5s infinite alternate linear;
          animation: pulse .5s infinite alternate linear;
  background: red;
  z-index: 2;
}

@-webkit-keyframes pulse {
  0% { left:0; right: 95%; }
  50% {	left: 30%; right: 30%; }
  100% {	left: 95%; right: 0; }
}

@keyframes pulse {
  0% { left:0; right: 95%; }
  50% {	left: 30%; right: 30%; }
  100% {	left: 95%; right: 0; }
}


.ol-control.ol-search.IGNF-parcelle {
  background-color: rgba(255,255,255,.7);
  padding-bottom: .2em;
}
.ol-search.IGNF-parcelle input {
  width: 14.5em;
}
.ol-search.IGNF-parcelle input:-moz-read-only {
  background: #ccc;
  opacity: .8;
}
.ol-search.IGNF-parcelle input:read-only {
  background: #ccc;
  opacity: .8;
}
.ol-search.IGNF-parcelle.ol-collapsed-list > ul.autocomplete {
  display: none;
}

.ol-search.IGNF-parcelle label {
  display: block;
  clear: both;
}

.ol-search.IGNF-parcelle > div input {
  text-align: right;
}
.ol-search.IGNF-parcelle > div input,
.ol-search.IGNF-parcelle > div label {
  width: 4em;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  display: inline-block;
  margin: .1em;
  font-size: 1em;
  overflow: hidden;
  text-overflow: ellipsis;
}
.ol-search.IGNF-parcelle > div label {
  letter-spacing: -0.04em;
}
.ol-search.IGNF-parcelle ul.autocomplete-page {
  margin-top:.5em;
  width:100%;
  text-align: center;
  display: none;
}
.ol-search.IGNF-parcelle.ol-collapsed-list ul.autocomplete-parcelle,
.ol-search.IGNF-parcelle.ol-collapsed-list ul.autocomplete-page {
  display: block;
}
.ol-search.IGNF-parcelle.ol-collapsed ul.autocomplete-page,
.ol-search.IGNF-parcelle.ol-collapsed ul.autocomplete-parcelle,
.ol-search.IGNF-parcelle ul.autocomplete-parcelle {
  display: none;
}
.ol-search.IGNF-parcelle ul.autocomplete-page li {
  display: inline-block;
  color: #fff;
  background: rgba(0,60,136,.5);
  border-radius: 50%;
  width: 1.3em;
  height: 1.3em;
  padding: .1em;
  margin: 0 .1em;
}
.ol-search.IGNF-parcelle ul.autocomplete-page li.selected {
  background: rgba(0,60,136,1);
}

/* GPS */
.ol-searchgps input.search,
.ol-searchcoord input.search {
  display: none;
}
.ol-control.ol-searchgps > button:first-child,
.ol-control.ol-searchcoord > button:first-child {
  background-image: none;
}
.ol-control.ol-searchgps > button:first-child:before,
.ol-control.ol-searchcoord > button:first-child:before {
  content: "x/y";
  position: unset;
  display: block;
  -webkit-transform: scaleX(.8);
          transform: scaleX(.8);
  border: unset;
  border-radius: 0;
  width: auto;
  height: auto;
}
.ol-control.ol-searchgps > button:first-child:after,
.ol-control.ol-searchcoord > button:first-child:after {
  content: unset;
}
.ol-control.ol-searchgps .ol-latitude,
.ol-control.ol-searchgps .ol-longitude,
.ol-control.ol-searchcoord .ol-latitude,
.ol-control.ol-searchcoord .ol-longitude {
  clear: both;
}
.ol-control.ol-searchgps .ol-latitude label,
.ol-control.ol-searchgps .ol-longitude label,
.ol-control.ol-searchcoord .ol-latitude label,
.ol-control.ol-searchcoord .ol-longitude label {
  width: 5.5em;
  display: inline-block;
  text-align: right;
  -webkit-transform: scaleX(.8);
          transform: scaleX(.8);
  margin: 0 -.8em 0 0;
  -webkit-transform-origin: 0 0;
          transform-origin: 0 0;
}
.ol-control.ol-searchcoord .ol-latitude label,
.ol-control.ol-searchcoord .ol-longitude label {
  width: 2em;
  margin: 0;
}
.ol-control.ol-searchgps .ol-latitude input,
.ol-control.ol-searchgps .ol-longitude input {
  max-width: 10em;
}

.ol-control.ol-searchgps .ol-ext-toggle-switch {
  cursor: pointer;
  float: right;
  margin: .5em;
  font-size: .9em;
}

.ol-searchgps .ol-decimal{
  display: inline-block;
  margin-right: .7em;
}
.ol-searchgps .ol-dms,
.ol-searchgps.ol-dms .ol-decimal {
  display: none;
  width: 3em;
  text-align: right;
}
.ol-searchgps.ol-dms .ol-dms {
  display: inline-block;
}

.ol-searchgps span.ol-dms {
  width: .5em;
  text-align: left;
}
.ol-searchgps.ol-control.ol-collapsed button.ol-geoloc,
.ol-searchgps.ol-control.ol-collapsed button.ol-centerloc,
.ol-searchcoord.ol-control.ol-collapsed button.ol-centerloc,
.ol-searchcoord.ol-control.ol-collapsed button.ol-geoloc {
  display: none;
}
.ol-searchgps button.ol-geoloc,
.ol-searchgps button.ol-centerloc,
.ol-searchcoord button.ol-centerloc,
.ol-searchcoord button.ol-geoloc {
  top: 0;
  float: right;
  margin-right: 3px;
  background-image: none;
  position: relative;
}
.ol-searchgps button.ol-geoloc:before,
.ol-searchcoord button.ol-geoloc:before {
  content:"";
  position: absolute;
  left: 50%;
  top: 50%;
  width: .6em;
  height: .6em;
  border: .1em solid currentColor;
  border-radius: 50%;
  -webkit-transform: translate(-50%,-50%);
          transform: translate(-50%,-50%);
}
.ol-searchgps button.ol-geoloc:after,
.ol-searchcoord button.ol-geoloc:after {
  content:"";
  position: absolute;
  left: 50%;
  top: 50%;
  width: .2em;
  height: .2em;
  background-color: transparent;
  -webkit-transform: translate(-50%,-50%);
          transform: translate(-50%,-50%);
  -webkit-box-shadow: 
    .45em 0 currentColor, -.45em 0 currentColor, 0 -.45em currentColor, 0 .45em currentColor,
    .25em 0 currentColor, -.25em 0 currentColor, 0 -.25em currentColor, 0 .25em currentColor;
          box-shadow: 
    .45em 0 currentColor, -.45em 0 currentColor, 0 -.45em currentColor, 0 .45em currentColor,
    .25em 0 currentColor, -.25em 0 currentColor, 0 -.25em currentColor, 0 .25em currentColor;
}

.ol-searchgps button.ol-centerloc:before,
.ol-searchcoord button.ol-centerloc:before {
  content: "\u{1F4CD}";
  border: 0;
  width: auto;
  height: auto;
  padding: 0;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}
.ol-searchgps button.ol-centerloc:after,
.ol-searchcoord button.ol-centerloc:after {
  display: none;
}
.ol-control.ol-select {
  top: .5em;
  left: 3em;
  background-color: rgba(255,255,255,.5);
}
.ol-control.ol-select:hover {
  background-color: rgba(255,255,255,.7);
}
.ol-touch .ol-control.ol-select {
  left: 3.5em;
}
.ol-control.ol-select > button:before {
  content: "A";
  font-size: .6em;
  font-weight: normal;
  position: absolute;
  -webkit-box-sizing: content-box;
          box-sizing: content-box;
  width: 1em;
  height: 1em;
  background-color: transparent;
  border: .2em solid currentColor;
  border-radius: 100%;
  top: .5em;
  left: .5em;
  line-height: 1em;
  text-align: center;
}
.ol-control.ol-select > button:after {
  content: "";
  position: absolute;
  top: 1.15em;
  left: 1em;
  width: .45em;
  height: .15em;
  background-color: currentColor;
  border-radius: .05em;
  -webkit-transform: rotate(45deg);
          transform: rotate(45deg);
  -webkit-box-shadow: -0.18em 0 0 -0.03em;
          box-shadow: -0.18em 0 0 -0.03em;
}
.ol-select > div button {
  width: auto;
  padding: 0 .5em;
  float: right;
  font-weight: normal;
  height: 1.2em;
  line-height: 1.2em;
}
.ol-select .ol-delete {
    width: 1.5em;
  height: 1em;
  vertical-align: middle;
  display: inline-block;
  position: relative;
  cursor: pointer;
}
.ol-select .ol-delete:before {
  content:'\\00d7';
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  width: 100%;
  text-align: center;
  font-weight: bold;
}
.ol-control.ol-select input {
  font-size: 1em;
}
.ol-control.ol-select select {
  font-size: 1em;
  max-width: 10em;
}
.ol-control.ol-select select option.ol-default {
  color: #999;
  font-style: italic;
}
.ol-control.ol-select > div {
  display: block;
  margin: .25em;
}
.ol-control.ol-select.ol-collapsed > div {
  display: none;
}

.ol-control.ol-select.ol-select-check {
  max-width: 20em;
}
.ol-control.ol-select label.ol-ext-check {
  margin-right: 1em;
}
.ol-control.ol-select label.ol-ext-toggle-switch span {
  font-size: 1.1em;
}

.ol-select ul {
  list-style: none;
  margin: 0;
  padding: 0;
}
.ol-control.ol-select input[type="search"],
.ol-control.ol-select input[type="text"]  {
  width: 8em;
}

.ol-select .ol-autocomplete {
  display: inline;
}
.ol-select .ol-autocomplete ul {
  position: absolute;
  display: block;
  background: #fff;
  border: 1px solid #999;
  min-width: 10em;
  font-size: .85em;
}
.ol-select .ol-autocomplete ul li {
  padding: 0 .5em;
}
.ol-select .ol-autocomplete ul li:hover {
  color: #fff;
  background: rgba(0,60,136,.5);
}
.ol-select ul.ol-hidden {
  display: none;
}

.ol-select-multi li > div:hover,
.ol-select-multi li > div.ol-control.ol-select {
  position: relative;
  top: unset;
  left: unset;
  background: transparent;
}
.ol-select-multi li > div  > button,
.ol-select-multi li > div  .ol-ok {
  display: none;
}
.ol-select-multi li .ol-control.ol-select.ol-collapsed > div,
.ol-select-multi li > div  > div {
  display: block;
}

.ol-control.ol-status {
  top: 0;
  left: 0;
  background: rgba(0,0,0,.2);
  color: #fff;
  font-size: .9em;
  padding: .3em 3em;
  border-radius: 0;
  width: 100%;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  pointer-events: none!important;
  display: none;
}
.ol-control.ol-status.ol-visible {
  display: initial;
}
.ol-control.ol-status.ol-bottom {
  top: auto;
  bottom: 0;
}
.ol-control.ol-status.ol-left {
  top: 0;
  bottom: 0;
  padding: .3em .5em .3em 3em;
  width: auto;
}
.ol-control.ol-status.ol-right {
  top: 0;
  bottom: 0;
  left: auto;
  right: 0;
  padding: .3em 3em .3em .5em;
  width: auto;
}
.ol-control.ol-status.ol-center {
  top: 50%;
  -webkit-transform: translateY(-50%);
          transform: translateY(-50%);
}

.ol-control.ol-storymap {
  top: .5em;
  left: .5em;
  bottom: .5em;
  max-width: 35%;
  border-radius: .5em;
  position: absolute;
  height: auto;
  background-color: rgba(255,255,255,.5);
}
.ol-storymap {
  overflow: hidden;
  padding: 0;
  height: 100%;
  position: relative;
}
.ol-storymap > div {
  overflow: hidden;
  padding: 0;
  height: 100%;
  position: relative;
  scroll-behavior: smooth;
  -webkit-user-select: none;
     -moz-user-select: none;
      -ms-user-select: none;
          user-select: none;
}
.ol-storymap >div.ol-move {
  scroll-behavior: unset;
}

.ol-control.ol-storymap .chapter {
  position: relative;
  padding: .5em;
  overflow: hidden;
}
.ol-control.ol-storymap .chapter:last-child {
  margin-bottom: 100%;
}
.ol-storymap .chapter {
  cursor: pointer;
  opacity: .4;
}
.ol-storymap .chapter.ol-select {
  cursor: default;
  opacity: 1;
  background-color: rgba(255,255,255,.8);
}

.ol-storymap .ol-scroll-top,
.ol-storymap .ol-scroll-next {
  position: relative;
  min-height: 1.7em;
  color: rgba(0,60,136,.5);
  text-align: center;
  cursor: pointer;
}
.ol-storymap .ol-scroll-next span {
  padding-bottom: 1.4em;
  display: block;
}
.ol-storymap .ol-scroll-top span {
  padding-top: 1.4em;
  display: block;
}

.ol-storymap .ol-scroll-top:before,
.ol-storymap .ol-scroll-next:before {
  content: "";
  border: .3em solid currentColor;
  border-radius: .3em;
  border-color: transparent currentColor currentColor transparent;
  width: .8em;
  height: .8em;
  display: block;
  position: absolute;
  left: 50%;
  -webkit-transform: translateX(-50%) rotate(45deg);
          transform: translateX(-50%) rotate(45deg);
  -webkit-animation: ol-bounce-bottom 0.35s linear infinite alternate;
          animation: ol-bounce-bottom 0.35s linear infinite alternate;
  pointer-events: none;
}
.ol-storymap .ol-scroll-top:before {
  border-color: currentColor transparent transparent currentColor;
  -webkit-animation: ol-bounce-top 0.35s linear infinite alternate;
          animation: ol-bounce-top 0.35s linear infinite alternate;
}

@-webkit-keyframes ol-bounce-top{
  from {top: -.2em;}
  to   {top: .5em;}
}

@keyframes ol-bounce-top{
  from {top: -.2em;}
  to   {top: .5em;}
}
@-webkit-keyframes ol-bounce-bottom{
  from {bottom: -.2em;}
  to   {bottom: .5em;}
}
@keyframes ol-bounce-bottom{
  from {bottom: -.2em;}
  to   {bottom: .5em;}
}

.ol-storymap img[data-title] {
  cursor: pointer;
}

/* scrollLine / scrollbox */
.ol-storymap.scrollLine,
.ol-storymap.scrollBox {
  top: 0;
  bottom: 0;
  background-color: transparent;
  border-radius: 0;
  max-width: 40%;
}
.ol-storymap.scrollLine .chapter,
.ol-storymap.scrollBox .chapter {
  background-color: #fff;
  margin: 100% 0;
}
.ol-storymap.scrollLine .chapter:first-child,
.ol-storymap.scrollBox .chapter:first-child {
  margin-top: 3em;
}
.ol-storymap.scrollLine .chapter.ol-select,
.ol-storymap.scrollLine .chapter,
.ol-storymap.scrollBox .chapter.ol-select,
.ol-storymap.scrollBox .chapter {
  opacity: 1;
}

.ol-storymap.scrollLine .ol-scrolldiv,
.ol-storymap.scrollBox .ol-scrolldiv {
  padding-right: 30px;
}
.ol-storymap.scrollLine:before,
.ol-storymap.scrollBox:before {
  content: "";
  position: absolute;
  width: 2px;
  height: 100%;
  top: 0;
  bottom: 0;
  right: 14px;
  background-color: #fff;
}

.ol-storymap.scrollLine .ol-scroll,
.ol-storymap.scrollBox .ol-scroll {
  display: block!important;
  padding: 0;
  width: 1px;
  opacity: 1!important;
  right: 15px;
  overflow: visible;
  -webkit-transition: none;
  transition: none;
}
.ol-storymap.scrollLine .ol-scroll > div {
  background-color: transparent;
  overflow: visible;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
  -webkit-box-shadow: unset;
          box-shadow: unset;
}
.ol-storymap.scrollLine .ol-scroll > div:before {
  content: "";
  position: absolute;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background-color: #0af;
  border: 2px solid #fff;
  left: 50%;
  top: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}

.ol-storymap.scrollBox .ol-scroll > div {
  display: none;
}
.ol-storymap.scrollBox .chapter:after {
  content: "";
  width: 20px;
  height: 20px;
  position: absolute;
  top: Min(30%, 5em);
  right: -24.5px;
  -webkit-box-shadow: 0 0 0 2px #fff, inset 0 0 0 15px #0af;
          box-shadow: 0 0 0 2px #fff, inset 0 0 0 15px #0af; 
  border-radius: 50%;
  border: 5px solid transparent;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  z-index: 1;
}

.ol-swipe {
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  -ms-touch-action: none;
      touch-action: none;
}

.ol-swipe:before {
  content: "";
  position: absolute;
  top: -5000px;
  bottom: -5000px;
  left: 50%;
  width: 4px;
  background: #fff;
  z-index:-1;
  -webkit-transform: translate(-2px, 0);
          transform: translate(-2px, 0);
}
.ol-swipe.horizontal:before {
  left: -5000px;
  right: -5000px;
  top: 50%;
  bottom: auto;
  width: auto;
  height: 4px;
}

.ol-swipe,
.ol-swipe button {
  cursor: ew-resize;
}
.ol-swipe.horizontal,
.ol-swipe.horizontal button {
  cursor: ns-resize;
}

.ol-swipe:after,
.ol-swipe button:before,
.ol-swipe button:after {
  content: "";
  position: absolute;
  top: 25%;
  bottom: 25%;
  left: 50%;
  width: 2px;
  background: currentColor;
  transform: translate(-1px, 0);
  -webkit-transform: translate(-1px, 0);
}
.ol-swipe button:after {
  -webkit-transform: translateX(4px);
          transform: translateX(4px);
}
.ol-swipe button:before {
  -webkit-transform: translateX(-6px);
          transform: translateX(-6px);
}

.ol-control.ol-timeline {
  bottom: 0;
  left: 0;
  right: 0;
  -webkit-transition: .3s;
  transition: .3s;
  background-color: rgba(255,255,255,.4);
}
.ol-control.ol-timeline.ol-collapsed {
  -webkit-transform: translateY(100%);
          transform: translateY(100%);
}
.ol-timeline {
  overflow: hidden;
  padding: 2px 0 0;
}
.ol-timeline .ol-scroll {
  overflow: hidden;
  padding: 0;
  scroll-behavior: smooth;
  line-height: 1em;
  height: 6em;
  padding: 0 50%;
}
.ol-timeline .ol-scroll.ol-move {
  scroll-behavior: unset;
}

.ol-timeline.ol-hasbutton .ol-scroll {
  margin-left: 1.5em;
  padding: 0 calc(50% - .75em);
}
.ol-timeline .ol-buttons {
  display: none;
  position: absolute;
  top: 0;
  background: rgba(255,255,255,.5);
  width: 1.5em;
  bottom: 0;
  left: 0;
  z-index: 10;
}
.ol-timeline.ol-hasbutton .ol-buttons {
  display: block;
}
.ol-timeline .ol-buttons button {
  font-size: 1em;
  margin: 1px;
  position: relative;
}
.ol-timeline .ol-buttons .ol-zoom-in:before,
.ol-timeline .ol-buttons .ol-zoom-out:before {
  content: "+";
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}
.ol-timeline .ol-buttons .ol-zoom-out:before{
  content: '\u2212';
}

.ol-timeline .ol-scroll > div {
  height: 100%;
  position: relative;
}

.ol-timeline .ol-scroll .ol-times {
  background: rgba(255,255,255,.5);
  height: 1em;
  bottom: 0;
  position: absolute;
  left: -1000px;
  right: -1000px;
}
.ol-timeline .ol-scroll .ol-time {
  position: absolute;
  font-size: .7em;
  color: #999;
  bottom: 0;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
}
.ol-timeline .ol-scroll .ol-time.ol-year {
  color: #666;
  z-index: 1;
}
.ol-timeline .ol-scroll .ol-time:before {
  content: "";
  position: absolute;
  bottom: 1.2em;
  left: 50%;
  height: 500px;
  border-left: 1px solid currentColor;
}

.ol-timeline .ol-scroll .ol-features {
  position: absolute;
  top: 0;
  bottom: 1em;
  left: -200px;
  right: -1000px;
  margin: 0 0 0 200px;
  overflow: hidden;
}

.ol-timeline .ol-scroll .ol-feature {
  position: absolute;
  font-size: .7em;
  color: #999;
  top: 0;
  background: #fff;
  max-width: 3em;
  max-height: 2.4em;
  min-height: 1em;
  line-height: 1.2em;
  border: 1px solid #ccc;
  overflow: hidden;
  padding: 0 .5em 0 0;
  -webkit-transition: all .3s;
  transition: all .3s;
  cursor: pointer;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}

.ol-timeline.ol-zoomhover .ol-scroll .ol-feature:hover,
.ol-timeline.ol-zoomhover .ol-scroll .ol-feature.ol-select {
  z-index: 1;
  -webkit-transform: scale(1.2);
          transform: scale(1.2);
  background: #eee;
  /* max-width: 14em!important; */
}

/* Center */
.ol-timeline .ol-center-date {
  display: none;
  position: absolute;
  left: 50%;
  height: 100%;
  width: 2px;
  bottom: 0;
  z-index: 2;
  pointer-events: none;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
  background-color: #f00;
  opacity: .4;
}
.ol-timeline.ol-hasbutton .ol-center-date {
  left: calc(50% + .75em);
}

/* Show center */ 
.ol-timeline.ol-pointer .ol-center-date {
  display: block;
}
.ol-timeline.ol-pointer .ol-center-date:before, 
.ol-timeline.ol-pointer .ol-center-date:after {
  content: '';
  border: 0.3em solid transparent;
  border-width: .3em .25em;
  position: absolute;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
}
.ol-timeline.ol-pointer .ol-center-date:before {
  border-top-color: #f00;
  top: 0;
}
.ol-timeline.ol-pointer .ol-center-date:after {
  border-bottom-color: #f00;
  bottom: 0
}

/* show interval */
.ol-timeline.ol-interval .ol-center-date {
  display: block;
  background-color: transparent;
  border: 0 solid #000;
  border-width: 0 10000px;
  -webkit-box-sizing: content-box;
          box-sizing: content-box;
  opacity: .2;
}
.ol-control.ol-videorec {
  top: 0;
  left: 50%;
  -webkit-transform: translateX(-50%);
          transform: translateX(-50%);
  white-space: nowrap;
}

.ol-control.ol-videorec button {
  position: relative;
  display: inline-block;
  vertical-align: middle;
}

.ol-control.ol-videorec button:before {
  content: "";
  position: absolute;
  top: 50%;
  left: 50%;
  width: .8em;
  height: .8em;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  background-color: currentColor;
}
.ol-control.ol-videorec button.ol-start:before {
  width: .9em;
  height: .9em;
  border-radius: 50%;
  background-color: #c00;
}
.ol-control.ol-videorec button.ol-pause:before {
  width: .2em;
  background-color: transparent;
  -webkit-box-shadow: -.2em 0, .2em 0;
          box-shadow: -.2em 0, .2em 0;
}
.ol-control.ol-videorec button.ol-resume:before {
  border-style: solid;
  background: transparent;
  width: auto;
  border-width: .4em 0 .4em .6em;
  border-color: transparent transparent transparent currentColor;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}

.ol-control.ol-videorec button.ol-stop,
.ol-control.ol-videorec button.ol-pause,
.ol-control.ol-videorec button.ol-resume,
.ol-control.ol-videorec[data-state="rec"] .ol-start,
.ol-control.ol-videorec[data-state="pause"] .ol-start {
  display: none;
}
.ol-control.ol-videorec[data-state="rec"] .ol-stop,
.ol-control.ol-videorec[data-state="pause"] .ol-stop,
.ol-control.ol-videorec[data-state="rec"] .ol-pause,
.ol-control.ol-videorec[data-state="pause"] .ol-resume {
  display: inline-block;
}

.ol-control.ol-wmscapabilities {
  top: .5em;
  right: 2.5em;
}
.ol-touch .ol-control.ol-wmscapabilities {
  right: 3em;
}
.ol-control.ol-wmscapabilities.ol-hidden {
  display: none;
}
.ol-control.ol-wmscapabilities button:before {
  content: "+";
  position: absolute;
  top: calc(50% - .35em);
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}
.ol-control.ol-wmscapabilities button:after {
  content: "";
  width: .75em;
  height: .75em;
  position: absolute;
  background: transparent;
  top: calc(50% - .05em);
  left: 50%;
  -webkit-transform: scaleY(.6) translate(-50%, -50%) rotate(45deg);
          transform: scaleY(.6) translate(-50%, -50%) rotate(45deg);
  -webkit-box-shadow: inset -.18em -.18em currentColor, -.4em .1em 0 -.25em currentColor, .1em -.35em 0 -.25em currentColor, .15em .15em currentColor;
          box-shadow: inset -.18em -.18em currentColor, -.4em .1em 0 -.25em currentColor, .1em -.35em 0 -.25em currentColor, .15em .15em currentColor;
  border-radius: .1em 0;
  border: .15em solid transparent;
  border-width: 0 .15em .15em 0;
}

.ol-wmscapabilities .ol-searching {
  opacity: .5;
}
.ol-wmscapabilities .ol-searching .ol-url:after{
  content: "";
  width: .7em;
  height: .7em;
  background-color: currentColor;
  position: absolute;
  top: 6em;
  border-radius: 50%;
  display: block;
  left: calc(50% - .35em);
  -webkit-box-shadow: 0 1em currentColor, 0 -1em currentColor, 1em 0 currentColor, -1em 0 currentColor;
          box-shadow: 0 1em currentColor, 0 -1em currentColor, 1em 0 currentColor, -1em 0 currentColor;
  -webkit-animation:ol-wmscapabilities-rotate 2s linear infinite;
          animation:ol-wmscapabilities-rotate 2s linear infinite;
}
@-webkit-keyframes ol-wmscapabilities-rotate { 
  100% { -webkit-transform:rotate(360deg); transform:rotate(360deg); } 
}
@keyframes ol-wmscapabilities-rotate { 
  100% { -webkit-transform:rotate(360deg); transform:rotate(360deg); } 
}

.ol-wmscapabilities .ol-url input {
  width: calc(100% - 10em);
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  min-width: Min(100%, 20em);
}
.ol-wmscapabilities .ol-url select {
  width: 2em;
  height: 100%;
  padding: 1px;
}
.ol-wmscapabilities .ol-url button {
  width: 7.5em;
  margin-left: .5em;
}
.ol-wmscapabilities .ol-result {
  display: none;
  margin-top: .5em;
}
.ol-wmscapabilities .ol-result.ol-visible {
  display: block;
}

.ol-wmscapabilities .ol-select-list {
  position: relative;
  border: 1px solid #369;
  overflow-x: hidden;
  width: calc(100% - 120px);
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  max-height: 14.5em;
}
.ol-wmscapabilities .ol-select-list div {
  padding: 0 .5em;
  cursor: pointer;
  width: 100%;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  text-overflow: ellipsis;
  overflow: hidden;
}
.ol-wmscapabilities .ol-select-list .level-1 {
  padding-left: 1em;
}
.ol-wmscapabilities .ol-select-list .level-2 {
  padding-left: 1.5em;
}
.ol-wmscapabilities .ol-select-list .level-3 {
  padding-left: 2em;
}
.ol-wmscapabilities .ol-select-list .level-4 {
  padding-left: 2.5em;
}
.ol-wmscapabilities .ol-select-list .level-5 {
  padding-left: 3em;
}

.ol-wmscapabilities .ol-select-list .ol-info {
  font-style: italic;
}
.ol-wmscapabilities .ol-select-list .ol-title {
  background-color: rgba(0,60,136,.1);
}
.ol-wmscapabilities .ol-select-list div:hover {
  background-color: rgba(0,60,136,.5);
  color: #fff;
}
.ol-wmscapabilities .ol-select-list div.selected {
  background-color: rgba(0,60,136,.7);
  color: #fff;
}

.ol-wmscapabilities .ol-preview {
  width: 100px;
  float: right;
  background: rgba(0,60,136,.1);
  color: #666;
  padding: 0 5px 5px;
  text-align: center;
  margin-left: 10px;
}
.ol-wmscapabilities .ol-preview.tainted {
  width: 100px;
  float: right;
  background: rgba(136,0,60,.1);
  color: #666;
  padding: 0 5px 5px;
  text-align: center;
  margin-left: 10px;
}
.ol-wmscapabilities .ol-preview img {
  width: 100%;
  display: block;
  background: #fff;
}
.ol-wmscapabilities .ol-legend {
  max-width: 100%;
  display: none;
}
.ol-wmscapabilities .ol-legend.visible {
  display: block;
}
.ol-wmscapabilities .ol-buttons {
  clear: both;
  text-align: right;
}
.ol-wmscapabilities .ol-data p {
  margin: 0;
}
.ol-wmscapabilities .ol-data p.ol-title {
  font-weight: bold;
  margin: 1em 0 .5em;
}
.ol-wmscapabilities .ol-error {
  color: #800;
}

.ol-wmscapabilities ul.ol-wmsform {
  display: none;
  list-style: none;
  padding: 0;
}
.ol-wmscapabilities ul.ol-wmsform.visible {
  display: block;
}
.ol-wmscapabilities .ol-wmsform label {
  display: inline-block;
  text-align: right;
  width: calc(40% - .5em);
  margin-right: .5em;
}
.ol-wmscapabilities .ol-wmsform input {
  display: inline-block;
  width: 60%;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}
.ol-wmscapabilities .ol-wmsform input[type="checkbox"] {
  width: auto;
}
.ol-wmscapabilities .ol-wmsform button {
  float: right;
  margin: 1em 0;
}

.ol-wmscapabilities ul.ol-wmsform li[data-param="extent"] input {
  width: calc(60% - 2em);
}
.ol-wmscapabilities ul.ol-wmsform li[data-param="extent"] button {
  position: relative;
  width: 2em;
  height: 1.6em;
  margin: 0;
  vertical-align: middle;
  color: #444;
}
.ol-wmscapabilities ul.ol-wmsform li[data-param="extent"] button:before,
.ol-wmscapabilities ul.ol-wmsform li[data-param="extent"] button:after {
  content: "";
  position: absolute;
  width: .25em;
  height: .9em;
  border: .1em solid currentColor;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%) skewY(-15deg);
          transform: translate(-50%, -50%) skewY(-15deg);
}
.ol-wmscapabilities ul.ol-wmsform li[data-param="extent"] button:after {
  -webkit-transform: translateX(.4em) translate(-50%, -50%) skewY(15deg);
          transform: translateX(.4em) translate(-50%, -50%) skewY(15deg);
  -webkit-box-shadow: -0.8em 0.25em;
          box-shadow: -0.8em 0.25em;
}

.ol-ext-dialog.ol-wmscapabilities form {
  width: 600px;
  min-height: 15em;
  top: 15%;
  -webkit-transform: translate(-50%, -15%);
          transform: translate(-50%, -15%);
}
.ol-ext-dialog.ol-wmscapabilities .ol-content {
  max-height: calc(100vh - 6em);
}

.ol-ext-dialog.ol-wmtscapabilities [data-param="map"] {
  display: none;
}
.ol-ext-dialog [data-param="style"] {
  display: none;
}
.ol-ext-dialog.ol-wmtscapabilities [data-param="style"] {
  display: list-item;
}
.ol-ext-dialog.ol-wmtscapabilities [data-param="proj"],
.ol-ext-dialog.ol-wmtscapabilities [data-param="version"] {
  opacity: .6;
  pointer-events: none;
}

.ol-ext-dialog.ol-wmscapabilities button.ol-wmsform {
  width: 1.8em;
  text-align: center;
}
.ol-ext-dialog.ol-wmscapabilities button.ol-wmsform:before {
  content: "+";
}
.ol-ext-dialog.ol-wmscapabilities .ol-form button.ol-wmsform:before {
  content: "-";
}

.ol-ext-dialog.ol-wmscapabilities .ol-form button.ol-load,
.ol-ext-dialog.ol-wmscapabilities .ol-form .ol-legend {
  display: none;
}
.ol-ext-dialog.ol-wmscapabilities .ol-form ul.ol-wmsform {
  display: block;
  clear: both;
}
.ol-target-overlay .ol-target 
{	border: 1px solid transparent;
	-webkit-box-shadow: 0 0 1px 1px #fff;
	        box-shadow: 0 0 1px 1px #fff;
	display: block;
	height: 20px;
	width: 0;
}

.ol-target-overlay .ol-target:after,
.ol-target-overlay .ol-target:before
{	content:"";
	border: 1px solid #369;
	-webkit-box-shadow: 0 0 1px 1px #fff;
	        box-shadow: 0 0 1px 1px #fff;
	display: block;
	width: 20px;
	height: 0;
	position:absolute;
	top:10px;
	left:-10px;
}
.ol-target-overlay .ol-target:after
{	-webkit-box-shadow: none;	box-shadow: none;
	height: 20px;
	width: 0;
	top:0px;
	left:0px;
}

.ol-overlaycontainer .ol-touch-cursor {
  /* human fingertips are typically 16x20 mm = 45x57 pixels
    source: http://touchlab.mit.edu/publications/2003_009.pdf
  */
  width: 56px;
  height: 56px;
  margin: 6px;
  border-radius: 50%;
  cursor: pointer;
  background: rgba(255,255,255,.4);
  -webkit-box-shadow: inset 0 0 0 5px #369;
          box-shadow: inset 0 0 0 5px #369;
}

.ol-overlaycontainer .ol-touch-cursor:after {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  width: 50%;
  height: 50%;
  background: radial-gradient(circle at 100% 100%, transparent, transparent 70%, #369 70%, #369)
}

.ol-overlaycontainer .ol-touch-cursor .ol-button {
  position: absolute;
  color: #369;
  height: 55%;
  width: 55%;
  border-radius: 50%;
  cursor: pointer;
  background: rgba(255,255,255,.4);
  -webkit-box-shadow: inset 0 0 0 3px currentColor;
          box-shadow: inset 0 0 0 3px currentColor;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%) scale(0);
          transform: translate(-50%, -50%) scale(0);
  -webkit-transition: all .5s, opacity 0s, background 0s;
  transition: all .5s, opacity 0s, background 0s;
  overflow: hidden;
}
.ol-overlaycontainer .ol-touch-cursor.active.disable .ol-button {
  opacity: .8;
  background: rgba(51, 102, 153, .2);
}
.ol-overlaycontainer .ol-touch-cursor.active .ol-button {
  -webkit-transform: translate(-50%, -50%) scale(1);
          transform: translate(-50%, -50%) scale(1);
}
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-0 {
  top: -18%;
  left: 118%;
}
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-1 {
  top: 50%;
  left: 140%;
}
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-2 {
  top: 120%;
  left: 120%;
}
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-3 {
  top: 140%;
  left: 50%;
}
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-4 {
  top: 118%;
  left: -18%;
}
/* extra buttons */
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-5 {
  top: 50%;
  left: -40%;
}
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-6 {
  top: -18%;
  left: -18%;
}
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-7 {
  top: -40%;
  left: 50%;
}

.ol-overlaycontainer .ol-touch-cursor .ol-button:before {
  content: "";
  width: 1.5em;
  height: 1em;
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  line-height: 1em;
  text-align: center;
}
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-add:before,
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-remove:before {
  content: "\u2212";
  line-height: .95em;
  font-size: 1.375em;
  font-weight: bold;
}
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-add:before {
  content: "+";
}
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-x:before {
  content: "\\00D7";
  font-size: 1.2em;
  font-weight: bold;
}
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-move:before {
  content: "\\2725";
  font-size: 1.2em;
}
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-check:before {
  content: "\\2713";
  font-weight: bold;
}

.ol-overlaycontainer .ol-touch-cursor.nodrawing .ol-button.ol-button-x,
.ol-overlaycontainer .ol-touch-cursor.nodrawing .ol-button.ol-button-remove,
.ol-overlaycontainer .ol-touch-cursor.nodrawing .ol-button.ol-button-check {
  opacity: .8;
  background: rgba(51, 102, 153, .2);
}

.ol-overlaycontainer .ol-touch-cursor .ol-button > div {
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}

.ol-overlaycontainer .ol-touch-cursor .ol-button-type:before {
  content: "\\21CE";
  font-weight: bold;
}



/* remove outline on focus */
.mapboxgl-canvas:focus {
  outline: none;
}
.ol-perspective-map {
  position: absolute;
  left: 50%;
  top: 50%;
  width: 200%;
  height: 200%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}
.ol-perspective-map .ol-layer {
  z-index: -1!important; /* bug using Chrome ? */
}
.ol-perspective-map .ol-layers {
  -webkit-transform: translateY(0) perspective(200px) rotateX(0deg) scaleY(1);
          transform: translateY(0) perspective(200px) rotateX(0deg) scaleY(1);
}

.ol-perspective-map .ol-overlaycontainer,
.ol-perspective-map .ol-overlaycontainer-stopevent {
  width: 50%!important;
  height: 50%!important;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}

.ol-overlay-container .ol-magnify 
{	background: rgba(0,0,0, 0.5);
	border:3px solid #369;
	border-radius: 50%;
	height: 150px;
	width: 150px;
	overflow: hidden;
	-webkit-box-shadow: 5px 5px 5px rgba(0, 0, 0, 0.5);
	        box-shadow: 5px 5px 5px rgba(0, 0, 0, 0.5);
	position:relative;
	z-index:0;
}

.ol-overlay-container .ol-magnify:before 
{	border-radius: 50%;
	-webkit-box-shadow: 0 0 40px 2px rgba(0, 0, 0, 0.25) inset;
	        box-shadow: 0 0 40px 2px rgba(0, 0, 0, 0.25) inset;
	content: "";
	display: block;
	height: 100%;
	left: 0;
	position: absolute;
	top: 0;
	width: 100%;
	z-index: 1;
}

.ol-overlay-container .ol-magnify:after 
{
	border-radius: 50%;
	-webkit-box-shadow: 0 0 20px 7px rgba(255, 255, 255, 1);
	        box-shadow: 0 0 20px 7px rgba(255, 255, 255, 1);
	content: "";
	display: block;
	height: 0;
	left: 23%;
	position: absolute;
	top: 20%;
	width: 20%;
	z-index: 1;
	transform: rotate(-40deg);
	-webkit-transform: rotate(-40deg);
}
/** popup animation using visible class
*/
.ol-popup.anim {
  visibility: hidden;
}

.ol-popup.anim.visible {
  visibility: visible;
}
/** No transform when visible 
*/
.ol-popup.anim.visible > div {
  visibility: visible;
  -webkit-transform: none;
          transform: none;
  -webkit-animation: ol-popup_bounce 0.4s ease 1;
          animation: ol-popup_bounce 0.4s ease 1;
}

@-webkit-keyframes ol-popup_bounce {
  from { -webkit-transform: scale(0); transform: scale(0); }
  50%  { -webkit-transform: scale(1.1); transform: scale(1.1) }
  80%  { -webkit-transform: scale(0.95); transform: scale(0.95) }
  to   { -webkit-transform: scale(1); transform: scale(1); }
}

@keyframes ol-popup_bounce {
  from { -webkit-transform: scale(0); transform: scale(0); }
  50%  { -webkit-transform: scale(1.1); transform: scale(1.1) }
  80%  { -webkit-transform: scale(0.95); transform: scale(0.95) }
  to   { -webkit-transform: scale(1); transform: scale(1); }
}

/** Transform Origin */
.ol-popup.anim.ol-popup-bottom.ol-popup-left > div  {
  -webkit-transform-origin:0 100%;
          transform-origin:0 100%;
}
.ol-popup.anim.ol-popup-bottom.ol-popup-right > div {
  -webkit-transform-origin:100% 100%;
          transform-origin:100% 100%;
}
.ol-popup.anim.ol-popup-bottom.ol-popup-center > div {
  -webkit-transform-origin:50% 100%;
          transform-origin:50% 100%;
}
.ol-popup.anim.ol-popup-top.ol-popup-left > div {
  -webkit-transform-origin:0 0;
          transform-origin:0 0;
}
.ol-popup.anim.ol-popup-top.ol-popup-right > div {
  -webkit-transform-origin:100% 0;
          transform-origin:100% 0;
}
.ol-popup.anim.ol-popup-top.ol-popup-center > div {
  -webkit-transform-origin:50% 0;
          transform-origin:50% 0;
}
.ol-popup.anim.ol-popup-middle.ol-popup-left > div {
  -webkit-transform-origin:0 50%;
          transform-origin:0 50%;
}
.ol-popup.anim.ol-popup-middle.ol-popup-right > div {
  -webkit-transform-origin:100% 50%;
          transform-origin:100% 50%;
}

.ol-overlaycontainer-stopevent {
  /* BOUG ol6.1 to enable DragOverlay interaction 
  position: initial!important;
  */
}

/** ol.popup */
.ol-popup {
  font-size:0.9em;
  -webkit-user-select: none;  
  -moz-user-select: none;    
  -ms-user-select: none;      
  user-select: none;
}
.ol-popup .ol-popup-content {
  overflow:hidden;
  cursor: default;
  padding: 0.25em 0.5em;
}
.ol-popup.hasclosebox .ol-popup-content {
  margin-right: 1.7em;
}
.ol-popup .ol-popup-content:after {
  clear: both;
  content: "";
  display: block;
  font-size: 0;
  height: 0;
}

/** Anchor position */
.ol-popup .anchor {
  display: block;
  width: 0px;
  height: 0px;
  background:red;
  position: absolute;
  margin: -11px 22px;
  pointer-events: none;
}
.ol-popup .anchor:after,
.ol-popup .anchor:before {
  position:absolute;
}
.ol-popup-right .anchor:after,
.ol-popup-right .anchor:before {
  right:0;
}
.ol-popup-top .anchor { top:0; }
.ol-popup-bottom .anchor { bottom:0; }
.ol-popup-right .anchor { right:0; }
.ol-popup-left .anchor { left:0; }
.ol-popup-center .anchor { 
  left:50%; 
  margin-left: 0!important;
}
.ol-popup-middle .anchor { 
  top:50%; 
  margin-top: 0!important;
}
.ol-popup-center.ol-popup-middle .anchor { 
  display:none; 
}

/** Fixed popup */
.ol-popup.ol-fixed {
  margin: 0!important;
  top: .5em!important;
  right: .5em!important;
  left: auto!important;
  bottom: auto!important;
  -webkit-transform: none!important;
          transform: none!important;
}
.ol-popup.ol-fixed .anchor {
  display: none;
}
.ol-popup.ol-fixed.anim > div {
  -webkit-animation: none;
          animation: none;
}

.ol-popup .ol-fix {
  width: 1em;
  height: .9em;
  background: #fff;
  position: relative;
  float: right;
  margin: .2em;
  cursor: pointer;
}
.ol-popup .ol-fix:before {
  content: "";
  width: .8em;
  height: .7em;
  display: block;
  border: .1em solid #666;
      border-right-width: 0.1em;
  border-right-width: .3em;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  margin: .1em;
}

/** Add a shadow to the popup */
.ol-popup.shadow {
  -webkit-box-shadow: 2px 2px 2px 2px rgba(0,0,0,0.5);
          box-shadow: 2px 2px 2px 2px rgba(0,0,0,0.5);
}

/** Close box */
.ol-popup .closeBox {
  background-color: rgba(0, 60, 136, 0.5);
  color: #fff;
  border: 0;
  border-radius: 2px;
  cursor: pointer;
  float: right;
  font-size: 0.9em;
  font-weight: 700;
  width: 1.4em;
  height: 1.4em;
  margin: 5px 5px 0 0;
  padding: 0;
  position: relative;
  display:none;
}
.ol-popup.hasclosebox .closeBox {
  display:block;
}

.ol-popup .closeBox:hover {
  background-color: rgba(0, 60, 136, 0.7);
}
/* the X */
.ol-popup .closeBox:after {
  content: "\\00d7";
  font-size:1.5em;
  top: 50%;
  left: 0;
  right: 0;
  width: 100%;
  text-align: center;
  line-height: 1em;
  margin: -0.5em 0;
  position: absolute;
}

/** Modify touch poup */
.ol-popup.modifytouch {
  background-color: #eee;
}
.ol-popup.modifytouch .ol-popup-content {	
  padding: 0 0.25em;
  font-size: 0.85em;
  white-space: nowrap;
}
.ol-popup.modifytouch .ol-popup-content a {
  text-decoration: none;
}

/** Tool tips popup*/
.ol-popup.tooltips {
  background-color: #ffa;
}
.ol-popup.tooltips .ol-popup-content{
  padding: 0 0.25em;
  font-size: 0.85em;
  white-space: nowrap;
}

/** Default popup */
.ol-popup.default > div {
  background-color: #fff;
  border:1px solid #69f;
  border-radius: 5px;
}
.ol-popup.default {
  margin: -11px 0;
  -webkit-transform: translate(0, -22px);
          transform: translate(0, -22px);
}
.ol-popup-top.ol-popup.default {
  margin: 11px 0;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-left.default {
  margin: -11px -22px;
  -webkit-transform: translate(0, -22px);
          transform: translate(0, -22px);
}
.ol-popup-top.ol-popup-left.default {
  margin: 11px -22px;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-right.default {
  margin: -11px 22px;
  -webkit-transform: translate(44px, -22px);
          transform: translate(44px, -22px);
}
.ol-popup-top.ol-popup-right.default {
  margin: 11px 22px;
  -webkit-transform: translate(44px, 0);
          transform: translate(44px, 0);
}
.ol-popup-middle.default {
  margin:0 10px;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-middle.ol-popup-right.default {
  margin:0 -10px;
  -webkit-transform: translate(-20px, 0);
          transform: translate(-20px, 0);
}

.ol-popup.default .anchor {
  color: #69f;
}
.ol-popup.default .anchor:after,
.ol-popup.default .anchor:before {
  content:"";
  border-color: currentColor transparent;
  border-style: solid;
  border-width: 11px;
  margin: 0 -11px;
}
.ol-popup.default .anchor:after {
  border-color: #fff transparent;
  border-width: 11px;
  margin: 2px -11px;
}

.ol-popup-top.default .anchor:before,
.ol-popup-top.default .anchor:after {
  border-top:0;
  top:0;
}

.ol-popup-bottom.default .anchor:before,
.ol-popup-bottom.default .anchor:after {
  border-bottom:0;
  bottom:0;
}

.ol-popup-middle.default .anchor:before {
  margin: -11px -33px;
  border-color: transparent currentColor;
}
.ol-popup-middle.default .anchor:after {
  margin: -11px -31px;
  border-color: transparent #fff;
}
.ol-popup-middle.ol-popup-left.default .anchor:before,
.ol-popup-middle.ol-popup-left.default .anchor:after {
  border-left:0;
}
.ol-popup-middle.ol-popup-right.default .anchor:before,
.ol-popup-middle.ol-popup-right.default .anchor:after {
  border-right:0;
}

/** Placemark popup */
.ol-popup.placemark {
  color: #c00;
  margin: -.65em 0;
  -webkit-transform: translate(0, -1.3em);
          transform: translate(0, -1.3em);
}
.ol-popup.placemark > div {
  position: relative;
  font-size: 15px;	
  background-color: #fff;
  border: 0;
  -webkit-box-shadow: inset 0 0 0 0.45em;
          box-shadow: inset 0 0 0 0.45em;
  width: 2em;
  height: 2em;
  border-radius: 50%;
  min-width: unset;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}
.ol-popup.placemark .ol-popup-content {
  overflow: hidden;
  cursor: default;
  text-align: center;
  padding: .25em 0;
  width: 1em;
  height: 1em;
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  line-height: 1em;
}
.ol-popup.placemark .anchor {
  margin: 0;
}

.ol-popup.placemark .anchor:before {
  content: "";
  margin: -.5em -.5em;
  background: transparent;
  width: 1em;
  height: .5em;
  border-radius: 50%;
  -webkit-box-shadow: 0 1em 0.5em rgba(0,0,0,.5);
          box-shadow: 0 1em 0.5em rgba(0,0,0,.5);
}
.ol-popup.placemark .anchor:after {
  content: "";
  border-color: currentColor transparent;
  border-style: solid;
  border-width: 1em .7em 0;
  margin: -.75em -.7em;
  bottom:0;
}

/** Placemark Shield */
.ol-popup.placemark.shield > div {
  border-radius: .2em;
}

.ol-popup.placemark.shield .anchor:after {
    border-width: .8em 1em 0;
    margin: -.7em -1em;
}

/** Placemark Blazon */
.ol-popup.placemark.blazon > div {
  border-radius: .2em;
}

/** Placemark Needle/Pushpin */
.ol-popup.placemark.pushpin {	
  margin: -2.2em 0;
  -webkit-transform: translate(0, -4em);
          transform: translate(0, -4em);
}
.ol-popup.placemark.pushpin > div {	
  border-radius: 0;
  background: transparent!important;
  -webkit-box-shadow: inset 2em 0 currentColor;
          box-shadow: inset 2em 0 currentColor;
  width: 1.1em;
}
.ol-popup.placemark.pushpin > div:before {
  content: "";
  width: 1.3em;
  height: 1.5em;
  border-style: solid;
  position: absolute;
  left: 50%;
  top: 50%;
  -webkit-transform: translate(-50%,-50%);
          transform: translate(-50%,-50%);
  border-color: currentColor transparent;
  border-width: .3em .5em .5em;
  pointer-events: none;
}

.ol-popup.placemark.needle {	
  margin: -2em 0;
  -webkit-transform: translate(0, -4em);
          transform: translate(0, -4em);
}
.ol-popup.placemark.pushpin .anchor,
.ol-popup.placemark.needle .anchor {
  margin: -1.2em;
}
.ol-popup.placemark.pushpin .anchor:after,
.ol-popup.placemark.needle .anchor:after {
  border-style: solid;
    border-width: 2em .15em 0;
    margin: -.55em -0.2em;
    width: .1em;
}
.ol-popup.placemark.pushpin .anchor:before,
.ol-popup.placemark.needle .anchor:before {
    margin: -.75em -.5em;
}

/** Placemark Flag */
.ol-popup.placemark.flagv {
  margin: -2em 1em;
  -webkit-transform: translate(0, -4em);
          transform: translate(0, -4em);
}
.ol-popup.placemark.flagv > div {
  border-radius: 0;
  -webkit-box-shadow: none;
          box-shadow: none;
  background-color: transparent;
}
.ol-popup.placemark.flagv > div:before {
  content: "";
  border: 1em solid transparent;
  position: absolute;
  border-left: 2em solid currentColor;
  pointer-events: none;
}
.ol-popup.placemark.flagv .anchor {
  margin: -1.4em;
}

.ol-popup.placemark.flag {	
  margin: -2em 1em;
  -webkit-transform: translate(0, -4em);
          transform: translate(0, -4em);
}
.ol-popup.placemark.flag > div {	
  border-radius: 0;
  -webkit-transform-origin: 0% 150%!important;
          transform-origin: 0% 150%!important;
}
.ol-popup.placemark.flag .anchor {
  margin: -1.4em;
}
.ol-popup.placemark.flagv .anchor:after, 
.ol-popup.placemark.flag .anchor:after {
  border-style: solid;
  border-width: 2em .15em 0;
  margin: -.55em -1em;
  width: .1em;
}
.ol-popup.placemark.flagv .anchor:before,
.ol-popup.placemark.flag .anchor:before {
  margin: -.75em -1.25em;
}

.ol-popup.placemark.flag.finish {
  margin: -2em 1em;
}
.ol-popup.placemark.flag.finish > div {
  background-image: 
    linear-gradient(45deg, currentColor 25%, transparent 25%, transparent 75%, currentColor 75%, currentColor), 
    linear-gradient(45deg, currentColor 25%, transparent 25%, transparent 75%, currentColor 75%, currentColor);
  background-size: 1em 1em;
  background-position: .5em 0, 0 .5em;
  -webkit-box-shadow: inset 0 0 0 .25em;
          box-shadow: inset 0 0 0 .25em;
}

/** Black popup */
.ol-popup.black .closeBox {
  background-color: rgba(0,0,0, 0.5);
  border-radius: 5px;
  color: #f80;
}
.ol-popup.black .closeBox:hover {
  background-color: rgba(0,0,0, 0.7);
  color:#da2;
}

.ol-popup.black {
  margin: -20px 0;
  -webkit-transform: translate(0, -40px);
          transform: translate(0, -40px);
}
.ol-popup.black > div{
  background-color: rgba(0,0,0,0.6);
  border-radius: 5px;
  color:#fff;
}
.ol-popup-top.ol-popup.black {
  margin: 20px 0;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-left.black {
  margin: -20px -22px;
  -webkit-transform: translate(0, -40px);
          transform: translate(0, -40px);
}
.ol-popup-top.ol-popup-left.black {
  margin: 20px -22px;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-right.black {
  margin: -20px 22px;
  -webkit-transform: translate(44px, -40px);
          transform: translate(44px, -40px);
}
.ol-popup-top.ol-popup-right.black {
  margin: 20px 22px;
  -webkit-transform: translate(44px, 0);
          transform: translate(44px, 0);
}
.ol-popup-middle.black {
  margin: 0 11px;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-left.ol-popup-middle.black {
  -webkit-transform: none;
          transform: none;
}
.ol-popup-right.ol-popup-middle.black {
  margin:0 -11px;
  -webkit-transform: translate(-22px, 0);
          transform: translate(-22px, 0);
}

.ol-popup.black .anchor {
  margin: -20px 11px;
  color: rgba(0,0,0,0.6);
} 
.ol-popup.black .anchor:before {
  content:"";
  border-color: currentColor transparent;
  border-style: solid;
  border-width: 20px 11px;
}

.ol-popup-top.black .anchor:before {
  border-top:0;
  top:0;
}

.ol-popup-bottom.black .anchor:before {
  border-bottom:0;
  bottom:0;
}

.ol-popup-middle.black .anchor:before {
  margin: -20px -22px;
  border-color: transparent currentColor;
}
.ol-popup-middle.ol-popup-left.black .anchor:before {
  border-left:0;
}
.ol-popup-middle.ol-popup-right.black .anchor:before {
  border-right:0;
}

.ol-popup-center.black .anchor:before {
  margin: 0 -10px;
}


/** Green tips popup */
.ol-popup.tips .closeBox {
  background-color: #f00;
  border-radius: 50%;
  color: #fff;
  width:1.2em;
  height:1.2em;
}
.ol-popup.tips .closeBox:hover {
  background-color: #f40;
}

.ol-popup.tips {
  margin: -20px 0;
  -webkit-transform: translate(0,-40px);
          transform: translate(0,-40px);
}
.ol-popup.tips > div {
  background-color: #cea;
  border: 5px solid #ad7;
  border-radius: 5px;
  color:#333;
}
.ol-popup-top.ol-popup.tips {
  margin: 20px 0;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-left.tips {
  margin: -20px -22px;
  -webkit-transform: translate(0,-40px);
          transform: translate(0,-40px);
}
.ol-popup-top.ol-popup-left.tips {
  margin: 20px -22px;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-right.tips {
  margin: -20px 22px;
  -webkit-transform: translate(44px,-40px);
          transform: translate(44px,-40px);
}
.ol-popup-top.ol-popup-right.tips {
  margin: 20px 22px;
  -webkit-transform: translate(44px,0);
          transform: translate(44px,0);
}
.ol-popup-middle.tips {
  margin:0;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-left.ol-popup-middle.tips {
  margin: 0 22px;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-right.ol-popup-middle.tips {
  margin: 0 -22px;
  -webkit-transform: translate(-44px,0);
          transform: translate(-44px,0);
}

.ol-popup.tips .anchor {
  margin: -18px 22px;
  color: #ad7;
} 
.ol-popup.tips .anchor:before {
  content:"";
  border-color: currentColor transparent;
  border-style: solid;
  border-width: 20px 11px;
}

.ol-popup-top.tips .anchor:before {
  border-top:0;
  top:0;
}
.ol-popup-bottom.tips .anchor:before {
  border-bottom:0;
  bottom:0;
}
.ol-popup-center.tips .anchor:before {
  border-width: 20px 6px;
  margin: 0 -6px;
}
.ol-popup-left.tips .anchor:before {
  border-left:0;
  margin-left:0;
}
.ol-popup-right.tips .anchor:before {
  border-right:0;
  margin-right:0;
}

.ol-popup-middle.tips .anchor:before {
  margin: -6px -41px;
  border-color: transparent currentColor;
  border-width:6px 20px;
}
.ol-popup-middle.ol-popup-left.tips .anchor:before {
  border-left:0;
}
.ol-popup-middle.ol-popup-right.tips .anchor:before {
  border-right:0;
}

/** Warning popup */
.ol-popup.warning .closeBox {
  background-color: #f00;
  border-radius: 50%;
  color: #fff;
  font-size: 0.83em;
}
.ol-popup.warning .closeBox:hover {
  background-color: #f40;
}

.ol-popup.warning {
  background-color: #fd0;
  border-radius: 3px;
  border:4px dashed #f00;
  margin:20px 0;
  color:#900;
  margin: -28px 10px;
  -webkit-transform: translate(0, -56px);
          transform: translate(0, -56px);
}
.ol-popup-top.ol-popup.warning {
  margin: 28px 10px;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-left.warning {
  margin: -28px -22px;
  -webkit-transform: translate(0, -56px);
          transform: translate(0, -56px);
}
.ol-popup-top.ol-popup-left.warning {
  margin: 28px -22px;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-right.warning {
  margin: -28px 22px;
  -webkit-transform: translate(44px, -56px);
          transform: translate(44px, -56px);
}
.ol-popup-top.ol-popup-right.warning {
  margin: 28px 22px;
  -webkit-transform: translate(44px, 0);
          transform: translate(44px, 0);
}
.ol-popup-middle.warning {
  margin:0;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-left.ol-popup-middle.warning {
  margin:0 22px;
  -webkit-transform: none;
          transform: none;
}
.ol-popup-right.ol-popup-middle.warning {
  margin:0 -22px;
  -webkit-transform: translate(-44px, 0);
          transform: translate(-44px, 0);
}

.ol-popup.warning .anchor {
  margin: -33px 7px;
} 
.ol-popup.warning .anchor:before {
  content:"";
  border-color: #f00 transparent;
  border-style: solid;
  border-width: 30px 11px;
}

.ol-popup-top.warning .anchor:before {
  border-top:0;
  top:0;
}
.ol-popup-bottom.warning .anchor:before {
  border-bottom:0;
  bottom:0;
}

.ol-popup-center.warning .anchor:before {
  margin: 0 -21px;
}
.ol-popup-middle.warning .anchor:before {
  margin: -10px -33px;
  border-color: transparent #f00;
  border-width:10px 22px;
}
.ol-popup-middle.ol-popup-left.warning .anchor:before {
  border-left:0;
}
.ol-popup-middle.ol-popup-right.warning .anchor:before {
  border-right:0;
}

.ol-popup .ol-popupfeature table {
  width: 100%;
}
.ol-popup .ol-popupfeature table td {
  max-width: 25em;
  overflow: hidden;
  text-overflow: ellipsis;
}
.ol-popup .ol-popupfeature table td img {
  max-width: 100px;
  max-height: 100px;
}
.ol-popup .ol-popupfeature tr:nth-child(2n+1) {
  background-color: #eee;
}
.ol-popup .ol-popupfeature .ol-zoombt {
  border: 0;
  width: 2em;
  height: 2em;
  display: inline-block;
  color: rgba(0,60,136,.5);
  position: relative;
  background: transparent;
  outline: none;
}
.ol-popup .ol-popupfeature .ol-zoombt:before {
  content: "";
  position: absolute;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  width: 1em;
  height: 1em;
  background-color: transparent;
  border: .17em solid currentColor;
  border-radius: 100%;
  top: .3em;
  left: .3em;
}
.ol-popup .ol-popupfeature .ol-zoombt:after {
  content: "";
  position: absolute;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  top: 1.35em;
  left: 1.15em;
  border-width: .1em .3em;
  border-style: solid;
  border-radius: .03em;
  -webkit-transform: rotate(45deg);
          transform: rotate(45deg);
  -webkit-box-shadow: -0.2em 0 0 -0.04em;
          box-shadow: -0.2em 0 0 -0.04em;
}

.ol-popup .ol-popupfeature .ol-count{
  float: right;
  margin: .25em 0;
}
.ol-popup .ol-popupfeature .ol-prev,
.ol-popup .ol-popupfeature .ol-next {
  border-style: solid;
  border-color: transparent rgba(0,60,136,.5);
  border-width: .5em 0 .5em .5em;
  display: inline-block;
  vertical-align: bottom;
  margin: 0 .5em;
  cursor: pointer;
}
.ol-popup .ol-popupfeature .ol-prev{
  border-width: .5em .5em .5em 0;
}

.ol-popup.tooltips.black {
  background-color: transparent;
}
.ol-popup.tooltips.black > div {
  -webkit-transform: scaleY(1.3);
          transform: scaleY(1.3);
  padding: .2em .5em;
  background-color: rgba(0,0,0, 0.5);
}
.ol-popup-middle.tooltips.black .anchor:before {
  border-width: 5px 10px;
  margin: -5px -21px;
}

.ol-popup-center.ol-popup-middle { 
  margin: 0;
}

.ol-popup-top.ol-popup-left.ol-fixPopup,
.ol-popup-top.ol-popup-right.ol-fixPopup,
.ol-popup.ol-fixPopup {
  margin: 0;
}

.ol-miniscroll {
  position: relative;
}
.ol-miniscroll:hover .ol-scroll {
  opacity: .5;
  -webkit-transition: opacity 1s;
  transition: opacity 1s;
}
.ol-miniscroll .ol-scroll {
  -ms-touch-action: none;
      touch-action: none;
  position: absolute;
  right: 0px;
  width: 9px;
  height: auto;
  max-height: 100%;
  opacity: 0;
  border-radius: 9px;
  -webkit-transition: opacity 1s .5s;
  transition: opacity 1s .5s;
  overflow: hidden;
  z-index: 1;
}
.ol-miniscroll .ol-scroll > div {
  -ms-touch-action: none;
      touch-action: none;
  position: absolute;
  top: 0;
  right: 0px;
  width: 9px;
  height: 9px;
  -webkit-box-shadow: inset 10px 0 currentColor;
          box-shadow: inset 10px 0 currentColor;
  border-radius: 9px / 12px;
  border: 2px solid transparent;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  cursor: pointer;
}
.ol-miniscroll .ol-scroll.ol-100pc {
  opacity: 0;
}

.ol-viewport canvas.ol-fixedoverlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}
/* Toggle Switch */
.ol-ext-toggle-switch {
  cursor: pointer;
  position: relative;
}
.ol-ext-toggle-switch input[type="radio"],
.ol-ext-toggle-switch input[type="checkbox"] {
  display: none;
}
.ol-ext-toggle-switch span {
  color: rgba(0,60,136,.5);
  position: relative;
  cursor: pointer;
  background-color: #ccc;
  -webkit-transition: .4s, background-color 0s, border-color 0s;
  transition: .4s, background-color 0s, border-color 0s;
  width: 1.6em;
  height: 1em;
  display: inline-block;
  border-radius: 1em;
  font-size: 1.3em;
  vertical-align: middle;
  margin: -.15em .2em .15em;
}
.ol-ext-toggle-switch span:before {
  position: absolute;
  content: "";
  height: 1em;
  width: 1em;
  left: 0;
  top: 50%;
  background-color: #fff;
  -webkit-transition: .4s;
  transition: .4s;
  border-radius: 1em;
  display: block;
  -webkit-transform: translateY(-50%);
          transform: translateY(-50%);
  border: 2px solid #ccc;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}
.ol-ext-toggle-switch:hover span {
  background-color: #999;
}
.ol-ext-toggle-switch:hover span:before {
  border-color: #999;
}

.ol-ext-toggle-switch input:checked + span {
  background-color: currentColor;
}
.ol-ext-toggle-switch input:checked + span:before {
  -webkit-transform: translate(.6em,-50%);
          transform: translate(.6em,-50%);
  border-color: currentColor;
}

/* Check/radio buttons */
.ol-ext-check {
  position: relative;
  display: inline-block;
}
.ol-ext-check input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
  height: 0;
  width: 0;
}
.ol-ext-check span {
  color: rgba(0,60,136,.5);
  position: relative;
  display: inline-block;
  width: 1em;
  height: 1em;
  margin: -.1em .5em .1em;
  background-color: #ccc;
  vertical-align: middle;
}
.ol-ext-check:hover span {
  background-color: #999;
}
.ol-ext-checkbox input:checked ~ span {
  background-color: currentColor;
}
.ol-ext-checkbox input:checked ~ span:before {
  content: "";
  position: absolute;
  width: .5em;
  height: .8em;
  top: 50%;
  left: 50%;
  -webkit-transform: translateY(-.1em) translate(-50%, -50%) rotate(45deg);
          transform: translateY(-.1em) translate(-50%, -50%) rotate(45deg);
  -webkit-box-shadow: inset -0.2em -0.2em #fff;
          box-shadow: inset -0.2em -0.2em #fff;
}

.ol-ext-radio span {
  width: 1.1em;
  height: 1.1em;
  border-radius: 50%;
}
.ol-ext-radio:hover input:checked ~ span {
  background-color: #ccc;
}
.ol-ext-radio input:checked ~ span:before {
  content: "";
  position: absolute;
  width: 50%;
  height: 50%;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  border-radius: 50%;
  background-color: currentColor;
}

.ol-collection-list {
  margin: 0;
  padding: 0;
  list-style: none;
}
.ol-collection-list li {
  position: relative;
  padding: 0 2em 0 1em;
}
.ol-collection-list li:hover {
  background-color: rgba(0,60,136,.2);
}
.ol-collection-list li.ol-select {
  background-color: rgba(0,60,136,.5);
  color: #fff;
}

.ol-collection-list li .ol-order {
  position: absolute;
  -ms-touch-action: none;
      touch-action: none;
  right: 0;
  top: 50%;
  -webkit-transform: translateY(-50%);
          transform: translateY(-50%);
  width: 2em;
  height: 100%;
  cursor: n-resize;
}
.ol-collection-list li .ol-order:before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  width: 18px;
  height: 2px;
  background-color: currentColor;
  -webkit-box-shadow: 0 5px, 0 -5px;
          box-shadow: 0 5px, 0 -5px;
  border-radius: 2px;
}

.ol-ext-colorpicker.ol-popup {
  width: 2em;
  height: 1.5em;
  background-color: transparent;
  background-image: 
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%), 
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%);
  background-size: 10px 10px;
  background-position: 0 -1px, 5px 4px;
}

.ol-ext-colorpicker .ol-tabbar {
  background-color: #eee;
  border-bottom: 1px solid #999;
  display: none;
}
.ol-ext-colorpicker.ol-tab .ol-tabbar {
  display: block;
}

.ol-ext-colorpicker .ol-tabbar > div {
  display: inline-block;
  background-color: #fff;
  padding: 0 .5em;
  border: 1px solid #999;
  border-radius: 2px 2px 0 0;
  position: relative;
  top: 1px;
  cursor: pointer;
}
.ol-ext-colorpicker .ol-tabbar > div:nth-child(1) {
  border-bottom-color: #fff;
}
.ol-ext-colorpicker.ol-picker-tab .ol-tabbar > div:nth-child(1) {
  border-bottom-color: #999;
}
.ol-ext-colorpicker.ol-picker-tab .ol-tabbar > div:nth-child(2) {
  border-bottom-color: #fff;
}

.ol-ext-colorpicker.ol-popup.ol-tab .ol-popup {
  width: 180px;
}
.ol-ext-colorpicker.ol-tab .ol-palette {
  margin: 0 10px;
}
.ol-ext-colorpicker.ol-tab .ol-container {
  display: none;
}
.ol-ext-colorpicker.ol-tab.ol-picker-tab .ol-container {
  display: block;
}
.ol-ext-colorpicker.ol-tab.ol-picker-tab .ol-palette {
  display: none;
}

.ol-ext-colorpicker.ol-popup .ol-popup {
  width: 340px;
}

.ol-ext-colorpicker.ol-popup .ol-vignet {
  content: "";
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  border: 0;
  background-color: currentColor;
  pointer-events: none;
}

.ol-ext-colorpicker .ol-container {
  position: relative;
  display: inline-block;
  vertical-align: top;
}
.ol-ext-colorpicker .ol-cursor {
  pointer-events: none;
}

.ol-ext-colorpicker .ol-picker {
  position: relative;
  cursor: crosshair;
  width: 150px;
  height: 150px;
  border: 5px solid #fff;
  background-color: currentColor;
  background-image: -webkit-gradient(linear, left top, left bottom, from(0), color-stop(#000), to(transparent)),
    -webkit-gradient(linear, left top, right top, from(#fff), to(transparent));
  background-image: linear-gradient(0, #000, transparent),
    linear-gradient(90deg, #fff, transparent);
}
.ol-ext-colorpicker .ol-picker .ol-cursor {
  position: absolute;
  left: 50%;
  top: 50%;
  border: 1px solid rgba(0,0,0,.7);
  -webkit-box-shadow: 0 0 0 1px rgba(255,255,255,.7);
          box-shadow: 0 0 0 1px rgba(255,255,255,.7);
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  width: 10px;
  height: 10px;
  border-radius: 50%;
}

.ol-ext-colorpicker .ol-slider {
  position: relative;
  cursor: crosshair;
  background-color: #fff;
  height: 10px;
  width: 150px;
  margin: 5px 0 10px;
  border: 5px solid #fff;
  border-width: 0 5px;
  background-image: 
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%), 
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%);
  background-size: 10px 10px;
  background-position: 0 -1px, 5px 4px;
}
.ol-ext-colorpicker .ol-slider > div {
  width: 100%;
  height: 100%;
  background-image: linear-gradient(45deg, transparent, #fff);
  pointer-events: none;
}
.ol-ext-colorpicker .ol-slider .ol-cursor {
  position: absolute;
  width: 4px;
  height: 12px;
  border: 1px solid #000;
  top: 50%;
  left: 0;
  background: transparent;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}
.ol-ext-colorpicker .ol-tint {
  position: absolute;
  cursor: crosshair;
  width: 10px;
  height: 150px;
  border: 5px solid #fff;
  border-width: 5px 0;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  top: 0;
  right: 5px;
  background-image: -webkit-gradient(linear, left top, left bottom, from(0), color-stop(#f00), color-stop(#f0f), color-stop(#00f), color-stop(#0ff), color-stop(#0f0), color-stop(#ff0), to(#f00));
  background-image: linear-gradient(0, #f00, #f0f, #00f, #0ff, #0f0, #ff0, #f00)
}
.ol-ext-colorpicker .ol-tint .ol-cursor {
  position: absolute;
  top: 0;
  left: 50%;
  border: 1px solid #000;
  height: 4px;
  width: 12px;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}

.ol-ext-colorpicker .ol-clear {
  position: absolute;
  border: 2px solid #999;
  right: 4px;
  top: 163px;
  width: 10px;
  height: 10px;
}
.ol-ext-colorpicker .ol-clear:before,
.ol-ext-colorpicker .ol-clear:after {
  content: "";
  position: absolute;
  width: 15px;
  height: 2px;
  background-color: #999;
  top: 50%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%) rotate(45deg);
          transform: translate(-50%, -50%) rotate(45deg);
}
.ol-ext-colorpicker .ol-clear:after {
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);
          transform: translate(-50%, -50%) rotate(-45deg);
}

.ol-ext-colorpicker.ol-nopacity .ol-slider,
.ol-ext-colorpicker.ol-nopacity .ol-clear {
  display: none;
}
.ol-ext-colorpicker.ol-nopacity .ol-alpha {
  display: none;
}

.ol-ext-colorpicker .ol-rgb {
  position: relative;
  padding: 5px;
  width: 170px;
  display: none;
}
.ol-ext-colorpicker .ol-rgb input {
  width: 25%;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  padding: 0 0 0 2px;
  border: 1px solid #999;
  border-radius: 2px;
  font-size: 13px;
}
.ol-ext-colorpicker .ol-rgb input:nth-child(1) {
	background-color: rgba(255,0,0,.1);
}
.ol-ext-colorpicker .ol-rgb input:nth-child(2) {
	background-color: rgba(0,255,0,.1);
}
.ol-ext-colorpicker .ol-rgb input:nth-child(3) {
	background-color: rgba(0,0,255,.12);
}

.ol-ext-colorpicker button,
.ol-ext-colorpicker .ol-txt-color {
  font-size: 13px;
  margin: 0 5px 5px;
  text-align: center;
  width: 170px;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
  padding: 0;
  border: 1px solid #999;
  border-radius: 2px;
  display: block;
}
.ol-ext-colorpicker button {
  background-color: #eee;
}
.ol-ext-colorpicker button:hover {
  background-color: #e9e9e9;
}

.ol-ext-colorpicker .ol-txt-color.ol-error {
  background-color: rgba(255,0,0,.2);
}

.ol-ext-colorpicker .ol-palette {
  padding: 2px;
  display: inline-block;
  width: 152px;
}
.ol-ext-colorpicker .ol-palette > div {
  width: 15px;
  height: 15px;
  display: inline-block;
  background-image: 
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%), 
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%);
  background-size: 10px 10px;
  background-position: 0 0, 5px 5px;
  margin: 2px;
  -webkit-box-shadow: 0 0 2px 0px #666;
          box-shadow: 0 0 2px 0px #666;
  border-radius: 1px;
  cursor: pointer;
  position: relative;
}
.ol-ext-colorpicker .ol-palette > div:before {
  content: "";
  position: absolute;
  background-color: currentColor;
  width: 100%;
  height: 100%;
}
.ol-ext-colorpicker .ol-palette > div.ol-select:after {
  content: "";
  position: absolute;
  width: 6px;
  height: 12px;
  -webkit-box-shadow: 1px 1px #fff, 2px 2px #000;
          box-shadow: 1px 1px #fff, 2px 2px #000;
  top: 30%;
  left: 50%;
  -webkit-transform: translate(-50%, -50%) rotate(45deg);
          transform: translate(-50%, -50%) rotate(45deg);
}
.ol-ext-colorpicker .ol-palette > div:hover {
  -webkit-box-shadow: 0 0 2px 1px #d90;
          box-shadow: 0 0 2px 1px #d90;
}
.ol-ext-colorpicker .ol-palette hr {
  margin: 0;
}

.ol-input-popup {
  display: inline-block;
  position: relative;
}
.ol-input-popup .ol-popup {
  position: absolute;
  -webkit-box-shadow: 1px 1px 3px 1px #999;
          box-shadow: 1px 1px 3px 1px #999;
  background-color: #fff;
  z-index: 1;
  display: none;
  left: -5px;
  padding: 0;
  margin: 0;
  list-style: none;
  white-space: nowrap;
}
.ol-input-popup.ol-hover:hover .ol-popup,
.ol-input-popup.ol-focus .ol-popup {
  display: block;
}
.ol-input-popup.ol-right .ol-popup {
  left: auto;
  right: -5px;
}
.ol-input-popup.ol-middle .ol-popup {
  top: 50%;
  -webkit-transform: translateY(-50%);
          transform: translateY(-50%);
}


.ol-input-popup .ol-popup li {
  position: relative;
  padding: 10px 5px;
}

.ol-input-popup li:hover {
  background-color: #ccc;
}
.ol-input-popup li.ol-selected {
  background-color: #ccc;
}

.ol-input-popup.ol-fixed:hover .ol-popup,
.ol-input-popup.ol-fixed .ol-popup {
  position: relative;
  left: 0;
  -webkit-box-shadow: unset;
          box-shadow: unset;
  background-color: transparent;
  display: inline-block;
  vertical-align: middle;
}
.ol-input-popup.ol-fixed.ol-left .ol-popup {
  float: left;
}

.ol-input-popup > div {
  position: relative;
  display: inline-block;
  vertical-align: middle;
  border-radius: 2px;
  border: 1px solid #999;
  padding: 3px 20px 3px 10px
}
.ol-input-popup > div:before {
  position: absolute;
  content: "";
  right: 5px;
  top: 50%;
  border: 5px solid transparent;
  border-top: 5px solid #999;
}

.ol-ext-popup-input {
  display: inline-block;
  vertical-align: top;
}
.ol-ext-popup-input.ol-popup {
  position: relative;
  width: 2em;
  height: 1.5em;
  display: inline-block;
  border: 3px solid #fff;
  border-right-width: 1em;
  -webkit-box-shadow: 0 0 2px 1px #666;
          box-shadow: 0 0 2px 1px #666;
  border-radius: 2px;
  -webkit-box-sizing: content-box;
          box-sizing: content-box;
  -webkit-user-select: none;
     -moz-user-select: none;
      -ms-user-select: none;
          user-select: none;
  vertical-align: middle;
}
.ol-ext-popup-input.ol-popup:after {
  content: "";
  position: absolute;
  border: .5em solid #aaa;
  border-width: .5em .3em 0;
  border-color: #999 transparent;
  right: -.8em;
  top: 50%;
  -webkit-transform: translateY(-50%);
          transform: translateY(-50%);
  pointer-events: none;
}

.ol-ext-popup-input * {
  -webkit-box-sizing: content-box;
          box-sizing: content-box;
}

.ol-ext-popup-input.ol-popup .ol-popup {
  position: absolute;
  top: 100%;
  min-width: 3em;
  min-height: 3em;
  left: 0;
  -webkit-box-shadow: 1px 1px 3px 1px #999;
          box-shadow: 1px 1px 3px 1px #999;
  display: block;
  background-color: #fff;
  display: none;
  z-index: 1;
}
.ol-ext-popup-input.ol-popup .ol-popup.ol-visible {
  display: block;
}

.ol-ext-popup-input.ol-popup-fixed .ol-popup {
  position: fixed;
  top: auto;
  left: auto;
}

.ol-input-popup.ol-size li {
  display: table-cell;
  height: 100%;
  padding: 5px;
  vertical-align: middle;
}

.ol-input-popup.ol-size li > * {
  background-color: #369;
  border-radius: 50%;
  vertical-align: middle;
  width: 1em;
  height: 1em;
}

.ol-input-popup.ol-size li > .ol-option-0 {
  position: relative;
  width: 1em;
  height: 1em;
  border: 2px solid currentColor;
  color: #aaa;
  background-color: transparent;
  -webkit-box-sizing: border-box;
          box-sizing: border-box;
}
.ol-input-popup.ol-size li > *:before {
  position: absolute;
  left: 50%;
  top: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
}
.ol-input-popup.ol-size li > .ol-option-0:before {
  content: "";
  width: 1em;
  height: 2px;
  background-color: #aaa;
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);
          transform: translate(-50%, -50%) rotate(-45deg);
}

.ol-input-slider {
  display: inline-block;
  position: relative;
}
.ol-input-slider .ol-popup {
  position: absolute;
  -webkit-box-shadow: 1px 1px 3px 1px #999;
          box-shadow: 1px 1px 3px 1px #999;
  background-color: #fff;
  z-index: 1;
  display: none;
  left: -5px;
}
.ol-input-slider.ol-right .ol-popup {
  left: auto;
  right: -5px;
}
.ol-input-slider.ol-hover:hover .ol-popup,
.ol-input-slider.ol-focus .ol-popup {
  display: block;
  white-space: nowrap;
}
.ol-input-slider.ol-hover:hover .ol-popup > *,
.ol-input-slider.ol-focus .ol-popup > * {
  display: inline-block;
  vertical-align: middle;
}
.ol-input-slider.ol-hover:hover .ol-popup > .ol-before,
.ol-input-slider.ol-focus .ol-popup > .ol-before {
  margin-left: 10px;
}
.ol-input-slider.ol-hover:hover .ol-popup > .ol-after,
.ol-input-slider.ol-focus .ol-popup > .ol-after {
  margin-right: 10px;
}
.ol-input-slider .ol-slider {
  display: inline-block;
  vertical-align: middle;
  position: relative;
  width: 100px;
  height: 3px;
  border: 0 solid transparent;
  border-width: 10px 15px;
  -webkit-box-shadow: inset 0 0 0 1px #999;
          box-shadow: inset 0 0 0 1px #999;
  -webkit-box-sizing: content-box;
          box-sizing: content-box;
  cursor: pointer;
}

.ol-input-slider .ol-slider > .ol-cursor {
  position: absolute;
  width: 5px;
  height: 10px;
  top: 50%;
  -webkit-transform: translate(-50%, -50%);
          transform: translate(-50%, -50%);
  background-color: #999;
  pointer-events: none;
}
.ol-input-range .ol-slider {
  cursor: default;
  width: 150px;
}
.ol-input-range .ol-slider > .ol-cursor {
  pointer-events: all;
  cursor: pointer;
  width: 10px;
  border-radius: 50%;
  background-color: rgb(0,60,136);
}
.ol-input-range.ol-moving .ol-slider > .ol-cursor {
  pointer-events: none;
}
.ol-input-range .ol-slider > .ol-back {
  position: absolute;
  top: 50%;
  -webkit-transform: translateY(-50%);
          transform: translateY(-50%);
  left: 30%;
  right: 20%;
  height: 100%;
  background-color: rgb(0,60,136);
  pointer-events: none;
}

.ol-input-slider.ol-fixed:hover .ol-popup,
.ol-input-slider.ol-fixed .ol-popup {
  position: relative;
  left: 0;
  -webkit-box-shadow: unset;
          box-shadow: unset;
  background-color: transparent;
  display: inline-block;
  vertical-align: middle;
}
.ol-input-slider.ol-fixed.ol-left .ol-popup {
  float: left;
}

/* Grow */
.ol-input-slider.ol-size .ol-slider {
  height: auto;
  -webkit-box-shadow: none;
          box-shadow: none;
}
.ol-input-slider.ol-size .ol-slider .ol-back {
  width: 0;
  color: #aaa;
  border: 0 solid transparent;
  border-width: 0 0 20px 100px;
  border-color: currentColor transparent;
  pointer-events: none;
}

.ol-input-slider.ol-size .ol-slider > .ol-cursor {
  width: 2px;
  height: calc(100% + 4px);
  border-width: 5px 3px;
  border-style: solid;
  border-color: #f00 transparent;
  -o-border-image: initial;
     border-image: initial;
  background-color: transparent;
  -webkit-box-shadow: inset 3px 0px #f00;
          box-shadow: inset 3px 0px #f00;
}

.ol-input-popup.ol-width li {
  padding: 5px;
}


.ol-input-popup.ol-width li > * {
  background-color: #369;
  width: 100px;
  height: 1em;
}

.ol-input-popup.ol-width li > .ol-option-0 {
  position: relative;
  height: 1px;
  background-image: linear-gradient(90deg,#aaa 2px, transparent 2px);
  background-color: transparent;
  background-size: 4px;
}
`,"",{version:3,sources:["webpack://./node_modules/ol-ext/dist/ol-ext.css"],names:[],mappings:"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aAAa;EACb,SAAS;EACT,WAAW;EACX,YAAY;EACZ,UAAU;EACV,QAAQ;EACR,SAAS;EACT,wCAAwC;UAChC,gCAAgC;AAC1C;AACA;;EAEE,aAAa;AACf;AACA;EACE,SAAS;EACT,SAAS;EACT,sCAAsC;AACxC;AACA;EACE,sCAAsC;AACxC;AACA;EACE,WAAW;AACb;AACA;EACE,YAAY;EACZ,eAAe;EACf,mBAAmB;EACnB,kBAAkB;EAClB,+BAA+B;UACvB,uBAAuB;EAC/B,UAAU;EACV,WAAW;EACX,6BAA6B;EAC7B,+BAA+B;EAC/B,mBAAmB;EACnB,SAAS;EACT,UAAU;EACV,gBAAgB;EAChB,kBAAkB;AACpB;AACA;EACE,WAAW;EACX,kBAAkB;EAClB,WAAW;EACX,SAAS;EACT,YAAY;EACZ,aAAa;EACb,8BAA8B;EAC9B,oBAAoB;EACpB,gCAAgC;UACxB,wBAAwB;EAChC,uCAAuC;UAC/B,+BAA+B;AACzC;AACA;EACE,WAAW;EACX,eAAe;EACf,YAAY;EACZ,mBAAmB;EACnB,aAAa;EACb,kBAAkB;AACpB;AACA;IACI,YAAY;EACd,WAAW;EACX,sBAAsB;EACtB,qBAAqB;EACrB,kBAAkB;EAClB,eAAe;AACjB;AACA;EACE,eAAe;EACf,kBAAkB;EAClB,QAAQ;EACR,SAAS;EACT,wCAAwC;UAChC,gCAAgC;EACxC,WAAW;EACX,kBAAkB;EAClB,iBAAiB;AACnB;AACA;EACE,cAAc;AAChB;AACA;EACE,cAAc;EACd,eAAe;AACjB;AACA;EACE,WAAW;EACX,kBAAkB;AACpB;AACA;EACE,cAAc;EACd,aAAa;AACf;AACA;EACE,aAAa;AACf;;AAEA;EACE,eAAe;AACjB;AACA;EACE,iBAAiB;AACnB;AACA;EACE,gBAAgB;AAClB;;AAEA;EACE,gBAAgB;EAChB,SAAS;EACT,UAAU;AACZ;AACA;;EAEE,UAAU;AACZ;;AAEA;EACE,eAAe;AACjB;AACA;EACE,kBAAkB;EAClB,cAAc;EACd,gBAAgB;EAChB,sBAAsB;EACtB,eAAe;EACf,gBAAgB;AAClB;AACA;EACE,eAAe;A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9B,kBAAkB;AACpB;AACA;EACE,eAAe;EACf,eAAe;EACf,WAAW;EACX,8BAA8B;UACtB,sBAAsB;EAC9B,uBAAuB;EACvB,gBAAgB;AAClB;AACA;EACE,iBAAiB;AACnB;AACA;EACE,mBAAmB;AACrB;AACA;EACE,iBAAiB;AACnB;AACA;EACE,mBAAmB;AACrB;AACA;EACE,iBAAiB;AACnB;;AAEA;EACE,kBAAkB;AACpB;AACA;EACE,mCAAmC;AACrC;AACA;EACE,mCAAmC;EACnC,WAAW;AACb;AACA;EACE,mCAAmC;EACnC,WAAW;AACb;;AAEA;EACE,YAAY;EACZ,YAAY;EACZ,6BAA6B;EAC7B,WAAW;EACX,kBAAkB;EAClB,kBAAkB;EAClB,iBAAiB;AACnB;AACA;EACE,YAAY;EACZ,YAAY;EACZ,6BAA6B;EAC7B,WAAW;EACX,kBAAkB;EAClB,kBAAkB;EAClB,iBAAiB;AACnB;AACA;EACE,WAAW;EACX,cAAc;EACd,gBAAgB;AAClB;AACA;EACE,eAAe;EACf,aAAa;AACf;AACA;EACE,cAAc;AAChB;AACA;EACE,WAAW;EACX,iBAAiB;AACnB;AACA;EACE,SAAS;AACX;AACA;EACE,iBAAiB;EACjB,kBAAkB;AACpB;AACA;EACE,WAAW;AACb;;AAEA;EACE,aAAa;EACb,gBAAgB;EAChB,UAAU;AACZ;AACA;EACE,cAAc;AAChB;AACA;EACE,qBAAqB;EACrB,iBAAiB;EACjB,uBAAuB;EACvB,kBAAkB;AACpB;AACA;EACE,qBAAqB;EACrB,UAAU;EACV,8BAA8B;UACtB,sBAAsB;AAChC;AACA;EACE,WAAW;AACb;AACA;EACE,YAAY;EACZ,aAAa;AACf;;AAEA;EACE,sBAAsB;AACxB;AACA;EACE,kBAAkB;EAClB,UAAU;EACV,aAAa;EACb,SAAS;EACT,sBAAsB;EACtB,WAAW;AACb;AACA;;EAEE,WAAW;EACX,kBAAkB;EAClB,YAAY;EACZ,YAAY;EACZ,+BAA+B;EAC/B,QAAQ;EACR,SAAS;EACT,sDAAsD;UAC9C,8CAA8C;AACxD;AACA;EACE,sEAAsE;UAC9D,8DAA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AEA,iBAAiB;AACjB;EACE,kCAAkC;EAClC,kBAAkB;EAClB,WAAW;AACb;AACA;EACE,kCAAkC;EAClC,UAAU;AACZ;;AAEA;EACE,eAAe;EACf,sCAAsC;UAC9B,8BAA8B;AACxC;AACA;EACE,iCAAiC;EACjC,kBAAkB;EAClB,UAAU;AACZ;AACA;EACE,cAAc;EACd,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,mBAAmB;EACnB,sCAAsC;UAC9B,8BAA8B;AACxC;AACA;EACE,kBAAkB;EAClB,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,kBAAkB;EAClB,yCAAyC;UACjC,iCAAiC;AAC3C;AACA;EACE,iBAAiB;EACjB,qCAAqC;UAC7B,6BAA6B;AACvC;AACA;EACE,cAAc;EACd,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,cAAc;EACd,sCAAsC;UAC9B,8BAA8B;AACxC;;AAEA;EACE,kBAAkB;EAClB,sBAAsB;AACxB;AACA;EACE,UAAU;EACV,sCAAsC;EACtC,mBAAmB;EACnB,uBAAuB;AACzB;;AAEA;EACE,YAAY;EACZ,KAAK;AACP;;AAEA;EACE,eAAe;EACf,QAAQ;AACV;;AAEA;EACE,mBAAmB;EACnB,sCAAsC;AACxC;AACA;EACE,aAAa;AACf;AACA;EACE,cAAc;AAChB;;AAEA;EACE,eAAe;AACjB;;;AAGA,sBAAsB;AACtB;EACE,sBAAsB;EACtB,kBAAkB;EAClB,WAAW;EACX,WAAW;EACX,YAAY;AACd;AACA;EACE,sBAAsB;AACxB;;AAEA;EACE,eAAe;EACf,qCAAqC;UAC7B,6BAA6B;AACvC;AACA;EACE,sBAAsB;EACtB,sBAAsB;EACtB,kBAAkB;EAClB,UAAU;AACZ;AACA;EACE,cAAc;EACd,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,mBAAmB;EACnB,qCAAqC;UAC7B,6BAA6B;AACvC;AACA;EACE,kBAAkB;EAClB,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,kBAAkB;EAClB,wCAAwC;UAChC,gCAAgC;AAC1C;AACA;EACE,iBAAiB;EACjB,oCAAoC;UAC5B,4BAA4B;AACtC;AACA;EACE,QAAQ;EACR,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,cAAc;EACd,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,eAAe;EACf,qCAAqC;UAC7B,6BAA6B;AACvC;;AAEA;EACE,kBAAkB;EAClB,WAAW;AACb;AACA;EACE,UAAU;EACV,sCAAsC;EACtC,mBAAmB;EACnB,uBAAuB;AACzB;;AAEA;EACE,YAAY;EACZ,KAAK;AACP;AACA;EACE,eAAe;EACf,QAAQ;AACV;AACA;EACE,sBAAsB;EACtB,cAAc;AAChB;AACA;EACE,aAAa;EACb,aAAa;AACf;AACA;EACE,cAAc;EACd,cAAc;AAChB;;AAEA;EACE,kBAAkB;EAClB,sCAAsC;EACtC,qBAAqB;AACvB;AACA;EACE,aAAa;AACf;AACA;EACE,cAAc;AAChB;;AAEA,mBAAmB;AACnB;EACE,sBAAsB;EACtB,kBAAkB;EAClB,WAAW;EACX,iBAAiB;AACnB;AACA;EACE,sBAAsB;AACxB;;AAEA;EACE,sBAAsB;EACtB,kBAAkB;EAClB,sBAAsB;EACtB,aAAa;EACb,UAAU;EACV,kBAAkB;EAClB,sCAAsC;UAC9B,8BAA8B;AACxC;AACA;EACE,iBAAiB;EACjB,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,mBAAmB;EACnB,sCAAsC;UAC9B,8BAA8B;AACxC;AACA;EACE,kBAAkB;EAClB,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,kBAAkB;EAClB,yCAAyC;UACjC,iCAAiC;AAC3C;AACA;EACE,iBAAiB;EACjB,qCAAqC;UAC7B,6BAA6B;AACvC;AACA;EACE,QAAQ;EACR,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,aAAa;EACb,uBAAuB;UACf,eAAe;AACzB;AACA;EACE,cAAc;EACd,sCAAsC;UAC9B,8BAA8B;AACxC;;AAEA;EACE,iBAAiB;AACnB;AACA;EACE,UAAU;EACV,8BAA8B;EAC9B,mBAAmB;EACnB,uBAAuB;AACzB;;AAEA;EACE,YAAY;EACZ,KA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",sourcesContent:[`.ol-control i {\r
  cursor: default;\r
}\r
\r
/* Bar style */\r
.ol-control.ol-bar {\r
  left: 50%;\r
  min-height: 1em;\r
  min-width: 1em;\r
  position: absolute;\r
  top: 0.5em;\r
  transform: translate(-50%,0);\r
  -webkit-transform: translate(-50%,0);\r
  white-space: nowrap;\r
}\r
\r
/* Hide subbar when not inserted in a parent bar */\r
.ol-control.ol-toggle .ol-option-bar {\r
  display: none;\r
}\r
\r
/* Default position for controls */\r
.ol-control.ol-bar .ol-bar {\r
  position: static;\r
}\r
.ol-control.ol-bar .ol-control {\r
  position: relative;\r
  top: auto;\r
  left:auto;\r
  right:auto;\r
  bottom: auto;\r
  display: inline-block;\r
  vertical-align: middle;\r
  background-color: transparent;\r
  padding: 0;\r
  margin: 0;\r
  transform: none;\r
  -webkit-transform: none;\r
}\r
.ol-control.ol-bar .ol-bar {\r
  position: static;\r
}\r
.ol-control.ol-bar .ol-control button {\r
  margin:2px 1px;\r
  outline: none;\r
}\r
\r
/* Positionning */\r
.ol-control.ol-bar.ol-left {\r
  left: 0.5em;\r
  top: 50%;\r
  -webkit-transform: translate(0px, -50%);\r
          transform: translate(0px, -50%);\r
}\r
.ol-control.ol-bar.ol-left .ol-control {\r
  display: block;\r
}\r
\r
.ol-control.ol-bar.ol-right {\r
  left: auto;\r
  right: 0.5em;\r
  top: 50%;\r
  -webkit-transform: translate(0px, -50%);\r
          transform: translate(0px, -50%);\r
}\r
.ol-control.ol-bar.ol-right .ol-control {\r
  display: block;\r
}\r
\r
.ol-control.ol-bar.ol-bottom {\r
  top: auto;\r
  bottom: 0.5em;\r
}\r
\r
.ol-control.ol-bar.ol-top.ol-left,\r
.ol-control.ol-bar.ol-top.ol-right {\r
  top: 4.5em;\r
  -webkit-transform:none;\r
          transform:none;\r
}\r
.ol-touch .ol-control.ol-bar.ol-top.ol-left,\r
.ol-touch .ol-control.ol-bar.ol-top.ol-right {\r
  top: 5.5em;\r
}\r
.ol-control.ol-bar.ol-bottom.ol-left,\r
.ol-control.ol-bar.ol-bottom.ol-right {\r
  top: auto;\r
  bottom: 0.5em;\r
  -webkit-transform:none;\r
          transform:none;\r
}\r
\r
/* Group buttons */\r
.ol-control.ol-bar.ol-group {\r
  margin: 1px 1px 1px 0;\r
}\r
.ol-control.ol-bar.ol-right .ol-group,\r
.ol-control.ol-bar.ol-left .ol-group {\r
  margin: 1px 1px 0 1px;\r
}\r
\r
.ol-control.ol-bar.ol-group button {\r
  border-radius:0;\r
  margin: 0 0 0 1px;\r
}\r
.ol-control.ol-bar.ol-right.ol-group button,\r
.ol-control.ol-bar.ol-left.ol-group button,\r
.ol-control.ol-bar.ol-right .ol-group button,\r
.ol-control.ol-bar.ol-left .ol-group button {\r
  margin: 0 0 1px 0;\r
}\r
.ol-control.ol-bar.ol-group .ol-control:first-child > button {\r
  border-radius: 5px 0 0 5px;\r
}\r
.ol-control.ol-bar.ol-group .ol-control:last-child > button {\r
  border-radius: 0 5px 5px 0;\r
}\r
.ol-control.ol-bar.ol-left.ol-group .ol-control:first-child > button,\r
.ol-control.ol-bar.ol-right.ol-group .ol-control:first-child > button,\r
.ol-control.ol-bar.ol-left .ol-group .ol-control:first-child > button,\r
.ol-control.ol-bar.ol-right .ol-group .ol-control:first-child > button {\r
  border-radius: 5px 5px 0 0;\r
}\r
.ol-control.ol-bar.ol-left.ol-group .ol-control:last-child > button,\r
.ol-control.ol-bar.ol-right.ol-group .ol-control:last-child > button,\r
.ol-control.ol-bar.ol-left .ol-group .ol-control:last-child > button,\r
.ol-control.ol-bar.ol-right .ol-group .ol-control:last-child > button {\r
  border-radius: 0 0 5px 5px;\r
}\r
\r
/* */\r
.ol-control.ol-bar .ol-rotate {\r
  opacity:1;\r
  visibility: visible;\r
}\r
.ol-control.ol-bar .ol-rotate button {\r
  display: block\r
}\r
\r
/* Active buttons */\r
.ol-control.ol-bar .ol-toggle.ol-active > button,\r
.ol-control.ol-bar .ol-toggle.ol-active button:hover {\r
  background-color: #00AAFF;\r
  color: #fff;\r
}\r
.ol-control.ol-toggle button:disabled {\r
  background-color: #ccc;\r
}\r
\r
/* Subbar toolbar */\r
.ol-control.ol-bar .ol-control.ol-option-bar {\r
  display: none;\r
  position:absolute;\r
  top:100%;\r
  left:0;\r
  margin: 5px 0;\r
  border-radius: 0;\r
  background-color: rgba(255,255,255, 0.8);\r
  /* border: 1px solid rgba(0, 60, 136, 0.5); */\r
  -webkit-box-shadow: 0 0 0 1px rgba(0, 60, 136, 0.5), 1px 1px 2px rgba(0, 0, 0, 0.5);\r
          box-shadow: 0 0 0 1px rgba(0, 60, 136, 0.5), 1px 1px 2px rgba(0, 0, 0, 0.5);\r
}\r
\r
.ol-control.ol-bar .ol-option-bar:before {\r
  content: "";\r
  border: 0.5em solid transparent;\r
  border-color: transparent transparent rgba(0, 60, 136, 0.5);\r
  position: absolute;\r
  bottom: 100%;\r
  left: 0.3em;\r
  pointer-events: none;\r
}\r
\r
.ol-control.ol-bar .ol-option-bar .ol-control {\r
  display: table-cell;\r
}\r
.ol-control.ol-bar .ol-control .ol-bar\r
{	display: none;\r
}\r
.ol-control.ol-bar .ol-control.ol-active > .ol-option-bar {\r
  display: block;\r
}\r
\r
.ol-control.ol-bar .ol-control.ol-collapsed ul {\r
  display: none;\r
}\r
\r
.ol-control.ol-bar .ol-control.ol-text-button > div:hover,\r
.ol-control.ol-bar .ol-control.ol-text-button > div {\r
  background-color: transparent;\r
  color: rgba(0, 60, 136, 0.5);\r
  width: auto;\r
  min-width: 1.375em;\r
  margin: 0;\r
}\r
\r
.ol-control.ol-bar .ol-control.ol-text-button {\r
  font-size:0.9em;\r
  border-left: 1px solid rgba(0, 60, 136, 0.8);\r
  border-radius: 0;\r
}\r
.ol-control.ol-bar .ol-control.ol-text-button:first-child {\r
  border-left:0;\r
}\r
.ol-control.ol-bar .ol-control.ol-text-button > div {\r
  padding: .11em 0.3em;\r
  font-weight: normal;\r
  font-size: 1.14em;\r
  font-family: Arial,Helvetica,sans-serif;\r
}\r
.ol-control.ol-bar .ol-control.ol-text-button div:hover {\r
  color: rgba(0, 60, 136, 1);\r
}\r
\r
.ol-control.ol-bar.ol-bottom .ol-option-bar {\r
  top: auto;\r
  bottom: 100%;\r
}\r
.ol-control.ol-bar.ol-bottom .ol-option-bar:before {\r
  border-color: rgba(0, 60, 136, 0.5) transparent transparent ;\r
  bottom: auto;\r
  top: 100%;\r
}\r
\r
.ol-control.ol-bar.ol-left .ol-option-bar {\r
  left:100%;\r
  top: 0;\r
  bottom: auto;\r
  margin: 0 5px;\r
}\r
.ol-control.ol-bar.ol-left .ol-option-bar:before {\r
  border-color: transparent rgba(0, 60, 136, 0.5) transparent transparent;\r
  bottom: auto;\r
  right: 100%;\r
  left: auto;\r
  top: 0.3em;\r
}\r
.ol-control.ol-bar.ol-right .ol-option-bar {\r
  right:100%;\r
  left:auto;\r
  top: 0;\r
  bottom: auto;\r
  margin: 0 5px;\r
}\r
.ol-control.ol-bar.ol-right .ol-option-bar:before {\r
  border-color: transparent transparent transparent rgba(0, 60, 136, 0.5);\r
  bottom: auto;\r
  left: 100%;\r
  top: 0.3em;\r
}\r
\r
.ol-control.ol-bar.ol-left .ol-option-bar .ol-option-bar,\r
.ol-control.ol-bar.ol-right .ol-option-bar .ol-option-bar {\r
  top: 100%;\r
  bottom: auto;\r
  left: 0.3em;\r
  right: auto;\r
  margin: 5px 0;\r
}\r
.ol-control.ol-bar.ol-right .ol-option-bar .ol-option-bar {\r
  right: 0.3em;\r
  left: auto;\r
}\r
.ol-control.ol-bar.ol-left .ol-option-bar .ol-option-bar:before,\r
.ol-control.ol-bar.ol-right .ol-option-bar .ol-option-bar:before {\r
  border-color: transparent transparent rgba(0, 60, 136, 0.5);\r
  bottom: 100%;\r
  top: auto;\r
  left: 0.3em;\r
  right: auto;\r
}\r
.ol-control.ol-bar.ol-right .ol-option-bar .ol-option-bar:before {\r
  right: 0.3em;\r
  left: auto;\r
}\r
\r
.ol-control-title {\r
  position: absolute;\r
  top: 0;\r
  left: 0;\r
  right: 0;\r
}\r
\r
.ol-center-position {\r
  position: absolute;\r
  bottom: 0;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
  background-color: rgba(255,255,255,.8);\r
  padding: .1em 1em;\r
}\r
\r
.ol-compassctrl {\r
  display: none;\r
  top: 1em;\r
  left: auto;\r
  right: 1em;\r
}\r
.ol-compassctrl.ol-visible {\r
  display: block!important;\r
}\r
.ol-ext-dialog {\r
  position: fixed;\r
  top: -100%;\r
  left: 0;\r
  width: 150%;\r
  height: 100%;\r
  opacity: 0;\r
  background-color: rgba(0,0,0,.5);\r
  z-index: 1000;\r
  pointer-events: none!important;\r
  -webkit-transition: opacity .2s, top 0s .2s;\r
  transition: opacity .2s, top 0s .2s;\r
}\r
.ol-ext-dialog.ol-visible {\r
  opacity: 1;\r
  top: 0;\r
  pointer-events: all!important;\r
  -webkit-transition: opacity .2s, top 0s;\r
  transition: opacity .2s, top 0s;\r
}\r
\r
.ol-viewport .ol-ext-dialog {\r
  position: absolute;\r
}\r
.ol-ext-dialog > form > h2 {\r
  margin: 0 .5em .5em 0;\r
  display: none;\r
  overflow: hidden;\r
  text-overflow: ellipsis;\r
  white-space: nowrap;\r
}\r
.ol-ext-dialog > form.ol-title > h2 {\r
  display: block;\r
}\r
.ol-ext-dialog > form {\r
  position: absolute;\r
  top: 0;\r
  left: 33.33%;\r
  min-width: 5em;\r
  max-width: 60%;\r
  min-height: 3em;\r
  max-height: 100%;\r
  background-color: #fff;\r
  border: 1px solid #333;\r
  -webkit-box-shadow: 3px 3px 4px rgba(0,0,0, 0.5);\r
          box-shadow: 3px 3px 4px rgba(0,0,0, 0.5);\r
  -webkit-transform: translate(-50%, -30%);\r
          transform: translate(-50%, -30%);\r
  -webkit-transition: top .2s, -webkit-transform .2s;\r
  transition: top .2s, -webkit-transform .2s;\r
  transition: top .2s, transform .2s;\r
  transition: top .2s, transform .2s, -webkit-transform .2s;\r
  padding: 1em;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  overflow-x: hidden;\r
  overflow-y: auto;\r
}\r
.ol-ext-dialog > form.ol-closebox {\r
  padding-top: 1.5em;\r
}\r
.ol-ext-dialog > form.ol-title {\r
  padding-top: 1em;\r
}\r
.ol-ext-dialog > form.ol-button {\r
  padding-bottom: .5em;\r
}\r
\r
.ol-ext-dialog.ol-zoom > form {\r
  top: 30%;\r
  -webkit-transform: translate(-50%, -30%) scale(0);\r
          transform: translate(-50%, -30%) scale(0);\r
}\r
.ol-ext-dialog.ol-visible > form {\r
  top: 30%;\r
}\r
.ol-ext-dialog.ol-zoom.ol-visible > form {\r
  -webkit-transform: translate(-50%, -30%) scale(1);\r
          transform: translate(-50%, -30%) scale(1);\r
}\r
\r
.ol-ext-dialog > form .ol-content {\r
  overflow-x: hidden;\r
  overflow-y: auto;\r
}\r
\r
.ol-ext-dialog > form .ol-closebox {\r
  position: absolute;\r
  top: .5em;\r
  right: .5em;\r
  width: 1em;\r
  height: 1em;\r
  cursor: pointer;\r
  display: none;\r
}\r
.ol-ext-dialog > form.ol-closebox .ol-closebox {\r
  display: block;\r
}\r
.ol-ext-dialog > form .ol-closebox:before,\r
.ol-ext-dialog > form .ol-closebox:after {\r
  content: "";\r
  position: absolute;\r
  background-color: currentColor;\r
  top: 50%;\r
  left: 50%;\r
  width: 1em;\r
  height: .1em;\r
  border-radius: .1em;\r
  -webkit-transform: translate(-50%, -50%) rotate(45deg);\r
          transform: translate(-50%, -50%) rotate(45deg);\r
}\r
.ol-ext-dialog > form .ol-closebox:before {\r
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);\r
          transform: translate(-50%, -50%) rotate(-45deg);\r
}\r
\r
.ol-ext-dialog > form .ol-buttons {\r
  text-align: right;\r
  overflow-x: hidden;\r
}\r
.ol-ext-dialog > form .ol-buttons input {\r
  margin-top: .5em;\r
  padding: .5em;\r
  background: none;\r
  border: 0;\r
  font-size: 1em;\r
  color: rgba(0,60,136,1);\r
  cursor: pointer;\r
  border-radius: .25em;\r
  outline-width: 0;\r
}\r
.ol-ext-dialog > form .ol-buttons input:hover {\r
  background-color:  rgba(0,60,136,.1);\r
}\r
.ol-ext-dialog > form .ol-buttons input[type=submit] {\r
  font-weight: bold;\r
}\r
\r
.ol-ext-dialog .ol-progress-message {\r
  font-size: .9em;\r
  text-align: center;\r
  padding-bottom: .5em;\r
}\r
.ol-ext-dialog .ol-progress-bar {\r
  border: 1px solid #369;\r
  width: 20em;\r
  height: 1em;\r
  max-width: 100%;\r
  padding: 2px;\r
  margin: .5em auto 0;\r
  overflow: hidden;\r
}\r
.ol-ext-dialog .ol-progress-bar > div {\r
  background: #369;\r
  height: 100%;\r
  width: 50%;\r
  -webkit-transition: width .3s;\r
  transition: width .3s;\r
}\r
.ol-ext-dialog .ol-progress-bar > div.notransition {\r
  -webkit-transition: unset;\r
  transition: unset;\r
}\r
\r
/* full screen */\r
.ol-ext-dialog.ol-fullscreen-dialog form {\r
  top: 1em;\r
  -webkit-transform: none;\r
          transform: none;\r
  left: 1em;\r
  bottom: 1em;\r
  right: 1em;\r
  max-width: calc(66.6% - 2em);\r
  text-align: center;\r
  background: transparent;\r
  -webkit-box-shadow: none;\r
          box-shadow: none;\r
  border: none;\r
  color: #fff;\r
}\r
.ol-ext-dialog.ol-fullscreen-dialog form .ol-closebox {\r
  top: 0;\r
  right: 0;\r
  font-size: 2em;\r
}\r
.ol-ext-dialog.ol-fullscreen-dialog .ol-closebox:before,\r
.ol-ext-dialog.ol-fullscreen-dialog .ol-closebox:after {\r
  border: .1em solid currentColor;\r
}\r
.ol-ext-dialog.ol-fullscreen-dialog img,\r
.ol-ext-dialog.ol-fullscreen-dialog video {\r
  max-width: 100%;\r
}\r
\r
/* Fullscreen dialog */\r
body > .ol-ext-dialog .ol-content {\r
  max-height: calc(100vh - 10em);\r
}\r
\r
body > .ol-ext-dialog > form {\r
  overflow: visible;\r
}\r
.ol-editbar .ol-button button {\r
  position: relative;\r
  display: inline-block;\r
  font-style: normal;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  vertical-align: middle;\r
}\r
.ol-editbar .ol-button button:before, \r
.ol-editbar .ol-button button:after {\r
  content: "";\r
  border-width: 0;\r
  position: absolute;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  background-color: currentColor;\r
}\r
.ol-editbar .ol-button button:focus {\r
  outline: none;\r
}\r
\r
.ol-editbar .ol-selection > button:before {\r
  width: .6em;\r
  height: 1em;\r
  background-color: transparent;\r
  border: .5em solid currentColor;\r
  border-width: 0 .25em .65em;\r
  border-color: currentColor transparent;\r
  -webkit-box-shadow:0 0.6em 0 -0.23em;\r
          box-shadow:0 0.6em 0 -0.23em;\r
  top: .35em;\r
  left: .5em;\r
  -webkit-transform: translate(-50%, -50%) rotate(-30deg);\r
          transform: translate(-50%, -50%) rotate(-30deg);\r
}\r
.ol-editbar .ol-selection0 > button:after {\r
  width: .28em;\r
  height: .6em;\r
  background-color: transparent;\r
  border: .5em solid currentColor;\r
  border-width: 0 .05em .7em;\r
  border-color: currentColor transparent;\r
  top: .5em;\r
  left: .7em;\r
  -webkit-transform: rotate(-45deg);\r
          transform: rotate(-45deg);\r
}\r
\r
.ol-editbar .ol-delete button:after,\r
.ol-editbar .ol-delete button:before {\r
  width: 1em;\r
  height: .2em;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%) rotate(45deg);\r
          transform: translate(-50%, -50%) rotate(45deg);\r
}\r
.ol-editbar .ol-delete button:after {\r
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);\r
          transform: translate(-50%, -50%) rotate(-45deg);\r
}\r
\r
.ol-editbar .ol-info button:before {\r
  width: .25em;\r
  height: .6em;\r
  border-radius: .03em;\r
  top: .47em;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
}\r
.ol-editbar .ol-info button:after {\r
  width: .25em;\r
  height: .2em;\r
  border-radius: .03em;\r
  -webkit-box-shadow: -0.1em 0.35em, -0.1em 0.82em, 0.1em 0.82em;\r
          box-shadow: -0.1em 0.35em, -0.1em 0.82em, 0.1em 0.82em;\r
  top: .12em;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
}\r
\r
.ol-editbar .ol-drawpoint button:before {\r
  width: .7em;\r
  height: .7em;\r
  border-radius: 50%;\r
  border: .15em solid currentColor;\r
  background-color: transparent;\r
  top: .2em;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
}\r
.ol-editbar .ol-drawpoint button:after {\r
  width: .4em;\r
  height: .4em;\r
  border: .15em solid currentColor;\r
  border-color: currentColor transparent;\r
  border-width: .4em .2em 0;\r
  background-color: transparent;\r
  top: .8em;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
}\r
\r
.ol-editbar .ol-drawline > button:before,\r
.ol-editbar .ol-drawpolygon > button:before,\r
.ol-editbar .ol-drawhole > button:before {\r
  width: .8em;\r
  height: .8em;\r
  border: .13em solid currentColor;\r
  background-color: transparent;\r
  border-width: .2em .13em .09em;\r
  top: .2em;\r
  left: .25em;\r
  -webkit-transform: rotate(10deg) perspective(1em) rotateX(40deg);\r
          transform: rotate(10deg) perspective(1em) rotateX(40deg);\r
}\r
.ol-editbar .ol-drawline > button:before {\r
  border-bottom: 0;\r
}\r
.ol-editbar .ol-drawline > button:after,\r
.ol-editbar .ol-drawhole > button:after,\r
.ol-editbar .ol-drawpolygon > button:after {\r
  width: .3em;\r
  height: .3em;\r
  top: 0.2em;\r
  left: .25em;\r
  -webkit-box-shadow: -0.2em 0.55em, 0.6em 0.1em, 0.65em 0.7em;\r
          box-shadow: -0.2em 0.55em, 0.6em 0.1em, 0.65em 0.7em;\r
}\r
.ol-editbar .ol-drawhole > button:after {\r
  -webkit-box-shadow: -0.2em 0.55em, 0.6em 0.1em, 0.65em 0.7em, 0.25em 0.35em;\r
          box-shadow: -0.2em 0.55em, 0.6em 0.1em, 0.65em 0.7em, 0.25em 0.35em;\r
}\r
\r
\r
.ol-editbar .ol-offset > button i,\r
.ol-editbar .ol-transform > button i {\r
  position: absolute;\r
  width: .9em;\r
  height: .9em;\r
  overflow: hidden;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
.ol-editbar .ol-offset > button i{\r
  width: .8em;\r
  height: .8em;\r
}\r
\r
.ol-editbar .ol-offset > button i:before,\r
.ol-editbar .ol-transform > button i:before,\r
.ol-editbar .ol-transform > button i:after {\r
  content: "";\r
  height: 1em;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%) rotate(45deg);\r
          transform: translate(-50%, -50%) rotate(45deg);\r
  -webkit-box-shadow: 0.5em 0 0 0.1em, -0.5em 0 0 0.1em;\r
          box-shadow: 0.5em 0 0 0.1em, -0.5em 0 0 0.1em;\r
  width: .1em;\r
  position: absolute;\r
  background-color: currentColor;\r
}\r
.ol-editbar .ol-offset > button i:before{\r
  -webkit-box-shadow: 0.45em 0 0 0.1em, -0.45em 0 0 0.1em;\r
          box-shadow: 0.45em 0 0 0.1em, -0.45em 0 0 0.1em;\r
}\r
.ol-editbar .ol-transform > button i:after {\r
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);\r
          transform: translate(-50%, -50%) rotate(-45deg);\r
}\r
\r
.ol-editbar .ol-split > button:before {\r
  width: .3em;\r
  height: .3em;\r
  top: .81em;\r
  left: .75em;\r
  border-radius: 50%;\r
  -webkit-box-shadow: 0.1em -0.4em, -0.15em -0.25em;\r
          box-shadow: 0.1em -0.4em, -0.15em -0.25em;\r
}\r
.ol-editbar .ol-split > button:after {\r
  width: .8em;\r
  height: .8em;\r
  top: .15em;\r
  left: -.1em;\r
  border: .1em solid currentColor;\r
  border-width: 0 .2em .2em 0;\r
  background-color: transparent;\r
  border-radius: .1em;\r
  -webkit-transform: rotate(20deg) scaleY(.6) rotate(-45deg);\r
          transform: rotate(20deg) scaleY(.6) rotate(-45deg);\r
}\r
\r
.ol-editbar .ol-drawregular > button:before {\r
  width: .9em;\r
  height: .9em;\r
  top: 50%;\r
  left: 50%;\r
  border: .1em solid currentColor;\r
  background-color: transparent;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
.ol-editbar .ol-drawregular .ol-bar .ol-text-button > div > div > div {\r
  border: .5em solid currentColor;\r
  border-color: transparent currentColor;\r
  display: inline-block;\r
  cursor: pointer;\r
  vertical-align: text-bottom;\r
}\r
.ol-editbar .ol-drawregular .ol-bar:before,\r
.ol-control.ol-bar.ol-editbar .ol-drawregular .ol-bar {\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
}\r
.ol-editbar .ol-drawregular .ol-bar .ol-text-button {\r
  min-width: 6em;\r
  text-align: center;\r
}\r
.ol-editbar .ol-drawregular .ol-bar .ol-text-button > div > div > div:first-child {\r
  border-width: .5em .5em .5em 0;\r
  margin: 0 .5em 0 0;\r
}\r
.ol-editbar .ol-drawregular .ol-bar .ol-text-button > div > div > div:last-child {\r
  border-width: .5em 0 .5em .5em;\r
  margin: 0 0 0 .5em;\r
}\r
\r
.ol-feature-list {\r
  position: relative;\r
}\r
.ol-control.ol-feature-list {\r
  position: absolute;\r
  bottom: 0;\r
  left: 0;\r
  width: 100%;\r
}\r
.ol-control.ol-feature-list > button:before {\r
  content: "";\r
  position: absolute;\r
  width: 60%;\r
  height: 50%;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  -webkit-box-shadow: inset 0 2.5px;\r
          box-shadow: inset 0 2.5px;  \r
  background-image: -webkit-gradient(linear, left top, right top, from(currentColor), to(transparent));  \r
  background-image: linear-gradient(90deg, currentColor, transparent);\r
}\r
\r
.ol-control.ol-feature-list.ol-collapsed {\r
  bottom: .5em;\r
  left: .5em;\r
  width: auto;\r
}\r
.ol-feature-list .ol-content {\r
  background-color: #f4f6f8;\r
  clear: both;\r
}\r
.ol-feature-list.ol-collapsed .ol-content > * {\r
  display: none;\r
}\r
.ol-feature-list .ol-header {\r
  position: relative;\r
  overflow: hidden;\r
}\r
.ol-feature-list.ol-collapsed .ol-header {\r
  display: block;\r
}\r
\r
.ol-control.ol-feature-list > button,\r
.ol-control.ol-feature-list.ol-collapsed .ol-content {\r
  display: none;\r
}\r
.ol-control.ol-feature-list.ol-collapsed > button {\r
  display: block;\r
}\r
\r
.ol-feature-list .ol-sizer {\r
  width: 100%;\r
  height: 1px;\r
  cursor: row-resize;\r
  background-color: #ccc;\r
  position: relative;\r
  border: 1px solid #f4f6f8;\r
  border-width: 1px 0;\r
}\r
.ol-feature-list.ol-bottom .ol-sizer {\r
  position: absolute;\r
  bottom: 0;\r
}\r
.ol-feature-list .ol-sizer:before {\r
  content: "";\r
  width: 1.5em;\r
  height: 4px;\r
  background-color: #ddd;\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  z-index: 1;\r
  border: 3px solid transparent;\r
  border-width: 3px 1.5em;\r
  -webkit-box-shadow: inset 0 -1px #aaa, inset 0 1px #aaa;\r
          box-shadow: inset 0 -1px #aaa, inset 0 1px #aaa;\r
  border-radius: 2px;\r
}\r
.ol-feature-list .ol-sizer:hover:before {\r
  background-color: #ccc;\r
}\r
\r
.ol-feature-list .ol-content .ol-header p {\r
  display: inline-block;\r
  margin: 0.1em .25em;\r
  vertical-align: middle;\r
  white-space: nowrap;\r
  width: calc(100% - 2em);\r
  text-overflow: ellipsis;\r
  overflow: hidden;\r
}\r
.ol-feature-list .ol-content .ol-header .ol-buttons {\r
  position: absolute;\r
  right: 0;\r
  top: 50%;\r
  -webkit-transform: translateY(-50%);\r
          transform: translateY(-50%);\r
}\r
\r
.ol-feature-list .ol-content .ol-header button {\r
  position: relative;\r
  border: 0;\r
  background: transparent;\r
  vertical-align: middle;\r
  width: 1em;\r
  height: 1em;\r
  margin: 0 .2em;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
.ol-feature-list .ol-content .ol-header button.ol-closebox:before,\r
.ol-feature-list .ol-content .ol-header button.ol-closebox:after {\r
  content: "";\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  -webkit-transform: translate(-50%, -50%) rotate(45deg);\r
          transform: translate(-50%, -50%) rotate(45deg);\r
  background: currentColor;\r
  width: .1em;\r
  height: 1em;\r
  display: block;\r
  font-style: normal;\r
}\r
.ol-feature-list .ol-content .ol-header button.ol-closebox:after {\r
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);\r
          transform: translate(-50%, -50%) rotate(-45deg);\r
}\r
\r
.ol-feature-list.ol-collapsed .ol-content .ol-header button.ol-closebox {\r
  border: 1px solid currentColor;\r
}\r
.ol-feature-list.ol-collapsed .ol-content .ol-header button.ol-closebox:before {\r
  -webkit-transform: translate(-50%, -50%) rotate(90deg);\r
          transform: translate(-50%, -50%) rotate(90deg);\r
}\r
.ol-feature-list.ol-collapsed .ol-content .ol-header button.ol-closebox:after {\r
  display: none;\r
}\r
\r
.ol-feature-list .ol-scroll-container {\r
  overflow: auto;\r
  height: 10em;\r
  min-height: 3em;\r
}\r
.ol-feature-list .ol-scroll-container:focus {\r
  outline: unset;\r
}\r
\r
.ol-feature-list table {\r
  background-color: #fff;\r
  border-collapse: collapse;\r
  white-space: nowrap;\r
  position: relative;\r
}\r
.ol-feature-list table thead {\r
  background-color: #f4f6f8;\r
  position: -webkit-sticky;\r
  position: sticky;\r
  top: 0;\r
  overflow: hidden;\r
}\r
/* Fixed height */\r
.ol-feature-list table tr {\r
  height: 1.4em;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
.ol-feature-list table tbody tr:hover {\r
  background-color: #eee;\r
  cursor: pointer;\r
}\r
\r
.ol-feature-list table tbody tr.ol-selected {\r
  background-color: #eee;\r
}\r
\r
.ol-feature-list table td {\r
  border: 0 solid #ddd;\r
  border-width: 0 0 1px 1px;\r
  overflow: hidden;\r
  width: 100px;\r
}\r
.ol-feature-list table td.number {\r
  text-align: right;\r
}\r
.ol-feature-list table td.undefined {\r
  text-align: center;\r
  background-color: #fafafa;\r
}\r
.ol-feature-list table tr:hover td,\r
.ol-feature-list table .ol-selected td {\r
  background-color: transparent;\r
}\r
.ol-feature-list thead td {\r
  position: relative;\r
}\r
.ol-feature-list thead td.sort {\r
  padding-right: 1.5em;\r
}\r
\r
.ol-feature-list thead td p {\r
  margin: 0;\r
  display: inline-block;\r
}\r
.ol-feature-list table td button {\r
  position: absolute;\r
  width: 1.2em;\r
  height: 1.4em;\r
  top: 0;\r
  right: 0;\r
  margin: .1em 0;\r
  padding: 0;\r
  background-color: transparent;\r
  border: 0;\r
}\r
.ol-feature-list thead td button.sort:before {\r
  content: '\u21C5';\r
  font-style: normal;\r
  font-weight: normal;\r
  cursor: pointer;\r
  font-size: 1em;\r
  line-height: 1em;\r
}\r
.ol-feature-list thead td button.sortup:before {\r
  content: '\u21A7';\r
}\r
.ol-feature-list thead td button.sortdown:before {\r
  content: '\u21A5';\r
}\r
\r
.ol-gauge {\r
  top: 0.5em;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
  transform: translateX(-50%);\r
  background-color: rgba(255,255,255,.5);\r
  padding: 0.1em 0.5em;\r
}\r
\r
.ol-gauge > * {\r
  display: inline-block;\r
  vertical-align: middle;\r
}\r
.ol-gauge > span {\r
  margin: 0 0.5em 0 0;\r
}\r
.ol-gauge > div {\r
  display: inline-block;\r
  width: 200px;\r
  border: 1px solid rgba(0,60,136,.5);\r
  border-radius: 3px;\r
  padding:1px;\r
}\r
.ol-gauge button {\r
  height: 0.8em;\r
  margin:0;\r
  max-width:100%;\r
}\r
\r
.ol-control.ol-bookmark {\r
  top: 0.5em;\r
  left: 3em;\r
  background-color: rgba(255,255,255,.5);\r
}\r
.ol-control.ol-bookmark button {\r
  position: relative;\r
}\r
.ol-control.ol-bookmark > button::before {\r
  content: "";\r
  position: absolute;\r
  border-width: 10px 5px 4px;\r
  border-style: solid;\r
  border-color: currentColor;\r
  border-bottom-color: transparent;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
  transform: translate(-50%, -50%);\r
  height: 0;\r
}\r
\r
\r
.ol-control.ol-bookmark > div {\r
  display: block;\r
  min-width: 5em;\r
}\r
.ol-control.ol-bookmark.ol-collapsed > div {\r
  display: none;\r
}\r
.ol-control.ol-bookmark input {\r
  font-size: 0.9em;\r
  margin: 0 0.5em 0.5em;\r
  padding: 0 0.5em;\r
}\r
.ol-control.ol-bookmark ul {\r
  margin: 0 0 0.5em;\r
  padding: 0;\r
  list-style: none;\r
  min-width: 10em;\r
}\r
.ol-control.ol-bookmark li {\r
  color: rgba(0,60,136,0.8);\r
  font-size: 0.9em;\r
  padding: 0 0.2em 0 0.5em;\r
  cursor: default;\r
  clear:both;\r
}\r
\r
.ol-control.ol-bookmark li:hover {\r
  background-color: rgba(0,60,136,.5);\r
  color: #fff;\r
}\r
\r
.ol-control.ol-bookmark > div button {\r
  width: 1em;\r
  height: 0.8em;\r
  float: right;\r
  background-color: transparent;\r
  cursor: pointer;\r
  border-radius: 0;\r
}\r
.ol-control.ol-bookmark > div button:before {\r
  content: "\\2A2F";\r
  color: #936;\r
  font-size: 1.2em;\r
  line-height: 1em;\r
  border-radius: 0;\r
    position: absolute;\r
    top: 50%;\r
    left: 50%;\r
    -webkit-transform: translate(-50%, -50%);\r
    transform: translate(-50%, -50%);\r
}\r
\r
.ol-bookmark ul li button,\r
.ol-bookmark input {\r
  display: none;\r
}\r
.ol-bookmark.ol-editable ul li button,\r
.ol-bookmark.ol-editable input {\r
  display: block;\r
}\r
\r
\r
.ol-control.ol-geobt {\r
  top: auto;\r
  left: auto;\r
  right: .5em;\r
  bottom: 3em;\r
}\r
.ol-touch .ol-control.ol-geobt {\r
  bottom: 3.5em;\r
}\r
.ol-control.ol-geobt button:before {\r
  content: "";\r
  position: absolute;\r
  background: transparent;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  border: .16em solid currentColor;\r
  width: .4em;\r
  height: .4em;\r
  border-radius: 50%;\r
}\r
.ol-control.ol-geobt button:after {\r
  content: "";\r
  position: absolute;\r
  width: .2em;\r
  height: .2em;\r
  background: transparent;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  -webkit-box-shadow: .42em 0, -.42em 0, 0 .42em, 0 -.42em;\r
          box-shadow: .42em 0, -.42em 0, 0 .42em, 0 -.42em;\r
}\r
\r
.ol-control.ol-bar.ol-geobar .ol-control {\r
	display: inline-block;\r
	vertical-align: middle;\r
}\r
\r
.ol-control.ol-bar.ol-geobar .ol-bar {\r
  display: none;\r
}\r
.ol-bar.ol-geobar.ol-active .ol-bar {\r
  display: inline-block;\r
}\r
\r
.ol-bar.ol-geobar .geolocBt button:before,\r
.ol-bar.ol-geobar .geolocBt button:after {\r
  content: "";\r
  display: block;\r
  position: absolute;\r
  border: 1px solid transparent;\r
  border-width: 0.3em 0.8em 0 0.2em;\r
  border-color: currentColor transparent transparent;\r
  -webkit-transform: rotate(-30deg);\r
  transform: rotate(-30deg);\r
  top: .45em;\r
  left: 0.15em;\r
  font-size: 1.2em;\r
}\r
.ol-bar.ol-geobar .geolocBt button:after {\r
  border-width: 0 0.8em .3em 0.2em;\r
  border-color: transparent transparent currentColor;\r
	-webkit-transform: rotate(-61deg);\r
	transform: rotate(-61deg);\r
}\r
\r
.ol-bar.ol-geobar .startBt button:before {\r
  content: "";\r
  display: block;\r
  position: absolute;\r
  width: 1em;\r
  height: 1em;\r
  background-color: #800;\r
  border-radius: 50%;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%,-50%);\r
  transform: translate(-50%,-50%);\r
}\r
.ol-bar.ol-geobar .pauseBt button:before,\r
.ol-bar.ol-geobar .pauseBt button:after {\r
  content: "";\r
  display: block;\r
  position: absolute;\r
  width: .25em;\r
  height: 1em;\r
  background-color: currentColor;\r
  top: 50%;\r
  left: 35%;\r
  -webkit-transform: translate(-50%,-50%);\r
  transform: translate(-50%,-50%);\r
}\r
.ol-bar.ol-geobar .pauseBt button:after {\r
  left: 65%;\r
}\r
\r
.ol-control.ol-bar.ol-geobar .centerBt,\r
.ol-control.ol-bar.ol-geobar .pauseBt,\r
.ol-bar.ol-geobar.pauseTrack .startBt,\r
.ol-bar.ol-geobar.centerTrack .startBt,\r
.ol-bar.ol-geobar.centerTrack.pauseTrack .pauseBt,\r
.ol-bar.ol-geobar.centerTrack .pauseBt {\r
  display: none;\r
}\r
.ol-bar.ol-geobar.pauseTrack .pauseBt,\r
.ol-bar.ol-geobar.centerTrack .centerBt{\r
  display: inline-block;\r
}\r
\r
.ol-control.ol-globe\r
{	position: absolute;\r
	left: 0.5em;\r
	bottom: 0.5em;\r
	border-radius: 50%;\r
	opacity: 0.7;\r
	transform: scale(0.5);\r
	transform-origin: 0 100%;\r
	-webkit-transform: scale(0.5);\r
	-webkit-transform-origin: 0 100%;\r
}\r
.ol-control.ol-globe:hover\r
{	opacity: 0.9;\r
}\r
\r
.ol-control.ol-globe .panel\r
{	display:block;\r
	width:170px;\r
	height:170px;\r
	background-color:#fff;\r
	cursor: pointer;\r
	border-radius: 50%;\r
	overflow: hidden;\r
	-webkit-box-shadow: 0 0 10px 5px rgba(255, 255, 255, 0.5);\r
	        box-shadow: 0 0 10px 5px rgba(255, 255, 255, 0.5);\r
}\r
.ol-control.ol-globe .panel .ol-viewport\r
{	border-radius: 50%;\r
}\r
\r
.ol-control.ol-globe .ol-pointer\r
{	display: block;\r
	background-color: #fff;\r
	width:10px;\r
	height: 10px;\r
	border:10px solid red;\r
	position: absolute;\r
	top: 50%;\r
	left:50%;\r
	transform: translate(-15px, -40px);\r
	-webkit-transform: translate(-15px, -40px);\r
	border-radius: 50%;\r
	z-index:1;\r
	transition: opacity 0.15s, top 0s, left 0s;\r
	-webkit-transition: opacity 0.15s, top 0s, left 0s;\r
}\r
.ol-control.ol-globe .ol-pointer.hidden\r
{	opacity:0;\r
	transition: opacity 0.15s, top 3s, left 5s;\r
	-webkit-transition: opacity 0.15s, top 3s, left 5s;\r
}\r
\r
.ol-control.ol-globe .ol-pointer::before\r
{	border-radius: 50%;\r
	-webkit-box-shadow: 6px 6px 10px 5px #000;\r
	        box-shadow: 6px 6px 10px 5px #000;\r
	content: "";\r
	display: block;\r
	height: 0;\r
	left: 0;\r
	position: absolute;\r
	top: 23px;\r
	width: 0;\r
}\r
.ol-control.ol-globe .ol-pointer::after\r
{	content:"";\r
	width:0;\r
	height:0;\r
	display: block;\r
	position: absolute;\r
	border-width: 20px 10px 0;\r
	border-color: red transparent;\r
	border-style: solid;\r
	left: -50%;\r
	top: 100%;\r
}\r
\r
.ol-control.ol-globe .panel::before {\r
  border-radius: 50%;\r
  -webkit-box-shadow: -20px -20px 80px 2px rgba(0, 0, 0, 0.7) inset;\r
          box-shadow: -20px -20px 80px 2px rgba(0, 0, 0, 0.7) inset;\r
  content: "";\r
  display: block;\r
  height: 100%;\r
  left: 0;\r
  position: absolute;\r
  top: 0;\r
  width: 100%;\r
  z-index: 1;\r
}\r
.ol-control.ol-globe .panel::after {\r
  border-radius: 50%;\r
  -webkit-box-shadow: 0 0 20px 7px rgba(255, 255, 255, 1);\r
          box-shadow: 0 0 20px 7px rgba(255, 255, 255, 1);\r
  content: "";\r
  display: block;\r
  height: 0;\r
  left: 23%;\r
  position: absolute;\r
  top: 20%;\r
  -webkit-transform: rotate(-40deg);\r
          transform: rotate(-40deg);\r
  width: 20%;\r
  z-index: 1;\r
}\r
\r
\r
.ol-control.ol-globe.ol-collapsed .panel\r
{	display:none;\r
}\r
\r
.ol-control-top.ol-globe\r
{	bottom: auto;\r
	top: 5em;\r
	transform-origin: 0 0;\r
	-webkit-transform-origin: 0 0;\r
}\r
.ol-control-right.ol-globe\r
{	left: auto;\r
	right: 0.5em;\r
	transform-origin: 100% 100%;\r
	-webkit-transform-origin: 100% 100%;\r
}\r
.ol-control-right.ol-control-top.ol-globe\r
{	left: auto;\r
	right: 0.5em;\r
	transform-origin: 100% 0;\r
	-webkit-transform-origin: 100% 0;\r
}\r
\r
.ol-gridreference\r
{	background: #fff;\r
	border: 1px solid #000;\r
	overflow: auto;\r
	max-height: 100%;\r
	top:0;\r
	right:0;\r
}\r
.ol-gridreference input\r
{	width:100%;\r
}\r
.ol-gridreference ul\r
{	margin:0;\r
	padding:0;\r
	list-style: none;\r
} \r
.ol-gridreference li\r
{	padding: 0 0.5em;\r
	cursor: pointer;\r
}\r
.ol-gridreference ul li:hover \r
{	background-color: #ccc;\r
}\r
.ol-gridreference li.ol-title,\r
.ol-gridreference li.ol-title:hover\r
{	background:rgba(0,60,136,.5);\r
	color:#fff;\r
	cursor:default;\r
}\r
.ol-gridreference ul li .ol-ref\r
{	margin-left: 0.5em;\r
}\r
.ol-gridreference ul li .ol-ref:before\r
{	content:"(";\r
}\r
.ol-gridreference ul li .ol-ref:after\r
{	content:")";\r
}\r
\r
.ol-control.ol-imageline {\r
  bottom:0;\r
  left: 0;\r
  right: 0;\r
  padding: 0;\r
  overflow: visible;\r
  -webkit-transition: .3s;\r
  transition: .3s;\r
  border-radius: 0;\r
}\r
.ol-control.ol-imageline.ol-collapsed {\r
  -webkit-transform: translateY(100%);\r
          transform: translateY(100%);\r
}\r
.ol-imageline > div {\r
  height: 4em;\r
  position: relative;\r
  white-space: nowrap;\r
  scroll-behavior: smooth;\r
  overflow: hidden;\r
  width: 100%;\r
}\r
.ol-imageline > div.ol-move {\r
  scroll-behavior: unset;\r
}\r
\r
.ol-control.ol-imageline button {\r
  position: absolute;\r
  top: -1em;\r
  -webkit-transform: translateY(-100%);\r
          transform: translateY(-100%);\r
  margin: .65em;\r
  -webkit-box-shadow: 0 0 0 0.15em rgba(255,255,255,.4);\r
          box-shadow: 0 0 0 0.15em rgba(255,255,255,.4);\r
}\r
.ol-control.ol-imageline button:before {\r
  content: '';\r
  position: absolute;\r
  -webkit-transform: translate(-50%, -50%) rotate(135deg);\r
          transform: translate(-50%, -50%) rotate(135deg);\r
  top: 40%;\r
  left: 50%;\r
  width: .4em;\r
  height: .4em;\r
  border: .1em solid currentColor;\r
  border-width: .15em .15em 0 0;\r
}\r
.ol-control.ol-imageline.ol-collapsed button:before {\r
  top: 60%;\r
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);\r
          transform: translate(-50%, -50%) rotate(-45deg);\r
}\r
\r
.ol-imageline,\r
.ol-imageline:hover {\r
  background-color: rgba(0,0,0,.75);\r
}\r
\r
.ol-imageline.ol-arrow:after,\r
.ol-imageline.ol-arrow:before {\r
  content: "";\r
  position: absolute;\r
  top: 50%;\r
  left: .2em;\r
  border-color: #fff #000;\r
  border-width: 1em .6em 1em 0;\r
  border-style: solid;\r
  -webkit-transform: translateY(-50%);\r
          transform: translateY(-50%);\r
  z-index: 1;\r
  opacity: .8;\r
  pointer-events: none;\r
  -webkit-box-shadow: -0.6em 0 0 1em #fff;\r
          box-shadow: -0.6em 0 0 1em #fff;\r
}\r
.ol-imageline.ol-arrow:after {\r
  border-width: 1em 0 1em .6em;\r
  left: auto;\r
  right: .2em;\r
  -webkit-box-shadow: 0.6em 0 0 1em #fff;\r
          box-shadow: 0.6em 0 0 1em #fff;\r
}\r
.ol-imageline.ol-scroll0.ol-arrow:before {\r
  display: none;\r
}\r
.ol-imageline.ol-scroll1.ol-arrow:after {\r
  display: none;\r
}\r
\r
\r
.ol-imageline .ol-image {\r
  position: relative;\r
  height: 100%;\r
  display: inline-block;\r
  cursor: pointer;\r
}\r
.ol-imageline img {\r
  max-height: 100%;\r
  border: .25em solid transparent;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  opacity: 0;\r
  -webkit-transition: 1s;\r
  transition: 1s;\r
}\r
.ol-imageline img.ol-loaded {\r
  opacity:1;\r
}\r
\r
.ol-imageline .ol-image.select {\r
  background-color: #fff;\r
}\r
.ol-imageline .ol-image span {\r
  position: absolute;\r
  width: 125%;\r
  max-height: 2.4em;\r
  left: 50%;\r
  bottom: 0;\r
  display: none;\r
  color: #fff;\r
  background-color: rgba(0,0,0,.5);\r
  font-size: .8em;\r
  overflow: hidden;\r
  white-space: normal;\r
  text-align: center;\r
  line-height: 1.2em;\r
  -webkit-transform: translateX(-50%) scaleX(.8);\r
          transform: translateX(-50%) scaleX(.8);\r
}\r
/*\r
.ol-imageline .ol-image.select span,\r
*/\r
.ol-imageline .ol-image:hover span {\r
  display: block;\r
}\r
\r
.ol-control.ol-routing.ol-isochrone .ol-method-time,\r
.ol-control.ol-routing.ol-isochrone .ol-method-distance,\r
.ol-control.ol-routing.ol-isochrone > button {\r
  position: relative;\r
}\r
.ol-control.ol-routing.ol-isochrone .ol-method-time:before,\r
.ol-control.ol-routing.ol-isochrone > button:before {\r
  content: '';\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  border: .1em solid currentColor;\r
  width: .8em;\r
  height: .8em;\r
  border-radius: 50%;\r
  -webkit-box-shadow: 0 -0.5em 0 -0.35em, 0.4em -0.35em 0 -0.35em;\r
          box-shadow: 0 -0.5em 0 -0.35em, 0.4em -0.35em 0 -0.35em;\r
  clip: unset;\r
}\r
.ol-control.ol-routing.ol-isochrone .ol-method-time:after,\r
.ol-control.ol-routing.ol-isochrone > button:after {\r
  content: '';\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%) rotate(-60deg);\r
          transform: translate(-50%, -50%) rotate(-60deg);\r
  border-radius: 50%;\r
  border: .3em solid transparent;\r
  border-right-color: currentColor;\r
  -webkit-box-shadow: none;\r
          box-shadow: none;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  clip: unset;\r
}\r
\r
.ol-control.ol-routing.ol-isochrone .ol-method-distance:before {\r
  content: '';\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%) rotate(-30deg);\r
          transform: translate(-50%, -50%) rotate(-30deg);\r
  width: 1em;\r
  height: .5em;\r
  border: .1em solid currentColor;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box\r
}\r
.ol-control.ol-routing.ol-isochrone .ol-method-distance:after {\r
  content: '';\r
  position: absolute;\r
  width: .1em;\r
  height: .15em;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%) rotate(-30deg);\r
          transform: translate(-50%, -50%) rotate(-30deg);\r
  -webkit-box-shadow: inset 0 -0.15em, 0 0.1em, 0.25em 0.1em, -0.25em 0.1em;\r
          box-shadow: inset 0 -0.15em, 0 0.1em, 0.25em 0.1em, -0.25em 0.1em;\r
}\r
\r
.ol-control.ol-routing.ol-isochrone .ol-direction-direct:before,\r
.ol-control.ol-routing.ol-isochrone .ol-direction-reverse:before {\r
  content: '';\r
  position: absolute;\r
  top: 50%;\r
  left: 30%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  width: .3em;\r
  height: .3em;\r
  border-radius: 50%;\r
  border: .1em solid currentColor;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  -webkit-box-shadow: 0.25em 0 0 -0.05em;\r
          box-shadow: 0.25em 0 0 -0.05em;\r
}\r
.ol-control.ol-routing.ol-isochrone .ol-direction-direct:after,\r
.ol-control.ol-routing.ol-isochrone .ol-direction-reverse:after {\r
  content: '';\r
  position: absolute;\r
  top: 50%;\r
  left: 70%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  border: .4em solid transparent;\r
  border-width: .4em 0 .4em .4em;\r
  border-color: transparent currentColor;\r
}\r
.ol-control.ol-routing.ol-isochrone .ol-direction-reverse:after {\r
  border-width: .4em .4em .4em 0;\r
}\r
\r
.ol-control.ol-isochrone.ol-collapsed .content {\r
  display: none;\r
}\r
.ol-control.ol-isochrone input[type="number"] {\r
  width: 3em;\r
  text-align: right;\r
  margin: 0 .1em;\r
}\r
.ol-control.ol-isochrone .ol-distance input[type="number"] {\r
  width: 5em;\r
}\r
\r
.ol-isochrone .ol-time,\r
.ol-isochrone .ol-distance {\r
  display: none;\r
}\r
.ol-isochrone .ol-time.selected,\r
.ol-isochrone .ol-distance.selected {\r
  display: block;\r
}\r
\r
.ol-control.ol-layerswitcher-popup {\r
  position: absolute;\r
  right: 0.5em;\r
  text-align: left;\r
  top: 3em;\r
}\r
.ol-control.ol-layerswitcher-popup .panel {\r
  clear:both;\r
  background:#fff;\r
}\r
\r
.ol-layerswitcher-popup .panel {\r
  list-style: none;\r
  padding: 0.25em;\r
  margin:0;\r
  overflow: hidden;\r
}\r
\r
.ol-layerswitcher-popup .panel ul {\r
  list-style: none;\r
  padding: 0 0 0 20px;\r
  overflow: hidden;\r
}\r
\r
.ol-layerswitcher-popup.ol-collapsed .panel {\r
  display:none;\r
}\r
.ol-layerswitcher-popup.ol-forceopen .panel {\r
  display:block;\r
}\r
\r
.ol-layerswitcher-popup button  {\r
  background-color: white;\r
  background-image: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAACE1BMVEX///8A//8AgICA//8AVVVAQID///8rVVVJtttgv98nTmJ2xNgkW1ttyNsmWWZmzNZYxM4gWGgeU2JmzNNr0N1Rwc0eU2VXxdEhV2JqytQeVmMhVmNoydUfVGUgVGQfVGQfVmVqy9hqy9dWw9AfVWRpydVry9YhVmMgVGNUw9BrytchVWRexdGw294gVWQgVmUhVWPd4N6HoaZsy9cfVmQgVGRrytZsy9cgVWQgVWMgVWRsy9YfVWNsy9YgVWVty9YgVWVry9UgVWRsy9Zsy9UfVWRsy9YgVWVty9YgVWRty9Vsy9aM09sgVWRTws/AzM0gVWRtzNYgVWRuy9Zsy9cgVWRGcHxty9bb5ORbxdEgVWRty9bn6OZTws9mydRfxtLX3Nva5eRix9NFcXxOd4JPeINQeIMiVmVUws9Vws9Vw9BXw9BYxNBaxNBbxNBcxdJexdElWWgmWmhjyNRlx9IqXGtoipNpytVqytVryNNrytZsjZUuX210k5t1y9R2zNR3y9V4lp57zth9zdaAnKOGoaeK0NiNpquV09mesrag1tuitbmj1tuj19uktrqr2d2svcCu2d2xwMO63N+7x8nA3uDC3uDFz9DK4eHL4eLN4eIyYnDX5OM5Z3Tb397e4uDf4uHf5uXi5ePi5+Xj5+Xk5+Xm5+Xm6OY6aHXQ19fT4+NfhI1Ww89gx9Nhx9Nsy9ZWw9Dpj2abAAAAWnRSTlMAAQICAwQEBgcIDQ0ODhQZGiAiIyYpKywvNTs+QklPUlNUWWJjaGt0dnd+hIWFh4mNjZCSm6CpsbW2t7nDzNDT1dje5efr7PHy9PT29/j4+Pn5+vr8/f39/f6DPtKwAAABTklEQVR4Xr3QVWPbMBSAUTVFZmZmhhSXMjNvkhwqMzMzMzPDeD+xASvObKePPa+ffHVl8PlsnE0+qPpBuQjVJjno6pZpSKXYl7/bZyFaQxhf98hHDKEppwdWIW1frFnrxSOWHFfWesSEWC6R/P4zOFrix3TzDFLlXRTR8c0fEEJ1/itpo7SVO9Jdr1DVxZ0USyjZsEY5vZfiiAC0UoTGOrm9PZLuRl8X+Dq1HQtoFbJZbv61i+Poblh/97TC7n0neCcK0ETNUrz1/xPHf+DNAW9Ac6t8O8WH3Vp98f5lCaYKAOFZMLyHL4Y0fe319idMNgMMp+zWVSybUed/+/h7I4wRAG1W6XDy4XmjR9HnzvDRZXUAYDFOhC1S/Hh+fIXxen+eO+AKqbs+wAo30zDTDvDxKoJN88sjUzDFAvBzEUGFsnADoIvAJzoh2BZ8sner+Ke/vwECuQAAAABJRU5ErkJggg==");\r
  background-position: center;\r
  background-repeat: no-repeat;\r
  float: right;\r
  height: 38px;\r
  width: 38px;\r
}\r
\r
.ol-layerswitcher-popup li {\r
  color:#369;\r
  padding:0.25em 1em;\r
  font-family:"Trebuchet MS",Helvetica,sans-serif;\r
  cursor:pointer;\r
}\r
.ol-layerswitcher-popup li.ol-header {\r
  display: none;\r
}\r
.ol-layerswitcher-popup li.select,\r
.ol-layerswitcher-popup li.ol-visible {\r
  background:rgba(0, 60, 136, 0.7);\r
  color:#fff;\r
}\r
.ol-layerswitcher-popup li:hover {\r
  background:rgba(0, 60, 136, 0.9);\r
  color:#fff;\r
}\r
\r
.ol-control.ol-layerswitcher.ol-layer-shop {\r
  height: calc(100% - 4em);\r
  max-height: unset;\r
  max-width: 16em;\r
  background-color: transparent;\r
  pointer-events: none!important;\r
  overflow: visible;\r
}\r
.ol-control.ol-layerswitcher > * {\r
  pointer-events: auto;\r
}\r
\r
.ol-control.ol-layer-shop > button,\r
.ol-control.ol-layer-shop .panel-container {\r
  -webkit-box-shadow: 0 0 0 3px rgba(255,255,255,.5);\r
          box-shadow: 0 0 0 3px rgba(255,255,255,.5);\r
}\r
.ol-control.ol-layerswitcher.ol-layer-shop .panel-container {\r
  overflow-y: scroll;\r
  max-height: calc(100% - 6.5em);\r
  border: 2px solid #369;\r
  border-width: 2px 0;\r
  padding: 0;\r
}\r
.ol-control.ol-layer-shop .panel {\r
  padding: 0;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  margin: .25em .5em;\r
}\r
.ol-control.ol-layerswitcher.ol-layer-shop .panel-container.ol-scrolldiv {\r
  overflow: hidden;\r
}\r
.ol-control.ol-layer-shop .ol-scroll {\r
  background-color: rgba(0,0,0,.3);\r
  opacity: .5;\r
}\r
.ol-layerswitcher.ol-layer-shop ul.panel li.ol-header {\r
  display: none;\r
}\r
.ol-layerswitcher.ol-layer-shop ul.panel li {\r
  margin-right: 0;\r
  padding-right: 0;\r
}\r
.ol-layerswitcher.ol-layer-shop .layerup {\r
  height: 1.5em;\r
  width: 1.4em;\r
  margin: 0;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  border-radius: 3px;\r
  background-color: transparent;\r
  color: rgba(0,60,136,1);\r
}\r
.ol-layerswitcher.ol-layer-shop .layerup:hover {\r
  background-color: rgba(0,60,136,.3);\r
}\r
.ol-layerswitcher.ol-layer-shop .layerup:before {\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  border: 0;\r
  background-color: currentColor;\r
  width: 1em;\r
  height: 2px;\r
  -webkit-box-shadow: 0 -4px, 0 4px;\r
          box-shadow: 0 -4px, 0 4px;\r
}\r
.ol-layerswitcher.ol-layer-shop .layerup:after {\r
  content: unset;\r
}\r
\r
.ol-control.ol-layer-shop .ol-title-bar {\r
  background-color: rgba(255,255,255,.5);\r
  font-size: .9em;\r
  height: calc(2.8em - 4px);\r
  max-width: 14.6em;\r
  padding: .7em .5em;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  overflow: hidden;\r
  text-overflow: ellipsis;\r
  white-space: nowrap;\r
  text-align: right;\r
  -webkit-transform: scaleY(1.1);\r
          transform: scaleY(1.1);\r
  -webkit-transition: width 0s, -webkit-transform .1s;\r
  transition: width 0s, -webkit-transform .1s;\r
  transition: transform .1s, width 0s;\r
  transition: transform .1s, width 0s, -webkit-transform .1s;\r
  -webkit-transform-origin: 100% 0;\r
          transform-origin: 100% 0;\r
}\r
.ol-control.ol-layer-shop:hover .ol-title-bar {\r
  background-color: rgba(255,255,255,.7);\r
}\r
.ol-control.ol-layer-shop.ol-collapsed .ol-title-bar {\r
  max-width: 10em;\r
  -webkit-transform: scale(.9, 1.1);\r
          transform: scale(.9, 1.1);\r
}\r
.ol-control.ol-layer-shop.ol-forceopen .ol-title-bar {\r
  max-width: 14.6em;\r
  -webkit-transform: scaleY(1.1);\r
          transform: scaleY(1.1);\r
}\r
\r
.ol-control.ol-layer-shop .ol-bar {\r
  position: relative;\r
  height: 1.75em;\r
  clear: both;\r
  -webkit-box-shadow: 0 0 0 3px rgba(255,255,255,.5);\r
          box-shadow: 0 0 0 3px rgba(255,255,255,.5);\r
  background-color: #fff;\r
  text-align: right;\r
  z-index: 10;\r
}\r
.ol-control.ol-layer-shop.ol-collapsed .ol-scroll,\r
.ol-control.ol-layer-shop.ol-collapsed .ol-bar {\r
  border-width: 2px 0 0;\r
  display: none;\r
}\r
.ol-control.ol-layer-shop.ol-forceopen .ol-scroll,\r
.ol-control.ol-layer-shop.ol-forceopen .ol-bar  {\r
  display: block;\r
}\r
.ol-control.ol-layer-shop .ol-bar > * {\r
  font-size: .9em;\r
  display: inline-block;\r
  vertical-align: middle;\r
  margin-top: .25em;\r
  background-color: transparent;\r
}\r
\r
.ol-layer-shop .ol-bar .ol-button,\r
.ol-touch .ol-layer-shop .ol-bar .ol-button {\r
  position: relative;\r
  top: unset;\r
  left: unset;\r
  bottom: unset;\r
  right: unset;\r
  margin: 0;\r
}\r
.ol-layer-shop .ol-bar button {\r
  background-color: #fff;\r
  color: rgba(0,60,136,1)\r
}\r
.ol-layer-shop .ol-bar button:hover {\r
  background-color: rgba(0,60,136,.2);\r
}\r
\r
/* Touch device */\r
.ol-touch .ol-layerswitcher.ol-layer-shop > button {\r
  font-size: 1.7em;\r
}\r
.ol-touch .ol-layer-shop .ol-bar {\r
  height: 2em;\r
}\r
.ol-touch .ol-layer-shop .ol-control button {\r
  font-size: 1.4em;\r
}\r
.ol-touch .ol-control.ol-layer-shop .panel {\r
  max-height: calc(100% - 7em);\r
}\r
.ol-touch .ol-control.ol-layer-shop .panel label {\r
  height: 1.8em;\r
}\r
.ol-touch .ol-control.ol-layer-shop .panel label span {\r
  margin-left: .5em;\r
  padding-top: .25em;\r
}\r
.ol-touch .ol-control.ol-layer-shop .panel label:before,\r
.ol-touch .ol-control.ol-layer-shop .panel label:after {\r
  font-size: 1.3em;\r
  z-index: 1;\r
}\r
\r
.ol-control.ol-layerswitcher {\r
  position: absolute;\r
  right: 0.5em;\r
  text-align: left;\r
  top: 3em;\r
  max-height: calc(100% - 6em);\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  overflow: hidden;\r
}\r
.ol-control.ol-layerswitcher .ol-switchertopdiv,\r
.ol-control.ol-layerswitcher .ol-switcherbottomdiv {\r
  display: block\r
}\r
.ol-control.ol-layerswitcher.ol-collapsed .ol-switchertopdiv,\r
.ol-control.ol-layerswitcher.ol-collapsed .ol-switcherbottomdiv {\r
  display: none;\r
}\r
.ol-layerswitcher.ol-forceopen.ol-collapsed .ol-switchertopdiv,\r
.ol-layerswitcher.ol-forceopen.ol-collapsed .ol-switcherbottomdiv {\r
  display: block;\r
}\r
\r
.ol-control.ol-layerswitcher .ol-switchertopdiv,\r
.ol-control.ol-layerswitcher .ol-switcherbottomdiv {\r
  position: absolute;\r
  top:0;\r
  left:0;\r
  right:0;\r
  height: 45px;\r
  background: #fff; \r
  z-index:2;\r
  opacity:1;\r
  cursor: pointer;\r
  border-top:2px solid transparent;\r
  border-bottom:2px solid #369;\r
  margin:0 2px;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
.ol-control.ol-layerswitcher .ol-switcherbottomdiv {\r
  top: auto;\r
  bottom: 0;\r
  height: 2em;\r
  border-top:2px solid #369;\r
  border-bottom:2px solid transparent;\r
}\r
.ol-control.ol-layerswitcher .ol-switchertopdiv:before,\r
.ol-control.ol-layerswitcher .ol-switcherbottomdiv:before {\r
  content:"";\r
  position: absolute;\r
  left:50%;\r
  top:50%;\r
  border:10px solid transparent;\r
  width:0;\r
  height:0;\r
  transform: translate(-50%, -50%);\r
  -webkit-transform: translate(-50%, -50%);\r
  opacity:0.8;\r
}\r
\r
.ol-control.ol-layerswitcher .ol-switchertopdiv:hover:before,\r
.ol-control.ol-layerswitcher .ol-switcherbottomdiv:hover:before {\r
  opacity:1;\r
}\r
.ol-control.ol-layerswitcher .ol-switchertopdiv:before {\r
  border-bottom-color: #369;\r
  border-top: 0;\r
}\r
.ol-control.ol-layerswitcher .ol-switcherbottomdiv:before {\r
  border-top-color: #369;\r
  border-bottom: 0;\r
}\r
\r
.ol-control.ol-layerswitcher .panel-container {\r
  background-color: #fff;\r
  border-radius: 0 0 2px 2px;\r
  clear: both;\r
  display: block; /* display:block to show panel on over */\r
  padding: 0.5em 0.5em 0;\r
}\r
\r
.ol-layerswitcher .panel {\r
  list-style: none;\r
  padding: 0;\r
  margin: 0;\r
  overflow: hidden;\r
  font-family: Tahoma,Geneva,sans-serif;\r
  font-size:0.9em;\r
  -webkit-transition: top 0.3s;\r
  transition: top 0.3s;\r
  position: relative;\r
  top:0;\r
}\r
\r
.ol-layerswitcher .panel ul {\r
  list-style: none;\r
  padding: 0 0 0 20px;\r
  overflow: hidden;\r
  clear: both;\r
}\r
\r
/** Customize checkbox\r
*/\r
.ol-layerswitcher input[type="radio"],\r
.ol-layerswitcher input[type="checkbox"] {\r
  display:none;\r
}\r
\r
.ol-layerswitcher .panel li {\r
  -weblit-transition: -webkit-transform 0.2s linear;\r
  -webkit-transition: -webkit-transform 0.2s linear;\r
  transition: -webkit-transform 0.2s linear;\r
  transition: transform 0.2s linear;\r
  transition: transform 0.2s linear, -webkit-transform 0.2s linear;\r
  clear: both;\r
  display: block;\r
  border:1px solid transparent;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
.ol-layerswitcher .panel li.ol-layer-select {\r
  background-color: rgba(0,60,136,.2);\r
  margin: 0 -.5em;\r
  padding: 0 .5em\r
}\r
/* drag and drop */\r
.ol-layerswitcher .panel li.drag {\r
  opacity: 0.5;\r
  transform:scale(0.8);\r
  -webkit-transform:scale(0.8);\r
}\r
.ol-dragover {\r
  background:rgba(51,102,153,0.5);\r
  opacity:0.8;\r
}\r
.ol-layerswitcher .panel li.forbidden,\r
.forbidden .ol-layerswitcher-buttons div,\r
.forbidden .layerswitcher-opacity div {\r
  background:rgba(255,0,0,0.5);\r
  color:#f00!important;\r
}\r
\r
/* cursor management */\r
.ol-layerswitcher.drag,\r
.ol-layerswitcher.drag * {\r
  cursor:not-allowed!important;\r
  cursor:no-drop!important;\r
}\r
.ol-layerswitcher.drag .panel li.dropover,\r
.ol-layerswitcher.drag .panel li.dropover * {\r
  cursor: pointer!important;\r
  cursor: n-resize!important;\r
  cursor: ns-resize!important;\r
  cursor: -webkit-grab!important;\r
  cursor: grab!important;\r
  cursor: -webkit-grabbing!important;\r
  cursor: grabbing!important;\r
}\r
\r
.ol-layerswitcher .panel li.dropover {\r
  background: rgba(51, 102, 153, 0.5);\r
}\r
\r
.ol-layerswitcher .panel li label {\r
  display: inline-block;\r
  height: 1.4em;\r
  max-width: 12em;\r
  overflow: hidden;\r
  white-space: nowrap;\r
  text-overflow: ellipsis;\r
  padding: 0 0 0 1.7em;\r
  position: relative;\r
}\r
\r
.ol-layerswitcher .panel li label span {\r
  display: inline-block;\r
  width: 100%;\r
  height: 100%;\r
  overflow: hidden;\r
  text-overflow: ellipsis;\r
  padding-right: .2em;\r
}\r
.ol-layerswitcher [type="radio"] + label:before,\r
.ol-layerswitcher [type="checkbox"] + label:before,\r
.ol-layerswitcher [type="radio"]:checked + label:after,\r
.ol-layerswitcher [type="checkbox"]:checked + label:after {\r
  content: '';\r
  position: absolute;\r
  left: 0.1em; top: 0.1em;\r
  width: 1.2em; height: 1.2em; \r
  border: 2px solid #369;\r
  background: #fff;\r
  -webkit-box-sizing:border-box;\r
          box-sizing:border-box;\r
}\r
\r
.ol-layerswitcher [type="radio"] + label:before,\r
.ol-layerswitcher [type="radio"] + label:after {\r
  border-radius: 50%;\r
}\r
\r
.ol-layerswitcher [type="radio"]:checked + label:after {\r
  background: #369 none repeat scroll 0 0;\r
  margin: 0.3em;\r
  width: 0.6em;\r
  height: 0.6em;\r
}\r
\r
.ol-layerswitcher [type="checkbox"]:checked + label:after {\r
  background: transparent;\r
  border-width: 0 3px 3px 0;\r
  border-style: solid;\r
  border-color: #369;\r
    width: 0.7em;\r
    height: 1em;\r
    -webkit-transform: rotate(45deg);\r
    transform: rotate(45deg);\r
    left: 0.55em;\r
    top: -0.05em;\r
    -webkit-box-shadow: 1px 0px 1px 1px #fff;\r
            box-shadow: 1px 0px 1px 1px #fff;\r
}\r
\r
.ol-layerswitcher .panel li.ol-layer-hidden {\r
  opacity: 0.6;\r
}\r
\r
.ol-layerswitcher.ol-collapsed .panel-container {\r
  display:none;\r
}\r
.ol-layerswitcher.ol-forceopen .panel-container {\r
  display:block;\r
}\r
\r
.ol-layerswitcher-image > button,\r
.ol-layerswitcher > button {\r
  background-color: white;\r
  float: right;\r
  z-index: 10;\r
  position: relative;\r
  font-size: 1.7em;\r
}\r
.ol-touch .ol-layerswitcher-image > button,\r
.ol-touch .ol-layerswitcher > button {\r
  font-size: 2.5em;\r
}\r
.ol-layerswitcher-image > button:before,\r
.ol-layerswitcher-image > button:after,\r
.ol-layerswitcher > button:before,\r
.ol-layerswitcher > button:after {\r
  content: "";\r
  position:absolute;\r
  width: .75em;\r
  height: .75em;\r
  border-radius: 0.15em;\r
  -webkit-transform: scaleY(.8) rotate(45deg);\r
  transform: scaleY(.8) rotate(45deg);\r
}\r
.ol-layerswitcher-image > button:before,\r
.ol-layerswitcher > button:before {\r
  background: #e2e4e1;\r
  top: .32em;\r
  left: .34em;\r
  -webkit-box-shadow: 0.1em 0.1em #325158;\r
  box-shadow: 0.1em 0.1em #325158;\r
}\r
.ol-layerswitcher-image > button:after,\r
.ol-layerswitcher > button:after {\r
  top: .22em;\r
  left: .34em;\r
  background: #83bcc5;\r
  background-image: radial-gradient( circle at .85em .6em, #70b3be 0, #70b3be .65em, #83bcc5 .65em);\r
}\r
.ol-layerswitcher-buttons {\r
  display:block;\r
  float: right;\r
  text-align:right;\r
}\r
.ol-layerswitcher-buttons > div {\r
  display: inline-block;\r
  position: relative;\r
  cursor: pointer;\r
  height:1em;\r
  width:1em;\r
  margin:2px;\r
  line-height: 1em;\r
    text-align: center;\r
    background: #369;\r
    vertical-align: middle;\r
    color: #fff;\r
}\r
\r
.ol-layerswitcher .panel li > div {\r
  display: inline-block;\r
  position: relative;\r
}\r
\r
/* line break */\r
.ol-layerswitcher .ol-separator {\r
  display:block;\r
  width:0;\r
  height:0;\r
  padding:0;\r
  margin:0;\r
}\r
\r
.ol-layerswitcher .layerup {\r
  float: right;\r
  height:2.5em;\r
  background-color: #369;\r
  opacity: 0.5;\r
  cursor: move;\r
  cursor: ns-resize;\r
}\r
\r
.ol-layerswitcher .layerup:before,\r
.ol-layerswitcher .layerup:after {\r
  border-color: #fff transparent;\r
  border-style: solid;\r
  border-width: 0.4em 0.4em 0;\r
  content: "";\r
  height: 0;\r
  position: absolute;\r
  bottom: 3px;\r
  left: 0.1em;\r
  width: 0;\r
}\r
.ol-layerswitcher .layerup:after {\r
  border-width: 0 0.4em 0.4em;\r
  top:3px;\r
  bottom: auto;\r
}\r
\r
.ol-layerswitcher .layerInfo {\r
  background: #369;\r
  border-radius: 100%;\r
}\r
.ol-layerswitcher .layerInfo:before {\r
  color: #fff;\r
  content: "i";\r
  display: block;\r
  font-size: 0.8em;\r
  font-weight: bold;\r
  text-align: center;\r
  width: 1.25em;\r
  position:absolute;\r
  left: 0;\r
  top: 0;\r
}\r
\r
.ol-layerswitcher .layerTrash {\r
  background: #369;\r
}\r
.ol-layerswitcher .layerTrash:before {\r
  color: #fff;\r
  content: "\\00d7";\r
  font-size:1em;\r
  top: 50%;\r
  left: 0;\r
  right: 0;\r
  text-align: center;\r
  line-height: 1em;\r
  margin: -0.5em 0;\r
  position: absolute;\r
}\r
\r
.ol-layerswitcher .layerExtent {\r
  background: #369;\r
}\r
.ol-layerswitcher .layerExtent:before {\r
  border-right: 1px solid #fff;\r
  border-bottom: 1px solid #fff;\r
  content: "";\r
  display: block;\r
  position: absolute;\r
  left: 6px;\r
  right: 2px;\r
  top: 6px;\r
  bottom: 3px;\r
}\r
.ol-layerswitcher .layerExtent:after {\r
  border-left: 1px solid #fff;\r
  border-top: 1px solid #fff;\r
  content: "";\r
  display: block;\r
  position: absolute;\r
  bottom: 6px;\r
  left: 2px;\r
  right: 6px;\r
  top: 3px;\r
}\r
\r
.ol-layerswitcher .expend-layers,\r
.ol-layerswitcher .collapse-layers {\r
  margin: 0 2px;\r
  background-color: transparent;\r
}\r
.ol-layerswitcher .expend-layers:before,\r
.ol-layerswitcher .collapse-layers:before {\r
  content:"";\r
  position:absolute;\r
  top:50%;\r
  left:0;\r
  margin-top:-2px;\r
  height:4px;\r
  width:100%;\r
  background:#369;\r
}\r
.ol-layerswitcher .expend-layers:after {\r
  content:"";\r
  position:absolute;\r
  left:50%;\r
  top:0;\r
  margin-left:-2px;\r
  width:4px;\r
  height:100%;\r
  background:#369;\r
}\r
/*\r
.ol-layerswitcher .collapse-layers:before {\r
  content:"";\r
  position:absolute;\r
  border:0.5em solid #369;\r
  border-color: #369 transparent transparent;\r
  margin-top:0.25em;\r
}\r
.ol-layerswitcher .expend-layers:before {\r
  content:"";\r
  position:absolute;\r
  border:0.5em solid #369;\r
  border-color: transparent transparent transparent #369 ;\r
  margin-left:0.25em;\r
}\r
*/\r
\r
.ol-layerswitcher .layerswitcher-opacity {\r
  position:relative;\r
  border: 1px solid #369;\r
  height: 3px;\r
  width: 120px;\r
  margin:5px 1em 10px 7px;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  border-radius: 3px;\r
  background: #69c;\r
  background: -webkit-gradient(linear, left top, right top, from(rgba(0,60,136,0)), to(rgba(0,60,136,0.6)));\r
  background: linear-gradient(to right, rgba(0,60,136,0), rgba(0,60,136,0.6));\r
  cursor: pointer;\r
  -webkit-box-shadow: 1px 1px 1px rgba(0,0,0,0.5);\r
          box-shadow: 1px 1px 1px rgba(0,0,0,0.5);\r
}\r
\r
.ol-layerswitcher .layerswitcher-opacity .layerswitcher-opacity-cursor,\r
.ol-layerswitcher .layerswitcher-opacity .layerswitcher-opacity-cursor:before {\r
  position: absolute;\r
  width: 20px;\r
  height: 20px;\r
  top: 50%;\r
  left: 50%;\r
  background: rgba(0,60,136,0.5);\r
  border-radius: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
  transform: translate(-50%, -50%);\r
  z-index: 1;\r
}\r
.ol-layerswitcher .layerswitcher-opacity .layerswitcher-opacity-cursor:before {\r
  content: "";\r
  position: absolute;\r
  width: 50%;\r
  height: 50%;\r
}\r
.ol-touch .ol-layerswitcher .layerswitcher-opacity .layerswitcher-opacity-cursor {\r
  width: 26px;\r
  height: 26px;\r
}\r
\r
.ol-layerswitcher .layerswitcher-opacity-label { \r
  display:none;\r
  position: absolute;\r
  right: -2.5em;\r
  bottom: 5px;\r
  font-size: 0.8em;\r
}\r
.ol-layerswitcher .layerswitcher-opacity-label::after {\r
  content:"%";\r
}\r
\r
.ol-layerswitcher .layerswitcher-progress {\r
  display:block;\r
  margin:-4px 1em 2px 7px;\r
  width: 120px;\r
}\r
.ol-layerswitcher .layerswitcher-progress div {\r
  background-color: #369;\r
  height:2px;\r
  display:block;\r
  width:0;\r
}\r
\r
.ol-control.ol-layerswitcher-image {\r
  position: absolute;\r
  right: 0.5em;\r
  text-align: left;\r
  top: 1em;\r
  transition: all 0.2s ease 0s;\r
  -webkit-transition: all 0.2s ease 0s;\r
}\r
.ol-control.ol-layerswitcher-image.ol-collapsed {\r
  top:3em;\r
  -webkit-transition: none;\r
  transition: none;\r
}\r
\r
.ol-layerswitcher-image .panel {\r
  list-style: none;\r
  padding: 0.25em;\r
  margin:0;\r
  overflow: hidden;\r
}\r
\r
.ol-layerswitcher-image .panel ul {\r
  list-style: none;\r
  padding: 0 0 0 20px;\r
  overflow: hidden;\r
}\r
\r
.ol-layerswitcher-image.ol-collapsed .panel {\r
  display:none;\r
}\r
.ol-layerswitcher-image.ol-forceopen .panel {\r
  display:block;\r
  clear:both;\r
}\r
\r
.ol-layerswitcher-image button {\r
  float: right;\r
  display:none;\r
}\r
\r
.ol-layerswitcher-image.ol-collapsed button {\r
  display:block;\r
  position:relative;\r
}\r
\r
.ol-layerswitcher-image li {\r
  border-radius: 4px;\r
  border: 3px solid transparent;\r
  -webkit-box-shadow: 1px 1px 4px rgba(0, 0, 0, 0.5);\r
          box-shadow: 1px 1px 4px rgba(0, 0, 0, 0.5);\r
  display: inline-block;\r
  width: 64px;\r
  height: 64px;\r
  margin:2px;\r
  position: relative;\r
  background-color: #fff;\r
  overflow: hidden;\r
  vertical-align: middle;\r
  cursor:pointer;\r
}\r
.ol-layerswitcher-image li.ol-layer-hidden {\r
  opacity: 0.5;\r
  border-color:#555;\r
}\r
.ol-layerswitcher-image li.ol-header {\r
  display: none;\r
}\r
\r
.ol-layerswitcher-image li img {\r
  position:absolute;\r
  max-width:100%;\r
}\r
.ol-layerswitcher-image li.select,\r
.ol-layerswitcher-image li.ol-visible {\r
  border: 3px solid red;\r
}\r
\r
.ol-layerswitcher-image li p {\r
  display:none;\r
}\r
.ol-layerswitcher-image li:hover p {\r
  background-color: rgba(0, 0, 0, 0.5);\r
  color: #fff;\r
  bottom: 0;\r
  display: block;\r
  left: 0;\r
  margin: 0;\r
  overflow: hidden;\r
  position: absolute;\r
  right: 0;\r
  text-align: center;\r
  height:1.2em;\r
  font-family:Verdana, Geneva, sans-serif;\r
  font-size:0.8em;\r
}\r
.ol-control.ol-legend {\r
  bottom: .5em;\r
  left: .5em;\r
  z-index: 1;\r
  max-height: 90%;\r
  max-width: 90%;\r
  overflow-x: hidden;\r
  overflow-y: auto;\r
  background-color: rgba(255,255,255,.6);\r
}\r
.ol-control.ol-legend:hover {\r
  background-color: rgba(255,255,255,.8);\r
}\r
.ol-control.ol-legend.ol-empty,\r
.ol-control.ol-legend.ol-collapsed {\r
  overflow: hidden;\r
}\r
.ol-control.ol-legend button {\r
  position: relative;\r
  display: none;\r
}\r
.ol-control.ol-legend.ol-empty button,\r
.ol-control.ol-legend.ol-collapsed button {\r
  display: block;\r
}\r
.ol-control.ol-legend.ol-uncollapsible button {\r
  display: none;\r
}\r
\r
.ol-control.ol-legend > ul,\r
.ol-control.ol-legend > canvas {\r
  margin: 2px;\r
}\r
\r
.ol-control.ol-legend button.ol-closebox {\r
  display: block;\r
  position: absolute;\r
  top: 0;\r
  right: 0;\r
  background: none;\r
  cursor: pointer;\r
  z-index: 1;\r
}\r
.ol-control.ol-legend.ol-empty button.ol-closebox,\r
.ol-control.ol-legend.ol-uncollapsible button.ol-closebox,\r
.ol-control.ol-legend.ol-collapsed button.ol-closebox {\r
  display: none;\r
}\r
.ol-control.ol-legend button.ol-closebox:before {\r
  content: "\\D7";\r
  background: none;\r
  color: rgba(0,60,136,.5);\r
  font-size: 1.3em;\r
}\r
.ol-control.ol-legend button.ol-closebox:hover:before {\r
  color: rgba(0,60,136,1);\r
}\r
.ol-control.ol-legend .ol-legendImg {\r
  display: block;\r
}\r
.ol-control.ol-legend.ol-empty .ol-legendImg,\r
.ol-control.ol-legend.ol-collapsed .ol-legendImg {\r
  display: none;\r
}\r
.ol-control.ol-legend.ol-uncollapsible .ol-legendImg {\r
  display: block;\r
}\r
\r
.ol-control.ol-legend > button:first-child:before,\r
.ol-control.ol-legend > button:first-child:after {\r
  content: "";\r
  position: absolute;\r
  top: .25em;\r
  left: .2em;\r
  width: .2em;\r
  height: .2em;\r
  background-color: currentColor;\r
  -webkit-box-shadow: 0 0.35em, 0 0.7em;\r
          box-shadow: 0 0.35em, 0 0.7em;\r
}\r
.ol-control.ol-legend button:first-child:after {\r
  top: .27em;\r
  left: .55em;\r
  height: .15em;\r
  width: .6em;\r
}\r
\r
ul.ol-legend {\r
  position: absolute;\r
  top: 0;\r
  left: 0;\r
  width: 100%;\r
  margin: 0;\r
  padding: 0;\r
  list-style: none;\r
}\r
.ol-control.ol-legend.ol-empty ul,\r
.ol-control.ol-legend.ol-collapsed ul {\r
  display: none;\r
}\r
.ol-control.ol-legend.ol-uncollapsible ul {\r
  display: block;\r
}\r
ul.ol-legend li.ol-title {\r
  text-align: center;\r
  font-weight: bold;\r
}\r
ul.ol-legend li.ol-title > div:first-child {\r
  width: 0!important;\r
}\r
ul.ol-legend li {\r
  overflow: hidden;\r
  padding: 0;\r
  white-space: nowrap;\r
}\r
ul.ol-legend li div {\r
  display: inline-block;\r
  vertical-align: top;\r
}\r
\r
.ol-control.ol-legend .ol-legend {\r
  display: inline-block;\r
}\r
.ol-control.ol-legend.ol-empty .ol-legend,\r
.ol-control.ol-legend.ol-collapsed .ol-legend {\r
  display: none;\r
}\r
.ol-control.ol-legend.ol-empty button {\r
  opacity: .4;\r
}\r
.ol-control.ol-mapzone {\r
  position: absolute;\r
  right: 0.5em;\r
  text-align: left;\r
  top: .5em;\r
  max-height: calc(100% - 6em);\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  overflow: hidden;\r
}\r
\r
.ol-control.ol-mapzone.ol-collapsed {\r
  top: 3em;\r
}\r
\r
.ol-control.ol-mapzone button {\r
  position: relative;\r
  float: right;\r
  margin-top: 2.5em;\r
}\r
.ol-touch .ol-control.ol-mapzone button {\r
  margin-top: 1.67em;\r
}\r
.ol-control.ol-mapzone.ol-collapsed button {\r
  margin-top: 0;\r
}\r
\r
.ol-control.ol-mapzone button i {\r
  border: .1em solid currentColor;\r
  border-radius: 50%;\r
  width: .9em;\r
  height: .9em; \r
  overflow: hidden;\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
.ol-control.ol-mapzone button i:before {\r
  content: "";\r
  background-color: currentColor;\r
  width: 0.4em;\r
  height: .4em;\r
  position: absolute;\r
  left: .5em;\r
  top: 0.3em;\r
  border-radius: 50%;\r
  -webkit-box-shadow: .05em .3em 0 -.051em currentColor,\r
  	-.05em -.35em 0 -.1em currentColor,\r
  	-.5em -.35em 0 0em currentColor,\r
  	-.65em .1em 0 -.03em currentColor,\r
  	-.65em -.05em 0 -.05em currentColor;\r
          box-shadow: .05em .3em 0 -.051em currentColor,\r
  	-.05em -.35em 0 -.1em currentColor,\r
  	-.5em -.35em 0 0em currentColor,\r
  	-.65em .1em 0 -.03em currentColor,\r
  	-.65em -.05em 0 -.05em currentColor\r
}\r
\r
.ol-mapzone > div {\r
  position: relative;\r
  display: inline-block;\r
  width: 5em;\r
  height: 5em;\r
  margin: 0 .2em 0 0;\r
}\r
.ol-control.ol-mapzone.ol-collapsed > div {\r
  display: none;\r
}\r
.ol-mapzone > div p {\r
  margin: 0;\r
  position: absolute;\r
  bottom: 0;\r
  /* background: rgba(255,255,255,.5); */\r
  color: #fff;\r
  font-weight: bold;\r
  text-align: center;\r
  width: 160%;\r
  overflow: hidden;\r
  font-family: 'Lucida Grande',Verdana,Geneva,Lucida,Arial,Helvetica,sans-serif;\r
  -webkit-transform: scaleX(.625);\r
          transform: scaleX(.625);\r
  -webkit-transform-origin: 0 0;\r
          transform-origin: 0 0;\r
  cursor: default;\r
}\r
\r
.ol-notification {\r
  width: 150%;\r
  bottom: 0;\r
  border: 0;\r
  background: none;\r
  margin: 0;\r
  padding: 0;\r
}\r
.ol-notification > div,\r
.ol-notification > div:hover {\r
  position: absolute;\r
  background-color: rgba(0,0,0,.8);\r
  color: #fff;\r
  bottom: 0;\r
  left: 33.33%;\r
  max-width: calc(66% - 4em);\r
  min-width: 5em;\r
  max-height: 5em;\r
  min-height: 1em;\r
  border-radius: 4px 4px 0 0;\r
  padding: .2em .5em;\r
  text-align: center;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
  -webkit-transition: .3s;\r
  transition: .3s;\r
  opacity: 1;\r
}\r
.ol-notification.ol-collapsed > div {\r
  bottom: -5em;\r
  opacity: 0;\r
}\r
\r
.ol-notification a {\r
  color: #9cf;\r
  cursor: pointer;\r
}\r
\r
.ol-notification .ol-close,\r
.ol-notification .ol-close:hover {\r
  padding-right: 1.5em;\r
}\r
\r
.ol-notification .closeBox {\r
  position: absolute;\r
  top: 0;\r
  right: 0.3em;\r
}\r
.ol-notification .closeBox:before {\r
  content: '\\00d7';\r
}\r
.ol-overlay {\r
  position: absolute;\r
  top: 0;\r
  left: 0;\r
  width:100%;\r
  height: 100%;\r
  background-color: rgba(0,0,0,0.4);\r
  padding: 1em;\r
  color: #fff;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  z-index: 1;\r
  opacity: 0;\r
  display: none;\r
  cursor: default;\r
  overflow: hidden;\r
  -webkit-transition: all 0.5s;\r
  transition: all 0.5s;\r
  pointer-events: none;\r
  z-index: 9;\r
}\r
\r
.ol-overlay.slide-up {\r
  transform: translateY(100%);\r
  -webkit-transform: translateY(100%);\r
}\r
.ol-overlay.slide-down {\r
  -webkit-transform: translateY(-100%);\r
  transform: translateY(-100%);\r
}\r
.ol-overlay.slide-left\r
{	-webkit-transform: translateX(-100%);\r
  transform: translateX(-100%);\r
}\r
.ol-overlay.slide-right {\r
  -webkit-transform: translateX(100%);\r
  transform: translateX(100%);\r
}\r
.ol-overlay.zoom {\r
  top: 50%;\r
  left: 50%;\r
  opacity:0.5;\r
  -webkit-transform: translate(-50%,-50%) scale(0);\r
  transform: translate(-50%,-50%) scale(0);\r
}\r
.ol-overlay.zoomout {\r
  -webkit-transform: scale(3);\r
  transform: scale(3);\r
}\r
.ol-overlay.zoomrotate {\r
  top: 50%;\r
  left: 50%;\r
  opacity:0.5;\r
  -webkit-transform: translate(-50%,-50%) scale(0) rotate(360deg);\r
  transform: translate(-50%,-50%) scale(0) rotate(360deg);\r
}\r
.ol-overlay.stretch {\r
  top: 50%;\r
  left: 50%;\r
  opacity:0.5;\r
  -webkit-transform: translate(-50%,-50%) scaleX(0);\r
  transform: translate(-50%,-50%) scaleX(0) ;\r
}\r
.ol-overlay.stretchy {\r
  top: 50%;\r
  left: 50%;\r
  opacity:0.5;\r
  -webkit-transform: translate(-50%,-50%) scaleY(0);\r
  transform: translate(-50%,-50%) scaleY(0) ;\r
}\r
.ol-overlay.wipe {\r
  opacity: 1;\r
  /* clip: must be set programmatically */\r
  /* clip-path: use % but not crossplatform (IE) */\r
}\r
.ol-overlay.flip {\r
  -webkit-transform: perspective(600px) rotateY(180deg);\r
  transform: perspective(600px) rotateY(180deg);\r
}\r
.ol-overlay.card {\r
  opacity: 0.5;\r
  -webkit-transform: translate(-80%, 100%) rotate(-0.5turn);\r
  transform: translate(-80%, 100%) rotate(-0.5turn);\r
}\r
.ol-overlay.book {\r
  -webkit-transform: perspective(600px) rotateY(-180deg) scaleX(0.6);\r
  transform: perspective(600px) rotateY(-180deg) scaleX(0.6) ;\r
  -webkit-transform-origin: 10% 50%;\r
  transform-origin: 10% 50%;\r
}\r
.ol-overlay.book.visible {\r
  -webkit-transform-origin: 10% 50%;\r
  transform-origin: 10% 50%;\r
}\r
\r
.ol-overlay.ol-visible {\r
  opacity:1;\r
  top: 0;\r
  left: 0;\r
  right: 0;\r
  bottom: 0;\r
  -webkit-transform: none;\r
  transform: none;\r
  pointer-events: all;  \r
}\r
\r
.ol-overlay .ol-closebox {\r
  position: absolute;\r
  top: 1em;\r
  right: 1em;\r
  width: 1em;\r
  height: 1em;\r
  cursor: pointer;\r
  z-index:1;\r
}\r
.ol-overlay .ol-closebox:before {\r
  content: "\\274c";\r
  display: block;\r
  text-align: center;\r
  vertical-align: middle;\r
}\r
\r
.ol-overlay .ol-fullscreen-image {\r
  position: absolute;\r
  top: 0;\r
  left: 0;\r
  bottom: 0;\r
  right: 0;\r
}\r
.ol-overlay .ol-fullscreen-image img {\r
  position: absolute;\r
  max-width: 100%;\r
  max-height: 100%;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  padding: 1em;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
  transform: translate(-50%, -50%);\r
}\r
.ol-overlay .ol-fullscreen-image.ol-has-title img {\r
  padding-bottom: 3em;\r
}\r
.ol-overlay .ol-fullscreen-image p {\r
  background-color: rgba(0,0,0,.5);\r
  padding: .5em;\r
  position: absolute;\r
  left: 0;\r
  right: 0;\r
  bottom: 0;\r
  margin: 0;\r
  text-align: center;\r
}\r
.ol-control.ol-overview\r
{	position: absolute;\r
	left: 0.5em;\r
	text-align: left;\r
	bottom: 0.5em;\r
}\r
\r
.ol-control.ol-overview .panel\r
{	display:block;\r
	width:150px;\r
	height:150px;\r
	margin:2px;\r
	background-color:#fff;\r
	border:1px solid #369;\r
	cursor: pointer;\r
}\r
\r
.ol-overview:not(.ol-collapsed) button\r
{	position:absolute;\r
	bottom:2px;\r
	left:2px;\r
	z-index:2;\r
}\r
\r
.ol-control.ol-overview.ol-collapsed .panel\r
{	display:none;\r
}\r
\r
.ol-overview.ol-collapsed button:before\r
{	content:'\\00bb';\r
}\r
.ol-overview button:before\r
{	content:'\\00ab';\r
}\r
\r
\r
.ol-control-right.ol-overview\r
{	left: auto;\r
	right: 0.5em;\r
}\r
.ol-control-right.ol-overview:not(.ol-collapsed) button\r
{	left:auto;\r
	right:2px;\r
}\r
.ol-control-right.ol-overview.ol-collapsed button:before\r
{	content:'\\00ab';\r
}\r
.ol-control-right.ol-overview button:before\r
{	content:'\\00bb';\r
}\r
\r
.ol-control-top.ol-overview\r
{	bottom: auto;\r
	top: 5em;\r
}\r
.ol-control-top.ol-overview:not(.ol-collapsed) button\r
{	bottom:auto;\r
	top:2px;\r
}\r
\r
.ol-permalink {\r
  position: absolute;\r
  top:0.5em;\r
  right: 2.5em;\r
}\r
.ol-touch .ol-permalink {\r
  right: 3em;\r
}\r
\r
.ol-permalink button i {\r
  position: absolute;\r
  width: 1em;\r
  height: 1em;\r
  display: block;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
.ol-permalink button i:before {\r
  content: '\\2197';\r
  position: absolute;\r
  border: 1px solid currentColor;\r
  left: 0;\r
  top: 0;\r
  width: 0.3em;\r
  height: 1em;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  border-width: 1px 0 0 1px;\r
  padding: 0 0.2em;\r
}\r
.ol-permalink button i:after {\r
  content: '';\r
  position: absolute;\r
  border: 1px solid currentColor;\r
  right: 0;\r
  bottom: 0;\r
  width: 1em;\r
  height: 0.3em;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  border-width: 0 1px 1px 0;\r
  padding: 0.2em;\r
}\r
.ol-control.ol-print {\r
  top:.5em;\r
  left: 3em;\r
}\r
.ol-control.ol-print button:before {\r
  content: "";\r
  width: .9em;\r
  height: .35em;\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
  -webkit-box-shadow: inset 0 0 0 0.1em, inset 0.55em 0, 0 0.2em 0 -0.1em;\r
          box-shadow: inset 0 0 0 0.1em, inset 0.55em 0, 0 0.2em 0 -0.1em;\r
}\r
.ol-control.ol-print button:after {\r
  content: "";\r
  width: .7em;\r
  height: .6em;\r
  position: absolute;\r
  left: 50%;\r
  top: 25%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
  -webkit-box-shadow: inset 0 0 0 0.15em;\r
          box-shadow: inset 0 0 0 0.15em;\r
}\r
.ol-ext-print-dialog {\r
  width: 100%;\r
  height: 100%;\r
}\r
.ol-ext-print-dialog > form .ol-closebox {\r
  right: auto;\r
  left: 16.5em;\r
  z-index: 1;\r
  color: #999;\r
}\r
.ol-ext-print-dialog .ol-content[data-status="printing"] {\r
  opacity: .5;\r
}\r
.ol-ext-print-dialog .ol-content .ol-error {\r
  display: none;\r
  background: #b00;\r
  color: yellow;\r
  text-align: center;\r
  padding: 1em .5em;\r
  font-weight: bold;\r
  margin: 0 -1em;\r
}\r
.ol-ext-print-dialog .ol-content[data-status="error"] .ol-error {\r
  display: block;\r
}\r
\r
\r
.ol-ext-print-dialog > form,\r
.ol-ext-print-dialog.ol-visible > form {\r
  -webkit-transition: none;\r
  transition: none;\r
  top: 1em;\r
  left: 1em;\r
  bottom: 1em;\r
  right: 1em;\r
  -webkit-transform: none;\r
          transform: none;\r
  max-width: 100%;\r
  max-height: 100%;\r
  background-color: #eee;\r
  padding: 0;\r
}\r
.ol-ext-print-dialog .ol-print-map {\r
  position: absolute;\r
  top: 0;\r
  bottom: 0;\r
  right: 0;\r
  width: calc(100% - 18em);\r
  overflow: hidden;\r
}\r
.ol-ext-print-dialog .ol-print-map .ol-page {\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  background: #fff;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
.ol-ext-print-dialog .ol-print-map .ol-page.margin {\r
  -webkit-box-sizing: content-box;\r
          box-sizing: content-box;\r
}\r
.ol-ext-print-dialog .ol-map {\r
  width: 100%;\r
  height: 100%;\r
}\r
.ol-ext-print-dialog .ol-print-map .ol-control {\r
  display: none!important;\r
}\r
\r
.ol-ext-print-dialog .ol-print-param {\r
  position: absolute;\r
  overflow-x: hidden;\r
  top: 0;\r
  bottom: 0;\r
  left: 0;\r
  width: 18em;\r
  background-color: #fff;\r
  padding: 1em;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
\r
.ol-ext-print-dialog .ol-print-param h2 {\r
  display: block;\r
  color: rgba(0,60,136,.7);\r
  font-size: 1.1em;\r
}\r
.ol-ext-print-dialog .ol-print-param ul {\r
  padding: 0;\r
  list-style: none;\r
}\r
.ol-ext-print-dialog .ol-print-param li {\r
  position: relative;\r
  margin: .5em 0;\r
  font-size: .9em;\r
}\r
.ol-ext-print-dialog .ol-print-param li.hidden {\r
  display: none;\r
}\r
.ol-ext-print-dialog .ol-print-param label {\r
  width: 8em;\r
  display: inline-block;\r
  vertical-align: middle;\r
}\r
\r
.ol-ext-print-dialog select {\r
  outline: none;\r
  vertical-align: middle;\r
}\r
\r
.ol-ext-print-dialog .ol-orientation {\r
  text-align: center;\r
}\r
.ol-ext-print-dialog .ol-orientation label {\r
  position: relative;\r
  width: 7em;\r
  cursor: pointer;\r
}\r
.ol-ext-print-dialog .ol-orientation input {\r
  position: absolute;\r
  opacity: 0;\r
  width: 0;\r
  height: 0;\r
}\r
.ol-ext-print-dialog .ol-orientation span {\r
  position: relative;\r
  width: 80%;\r
  display: block;\r
  padding: 3.5em 0 .2em;\r
}\r
.ol-ext-print-dialog .ol-orientation span:before {\r
  content: "";\r
  position: absolute;\r
  width: 2em;\r
  height: 2.6em;\r
  bottom: 1.5em;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
  color: #333;\r
  background-color: currentColor;\r
  border: 1px solid currentColor;\r
  border-radius: 0 1em 0 0;\r
  opacity: .5;\r
  overflow: hidden;\r
  -webkit-box-shadow: inset 1.3em -1.91em #ddd;\r
          box-shadow: inset 1.3em -1.91em #ddd;\r
}\r
\r
.ol-ext-print-dialog .ol-orientation .landscape span:before {\r
  width: 2.6em;\r
  height: 2em;\r
  margin: .2em 0;\r
  -webkit-box-shadow: inset 1.91em -1.3em #ddd;\r
          box-shadow: inset 1.91em -1.3em #ddd;\r
}\r
.ol-ext-print-dialog .ol-orientation input:checked + span {\r
  opacity: 1;\r
  -webkit-box-shadow: 0 0 .2em rgba(0,0,0,.5);\r
          box-shadow: 0 0 .2em rgba(0,0,0,.5);\r
}\r
\r
.ol-ext-print-dialog .ol-ext-toggle-switch span {\r
  position: absolute;\r
  right: -2em;\r
  top: 50%;\r
  -webkit-transform: translateY(-50%);\r
          transform: translateY(-50%);\r
}\r
\r
.ol-print-title input[type=text] {\r
  margin-top: .5em;\r
  width: calc(100% - 6em);\r
  margin-left: 6em;\r
}\r
\r
.ol-ext-print-dialog .ol-size option:first-child {\r
  font-style: italic;\r
}\r
\r
.ol-ext-print-dialog .ol-saveas,\r
.ol-ext-print-dialog .ol-savelegend {\r
  text-align: center;\r
}\r
.ol-ext-print-dialog .ol-saveas select,\r
.ol-ext-print-dialog .ol-savelegend select {\r
  background-color: rgba(0,60,136,.7);\r
  color: #fff;\r
  padding: .5em;\r
  margin: 1em 0 0;\r
  font-size: 1em;\r
  border: 0;\r
  font-weight: bold;\r
  max-width: 12em;\r
}\r
.ol-ext-print-dialog .ol-saveas select option,\r
.ol-ext-print-dialog .ol-savelegend select option {\r
  background-color: #fff;\r
  color: #666;\r
}\r
.ol-ext-print-dialog .ol-savelegend select {\r
  margin-top: 0;\r
}\r
\r
.ol-ext-print-dialog .ol-ext-buttons {\r
  text-align: right;\r
  border-top: 1px solid #ccc;\r
  padding: .8em .5em;\r
  margin: 0 -1em;\r
}\r
.ol-ext-print-dialog button {\r
  font-size: 1em;\r
  margin: 0 .2em;\r
  border: 1px solid #999;\r
  background: none;\r
  padding: .3em 1em;\r
  color: #333;\r
}\r
.ol-ext-print-dialog button[type="submit"] {\r
  background-color: rgba(0,60,136,.7);\r
  color: #fff;\r
  font-weight: bold;\r
}\r
\r
.ol-ext-print-dialog .ol-clipboard-copy {\r
  position: absolute;\r
  pointer-events: none;\r
  top: 0;\r
  background-color: rgba(0,0,0,.5);\r
  color: #fff;\r
  padding: .5em 1em;\r
  border-radius: 1em;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
  width: -webkit-fit-content;\r
  width: -moz-fit-content;\r
  width: fit-content;\r
  -webkit-transition: 0s;\r
  transition: 0s;\r
  opacity: 0;\r
}\r
.ol-ext-print-dialog .ol-clipboard-copy.visible {\r
  -webkit-animation: 1s ol-clipboard-copy;\r
          animation: 1s ol-clipboard-copy;\r
}\r
.ol-ext-print-dialog .ol-print-map .ol-control.ol-canvas-control {\r
  display: block!important;\r
}\r
.ol-ext-print-dialog .ol-print-map .ol-control.ol-print-compass {\r
  display: block!important;\r
}\r
.ol-ext-print-dialog .ol-print-map .ol-control.olext-print-compass {\r
  top: 0;\r
  right: 0;\r
  width: 60px;\r
  height: 60px;\r
  margin: 20px;\r
}\r
\r
@-webkit-keyframes ol-clipboard-copy { \r
  0% { opacity: 0; top: 0; }\r
  80% { opacity: 1; top: -3em; }\r
  100% { opacity: 0; top: -3em; }  \r
}\r
\r
@keyframes ol-clipboard-copy { \r
  0% { opacity: 0; top: 0; }\r
  80% { opacity: 1; top: -3em; }\r
  100% { opacity: 0; top: -3em; }  \r
}\r
\r
@media print {\r
  body.ol-print-document {\r
    margin: 0!important;\r
    padding: 0!important;\r
  }\r
  body.ol-print-document > * {\r
    display: none!important;\r
  }\r
  body.ol-print-document > .ol-ext-print-dialog {\r
    display: block!important;\r
  }\r
  body.ol-print-document > .ol-ext-print-dialog .ol-content {\r
    max-height: unset!important;\r
    max-width: unset!important;\r
    width: unset!important;\r
    height: unset!important;\r
  }\r
  .ol-ext-print-dialog > form,\r
  .ol-ext-print-dialog {\r
    position: unset;\r
    -webkit-box-shadow: none;\r
            box-shadow: none;\r
    background: none!important;\r
    border: 0;\r
  }\r
  .ol-ext-print-dialog > form > *,\r
  .ol-ext-print-dialog .ol-print-param {\r
    display: none!important;\r
    background: none;\r
  } \r
  .ol-ext-print-dialog .ol-content {\r
    display: block!important;\r
    border: 0;\r
    background: none;\r
  }\r
  .ol-ext-print-dialog .ol-print-map {\r
    position: unset; \r
    background: none;\r
    width: auto;\r
    overflow: visible;\r
  }\r
  .ol-ext-print-dialog .ol-print-map .ol-page {\r
    -webkit-transform: none!important;\r
            transform: none!important;\r
    -webkit-box-shadow: none!important;\r
            box-shadow: none!important;\r
    position: unset;\r
  }\r
}\r
\r
@media (max-width: 25em) {\r
  .ol-ext-print-dialog .ol-print-param {\r
    width: 13em;\r
  }\r
  .ol-ext-print-dialog .ol-print-map {\r
    width: calc(100% - 13em);\r
  }\r
  .ol-ext-print-dialog .ol-print-param .ol-print-title input[type="text"] {\r
    width: 100%;\r
    margin: 0;\r
  }\r
}\r
.ol-ext-print-dialog2x .ol-map,\r
.ol-ext-print-dialog2x .ol-map2 {\r
  width: 100%;\r
  height: 50%;\r
}\r
.ol-ext-print-dialog2x[data-orientation="landscape"] .ol-map,\r
.ol-ext-print-dialog2x[data-orientation="landscape"] .ol-map2 {\r
  display: inline-block;\r
  width: 50%;\r
  height: 100%;\r
}\r
\r
.ol-ext-print-dialog2x[data-clip-map] .ol-map,\r
.ol-ext-print-dialog2x[data-swipe-orientation] .ol-map {\r
  width: 100%!important;\r
  height: 100%!important;\r
}\r
.ol-ext-print-dialog2x[data-clip-map] .ol-map2,\r
.ol-ext-print-dialog2x[data-swipe-orientation] .ol-map2 {\r
  width: 100%!important;\r
  height: 100%!important;\r
  position: relative!important;\r
  left: 0;\r
  top: -100%;\r
}\r
.ol-profile {\r
  position: relative;\r
  -webkit-user-select: none;\r
     -moz-user-select: none;\r
      -ms-user-select: none;\r
          user-select: none;\r
}\r
.ol-control.ol-profile {\r
  position: absolute;\r
  top: 0.5em;\r
  right: 3em;\r
  text-align: right;\r
  overflow: hidden;\r
}\r
.ol-profile .ol-zoom-out {\r
  position: absolute;\r
  top: 10px;\r
  right: 10px;\r
  width: 1em;\r
  height: 1em;\r
  padding: 0;\r
  border: 1px solid #000;\r
  border-radius: 2px;\r
  cursor: pointer;\r
}\r
.ol-profile .ol-zoom-out:before {\r
  content: '';\r
  height: 2px;\r
  width: 60%;\r
  background: currentColor;\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
\r
.ol-profile .ol-inner  {\r
  position: relative;\r
  padding: 0.5em;\r
  font-size: 0.8em;\r
}\r
.ol-control.ol-profile .ol-inner {\r
  display: block;\r
  background-color: rgba(255,255,255,0.7);\r
  margin: 2.3em 2px 2px;\r
}\r
.ol-control.ol-profile.ol-collapsed .ol-inner {\r
  display: none;\r
}\r
\r
.ol-profile canvas {\r
  display: block;\r
}\r
.ol-profile button {\r
  display: block;\r
  position: absolute;\r
  right: 0;\r
  overflow: hidden;\r
}\r
.ol-profile button i {\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  width: 1em;\r
  height: 1em;\r
  overflow: hidden;\r
}\r
.ol-profile button i:before,\r
.ol-profile button i:after {\r
  content: "";\r
  position: absolute;\r
  display: block;\r
  background-color: currentColor;\r
  width: 1em;\r
  height: .9em;\r
  -webkit-transform: scaleX(.8) translate(-.25em, .5em) rotate(45deg);\r
          transform: scaleX(.8) translate(-.25em, .5em) rotate(45deg);\r
}\r
.ol-profile button i:after {\r
  -webkit-transform: scaleX(.8) translate(.35em, .7em) rotate(45deg);\r
          transform: scaleX(.8) translate(.35em, .7em) rotate(45deg);\r
}\r
\r
.ol-profile.ol-collapsed button {\r
  position: static;\r
}\r
\r
.ol-profile .ol-profilebar,\r
.ol-profile .ol-profilecursor {\r
  position:absolute;\r
  pointer-events: none;\r
  width: 1px;\r
  display: none;\r
}\r
.ol-profile .ol-profilecursor {\r
  width: 0;\r
  height: 0;\r
}\r
.ol-profile .ol-profilecursor:before {\r
  content:"";\r
  pointer-events: none;\r
  display: block;\r
  margin: -2px;\r
  width:5px;\r
  height:5px;\r
}\r
.ol-profile .ol-profilebar,\r
.ol-profile .ol-profilecursor:before {\r
  background: red;\r
}\r
\r
.ol-profile table {\r
  text-align: center;\r
  width: 100%;\r
}\r
\r
.ol-profile table span {\r
  display: block;\r
}\r
\r
.ol-profilepopup {\r
  background-color: rgba(255, 255, 255, 0.5);\r
  margin: 0.5em;\r
  padding: 0 0.5em;\r
  position: absolute;\r
  top:-1em;\r
  white-space: nowrap;\r
}\r
.ol-profilepopup.ol-left {\r
  right:0;\r
}\r
\r
\r
.ol-profile table td {\r
  padding: 0 2px;\r
}\r
\r
.ol-profile table .track-info {\r
  display: table-row;\r
}\r
.ol-profile table .point-info {\r
  display: none;\r
}\r
.ol-profile .over table .track-info {\r
  display: none;\r
}\r
.ol-profile .over table .point-info {\r
  display: table-row;\r
}\r
\r
.ol-profile p {\r
  text-align: center;\r
  margin:0;\r
}\r
\r
.ol-control.ol-progress-bar {\r
  position: absolute;\r
  top: 0;\r
  bottom: 0;\r
  left: 0;\r
  right: 0;\r
  padding: 0;\r
  pointer-events: none!important;\r
  background-color: transparent;\r
}\r
\r
.ol-control.ol-progress-bar > .ol-bar {\r
  position: absolute;\r
  background-color: rgba(0,60,136,.5);\r
  left: 0;\r
  bottom: 0;\r
  height: .5em;\r
  width: 0;\r
  -webkit-transition: width .2s;\r
  transition: width .2s;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
\r
.ol-progress-bar > .ol-waiting {\r
  display: none;\r
}\r
\r
.ol-viewport .ol-control.ol-progress-bar > .ol-waiting {\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  color: #fff;\r
  font-size: 2em;\r
  display: block;\r
  -webkit-animation: 1s linear infinite ol-progress-bar-blink;\r
          animation: 1s linear infinite ol-progress-bar-blink;\r
}\r
\r
@-webkit-keyframes ol-progress-bar-blink {\r
  0%, 30% {\r
    visibility: hidden;\r
  }\r
  100% {\r
    visibility: visible;\r
  }\r
}\r
\r
@keyframes ol-progress-bar-blink {\r
  0%, 30% {\r
    visibility: hidden;\r
  }\r
  100% {\r
    visibility: visible;\r
  }\r
}\r
\r
.ol-control.ol-routing {\r
  top: 0.5em;\r
  left: 3em;\r
  max-height: 90%;\r
  overflow-y: auto;\r
}\r
.ol-touch .ol-control.ol-routing {\r
  left: 3.5em;\r
}\r
.ol-control.ol-routing.ol-searching {\r
  opacity: .5;\r
}\r
\r
.ol-control.ol-routing .ol-car,\r
.ol-control.ol-routing > button {\r
  position: relative;\r
}\r
.ol-control.ol-routing .ol-car:after,\r
.ol-control.ol-routing > button:after {\r
  content: "";\r
  position: absolute;\r
  width: .78em;\r
  height: 0.6em;\r
  border-radius: 40% 50% 0 0 / 50% 70% 0 0;\r
  -webkit-box-shadow: inset 0 0 0 0.065em, -0.35em 0.14em 0 -0.09em, inset 0 -0.37em, inset -0.14em 0.005em;\r
          box-shadow: inset 0 0 0 0.065em, -0.35em 0.14em 0 -0.09em, inset 0 -0.37em, inset -0.14em 0.005em;\r
  clip: rect(0 1em .5em -1em);\r
  top: .35em;\r
  left: .4em;\r
}\r
.ol-control.ol-routing .ol-car:before,\r
.ol-control.ol-routing > button:before {\r
  content: "";\r
  position: absolute;\r
  width: .28em;\r
  height: .28em;\r
  border-radius: 50%;\r
  -webkit-box-shadow: inset 0 0 0 1em, 0.65em 0;\r
          box-shadow: inset 0 0 0 1em, 0.65em 0;\r
  top: 0.73em;\r
  left: .20em;\r
}\r
.ol-control.ol-routing .ol-pedestrian:after {\r
  content: "";\r
  position: absolute;\r
  width: .3em;\r
  height: .4em;\r
  top: .25em;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
  -webkit-box-shadow: inset 0.3em 0, 0.1em 0.5em 0 -0.1em, -0.1em 0.5em 0 -0.1em, 0.25em 0.1em 0 -0.1em, -0.25em 0.1em 0 -0.1em;\r
          box-shadow: inset 0.3em 0, 0.1em 0.5em 0 -0.1em, -0.1em 0.5em 0 -0.1em, 0.25em 0.1em 0 -0.1em, -0.25em 0.1em 0 -0.1em;\r
  border-top: .2em solid transparent;\r
}\r
.ol-control.ol-routing .ol-pedestrian:before {\r
  content: "";\r
  position: absolute;\r
  width: .3em;\r
  height: .3em;\r
  top: .1em;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
  border-radius: 50%;\r
  background-color: currentColor;\r
}\r
\r
.ol-control.ol-routing .content {\r
  margin: .5em;\r
}\r
\r
.ol-control.ol-routing.ol-collapsed .content {\r
  display: none;\r
}\r
\r
.ol-routing .ol-search.ol-collapsed ul {\r
	display: none;\r
}\r
.ol-routing .ol-search ul .copy {\r
  display: none;\r
}\r
.ol-routing .ol-search ul.history {\r
  /* display: none; */\r
}\r
.ol-routing .content .search-input > div > * {\r
  display: inline-block;\r
  vertical-align: top;\r
}\r
.ol-routing .ol-result ul {\r
  list-style: none;\r
  display: block;\r
}\r
.ol-routing .ol-result li {\r
  position: relative;\r
  min-height: 1.65em;\r
}\r
.ol-routing .ol-result li i {\r
  display: block;\r
  font-size: .8em;\r
  font-weight: bold;\r
}\r
\r
.ol-routing .ol-result li:before {\r
  content: "";\r
  border: 5px solid transparent;\r
  position: absolute;\r
  left: -1.75em;\r
  border-bottom-color: #369;\r
  border-width: .6em .4em .6em;\r
  -webkit-transform-origin: 50% 125%;\r
          transform-origin: 50% 125%;\r
  -webkit-box-shadow: 0 0.65em 0 -0.25em #369;\r
          box-shadow: 0 0.65em 0 -0.25em #369;\r
  top: -.8em;\r
}\r
.ol-routing .ol-result li:after {\r
  content: "";\r
  position: absolute;\r
  width: 0.25em;\r
  height: .6em;\r
  left: -1.5em;\r
  background: #369;\r
  top: .6em;\r
  border-radius: 0.1em;\r
}\r
.ol-routing .ol-result li.R:before {\r
  -webkit-transform: rotate(90deg);\r
          transform: rotate(90deg);\r
}\r
.ol-routing .ol-result li.FR:before {\r
  -webkit-transform: rotate(45deg);\r
          transform: rotate(45deg);\r
}\r
.ol-routing .ol-result li.L:before {\r
  -webkit-transform: rotate(-90deg);\r
          transform: rotate(-90deg);\r
}\r
.ol-routing .ol-result li.FL:before {\r
  -webkit-transform: rotate(-45deg);\r
          transform: rotate(-45deg);\r
}\r
\r
.ol-routing .content > i {\r
  vertical-align: middle;\r
  margin: 0 .3em 0 .1em;\r
  font-style: normal;\r
}\r
.ol-routing .ol-button,\r
.ol-routing .ol-button:focus,\r
.ol-routing .ol-pedestrian,\r
.ol-routing .ol-car {\r
  font-size: 1.1em;\r
  position: relative;\r
  display: inline-block;\r
  width: 1.4em;\r
  height: 1.4em;\r
  color: rgba(0,60,136,1);\r
  background-color: transparent;\r
  margin: 0 .1em;\r
  opacity: .5;\r
  vertical-align: middle;\r
  outline: none;\r
  cursor: pointer;\r
}\r
.ol-routing .ol-button:hover,\r
.ol-routing .ol-button.selected,\r
.ol-routing i.selected {\r
  opacity: 1;\r
  background: transparent;\r
}\r
\r
.ol-control.ol-routing {\r
  background-color: rgba(255,255,255,.25);\r
}\r
.ol-control.ol-routing:hover {\r
  background-color: rgba(255,255,255,.75);\r
}\r
\r
.search-input > div > button:before {\r
  content: '\\00b1';\r
}\r
.ol-viewport .ol-scale {\r
	left: .5em;\r
	bottom: 2.5em;\r
	text-align: center;\r
	-webkit-transform: scaleX(.8);\r
	-webkit-transform-origin: 0 0;\r
	transform: scaleX(.8);\r
	transform-origin: 0 0;\r
	background-color: rgba(255, 255, 255, 0.75);\r
}\r
.ol-viewport .ol-scale input {\r
	background: none;\r
    border: 0;\r
    width: 8em;\r
    text-align: center;\r
}\r
\r
.ol-search{\r
  top: 0.5em;\r
  left: 3em;\r
}\r
.ol-touch .ol-search {\r
  left: 3.5em;\r
}\r
.ol-search button {\r
  top: 2px;\r
  left: 2px;\r
  float: left;\r
}\r
.ol-control.ol-search > button:before {\r
  content: "";\r
  position: absolute;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  width: .7em;\r
  height: .7em;\r
  background-color: transparent;\r
  border: .12em solid currentColor;\r
  border-radius: 100%;\r
  top: .35em;\r
  left: .35em;\r
}\r
.ol-control.ol-search > button:after {\r
  content: "";\r
  position: absolute;\r
  top: 1.1em;\r
  left: .95em;\r
  width: .45em;\r
  height: .15em;\r
  background-color: currentColor;\r
  border-radius: .05em;\r
  -webkit-transform: rotate(45deg);\r
          transform: rotate(45deg);\r
  -webkit-box-shadow: -0.18em 0 0 -0.03em;\r
          box-shadow: -0.18em 0 0 -0.03em;\r
}\r
\r
.ol-search button.ol-revers {\r
  float: none;\r
  background-image: none;\r
  display: inline-block;\r
  vertical-align: bottom;\r
  position: relative;\r
  top: 0;\r
  left: 0;\r
}\r
.ol-search.ol-revers button.ol-revers {\r
  background-color: rgba(0,136,60,.5)\r
}\r
\r
.ol-control.ol-search.ol-collapsed button.ol-revers {\r
  display: none;\r
}\r
.ol-search button.ol-revers:before {\r
  content: "";\r
  border: .1em solid currentColor;\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  -webkit-transform: translate(-50%,-50%);\r
          transform: translate(-50%,-50%);\r
  border-radius: 50%;\r
  width: .55em;\r
  height: .55em;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
.ol-search button.ol-revers:after {\r
  content: "";\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  -webkit-transform: translate(-50%,-50%);\r
          transform: translate(-50%,-50%);\r
  width: .2em;\r
  height: .2em;\r
  background-color: transparent;\r
  -webkit-box-shadow: .35em 0 currentColor, 0 .35em currentColor, -.35em 0 currentColor, 0 -.35em currentColor;\r
          box-shadow: .35em 0 currentColor, 0 .35em currentColor, -.35em 0 currentColor, 0 -.35em currentColor;\r
}\r
\r
.ol-search input {\r
  display: inline-block;\r
  border: 0;\r
  margin: 1px 1px 1px 2px;\r
  font-size: 1.14em;\r
  padding-left: 0.3em;\r
  height: 1.375em;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  -webkit-transition: all 0.1s;\r
  transition: all 0.1s;\r
}\r
.ol-touch .ol-search input,\r
.ol-touch .ol-search ul {\r
  font-size: 1.5em;\r
}\r
.ol-search.ol-revers > ul,\r
.ol-control.ol-search.ol-collapsed > * {\r
  display: none;\r
}\r
.ol-control.ol-search.ol-collapsed > button {\r
  display: block;\r
}\r
\r
.ol-search ul {\r
  list-style: none;\r
  padding: 0;\r
  margin: 0;\r
  display: block;\r
  clear: both;\r
  cursor: pointer;\r
  max-width: 17em;\r
  overflow-x: hidden;\r
  z-index: 1;\r
  background: #fff;\r
}\r
/*\r
.ol-control.ol-search ul {\r
  position: absolute;\r
  box-shadow: 5px 5px 5px rgba(0,0,0,0.5);\r
}\r
*/\r
.ol-search ul li {\r
  padding: 0.1em 0.5em;\r
  white-space: nowrap;\r
  overflow: hidden;\r
  text-overflow: ellipsis;\r
}\r
.ol-search ul li.select,\r
.ol-search ul li:hover {\r
  background-color: rgba(0,60,136,.5);\r
  color: #fff;\r
}\r
.ol-search ul li img {\r
  float: left;\r
  max-height: 2em;\r
}\r
.ol-search li.copy {\r
    background: rgba(0,0,0,.5);\r
  color: #fff;\r
}\r
.ol-search li.copy a {\r
  color: #fff;\r
  text-decoration: none;\r
}\r
\r
.ol-search.searching:before {\r
  content: '';\r
  position: absolute;\r
  height: 3px;\r
  left: 0;\r
  top: 1.6em;\r
  -webkit-animation: pulse .5s infinite alternate linear;\r
          animation: pulse .5s infinite alternate linear;\r
  background: red;\r
  z-index: 2;\r
}\r
\r
@-webkit-keyframes pulse {\r
  0% { left:0; right: 95%; }\r
  50% {	left: 30%; right: 30%; }\r
  100% {	left: 95%; right: 0; }\r
}\r
\r
@keyframes pulse {\r
  0% { left:0; right: 95%; }\r
  50% {	left: 30%; right: 30%; }\r
  100% {	left: 95%; right: 0; }\r
}\r
\r
\r
.ol-control.ol-search.IGNF-parcelle {\r
  background-color: rgba(255,255,255,.7);\r
  padding-bottom: .2em;\r
}\r
.ol-search.IGNF-parcelle input {\r
  width: 14.5em;\r
}\r
.ol-search.IGNF-parcelle input:-moz-read-only {\r
  background: #ccc;\r
  opacity: .8;\r
}\r
.ol-search.IGNF-parcelle input:read-only {\r
  background: #ccc;\r
  opacity: .8;\r
}\r
.ol-search.IGNF-parcelle.ol-collapsed-list > ul.autocomplete {\r
  display: none;\r
}\r
\r
.ol-search.IGNF-parcelle label {\r
  display: block;\r
  clear: both;\r
}\r
\r
.ol-search.IGNF-parcelle > div input {\r
  text-align: right;\r
}\r
.ol-search.IGNF-parcelle > div input,\r
.ol-search.IGNF-parcelle > div label {\r
  width: 4em;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  display: inline-block;\r
  margin: .1em;\r
  font-size: 1em;\r
  overflow: hidden;\r
  text-overflow: ellipsis;\r
}\r
.ol-search.IGNF-parcelle > div label {\r
  letter-spacing: -0.04em;\r
}\r
.ol-search.IGNF-parcelle ul.autocomplete-page {\r
  margin-top:.5em;\r
  width:100%;\r
  text-align: center;\r
  display: none;\r
}\r
.ol-search.IGNF-parcelle.ol-collapsed-list ul.autocomplete-parcelle,\r
.ol-search.IGNF-parcelle.ol-collapsed-list ul.autocomplete-page {\r
  display: block;\r
}\r
.ol-search.IGNF-parcelle.ol-collapsed ul.autocomplete-page,\r
.ol-search.IGNF-parcelle.ol-collapsed ul.autocomplete-parcelle,\r
.ol-search.IGNF-parcelle ul.autocomplete-parcelle {\r
  display: none;\r
}\r
.ol-search.IGNF-parcelle ul.autocomplete-page li {\r
  display: inline-block;\r
  color: #fff;\r
  background: rgba(0,60,136,.5);\r
  border-radius: 50%;\r
  width: 1.3em;\r
  height: 1.3em;\r
  padding: .1em;\r
  margin: 0 .1em;\r
}\r
.ol-search.IGNF-parcelle ul.autocomplete-page li.selected {\r
  background: rgba(0,60,136,1);\r
}\r
\r
/* GPS */\r
.ol-searchgps input.search,\r
.ol-searchcoord input.search {\r
  display: none;\r
}\r
.ol-control.ol-searchgps > button:first-child,\r
.ol-control.ol-searchcoord > button:first-child {\r
  background-image: none;\r
}\r
.ol-control.ol-searchgps > button:first-child:before,\r
.ol-control.ol-searchcoord > button:first-child:before {\r
  content: "x/y";\r
  position: unset;\r
  display: block;\r
  -webkit-transform: scaleX(.8);\r
          transform: scaleX(.8);\r
  border: unset;\r
  border-radius: 0;\r
  width: auto;\r
  height: auto;\r
}\r
.ol-control.ol-searchgps > button:first-child:after,\r
.ol-control.ol-searchcoord > button:first-child:after {\r
  content: unset;\r
}\r
.ol-control.ol-searchgps .ol-latitude,\r
.ol-control.ol-searchgps .ol-longitude,\r
.ol-control.ol-searchcoord .ol-latitude,\r
.ol-control.ol-searchcoord .ol-longitude {\r
  clear: both;\r
}\r
.ol-control.ol-searchgps .ol-latitude label,\r
.ol-control.ol-searchgps .ol-longitude label,\r
.ol-control.ol-searchcoord .ol-latitude label,\r
.ol-control.ol-searchcoord .ol-longitude label {\r
  width: 5.5em;\r
  display: inline-block;\r
  text-align: right;\r
  -webkit-transform: scaleX(.8);\r
          transform: scaleX(.8);\r
  margin: 0 -.8em 0 0;\r
  -webkit-transform-origin: 0 0;\r
          transform-origin: 0 0;\r
}\r
.ol-control.ol-searchcoord .ol-latitude label,\r
.ol-control.ol-searchcoord .ol-longitude label {\r
  width: 2em;\r
  margin: 0;\r
}\r
.ol-control.ol-searchgps .ol-latitude input,\r
.ol-control.ol-searchgps .ol-longitude input {\r
  max-width: 10em;\r
}\r
\r
.ol-control.ol-searchgps .ol-ext-toggle-switch {\r
  cursor: pointer;\r
  float: right;\r
  margin: .5em;\r
  font-size: .9em;\r
}\r
\r
.ol-searchgps .ol-decimal{\r
  display: inline-block;\r
  margin-right: .7em;\r
}\r
.ol-searchgps .ol-dms,\r
.ol-searchgps.ol-dms .ol-decimal {\r
  display: none;\r
  width: 3em;\r
  text-align: right;\r
}\r
.ol-searchgps.ol-dms .ol-dms {\r
  display: inline-block;\r
}\r
\r
.ol-searchgps span.ol-dms {\r
  width: .5em;\r
  text-align: left;\r
}\r
.ol-searchgps.ol-control.ol-collapsed button.ol-geoloc,\r
.ol-searchgps.ol-control.ol-collapsed button.ol-centerloc,\r
.ol-searchcoord.ol-control.ol-collapsed button.ol-centerloc,\r
.ol-searchcoord.ol-control.ol-collapsed button.ol-geoloc {\r
  display: none;\r
}\r
.ol-searchgps button.ol-geoloc,\r
.ol-searchgps button.ol-centerloc,\r
.ol-searchcoord button.ol-centerloc,\r
.ol-searchcoord button.ol-geoloc {\r
  top: 0;\r
  float: right;\r
  margin-right: 3px;\r
  background-image: none;\r
  position: relative;\r
}\r
.ol-searchgps button.ol-geoloc:before,\r
.ol-searchcoord button.ol-geoloc:before {\r
  content:"";\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  width: .6em;\r
  height: .6em;\r
  border: .1em solid currentColor;\r
  border-radius: 50%;\r
  -webkit-transform: translate(-50%,-50%);\r
          transform: translate(-50%,-50%);\r
}\r
.ol-searchgps button.ol-geoloc:after,\r
.ol-searchcoord button.ol-geoloc:after {\r
  content:"";\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  width: .2em;\r
  height: .2em;\r
  background-color: transparent;\r
  -webkit-transform: translate(-50%,-50%);\r
          transform: translate(-50%,-50%);\r
  -webkit-box-shadow: \r
    .45em 0 currentColor, -.45em 0 currentColor, 0 -.45em currentColor, 0 .45em currentColor,\r
    .25em 0 currentColor, -.25em 0 currentColor, 0 -.25em currentColor, 0 .25em currentColor;\r
          box-shadow: \r
    .45em 0 currentColor, -.45em 0 currentColor, 0 -.45em currentColor, 0 .45em currentColor,\r
    .25em 0 currentColor, -.25em 0 currentColor, 0 -.25em currentColor, 0 .25em currentColor;\r
}\r
\r
.ol-searchgps button.ol-centerloc:before,\r
.ol-searchcoord button.ol-centerloc:before {\r
  content: "\u{1F4CD}";\r
  border: 0;\r
  width: auto;\r
  height: auto;\r
  padding: 0;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
.ol-searchgps button.ol-centerloc:after,\r
.ol-searchcoord button.ol-centerloc:after {\r
  display: none;\r
}\r
.ol-control.ol-select {\r
  top: .5em;\r
  left: 3em;\r
  background-color: rgba(255,255,255,.5);\r
}\r
.ol-control.ol-select:hover {\r
  background-color: rgba(255,255,255,.7);\r
}\r
.ol-touch .ol-control.ol-select {\r
  left: 3.5em;\r
}\r
.ol-control.ol-select > button:before {\r
  content: "A";\r
  font-size: .6em;\r
  font-weight: normal;\r
  position: absolute;\r
  -webkit-box-sizing: content-box;\r
          box-sizing: content-box;\r
  width: 1em;\r
  height: 1em;\r
  background-color: transparent;\r
  border: .2em solid currentColor;\r
  border-radius: 100%;\r
  top: .5em;\r
  left: .5em;\r
  line-height: 1em;\r
  text-align: center;\r
}\r
.ol-control.ol-select > button:after {\r
  content: "";\r
  position: absolute;\r
  top: 1.15em;\r
  left: 1em;\r
  width: .45em;\r
  height: .15em;\r
  background-color: currentColor;\r
  border-radius: .05em;\r
  -webkit-transform: rotate(45deg);\r
          transform: rotate(45deg);\r
  -webkit-box-shadow: -0.18em 0 0 -0.03em;\r
          box-shadow: -0.18em 0 0 -0.03em;\r
}\r
.ol-select > div button {\r
  width: auto;\r
  padding: 0 .5em;\r
  float: right;\r
  font-weight: normal;\r
  height: 1.2em;\r
  line-height: 1.2em;\r
}\r
.ol-select .ol-delete {\r
    width: 1.5em;\r
  height: 1em;\r
  vertical-align: middle;\r
  display: inline-block;\r
  position: relative;\r
  cursor: pointer;\r
}\r
.ol-select .ol-delete:before {\r
  content:'\\00d7';\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  width: 100%;\r
  text-align: center;\r
  font-weight: bold;\r
}\r
.ol-control.ol-select input {\r
  font-size: 1em;\r
}\r
.ol-control.ol-select select {\r
  font-size: 1em;\r
  max-width: 10em;\r
}\r
.ol-control.ol-select select option.ol-default {\r
  color: #999;\r
  font-style: italic;\r
}\r
.ol-control.ol-select > div {\r
  display: block;\r
  margin: .25em;\r
}\r
.ol-control.ol-select.ol-collapsed > div {\r
  display: none;\r
}\r
\r
.ol-control.ol-select.ol-select-check {\r
  max-width: 20em;\r
}\r
.ol-control.ol-select label.ol-ext-check {\r
  margin-right: 1em;\r
}\r
.ol-control.ol-select label.ol-ext-toggle-switch span {\r
  font-size: 1.1em;\r
}\r
\r
.ol-select ul {\r
  list-style: none;\r
  margin: 0;\r
  padding: 0;\r
}\r
.ol-control.ol-select input[type="search"],\r
.ol-control.ol-select input[type="text"]  {\r
  width: 8em;\r
}\r
\r
.ol-select .ol-autocomplete {\r
  display: inline;\r
}\r
.ol-select .ol-autocomplete ul {\r
  position: absolute;\r
  display: block;\r
  background: #fff;\r
  border: 1px solid #999;\r
  min-width: 10em;\r
  font-size: .85em;\r
}\r
.ol-select .ol-autocomplete ul li {\r
  padding: 0 .5em;\r
}\r
.ol-select .ol-autocomplete ul li:hover {\r
  color: #fff;\r
  background: rgba(0,60,136,.5);\r
}\r
.ol-select ul.ol-hidden {\r
  display: none;\r
}\r
\r
.ol-select-multi li > div:hover,\r
.ol-select-multi li > div.ol-control.ol-select {\r
  position: relative;\r
  top: unset;\r
  left: unset;\r
  background: transparent;\r
}\r
.ol-select-multi li > div  > button,\r
.ol-select-multi li > div  .ol-ok {\r
  display: none;\r
}\r
.ol-select-multi li .ol-control.ol-select.ol-collapsed > div,\r
.ol-select-multi li > div  > div {\r
  display: block;\r
}\r
\r
.ol-control.ol-status {\r
  top: 0;\r
  left: 0;\r
  background: rgba(0,0,0,.2);\r
  color: #fff;\r
  font-size: .9em;\r
  padding: .3em 3em;\r
  border-radius: 0;\r
  width: 100%;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  pointer-events: none!important;\r
  display: none;\r
}\r
.ol-control.ol-status.ol-visible {\r
  display: initial;\r
}\r
.ol-control.ol-status.ol-bottom {\r
  top: auto;\r
  bottom: 0;\r
}\r
.ol-control.ol-status.ol-left {\r
  top: 0;\r
  bottom: 0;\r
  padding: .3em .5em .3em 3em;\r
  width: auto;\r
}\r
.ol-control.ol-status.ol-right {\r
  top: 0;\r
  bottom: 0;\r
  left: auto;\r
  right: 0;\r
  padding: .3em 3em .3em .5em;\r
  width: auto;\r
}\r
.ol-control.ol-status.ol-center {\r
  top: 50%;\r
  -webkit-transform: translateY(-50%);\r
          transform: translateY(-50%);\r
}\r
\r
.ol-control.ol-storymap {\r
  top: .5em;\r
  left: .5em;\r
  bottom: .5em;\r
  max-width: 35%;\r
  border-radius: .5em;\r
  position: absolute;\r
  height: auto;\r
  background-color: rgba(255,255,255,.5);\r
}\r
.ol-storymap {\r
  overflow: hidden;\r
  padding: 0;\r
  height: 100%;\r
  position: relative;\r
}\r
.ol-storymap > div {\r
  overflow: hidden;\r
  padding: 0;\r
  height: 100%;\r
  position: relative;\r
  scroll-behavior: smooth;\r
  -webkit-user-select: none;\r
     -moz-user-select: none;\r
      -ms-user-select: none;\r
          user-select: none;\r
}\r
.ol-storymap >div.ol-move {\r
  scroll-behavior: unset;\r
}\r
\r
.ol-control.ol-storymap .chapter {\r
  position: relative;\r
  padding: .5em;\r
  overflow: hidden;\r
}\r
.ol-control.ol-storymap .chapter:last-child {\r
  margin-bottom: 100%;\r
}\r
.ol-storymap .chapter {\r
  cursor: pointer;\r
  opacity: .4;\r
}\r
.ol-storymap .chapter.ol-select {\r
  cursor: default;\r
  opacity: 1;\r
  background-color: rgba(255,255,255,.8);\r
}\r
\r
.ol-storymap .ol-scroll-top,\r
.ol-storymap .ol-scroll-next {\r
  position: relative;\r
  min-height: 1.7em;\r
  color: rgba(0,60,136,.5);\r
  text-align: center;\r
  cursor: pointer;\r
}\r
.ol-storymap .ol-scroll-next span {\r
  padding-bottom: 1.4em;\r
  display: block;\r
}\r
.ol-storymap .ol-scroll-top span {\r
  padding-top: 1.4em;\r
  display: block;\r
}\r
\r
.ol-storymap .ol-scroll-top:before,\r
.ol-storymap .ol-scroll-next:before {\r
  content: "";\r
  border: .3em solid currentColor;\r
  border-radius: .3em;\r
  border-color: transparent currentColor currentColor transparent;\r
  width: .8em;\r
  height: .8em;\r
  display: block;\r
  position: absolute;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%) rotate(45deg);\r
          transform: translateX(-50%) rotate(45deg);\r
  -webkit-animation: ol-bounce-bottom 0.35s linear infinite alternate;\r
          animation: ol-bounce-bottom 0.35s linear infinite alternate;\r
  pointer-events: none;\r
}\r
.ol-storymap .ol-scroll-top:before {\r
  border-color: currentColor transparent transparent currentColor;\r
  -webkit-animation: ol-bounce-top 0.35s linear infinite alternate;\r
          animation: ol-bounce-top 0.35s linear infinite alternate;\r
}\r
\r
@-webkit-keyframes ol-bounce-top{\r
  from {top: -.2em;}\r
  to   {top: .5em;}\r
}\r
\r
@keyframes ol-bounce-top{\r
  from {top: -.2em;}\r
  to   {top: .5em;}\r
}\r
@-webkit-keyframes ol-bounce-bottom{\r
  from {bottom: -.2em;}\r
  to   {bottom: .5em;}\r
}\r
@keyframes ol-bounce-bottom{\r
  from {bottom: -.2em;}\r
  to   {bottom: .5em;}\r
}\r
\r
.ol-storymap img[data-title] {\r
  cursor: pointer;\r
}\r
\r
/* scrollLine / scrollbox */\r
.ol-storymap.scrollLine,\r
.ol-storymap.scrollBox {\r
  top: 0;\r
  bottom: 0;\r
  background-color: transparent;\r
  border-radius: 0;\r
  max-width: 40%;\r
}\r
.ol-storymap.scrollLine .chapter,\r
.ol-storymap.scrollBox .chapter {\r
  background-color: #fff;\r
  margin: 100% 0;\r
}\r
.ol-storymap.scrollLine .chapter:first-child,\r
.ol-storymap.scrollBox .chapter:first-child {\r
  margin-top: 3em;\r
}\r
.ol-storymap.scrollLine .chapter.ol-select,\r
.ol-storymap.scrollLine .chapter,\r
.ol-storymap.scrollBox .chapter.ol-select,\r
.ol-storymap.scrollBox .chapter {\r
  opacity: 1;\r
}\r
\r
.ol-storymap.scrollLine .ol-scrolldiv,\r
.ol-storymap.scrollBox .ol-scrolldiv {\r
  padding-right: 30px;\r
}\r
.ol-storymap.scrollLine:before,\r
.ol-storymap.scrollBox:before {\r
  content: "";\r
  position: absolute;\r
  width: 2px;\r
  height: 100%;\r
  top: 0;\r
  bottom: 0;\r
  right: 14px;\r
  background-color: #fff;\r
}\r
\r
.ol-storymap.scrollLine .ol-scroll,\r
.ol-storymap.scrollBox .ol-scroll {\r
  display: block!important;\r
  padding: 0;\r
  width: 1px;\r
  opacity: 1!important;\r
  right: 15px;\r
  overflow: visible;\r
  -webkit-transition: none;\r
  transition: none;\r
}\r
.ol-storymap.scrollLine .ol-scroll > div {\r
  background-color: transparent;\r
  overflow: visible;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
  -webkit-box-shadow: unset;\r
          box-shadow: unset;\r
}\r
.ol-storymap.scrollLine .ol-scroll > div:before {\r
  content: "";\r
  position: absolute;\r
  width: 10px;\r
  height: 10px;\r
  border-radius: 50%;\r
  background-color: #0af;\r
  border: 2px solid #fff;\r
  left: 50%;\r
  top: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
\r
.ol-storymap.scrollBox .ol-scroll > div {\r
  display: none;\r
}\r
.ol-storymap.scrollBox .chapter:after {\r
  content: "";\r
  width: 20px;\r
  height: 20px;\r
  position: absolute;\r
  top: Min(30%, 5em);\r
  right: -24.5px;\r
  -webkit-box-shadow: 0 0 0 2px #fff, inset 0 0 0 15px #0af;\r
          box-shadow: 0 0 0 2px #fff, inset 0 0 0 15px #0af; \r
  border-radius: 50%;\r
  border: 5px solid transparent;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  z-index: 1;\r
}\r
\r
.ol-swipe {\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  -ms-touch-action: none;\r
      touch-action: none;\r
}\r
\r
.ol-swipe:before {\r
  content: "";\r
  position: absolute;\r
  top: -5000px;\r
  bottom: -5000px;\r
  left: 50%;\r
  width: 4px;\r
  background: #fff;\r
  z-index:-1;\r
  -webkit-transform: translate(-2px, 0);\r
          transform: translate(-2px, 0);\r
}\r
.ol-swipe.horizontal:before {\r
  left: -5000px;\r
  right: -5000px;\r
  top: 50%;\r
  bottom: auto;\r
  width: auto;\r
  height: 4px;\r
}\r
\r
.ol-swipe,\r
.ol-swipe button {\r
  cursor: ew-resize;\r
}\r
.ol-swipe.horizontal,\r
.ol-swipe.horizontal button {\r
  cursor: ns-resize;\r
}\r
\r
.ol-swipe:after,\r
.ol-swipe button:before,\r
.ol-swipe button:after {\r
  content: "";\r
  position: absolute;\r
  top: 25%;\r
  bottom: 25%;\r
  left: 50%;\r
  width: 2px;\r
  background: currentColor;\r
  transform: translate(-1px, 0);\r
  -webkit-transform: translate(-1px, 0);\r
}\r
.ol-swipe button:after {\r
  -webkit-transform: translateX(4px);\r
          transform: translateX(4px);\r
}\r
.ol-swipe button:before {\r
  -webkit-transform: translateX(-6px);\r
          transform: translateX(-6px);\r
}\r
\r
.ol-control.ol-timeline {\r
  bottom: 0;\r
  left: 0;\r
  right: 0;\r
  -webkit-transition: .3s;\r
  transition: .3s;\r
  background-color: rgba(255,255,255,.4);\r
}\r
.ol-control.ol-timeline.ol-collapsed {\r
  -webkit-transform: translateY(100%);\r
          transform: translateY(100%);\r
}\r
.ol-timeline {\r
  overflow: hidden;\r
  padding: 2px 0 0;\r
}\r
.ol-timeline .ol-scroll {\r
  overflow: hidden;\r
  padding: 0;\r
  scroll-behavior: smooth;\r
  line-height: 1em;\r
  height: 6em;\r
  padding: 0 50%;\r
}\r
.ol-timeline .ol-scroll.ol-move {\r
  scroll-behavior: unset;\r
}\r
\r
.ol-timeline.ol-hasbutton .ol-scroll {\r
  margin-left: 1.5em;\r
  padding: 0 calc(50% - .75em);\r
}\r
.ol-timeline .ol-buttons {\r
  display: none;\r
  position: absolute;\r
  top: 0;\r
  background: rgba(255,255,255,.5);\r
  width: 1.5em;\r
  bottom: 0;\r
  left: 0;\r
  z-index: 10;\r
}\r
.ol-timeline.ol-hasbutton .ol-buttons {\r
  display: block;\r
}\r
.ol-timeline .ol-buttons button {\r
  font-size: 1em;\r
  margin: 1px;\r
  position: relative;\r
}\r
.ol-timeline .ol-buttons .ol-zoom-in:before,\r
.ol-timeline .ol-buttons .ol-zoom-out:before {\r
  content: "+";\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
.ol-timeline .ol-buttons .ol-zoom-out:before{\r
  content: '\u2212';\r
}\r
\r
.ol-timeline .ol-scroll > div {\r
  height: 100%;\r
  position: relative;\r
}\r
\r
.ol-timeline .ol-scroll .ol-times {\r
  background: rgba(255,255,255,.5);\r
  height: 1em;\r
  bottom: 0;\r
  position: absolute;\r
  left: -1000px;\r
  right: -1000px;\r
}\r
.ol-timeline .ol-scroll .ol-time {\r
  position: absolute;\r
  font-size: .7em;\r
  color: #999;\r
  bottom: 0;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
}\r
.ol-timeline .ol-scroll .ol-time.ol-year {\r
  color: #666;\r
  z-index: 1;\r
}\r
.ol-timeline .ol-scroll .ol-time:before {\r
  content: "";\r
  position: absolute;\r
  bottom: 1.2em;\r
  left: 50%;\r
  height: 500px;\r
  border-left: 1px solid currentColor;\r
}\r
\r
.ol-timeline .ol-scroll .ol-features {\r
  position: absolute;\r
  top: 0;\r
  bottom: 1em;\r
  left: -200px;\r
  right: -1000px;\r
  margin: 0 0 0 200px;\r
  overflow: hidden;\r
}\r
\r
.ol-timeline .ol-scroll .ol-feature {\r
  position: absolute;\r
  font-size: .7em;\r
  color: #999;\r
  top: 0;\r
  background: #fff;\r
  max-width: 3em;\r
  max-height: 2.4em;\r
  min-height: 1em;\r
  line-height: 1.2em;\r
  border: 1px solid #ccc;\r
  overflow: hidden;\r
  padding: 0 .5em 0 0;\r
  -webkit-transition: all .3s;\r
  transition: all .3s;\r
  cursor: pointer;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
\r
.ol-timeline.ol-zoomhover .ol-scroll .ol-feature:hover,\r
.ol-timeline.ol-zoomhover .ol-scroll .ol-feature.ol-select {\r
  z-index: 1;\r
  -webkit-transform: scale(1.2);\r
          transform: scale(1.2);\r
  background: #eee;\r
  /* max-width: 14em!important; */\r
}\r
\r
/* Center */\r
.ol-timeline .ol-center-date {\r
  display: none;\r
  position: absolute;\r
  left: 50%;\r
  height: 100%;\r
  width: 2px;\r
  bottom: 0;\r
  z-index: 2;\r
  pointer-events: none;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
  background-color: #f00;\r
  opacity: .4;\r
}\r
.ol-timeline.ol-hasbutton .ol-center-date {\r
  left: calc(50% + .75em);\r
}\r
\r
/* Show center */ \r
.ol-timeline.ol-pointer .ol-center-date {\r
  display: block;\r
}\r
.ol-timeline.ol-pointer .ol-center-date:before, \r
.ol-timeline.ol-pointer .ol-center-date:after {\r
  content: '';\r
  border: 0.3em solid transparent;\r
  border-width: .3em .25em;\r
  position: absolute;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
}\r
.ol-timeline.ol-pointer .ol-center-date:before {\r
  border-top-color: #f00;\r
  top: 0;\r
}\r
.ol-timeline.ol-pointer .ol-center-date:after {\r
  border-bottom-color: #f00;\r
  bottom: 0\r
}\r
\r
/* show interval */\r
.ol-timeline.ol-interval .ol-center-date {\r
  display: block;\r
  background-color: transparent;\r
  border: 0 solid #000;\r
  border-width: 0 10000px;\r
  -webkit-box-sizing: content-box;\r
          box-sizing: content-box;\r
  opacity: .2;\r
}\r
.ol-control.ol-videorec {\r
  top: 0;\r
  left: 50%;\r
  -webkit-transform: translateX(-50%);\r
          transform: translateX(-50%);\r
  white-space: nowrap;\r
}\r
\r
.ol-control.ol-videorec button {\r
  position: relative;\r
  display: inline-block;\r
  vertical-align: middle;\r
}\r
\r
.ol-control.ol-videorec button:before {\r
  content: "";\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  width: .8em;\r
  height: .8em;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  background-color: currentColor;\r
}\r
.ol-control.ol-videorec button.ol-start:before {\r
  width: .9em;\r
  height: .9em;\r
  border-radius: 50%;\r
  background-color: #c00;\r
}\r
.ol-control.ol-videorec button.ol-pause:before {\r
  width: .2em;\r
  background-color: transparent;\r
  -webkit-box-shadow: -.2em 0, .2em 0;\r
          box-shadow: -.2em 0, .2em 0;\r
}\r
.ol-control.ol-videorec button.ol-resume:before {\r
  border-style: solid;\r
  background: transparent;\r
  width: auto;\r
  border-width: .4em 0 .4em .6em;\r
  border-color: transparent transparent transparent currentColor;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
\r
.ol-control.ol-videorec button.ol-stop,\r
.ol-control.ol-videorec button.ol-pause,\r
.ol-control.ol-videorec button.ol-resume,\r
.ol-control.ol-videorec[data-state="rec"] .ol-start,\r
.ol-control.ol-videorec[data-state="pause"] .ol-start {\r
  display: none;\r
}\r
.ol-control.ol-videorec[data-state="rec"] .ol-stop,\r
.ol-control.ol-videorec[data-state="pause"] .ol-stop,\r
.ol-control.ol-videorec[data-state="rec"] .ol-pause,\r
.ol-control.ol-videorec[data-state="pause"] .ol-resume {\r
  display: inline-block;\r
}\r
\r
.ol-control.ol-wmscapabilities {\r
  top: .5em;\r
  right: 2.5em;\r
}\r
.ol-touch .ol-control.ol-wmscapabilities {\r
  right: 3em;\r
}\r
.ol-control.ol-wmscapabilities.ol-hidden {\r
  display: none;\r
}\r
.ol-control.ol-wmscapabilities button:before {\r
  content: "+";\r
  position: absolute;\r
  top: calc(50% - .35em);\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
.ol-control.ol-wmscapabilities button:after {\r
  content: "";\r
  width: .75em;\r
  height: .75em;\r
  position: absolute;\r
  background: transparent;\r
  top: calc(50% - .05em);\r
  left: 50%;\r
  -webkit-transform: scaleY(.6) translate(-50%, -50%) rotate(45deg);\r
          transform: scaleY(.6) translate(-50%, -50%) rotate(45deg);\r
  -webkit-box-shadow: inset -.18em -.18em currentColor, -.4em .1em 0 -.25em currentColor, .1em -.35em 0 -.25em currentColor, .15em .15em currentColor;\r
          box-shadow: inset -.18em -.18em currentColor, -.4em .1em 0 -.25em currentColor, .1em -.35em 0 -.25em currentColor, .15em .15em currentColor;\r
  border-radius: .1em 0;\r
  border: .15em solid transparent;\r
  border-width: 0 .15em .15em 0;\r
}\r
\r
.ol-wmscapabilities .ol-searching {\r
  opacity: .5;\r
}\r
.ol-wmscapabilities .ol-searching .ol-url:after{\r
  content: "";\r
  width: .7em;\r
  height: .7em;\r
  background-color: currentColor;\r
  position: absolute;\r
  top: 6em;\r
  border-radius: 50%;\r
  display: block;\r
  left: calc(50% - .35em);\r
  -webkit-box-shadow: 0 1em currentColor, 0 -1em currentColor, 1em 0 currentColor, -1em 0 currentColor;\r
          box-shadow: 0 1em currentColor, 0 -1em currentColor, 1em 0 currentColor, -1em 0 currentColor;\r
  -webkit-animation:ol-wmscapabilities-rotate 2s linear infinite;\r
          animation:ol-wmscapabilities-rotate 2s linear infinite;\r
}\r
@-webkit-keyframes ol-wmscapabilities-rotate { \r
  100% { -webkit-transform:rotate(360deg); transform:rotate(360deg); } \r
}\r
@keyframes ol-wmscapabilities-rotate { \r
  100% { -webkit-transform:rotate(360deg); transform:rotate(360deg); } \r
}\r
\r
.ol-wmscapabilities .ol-url input {\r
  width: calc(100% - 10em);\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  min-width: Min(100%, 20em);\r
}\r
.ol-wmscapabilities .ol-url select {\r
  width: 2em;\r
  height: 100%;\r
  padding: 1px;\r
}\r
.ol-wmscapabilities .ol-url button {\r
  width: 7.5em;\r
  margin-left: .5em;\r
}\r
.ol-wmscapabilities .ol-result {\r
  display: none;\r
  margin-top: .5em;\r
}\r
.ol-wmscapabilities .ol-result.ol-visible {\r
  display: block;\r
}\r
\r
.ol-wmscapabilities .ol-select-list {\r
  position: relative;\r
  border: 1px solid #369;\r
  overflow-x: hidden;\r
  width: calc(100% - 120px);\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  max-height: 14.5em;\r
}\r
.ol-wmscapabilities .ol-select-list div {\r
  padding: 0 .5em;\r
  cursor: pointer;\r
  width: 100%;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  text-overflow: ellipsis;\r
  overflow: hidden;\r
}\r
.ol-wmscapabilities .ol-select-list .level-1 {\r
  padding-left: 1em;\r
}\r
.ol-wmscapabilities .ol-select-list .level-2 {\r
  padding-left: 1.5em;\r
}\r
.ol-wmscapabilities .ol-select-list .level-3 {\r
  padding-left: 2em;\r
}\r
.ol-wmscapabilities .ol-select-list .level-4 {\r
  padding-left: 2.5em;\r
}\r
.ol-wmscapabilities .ol-select-list .level-5 {\r
  padding-left: 3em;\r
}\r
\r
.ol-wmscapabilities .ol-select-list .ol-info {\r
  font-style: italic;\r
}\r
.ol-wmscapabilities .ol-select-list .ol-title {\r
  background-color: rgba(0,60,136,.1);\r
}\r
.ol-wmscapabilities .ol-select-list div:hover {\r
  background-color: rgba(0,60,136,.5);\r
  color: #fff;\r
}\r
.ol-wmscapabilities .ol-select-list div.selected {\r
  background-color: rgba(0,60,136,.7);\r
  color: #fff;\r
}\r
\r
.ol-wmscapabilities .ol-preview {\r
  width: 100px;\r
  float: right;\r
  background: rgba(0,60,136,.1);\r
  color: #666;\r
  padding: 0 5px 5px;\r
  text-align: center;\r
  margin-left: 10px;\r
}\r
.ol-wmscapabilities .ol-preview.tainted {\r
  width: 100px;\r
  float: right;\r
  background: rgba(136,0,60,.1);\r
  color: #666;\r
  padding: 0 5px 5px;\r
  text-align: center;\r
  margin-left: 10px;\r
}\r
.ol-wmscapabilities .ol-preview img {\r
  width: 100%;\r
  display: block;\r
  background: #fff;\r
}\r
.ol-wmscapabilities .ol-legend {\r
  max-width: 100%;\r
  display: none;\r
}\r
.ol-wmscapabilities .ol-legend.visible {\r
  display: block;\r
}\r
.ol-wmscapabilities .ol-buttons {\r
  clear: both;\r
  text-align: right;\r
}\r
.ol-wmscapabilities .ol-data p {\r
  margin: 0;\r
}\r
.ol-wmscapabilities .ol-data p.ol-title {\r
  font-weight: bold;\r
  margin: 1em 0 .5em;\r
}\r
.ol-wmscapabilities .ol-error {\r
  color: #800;\r
}\r
\r
.ol-wmscapabilities ul.ol-wmsform {\r
  display: none;\r
  list-style: none;\r
  padding: 0;\r
}\r
.ol-wmscapabilities ul.ol-wmsform.visible {\r
  display: block;\r
}\r
.ol-wmscapabilities .ol-wmsform label {\r
  display: inline-block;\r
  text-align: right;\r
  width: calc(40% - .5em);\r
  margin-right: .5em;\r
}\r
.ol-wmscapabilities .ol-wmsform input {\r
  display: inline-block;\r
  width: 60%;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
.ol-wmscapabilities .ol-wmsform input[type="checkbox"] {\r
  width: auto;\r
}\r
.ol-wmscapabilities .ol-wmsform button {\r
  float: right;\r
  margin: 1em 0;\r
}\r
\r
.ol-wmscapabilities ul.ol-wmsform li[data-param="extent"] input {\r
  width: calc(60% - 2em);\r
}\r
.ol-wmscapabilities ul.ol-wmsform li[data-param="extent"] button {\r
  position: relative;\r
  width: 2em;\r
  height: 1.6em;\r
  margin: 0;\r
  vertical-align: middle;\r
  color: #444;\r
}\r
.ol-wmscapabilities ul.ol-wmsform li[data-param="extent"] button:before,\r
.ol-wmscapabilities ul.ol-wmsform li[data-param="extent"] button:after {\r
  content: "";\r
  position: absolute;\r
  width: .25em;\r
  height: .9em;\r
  border: .1em solid currentColor;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%) skewY(-15deg);\r
          transform: translate(-50%, -50%) skewY(-15deg);\r
}\r
.ol-wmscapabilities ul.ol-wmsform li[data-param="extent"] button:after {\r
  -webkit-transform: translateX(.4em) translate(-50%, -50%) skewY(15deg);\r
          transform: translateX(.4em) translate(-50%, -50%) skewY(15deg);\r
  -webkit-box-shadow: -0.8em 0.25em;\r
          box-shadow: -0.8em 0.25em;\r
}\r
\r
.ol-ext-dialog.ol-wmscapabilities form {\r
  width: 600px;\r
  min-height: 15em;\r
  top: 15%;\r
  -webkit-transform: translate(-50%, -15%);\r
          transform: translate(-50%, -15%);\r
}\r
.ol-ext-dialog.ol-wmscapabilities .ol-content {\r
  max-height: calc(100vh - 6em);\r
}\r
\r
.ol-ext-dialog.ol-wmtscapabilities [data-param="map"] {\r
  display: none;\r
}\r
.ol-ext-dialog [data-param="style"] {\r
  display: none;\r
}\r
.ol-ext-dialog.ol-wmtscapabilities [data-param="style"] {\r
  display: list-item;\r
}\r
.ol-ext-dialog.ol-wmtscapabilities [data-param="proj"],\r
.ol-ext-dialog.ol-wmtscapabilities [data-param="version"] {\r
  opacity: .6;\r
  pointer-events: none;\r
}\r
\r
.ol-ext-dialog.ol-wmscapabilities button.ol-wmsform {\r
  width: 1.8em;\r
  text-align: center;\r
}\r
.ol-ext-dialog.ol-wmscapabilities button.ol-wmsform:before {\r
  content: "+";\r
}\r
.ol-ext-dialog.ol-wmscapabilities .ol-form button.ol-wmsform:before {\r
  content: "-";\r
}\r
\r
.ol-ext-dialog.ol-wmscapabilities .ol-form button.ol-load,\r
.ol-ext-dialog.ol-wmscapabilities .ol-form .ol-legend {\r
  display: none;\r
}\r
.ol-ext-dialog.ol-wmscapabilities .ol-form ul.ol-wmsform {\r
  display: block;\r
  clear: both;\r
}\r
.ol-target-overlay .ol-target \r
{	border: 1px solid transparent;\r
	-webkit-box-shadow: 0 0 1px 1px #fff;\r
	        box-shadow: 0 0 1px 1px #fff;\r
	display: block;\r
	height: 20px;\r
	width: 0;\r
}\r
\r
.ol-target-overlay .ol-target:after,\r
.ol-target-overlay .ol-target:before\r
{	content:"";\r
	border: 1px solid #369;\r
	-webkit-box-shadow: 0 0 1px 1px #fff;\r
	        box-shadow: 0 0 1px 1px #fff;\r
	display: block;\r
	width: 20px;\r
	height: 0;\r
	position:absolute;\r
	top:10px;\r
	left:-10px;\r
}\r
.ol-target-overlay .ol-target:after\r
{	-webkit-box-shadow: none;	box-shadow: none;\r
	height: 20px;\r
	width: 0;\r
	top:0px;\r
	left:0px;\r
}\r
\r
.ol-overlaycontainer .ol-touch-cursor {\r
  /* human fingertips are typically 16x20 mm = 45x57 pixels\r
    source: http://touchlab.mit.edu/publications/2003_009.pdf\r
  */\r
  width: 56px;\r
  height: 56px;\r
  margin: 6px;\r
  border-radius: 50%;\r
  cursor: pointer;\r
  background: rgba(255,255,255,.4);\r
  -webkit-box-shadow: inset 0 0 0 5px #369;\r
          box-shadow: inset 0 0 0 5px #369;\r
}\r
\r
.ol-overlaycontainer .ol-touch-cursor:after {\r
  content: "";\r
  position: absolute;\r
  top: 0;\r
  left: 0;\r
  width: 50%;\r
  height: 50%;\r
  background: radial-gradient(circle at 100% 100%, transparent, transparent 70%, #369 70%, #369)\r
}\r
\r
.ol-overlaycontainer .ol-touch-cursor .ol-button {\r
  position: absolute;\r
  color: #369;\r
  height: 55%;\r
  width: 55%;\r
  border-radius: 50%;\r
  cursor: pointer;\r
  background: rgba(255,255,255,.4);\r
  -webkit-box-shadow: inset 0 0 0 3px currentColor;\r
          box-shadow: inset 0 0 0 3px currentColor;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%) scale(0);\r
          transform: translate(-50%, -50%) scale(0);\r
  -webkit-transition: all .5s, opacity 0s, background 0s;\r
  transition: all .5s, opacity 0s, background 0s;\r
  overflow: hidden;\r
}\r
.ol-overlaycontainer .ol-touch-cursor.active.disable .ol-button {\r
  opacity: .8;\r
  background: rgba(51, 102, 153, .2);\r
}\r
.ol-overlaycontainer .ol-touch-cursor.active .ol-button {\r
  -webkit-transform: translate(-50%, -50%) scale(1);\r
          transform: translate(-50%, -50%) scale(1);\r
}\r
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-0 {\r
  top: -18%;\r
  left: 118%;\r
}\r
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-1 {\r
  top: 50%;\r
  left: 140%;\r
}\r
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-2 {\r
  top: 120%;\r
  left: 120%;\r
}\r
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-3 {\r
  top: 140%;\r
  left: 50%;\r
}\r
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-4 {\r
  top: 118%;\r
  left: -18%;\r
}\r
/* extra buttons */\r
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-5 {\r
  top: 50%;\r
  left: -40%;\r
}\r
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-6 {\r
  top: -18%;\r
  left: -18%;\r
}\r
.ol-overlaycontainer .ol-touch-cursor.active .ol-button-7 {\r
  top: -40%;\r
  left: 50%;\r
}\r
\r
.ol-overlaycontainer .ol-touch-cursor .ol-button:before {\r
  content: "";\r
  width: 1.5em;\r
  height: 1em;\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  line-height: 1em;\r
  text-align: center;\r
}\r
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-add:before,\r
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-remove:before {\r
  content: "\u2212";\r
  line-height: .95em;\r
  font-size: 1.375em;\r
  font-weight: bold;\r
}\r
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-add:before {\r
  content: "+";\r
}\r
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-x:before {\r
  content: "\\00D7";\r
  font-size: 1.2em;\r
  font-weight: bold;\r
}\r
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-move:before {\r
  content: "\\2725";\r
  font-size: 1.2em;\r
}\r
.ol-overlaycontainer .ol-touch-cursor .ol-button.ol-button-check:before {\r
  content: "\\2713";\r
  font-weight: bold;\r
}\r
\r
.ol-overlaycontainer .ol-touch-cursor.nodrawing .ol-button.ol-button-x,\r
.ol-overlaycontainer .ol-touch-cursor.nodrawing .ol-button.ol-button-remove,\r
.ol-overlaycontainer .ol-touch-cursor.nodrawing .ol-button.ol-button-check {\r
  opacity: .8;\r
  background: rgba(51, 102, 153, .2);\r
}\r
\r
.ol-overlaycontainer .ol-touch-cursor .ol-button > div {\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
\r
.ol-overlaycontainer .ol-touch-cursor .ol-button-type:before {\r
  content: "\\21CE";\r
  font-weight: bold;\r
}\r
\r
\r
\r
/* remove outline on focus */\r
.mapboxgl-canvas:focus {\r
  outline: none;\r
}\r
.ol-perspective-map {\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  width: 200%;\r
  height: 200%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
.ol-perspective-map .ol-layer {\r
  z-index: -1!important; /* bug using Chrome ? */\r
}\r
.ol-perspective-map .ol-layers {\r
  -webkit-transform: translateY(0) perspective(200px) rotateX(0deg) scaleY(1);\r
          transform: translateY(0) perspective(200px) rotateX(0deg) scaleY(1);\r
}\r
\r
.ol-perspective-map .ol-overlaycontainer,\r
.ol-perspective-map .ol-overlaycontainer-stopevent {\r
  width: 50%!important;\r
  height: 50%!important;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
\r
.ol-overlay-container .ol-magnify \r
{	background: rgba(0,0,0, 0.5);\r
	border:3px solid #369;\r
	border-radius: 50%;\r
	height: 150px;\r
	width: 150px;\r
	overflow: hidden;\r
	-webkit-box-shadow: 5px 5px 5px rgba(0, 0, 0, 0.5);\r
	        box-shadow: 5px 5px 5px rgba(0, 0, 0, 0.5);\r
	position:relative;\r
	z-index:0;\r
}\r
\r
.ol-overlay-container .ol-magnify:before \r
{	border-radius: 50%;\r
	-webkit-box-shadow: 0 0 40px 2px rgba(0, 0, 0, 0.25) inset;\r
	        box-shadow: 0 0 40px 2px rgba(0, 0, 0, 0.25) inset;\r
	content: "";\r
	display: block;\r
	height: 100%;\r
	left: 0;\r
	position: absolute;\r
	top: 0;\r
	width: 100%;\r
	z-index: 1;\r
}\r
\r
.ol-overlay-container .ol-magnify:after \r
{\r
	border-radius: 50%;\r
	-webkit-box-shadow: 0 0 20px 7px rgba(255, 255, 255, 1);\r
	        box-shadow: 0 0 20px 7px rgba(255, 255, 255, 1);\r
	content: "";\r
	display: block;\r
	height: 0;\r
	left: 23%;\r
	position: absolute;\r
	top: 20%;\r
	width: 20%;\r
	z-index: 1;\r
	transform: rotate(-40deg);\r
	-webkit-transform: rotate(-40deg);\r
}\r
/** popup animation using visible class\r
*/\r
.ol-popup.anim {\r
  visibility: hidden;\r
}\r
\r
.ol-popup.anim.visible {\r
  visibility: visible;\r
}\r
/** No transform when visible \r
*/\r
.ol-popup.anim.visible > div {\r
  visibility: visible;\r
  -webkit-transform: none;\r
          transform: none;\r
  -webkit-animation: ol-popup_bounce 0.4s ease 1;\r
          animation: ol-popup_bounce 0.4s ease 1;\r
}\r
\r
@-webkit-keyframes ol-popup_bounce {\r
  from { -webkit-transform: scale(0); transform: scale(0); }\r
  50%  { -webkit-transform: scale(1.1); transform: scale(1.1) }\r
  80%  { -webkit-transform: scale(0.95); transform: scale(0.95) }\r
  to   { -webkit-transform: scale(1); transform: scale(1); }\r
}\r
\r
@keyframes ol-popup_bounce {\r
  from { -webkit-transform: scale(0); transform: scale(0); }\r
  50%  { -webkit-transform: scale(1.1); transform: scale(1.1) }\r
  80%  { -webkit-transform: scale(0.95); transform: scale(0.95) }\r
  to   { -webkit-transform: scale(1); transform: scale(1); }\r
}\r
\r
/** Transform Origin */\r
.ol-popup.anim.ol-popup-bottom.ol-popup-left > div  {\r
  -webkit-transform-origin:0 100%;\r
          transform-origin:0 100%;\r
}\r
.ol-popup.anim.ol-popup-bottom.ol-popup-right > div {\r
  -webkit-transform-origin:100% 100%;\r
          transform-origin:100% 100%;\r
}\r
.ol-popup.anim.ol-popup-bottom.ol-popup-center > div {\r
  -webkit-transform-origin:50% 100%;\r
          transform-origin:50% 100%;\r
}\r
.ol-popup.anim.ol-popup-top.ol-popup-left > div {\r
  -webkit-transform-origin:0 0;\r
          transform-origin:0 0;\r
}\r
.ol-popup.anim.ol-popup-top.ol-popup-right > div {\r
  -webkit-transform-origin:100% 0;\r
          transform-origin:100% 0;\r
}\r
.ol-popup.anim.ol-popup-top.ol-popup-center > div {\r
  -webkit-transform-origin:50% 0;\r
          transform-origin:50% 0;\r
}\r
.ol-popup.anim.ol-popup-middle.ol-popup-left > div {\r
  -webkit-transform-origin:0 50%;\r
          transform-origin:0 50%;\r
}\r
.ol-popup.anim.ol-popup-middle.ol-popup-right > div {\r
  -webkit-transform-origin:100% 50%;\r
          transform-origin:100% 50%;\r
}\r
\r
.ol-overlaycontainer-stopevent {\r
  /* BOUG ol6.1 to enable DragOverlay interaction \r
  position: initial!important;\r
  */\r
}\r
\r
/** ol.popup */\r
.ol-popup {\r
  font-size:0.9em;\r
  -webkit-user-select: none;  \r
  -moz-user-select: none;    \r
  -ms-user-select: none;      \r
  user-select: none;\r
}\r
.ol-popup .ol-popup-content {\r
  overflow:hidden;\r
  cursor: default;\r
  padding: 0.25em 0.5em;\r
}\r
.ol-popup.hasclosebox .ol-popup-content {\r
  margin-right: 1.7em;\r
}\r
.ol-popup .ol-popup-content:after {\r
  clear: both;\r
  content: "";\r
  display: block;\r
  font-size: 0;\r
  height: 0;\r
}\r
\r
/** Anchor position */\r
.ol-popup .anchor {\r
  display: block;\r
  width: 0px;\r
  height: 0px;\r
  background:red;\r
  position: absolute;\r
  margin: -11px 22px;\r
  pointer-events: none;\r
}\r
.ol-popup .anchor:after,\r
.ol-popup .anchor:before {\r
  position:absolute;\r
}\r
.ol-popup-right .anchor:after,\r
.ol-popup-right .anchor:before {\r
  right:0;\r
}\r
.ol-popup-top .anchor { top:0; }\r
.ol-popup-bottom .anchor { bottom:0; }\r
.ol-popup-right .anchor { right:0; }\r
.ol-popup-left .anchor { left:0; }\r
.ol-popup-center .anchor { \r
  left:50%; \r
  margin-left: 0!important;\r
}\r
.ol-popup-middle .anchor { \r
  top:50%; \r
  margin-top: 0!important;\r
}\r
.ol-popup-center.ol-popup-middle .anchor { \r
  display:none; \r
}\r
\r
/** Fixed popup */\r
.ol-popup.ol-fixed {\r
  margin: 0!important;\r
  top: .5em!important;\r
  right: .5em!important;\r
  left: auto!important;\r
  bottom: auto!important;\r
  -webkit-transform: none!important;\r
          transform: none!important;\r
}\r
.ol-popup.ol-fixed .anchor {\r
  display: none;\r
}\r
.ol-popup.ol-fixed.anim > div {\r
  -webkit-animation: none;\r
          animation: none;\r
}\r
\r
.ol-popup .ol-fix {\r
  width: 1em;\r
  height: .9em;\r
  background: #fff;\r
  position: relative;\r
  float: right;\r
  margin: .2em;\r
  cursor: pointer;\r
}\r
.ol-popup .ol-fix:before {\r
  content: "";\r
  width: .8em;\r
  height: .7em;\r
  display: block;\r
  border: .1em solid #666;\r
      border-right-width: 0.1em;\r
  border-right-width: .3em;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  margin: .1em;\r
}\r
\r
/** Add a shadow to the popup */\r
.ol-popup.shadow {\r
  -webkit-box-shadow: 2px 2px 2px 2px rgba(0,0,0,0.5);\r
          box-shadow: 2px 2px 2px 2px rgba(0,0,0,0.5);\r
}\r
\r
/** Close box */\r
.ol-popup .closeBox {\r
  background-color: rgba(0, 60, 136, 0.5);\r
  color: #fff;\r
  border: 0;\r
  border-radius: 2px;\r
  cursor: pointer;\r
  float: right;\r
  font-size: 0.9em;\r
  font-weight: 700;\r
  width: 1.4em;\r
  height: 1.4em;\r
  margin: 5px 5px 0 0;\r
  padding: 0;\r
  position: relative;\r
  display:none;\r
}\r
.ol-popup.hasclosebox .closeBox {\r
  display:block;\r
}\r
\r
.ol-popup .closeBox:hover {\r
  background-color: rgba(0, 60, 136, 0.7);\r
}\r
/* the X */\r
.ol-popup .closeBox:after {\r
  content: "\\00d7";\r
  font-size:1.5em;\r
  top: 50%;\r
  left: 0;\r
  right: 0;\r
  width: 100%;\r
  text-align: center;\r
  line-height: 1em;\r
  margin: -0.5em 0;\r
  position: absolute;\r
}\r
\r
/** Modify touch poup */\r
.ol-popup.modifytouch {\r
  background-color: #eee;\r
}\r
.ol-popup.modifytouch .ol-popup-content {	\r
  padding: 0 0.25em;\r
  font-size: 0.85em;\r
  white-space: nowrap;\r
}\r
.ol-popup.modifytouch .ol-popup-content a {\r
  text-decoration: none;\r
}\r
\r
/** Tool tips popup*/\r
.ol-popup.tooltips {\r
  background-color: #ffa;\r
}\r
.ol-popup.tooltips .ol-popup-content{\r
  padding: 0 0.25em;\r
  font-size: 0.85em;\r
  white-space: nowrap;\r
}\r
\r
/** Default popup */\r
.ol-popup.default > div {\r
  background-color: #fff;\r
  border:1px solid #69f;\r
  border-radius: 5px;\r
}\r
.ol-popup.default {\r
  margin: -11px 0;\r
  -webkit-transform: translate(0, -22px);\r
          transform: translate(0, -22px);\r
}\r
.ol-popup-top.ol-popup.default {\r
  margin: 11px 0;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-left.default {\r
  margin: -11px -22px;\r
  -webkit-transform: translate(0, -22px);\r
          transform: translate(0, -22px);\r
}\r
.ol-popup-top.ol-popup-left.default {\r
  margin: 11px -22px;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-right.default {\r
  margin: -11px 22px;\r
  -webkit-transform: translate(44px, -22px);\r
          transform: translate(44px, -22px);\r
}\r
.ol-popup-top.ol-popup-right.default {\r
  margin: 11px 22px;\r
  -webkit-transform: translate(44px, 0);\r
          transform: translate(44px, 0);\r
}\r
.ol-popup-middle.default {\r
  margin:0 10px;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-middle.ol-popup-right.default {\r
  margin:0 -10px;\r
  -webkit-transform: translate(-20px, 0);\r
          transform: translate(-20px, 0);\r
}\r
\r
.ol-popup.default .anchor {\r
  color: #69f;\r
}\r
.ol-popup.default .anchor:after,\r
.ol-popup.default .anchor:before {\r
  content:"";\r
  border-color: currentColor transparent;\r
  border-style: solid;\r
  border-width: 11px;\r
  margin: 0 -11px;\r
}\r
.ol-popup.default .anchor:after {\r
  border-color: #fff transparent;\r
  border-width: 11px;\r
  margin: 2px -11px;\r
}\r
\r
.ol-popup-top.default .anchor:before,\r
.ol-popup-top.default .anchor:after {\r
  border-top:0;\r
  top:0;\r
}\r
\r
.ol-popup-bottom.default .anchor:before,\r
.ol-popup-bottom.default .anchor:after {\r
  border-bottom:0;\r
  bottom:0;\r
}\r
\r
.ol-popup-middle.default .anchor:before {\r
  margin: -11px -33px;\r
  border-color: transparent currentColor;\r
}\r
.ol-popup-middle.default .anchor:after {\r
  margin: -11px -31px;\r
  border-color: transparent #fff;\r
}\r
.ol-popup-middle.ol-popup-left.default .anchor:before,\r
.ol-popup-middle.ol-popup-left.default .anchor:after {\r
  border-left:0;\r
}\r
.ol-popup-middle.ol-popup-right.default .anchor:before,\r
.ol-popup-middle.ol-popup-right.default .anchor:after {\r
  border-right:0;\r
}\r
\r
/** Placemark popup */\r
.ol-popup.placemark {\r
  color: #c00;\r
  margin: -.65em 0;\r
  -webkit-transform: translate(0, -1.3em);\r
          transform: translate(0, -1.3em);\r
}\r
.ol-popup.placemark > div {\r
  position: relative;\r
  font-size: 15px;	\r
  background-color: #fff;\r
  border: 0;\r
  -webkit-box-shadow: inset 0 0 0 0.45em;\r
          box-shadow: inset 0 0 0 0.45em;\r
  width: 2em;\r
  height: 2em;\r
  border-radius: 50%;\r
  min-width: unset;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
.ol-popup.placemark .ol-popup-content {\r
  overflow: hidden;\r
  cursor: default;\r
  text-align: center;\r
  padding: .25em 0;\r
  width: 1em;\r
  height: 1em;\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  line-height: 1em;\r
}\r
.ol-popup.placemark .anchor {\r
  margin: 0;\r
}\r
\r
.ol-popup.placemark .anchor:before {\r
  content: "";\r
  margin: -.5em -.5em;\r
  background: transparent;\r
  width: 1em;\r
  height: .5em;\r
  border-radius: 50%;\r
  -webkit-box-shadow: 0 1em 0.5em rgba(0,0,0,.5);\r
          box-shadow: 0 1em 0.5em rgba(0,0,0,.5);\r
}\r
.ol-popup.placemark .anchor:after {\r
  content: "";\r
  border-color: currentColor transparent;\r
  border-style: solid;\r
  border-width: 1em .7em 0;\r
  margin: -.75em -.7em;\r
  bottom:0;\r
}\r
\r
/** Placemark Shield */\r
.ol-popup.placemark.shield > div {\r
  border-radius: .2em;\r
}\r
\r
.ol-popup.placemark.shield .anchor:after {\r
    border-width: .8em 1em 0;\r
    margin: -.7em -1em;\r
}\r
\r
/** Placemark Blazon */\r
.ol-popup.placemark.blazon > div {\r
  border-radius: .2em;\r
}\r
\r
/** Placemark Needle/Pushpin */\r
.ol-popup.placemark.pushpin {	\r
  margin: -2.2em 0;\r
  -webkit-transform: translate(0, -4em);\r
          transform: translate(0, -4em);\r
}\r
.ol-popup.placemark.pushpin > div {	\r
  border-radius: 0;\r
  background: transparent!important;\r
  -webkit-box-shadow: inset 2em 0 currentColor;\r
          box-shadow: inset 2em 0 currentColor;\r
  width: 1.1em;\r
}\r
.ol-popup.placemark.pushpin > div:before {\r
  content: "";\r
  width: 1.3em;\r
  height: 1.5em;\r
  border-style: solid;\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  -webkit-transform: translate(-50%,-50%);\r
          transform: translate(-50%,-50%);\r
  border-color: currentColor transparent;\r
  border-width: .3em .5em .5em;\r
  pointer-events: none;\r
}\r
\r
.ol-popup.placemark.needle {	\r
  margin: -2em 0;\r
  -webkit-transform: translate(0, -4em);\r
          transform: translate(0, -4em);\r
}\r
.ol-popup.placemark.pushpin .anchor,\r
.ol-popup.placemark.needle .anchor {\r
  margin: -1.2em;\r
}\r
.ol-popup.placemark.pushpin .anchor:after,\r
.ol-popup.placemark.needle .anchor:after {\r
  border-style: solid;\r
    border-width: 2em .15em 0;\r
    margin: -.55em -0.2em;\r
    width: .1em;\r
}\r
.ol-popup.placemark.pushpin .anchor:before,\r
.ol-popup.placemark.needle .anchor:before {\r
    margin: -.75em -.5em;\r
}\r
\r
/** Placemark Flag */\r
.ol-popup.placemark.flagv {\r
  margin: -2em 1em;\r
  -webkit-transform: translate(0, -4em);\r
          transform: translate(0, -4em);\r
}\r
.ol-popup.placemark.flagv > div {\r
  border-radius: 0;\r
  -webkit-box-shadow: none;\r
          box-shadow: none;\r
  background-color: transparent;\r
}\r
.ol-popup.placemark.flagv > div:before {\r
  content: "";\r
  border: 1em solid transparent;\r
  position: absolute;\r
  border-left: 2em solid currentColor;\r
  pointer-events: none;\r
}\r
.ol-popup.placemark.flagv .anchor {\r
  margin: -1.4em;\r
}\r
\r
.ol-popup.placemark.flag {	\r
  margin: -2em 1em;\r
  -webkit-transform: translate(0, -4em);\r
          transform: translate(0, -4em);\r
}\r
.ol-popup.placemark.flag > div {	\r
  border-radius: 0;\r
  -webkit-transform-origin: 0% 150%!important;\r
          transform-origin: 0% 150%!important;\r
}\r
.ol-popup.placemark.flag .anchor {\r
  margin: -1.4em;\r
}\r
.ol-popup.placemark.flagv .anchor:after, \r
.ol-popup.placemark.flag .anchor:after {\r
  border-style: solid;\r
  border-width: 2em .15em 0;\r
  margin: -.55em -1em;\r
  width: .1em;\r
}\r
.ol-popup.placemark.flagv .anchor:before,\r
.ol-popup.placemark.flag .anchor:before {\r
  margin: -.75em -1.25em;\r
}\r
\r
.ol-popup.placemark.flag.finish {\r
  margin: -2em 1em;\r
}\r
.ol-popup.placemark.flag.finish > div {\r
  background-image: \r
    linear-gradient(45deg, currentColor 25%, transparent 25%, transparent 75%, currentColor 75%, currentColor), \r
    linear-gradient(45deg, currentColor 25%, transparent 25%, transparent 75%, currentColor 75%, currentColor);\r
  background-size: 1em 1em;\r
  background-position: .5em 0, 0 .5em;\r
  -webkit-box-shadow: inset 0 0 0 .25em;\r
          box-shadow: inset 0 0 0 .25em;\r
}\r
\r
/** Black popup */\r
.ol-popup.black .closeBox {\r
  background-color: rgba(0,0,0, 0.5);\r
  border-radius: 5px;\r
  color: #f80;\r
}\r
.ol-popup.black .closeBox:hover {\r
  background-color: rgba(0,0,0, 0.7);\r
  color:#da2;\r
}\r
\r
.ol-popup.black {\r
  margin: -20px 0;\r
  -webkit-transform: translate(0, -40px);\r
          transform: translate(0, -40px);\r
}\r
.ol-popup.black > div{\r
  background-color: rgba(0,0,0,0.6);\r
  border-radius: 5px;\r
  color:#fff;\r
}\r
.ol-popup-top.ol-popup.black {\r
  margin: 20px 0;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-left.black {\r
  margin: -20px -22px;\r
  -webkit-transform: translate(0, -40px);\r
          transform: translate(0, -40px);\r
}\r
.ol-popup-top.ol-popup-left.black {\r
  margin: 20px -22px;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-right.black {\r
  margin: -20px 22px;\r
  -webkit-transform: translate(44px, -40px);\r
          transform: translate(44px, -40px);\r
}\r
.ol-popup-top.ol-popup-right.black {\r
  margin: 20px 22px;\r
  -webkit-transform: translate(44px, 0);\r
          transform: translate(44px, 0);\r
}\r
.ol-popup-middle.black {\r
  margin: 0 11px;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-left.ol-popup-middle.black {\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-right.ol-popup-middle.black {\r
  margin:0 -11px;\r
  -webkit-transform: translate(-22px, 0);\r
          transform: translate(-22px, 0);\r
}\r
\r
.ol-popup.black .anchor {\r
  margin: -20px 11px;\r
  color: rgba(0,0,0,0.6);\r
} \r
.ol-popup.black .anchor:before {\r
  content:"";\r
  border-color: currentColor transparent;\r
  border-style: solid;\r
  border-width: 20px 11px;\r
}\r
\r
.ol-popup-top.black .anchor:before {\r
  border-top:0;\r
  top:0;\r
}\r
\r
.ol-popup-bottom.black .anchor:before {\r
  border-bottom:0;\r
  bottom:0;\r
}\r
\r
.ol-popup-middle.black .anchor:before {\r
  margin: -20px -22px;\r
  border-color: transparent currentColor;\r
}\r
.ol-popup-middle.ol-popup-left.black .anchor:before {\r
  border-left:0;\r
}\r
.ol-popup-middle.ol-popup-right.black .anchor:before {\r
  border-right:0;\r
}\r
\r
.ol-popup-center.black .anchor:before {\r
  margin: 0 -10px;\r
}\r
\r
\r
/** Green tips popup */\r
.ol-popup.tips .closeBox {\r
  background-color: #f00;\r
  border-radius: 50%;\r
  color: #fff;\r
  width:1.2em;\r
  height:1.2em;\r
}\r
.ol-popup.tips .closeBox:hover {\r
  background-color: #f40;\r
}\r
\r
.ol-popup.tips {\r
  margin: -20px 0;\r
  -webkit-transform: translate(0,-40px);\r
          transform: translate(0,-40px);\r
}\r
.ol-popup.tips > div {\r
  background-color: #cea;\r
  border: 5px solid #ad7;\r
  border-radius: 5px;\r
  color:#333;\r
}\r
.ol-popup-top.ol-popup.tips {\r
  margin: 20px 0;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-left.tips {\r
  margin: -20px -22px;\r
  -webkit-transform: translate(0,-40px);\r
          transform: translate(0,-40px);\r
}\r
.ol-popup-top.ol-popup-left.tips {\r
  margin: 20px -22px;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-right.tips {\r
  margin: -20px 22px;\r
  -webkit-transform: translate(44px,-40px);\r
          transform: translate(44px,-40px);\r
}\r
.ol-popup-top.ol-popup-right.tips {\r
  margin: 20px 22px;\r
  -webkit-transform: translate(44px,0);\r
          transform: translate(44px,0);\r
}\r
.ol-popup-middle.tips {\r
  margin:0;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-left.ol-popup-middle.tips {\r
  margin: 0 22px;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-right.ol-popup-middle.tips {\r
  margin: 0 -22px;\r
  -webkit-transform: translate(-44px,0);\r
          transform: translate(-44px,0);\r
}\r
\r
.ol-popup.tips .anchor {\r
  margin: -18px 22px;\r
  color: #ad7;\r
} \r
.ol-popup.tips .anchor:before {\r
  content:"";\r
  border-color: currentColor transparent;\r
  border-style: solid;\r
  border-width: 20px 11px;\r
}\r
\r
.ol-popup-top.tips .anchor:before {\r
  border-top:0;\r
  top:0;\r
}\r
.ol-popup-bottom.tips .anchor:before {\r
  border-bottom:0;\r
  bottom:0;\r
}\r
.ol-popup-center.tips .anchor:before {\r
  border-width: 20px 6px;\r
  margin: 0 -6px;\r
}\r
.ol-popup-left.tips .anchor:before {\r
  border-left:0;\r
  margin-left:0;\r
}\r
.ol-popup-right.tips .anchor:before {\r
  border-right:0;\r
  margin-right:0;\r
}\r
\r
.ol-popup-middle.tips .anchor:before {\r
  margin: -6px -41px;\r
  border-color: transparent currentColor;\r
  border-width:6px 20px;\r
}\r
.ol-popup-middle.ol-popup-left.tips .anchor:before {\r
  border-left:0;\r
}\r
.ol-popup-middle.ol-popup-right.tips .anchor:before {\r
  border-right:0;\r
}\r
\r
/** Warning popup */\r
.ol-popup.warning .closeBox {\r
  background-color: #f00;\r
  border-radius: 50%;\r
  color: #fff;\r
  font-size: 0.83em;\r
}\r
.ol-popup.warning .closeBox:hover {\r
  background-color: #f40;\r
}\r
\r
.ol-popup.warning {\r
  background-color: #fd0;\r
  border-radius: 3px;\r
  border:4px dashed #f00;\r
  margin:20px 0;\r
  color:#900;\r
  margin: -28px 10px;\r
  -webkit-transform: translate(0, -56px);\r
          transform: translate(0, -56px);\r
}\r
.ol-popup-top.ol-popup.warning {\r
  margin: 28px 10px;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-left.warning {\r
  margin: -28px -22px;\r
  -webkit-transform: translate(0, -56px);\r
          transform: translate(0, -56px);\r
}\r
.ol-popup-top.ol-popup-left.warning {\r
  margin: 28px -22px;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-right.warning {\r
  margin: -28px 22px;\r
  -webkit-transform: translate(44px, -56px);\r
          transform: translate(44px, -56px);\r
}\r
.ol-popup-top.ol-popup-right.warning {\r
  margin: 28px 22px;\r
  -webkit-transform: translate(44px, 0);\r
          transform: translate(44px, 0);\r
}\r
.ol-popup-middle.warning {\r
  margin:0;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-left.ol-popup-middle.warning {\r
  margin:0 22px;\r
  -webkit-transform: none;\r
          transform: none;\r
}\r
.ol-popup-right.ol-popup-middle.warning {\r
  margin:0 -22px;\r
  -webkit-transform: translate(-44px, 0);\r
          transform: translate(-44px, 0);\r
}\r
\r
.ol-popup.warning .anchor {\r
  margin: -33px 7px;\r
} \r
.ol-popup.warning .anchor:before {\r
  content:"";\r
  border-color: #f00 transparent;\r
  border-style: solid;\r
  border-width: 30px 11px;\r
}\r
\r
.ol-popup-top.warning .anchor:before {\r
  border-top:0;\r
  top:0;\r
}\r
.ol-popup-bottom.warning .anchor:before {\r
  border-bottom:0;\r
  bottom:0;\r
}\r
\r
.ol-popup-center.warning .anchor:before {\r
  margin: 0 -21px;\r
}\r
.ol-popup-middle.warning .anchor:before {\r
  margin: -10px -33px;\r
  border-color: transparent #f00;\r
  border-width:10px 22px;\r
}\r
.ol-popup-middle.ol-popup-left.warning .anchor:before {\r
  border-left:0;\r
}\r
.ol-popup-middle.ol-popup-right.warning .anchor:before {\r
  border-right:0;\r
}\r
\r
.ol-popup .ol-popupfeature table {\r
  width: 100%;\r
}\r
.ol-popup .ol-popupfeature table td {\r
  max-width: 25em;\r
  overflow: hidden;\r
  text-overflow: ellipsis;\r
}\r
.ol-popup .ol-popupfeature table td img {\r
  max-width: 100px;\r
  max-height: 100px;\r
}\r
.ol-popup .ol-popupfeature tr:nth-child(2n+1) {\r
  background-color: #eee;\r
}\r
.ol-popup .ol-popupfeature .ol-zoombt {\r
  border: 0;\r
  width: 2em;\r
  height: 2em;\r
  display: inline-block;\r
  color: rgba(0,60,136,.5);\r
  position: relative;\r
  background: transparent;\r
  outline: none;\r
}\r
.ol-popup .ol-popupfeature .ol-zoombt:before {\r
  content: "";\r
  position: absolute;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  width: 1em;\r
  height: 1em;\r
  background-color: transparent;\r
  border: .17em solid currentColor;\r
  border-radius: 100%;\r
  top: .3em;\r
  left: .3em;\r
}\r
.ol-popup .ol-popupfeature .ol-zoombt:after {\r
  content: "";\r
  position: absolute;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  top: 1.35em;\r
  left: 1.15em;\r
  border-width: .1em .3em;\r
  border-style: solid;\r
  border-radius: .03em;\r
  -webkit-transform: rotate(45deg);\r
          transform: rotate(45deg);\r
  -webkit-box-shadow: -0.2em 0 0 -0.04em;\r
          box-shadow: -0.2em 0 0 -0.04em;\r
}\r
\r
.ol-popup .ol-popupfeature .ol-count{\r
  float: right;\r
  margin: .25em 0;\r
}\r
.ol-popup .ol-popupfeature .ol-prev,\r
.ol-popup .ol-popupfeature .ol-next {\r
  border-style: solid;\r
  border-color: transparent rgba(0,60,136,.5);\r
  border-width: .5em 0 .5em .5em;\r
  display: inline-block;\r
  vertical-align: bottom;\r
  margin: 0 .5em;\r
  cursor: pointer;\r
}\r
.ol-popup .ol-popupfeature .ol-prev{\r
  border-width: .5em .5em .5em 0;\r
}\r
\r
.ol-popup.tooltips.black {\r
  background-color: transparent;\r
}\r
.ol-popup.tooltips.black > div {\r
  -webkit-transform: scaleY(1.3);\r
          transform: scaleY(1.3);\r
  padding: .2em .5em;\r
  background-color: rgba(0,0,0, 0.5);\r
}\r
.ol-popup-middle.tooltips.black .anchor:before {\r
  border-width: 5px 10px;\r
  margin: -5px -21px;\r
}\r
\r
.ol-popup-center.ol-popup-middle { \r
  margin: 0;\r
}\r
\r
.ol-popup-top.ol-popup-left.ol-fixPopup,\r
.ol-popup-top.ol-popup-right.ol-fixPopup,\r
.ol-popup.ol-fixPopup {\r
  margin: 0;\r
}\r
\r
.ol-miniscroll {\r
  position: relative;\r
}\r
.ol-miniscroll:hover .ol-scroll {\r
  opacity: .5;\r
  -webkit-transition: opacity 1s;\r
  transition: opacity 1s;\r
}\r
.ol-miniscroll .ol-scroll {\r
  -ms-touch-action: none;\r
      touch-action: none;\r
  position: absolute;\r
  right: 0px;\r
  width: 9px;\r
  height: auto;\r
  max-height: 100%;\r
  opacity: 0;\r
  border-radius: 9px;\r
  -webkit-transition: opacity 1s .5s;\r
  transition: opacity 1s .5s;\r
  overflow: hidden;\r
  z-index: 1;\r
}\r
.ol-miniscroll .ol-scroll > div {\r
  -ms-touch-action: none;\r
      touch-action: none;\r
  position: absolute;\r
  top: 0;\r
  right: 0px;\r
  width: 9px;\r
  height: 9px;\r
  -webkit-box-shadow: inset 10px 0 currentColor;\r
          box-shadow: inset 10px 0 currentColor;\r
  border-radius: 9px / 12px;\r
  border: 2px solid transparent;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  cursor: pointer;\r
}\r
.ol-miniscroll .ol-scroll.ol-100pc {\r
  opacity: 0;\r
}\r
\r
.ol-viewport canvas.ol-fixedoverlay {\r
  position: absolute;\r
  top: 0;\r
  left: 0;\r
  width: 100%;\r
  height: 100%;\r
}\r
/* Toggle Switch */\r
.ol-ext-toggle-switch {\r
  cursor: pointer;\r
  position: relative;\r
}\r
.ol-ext-toggle-switch input[type="radio"],\r
.ol-ext-toggle-switch input[type="checkbox"] {\r
  display: none;\r
}\r
.ol-ext-toggle-switch span {\r
  color: rgba(0,60,136,.5);\r
  position: relative;\r
  cursor: pointer;\r
  background-color: #ccc;\r
  -webkit-transition: .4s, background-color 0s, border-color 0s;\r
  transition: .4s, background-color 0s, border-color 0s;\r
  width: 1.6em;\r
  height: 1em;\r
  display: inline-block;\r
  border-radius: 1em;\r
  font-size: 1.3em;\r
  vertical-align: middle;\r
  margin: -.15em .2em .15em;\r
}\r
.ol-ext-toggle-switch span:before {\r
  position: absolute;\r
  content: "";\r
  height: 1em;\r
  width: 1em;\r
  left: 0;\r
  top: 50%;\r
  background-color: #fff;\r
  -webkit-transition: .4s;\r
  transition: .4s;\r
  border-radius: 1em;\r
  display: block;\r
  -webkit-transform: translateY(-50%);\r
          transform: translateY(-50%);\r
  border: 2px solid #ccc;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
.ol-ext-toggle-switch:hover span {\r
  background-color: #999;\r
}\r
.ol-ext-toggle-switch:hover span:before {\r
  border-color: #999;\r
}\r
\r
.ol-ext-toggle-switch input:checked + span {\r
  background-color: currentColor;\r
}\r
.ol-ext-toggle-switch input:checked + span:before {\r
  -webkit-transform: translate(.6em,-50%);\r
          transform: translate(.6em,-50%);\r
  border-color: currentColor;\r
}\r
\r
/* Check/radio buttons */\r
.ol-ext-check {\r
  position: relative;\r
  display: inline-block;\r
}\r
.ol-ext-check input {\r
  position: absolute;\r
  opacity: 0;\r
  cursor: pointer;\r
  height: 0;\r
  width: 0;\r
}\r
.ol-ext-check span {\r
  color: rgba(0,60,136,.5);\r
  position: relative;\r
  display: inline-block;\r
  width: 1em;\r
  height: 1em;\r
  margin: -.1em .5em .1em;\r
  background-color: #ccc;\r
  vertical-align: middle;\r
}\r
.ol-ext-check:hover span {\r
  background-color: #999;\r
}\r
.ol-ext-checkbox input:checked ~ span {\r
  background-color: currentColor;\r
}\r
.ol-ext-checkbox input:checked ~ span:before {\r
  content: "";\r
  position: absolute;\r
  width: .5em;\r
  height: .8em;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translateY(-.1em) translate(-50%, -50%) rotate(45deg);\r
          transform: translateY(-.1em) translate(-50%, -50%) rotate(45deg);\r
  -webkit-box-shadow: inset -0.2em -0.2em #fff;\r
          box-shadow: inset -0.2em -0.2em #fff;\r
}\r
\r
.ol-ext-radio span {\r
  width: 1.1em;\r
  height: 1.1em;\r
  border-radius: 50%;\r
}\r
.ol-ext-radio:hover input:checked ~ span {\r
  background-color: #ccc;\r
}\r
.ol-ext-radio input:checked ~ span:before {\r
  content: "";\r
  position: absolute;\r
  width: 50%;\r
  height: 50%;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  border-radius: 50%;\r
  background-color: currentColor;\r
}\r
\r
.ol-collection-list {\r
  margin: 0;\r
  padding: 0;\r
  list-style: none;\r
}\r
.ol-collection-list li {\r
  position: relative;\r
  padding: 0 2em 0 1em;\r
}\r
.ol-collection-list li:hover {\r
  background-color: rgba(0,60,136,.2);\r
}\r
.ol-collection-list li.ol-select {\r
  background-color: rgba(0,60,136,.5);\r
  color: #fff;\r
}\r
\r
.ol-collection-list li .ol-order {\r
  position: absolute;\r
  -ms-touch-action: none;\r
      touch-action: none;\r
  right: 0;\r
  top: 50%;\r
  -webkit-transform: translateY(-50%);\r
          transform: translateY(-50%);\r
  width: 2em;\r
  height: 100%;\r
  cursor: n-resize;\r
}\r
.ol-collection-list li .ol-order:before {\r
  content: '';\r
  position: absolute;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  width: 18px;\r
  height: 2px;\r
  background-color: currentColor;\r
  -webkit-box-shadow: 0 5px, 0 -5px;\r
          box-shadow: 0 5px, 0 -5px;\r
  border-radius: 2px;\r
}\r
\r
.ol-ext-colorpicker.ol-popup {\r
  width: 2em;\r
  height: 1.5em;\r
  background-color: transparent;\r
  background-image: \r
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%), \r
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%);\r
  background-size: 10px 10px;\r
  background-position: 0 -1px, 5px 4px;\r
}\r
\r
.ol-ext-colorpicker .ol-tabbar {\r
  background-color: #eee;\r
  border-bottom: 1px solid #999;\r
  display: none;\r
}\r
.ol-ext-colorpicker.ol-tab .ol-tabbar {\r
  display: block;\r
}\r
\r
.ol-ext-colorpicker .ol-tabbar > div {\r
  display: inline-block;\r
  background-color: #fff;\r
  padding: 0 .5em;\r
  border: 1px solid #999;\r
  border-radius: 2px 2px 0 0;\r
  position: relative;\r
  top: 1px;\r
  cursor: pointer;\r
}\r
.ol-ext-colorpicker .ol-tabbar > div:nth-child(1) {\r
  border-bottom-color: #fff;\r
}\r
.ol-ext-colorpicker.ol-picker-tab .ol-tabbar > div:nth-child(1) {\r
  border-bottom-color: #999;\r
}\r
.ol-ext-colorpicker.ol-picker-tab .ol-tabbar > div:nth-child(2) {\r
  border-bottom-color: #fff;\r
}\r
\r
.ol-ext-colorpicker.ol-popup.ol-tab .ol-popup {\r
  width: 180px;\r
}\r
.ol-ext-colorpicker.ol-tab .ol-palette {\r
  margin: 0 10px;\r
}\r
.ol-ext-colorpicker.ol-tab .ol-container {\r
  display: none;\r
}\r
.ol-ext-colorpicker.ol-tab.ol-picker-tab .ol-container {\r
  display: block;\r
}\r
.ol-ext-colorpicker.ol-tab.ol-picker-tab .ol-palette {\r
  display: none;\r
}\r
\r
.ol-ext-colorpicker.ol-popup .ol-popup {\r
  width: 340px;\r
}\r
\r
.ol-ext-colorpicker.ol-popup .ol-vignet {\r
  content: "";\r
  position: absolute;\r
  width: 100%;\r
  height: 100%;\r
  top: 0;\r
  left: 0;\r
  border: 0;\r
  background-color: currentColor;\r
  pointer-events: none;\r
}\r
\r
.ol-ext-colorpicker .ol-container {\r
  position: relative;\r
  display: inline-block;\r
  vertical-align: top;\r
}\r
.ol-ext-colorpicker .ol-cursor {\r
  pointer-events: none;\r
}\r
\r
.ol-ext-colorpicker .ol-picker {\r
  position: relative;\r
  cursor: crosshair;\r
  width: 150px;\r
  height: 150px;\r
  border: 5px solid #fff;\r
  background-color: currentColor;\r
  background-image: -webkit-gradient(linear, left top, left bottom, from(0), color-stop(#000), to(transparent)),\r
    -webkit-gradient(linear, left top, right top, from(#fff), to(transparent));\r
  background-image: linear-gradient(0, #000, transparent),\r
    linear-gradient(90deg, #fff, transparent);\r
}\r
.ol-ext-colorpicker .ol-picker .ol-cursor {\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  border: 1px solid rgba(0,0,0,.7);\r
  -webkit-box-shadow: 0 0 0 1px rgba(255,255,255,.7);\r
          box-shadow: 0 0 0 1px rgba(255,255,255,.7);\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  width: 10px;\r
  height: 10px;\r
  border-radius: 50%;\r
}\r
\r
.ol-ext-colorpicker .ol-slider {\r
  position: relative;\r
  cursor: crosshair;\r
  background-color: #fff;\r
  height: 10px;\r
  width: 150px;\r
  margin: 5px 0 10px;\r
  border: 5px solid #fff;\r
  border-width: 0 5px;\r
  background-image: \r
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%), \r
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%);\r
  background-size: 10px 10px;\r
  background-position: 0 -1px, 5px 4px;\r
}\r
.ol-ext-colorpicker .ol-slider > div {\r
  width: 100%;\r
  height: 100%;\r
  background-image: linear-gradient(45deg, transparent, #fff);\r
  pointer-events: none;\r
}\r
.ol-ext-colorpicker .ol-slider .ol-cursor {\r
  position: absolute;\r
  width: 4px;\r
  height: 12px;\r
  border: 1px solid #000;\r
  top: 50%;\r
  left: 0;\r
  background: transparent;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
.ol-ext-colorpicker .ol-tint {\r
  position: absolute;\r
  cursor: crosshair;\r
  width: 10px;\r
  height: 150px;\r
  border: 5px solid #fff;\r
  border-width: 5px 0;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  top: 0;\r
  right: 5px;\r
  background-image: -webkit-gradient(linear, left top, left bottom, from(0), color-stop(#f00), color-stop(#f0f), color-stop(#00f), color-stop(#0ff), color-stop(#0f0), color-stop(#ff0), to(#f00));\r
  background-image: linear-gradient(0, #f00, #f0f, #00f, #0ff, #0f0, #ff0, #f00)\r
}\r
.ol-ext-colorpicker .ol-tint .ol-cursor {\r
  position: absolute;\r
  top: 0;\r
  left: 50%;\r
  border: 1px solid #000;\r
  height: 4px;\r
  width: 12px;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
\r
.ol-ext-colorpicker .ol-clear {\r
  position: absolute;\r
  border: 2px solid #999;\r
  right: 4px;\r
  top: 163px;\r
  width: 10px;\r
  height: 10px;\r
}\r
.ol-ext-colorpicker .ol-clear:before,\r
.ol-ext-colorpicker .ol-clear:after {\r
  content: "";\r
  position: absolute;\r
  width: 15px;\r
  height: 2px;\r
  background-color: #999;\r
  top: 50%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%) rotate(45deg);\r
          transform: translate(-50%, -50%) rotate(45deg);\r
}\r
.ol-ext-colorpicker .ol-clear:after {\r
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);\r
          transform: translate(-50%, -50%) rotate(-45deg);\r
}\r
\r
.ol-ext-colorpicker.ol-nopacity .ol-slider,\r
.ol-ext-colorpicker.ol-nopacity .ol-clear {\r
  display: none;\r
}\r
.ol-ext-colorpicker.ol-nopacity .ol-alpha {\r
  display: none;\r
}\r
\r
.ol-ext-colorpicker .ol-rgb {\r
  position: relative;\r
  padding: 5px;\r
  width: 170px;\r
  display: none;\r
}\r
.ol-ext-colorpicker .ol-rgb input {\r
  width: 25%;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  padding: 0 0 0 2px;\r
  border: 1px solid #999;\r
  border-radius: 2px;\r
  font-size: 13px;\r
}\r
.ol-ext-colorpicker .ol-rgb input:nth-child(1) {\r
	background-color: rgba(255,0,0,.1);\r
}\r
.ol-ext-colorpicker .ol-rgb input:nth-child(2) {\r
	background-color: rgba(0,255,0,.1);\r
}\r
.ol-ext-colorpicker .ol-rgb input:nth-child(3) {\r
	background-color: rgba(0,0,255,.12);\r
}\r
\r
.ol-ext-colorpicker button,\r
.ol-ext-colorpicker .ol-txt-color {\r
  font-size: 13px;\r
  margin: 0 5px 5px;\r
  text-align: center;\r
  width: 170px;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
  padding: 0;\r
  border: 1px solid #999;\r
  border-radius: 2px;\r
  display: block;\r
}\r
.ol-ext-colorpicker button {\r
  background-color: #eee;\r
}\r
.ol-ext-colorpicker button:hover {\r
  background-color: #e9e9e9;\r
}\r
\r
.ol-ext-colorpicker .ol-txt-color.ol-error {\r
  background-color: rgba(255,0,0,.2);\r
}\r
\r
.ol-ext-colorpicker .ol-palette {\r
  padding: 2px;\r
  display: inline-block;\r
  width: 152px;\r
}\r
.ol-ext-colorpicker .ol-palette > div {\r
  width: 15px;\r
  height: 15px;\r
  display: inline-block;\r
  background-image: \r
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%), \r
    linear-gradient(45deg, #aaa 25%, transparent 25%, transparent 75%, #aaa 75%);\r
  background-size: 10px 10px;\r
  background-position: 0 0, 5px 5px;\r
  margin: 2px;\r
  -webkit-box-shadow: 0 0 2px 0px #666;\r
          box-shadow: 0 0 2px 0px #666;\r
  border-radius: 1px;\r
  cursor: pointer;\r
  position: relative;\r
}\r
.ol-ext-colorpicker .ol-palette > div:before {\r
  content: "";\r
  position: absolute;\r
  background-color: currentColor;\r
  width: 100%;\r
  height: 100%;\r
}\r
.ol-ext-colorpicker .ol-palette > div.ol-select:after {\r
  content: "";\r
  position: absolute;\r
  width: 6px;\r
  height: 12px;\r
  -webkit-box-shadow: 1px 1px #fff, 2px 2px #000;\r
          box-shadow: 1px 1px #fff, 2px 2px #000;\r
  top: 30%;\r
  left: 50%;\r
  -webkit-transform: translate(-50%, -50%) rotate(45deg);\r
          transform: translate(-50%, -50%) rotate(45deg);\r
}\r
.ol-ext-colorpicker .ol-palette > div:hover {\r
  -webkit-box-shadow: 0 0 2px 1px #d90;\r
          box-shadow: 0 0 2px 1px #d90;\r
}\r
.ol-ext-colorpicker .ol-palette hr {\r
  margin: 0;\r
}\r
\r
.ol-input-popup {\r
  display: inline-block;\r
  position: relative;\r
}\r
.ol-input-popup .ol-popup {\r
  position: absolute;\r
  -webkit-box-shadow: 1px 1px 3px 1px #999;\r
          box-shadow: 1px 1px 3px 1px #999;\r
  background-color: #fff;\r
  z-index: 1;\r
  display: none;\r
  left: -5px;\r
  padding: 0;\r
  margin: 0;\r
  list-style: none;\r
  white-space: nowrap;\r
}\r
.ol-input-popup.ol-hover:hover .ol-popup,\r
.ol-input-popup.ol-focus .ol-popup {\r
  display: block;\r
}\r
.ol-input-popup.ol-right .ol-popup {\r
  left: auto;\r
  right: -5px;\r
}\r
.ol-input-popup.ol-middle .ol-popup {\r
  top: 50%;\r
  -webkit-transform: translateY(-50%);\r
          transform: translateY(-50%);\r
}\r
\r
\r
.ol-input-popup .ol-popup li {\r
  position: relative;\r
  padding: 10px 5px;\r
}\r
\r
.ol-input-popup li:hover {\r
  background-color: #ccc;\r
}\r
.ol-input-popup li.ol-selected {\r
  background-color: #ccc;\r
}\r
\r
.ol-input-popup.ol-fixed:hover .ol-popup,\r
.ol-input-popup.ol-fixed .ol-popup {\r
  position: relative;\r
  left: 0;\r
  -webkit-box-shadow: unset;\r
          box-shadow: unset;\r
  background-color: transparent;\r
  display: inline-block;\r
  vertical-align: middle;\r
}\r
.ol-input-popup.ol-fixed.ol-left .ol-popup {\r
  float: left;\r
}\r
\r
.ol-input-popup > div {\r
  position: relative;\r
  display: inline-block;\r
  vertical-align: middle;\r
  border-radius: 2px;\r
  border: 1px solid #999;\r
  padding: 3px 20px 3px 10px\r
}\r
.ol-input-popup > div:before {\r
  position: absolute;\r
  content: "";\r
  right: 5px;\r
  top: 50%;\r
  border: 5px solid transparent;\r
  border-top: 5px solid #999;\r
}\r
\r
.ol-ext-popup-input {\r
  display: inline-block;\r
  vertical-align: top;\r
}\r
.ol-ext-popup-input.ol-popup {\r
  position: relative;\r
  width: 2em;\r
  height: 1.5em;\r
  display: inline-block;\r
  border: 3px solid #fff;\r
  border-right-width: 1em;\r
  -webkit-box-shadow: 0 0 2px 1px #666;\r
          box-shadow: 0 0 2px 1px #666;\r
  border-radius: 2px;\r
  -webkit-box-sizing: content-box;\r
          box-sizing: content-box;\r
  -webkit-user-select: none;\r
     -moz-user-select: none;\r
      -ms-user-select: none;\r
          user-select: none;\r
  vertical-align: middle;\r
}\r
.ol-ext-popup-input.ol-popup:after {\r
  content: "";\r
  position: absolute;\r
  border: .5em solid #aaa;\r
  border-width: .5em .3em 0;\r
  border-color: #999 transparent;\r
  right: -.8em;\r
  top: 50%;\r
  -webkit-transform: translateY(-50%);\r
          transform: translateY(-50%);\r
  pointer-events: none;\r
}\r
\r
.ol-ext-popup-input * {\r
  -webkit-box-sizing: content-box;\r
          box-sizing: content-box;\r
}\r
\r
.ol-ext-popup-input.ol-popup .ol-popup {\r
  position: absolute;\r
  top: 100%;\r
  min-width: 3em;\r
  min-height: 3em;\r
  left: 0;\r
  -webkit-box-shadow: 1px 1px 3px 1px #999;\r
          box-shadow: 1px 1px 3px 1px #999;\r
  display: block;\r
  background-color: #fff;\r
  display: none;\r
  z-index: 1;\r
}\r
.ol-ext-popup-input.ol-popup .ol-popup.ol-visible {\r
  display: block;\r
}\r
\r
.ol-ext-popup-input.ol-popup-fixed .ol-popup {\r
  position: fixed;\r
  top: auto;\r
  left: auto;\r
}\r
\r
.ol-input-popup.ol-size li {\r
  display: table-cell;\r
  height: 100%;\r
  padding: 5px;\r
  vertical-align: middle;\r
}\r
\r
.ol-input-popup.ol-size li > * {\r
  background-color: #369;\r
  border-radius: 50%;\r
  vertical-align: middle;\r
  width: 1em;\r
  height: 1em;\r
}\r
\r
.ol-input-popup.ol-size li > .ol-option-0 {\r
  position: relative;\r
  width: 1em;\r
  height: 1em;\r
  border: 2px solid currentColor;\r
  color: #aaa;\r
  background-color: transparent;\r
  -webkit-box-sizing: border-box;\r
          box-sizing: border-box;\r
}\r
.ol-input-popup.ol-size li > *:before {\r
  position: absolute;\r
  left: 50%;\r
  top: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
}\r
.ol-input-popup.ol-size li > .ol-option-0:before {\r
  content: "";\r
  width: 1em;\r
  height: 2px;\r
  background-color: #aaa;\r
  -webkit-transform: translate(-50%, -50%) rotate(-45deg);\r
          transform: translate(-50%, -50%) rotate(-45deg);\r
}\r
\r
.ol-input-slider {\r
  display: inline-block;\r
  position: relative;\r
}\r
.ol-input-slider .ol-popup {\r
  position: absolute;\r
  -webkit-box-shadow: 1px 1px 3px 1px #999;\r
          box-shadow: 1px 1px 3px 1px #999;\r
  background-color: #fff;\r
  z-index: 1;\r
  display: none;\r
  left: -5px;\r
}\r
.ol-input-slider.ol-right .ol-popup {\r
  left: auto;\r
  right: -5px;\r
}\r
.ol-input-slider.ol-hover:hover .ol-popup,\r
.ol-input-slider.ol-focus .ol-popup {\r
  display: block;\r
  white-space: nowrap;\r
}\r
.ol-input-slider.ol-hover:hover .ol-popup > *,\r
.ol-input-slider.ol-focus .ol-popup > * {\r
  display: inline-block;\r
  vertical-align: middle;\r
}\r
.ol-input-slider.ol-hover:hover .ol-popup > .ol-before,\r
.ol-input-slider.ol-focus .ol-popup > .ol-before {\r
  margin-left: 10px;\r
}\r
.ol-input-slider.ol-hover:hover .ol-popup > .ol-after,\r
.ol-input-slider.ol-focus .ol-popup > .ol-after {\r
  margin-right: 10px;\r
}\r
.ol-input-slider .ol-slider {\r
  display: inline-block;\r
  vertical-align: middle;\r
  position: relative;\r
  width: 100px;\r
  height: 3px;\r
  border: 0 solid transparent;\r
  border-width: 10px 15px;\r
  -webkit-box-shadow: inset 0 0 0 1px #999;\r
          box-shadow: inset 0 0 0 1px #999;\r
  -webkit-box-sizing: content-box;\r
          box-sizing: content-box;\r
  cursor: pointer;\r
}\r
\r
.ol-input-slider .ol-slider > .ol-cursor {\r
  position: absolute;\r
  width: 5px;\r
  height: 10px;\r
  top: 50%;\r
  -webkit-transform: translate(-50%, -50%);\r
          transform: translate(-50%, -50%);\r
  background-color: #999;\r
  pointer-events: none;\r
}\r
.ol-input-range .ol-slider {\r
  cursor: default;\r
  width: 150px;\r
}\r
.ol-input-range .ol-slider > .ol-cursor {\r
  pointer-events: all;\r
  cursor: pointer;\r
  width: 10px;\r
  border-radius: 50%;\r
  background-color: rgb(0,60,136);\r
}\r
.ol-input-range.ol-moving .ol-slider > .ol-cursor {\r
  pointer-events: none;\r
}\r
.ol-input-range .ol-slider > .ol-back {\r
  position: absolute;\r
  top: 50%;\r
  -webkit-transform: translateY(-50%);\r
          transform: translateY(-50%);\r
  left: 30%;\r
  right: 20%;\r
  height: 100%;\r
  background-color: rgb(0,60,136);\r
  pointer-events: none;\r
}\r
\r
.ol-input-slider.ol-fixed:hover .ol-popup,\r
.ol-input-slider.ol-fixed .ol-popup {\r
  position: relative;\r
  left: 0;\r
  -webkit-box-shadow: unset;\r
          box-shadow: unset;\r
  background-color: transparent;\r
  display: inline-block;\r
  vertical-align: middle;\r
}\r
.ol-input-slider.ol-fixed.ol-left .ol-popup {\r
  float: left;\r
}\r
\r
/* Grow */\r
.ol-input-slider.ol-size .ol-slider {\r
  height: auto;\r
  -webkit-box-shadow: none;\r
          box-shadow: none;\r
}\r
.ol-input-slider.ol-size .ol-slider .ol-back {\r
  width: 0;\r
  color: #aaa;\r
  border: 0 solid transparent;\r
  border-width: 0 0 20px 100px;\r
  border-color: currentColor transparent;\r
  pointer-events: none;\r
}\r
\r
.ol-input-slider.ol-size .ol-slider > .ol-cursor {\r
  width: 2px;\r
  height: calc(100% + 4px);\r
  border-width: 5px 3px;\r
  border-style: solid;\r
  border-color: #f00 transparent;\r
  -o-border-image: initial;\r
     border-image: initial;\r
  background-color: transparent;\r
  -webkit-box-shadow: inset 3px 0px #f00;\r
          box-shadow: inset 3px 0px #f00;\r
}\r
\r
.ol-input-popup.ol-width li {\r
  padding: 5px;\r
}\r
\r
\r
.ol-input-popup.ol-width li > * {\r
  background-color: #369;\r
  width: 100px;\r
  height: 1em;\r
}\r
\r
.ol-input-popup.ol-width li > .ol-option-0 {\r
  position: relative;\r
  height: 1px;\r
  background-image: linear-gradient(90deg,#aaa 2px, transparent 2px);\r
  background-color: transparent;\r
  background-size: 4px;\r
}\r
`],sourceRoot:""}]);const b=p},74096:(t,C,o)=>{o.d(C,{A:()=>s});var e=o(71354),l=o.n(e),i=o(76314),a=o.n(i),A=a()(l());A.push([t.id,`:root,
:host {
  --ol-background-color: white;
  --ol-accent-background-color: #F5F5F5;
  --ol-subtle-background-color: rgba(128, 128, 128, 0.25);
  --ol-partial-background-color: rgba(255, 255, 255, 0.75);
  --ol-foreground-color: #333333;
  --ol-subtle-foreground-color: #666666;
  --ol-brand-color: #00AAFF;
}

.ol-box {
  box-sizing: border-box;
  border-radius: 2px;
  border: 1.5px solid var(--ol-background-color);
  background-color: var(--ol-partial-background-color);
}

.ol-mouse-position {
  top: 8px;
  right: 8px;
  position: absolute;
}

.ol-scale-line {
  background: var(--ol-partial-background-color);
  border-radius: 4px;
  bottom: 8px;
  left: 8px;
  padding: 2px;
  position: absolute;
}

.ol-scale-line-inner {
  border: 1px solid var(--ol-subtle-foreground-color);
  border-top: none;
  color: var(--ol-foreground-color);
  font-size: 10px;
  text-align: center;
  margin: 1px;
  will-change: contents, width;
  transition: all 0.25s;
}

.ol-scale-bar {
  position: absolute;
  bottom: 8px;
  left: 8px;
}

.ol-scale-bar-inner {
  display: flex;
}

.ol-scale-step-marker {
  width: 1px;
  height: 15px;
  background-color: var(--ol-foreground-color);
  float: right;
  z-index: 10;
}

.ol-scale-step-text {
  position: absolute;
  bottom: -5px;
  font-size: 10px;
  z-index: 11;
  color: var(--ol-foreground-color);
  text-shadow: -1.5px 0 var(--ol-partial-background-color), 0 1.5px var(--ol-partial-background-color), 1.5px 0 var(--ol-partial-background-color), 0 -1.5px var(--ol-partial-background-color);
}

.ol-scale-text {
  position: absolute;
  font-size: 12px;
  text-align: center;
  bottom: 25px;
  color: var(--ol-foreground-color);
  text-shadow: -1.5px 0 var(--ol-partial-background-color), 0 1.5px var(--ol-partial-background-color), 1.5px 0 var(--ol-partial-background-color), 0 -1.5px var(--ol-partial-background-color);
}

.ol-scale-singlebar {
  position: relative;
  height: 10px;
  z-index: 9;
  box-sizing: border-box;
  border: 1px solid var(--ol-foreground-color);
}

.ol-scale-singlebar-even {
  background-color: var(--ol-subtle-foreground-color);
}

.ol-scale-singlebar-odd {
  background-color: var(--ol-background-color);
}

.ol-unsupported {
  display: none;
}

.ol-viewport,
.ol-unselectable {
  -webkit-touch-callout: none;
  -webkit-user-select: none;
  -moz-user-select: none;
  user-select: none;
  -webkit-tap-highlight-color: transparent;
}

.ol-viewport canvas {
  all: unset;
}

.ol-selectable {
  -webkit-touch-callout: default;
  -webkit-user-select: text;
  -moz-user-select: text;
  user-select: text;
}

.ol-grabbing {
  cursor: -webkit-grabbing;
  cursor: -moz-grabbing;
  cursor: grabbing;
}

.ol-grab {
  cursor: move;
  cursor: -webkit-grab;
  cursor: -moz-grab;
  cursor: grab;
}

.ol-control {
  position: absolute;
  background-color: var(--ol-subtle-background-color);
  border-radius: 4px;
}

.ol-zoom {
  top: .5em;
  left: .5em;
}

.ol-rotate {
  top: .5em;
  right: .5em;
  transition: opacity .25s linear, visibility 0s linear;
}

.ol-rotate.ol-hidden {
  opacity: 0;
  visibility: hidden;
  transition: opacity .25s linear, visibility 0s linear .25s;
}

.ol-zoom-extent {
  top: 4.643em;
  left: .5em;
}

.ol-full-screen {
  right: .5em;
  top: .5em;
}

.ol-control button {
  display: block;
  margin: 1px;
  padding: 0;
  color: var(--ol-subtle-foreground-color);
  font-weight: bold;
  text-decoration: none;
  font-size: inherit;
  text-align: center;
  height: 1.375em;
  width: 1.375em;
  line-height: .4em;
  background-color: var(--ol-background-color);
  border: none;
  border-radius: 2px;
}

.ol-control button::-moz-focus-inner {
  border: none;
  padding: 0;
}

.ol-zoom-extent button {
  line-height: 1.4em;
}

.ol-compass {
  display: block;
  font-weight: normal;
  will-change: transform;
}

.ol-touch .ol-control button {
  font-size: 1.5em;
}

.ol-touch .ol-zoom-extent {
  top: 5.5em;
}

.ol-control button:hover,
.ol-control button:focus {
  text-decoration: none;
  outline: 1px solid var(--ol-subtle-foreground-color);
  color: var(--ol-foreground-color);
}

.ol-zoom .ol-zoom-in {
  border-radius: 2px 2px 0 0;
}

.ol-zoom .ol-zoom-out {
  border-radius: 0 0 2px 2px;
}

.ol-attribution {
  text-align: right;
  bottom: .5em;
  right: .5em;
  max-width: calc(100% - 1.3em);
  display: flex;
  flex-flow: row-reverse;
  align-items: center;
}

.ol-attribution a {
  color: var(--ol-subtle-foreground-color);
  text-decoration: none;
}

.ol-attribution ul {
  margin: 0;
  padding: 1px .5em;
  color: var(--ol-foreground-color);
  text-shadow: 0 0 2px var(--ol-background-color);
  font-size: 12px;
}

.ol-attribution li {
  display: inline;
  list-style: none;
}

.ol-attribution li:not(:last-child):after {
  content: " ";
}

.ol-attribution img {
  max-height: 2em;
  max-width: inherit;
  vertical-align: middle;
}

.ol-attribution button {
  flex-shrink: 0;
}

.ol-attribution.ol-collapsed ul {
  display: none;
}

.ol-attribution:not(.ol-collapsed) {
  background: var(--ol-partial-background-color);
}

.ol-attribution.ol-uncollapsible {
  bottom: 0;
  right: 0;
  border-radius: 4px 0 0;
}

.ol-attribution.ol-uncollapsible img {
  margin-top: -.2em;
  max-height: 1.6em;
}

.ol-attribution.ol-uncollapsible button {
  display: none;
}

.ol-zoomslider {
  top: 4.5em;
  left: .5em;
  height: 200px;
}

.ol-zoomslider button {
  position: relative;
  height: 10px;
}

.ol-touch .ol-zoomslider {
  top: 5.5em;
}

.ol-overviewmap {
  left: 0.5em;
  bottom: 0.5em;
}

.ol-overviewmap.ol-uncollapsible {
  bottom: 0;
  left: 0;
  border-radius: 0 4px 0 0;
}

.ol-overviewmap .ol-overviewmap-map,
.ol-overviewmap button {
  display: block;
}

.ol-overviewmap .ol-overviewmap-map {
  border: 1px solid var(--ol-subtle-foreground-color);
  height: 150px;
  width: 150px;
}

.ol-overviewmap:not(.ol-collapsed) button {
  bottom: 0;
  left: 0;
  position: absolute;
}

.ol-overviewmap.ol-collapsed .ol-overviewmap-map,
.ol-overviewmap.ol-uncollapsible button {
  display: none;
}

.ol-overviewmap:not(.ol-collapsed) {
  background: var(--ol-subtle-background-color);
}

.ol-overviewmap-box {
  border: 1.5px dotted var(--ol-subtle-foreground-color);
}

.ol-overviewmap .ol-overviewmap-box:hover {
  cursor: move;
}
`,"",{version:3,sources:["webpack://./node_modules/ol/ol.css"],names:[],mappings:"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",sourcesContent:[`:root,
:host {
  --ol-background-color: white;
  --ol-accent-background-color: #F5F5F5;
  --ol-subtle-background-color: rgba(128, 128, 128, 0.25);
  --ol-partial-background-color: rgba(255, 255, 255, 0.75);
  --ol-foreground-color: #333333;
  --ol-subtle-foreground-color: #666666;
  --ol-brand-color: #00AAFF;
}

.ol-box {
  box-sizing: border-box;
  border-radius: 2px;
  border: 1.5px solid var(--ol-background-color);
  background-color: var(--ol-partial-background-color);
}

.ol-mouse-position {
  top: 8px;
  right: 8px;
  position: absolute;
}

.ol-scale-line {
  background: var(--ol-partial-background-color);
  border-radius: 4px;
  bottom: 8px;
  left: 8px;
  padding: 2px;
  position: absolute;
}

.ol-scale-line-inner {
  border: 1px solid var(--ol-subtle-foreground-color);
  border-top: none;
  color: var(--ol-foreground-color);
  font-size: 10px;
  text-align: center;
  margin: 1px;
  will-change: contents, width;
  transition: all 0.25s;
}

.ol-scale-bar {
  position: absolute;
  bottom: 8px;
  left: 8px;
}

.ol-scale-bar-inner {
  display: flex;
}

.ol-scale-step-marker {
  width: 1px;
  height: 15px;
  background-color: var(--ol-foreground-color);
  float: right;
  z-index: 10;
}

.ol-scale-step-text {
  position: absolute;
  bottom: -5px;
  font-size: 10px;
  z-index: 11;
  color: var(--ol-foreground-color);
  text-shadow: -1.5px 0 var(--ol-partial-background-color), 0 1.5px var(--ol-partial-background-color), 1.5px 0 var(--ol-partial-background-color), 0 -1.5px var(--ol-partial-background-color);
}

.ol-scale-text {
  position: absolute;
  font-size: 12px;
  text-align: center;
  bottom: 25px;
  color: var(--ol-foreground-color);
  text-shadow: -1.5px 0 var(--ol-partial-background-color), 0 1.5px var(--ol-partial-background-color), 1.5px 0 var(--ol-partial-background-color), 0 -1.5px var(--ol-partial-background-color);
}

.ol-scale-singlebar {
  position: relative;
  height: 10px;
  z-index: 9;
  box-sizing: border-box;
  border: 1px solid var(--ol-foreground-color);
}

.ol-scale-singlebar-even {
  background-color: var(--ol-subtle-foreground-color);
}

.ol-scale-singlebar-odd {
  background-color: var(--ol-background-color);
}

.ol-unsupported {
  display: none;
}

.ol-viewport,
.ol-unselectable {
  -webkit-touch-callout: none;
  -webkit-user-select: none;
  -moz-user-select: none;
  user-select: none;
  -webkit-tap-highlight-color: transparent;
}

.ol-viewport canvas {
  all: unset;
}

.ol-selectable {
  -webkit-touch-callout: default;
  -webkit-user-select: text;
  -moz-user-select: text;
  user-select: text;
}

.ol-grabbing {
  cursor: -webkit-grabbing;
  cursor: -moz-grabbing;
  cursor: grabbing;
}

.ol-grab {
  cursor: move;
  cursor: -webkit-grab;
  cursor: -moz-grab;
  cursor: grab;
}

.ol-control {
  position: absolute;
  background-color: var(--ol-subtle-background-color);
  border-radius: 4px;
}

.ol-zoom {
  top: .5em;
  left: .5em;
}

.ol-rotate {
  top: .5em;
  right: .5em;
  transition: opacity .25s linear, visibility 0s linear;
}

.ol-rotate.ol-hidden {
  opacity: 0;
  visibility: hidden;
  transition: opacity .25s linear, visibility 0s linear .25s;
}

.ol-zoom-extent {
  top: 4.643em;
  left: .5em;
}

.ol-full-screen {
  right: .5em;
  top: .5em;
}

.ol-control button {
  display: block;
  margin: 1px;
  padding: 0;
  color: var(--ol-subtle-foreground-color);
  font-weight: bold;
  text-decoration: none;
  font-size: inherit;
  text-align: center;
  height: 1.375em;
  width: 1.375em;
  line-height: .4em;
  background-color: var(--ol-background-color);
  border: none;
  border-radius: 2px;
}

.ol-control button::-moz-focus-inner {
  border: none;
  padding: 0;
}

.ol-zoom-extent button {
  line-height: 1.4em;
}

.ol-compass {
  display: block;
  font-weight: normal;
  will-change: transform;
}

.ol-touch .ol-control button {
  font-size: 1.5em;
}

.ol-touch .ol-zoom-extent {
  top: 5.5em;
}

.ol-control button:hover,
.ol-control button:focus {
  text-decoration: none;
  outline: 1px solid var(--ol-subtle-foreground-color);
  color: var(--ol-foreground-color);
}

.ol-zoom .ol-zoom-in {
  border-radius: 2px 2px 0 0;
}

.ol-zoom .ol-zoom-out {
  border-radius: 0 0 2px 2px;
}

.ol-attribution {
  text-align: right;
  bottom: .5em;
  right: .5em;
  max-width: calc(100% - 1.3em);
  display: flex;
  flex-flow: row-reverse;
  align-items: center;
}

.ol-attribution a {
  color: var(--ol-subtle-foreground-color);
  text-decoration: none;
}

.ol-attribution ul {
  margin: 0;
  padding: 1px .5em;
  color: var(--ol-foreground-color);
  text-shadow: 0 0 2px var(--ol-background-color);
  font-size: 12px;
}

.ol-attribution li {
  display: inline;
  list-style: none;
}

.ol-attribution li:not(:last-child):after {
  content: " ";
}

.ol-attribution img {
  max-height: 2em;
  max-width: inherit;
  vertical-align: middle;
}

.ol-attribution button {
  flex-shrink: 0;
}

.ol-attribution.ol-collapsed ul {
  display: none;
}

.ol-attribution:not(.ol-collapsed) {
  background: var(--ol-partial-background-color);
}

.ol-attribution.ol-uncollapsible {
  bottom: 0;
  right: 0;
  border-radius: 4px 0 0;
}

.ol-attribution.ol-uncollapsible img {
  margin-top: -.2em;
  max-height: 1.6em;
}

.ol-attribution.ol-uncollapsible button {
  display: none;
}

.ol-zoomslider {
  top: 4.5em;
  left: .5em;
  height: 200px;
}

.ol-zoomslider button {
  position: relative;
  height: 10px;
}

.ol-touch .ol-zoomslider {
  top: 5.5em;
}

.ol-overviewmap {
  left: 0.5em;
  bottom: 0.5em;
}

.ol-overviewmap.ol-uncollapsible {
  bottom: 0;
  left: 0;
  border-radius: 0 4px 0 0;
}

.ol-overviewmap .ol-overviewmap-map,
.ol-overviewmap button {
  display: block;
}

.ol-overviewmap .ol-overviewmap-map {
  border: 1px solid var(--ol-subtle-foreground-color);
  height: 150px;
  width: 150px;
}

.ol-overviewmap:not(.ol-collapsed) button {
  bottom: 0;
  left: 0;
  position: absolute;
}

.ol-overviewmap.ol-collapsed .ol-overviewmap-map,
.ol-overviewmap.ol-uncollapsible button {
  display: none;
}

.ol-overviewmap:not(.ol-collapsed) {
  background: var(--ol-subtle-background-color);
}

.ol-overviewmap-box {
  border: 1.5px dotted var(--ol-subtle-foreground-color);
}

.ol-overviewmap .ol-overviewmap-box:hover {
  cursor: move;
}
`],sourceRoot:""}]);const s=A},97544:(t,C,o)=>{var e=o(85072),l=o.n(e),i=o(97825),a=o.n(i),A=o(77659),s=o.n(A),c=o(55056),p=o.n(c),d=o(10540),b=o.n(d),m=o(41113),E=o.n(m),n=o(33011),r={};r.styleTagTransform=E(),r.setAttributes=p(),r.insert=s().bind(null,"head"),r.domAPI=a(),r.insertStyleElement=b();var B=l()(n.A,r),g=n.A&&n.A.locals?n.A.locals:void 0},25045:(t,C,o)=>{var e=o(85072),l=o.n(e),i=o(97825),a=o.n(i),A=o(77659),s=o.n(A),c=o(55056),p=o.n(c),d=o(10540),b=o.n(d),m=o(41113),E=o.n(m),n=o(74096),r={};r.styleTagTransform=E(),r.setAttributes=p(),r.insert=s().bind(null,"head"),r.domAPI=a(),r.insertStyleElement=b();var B=l()(n.A,r),g=n.A&&n.A.locals?n.A.locals:void 0},69029:t=>{t.exports="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAACE1BMVEX///8A//8AgICA//8AVVVAQID///8rVVVJtttgv98nTmJ2xNgkW1ttyNsmWWZmzNZYxM4gWGgeU2JmzNNr0N1Rwc0eU2VXxdEhV2JqytQeVmMhVmNoydUfVGUgVGQfVGQfVmVqy9hqy9dWw9AfVWRpydVry9YhVmMgVGNUw9BrytchVWRexdGw294gVWQgVmUhVWPd4N6HoaZsy9cfVmQgVGRrytZsy9cgVWQgVWMgVWRsy9YfVWNsy9YgVWVty9YgVWVry9UgVWRsy9Zsy9UfVWRsy9YgVWVty9YgVWRty9Vsy9aM09sgVWRTws/AzM0gVWRtzNYgVWRuy9Zsy9cgVWRGcHxty9bb5ORbxdEgVWRty9bn6OZTws9mydRfxtLX3Nva5eRix9NFcXxOd4JPeINQeIMiVmVUws9Vws9Vw9BXw9BYxNBaxNBbxNBcxdJexdElWWgmWmhjyNRlx9IqXGtoipNpytVqytVryNNrytZsjZUuX210k5t1y9R2zNR3y9V4lp57zth9zdaAnKOGoaeK0NiNpquV09mesrag1tuitbmj1tuj19uktrqr2d2svcCu2d2xwMO63N+7x8nA3uDC3uDFz9DK4eHL4eLN4eIyYnDX5OM5Z3Tb397e4uDf4uHf5uXi5ePi5+Xj5+Xk5+Xm5+Xm6OY6aHXQ19fT4+NfhI1Ww89gx9Nhx9Nsy9ZWw9Dpj2abAAAAWnRSTlMAAQICAwQEBgcIDQ0ODhQZGiAiIyYpKywvNTs+QklPUlNUWWJjaGt0dnd+hIWFh4mNjZCSm6CpsbW2t7nDzNDT1dje5efr7PHy9PT29/j4+Pn5+vr8/f39/f6DPtKwAAABTklEQVR4Xr3QVWPbMBSAUTVFZmZmhhSXMjNvkhwqMzMzMzPDeD+xASvObKePPa+ffHVl8PlsnE0+qPpBuQjVJjno6pZpSKXYl7/bZyFaQxhf98hHDKEppwdWIW1frFnrxSOWHFfWesSEWC6R/P4zOFrix3TzDFLlXRTR8c0fEEJ1/itpo7SVO9Jdr1DVxZ0USyjZsEY5vZfiiAC0UoTGOrm9PZLuRl8X+Dq1HQtoFbJZbv61i+Poblh/97TC7n0neCcK0ETNUrz1/xPHf+DNAW9Ac6t8O8WH3Vp98f5lCaYKAOFZMLyHL4Y0fe319idMNgMMp+zWVSybUed/+/h7I4wRAG1W6XDy4XmjR9HnzvDRZXUAYDFOhC1S/Hh+fIXxen+eO+AKqbs+wAo30zDTDvDxKoJN88sjUzDFAvBzEUGFsnADoIvAJzoh2BZ8sner+Ke/vwECuQAAAABJRU5ErkJggg=="}}]);

//# sourceMappingURL=geomapPanel.4cd3fb8fabc339209d05.js.map
(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[8888],{27777:t=>{var s=`<div ng-transclude class="gf-form-query-content"></div>

`,a=s,e="public/app/angular/panel/partials/query_editor_row.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},37791:t=>{var s=`<datasource-http-settings-next on-change="onChange" datasourceconfig="current" showaccessoptions="showAccessOption" defaulturl="suggestUrl" showforwardoauthidentityoption="showForwardOAuthIdentityOption" securesocksdsproxyenabled="secureSocksDSProxyEnabled">
`,a=s,e="public/app/angular/partials/http_settings_next.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},3867:t=>{var s=`<div class="gf-form-group">
  <div class="gf-form">
    <h6>TLS/SSL Auth Details</h6>
    <info-popover mode="header">TLS/SSL certificates are encrypted and stored in the Grafana database.</info-popover>
  </div>
  <div ng-if="current.jsonData.tlsAuthWithCACert">
    <div class="gf-form-inline">
      <div class="gf-form gf-form--v-stretch"><label class="gf-form-label width-7">CA Cert</label></div>
      <div class="gf-form gf-form--grow" ng-if="!current.secureJsonFields.tlsCACert">
        <textarea rows="7" class="gf-form-input gf-form-textarea" ng-model="current.secureJsonData.tlsCACert" placeholder="Begins with -----BEGIN CERTIFICATE-----"></textarea>
      </div>

      <div class="gf-form" ng-if="current.secureJsonFields.tlsCACert">
        <input type="text" class="gf-form-input max-width-12" disabled="disabled" value="configured">
        <button type="reset" aria-label="Reset CA Cert" class="btn btn-secondary gf-form-btn" ng-click="current.secureJsonFields.tlsCACert = false">
          reset
        </button>
      </div>
    </div>
  </div>

  <div ng-if="current.jsonData.tlsAuth">
    <div class="gf-form-inline">
      <div class="gf-form gf-form--v-stretch"><label class="gf-form-label width-7">Client Cert</label></div>
      <div class="gf-form gf-form--grow" ng-if="!current.secureJsonFields.tlsClientCert">
        <textarea rows="7" class="gf-form-input gf-form-textarea" ng-model="current.secureJsonData.tlsClientCert" placeholder="Begins with -----BEGIN CERTIFICATE-----" required></textarea>
      </div>
      <div class="gf-form" ng-if="current.secureJsonFields.tlsClientCert">
        <input type="text" class="gf-form-input max-width-12" disabled="disabled" value="configured">
        <button class="btn btn-secondary gf-form-btn" aria-label="Reset Client Cert" type="reset" ng-click="current.secureJsonFields.tlsClientCert = false">
          reset
        </button>
      </div>
    </div>

    <div class="gf-form-inline">
      <div class="gf-form gf-form--v-stretch"><label class="gf-form-label width-7">Client Key</label></div>
      <div class="gf-form gf-form--grow" ng-if="!current.secureJsonFields.tlsClientKey">
        <textarea rows="7" class="gf-form-input gf-form-textarea" ng-model="current.secureJsonData.tlsClientKey" placeholder="Begins with -----BEGIN RSA PRIVATE KEY-----" required></textarea>
      </div>
      <div class="gf-form" ng-if="current.secureJsonFields.tlsClientKey">
        <input type="text" class="gf-form-input max-width-12" disabled="disabled" value="configured">
        <button class="btn btn-secondary gf-form-btn" type="reset" aria-label="Reset Client Key" ng-click="current.secureJsonFields.tlsClientKey = false">
          reset
        </button>
      </div>
    </div>
  </div>
</div>
`,a=s,e="public/app/angular/partials/tls_auth_settings.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},33417:t=>{var s=`<page-header model="ctrl.navModel"></page-header>

<div class="page-container page-body">

  <div class="grafana-info-box span8">
    Grafana is a multi-tenant system where most can be configured per organization. These
    admin pages are for server admins where you can manage orgs, & all users across all orgs.
  </div>

</div>

<footer>
`,a=s,e="public/app/features/admin/partials/admin_home.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},51959:t=>{var s=`<page-header model="navModel"></page-header>

<div class="page-container page-body">
  <h3 class="page-sub-heading">Edit Organization</h3>

  <form name="orgDetailsForm" class="gf-form-group">
    <div class="gf-form">
      <span class="gf-form-label width-10">Name</span>
      <input type="text" required ng-model="org.name" class="gf-form-input max-width-14">
    </div>

    <div class="gf-form-button-row">
      <button type="submit" class="btn btn-primary" ng-click="update()" ng-show="!createMode">Update</button>
    </div>
  </form>

  <h3 class="page-heading">Organization Users</h3>

  <table class="filter-table">
    <tr>
      <th>Username</th>
      <th>Email</th>
      <th>Role</th>
      <th></th>
    </tr>
    <tr ng-repeat="orgUser in orgUsers">
      <td>{{orgUser.login}}</td>
      <td>{{orgUser.email}}</td>
      <td>
        <div class="gf-form">
          <span class="gf-form-select-wrapper">
            <select type="text" ng-model="orgUser.role" class="gf-form-input max-width-8" ng-options="f for f in ['Viewer', 'Editor', 'Admin']" ng-change="updateOrgUser(orgUser)">
            </select>
          </span>
        </div>
      </td>
      <td style="width: 1%">
        <a ng-click="removeOrgUser(orgUser)" class="btn btn-danger btn-small">
          <icon name="'times'" style="margin-bottom: 0;"></icon>
        </a>
      </td>
    </tr>
  </table>
</div>

<footer>
`,a=s,e="public/app/features/admin/partials/edit_org.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},5735:t=>{var s=`<page-header model="ctrl.navModel"></page-header>

<div class="page-container page-body">
	<table class="filter-table form-inline">
		<thead>
			<tr>
				<th>Name</th>
				<th>Value</th>
			</tr>
		</thead>
		<tbody>
			<tr>
				<td>Total dashboards</td>
				<td>{{ctrl.stats.dashboards}}</td>
			</tr>
			<tr>
				<td>Total users</td>
				<td>{{ctrl.stats.users}}</td>
			</tr>
			<tr>
				<td>Active users (seen last 14 days)</td>
				<td>{{ctrl.stats.activeUsers}}</td>
			</tr>
			<tr>
				<td>Total organizations</td>
				<td>{{ctrl.stats.orgs}}</td>
			</tr>
			<tr>
				<td>Total datasources</td>
				<td>{{ctrl.stats.datasources}}</td>
			</tr>
			<tr>
				<td>Total playlists</td>
				<td>{{ctrl.stats.playlists}}</td>
			</tr>
			<tr>
				<td>Total snapshots</td>
				<td>{{ctrl.stats.snapshots}}</td>
			</tr>
			<tr>
				<td>Total dashboard tags</td>
				<td>{{ctrl.stats.tags}}</td>
			</tr>
			<tr>
				<td>Total starred dashboards</td>
				<td>{{ctrl.stats.stars}}</td>
			</tr>
      <tr>
				<td>Total alerts</td>
				<td>{{ctrl.stats.alerts}}</td>
			</tr>
		</tbody>
	</table>
</div>
`,a=s,e="public/app/features/admin/partials/stats.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},705:t=>{var s=`<page-header model="ctrl.navModel"></page-header>

<div class="page-container page-body">

	<h3 class="page-heading">Buttons</h3>

	<div class="tab-pane">
		<div ng-repeat="variant in ctrl.buttonVariants" class="row">
			<div ng-repeat="btnSize in ctrl.buttonSizes" class="style-guide-button-list p-a-2 col-md-4">
				<button ng-repeat="buttonName in ctrl.buttonNames" class="btn btn{{variant}}{{buttonName}} {{btnSize}}">
					btn{{variant}}{{buttonName}}
				</button>
			</div>
		</div>
	</div>

	<h3 class="page-heading">Forms</h3>

	<div class="gf-form-inline">
		<div class="gf-form">
			<label class="gf-form-label">Label</label>
			<input type="text" class="gf-form-input">
		</div>
	</div>

</div>

`,a=s,e="public/app/features/admin/partials/styleguide.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},82123:t=>{var s=`
<div class="graph-annotation">
	<div class="graph-annotation__header">
		<div class="graph-annotation__user" bs-tooltip="'Created by {{ctrl.login}}'">
		</div>

		<div class="graph-annotation__title">
			<span ng-if="!ctrl.event.id">Add annotation</span>
			<span ng-if="ctrl.event.id">Edit annotation</span>
		</div>

    <div class="graph-annotation__time">{{ctrl.timeFormated}}</div>
	</div>

	<form name="ctrl.form" class="graph-annotation__body text-center">
		<div style="display: inline-block">
			<div class="gf-form gf-form--v-stretch">
				<span class="gf-form-label width-7">Description</span>
				<textarea class="gf-form-input width-20" rows="2" ng-model="ctrl.event.text" placeholder="Description"></textarea>
			</div>

			<div class="gf-form">
				<span class="gf-form-label width-7">Tags</span>
				<bootstrap-tagsinput ng-model="ctrl.event.tags" tagclass="label label-tag" placeholder="add tags">
				</bootstrap-tagsinput>
			</div>

			<div class="gf-form-button-row">
				<button type="submit" class="btn btn-primary" ng-click="ctrl.save()">Save</button>
				<button ng-if="ctrl.event.id && ctrl.canDelete()" type="submit" class="btn btn-danger" ng-click="ctrl.delete()">Delete</button>
				<a class="btn-text" ng-click="ctrl.close();">Cancel</a>
			</div>
		</div>
	</form>
</div>
`,a=s,e="public/app/features/annotations/partials/event_editor.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},3378:t=>{var s=`<div class="modal-body" ng-cloak>
  <div class="modal-header">
    <h2 class="modal-header-title">
      <icon name="'{{icon}}'" size="'lg'"></icon>
      <span class="p-l-1">
        {{title}}
      </span>
    </h2>

    <a class="modal-header-close" ng-click="dismiss();">
      <icon name="'times'"></icon>
    </a>
  </div>

  <div class="modal-content text-center">
    <div class="confirm-modal-text">
      {{text}}
      <div ng-if="text2 && text2htmlBind" class="confirm-modal-text2" ng-bind-html="text2"></div>
      <div ng-if="text2 && !text2htmlBind" class="confirm-modal-text2">{{text2}}</div>
    </div>

    <div class="modal-content-confirm-text" ng-if="confirmText">
      <input type="text" class="gf-form-input width-16" style="display: inline-block;" placeholder="Type {{confirmText}} to confirm" ng-model="confirmInput" ng-change="updateConfirmText(confirmInput)">
    </div>

    <div class="confirm-modal-buttons">
      <button ng-show="onAltAction" type="button" class="btn btn-primary" ng-click="dismiss();onAltAction();">
        {{altActionText}}
      </button>
      <button ng-show="onConfirm" type="button" class="btn btn-danger" ng-click="onConfirm();dismiss();" ng-disabled="!confirmTextValid" give-focus="true" aria-label="{{selectors.delete}}">
        {{yesText}}
      </button>
      <button type="button" class="btn btn-inverse" ng-click="dismiss()">{{noText}}</button>
    </div>
  </div>
</div>
`,a=s,e="public/app/partials/confirm_modal.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},21585:t=>{var s=`<div class="modal-header">
  <button type="button" class="close" data-dismiss="modal" aria-hidden="true">\xD7</button>
  <h3>{{modal.title}}</h3>
</div>
<div class="modal-body">

  <div ng-bind-html="modal.body"></div>

</div>
<div class="modal-footer">
  <button type="button" class="btn btn-danger" ng-click="dismiss()">Close</button>
</div>`,a=s,e="public/app/partials/modal.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},96599:t=>{var s=`<page-header model="navModel"></page-header>

<div class="page-container page-body">
	<div class="signup">
		<h3 class="p-b-1">Reset password</h3>

		<div ng-if="ldapEnabled || authProxyEnabled">
			You cannot reset password when LDAP or Auth Proxy authentication is enabled.
		</div>
		<div ng-if="disableLoginForm">
			You cannot reset password when login form is disabled.
		</div>
		<form name="sendResetForm" class="login-form gf-form-group" ng-show="mode === 'send'" ng-hide="ldapEnabled || authProxyEnabled || disableLoginForm || mode === 'reset'">
			<div class="gf-form">
					<span class="gf-form-label width-7">User</span>
					<input type="text" name="username" class="gf-form-input max-width-14" required ng-model="formModel.userOrEmail" placeholder="email or username">
			</div>
			<div class="gf-form-button-row">
				<button type="submit" class="btn btn-primary" ng-click="sendResetEmail();" ng-disabled="!sendResetForm.$valid">
					Reset Password
				</button>
				<a href="login" class="btn btn-inverse">
					Back
				</a>

			</div>
		</form>
		<div ng-show="mode === 'email-sent'">
			An email with a reset link has been sent to the email address. <br>
			You should receive it shortly.
			<div class="p-t-1">
				<a href="login" class="btn btn-primary p-t-1">
					Login
				</a>
			</div>
		</div>
		<form name="resetForm" class="login-form gf-form-group" ng-show="mode === 'reset'">
			<div class="gf-form">
				<span class="gf-form-label width-9">New Password</span>
				<input type="password" name="NewPassword" class="gf-form-input max-width-14" required ng-minlength="4" ng-model="formModel.newPassword" placeholder="password" watch-change="formModel.newPassword = inputValue;">
			</div>
			<div class="gf-form">
				<span class="gf-form-label width-9">Confirm Password</span>
				<input type="password" name="ConfirmPassword" class="gf-form-input max-width-14" required ng-minlength="4" ng-model="formModel.confirmPassword" placeholder="confirm password">
			</div>
			<div class="signup__password-strength">
				<password-strength password="formModel.newPassword"></password-strength>
			</div>
			<div class="gf-form-button-row">
				<button type="submit" class="btn btn-primary" ng-click="submitReset();" ng-disabled="!resetForm.$valid">
					Reset Password
				</button>
			</div>
		</form>
	</div>
</div>

<footer>
`,a=s,e="public/app/partials/reset_password.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},11774:t=>{var s=`<page-header model="navModel"></page-header>

<div class="page-container page-body">
	<h3 class="page-sub-heading">Hello {{greeting}}.</h3>

	<div class="modal-tagline p-b-2">
		<em>{{invitedBy}}</em> has invited you to join Grafana and the organization <span class="highlight-word">{{contextSrv.user.orgName}}</span><br>Please complete the following and choose a password to accept your invitation and continue:
	</div>

	<form name="inviteForm" class="login-form gf-form-group">
		<div class="gf-form">
			<span class="gf-form-label width-7">Email</span>
			<input type="email" name="email" class="gf-form-input max-width-21" required ng-model="formModel.email" placeholder="Email">
		</div>
		<div class="gf-form">
			<span class="gf-form-label width-7">Name</span>
			<input type="text" name="name" class="gf-form-input max-width-21" ng-model="formModel.name" placeholder="Name (optional)">
		</div>
		<div class="gf-form">
			<span class="gf-form-label width-7">Username</span>
			<input type="text" name="username" class="gf-form-input max-width-21" required ng-model="formModel.username" placeholder="Username">
		</div>
		<div class="gf-form">
			<span class="gf-form-label width-7">Password</span>
			<input type="password" name="password" class="gf-form-input max-width-21" required ng-model="formModel.password" id="inputPassword" placeholder="password">
		</div>

		<div class="gf-form-button-row">
			<button type="submit" class="btn btn-primary" ng-click="submit();" ng-disable="!inviteForm.$valid">
				Sign Up
			</button>
		</div>
	</form>
</div>

<footer>
`,a=s,e="public/app/partials/signup_invited.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},49563:t=>{var s=`<div class="editor-row">
  <div class="gf-form-group" ng-repeat="yaxis in ctrl.panel.yaxes">
    <h5 class="section-heading" ng-show="$index === 0">Left Y</h5>
    <h5 class="section-heading" ng-show="$index === 1">Right Y</h5>

    <gf-form-switch class="gf-form" label="Show" label-class="width-6" checked="yaxis.show" on-change="ctrl.render()"></gf-form-switch>

    <div ng-if="yaxis.show">
      <div class="gf-form gf-form--grow">
        <label class="gf-form-label width-6">
          Unit
				  <info-popover mode="right-normal">The default unit used when not defined by the datasource or in the Fields or Overrides configuration.</info-popover>
        </label>
        <unit-picker on-change="ctrl.setUnitFormat(yaxis)" value="yaxis.format" class="flex-grow-1">
      </div>
    </div>

    <div class="gf-form">
      <label for="yaxis-scale-select-{{$index}}" class="gf-form-label width-6">Scale</label>
      <div class="gf-form-select-wrapper max-width-20">
        <select id="yaxis-scale-select-{{$index}}" class="gf-form-input" ng-model="yaxis.logBase" ng-options="v as k for (k, v) in ctrl.logScales" ng-change="ctrl.render()"></select>
      </div>
    </div>

    <div class="gf-form">
      <label class="gf-form-label width-6">Y-Min</label>
      <input type="text" class="gf-form-input" placeholder="auto" empty-to-null ng-model="yaxis.min" ng-change="ctrl.render()" ng-model-onblur>
    </div>
    <div class="gf-form">
      <label class="gf-form-label width-6">Y-Max</label>
      <input type="text" class="gf-form-input" placeholder="auto" empty-to-null ng-model="yaxis.max" ng-change="ctrl.render()" ng-model-onblur>
    </div>

    <div ng-if="yaxis.show">
      <div class="gf-form">
        <label class="gf-form-label width-6">Decimals</label>
        <input type="number" class="gf-form-input width-5" placeholder="auto" empty-to-null bs-tooltip="'Override automatic decimal precision for y-axis'" data-placement="right" ng-model="yaxis.decimals" ng-change="ctrl.render()" ng-model-onblur>
      </div>

      <div class="gf-form">
        <label for="yaxis-label-select-{{$index}}" class="gf-form-label width-6">Label</label>
        <input id="yaxis-label-select-{{$index}}" type="text" class="gf-form-input max-width-20" ng-model="yaxis.label" ng-change="ctrl.render()" ng-model-onblur>
      </div>
    </div>
  </div>

  <div class="gf-form-group">
    <h5 class="section-heading">Y-Axes</h5>
    <gf-form-switch class="gf-form" label="Align Y-Axes" label-class="width-6" switch-class="width-5" checked="ctrl.panel.yaxis.align" on-change="ctrl.render()"></gf-form-switch>
    <div class="gf-form" ng-show="ctrl.panel.yaxis.align">
      <label class="gf-form-label width-6">
        Level
      </label>
      <input type="number" class="gf-form-input width-6" placeholder="0" ng-model="ctrl.panel.yaxis.alignLevel" ng-change="ctrl.render()" ng-model-onblur bs-tooltip="'Alignment of Y-axes are based on this value, starting from Y=0'" data-placement="right">
    </div>
  </div>

  <div class="gf-form-group" aria-label="{{ctrl.selectors.xAxisSection}}">
    <h5 class="section-heading">X-Axis</h5>
    <gf-form-switch class="gf-form" label="Show" label-class="width-6" checked="ctrl.panel.xaxis.show" on-change="ctrl.render()"></gf-form-switch>

    <div class="gf-form">
      <label for="xaxis-mode-select" class="gf-form-label width-6">Mode</label>
      <div class="gf-form-select-wrapper max-width-15">
        <select id="xaxis-mode-select" class="gf-form-input" ng-model="ctrl.panel.xaxis.mode" ng-options="v as k for (k, v) in ctrl.xAxisModes" ng-change="ctrl.xAxisModeChanged()"></select>
      </div>
    </div>

    <!-- Series mode -->
    <div class="gf-form" ng-if="ctrl.panel.xaxis.mode === 'series'">
      <label class="gf-form-label width-6">Value</label>
      <metric-segment-model property="ctrl.panel.xaxis.values[0]" options="ctrl.xAxisStatOptions" on-change="ctrl.xAxisValueChanged()" custom="false" css-class="width-10" select-mode="true"></metric-segment-model>
    </div>

    <!-- Histogram mode -->
    <div class="gf-form" ng-if="ctrl.panel.xaxis.mode === 'histogram'">
      <label class="gf-form-label width-6">Buckets</label>
      <input type="number" class="gf-form-input max-width-8" ng-model="ctrl.panel.xaxis.buckets" placeholder="auto" ng-change="ctrl.render()" ng-model-onblur bs-tooltip="'Number of buckets'" data-placement="right">
    </div>

    <div class="gf-form-inline" ng-if="ctrl.panel.xaxis.mode === 'histogram'">
      <div class="gf-form">
        <label class="gf-form-label width-6">X-Min</label>
        <input type="number" class="gf-form-input width-5" placeholder="auto" empty-to-null ng-model="ctrl.panel.xaxis.min" ng-change="ctrl.render()" ng-model-onblur>
      </div>
      <div class="gf-form">
        <label class="gf-form-label width-6">X-Max</label>
        <input type="number" class="gf-form-input width-5" placeholder="auto" empty-to-null ng-model="ctrl.panel.xaxis.max" ng-change="ctrl.render()" ng-model-onblur>
      </div>
    </div>
  </div>
</div>
`,a=s,e="public/app/plugins/panel/graph/axes_editor.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},92986:t=>{var s=`<div class="gf-form-group">
  <div class="grafana-info-box">
    <h5>Migration</h5>
    <p>
      Consider switching to the Time series visualization type. It is a more capable and performant version of this panel.
    </p>
    <p>
      <button class="btn btn-primary" ng-click="ctrl.migrateToReact()">
        Migrate
      </button>
    </p>
    <p>Some features are not supported in the new panel yet.</p>
  </div>

  <gf-form-switch class="gf-form" label="Bars" label-class="width-8" checked="ctrl.panel.bars" on-change="ctrl.render()"></gf-form-switch>

  <gf-form-switch class="gf-form" label="Lines" label-class="width-8" checked="ctrl.panel.lines" on-change="ctrl.render()"></gf-form-switch>

  <div class="gf-form" ng-if="ctrl.panel.lines">
    <label class="gf-form-label width-8" for="linewidth-select-input">Line width</label>
    <div class="gf-form-select-wrapper max-width-5">
      <select id="linewidth-select-input" class="gf-form-input" ng-model="ctrl.panel.linewidth" ng-options="f for f in [0,1,2,3,4,5,6,7,8,9,10]" ng-change="ctrl.render()"></select>
    </div>
  </div>

  <gf-form-switch ng-disabled="!ctrl.panel.lines" class="gf-form" label="Staircase" label-class="width-8" checked="ctrl.panel.steppedLine" on-change="ctrl.render()"></gf-form-switch>

  <div class="gf-form" ng-if="ctrl.panel.lines">
    <label class="gf-form-label width-8" for="fill-select-input">Area fill</label>
    <div class="gf-form-select-wrapper max-width-5">
      <select id="fill-select-input" class="gf-form-input" ng-model="ctrl.panel.fill" ng-options="f for f in [0,1,2,3,4,5,6,7,8,9,10]" ng-change="ctrl.render()"></select>
    </div>
  </div>

  <div class="gf-form" ng-if="ctrl.panel.lines && ctrl.panel.fill">
    <label class="gf-form-label width-8">Fill gradient</label>
    <div class="gf-form-select-wrapper max-width-5">
      <select class="gf-form-input" ng-model="ctrl.panel.fillGradient" ng-options="f for f in [0,1,2,3,4,5,6,7,8,9,10]" ng-change="ctrl.render()"></select>
    </div>
  </div>

  <gf-form-switch class="gf-form" label="Points" label-class="width-8" checked="ctrl.panel.points" on-change="ctrl.render()"></gf-form-switch>

  <div class="gf-form" ng-if="ctrl.panel.points">
    <label class="gf-form-label width-8" for="pointradius-select-input">Point Radius</label>
    <div class="gf-form-select-wrapper max-width-5">
      <select id="pointradius-select-input" class="gf-form-input" ng-model="ctrl.panel.pointradius" ng-options="f for f in [0.5,1,2,3,4,5,6,7,8,9,10]" ng-change="ctrl.render()"></select>
    </div>
  </div>

  <gf-form-switch class="gf-form" label="Alert thresholds" label-class="width-8" checked="ctrl.panel.options.alertThreshold" on-change="ctrl.render()"></gf-form-switch>
</div>

<div class="gf-form-group">
  <h5 class="section-heading">Stacking and null value</h5>
  <gf-form-switch class="gf-form" label="Stack" label-class="width-7" checked="ctrl.panel.stack" on-change="ctrl.render()">
  </gf-form-switch>
  <gf-form-switch class="gf-form" ng-show="ctrl.panel.stack" label="Percent" label-class="width-7" checked="ctrl.panel.percentage" on-change="ctrl.render()">
  </gf-form-switch>
  <div class="gf-form">
    <label class="gf-form-label width-7" for="null-value-select-input">Null value</label>
    <div class="gf-form-select-wrapper">
      <select id="null-value-select-input" class="gf-form-input max-width-9" ng-model="ctrl.panel.nullPointMode" ng-options="f for f in ['connected', 'null', 'null as zero']" ng-change="ctrl.render()"></select>
    </div>
  </div>
</div>

<div class="gf-form-group">
  <h5 class="section-heading">Hover tooltip</h5>
  <div class="gf-form">
    <label class="gf-form-label width-9" for="tooltip-mode-select-input">Mode</label>
    <div class="gf-form-select-wrapper max-width-8">
      <select id="tooltip-mode-select-input" class="gf-form-input" ng-model="ctrl.panel.tooltip.shared" ng-options="f.value as f.text for f in [{text: 'All series', value: true}, {text: 'Single', value: false}]" ng-change="ctrl.render()"></select>
    </div>
  </div>
  <div class="gf-form">
    <label class="gf-form-label width-9" for="tooltip-sort-select-input">Sort order</label>
    <div class="gf-form-select-wrapper max-width-8">
      <select id="tooltip-sort-select-input" class="gf-form-input" ng-model="ctrl.panel.tooltip.sort" ng-options="f.value as f.text for f in [{text: 'None', value: 0}, {text: 'Increasing', value: 1}, {text: 'Decreasing', value: 2}]" ng-change="ctrl.render()"></select>
    </div>
  </div>
  <div class="gf-form" ng-show="ctrl.panel.stack">
    <label class="gf-form-label width-9">Stacked value</label>
    <div class="gf-form-select-wrapper max-width-8">
      <select class="gf-form-input" ng-model="ctrl.panel.tooltip.value_type" ng-options="f for f in ['cumulative','individual']" ng-change="ctrl.render()"></select>
    </div>
  </div>
</div>
`,a=s,e="public/app/plugins/panel/graph/tab_display.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},15607:t=>{var s=`<div class="editor-row">
  <div class="section gf-form-group">
    <h5 class="section-heading">Options</h5>
    <gf-form-switch class="gf-form" label="Show" label-class="width-7" checked="ctrl.panel.legend.show" on-change="ctrl.render()" aria-label="gpl show legend">
    </gf-form-switch>
    <gf-form-switch class="gf-form" label="As Table" label-class="width-7" checked="ctrl.panel.legend.alignAsTable" on-change="ctrl.render()">
    </gf-form-switch>
    <gf-form-switch class="gf-form" label="To the right" label-class="width-7" checked="ctrl.panel.legend.rightSide" on-change="ctrl.render()">
    </gf-form-switch>
    <div ng-if="ctrl.panel.legend.rightSide" class="gf-form">
      <label class="gf-form-label width-7">Width</label>
      <input type="number" class="gf-form-input max-width-5" placeholder="250" bs-tooltip="'Set a min-width for the legend side table/block'" data-placement="right" ng-model="ctrl.panel.legend.sideWidth" ng-change="ctrl.render()" ng-model-onblur>
    </div>
  </div>

  <div class="section gf-form-group">
    <h5 class="section-heading">Values</h5>

    <div class="gf-form-inline">
      <gf-form-switch class="gf-form" label="Min" label-class="width-7" checked="ctrl.panel.legend.min" on-change="ctrl.legendValuesOptionChanged()">
      </gf-form-switch>

      <gf-form-switch class="gf-form max-width-12" label="Max" label-class="width-7" switch-class="max-width-5" checked="ctrl.panel.legend.max" on-change="ctrl.legendValuesOptionChanged()">
      </gf-form-switch>
    </div>

    <div class="gf-form-inline">
      <gf-form-switch class="gf-form" label="Avg" label-class="width-7" checked="ctrl.panel.legend.avg" on-change="ctrl.legendValuesOptionChanged()">
      </gf-form-switch>

      <gf-form-switch class="gf-form max-width-12" label="Current" label-class="width-7" switch-class="max-width-5" checked="ctrl.panel.legend.current" on-change="ctrl.legendValuesOptionChanged()">
      </gf-form-switch>
    </div>

    <div class="gf-form-inline">
      <gf-form-switch class="gf-form" label="Total" label-class="width-7" checked="ctrl.panel.legend.total" on-change="ctrl.legendValuesOptionChanged()">
      </gf-form-switch>

      <div class="gf-form">
        <label class="gf-form-label width-7">Decimals</label>
        <input type="number" class="gf-form-input width-5" placeholder="auto" bs-tooltip="'Override automatic decimal precision for legend and tooltips'" data-placement="right" ng-model="ctrl.panel.decimals" ng-change="ctrl.render()" ng-model-onblur>
      </div>
    </div>
  </div>

  <div class="section gf-form-group">
    <h5 class="section-heading">Hide series</h5>
    <gf-form-switch class="gf-form" label="With only nulls" label-class="width-7" checked="ctrl.panel.legend.hideEmpty" on-change="ctrl.render()">
    </gf-form-switch>
    <gf-form-switch class="gf-form" label="With only zeros" label-class="width-7" checked="ctrl.panel.legend.hideZero" on-change="ctrl.render()">
    </gf-form-switch>
  </div>
</div>
`,a=s,e="public/app/plugins/panel/graph/tab_legend.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},84013:t=>{var s=`<div class="graph-series-override" ng-repeat="override in ctrl.panel.seriesOverrides" ng-controller="SeriesOverridesCtrl">
  <div class="gf-form">
    <label class="gf-form-label">Alias or regex</label>
    <input type="text" ng-model="override.alias" bs-typeahead="getSeriesNames" ng-blur="ctrl.render()" data-min-length="0" data-items="100" class="gf-form-input width-15" placeholder="For regex use /pattern/">
    <label class="gf-form-label pointer" ng-click="ctrl.removeSeriesOverride(override)">
      <icon name="'trash-alt'"></icon>
    </label>
  </div>
  <div class="graph-series-override__properties">
    <div class="gf-form" ng-repeat="option in currentOverrides">
      <label class="gf-form-label gf-form-label--grow">
        <span ng-show="option.propertyName === 'color'">
          Color: <icon name="'circle'" type="'mono'" ng-style="{color:option.value}"></icon>
        </span>
        <span ng-show="option.propertyName !== 'color'"> {{ option.name }}: {{ option.value }} </span>
        <icon name="'times'" size="'sm'" ng-click="removeOverride(option)" style="margin-right: 4px;cursor: pointer;"></icon>
      </label>
    </div>
    <div class="gf-form">
      <span class="dropdown" dropdown-typeahead2="overrideMenu" dropdown-typeahead-on-select="setOverride($item, $subItem)" button-template-class="gf-form-label"></span>
    </div>
  </div>
</div>
<div class="gf-form-button-row">
  <button class="btn btn-inverse" ng-click="ctrl.addSeriesOverride()">
    <icon name="'plus'"></icon>&nbsp;Add series override
  </button>
</div>
`,a=s,e="public/app/plugins/panel/graph/tab_series_overrides.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},96942:t=>{var s=`<graph-threshold-form panel-ctrl="ctrl"></graph-threshold-form>
`,a=s,e="public/app/plugins/panel/graph/tab_thresholds.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},68869:t=>{var s=`<graph-time-region-form panel-ctrl="ctrl"></graph-time-region-form>
`,a=s,e="public/app/plugins/panel/graph/tab_time_regions.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},95127:t=>{var s=`<div class="gf-form-group">
  <p class="muted" ng-show="ctrl.disabled">
    Visual thresholds options <strong>disabled.</strong> Visit the Alert tab to update your thresholds.<br>
    To re-enable thresholds, the alert rule must be deleted from this panel.
  </p>
  <div ng-class="{'thresholds-form-disabled': ctrl.disabled}">
    <div class="gf-form-inline" ng-repeat="threshold in ctrl.panel.thresholds">
      <div class="gf-form">
        <label class="gf-form-label">T{{$index+1}}</label>
      </div>

      <div class="gf-form">
        <div class="gf-form-select-wrapper">
          <select class="gf-form-input" ng-model="threshold.op" ng-options="f for f in ['gt', 'lt']" ng-change="ctrl.render()" ng-disabled="ctrl.disabled"></select>
        </div>
        <input type="number" ng-model="threshold.value" class="gf-form-input width-8" ng-change="ctrl.render()" placeholder="value" ng-disabled="ctrl.disabled">
      </div>

      <div class="gf-form">
        <label class="gf-form-label">Color</label>
        <div class="gf-form-select-wrapper">
          <select class="gf-form-input" ng-model="threshold.colorMode" ng-options="f for f in ['custom', 'critical', 'warning', 'ok']" ng-change="ctrl.onThresholdTypeChange($index)" ng-disabled="ctrl.disabled">
          </select>
        </div>
      </div>

      <gf-form-switch class="gf-form" label="Fill" checked="threshold.fill" on-change="ctrl.render()" ng-disabled="ctrl.disabled"></gf-form-switch>

      <div class="gf-form" ng-if="threshold.fill && threshold.colorMode === 'custom'">
        <label class="gf-form-label">Fill color</label>
        <span class="gf-form-label">
          <color-picker color="threshold.fillColor" on-change="ctrl.onFillColorChange($index)"></color-picker>
        </span>
      </div>

      <gf-form-switch class="gf-form" label="Line" checked="threshold.line" on-change="ctrl.render()" ng-disabled="ctrl.disabled"></gf-form-switch>

      <div class="gf-form" ng-if="threshold.line && threshold.colorMode === 'custom'">
        <label class="gf-form-label">Line color</label>
        <span class="gf-form-label">
          <color-picker color="threshold.lineColor" on-change="ctrl.onLineColorChange($index)"></color-picker>
        </span>
      </div>

      <div class="gf-form">
        <label class="gf-form-label">Y-Axis</label>
        <div class="gf-form-select-wrapper">
          <select class="gf-form-input" ng-model="threshold.yaxis" ng-init="threshold.yaxis = threshold.yaxis === 'left' || threshold.yaxis === 'right' ? threshold.yaxis : 'left'" ng-options="f for f in ['left', 'right']" ng-change="ctrl.render()" ng-disabled="ctrl.disabled">
          </select>
        </div>
      </div>

      <div class="gf-form">
        <label class="gf-form-label">
          <a class="pointer" ng-click="ctrl.removeThreshold($index)" ng-disabled="ctrl.disabled">
            <icon name="'trash-alt'"></icon>
          </a>
        </label>
      </div>
    </div>

    <div class="gf-form-button-row">
      <button class="btn btn-inverse" ng-click="ctrl.addThreshold()" ng-disabled="ctrl.disabled">
        <icon name="'plus'"></icon>&nbsp;Add threshold
      </button>
    </div>
  </div>
</div>
`,a=s,e="public/app/plugins/panel/graph/thresholds_form.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},57254:t=>{var s=`<div class="gf-form-group">
  <div class="gf-form-inline" ng-repeat="timeRegion in ctrl.panel.timeRegions">
    <div class="gf-form">
      <label class="gf-form-label">T{{$index+1}}</label>
    </div>

    <div class="gf-form">
      <label class="gf-form-label">From</label>
      <div class="gf-form-select-wrapper">
        <select class="gf-form-input width-6" ng-model="timeRegion.fromDayOfWeek" ng-options="f.d as f.value for f in [{d: undefined, value: 'Any'}, {d:1, value: 'Mon'}, {d:2, value: 'Tue'}, {d:3, value: 'Wed'}, {d:4, value: 'Thu'}, {d:5, value: 'Fri'}, {d:6, value: 'Sat'}, {d:7, value: 'Sun'}]" ng-change="ctrl.render()"></select>
      </div>
      <input type="text" ng-maxlength="5" ng-model="timeRegion.from" class="gf-form-input width-5" ng-change="ctrl.render()" placeholder="hh:mm">
      <label class="gf-form-label">To</label>
      <div class="gf-form-select-wrapper">
        <select class="gf-form-input width-6" ng-model="timeRegion.toDayOfWeek" ng-options="f.d as f.value for f in [{d: undefined, value: 'Any'}, {d:1, value: 'Mon'}, {d:2, value: 'Tue'}, {d:3, value: 'Wed'}, {d:4, value: 'Thu'}, {d:5, value: 'Fri'}, {d:6, value: 'Sat'}, {d:7, value: 'Sun'}]" ng-change="ctrl.render()"></select>
      </div>
      <input type="text" ng-maxlength="5" ng-model="timeRegion.to" class="gf-form-input width-5" ng-change="ctrl.render()" placeholder="hh:mm">
    </div>

    <div class="gf-form">
      <label class="gf-form-label">Color</label>
      <div class="gf-form-select-wrapper">
        <select class="gf-form-input" ng-model="timeRegion.colorMode" ng-options="f.key as f.value for f in ctrl.colorModes" ng-change="ctrl.render()">
        </select>
      </div>
    </div>

    <gf-form-switch class="gf-form" label="Fill" checked="timeRegion.fill" on-change="ctrl.render()"></gf-form-switch>

    <div class="gf-form" ng-if="timeRegion.fill && timeRegion.colorMode === 'custom'">
      <label class="gf-form-label">Fill color</label>
      <span class="gf-form-label">
        <color-picker color="timeRegion.fillColor" on-change="ctrl.onFillColorChange($index)"></color-picker>
      </span>
    </div>

    <gf-form-switch class="gf-form" label="Line" checked="timeRegion.line" on-change="ctrl.render()"></gf-form-switch>

    <div class="gf-form" ng-if="timeRegion.line && timeRegion.colorMode === 'custom'">
      <label class="gf-form-label">Line color</label>
      <span class="gf-form-label">
        <color-picker color="timeRegion.lineColor" on-change="ctrl.onLineColorChange($index)"></color-picker>
      </span>
    </div>

    <div class="gf-form">
      <label class="gf-form-label">
        <a class="pointer" ng-click="ctrl.removeTimeRegion($index)">
          <icon name="'trash-alt'"></icon>
        </a>
      </label>
    </div>
  </div>

  <div class="gf-form-button-row">
    <button class="btn btn-inverse" ng-click="ctrl.addTimeRegion()">
      <icon name="'plus'"></icon>&nbsp;Add time region<tip>All configured time regions refer to UTC time</tip>
    </button>
  </div>
</div>
`,a=s,e="public/app/plugins/panel/graph/time_regions_form.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},43860:t=>{var s=`<div class="editor-row">
  <div class="section gf-form-group">
    <h5 class="section-heading">Y Axis</h5>
    <div class="gf-form">
      <label class="gf-form-label width-8">Unit</label>
      <unit-picker on-change="editor.setUnitFormat" value="ctrl.panel.yAxis.format" class="width-12"></unit-picker>
    </div>
    <div ng-if="ctrl.panel.dataFormat == 'timeseries'">
      <div class="gf-form">
        <label class="gf-form-label width-8">Scale</label>
        <div class="gf-form-select-wrapper width-12">
          <select class="gf-form-input" ng-model="ctrl.panel.yAxis.logBase" ng-options="v as k for (k, v) in editor.logScales" ng-change="ctrl.refresh()"></select>
        </div>
      </div>
      <div class="gf-form">
        <label class="gf-form-label width-8">Y-Min</label>
        <input type="text" class="gf-form-input width-12" placeholder="auto" empty-to-null ng-model="ctrl.panel.yAxis.min" ng-change="ctrl.render()" ng-model-onblur>
      </div>
      <div class="gf-form">
        <label class="gf-form-label width-8">Y-Max</label>
        <input type="text" class="gf-form-input width-12" placeholder="auto" empty-to-null ng-model="ctrl.panel.yAxis.max" ng-change="ctrl.render()" ng-model-onblur>
      </div>
    </div>
    <div class="gf-form">
      <label class="gf-form-label width-8">Width</label>
      <input type="text" class="gf-form-input width-12" placeholder="auto" empty-to-null ng-model="ctrl.panel.yAxis.width" ng-change="ctrl.render()" ng-model-onblur>
    </div>
    <div class="gf-form">
      <label class="gf-form-label width-8">Decimals</label>
      <input type="number" class="gf-form-input width-12" placeholder="auto" data-placement="right" bs-tooltip="'Override automatic decimal precision for axis.'" ng-model="ctrl.panel.yAxis.decimals" ng-change="ctrl.render()" ng-model-onblur>
    </div>
    <div class="gf-form" ng-if="ctrl.panel.dataFormat == 'tsbuckets'">
      <label class="gf-form-label width-8">Bucket bound</label>
      <div class="gf-form-select-wrapper max-width-12">
        <select class="gf-form-input" ng-model="ctrl.panel.yBucketBound" ng-options="v as k for (k, v) in editor.yBucketBoundModes" ng-change="ctrl.render()" data-placement="right" bs-tooltip="'Use series label as an upper or lower bucket bound.'">
        </select>
      </div>
    </div>
    <gf-form-switch ng-if="ctrl.panel.dataFormat == 'tsbuckets'" class="gf-form" label-class="width-8" label="Reverse order" checked="ctrl.panel.reverseYBuckets" on-change="ctrl.refresh()">
    </gf-form-switch>
  </div>

  <div class="section gf-form-group" ng-if="ctrl.panel.dataFormat == 'timeseries'">
    <h5 class="section-heading">Buckets</h5>
    <div class="gf-form-inline">
      <div class="gf-form">
        <label class="gf-form-label width-4">Y Axis</label>
      </div>
      <div class="gf-form" ng-show="ctrl.panel.yAxis.logBase === 1">
        <label class="gf-form-label width-5">Buckets</label>
        <input type="number" class="gf-form-input width-5" placeholder="auto" data-placement="right" bs-tooltip="'Number of buckets for Y axis.'" ng-model="ctrl.panel.yBucketNumber" ng-change="ctrl.refresh()" ng-model-onblur>
      </div>
      <div class="gf-form" ng-show="ctrl.panel.yAxis.logBase === 1">
        <label class="gf-form-label width-4">Size</label>
        <input type="number" class="gf-form-input width-5" placeholder="auto" data-placement="right" bs-tooltip="'Size of bucket. Has priority over Buckets option.'" ng-model="ctrl.panel.yBucketSize" ng-change="ctrl.refresh()" ng-model-onblur>
      </div>
      <div class="gf-form" ng-show="ctrl.panel.yAxis.logBase !== 1">
        <label class="gf-form-label width-10">Split Factor</label>
        <input type="number" class="gf-form-input width-9" placeholder="1" data-placement="right" bs-tooltip="'For log scales only. By default Y values is splitted by integer powers of log base (1, 2, 4, 8, 16, ... for log2). This option allows to split each default bucket into specified number of buckets.'" ng-model="ctrl.panel.yAxis.splitFactor" ng-change="ctrl.refresh()" ng-model-onblur>
        
      </div>
    </div>
    <div class="gf-form-inline">
      <div class="gf-form">
        <label class="gf-form-label width-4">X Axis</label>
      </div>
      <div class="gf-form">
        <label class="gf-form-label width-5">Buckets</label>
        <input type="number" class="gf-form-input width-5" placeholder="auto" data-placement="right" bs-tooltip="'Number of buckets for X axis.'" ng-model="ctrl.panel.xBucketNumber" ng-change="ctrl.refresh()" ng-model-onblur>
      </div>
      <div class="gf-form">
        <label class="gf-form-label width-4">Size</label>
        <input type="text" class="gf-form-input width-5" placeholder="auto" data-placement="right" bs-tooltip="'Size of bucket. Number or interval (10s, 5m, 1h, etc). Supported intervals: ms, s, m, h, d, w, M, y. Has priority over Buckets option.'" ng-model="ctrl.panel.xBucketSize" ng-change="ctrl.refresh()" ng-model-onblur>
      </div>
    </div>
  </div>

  <div class="section gf-form-group">
    <h5 class="section-heading">Data format</h5>
    <div class="gf-form">
      <label class="gf-form-label width-5">Format</label>
      <div class="gf-form-select-wrapper max-width-15">
        <select class="gf-form-input" ng-model="ctrl.panel.dataFormat" ng-options="v as k for (k, v) in editor.dataFormats" ng-change="ctrl.render()" data-placement="right" bs-tooltip="'Time series: create heatmap from regular time series. <br>Time series buckets: use histogram data returned from data source. Each series represents bucket which upper/lower bound is a series label.'">
        </select>
      </div>
    </div>
  </div>
</div>
`,a=s,e="public/app/plugins/panel/heatmap/partials/axes_editor.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},58733:t=>{var s=`<div class="editor-row">
  <div class="section gf-form-group">
    <h5 class="section-heading">Colors</h5>
    <div class="gf-form">
      <label class="gf-form-label width-9">Mode</label>
      <div class="gf-form-select-wrapper width-8">
        <select class="input-small gf-form-input" ng-model="ctrl.panel.color.mode" ng-options="s for s in ctrl.colorModes" ng-change="ctrl.render()"></select>
      </div>
    </div>

    <div ng-show="ctrl.panel.color.mode === 'opacity'">
      <div class="gf-form">
        <label class="gf-form-label width-9">Color</label>
        <span class="gf-form-label">
          <color-picker color="ctrl.panel.color.cardColor" on-change="ctrl.onCardColorChange"></color-picker>
        </span>
      </div>
      <div class="gf-form">
        <label class="gf-form-label width-9">Scale</label>
        <div class="gf-form-select-wrapper width-8">
          <select class="input-small gf-form-input" ng-model="ctrl.panel.color.colorScale" ng-options="s for s in ctrl.opacityScales" ng-change="ctrl.render()"></select>
        </div>
      </div>
      <div class="gf-form" ng-if="ctrl.panel.color.colorScale === 'sqrt'">
        <label class="gf-form-label width-9">Exponent</label>
        <input type="number" class="gf-form-input width-8" placeholder="auto" data-placement="right" bs-tooltip="''" ng-model="ctrl.panel.color.exponent" ng-change="ctrl.refresh()" ng-model-onblur>
      </div>
    </div>

    <div ng-show="ctrl.panel.color.mode === 'spectrum'">
      <div class="gf-form">
        <label class="gf-form-label width-9">Scheme</label>
        <div class="gf-form-select-wrapper width-8">
          <select class="input-small gf-form-input" ng-model="ctrl.panel.color.colorScheme" ng-options="s.value as s.name for s in ctrl.colorSchemes" ng-change="ctrl.render()"></select>
        </div>
      </div>
    </div>

    <div class="gf-form">
      <color-legend></color-legend>
    </div>
  </div>

  <div class="section gf-form-group">
    <h5 class="section-heading">Color scale</h5>
    <div class="gf-form">
      <label class="gf-form-label width-8">Min</label>
      <input type="number" ng-model="ctrl.panel.color.min" class="gf-form-input width-5" placeholder="auto" data-placement="right" bs-tooltip="''" ng-change="ctrl.refresh()" ng-model-onblur>
    </div>
    <div class="gf-form">
      <label class="gf-form-label width-8">Max</label>
      <input type="number" ng-model="ctrl.panel.color.max" class="gf-form-input width-5" placeholder="auto" data-placement="right" bs-tooltip="''" ng-change="ctrl.refresh()" ng-model-onblur>
    </div>
  </div>

  <div class="section gf-form-group">
    <h5 class="section-heading">Legend</h5>
    <gf-form-switch class="gf-form" label-class="width-8" label="Show legend" checked="ctrl.panel.legend.show" on-change="ctrl.render()">
    </gf-form-switch>
  </div>

  <div class="section gf-form-group">
    <h5 class="section-heading">Buckets</h5>
    <gf-form-switch class="gf-form" label-class="width-8" label="Hide zero" checked="ctrl.panel.hideZeroBuckets" on-change="ctrl.render()">
    </gf-form-switch>
    <div class="gf-form">
      <label class="gf-form-label width-8">Space</label>
      <input type="number" class="gf-form-input width-5" placeholder="auto" data-placement="right" bs-tooltip="''" ng-model="ctrl.panel.cards.cardPadding" ng-change="ctrl.refresh()" ng-model-onblur>
    </div>
    <div class="gf-form">
      <label class="gf-form-label width-8">Round</label>
      <input type="number" class="gf-form-input width-5" placeholder="auto" data-placement="right" bs-tooltip="''" ng-model="ctrl.panel.cards.cardRound" ng-change="ctrl.refresh()" ng-model-onblur>
    </div>
  </div>

  <div class="section gf-form-group">
    <h5 class="section-heading">Tooltip</h5>
    <gf-form-switch class="gf-form" label-class="width-8" label="Show tooltip" checked="ctrl.panel.tooltip.show" on-change="ctrl.render()">
    </gf-form-switch>
    <div ng-if="ctrl.panel.tooltip.show">
      <gf-form-switch class="gf-form" label-class="width-8" label="Histogram" checked="ctrl.panel.tooltip.showHistogram" on-change="ctrl.render()">
      </gf-form-switch>
      <div class="gf-form">
        <label class="gf-form-label width-8">Decimals</label>
        <input type="number" class="gf-form-input width-5" placeholder="auto" data-placement="right" bs-tooltip="'Max decimal precision for tooltip.'" ng-model="ctrl.panel.tooltipDecimals" ng-change="ctrl.render()" ng-model-onblur>
      </div>
    </div>
  </div>
</div>
`,a=s,e="public/app/plugins/panel/heatmap/partials/display_editor.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},92929:t=>{var s=`<div class="edit-tab-content" ng-repeat="style in editor.panel.styles">
  <div class="gf-form-group">
    <h5 class="section-heading">Options</h5>
    <div class="gf-form-inline">
      <div class="gf-form gf-form--grow">
        <label class="gf-form-label width-8">Name pattern</label>
        <input type="text" placeholder="Name or regex" class="gf-form-input max-width-15" ng-model="style.pattern" bs-tooltip="'Specify regex using /my.*regex/ syntax'" bs-typeahead="editor.getColumnNames" ng-blur="editor.render()" data-min-length="0" data-items="100" ng-model-onblur data-placement="right">
      </div>
    </div>
    <div class="gf-form gf-form--grow" ng-if="style.type !== 'hidden'">
      <label class="gf-form-label width-8">Column Header</label>
      <input type="text" class="gf-form-input max-width-15" ng-model="style.alias" ng-change="editor.render()" ng-model-onblur placeholder="Override header label">
    </div>
    <gf-form-switch class="gf-form" label-class="width-8" label="Render value as link" checked="style.link" on-change="editor.render()"></gf-form-switch>
  </div>

  <div class="gf-form-group">
    <h5 class="section-heading">Type</h5>

    <div class="gf-form gf-form--grow">
      <label class="gf-form-label width-8">Type</label>
      <div class="gf-form-select-wrapper width-16">
        <select class="gf-form-input" ng-model="style.type" ng-options="c.value as c.text for c in editor.columnTypes" ng-change="editor.render()"></select>
      </div>
    </div>
    <div class="gf-form gf-form--grow" ng-if="style.type === 'date'">
      <label class="gf-form-label width-8">Date Format</label>
      <gf-form-dropdown model="style.dateFormat" css-class="gf-form-input width-16" lookup-text="true" get-options="editor.dateFormats" on-change="editor.render()" allow-custom="true">
      </gf-form-dropdown>
    </div>

    <div ng-if="style.type === 'string'">
      <gf-form-switch class="gf-form" label-class="width-8" ng-if="style.type === 'string'" label="Sanitize HTML" checked="style.sanitize" on-change="editor.render()"></gf-form-switch>
    </div>

    <div ng-if="style.type !== 'hidden'">
      <div class="gf-form gf-form--grow">
        <label class="gf-form-label width-8">Align</label>
        <gf-form-dropdown model="style.align" css-class="gf-form-input" lookup-text="true" get-options="editor.alignTypes" allow-custom="false" on-change="editor.render()" allow-custom="false">
      </div>
    </div>

    <div ng-if="style.type === 'string'">
      <gf-form-switch class="gf-form" label-class="width-10" ng-if="style.type === 'string'" label="Preserve Formatting" checked="style.preserveFormat" on-change="editor.render()"></gf-form-switch>
    </div>

    <div ng-if="style.type === 'number'">
      <div class="gf-form">
        <label class="gf-form-label width-8">Unit</label>
        <unit-picker on-change="editor.setUnitFormat(style)" value="style.unit" width="16"></unit-picker>
      </div>
      <div class="gf-form">
        <label class="gf-form-label width-8">Decimals</label>
        <input type="number" class="gf-form-input width-4" data-placement="right" ng-model="style.decimals" ng-change="editor.render()" ng-model-onblur>
      </div>
    </div>
  </div>

  <div class="gf-form-group" ng-if="style.type === 'string'">
    <h5 class="section-heading">Value Mappings</h5>
    <div class="editor-row">
      <div class="gf-form-group">
        <div class="gf-form">
          <span class="gf-form-label">
            Type
          </span>
          <div class="gf-form-select-wrapper">
            <select class="gf-form-input" ng-model="style.mappingType" ng-options="c.value as c.text for c in editor.mappingTypes" ng-change="editor.render()"></select>
          </div>
        </div>
        <div class="gf-form-group" ng-if="style.mappingType==1">
          <div class="gf-form" ng-repeat="map in style.valueMaps">
            <span class="gf-form-label">
              <icon name="'times'" ng-click="editor.removeValueMap(style, $index)"></icon>
            </span>
            <input type="text" class="gf-form-input max-width-6" ng-model="map.value" placeholder="Value" ng-blur="editor.render()">
            <label class="gf-form-label">
              <icon name="'arrow-right'"></icon>
            </label>
            <input type="text" class="gf-form-input max-width-8" ng-model="map.text" placeholder="Text" ng-blur="editor.render()">
          </div>
          <div class="gf-form">
            <label class="gf-form-label">
              <a class="pointer" ng-click="editor.addValueMap(style)"><icon name="'plus'"></icon></a>
            </label>
          </div>
        </div>
        <div class="gf-form-group" ng-if="style.mappingType==2">
          <div class="gf-form" ng-repeat="rangeMap in style.rangeMaps">
            <span class="gf-form-label">
              <icon name="'times'" ng-click="editor.removeRangeMap(style, $index)"></icon>
            </span>
            <span class="gf-form-label">From</span>
            <input type="text" ng-model="rangeMap.from" class="gf-form-input max-width-6" ng-blur="editor.render()">
            <span class="gf-form-label">To</span>
            <input type="text" ng-model="rangeMap.to" class="gf-form-input max-width-6" ng-blur="editor.render()">
            <span class="gf-form-label">Text</span>
            <input type="text" ng-model="rangeMap.text" class="gf-form-input max-width-8" ng-blur="editor.render()">
          </div>
          <div class="gf-form">
            <label class="gf-form-label">
              <a class="pointer" ng-click="editor.addRangeMap(style)"><icon name="'plus'"></icon></a>
            </label>
          </div>
        </div>
      </div>
    </div>
  </div>

  <div class="gf-form-group" ng-if="['number', 'string'].indexOf(style.type) !== -1">
    <h5 class="section-heading">Thresholds</h5>
    <div class="gf-form">
      <label class="gf-form-label width-8">Thresholds
        <tip>Comma separated values</tip>
      </label>
      <input type="text" class="gf-form-input width-10" ng-model="style.thresholds" placeholder="50,80" ng-blur="editor.render()" array-join>
    </div>
    <div class="gf-form">
      <label class="gf-form-label width-8">Color Mode</label>
      <div class="gf-form-select-wrapper width-10">
        <select class="gf-form-input" ng-model="style.colorMode" ng-options="c.value as c.text for c in editor.colorModes" ng-change="editor.render()"></select>
      </div>
    </div>
    <div class="gf-form">
      <label class="gf-form-label width-8">Colors</label>
      <span class="gf-form-label">
        <color-picker color="style.colors[0]" on-change="editor.onColorChange(style, 0)"></color-picker>
      </span>
      <span class="gf-form-label">
        <color-picker color="style.colors[1]" on-change="editor.onColorChange(style, 1)"></color-picker>
      </span>
      <span class="gf-form-label">
        <color-picker color="style.colors[2]" on-change="editor.onColorChange(style, 2)"></color-picker>
      </span>
      <div class="gf-form-label">
        <a class="pointer" ng-click="editor.invertColorOrder($index)">Invert</a>
      </div>
    </div>
  </div>

  <div class="section gf-form-group" ng-if="style.link">
    <h5 class="section-heading">Link</h5>
    <div class="gf-form">
      <label class="gf-form-label width-9">
        Url
        <info-popover mode="right-normal">
          <p>Specify an URL (relative or absolute)</p>
          <span>
            Use special variables to specify cell values:
            <br>
            <em>\${__cell}</em> refers to current cell value
            <br>
            <em>\${__cell_n}</em> refers to Nth column value in current row. Column indexes are started from 0. For
            instance, <em>\${__cell_1}</em> refers to second column's value.
            <br>
            <em>\${__cell:raw}</em> disables all encoding. Useful if the value is a complete URL. By default values are
            URI encoded.
          </span>
        </info-popover>
      </label>
      <input type="text" class="gf-form-input width-29" ng-model="style.linkUrl" ng-blur="editor.render()" ng-model-onblur data-placement="right">
    </div>
    <div class="gf-form">
      <label class="gf-form-label width-9">
        Tooltip
        <info-popover mode="right-normal">
          <p>Specify text for link tooltip.</p>
          <span>
            This title appears when user hovers pointer over the cell with link. Use the same variables as for URL.
          </span>
        </info-popover>
      </label>
      <input type="text" class="gf-form-input width-29" ng-model="style.linkTooltip" ng-blur="editor.render()" ng-model-onblur data-placement="right">
    </div>
    <gf-form-switch class="gf-form" label-class="width-9" label="Open in new tab" checked="style.linkTargetBlank"></gf-form-switch>
  </div>

  <div class="clearfix"></div>
  <div class="gf-form-group">
    <button class="btn btn-danger btn-small" ng-click="editor.removeColumnStyle(style)">
      <icon name="'trash-alt'"></icon> Remove Rule
    </button>
  </div>

  <hr>
</div>

<button class="btn btn-inverse" ng-click="editor.addColumnStyle()">
  <icon name="'plus'"></icon>&nbsp;Add column style
</button>
`,a=s,e="public/app/plugins/panel/table-old/column_options.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},46359:t=>{var s=`<div class="gf-form-group">
  <div class="grafana-info-box">
    <h5>Table migration</h5>
    <p>
      This panel is deprecated. Please migrate to the new Table panel.
    </p>
    <p>
      <button class="btn btn-primary" ng-click="ctrl.migrateToPanel('table')">
        Migrate to Table panel
      </button>
    </p>
    <p><b>NOTE:</b> Sorting is not persisted after migration.</p>
    <p ng-if="ctrl.panelHasRowColorMode">
      <b>NOTE:</b> Row color mode is no longer supported and will fallback to cell color mode.
    </p>
    <p ng-if="ctrl.panelHasLinks">
      <b>NOTE:</b> Links that specify cell values will need to be updated manually after migration.
    </p>
  </div>
  <h5 class="section-heading">Data</h5>
  <div class="gf-form gf-form--grow">
    <label class="gf-form-label width-8">Table Transform</label>
    <div class="gf-form-select-wrapper">
      <select class="gf-form-input" ng-model="editor.panel.transform" ng-options="k as v.description for (k, v) in editor.transformers" ng-change="editor.transformChanged()"></select>
    </div>
  </div>
  <div class="gf-form-inline">
    <div class="gf-form">
      <label class="gf-form-label width-8">Columns</label>
    </div>
    <div class="gf-form" ng-repeat="column in editor.panel.columns">
      <label class="gf-form-label">
        <icon name="'times'" ng-click="editor.removeColumn(column)"></icon>
        <span>{{ column.text }}</span>
      </label>
    </div>
    <div class="gf-form" ng-show="editor.canSetColumns">
      <metric-segment segment="editor.addColumnSegment" get-options="editor.getColumnOptions()" on-change="editor.addColumn()"></metric-segment>
    </div>
    <div class="gf-form" ng-hide="editor.canSetColumns">
      <label class="gf-form-label">
        Auto
        <info-popover mode="right-normal" ng-if="editor.columnsHelpMessage">
          {{ editor.columnsHelpMessage }}
        </info-popover>
      </label>
    </div>
  </div>
</div>

<div class="gf-form-group">
  <h5 class="section-heading">Paging</h5>
  <div class="gf-form">
    <label class="gf-form-label width-8">Rows per page</label>
    <input type="number" class="gf-form-input width-7" placeholder="100" data-placement="right" ng-model="editor.panel.pageSize" ng-change="editor.render()" ng-model-onblur>
  </div>
  <div class="gf-form max-width-17">
    <label class="gf-form-label width-8">Font size</label>
    <div class="gf-form-select-wrapper width-7">
      <select class="gf-form-input" ng-model="editor.panel.fontSize" ng-options="f for f in editor.fontSizes" ng-change="editor.render()"></select>
    </div>
  </div>
</div>
`,a=s,e="public/app/plugins/panel/table-old/editor.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},88826:t=>{var s=`<div class="table-panel-container">
  <div class="table-panel-header-bg" ng-show="ctrl.table.rows.length"></div>
  <div class="table-panel-scroll" ng-show="ctrl.table.rows.length">
    <table class="table-panel-table">
      <thead>
        <tr>
          <th ng-repeat="col in ctrl.table.columns" ng-if="!col.hidden">
            <div class="table-panel-table-header-inner pointer" ng-click="ctrl.toggleColumnSort(col, $index)">
              {{col.title}}
              <span class="table-panel-table-header-controls" ng-if="col.sort">
                <icon name="'angle-down'" ng-show="col.desc"></icon>
                <icon name="'angle-up'" ng-hide="col.desc"></icon>
              </span>
            </div>
          </th>
        </tr>
      </thead>
      <tbody></tbody>
    </table>
  </div>
</div>
<div class="datapoints-warning" ng-show="ctrl.table.rows.length===0">
  <span class="small"> No data to show <tip>Nothing returned by data query</tip> </span>
</div>
<div class="table-panel-footer"></div>
`,a=s,e="public/app/plugins/panel/table-old/module.html";window.angular.module("ng").run(["$templateCache",function(l){l.put(e,a)}]),t.exports=e},12631:(t,s,a)=>{var e={"./angular/panel/partials/query_editor_row.html":27777,"./angular/partials/http_settings_next.html":37791,"./angular/partials/tls_auth_settings.html":3867,"./features/admin/partials/admin_home.html":33417,"./features/admin/partials/edit_org.html":51959,"./features/admin/partials/stats.html":5735,"./features/admin/partials/styleguide.html":705,"./features/annotations/partials/event_editor.html":82123,"./partials/confirm_modal.html":3378,"./partials/modal.html":21585,"./partials/reset_password.html":96599,"./partials/signup_invited.html":11774,"./plugins/panel/graph/axes_editor.html":49563,"./plugins/panel/graph/tab_display.html":92986,"./plugins/panel/graph/tab_legend.html":15607,"./plugins/panel/graph/tab_series_overrides.html":84013,"./plugins/panel/graph/tab_thresholds.html":96942,"./plugins/panel/graph/tab_time_regions.html":68869,"./plugins/panel/graph/thresholds_form.html":95127,"./plugins/panel/graph/time_regions_form.html":57254,"./plugins/panel/heatmap/partials/axes_editor.html":43860,"./plugins/panel/heatmap/partials/display_editor.html":58733,"./plugins/panel/table-old/column_options.html":92929,"./plugins/panel/table-old/editor.html":46359,"./plugins/panel/table-old/module.html":88826,"app/angular/panel/partials/query_editor_row.html":27777,"app/angular/partials/http_settings_next.html":37791,"app/angular/partials/tls_auth_settings.html":3867,"app/features/admin/partials/admin_home.html":33417,"app/features/admin/partials/edit_org.html":51959,"app/features/admin/partials/stats.html":5735,"app/features/admin/partials/styleguide.html":705,"app/features/annotations/partials/event_editor.html":82123,"app/partials/confirm_modal.html":3378,"app/partials/modal.html":21585,"app/partials/reset_password.html":96599,"app/partials/signup_invited.html":11774,"app/plugins/panel/graph/axes_editor.html":49563,"app/plugins/panel/graph/tab_display.html":92986,"app/plugins/panel/graph/tab_legend.html":15607,"app/plugins/panel/graph/tab_series_overrides.html":84013,"app/plugins/panel/graph/tab_thresholds.html":96942,"app/plugins/panel/graph/tab_time_regions.html":68869,"app/plugins/panel/graph/thresholds_form.html":95127,"app/plugins/panel/graph/time_regions_form.html":57254,"app/plugins/panel/heatmap/partials/axes_editor.html":43860,"app/plugins/panel/heatmap/partials/display_editor.html":58733,"app/plugins/panel/table-old/column_options.html":92929,"app/plugins/panel/table-old/editor.html":46359,"app/plugins/panel/table-old/module.html":88826};function l(r){var o=i(r);return a(o)}function i(r){if(!a.o(e,r)){var o=new Error("Cannot find module '"+r+"'");throw o.code="MODULE_NOT_FOUND",o}return e[r]}l.keys=function(){return Object.keys(e)},l.resolve=i,t.exports=l,l.id=12631}}]);

//# sourceMappingURL=AngularApp.3edb8b6096e94f8798fe.js.map
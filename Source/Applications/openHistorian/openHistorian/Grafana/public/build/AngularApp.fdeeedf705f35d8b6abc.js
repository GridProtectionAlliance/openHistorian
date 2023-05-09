(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[4704],{11461:(l,a,t)=>{var e={"./angular/panel/partials/query_editor_row.html":58126,"./angular/partials/http_settings_next.html":26399,"./angular/partials/tls_auth_settings.html":32167,"./features/admin/partials/admin_home.html":60808,"./features/admin/partials/edit_org.html":4569,"./features/admin/partials/stats.html":33374,"./features/admin/partials/styleguide.html":60814,"./features/alerting/partials/alert_tab.html":24957,"./features/annotations/partials/event_editor.html":44981,"./partials/confirm_modal.html":20087,"./partials/modal.html":66058,"./partials/reset_password.html":6558,"./partials/signup_invited.html":42456,"./plugins/panel/graph/axes_editor.html":17258,"./plugins/panel/graph/tab_display.html":55932,"./plugins/panel/graph/tab_legend.html":3599,"./plugins/panel/graph/tab_series_overrides.html":96865,"./plugins/panel/graph/tab_thresholds.html":23850,"./plugins/panel/graph/tab_time_regions.html":65450,"./plugins/panel/graph/thresholds_form.html":6372,"./plugins/panel/graph/time_regions_form.html":92233,"./plugins/panel/heatmap/partials/axes_editor.html":99127,"./plugins/panel/heatmap/partials/display_editor.html":25477,"./plugins/panel/table-old/column_options.html":76071,"./plugins/panel/table-old/editor.html":83732,"./plugins/panel/table-old/module.html":90564,"app/angular/panel/partials/query_editor_row.html":58126,"app/angular/partials/http_settings_next.html":26399,"app/angular/partials/tls_auth_settings.html":32167,"app/features/admin/partials/admin_home.html":60808,"app/features/admin/partials/edit_org.html":4569,"app/features/admin/partials/stats.html":33374,"app/features/admin/partials/styleguide.html":60814,"app/features/alerting/partials/alert_tab.html":24957,"app/features/annotations/partials/event_editor.html":44981,"app/partials/confirm_modal.html":20087,"app/partials/modal.html":66058,"app/partials/reset_password.html":6558,"app/partials/signup_invited.html":42456,"app/plugins/panel/graph/axes_editor.html":17258,"app/plugins/panel/graph/tab_display.html":55932,"app/plugins/panel/graph/tab_legend.html":3599,"app/plugins/panel/graph/tab_series_overrides.html":96865,"app/plugins/panel/graph/tab_thresholds.html":23850,"app/plugins/panel/graph/tab_time_regions.html":65450,"app/plugins/panel/graph/thresholds_form.html":6372,"app/plugins/panel/graph/time_regions_form.html":92233,"app/plugins/panel/heatmap/partials/axes_editor.html":99127,"app/plugins/panel/heatmap/partials/display_editor.html":25477,"app/plugins/panel/table-old/column_options.html":76071,"app/plugins/panel/table-old/editor.html":83732,"app/plugins/panel/table-old/module.html":90564};function n(s){var r=o(s);return t(r)}function o(s){if(!t.o(e,s)){var r=new Error("Cannot find module '"+s+"'");throw r.code="MODULE_NOT_FOUND",r}return e[s]}n.keys=function(){return Object.keys(e)},n.resolve=o,l.exports=n,n.id=11461},58126:l=>{var a=`<div ng-transclude class="gf-form-query-content"></div>

`,t=a,e="public/app/angular/panel/partials/query_editor_row.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},26399:l=>{var a=`<datasource-http-settings-next on-change="onChange" datasourceconfig="current" showaccessoptions="showAccessOption" defaulturl="suggestUrl" showforwardoauthidentityoption="showForwardOAuthIdentityOption">
`,t=a,e="public/app/angular/partials/http_settings_next.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},32167:l=>{var a=`<div class="gf-form-group">
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
`,t=a,e="public/app/angular/partials/tls_auth_settings.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},60808:l=>{var a=`<page-header model="ctrl.navModel"></page-header>

<div class="page-container page-body">

  <div class="grafana-info-box span8">
    Grafana is a multi-tenant system where most can be configured per organization. These
    admin pages are for server admins where you can manage orgs, & all users across all orgs.
  </div>

</div>

<footer>
`,t=a,e="public/app/features/admin/partials/admin_home.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},4569:l=>{var a=`<page-header model="navModel"></page-header>

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
`,t=a,e="public/app/features/admin/partials/edit_org.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},33374:l=>{var a=`<page-header model="ctrl.navModel"></page-header>

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
`,t=a,e="public/app/features/admin/partials/stats.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},60814:l=>{var a=`<page-header model="ctrl.navModel"></page-header>

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

`,t=a,e="public/app/features/admin/partials/styleguide.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},24957:l=>{var a=`<div ng-if="ctrl.panel.alert">
  <div class="alert alert-error m-b-2" ng-show="ctrl.error">
    <icon name="'exclamation-triangle'"></icon> {{ctrl.error}}
  </div>

  <div class="gf-form-group">
    <h4 class="section-heading">Rule</h4>
    <div class="gf-form-inline">
      <div class="gf-form">
        <span class="gf-form-label width-6">Name</span>
        <input type="text" class="gf-form-input width-20 gf-form-input--has-help-icon" ng-model="ctrl.alert.name">
        <info-popover mode="right-absolute">
          If you want to apply templating to the alert rule name, you must use the following syntax - \${Label}
        </info-popover>
      </div>
      <div class="gf-form">
        <span class="gf-form-label width-9">Evaluate every</span>
        <input class="gf-form-input max-width-6" type="text" ng-model="ctrl.alert.frequency" ng-blur="ctrl.checkFrequency()">
      </div>
      <div class="gf-form max-width-11">
        <label class="gf-form-label width-5">For</label>
        <input type="text" class="gf-form-input max-width-6 gf-form-input--has-help-icon" ng-model="ctrl.alert.for" spellcheck="false" placeholder="5m" ng-pattern="/(^\\d+([dhms])$)|(0)|(^$)/">
        <info-popover mode="right-absolute">
          If an alert rule has a configured and the query violates the configured threshold, then it goes from OK
          to Pending. Grafana does not send any notifications for that change. Once the alert rule has been
          firing for more than For duration, then the alert changes to Alerting and sends alert notifications.
        </info-popover>
      </div>
    </div>
    <div class="gf-form" ng-if="ctrl.frequencyWarning">
      <label class="gf-form-label text-warning">
        <icon name="'exclamation-triangle'"></icon> {{ctrl.frequencyWarning}}
      </label>
    </div>
  </div>

  <div class="gf-form-group">
    <h4 class="section-heading">Conditions</h4>
    <div class="gf-form-inline" ng-repeat="conditionModel in ctrl.conditionModels">
      <div class="gf-form">
        <metric-segment-model css-class="query-keyword width-5" ng-if="$index" property="conditionModel.operator.type" options="ctrl.evalOperators" custom="false"></metric-segment-model>
        <span class="gf-form-label query-keyword width-5" ng-if="$index===0">WHEN</span>
      </div>
      <div class="gf-form">
        <query-part-editor class="gf-form-label query-part width-9" part="conditionModel.reducerPart" handle-event="ctrl.handleReducerPartEvent(conditionModel, $event)">
        </query-part-editor>
        <span class="gf-form-label query-keyword">OF</span>
      </div>
      <div class="gf-form">
        <query-part-editor class="gf-form-label query-part" part="conditionModel.queryPart" handle-event="ctrl.handleQueryPartEvent(conditionModel, $event)">
        </query-part-editor>
      </div>
      <div class="gf-form">
        <metric-segment-model property="conditionModel.evaluator.type" options="ctrl.evalFunctions" custom="false" css-class="query-keyword" on-change="ctrl.evaluatorTypeChanged(conditionModel.evaluator)"></metric-segment-model>
        <input class="gf-form-input max-width-9" type="number" step="any" ng-hide="conditionModel.evaluator.params.length === 0" ng-model="conditionModel.evaluator.params[0]" ng-change="ctrl.evaluatorParamsChanged()">
        <label class="gf-form-label query-keyword" ng-show="conditionModel.evaluator.params.length === 2">TO</label>
        <input class="gf-form-input max-width-9" type="number" step="any" ng-if="conditionModel.evaluator.params.length === 2" ng-model="conditionModel.evaluator.params[1]" ng-change="ctrl.evaluatorParamsChanged()">
      </div>
      <div class="gf-form">
        <label class="gf-form-label">
          <a class="pointer" tabindex="1" ng-click="ctrl.removeCondition($index)">
            <icon name="'trash-alt'"></icon>
          </a>
        </label>
      </div>
    </div>

    <div class="gf-form">
      <label class="gf-form-label dropdown">
        <a class="pointer dropdown-toggle" data-toggle="dropdown">
          <icon name="'plus-circle'"></icon>
        </a>
        <ul class="dropdown-menu" role="menu">
          <li ng-repeat="ct in ctrl.conditionTypes" role="menuitem">
            <a ng-click="ctrl.addCondition(ct.value);">{{ct.text}}</a>
          </li>
        </ul>
      </label>
    </div>
  </div>

  <div class="gf-form-group">
    <h4 class="section-heading">No data and error handling</h4>
    <div class="gf-form-inline">
      <div class="gf-form">
        <span class="gf-form-label width-15">If no data or all values are null</span>
      </div>
      <div class="gf-form">
        <span class="gf-form-label query-keyword">set state to</span>
        <div class="gf-form-select-wrapper">
          <select class="gf-form-input" ng-model="ctrl.alert.noDataState" ng-options="f.value as f.text for f in ctrl.noDataModes">
          </select>
        </div>
      </div>
    </div>

    <div class="gf-form-inline">
      <div class="gf-form">
        <span class="gf-form-label width-15">If execution error or timeout</span>
      </div>
      <div class="gf-form">
        <span class="gf-form-label query-keyword">set state to</span>
        <div class="gf-form-select-wrapper">
          <select class="gf-form-input" ng-model="ctrl.alert.executionErrorState" ng-options="f.value as f.text for f in ctrl.executionErrorModes">
          </select>
        </div>
      </div>
    </div>
  </div>

  <h4 class="section-heading">Notifications</h4>
  <div class="gf-form-inline">
    <div class="gf-form">
      <span class="gf-form-label width-8">Send to</span>
    </div>
    <div class="gf-form" ng-repeat="nc in ctrl.alertNotifications">
      <span class="gf-form-label">
        <icon name="'{{nc.iconClass}}'"></icon>
        &nbsp;{{nc.name}}&nbsp;<span ng-if="nc.isDefault">(default)</span>
        <icon name="'times'" class="pointer muted" ng-click="ctrl.removeNotification(nc)" ng-if="nc.isDefault === false"></icon>
      </span>
    </div>
    <div class="gf-form">
      <metric-segment segment="ctrl.addNotificationSegment" get-options="ctrl.getNotifications()" on-change="ctrl.notificationAdded()"></metric-segment>
    </div>
  </div>
  <div class="gf-form gf-form--v-stretch">
    <span class="gf-form-label width-8">Message</span>
    <textarea class="gf-form-input gf-form-input--has-help-icon" rows="10" ng-model="ctrl.alert.message" placeholder="Notification message details..."></textarea>
    <info-popover mode="right-absolute">
      If you want to apply templating to the alert rule name, use the following syntax - \${Label}
    </info-popover>
  </div>
  <div class="gf-form">
    <span class="gf-form-label width-8">Tags</span>
    <div class="gf-form-group">
      <div class="gf-form-inline" ng-repeat="(name, value) in ctrl.alert.alertRuleTags">
        <label class="gf-form-label width-15">{{ name }}</label>
        <input class="gf-form-input width-15" placeholder="Tag value..." ng-model="ctrl.alert.alertRuleTags[name]" type="text">
        <label class="gf-form-label">
          <a class="pointer" tabindex="1" ng-click="ctrl.removeAlertRuleTag(name)">
            <icon name="'trash-alt'"></icon>
          </a>
        </label>
      </div>
      <div class="gf-form-inline">
        <div class="gf-form">
          <input class="gf-form-input width-15" placeholder="New tag name..." ng-model="ctrl.newAlertRuleTag.name" type="text">
          <input class="gf-form-input width-15" placeholder="New tag value..." ng-model="ctrl.newAlertRuleTag.value" type="text">
        </div>
      </div>
      <div class="gf-form">
        <label class="gf-form-label">
          <a class="pointer" tabindex="1" ng-click="ctrl.addAlertRuleTag()">
            <icon name="'plus-circle'"></icon>&nbsp;Add Tag
          </a>
        </label>
      </div>
    </div>
  </div>
</div>
`,t=a,e="public/app/features/alerting/partials/alert_tab.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},44981:l=>{var a=`
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
`,t=a,e="public/app/features/annotations/partials/event_editor.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},20087:l=>{var a=`<div class="modal-body" ng-cloak>
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
`,t=a,e="public/app/partials/confirm_modal.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},66058:l=>{var a=`<div class="modal-header">
  <button type="button" class="close" data-dismiss="modal" aria-hidden="true">\xD7</button>
  <h3>{{modal.title}}</h3>
</div>
<div class="modal-body">

  <div ng-bind-html="modal.body"></div>

</div>
<div class="modal-footer">
  <button type="button" class="btn btn-danger" ng-click="dismiss()">Close</button>
</div>`,t=a,e="public/app/partials/modal.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},6558:l=>{var a=`<page-header model="navModel"></page-header>

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
`,t=a,e="public/app/partials/reset_password.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},42456:l=>{var a=`<page-header model="navModel"></page-header>

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
`,t=a,e="public/app/partials/signup_invited.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},17258:l=>{var a=`<div class="editor-row">
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
`,t=a,e="public/app/plugins/panel/graph/axes_editor.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},55932:l=>{var a=`<div class="gf-form-group">
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
    <p>Some features like colored time regions and negative transforms are not supported in the new panel yet.</p>    
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
`,t=a,e="public/app/plugins/panel/graph/tab_display.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},3599:l=>{var a=`<div class="editor-row">
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
`,t=a,e="public/app/plugins/panel/graph/tab_legend.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},96865:l=>{var a=`<div class="graph-series-override" ng-repeat="override in ctrl.panel.seriesOverrides" ng-controller="SeriesOverridesCtrl">
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
`,t=a,e="public/app/plugins/panel/graph/tab_series_overrides.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},23850:l=>{var a=`<graph-threshold-form panel-ctrl="ctrl"></graph-threshold-form>
`,t=a,e="public/app/plugins/panel/graph/tab_thresholds.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},65450:l=>{var a=`<graph-time-region-form panel-ctrl="ctrl"></graph-time-region-form>
`,t=a,e="public/app/plugins/panel/graph/tab_time_regions.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},6372:l=>{var a=`<div class="gf-form-group">
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
`,t=a,e="public/app/plugins/panel/graph/thresholds_form.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},92233:l=>{var a=`<div class="gf-form-group">
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
`,t=a,e="public/app/plugins/panel/graph/time_regions_form.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},99127:l=>{var a=`<div class="editor-row">
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
`,t=a,e="public/app/plugins/panel/heatmap/partials/axes_editor.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},25477:l=>{var a=`<div class="editor-row">
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
`,t=a,e="public/app/plugins/panel/heatmap/partials/display_editor.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},76071:l=>{var a=`<div class="edit-tab-content" ng-repeat="style in editor.panel.styles">
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
`,t=a,e="public/app/plugins/panel/table-old/column_options.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},83732:l=>{var a=`<div class="gf-form-group">
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
`,t=a,e="public/app/plugins/panel/table-old/editor.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e},90564:l=>{var a=`<div class="table-panel-container">
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
`,t=a,e="public/app/plugins/panel/table-old/module.html";window.angular.module("ng").run(["$templateCache",function(n){n.put(e,t)}]),l.exports=e}}]);

//# sourceMappingURL=AngularApp.fdeeedf705f35d8b6abc.js.map
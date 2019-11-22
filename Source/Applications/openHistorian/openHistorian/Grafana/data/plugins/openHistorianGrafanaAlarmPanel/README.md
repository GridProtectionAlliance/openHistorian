# openHistorian Device Alarm Panel

This plugin is a Grana panel to show the status of all PMUs connected to the openHistorian.
For this Panel to work the openHistorian Data Source plugin has to be installed and set-up and the openHistorian has to be installed.
### Overview

Each connected PMU is displayed as a box in this panel. The color oe each Box indicates the status of the PMU. The following states are defined:
- Good State (Green): This is the simplest state to describe, it simply means that no alarm state is active.
- Alarm State (Red): This state means no data has been received over a configured period.
- Not Available State (Yellow): This state represents a “warning” state meaning data is not currently available (or old), but it has not reached the configured alarm period, i.e., “on its way to alarm state if nothing changes”. The alarm state time period is reached much more quickly than the alarm state. This is important since a device may simply be in a restarting state, or the PDC system may be failing over during patching. The configuration values for reaching not available are defined with lead and lag time tolerances. The Not Available state takes precedence over Bad Data.
- Bad Data State (Blue): This state means that one of devices in the connection group is consistently reporting bad data, i.e., the device is self-reporting a bad data flag – the openHistorian is not currently trying to deduce data quality for the purposes of alarming. In the case of IEEE C37.118, this means that bit 15 of the status flags has been set. For other protocols, e.g., IEEE 1344, this will be bit 14. The Bad Data state takes precedence over Bad Time.
- Bad Time State (Purple): This state means that one of the devices in the connection group is consistently report bad time, i.e., the device is self-reporting a bad time flag – or, the openPDC finds that the measurement time is consistently outside of configured time tolerances as compared to the local system clock– this catches devices with a floating clock, as can often happen with daylight savings time transitions. Note that for bad time states triggered solely on the basis of old timestamps will be superseded by Not Available state once data has been detected to be consistently stale. In the case of IEEE C37.118, the bad time flag is bit 13 of the status flags. Like in the case of Bad Data, the actual bit will be different for other protocols.
- Out of Service State: This state simply represents that a device has been disabled through configuration and is currently not reporting data.
- Acknowledged State: When a device is in any alarm state, the dashboard user can change the alarm to “acknowledged” to mean that the alarm is known and being worked. If the alarm state consistently transitions to “Good” for a configured period, the acknowledged state will be automatically cleared – otherwise the acknowledged state will remain until manually reset.

### Settings

The [Grafana SDK Mocks](https://github.com/grafana/grafana-sdk-mocks) package contains mocks for the Grafana classes that a plugin needs to build in TypeScript. It also contains some of the commonly used util classes that are used in plugins. This allows you to write unit tests for your plugin.

It is already included in the package.json but if you need to add it again then the command is:

`npm install --save-dev grafana/grafana-sdk-mocks`

It also contains a TypeScript Typings file - common.d.ts that you can refer to in your classes that use classes or functions from core Grafana. Use the following [triple slash directive](https://www.typescriptlang.org/docs/handbook/triple-slash-directives.html) to use Grafana classes in your code. The directive will point the TypeScript compiler at the mocks package so that it can find the files it needs to build. Place the directive at the top of all your TypeScript files:

```js
///<reference path="../node_modules/grafana-sdk-mocks/app/headers/common.d.ts" />
```

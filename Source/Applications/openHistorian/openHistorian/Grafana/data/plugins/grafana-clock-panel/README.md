## Clock Panel Plugin for Grafana

The Clock Panel can show the current time or a countdown/countup and updates every second.

Show the time in another office or show a countdown/countup to an important event.

### Plugin options

#### Options

- **Mode**:

  Default is time.
  If countdown is chosen then set the Countdown End Time to start the countdown.
  If countup is chosen then set the Countup Begin Time to start the countup.

- **12 or 24 hour**:

  Show time in the 12/24 hour format.

- **Timezone**:

  This timezones are supplied by the moment timezone library. Timezone can be set or left to default. Default is moment's guess (whatever that is on your computer). Timezone is also used to calculate countdown deadline in countdown mode.

- **Locale**:

  Locales for date-formatting are supplied by the moment library. The locale can be set or left to default. Default is moment's guess.

- **Countdown End Time**:

  Used in conjunction with the mode being set to countdown. Choose a date and time to count down to.
  
  This field also supports dashboard (constant) variables (e.g. ``${countdown_target}``) to dynamically set the countdown deadline for the Dashboard.

- **Countdown End Text**:

  The text to show when the countdown ends. E.g. LIFTOFF

- **Countup Begin Time**:

  Used in conjunction with the mode being set to countup. Choose a date and time to count up from.

- **Countup End Text**:

  The text to show before the countup starts. E.g. LIFTOFF

- **Date/Time formatting options**:

  The font size, weight and date/time formatting can be customized here. If the seconds ticking annoys you then change the time format to HH:mm for the 24 hour clock or h:mm A for the 12 hour clock, or see the [full list of formatting options](https://momentjs.com/docs/#/displaying/).

- **Bg Color**:

  Choose a background color for the clock with the color picker.

#### Refresh
  
- **Sync**:

  The clock is paused and only updated when the dashboard refreshes - the clock will show the timestamp for the last refresh.

### Screenshots

- [Screenshot of two clocks and a countdown](https://raw.githubusercontent.com/grafana/clock-panel/06ecf59c191db642127c6153bc3145e93a1df1f8/src/img/screenshot-clocks.png)
- [Screenshot of the options screen](https://raw.githubusercontent.com/grafana/clock-panel/06ecf59c191db642127c6153bc3145e93a1df1f8/src/img/screenshot-clock-options.png)

### Development

Using Docker:

1. Clone the repository and `cd` to it
1. make sure you have [yarn]( https://yarnpkg.com/) installed
1. install project dependencies: `yarn install --pure-lockfile`
1. Start the "watch" task: `yarn watch`
1. Run a local Grafana instance with the development version of the plugin: `docker run -p 3000:3000 -d --name grafana-plugin-dev --env GF_AUTH_ANONYMOUS_ORG_ROLE="Admin" --env GF_AUTH_ANONYMOUS_ENABLED="true" --env GF_AUTH_BASIC_ENABLED="false" --env GF_DEFAULT_APP_MODE="development" --volume $(pwd)/dist:/var/lib/grafana/plugins/clock-panel grafana/grafana`
2. Check the logs to see that Grafana has started up: `docker logs -f grafana-plugin-dev`
3. Open Grafana at http://localhost:3000/
4. Log in with username "admin" and password "admin"
5. Create new dashboard and add the plugin

To build a production build with minification: `yarn build`


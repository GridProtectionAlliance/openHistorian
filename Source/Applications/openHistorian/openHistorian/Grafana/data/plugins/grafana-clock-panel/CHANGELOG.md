# Change Log

## 2.1.8

- Improves migration handling for non-query panels to eliminate error message display

## [2.1.7]

- Handles migration for non-query clock panels to eliminate error message display
- Fixes [#186](https://github.com/grafana/clock-panel/issues/186)
- Fixes [#189](https://github.com/grafana/clock-panel/issues/189)
- Cleans up any legacy (angular) settings in top level of panel json
- Adds tests for migrations

## [2.1.2]
- Improving wrapping of the panel elements to be more responsive to different panel sizes https://github.com/grafana/clock-panel/pull/117
- Fixing a placeholder for the font size field https://github.com/grafana/clock-panel/pull/116
- 
## [2.1.1]
- Migrate to create-plugin instead of toolkit
- Small typo fixes
- Bump grafana packages / dependencies

## [2.1.0]
- Added support to set timezone from template variable

## [2.0.0]
- Prevent clock panel from crashing Grafana 9.x.x
- Drop support for Grafana 7.x.x

## [1.3.1]

- Fixes error on AMG related to dependency imports

## [1.3.0]

- Added support for count up mode
- Added support for template variables in count down/up time setting.

## [1.2.0]

- Support local for date formats
- Support refresh with dashboard time
- Added dependency on Grafana 7.4+

## [1.1.1]

- Improved background

## [1.1.0]

- Support for Grafana 7+
- Built with @grafana/toolkit

## v1.0.3

- Adds support for displaying timezones

## v1.0.1

- Updates Lodash dependency to fix security warning

## v1.0.0

- Dashboard sync/refresh feature - can show timestamp for last dashboard refresh.
- Tech - converted to TypeScript and Webpack.

## v0.0.9

- Fixes bug with default properties not getting deep cloned [#20](https://github.com/grafana/clock-panel/issues/20)

## v0.0.8

- Remove extraneous comma when 1 second left in the countdown. PR from @linkslice

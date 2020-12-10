// Libraries
import _, { defaults } from 'lodash';
// Utils
import getFactors from 'app/core/utils/factors';
import kbn from 'app/core/utils/kbn';
// Types
import { PanelModel } from './PanelModel';
import { DashboardModel } from './DashboardModel';
import { DataLinkBuiltInVars, DataLink, urlUtil } from '@grafana/data';
// Constants
import {
  DEFAULT_PANEL_SPAN,
  DEFAULT_ROW_HEIGHT,
  GRID_CELL_HEIGHT,
  GRID_CELL_VMARGIN,
  GRID_COLUMN_COUNT,
  MIN_PANEL_HEIGHT,
} from 'app/core/constants';
import { isMulti, isQuery } from 'app/features/variables/guard';
import { alignCurrentWithMulti } from 'app/features/variables/shared/multiOptions';
import { VariableTag } from '../../variables/types';

export class DashboardMigrator {
  dashboard: DashboardModel;

  constructor(dashboardModel: DashboardModel) {
    this.dashboard = dashboardModel;
  }

  updateSchema(old: any) {
    let i, j, k, n;
    const oldVersion = this.dashboard.schemaVersion;
    const panelUpgrades = [];
    this.dashboard.schemaVersion = 26;

    if (oldVersion === this.dashboard.schemaVersion) {
      return;
    }

    // version 2 schema changes
    if (oldVersion < 2) {
      if (old.services) {
        if (old.services.filter) {
          this.dashboard.time = old.services.filter.time;
          this.dashboard.templating.list = old.services.filter.list || [];
        }
      }

      panelUpgrades.push((panel: any) => {
        // rename panel type
        if (panel.type === 'graphite') {
          panel.type = 'graph';
        }
        if (panel.type !== 'graph') {
          return;
        }

        if (_.isBoolean(panel.legend)) {
          panel.legend = { show: panel.legend };
        }

        if (panel.grid) {
          if (panel.grid.min) {
            panel.grid.leftMin = panel.grid.min;
            delete panel.grid.min;
          }

          if (panel.grid.max) {
            panel.grid.leftMax = panel.grid.max;
            delete panel.grid.max;
          }
        }

        if (panel.y_format) {
          if (!panel.y_formats) {
            panel.y_formats = [];
          }
          panel.y_formats[0] = panel.y_format;
          delete panel.y_format;
        }

        if (panel.y2_format) {
          if (!panel.y_formats) {
            panel.y_formats = [];
          }
          panel.y_formats[1] = panel.y2_format;
          delete panel.y2_format;
        }
      });
    }

    // schema version 3 changes
    if (oldVersion < 3) {
      // ensure panel ids
      let maxId = this.dashboard.getNextPanelId();
      panelUpgrades.push((panel: any) => {
        if (!panel.id) {
          panel.id = maxId;
          maxId += 1;
        }
      });
    }

    // schema version 4 changes
    if (oldVersion < 4) {
      // move aliasYAxis changes
      panelUpgrades.push((panel: any) => {
        if (panel.type !== 'graph') {
          return;
        }
        _.each(panel.aliasYAxis, (value, key) => {
          panel.seriesOverrides = [{ alias: key, yaxis: value }];
        });
        delete panel.aliasYAxis;
      });
    }

    if (oldVersion < 6) {
      // move pulldowns to new schema
      const annotations: any = _.find(old.pulldowns, { type: 'annotations' });

      if (annotations) {
        this.dashboard.annotations = {
          list: annotations.annotations || [],
        };
      }

      // update template variables
      for (i = 0; i < this.dashboard.templating.list.length; i++) {
        const variable = this.dashboard.templating.list[i];
        if (variable.datasource === void 0) {
          variable.datasource = null;
        }
        if (variable.type === 'filter') {
          variable.type = 'query';
        }
        if (variable.type === void 0) {
          variable.type = 'query';
        }
        if (variable.allFormat === void 0) {
          variable.allFormat = 'glob';
        }
      }
    }

    if (oldVersion < 7) {
      if (old.nav && old.nav.length) {
        this.dashboard.timepicker = old.nav[0];
      }

      // ensure query refIds
      panelUpgrades.push((panel: any) => {
        _.each(panel.targets, target => {
          if (!target.refId) {
            target.refId = panel.getNextQueryLetter && panel.getNextQueryLetter();
          }
        });
      });
    }

    if (oldVersion < 8) {
      panelUpgrades.push((panel: any) => {
        _.each(panel.targets, target => {
          // update old influxdb query schema
          if (target.fields && target.tags && target.groupBy) {
            if (target.rawQuery) {
              delete target.fields;
              delete target.fill;
            } else {
              target.select = _.map(target.fields, field => {
                const parts = [];
                parts.push({ type: 'field', params: [field.name] });
                parts.push({ type: field.func, params: [] });
                if (field.mathExpr) {
                  parts.push({ type: 'math', params: [field.mathExpr] });
                }
                if (field.asExpr) {
                  parts.push({ type: 'alias', params: [field.asExpr] });
                }
                return parts;
              });
              delete target.fields;
              _.each(target.groupBy, part => {
                if (part.type === 'time' && part.interval) {
                  part.params = [part.interval];
                  delete part.interval;
                }
                if (part.type === 'tag' && part.key) {
                  part.params = [part.key];
                  delete part.key;
                }
              });

              if (target.fill) {
                target.groupBy.push({ type: 'fill', params: [target.fill] });
                delete target.fill;
              }
            }
          }
        });
      });
    }

    // schema version 9 changes
    if (oldVersion < 9) {
      // move aliasYAxis changes
      panelUpgrades.push((panel: any) => {
        if (panel.type !== 'singlestat' && panel.thresholds !== '') {
          return;
        }

        if (panel.thresholds) {
          const k = panel.thresholds.split(',');

          if (k.length >= 3) {
            k.shift();
            panel.thresholds = k.join(',');
          }
        }
      });
    }

    // schema version 10 changes
    if (oldVersion < 10) {
      // move aliasYAxis changes
      panelUpgrades.push((panel: any) => {
        if (panel.type !== 'table') {
          return;
        }

        _.each(panel.styles, style => {
          if (style.thresholds && style.thresholds.length >= 3) {
            const k = style.thresholds;
            k.shift();
            style.thresholds = k;
          }
        });
      });
    }

    if (oldVersion < 12) {
      // update template variables
      _.each(this.dashboard.getVariables(), (templateVariable: any) => {
        if (templateVariable.refresh) {
          templateVariable.refresh = 1;
        }
        if (!templateVariable.refresh) {
          templateVariable.refresh = 0;
        }
        if (templateVariable.hideVariable) {
          templateVariable.hide = 2;
        } else if (templateVariable.hideLabel) {
          templateVariable.hide = 1;
        }
      });
    }

    if (oldVersion < 12) {
      // update graph yaxes changes
      panelUpgrades.push((panel: any) => {
        if (panel.type !== 'graph') {
          return;
        }
        if (!panel.grid) {
          return;
        }

        if (!panel.yaxes) {
          panel.yaxes = [
            {
              show: panel['y-axis'],
              min: panel.grid.leftMin,
              max: panel.grid.leftMax,
              logBase: panel.grid.leftLogBase,
              format: panel.y_formats[0],
              label: panel.leftYAxisLabel,
            },
            {
              show: panel['y-axis'],
              min: panel.grid.rightMin,
              max: panel.grid.rightMax,
              logBase: panel.grid.rightLogBase,
              format: panel.y_formats[1],
              label: panel.rightYAxisLabel,
            },
          ];

          panel.xaxis = {
            show: panel['x-axis'],
          };

          delete panel.grid.leftMin;
          delete panel.grid.leftMax;
          delete panel.grid.leftLogBase;
          delete panel.grid.rightMin;
          delete panel.grid.rightMax;
          delete panel.grid.rightLogBase;
          delete panel.y_formats;
          delete panel.leftYAxisLabel;
          delete panel.rightYAxisLabel;
          delete panel['y-axis'];
          delete panel['x-axis'];
        }
      });
    }

    if (oldVersion < 13) {
      // update graph yaxes changes
      panelUpgrades.push((panel: any) => {
        if (panel.type !== 'graph') {
          return;
        }
        if (!panel.grid) {
          return;
        }

        if (!panel.thresholds) {
          panel.thresholds = [];
        }
        const t1: any = {},
          t2: any = {};

        if (panel.grid.threshold1 !== null) {
          t1.value = panel.grid.threshold1;
          if (panel.grid.thresholdLine) {
            t1.line = true;
            t1.lineColor = panel.grid.threshold1Color;
            t1.colorMode = 'custom';
          } else {
            t1.fill = true;
            t1.fillColor = panel.grid.threshold1Color;
            t1.colorMode = 'custom';
          }
        }

        if (panel.grid.threshold2 !== null) {
          t2.value = panel.grid.threshold2;
          if (panel.grid.thresholdLine) {
            t2.line = true;
            t2.lineColor = panel.grid.threshold2Color;
            t2.colorMode = 'custom';
          } else {
            t2.fill = true;
            t2.fillColor = panel.grid.threshold2Color;
            t2.colorMode = 'custom';
          }
        }

        if (_.isNumber(t1.value)) {
          if (_.isNumber(t2.value)) {
            if (t1.value > t2.value) {
              t1.op = t2.op = 'lt';
              panel.thresholds.push(t1);
              panel.thresholds.push(t2);
            } else {
              t1.op = t2.op = 'gt';
              panel.thresholds.push(t1);
              panel.thresholds.push(t2);
            }
          } else {
            t1.op = 'gt';
            panel.thresholds.push(t1);
          }
        }

        delete panel.grid.threshold1;
        delete panel.grid.threshold1Color;
        delete panel.grid.threshold2;
        delete panel.grid.threshold2Color;
        delete panel.grid.thresholdLine;
      });
    }

    if (oldVersion < 14) {
      this.dashboard.graphTooltip = old.sharedCrosshair ? 1 : 0;
    }

    if (oldVersion < 16) {
      this.upgradeToGridLayout(old);
    }

    if (oldVersion < 17) {
      panelUpgrades.push((panel: any) => {
        if (panel.minSpan) {
          const max = GRID_COLUMN_COUNT / panel.minSpan;
          const factors = getFactors(GRID_COLUMN_COUNT);
          // find the best match compared to factors
          // (ie. [1,2,3,4,6,12,24] for 24 columns)
          panel.maxPerRow =
            factors[
              _.findIndex(factors, o => {
                return o > max;
              }) - 1
            ];
        }
        delete panel.minSpan;
      });
    }

    if (oldVersion < 18) {
      // migrate change to gauge options
      panelUpgrades.push((panel: any) => {
        if (panel['options-gauge']) {
          panel.options = panel['options-gauge'];
          panel.options.valueOptions = {
            unit: panel.options.unit,
            stat: panel.options.stat,
            decimals: panel.options.decimals,
            prefix: panel.options.prefix,
            suffix: panel.options.suffix,
          };

          // correct order
          if (panel.options.thresholds) {
            panel.options.thresholds.reverse();
          }

          // this options prop was due to a bug
          delete panel.options.options;
          delete panel.options.unit;
          delete panel.options.stat;
          delete panel.options.decimals;
          delete panel.options.prefix;
          delete panel.options.suffix;
          delete panel['options-gauge'];
        }
      });
    }

    if (oldVersion < 19) {
      // migrate change to gauge options
      panelUpgrades.push((panel: any) => {
        if (panel.links && _.isArray(panel.links)) {
          panel.links = panel.links.map(upgradePanelLink);
        }
      });
    }

    if (oldVersion < 20) {
      const updateLinks = (link: DataLink) => {
        return {
          ...link,
          url: updateVariablesSyntax(link.url),
        };
      };
      panelUpgrades.push((panel: any) => {
        // For graph panel
        if (panel.options && panel.options.dataLinks && _.isArray(panel.options.dataLinks)) {
          panel.options.dataLinks = panel.options.dataLinks.map(updateLinks);
        }

        // For panel with fieldOptions
        if (panel.options && panel.options.fieldOptions && panel.options.fieldOptions.defaults) {
          if (panel.options.fieldOptions.defaults.links && _.isArray(panel.options.fieldOptions.defaults.links)) {
            panel.options.fieldOptions.defaults.links = panel.options.fieldOptions.defaults.links.map(updateLinks);
          }
          if (panel.options.fieldOptions.defaults.title) {
            panel.options.fieldOptions.defaults.title = updateVariablesSyntax(
              panel.options.fieldOptions.defaults.title
            );
          }
        }
      });
    }

    if (oldVersion < 21) {
      const updateLinks = (link: DataLink) => {
        return {
          ...link,
          url: link.url.replace(/__series.labels/g, '__field.labels'),
        };
      };
      panelUpgrades.push((panel: any) => {
        // For graph panel
        if (panel.options && panel.options.dataLinks && _.isArray(panel.options.dataLinks)) {
          panel.options.dataLinks = panel.options.dataLinks.map(updateLinks);
        }

        // For panel with fieldOptions
        if (panel.options && panel.options.fieldOptions && panel.options.fieldOptions.defaults) {
          if (panel.options.fieldOptions.defaults.links && _.isArray(panel.options.fieldOptions.defaults.links)) {
            panel.options.fieldOptions.defaults.links = panel.options.fieldOptions.defaults.links.map(updateLinks);
          }
        }
      });
    }

    if (oldVersion < 22) {
      panelUpgrades.push((panel: any) => {
        if (panel.type !== 'table') {
          return;
        }

        _.each(panel.styles, style => {
          style.align = 'auto';
        });
      });
    }

    if (oldVersion < 23) {
      for (const variable of this.dashboard.templating.list) {
        if (!isMulti(variable)) {
          continue;
        }
        const { multi, current } = variable;
        variable.current = alignCurrentWithMulti(current, multi);
      }
    }

    if (oldVersion < 24) {
      // 7.0
      // - migrate existing tables to 'table-old'
      panelUpgrades.push((panel: any) => {
        const wasAngularTable = panel.type === 'table';
        if (wasAngularTable && !panel.styles) {
          return; // styles are missing so assumes default settings
        }
        const wasReactTable = panel.table === 'table2';
        if (!wasAngularTable || wasReactTable) {
          return;
        }
        panel.type = wasAngularTable ? 'table-old' : 'table';
      });
    }

    if (oldVersion < 25) {
      for (const variable of this.dashboard.templating.list) {
        if (!isQuery(variable)) {
          continue;
        }

        const { tags, current } = variable;
        if (!Array.isArray(tags)) {
          variable.tags = [];
          continue;
        }

        const currentTags = current?.tags ?? [];
        const currents = currentTags.reduce((all, tag) => {
          if (tag && tag.hasOwnProperty('text') && typeof tag['text'] === 'string') {
            all[tag.text] = tag;
          }
          return all;
        }, {} as Record<string, VariableTag>);

        const newTags: VariableTag[] = [];

        for (const tag of tags) {
          if (typeof tag === 'object') {
            // new format let's assume it's correct
            newTags.push(tag);
            continue;
          }

          if (typeof tag !== 'string') {
            // something that we do not support
            continue;
          }

          newTags.push(defaults(currents[tag], { text: tag, selected: false }));
        }
        variable.tags = newTags;
      }
    }

    if (oldVersion < 26) {
      panelUpgrades.push((panel: any) => {
        const wasReactText = panel.type === 'text2';
        if (!wasReactText) {
          return;
        }

        panel.type = 'text';
        delete panel.options.angular;
      });
    }

    if (panelUpgrades.length === 0) {
      return;
    }

    for (j = 0; j < this.dashboard.panels.length; j++) {
      for (k = 0; k < panelUpgrades.length; k++) {
        panelUpgrades[k].call(this, this.dashboard.panels[j]);
        if (this.dashboard.panels[j].panels) {
          for (n = 0; n < this.dashboard.panels[j].panels.length; n++) {
            panelUpgrades[k].call(this, this.dashboard.panels[j].panels[n]);
          }
        }
      }
    }
  }

  upgradeToGridLayout(old: any) {
    let yPos = 0;
    const widthFactor = GRID_COLUMN_COUNT / 12;

    const maxPanelId = _.max(
      _.flattenDeep(
        _.map(old.rows, row => {
          return _.map(row.panels, 'id');
        })
      )
    );
    let nextRowId = maxPanelId + 1;

    if (!old.rows) {
      return;
    }

    // Add special "row" panels if even one row is collapsed, repeated or has visible title
    const showRows = _.some(old.rows, row => row.collapse || row.showTitle || row.repeat);

    for (const row of old.rows) {
      if (row.repeatIteration) {
        continue;
      }

      const height: any = row.height || DEFAULT_ROW_HEIGHT;
      const rowGridHeight = getGridHeight(height);

      const rowPanel: any = {};
      let rowPanelModel: PanelModel | undefined;

      if (showRows) {
        // add special row panel
        rowPanel.id = nextRowId;
        rowPanel.type = 'row';
        rowPanel.title = row.title;
        rowPanel.collapsed = row.collapse;
        rowPanel.repeat = row.repeat;
        rowPanel.panels = [];
        rowPanel.gridPos = {
          x: 0,
          y: yPos,
          w: GRID_COLUMN_COUNT,
          h: rowGridHeight,
        };
        rowPanelModel = new PanelModel(rowPanel);
        nextRowId++;
        yPos++;
      }

      const rowArea = new RowArea(rowGridHeight, GRID_COLUMN_COUNT, yPos);

      for (const panel of row.panels) {
        panel.span = panel.span || DEFAULT_PANEL_SPAN;
        if (panel.minSpan) {
          panel.minSpan = Math.min(GRID_COLUMN_COUNT, (GRID_COLUMN_COUNT / 12) * panel.minSpan);
        }
        const panelWidth = Math.floor(panel.span) * widthFactor;
        const panelHeight = panel.height ? getGridHeight(panel.height) : rowGridHeight;

        const panelPos = rowArea.getPanelPosition(panelHeight, panelWidth);
        yPos = rowArea.yPos;
        panel.gridPos = {
          x: panelPos.x,
          y: yPos + panelPos.y,
          w: panelWidth,
          h: panelHeight,
        };
        rowArea.addPanel(panel.gridPos);

        delete panel.span;

        if (rowPanelModel && rowPanel.collapsed) {
          rowPanelModel.panels.push(panel);
        } else {
          this.dashboard.panels.push(new PanelModel(panel));
        }
      }

      if (rowPanelModel) {
        this.dashboard.panels.push(rowPanelModel);
      }

      if (!(rowPanelModel && rowPanel.collapsed)) {
        yPos += rowGridHeight;
      }
    }
  }
}

function getGridHeight(height: number | string) {
  if (_.isString(height)) {
    height = parseInt(height.replace('px', ''), 10);
  }

  if (height < MIN_PANEL_HEIGHT) {
    height = MIN_PANEL_HEIGHT;
  }

  const gridHeight = Math.ceil(height / (GRID_CELL_HEIGHT + GRID_CELL_VMARGIN));
  return gridHeight;
}

/**
 * RowArea represents dashboard row filled by panels
 * area is an array of numbers represented filled column's cells like
 *  -----------------------
 * |******** ****
 * |******** ****
 * |********
 *  -----------------------
 *  33333333 2222 00000 ...
 */
class RowArea {
  area: number[];
  yPos: number;
  height: number;

  constructor(height: number, width = GRID_COLUMN_COUNT, rowYPos = 0) {
    this.area = new Array(width).fill(0);
    this.yPos = rowYPos;
    this.height = height;
  }

  reset() {
    this.area.fill(0);
  }

  /**
   * Update area after adding the panel.
   */
  addPanel(gridPos: any) {
    for (let i = gridPos.x; i < gridPos.x + gridPos.w; i++) {
      if (!this.area[i] || gridPos.y + gridPos.h - this.yPos > this.area[i]) {
        this.area[i] = gridPos.y + gridPos.h - this.yPos;
      }
    }
    return this.area;
  }

  /**
   * Calculate position for the new panel in the row.
   */
  getPanelPosition(panelHeight: number, panelWidth: number, callOnce = false): any {
    let startPlace, endPlace;
    let place;
    for (let i = this.area.length - 1; i >= 0; i--) {
      if (this.height - this.area[i] > 0) {
        if (endPlace === undefined) {
          endPlace = i;
        } else {
          if (i < this.area.length - 1 && this.area[i] <= this.area[i + 1]) {
            startPlace = i;
          } else {
            break;
          }
        }
      } else {
        break;
      }
    }

    if (startPlace !== undefined && endPlace !== undefined && endPlace - startPlace >= panelWidth - 1) {
      const yPos = _.max(this.area.slice(startPlace));
      place = {
        x: startPlace,
        y: yPos,
      };
    } else if (!callOnce) {
      // wrap to next row
      this.yPos += this.height;
      this.reset();
      return this.getPanelPosition(panelHeight, panelWidth, true);
    } else {
      return null;
    }

    return place;
  }
}

function upgradePanelLink(link: any): DataLink {
  let url = link.url;

  if (!url && link.dashboard) {
    url = `dashboard/db/${kbn.slugifyForUrl(link.dashboard)}`;
  }

  if (!url && link.dashUri) {
    url = `dashboard/${link.dashUri}`;
  }

  // some models are incomplete and have no dashboard or dashUri
  if (!url) {
    url = '/';
  }

  if (link.keepTime) {
    url = urlUtil.appendQueryToUrl(url, `$${DataLinkBuiltInVars.keepTime}`);
  }

  if (link.includeVars) {
    url = urlUtil.appendQueryToUrl(url, `$${DataLinkBuiltInVars.includeVars}`);
  }

  if (link.params) {
    url = urlUtil.appendQueryToUrl(url, link.params);
  }

  return {
    url: url,
    title: link.title,
    targetBlank: link.targetBlank,
  };
}

function updateVariablesSyntax(text: string) {
  const legacyVariableNamesRegex = /(__series_name)|(\$__series_name)|(__value_time)|(__field_name)|(\$__field_name)/g;

  return text.replace(legacyVariableNamesRegex, (match, seriesName, seriesName1, valueTime, fieldName, fieldName1) => {
    if (seriesName) {
      return '__series.name';
    }
    if (seriesName1) {
      return '${__series.name}';
    }
    if (valueTime) {
      return '__value.time';
    }
    if (fieldName) {
      return '__field.name';
    }
    if (fieldName1) {
      return '${__field.name}';
    }
    return match;
  });
}

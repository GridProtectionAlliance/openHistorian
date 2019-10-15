import React, { Component } from 'react';

import { renderMarkdown, LinkModelSupplier, ScopedVars } from '@grafana/data';
import { Tooltip, PopoverContent } from '@grafana/ui';

import { PanelModel } from 'app/features/dashboard/state/PanelModel';
import templateSrv from 'app/features/templating/template_srv';
import { getTimeSrv, TimeSrv } from 'app/features/dashboard/services/TimeSrv';

enum InfoMode {
  Error = 'Error',
  Info = 'Info',
  Links = 'Links',
}

interface Props {
  panel: PanelModel;
  title?: string;
  description?: string;
  scopedVars?: ScopedVars;
  links?: LinkModelSupplier<PanelModel>;
  error?: string;
}

export class PanelHeaderCorner extends Component<Props> {
  timeSrv: TimeSrv = getTimeSrv();

  getInfoMode = () => {
    const { panel, error } = this.props;
    if (error) {
      return InfoMode.Error;
    }
    if (!!panel.description) {
      return InfoMode.Info;
    }
    if (panel.links && panel.links.length) {
      return InfoMode.Links;
    }

    return undefined;
  };

  getInfoContent = (): JSX.Element => {
    const { panel } = this.props;
    const markdown = panel.description || '';
    const interpolatedMarkdown = templateSrv.replace(markdown, panel.scopedVars);
    const markedInterpolatedMarkdown = renderMarkdown(interpolatedMarkdown);
    const links = this.props.links && this.props.links.getLinks(panel);

    return (
      <div className="panel-info-content markdown-html">
        <div dangerouslySetInnerHTML={{ __html: markedInterpolatedMarkdown }} />

        {links && links.length > 0 && (
          <ul className="panel-info-corner-links">
            {links.map((link, idx) => {
              return (
                <li key={idx}>
                  <a className="panel-info-corner-links__item" href={link.href} target={link.target}>
                    {link.title}
                  </a>
                </li>
              );
            })}
          </ul>
        )}
      </div>
    );
  };

  renderCornerType(infoMode: InfoMode, content: PopoverContent) {
    const theme = infoMode === InfoMode.Error ? 'error' : 'info';
    return (
      <Tooltip content={content} placement="top-start" theme={theme}>
        <div className={`panel-info-corner panel-info-corner--${infoMode.toLowerCase()}`}>
          <i className="fa" />
          <span className="panel-info-corner-inner" />
        </div>
      </Tooltip>
    );
  }

  render() {
    const infoMode: InfoMode | undefined = this.getInfoMode();

    if (!infoMode) {
      return null;
    }

    if (infoMode === InfoMode.Error) {
      return this.renderCornerType(infoMode, this.props.error);
    }

    if (infoMode === InfoMode.Info || infoMode === InfoMode.Links) {
      return this.renderCornerType(infoMode, this.getInfoContent);
    }

    return null;
  }
}

export default PanelHeaderCorner;

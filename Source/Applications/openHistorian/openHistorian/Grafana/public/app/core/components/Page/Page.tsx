// Libraries
import React, { Component, HTMLAttributes } from 'react';
import { getTitleFromNavModel } from 'app/core/selectors/navModel';

// Components
import PageHeader from '../PageHeader/PageHeader';
import { Footer } from '../Footer/Footer';
import PageContents from './PageContents';
import { CustomScrollbar } from '@grafana/ui';
import { NavModel } from '@grafana/data';
import { isEqual } from 'lodash';
import { Branding } from '../Branding/Branding';

interface Props extends HTMLAttributes<HTMLDivElement> {
  children: React.ReactNode;
  navModel: NavModel;
}

class Page extends Component<Props> {
  static Header = PageHeader;
  static Contents = PageContents;

  componentDidMount() {
    this.updateTitle();
  }

  componentDidUpdate(prevProps: Props) {
    if (!isEqual(prevProps.navModel, this.props.navModel)) {
      this.updateTitle();
    }
  }

  updateTitle = () => {
    const title = this.getPageTitle;
    document.title = title ? title + ' - ' + Branding.AppTitle : Branding.AppTitle;
  };

  get getPageTitle() {
    const { navModel } = this.props;
    if (navModel) {
      return getTitleFromNavModel(navModel) || undefined;
    }
    return undefined;
  }

  render() {
    const { navModel, children, ...otherProps } = this.props;
    return (
      <div {...otherProps} className="page-scrollbar-wrapper">
        <CustomScrollbar autoHeightMin={'100%'} className="custom-scrollbar--page">
          <div className="page-scrollbar-content">
            <PageHeader model={navModel} />
            {children}
            <Footer />
          </div>
        </CustomScrollbar>
      </div>
    );
  }
}

export default Page;

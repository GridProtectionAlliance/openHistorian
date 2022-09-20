import { throttle } from 'lodash';
import React, { PureComponent } from 'react';
import Draggable, { DraggableEventHandler } from 'react-draggable';

import { PanelModel } from '../state/PanelModel';

interface Props {
  isEditing: boolean;
  render: (styles: object) => JSX.Element;
  panel: PanelModel;
}

interface State {
  editorHeight: number;
}

export class PanelResizer extends PureComponent<Props, State> {
  initialHeight: number = Math.floor(document.documentElement.scrollHeight * 0.3);
  prevEditorHeight?: number;
  throttledChangeHeight: (height: number) => void;
  throttledResizeDone?: () => void;
  noStyles: object = {};

  constructor(props: Props) {
    super(props);

    this.state = {
      editorHeight: this.initialHeight,
    };

    this.throttledChangeHeight = throttle(this.changeHeight, 20, { trailing: true });
  }

  get largestHeight() {
    return document.documentElement.scrollHeight * 0.9;
  }
  get smallestHeight() {
    return 100;
  }

  changeHeight = (height: number) => {
    const sh = this.smallestHeight;
    const lh = this.largestHeight;
    height = height < sh ? sh : height;
    height = height > lh ? lh : height;

    this.prevEditorHeight = this.state.editorHeight;
    this.setState({
      editorHeight: height,
    });
  };

  onDrag: DraggableEventHandler = (evt, data) => {
    const newHeight = this.state.editorHeight + data.y;
    this.throttledChangeHeight(newHeight);
  };

  render() {
    const { render, isEditing } = this.props;
    const { editorHeight } = this.state;

    return (
      <>
        {render(isEditing ? { height: editorHeight } : this.noStyles)}
        {isEditing && (
          <div className="panel-editor-container__resizer">
            <Draggable axis="y" grid={[100, 1]} onDrag={this.onDrag} position={{ x: 0, y: 0 }}>
              <div className="panel-editor-resizer">
                <div className="panel-editor-resizer__handle" />
              </div>
            </Draggable>
          </div>
        )}
      </>
    );
  }
}

import { Global } from '@emotion/react';
import Tree from 'rc-tree';
import React, { Key, useEffect, useMemo, useState } from 'react';
import SVG from 'react-inlinesvg';

import { SelectableValue, StandardEditorProps } from '@grafana/data';
import { config } from '@grafana/runtime';
import { Button, HorizontalGroup, useTheme2 } from '@grafana/ui';
import { ElementState } from 'app/features/canvas/runtime/element';

import { AddLayerButton } from '../../../../core/components/Layers/AddLayerButton';
import { CanvasElementOptions, canvasElementRegistry } from '../../../../features/canvas';
import { notFoundItem } from '../../../../features/canvas/elements/notFound';
import { getGlobalStyles } from '../globalStyles';
import { PanelOptions } from '../models.gen';
import { getTreeData, onNodeDrop, TreeElement } from '../tree';
import { DragNode, DropNode } from '../types';
import { doSelect, getElementTypes } from '../utils';

import { TreeNodeTitle } from './TreeNodeTitle';
import { TreeViewEditorProps } from './elementEditor';

let allowSelection = true;

export const TreeNavigationEditor = ({ item }: StandardEditorProps<any, TreeViewEditorProps, PanelOptions>) => {
  const [treeData, setTreeData] = useState(getTreeData(item?.settings?.scene.root));
  const [autoExpandParent, setAutoExpandParent] = useState(true);
  const [expandedKeys, setExpandedKeys] = useState<Key[]>([]);
  const [selectedKeys, setSelectedKeys] = useState<Key[]>([]);

  const theme = useTheme2();
  const globalCSS = getGlobalStyles(theme);

  const selectedBgColor = theme.v1.colors.formInputBorderActive;
  const { settings } = item;
  const selection = useMemo(
    () => (settings?.selected ? settings.selected.map((v) => v.getName()) : []),
    [settings?.selected]
  );

  const selectionByUID = useMemo(
    () => (settings?.selected ? settings.selected.map((v) => v.UID) : []),
    [settings?.selected]
  );

  useEffect(() => {
    setTreeData(getTreeData(item?.settings?.scene.root, selection, selectedBgColor));
    setSelectedKeys(selectionByUID);
    setAllowSelection();
  }, [item?.settings?.scene.root, selectedBgColor, selection, selectionByUID]);

  if (!settings) {
    return <div>No settings</div>;
  }

  const layer = settings.layer;
  if (!layer) {
    return <div>Missing layer?</div>;
  }

  const onSelect = (selectedKeys: Key[], info: { node: { dataRef: ElementState } }) => {
    if (allowSelection && item.settings?.scene) {
      doSelect(item.settings.scene, info.node.dataRef);
    }
  };

  const allowDrop = () => {
    return true;
  };

  const onDrop = (info: { node: DropNode; dragNode: DragNode; dropPosition: number; dropToGap: boolean }) => {
    const destPos = info.node.pos.split('-');
    const destPosition = info.dropPosition - Number(destPos[destPos.length - 1]);

    const srcEl = info.dragNode.dataRef;
    const destEl = info.node.dataRef;

    const data = onNodeDrop(info, treeData);

    setTreeData(data);
    destEl.parent?.scene.reorderElements(srcEl, destEl, info.dropToGap, destPosition);
  };

  const onExpand = (expandedKeys: Key[]) => {
    setExpandedKeys(expandedKeys);
    setAutoExpandParent(false);
  };

  const getSvgIcon = (path = '', style = {}) => <SVG src={path} title={'Node Icon'} style={{ ...style }} />;

  const switcherIcon = (obj: { isLeaf: boolean; expanded: boolean }) => {
    if (obj.isLeaf) {
      // TODO: Implement element specific icons
      return getSvgIcon('');
    }

    return getSvgIcon('public/img/icons/unicons/angle-right.svg', {
      transform: `rotate(${obj.expanded ? 90 : 0}deg)`,
      fill: theme.colors.text.primary,
    });
  };

  const setAllowSelection = (allow = true) => {
    allowSelection = allow;
  };

  const onAddItem = (sel: SelectableValue<string>) => {
    const newItem = canvasElementRegistry.getIfExists(sel.value) ?? notFoundItem;
    const newElementOptions = newItem.getNewOptions() as CanvasElementOptions;
    newElementOptions.type = newItem.id;
    const newElement = new ElementState(newItem, newElementOptions, layer);
    newElement.updateData(layer.scene.context);
    layer.elements.push(newElement);
    layer.scene.save();

    layer.reinitializeMoveable();
  };

  const onClearSelection = () => {
    layer.scene.clearCurrentSelection();
  };

  const onTitleRender = (nodeData: TreeElement) => {
    return <TreeNodeTitle nodeData={nodeData} setAllowSelection={setAllowSelection} settings={settings} />;
  };

  // TODO: This functionality is currently kinda broken / no way to decouple / delete created frames at this time
  const onFrameSelection = () => {
    if (layer.scene) {
      layer.scene.frameSelection();
    } else {
      console.warn('no scene!');
    }
  };

  const typeOptions = getElementTypes(settings.scene.shouldShowAdvancedTypes);

  return (
    <>
      <Global styles={globalCSS} />
      <Tree
        selectable={true}
        onSelect={onSelect}
        draggable={true}
        defaultExpandAll={true}
        autoExpandParent={autoExpandParent}
        showIcon={false}
        allowDrop={allowDrop}
        onDrop={onDrop}
        expandedKeys={expandedKeys}
        onExpand={onExpand}
        treeData={treeData}
        titleRender={onTitleRender}
        switcherIcon={switcherIcon}
        selectedKeys={selectedKeys}
        multiple={true}
      />

      <HorizontalGroup>
        <div style={{ marginLeft: '18px' }}>
          <AddLayerButton onChange={onAddItem} options={typeOptions} label={'Add item'} />
        </div>
        {selection.length > 0 && (
          <Button size="sm" variant="secondary" onClick={onClearSelection}>
            Clear selection
          </Button>
        )}
        {selection.length > 1 && config.featureToggles.canvasPanelNesting && (
          <Button size="sm" variant="secondary" onClick={onFrameSelection}>
            Frame selection
          </Button>
        )}
      </HorizontalGroup>
    </>
  );
};

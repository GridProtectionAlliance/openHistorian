import { ElementState } from '../../../features/canvas/runtime/element';

export enum LayerActionID {
  Delete = 'delete',
  Duplicate = 'duplicate',
  MoveTop = 'move-top',
  MoveBottom = 'move-bottom',
}

export interface DragNode {
  key: number;
  dataRef: ElementState;
}

export interface DropNode extends DragNode {
  pos: string;
}

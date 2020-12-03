import React, { FunctionComponent, useEffect, useState } from 'react';
import { LegacyForms } from '@grafana/ui';
import { AnnotationQuery } from '@grafana/data';
const { Switch } = LegacyForms;

interface Props {
  annotations: AnnotationQuery[];
  onAnnotationChanged: (annotation: any) => void;
}

export const Annotations: FunctionComponent<Props> = ({ annotations, onAnnotationChanged }) => {
  const [visibleAnnotations, setVisibleAnnotations] = useState<any>([]);
  useEffect(() => {
    setVisibleAnnotations(annotations.filter(annotation => annotation.hide !== true));
  }, [annotations]);

  if (visibleAnnotations.length === 0) {
    return null;
  }

  return (
    <>
      {visibleAnnotations.map((annotation: any) => {
        return (
          <div
            key={annotation.name}
            className={annotation.enable ? 'submenu-item' : 'submenu-item annotation-disabled'}
          >
            <Switch
              label={annotation.name}
              className="gf-form"
              checked={annotation.enable}
              onChange={() => onAnnotationChanged(annotation)}
            />
          </div>
        );
      })}
    </>
  );
};

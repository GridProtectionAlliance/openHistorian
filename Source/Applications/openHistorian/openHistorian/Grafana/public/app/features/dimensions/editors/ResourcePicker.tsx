import { css } from '@emotion/css';
import React, { createRef } from 'react';
import SVG from 'react-inlinesvg';

import { GrafanaTheme2 } from '@grafana/data';
import {
  Button,
  InlineField,
  InlineFieldRow,
  Input,
  LinkButton,
  Popover,
  PopoverController,
  useStyles2,
  useTheme2,
} from '@grafana/ui';
import { closePopover } from '@grafana/ui/src/utils/closePopover';

import { getPublicOrAbsoluteUrl } from '../resource';
import { MediaType, ResourceFolderName, ResourcePickerSize } from '../types';

import { ResourcePickerPopover } from './ResourcePickerPopover';

interface Props {
  onChange: (value?: string) => void;
  mediaType: MediaType;
  folderName: ResourceFolderName;
  size: ResourcePickerSize;
  onClear?: (event: React.MouseEvent) => void;
  value?: string; //img/icons/unicons/0-plus.svg
  src?: string;
  name?: string;
  placeholder?: string;
  color?: string;
}

export const ResourcePicker = (props: Props) => {
  const { value, src, name, placeholder, onChange, onClear, mediaType, folderName, size, color } = props;

  const styles = useStyles2(getStyles);
  const theme = useTheme2();

  const pickerTriggerRef = createRef<any>();
  const popoverElement = (
    <ResourcePickerPopover onChange={onChange} value={value} mediaType={mediaType} folderName={folderName} />
  );

  let sanitizedSrc = src;
  if (!sanitizedSrc && value) {
    sanitizedSrc = getPublicOrAbsoluteUrl(value);
  }

  const colorStyle = color && {
    fill: theme.visualization.getColorByName(color),
  };

  const renderSmallResourcePicker = () => {
    if (value && sanitizedSrc) {
      return <SVG src={sanitizedSrc} className={styles.icon} style={{ ...colorStyle }} />;
    } else {
      return (
        <LinkButton variant="primary" fill="text" size="sm">
          Set icon
        </LinkButton>
      );
    }
  };

  const renderNormalResourcePicker = () => (
    <InlineFieldRow>
      <InlineField label={null} grow>
        <Input
          value={name}
          placeholder={placeholder}
          readOnly={true}
          prefix={sanitizedSrc && <SVG src={sanitizedSrc} className={styles.icon} style={{ ...colorStyle }} />}
          suffix={<Button icon="times" variant="secondary" fill="text" size="sm" onClick={onClear} />}
        />
      </InlineField>
    </InlineFieldRow>
  );

  return (
    <PopoverController content={popoverElement}>
      {(showPopper, hidePopper, popperProps) => {
        return (
          <>
            {pickerTriggerRef.current && (
              <Popover
                {...popperProps}
                referenceElement={pickerTriggerRef.current}
                onMouseEnter={showPopper}
                onKeyDown={(event: any) => {
                  closePopover(event, hidePopper);
                }}
              />
            )}

            <div ref={pickerTriggerRef} onClick={showPopper} className={styles.pointer}>
              {size === ResourcePickerSize.SMALL && renderSmallResourcePicker()}
              {size === ResourcePickerSize.NORMAL && renderNormalResourcePicker()}
            </div>
          </>
        );
      }}
    </PopoverController>
  );
};

const getStyles = (theme: GrafanaTheme2) => ({
  pointer: css`
    cursor: pointer;
    input[readonly] {
      cursor: pointer;
    }
  `,
  icon: css`
    vertical-align: middle;
    display: inline-block;
    fill: currentColor;
    width: 25px;
  `,
});

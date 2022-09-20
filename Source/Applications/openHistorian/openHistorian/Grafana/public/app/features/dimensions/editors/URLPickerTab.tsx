import { css } from '@emotion/css';
import React, { Dispatch, SetStateAction } from 'react';
import SVG from 'react-inlinesvg';

import { GrafanaTheme2 } from '@grafana/data';
import { Field, Input, Label, useStyles2 } from '@grafana/ui';

import { getPublicOrAbsoluteUrl } from '../resource';
import { MediaType } from '../types';

interface Props {
  newValue: string;
  setNewValue: Dispatch<SetStateAction<string>>;
  mediaType: MediaType;
}

export const URLPickerTab = (props: Props) => {
  const { newValue, setNewValue, mediaType } = props;
  const styles = useStyles2(getStyles);

  const imgSrc = getPublicOrAbsoluteUrl(newValue!);

  let shortName = newValue?.substring(newValue.lastIndexOf('/') + 1, newValue.lastIndexOf('.'));
  if (shortName.length > 20) {
    shortName = shortName.substring(0, 20) + '...';
  }

  return (
    <>
      <Field>
        <Input onChange={(e) => setNewValue(e.currentTarget.value)} value={newValue} />
      </Field>
      <div className={styles.iconContainer}>
        <Field label="Preview">
          <div className={styles.iconPreview}>
            {mediaType === MediaType.Icon && <SVG src={imgSrc} className={styles.img} />}
            {mediaType === MediaType.Image && newValue && <img src={imgSrc} className={styles.img} />}
          </div>
        </Field>
        <Label>{shortName}</Label>
      </div>
    </>
  );
};

const getStyles = (theme: GrafanaTheme2) => ({
  iconContainer: css`
    display: flex;
    flex-direction: column;
    width: 80%;
    align-items: center;
    align-self: center;
  `,
  iconPreview: css`
    width: 238px;
    height: 198px;
    border: 1px solid ${theme.colors.border.medium};
    display: flex;
    align-items: center;
    justify-content: center;
  `,
  img: css`
    width: 147px;
    height: 147px;
    fill: ${theme.colors.text.primary};
  `,
});

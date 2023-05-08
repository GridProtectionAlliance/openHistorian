import React from 'react';

import { selectors } from '@grafana/e2e-selectors';
import { Button, LinkButton } from '@grafana/ui';
import { contextSrv } from 'app/core/core';
import { AccessControlAction } from 'app/types';

export interface Props {
  exploreUrl: string;
  canSave: boolean;
  canDelete: boolean;
  onDelete: () => void;
  onSubmit: (event: React.MouseEvent<HTMLButtonElement>) => void;
  onTest: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
}

export function ButtonRow({ canSave, canDelete, onDelete, onSubmit, onTest, exploreUrl }: Props) {
  const canExploreDataSources = contextSrv.hasPermission(AccessControlAction.DataSourcesExplore);

  return (
    <div className="gf-form-button-row">
      <Button variant="secondary" fill="solid" type="button" onClick={() => history.back()}>
        Back
      </Button>
      <LinkButton variant="secondary" fill="solid" href={exploreUrl} disabled={!canExploreDataSources}>
        Explore
      </LinkButton>
      <Button
        type="button"
        variant="destructive"
        disabled={!canDelete}
        onClick={onDelete}
        aria-label={selectors.pages.DataSource.delete}
      >
        Delete
      </Button>
      {canSave && (
        <Button
          type="submit"
          variant="primary"
          disabled={!canSave}
          onClick={(event) => onSubmit(event)}
          aria-label={selectors.pages.DataSource.saveAndTest}
        >
          Save &amp; test
        </Button>
      )}
      {!canSave && (
        <Button variant="primary" onClick={onTest}>
          Test
        </Button>
      )}
    </div>
  );
}

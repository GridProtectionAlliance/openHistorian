import React, { FC, Fragment, useMemo, useState } from 'react';
import { useDispatch } from 'react-redux';

import { ConfirmModal, useStyles2 } from '@grafana/ui';
import { contextSrv } from 'app/core/services/context_srv';
import { AlertManagerCortexConfig } from 'app/plugins/datasource/alertmanager/types';

import { Authorize } from '../../components/Authorize';
import { deleteTemplateAction } from '../../state/actions';
import { getAlertTableStyles } from '../../styles/table';
import { getNotificationsPermissions } from '../../utils/access-control';
import { makeAMLink } from '../../utils/misc';
import { CollapseToggle } from '../CollapseToggle';
import { DetailsField } from '../DetailsField';
import { ProvisioningBadge } from '../Provisioning';
import { ActionIcon } from '../rules/ActionIcon';

import { ReceiversSection } from './ReceiversSection';
import { TemplateEditor } from './TemplateEditor';

interface Props {
  config: AlertManagerCortexConfig;
  alertManagerName: string;
}

export const TemplatesTable: FC<Props> = ({ config, alertManagerName }) => {
  const dispatch = useDispatch();
  const [expandedTemplates, setExpandedTemplates] = useState<Record<string, boolean>>({});
  const tableStyles = useStyles2(getAlertTableStyles);
  const permissions = getNotificationsPermissions(alertManagerName);

  const templateRows = useMemo(() => {
    const templates = Object.entries(config.template_files);

    return templates.map(([name, template]) => ({
      name,
      template,
      provenance: (config.template_file_provenances ?? {})[name],
    }));
  }, [config]);
  const [templateToDelete, setTemplateToDelete] = useState<string>();

  const deleteTemplate = () => {
    if (templateToDelete) {
      dispatch(deleteTemplateAction(templateToDelete, alertManagerName));
    }
    setTemplateToDelete(undefined);
  };

  return (
    <ReceiversSection
      title="Message templates"
      description="Templates construct the messages that get sent to the contact points."
      addButtonLabel="New template"
      addButtonTo={makeAMLink('/alerting/notifications/templates/new', alertManagerName)}
      showButton={contextSrv.hasPermission(permissions.create)}
    >
      <table className={tableStyles.table} data-testid="templates-table">
        <colgroup>
          <col className={tableStyles.colExpand} />
          <col />
          <col />
        </colgroup>
        <thead>
          <tr>
            <th></th>
            <th>Template</th>
            <Authorize actions={[permissions.update, permissions.delete]}>
              <th>Actions</th>
            </Authorize>
          </tr>
        </thead>
        <tbody>
          {!templateRows.length && (
            <tr className={tableStyles.evenRow}>
              <td colSpan={3}>No templates defined.</td>
            </tr>
          )}
          {templateRows.map(({ name, template, provenance }, idx) => {
            const isExpanded = !!expandedTemplates[name];
            return (
              <Fragment key={name}>
                <tr key={name} className={idx % 2 === 0 ? tableStyles.evenRow : undefined}>
                  <td>
                    <CollapseToggle
                      isCollapsed={!expandedTemplates[name]}
                      onToggle={() => setExpandedTemplates({ ...expandedTemplates, [name]: !isExpanded })}
                    />
                  </td>
                  <td>
                    {name} {provenance && <ProvisioningBadge />}
                  </td>
                  <td className={tableStyles.actionsCell}>
                    {provenance && (
                      <ActionIcon
                        to={makeAMLink(
                          `/alerting/notifications/templates/${encodeURIComponent(name)}/edit`,
                          alertManagerName
                        )}
                        tooltip="view template"
                        icon="file-alt"
                      />
                    )}
                    {!provenance && (
                      <Authorize actions={[permissions.update, permissions.delete]}>
                        <Authorize actions={[permissions.update]}>
                          <ActionIcon
                            to={makeAMLink(
                              `/alerting/notifications/templates/${encodeURIComponent(name)}/edit`,
                              alertManagerName
                            )}
                            tooltip="edit template"
                            icon="pen"
                          />
                        </Authorize>
                        <Authorize actions={[permissions.delete]}>
                          <ActionIcon
                            onClick={() => setTemplateToDelete(name)}
                            tooltip="delete template"
                            icon="trash-alt"
                          />
                        </Authorize>
                      </Authorize>
                    )}
                  </td>
                </tr>
                {isExpanded && (
                  <tr className={idx % 2 === 0 ? tableStyles.evenRow : undefined}>
                    <td></td>
                    <td colSpan={2}>
                      <DetailsField label="Description" horizontal={true}>
                        <TemplateEditor
                          width={'auto'}
                          height={'auto'}
                          autoHeight={true}
                          value={template}
                          showLineNumbers={false}
                          monacoOptions={{
                            readOnly: true,
                            scrollBeyondLastLine: false,
                          }}
                        />
                      </DetailsField>
                    </td>
                  </tr>
                )}
              </Fragment>
            );
          })}
        </tbody>
      </table>

      {!!templateToDelete && (
        <ConfirmModal
          isOpen={true}
          title="Delete template"
          body={`Are you sure you want to delete template "${templateToDelete}"?`}
          confirmText="Yes, delete"
          onConfirm={deleteTemplate}
          onDismiss={() => setTemplateToDelete(undefined)}
        />
      )}
    </ReceiversSection>
  );
};

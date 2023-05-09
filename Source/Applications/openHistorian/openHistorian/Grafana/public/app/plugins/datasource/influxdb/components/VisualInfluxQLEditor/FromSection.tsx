import React from 'react';

import { Seg } from './Seg';
import { toSelectableValue } from './toSelectableValue';

// we use the value "default" as a magic-value, it means
// we use the default retention-policy.
// unfortunately, IF the user has a retention-policy named "default",
// and it is not the default-retention-policy in influxdb,
// bad things will happen.
// https://github.com/grafana/grafana/issues/4347 :-(
// FIXME: we could maybe at least detect here that problem-is-happening,
// and show an error message or something.
// unfortunately, currently the ResponseParser does not return the
// is-default info for the retention-policies, so that should change first.

type Props = {
  onChange: (policy: string | undefined, measurement: string | undefined) => void;
  policy: string | undefined;
  measurement: string | undefined;
  getPolicyOptions: () => Promise<string[]>;
  getMeasurementOptions: (filter: string) => Promise<string[]>;
};

export const FromSection = ({
  policy,
  measurement,
  onChange,
  getPolicyOptions,
  getMeasurementOptions,
}: Props): JSX.Element => {
  const handlePolicyLoadOptions = async () => {
    const allPolicies = await getPolicyOptions();
    return allPolicies.map(toSelectableValue);
  };

  const handleMeasurementLoadOptions = async (filter: string) => {
    const allMeasurements = await getMeasurementOptions(filter);
    return allMeasurements.map(toSelectableValue);
  };

  return (
    <>
      <Seg
        allowCustomValue
        value={policy ?? ''}
        loadOptions={handlePolicyLoadOptions}
        onChange={(v) => {
          onChange(v.value, measurement);
        }}
      />
      <Seg
        allowCustomValue
        value={measurement ?? 'select measurement'}
        loadOptions={handleMeasurementLoadOptions}
        filterByLoadOptions
        onChange={(v) => {
          onChange(policy, v.value);
        }}
      />
    </>
  );
};

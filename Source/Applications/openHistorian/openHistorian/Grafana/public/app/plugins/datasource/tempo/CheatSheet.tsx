import React from 'react';

export default function CheatSheet() {
  return (
    <div>
      <h2 id="tempo-cheat-sheet">Tempo Cheat Sheet</h2>
      <p>
        Tempo is a trace id lookup store. Enter a trace id in the above field and hit “Run Query” to retrieve your
        trace. Tempo is generally paired with other datasources such as Loki or Prometheus to find traces.
      </p>
      <p>
        Here are some{' '}
        <a href="https://grafana.com/docs/tempo/latest/guides/instrumentation/" target="blank">
          instrumentation examples
        </a>{' '}
        to get you started with trace discovery through logs and metrics (exemplars).
      </p>
    </div>
  );
}

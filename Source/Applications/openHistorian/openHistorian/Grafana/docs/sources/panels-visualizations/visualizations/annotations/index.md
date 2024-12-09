---
aliases:
  - ../../features/panels/annotations/
  - ../../panels/visualizations/annotations/
  - ../../visualizations/annotations/
description: Configure options for Grafana's annotations list visualization
keywords:
  - grafana
  - Annotations
  - panel
  - documentation
labels:
  products:
    - cloud
    - enterprise
    - oss
title: Annotations list
weight: 100
---

# Annotations list

The annotations list shows a list of available annotations you can use to view annotated data. Various options are available to filter the list based on tags and on the current dashboard.

## Panel options

{{< docs/shared lookup="visualizations/panel-options.md" source="grafana" version="<GRAFANA_VERSION>" >}}

## Annotation query

The following options control the source query for the list of annotations.

### Query Filter

Use the query filter to create a list of annotations from all dashboards in your organization or the current dashboard in which this panel is located. It has the following options:

- All dashboards - List annotations from all dashboards in the current organization.
- This dashboard - Limit the list to the annotations on the current dashboard.

### Time Range

Use the time range option to specify whether the list should be limited to the current time range. It has the following options:

- None - no time range limit for the annotations query.
- This dashboard - Limit the list to the time range of the dashboard where the annotations list is available.

### Tags

Use the tags option to filter the annotations by tags. You can add multiple tags in order to refine the list.

{{% admonition type="note" %}}
Optionally, leave the tag list empty and filter on the fly by selecting tags that are listed as part of the results on the panel itself.
{{% /admonition %}}

### Limit

Use the limit option to limit the number of results returned.

## Display

These options control additional meta-data included in the annotations list display.

### Show user

Use this option to show or hide which user created the annotation.

### Show time

Use this option to show or hide the time the annotation creation time.

### Show Tags

Use this option to show or hide the tags associated with an annotation. _NB_: You can use the tags to live-filter the annotations list on the visualization itself.

## Link behavior

### Link target

Use this option to chose how to view the annotated data. It has the following options.

- Panel - This option will take you directly to a full-screen view of the panel with the corresponding annotation
- Dashboard - This option will focus the annotation in the context of a complete dashboard

### Time before

Use this option to set the time range before the annotation. Use duration string values like "1h" = 1 hour, "10m" = 10 minutes, etc.

### Time after

Use this option to set the time range after the annotation.

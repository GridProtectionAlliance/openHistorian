// Copyright 2023 Grafana Labs
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package grafanaplugin

import (
	"github.com/grafana/grafana/packages/grafana-schema/src/common"
	"github.com/grafana/grafana/pkg/plugins/pfs"
)

// This file (with its sibling .cue files) implements pfs.GrafanaPlugin
pfs.GrafanaPlugin

composableKinds: DataQuery: {
	maturity: "experimental"

	lineage: {
		seqs: [
			{
				schemas: [
					// v0.0
					{
						common.DataQuery

						// Specifies the query label selectors.
						labelSelector: string | *"{}"
						// Specifies the type of profile to query.
						profileTypeId:   string
						#ParcaQueryType: "metrics" | "profile" | *"both" @cuetsy(kind="type")
					},
				]
			},
		]
	}
}

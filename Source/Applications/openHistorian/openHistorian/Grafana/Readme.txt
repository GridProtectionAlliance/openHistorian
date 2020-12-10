﻿Be careful making updates to this folder, especially custom.ini and grafana.db (see below), as these are the files that get included with the openHistorian setup package.

Do not point your debug system to this folder - instead copy the contents of this folder to the debug output folder, i.e.: openHistorian/Build/Output/Debug/Applications/openHistorian/Grafana

When Grafana exists in the debug output folder, you can properly debug the openHistorian while also running a self-hosted copy of Grafana

When updating to a new Grafana version, include the following files for deployment (this may change for newer versions):

bin/
	* (all files)
conf/
	custom.ini (from openHistorian)
	* (all files)
data/ (from openHistorian)
	plugins/
		* (all files)
	grafana.db
	Home.json
plugins-bundled/
	* (all files)
public/
	* (all files)
scripts/
	* (all files)
tools/
	* (all files)
LICENSE.md
NOTICE.md
README.md
Readme.txt (from openHistorian)
VERSION

Make sure data/log and data/sessions folders are not included in the deployed installation files.

Always run the "WixFolderGen" tool after updates to make sure any new files get included within the install package.

Note that the conf/custom.ini file contains settings to properly allow self-hosting of Grafana and using the openHistorian for user authentication as well as a front-end to Grafana through the openHistorian primary url: http://localhost:8180/grafana

If you need to make changes to the data/grafana.db to include custom dashboards or new default data sources, make sure to clean up the database such that no spurious records remain after updates, e.g., additional users that may have been auto-synchronized by the openHistorian - the deployed database should have a single user "admin". More specifically, only the following tables should have records:

dashboard (at least 1 record for updated home page)
data_source (at least 2 records for OHDATA and OHSTAT)
migration_log (should contain records for migration log up to deployed version)
org (1 record)
org_user (1 record)
preferences (1 record)
sqlite_sequence (make sure sequence numbers match current records, especially if records where deleted)
user (1 record for "admin" user)

All other tables should be empty, including dashboard_version.
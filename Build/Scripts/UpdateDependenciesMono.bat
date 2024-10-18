::*******************************************************************************************************
::  UpdateDependencies.bat - Gbtc
::
::  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
::
::  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
::  the NOTICE file distributed with this work for additional information regarding copyright ownership.
::  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
::  not use this file except in compliance with the License. You may obtain a copy of the License at:
::
::      http://www.opensource.org/licenses/eclipse-1.0.php
::
::  Unless agreed to in writing, the subject software distributed under the License is distributed on an
::  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
::  License for the specific language governing permissions and limitations.
::
::  Code Modification History:
::  -----------------------------------------------------------------------------------------------------
::  07/31/2013 - J. Ritchie Carroll
::       Generated original version of source code.
::  08/26/2013 - J. Ritchie Carroll
::       Updated to roll-down schema files from Grid Solutions Framework.
::
::*******************************************************************************************************

@ECHO OFF

SETLOCAL

SET pwd=%CD%
IF "%git%" == "" SET git=%PROGRAMFILES(X86)%\Git\cmd\git.exe

SET defaulttarget=%LOCALAPPDATA%\Temp\openHistorian
IF "%source%" == "" SET source=\\GPAWEB\NightlyBuilds\GridSolutionsFramework\Mono
IF "%sttp%" == "" SET sttp=\\GPAWEB\NightlyBuilds\sttp-gsfapi\Beta
IF "%target%" == "" SET target=%defaulttarget%

SET libraries=%source%\Libraries\*.*
SET sttplibrary=%sttp%\lib\sttp.gsf.dll
SET dependencies=%target%\Source\Dependencies\GSF
SET sourcemasterbuild=%source%\Build Scripts\MasterBuild.buildproj
SET targetmasterbuild=%target%\Build\Scripts
SET sourcetools=%source%\Tools
SET targettools=%target%\Source\Applications\openHistorian\openHistorianSetup

::Grafana Panels
::SET GrafanaSource=\\GPAWEB\NightlyBuilds\GrafanaPanels\Binaries
::SET GrafanaTarget=%target%\Source\Applications\openHistorian\openHistorian\Grafana\data\plugins

ECHO.
ECHO Entering working directory...
IF EXIST "%target%" IF NOT EXIST "%target%"\.git RMDIR /S /Q "%target%"
IF NOT EXIST "%target%" MKDIR "%target%"
CD /D %target%

:UpdateDependencies
ECHO.
ECHO Updating dependencies...
XCOPY "%libraries%" "%dependencies%\" /Y /E
XCOPY "%sttplibrary%" "%dependencies%\" /Y
XCOPY "%sourcemasterbuild%" "%targetmasterbuild%\" /Y
COPY /Y "%sourcetools%\ConfigCrypter\ConfigCrypter.exe" "%targettools%\ConfigCrypter.exe"
COPY /Y "%sourcetools%\ConfigEditor\ConfigEditor.exe" "%targettools%\ConfigurationEditor.exe"
COPY /Y "%sourcetools%\CSVDataManager\CSVDataManager.exe" "%targettools%\CSVDataManager.exe"
COPY /Y "%sourcetools%\DataMigrationUtility\DataMigrationUtility.exe" "%targettools%\DataMigrationUtility.exe"
COPY /Y "%sourcetools%\HistorianPlaybackUtility\HistorianPlaybackUtility.exe" "%targettools%\HistorianPlaybackUtility.exe"
COPY /Y "%sourcetools%\HistorianView\HistorianView.exe" "%targettools%\HistorianView.exe"
COPY /Y "%sourcetools%\StatHistorianReportGenerator\StatHistorianReportGenerator.exe" "%targettools%\StatHistorianReportGenerator.exe"
COPY /Y "%sourcetools%\NoInetFixUtil\NoInetFixUtil.exe" "%targettools%\NoInetFixUtil.exe"
COPY /Y "%sourcetools%\DNP3ConfigGenerator\DNP3ConfigGenerator.exe" "%targettools%\DNP3ConfigGenerator.exe"
COPY /Y "%sourcetools%\LogFileViewer\LogFileViewer.exe" "%targettools%\LogFileViewer.exe"
COPY /Y "%sourcetools%\UpdateTagNames\UpdateTagNames.exe" "%targettools%\UpdateTagNames.exe"
COPY /Y "%sourcetools%\GEPDataExtractor\GEPDataExtractor.exe" "%targettools%\GEPDataExtractor.exe"
COPY /Y "%sourcetools%\CreateOutputStream\CreateOutputStream.exe" "%targettools%\CreateOutputStream.exe"
COPY /Y "%sourcetools%\BulkCalculationState\BulkCalculationState.exe" "%targettools%\BulkCalculationState.exe"
COPY /Y "%sourcetools%\AdapterExplorer\AdapterExplorer.exe" "%targettools%\AdapterExplorer.exe"
::ECHO Updating Grafana Panels...
::XCOPY "%GrafanaSource%\GPA-PhasorMap" "%GrafanaTarget%\grafana-pmumap-panel\" /Y /E /U
::XCOPY "%GrafanaSource%\Grafana-oh-datadownload" "%GrafanaTarget%\openhistporian-datadownload-panel\" /Y /E /U
::XCOPY "%GrafanaSource%\openHistorianGrafanaAlarmPanel" "%GrafanaTarget%\openHistorianGrafanaAlarmPanel\" /Y /E /U
::XCOPY "%GrafanaSource%\openHistorian-grafana" "%GrafanaTarget%\gridprotectionalliance-openhistorian-datasource\" /Y /E /U

:Finish
ECHO.
ECHO Update complete

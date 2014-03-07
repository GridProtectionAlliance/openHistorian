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
::  07/28/2011 - Stephen C. Wills
::       Generated original version of source code.
::
::*******************************************************************************************************

@ECHO OFF

SET tfs="%VS110COMNTOOLS%\..\IDE\tf.exe"
SET db1="openHistorian.db"
SET db2="openHistorian-InitialDataSet.db"
SET db3="openHistorian-SampleDataSet.db"
SET script1="openHistorian.sql"
SET script2="InitialDataSet.sql"
SET script3="SampleDataSet.sql"
SET dbfiles=%db1% %db2% %db3%
SET scripts=%script1% %script2% %script3%
SET /p checkin=Check-in updates (Y or N)? 

ECHO.
ECHO Getting latest version...
%tfs% get %dbfiles% %scripts% /version:T /force /noprompt

ECHO.
ECHO Checking out DBs...
%tfs% checkout %dbfiles% /noprompt

ECHO.
ECHO Updating DBs...
DEL %dbfiles%

sqlite3 %db1% < %script1%
ECHO %db1%

COPY %db1% %db2% > NUL
sqlite3 %db2% < %script2%
ECHO %db2%

COPY %db2% %db3% > NUL
sqlite3 %db3% < %script3%
ECHO %db3%

IF /I "%checkin%" == "Y" GOTO Checkin
GOTO Finalize

:Checkin
ECHO.
ECHO Checking in dependencies...
%tfs% checkin %dbfiles% /noprompt /comment:"Grid Solution Framework Time-series Schema: Updated SQLite databases."

:Finalize
ECHO.
ECHO Update complete
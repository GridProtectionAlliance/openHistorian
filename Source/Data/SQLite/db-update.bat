::*******************************************************************************************************
::  UpdateDependencies.bat - Gbtc
::
::  Tennessee Valley Authority, 2009
::  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
::
::  This software is made freely available under the TVA Open Source Agreement (see below).
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
ECHO Checking in DBs...
%tfs% checkin %dbfiles% /noprompt /comment:"VS2012: Updated SQLite databases."

:Finalize
ECHO.
ECHO Update complete
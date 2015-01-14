::*******************************************************************************************************
::  db-update.bat - Gbtc
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

SETLOCAL EnableDelayedExpansion

SET tfs="%VS110COMNTOOLS%\..\IDE\tf.exe"
SET db[1]="openHistorian.db"
SET db[2]="openHistorian-InitialDataSet.db"
SET db[3]="openHistorian-SampleDataSet.db"
SET script[1]="openHistorian.sql"
SET script[2]="InitialDataSet.sql"
SET script[3]="SampleDataSet.sql"

FOR %%i IN (1 2 3) DO (
    IF NOT "!update!" == "true" (
        ECHO Checking for pending edits on !script[%%i]!...
        FOR /f "delims=" %%s IN ('CALL %tfs% status !script[%%i]!') DO SET status=%%s
        IF NOT "!status!" == "There are no pending changes." SET update=true
    )
    
    IF NOT "!update!" == "true" (
        ECHO Checking for changes to !script[%%i]! since last update...
        FOR /f "tokens=2 delims=;" %%c IN ('CALL %tfs% localversions !db[%%i]!') DO SET changeset=%%c
        
        FOR /f %%c IN ('CALL %tfs% history !script[%%i]! /version:!changeset!~T /noprompt') DO (
            SET numeric=true
            FOR /f "delims=0123456789" %%a IN ("%%c") DO SET numeric=false
            
            IF "!numeric!" == "true" (
                IF %%c GTR !changeset:~1! SET update=true
            )
        )
    )
    
    IF "!update!" == "true" (
        ECHO Updating !db[%%i]!...
        %tfs% checkout !db[%%i]! /noprompt
        
        IF "!prevdb!" == "" (
            DEL !db[%%i]!
        ) ELSE (
            COPY /Y !prevdb! !db[%%i]!
        )
        
        sqlite3 !db[%%i]! < !script[%%i]!
    )
    
    SET prevdb=!db[%%i]!
)
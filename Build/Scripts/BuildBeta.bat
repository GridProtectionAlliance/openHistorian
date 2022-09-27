::*******************************************************************************************************
::  BuildBeta.bat - Gbtc
::
::  Tennessee Valley Authority, 2009
::  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
::
::  This software is made freely available under the TVA Open Source Agreement (see below).
::
::  Code Modification History:
::  -----------------------------------------------------------------------------------------------------
::  10/05/2009 - Pinal C. Patel
::       Generated original version of source code.
::  10/20/2009 - Pinal C. Patel
::       Modified to force a build and suppress archives from being published to public locations.
::  10/03/2010 - Pinal C. Patel
::       Updated to use MSBuild 4.0.
::
::*******************************************************************************************************

@ECHO OFF

SetLocal

IF NOT "%1" == "" SET logflag=/l:FileLogger,Microsoft.Build.Engine;logfile=%1

ECHO BuildBeta: C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe openHistorian.buildproj /p:ForceBuild=true %logflag%
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" openHistorian.buildproj /p:SourceArchiveWorkingDirectory=D:\temp\openHistorian\ /p:ForceBuild=true %logFlag%
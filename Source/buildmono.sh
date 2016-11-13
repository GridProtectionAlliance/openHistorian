#!/bin/sh
#Build openHistorian on Mono - to execute, chmod +x buildmono.sh
xbuild /p:Configuration=Mono /p:PreBuildEvent="" /p:PostBuildEvent="" openHistorian.sln
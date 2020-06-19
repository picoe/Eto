#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
msbuild /v:minimal /t:Package /p:InformationalVersion=$1 $DIR/build/Build.proj

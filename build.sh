#!/bin/bash

cd build
xbuild /t:Package /p:BuildVersion=$1 Build.proj

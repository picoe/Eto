#!/bin/bash

xbuild /t:Package /p:BuildVersion=$1 Build.proj

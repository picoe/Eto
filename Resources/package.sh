#!/bin/bash

xbuild /t:Package /p:BuildVersion=$1 /p:Platform=Mac Publish.targets

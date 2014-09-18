#!/bin/bash

xbuild /t:PrepareMac /p:BuildVersion=$1 Publish.targets

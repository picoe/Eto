#!/bin/bash

ulimit -n 5000

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
msbuild /t:BuildAddins $DIR/Build.proj

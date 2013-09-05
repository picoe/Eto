#!/bin/bash

function tounix () {
	SAVEIFS=$IFS
	IFS=$(echo -en "\n\b")
	for f in $1
	do
		ff=${f#$SOURCE_DIR}
		echo "Processing $ff"
		# remove BOM
		if [[ -f "$f" && `head -c 3 "$f"` == $'\xef\xbb\xbf' ]]; then
			# file exists and has UTF-8 BOM
			tail -c +4 "$f" > "$f.bak" && mv "$f.bak" "$f"
		fi
		# CRLF to LF
		perl -pi -e 's/\r\n/\n/g' "$f"
	
	done
	IFS=$SAVEIFS
}

function towindows () {
	SAVEIFS=$IFS
	IFS=$(echo -en "\n\b")
	for f in $1
	do
		ff=${f#$SOURCE_DIR}
		echo "Processing $ff"
		# remove BOM
		if [[ -f "$f" && `head -c 3 "$f"` == $'\xef\xbb\xbf' ]]; then
			# file exists and has UTF-8 BOM
			tail -c +4 "$f" > "$f.bak" && mv "$f.bak" "$f"
		fi
		
		# LF to CRLF
		perl -pi -e 's/\r\n/\n/g' "$f"
		perl -pi -e 's/\n/\r\n/' "$f"
	done
	IFS=$SAVEIFS
}

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
SOURCE_DIR=$DIR/../Source

source_files=$(find $SOURCE_DIR -name *.cs)
project_files=$(find $SOURCE_DIR -name *.csproj)

if [ "$1" == "win"  -o  "$1" == "windows" ]
then
	towindows "$source_files"
	towindows "$project_files"

elif [ "$1" == "unix" ]
then
	tounix "$source_files"
	tounix "$project_files"
	
else

echo
echo "This script is used to ensure all files follow platform conventions"
echo "when sharing code between Windows and *nix systems, and removes the BOM"
echo "from all files."
echo
echo "usage: ./fixfiles.sh (unix|windows)"
echo
echo "  windows: use windows CRLF line endings"
echo "  unix: use unix LF line endings"

fi



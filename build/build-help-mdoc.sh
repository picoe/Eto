#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
BUILD_OUTPUT_PATH=$DIR/../artifacts
INPUT_PATH=$BUILD_OUTPUT_PATH/Release/net45

OUTPUT_PATH=$BUILD_OUTPUT_PATH/help
MDOC_OUTPUT_PATH=$OUTPUT_PATH/mdoc
MDOC_SOURCES_PATH=$MDOC_OUTPUT_PATH/sources
XML_OUTPUT_PATH=$OUTPUT_PATH/xml
HTML_OUTPUT_PATH=$OUTPUT_PATH/html-mdoc

rm -rf $MDOC_OUTPUT_PATH
rm -rf $XML_OUTPUT_PATH
mkdir -p $XML_OUTPUT_PATH
cp -r $DIR/xml/* $XML_OUTPUT_PATH
mdoc update -i "$INPUT_PATH/Eto.xml" -o "$XML_OUTPUT_PATH" "$INPUT_PATH/Eto.dll"
mkdir -p $MDOC_SOURCES_PATH
mdoc assemble -o "$MDOC_SOURCES_PATH/Eto" "$XML_OUTPUT_PATH" 

ETODOC='<?xml version="1.0"?>\n
<monodoc>\n
\t<node label="Eto.Forms" name="Eto" parent="libraries" />\n
\t<source provider="ecma" basefile="Eto" path="Eto"/>\n
</monodoc>'

echo -e $ETODOC > "$MDOC_SOURCES_PATH/Eto.source"

MONODOC='<?xml version="1.0"?>\n
<node label="Eto.Forms Documentation" name="libraries">\n
\t<node label="Commands and Files" name="man" />\n
\t<node label="Languages" name="languages" />\n
\t<node label="Tools" name="tools" />\n
\t<node label="Various" name="various" />\n
</node>'

echo -e $MONODOC > $MDOC_OUTPUT_PATH/monodoc.xml

if [ "$1" == "html" ]
then
	rm -rf $HTML_OUTPUT_PATH
	mdoc export-html -out "$HTML_OUTPUT_PATH" "$XML_OUTPUT_PATH"
	# mdoc export-html-webdoc --out="$HTML_OUTPUT_PATH" "$MDOC_SOURCES_PATH/Eto.tree"
fi

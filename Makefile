

VERSION=1.2

OUTPUT_DIR=./BuildOutput
OUTPUT_ZIP=Eto.Forms-$(VERSION).zip
MD_PATH=/Applications/Xamarin\ Studio.app
MDTOOL=$(MD_PATH)/Contents/MacOS/mdtool
SOURCE=./Source
ETO_SLN="$(SOURCE)/Eto.sln"
ETO_IOS_SLN="$(SOURCE)/Eto - iOS.sln"
ZIP=zip -9 -r

assemblies=\
"Eto.Json.dll"\
"Eto.Platform.Gtk.dll"\
"Eto.Platform.Mac.dll"\
"Eto.Platform.Windows.dll"\
"Eto.Platform.Wpf.dll"\
"Eto.dll"


all: info

clean:
	rm -Rf $(OUTPUT_DIR)

info:
	@echo
	@echo NOTE: This script is used to build the release package.  Follow these steps:
	@echo "   1. on windows: buildwin.cmd"
	@echo "   2. on osx: make release"
	@echo
	@echo This will produce $(OUTPUT_ZIP) in the BuildOutput directory for release.
	@echo

release: zip zip-mac


	
zip-mac: build-mac
	cd $(OUTPUT_DIR); \
	$(ZIP) $(OUTPUT_ZIP) Debug/Eto.Test.Mac.app -x *.mdb;

build-mac:
	$(MDTOOL) build -c:Mac\ Debug $(ETO_SLN)

build-ios:
	$(MDTOOL) build -c:Debug $(ETO_IOS_SLN)
	$(MDTOOL) build -c:Release $(ETO_IOS_SLN)
	
debug-bin:
	$(call process-release,${OUTPUT_DIR}/Debug)
	$(call convert-pdb,${OUTPUT_DIR}/Debug)

release-bin:
	$(call process-release,$(OUTPUT_DIR)/Release)
	$(call convert-pdb,$(OUTPUT_DIR)/Release)
	rm -fR $(OUTPUT_DIR)/Release/Eto.Test.*

zip: debug-bin release-bin build-ios
	@echo Zipping to $(OUTPUT_ZIP)
	cd $(OUTPUT_DIR); \
	rm -f $(OUTPUT_ZIP); \
	$(ZIP) $(OUTPUT_ZIP) Debug Release iOS;


define process-release
	cd $1; \
	rm -f *.mdb; \
	rm -f Eto.Test*.pdb; \
	rm -f XamMac.*; \
	rm -f Eto.Test.XamMac*; \
	rm -f *.vshost.exe*; \
	rm -f Newtonsoft.Json.pdb; \
	rm -f Newtonsoft.Json.xml;
endef

define convert-pdb
	cd $1; \
	for i in $(assemblies); do \
		echo Converting $$i...; \
		pdb2mdb $$i; \
	done

endef

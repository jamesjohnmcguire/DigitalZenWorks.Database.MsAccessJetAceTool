CD %~dp0
CD ..\SourceCode

CALL dotnet publish --configuration Release --output Release  -p:DebugType=embedded;EnableCompressionInSingleFile=true;IncludeNativeLibrariesForSelfExtract=true;PublishReadyToRun=true;PublishSingleFile=true -restore --runtime win-x64 --self-contained true -target:rebuild MsAccessJetAceTool

IF "%1"=="release" GOTO release
GOTO end

:release

CD Release
DEL /Q *.xml *.accdb

7z u MsAccessJetAceTool.zip .
MOVE MsAccessJetAceTool.zip ..

CD ..
gh release create v%2 --notes %3 MsAccessJetAceTool.zip

:end

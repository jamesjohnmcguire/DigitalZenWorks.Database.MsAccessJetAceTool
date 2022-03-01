CD %~dp0
CD ..\SourceCode

CALL dotnet publish --configuration Release -p:PublishReadyToRun=true;PublishSingleFile=true --runtime win-x64 --self-contained true --output Release MsAccessJetAceTool

IF "%1"=="release" GOTO release
GOTO end

:release

CD Release
7z u MsAccessJetAceTool.zip .
MOVE MsAccessJetAceTool.zip ..

CD ..
gh release create v%2 --notes %3 MsAccessJetAceTool.zip

:end

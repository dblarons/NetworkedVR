If (Test-Path .\Assets\packages) {
    Remove-Item -Recurse -Force .\Assets\packages
}

If (Test-Path .\Assets\Scripts\flatbuffers\compiled) {
    Remove-Item -Recurse -Force .\Assets\Scripts\flatbuffers\compiled
}

nuget install

$badDlls = @(
    ".\Assets\packages\DevZH.FlatBuffers.1.4.0\lib\netstandard1.1",
    ".\Assets\packages\DevZH.FlatBuffers.1.4.0\lib\netstandard1.0",

    ".\Assets\packages\NetMQ.4.0.0.4-rc4\lib\monoandroid60",
    ".\Assets\packages\NetMQ.4.0.0.4-rc4\lib\net40",
    ".\Assets\packages\NetMQ.4.0.0.4-rc4\lib\netstandard1.3",
    ".\Assets\packages\NetMQ.4.0.0.4-rc4\lib\xamarinios10",

    ".\Assets\packages\AsyncIO.0.1.23.0\lib\net40",
    ".\Assets\packages\AsyncIO.0.1.23.0\lib\netstandard1.3",
    ".\Assets\packages\AsyncIO.0.1.23.0\lib\xamarinios10"
)

foreach ($dll in $badDlls) {
    If (Test-Path $dll) {
       Remove-Item -Recurse -Force $dll
    }
}

.\scripts\flatc.exe --csharp --gen-onefile -o Assets\Scripts\flatbuffers\compiled Assets\Scripts\flatbuffers\schemas\networking.fbs

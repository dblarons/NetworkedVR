param (
    [switch]$setup = $false,
    [switch]$fbcompile = $false,
    [switch]$nugetreinstall = $false,
    [switch]$nugetinstall = $false
)

function NugetClean() {
    If (Test-Path .\Assets\packages) {
        Remove-Item -Recurse -Force .\Assets\packages
    }
}

function NugetInstall() {
    nuget install
}

function CleanFlatBuffers() {
    If (Test-Path .\Assets\Scripts\flatbuffers\compiled) {
        Remove-Item -Recurse -Force .\Assets\Scripts\flatbuffers\compiled
    }
}

function CompileFlatBuffers() {
    .\scripts\flatc.exe --csharp --gen-onefile -o Assets\Scripts\flatbuffers\compiled Assets\Scripts\flatbuffers\schemas\helloworld.fbs
}

If ($setup) {
    NugetClean
    NugetInstall
    CleanFlatBuffers
    CompileFlatBuffers
    return
}

If($fbcompile) {
    CleanFlatBuffers
    CompileFlatBuffers
    return
}

If($nugetreinstall) {
    NugetClean
    NugetInstall
    return
}

If($nugetinstall) {
    NugetInstall
    return
}
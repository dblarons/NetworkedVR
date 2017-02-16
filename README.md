# Networked VR Project

## Papers

Papers, overviews, and lists are in the [`reports`](reports) directory.

## Code

Code is in the [`Assets/Scripts`](Assets/Scripts) directory. This is a Unity naming convention.

## Set up directions
- Download [Unity v.5.5.0](https://unity3d.com/get-unity/download/archive)
- Clone this Github repo
- Install Nuget packages:
  - Mac
    - Install Nuget:  `brew install nuget`
    - On command line, navigate to NetworkedVR directory
    - Run command: `nuget install`
    - Navigate to the ./Assets/packages/ folder and delete all folders and meta 
      files except for `net35` in both the NetMQ and AsyncIO packages
  - PC
    - Open Visual Studio 2015
    - Navigate to Tools > Nuget Package Manager > Package Manager Console and click
    - When console appears, a pop up should appear asking if you want to restore packages; confirm
    - After packages install, navigate to the ./Assets/packages/ folder and delete 
      all folders and meta files except for `net35` in both the NetMQ and AsyncIO packages
- Flatbuffers:
  - Mac
    - `brew install flatbuffers`
    - Run [Bash compile script](./scripts/fbs_compile)
  - PC
    - Run [PowerShell compile script](./scripts/fbs_compile/ps1)
- Open Unity and try to press the play button
- Let Aaron know if you have any compile errors at this point
- Building on Linux:
  - When installing Unity, check the "Linux build support" checkbox.
  - Once in Unity, go to File > Build Settings
  - Select "Linux" as your target platform
  - Check the "Headless mode" checkbox
  - Click Build
  - Move executable to a Linux box, make it executable, and run it

## Report Notes

- [X] State of the art report
- [ ] Final report
  - [ ] Intro
  - [ ] What problem you are solving
  - [ ] Articlulate challenges (first 3-4 pages of paper)
  - [ ] How did I solve challenges
  - [ ] Implementation
  - [ ] Design
  - [ ] Final results
- [ ] Dr. Bodenheimer – Go through core VR problems
  - [ ] Clarify what this entails

## Packages

This project uses Nuget, but dependency management with Unity is a bit of
a mess given that it uses .NET 2.0 / C# 3.5, so the following is a record of
workarounds required for each installed package.

- NetMQ v4
  - Delete all folders and meta files except for `net35` in for both NetMQ and AsyncIO packages
  - [ ] Figure out how to only install net35 dep
    - Setting targetFramework does not appear to work
- Flatbuffers
  - Install flatc compiler
  - Mac: brew install flatbuffers
  - PC: Download latest flatc exe https://github.com/google/flatbuffers/releases
  - Compile flatbuffers into Assets/Scripts/flatbuffers/compiled

# Networked VR Project

## Papers

Papers, overviews, and lists are in the [`reports`](reports) directory.

## Code

Code is in the [`Assets/Scripts`](Assets/Scripts) directory. This is a Unity naming convention.

## Set up directions
- Download [Unity v.5.6.0b9](https://unity3d.com/unity/beta)
  - When installing Unity on Windows, check the "Linux build support" checkbox.
- Install SteamVR (first install Steam, then select tools from the drop down menu and scroll down until you see SteamVR)
- Clone this Github repo
- Install the [Nuget package manager CLI](https://docs.microsoft.com/en-us/nuget/guides/install-nuget)
  - I recommend first installing Chocolatey and then installing with Chocolatey
- Open PowerShell and run the following command:
  - powershell .\build.ps1 -setup
- Either open the project in Unity or double click on .\Assets\Playground.unity
- On Windows, press the play button
  - Let Aaron know if you have any compile errors at this point
- Building on Linux:
  - Once in Unity on Windows, go to File > Build Settings
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
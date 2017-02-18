# Networked VR Project

## Papers

Papers, overviews, and lists are in the [`reports`](reports) directory.

## Code

Code is in the [`Assets/Scripts`](Assets/Scripts) directory. This is a Unity naming convention.

## Set up directions
- Download [Unity v.5.5.0](https://unity3d.com/get-unity/download/archive)
- Clone this Github repo
- Open PowerShell and run the following command:
  - powershell .\build.ps1 -setup
- Either open the project in Unity or double click on .\Assets\Playground.unity
- Let Aaron know if you have any compile errors at this point
- Building on Linux:
  - When installing Unity on Mac or Windows, check the "Linux build support" checkbox.
  - Once in Unity on Mac or Windows, go to File > Build Settings
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
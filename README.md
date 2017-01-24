# Networked VR Project

## Papers

Papers, overviews, and lists are in the [`reports`](reports) directory.

## Code

Code is in the [`Assets/Scripts`](Assets/Scripts) directory. This is a Unity naming convention.

## Report Notes

- [ ] State of the art report
  - [ ] Send Dr. Dubey an initial list of papers that I have identified
    - Those will be my initial survey
  - [ ] Why is my implementation different than others?
    - Explore edge computing and how to create a p2p game using an edge
      computing design
      - One peer acts like central server until overloaded, then migrates state
        to another peer and all peers go communicate with that peer.
    - What problems does an edge computing approach have?
      - Redundancy: What happens if the peer which is currently acting as the
        central server is disconnected from the internet?
      - Performance: How many frames are dropped when migrating state from one
        peer to another?
    - What types of games should I focus on?
      - Leave out MMORPGs
      - Focus on 4-8 player high interaction games (requirement for
        consistently high FPS with lots of state updates like in VR settings)
    - Must determine some threshold at which state must be migrated
      - FPS threshold?
      - Network latency threshold?
  - [ ] Describing and learning a new technique
  - [ ] Going beyond existing and do something new
  - [ ] What is innovative?
    - [ ] First must understand what has been done
- [ ] Final report
  - [ ] Intro
  - [ ] What problem you are solving
  - [ ] Articlulate challenges (first 3-4 pages of paper)
  - [ ] How did I solve challenges
  - [ ] Implementation
  - [ ] Design
  - [ ] Final results
- [ ] Create private repo
  - [ ] Include docs
  - [ ] GitHub
  - [ ] Can be public after finished
- [ ] Look into zeromq more
  - [ ] Go through user guide
- [ ] Dr. Bodenheimer – Go through core VR problems
  - [ ] Clarify what this entails
- [X] Touch base with Dr. Johnson

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
    - [ ] Script this on Windows
      - I already wrote a bash script

Thoughts: If I have time, I should absolutely implement this protocol. If there
is not enough time, I can at the very least cite this paper as proof that
cheating _can_ be avoided, even if I do not avoid it in my implementation.

- [Low Latency and Cheat-proof Event Ordering for Peer-to-Peer Games](https://pdfs.semanticscholar.org/07b1/bb1faa9ad2d367359d1bb800023e6d90a783.pdf)
  - (page 1) New-Event Ordering (NEO) protocol
  - (page 1) Divide time into "rounds" which bound the max latency from one
    player to a majority of other players
  - (page 2) Cheat categories
    - Game, application, protocol, or network
  - (page 2) Five common protocol level cheats
    - Fixed-Delay Cheat: Artificially send packets slower than you receive them
      so that you can see other player's positions before you move yourself
    - Timestamp Cheat: Artificially send packets with timestamps that are
      before those that you receive making it look like you made a move first
    - Suppressed Update Cheat: Don't send updates to one or two players so that
      you are hidden from them but they are not hidden from you. Send an update
      just before you will be dropped.
    - Inconsistency Cheat: Send everyone your real position except for one
      player. That player is disadvantaged and no other players will agree with
      them. Later merged the timelines to hide the cheat.
    - Collusion Cheat: Two players colluding share information about a third
      player, allowing them two perspectives on that player when they should
      only have one.
  - (page 4) This paper focuses on a small subset of players; essentially
    a group of players within one AOI
  - (page 4) Players must send their packets within the round time to prove
    that they made their update in an orderly fashion
  - (page 4) Players vote on packets that were received from other players
    which provides proof that players sent their updates on time
  - (page 4) A drawback is that a majority of players can collude to cheat
    (kind of like Bitcoin)
  - (page 4-5) Basic NEO protocol
    - Solves for suppressed update and timestamp cheats
    - Send encrypted update to everyone; in next round, send key to that update
    - Players vote on packets received using bit vectors
    - A majority of votes must be received for a vote to be valid
  - (page 5) NEO with Pipelined Rounds
    - Use a pipeline similar to the one in a processor
    - Send updates continuiously and, instead of sending a key for the last
      round, send a key for several rounds ago
  - (page 6) Ajusting the Round Duration
    - If all updates are early, decrease round duraction, and vice versa

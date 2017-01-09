Thoughts: How does cheating change in a VR system? Since the player's headset
and controllers are connected to their body, how does that change what cheating
may look like? Using the proposed lockstep protocol would likely be too slow
while rendering at 90 frames per second.

- [Cheat-Proof Playout for Centralized and Distributed Online Games
  (2001)](http://citeseer.ist.psu.edu/viewdoc/download;jsessionid=9523AE244079D1A67E338D07D0D5613D?doi=10.1.1.29.2060&rep=rep1&type=pdf)

  - Cheating Under Dead Reckoning

    - Suppress-correct cheat (p. 3)

      - If only n packets can be dropped before the client is disconnected,
        drop n-1 out of n packets and send a fabricated n'th packet. Other
        players dead reckon your position, but cannot do so accurately. This
        form of cheating is difficult to detect.

    - Lookahead cheat (p. 4)

      - When using a stop-and-wait protocol where all players announce their
        actions before moving onto the next turn, the last player to announce
        can cheat since they know the moves of all other players.

    - Lockstep protocol (p. 4)

      - All players announce their moves as an encrypted packet, then they
        announce their moves in plain text. Other players can decrypt the text
        to check that no players are cheating.

      - Downside: Must send two packets for every one that was sent before.

    - Asynchronous Synchronization (p. 5)

      - Only send updates to clients which are in a player's sphere of
        influence. This is just an optimization of the Lockstep Protocol
        proposed previously.

      - Clients can move game time forward without synchronizing with peers if
        no interactions are possible. (p. 6)

      - If a player is 20 SOI (a unit introduced in this paper) away, they are
        synced with every 20 frames. (p. 7)

    - Secret Posessions (p. 7)

      - When a secret posession is obtained, a player composes a "promise" that
        it signs and shares with another player. At a predetermined time (e.g.
        the end of the game), the player must prove that it did in fact have
        that posession. This allows for cheating detection.

      - In a client-server architecture, this can be implemented with an
        "observer service" which keeps track of these promises and decrypts them
        in real time.



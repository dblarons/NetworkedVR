Thoughts: Building on this distributed system approach is more inline with my
project description and may be worth pursuing. I need to consider what, if any,
special cases are introduced by VR and look into those. Virtual Reality
environments are called out specifically on page 21. If I do use some of these
ideas, I should prioritize delta-encoding and state/execution partitioning over
area of interest filtering, as my demo will not be large enough to require any
filtering.

- [A Distributed Architecture for Interactive Multiplayer
Games (2005)](https://pdfs.semanticscholar.org/eea9/b20c80882d61255039d1c781ed3a8e219ee2.pdf)=

  - Single copy consistency model (abstract)

  - When a player performs a read or write, it is most likely to be located
    near that player (p. 1)

  - Distributed infrastructure: clients connect to the closest distributed
    server; alternative to client/server and p2p; "federated design" (p. 1)

  - Optimizations to decrease bandwidth cost (p. 3)

    - Area of interest filtering

      - Only send updates for objects within the defined area of interest

    - Delta-encoding

      - Instead of sending whole objects, send deltas between objects

  - State partitioning (p. 5)

    - Each object is identified by a GUID

    - Primary copy that exists on one node in the system

      - All updates must go to the primary node

    - Secondary copies that can exist on other nodes

  - Execution partitioning (p. 6)

    - Node executes think functions only for the primary copies that it owns

    - Node predicts which secondary copies must be made in order for think
      functions to execute on the primary node correctly and makes the copies

      - "Area of interest" of a primary object

      - Specified with range predicates over multiple object attributes

        - What does this entail?

  - State placement

    - Place primary copies that are within the "Area of interest" of each other
      on the same node to reduce the number of secondary copies that must be
      made

    - Sample object API

  - Replica Management (p. 11)

    - Primary copy forwards delta-encoded updates to all replicas

    - Secondary copies forward updates to the primary copy

  - Delete replicas if no updates arrive

  - Follow up:

    - Look into Akamai as a centrally administered, distributed server infrastructure

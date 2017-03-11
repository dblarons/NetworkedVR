Thoughts: Separating packet routing and zone of visibility calculations into
their own service (or library) allows the game server to be dramatically
simpler.

- [Matrix: Adaptive Middleware for Distributed Multiplayer Games (2005)](http://download.springer.com/static/pdf/566/chp%253A10.1007%252F11587552_20.pdf?originUrl=http%3A%2F%2Flink.springer.com%2Fchapter%2F10.1007%2F11587552_20&token2=exp=1483907019~acl=%2Fstatic%2Fpdf%2F566%2Fchp%25253A10.1007%25252F11587552_20.pdf%3ForiginUrl%3Dhttp%253A%252F%252Flink.springer.com%252Fchapter%252F10.1007%252F11587552_20*~hmac=a77ad03da305e55f04c8bb111f5c88e12169cd5dfc24cf47e21517bc4ba67567)

  - Introduction

    - "zone of visibility" for a player (p. 391)

      - Update players only with events that occur within their zone of
        visibility

  - Supports Game Requirements

    - Matrix uses "an O(1) route lookup mechanism to determine where to send
      packets" (p. 392)

  - Game Servers

    - Game servers must identify players by globally unique IDs

    - Game servers send their load to Matrix, which serves as a sort of behind
      the scenes load balancer. Rather than load balancing by sitting in
      between the client and the server, Matrix sits behind the server and
      monitors it, telling it when it must split part of its game world into
      a new server.

    - When a new server is needed, all game state is sent through Matrix to the
      new server

  - Matrix Servers

    - Matrix servers handle all packet routing, so game servers can be agnostic
      of other game servers

    - "Split left": Left piece of map is split off into a new server, after
      which it is considered a child of the original server

    - When a server is underutilized, it inspects its children and may reclaim
      them



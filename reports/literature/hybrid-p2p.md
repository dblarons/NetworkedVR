Thoughts: The idea of having a relatively static distributed hash table that
complements and is queried by a dynamic list of peers and objects which are
colocated is interesting. While only keeping track of local peers and objects
and having a single source of truth to query them is an important
simplification, it still seems quite complicated to be constantly managing the
lists of local peers/objects and recalculating AOI polygons. This paper solves
a problem that I can perhaps avoid if I do not consider MMORPGs. If I consider
only 4-8 player games, then one computer can reliably handle state for all of
the players, if only for a limited time.

- [VoroGame : A Hybrid P2P Architecture for Massively Multiplayer Games](https://www.researchgate.net/profile/Romain_Cavagna2/publication/224385913_VoroGame_A_Hybrid_P2P_Architecture_for_Massively_Multiplayer_Games/links/53f356c50cf2da87974469b2.pdf)
  - (page 2) How do we dynamically partition the game state while still only
    sending players updates about their AOI (area of interest)?
  - (page 2) Use a distributed hash table (DHT) to maintain game state amongst
    all peers and a Voronoi diagram for game world decomposition
  - (page 3) Map peers and objects randomly to a DHT address space
    - Don't worry about moving objects between peers as they move in the
      virtual world
    - Objects owned by a peer aren't necessarily colocated
  - (page 3) Each peer keeps a list of:
    - The objects inside it's region of responsibility
    - The peers that need to be informed about changes in those objects
    - For each object inside it's list, the peer tells the "DHT responsible"
      the list of peers to which that object is visible
    - When the peer moves, it updates its list of objects and peers which need
      to be udpated about those objects
  - (pages 4-5) Movement
    - When peers and objects move polygons must be recalculated and peer and
      object visibility lists must be updated

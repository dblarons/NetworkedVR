Thoughts:


- (p. 1) Introduction
  - Reshards data across machines as the number of servers and data changes
  - Data is stored in schematized semi-relational tables
  - Data is versioned and old versions can be retrieved
  - Old versions are garbage collected
  - Spanner can be configured by its user to control distance from clients,
    distance between replicas, etc.
  - Has externally consistent reads and writes
  - Has globally consistent reads across the database at a timestamp
  - TrueTime API allows for consistent timestamps across different regions
- (p. 2) 2. Implementation
  - Zones are the set of locations across which data can be replicated
  - One 'zonemaster' has between 100 and 1000s of 'spanservers'
  - 'placement driver' finds data that needs to be moved
  - 'zonemaster' assigns data to 'spanservers'; 'spanservers' server data to
  clients
- (p. 2) 2.1 Spanserver Software Stack
  - 'spanserver' has between 100 and 1000 tablets
    - tablets have the signature (key:string, timestamp:int64) -> string
    - There is one Paxos state machine on top of each tablet
    - Paxos state machines are used to consistently replicate the mappings
      - Reads only occur from machines that are up to date
- (p. 3) 2.2 Directories and Placement
  - Directory: set of contiguous keys that share a common prefix
  - Directories are the unit of data movement between Paxos groups
  - Directories that are frequently accessed together can be colocated together
  - 'Movedir' is a background task that only registers a transaction once it
  moves the last piece
  - Users can specify locations at a directory level
- (p. 4) 2.3 Data Model
  - Spanner is closer to a relational database than a key value store like BigTable
- (p. 5) 3 TrueTime
  - TrueTime has bounded time uncertainty
  - 'time master' machines per datacenter, and 'timeslave daemons' per machine
  - Masters either have GPS antennas or atomic clocks
  - Masters evict themselves if they are diverging from the other clocks
- (p. 6) 4 Concurrency Control
- (p. 6) 4.1 Timestamp Management
  - Supports read-write transactions, read-only transactions, and snapshot reads
  - Reads are non-blocking
- (p. 8) 4.2.1 Read-Write Transactions
  - Writes are buffered in a transactions so that reads during the write are not corrupted

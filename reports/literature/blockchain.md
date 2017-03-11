Thoughts:

[Bitcoin and Beyond: A Technical Survey on Decentralized Digital Currencies](http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.738.1406&rep=rep1&type=pdf)
- Introduction
  - Quorom systems were susceptible to the Sybil attack where many peers inject false information
  - (p. 2) 2.1 The Starting Point
    - Transaction (TX): Signed contract verifiable with a public key
    - Coins need serial numbers to be uniquely identifiable
  - (p. 3) 2.2 Proof of Work
    - Block chain is the distributed ledger
    - Bitcoin uses proof of work to solve for double spending and Sybil attacks
    - The ability to verify depends on computing power rather than number of peers
      - Harder to fake computing power than number of peers
  - (p. 4) 2.3 Blockchain
    - Blocks keep a hash of the previously validated block (i.e. a pointer to it)
    - The block proves that a transaction must have existed at that time since it is included in the block
    - Forks
      - Mining is continued on the longest locally known fork (highest amt of computational effort thus far
      - If one user maintains over 50% of the compute power, they can "catch up" a forged fork with a real fork and thereby double spend
      - Transactions are commonly considered steady after six consecutive block verifications
  - (p. 5) 2.4 Transactions
    - Each output of a transaction can be considered spent or unspent
    - New transactions reference previous transactions that have unspent outputs
    - A person validates that they can spend the result of an output by showing their signature (generated with private/public keypair)
    - Does not need a bank to issue serial numbers because transaction hashes take their place
 

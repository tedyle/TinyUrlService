# TinyUrlService

Two approaches for size limited caching:
1. MemoryLimitMegabytes (For example: 10 Megabytes)
  Advantages:
    a.  You are in charge of the cache size
    
  Disadvantages:
    a. May not have enough memory on your machine
    b. May have unused memory on your machine when it's needed by your cache
    
2. PhysicalMemoryLimitPercentage (For example: 20)
  Advantages:
    a. Cache is in sync with machine's memory
    
  Disadvantages:
    a. You are not in charge of your cache size
    b. May cause performance issues. Other apps consume memory on your machine. 
    

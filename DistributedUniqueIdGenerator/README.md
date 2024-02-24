<h1 align="center">Unique ID Generator System</h1>
<br/>

## Problem Statement
We need a distributed Unique ID Generator to assign distinct identifiers to various events. The API should generate 100 million unique IDs daily.

## Background
In a distributed environment, databases scale horizontally, partitioning the same table into shards across regions. This setup acts as a unified database to external systems. We require a distributed sequence generator to provide unique IDs for each API request, even concurrently.

## Solution Overview
- Use a central range handler microservice to allocate ranges to servers, eliminating ID duplication.
- Servers claim and increment their assigned range locally, enabling concurrent request handling.
- A load balancer further distributes the workload across app servers.

## High-level Design
![image](https://github.com/dawoodhussain/High-level-System-Design/assets/33810029/dbb0360f-8e72-4e7f-ab19-855d3ac7c138)

### Clients
- Developed a console app as clients to request unique IDs from the load balancer concurrently.
- Configurable simultaneous requests.
- Identifies and logs duplicate IDs using a concurrent Dictionary.
- [Here is the code](https://github.com/dawoodhussain/High-level-System-Design/tree/main/Utils/ConcurrentRequestsTest)

### Load Balancer
- Built an ASP.NET Core Web API using YARP for load balancing with Round Robin algorithm.
- Includes Swagger and HealthChecks.
- Distributes traffic among configured app servers.
- Rate limiting implemented for 429 responses.
- Server URLs and balancing algorithms configurable in appsettings.json.
- [Here is the code](https://github.com/dawoodhussain/High-level-System-Design/tree/main/DistributedUniqueIdGenerator/GenerateSequenceApiLB)

### App Servers
- Created a minimal API acting as APP servers.
- On initial request from load balancer, fetches available range from range handler service.
- Responds to requests by incrementing IDs within range until exhausted, then requests a new range.
- Project duplicated four times for load balancing across servers.
- [Here is the code](https://github.com/dawoodhussain/High-level-System-Design/tree/main/DistributedUniqueIdGenerator/AppServers)

### RangeHandlerService
- Developed a centralized minimal API.
- Initializes and loads ranges into a ConcurrentQueue at project start.
- App servers dequeue from queue for ranges.
- When empty, reloads queue from next available numbers.
- Configurable UpperRangeLimit & RangeSplit in appsettings.json.
- [Here is the code](https://github.com/dawoodhussain/High-level-System-Design/tree/main/DistributedUniqueIdGenerator/RangeHandlerService)

## Tech Stack
- C# console application
- Asp.net core Web API
- Minimal APIs
- YARP
- HealthCheck
- Swagger
- CORS policy
- RateLimiter
- ConcurrentQueue
- ConcurrentDictionary

## Next Enhancements
1. Modify the logic to have Casualty for the unique id generated.
2. Utilize Docker compose to replicate app servers instead of duplicating code.

# CoAPWorks Samples
This repository contains a collection of SDKs , samples and guides to work with [CoAPWorks](https://coapworks.com) APIs.

## Idaax.CoAPWorks.CoAPAPI.NET.Sample
This sample showcases how we can use the CoAPSharp library (.NET Standard) and communicate with the CoAP APIs exposed by CoAPWorks.

This package provides wrapper classes for a field deployed device to perform the following:

1. Get current UTC date and time
2. Get pre-configured location time (You setup GPS co-ordinates of the machine in CoAPWorks portal)
3. Get location time based on longitude and latitude
4. Get machine settings (The configuration items for your field deployed machines that are maintained at the server side)
5. Send email alert from the machine
6. Send a machine heartbeat
7. Send machine feeds for storage

## Idaax.CoAPWorks.WebAPI.NET.Sample
This sample shows how you can use the CoAPWorks HTTP RESTful Web APIs to automate most of CoAPWorks machine management tasks.
Before you can run these examples, you should create an HTTP Web Client instance on CoAPWorks website. 

The examples cover the following:
1. Create a new machine
2. Update a machine details
3. Check if a machine exists
4. Get machine heartbeats
5. Create machine channel
6. Update machine channel
7. Update machine channel custom JavaScript
8. Get the custom Javascript associated with the channel
9. Get the channel feeds
10. Update machine settings
11. Read the machine settings
12. Delete machine settings
13. Delete a machine channel
14. Delete a machine

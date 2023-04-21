# CoAPWorks Samples
This repository contains a collection of SDKs , samples and guides to work with [CoAPWorks](https://coapworks.com) APIs.

## Idaax.CoAPWorks.CoAPAPI.Client
This is the source code for the CoAP API client library. This creates a nuget package for .NET based projects. The Nuget package can be found [here](https://www.nuget.org/packages/Idaax.CoAPWorks.CoAPAPI.Client)
This package provides wrapper classes for a field deployed device to perform the following:

1. Get current UTC date and time
2. Get pre-configured location time (You setup GPS co-ordinates of the machine in CoAPWorks portal)
3. Get location time based on longitude and latitude
4. Get machine settings (The configuration items for your field deployed machines that are maintained at the server side)
5. Send email alert from the machine
6. Send a machine heartbeat
7. Send machine feeds for storage

## Idaax.CoAPWorks.CoAPAPI.Client.Sample
This sample project shows how to use the [Idaax.CoAPWorks.CoAPAPI.Client Nuget package](https://www.nuget.org/packages/Idaax.CoAPWorks.CoAPAPI.Client)

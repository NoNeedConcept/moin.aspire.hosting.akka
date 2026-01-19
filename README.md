# moin.aspire.hosting.akka

> .NET Aspire Hosting Integration for Akka.NET - Because distributed actor systems deserve proper orchestration

[![NuGet](https://img.shields.io/nuget/v/moin.aspire.hosting.akka.svg)](https://www.nuget.org/packages/moin.aspire.hosting.akka/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/NoNeedConcept/moin.aspire.hosting.akka/build-and-release.yml?branch=main)](https://github.com/NoNeedConcept/moin.aspire.hosting.akka/actions)
[![License](https://img.shields.io/github/license/NoNeedConcept/moin.aspire.hosting.akka)](LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/moin.aspire.hosting.akka.svg)](https://www.nuget.org/packages/moin.aspire.hosting.akka/)

<p align="center">
     <img width="128" height="128" src="docs/logo/logo.png" alt="logo">
</p>

## Overview

**moin.aspire.hosting.akka** bridges the gap between .NET Aspire and Akka.NET. If you want to build modern cloud-native applications with the Actor Model but don't want to sacrifice Aspire's orchestration, service discovery, and developer experience - this integration is for you.

This library provides seamless integration of Akka.NET into the .NET Aspire hosting model with built-in support for Lighthouse-based cluster discovery, making it as simple as possible to define and orchestrate Akka.NET-based microservices in your Aspire AppHost.

## Why moin.aspire.hosting.akka?

* **Aspire-First Design**: Seamless integration into the Aspire ecosystem
* **Lighthouse Support**: Built-in Lighthouse nodes for automatic cluster discovery
* **Type-Safe Configuration**: Strongly-typed APIs for Akka.NET cluster configuration
* **Service Discovery**: Automatic integration with Aspire service discovery
* **Developer Experience**: Debug multi-node Akka.NET clusters locally without complex setups
* **Production Ready**: From development to deployment - consistent and reliable
* **Observability**: Full integration with Aspire Dashboard for monitoring and tracing

## Prerequisites

Before you can use moin.aspire.hosting.akka, you need:

* **.NET 8.0 SDK** or higher
* **.NET Aspire Workload** installed
* **Docker** for running containerized Akka.NET nodes
* **Akka.NET** knowledge (recommended)

## ⚠️ Deployment Model Limitation

> **Important:** `moin.aspire.hosting.akka` currently supports **only homogeneous deployment models**.

This means:

- ✅ **All Akka nodes must be Docker-based** (`AddDockerfile`)

  **or**

- ✅ **All Akka nodes must be Project-based** (`AddProject`)

- ❌ **Mixing Docker containers and project references within the same Akka cluster is not supported**

### Why?

.NET Aspire treats Docker resources and project resources differently in terms of:

- networking
- service discovery
- endpoint resolution
- lifecycle management

Akka.NET clustering (especially Lighthouse-based discovery) requires **consistent addressing and networking semantics across all nodes**.  
A mixed environment would result in **invalid seed node addresses** and **unstable cluster formation**.

> Support for mixed environments may be evaluated in the future, but is currently **out of scope**.


### Install .NET Aspire Workload

```bash
dotnet workload update
dotnet workload install aspire
```

### Create New Aspire Project (Optional)

```bash
dotnet new aspire-starter -n MyAkkaApp
cd MyAkkaApp
```

## Installation

### Package Manager

```powershell
Install-Package moin.aspire.hosting.akka
```

### .NET CLI

```bash
dotnet add package moin.aspire.hosting.akka
```

### PackageReference

```xml
<PackageReference Include="moin.aspire.hosting.akka" Version="1.0.0" />
```

## Quick Start

Here's the complete example from the repository showing how to set up a multi-node Akka.NET cluster with Lighthouse in your Aspire AppHost:

### 1. AppHost Configuration

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Define first Akka node
var nodeOne = builder
    .AddDockerfile("AkkaNodeOne", "../../", "example/akka.node.one/Dockerfile")
    .WithContainerName("akka.node.one")
    .WithImageRegistry("localhost")
    .WithImageTag("dev")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithOtlpExporter()
    .WithHttpEndpoint(targetPort: 8080)
    .WithHttpHealthCheck(path: "/health")
    .WithHttpHealthCheck(path: "/alive")
    .WithHttpHealthCheck(path: "/started");

// Define second Akka node
var nodeTwo = builder
    .AddDockerfile("AkkaNodeTwo", "../../", "example/akka.node.two/Dockerfile")
    .WithContainerName("akka.node.two")
    .WithImageRegistry("localhost")
    .WithImageTag("dev")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithOtlpExporter()
    .WithHttpEndpoint(targetPort: 8080)
    .WithHttpHealthCheck(path: "/health")
    .WithHttpHealthCheck(path: "/alive")
    .WithHttpHealthCheck(path: "/started");

// Configure Akka cluster with Lighthouse
builder.AddAkka("akka", "testing", akkaConfigure: akkaBuilder =>
{
    akkaBuilder
        .WithLighthouse(3)  // 3 Lighthouse seed nodes for cluster discovery
        .WithNode(targetPort: 8000, resource: nodeOne, endpointName: "akka")
        .WithNode(targetPort: 8001, resource: nodeTwo, endpointName: "akka");
});

await builder.Build().RunAsync();
```

## Usage Examples

### Basic Cluster Setup

The simplest way to create an Akka.NET cluster with automatic discovery:

```csharp
builder.AddAkka("my-cluster", "MyActorSystem", akkaConfigure: akkaBuilder =>
{
    akkaBuilder.WithLighthouse(3);  // 3 Lighthouse nodes for HA
});
```

## Features

moin.aspire.hosting.akka provides comprehensive support for the Akka.NET ecosystem:

* ✅ **Lighthouse Integration** - Automatic cluster discovery with Lighthouse nodes
* ✅ **Multi-Node Clusters** - Easy configuration of complex cluster topologies
* ✅ **Docker Support** - Native support for Dockerfile-based Akka services
* ✅ **Service Discovery** - Automatic Aspire service discovery integration
* ✅ **Health Checks** - Built-in health check support for all nodes
* ✅ **Observability** - Full integration with Aspire Dashboard (metrics, traces, logs)
* ✅ **Type-Safe Configuration** - Strongly-typed builder API
* ✅ **Environment Management** - Seamless environment variable injection
* ✅ **OTLP Export** - OpenTelemetry integration for monitoring
* ✅ **Development Mode** - Easy local multi-node cluster testing

## Architecture

moin.aspire.hosting.akka follows the Aspire hosting model and integrates seamlessly:

```
┌─────────────────────────────┐
│   Aspire AppHost            │
│   (Program.cs)              │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│  moin.aspire.hosting.akka   │  ← This library
│  - AddAkka extension        │
│  - Lighthouse management    │
│  - Node configuration       │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│  Lighthouse Seed Nodes      │
│  (Automatic Discovery)      │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│  Akka.NET Worker Nodes      │
│  - Docker containers        │
│  - Actor systems            │
│  - Cluster members          │
└─────────────────────────────┘
```

## Lighthouse Configuration

Lighthouse is a discovery service for Akka.NET clusters. moin.aspire.hosting.akka automatically provisions Lighthouse nodes for you:

### How It Works

1. **Lighthouse Nodes**: The library automatically creates the specified number of Lighthouse seed nodes
2. **Service Discovery**: Worker nodes automatically discover Lighthouse nodes via Aspire service discovery
3. **Cluster Formation**: Nodes join the cluster by connecting to any available Lighthouse node
4. **High Availability**: Multiple Lighthouse nodes ensure no single point of failure

### Recommended Configuration

```csharp
// Development: 1-2 Lighthouse nodes
akkaBuilder.WithLighthouse(1);  // Single node for local dev

// Production: 3+ Lighthouse nodes for HA
akkaBuilder.WithLighthouse(3);  // Recommended for production

// High-scale: 5+ Lighthouse nodes
akkaBuilder.WithLighthouse(5);  // For very large clusters
```
x
## Roadmap

Planned features for upcoming releases:

* Native support for Akka.Management
* Built-in split-brain resolver configuration
* Automatic sharding setup
* Performance profiling integration
* Cluster visualization in Aspire Dashboard
* Support for multiple Akka.NET versions
* Advanced routing strategies
* Metrics aggregation across nodes

## Contributing

Contributions are welcome! This library grows with the community's needs.

### How to Contribute

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Write tests for your changes
4. Ensure all tests pass: `dotnet test`
5. Submit a Pull Request

### Contribution Guidelines

* Follow existing code style and conventions
* Include unit tests for new features
* Update documentation for API changes
* Keep changes focused and atomic
* Add examples for new features

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Built with ❤️ for the .NET Aspire and Akka.NET communities**

For questions, support, or feature requests, please [open an issue](https://github.com/NoNeedConcept/moin.aspire.hosting.akka/issues).

### Related Projects

* [.NET Aspire](https://github.com/dotnet/aspire) - The cloud-native development framework
* [Akka.NET](https://github.com/akkadotnet/akka.net) - Actor model for .NET
* [Akka.Hosting](https://github.com/akkadotnet/Akka.Hosting) - Hosting infrastructure for Akka.NET
* [Lighthouse](https://github.com/petabridge/lighthouse) - Service discovery for Akka.NET clusters
* [Petabridge Akka.NET Samples](https://github.com/petabridge/akkadotnet-code-samples) - Code samples for Akka.NET
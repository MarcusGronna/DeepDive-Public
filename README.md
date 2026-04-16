# DeepDiveInfra

A self-hosted, cloud-native style learning project focused on the delivery chain around a deliberately simple backend application.

The goal is not the app itself — it is understanding the full path from source code to a running workload:
**code → test → build → Docker image → local Kubernetes → deployment and observability.**

---

## What this project is

This is a learning-focused infrastructure sprint. A small ASP.NET Core Web API serves as the deployable artifact, but the real work is in everything that surrounds it: CI/CD, containerization, Linux tooling, Kubernetes, and eventually observability.

This is **not** a managed cloud project. There is no Azure, no AWS, no Terraform. It is a local, self-hosted lab case designed to build practical understanding of how software gets from a repository to a running container in a cluster.

## Why the app is simple on purpose

The backend is intentionally minimal so that the focus stays on infrastructure and delivery, not business logic. The API manages a single domain entity (Mission) with three endpoints. That is enough to have something real to build, test, containerize, and deploy — without the app itself becoming the point.

## What has been built so far

**Application:**
- ASP.NET Core Web API (.NET 10) with minimal API style
- SQLite database via Entity Framework Core
- Automatic database migrations on startup
- One domain entity: `Mission` (Id, Title, Region, ThreatLevel, CreatedAt)
- Endpoints: `GET /health`, `GET /missions`, `POST /missions`
- OpenAPI spec with Scalar API reference (development mode)

**Testing:**
- xUnit integration test project
- `WebApplicationFactory`-based test for `GET /health`

**CI/CD:**
- GitHub Actions workflow (`ci.yml`) running on `ubuntu-latest`
- Pipeline steps: checkout → .NET setup → restore → build → test
- Triggers on push and pull request

**Containerization:**
- Multi-stage Dockerfile (SDK build → ASP.NET runtime)
- App exposed on port 8080
- `.dockerignore` configured to keep the image clean

**Local Kubernetes (planned/in progress):**
- kind as the local cluster provider (runs Kubernetes nodes as Docker containers)
- kubectl for cluster interaction
- WSL Ubuntu as the Linux-oriented working environment

## How the environment is structured

This project runs across several layers on a Windows host:

| Layer | Role |
|---|---|
| **Windows host** | Development machine — IDE, Git, browser |
| **WSL (Ubuntu)** | Linux working environment for Docker, kubectl, kind, and shell-based tooling |
| **Docker Desktop** | Container runtime, shared between Windows and WSL |
| **kind** | Local Kubernetes cluster — runs cluster nodes as Docker containers inside Docker Desktop |
| **kubectl** | CLI tool used from WSL to interact with the kind cluster |

kind uses Docker containers to simulate Kubernetes nodes. This means the full stack — from building an image to deploying it into a cluster — runs locally without any cloud accounts or remote infrastructure.

WSL provides the Linux environment where most infrastructure tooling lives. Docker Desktop bridges the container runtime between Windows and WSL so that both sides can work with the same Docker daemon.

## What this setup means conceptually

This is not "just an app." It is a delivery-chain project. The interesting part is the path a change takes:

1. Code is written and pushed to GitHub
2. GitHub Actions restores, builds, and tests automatically
3. A Docker image is built from the published output
4. The image is loaded into a local Kubernetes cluster (kind)
5. Kubernetes runs the workload as a pod
6. (Planned) Prometheus scrapes metrics; Grafana visualizes them

Each layer introduces its own concerns, failure modes, and configuration surface. Understanding what happens _between_ layers is the core learning objective.

## What I am learning through this

- The difference between a development environment, a build/test environment, a container runtime, a deploy target, and a runtime environment
- How containers work as deployable artifacts — not just a way to run things locally
- How Kubernetes fits into the delivery flow and what it actually manages
- How infrastructure problems often happen between layers (networking, volumes, permissions, configuration), not only inside application code
- How CI/CD pipelines connect the development workflow to the deployment target
- How GitHub Actions works, including the difference between GitHub-hosted and self-hosted runners

## Advantages and trade-offs

**Advantages:**
- Realistic scope without cloud cost or account complexity
- Strong learning value — every layer is visible and debuggable
- Local, cheap, and fully repeatable
- Practical Linux, Docker, and Kubernetes experience on a Windows machine

**Trade-offs and limitations:**
- This is not production infrastructure — it is a learning lab
- Extra complexity from the Windows + WSL + Docker Desktop + kind stack
- kind is excellent for learning but does not replicate real cloud Kubernetes behavior (no load balancers, no cloud networking, no persistent volume providers)
- SQLite is chosen for simplicity — it is not ideal for containerized or clustered workloads where a network-accessible database would be more appropriate
- No TLS, authentication, or security hardening

## How to talk about this project

> This is a self-hosted, local cloud-native style learning case. The focus is on the delivery chain — CI/CD, Docker, Kubernetes, Linux tooling, and observability — built around a deliberately simple ASP.NET Core backend. The environment runs on WSL and kind, not on managed cloud services. The goal is to understand how software moves from source code to a running workload in a cluster, and where things break between layers.

Use this framing in a portfolio or interview setting. Be clear that it is a learning project, not a production platform. The value is in the infrastructure understanding, not the application complexity.

## Current progress

- [x] ASP.NET Core Web API with SQLite and EF Core
- [x] Domain model and startup migrations
- [x] Health, GET, and POST endpoints
- [x] xUnit integration test for `/health`
- [x] GitHub Actions CI pipeline (restore → build → test)
- [x] Multi-stage Dockerfile
- [ ] Container image built and tested locally
- [ ] kind cluster provisioned
- [ ] Kubernetes manifests (Deployment, Service)
- [ ] App deployed and reachable in the cluster
- [ ] Prometheus and Grafana observability
- [ ] Self-hosted runner integration

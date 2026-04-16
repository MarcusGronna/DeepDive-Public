# DeepDive — Local Cloud-Native Delivery Lab

A self-hosted, cloud-native style portfolio project focused on the full delivery chain around a deliberately simple ASP.NET Core backend: **CI/CD, Docker, Kubernetes, and observability — all running locally.**

> The application is intentionally minimal. The real work is everything that surrounds it: automated testing, containerization, local cluster deployment, Prometheus metrics, and Grafana dashboards.

---

## Why This Project Exists

This project exists to demonstrate practical, hands-on understanding of how software moves from source code to a running, monitored workload — without relying on managed cloud services.

It is a **local infrastructure lab**, not a production system. There is no Azure, no AWS, no Terraform. The entire stack — from CI to container to cluster to monitoring — runs on a local machine using open-source tooling. The goal is to make the delivery chain visible, debuggable, and fully reproducible.

---

## What This Project Demonstrates

- Building and testing an application through **GitHub Actions CI**
- Packaging the application as a **multi-stage Docker image**
- Deploying to a **local Kubernetes cluster** using kind
- Exposing **Prometheus metrics** from the application at runtime
- Collecting and visualizing metrics with **Prometheus and Grafana**
- Scripting the full **build → load → deploy** flow for local iteration
- Structuring a project so that infrastructure and delivery concerns are first-class

---

## Tech Stack

| Layer            | Technology                                      |
|------------------|------------------------------------------------|
| **Backend**      | ASP.NET Core (.NET 10), minimal API style       |
| **Database**     | SQLite via Entity Framework Core                |
| **Metrics**      | prometheus-net (HTTP metrics + custom counters)  |
| **API Docs**     | OpenAPI + Scalar (development mode)             |
| **Testing**      | xUnit, WebApplicationFactory                    |
| **CI**           | GitHub Actions (`ci.yml`)                       |
| **Container**    | Docker, multi-stage build                       |
| **Orchestration**| Kubernetes (kind — local cluster)               |
| **Monitoring**   | Prometheus + Grafana (deployed in-cluster)       |

---

## Architecture / Delivery Flow

```
Code → GitHub → GitHub Actions CI → Docker build → kind load → Kubernetes deployment → Prometheus scraping → Grafana dashboards
```

**Step by step:**

1. Code is pushed to GitHub
2. GitHub Actions runs restore, build, and test automatically
3. A Docker image is built locally from the published output
4. The image is loaded into a local kind Kubernetes cluster
5. Kubernetes runs the workload as a pod behind a ClusterIP Service
6. Prometheus scrapes the `/metrics` endpoint exposed by the application
7. Grafana visualizes the collected metrics

---

## Repository Structure

```
.
├── .github/workflows/
│   └── ci.yml                  # GitHub Actions CI pipeline
├── InfraSprint.Api/
│   ├── Program.cs              # Application entry point and endpoint definitions
│   ├── Models/Mission.cs       # Domain entity
│   ├── Data/AppDbContext.cs     # EF Core database context
│   ├── Migrations/             # Auto-generated EF Core migrations
│   └── InfraSprint.Api.csproj  # Project file with dependencies
├── InfraSprint.Tests/
│   ├── HealthEndpointTests.cs  # Integration test for /health
│   └── InfraSprint.Tests.csproj
├── k8s/
│   └── app.yaml                # Kubernetes Deployment + Service manifest
├── scripts/
│   └── deploy-local.sh         # Build, load, and deploy script for kind
├── Dockerfile                  # Multi-stage Docker build
├── .dockerignore
├── InfraSprint.slnx            # Solution file
└── README.md
```

---

## How to Run Locally

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run the API

```bash
dotnet restore
dotnet build
dotnet run --project InfraSprint.Api
```

The API starts on `http://localhost:8080` (or the configured port) with these endpoints:

| Method | Path        | Description                          |
|--------|-------------|--------------------------------------|
| GET    | `/health`   | Health check                         |
| GET    | `/missions` | List all missions (newest first)     |
| POST   | `/missions` | Create a new mission                 |
| GET    | `/metrics`  | Prometheus metrics endpoint          |

### Run Tests

```bash
dotnet test
```

---

## Run with Docker

### Build the image

```bash
docker build -t infrasprint-api:dev .
```

### Run the container

```bash
docker run -p 8080:8080 infrasprint-api:dev
```

The Dockerfile uses a multi-stage build: the .NET SDK compiles and publishes the application, and the final image runs on the lightweight ASP.NET runtime base.

---

## Deploy to Local Kubernetes

### Prerequisites

- [Docker](https://www.docker.com/)
- [kind](https://kind.sigs.k8s.io/)
- [kubectl](https://kubernetes.io/docs/tasks/tools/)

### Create a kind cluster (one-time)

```bash
kind create cluster --name infrasprint
```

### Deploy the application

The included script handles the full flow — build, load, apply, and restart:

```bash
./scripts/deploy-local.sh
```

This script:
1. Builds the Docker image (`infrasprint-api:dev`)
2. Loads it into the kind cluster
3. Applies the Kubernetes manifest (`k8s/app.yaml`)
4. Restarts the deployment to pick up the new image

### Verify the deployment

```bash
kubectl get pods -A
```

<img src="https://github.com/user-attachments/assets/33339cf3-cdd8-4fd7-8fb6-9da4b90a9f9a" alt="kubectl get pods -A showing all running pods across namespaces, including infrasprint-api in default and Prometheus/Grafana in monitoring" />

*All pods running in the local kind cluster — the `infrasprint-api` pod in the default namespace, core Kubernetes components in kube-system, and Prometheus + Grafana in the monitoring namespace.*

### Access the application from within the cluster

```bash
kubectl port-forward svc/infrasprint-api 8080:8080
curl http://localhost:8080/health
```

---

## CI Overview

GitHub Actions runs on every push and pull request to `main`.

**Pipeline steps (`ci.yml`):**

1. Checkout repository
2. Setup .NET 10 SDK
3. Restore dependencies
4. Build in Release configuration
5. Run tests

The pipeline validates that the application compiles and all tests pass before any manual deployment step.

<img src="https://github.com/user-attachments/assets/4f399a89-4bf7-47a1-a5bf-2ff8b9bf3fb7" alt="GitHub Actions CI/CD pipeline showing successful build-and-test and deploy steps" />

*A successful CI/CD run with build-and-test and deploy stages completing in under a minute.*

---

## Observability / Monitoring

The application exposes Prometheus-compatible metrics at `/metrics` using the `prometheus-net.AspNetCore` library.

**What is instrumented:**

- **HTTP request metrics** — automatically collected via `UseHttpMetrics()` (request duration, status codes, methods)
- **Custom counter** — `missions_created_total` increments every time a new mission is created via `POST /missions`

**In-cluster monitoring stack:**

The local Kubernetes cluster runs Prometheus and Grafana in the `monitoring` namespace. Prometheus is configured to scrape the application's `/metrics` endpoint via service annotations:

```yaml
annotations:
  prometheus.io/scrape: "true"
  prometheus.io/path: "/metrics"
  prometheus.io/port: "8080"
```

<img src="https://github.com/user-attachments/assets/6072deef-252b-4eb4-a71a-35ed259e5f87" alt="Grafana dashboard showing HTTP request metrics and application monitoring data" />

*Grafana dashboard visualizing metrics scraped by Prometheus from the application's `/metrics` endpoint.*

---

## Screenshots

<details>
<summary>CI/CD pipeline — build, test, and deploy</summary>

<img src="https://github.com/user-attachments/assets/4f399a89-4bf7-47a1-a5bf-2ff8b9bf3fb7" alt="GitHub Actions CI/CD pipeline" />

Shows a successful GitHub Actions workflow run with build-and-test and deploy stages completing on push to main.

</details>

<details>
<summary>Observability — Grafana dashboard</summary>

<img src="https://github.com/user-attachments/assets/6072deef-252b-4eb4-a71a-35ed259e5f87" alt="Grafana monitoring dashboard" />

Shows the Grafana dashboard visualizing application metrics collected by Prometheus from the running workload inside the local Kubernetes cluster.

</details>

<details>
<summary>Kubernetes cluster — all pods running</summary>

<img src="https://github.com/user-attachments/assets/33339cf3-cdd8-4fd7-8fb6-9da4b90a9f9a" alt="kubectl get pods -A" />

Shows the infrasprint-api deployment alongside the full monitoring stack (Prometheus server, Grafana, alertmanager, kube-state-metrics, node-exporter, pushgateway).

</details>

<details>
<summary>Application running — endpoint response</summary>

<img src="https://github.com/user-attachments/assets/0130e113-2a95-4351-90d6-e0ea94da7b6b" alt="Application endpoint response" />

Shows the application responding to requests, demonstrating the API running inside the local Kubernetes cluster.

</details>

<details>
<summary>Project overview</summary>

<img src="https://github.com/user-attachments/assets/4029850e-53fc-4293-a02c-05c6ff785ab6" alt="Project screenshot" />

Additional project screenshot showing the infrastructure and deployment setup.

</details>

---

## Key Learning Outcomes

- **Delivery chain thinking** — understanding each stage from source to running workload and where failures happen between layers
- **Containerization** — building production-style multi-stage Docker images, not just running apps in containers
- **Kubernetes fundamentals** — Deployments, Services, pod lifecycle, image loading with kind, and rolling updates
- **CI/CD** — structuring a GitHub Actions pipeline that gates builds and tests on every change
- **Observability** — instrumenting an application with Prometheus metrics and building a monitoring stack inside a local cluster
- **Linux tooling** — working with Docker, kubectl, and shell scripting in a WSL-based environment

---

## Limitations

- **Not a production system** — this is a local learning lab with no TLS, authentication, or security hardening
- **SQLite** is used for simplicity; it is not suitable for clustered or multi-replica workloads
- **kind** does not replicate real cloud Kubernetes behavior (no load balancers, no cloud networking, no managed persistent volumes)
- **No automated image push** — the CI pipeline builds and tests the application, but the Docker image is built and loaded into kind locally
- **Single replica** — the deployment runs one pod; scaling and high availability are not addressed

---

## Next Improvements

- Add Docker image build step to the CI pipeline
- Introduce Helm charts or Kustomize for manifest management
- Add structured logging and log aggregation
- Expand test coverage beyond the health endpoint
- Explore network policies and resource limits in Kubernetes
- Investigate self-hosted GitHub Actions runners for local CI/CD loops

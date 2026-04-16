# DeepDive — Cloud-Native Delivery Portfolio Project

> **This is a public portfolio / demo version of an infrastructure-focused learning project.** The complete operational setup — including the self-hosted runner configuration, full CI/CD deployment pipeline, and monitoring stack provisioning — was developed and maintained in a private repository. This public repo is intended to document and showcase the project, not to expose the full private deployment implementation.

---

## Project Purpose

The goal of this project was to learn and demonstrate cloud-native style delivery concepts — CI/CD pipelines, containerization, Kubernetes-based deployment, and observability — using a self-hosted, local setup built entirely with open-source tooling.

The backend application is **intentionally simple**. It exists as a deployment target so the focus stays on the infrastructure and delivery chain surrounding it: automated testing, Docker image builds, Kubernetes manifests, Prometheus metrics, and Grafana dashboards.

This is a learning lab, not a production system. There is no Azure, no AWS, no Terraform. The entire stack ran locally on a single machine.

---

## What This Project Demonstrates

- **GitHub Actions CI/CD** — automated build, test, and deployment pipelines triggered on push and pull request
- **Docker containerization** — multi-stage image builds producing lightweight runtime containers
- **Kubernetes-based deployment** — running workloads in a local kind cluster with Deployments and Services
- **Observability** — application-level Prometheus metrics scraped in-cluster and visualized through Grafana dashboards
- **Delivery flow understanding** — structuring a project so that every stage from source code to monitored workload is visible and intentional

---

## Public Repo vs Private Implementation

This repository is a **showcase / demo repo**. It contains the application source code, Dockerfile, a basic Kubernetes manifest, a local deploy helper script, and a CI workflow that runs build and test steps.

The **full operational setup** was developed in a separate private repository. That private repo included:

- The actual self-hosted GitHub Actions runner configuration
- The complete CI/CD pipeline with build, test, **and deploy** stages
- Prometheus and Grafana provisioning and configuration within the cluster
- Operational details specific to the local infrastructure environment

These components are not included in this public repository because they contain environment-specific configuration that should not be exposed publicly. The screenshots in this README were captured from the working private setup and are included here as project evidence.

---

## Tech Stack

| Layer            | Technology                                      |
|------------------|------------------------------------------------|
| **Backend**      | ASP.NET Core (.NET 10), minimal API style       |
| **Database**     | SQLite via Entity Framework Core                |
| **Metrics**      | prometheus-net (HTTP metrics + custom counters)  |
| **API Docs**     | OpenAPI + Scalar (development mode)             |
| **Testing**      | xUnit, WebApplicationFactory                    |
| **CI**           | GitHub Actions                                  |
| **Container**    | Docker, multi-stage build                       |
| **Orchestration**| Kubernetes (kind — local cluster)               |
| **Monitoring**   | Prometheus + Grafana (deployed in-cluster)       |

---

## Architecture / Delivery Flow

The full delivery flow, as it ran in the private setup, followed this path:

```
Code push → GitHub → GitHub Actions CI/CD → Docker build → kind load → Kubernetes deployment → Prometheus scraping → Grafana dashboards
```

**Conceptual stages:**

1. Code is pushed to GitHub
2. GitHub Actions runs restore, build, and test steps automatically
3. On success, the pipeline builds a Docker image and loads it into the local kind cluster
4. Kubernetes runs the workload as a pod behind a ClusterIP Service
5. Prometheus scrapes the `/metrics` endpoint exposed by the application
6. Grafana visualizes the collected metrics on a dashboard

> **Note:** The CI workflow included in this public repo (`ci.yml`) covers the build-and-test stage. The deploy stage, self-hosted runner integration, and monitoring stack setup were part of the private repository.

---

## Repository Structure

The files in this public repo represent the application and a subset of the infrastructure configuration:

```
.
├── .github/workflows/
│   └── ci.yml                  # GitHub Actions pipeline (build + test)
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
│   └── deploy-local.sh         # Local build → kind load → deploy helper
├── Dockerfile                  # Multi-stage Docker build
├── .dockerignore
├── InfraSprint.slnx            # Solution file
└── README.md
```

The application exposes the following endpoints:

| Method | Path        | Description                          |
|--------|-------------|--------------------------------------|
| GET    | `/health`   | Health check                         |
| GET    | `/missions` | List all missions (newest first)     |
| POST   | `/missions` | Create a new mission                 |
| GET    | `/metrics`  | Prometheus metrics endpoint          |

---

## Screenshots / Project Evidence

The following screenshots were captured from the working project environment (private repo setup). They are included here to document what the project looked like in practice.

<details>
<summary>CI/CD pipeline — build, test, and deploy</summary>

<img src="https://github.com/user-attachments/assets/4f399a89-4bf7-47a1-a5bf-2ff8b9bf3fb7" alt="GitHub Actions CI/CD pipeline showing successful build-and-test and deploy steps" />

*A successful GitHub Actions workflow run showing build-and-test and deploy stages completing on push to main. The deploy stage ran on a self-hosted runner in the private repo and is not included in this public repository.*

</details>

<details>
<summary>Kubernetes cluster — all pods running</summary>

<img src="https://github.com/user-attachments/assets/33339cf3-cdd8-4fd7-8fb6-9da4b90a9f9a" alt="kubectl get pods -A showing all running pods across namespaces, including infrasprint-api in default and Prometheus/Grafana in monitoring" />

*Output of `kubectl get pods -A` from the local kind cluster. Shows the `infrasprint-api` pod in the default namespace, core Kubernetes components in kube-system, and the full monitoring stack (Prometheus, Grafana, alertmanager, kube-state-metrics, node-exporter, pushgateway) in the monitoring namespace.*

</details>

<details>
<summary>Observability — Grafana dashboard</summary>

<img src="https://github.com/user-attachments/assets/4029850e-53fc-4293-a02c-05c6ff785ab6" alt="Grafana dashboard showing metrics from the api-app" />

*Grafana dashboard displaying metrics collected by Prometheus from the running `infrasprint-api` workload. Prometheus was configured to scrape the application's `/metrics` endpoint using service annotations in the Kubernetes manifest.*

</details>

<details>
<summary>Observability — Updated Grafana dashboard</summary>

<img src="https://github.com/user-attachments/assets/ecfc8580-a11b-4e26-8c64-d3391aeef41f" alt="Updated Grafana dashboard with expanded panels including deployment replicas, pod counts, latency, and request metrics" />

*Updated Grafana dashboard with expanded panels covering available deployment replicas, node and pod counts, running pods by namespace, average latency by route, missions created total, requests by route, API running pods, and container restarts.*

</details>

<details>
<summary>Observability — Grafana dashboard under external load</summary>

<img src="https://github.com/user-attachments/assets/6cd56f48-2fa2-4de3-a379-d9f23e9b68c2" alt="Grafana dashboard showing metrics under additional load from an external script curling the API endpoints" />

*Grafana dashboard capturing metrics while an external script was actively curling the API endpoints, generating additional load. This demonstrates how the monitoring stack visualizes increased traffic and latency patterns in real time.*

</details>

<details>
<summary>Application running — endpoint response</summary>

<img src="https://github.com/user-attachments/assets/0130e113-2a95-4351-90d6-e0ea94da7b6b" alt="Application endpoint response" />

*The API responding to requests from within the local Kubernetes cluster, demonstrating that the deployment, service networking, and port-forwarding were all functioning correctly.*

</details>

<details>
<summary>Docker running in the terminal</summary>

<img src="https://github.com/user-attachments/assets/6072deef-252b-4eb4-a71a-35ed259e5f87" alt="Docker running in the terminal" />

*Docker Desktop running in the terminal environment. Docker was the container runtime used for building images and powering the local kind Kubernetes cluster.*

</details>

---

## Key Learning Outcomes

- **Delivery chain thinking** — understanding each stage from source to running workload and recognizing where failures occur between layers
- **Containerization** — building production-style multi-stage Docker images, not just running applications in containers
- **Kubernetes fundamentals** — Deployments, Services, pod lifecycle, image loading with kind, and rolling updates
- **CI/CD pipeline design** — structuring a GitHub Actions pipeline that gates deployments behind passing builds and tests
- **Observability** — instrumenting an application with Prometheus metrics and building a monitoring stack inside a local cluster
- **Linux tooling** — working with Docker, kubectl, and shell scripting in a WSL-based development environment

---

## If Extended Further

If this project were taken further toward a more production-like setup, the next steps could include:

- Pushing Docker images to a container registry and pulling them in the cluster instead of using `kind load`
- Introducing Helm charts or Kustomize for more manageable manifest templating
- Adding structured logging and a log aggregation layer (e.g., Loki or EFK stack)
- Expanding test coverage beyond the health endpoint
- Applying network policies and resource limits in Kubernetes
- Moving to a cloud-managed Kubernetes cluster to explore real ingress, load balancing, and persistent storage

---

## AI-Assisted Workflow

AI tools (including GitHub Copilot and ChatGPT) were used during this project as support for learning, debugging, and iteration. They assisted with understanding documentation, troubleshooting configuration issues, and exploring unfamiliar tooling — not as a substitute for understanding the concepts and decisions behind the project.

# 🧠 ObservabilityLib

A full-stack application demonstrating **.NET 8 Observability**, **Dockerized architecture**, and a **SQLite-backed Web API** with a modern frontend.

---

## 📁 Project Structure

ObservabilityLib/
├── devops/ # Deployment scripts, CI/CD configs, docker-compose
├── frontend/ # Frontend application (razor pages)
├── Observability/ # Shared observability library (metrics, tracing, logging)
├── ObservabilityTest/ # Unit and integration tests
├── web/ # ASP.NET Core Web API (backend)
└── ObservabilityLib.sln # Solution file

---

## ⚙️ Technologies Used

| Layer | Tech Stack |
|-------|-------------|
| **Backend** | .NET 8 (ASP.NET Core Web API) |
| **Database** | SQLite |
| **Frontend** | razor pages (Jquery,Bootrstrap) |
| **Observability** | Custom library for metrics, tracing, structured logging |
| **Containerization** | Docker & Docker Compose |
| **Testing** | xUnit / NUnit (in `ObservabilityTest`) |

---

## 🚀 Getting Started (Development)

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/yourusername/ObservabilityLib.git
cd ObservabilityLib

2️⃣ Build and Run Locally

Make sure you have:

.NET 8 SDK


SQLite (preinstalled in most environments)

Run backend:
cd web
dotnet restore
dotnet build
dotnet ef database update  # Apply migrations
dotnet run
```

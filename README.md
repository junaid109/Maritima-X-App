# Maritima-X: Integrated Maritime Operations Center (IMOC)

**Maritima-X** is a high-performance Hybrid Desktop Application merging industrial-grade **.NET** orchestration with high-fidelity **Unity 3D** visualization. It serves as a "Command & Control" Digital Twin system for real-time maritime operations.

## 🏗️ Project Architecture

The project is built as a **Hybrid Desktop Application**:
*   **.NET 10 (WPF)**: Acts as the "Command & Control" shell. It manages high-density data, global state, and API communication.
*   **Unity (URP)**: Acts as the "Spatial Digital Twin." It renders the ship model, overlays thermal/sensor data, and handles 3D interactions.

## 🚀 Key Features

*   **3D Spatial Telemetry**: Real-time sensor data mapping (e.g., Engine heatmaps, Hull stress).
*   **Remote CCTV Integration**: Floating RTSP video streams integrated into the 3D space.
*   **Fleet Route Optimization**: Fuel-efficient pathfinding via .NET background services visualized as "Ghost Ships."
*   **Emergency Mode**: Instant state synchronization triggering visual/audio alarms across the twin.

## 🛠️ Technology Stack

| Layer | Technology | Role |
| :--- | :--- | :--- |
| **Shell UI** | .NET 10 WPF | Main Application Window, DataGrids, Docking |
| **3D Engine** | Unity 6 / 2022 LTS | High-fidelity rendering, Physics, Audio |
| **Backend** | ASP.NET Core Web API | Microservices, Data Aggregation |
| **Database** | PostgreSQL + TimescaleDB | Time-series telemetry storage |
| **IPC** | Named Pipes / Shared Memory | Ultra-low latency Shell <-> Engine communication |

## 📦 Usage

*(Instructions for building and running will be added as development progresses)*

## 📄 License

Proprietary Software.

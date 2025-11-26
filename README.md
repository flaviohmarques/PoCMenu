# PoCMenu

**PoCMenu** is a full-stack Proof of Concept (PoC) application designed to demonstrate a modern approach. 
It features a decoupled architecture with a **React** frontend and a **.NET 10** backend API.

## 📂 Project Structure

The repository is organized into two main directories:

| Directory | Service | Description |
| :--- | :--- | :--- |
| **`/MenuManagementAPI`** | **Backend** | A RESTful API built with **.NET 10 (C#)** that handles business logic and data management. |
| **`/menu_poc`** | **Frontend** | A Single Page Application (SPA) built with **React** and **TypeScript** for the user interface. |

## 🚀 Tech Stack

### Backend
* **Framework:** .NET 10
* **Language:** C#
* **Architecture:** REST API

### Frontend
* **Framework:** React
* **Language:** TypeScript
* **Styling:** CSS

## 🛠️ Getting Started

Follow the steps below to set up the project locally.

### Prerequisites

Ensure you have the following installed on your machine:
* [**Node.js**](https://nodejs.org/) (LTS version recommended)
* [**.NET 10 SDK**](https://dotnet.microsoft.com/download)

---

### 1. Backend Setup

1.  Navigate to the API folder:
    ```bash
    cd MenuManagementAPI
    ```

2.  Restore the .NET dependencies:
    ```bash
    dotnet restore
    ```

3.  Run the API:
    ```bash
    dotnet run --project "MenuManagementAPI.Presentation\MenuManagementAPI.WebApi.csproj" --urls "https://localhost:7241"
    ```

### 2. Frontend Setup

1.  Open a new terminal and navigate to the frontend folder:
    ```bash
    cd menu_poc
    ```

2.  Install the pnpm:
    ```bash
    npm install -g pnpm
    ```

3.  Install the dependencies:
    ```bash
    pnpm install
    ```

4.  Start the React application locally:
    ```bash
    pnpm dev
    ```
    > The application should automatically open in your default browser at `http://localhost:3000`.

## 🤝 Contributing

Contributions are welcome! Please follow these steps:
1.  Fork the repository.
2.  Create a feature branch (`git checkout -b feature/NewFeature`).
3.  Commit your changes.
4.  Push to the branch.
5.  Open a Pull Request.

## 📝 License

This project is open-source.

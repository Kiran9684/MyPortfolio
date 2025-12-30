# Local IIS Deployment Guide

This guide outlines the steps to deploy the **[MyPortfolioApp]** MVC application to a local Internet Information Services (IIS) server on Windows.

## Prerequisites

1.  **Windows OS** (Pro, Enterprise, or Education recommended).
2.  **IIS Enabled**:
    * Go to *Turn Windows features on or off*.
    * Ensure **Internet Information Services** is checked.
    * Under *World Wide Web Services > Application Development Features*, ensure **ASP.NET 4.8** (for Framework) or standard IIS features are selected.
3.  **Hosting Bundle** (If using .NET Core/.NET 5+):
    * Install the [ASP.NET Core Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/hosting) matching the project's runtime version.

---

## Step 1: Publish the Application

1.  Open the solution in **Visual Studio**.
2.  Right-click the project and select **Publish**.
3.  Create a **Folder** profile.
4.  Set the **Target Location** to a local directory (e.g., `C:\inetpub\wwwroot\[YourAppName]`).
5.  Click **Publish**.

> **Note:** Ensure you build the project in **Release** mode before publishing.

---

## Step 2: Configure IIS

1.  Open **IIS Manager** (Press `Win` + `R`, type `inetmgr`).
2.  Right-click **Sites** -> **Add Website**.
3.  Configure the site:
    * **Site name:** `[YourAppName]`
    * **Physical path:** The folder path from Step 1.
    * **Port:** `8080` (or your preferred port).
4.  Click **OK**.

### Configure Application Pool
1.  In IIS Manager, click **Application Pools**.
2.  Find the pool named `[My Portfolio App]` (created automatically with the site).
3.  Right-click -> **Basic Settings**.
4.  Set the **.NET CLR Version**:
    * **For .NET Framework (MVC 5):** Select `.NET CLR Version v4.0`.
    * **For .NET Core / .NET 5+:** Select **No Managed Code**.
5.  Click **OK**.

---

## Step 3: Set Folder Permissions

*This is the most common cause of "500 Internal Server Errors".*

1.  Navigate to the publish folder (`C:\inetpub\wwwroot\[YourAppName]`) in File Explorer.
2.  Right-click the folder -> **Properties** -> **Security**.
3.  Click **Edit** -> **Add**.
4.  Enter the object name: `IIS AppPool\[YourAppName]`
    * *Note: You must replace `[YourAppName]` with the exact name of your App Pool.*
5.  Click **Check Names** (it should become underlined).
6.  Click **OK**.
7.  Grant **Read & Execute** and **List folder contents**.
    * *If the app writes logs or uploads files, also grant **Modify** permissions.*
8.  Click **Apply**.
---
## Step 4: 🚀 IIS Deployment Setup: SQL Server Permissions

**Important:** When running the application via IIS, it runs under a virtual account (AppPool Identity) rather than your personal Windows user. You must grant this account permission to access the database.

**Error Symptom:** If this step is skipped, you will see the error:
> `Microsoft.Data.SqlClient.SqlException: Login failed for user 'IIS APPPOOL\MyPortfolioApp'.`

### Steps to Fix

1.  **Open SQL Server Management Studio (SSMS)** and connect to your database instance.
2.  Expand the root **Security** folder (not the one inside the specific database).
3.  Right-click **Logins** and select **New Login...**.
4.  **Configure the Login:**
    * **Login Name:** Type the exact name of your IIS App Pool.
        * Format: `IIS APPPOOL\YourAppPoolName` (e.g., `IIS APPPOOL\MyPortfolioApp`).
        * *Note: Do not click "Search", just type it manually.*
    * **Authentication:** Select **Windows authentication**.
5.  **Map the User:**
    * Click **User Mapping** in the left sidebar.
    * Check the box next to your database (e.g., `MyPortfolioDb`).
    * In the bottom "Database role membership" section, check:
        * `db_datareader` (Read access)
        * `db_datawriter` (Write access)
6.  Click **OK**.

---

## Step 5: Access the Application

Open your browser and navigate to:
http://localhost:8080

### (Optional) Setup Custom Domain
To access via `http://[your-custom-name].local`:

1.  **IIS:** Edit Bindings for the site and add Host Name: `[your-custom-name].local` on Port 80.
2.  **Hosts File:** Open Notepad as Admin, edit `C:\Windows\System32\drivers\etc\hosts`, and add:
    ```text
    127.0.0.1   [your-custom-name].local
    ```
---

## Issues Resolved During Local Setup: 

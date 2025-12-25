# Classic EF Core Multi‑Project Layout

## ✅ Your Setup

### **MyPortfolio.DataAccess**
* Contains your `DbContext`
* Contains your entity classes
* Contains your Fluent API configurations
* **Type:** Class library (DLL)

### **MyPortfolio.Web**
* Contains your connection string
* Contains your `appsettings.json`
* Runs the application
* **Type:** Web App

---

## ✅ Which Project Is the Startup Project?
**Startup project** = the project that EF Core must run to read configuration, including the connection string.

👉 **Startup Project:** `MyPortfolio.Web`

**Why?**
* EF Core needs to execute your app’s code at design time.
* It must read the connection string from `appsettings.json`.
* Only the web project knows how to build the host and load configuration.

---

## ✅ Which Project Is the Target Project?
**Target project** = the project where EF Core will add migrations and update the model.

👉 **Target Project:** `MyPortfolio.DataAccess`

**Why?**
* Your `DbContext` lives in `MyPortfolio.DataAccess`.
* Therefore, migrations should be stored in that project.

---

## ✅ Putting It All Together

When running EF Core CLI commands, use the following flags:

```text
--project          → MyPortfolio.DataAccess
--startup-project  → MyPortfolio.Web

Creating Migrations: 
dotnet ef migrations add InitialCreate --project MyPortfolio.DataAccess --startup-project MyPortfolio.Web  → For .NET CLI
Add-Migration InitialCreate -Project MyPortfolio.DataAccess -StartupProject MyPortfolio.Web → For Package Manager Console

Applying Migrattions: 
Update-Database -Project MyPortfolio.DataAccess -StartupProject MyPortfolio.Web → For Package Manager Console


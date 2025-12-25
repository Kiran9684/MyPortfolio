# 📐 Technical Architecture & Process Flows

This document outlines the core business logic and data flow sequences for the **MyPortfolio** application. It serves as a reference for the N-Tier architecture interactions (Web Controller -> Business Layer -> Data Access Layer).

## 1. User Authentication (Login)
**Feature:** Secure user login with password hashing and audit logging.

### 📝 Logic Summary
1.  **Input:** Controller receives Email & Password.
2.  **Verification:**
    * BL calls DAL to fetch user by Email.
    * BL hashes the input password using the stored `PasswordSalt` and compares it with `PasswordHash`.
3.  **Audit:** If credentials are valid, the system updates `LastLoginDate` to `GETUTCDATE()`.
4.  **Completion:** Controller generates an encrypted Authentication Cookie.

### 🖼️ Visual Flow
*(Replace the link below with your actual Napkin AI image path, e.g., `docs/images/auth-flow.png`)*
![User Authentication Flow](./docs/images/auth_flow_placeholder.png)

## 2. User Registration
**Feature:** New user account creation with duplicate checks and secure password hashing.

### 📝 Logic Summary
1.  **Input:** Controller receives First Name, Last Name, Email, and Password.
2.  **Uniqueness Check:**
    * BL calls DAL to check if the email already exists (`IsEmailUnique`).
    * If true, the process aborts to prevent duplicate accounts.
3.  **Security Preparation:**
    * BL generates a random cryptographic **Salt**.
    * BL hashes the input password combined with the Salt.
4.  **Persistence:**
    * A new `User` entity is created with `IsActive = true` and `CreatedDate = GETUTCDATE()`.
    * DAL saves the entity to the SQL Database.
5.  **Completion:** Controller redirects the user to the Login page.

### 🖼️ Visual Flow
*(Replace the link below with your actual Napkin AI image path)*
![User Registration Flow](./docs/images/registration_flow_placeholder.png)

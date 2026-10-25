# Request Processing

This document describes how requests flow through MyPortfolio, from user interactions in the browser to the application layers and database.

It uses Mermaid sequence diagrams to illustrate the interactions between the presentation layer, business layer, data access layer, shared models, and SQL Server.

## Table of Contents

- [1. User Registration](#1-user-registration)
- [2. User Login](#2-user-login)
- [3. User Profile Management](#3-user-profile-management)
- [4. Profile Picture Upload](#4-profile-picture-upload)
- [5. Profile Picture Retrieval](#5-profile-picture-retrieval)
- [6. User Logout](#6-user-logout)

---

## 1. User Registration

Illustrates the registration flow, including form submission, server-side validation, business-layer processing, and persistence.

```mermaid
sequenceDiagram
    actor User
    participant Browser
    participant Controller as MVC Controller
    participant Business as MyPortfolioBusiness
    participant Repository as UserRepository
    participant DB as SQL Server

    User->>Browser: Enter registration details
    Browser->>Controller: Submit registration request
    Controller->>Controller: Validate model

    alt Invalid model
        Controller-->>Browser: Return validation errors
    else Valid model
        Controller->>Business: CreateUserAccountAsync()
        Business->>Repository: Create user account
        Repository->>DB: Save user data
        DB-->>Repository: Persistence result
        Repository-->>Business: Operation result
        Business-->>Controller: Registration result
        Controller-->>Browser: Return success or error feedback
        Browser-->>User: Display notification
    end
```

## 2. User Login

Illustrates credential verification and the creation of an authentication cookie after successful authentication.

```mermaid
sequenceDiagram
    actor User
    participant Browser
    participant Controller as UserController
    participant Business as MyPortfolioBusiness
    participant Repository as UserRepository
    participant Hasher as IPasswordHasher
    participant Auth as Cookie Authentication

    User->>Browser: Enter login credentials
    Browser->>Controller: Submit credentials
    Controller->>Business: ValidateUserAccountAsync()
    Business->>Repository: Retrieve account details
    Repository-->>Business: Account and password data
    Business->>Hasher: Verify password

    alt Invalid credentials or inactive account
        Hasher-->>Business: Verification result
        Business-->>Controller: Authentication failure
        Controller-->>Browser: Return error feedback
    else Valid credentials and active account
        Hasher-->>Business: Verification result
        Business-->>Controller: Validated user details
        Controller->>Auth: Create claims identity and sign in
        Auth-->>Controller: Authentication cookie created
        Controller-->>Browser: Return success response
        Browser-->>User: Show authenticated application
    end
```

## 3. User Profile Management

Illustrates retrieving and updating the authenticated user's profile information.

```mermaid
sequenceDiagram
    actor User
    participant Browser
    participant JS as JavaScript Modules
    participant Controller as UserController
    participant Business as MyPortfolioBusiness
    participant Repository as UserRepository
    participant DB as SQL Server

    User->>Browser: Open or edit profile
    Browser->>JS: Trigger profile operation
    JS->>Controller: Send HTTP/AJAX request
    Controller->>Controller: Check authentication and validate request
    Controller->>Business: Retrieve or update profile
    Business->>Repository: Execute data operation
    Repository->>DB: Read or update profile data
    DB-->>Repository: Database result
    Repository-->>Business: Profile data or operation result
    Business-->>Controller: Return operation result
    Controller-->>JS: Return response
    JS-->>Browser: Update UI and show notification
    Browser-->>User: Display profile or operation feedback
```

## 4. Profile Picture Upload

Illustrates the profile-picture upload workflow, including browser-side preview, AJAX submission, server-side validation, and database persistence.

```mermaid
sequenceDiagram
    actor User
    participant Browser
    participant JS as UserProfile JavaScript
    participant Controller as UserController
    participant Facade as UserFacade
    participant Business as MyPortfolioBusiness
    participant Repository as UserRepository
    participant DB as SQL Server

    User->>Browser: Select JPEG or PNG image
    Browser->>JS: File selected
    JS->>JS: Validate file and generate preview
    JS-->>Browser: Display image preview
    User->>Browser: Submit picture
    Browser->>JS: Trigger upload
    JS->>Controller: AJAX request with FormData
    Controller->>Facade: ValidateProfilePictureAsync()
    Facade->>Facade: Validate extension, size and signature

    alt Invalid image
        Facade-->>Controller: Validation failure
        Controller-->>JS: Return upload error
        JS-->>Browser: Display error notification
    else Valid image
        Facade-->>Controller: Validated image bytes and MIME type
        Controller->>Business: UpdateProfilePictureAsync()
        Business->>Repository: Persist profile picture
        Repository->>DB: Save image data and MIME type
        DB-->>Repository: Persistence result
        Repository-->>Business: Operation result
        Business-->>Controller: Updated picture details
        Controller-->>JS: Return image URL and result
        JS-->>Browser: Update displayed profile picture
        Browser-->>User: Show success feedback
    end
```

## 5. Profile Picture Retrieval

Illustrates retrieving a stored profile picture through an authorized endpoint. The image identifier is validated, and the authenticated user's identity is used when requesting the image.

```mermaid
sequenceDiagram
    actor User
    participant Browser
    participant Controller as UserController
    participant Business as MyPortfolioBusiness
    participant Repository as UserRepository
    participant DB as SQL Server

    User->>Browser: View profile picture
    Browser->>Controller: Request image using image ID
    Controller->>Controller: Check authorization and validate GUID

    alt Invalid image ID
        Controller-->>Browser: Return 400 Bad Request
    else Valid image ID
        Controller->>Business: GetUserProfilePictureAsync(userId, imageId)
        Business->>Repository: Retrieve picture for user
        Repository->>DB: Query using user ID and image ID
        DB-->>Repository: Image data and MIME type
        Repository-->>Business: Picture result
        Business-->>Controller: Picture data and content type

        alt Picture not found
            Controller-->>Browser: Return 404 Not Found
        else Picture found
            Controller-->>Browser: Return image bytes with MIME type
            Browser-->>User: Display profile picture
        end
    end
```

## 6. User Logout

Illustrates signing out the authenticated user by clearing the application authentication session.

```mermaid
sequenceDiagram
    actor User
    participant Browser
    participant Controller as UserController
    participant Auth as Cookie Authentication

    User->>Browser: Select logout
    Browser->>Controller: Send logout request
    Controller->>Auth: Sign out
    Auth-->>Controller: Authentication session cleared
    Controller-->>Browser: Redirect to login page
    Browser-->>User: Display login page
```

---

## Notes

- The diagrams describe the intended request flow based on the current application design; implementation details may vary by controller action.
- The Web layer handles HTTP requests and responses, while business operations and persistence are delegated to their respective layers.
- `MyPortfolio.Models` contains shared view models and contracts used for communication between application components; it is not a separate runtime processing stage.
- Profile-picture upload and retrieval are documented here as feature-specific request flows rather than separate top-level architecture layers.
- As new features are implemented, add their sequence diagrams to this document and include a corresponding entry in the table of contents.

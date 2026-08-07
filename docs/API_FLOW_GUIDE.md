# API Flow Guide

## 1. Big Picture Flow

Every API in this solution follows the same pattern:

`Controller -> MediatR Command/Query -> Validator -> Handler -> Repository -> Database -> ApiResponse`

### Startup Flow

1. `Program.cs` loads configuration from `appsettings.json`.
2. `JwtSettings` is loaded for authentication.
3. `EmailSettings` is loaded for SMTP email sending.
4. The app builds the service container.
5. The database is migrated.
6. `DatabaseSeeder` seeds:
   - `SuperAdmin` role
   - default super-admin user
7. Middleware runs:
   - Swagger
   - Authentication
   - Authorization
8. Controllers become available.

## 2. Important Config Note

`EmailSettings` in `appsettings.json` are only for SMTP email sending.

They are used when the system sends:
- forgot-password emails
- academy admin credential emails

The super-admin password is **not** read from `appsettings.json`.
It is created in `DatabaseSeeder`:
- email: `admin@academy.com`
- password: `Admin@123`

## 3. API Flow By Endpoint

## Auth APIs

### `POST /api/auth/login`

Flow:
1. Controller receives `LoginCommand`.
2. `LoginValidator` checks email and password are present and valid.
3. `AuthCommandHandlers.Login` loads user by email.
4. It checks:
   - user exists
   - user is active
   - password matches BCrypt hash
5. If valid:
   - JWT token is generated
   - refresh token is generated
   - refresh token is saved to DB
   - response returns token, refresh token, and user data

### `POST /api/auth/change-password`

Flow:
1. Current user is read from JWT claims.
2. Current password is verified against the stored hash.
3. New password is hashed with BCrypt.
4. User record is updated.
5. Password reset fields are cleared.

### `POST /api/auth/forgot-password`

Flow:
1. User is found by email.
2. If user exists:
   - reset token is created
   - expiry is set to 30 minutes
   - token is saved
   - email is sent using SMTP settings
3. If user does not exist:
   - system still returns success to avoid account probing

### `POST /api/auth/reset-password`

Flow:
1. Token is searched in DB.
2. Token is checked for expiry.
3. New password is hashed.
4. Password and reset fields are updated.

### `POST /api/auth/refresh-token`

Flow:
1. Refresh token is checked in DB.
2. Expiry is validated.
3. New access token and refresh token are generated.
4. Refresh token is updated in DB.

### `GET /api/auth/me`

Flow:
1. User ID is read from JWT claims.
2. User is loaded from DB.
3. Current user DTO is returned.

## Academies APIs

All academy endpoints require permission-based authorization.

### `POST /api/academies`

Permission: `academy.create`

Flow:
1. Controller sends `CreateAcademyCommand`.
2. Academy is created.
3. Academy admin role is ensured.
4. Temporary password is generated and hashed.
5. Academy admin user is created with `MustChangePassword = true`.
6. Credentials are emailed to the admin.

### `PUT /api/academies/{id}`

Permission: `academy.update`

Flow:
1. Academy is loaded by ID.
2. Request fields are mapped onto the entity.
3. Updated academy is saved.

### `DELETE /api/academies/{id}`

Permission: `academy.delete`

Flow:
1. Academy is deleted by ID.
2. Success or not-found response is returned.

### `GET /api/academies`

Permission: `academy.view`

Flow:
1. All academies are fetched.
2. DTO list is returned.

### `GET /api/academies/{id}`

Permission: `academy.view`

Flow:
1. Academy is fetched by ID.
2. DTO is returned or not-found is returned.

## Roles APIs

### `POST /api/roles`

Permission: `role.create`

Flow:
1. Check role name already exists.
2. Create role.
3. Return created role DTO.

### `PUT /api/roles/{id}`

Permission: `role.update`

Flow:
1. Load role by ID.
2. Map request values.
3. Save updated role.

### `DELETE /api/roles/{id}`

Permission: `role.delete`

Flow:
1. Delete role by ID.
2. Return deleted or not-found.

### `GET /api/roles`

Permission: `role.view`

Flow:
1. Load all roles.
2. Return role DTO list.

### `GET /api/roles/{id}`

Permission: `role.view`

Flow:
1. Load role by ID.
2. Return role DTO or not-found.

## Permissions APIs

### `POST /api/permissions`

Permission: `permission.create`

Flow:
1. Check permission name already exists.
2. Create permission.
3. Return created DTO.

### `PUT /api/permissions/{id}`

Permission: `permission.update`

Flow:
1. Load permission by ID.
2. Check duplicate name conflict.
3. Map and save.

### `DELETE /api/permissions/{id}`

Permission: `permission.delete`

Flow:
1. Delete permission by ID.
2. Return deleted or not-found.

### `GET /api/permissions`

Permission: `permission.view`

Flow:
1. Load all permissions.
2. Return DTO list.

### `GET /api/permissions/{id}`

Permission: `permission.view`

Flow:
1. Load permission by ID.
2. Return DTO or not-found.

## Role Permission APIs

### `POST /api/role-permissions/assign`

Permission: `rolepermission.assign`

Flow:
1. Load role by ID.
2. Load permission by ID.
3. Check whether the role already has that permission.
4. Save the role-permission link.

### `DELETE /api/role-permissions/remove`

Permission: `rolepermission.remove`

Flow:
1. Load role by ID.
2. Load permission by ID.
3. Remove the role-permission link.

### `GET /api/role-permissions/{roleId}`

Permission: `rolepermission.view`

Flow:
1. Load role by ID.
2. Fetch all permissions assigned to that role.
3. Return the permission list.

## 4. Authorization Flow

Permission-protected endpoints use:
- JWT authentication
- `HasPermission(...)` attribute
- `PermissionAuthorizationHandler`

Authorization steps:
1. JWT is validated.
2. `userId` claim is read from token.
3. User is loaded from DB.
4. Role permissions are checked.
5. Request is allowed only if the role contains the required permission.

## 5. Short Reminder

If login fails even with the "same password", check:
- the email value in the request
- whether the password was URL-encoded by the client
- whether the stored user hash is a valid BCrypt hash
- whether the user is active
- whether the request is hitting the correct database from `appsettings.json`

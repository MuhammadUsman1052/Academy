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

## 6. Security Model Summary

The backend now uses three layers of protection for the protected modules:

1. Controller-level `[Authorize]`
2. Endpoint-level `[HasPermission("module.action")]`
3. Handler-level academy scoping for business rules

What this means:
- Authentication proves who the user is
- Permissions prove what the user can do
- Handler rules prove which academy or role scope the user is allowed to touch

Important examples:
- SuperAdmin can manage global data
- AcademyAdmin can only work inside their academy scope
- Role, permission, and academy endpoints are protected with both auth and permission rules

## 7. Current Folder Structure

The main implementation classes are now grouped more cleanly:

### API
- `Api/Authorization/` for permission policy and attribute handling
- `Api/Services/Permissions/` for controller permission scanning
- `Api/Controllers/` for endpoints

### Infrastructure
- `Infrastructure/Repositories/` for repository implementations
- `Infrastructure/Services/Common/` for shared technical services
- `Infrastructure/Services/Permissions/` for permission and sync services
- `Infrastructure/Services/Email/` for email settings and SMTP service
- `Infrastructure/Seed/` for startup seeding

## 8. Simple Test Scenario

Use this flow to test every module in a clean way.

### Step 1: Start the backend
1. Run the API project.
2. Confirm Swagger opens.
3. Confirm the app seeds `SuperAdmin` and the default user.

### Step 2: Login test
1. Call `POST /api/auth/login`
2. Use:
   - Email: `admin@academy.com`
   - Password: `Admin@123`
3. Expected result:
   - `Success = true`
   - JWT token returned
   - refresh token returned

### Step 3: Protected auth test
1. Call `GET /api/auth/me` with the bearer token.
2. Expected result:
   - current user data returned
3. Negative test:
   - remove the token
   - expected result: unauthorized/forbidden response

### Step 4: Academy module test
1. Call `POST /api/academies` as SuperAdmin.
2. Expected result:
   - academy created
   - academy admin role created automatically
   - academy owner user created
   - email attempt sent
3. Negative test:
   - call the same endpoint with a user that does not have `academy.create`
   - expected result: access denied

### Step 5: Role module test
1. Call `POST /api/roles`
2. Expected result:
   - role created in the correct scope
3. Negative test:
   - use a role without `role.create`
   - expected result: access denied

### Step 6: Permission module test
1. Call `GET /api/permissions`
2. Expected result:
   - permission list returned
3. Call `PUT /api/permissions/{id}` with a valid token and permission
4. Negative test:
   - call without `permission.update`
   - expected result: access denied

### Step 7: Role-permission module test
1. Call `GET /api/role-permissions/{roleId}`
2. Expected result:
   - all role permissions returned
3. Call `POST /api/role-permissions/assign`
4. Call `DELETE /api/role-permissions/remove`
5. Negative test:
   - use a user without the assign/remove permission
   - expected result: access denied

### Step 8: Security edge tests
1. Try login with the wrong password
2. Try calling a protected endpoint without a token
3. Try calling an academy-scoped endpoint with a role from another academy
4. Try creating a duplicate role or academy

Expected result:
- login rejects invalid credentials
- protected endpoints reject missing token
- academy scope is enforced in handlers
- duplicates return validation/business errors

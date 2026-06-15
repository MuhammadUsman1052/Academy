namespace TheMathAndScienceAcademy.Application.Common;

public static class ResponseMessages
{
    public const string LoginSuccessful = "Login successful.";
    public const string InvalidCredentials = "Invalid email or password.";
    public const string UserNotFound = "User not found.";
    public const string UserInactive = "User account is inactive.";
    public const string PasswordChangedSuccessfully = "Password changed successfully.";
    public const string PasswordChangeRequired = "Password change required.";
    public const string CurrentPasswordIncorrect = "Current password is incorrect.";
    public const string PasswordChangeFailed = "Password change failed.";
    public const string PasswordResetRequestProcessed = "Password reset request processed successfully.";
    public const string InvalidOrExpiredResetToken = "Invalid or expired reset token.";
    public const string PasswordResetFailed = "Failed to reset password.";
    public const string PasswordResetSuccessful = "Password reset successfully.";
    public const string InvalidOrExpiredRefreshToken = "Invalid or expired refresh token.";
    public const string RefreshTokenFailed = "Failed to refresh token.";
    public const string RefreshTokenSuccessful = "Refresh token generated successfully.";

    public const string RoleCreated = "Role created successfully.";
    public const string RoleCreateFailed = "Failed to create role.";
    public const string RoleUpdated = "Role updated successfully.";
    public const string RoleUpdateFailed = "Role not updated.";
    public const string RoleDeleted = "Role deleted successfully.";
    public const string RoleNotFound = "Role not found.";
    public const string RoleAlreadyExists = "Role already exists.";

    public const string PermissionCreated = "Permission created successfully.";
    public const string PermissionCreateFailed = "Failed to create permission.";
    public const string PermissionUpdated = "Permission updated successfully.";
    public const string PermissionUpdateFailed = "Permission not updated.";
    public const string PermissionDeleted = "Permission deleted successfully.";
    public const string PermissionNotFound = "Permission not found.";
    public const string PermissionAlreadyExists = "Permission already exists.";
    public const string PermissionAssigned = "Permission assigned successfully.";
    public const string PermissionRemoved = "Permission removed successfully.";
    public const string PermissionAlreadyAssigned = "Permission is already assigned to role.";
    public const string PermissionAssignFailed = "Failed to assign permission to role.";

    public const string UserCreated = "User created successfully.";
    public const string UserUpdated = "User updated successfully.";
    public const string UserDeleted = "User deleted successfully.";

    public const string AcademyCreated = "Academy created successfully.";
    public const string AcademyCreateFailed = "Failed to create academy.";
    public const string AcademyUpdated = "Academy updated successfully.";
    public const string AcademyUpdateFailed = "Failed to update academy.";
    public const string AcademyDeleted = "Academy deleted successfully.";
    public const string AcademyNotFound = "Academy not found.";
    public const string AcademyAlreadyExists = "Academy already exists.";
    public const string AcademyAdminRoleCreateFailed = "Failed to create academy admin role.";
    public const string AcademyAdminAlreadyExists = "Academy admin email already exists.";

    public const string ValidationFailed = "Validation failed.";
    public const string Unauthorized = "Unauthorized.";
    public const string Forbidden = "Forbidden.";
    public const string InternalServerError = "Internal server error.";
    public const string RecordNotFound = "Record not found.";
    public const string Success = "Success.";
}

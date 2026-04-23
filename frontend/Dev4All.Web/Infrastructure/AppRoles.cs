namespace Dev4All.Web.Infrastructure;

public static class AppRoles
{
    public const string Customer = "Customer";
    public const string Developer = "Developer";
    public const string Admin = "Admin";
}

public static class AppPolicies
{
    public const string CustomerOrAdmin = "CustomerOrAdmin";
    public const string DeveloperOrAdmin = "DeveloperOrAdmin";
}

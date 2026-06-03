namespace Nailify.Capstone.Presentation.Middlewares
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HasRoleAttribute : Attribute
    {
        public string[] AllowedRoles { get; }

        public HasRoleAttribute(params string[] roles)
        {
            AllowedRoles = roles;
        }
    }
}

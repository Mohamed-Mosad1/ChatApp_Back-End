namespace ChatApp.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SkipLoggingAttribute : Attribute
    {
    }
}

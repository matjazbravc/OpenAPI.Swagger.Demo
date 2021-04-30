using System;
using System.Linq;

namespace CompanyWebApi.Services.Helpers
{
    /// <summary>
    /// Detect if we are running as part of a nUnit/xUnitunit test.
    /// This is DIRTY and should only be used if absolutely necessary 
    /// as its usually a sign of bad design.
    /// </summary>    
    public static class UnitTestDetector
    {
        public static bool IsRunningFromUnitTest()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Any(assembly => 
                assembly.FullName.ToLowerInvariant().StartsWith("nunit") ||
                assembly.FullName.ToLowerInvariant().StartsWith("xunit"));
        }
    }
}

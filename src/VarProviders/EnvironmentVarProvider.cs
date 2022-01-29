using System;

namespace StringExpansion.VarProviders
{
    public class EnvironmentVarProvider : IVarProvider
    {
        public static readonly EnvironmentVarProvider DefaultInstance = new EnvironmentVarProvider();
        
        public string GetVar(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}

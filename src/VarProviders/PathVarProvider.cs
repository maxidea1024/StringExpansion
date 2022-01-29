using System;
using System.IO;

namespace StringExpansion.VarProviders
{
    public class PathVarProvider : IVarProvider
    {
        public static readonly PathVarProvider DefaultInstance = new PathVarProvider();
        
        public string GetVar(string name)
        {
            string path = null;
            
            if (name == "home")
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            else if (name == "module")
            {
                path = AppContext.BaseDirectory;
            }

            var envVar = Environment.GetEnvironmentVariable(name);
            if (envVar != null)
            {
                path = envVar;
            }

            if (path != null)
            {
                return MakeNonPathTerm(path);
            }

            return null;
        }

        private string MakeNonPathTerm(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            if (path.EndsWith(Path.DirectorySeparatorChar))
                return path.Substring(0, path.Length - 1);

            return path;
        }
    }
}

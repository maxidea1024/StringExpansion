namespace StringExpansion.VarProviders
{
    public class IntegratedVarProvider : IVarProvider
    {
        private readonly IVarProvider[] _providers;

        public IntegratedVarProvider(params IVarProvider[] providers)
        {
            _providers = providers;
        }

        public string GetVar(string name)
        {
            foreach (var provider in _providers)
            {
                var result = provider.GetVar(name);
                if (result != null)
                    return result;
            }
            
            return null;
        }
    }
}

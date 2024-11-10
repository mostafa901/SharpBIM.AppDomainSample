using System;
using System.Diagnostics;

namespace SharpBIMAddin
{
    // this is a workable appdomain load system
    /// <summary>
    /// Facilitates the execution of code within a separate Application Domain.
    /// </summary>
    public abstract class AssemblySandbox : MarshalByRefObject
    {
        #region Public Constructors

        public AssemblySandbox()
        { }

        #endregion Public Constructors

        #region Protected Properties

        protected AppDomain Domain { get; private set; }
        protected AssemblyResolver Resolver { get; set; }

        #endregion Protected Properties

        #region Private Methods

        private void Domain_DomainUnload(object sender, EventArgs e)
        {
            Domain.DomainUnload -= Domain_DomainUnload;
            Debug.WriteLine($"Domain Named: {Domain.FriendlyName} has been unloaded");
        }

        #endregion Private Methods

        #region Public Methods

        public virtual void Dispose()
        {
            try
            {
                // avoid disposing main domain by mistake
                Resolver?.Dispose();
                Resolver = null;
                if (Domain.IsDefaultAppDomain())
                    return;

                AppDomain.Unload(Domain);
            }
            catch (Exception e) { }
        }

        public abstract void LoadAssembly(string assemblyPath);

        public void SetDomain(AppDomain domain)
        {
            if (Domain == null && domain != null)
            {
                Domain = domain;
                Resolver = new AssemblyResolver(Domain);
                Domain.DomainUnload += Domain_DomainUnload;
            }
        }

        #endregion Public Methods
    }
}
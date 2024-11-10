using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SharpBIMAddin
{
    public class AssemblyResolver : IDisposable
    {
        #region Private Fields

        private AppDomain Domain;

        private List<string> PossibleFolderLocations = new List<string>();

        #endregion Private Fields

        #region Public Constructors

        public AssemblyResolver(AppDomain domain)
        {
            Domain = domain;
            addPath(domain.BaseDirectory);
            Domain.AssemblyResolve += ResolveAssembly;
        }

        #endregion Public Constructors

        #region Private Methods

        private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            if (args == null || string.IsNullOrWhiteSpace(args.Name))
            {
                return null;
            }
            string[] array = args.Name.Split(
                new char[1] { ',' },
                StringSplitOptions.RemoveEmptyEntries
            );
            if (array.Length <= 2)
            {
                return null;
            }

            var assemblyName = new AssemblyName(args.Name);
            if (assemblyName.Name.EndsWith("resources"))
                return null;

            foreach (var path in PossibleFolderLocations)
            {
                string assemPath = Path.Combine(path, assemblyName.Name + ".dll");
                if (File.Exists(assemPath))
                {
                    return ((AppDomain)sender).Load(File.ReadAllBytes(assemPath));
                }
            }
            return null;
        }

        #endregion Private Methods

        #region Public Methods

        public void addPath(string path)
        {
            if (!PossibleFolderLocations.Contains(path))
            {
                PossibleFolderLocations.Add(path);
            }
        }

        public void Dispose()
        {
            Domain.AssemblyResolve -= ResolveAssembly;
            Domain = null;
        }



        #endregion Public Methods
    }
}
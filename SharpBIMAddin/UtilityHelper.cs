using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBIMAddin
{
    public static class UtilityHelper
    {
        public static T LoadAssemblyViaAppDomain<T>(string targetDll)
          where T : AssemblySandbox
        {
            var domSetup = new AppDomainSetup
            {
                ShadowCopyFiles = "ShadowCopied",
                ShadowCopyDirectories = "ShadowDires",
                CachePath = Path.GetTempPath(),
                ApplicationBase = Path.GetDirectoryName(targetDll)
            };

            AppDomain domain = AppDomain.CreateDomain("IsolatedDomain", null, domSetup);
            Type assemblySandboxType = typeof(T);
            var assemblySandbox = (T)
                domain.CreateInstanceAndUnwrap(
                    assemblySandboxType.Assembly.FullName,
                    assemblySandboxType.ToString()
                );
            assemblySandbox.SetDomain(domain);

            return assemblySandbox;
        }
    }
}
# SharpBIM.AppDomainSample

This repository demonstrates how to resolve **DLL conflicts** in Revit plugins by leveraging **AppDomain** isolation. Specifically, it showcases how to load conflicting assemblies (like `Auth0.Client` and its dependencies) in an isolated AppDomain to bypass version constraints imposed by Revit’s runtime. This technique enables you to load and use custom DLL versions independently of Revit’s internal assemblies, ensuring smooth integration and preventing dependency issues.

## Key Features

- **AppDomain Isolation**: Isolates conflicting DLLs in a separate AppDomain, allowing them to load independently from Revit’s core assemblies.
- **Cross-Domain Communication**: Uses `MarshalByRefObject` to allow data exchange and method execution between the isolated AppDomain and the default Revit AppDomain.
- **Dynamic Assembly Loading**: Demonstrates how to load assemblies dynamically within the isolated AppDomain, managing dependencies without affecting Revit.
- **Seamless UI Integration**: Offers a solution for handling UI library conflicts by loading UI components in a separate domain and returning to the Revit context after completion.

![SharpBIM_DllConflict (1)](https://github.com/user-attachments/assets/df9ea912-5ee7-49e2-a001-ce8dd61de90a)

[Read this article](https://sharpbim.hashnode.dev/using-appdomains-to-resolve-dll-conflicts-in-revit-plugins) for more over view 
## How It Works

1. **Creating an Isolated AppDomain**: The code sets up a custom AppDomain with a defined setup, including application base and dependency paths.
   
   ```csharp
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

   ```

2. **Loading Assemblies in the Isolated Domain**: The sample demonstrates how to load assemblies like `Auth0.Client` inside the isolated AppDomain.
   
      ```csharp
            private string RunWithAppDomain(string dllPath)
           {
               var authService = UtilityHelper.LoadAssemblyViaAppDomain<AuthSandbox>(dllPath);
               authService.LoadAssembly(dllPath);
               string result = "";
               try
               {
                   result = authService.ExecuteLogin();
                   authService.Dispose();
               }
               catch (Exception)
               {
                   // AppDomain is unloaded I have verified that through the unloaded message in the Debugger the exception here is because the whole
                   // thread in the created appdomain is now aborted
               }
               return result;
           }
           ```

3. **Cross-Domain Execution**: Through `MarshalByRefObject`, the code enables cross-domain communication, allowing interaction between the isolated domain and the main Revit domain.

   ```csharp
     public abstract class AssemblySandbox : MarshalByRefObject
   {
       public void SetDomain(AppDomain domain)
        {
            if (Domain == null && domain != null)
            {
                Domain = domain;
                Resolver = new AssemblyResolver(Domain);
                Domain.DomainUnload += Domain_DomainUnload;
            }
        }
   } 
   
   ```

## Benefits

- **Resolve DLL Version Conflicts**: Bypass issues where Revit locks certain DLL versions by loading the necessary version in an isolated domain.
- **Maintain Stability**: Prevent Revit's internal dependencies from being overridden by external libraries, ensuring the stability of the application.
- **Flexible Plugin Development**: This approach allows plugins to manage their own dependencies, facilitating smoother integration of external libraries and services.

## Installation & Usage

1. Clone the repository to your local machine:
   ```bash
   https://github.com/mostafa901/SharpBIM.AppDomainSample.git
   ```

2. Open the project in your development environment (e.g., Visual Studio).

3. Review the code in [SharpBIMAddinCommand.cs](https://github.com/mostafa901/SharpBIM.AppDomainSample/blob/78d632851bd6a3165738a46dc4e9b94590a2766b/SharpBIMAddin/SharpBIMAddinCommand.cs#L88) to see how the AppDomain is created and how assemblies are loaded dynamically.

4. Modify the `AuthSandbox.cs` class to adapt it for your specific needs, such as interacting with different libraries or performing tasks in the isolated domain.

5. Build and run the solution to test how the isolated AppDomain handles DLL loading and cross-domain communication.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

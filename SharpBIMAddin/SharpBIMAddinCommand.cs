using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using SharpBIMAddin.Models;

namespace SharpBIMAddin
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class SharpBIMAddinCommand : IExternalCommand
    {
        #region Public Fields

        public static string dllPath;

        #endregion Public Fields

        #region Private Methods

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

        private string RunWithoutAppDomain(string dllPath)
        {
            var authService = new AuthSandbox();
            authService.SetDomain(AppDomain.CurrentDomain);
            authService.LoadAssembly(dllPath);
            var result = authService.ExecuteLogin();
            return result;
        }

        #endregion Private Methods

        #region Public Methods

        public Result Execute(
                            ExternalCommandData commandData,
            ref string message,
            ElementSet elements
        )
        {
            string result = "";
            try
            {
                dllPath =
                    dllPath
                    ?? Path.Combine(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        "SharpBIM.AuthLogin.dll"
                    );
                TaskDialog td = new TaskDialog("Login direction");
                td.TitleAutoPrefix = false;
                td.AddCommandLink(
                    TaskDialogCommandLinkId.CommandLink1,
                    "Run Login without AppDomain"
                );
                td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Run Login with AppDomain");
                var selection = td.Show();

                switch (selection)
                {
                    case TaskDialogResult.CommandLink1:
                        result = RunWithoutAppDomain(dllPath);

                        break;

                    case TaskDialogResult.CommandLink2:
                        result = RunWithAppDomain(dllPath);
                        break;

                    default:
                        result = "Nothing is selected";
                        break;
                }
                try
                {
                    var user = JsonConvert.DeserializeObject<GoogleUser>(result);
                    TaskDialog.Show("Success", $"Welcome on Board {user.FirstName}");
                }
                catch (Exception)
                {
                    throw new InvalidDataException($"Login failed\r\n\r\n{result}");
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
            return Result.Succeeded;
        }

      

        #endregion Public Methods
    }
}
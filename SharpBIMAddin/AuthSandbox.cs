using System;
using System.IO;
using System.Threading;
using SharpBIM.Interfaces;

namespace SharpBIMAddin
{
    public class AuthSandbox : AssemblySandbox
    {
        #region Private Fields

        private IAuthService _authService;

        private CancellationTokenSource CancelToken;

        #endregion Private Fields

        #region Public Methods

        public string ExecuteLogin()
        {
            CancelToken = new CancellationTokenSource();
            var result = _authService.Login(CancelToken.Token);
            result.Wait(60000, CancelToken.Token);
            CancelToken.Cancel();
            var userResult = result.Result;
            result.Dispose();
            return userResult;
        }

        public override void LoadAssembly(string assemblyPath)
        {
            var assembly = Domain.Load(File.ReadAllBytes(assemblyPath));
            var type = assembly.GetType("SharpBIM.AuthLogin.GoogleAuth");
            _authService = (IAuthService)Activator.CreateInstance(type);
        }

        #endregion Public Methods
    }
}
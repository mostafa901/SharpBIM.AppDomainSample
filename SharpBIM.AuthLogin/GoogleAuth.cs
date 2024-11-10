using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Auth0.OidcClient;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using Newtonsoft.Json;
using SharpBIM.Interfaces;

namespace SharpBIM.AuthLogin
{
    public class GoogleAuth : IAuthService
    {
        #region Private Fields

        private static SystemBrowser _browser = new SystemBrowser();

        private Auth0ClientOptions _googleOptions = new Auth0ClientOptions
        {
            // What credentials to use is there a sandbox credentials for testing
            Browser = new SystemBrowser(),
            Domain = "your.Domain.com", //application domain,
            ClientId = "AppclientKey", //application clientId
            RedirectUri = "http://localhost:8888",
            PostLogoutRedirectUri = "http://localhost:8888/logout"
        };

        private Auth0Client client;

        #endregion Private Fields

        #region Private Methods

        private IUserInfo GetUserFromToken(LoginResult loginResult)
        {
            var id_token = loginResult.IdentityToken;
            var userClaims = loginResult.User.Claims;

            IUserInfo user = new UserInfo();

            user.ExpireTime = DateTime.Now.AddSeconds(loginResult.TokenResponse.ExpiresIn);
            user.Token = loginResult.AccessToken;
            user.RefreshToken = loginResult.RefreshToken;
            user.IsExpired = false;

            user.FirstName = userClaims.ElementAt(0).Subject.Name;
            user.Country = userClaims.FirstOrDefault(o => o.Type.EndsWith("country"))?.Value;
            user.Picture = userClaims.FirstOrDefault(o => o.Type.EndsWith("picture"))?.Value;
            user.Email = userClaims.FirstOrDefault(o => o.Type.EndsWith("email"))?.Value;
            user.Company = userClaims.FirstOrDefault(o => o.Type.EndsWith("company"))?.Value;
            return user;
        }

        #endregion Private Methods

        #region Public Methods

        public async Task<string> Login(CancellationToken token)
        {
            var client = new Auth0Client(_googleOptions);
            // this is how to logout, the return should be SUCCESS var logoutUser = await client.LogoutAsync(cancellationToken: token);

            var loginResult = await client.LoginAsync(cancellationToken: token);

            if (loginResult.IsError)
            {
                return $"An error occurred during login: {loginResult.Error}";
            }
            else
            {
                var user = GetUserFromToken(loginResult);
                var jsop = new JsonSerializerSettings();
                jsop.Formatting = Formatting.Indented;
                jsop.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                return JsonConvert.SerializeObject(user, jsop);
            }
        }

        #endregion Public Methods
    }
}
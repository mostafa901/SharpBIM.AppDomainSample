using System;
using SharpBIM.Interfaces;

namespace SharpBIM.AuthLogin
{
    public class UserInfo : IUserInfo
    {
        #region Public Properties

        public string Company { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public DateTime ExpireTime { get; set; }
        public string FirstName { get; set; }
        public bool IsExpired { get; set; }
        public string Picture { get; set; }
        public string RefreshToken { get; set; }
        public string Token { get; set; }

        #endregion Public Properties
    }
}
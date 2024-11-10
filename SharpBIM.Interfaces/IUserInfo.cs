using System;

namespace SharpBIM.Interfaces
{
    public interface IUserInfo
    {
        DateTime ExpireTime { get; set; }
        string Token { get; set; }
        string RefreshToken { get; set; }
        bool IsExpired { get; set; }
        string FirstName { get; set; }
        string Country { get; set; }
        string Picture { get; set; }
        string Email { get; set; }
        string Company { get; set; }
    }
}

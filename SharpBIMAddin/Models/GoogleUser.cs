using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpBIM.Interfaces;

namespace SharpBIMAddin.Models
{
    public class GoogleUser : IUserInfo
    {
        public DateTime ExpireTime { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool IsExpired { get; set; }
        public string FirstName { get; set; }
        public string Country { get; set; }
        public string Picture { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpBIM.Interfaces
{
    public interface IAuthService
    {
        // add more functions here
        Task<string> Login(CancellationToken token);

    }
}

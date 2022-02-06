using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Net6.ExceptionService
{
    public interface IUserService
    {
        bool Validate(string id, string name);
    }
}

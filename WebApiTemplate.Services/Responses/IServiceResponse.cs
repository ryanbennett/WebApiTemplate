using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiTemplate.Services.Responses
{
    public interface IServiceResponse<out T> : IServiceResponse
    {
        T Result { get; }
    }

    public interface IServiceResponse
    {
        IEnumerable<string> ErrorMessages { get; }
        bool OperationSuccessful { get; }

    }
}

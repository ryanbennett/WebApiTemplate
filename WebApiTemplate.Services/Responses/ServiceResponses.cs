using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiTemplate.Services.Responses
{
    public class ServiceResponse<T> : ServiceResponse, IServiceResponse<T>
    {
        public T Result { get; }

        public ServiceResponse(T result = default(T), bool operationSuccessful = true, IEnumerable<string> errorMessages = null)
            : base(operationSuccessful, errorMessages)
        {
            Result = result;
        }
    }

    public class ServiceResponse : IServiceResponse
    {
        public IEnumerable<string> ErrorMessages { get; }
        public bool OperationSuccessful { get; }

        public ServiceResponse(bool operationSuccessful = true, IEnumerable<string> errorMessages = null)
        {
            ErrorMessages = errorMessages ?? Enumerable.Empty<string>();
            OperationSuccessful = operationSuccessful;
        }

        public static ServiceResponse Aggregate(IEnumerable<IServiceResponse> responses)
        {
            if (responses == null)
            {
                throw new ArgumentNullException(nameof(responses));
            }

            var operationSuccessful = true;
            var errorMessages = Enumerable.Empty<string>();
            var responsesWithValues = responses.Where(response => response != null);

            foreach (var response in responsesWithValues)
            {
                if (!response.OperationSuccessful)
                {
                    operationSuccessful = false;
                }

                if (response.ErrorMessages != null)
                {
                    errorMessages = errorMessages.Concat(response.ErrorMessages);
                }
            }

            return new ServiceResponse(operationSuccessful, errorMessages);
        }
    }
}

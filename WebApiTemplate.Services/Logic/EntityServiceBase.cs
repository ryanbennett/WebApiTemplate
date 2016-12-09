using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using WebApiTemplate.Data.Interfaces;
using WebApiTemplate.Data.Repositories;
using WebApiTemplate.Services.Responses;

namespace WebApiTemplate.Services.Logic
{
    public class EntityServiceBase<T> where T:class, IEntity, new()
    {
        protected TelemetryClient _telemetryClient = new TelemetryClient();
        protected EntityRepository<T> _repository = new EntityRepository<T>();
        protected static readonly IPrincipal User = Thread.CurrentPrincipal;

        public async Task<ServiceResponse<T>> GetById(long id)
        {
            try
            {
                var result = await _repository.GetById(id);
                return HappyIfNotNull(result);
            }
            catch (Exception ex)
            {
                return Sad<T>(ex);
            }
           
        }

        public async Task<ServiceResponse<List<T>>> Query(Func<T,bool> predicate)
        {
            try
            {
                var results = await _repository.ExecuteQuery(predicate).ToListAsync();
                return HappyIfNotNull(results);
            }
            catch (Exception ex)
            {
                return Sad<List<T>>(ex);
            }
        }

        public async Task<ServiceResponse> Update(T model)
        {
            try
            {
                await _repository.UpdateAsync(model);
                return Happy();
            }
            catch (Exception ex)
            {
                return SadVoid(ex);
            }
        }


        protected ServiceResponse Happy()
        {
            return new ServiceResponse();
        }

        protected ServiceResponse<T> Happy(T result)
        {
            return new ServiceResponse<T>(result);
        }

        protected  ServiceResponse<T> HappyIfNotNull(T result)
        {
            var errors = new List<string>();
            var resultIsNull = result == null;
            if (resultIsNull)
            {
                errors.Add("Result was null");
            }

            return new ServiceResponse<T>(result, !resultIsNull, errors);
        }

        protected ServiceResponse<List<T>> HappyIfNotNull(List<T> result)
        {
            var errors = new List<string>();
            var resultIsNull = result == null;
            if (resultIsNull)
            {
                errors.Add("Result was null");
            }

            return new ServiceResponse<List<T>>(result, !resultIsNull, errors);
        }

        protected  ServiceResponse Sad()
        {
            return new ServiceResponse(false);
        }

        protected  ServiceResponse Sad(IEnumerable<string> errorMessages)
        {
            return new ServiceResponse(false, errorMessages);
        }

        protected  ServiceResponse<T> Sad(T result)
        {
            return new ServiceResponse<T>(result, false);
        }

        protected  ServiceResponse<T> Sad(string errorMessage)
        {
            var errorMessages = new[] { errorMessage };
            return Sad<T>(errorMessages);
        }

        protected  ServiceResponse<T> Sad<T>(IEnumerable<string> errorMessages)
        {
            return new ServiceResponse<T>(default(T), false, errorMessages);
        }

        protected  ServiceResponse<T> Sad<T>(Exception e)
        {
            _telemetryClient.TrackException(e);

            var messages = new List<string>();
            messages.Add(e.Message);
            messages.Add(e.InnerException?.Message);
            return Sad<T>(messages);
        }

        protected ServiceResponse SadVoid(Exception e)
        {
            _telemetryClient.TrackException(e);

            var messages = new List<string>();
            messages.Add(e.Message);
            messages.Add(e.InnerException?.Message);
            return Sad(messages);
        }

    }
}

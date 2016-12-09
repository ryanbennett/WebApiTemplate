using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using WebApiTemplate.Data.Contexts;
using WebApiTemplate.Data.Interfaces;

namespace WebApiTemplate.Data.Repositories
{
    public class EntityRepository<T> where T : class, IEntity, new()
    {
        protected TelemetryClient _telemetryClient = new TelemetryClient();
        public async Task<T> GetById(long id)
        {
            try
            {
                return await ExecuteQuery(t => t.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                _telemetryClient.TrackException(e);
                return null;
            }

        }

        public async Task<T> Create(T model)
        {
            using (var context = GetContext())
            {
                try
                {
                    context.Set<T>().Add(model);
                    await context.SaveChangesAsync();
                    return model;
                }
                catch (Exception e)
                {
                    _telemetryClient.TrackException(e);
                    return null;
                }
            }
        }

        public async Task UpdateAsync(T model)
        {
            using (var context = GetContext())
            {
                try
                {
                    context.Set<T>().Attach(model);
                    var entry = context.Entry(model);
                    entry.State = EntityState.Modified;

                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _telemetryClient.TrackException(e);
                }
            }
        }

        public IQueryable<T> ExecuteQuery(Func<T, bool> query)
        {

            using (var context = GetContext())
            {
                try
                {
                    var enumerable = context.Set<T>().Where(query);
                    return enumerable.AsQueryable();
                }
                catch (Exception e)
                {
                    _telemetryClient.TrackException(e);
                    return null;
                }
            }
        }

        protected EntityContext GetContext()
        {
            return new EntityContext();
        }
    }
}

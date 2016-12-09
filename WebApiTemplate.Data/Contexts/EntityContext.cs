using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using WebApiTemplate.Data.Interfaces;

namespace WebApiTemplate.Data.Contexts
{
    public class EntityContext : DbContext
    {


        public EntityContext()
            : base("DefaultConnection")
        {
          
        }

        public static EntityContext Create()
        {
            return new EntityContext();
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();

            bool saveFailed = false;
            int result = 0;
            int count = 0;
            do
            {
                saveFailed = false;

                try
                {
                    result = base.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    count++;
                    if (count > 3) return 0; //can also throw here
                    // Update the values of the entity that failed to save from the store 
                    foreach (var entry in ex.Entries)
                    {
                        entry.Reload();
                    }


                }

            } while (saveFailed);

            return result;
        }

        private void UpdateTimestamps()
        {
            var userId = Thread.CurrentPrincipal?.Identity?.GetUserId();
            var timestamp = DateTimeOffset.UtcNow;

            var entitiesWithTimestamp = ChangeTracker.Entries<IHasTimestamp>().Select(x => x.Entity);

            foreach (var entity in entitiesWithTimestamp)
            {
                if (entity is IIgnoreTimestampIfExists)
                {
                    if (string.IsNullOrEmpty(entity.EditedBy))
                    {
                        entity.EditedBy = userId;
                    }

                    if (!entity.Timestamp.HasValue || entity.Timestamp == DateTimeOffset.MinValue)
                    {
                        entity.Timestamp = timestamp;
                    }

                    continue;
                }

                entity.EditedBy = userId;
                entity.Timestamp = timestamp;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            bool saveFailed;
            UpdateTimestamps();
            int result = 0;
            do
            {
                saveFailed = false;

                try
                {
                    result = await base.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store 
                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync(cancellationToken);
                    }
             

                }

            } while (saveFailed);
            return result;
        }



    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using IFramework.Domain;
using IFramework.Infrastructure;
using IFramework.UnitOfWork;
using System.Web;
using System.ServiceModel;
using System.Data.Entity.Core.Objects;

namespace IFramework.EntityFramework
{
    public static class QueryableCollectionInitializer
    {
        public static void InitializeQueryableCollections(this MSDbContext context, object entity)
        {
            var dbEntity = entity as Entity;
            if (dbEntity != null)
            {
                ((dynamic)dbEntity).DomainContext = context;
            }
        }
    }

    public class MSDbContext : DbContext
    {
        public MSDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            InitObjectContext();
           
            //if ((BaseUnitOfWork.UnitOfWorkLifetimeManagerType == typeof(PerMessageContextLifetimeManager) 
            //        && PerMessageContextLifetimeManager.CurrentMessageContext != null)
            //    || (BaseUnitOfWork.UnitOfWorkLifetimeManagerType == typeof(PerExecutionContextLifetimeManager)
            //        && (PerExecutionContextLifetimeManager.CurrentHttpContext != null || OperationContext.Current != null))
            //    || (BaseUnitOfWork.UnitOfWorkLifetimeManagerType == typeof(PerMessageOrExecutionContextLifetimeManager)
            //        && (PerMessageContextLifetimeManager.CurrentMessageContext != null
            //            || PerExecutionContextLifetimeManager.CurrentHttpContext != null 
            //            || OperationContext.Current != null)))
            //{
            //    var unitOfWork = (IoCFactory.Resolve<IUnitOfWork>() as UnitOfWork);
            //    unitOfWork.RegisterDbContext(this);
            //}
        }


        protected void InitObjectContext()
        {
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            if (objectContext != null)
            {
                objectContext.ObjectMaterialized +=
                                 (s, e) => this.InitializeQueryableCollections(e.Entity);
            }
        }

        public virtual void Rollback()
        {
            var context = (this as IObjectContextAdapter).ObjectContext;
            ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Deleted)
                                   .ForEach(e =>
                                   {
                                       e.State = EntityState.Detached;
                                   });
            var refreshableObjects = ChangeTracker.Entries()
                                                  .Where(e => e.State == EntityState.Modified || e.State == EntityState.Unchanged)
                                                  .Select(c => c.Entity);
            context.Refresh(RefreshMode.StoreWins, refreshableObjects);
            ChangeTracker.Entries().ForEach(e =>
            {
                if (e.Entity is AggregateRoot)
                {
                    (e.Entity as AggregateRoot).Rollback();
                }
            });
        }
    }
}

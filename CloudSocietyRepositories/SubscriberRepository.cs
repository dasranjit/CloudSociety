using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;

namespace CloudSociety.Repositories
{
    public class SubscriberRepository : IGenericRepository<Subscriber>
    {
//        private CloudSocietyModels.CloudSocietyEntities _entities;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _entityname = "Subscriber";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;
        
        ////private void CreateContext() // public SubscriberRepository()
        ////{
        ////    try 
        ////    {
        ////        using (_entities = new CloudSocietyModels.CloudSocietyEntities()) 
        ////        {
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext," - ","Entity Context Creation"));
        ////        throw;
        ////    }
        ////}

        public Subscriber GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Subscribers.FirstOrDefault(s => s.SubscriberID == id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get");
                sb.AppendLine("ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
        
        public bool Add(Subscriber entity)
        {
            try
            {
//                CreateContext();
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SubscriberID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                entities.Subscribers.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.SubscriberID.ToString() + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(Subscriber entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSubscriber = entities.Subscribers.FirstOrDefault(s => s.SubscriberID == entity.SubscriberID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.Subscribers.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SubscriberID.ToString() + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(Subscriber entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var Subscriber = entities.Subscribers.FirstOrDefault(s => s.SubscriberID == entity.SubscriberID);
                entities.Subscribers.DeleteObject(Subscriber);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SubscriberID.ToString() + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<Subscriber> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Subscribers.ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
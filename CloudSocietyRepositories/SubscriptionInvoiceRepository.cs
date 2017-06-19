using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;
using CloudSocietyLib.Interfaces;
using System.Data.Objects;

namespace CloudSociety.Repositories
{
    public class SubscriptionInvoiceRepository : ISubscriptionInvoiceRepository
    {
        const string _entityname = "SubscriptionInvoice";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SubscriptionInvoice GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var entity = entities.SubscriptionInvoices.FirstOrDefault(s => s.SubscriptionInvoiceID == id);
                //entities.LoadProperty(entity, "Subscriber");
                //entities.LoadProperty(entity, "Society");
//                entities.SaveChanges();
                return entity;
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

        public bool Edit(SubscriptionInvoice entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSubscriptionInvoice = entities.SubscriptionInvoices.FirstOrDefault(s => s.SubscriptionInvoiceID == entity.SubscriptionInvoiceID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SubscriptionInvoices.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SubscriptionInvoiceID.ToString() + ", " + "Amount: " + entity.Amount);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SubscriptionInvoice entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SubscriptionInvoice = entities.SubscriptionInvoices.FirstOrDefault(s => s.SubscriptionInvoiceID == entity.SubscriptionInvoiceID);
                entities.SubscriptionInvoices.DeleteObject(SubscriptionInvoice);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SubscriptionInvoiceID.ToString() + ", " + "Amount: " + entity.Amount);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SubscriptionInvoice> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SubscriptionInvoices.ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public Guid? Create(string Subscriptions, Guid SubscriberID)
        {
            try
            {
                Guid? SubscriptionInvoiceID = Guid.NewGuid();   // null;
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var curUser = Membership.GetUser();
                ObjectParameter[] qparams = 
                    { new ObjectParameter("Subscriptions", Subscriptions), new ObjectParameter("CreatedByID", (Guid)curUser.ProviderUserKey),
                      new ObjectParameter("SubscriberID", SubscriberID), new ObjectParameter("SubscriptionInvoiceID", SubscriptionInvoiceID) };
                entities.ExecuteFunction("InsertSubscriptionInvoicesForSubscriptions", qparams);
                return SubscriptionInvoiceID;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Subscriptions: " + Subscriptions);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return null;
            }
        }


        public IEnumerable<SubscriptionInvoice> ListPending()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SubscriptionInvoices.Where(i => (i.PaidAmount == null ? 0 : i.PaidAmount) < i.InvoiceAmount).OrderBy(i => i.InvoiceNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Pending List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }


        public Guid? Create(string Subscriptions)
        {
            try
            {
                Guid? SubscriptionInvoiceID = Guid.NewGuid();
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var curUser = Membership.GetUser();
                ObjectParameter[] qparams = 
                    { new ObjectParameter("Subscriptions", Subscriptions), new ObjectParameter("CreatedByID", (Guid)curUser.ProviderUserKey),
                      new ObjectParameter("SubscriptionInvoiceID", SubscriptionInvoiceID) };
                entities.ExecuteFunction("InsertSubscriptionInvoicesForSubscriptionsForCompany", qparams);
                return SubscriptionInvoiceID;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Subscriptions: " + Subscriptions);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return null;
            }
        }
    }
}
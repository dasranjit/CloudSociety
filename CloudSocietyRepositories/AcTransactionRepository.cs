using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;

namespace CloudSociety.Repositories
{
    public class AcTransactionRepository : IAcTransactionRepository
    {
        const string _entityname = "AcTransaction";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public AcTransaction GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcTransactions.FirstOrDefault(s => s.AcTransactionID == id);
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

        public bool Add(AcTransaction entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.AcTransactionID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.AcTransactions.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyID.ToString() + ", " + "Doc Type: " + entity.DocType);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(AcTransaction entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalEntity = entities.AcTransactions.FirstOrDefault(s => s.AcTransactionID == entity.AcTransactionID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.AcTransactions.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.AcTransactionID.ToString() + ", " + "Doc No: " + entity.DocNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(AcTransaction entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
//                var OriginalEntity = entities.AcTransactions.FirstOrDefault(s => s.AcTransactionID == entity.AcTransactionID);
                // may need to load direct child entities
                var OriginalEntity = (from a in entities.AcTransactions.Include("AcTransactionAcs").Include("AcTransactionTDS")
                                      select a).SingleOrDefault(s => s.AcTransactionID == entity.AcTransactionID);
                entities.AcTransactions.DeleteObject(OriginalEntity);
                // direct child entities will be deleted through cascade defined in model
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.AcTransactionID.ToString() + ", Doc No: " + entity.DocNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        //public IEnumerable<AcTransaction> ListByParentId(Guid parentid)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.AcTransactions.Where(b => b.SocietyID==parentid).OrderBy(b => b.DocNo).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public IEnumerable<AcTransaction> ListBySocietyIDDocTypePeriod(Guid societyid, string doctype, DateTime startdate, DateTime enddate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
//                return entities.AcTransactions.Where(b => b.SocietyID == societyid && b.DocType==doctype && b.DocDate.Date >= startdate.Date && b.DocDate.Date <= enddate.Date).OrderBy(b => b.DocNo).ToList();
                return entities.AcTransactions.Where(b => b.SocietyID == societyid && b.DocType == doctype && b.DocDate >= startdate && b.DocDate <= enddate).OrderBy(b => b.DocNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + societyid.ToString() + " from " + startdate.ToString("dd-MMM-yy") + " to " + enddate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcTransaction> ListBySocietySubscriptionIDDocType(Guid societysubscriptionid, string doctype)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcTransactions.Where(b => b.SocietySubscriptionID == societysubscriptionid && b.DocType == doctype).OrderBy(b => b.DocNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Year " + societysubscriptionid.ToString()+", Doc Type "+doctype);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}

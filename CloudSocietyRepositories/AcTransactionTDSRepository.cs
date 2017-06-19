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
    public class AcTransactionTDSRepository : IGenericChildRepository<AcTransactionTDS>
    {
        const string _entityname = "AcTransactionTDS";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public AcTransactionTDS GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcTransactionTDSSet.FirstOrDefault(s => s.AcTransactionTDSID == id);
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

        public bool Add(AcTransactionTDS entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.AcTransactionTDSID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.AcTransactionTDSSet.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyID.ToString() + ", " + "Name: " + entity.TDSCategory.Category);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(AcTransactionTDS entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalAcTransactionTDS = entities.AcTransactionTDSSet.FirstOrDefault(s => s.AcTransactionTDSID == entity.AcTransactionTDSID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.AcTransactionTDSSet.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.AcTransactionTDSID.ToString() + ", " + "Name: " + entity.TDSCategory.Category);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(AcTransactionTDS entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var AcTransactionTDS = entities.AcTransactionTDSSet.FirstOrDefault(s => s.AcTransactionTDSID == entity.AcTransactionTDSID);
                entities.AcTransactionTDSSet.DeleteObject(AcTransactionTDS);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.AcTransactionTDSID.ToString());// + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<AcTransactionTDS> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcTransactionTDSSet.Where(b => b.SocietyID == parentid).OrderBy(b => b.CreatedOn).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
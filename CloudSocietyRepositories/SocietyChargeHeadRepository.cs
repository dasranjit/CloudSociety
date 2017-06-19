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
    public class SocietyChargeHeadRepository : IGenericComboKeyRepository<SocietyChargeHead>
    {
        const string _entityname = "SocietyChargeHead";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public bool Add(SocietyChargeHead entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.ChargeHeadID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyChargeHeads.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyID.ToString() + ", " + "Name: " + entity.ChargeHead);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyChargeHead entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSocietyChargeHead = entities.SocietyChargeHeads.FirstOrDefault(s => s.SocietyID == entity.SocietyID && s.ChargeHeadID == entity.ChargeHeadID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyChargeHeads.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("Society ID: " + entity.SocietyID.ToString() + ", " + "Name: " + entity.ChargeHead);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyChargeHead entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SocietyChargeHead = entities.SocietyChargeHeads.FirstOrDefault(s => s.SocietyID == entity.SocietyID && s.ChargeHeadID == entity.ChargeHeadID);
                entities.SocietyChargeHeads.DeleteObject(SocietyChargeHead);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("Society ID: " + entity.SocietyID.ToString() + ", " + "Name: " + entity.ChargeHead);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyChargeHead> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyChargeHeads.Where(b => b.SocietyID == parentid).OrderBy(b => b.Sequence).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Society " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public SocietyChargeHead GetByIds(Guid parentid, Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyChargeHeads.FirstOrDefault(s => s.SocietyID == parentid && s.ChargeHeadID == id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get");
                sb.AppendLine("Society ID: "+parentid.ToString()+ ", ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyChargeHead> ListById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyChargeHeads.Where(b => b.ChargeHeadID == id).OrderBy(b => b.Society.Name).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Head " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
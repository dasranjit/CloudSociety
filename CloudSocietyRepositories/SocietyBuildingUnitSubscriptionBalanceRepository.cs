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
    public class SocietyBuildingUnitSubscriptionBalanceRepository : ISocietyBuildingUnitSubscriptionBalanceRepository
    {
        const string _entityname = "SocietyBuildingUnitSubscriptionBalance";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public bool Add(SocietyBuildingUnitSubscriptionBalance entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietyBuildingUnitSubscriptionBalanceID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyBuildingUnitSubscriptionBalances.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Unit ID: " + entity.SocietyBuildingUnitID.ToString() + ", Year ID: " + entity.SocietySubscriptionID);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyBuildingUnitSubscriptionBalance entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSociety = entities.SocietyBuildingUnitSubscriptionBalances.FirstOrDefault(s => s.SocietyBuildingUnitSubscriptionBalanceID == entity.SocietyBuildingUnitSubscriptionBalanceID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyBuildingUnitSubscriptionBalances.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SocietyBuildingUnitSubscriptionBalanceID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyBuildingUnitSubscriptionBalance entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SocietyBuildingUnitSubscriptionBalance = entities.SocietyBuildingUnitSubscriptionBalances.FirstOrDefault(s => s.SocietyBuildingUnitSubscriptionBalanceID == entity.SocietyBuildingUnitSubscriptionBalanceID);
                entities.SocietyBuildingUnitSubscriptionBalances.DeleteObject(SocietyBuildingUnitSubscriptionBalance);  // entity
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SocietyBuildingUnitSubscriptionBalanceID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyBuildingUnitSubscriptionBalance> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBuildingUnitSubscriptionBalances.Where(b => b.SocietySubscriptionID == parentid).OrderBy(b => b.SocietyBuildingUnit.SocietyBuilding.Building+b.SocietyBuildingUnit.Unit).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Year " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyBuildingUnitSubscriptionBalance> ListBySocietyBuildingUnitID(Guid societybuildingunitid)   // used for Building Unit Op Bal 
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBuildingUnitSubscriptionBalances.Where(b => b.SocietyBuildingUnitID == societybuildingunitid && b.SocietySubscriptionID == null).OrderBy(b => b.BillAbbreviation).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Unit " + societybuildingunitid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public SocietyBuildingUnitSubscriptionBalance GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBuildingUnitSubscriptionBalances.FirstOrDefault(s => s.SocietyBuildingUnitSubscriptionBalanceID == id);
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
        
        public IEnumerable<SocietyBuildingUnitBalanceWithBillReceiptExistCheck> ListOpeningBalanceBySocietyBuildingUnitIDWithBillReceiptExistCheck(Guid societybuildingunitid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListSocietyBuildingUnitOpeningBalancesBySocietyBuildingUnitIDWithBillReceiptExistCheck(societybuildingunitid).OrderBy(b => b.Member).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Opening Balance with Reference Check by Unit " + societybuildingunitid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
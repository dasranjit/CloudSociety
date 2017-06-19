using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using CloudSocietyLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;

namespace CloudSociety.Repositories
{
    public class SocietyBuildingUnitTransferRepository : ISocietyBuildingUnitTransferRepository
    {
        const string _entityname = "SocietyBuildingUnitTransfer";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SocietyBuildingUnitTransfer GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBuildingUnitTransfers.FirstOrDefault(s => s.SocietyBuildingUnitTransferID == id);
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

        public bool Add(SocietyBuildingUnitTransfer entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietyBuildingUnitTransferID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyBuildingUnitTransfers.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Unit ID: " + entity.SocietyBuildingUnitID.ToString() + ", " + "Member ID: " + entity.SocietyMemberID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyBuildingUnitTransfer entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalEntity = entities.SocietyBuildingUnitTransfers.FirstOrDefault(s => s.SocietyBuildingUnitTransferID == entity.SocietyBuildingUnitTransferID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyBuildingUnitTransfers.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("Unit: " + entity.SocietyBuildingUnit.Unit + ", " + "Name: " + entity.SocietyMember.Member);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyBuildingUnitTransfer entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalEntity = entities.SocietyBuildingUnitTransfers.FirstOrDefault(s => s.SocietyBuildingUnitTransferID == entity.SocietyBuildingUnitTransferID);
                entities.SocietyBuildingUnitTransfers.DeleteObject(OriginalEntity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("Unit: " + entity.SocietyBuildingUnit.Unit + ", " + "Name: " + entity.SocietyMember.Member);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyBuildingUnitTransfer> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBuildingUnitTransfers.Where(b => b.SocietyBuildingUnitID == parentid).OrderBy(b => b.TransferDate).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyBuildingUnitTransfer> ListBySocietyBuildingUnitIDSocietyMemberID(Guid societybuildingunitid, Guid societymemberid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBuildingUnitTransfers.Where(b => b.SocietyBuildingUnitID == societybuildingunitid && b.SocietyMemberID== societymemberid).OrderBy(b => b.TransferDate).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Unit " + societybuildingunitid.ToString() + ", Member "+societymemberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
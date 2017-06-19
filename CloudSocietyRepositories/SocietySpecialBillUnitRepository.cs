using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Data.Objects;
using System.Web.Security;

namespace CloudSociety.Repositories
{
    public class SocietySpecialBillUnitRepository : ISocietySpecialBillUnitRepository
    {
        const string _entityname = "SocietySpecialBillUnit";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public bool Add(SocietySpecialBillUnit entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entity.CreatedOn = DateTime.Now;
                entities.SocietySpecialBillUnits.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietySpecialBillID.ToString()+ ", " + "Unit ID: " + entity.SocietyBuildingUnitID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietySpecialBillUnit entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SocietySpecialBillUnit = entities.SocietySpecialBillUnits.FirstOrDefault(s => s.SocietySpecialBillID == entity.SocietySpecialBillID && s.SocietyBuildingUnitID == entity.SocietyBuildingUnitID);
                entities.SocietySpecialBillUnits.DeleteObject(SocietySpecialBillUnit);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("Parent ID: " + entity.SocietySpecialBillID.ToString() + ", " + "Unit ID: " + entity.SocietyBuildingUnitID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietySpecialBillUnit> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietySpecialBillUnits.Where(s => s.SocietySpecialBillID == parentid).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Special Bill " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public SocietySpecialBillUnit GetByIds(Guid parentid, Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietySpecialBillUnits.FirstOrDefault(s => s.SocietySpecialBillID == parentid && s.SocietyBuildingUnitID == id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get");
                sb.AppendLine("Special Bill ID: " + parentid.ToString() + ", Unit ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public void DeleteByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                ObjectParameter[] qparams = { new ObjectParameter("SocietySpecialBillID", parentid) };
                entities.ExecuteFunction("DeleteSocietySpecialBillUnitsBySocietySpecialBillID", qparams);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete for Parent");
                sb.AppendLine("Parent ID: " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
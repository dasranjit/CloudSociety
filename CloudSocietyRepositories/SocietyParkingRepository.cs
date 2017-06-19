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
    public class SocietyParkingRepository : ISocietyParkingRepository   // IGenericChildRepository<SocietyParking>
    {
        const string _entityname = "SocietyParking";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SocietyParking GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyParkings.FirstOrDefault(s => s.SocietyParkingID == id);
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

        public bool Add(SocietyParking entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietyParkingID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyParkings.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyID.ToString() + ", " + "Name: " + entity.ParkingNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyParking entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSocietyParking = entities.SocietyParkings.FirstOrDefault(s => s.SocietyParkingID == entity.SocietyParkingID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyParkings.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SocietyParkingID.ToString() + ", " + "Name: " + entity.ParkingNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyParking entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SocietyParking = entities.SocietyParkings.FirstOrDefault(s => s.SocietyParkingID == entity.SocietyParkingID);
                entities.SocietyParkings.DeleteObject(SocietyParking);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SocietyParkingID.ToString());// + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyParking> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyParkings.Where(b => b.SocietyID == parentid).OrderBy(b => b.ParkingNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyParkingWithMember> ListWithMemberAsOnDateBySocietyID(Guid societyid, DateTime asondate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListSocietyParkingsWithMemberBySocietyID(societyid,asondate).OrderBy(b => b.ParkingNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List with Member by Society " + societyid.ToString()+" As On "+asondate.ToString("dd-MMM-yyyy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
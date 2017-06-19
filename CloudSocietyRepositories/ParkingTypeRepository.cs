using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using CommonLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;

namespace CloudSociety.Repositories
{
    public class ParkingTypeRepository : IGenericRepository<ParkingType>
    {
//        private CloudSocietyModels.CloudSocietyEntities entities = new CloudSocietyModels.CloudSocietyEntities();
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _entityname = "ParkingType";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        //public ParkingTypeRepository()
        //{
        //    try 
        //    {
        //        using (entities = new CloudSocietyModels.CloudSocietyEntities()) 
        //        {
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext," - ","Entity Context Creation"));
        //        throw;
        //    }
        //}

        public ParkingType GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ParkingTypes.FirstOrDefault(s => s.ParkingTypeID == id);
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

        public bool Add(ParkingType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.ParkingTypeID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.ParkingTypes.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.ParkingTypeID.ToString() + ", " + "Name: " + entity.Type);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(ParkingType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalEntity = entities.ParkingTypes.FirstOrDefault(s => s.ParkingTypeID == entity.ParkingTypeID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.ParkingTypes.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.ParkingTypeID.ToString() + ", " + "Name: " + entity.Type);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(ParkingType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalEntity = entities.ParkingTypes.FirstOrDefault(s => s.ParkingTypeID == entity.ParkingTypeID);
                entities.ParkingTypes.DeleteObject(OriginalEntity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.ParkingTypeID.ToString() + ", " + "Name: " + entity.Type);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<ParkingType> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ParkingTypes.ToList();
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
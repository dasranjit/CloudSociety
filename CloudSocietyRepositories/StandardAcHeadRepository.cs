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
    public class StandardAcHeadRepository : IGenericRepository<StandardAcHead>
    {
        //private CloudSocietyModels.CloudSocietyEntities entities = new CloudSocietyModels.CloudSocietyEntities();
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _entityname = "StandardAcHead";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        //public StandardAcHeadRepository()
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

        public StandardAcHead GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.StandardAcHeads.FirstOrDefault(s => s.AcHeadID == id);
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

        public bool Add(StandardAcHead entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.AcHeadID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.StandardAcHeads.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.AcHeadID.ToString() + ", " + "Name: " + entity.AcHead);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(StandardAcHead entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalStandardAcHead = entities.StandardAcHeads.FirstOrDefault(s => s.AcHeadID == entity.AcHeadID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.StandardAcHeads.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.AcHeadID.ToString() + ", " + "Name: " + entity.AcHead);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(StandardAcHead entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var StandardAcHead = entities.StandardAcHeads.FirstOrDefault(s => s.AcHeadID == entity.AcHeadID);
                entities.StandardAcHeads.DeleteObject(StandardAcHead);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.AcHeadID.ToString() + ", " + "Name: " + entity.AcHead);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<StandardAcHead> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.StandardAcHeads.OrderBy(b => b.Sequence).ToList();
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
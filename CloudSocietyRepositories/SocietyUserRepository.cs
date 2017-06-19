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
    public class SocietyUserRepository : ISocietyUserRepository
    {
        const string _entityname = "SocietyUser";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public bool Add(SocietyUser entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entity.CreatedOn = DateTime.Now;
                entities.SocietyUsers.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.SocietyID.ToString());//+ ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        //public bool Edit(SocietyUser entity)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        var OriginalSociety = entities.SocietyUsers.FirstOrDefault(s => s.SocietyID == entity.SocietyID);
        //        var curUser = Membership.GetUser();
        //        entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
        //        entities.SocietyUsers.ApplyCurrentValues(entity);
        //        entities.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
        //        sb.AppendLine("ID: " + entity.SocietyID.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        return false;
        //    }
        //    return true;
        //}

        //public bool Delete(SocietyUser entity)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        entities.SocietyUsers.DeleteObject(entity);
        //        entities.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
        //        sb.AppendLine("ID: " + entity.SocietyID.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        return false;
        //    }
        //    return true;
        //}

        public void DeleteBySocietyID(Guid societyid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                ObjectParameter[] qparams = { new ObjectParameter("SocietyID", societyid) };
                entities.ExecuteFunction("DeleteSocietyUsersBySocietyID", qparams);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete for Society");
                sb.AppendLine("ID: " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public void DeleteByUserID(Guid userid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                ObjectParameter[] qparams = { new ObjectParameter("UserID", userid) };
                entities.ExecuteFunction("DeleteSocietyUsersByUserID", qparams);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete for User");
                sb.AppendLine("ID: " + userid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyUser> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyUsers.Where(s => s.SocietyID == parentid).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Society " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Repositories
{
    public class SocietyBillSeriesRepository : ISocietyBillSeriesRepository
    {
        const string _entityname = "SocietyBillSeries";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public bool Add(SocietyBillSeries entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyBillSeriesSet.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyID.ToString() + ", " + "Abbr: " + entity.BillAbbreviation);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyBillSeries entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalBillSeries = entities.SocietyBillSeriesSet.FirstOrDefault(s => s.SocietyID == entity.SocietyID && s.BillAbbreviation == entity.BillAbbreviation);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyBillSeriesSet.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("Society ID: " + entity.SocietyID.ToString() + ", " + "Abbr: " + entity.BillAbbreviation);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyBillSeries entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var BillSeries = entities.SocietyBillSeriesSet.FirstOrDefault(s => s.SocietyID == entity.SocietyID && s.BillAbbreviation == entity.BillAbbreviation);
                entities.SocietyBillSeriesSet.DeleteObject(BillSeries);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("Society ID: " + entity.SocietyID.ToString() + ", " + "Abbr: " + entity.BillAbbreviation);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyBillSeries> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBillSeriesSet.Where(b => b.SocietyID == parentid).OrderBy(b => b.BillAbbreviation).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public SocietyBillSeries GetByIdCode(Guid parentid, string code)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBillSeriesSet.FirstOrDefault(s => s.SocietyID == parentid && s.BillAbbreviation == code);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get");
                sb.AppendLine("Society ID: " + parentid.ToString() + " , Abbr: " + code);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        //public IEnumerable<SocietyBillSeriesWithLastBillDate> ListWithLastBillDateBySocietyID(Guid societyid)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.ListSocietyBillSeriesWithLastBillDateForSocietyID(societyid).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List with Last Bill Date by " + societyid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public IEnumerable<SocietyBillSeriesWithLastDates> ListWithLastDatesBySocietyID(Guid societyid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListSocietyBillSeriesWithLastDatesForSocietyID(societyid).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List with Last Dates by " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
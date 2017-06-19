using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using System.Web.Security;

namespace CloudSociety.Services
{
    public class CommunicationReplyService  // : IGenericChildRepository<CommunicationReply>
    {
        private IGenericChildRepository<CommunicationReply> _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "CommunicationReply";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public CommunicationReplyService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new CommunicationReplyCache();
        }

        //public CommunicationReply GetById(Guid id)
        //{
        //    try
        //    {
        //        return (_cache.GetById(id));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - Get", GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        public bool Add(Guid communicationId, String reply)
        {
            try
            {
                CommunicationReply entity = new CommunicationReply();
                entity.CommunicationID = communicationId;
                entity.Reply = reply;
                UserDetailService UserDetailService = new UserDetailService(_modelState);
                var m = Membership.GetUser();
                var userID = (Guid)m.ProviderUserKey;
                var ud = UserDetailService.GetById(userID);
                entity.BySocietyMemberID = (Guid)ud.SocietyMemberID;
                if (!_cache.Add(entity))
                {
                    _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                    return false;
                }
                return true;
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Create", GenericExceptionHandler.ExceptionMessage());
                return false;
            }
        }

        //public bool Edit(CommunicationReply entity)
        //{
        //    if (!_cache.Edit(entity))
        //    {
        //        _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
        //        return false;
        //    }
        //    return true;
        //}

        //public bool Delete(CommunicationReply entity)
        //{
        //    if (!_cache.Delete(entity))
        //    {
        //        _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
        //        return false;
        //    }
        //    return true;
        //}

        public IEnumerable<CommunicationReply> ListByParentId(Guid parentid)
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}
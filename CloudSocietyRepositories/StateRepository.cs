using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;

namespace CloudSociety.Repositories
{
    public class StateRepository : IReadOnlyRepository<State>
    {
        private CloudSocietyModels.CloudSocietyEntities _entities;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _entityname = "State";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        private void CreateContext()  // public StateRepository
        {
            try
            {
                _entities = new CloudSocietyModels.CloudSocietyEntities();
                //using (_entities = new CloudSocietyModels.CloudSocietyEntities())
                //{
                //}
            }
            catch (Exception ex)
            {
              
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Entity Context Creation"));
                throw;
            }
        }

        public State GetById(Guid id)
        {
            try
            {
                CreateContext();
                return _entities.States.FirstOrDefault(s => s.StateID == id);
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

        public IEnumerable<State> List()
        {
             try
            {
                CreateContext();
                return _entities.States.OrderBy(s => s.Name).ToList();
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
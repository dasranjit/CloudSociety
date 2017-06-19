using System;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Data.Metadata.Edm;
using System.Web.Security;

namespace CloudSocietyModels
{
    public partial class CloudSocietyEntities
    {
        internal void FixupByIDs()
        {
            var entries =
            from ose in this.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified)
            where ose.Entity != null
            select ose;
            foreach (var entry in entries)
            {
                var curUser = Membership.GetUser(); // System.Web.HttpContext.Current.User;
                var fieldMetaData = entry.CurrentValues.DataRecordInfo.FieldMetadata;
                FieldMetadata ByIDField;
                FieldMetadata OnField;
                string fieldTypeName;
                if (entry.State == EntityState.Added)
                {
                    ByIDField = fieldMetaData
                    .Where(f => f.FieldType.Name == "CreatedByID")
                    .FirstOrDefault();
                    OnField = fieldMetaData
                    .Where(f => f.FieldType.Name == "CreatedOn")
                    .FirstOrDefault();
                    if (OnField.FieldType != null)
                    {
                        fieldTypeName = OnField.FieldType.TypeUsage.EdmType.Name;
                        if (fieldTypeName == PrimitiveTypeKind.DateTime.ToString())
                        {
                            entry.CurrentValues.SetDateTime(OnField.Ordinal, DateTime.Now);
                        }
                    }
                }
                else
                {
                    ByIDField = fieldMetaData
                    .Where(f => f.FieldType.Name == "UpdatedByID")
                    .FirstOrDefault();
                }
                if (ByIDField.FieldType != null)
                {
                    fieldTypeName = ByIDField.FieldType.TypeUsage.EdmType.Name;
                    if (fieldTypeName == PrimitiveTypeKind.Guid.ToString())
                    {
                        entry.CurrentValues.SetGuid(ByIDField.Ordinal, (System.Guid)curUser.ProviderUserKey);
                    }
                }
            }
        }

        public override int SaveChanges(System.Data.Objects.SaveOptions options)
        {
 //           FixupByIDs();
            // TO DO: log the changes
            return base.SaveChanges(options);
        }

    }
}

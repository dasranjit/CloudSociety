//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;

namespace CloudSocietyEntities
{
    public partial class StandardAcCategory
    {
        #region Primitive Properties
        [Required]
        public virtual System.Guid CategoryID
        {
            get;
            set;
        }
    	
        [StringLength(100, ErrorMessage="Number of  characters cannot be more than 100")]
        [Required]
        public virtual string Category
        {
            get;
            set;
        }
    	
        [StringLength(1, ErrorMessage="Number of  characters cannot be more than 1")]
        [Required]
        public virtual string DrCr
        {
            get;
            set;
        }
    	
        [StringLength(1, ErrorMessage="Number of  characters cannot be more than 1")]
        public virtual string Nature
        {
            get;
            set;
        }
        [Required]
        public virtual byte Sequence
        {
            get;
            set;
        }
        [Required]
        public virtual System.Guid CreatedByID
        {
            get;
            set;
        }
        [Required]
        public virtual System.DateTime CreatedOn
        {
            get;
            set;
        }
        public virtual Nullable<System.Guid> UpdatedByID
        {
            get;
            set;
        }
        public virtual Nullable<System.DateTime> UpdatedOn
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<StandardAcSubCategory> StandardAcSubCategories
        {
            get
            {
                if (_standardAcSubCategories == null)
                {
                    var newCollection = new FixupCollection<StandardAcSubCategory>();
                    newCollection.CollectionChanged += FixupStandardAcSubCategories;
                    _standardAcSubCategories = newCollection;
                }
                return _standardAcSubCategories;
            }
            set
            {
                if (!ReferenceEquals(_standardAcSubCategories, value))
                {
                    var previousValue = _standardAcSubCategories as FixupCollection<StandardAcSubCategory>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupStandardAcSubCategories;
                    }
                    _standardAcSubCategories = value;
                    var newValue = value as FixupCollection<StandardAcSubCategory>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupStandardAcSubCategories;
                    }
                }
            }
        }
        private ICollection<StandardAcSubCategory> _standardAcSubCategories;

        #endregion

        #region Association Fixup
    
        private void FixupStandardAcSubCategories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (StandardAcSubCategory item in e.NewItems)
                {
                    item.StandardAcCategory = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (StandardAcSubCategory item in e.OldItems)
                {
                    if (ReferenceEquals(item.StandardAcCategory, this))
                    {
                        item.StandardAcCategory = null;
                    }
                }
            }
        }

        #endregion

    }
}

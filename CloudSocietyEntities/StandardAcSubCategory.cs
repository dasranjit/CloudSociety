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
    public partial class StandardAcSubCategory
    {
        #region Primitive Properties
        [Required]
        public virtual System.Guid SubCategoryID
        {
            get;
            set;
        }
    	
        [StringLength(100, ErrorMessage="Number of  characters cannot be more than 100")]
        [Required]
        public virtual string SubCategory
        {
            get;
            set;
        }
        [Required]
        public virtual System.Guid CategoryID
        {
            get { return _categoryID; }
            set
            {
                if (_categoryID != value)
                {
                    if (StandardAcCategory != null && StandardAcCategory.CategoryID != value)
                    {
                        StandardAcCategory = null;
                    }
                    _categoryID = value;
                }
            }
        }
        private System.Guid _categoryID;
        public virtual Nullable<bool> PartDetails
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
    
        public virtual StandardAcCategory StandardAcCategory
        {
            get { return _standardAcCategory; }
            set
            {
                if (!ReferenceEquals(_standardAcCategory, value))
                {
                    var previousValue = _standardAcCategory;
                    _standardAcCategory = value;
                    FixupStandardAcCategory(previousValue);
                }
            }
        }
        private StandardAcCategory _standardAcCategory;
    
        public virtual ICollection<StandardAcHead> StandardAcHeads
        {
            get
            {
                if (_standardAcHeads == null)
                {
                    var newCollection = new FixupCollection<StandardAcHead>();
                    newCollection.CollectionChanged += FixupStandardAcHeads;
                    _standardAcHeads = newCollection;
                }
                return _standardAcHeads;
            }
            set
            {
                if (!ReferenceEquals(_standardAcHeads, value))
                {
                    var previousValue = _standardAcHeads as FixupCollection<StandardAcHead>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupStandardAcHeads;
                    }
                    _standardAcHeads = value;
                    var newValue = value as FixupCollection<StandardAcHead>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupStandardAcHeads;
                    }
                }
            }
        }
        private ICollection<StandardAcHead> _standardAcHeads;

        #endregion

        #region Association Fixup
    
        private void FixupStandardAcCategory(StandardAcCategory previousValue)
        {
            if (previousValue != null && previousValue.StandardAcSubCategories.Contains(this))
            {
                previousValue.StandardAcSubCategories.Remove(this);
            }
    
            if (StandardAcCategory != null)
            {
                if (!StandardAcCategory.StandardAcSubCategories.Contains(this))
                {
                    StandardAcCategory.StandardAcSubCategories.Add(this);
                }
                if (CategoryID != StandardAcCategory.CategoryID)
                {
                    CategoryID = StandardAcCategory.CategoryID;
                }
            }
        }
    
        private void FixupStandardAcHeads(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (StandardAcHead item in e.NewItems)
                {
                    item.StandardAcSubCategory = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (StandardAcHead item in e.OldItems)
                {
                    if (ReferenceEquals(item.StandardAcSubCategory, this))
                    {
                        item.StandardAcSubCategory = null;
                    }
                }
            }
        }

        #endregion

    }
}

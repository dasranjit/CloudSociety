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
    public partial class MemberClass
    {
        #region Primitive Properties
        [Required]
        public virtual System.Guid MemberClassID
        {
            get;
            set;
        }
    	
        [StringLength(50, ErrorMessage="Number of  characters cannot be more than 50")]
        [Required]
        public virtual string Class
        {
            get;
            set;
        }
        [Required]
        public virtual bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
        private bool _active = true;
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
    
        public virtual ICollection<SocietyMemberJointHolder> SocietyMemberJointHolders
        {
            get
            {
                if (_societyMemberJointHolders == null)
                {
                    var newCollection = new FixupCollection<SocietyMemberJointHolder>();
                    newCollection.CollectionChanged += FixupSocietyMemberJointHolders;
                    _societyMemberJointHolders = newCollection;
                }
                return _societyMemberJointHolders;
            }
            set
            {
                if (!ReferenceEquals(_societyMemberJointHolders, value))
                {
                    var previousValue = _societyMemberJointHolders as FixupCollection<SocietyMemberJointHolder>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSocietyMemberJointHolders;
                    }
                    _societyMemberJointHolders = value;
                    var newValue = value as FixupCollection<SocietyMemberJointHolder>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSocietyMemberJointHolders;
                    }
                }
            }
        }
        private ICollection<SocietyMemberJointHolder> _societyMemberJointHolders;
    
        public virtual ICollection<SocietyMember> SocietyMembers
        {
            get
            {
                if (_societyMembers == null)
                {
                    var newCollection = new FixupCollection<SocietyMember>();
                    newCollection.CollectionChanged += FixupSocietyMembers;
                    _societyMembers = newCollection;
                }
                return _societyMembers;
            }
            set
            {
                if (!ReferenceEquals(_societyMembers, value))
                {
                    var previousValue = _societyMembers as FixupCollection<SocietyMember>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSocietyMembers;
                    }
                    _societyMembers = value;
                    var newValue = value as FixupCollection<SocietyMember>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSocietyMembers;
                    }
                }
            }
        }
        private ICollection<SocietyMember> _societyMembers;

        #endregion

        #region Association Fixup
    
        private void FixupSocietyMemberJointHolders(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SocietyMemberJointHolder item in e.NewItems)
                {
                    item.MemberClass = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SocietyMemberJointHolder item in e.OldItems)
                {
                    if (ReferenceEquals(item.MemberClass, this))
                    {
                        item.MemberClass = null;
                    }
                }
            }
        }
    
        private void FixupSocietyMembers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SocietyMember item in e.NewItems)
                {
                    item.MemberClass = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SocietyMember item in e.OldItems)
                {
                    if (ReferenceEquals(item.MemberClass, this))
                    {
                        item.MemberClass = null;
                    }
                }
            }
        }

        #endregion

    }
}

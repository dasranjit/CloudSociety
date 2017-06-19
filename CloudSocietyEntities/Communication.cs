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
    public partial class Communication
    {
        #region Primitive Properties
        [Required]
        public virtual System.Guid CommunicationID
        {
            get;
            set;
        }
        [Required]
        public virtual System.Guid SocietyID
        {
            get { return _societyID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_societyID != value)
                    {
                        if (Society != null && Society.SocietyID != value)
                        {
                            Society = null;
                        }
                        _societyID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _societyID;
        [Required]
        public virtual System.Guid FromSocietyMemberID
        {
            get { return _fromSocietyMemberID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_fromSocietyMemberID != value)
                    {
                        if (SocietyMember != null && SocietyMember.SocietyMemberID != value)
                        {
                            SocietyMember = null;
                        }
                        _fromSocietyMemberID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _fromSocietyMemberID;
        [Required]
        public virtual System.Guid CommunicationTypeID
        {
            get { return _communicationTypeID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_communicationTypeID != value)
                    {
                        if (CommunicationType != null && CommunicationType.CommunicationTypeID != value)
                        {
                            CommunicationType = null;
                        }
                        _communicationTypeID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _communicationTypeID;
    	
        [StringLength(500, ErrorMessage="Number of  characters cannot be more than 500")]
        [Required]
        public virtual string Subject
        {
            get;
            set;
        }
        [Required]
        public virtual string Details
        {
            get;
            set;
        }
        [Required]
        public virtual bool Published
        {
            get;
            set;
        }
        [Required]
        public virtual System.DateTime LastUpdate
        {
            get;
            set;
        }
        [Required]
        public virtual int Replies
        {
            get;
            set;
        }
        [Required]
        public virtual bool Closed
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
        public virtual Nullable<long> TicketNumber
        {
            get;
            set;
        }
        public virtual Nullable<System.DateTime> ClosedOn
        {
            get;
            set;
        }
        public virtual Nullable<System.Guid> ClosedBySocietyMemberID
        {
            get;
            set;
        }
        public virtual Nullable<System.Guid> ApprovedBySocietyMemberID
        {
            get { return _approvedBySocietyMemberID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_approvedBySocietyMemberID != value)
                    {
                        if (SocietyMember1 != null && SocietyMember1.SocietyMemberID != value)
                        {
                            SocietyMember1 = null;
                        }
                        _approvedBySocietyMemberID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _approvedBySocietyMemberID;
        public virtual Nullable<System.DateTime> ApprovedOn
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<CommunicationRecipient> CommunicationRecipients
        {
            get
            {
                if (_communicationRecipients == null)
                {
                    var newCollection = new FixupCollection<CommunicationRecipient>();
                    newCollection.CollectionChanged += FixupCommunicationRecipients;
                    _communicationRecipients = newCollection;
                }
                return _communicationRecipients;
            }
            set
            {
                if (!ReferenceEquals(_communicationRecipients, value))
                {
                    var previousValue = _communicationRecipients as FixupCollection<CommunicationRecipient>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCommunicationRecipients;
                    }
                    _communicationRecipients = value;
                    var newValue = value as FixupCollection<CommunicationRecipient>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCommunicationRecipients;
                    }
                }
            }
        }
        private ICollection<CommunicationRecipient> _communicationRecipients;
    
        public virtual ICollection<CommunicationReply> CommunicationReplies
        {
            get
            {
                if (_communicationReplies == null)
                {
                    var newCollection = new FixupCollection<CommunicationReply>();
                    newCollection.CollectionChanged += FixupCommunicationReplies;
                    _communicationReplies = newCollection;
                }
                return _communicationReplies;
            }
            set
            {
                if (!ReferenceEquals(_communicationReplies, value))
                {
                    var previousValue = _communicationReplies as FixupCollection<CommunicationReply>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCommunicationReplies;
                    }
                    _communicationReplies = value;
                    var newValue = value as FixupCollection<CommunicationReply>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCommunicationReplies;
                    }
                }
            }
        }
        private ICollection<CommunicationReply> _communicationReplies;
    
        public virtual CommunicationType CommunicationType
        {
            get { return _communicationType; }
            set
            {
                if (!ReferenceEquals(_communicationType, value))
                {
                    var previousValue = _communicationType;
                    _communicationType = value;
                    FixupCommunicationType(previousValue);
                }
            }
        }
        private CommunicationType _communicationType;
    
        public virtual Society Society
        {
            get { return _society; }
            set
            {
                if (!ReferenceEquals(_society, value))
                {
                    var previousValue = _society;
                    _society = value;
                    FixupSociety(previousValue);
                }
            }
        }
        private Society _society;
    
        public virtual SocietyMember SocietyMember
        {
            get { return _societyMember; }
            set
            {
                if (!ReferenceEquals(_societyMember, value))
                {
                    var previousValue = _societyMember;
                    _societyMember = value;
                    FixupSocietyMember(previousValue);
                }
            }
        }
        private SocietyMember _societyMember;
    
        public virtual SocietyMember SocietyMember1
        {
            get { return _societyMember1; }
            set
            {
                if (!ReferenceEquals(_societyMember1, value))
                {
                    var previousValue = _societyMember1;
                    _societyMember1 = value;
                    FixupSocietyMember1(previousValue);
                }
            }
        }
        private SocietyMember _societyMember1;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupCommunicationType(CommunicationType previousValue)
        {
            if (previousValue != null && previousValue.Communications.Contains(this))
            {
                previousValue.Communications.Remove(this);
            }
    
            if (CommunicationType != null)
            {
                if (!CommunicationType.Communications.Contains(this))
                {
                    CommunicationType.Communications.Add(this);
                }
                if (CommunicationTypeID != CommunicationType.CommunicationTypeID)
                {
                    CommunicationTypeID = CommunicationType.CommunicationTypeID;
                }
            }
        }
    
        private void FixupSociety(Society previousValue)
        {
            if (previousValue != null && previousValue.Communications.Contains(this))
            {
                previousValue.Communications.Remove(this);
            }
    
            if (Society != null)
            {
                if (!Society.Communications.Contains(this))
                {
                    Society.Communications.Add(this);
                }
                if (SocietyID != Society.SocietyID)
                {
                    SocietyID = Society.SocietyID;
                }
            }
        }
    
        private void FixupSocietyMember(SocietyMember previousValue)
        {
            if (previousValue != null && previousValue.Communications.Contains(this))
            {
                previousValue.Communications.Remove(this);
            }
    
            if (SocietyMember != null)
            {
                if (!SocietyMember.Communications.Contains(this))
                {
                    SocietyMember.Communications.Add(this);
                }
                if (FromSocietyMemberID != SocietyMember.SocietyMemberID)
                {
                    FromSocietyMemberID = SocietyMember.SocietyMemberID;
                }
            }
        }
    
        private void FixupSocietyMember1(SocietyMember previousValue)
        {
            if (previousValue != null && previousValue.Communications1.Contains(this))
            {
                previousValue.Communications1.Remove(this);
            }
    
            if (SocietyMember1 != null)
            {
                if (!SocietyMember1.Communications1.Contains(this))
                {
                    SocietyMember1.Communications1.Add(this);
                }
                if (ApprovedBySocietyMemberID != SocietyMember1.SocietyMemberID)
                {
                    ApprovedBySocietyMemberID = SocietyMember1.SocietyMemberID;
                }
            }
            else if (!_settingFK)
            {
                ApprovedBySocietyMemberID = null;
            }
        }
    
        private void FixupCommunicationRecipients(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CommunicationRecipient item in e.NewItems)
                {
                    item.Communication = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CommunicationRecipient item in e.OldItems)
                {
                    if (ReferenceEquals(item.Communication, this))
                    {
                        item.Communication = null;
                    }
                }
            }
        }
    
        private void FixupCommunicationReplies(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CommunicationReply item in e.NewItems)
                {
                    item.Communication = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CommunicationReply item in e.OldItems)
                {
                    if (ReferenceEquals(item.Communication, this))
                    {
                        item.Communication = null;
                    }
                }
            }
        }

        #endregion

    }
}

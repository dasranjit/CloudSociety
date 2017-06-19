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
    public partial class PayMode
    {
        #region Primitive Properties
    	
        [StringLength(2, ErrorMessage="Number of  characters cannot be more than 2")]
        [Required]
        public virtual string PayModeCode
        {
            get;
            set;
        }
    	
        [StringLength(50, ErrorMessage="Number of  characters cannot be more than 50")]
        [Required]
        public virtual string Mode
        {
            get;
            set;
        }
        [Required]
        public virtual bool AskDetails
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
        public virtual System.Guid AcHeadID
        {
            get { return _acHeadID; }
            set
            {
                if (_acHeadID != value)
                {
                    if (StandardAcHead != null && StandardAcHead.AcHeadID != value)
                    {
                        StandardAcHead = null;
                    }
                    _acHeadID = value;
                }
            }
        }
        private System.Guid _acHeadID;
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
    
        public virtual ICollection<SubscriptionInvoice> SubscriptionInvoices
        {
            get
            {
                if (_subscriptionInvoices == null)
                {
                    var newCollection = new FixupCollection<SubscriptionInvoice>();
                    newCollection.CollectionChanged += FixupSubscriptionInvoices;
                    _subscriptionInvoices = newCollection;
                }
                return _subscriptionInvoices;
            }
            set
            {
                if (!ReferenceEquals(_subscriptionInvoices, value))
                {
                    var previousValue = _subscriptionInvoices as FixupCollection<SubscriptionInvoice>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSubscriptionInvoices;
                    }
                    _subscriptionInvoices = value;
                    var newValue = value as FixupCollection<SubscriptionInvoice>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSubscriptionInvoices;
                    }
                }
            }
        }
        private ICollection<SubscriptionInvoice> _subscriptionInvoices;
    
        public virtual StandardAcHead StandardAcHead
        {
            get { return _standardAcHead; }
            set
            {
                if (!ReferenceEquals(_standardAcHead, value))
                {
                    var previousValue = _standardAcHead;
                    _standardAcHead = value;
                    FixupStandardAcHead(previousValue);
                }
            }
        }
        private StandardAcHead _standardAcHead;

        #endregion

        #region Association Fixup
    
        private void FixupStandardAcHead(StandardAcHead previousValue)
        {
            if (previousValue != null && previousValue.PayModes.Contains(this))
            {
                previousValue.PayModes.Remove(this);
            }
    
            if (StandardAcHead != null)
            {
                if (!StandardAcHead.PayModes.Contains(this))
                {
                    StandardAcHead.PayModes.Add(this);
                }
                if (AcHeadID != StandardAcHead.AcHeadID)
                {
                    AcHeadID = StandardAcHead.AcHeadID;
                }
            }
        }
    
        private void FixupSubscriptionInvoices(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SubscriptionInvoice item in e.NewItems)
                {
                    item.PayMode = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SubscriptionInvoice item in e.OldItems)
                {
                    if (ReferenceEquals(item.PayMode, this))
                    {
                        item.PayMode = null;
                    }
                }
            }
        }

        #endregion

    }
}

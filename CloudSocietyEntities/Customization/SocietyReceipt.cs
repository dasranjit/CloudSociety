namespace CloudSocietyEntities
{
    public partial class SocietyReceipt
    {
        public System.String SocietyReceiptDetails
        {
            get
            {
                return ReceiptNo + "-" + System.String.Format("{0:dd-MMM-yyyy}", ReceiptDate) + "-" + (PayRefNo ?? "") + "-" + Amount ;
            }
        }
    }
}


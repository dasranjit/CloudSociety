function showMail() {
    var TrialMail = document.getElementById("TrialMail");
    var InvoiceMail = document.getElementById("InvoiceMail");
    var BillMail = document.getElementById("BillMail");
    var TrialTerm = document.getElementById("T&C");
    //var SubscriberTerm = document.getElementById("ST");
    var TrialMailBody = document.getElementById("TrialMailBody");
    var InvoiceMailBody = document.getElementById("InvoiceMailBody");
    var BillMailBody = document.getElementById("BillMailBody");
    var TrialTerms = document.getElementById("TrialTerms");
    //var SubscriberTerms = document.getElementById("SubscriberTerms");
    if (TrialMail.checked) {
        TrialMailBody.style.visibility = "visible";
        InvoiceMailBody.style.visibility = "hidden";
        BillMailBody.style.visibility = "hidden";
        TrialTerms.style.visibility = "hidden";     
    }
    else if (InvoiceMail.checked) {
        InvoiceMailBody.style.visibility = "visible";
        TrialMailBody.style.visibility = "hidden";
        BillMailBody.style.visibility = "hidden";
        TrialTerms.style.visibility = "hidden";  
    }
    else if (BillMail.checked) {
        BillMailBody.style.visibility = "visible";
        TrialMailBody.style.visibility = "hidden";
        InvoiceMailBody.style.visibility = "hidden";
        TrialTerms.style.visibility = "hidden";  
    }
    else if (TrialTerm.checked) {
        TrialTerms.style.visibility = "visible";
        BillMailBody.style.visibility = "hidden";
        TrialMailBody.style.visibility = "hidden";
        InvoiceMailBody.style.visibility = "hidden";
    }
    else {
        TrialTerms.style.visibility = "hidden";  
        TrialMailBody.style.visibility = "hidden";
        InvoiceMailBody.style.visibility = "hidden";
        BillMailBody.style.visibility = "hidden";     
    }
}
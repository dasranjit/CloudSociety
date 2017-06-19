function changeMonthStringtoNumber(month) {
    switch (month) {
        case "Jan":
            month = 1;
            break;
        case "Feb":
            month = 2;
            break;
        case "Mar":
            month = 3;
            break;
        case "Apr":
            month = 4;
            break;
        case "May":
            month = 5;
            break;
        case "Jun":
            month = 6;
            break;
        case "Jul":
            month = 7;
            break;
        case "Aug":
            month = 8;
            break;
        case "Sep":
            month = 9;
            break;
        case "Oct":
            month = 10;
            break;
        case "Nov":
            month = 11;
            break;
        case "Dec":
            month = 12;
            break;
    }
    return (month);
}
function CheckReceiptDate() {   
    var day, month, year, dateValue;
    dateValue = document.getElementById("PaidOn").value;
    day = dateValue.substring(0, 2);
    month = changeMonthStringtoNumber(dateValue.substring(3, 6));
    year = dateValue.substring(7, 11);
    var paidOnDate = new Date(year, month, day);
    dateValue = document.getElementById("invoiceDate").value;
    day = dateValue.substring(0, 2);
    month = changeMonthStringtoNumber(dateValue.substring(3, 6));
    year = dateValue.substring(7, 11);
    var invoiceDate = new Date(year, month, day);    
    if (paidOnDate >= invoiceDate || paidOnDate == null)
        return true;
    else {
        alert("Paid on Date must be greater then or equal to Invoice Date (i.e " + document.getElementById("InvoiceDate").value + " )");
        return false;
    }
}
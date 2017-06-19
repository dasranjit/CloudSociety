/*function FilterStatus() {
    var payModeCode = document.getElementById("PayModeCode");
    var selectedPayModeCode = payModeCode.options[payModeCode.selectedIndex].value;

    if (selectedPayModeCode != "CA" && selectedPayModeCode != "CS") {        
        document.getElementById("NonCaseElements").style.visibility = "visible";
    }

    else {
        document.getElementById("NonCaseElements").style.visibility = "hidden";
    }
}*/

function ShowLastBillDate() {
    document.getElementById("displayLastBillDate").style.visibility = "visible";
}
function HideLastBillDate() {
    document.getElementById("displayLastBillDate").style.visibility = "hidden";
}

//function changeMonthStringtoNumber(month) {
//    switch (month) {
//        case "Jan":
//            month = 1;
//            break;
//        case "Feb":
//            month = 2;
//            break;
//        case "Mar":
//            month = 3;
//            break;
//        case "Apr":
//            month = 4;
//            break;
//        case "May":
//            month = 5;
//            break;
//        case "Jun":
//            month = 6;
//            break;
//        case "Jul":
//            month = 7;
//            break;
//        case "Aug":
//            month = 8;
//            break;
//        case "Sep":
//            month = 9;
//            break;
//        case "Oct":
//            month = 10;
//            break;
//        case "Nov":
//            month = 11;
//            break;
//        case "Dec":
//            month = 12;
//            break;
//    }
//    return (month);
//}
//function CheckReceiptDate() {
//    var day, month, year, dateValue;
//    dateValue = document.getElementById("ReceiptDate").value;
//    day = dateValue.substring(0, 2);
//    month = changeMonthStringtoNumber(dateValue.substring(3, 6));
//    year = dateValue.substring(7, 11);
//    var receiptDate = new Date(year, month, day);

//    dateValue = document.getElementById("StartRange").value;
//    day = dateValue.substring(0, 2);
//    month = changeMonthStringtoNumber(dateValue.substring(3, 6));
//    year = dateValue.substring(7, 11);
//    
//    var startRangeDate = new Date(year, month, day);

//    dateValue = document.getElementById("EndRange").value;
//    
//    day = dateValue.substring(0, 2);
//    month = changeMonthStringtoNumber(dateValue.substring(3, 6));
//    year = dateValue.substring(7, 11);
//    var endRangeDate = new Date(year, month, day);

//    if (receiptDate >= startRangeDate && receiptDate <= endRangeDate)
//        return true;
//    else {
//        alert("Receipt Date must be in between " + document.getElementById("StartRange").value + " and " + document.getElementById("EndRange").value);
//        return false;
//    }
//}
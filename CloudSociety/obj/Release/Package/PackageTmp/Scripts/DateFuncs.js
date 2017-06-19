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
function DateValidate(SDate, EDate) {

    var firstIndex = SDate.indexOf("-");
    var secondIndex = SDate.lastIndexOf("-");

    var d1 = SDate.substring(0, firstIndex);
    var m1 = changeMonthStringtoNumber(SDate.substring(firstIndex + 1, secondIndex));
    var y1 = SDate.substring(secondIndex + 1, SDate.length);
    var SDateFull = m1 + "/" + d1 + "/" + y1;

    var d2 = EDate.substring(0, firstIndex);
    var m2 = changeMonthStringtoNumber(EDate.substring(firstIndex + 1, secondIndex));
    var y2 = EDate.substring(secondIndex + 1, EDate.length);
    var EDateFull = m2 + "/" + d2 + "/" + y2;

    var startDate = new Date(SDateFull);
    var endDate = new Date(EDateFull);

    if (SDate != '' && EDate != '' && startDate > endDate) {
        alert('TO DATE must be greater than FROM DATE.');
        return false;
    }
    return true;
}
function CompareTwoDates() {
    var FromDate = document.getElementById("FromDate").value;
    var ToDate = document.getElementById("ToDate").value;
    var errorMsg = "";
    if (FromDate == "") { errorMsg += "FROM DATE"; }
    if (ToDate == "") { errorMsg += " TO DATE"; }
    if (errorMsg == "") {
        return DateValidate(FromDate, ToDate);
    }
    else {
        alert(errorMsg + " field(s) cannot be blank !!");
        return false;
    }
}
function CompareTwoDatesAllowNull() {
    var FromDate = document.getElementById("FromDate").value;
    var ToDate = document.getElementById("ToDate").value;
    var errorMsg = "";
    if (FromDate == "" && ToDate != "") { errorMsg += "FROM DATE"; }
    if (ToDate == "" && FromDate != "") { errorMsg += " TO DATE"; }
    if (errorMsg == "") {
        return DateValidate(FromDate, ToDate);
    }
    else {
        alert(errorMsg + " field(s) cannot be blank !!");
        return false;
    }
}
function CheckFutureDate(date) {   
    if (date == "") {
        alert("As On Date field cannot be blank !!");
        return false;
    }
    else {
        var firstIndex = date.indexOf("-");
        var secondIndex = date.lastIndexOf("-");
        var d1 = date.substring(0, firstIndex);
        var m1 = changeMonthStringtoNumber(date.substring(firstIndex + 1, secondIndex));
        var y1 = date.substring(secondIndex + 1, date.length);
        var FullDate = m1 + "/" + d1 + "/" + y1;
        var EnterDate = new Date(FullDate);
        var CurrentDate = new Date();       
        if (EnterDate > CurrentDate) {
            alert('AS ON DATE cannot be future date !!');
            return false;
        }
        return true;
    }
}
function DateRangeCheck(DateToCheck, SDate, EDate) {
//    debugger;
    var firstIndex = SDate.indexOf("-");
    var secondIndex = SDate.lastIndexOf("-");

    var d1 = SDate.substring(0, firstIndex);
    var m1 = changeMonthStringtoNumber(SDate.substring(firstIndex + 1, secondIndex));
    var y1 = SDate.substring(secondIndex + 1, SDate.length);
    var SDateFull = m1 + "/" + d1 + "/" + y1;

    var d2 = EDate.substring(0, firstIndex);
    var m2 = changeMonthStringtoNumber(EDate.substring(firstIndex + 1, secondIndex));
    var y2 = EDate.substring(secondIndex + 1, EDate.length);
    var EDateFull = m2 + "/" + d2 + "/" + y2;

    var d0 = DateToCheck.substring(0, firstIndex);
    var m0 = changeMonthStringtoNumber(DateToCheck.substring(firstIndex + 1, secondIndex));
    var y0 = DateToCheck.substring(secondIndex + 1, DateToCheck.length);
    var DateToCheckFull = m0 + "/" + d0 + "/" + y0;

    var startDate = new Date(SDateFull);
    var endDate = new Date(EDateFull);
    var checkDate = new Date(DateToCheckFull);

    if (SDate != '' && EDate != '' && (checkDate < startDate || checkDate > endDate)) {
        alert('DATE must be between ' + SDate + ' and ' + EDate);
        return false;
    }
    return true;
}

function NoOfMembers(term) 
{
    var Number = term.value;
    var Count = document.getElementById("NoOfServiceType").value;
    if (Number != "") 
    {
        var Numeeric = IsNumeric(Number);
        if (Numeeric == true) 
        {
            var NetAmount = 0;
            for (var r = 1; r <= Count; r++) 
            {
                //var Basis = document.getElementById("lblBasis+" + r).value;
                var Basis = GetElem(document.getElementById("lblBasis+" + r));
                var Chargeability = GetElem(document.getElementById("lblChargeability+" + r));
                var Rate = GetElem(document.getElementById("lblRate+" + r));
                var Period = GetElem(document.getElementById("lblPeriod+" + r));
                if (Basis == "M") 
                {
                    changeText(document.getElementById("lblQuantity+" + r), Number);
                   
                    if (Chargeability == "One Time") 
                    {
                        changeText(document.getElementById("lblAmount+" + r), (Rate * Number));
                    }
                    else 
                    {
                        
                        if (Period != "") 
                        {
                            changeText(document.getElementById("lblAmount+" + r), (Rate * Number * Period));
                        }
                    }
                }
                else if (Basis == "S") 
                {
                    if (Chargeability != "One Time") 
                    {
                        if (Period != "") 
                        {
                            changeText(document.getElementById("lblAmount+" + r), (Rate * Period));
                        }
                    }
                }

                var Chk = document.getElementById("Chk+" + r).checked;
                if (Chk == true) 
                {

                    var Amount = GetElem(document.getElementById("lblAmount+" + r));
                    if (Amount != "") 
                    {
                        NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                    }
                }

            }
            DisplayAmount(NetAmount);
        }
        else 
        {
            alert("No. Of Members should be a Number");
        }
    }
    else 
    {
        var NetAmount = 0;
        for (var r = 1; r <= Count; r++) 
        {
            //var Basis = document.getElementById("lblBasis+" + r).value;
            var Basis = GetElem(document.getElementById("lblBasis+" + r));
            if (Basis == "M") 
            {
                changeText(document.getElementById("lblQuantity+" + r), 0);
                changeText(document.getElementById("lblAmount+" + r), "");
            }

            var Chk = document.getElementById("Chk+" + r).checked;
            if (Chk == true) 
            {

                var Amount = GetElem(document.getElementById("lblAmount+" + r));
                if (Amount != "") 
                {
                    NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                }
            }

        }
        DisplayAmount(NetAmount);
    }
}



function IsNumeric(sText) 
{
    var ValidChars = "0123456789"; //Add "." for Decimal check
    var IsNumber = true;
    var Char;


    for (i = 0; i < sText.length && IsNumber == true; i++) 
    {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) == -1) 
        {
            IsNumber = false;
        }
    }
    return IsNumber;

}


function changeText(elem, changeVal) 
{
    if (typeof (elem.textContent) != "undefined") 
    {
        elem.textContent = changeVal;
    } else 
    {
        elem.innerText = changeVal;
    }
}

function GetElem(elem) 
{
    if (typeof (elem.textContent) != "undefined") 
    {
        return elem.textContent;
    } else 
    {
        return elem.innerText;
    }
}

function DisplayAmount(NetAmount) 
{
    changeText(document.getElementById("lblNetAmt"), NetAmount.toFixed(2));
    var Tax = (NetAmount / 100) * document.getElementById("TaxRate").value;
    changeText(document.getElementById("lblTaxAmt"), Tax.toFixed(2));
    changeText(document.getElementById("lblTotAmt"), (parseFloat(NetAmount.toFixed(2)) + parseFloat(Tax.toFixed(2))).toFixed(2));
}



function Months(term) 
{
    var SubscriptionStart = document.getElementById("SubscriptionStart").value;
    var SubscriptionEnd = document.getElementById("SubscriptionEnd").value;
    if (SubscriptionStart != "" && SubscriptionEnd != "") 
    {
        var Number = term.value;
        var ValidSubcribedMonthFlag = true;
        if (Number != "") 
        {
            if (IsNumeric(Number) == true) 
            {
                var flag = CheckSubscribedMonthsValidation(SubscriptionStart, SubscriptionEnd, Number);
                if (flag == false) 
                {
                    ValidSubcribedMonthFlag = false;
                    document.getElementById("SubscribedMonths").value = "";
                    Months(document.getElementById("SubscribedMonths"));
                }
            }
            else 
            {
                ValidSubcribedMonthFlag = false;
                alert("Subscribed Months should be a Number");
            }
        }

        if (ValidSubcribedMonthFlag == true) 
        {
            var Count = document.getElementById("NoOfServiceType").value;
            if (Number != "") 
            {
                var Numeeric = IsNumeric(Number);
                if (Numeeric == true) 
                {
                    var NetAmount = 0;
                    for (var r = 1; r <= Count; r++) 
                    {
                        //var Basis = document.getElementById("lblBasis+" + r).value;
                        var Basis = GetElem(document.getElementById("lblBasis+" + r));
                        var Chargeability = GetElem(document.getElementById("lblChargeability+" + r));
                        var Rate = GetElem(document.getElementById("lblRate+" + r));

                        if (Basis == "M") 
                        {
                            
                            if (Chargeability == "One Time") 
                            {

                            }
                            else 
                            {
                                changeText(document.getElementById("lblPeriod+" + r), Number);
                                var Quantity = GetElem(document.getElementById("lblQuantity+" + r));
                                if (Quantity != 0) 
                                {
                                    changeText(document.getElementById("lblAmount+" + r), (Rate * Number * Quantity));
                                }
                            }
                        }
                        else if (Basis == "S") 
                        {
                            
                            if (Chargeability != "One Time") 
                            {
                                changeText(document.getElementById("lblPeriod+" + r), Number);
                                changeText(document.getElementById("lblAmount+" + r), (Rate * Number));
                               
                            }
                        }

                        var Chk = document.getElementById("Chk+" + r).checked;
                        if (Chk == true) 
                        {

                            var Amount = GetElem(document.getElementById("lblAmount+" + r));
                            if (Amount != "") 
                            {
                                NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                            }
                        }

                    }
                    DisplayAmount(NetAmount);

                }
                else 
                {
                    alert("Subscribed Months should be a Number");
                }
            }
            else 
            {
                var NetAmount = 0;
                for (var r = 1; r <= Count; r++) 
                {
                    //var Basis = document.getElementById("lblBasis+" + r).value;
                    var Basis = GetElem(document.getElementById("lblBasis+" + r));
                    var Chargeability = GetElem(document.getElementById("lblChargeability+" + r));
                    if (Basis == "M") 
                    {
                        
                        if (Chargeability == "One Time") { }
                        else 
                        {
                            changeText(document.getElementById("lblPeriod+" + r), "");
                            changeText(document.getElementById("lblAmount+" + r), "");
                        }
                    }
                    else if (Basis == "S")
                    {
                        if (Chargeability != "One Time") 
                        {
                            changeText(document.getElementById("lblPeriod+" + r), "");
                            changeText(document.getElementById("lblAmount+" + r), "");
                        }
                    }


                    var Chk = document.getElementById("Chk+" + r).checked;
                    if (Chk == true) 
                    {

                        var Amount = GetElem(document.getElementById("lblAmount+" + r));
                        if (Amount != "") 
                        {
                            NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                        }
                    }

                }
                DisplayAmount(NetAmount);
            }
        }

    }
    else 
    {
        term.value = "";
        alert("Please Enter SubscriptionStart Date And SubscriptionEnd Date First !");

    }


}

function CheckBox() 
{
    var Count = document.getElementById("NoOfServiceType").value;
    var NetAmount = 0;
    var table = document.getElementById("tabServices");
    for (var r = 1; r <= Count; r++) 
    {

        var Chk = document.getElementById("Chk+" + r).checked;
        if (Chk == true) 
        {
            if ((r % 2) != 0) { table.rows[r].className = 'ServiceTypeTableRowEnabled'; }
            else { table.rows[r].className = 'altServiceTypeTableRowEnabled'; }

            
            var Quantity = GetElem(document.getElementById("lblQuantity+" + r));
            var Chargeability = GetElem(document.getElementById("lblChargeability+" + r));
            var Rate = GetElem(document.getElementById("lblRate+" + r));
            var Period = GetElem(document.getElementById("lblPeriod+" + r));

            if (Quantity != 0)
            {

                if (Chargeability == "One Time") { changeText(document.getElementById("lblAmount+" + r), (Quantity * Rate)); }
                else 
                {
                    if (Period != "") 
                    {
                        changeText(document.getElementById("lblAmount+" + r), (Rate * Quantity * Period));
                    }
                }
            }

            var Amount = GetElem(document.getElementById("lblAmount+" + r));
            if (Amount != "") 
            {
                NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
            }
        }
        else 
        {
            if ((r % 2) != 0) { table.rows[r].className = 'ServiceTypeTableRowDisabled'; }
            else { table.rows[r].className = 'altServiceTypeTableRowDisabled'; }
            changeText(document.getElementById("lblAmount+" + r), "");
        }

    }
    DisplayAmount(NetAmount);
}


function daysInMonth(month, year) 
{
    var dd = new Date(year, month, 0);
    return dd.getDate();
}

function getMonthIdx(monthName) 
{
    var months =
        {
            'Jan': 1, 'Feb': 2, 'Mar': 3, 'Apr': 4, 'May': 5, 'Jun': 6
            , 'Jul': 7, 'Aug': 8, 'Sep': 9, 'Oct': 10, 'Nov': 11, 'Dec': 12
        };
    return months[monthName];
}


function CheckDate(Start, End) 
{
    StartArray = Start.split("-");
    EndArray = End.split("-");

    if (StartArray[2] > EndArray[2]) { return false; }
    else if (StartArray[2] < EndArray[2]) { return true; }
    else 
    {
        if (getMonthIdx(StartArray[1]) > getMonthIdx(EndArray[1])) { return false; }
        else if (getMonthIdx(StartArray[1]) < getMonthIdx(EndArray[1])) { return true; }
        else 
        {
            if (StartArray[0] > EndArray[0]) { return false; }
            else if (StartArray[0] < EndArray[0]) { return true; }
            else { return true; }
        }
    }


}


function MonthendCheck(term) 
{
    var SubscriptionStart = document.getElementById("SubscriptionStart").value;
    var SubscriptionEnd = term.value;
    var CorrectDateFlag = true;
    if (SubscriptionStart != "") 
    {
        var Result = CheckDate(SubscriptionStart, SubscriptionEnd);
        if (Result == false) 
        {
            alert("End Date Connot be before Start date");
            term.value = "";
            CorrectDateFlag = false;
        }
    }
    if (CorrectDateFlag == true) 
    {
        myArray = SubscriptionEnd.split("-");
        if (myArray[0] != daysInMonth(getMonthIdx(myArray[1]), myArray[2])) 
        {
            alert("SubscriptionEnd Date should be End Date of the Month.");
            term.value = "";
        }
        else 
        {
            var SubscribedMonths = document.getElementById("SubscribedMonths").value;
            if (SubscribedMonths != "") 
            {
                document.getElementById("SubscribedMonths").value = "";
                Months(document.getElementById("SubscribedMonths"));
            }
        }
    }



}

function MonthStartCheck(term) 
{
    var SubscriptionStart = term.value;
    var SubscriptionEnd = document.getElementById("SubscriptionEnd").value;
    var day = SubscriptionStart.substring(0, 2);   
    if (day != "01")
        alert("Start date must be the 1st day of the month");
    if (SubscriptionEnd != "") 
    {
        var Result = CheckDate(SubscriptionStart, SubscriptionEnd);
        if (Result == false) 
        {
            alert("Start date Connot be After End Date");
            term.value = "";
        }
        else 
        {
            var SubscribedMonths = document.getElementById("SubscribedMonths").value;
            if (SubscribedMonths != "") 
            {
                document.getElementById("SubscribedMonths").value = "";
                Months(document.getElementById("SubscribedMonths"));
            }
        }
    }
    else 
    {
        var SubscribedMonths = document.getElementById("SubscribedMonths").value;
        if (SubscribedMonths != "") 
        {
            document.getElementById("SubscribedMonths").value = "";
            Months(document.getElementById("SubscribedMonths"));
        }
    }

}


function CheckSubscribedMonthsValidation(SubscriptionStart, SubscriptionEnd, Number) 
{
    StartArray = SubscriptionStart.split("-");
    EndArray = SubscriptionEnd.split("-");
    var MaxNoOfMonths = 0;
    if (StartArray[2] == EndArray[2]) 
    {
        MaxNoOfMonths = ((getMonthIdx(EndArray[1]) - getMonthIdx(StartArray[1])) + 1);
        //alert (MaxNoOfMonths);
    }
    else 
    {
        var YearDiff = (EndArray[2] - StartArray[2]);
        if (YearDiff == 1) 
        {
            MaxNoOfMonths = ((12 - (getMonthIdx(StartArray[1]) - 1)) + (getMonthIdx(EndArray[1])));
            //alert(MaxNoOfMonths); 
        }
        else 
        {
            MaxNoOfMonths = ((12 - (getMonthIdx(StartArray[1]) - 1)) + (getMonthIdx(EndArray[1]))) + ((YearDiff - 1) * 12);
            //alert(MaxNoOfMonths);
        }

    }
    if (Number > MaxNoOfMonths) { alert("SubscribedMonths cannot be Greater than " + MaxNoOfMonths); return false; } else { return true; }

}







function CheckBoxForActivatePendingSubscriptions() 
{
    var Count = document.getElementById("NoOfSubscriptions").value;
    
    var NetAmount = 0;
    var table = document.getElementById("tabSubscriptions");
    for (var r = 1; r <= Count; r++) 
    {

        var Chk = document.getElementById("Chk+" + r).checked;
        if (Chk == true) 
        {
            if ((r % 2) != 0) { table.rows[r].className = 'ServiceTypeTableRowEnabled'; }
            else { table.rows[r].className = 'altServiceTypeTableRowEnabled'; }


//            var Quantity = GetElem(document.getElementById("lblQuantity+" + r));
//            var Chargeability = GetElem(document.getElementById("lblChargeability+" + r));
//            var Rate = GetElem(document.getElementById("lblRate+" + r));
//            var Period = GetElem(document.getElementById("lblPeriod+" + r));

//            if (Quantity != 0) {

//                if (Chargeability == "One Time") { changeText(document.getElementById("lblAmount+" + r), (Quantity * Rate)); }
//                else {
//                    if (Period != "") {
//                        changeText(document.getElementById("lblAmount+" + r), (Rate * Quantity * Period));
//                    }
//                }
//            }

            var Amount = GetElem(document.getElementById("lblAmount+" + r));
            if (Amount != "") 
            {
                NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
            }
        }
        else 
        {
            if ((r % 2) != 0) { table.rows[r].className = 'ServiceTypeTableRowDisabled'; }
            else { table.rows[r].className = 'altServiceTypeTableRowDisabled'; }
        }

    }
    DisplayAmount(NetAmount);
}






function NoOfAdditionalMembers_onkeyup(term, NoOfMembers, NoOfInvoicedMembers,PaidMonths, InvoicedMonths) 
{
    var Number = term.value;
    var SubscribedMonths = document.getElementById("SubscribedMonths").value;
    var Count = document.getElementById("NoOfSubscriptions").value;
    if (Number != "") 
    {
        var Numeeric = IsNumeric(Number);
        if (Numeeric == true) 
        {
            var NetAmount = 0;
            for (var r = 1; r <= Count; r++) 
            {
                var ChargeabilityBasis = GetElem(document.getElementById("lblChargeabilityBasis+" + r));
                var ActiveStatus = GetElem(document.getElementById("lblActiveStatus+" + r));
                var Rate = GetElem(document.getElementById("lblRate+" + r));
                var Calc = "";
                var amt = 0;

                if (ChargeabilityBasis == "OM" && ActiveStatus == "P") 
                {
                    Calc = "( " + NoOfMembers + " + " + NoOfInvoicedMembers + " + " + Number + " ) * " + Rate;
                    changeText(document.getElementById("lblCalc+" + r), Calc);
                    amt = (parseFloat(NoOfMembers) + parseFloat(NoOfInvoicedMembers) + parseFloat(Number)) * Rate;
                    changeText(document.getElementById("lblAmount+" + r), amt);
                }
                else if (ChargeabilityBasis == "OM" && ActiveStatus != "P") 
                {
                    Calc = Number + " * " + Rate;
                    changeText(document.getElementById("lblCalc+" + r), Calc);
                    amt =  parseFloat(Number) * Rate;
                    changeText(document.getElementById("lblAmount+" + r), amt);
                }
                else if (ChargeabilityBasis == "MM" && ActiveStatus == "P") 
                {
                    Calc = "( " + PaidMonths + " + " + InvoicedMonths + " + " + SubscribedMonths + " ) * ( " + NoOfMembers + " + " + NoOfInvoicedMembers + " + " + Number  + " ) * " + Rate;
                    changeText(document.getElementById("lblCalc+" + r), Calc);
                    amt = (parseFloat(PaidMonths) + parseFloat(InvoicedMonths) + parseFloat(SubscribedMonths)) * (parseFloat(NoOfMembers) + parseFloat(NoOfInvoicedMembers) + parseFloat(Number) ) * Rate;
                    changeText(document.getElementById("lblAmount+" + r), amt);
                }
                else if (ChargeabilityBasis == "MM" && ActiveStatus != "P") 
                {
                    Calc = "( ( " + Number + " * ( " + PaidMonths + " + " + InvoicedMonths + " + " + SubscribedMonths + " ) ) + ( " + SubscribedMonths + " * ( " + NoOfMembers + " + " + NoOfInvoicedMembers + " ) ) ) * " + Rate;
                    changeText(document.getElementById("lblCalc+" + r), Calc);
                    amt = ((parseFloat(Number) * (parseFloat(PaidMonths) + parseFloat(InvoicedMonths) + parseFloat(SubscribedMonths))) + (parseFloat(SubscribedMonths) * (parseFloat(NoOfMembers) + parseFloat(NoOfInvoicedMembers)))) * Rate;
                    changeText(document.getElementById("lblAmount+" + r), amt);
                }

                var Chk = document.getElementById("Chk+" + r).checked;
                if (Chk == true) 
                {
                    var Amount = GetElem(document.getElementById("lblAmount+" + r));
                    if (Amount != "") 
                    {
                        NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                    }
                }

            }
            DisplayAmount(NetAmount);
        }
        else 
        {
            alert("No. Of Additional Members should be a Number");
        }
    }
    else 
    {
        var NetAmount = 0;
        for (var r = 1; r <= Count; r++) 
        {
            var ChargeabilityBasis = GetElem(document.getElementById("lblChargeabilityBasis+" + r));
            if (ChargeabilityBasis == "OM" || ChargeabilityBasis == "MM")
            {
                changeText(document.getElementById("lblCalc+" + r), "");
                changeText(document.getElementById("lblAmount+" + r), "");
            }

            var Chk = document.getElementById("Chk+" + r).checked;
            if (Chk == true) 
            {

                var Amount = GetElem(document.getElementById("lblAmount+" + r));
                if (Amount != "") 
                {
                    NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                }
            }

        }
        DisplayAmount(NetAmount);
    }
}





function SubscribedMonths_onkeyup(term, NoOfMembers, NoOfInvoicedMembers, PaidMonths, InvoicedMonths) 

{
    var NoOfAdditionalMembers = document.getElementById("NoOfAdditionalMembers").value;
    var Count = document.getElementById("NoOfSubscriptions").value;
    var SubscriptionStart = GetElem(document.getElementById("SubscriptionStart")); 
    var SubscriptionEnd = GetElem(document.getElementById("SubscriptionEnd"));

    var SubscribedMonths = term.value;
    
    if (SubscribedMonths != "") 
    {
        var Numeeric = IsNumeric(SubscribedMonths);
        if (Numeeric == true) 
        {
            var flag = CheckSubscribedMonthsValidation(SubscriptionStart, SubscriptionEnd, (parseFloat(SubscribedMonths) + parseFloat(PaidMonths) + parseFloat(InvoicedMonths)));
            if (flag == false) 
            {
                document.getElementById("SubscribedMonths").value = "";
                SubscribedMonths_onkeyup(document.getElementById("SubscribedMonths"));
            }
            else
            {
                var NetAmount = 0;
                for (var r = 1; r <= Count; r++) 
                {
                    var ChargeabilityBasis = GetElem(document.getElementById("lblChargeabilityBasis+" + r));
                    var ActiveStatus = GetElem(document.getElementById("lblActiveStatus+" + r));
                    var Rate = GetElem(document.getElementById("lblRate+" + r));
                    var Calc = "";
                    var amt = 0;

                    if (ChargeabilityBasis == "MS" && ActiveStatus == "P") 
                    {
                        Calc = "( " + PaidMonths  + " + " + InvoicedMonths + " + " + SubscribedMonths  + " ) * " + Rate;
                        changeText(document.getElementById("lblCalc+" + r), Calc);
                        amt = (parseFloat(PaidMonths) + parseFloat(InvoicedMonths) + parseFloat(SubscribedMonths)) * Rate;
                        changeText(document.getElementById("lblAmount+" + r), amt);
                    }
                    else if (ChargeabilityBasis == "MS" && ActiveStatus != "P") 
                    {
                        Calc = SubscribedMonths + " * " + Rate;
                        changeText(document.getElementById("lblCalc+" + r), Calc);
                        amt = parseFloat(SubscribedMonths) * Rate;
                        changeText(document.getElementById("lblAmount+" + r), amt);
                    }
                    else if (ChargeabilityBasis == "MM" && ActiveStatus == "P") 
                    {
                        Calc = "( " + PaidMonths + " + " + InvoicedMonths + " + " + SubscribedMonths + " ) * ( " + NoOfMembers + " + " + NoOfInvoicedMembers + " + " + NoOfAdditionalMembers  + " ) * " + Rate;
                        changeText(document.getElementById("lblCalc+" + r), Calc);
                        amt = (parseFloat(PaidMonths) + parseFloat(InvoicedMonths) + parseFloat(SubscribedMonths)) * (parseFloat(NoOfMembers) + parseFloat(NoOfInvoicedMembers) + parseFloat(NoOfAdditionalMembers)) * Rate;
                        changeText(document.getElementById("lblAmount+" + r), amt);
                    }
                    else if (ChargeabilityBasis == "MM" && ActiveStatus != "P") 
                    {
                        Calc = "( ( " + NoOfAdditionalMembers + " * ( " + PaidMonths + " + " + InvoicedMonths + " + " + SubscribedMonths + " ) ) + ( " + SubscribedMonths + " * ( " + NoOfMembers + " + " + NoOfInvoicedMembers + " ) ) ) * " + Rate;
                        changeText(document.getElementById("lblCalc+" + r), Calc);
                        amt = ((parseFloat(NoOfAdditionalMembers) * (parseFloat(PaidMonths) + parseFloat(InvoicedMonths) + parseFloat(SubscribedMonths))) + (parseFloat(SubscribedMonths) * (parseFloat(NoOfMembers) + parseFloat(NoOfInvoicedMembers)))) * Rate;
                        changeText(document.getElementById("lblAmount+" + r), amt);
                    }

                    var Chk = document.getElementById("Chk+" + r).checked;
                    if (Chk == true) 
                    {
                        var Amount = GetElem(document.getElementById("lblAmount+" + r));
                        if (Amount != "") 
                        {
                            NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                        }
                    }

                }
                DisplayAmount(NetAmount);
            }
        }
        else 
        {
            alert("No. Of Additional Members should be a Number");
        }
    }
    else 
    {
        var NetAmount = 0;
        for (var r = 1; r <= Count; r++) 
        {
            var ChargeabilityBasis = GetElem(document.getElementById("lblChargeabilityBasis+" + r));
            if (ChargeabilityBasis == "MS" || ChargeabilityBasis == "MM") 
            {
                changeText(document.getElementById("lblCalc+" + r), "");
                changeText(document.getElementById("lblAmount+" + r), "");
            }

            var Chk = document.getElementById("Chk+" + r).checked;
            if (Chk == true) {

                var Amount = GetElem(document.getElementById("lblAmount+" + r));
                if (Amount != "") {
                    NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                }
            }

        }
        DisplayAmount(NetAmount);
    }


}




function MonthendCheckforRenew(term) 
{
    var SubscriptionStart = document.getElementById("SubscriptionStart").value;
    var SubscriptionEnd = term.value;
    var CorrectDateFlag = true;
    if (SubscriptionStart != "") 
    {
        var Result = CheckDate(SubscriptionStart, SubscriptionEnd);
        if (Result == false) 
        {
            alert("End Date Connot be before Start date");
            term.value = "";
            CorrectDateFlag = false;
        }
    }
    if (CorrectDateFlag == true) 
    {
        myArray = SubscriptionEnd.split("-");
        if (myArray[0] != daysInMonth(getMonthIdx(myArray[1]), myArray[2])) 
        {
            alert("SubscriptionEnd Date should be End Date of the Month.");
            term.value = "";
        }
        else 
        {
            var SubscribedMonths = document.getElementById("SubscribedMonths").value;
            if (SubscribedMonths != "") 
            {
                document.getElementById("SubscribedMonths").value = "";
                //Months(document.getElementById("SubscribedMonths"));
            }
        }
    }



}




function NoOfMembersforRenew(term)
{
    var Number = term.value;
    var Count = document.getElementById("NoOfSubscriptions").value;
    if (Number != "") 
    {
        var Numeeric = IsNumeric(Number);
        if (Numeeric == true) 
        {
            var NetAmount = 0;
            for (var r = 1; r <= Count; r++) 
            {
                //var Basis = document.getElementById("lblBasis+" + r).value;
                var Basis = GetElem(document.getElementById("lblBasis+" + r));
                var Chargeability = GetElem(document.getElementById("lblChargeability+" + r));
                var Rate = GetElem(document.getElementById("lblRate+" + r));
                var Period = GetElem(document.getElementById("lblPeriod+" + r));
                if (Basis == "M") 
                {
                    changeText(document.getElementById("lblQuantity+" + r), Number);
                    
                    if (Chargeability == "One Time") 
                    {
                        changeText(document.getElementById("lblAmount+" + r), (Rate * Number));
                    }
                    else 
                    {
                        
                        if (Period != "") 
                        {
                            changeText(document.getElementById("lblAmount+" + r), (Rate * Number * Period));
                        }
                    }
                }
                else if (Basis == "S") 
                {
                    if (Chargeability != "One Time") 
                    {
                        if (Period != "") 
                        {
                            changeText(document.getElementById("lblAmount+" + r), (Rate * Period));
                        }
                    }
                }

                var Chk = document.getElementById("Chk+" + r).checked;
                if (Chk == true) 
                {

                    var Amount = GetElem(document.getElementById("lblAmount+" + r));
                    if (Amount != "") 
                    {
                        NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                    }
                }

            }
            DisplayAmount(NetAmount);
        }
        else 
        {
            alert("No. Of Members should be a Number");
        }
    }
    else 
    {
        var NetAmount = 0;
        for (var r = 1; r <= Count; r++) 
        {
            //var Basis = document.getElementById("lblBasis+" + r).value;
            var Basis = GetElem(document.getElementById("lblBasis+" + r));
            if (Basis == "M") 
            {
                changeText(document.getElementById("lblQuantity+" + r), 0);
                changeText(document.getElementById("lblAmount+" + r), "");
            }

            var Chk = document.getElementById("Chk+" + r).checked;
            if (Chk == true) 
            {

                var Amount = GetElem(document.getElementById("lblAmount+" + r));
                if (Amount != "") 
                {
                    NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                }
            }

        }
        DisplayAmount(NetAmount);
    }
}






function MonthsforRenew(term) 
{
    var SubscriptionStart = document.getElementById("SubscriptionStart").value;
    var SubscriptionEnd = document.getElementById("SubscriptionEnd").value;
    if (SubscriptionStart != "" && SubscriptionEnd != "") 
    {
        var Number = term.value;
        var ValidSubcribedMonthFlag = true;
        if (Number != "") 
        {
            if (IsNumeric(Number) == true) 
            {
                if (Number == 0)
                {
                    alert("SubscribedMonths cannot be 0")
                    ValidSubcribedMonthFlag = false;
                    document.getElementById("SubscribedMonths").value = "";
                    MonthsforRenew(document.getElementById("SubscribedMonths"));
                }
                else
                {
                    var flag = CheckSubscribedMonthsValidation(SubscriptionStart, SubscriptionEnd, Number);
                    if (flag == false) 
                    {
                        ValidSubcribedMonthFlag = false;
                        document.getElementById("SubscribedMonths").value = "";
                        MonthsforRenew(document.getElementById("SubscribedMonths"));
                    }
                }

            }
            else 
            {
                ValidSubcribedMonthFlag = false;
                alert("Subscribed Months should be a Number");
            }
        }

        if (ValidSubcribedMonthFlag == true) 
        {
            var Count = document.getElementById("NoOfSubscriptions").value;
            if (Number != "") 
            {
                var Numeeric = IsNumeric(Number);
                if (Numeeric == true) 
                {
                    var NetAmount = 0;
                    for (var r = 1; r <= Count; r++) 
                    {
                        //var Basis = document.getElementById("lblBasis+" + r).value;
                        var Basis = GetElem(document.getElementById("lblBasis+" + r));
                        var Chargeability = GetElem(document.getElementById("lblChargeability+" + r));
                        var Rate = GetElem(document.getElementById("lblRate+" + r));

                        if (Basis == "M") 
                        {

                            if (Chargeability == "One Time") { }
                            else 
                            {
                                changeText(document.getElementById("lblPeriod+" + r), Number);
                                var Quantity = GetElem(document.getElementById("lblQuantity+" + r));
                                if (Quantity != 0) 
                                {
                                    changeText(document.getElementById("lblAmount+" + r), (Rate * Number * Quantity));
                                }
                            }
                        }
                        else if (Basis == "S") 
                        {
                            if (Chargeability != "One Time") 
                            {
                                changeText(document.getElementById("lblPeriod+" + r), Number);
                                changeText(document.getElementById("lblAmount+" + r), (Rate * Number));
                            }
                        }

                        var Chk = document.getElementById("Chk+" + r).checked;
                        if (Chk == true) 
                        {

                            var Amount = GetElem(document.getElementById("lblAmount+" + r));
                            if (Amount != "") 
                            {
                                NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                            }
                        }

                    }
                    DisplayAmount(NetAmount);

                }
                else 
                {
                    alert("Subscribed Months should be a Number");
                }
            }
            else 
            {
                var NetAmount = 0;
                for (var r = 1; r <= Count; r++) 
                {
                    //var Basis = document.getElementById("lblBasis+" + r).value;
                    var Basis = GetElem(document.getElementById("lblBasis+" + r));
                    var Chargeability = GetElem(document.getElementById("lblChargeability+" + r));
                    if (Basis == "M") 
                    {

                        if (Chargeability == "One Time") { }
                        else 
                        {
                            changeText(document.getElementById("lblPeriod+" + r), "");
                            changeText(document.getElementById("lblAmount+" + r), "");
                        }
                    }
                    else if (Basis == "S") 
                    {
                        if (Chargeability != "One Time") 
                        {
                            changeText(document.getElementById("lblPeriod+" + r), "");
                            changeText(document.getElementById("lblAmount+" + r), "");
                        }
                    }


                    var Chk = document.getElementById("Chk+" + r).checked;
                    if (Chk == true) 
                    {

                        var Amount = GetElem(document.getElementById("lblAmount+" + r));
                        if (Amount != "") 
                        {
                            NetAmount = parseFloat(NetAmount) + parseFloat(Amount);
                        }
                    }

                }
                DisplayAmount(NetAmount);
            }
        }

    }
    else 
    {
        term.value = "";
        alert("Please Enter SubscriptionEnd Date First !");

    }


}
using System;

namespace CommonLib.Financial
{
    //This class provides 7 overloaded static methods to convert Indian currency in to its corresponding words,  
    //it works only with 2 decimal point number and string(eg : "125.22"),
    //it returns "Zero Only**" for a negetive number,
    //and it returns string, in Capitalized Each Word Format.
    public class ChangeCurrencyToWords
    {
        private ChangeCurrencyToWords()
        {
        }    
        public static string changeCurrencyToWords(string amount)
        {
            try
            {
                return currencyInToWords(double.Parse(amount));
            }
            catch (Exception)
            {
                return "Invalid Amount";
            }
        }
        public static string changeCurrencyToWords(Int64 amount)
        {
            try
            {
                if (amount < 0)
                    return "Zero Only**";
                else
                    return currencyInToWords((double)amount);
            }
            catch (Exception)
            {
                return "Invalid Amount";
            }
        }
        public static string changeCurrencyToWords(Int64? amount)
        {
            try
            {
                if (amount < 0)
                    return "Zero Only**";
                else
                    return currencyInToWords((double)amount);
            }
            catch (Exception)
            {
                return "Invalid Amount";
            }
        }
        public static string changeCurrencyToWords(decimal amount)
        {
            try
            {
                if (amount < 0)
                    return "Zero Only**";
                else
                    return currencyInToWords((double)amount);
            }
            catch (Exception)
            {
                return "Invalid Amount";
            }
        }
        public static string changeCurrencyToWords(decimal? amount)
        {
            try
            {
                if (amount < 0)
                    return "Zero Only**";
                else
                    return currencyInToWords((double)(amount ?? 0));
            }
            catch (Exception)
            {
                return "Invalid Amount";
            }
        }
        public static string changeCurrencyToWords(double amount)
        {
            try
            {
                if (amount < 0)
                    return "Zero Only**";
                else
                    return currencyInToWords(amount);
            }
            catch (Exception)
            {
                return "Invalid Amount";
            }
        }
        public static string changeCurrencyToWords(double? amount)
        {
            try
            {
                if (amount < 0)
                    return "Zero Only**";
                else
                    return currencyInToWords(amount ?? 0);
            }
            catch (Exception)
            {
                return "Invalid Amount";
            }
        }

        private static string currencyInToWords(double amount)
        {
            string inWords, strAmount;
            strAmount = amount.ToString();
            Int64 rupees, paisa;
            if (amount == 0)
                return "Zero Only**";
            if (amount < 0)
                return "Zero Only**";
            try
            {
                if ((amount - (Int64)amount) == 0)
                    return NumberToWords((Int64)amount) + " Rupees Only**";
                if (strAmount.IndexOf('.') == -1)
                    return NumberToWords((Int64)amount) + " Rupees Only**";
                else
                {
                    if (strAmount.Length - (strAmount.IndexOf('.') + 1) == 1)
                    {
                        strAmount += "0";
                    }
                    if (strAmount.Length - (strAmount.IndexOf('.') + 1) > 2)
                    {
                        return "Invalid Amount";
                    }
                    rupees = Int64.Parse(strAmount.Substring(0, (strAmount.Length - 3)));
                    paisa = Int64.Parse(strAmount.Substring((strAmount.Length - 2)));
                    inWords = "" + NumberToWords(rupees) + " Rupees And " + NumberToWords(paisa) + " Paisa Only**";
                    return inWords;
                }
            }
            catch (Exception)
            {
                return "Invalid Amount";
            }
        }
        private static string NumberToWords(Int64 number)
        {
            string words = "";
            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }
            if (number > 0)
            {
                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += " " + tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }
    }
}
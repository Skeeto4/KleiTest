using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // we need system to access Enum parser

public class RomanNumeral : MonoBehaviour
{
    public enum RomanNumber
    {
        I = 1,
        V = 5,
        X = 10,
        L = 50,
        C = 100
    }
    public static int Convert(string roman_numeral)
    {
        int result = -1; // initialize the result variable, default is -1
        if (IsValid(roman_numeral)) // if the string was a valid roman number
        {
            result = 0; // set result to 0 to prepare for counting.
            RomanNumber[] romanNumbers = new RomanNumber[roman_numeral.Length];

            for (int i = 0; i < roman_numeral.Length; i++) // lets first parse all the string characters into an array of RomanNumbers (the enum we defined)
            {
                string character = roman_numeral[i].ToString();
                RomanNumber romanNumber = (RomanNumber)System.Enum.Parse(typeof(RomanNumber), character); // parse to enum
                romanNumbers[i] = romanNumber;
            }

            for (int i = 0; i < romanNumbers.Length; i++)
            {
                bool thereIsNext = i < (romanNumbers.Length - 1);
                RomanNumber currentNumber = romanNumbers[i];
                RomanNumber nextNumber = thereIsNext ? romanNumbers[i + 1] : default(RomanNumber);

                bool shouldBeAdded = true; // if false this number will be substracted from the result.

                if (thereIsNext && nextNumber > currentNumber)
                {
                    shouldBeAdded = false;
                }

                int currentValue = (int)currentNumber;
                result += (currentValue * (shouldBeAdded ? 1 : -1));
            }
        }
        return result;
    }

    // Optional Function, filters invalid strings.
    public static bool IsValid(string roman_numeral)
    {
        bool valid = !string.IsNullOrEmpty(roman_numeral); // start by assuming it is valid if there is characters in the string.
        if (!valid) { return valid; } // exit if the string is empty or null. 

        // here some of the patterns i see in roman numbers and what I consider must be true for a string to be consider a valid roman number:
        //  RULES:
        // 1) each character in the string has to match a known roman number 
        // 2) no more than 3 of the same number can be repeated consecutively 
        // 3) V, and L don't get repeated consecutively ever, and in common, those two star with 5
        // 4) the number 99 is written 'XCIX' not 'IC', 999 is 'CMXCIX' and not 'IMIC' nor 'XMIX', 49 is 'XLIX' and not 'IL', this is a harder to find rule, but i will separate it in two:
        //      4.1) Those numbers that start with 5 can't be used to subtract, there is no 'VX' or 'XVX', so those numbers can't be placed before a bigger number on the string.
        //      4.2) Those numbers that don't start with 5 like I, X, C, (all powers of 10 btw) can be used to substract only if the number they substract is smaller or equal to 10 times their value:
        //          Example: I = 1, it can substract anything that is smaller or equal than 10 times themselves = 10.  X = 10 can only be used to substract anything smaller or equal than 100. so XC is allowed while XD or XM isn't
        // 5) when substracting, the number that is negative can't have as a previous number the same number, for example, 'IV' is allowed, but 'IIV' isn't
        // [Reasoning for rule #6]  'LVIX', 'VIX' succeeds in all previous rules, but it is an invalid number, how to make a rule for this one?
        //     should be avlid: "LIV" and "CXLIV" but 'LVIX', 'VIX' should be invalid.   
        // 6)  when we find a number that starts with '5' (like V, L, D), no number(S) at the right side can ever be bigger. for example 'VIX' V is 5, so because we found X at the right side, this is an invalid number. 

        RomanNumber[] romanNumbers = new RomanNumber[roman_numeral.Length];

        // Parse all chars and apply RULE #1 
        for (int i = 0; valid && i < roman_numeral.Length; i++) // lets first parse all the string characters into an array of RomanNumbers (the enum we defined) 
        {
            string character = roman_numeral[i].ToString();
            RomanNumber romanNumber;
            valid &= System.Enum.TryParse<RomanNumber>(character, out romanNumber); // is valid if the character matches one of the known roman number defined in our enum.
            if (valid)
            {
                romanNumbers[i] = romanNumber;
            }
        }

        RomanNumber? previousNumber = null;
        RomanNumber? highestNumberWith5Found = null;
        int timesFoundConsecutively = 0;

        for (int i = 0; valid && i < romanNumbers.Length; i++) // for each roman number while it is valid
        {

            // define previous current and next number
            bool thereIsPrevious = i > 0;
            bool thereIsNext = i < (romanNumbers.Length - 1);
            RomanNumber currentNumber = romanNumbers[i];
            RomanNumber nextNumber = thereIsNext ? romanNumbers[i + 1] : default(RomanNumber);
            previousNumber = thereIsPrevious ? romanNumbers[i - 1] : default(RomanNumber);

            // define properties of the number we are evaluating.
            bool startsWithFive = ((int)currentNumber).ToString()[0] == '5'; // if the roman number we got is not one of those that start with 5, like V, L, D etc.
            bool canBeRepeated = !startsWithFive;
            bool isSubstracting = !startsWithFive && thereIsNext && nextNumber > currentNumber;
            timesFoundConsecutively = (previousNumber == currentNumber) ? (timesFoundConsecutively + 1) : 0; // increase 'timesFoundConsecutively' counter if the character is the same than the previous evalueated, else reset it to 0

            //Apply the rules to filter and determine if this is a valid number.
            valid &= (canBeRepeated && timesFoundConsecutively < 3) || (!canBeRepeated && timesFoundConsecutively == 0); // RULE #2 and #3 are filtered here. check if the repeated pattern is still valid.
            valid &= (!startsWithFive || !thereIsNext || (int)nextNumber < (int)currentNumber); // if the number we are evaluating starts with 5, we check for rule (4.1 mentioned above).
            valid &= (startsWithFive || !thereIsNext || (int)nextNumber <= (int)currentNumber * 10); // if the number starts with 5 leave valid untouch, else, check if there is a next, if so, this number is only valid if the next number is smaller or equal than my current number multiply by 10, its the rule 4.2

            if (isSubstracting && previousNumber != null) // if it is substracting and there is a previous number defined
            {
                valid &= (previousNumber != currentNumber); // apply rule #5, 'IIV' isn't valid
            }

            if (highestNumberWith5Found != null)
            {
                valid &= currentNumber < (RomanNumber)highestNumberWith5Found; // Apply rule #6 filter
            }

            if (startsWithFive)
            {
                highestNumberWith5Found = currentNumber;
            }
        }

        return valid;
    }

    private void Start()
    {
        string[] parse = new string[36]
        {
            "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX",
            "XXI", "XXII", "XXIII", "XXIV", "XXV", "XXVI", "XXVII", "XXVIII", "XXIX",
            "LXXI", "LXXII", "LXXIII", "LXXIV", "LXXV", "LXXVI", "LXXVII", "LXXVIII", "LXXIX",
            "IC", "XCIX", "IIV", "LVIX", "VIX", "LIV", "CXLIV", "CCLXXXVIII", "CCLXXI" // some extra to test edge cases
        };

        for(int i = 0; i < parse.Length; i++)
        {
            int result = Convert(parse[i]);
            string value = result == -1 ? "Invalid" : result.ToString();
            Debug.Log(parse[i] + " = " + value);
        }
    }   

}

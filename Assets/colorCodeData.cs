using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public static class colorCodeData {

    public static readonly Dictionary<char, string> colorPatternAlphabet = new Dictionary<char, string>()
    {
        {'A', "012" },
        {'B', "434" },
        {'C', "424" },
        {'D', "033" },
        {'E', "041" },
        {'F', "343" },
        {'G', "102" },
        {'H', "333" },
        {'I', "432" },
        {'J', "341" },
        {'K', "221" },
        {'L', "312" },
        {'M', "340" },
        {'N', "021" },
        {'O', "322" },
        {'P', "223" },
        {'Q', "111" },
        {'R', "131" },
        {'S', "302" },
        {'T', "000" },
        {'U', "030" },
        {'V', "313" },
        {'W', "023" },
        {'X', "011" },
        {'Y', "310" },
        {'Z', "133" }
        
    };

    public static readonly Dictionary<char, string> colorPatternNumbers = new Dictionary<char, string>()
    {
        {'1', "010" },
        {'2', "101" },
        {'3', "000" },
        {'4', "111" },
        {'5', "100" },
        {'6', "001" }
    };

    public static string generateLetterSequence(char ch)
    {
        string output = "";

        foreach (char unit in colorPatternAlphabet[ch])
        {
            switch (unit)
            {
                case '0':
                    output += "0";
                    break;
                case '1':
                    output += "1";
                    break;
                case '2':
                    output += "2";
                    break;
                case '3':
                    output += "3";
                    break;
                case '4':
                    output += "4";
                    break;
            }
        }

        return output;
    }

    public static string generateNumberSequence(char ch)
    {
        string output = "";

        foreach (char unit in colorPatternNumbers[ch])
        {
            switch (unit)
            {
                case '0':
                    output += "0";
                    break;
                case '1':
                    output += "1";
                    break;
            }
        }

        return output;
    }

}

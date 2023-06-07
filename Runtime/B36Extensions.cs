using System;
using HeathenEngineering.SteamworksIntegration;
using UnityEngine;

namespace WolfeyFPS
{
    public static class B36Extensions
    {
        public static string LobbyToBase36(this Lobby lobby)
        {
            return DecimalToArbitrarySystem(
                Convert.ToInt64(lobby.AccountId.m_AccountID),
                36);
        }

        public static Lobby Base36ToLobby(this string lobbyCode)
        {
            return Lobby.Get(Convert.ToUInt32(ArbitraryToDecimalSystem(lobbyCode, 36)));
        }

        public static string ULongToB36(this ulong num)
        {
            return DecimalToArbitrarySystem(Convert.ToInt64(num), 36);
        }

        public static ulong B36ToULong(this string str)
        {
            return Convert.ToUInt64(ArbitraryToDecimalSystem(str, 36));
        }
        
        public static long ArbitraryToDecimalSystem(string number, int radix)
        {
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " +
                                            Digits.Length.ToString());

            if (String.IsNullOrEmpty(number))
                return 0;

            // Make sure the arbitrary numeral system number is in upper case
            number = number.ToUpperInvariant();

            long result = 0;
            long multiplier = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                char c = number[i];
                if (i == 0 && c == '-')
                {
                    // This is the negative sign symbol
                    result = -result;
                    break;
                }

                int digit = Digits.IndexOf(c);
                if (digit == -1)
                    throw new ArgumentException(
                        "Invalid character in the arbitrary numeral system number",
                        "number");

                result += digit * multiplier;
                multiplier *= radix;
            }

            return result;
        }
        
        public static string DecimalToArbitrarySystem(long decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        public static void CopyToClipboard(this string str)
        {
            TextEditor textEditor = new TextEditor
            {
                text = str
            };
            
            textEditor.SelectAll();
            textEditor.Copy();
        }
    }
}

using System;
using System.Text;


public static class BigNumberFormatter
{
    /// <summary>
    /// BigNumber를 string으로 변환합니다.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="decimals">소수점 자리 수</param>
    /// <param name="alphaStartGroup">몇 번째 부터 알파벳 넘버 시작할지</param>
    /// <returns></returns>
    public static string ToString(BigNumber value,int decimals = 2,int alphaStartGroup = 5)
    {
        if (value.Mantissa == 0) return "0";

        bool isNegative = value.Mantissa < 0;
        double m = Math.Abs(value.Mantissa);

        double log10 = Math.Log10(m) + value.Exponent;

        if (log10 < 3)
        {
            double approx = value.ToDoubleClamped();
            string s = Math.Floor(Math.Abs(approx)).ToString("N0");
            return isNegative ? "-" + s : s;
        }

        int group = (int)Math.Floor(log10 / 3.0);

        double scaledLog10 = log10 - (group * 3.0);
        double scaled = Math.Pow(10, scaledLog10);

        string fmt = "F" + decimals;

        if (group < alphaStartGroup)
        {
            string suffix = ToShortSuffix(group);
            string s = scaled.ToString(fmt) + suffix;
            return isNegative ? "-" + s : s;
        }

        int alphaIndex = 27 + (group - alphaStartGroup);
        string alphaSuffix = ToAlphaSuffix(alphaIndex);

        string result = scaled.ToString(fmt) + alphaSuffix;
        return isNegative ? "-" + result : result;
    }


    #region Private Methods

    // 1K = 10^3, 1M = 10^6, 1B = 10^9, 1T = 10^12
    private static string ToShortSuffix(int group)
    {
        switch (group)
        {
            case 1: return "K";
            case 2: return "M";
            case 3: return "B";
            case 4: return "T";
            default: return "";
        }
    }

    // 1AA = 10^15, 1AB = 10^18...
    private static string ToAlphaSuffix(int index)
    {
        int n = index;
        StringBuilder sb = new StringBuilder();

        while (n > 0)
        {
            n -= 1;
            char c = (char)('A' + (n % 26));
            sb.Insert(0, c);
            n /= 26;
        }
        return sb.ToString();
    }

    #endregion
}

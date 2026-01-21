using System;


[Serializable]
public struct BigNumber : IComparable<BigNumber>, IEquatable<BigNumber>
{
    #region Fields

    public double Mantissa; // 유효숫자
    public int Exponent;    // 지수

    public static readonly BigNumber Zero = new BigNumber(0, 0);
    public static readonly BigNumber One = new BigNumber(1, 0);

    #endregion


    #region Constructor

    public BigNumber(double mantissa, int exponent)
    {
        Mantissa = mantissa;
        Exponent = exponent;
        Normalize(ref Mantissa, ref Exponent); // 정규화 해서 반환
    }

    public static BigNumber FromDouble(double value) // double에서 생성
    {
        if (value <= 0) return Zero;

        int exp = (int)Math.Floor(Math.Log10(value));
        double man = value / Math.Pow(10, exp);
        return new BigNumber(man, exp);
    }

    #endregion


    #region Helper Methods

    private static void Normalize(ref double mantissa, ref int exponent)
    {
        if (mantissa == 0 || double.IsNaN(mantissa) || double.IsInfinity(mantissa))
        {
            mantissa = 0;
            exponent = 0;
            return;
        }

        double abs = Math.Abs(mantissa);
        int shift = (int)Math.Floor(Math.Log10(abs)); // 먼저 while로 O(n)이 아닌 O(1) 연산

        mantissa = mantissa / Math.Pow(10, shift);
        exponent += shift;

        // mantissa가 10 이상이되거나 1 미만으로 떨어지면 지수 보정
        if (Math.Abs(mantissa) >= 10.0)
        {
            mantissa /= 10.0;
            exponent += 1;
        }
        else if (Math.Abs(mantissa) < 1.0 && mantissa != 0.0)
        {
            mantissa *= 10.0;
            exponent -= 1;
        }
    }

    #endregion


    #region Operation and Comparision

    // IComparable
    public int CompareTo(BigNumber other)
    {
        if (Mantissa == 0 && other.Mantissa == 0) return 0;
        if (Exponent != other.Exponent) return Exponent.CompareTo(other.Exponent);
        return Mantissa.CompareTo(other.Mantissa);
    }

    // IEquatable
    public bool Equals(BigNumber other) => Mantissa.Equals(other.Mantissa) && Exponent == other.Exponent;

    // Operators
    public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
    public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;
    public static bool operator >=(BigNumber a, BigNumber b) => a.CompareTo(b) >= 0;
    public static bool operator <=(BigNumber a, BigNumber b) => a.CompareTo(b) <= 0;
    public static BigNumber operator *(BigNumber a, BigNumber b)
    {
        if (a.Mantissa == 0 || b.Mantissa == 0) return Zero;
        return new BigNumber(a.Mantissa * b.Mantissa, a.Exponent + b.Exponent);
    }
    public static BigNumber operator *(BigNumber a, double b)
    {
        if (a.Mantissa == 0 || b == 0) return Zero;
        return new BigNumber(a.Mantissa * b, a.Exponent);
    }
    public static BigNumber operator +(BigNumber a, BigNumber b)
    {
        if (a.Mantissa == 0) return b;
        if (b.Mantissa == 0) return a;

        // 큰 쪽 기준으로 맞추기
        BigNumber big = a.Exponent >= b.Exponent ? a : b;
        BigNumber small = a.Exponent >= b.Exponent ? b : a;

        int diff = big.Exponent - small.Exponent;

        // 지수 차이가 크면 무시
        if (diff > 15) return big;

        // small을 big 지수에 맞춰 스케일 조정
        double scaledSmall = small.Mantissa / Math.Pow(10, diff);
        return new BigNumber(big.Mantissa + scaledSmall, big.Exponent);
    }
    /// <summary>
    /// 왼쪽 매개변수가 오른쪽보다 커야합니다. 그렇지 않으면 Zero를 반환합니다.
    /// </summary>
    public static BigNumber operator -(BigNumber a, BigNumber b)
    {
        if (b.Mantissa == 0) return a;
        if (a < b) return Zero;

        BigNumber big = a;
        BigNumber small = b;

        int diff = big.Exponent - small.Exponent;
        if (diff > 15) return big;

        double scaledSmall = small.Mantissa / Math.Pow(10, diff);
        return new BigNumber(big.Mantissa - scaledSmall, big.Exponent);
    }
    public static BigNumber Pow(BigNumber value, double power)
    {
        if (value.Mantissa == 0) return Zero;
        if (power == 0) return One;

        double log10 = Math.Log10(value.Mantissa) + value.Exponent;
        double newLog10 = log10 * power;

        int newExp = (int)Math.Floor(newLog10);
        double newMan = Math.Pow(10, newLog10 - newExp);

        return new BigNumber(newMan, newExp);
    }

    #endregion


    #region String Helper

    /// <summary>
    /// Clamp해서 Double로 변환합니다.
    /// </summary>
    /// <param name="max"></param>
    /// <returns>넘치면 max 작으면 0을 반환합니다.</returns>
    public double ToDoubleClamped(double max = 1e308)
    {
        if (Mantissa == 0) return 0;
        if (Exponent > 308) return max;
        if (Exponent < -308) return 0;
        return Mantissa * Math.Pow(10, Exponent);
    }

    #endregion
}

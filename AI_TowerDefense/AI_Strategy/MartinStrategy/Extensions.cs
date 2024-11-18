using System;

namespace AI_Strategy {
public static class Extensions
{
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0) return min;
        if(val.CompareTo(max) > 0) return max;
        return val;
    }
    
    public static int RoundToEven(this int number)
    {
        if (number.IsEven())
        {
            return number;
        }

        //round to the nearest even number
        return number > 0 ? number + 1 : number - 1;
    }
    
    public static bool IsEven(this int number)
    {
        return number % 2 == 0;
    }
}
}
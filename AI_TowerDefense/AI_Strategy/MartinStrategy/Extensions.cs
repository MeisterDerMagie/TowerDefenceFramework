using System;
using System.Collections.Generic;
using GameFramework;

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

    public static List<Soldier> GetSoldiers(this PlayerLane lane)
    {
        List<Soldier> soldiers = new();
        
        for (int w = 0; w < PlayerLane.WIDTH; w++)
        {
            for (int h = 0; h < PlayerLane.HEIGHT; h++)
            {
                Cell cell = lane.GetCellAt(w, h);
                if(cell.Unit is Soldier soldier) soldiers.Add(soldier);
            }
        }

        return soldiers;
    }
    
    public static List<Tower> GetTowers(this PlayerLane lane)
    {
        List<Tower> towers = new();
        
        for (int w = 0; w < PlayerLane.WIDTH; w++)
        {
            for (int h = 0; h < PlayerLane.HEIGHT; h++)
            {
                Cell cell = lane.GetCellAt(w, h);
                if(cell.Unit is Tower tower) towers.Add(tower);
            }
        }

        return towers;
    }
}
}
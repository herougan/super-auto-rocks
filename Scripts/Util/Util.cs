using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Util
{

    #region Util
    // public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    // {
    //     if (value.CompareTo(min) < 0)
    //         return min;
    //     if (value.CompareTo(max) > 0)
    //         return max;

    //     return value;
    // }
    public static int Clamp( int value, int min, int max )
    {
        return (value < min) ? min : (value > max) ? max : value;
    }
    public static (int, int) ColRow(int idx) {
        int c = 0, r = 0;

        r = idx % 3;
        c = idx / 3;

        return (c, r);
    }

    public static int Index(int col, int row) {
        return row + col * 3;
    }

    public static int FlipX(int idx) {
        (int c, int r) = ColRow(idx);
        return Index(2-c, r);
    }

    public static int FlipY(int idx) {
        (int c, int r) = ColRow(idx);
        return Index(c, 2-r);
    }

    public static int FlipXY(int idx) {
        (int c, int r) = ColRow(idx);
        return Index(2-c,2-r);
    }

    public static int RandomWeightedIndex(List<int> vector) {
        int sum = 0;
        for (int i = 0; i < vector.Count; ++i) {
            sum += vector[i];
        }

        int r = UnityEngine.Random.Range(0, sum);
        int n = 0;

        for (int i = 0; i < vector.Count; ++i) {
            r -= vector[i];
            if (r < 0) {
                break;
            }
            ++n;
        }
        return n;
    }

    #endregion Util

    #region Printing

    public static void PrintTurretNameList(List<TurretName> names) {
        string s = "";
        foreach (TurretName name in names) {
            s += name + ",\t";
        }
        Debug.Log(s);
    }

    public static void PrintIntList(List<int> integers) {
        string s = "";
        foreach (int integer in integers) {
            s += integer + ",\t";
        }
        Debug.Log(s);
    }

    #endregion Printing
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.WSA;

public struct QuatC
{
    #region FIELDS
     public float w;
     public float x;
     public float y;
     public float z;
    #endregion

    #region PROPIERTIES
    private static QuatC NullQ
    {
        get
        {
            QuatC a;
            a.w = 1;
            a.x = 0;
            a.y = 0;
            a.z = 0;
            return a;

        }
    }
    #endregion

    #region CONSTRUCTORS
    public QuatC(float w, float x, float y, float z)
    {
        this.w = w;
        this.x = x; 
        this.y = y; 
        this.z = z;

    }
    #endregion

    #region OPERATORS
    public static bool operator ==(QuatC a, QuatC b)
    {
        if (a.w == b.w)
        {
            if (a.x == b.x)
            {
                if (a.y == b.y)
                {
                    if (a.z == b.z)
                    {


                        return true;

                    }

                    return false;

                }

                return false;

            }
            else
                return false;


        }
        else
            return false;
    }

    public static bool operator !=(QuatC a, QuatC b)
    {
        if (a.w == b.w)
        {
            if (a.x == b.x)
            {
                if (a.y == b.y)
                {
                    if (a.z == b.z)
                    {


                        return true;

                    }

                    return false;

                }

                return false;

            }
            else
                return true;


        }
        else
            return true;
    }
    #endregion

    #region METHODS

    internal static QuatC Multiply(QuatC q1, QuatC q2)
    {

        QuatC result;
        result.w = (q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z);
        result.x = (q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y);
        result.y = (q1.w * q2.y + q1.x * q2.z + q1.y * q2.w - q1.z * q2.x);
        result.z = (q1.w * q2.z + q1.x * q2.y + q1.y * q2.x - q1.z * q2.w);

        return result;

    }

    internal QuatC Rotate(QuatC currentRotation, Vector3C axis, float angle)
    {

        QuatC result;

        result.w = (float)Math.Cos((angle / 2) * Math.PI / 180);
        result.x = axis.x * (float)Math.Sin((angle / 2) * Math.PI / 180);
        result.y = axis.y * (float)Math.Sin((angle / 2) * Math.PI / 180);
        result.z = axis.z * (float)Math.Sin((angle / 2) * Math.PI / 180);

        result = Multiply(currentRotation, result);

        return result;

    }

    internal float CalculateLerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    internal static QuatC Inverse(QuatC q1)
    {

        QuatC result;
        result.w = q1.w;
        result.x = -q1.x;
        result.y = -q1.y;
        result.z = -q1.z;

        return result;

    }

    #endregion

    #region FUNCTIONS
    #endregion

}

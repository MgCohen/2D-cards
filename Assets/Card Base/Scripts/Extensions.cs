using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Extensions
{

    public static void Kill(this GameObject obj)
    {
        GameObject.Destroy(obj);
    }

    public static char GetRandomChar(this string text, bool withCaps)
    {
        string mytext;
        if (withCaps)
        {
            mytext = "ABCDEFGHIJKLMNOPQRSTUVXZabcdefghijklmnopqrstuvxz";
        }
        else
        {
            mytext = "abcdefghijklmnopqrstuvxz";
        }

        var index = mytext.Length;
        var value = UnityEngine.Random.Range(0, index);
        return mytext[value];
    }

    public static Vector3 Unidirectional(this Vector3 dir)
    {
        var x = Mathf.Abs(dir.x);
        var y = Mathf.Abs(dir.y);
        var z = Mathf.Abs(dir.z);
        float d = 0;
        if (x > y && x > z)
        {
            d = dir.x;
            dir = new Vector3(1, 0, 0);
        }
        else if (y > x && y > z)
        {
            d = dir.y;
            dir = new Vector3(0, 1, 0);
        }
        else if (z > x && z > y)
        {
            d = dir.z;
            dir = new Vector3(0, 0, 1);
        }
        else
        {
            return Vector3.zero;
        }

        if (d < 0)
        {
            dir *= -1;
        }
        return dir;
    }

    public static Vector2 Unidirectional(this Vector2 dir)
    {
        var vec = new Vector3(dir.x, dir.y, 0).Unidirectional();
        return new Vector2(vec.x, vec.y);
    }

    public static Vector3 Directional(this Vector3 dir)
    {

        var x = 0f;
        var y = 0f;
        var z = 0f;
        if (dir.x != 0) x = dir.x / Mathf.Abs(dir.x);
        if (dir.y != 0) y = dir.y / Mathf.Abs(dir.y);
        if (dir.z != 0) z = dir.z / Mathf.Abs(dir.z);

        return new Vector3(x, y, z);

    }

    public static Vector2 Directional(this Vector2 dir)
    {
        var vec = new Vector3(dir.x, dir.y, 0).Directional();
        return new Vector2(vec.x, vec.y);
    }

    public static bool IsUnidirectional(this Vector3 dir)
    {
        bool state = false;
        if(dir.x != 0 && dir.y == 0 && dir.z == 0)
        {
            state = true;
        }
        else if (dir.x == 0 && dir.y != 0 && dir.z == 0)
        {
            state = true;
        }
        else if (dir.x == 0 && dir.y == 0 && dir.z != 0)
        {
            state = true;
        }
        return state;
    }

    public static bool IsUnidirectional(this Vector2 dir)
    {
        var newDir = new Vector3(dir.x, dir.y, 0);
        return IsUnidirectional(newDir);
    }

    public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector3 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }

    public static Bounds GetBounds(this Transform target)   
    {
        bool first = true;
        Bounds bound = new Bounds();
        var rend = target.GetComponent<Renderer>();
        if (rend)
        {
            bound = rend.bounds;
            first = false;
        }
        foreach (Transform child in target)
        {
            var rends = child.GetComponentsInChildren<Renderer>();
            foreach (var mRend in rends)
            {
                var tempBound = mRend.bounds;
                if (first)
                {
                    bound = tempBound;
                    first = false;
                }
                else
                {
                    bound.Encapsulate(tempBound);
                }
            }
        }
        return bound;
    }

    public static List<T> Clone<T>(this List<T> list)
    {
        var newList = new List<T>();
        foreach(var t in list)
        {
            newList.Add(t);
        }
        return newList;
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}

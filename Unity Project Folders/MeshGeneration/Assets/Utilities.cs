using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    static public int IndexBound(int a,int b)
    {
        return a - b * (int) System.Math.Floor((float)a / b);
    }
    
}

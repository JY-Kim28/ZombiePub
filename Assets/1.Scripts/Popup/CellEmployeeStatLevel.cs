using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellEmployeeStatLevel : MonoBehaviour
{
    [SerializeField] Color activeColor;
    [SerializeField] Color deactiveColor;

    [SerializeField] Image[] points;

    public void SetLv(int lv)
    {
        int size = points.Length;
        for (int i = 0; i < size; ++i)
        {
            points[i].color = lv > i ? activeColor : deactiveColor;
        }
    }
}

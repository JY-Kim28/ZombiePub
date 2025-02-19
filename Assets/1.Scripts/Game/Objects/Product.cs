using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour
{
    public ushort value = 1;// { private set; get; }

    public ProductScriptableObject Data;

    public float H;

    private void Awake()
    {
        var child = transform.GetChild(0);
        H = child.GetComponent<MeshFilter>().mesh.bounds.size.y;
    }

    public void SetValue(ushort value)
    {
        this.value = value;
    }
}

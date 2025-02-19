using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductData", menuName = "ScriptableObjects/ProductScriptableObject", order = 1)]
public class ProductScriptableObject : ScriptableObject
{
    public PRODUCT_TYPE Type;
    public ushort Idx;
}
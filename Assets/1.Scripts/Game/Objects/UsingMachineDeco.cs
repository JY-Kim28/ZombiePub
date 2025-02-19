using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingMachineDeco : MonoBehaviour
{
    [SerializeField] Transform productTransform;
    public Transform ProductTransform => productTransform;

    [SerializeField] Transform[] usingPoints;
    public Transform[] UsingPoints => usingPoints;


}

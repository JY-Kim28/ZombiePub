using UnityEngine;
using DG.Tweening;

public class TrashBin : ObjectBase
{
    private void Start()
    {
        objType = OBJ_TYPE.TrashBin;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Worker>(out Worker worker))
        {
            worker.RemoveAllProduct(transform.parent, transform.position);
        }
    }



    public override bool IsNeedWorker()
    {
        return true;
    }
}

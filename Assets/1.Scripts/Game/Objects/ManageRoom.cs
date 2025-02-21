using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageRoom : ObjectBase
{

    private void Awake()
    {
        objType = OBJ_TYPE.ManageRoom;
    }

    public override void Initialize()
    {
        gameObject.SetActive(true);

        if (collectMachine != null)
            collectMachine.gameObject.SetActive(false);

        foreach (var d in decorations)
        {
            d.gameObject.SetActive(false);
        }
    }


    public override void SetDecoration(ushort decoIdx)
    {
        //decorationIdx = currLv;

        decoIdx = currLv;

        if (decorations.Length != 0)
        {
            foreach (var d in decorations)
            {
                d.gameObject.SetActive(false);
            }

            if (decoIdx > decorations.Length) decoIdx = (ushort)decorations.Length;

            decorations[decoIdx].gameObject.SetActive(true);
        }

        ShowDecoLv(currLv);
    }

}

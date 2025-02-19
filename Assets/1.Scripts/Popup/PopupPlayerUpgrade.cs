using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UIPopup("PopupPlayerUpgrade")]
public class PopupPlayerUpgrade : UIPopupBase
{
    [SerializeField] CellPlayerUpgrade[] statCells;

    public override void Show()
    {
        base.Show();

    }
}

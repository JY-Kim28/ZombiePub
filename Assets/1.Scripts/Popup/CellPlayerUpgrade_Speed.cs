using System.Collections;
using System.Collections.Generic;
using UniRx;

public class CellPlayerUpgrade_Speed : CellPlayerUpgrade
{
    protected override void SetStat()
    {
        Game.Player.Stat.speedLv.Subscribe(OnDrawStatLv);
        OnDrawStatLv(Game.Player.Stat.speedLv.Value);
    }

    protected override uint GetLevelUpPrice()
    {
        return (uint)(Game.Player.Stat.speedLv.Value + 1) * 100;
    }

    protected override void LvUp()
    {
        (Game.Player.Stat as PlayerStat).LvUpSpeed();
    }
}

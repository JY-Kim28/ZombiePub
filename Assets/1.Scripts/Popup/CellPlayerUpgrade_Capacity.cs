using UniRx;

public class CellPlayerUpgrade_Capacity : CellPlayerUpgrade
{
    protected override void SetStat()
    {
        Game.Player.Stat.capacityLv.Subscribe(OnDrawStatLv);
    }

    protected override uint GetLevelUpPrice()
    {
        return (uint)(Game.Player.Stat.capacityLv.Value + 1) * 100;
    }

    protected override void LvUp()
    {
        (Game.Player.Stat as PlayerStat).LvUpCapacity();
    }
}

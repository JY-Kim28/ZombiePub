using UniRx;

public class CellPlayerUpgrade_Amount : CellPlayerUpgrade
{

    protected override void SetStat()
    {
        Game.Player.Stat.amountLv.Subscribe(OnDrawStatLv);
        OnDrawStatLv(Game.Player.Stat.amountLv.Value);
    }

    protected override uint GetLevelUpPrice()
    {
        return (uint)(Game.Player.Stat.amountLv.Value + 1) * 100;
    }

    protected override void LvUp()
    {
        (Game.Player.Stat as PlayerStat).LvUpAmount();
    }
}

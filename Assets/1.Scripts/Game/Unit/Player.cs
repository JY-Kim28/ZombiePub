using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Worker
{
    protected override void Awake()
    {
        base.Awake();

        Stat = new PlayerStat();
        //Stat.SetBaseStat(3f, 10, 10);
        Stat.SetBaseStat(startStat.speed, startStat.capacity, startStat.amount);
    }

    public void Move(Quaternion angle)
    {
        transform.rotation = Quaternion.AngleAxis(45 - angle.eulerAngles.z, Vector3.up);
        transform.Translate(Vector3.forward * Time.deltaTime * Stat.speed);

        animator?.SetBool("Move", true);
    }

    public void MoveStop()
    {
        animator?.SetBool("Move", false);
    }

}

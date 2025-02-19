using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MissionManager : MonoBehaviour
{

    [SerializeField] Transform arrowTR;

    [Header("Missions")]
    [Space()]
    [SerializeField] Mission[] missions;

    public int currIdx = 0;

    public void SetData(int idx)
    {
        currIdx = idx;

        HideArrow();

        Play();
    }

    public void Play()
    {
        if(missions.Length <= currIdx)
        {
            return;
        }

        HideArrow();

        missions[currIdx].Play(OnCompletedMission);
    }

    private void OnCompletedMission()
    {
        Game.UI.HideMissionText();
        HideArrow();

        ++currIdx;

        Play();
    }

    public void HideArrow()
    {
        arrowTR.gameObject.SetActive(false);
    }

    public void ShowArrow(Vector3 pos)
    {
        arrowTR.transform.position = pos;
        arrowTR.gameObject.SetActive(true);
    }
}
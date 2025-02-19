using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public int stageIdx;

    [SerializeField] Transform outPointTR;
    public Transform OutPointTR => outPointTR;

    public MissionManager mission;

    //public ushort StageIdx;

    public int currStep = 1;
    List<LevelUpData> currStepLvUpData = new List<LevelUpData>();
    Queue<LevelUpData> lvUpDatas = new Queue<LevelUpData>();

    public List<TrashBin> trashBins;
    public List<UsingMachine> usingMachines;


    public Product[] productPrefabs;
    public ObjectBase[] objectList;
    public ProductPile pile;


    public Employee[] employees;
    public int employeeLimit = 2;
    [SerializeField] public Transform employeePoint;


    public void LoadResources(Action progressCallback)
    {
        usingMachines = new List<UsingMachine>();

        employees = new Employee[employeeLimit];

        Root.Resources.LoadProduct(productPrefabs, progressCallback);
    }

    public void SetLevelData(LevelUpData[] data)
    {
        foreach (var obj in objectList)
        {
            obj.Initialize();
            obj.SetData(0, 0);
        }

        int missionIdx = 0;



        string loadTxt = Config.LoadTxt($"Stage{stageIdx}");
        if (loadTxt != "")
        {
            string[] loadDatas = loadTxt.Split("=============\n"); //player, step & mission, objects, employee

            #region Player

            string[] playerDatas = loadDatas[0].Split(",");

            Game.Player.transform.position = new Vector3(float.Parse(playerDatas[0]), 0, float.Parse(playerDatas[1]));

            if(float.TryParse(playerDatas[2], out float t))
            {
                if(t != 0)
                {
                    ushort idx = ushort.Parse(playerDatas[3]);
                    ushort count = ushort.Parse(playerDatas[4]);

                    ProductScriptableObject pData = productPrefabs[idx + 1].Data;

                    for (int i = 0; i < count; ++i)
                    {
                        Product product = Root.Resources.GetProduct(pData);
                        Game.Player.InputProduct(product);
                    }
                }
            }

            #endregion Player

            #region step & mission

            string[] stepNMission = loadDatas[1].Split("\n");
            currStep = int.Parse(stepNMission[0]);
            missionIdx = int.Parse(stepNMission[1]);

            #endregion step & mission

            #region objects
            string[] objectsData = loadDatas[2].Split("\n");

            ObjectBase objBase;

            for (int i = 0; i < objectsData.Length; ++i)
            {
                if (objectsData[i] == "") continue;

                string[] objData = objectsData[i].Split("=");

                objBase = objectList[int.Parse(objData[0])];
                objBase.SetSaveData(objData[1]);

                if(objBase.currLv != 0)
                {
                    if (objBase as UsingMachine)
                    {
                        usingMachines.Add(objBase as UsingMachine);
                    }
                }
            }
            #endregion objects

            #region Employee
            string[] employeeData = loadDatas[3].Split("\n");
            for(int i = 0; i < employeeData.Length; ++i)
            {
                if(ushort.TryParse(employeeData[i], out ushort lv))
                {
                    if(lv != 0)
                    {
                        CreateEmployee();

                        employees[i].Stat.SetLv(lv);
                    }
                }
            }
            #endregion Employee
        }

        if (missionIdx == 0)
        {
            pile.Show();
        }
        else
        {
            Destroy(pile.gameObject);
        }

        mission.SetData(missionIdx);

        foreach (var d in data)
        {
            if(d.step >= currStep)
                lvUpDatas.Enqueue(d);
        }

        SetCurrentState();
    }

    public void SetCurrentState()
    {
        if(lvUpDatas.Count == 0)
        {
            Debug.LogWarning("Stage Clear!!");
            return;
        }

        List<Transform> trs = new List<Transform>();

        ObjectBase objBase;

        while(lvUpDatas.Count != 0 && lvUpDatas.Peek().step == currStep)
        {
            var lvUpData = lvUpDatas.Dequeue();

            objBase = objectList[lvUpData.objIdx - 1];

            if (lvUpData.needPrice == 0)
            {
                if(objBase.currLv < lvUpData.objLv)
                {
                    objBase.LevelUp();
                }

                if (objBase as UsingMachine)
                {
                    usingMachines.Add(objBase as UsingMachine);
                }
            }
            else
            {
                currStepLvUpData.Add(lvUpData);

                objBase.ShowCollectMachine(lvUpData, OnCompleteLevelUp);

                if(objBase.collectMachine != null)
                {
                    trs.Add(objBase.collectMachine.transform);
                }
            }
        }


        Game.Instance.ShowTargetAsync(trs).Forget();

        SetNextStep();
    }

    public void SetNextStep()
    {
        if (currStepLvUpData.Count == 0)
        {
            Debug.Log("LV");

            ++currStep;

            SetCurrentState();
        }
    }

    private void OnCompleteLevelUp(LevelUpData obj)
    {
        objectList[obj.objIdx - 1].LevelUp();

        if (objectList[obj.objIdx - 1] as UsingMachine)
        {
            usingMachines.Add(objectList[obj.objIdx - 1] as UsingMachine);
        }

        currStepLvUpData.Remove(obj);

        SetNextStep();
    }

    public bool SetCustomerToUsingMachine(Customer customer)
    {
        foreach (var m in usingMachines)
        {
            if (m.currLv == 0) continue;

            if (m.SetCustomer(customer))
            {
                return true;
            }
        }

        return false;
    }

    public ProductScriptableObject GetMoneyData()
    {
        return productPrefabs[0].Data;
    }

    public ProductScriptableObject GetTrashData()
    {
        return productPrefabs[1].Data;
    }


    public void CreateEmployee()
    {
        if(employees[^1] == null)
        {
            Employee employee = Root.Resources.GetEmployee();
            employee.transform.SetParent(transform.parent);
            employee.transform.position = employeePoint.position;
            employee.gameObject.SetActive(true);
            employee.Initialize();

            for(int i = 0; i < employeeLimit; ++i)
            {
                if(employees[i] == null)
                {
                    employees[i] = employee;
                    break;
                }
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            StateSave();
        }
    }

    private void OnApplicationQuit()
    {
        StateSave();
    }

    private void StateSave()
    {
        string line = "=============\n";

        StringBuilder sb = new StringBuilder();

        #region Player

        int productType = 0;
        int productIdx = 0;
        int count = 0;
        if(Game.Player.GetProductData() != null)
        {
            productType = (int)Game.Player.GetProductData().Type;
            productIdx = (int)Game.Player.GetProductData().Idx;
            count = Game.Player.GetProductsCount();
        }
        sb.Append($"{Game.Player.transform.position.x},{Game.Player.transform.position.z},{productType},{productIdx},{count}");
        sb.AppendLine();

        #endregion Player


        # region current step & mission
        sb.Append(line);

        sb.Append(currStep);
        sb.AppendLine();

        sb.Append(mission.currIdx);
        sb.AppendLine();

        #endregion current step & mission


        #region objects
        sb.Append(line);

        for (int i = 0; i < objectList.Length; ++i)
        {
            sb.AppendJoin("=", new string[] { i.ToString(), objectList[i].GetSaveData() });
            sb.AppendLine();
        }

        #endregion objects


        #region Employee
        sb.Append(line);

        for(int i =0; i < employeeLimit; ++i)
        {
            if(employees[i] == null)
            {
                sb.Append(0);
            }
            else
            {
                sb.Append(employees[i].Stat.lv);
            }

            if(i < employeeLimit)
                sb.AppendLine();
        }

        #endregion Employee


        Debug.Log(sb.ToString());

        Config.SaveTxt($"Stage{stageIdx}", sb.ToString());
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

public class Resources : MonoBehaviour
{
    Queue<IEnumerator> _ResourceFunc;
    int _TotalResourceLoadCount;
    Action<float> _ResourceLoadProgressCallback;


    public enum ATLAS
    {
        UI
    }

    Dictionary<ATLAS, SpriteAtlas> _AtlasDic = new Dictionary<ATLAS, SpriteAtlas>();

    #region Pool
    Dictionary<ProductScriptableObject, ObjectPool<Product>> productPoolDic = new Dictionary<ProductScriptableObject, ObjectPool<Product>>();
    Dictionary<int, ObjectPool<Customer>> customerPoolDic = new Dictionary<int, ObjectPool<Customer>>();
    ObjectPool<Employee> employeePool;
    #endregion Pool


    public void LoadResource(Action<float> progressCallback)
    {
        _ResourceLoadProgressCallback = progressCallback;

        _ResourceFunc = new Queue<IEnumerator>();
        _ResourceFunc.Enqueue(LoadAtlas());
        _ResourceFunc.Enqueue(LoadCustomer());
        _ResourceFunc.Enqueue(LoadEmployee());

        _TotalResourceLoadCount = _ResourceFunc.Count;

        LoadResources();
    }

    private IEnumerator LoadCustomer()
    {
        int max = Enum.GetValues(typeof(CUSTOMER_TYPE)).Length;

        for (int i = 1; i < max; ++i)
        {
            var handler = Addressables.LoadAssetAsync<GameObject>($"Customer{i}");
            while (handler.IsDone == false)
            {
                yield return null;
            }

            //GameObject customer = Instantiate(handler.Result);
            //customer.SetActive(false);

            var pool = new ObjectPool<Customer>(() => Instantiate(handler.Result.GetComponent<Customer>()));
            customerPoolDic.Add(i, pool);
        }

        LoadResources();
    }

    private IEnumerator LoadEmployee()
    {
        var handler = Addressables.LoadAssetAsync<GameObject>("Employee");
        while (handler.IsDone == false)
        {
            yield return null;
        }

        handler.Result.gameObject.SetActive(false);

        employeePool = new ObjectPool<Employee>(() => Instantiate(handler.Result.GetComponent<Employee>()));

        LoadResources();
    }

    private void LoadResources()
    {
        if (_ResourceFunc.Count != 0)
        {
            _ResourceLoadProgressCallback?.Invoke(_TotalResourceLoadCount - _ResourceFunc.Count / (float)_TotalResourceLoadCount);

            StartCoroutine(_ResourceFunc.Dequeue());
        }
        else
        {
            _ResourceLoadProgressCallback?.Invoke(1);
        }
    }

    private IEnumerator LoadAtlas()
    {
        string[] atlasNames = Enum.GetNames(typeof(ATLAS));
        int max = atlasNames.Length;

        for (int i = 0; i < max; ++i)
        {
            var handler = Addressables.LoadAssetAsync<SpriteAtlas>($"Atlas_{atlasNames[i]}");
            while (handler.IsDone == false)
            {
                yield return null;
            }

            _AtlasDic.Add((ATLAS)i, handler.Result);
        }

        LoadResources();
    }

    public Sprite GetSprite(ATLAS atlas, string spriteName)
    {
        return _AtlasDic[atlas].GetSprite(spriteName);
    }



    #region Load Pool

    public void LoadProduct(Product[] objs, Action completeCallback)
    {
        ObjectPool<Product> pool;

        int max = objs.Length;

        for (int i = 0; i < max; ++i)
        {
            if (productPoolDic.ContainsKey(objs[i].Data) == false)
            {
                GameObject obj = Instantiate(objs[i].gameObject);
                obj.SetActive(false);

                pool = new ObjectPool<Product>(() => Instantiate(obj).GetComponent<Product>());

                productPoolDic.Add(objs[i].Data, pool);
            }
        }

        completeCallback?.Invoke();
    }


    #endregion Load Pool

    public Customer GetCustomer(CUSTOMER_TYPE type)
    {
        return customerPoolDic[(int)type].Get();
    }

    public void ReleaseCustomer(Customer customer)
    {
        customer.gameObject.SetActive(false);
        customer.transform.SetParent(transform);
        customerPoolDic[(int)customer.type].Release(customer);
    }


    public Employee GetEmployee()
    {
        return employeePool.Get();
    }

    public void ReleaseEmployee(Employee employee)
    {
        employee.gameObject.SetActive(false);
        employee.transform.SetParent(transform);
        employeePool.Release(employee);
    }


    public Product GetProduct(ProductScriptableObject data)
    {
        if (productPoolDic.ContainsKey(data))
        {
            return productPoolDic[data].Get();
        }
        return null;
    }

    public void ReleaseProduct(Product product)
    {
        product.gameObject.SetActive(false);
        product.transform.SetParent(transform);
        productPoolDic[product.Data].Release(product);
    }


    #region Load CSV
    public LevelUpData[] LoadStage(int stage)
    {
        var csv = Addressables.LoadAssetAsync<TextAsset>($"StageData/{stage}.csv").WaitForCompletion();

        string text = csv.text.Replace("\r\n", "\n"); //git에 csv 업로드 시 \r\n이 \n으로 텍스트가 변환되므로 미리 다 바꿔놓기

        string[] lvUpDatas = text.Split('\n');

        int size = lvUpDatas.Length;

        LevelUpData[] levelUpDatas = new LevelUpData[size];

        LevelUpData lvData;
        for (int i = 0; i < size; ++i)
        {
            lvData = new LevelUpData();
            lvData.SetData(lvUpDatas[i]);
            levelUpDatas[i] = lvData;
        }

        return levelUpDatas;
    }

    #endregion Load CSV
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerMaker : ObjectBase
{
    float makeTime = 3;
    float time = 0;


    private void Update()
    {
        if (currLv == 0) return;

        if (makeTime != 0)
        {
            time -= Time.deltaTime;

            if (time <= 0)
            {
                time = makeTime;

                CreateCustomer();
            }
        }
    }

    private void CreateCustomer()
    {
        Customer customer = Root.Resources.GetCustomer();
        customer.transform.SetParent(transform.parent);
        customer.transform.position = transform.position;
        customer.gameObject.SetActive(true);

        customer.Create();
    }


}

using UnityEngine;

public class CustomerMaker : ObjectBase
{
    public CUSTOMER_TYPE customerType;
    public SellingMachine[] targetSellings;
    public Transform inTr;
    public Transform outTr;

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
        foreach(var sell in targetSellings)
        {
            if(sell.IsCustomerFull() == false)
            {
                Customer customer = Root.Resources.GetCustomer(customerType);
                customer.transform.SetParent(Game.Stage.customerContainerTR);
                customer.transform.position = inTr.position;
                customer.gameObject.SetActive(true);

                customer.SetOutPoint(outTr);
                customer.Create();

                break;
            }
        }

    }


}

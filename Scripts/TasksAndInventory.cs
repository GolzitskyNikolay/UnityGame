using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasksAndInventory : MonoBehaviour
{
    public class Object
    {
        public bool currentState = false;
        public GameObject gameObject;
    }

    public GameObject tasks;
    public GameObject inventory;

    private Object tasksObj = new Object();
    private Object inventoryObj = new Object();

    void Start()
    {
        tasksObj.gameObject = tasks;
        inventoryObj.gameObject = inventory;

        tasksObj.gameObject.SetActive(false);
        inventoryObj.gameObject.SetActive(false);
    }

    public void SetTasksOrInventory(GameObject objectToHideOrShow)
    {
        if (objectToHideOrShow == tasks)
        {
            if (!tasksObj.currentState)
            {
                if (inventoryObj.currentState)
                {
                    inventoryObj.currentState = false;
                    inventoryObj.gameObject.SetActive(false);

                }
                tasksObj.gameObject.SetActive(true);
                tasksObj.currentState = true;
            }
            else
            {
                tasksObj.gameObject.SetActive(false);
                tasksObj.currentState = false;
            }
        }

        else
        {
            if (!inventoryObj.currentState)
            {
                if (tasksObj.currentState)
                {
                    tasksObj.currentState = false;
                    tasksObj.gameObject.SetActive(false);
                }
                inventoryObj.gameObject.SetActive(true);
                inventoryObj.currentState = true;
            }
            else
            {
                inventoryObj.gameObject.SetActive(false);
                inventoryObj.currentState = false;
            }
        }
    }
}

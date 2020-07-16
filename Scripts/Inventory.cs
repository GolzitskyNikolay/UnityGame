using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Inventory : MonoBehaviour
{
    public RectTransform inventoryContent;
    public int objectsCount = 0;
    public int objectsMaxCount = 9;

    public void AddItemIfPossible(InvItem myItem)
    {
        bool isAdded = false;

        //columns in inventoty
        foreach (Transform column in inventoryContent)
        {
            foreach (Transform item in column)
            {
                if (item.GetComponentInChildren<Text>() == null)
                {
                    myItem.GetComponentInChildren<Text>().fontSize = 12;
                    myItem.SetCanThrought(false);
                    myItem.SetInInventory(true);
                    myItem.transform.SetParent(item, false);

                    myItem.transform.localPosition = myItem.GetStartPosition();
                    myItem.inventory.objectsCount++;
                    isAdded = true;
                    break;
                }
            }
            if (isAdded) break;
        }

        if (!isAdded)
        {
            StartCoroutine(NotEnoughfSpace());
        }
    }

    IEnumerator NotEnoughfSpace()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Message");
        Text message = go.transform.GetComponent<Text>();
        string warning = "В рюкзаке нет места!";
        message.text = warning;
        yield return new WaitForSeconds(2);

        message.text = "";
    }

    public void Scale(Transform transform)
    {
        StartCoroutine(ForScale(transform));
    }

    IEnumerator ForScale(Transform transform)
    {

        Vector3 scaleMin = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 scaleMax = new Vector3(transform.position.x * 1.5f, transform.position.x * 1.5f, transform.position.z * 1.5f);


        float timeToMin = 1;
        float timeToMax = 1;

        var startScale = transform.localScale;

        float elapsedTime = 0.0f;

        while ((elapsedTime += Time.deltaTime) <= timeToMax)
        {
            transform.localScale = Vector3.Lerp(startScale, scaleMax, elapsedTime / timeToMax);

            yield return null;
        }

        startScale = transform.localScale;

        elapsedTime = 0.0f;

        while ((elapsedTime += Time.deltaTime) <= timeToMin)
        {
            transform.localScale = Vector3.Lerp(startScale, scaleMin, elapsedTime / timeToMin);

            yield return null;
        }

        transform.localScale = scaleMin;
    }
}

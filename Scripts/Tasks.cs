using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Tasks : MonoBehaviour
{
    private ArrayList tasks = new ArrayList();

    public RectTransform content;
    public RectTransform prefab;

    public void AddTask(string text)
    {
        var newTask = GameObject.Instantiate(prefab.gameObject) as GameObject;
        newTask.transform.SetParent(content, false);
        newTask.GetComponentInChildren<Text>().text = text;
        tasks.Add(text);
    }

    public void DeleteTask(string text)
    {
        foreach (Transform child in content)
        {
            if (child.GetComponentInChildren<Text>().text.Equals(text))
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }


}


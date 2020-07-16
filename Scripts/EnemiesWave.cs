using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesWave : MonoBehaviour
{

    public GameObject enemyGirl;

    private int maxEnemiesCount = 0;
    private int currentEnemiesCount;
    private Transform player;

    public void SetCurrentEnemiesCount()
    {
        this.currentEnemiesCount--;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentEnemiesCount = maxEnemiesCount;
    }


    // Update is called once per frame
    void Update()
    {
        if (currentEnemiesCount == 0)
        {
            StartNewVawe();
        }
    }

    void StartNewVawe() {

        GameObject parent = GameObject.FindGameObjectWithTag("GameWorld");

        if (currentEnemiesCount == 0)
        {
            maxEnemiesCount += 2;
            currentEnemiesCount = maxEnemiesCount;

            for (int i = 1; i <= maxEnemiesCount; i++)
            {
                Instantiate(enemyGirl, parent.transform);
            }
        }
    }

}

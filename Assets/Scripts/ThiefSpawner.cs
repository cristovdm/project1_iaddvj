using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefSpawner : MonoBehaviour
{
    public GameObject thiefPrefab;
    public Vector2 spawnPosition = new Vector2(-31.6f, -18.38f);
    public float minRespawnTime = 20f;
    public float maxRespawnTime = 60f;

    private GameObject currentThief;

    void Start()
    {
        SpawnThief();
    }

    void SpawnThief()
    {
        if (currentThief != null)
        {
            Destroy(currentThief);
        }
        currentThief = Instantiate(thiefPrefab, spawnPosition, Quaternion.identity);
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minRespawnTime, maxRespawnTime));
            if (currentThief == null)
            {
                SpawnThief();
            }
        }
    }
}

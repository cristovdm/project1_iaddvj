using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefSpawner : MonoBehaviour
{
    public GameObject thiefPrefab;
    private Vector2 spawnPosition = new Vector2(-25f, 30f);
    public float minRespawnTime = 60f;
    public float maxRespawnTime = 150f;

    private GameObject currentThief;

    void Start()
    {
        if (Day.Instance.GetCurrentDay() >= 7)
        {
            SpawnThief();
        }
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

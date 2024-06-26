using System.Collections;
using UnityEngine;

public class SeaUrchinSpawner : MonoBehaviour
{
    public GameObject seaUrchinPrefab;
    public Vector2 spawnPosition = new Vector2(20f, 12f);
    private GameObject currentSeaUrchin;

    void Start()
    {
        SeaUrchin.OnSeaUrchinDestroyed += HandleSeaUrchinDestroyed;
        //SpawnSeaUrchin();
    }

    void OnDestroy()
    {
        SeaUrchin.OnSeaUrchinDestroyed -= HandleSeaUrchinDestroyed;
    }

    void SpawnSeaUrchin()
    {
        currentSeaUrchin = Instantiate(seaUrchinPrefab, spawnPosition, Quaternion.identity);
    }

    void HandleSeaUrchinDestroyed()
    {
        currentSeaUrchin = null;
        StartCoroutine(RespawnSeaUrchin());
    }

    IEnumerator RespawnSeaUrchin()
    {
        yield return new WaitForSeconds(30f);
        if (currentSeaUrchin == null)
        {
            SpawnSeaUrchin();
        }
    }
}

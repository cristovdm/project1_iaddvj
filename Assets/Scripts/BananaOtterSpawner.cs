using System.Collections;
using UnityEngine;

public class BananaOtterSpawner : MonoBehaviour
{
    public GameObject bananaOtterPrefab;
    private Vector2 spawnPosition = new Vector2(-34.0f, -1.0f);
    private GameObject currentBananaOtter;
    public Vector3 bananaOtterScale = new Vector3(3f, 3f, 3f);
    public float minX, minY, maxX, maxY;

    void Start()
    {

        if (Day.Instance.GetCurrentDay() >= 6)
        {
            BananaOtter.OnBananaOtterDestroyed += HandleBananaOtterDestroyed;
            currentBananaOtter = Instantiate(bananaOtterPrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(SpawnBananaOtterRoutine());
        }
    }

    void OnDestroy()
    {
        BananaOtter.OnBananaOtterDestroyed -= HandleBananaOtterDestroyed;
    }

    void SpawnBananaOtter()
    {
        if (currentBananaOtter == null)
        {
            // Instantiate currentBananaOtter if it's null
            currentBananaOtter = Instantiate(bananaOtterPrefab, spawnPosition, Quaternion.identity);
        }

        currentBananaOtter.transform.localScale = bananaOtterScale;
        BananaOtter bananaOtterScript = currentBananaOtter.GetComponent<BananaOtter>();
        if (bananaOtterScript != null)
        {
            bananaOtterScript.minX = minX;
            bananaOtterScript.minY = minY;
            bananaOtterScript.maxX = maxX;
            bananaOtterScript.maxY = maxY;
        }
    }

    IEnumerator SpawnBananaOtterRoutine()
    {
        while (true)
        {
            if (currentBananaOtter == null)
            {
                float delay = Random.Range(20f, 180f);
                yield return new WaitForSeconds(delay);
                if (currentBananaOtter == null)
                {
                    SpawnBananaOtter();
                }
            }
            yield return null;
        }
    }

    void HandleBananaOtterDestroyed()
    {
        currentBananaOtter = null;
        StartCoroutine(RespawnBananaOtter());
    }

    IEnumerator RespawnBananaOtter()
    {
        float delay = Random.Range(10f, 60f);
        yield return new WaitForSeconds(delay);
        if (currentBananaOtter == null)
        {
            SpawnBananaOtter();
        }
    }
}

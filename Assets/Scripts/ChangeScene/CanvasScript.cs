using UnityEngine;
public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowCanvas()
    {
        gameObject.SetActive(true);
    }
}
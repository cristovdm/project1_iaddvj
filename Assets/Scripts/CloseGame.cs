using UnityEngine;

public class CloseGame : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
            #if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
            #else
                        Application.Quit();
            #endif
                        }
        }
    }
}

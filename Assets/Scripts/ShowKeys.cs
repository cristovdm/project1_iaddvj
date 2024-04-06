using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowKeys : MonoBehaviour
{
    public float displayDuration = 2f; // How long the text will be displayed
    [SerializeField] private int value = 100;
    [SerializeField] private GameObject floatingTextPrefab;


    private void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    string keyName = GetKeyName(keyCode);
                    if (!string.IsNullOrEmpty(keyName))
                    {
                        ShowText(keyName);
                    }
                }
            }
        }
    }

    private string GetKeyName(KeyCode keyCode)
    {
        if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
        {
            return keyCode.ToString();
        }
        else if (keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9)
        {
            return keyCode.ToString().Substring(5);
        }
        else if (keyCode >= KeyCode.Keypad0 && keyCode <= KeyCode.Keypad9)
        {
            return keyCode.ToString().Substring(7);
        }
        else if (keyCode == KeyCode.UpArrow || keyCode == KeyCode.DownArrow || keyCode == KeyCode.LeftArrow || keyCode == KeyCode.RightArrow)
        {
            return keyCode.ToString().Replace("Arrow", "");
        }
        else
        {
            return string.Empty;
        }
    }

    private void ShowText(string key)
    {
        if (floatingTextPrefab) { 
        GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        prefab.GetComponentInChildren<TextMesh>().text = key;
        Destroy(prefab.gameObject, displayDuration);
        }
    }
}

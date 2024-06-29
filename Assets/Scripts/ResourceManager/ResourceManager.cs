using Inventory.Model;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ResourceManager : MonoBehaviour
{
    private static Dictionary<string, EdibleItemSO> itemDictionary = new Dictionary<string, EdibleItemSO>();

    public static EdibleItemSO LoadItem(string itemName)
    {
        EdibleItemSO item = null;
        if (itemDictionary.TryGetValue(itemName, out item))
        {
            return item;
        }
        else
        {
            Debug.LogWarning("El objeto con nombre " + itemName + " no fue encontrado.");
            return null;
        }
    }

    public static T LoadResource<T>(string resourceName) where T : Object
    {
        T loadedResource = null;

#if UNITY_EDITOR
        string assetPath = "Assets/Data/" + resourceName + ".asset";
        loadedResource = AssetDatabase.LoadAssetAtPath<T>(assetPath);
#endif

        if (loadedResource == null)
        {
            Debug.LogError("Failed to load resource: " + resourceName);
            return null;
        }

        return loadedResource;
    }
}

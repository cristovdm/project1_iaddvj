using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        string assetPath = "Assets/Data/" + resourceName + ".asset";

        T loadedResource = AssetDatabase.LoadAssetAtPath<T>(assetPath);

        if (loadedResource == null)
        {
            Debug.LogError("Failed to load resource: " + resourceName);
            return null;
        }

        return loadedResource;
    }
}
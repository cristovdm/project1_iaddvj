using UnityEngine;
using System.Collections.Generic;
using Inventory.Model;

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
            Debug.LogWarning("El objeto con nombre " + itemName + " no fue encontrado en el diccionario.");
            return null;
        }
    }

    public static T LoadResource<T>(string resourceName) where T : Object
    {
        T loadedResource = null;

        if (itemDictionary.ContainsKey(resourceName))
        {
            return itemDictionary[resourceName] as T;
        }

        loadedResource = Resources.Load<T>("Data/" + resourceName);

        if (loadedResource == null)
        {
            Debug.LogError("Failed to load resource: " + resourceName);
        }
        else
        {
            itemDictionary.Add(resourceName, loadedResource as EdibleItemSO);
        }

        return loadedResource;
    }
}

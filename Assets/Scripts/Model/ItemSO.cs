using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory.Model{
   [CreateAssetMenu]

   public class ItemSO : ScriptableObject
   {
      [field: SerializeField]
      public bool isStackable {get; set; }

      public int ID => GetInstanceID(); 

      [field: SerializeField]

      public int MaxStackSize {get; set;} = 1; 

      [field: SerializeField]

      public string Name {get; set;}


      [field: SerializeField]

      public Sprite ItemImage {get; set; }

   }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EdibleItemSO : ItemSO, IDestroyableItem
    {


        [field: SerializeField]
        public AudioClip actionSFX {get; private set;}

    }

    public interface IDestroyableItem
    {

    }

    public interface IItemAction
    {
        public string ActionName { get; }
        public AudioClip actionSFX { get; }
    }

}
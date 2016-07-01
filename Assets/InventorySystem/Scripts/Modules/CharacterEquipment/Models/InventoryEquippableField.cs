using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem.Models
{
    using System.Linq;

    [RequireComponent(typeof(InventoryUIItemWrapper))]
    [AddComponentMenu("InventorySystem/UI Helpers/Equippable field")]
    public partial class InventoryEquippableField : MonoBehaviour
    {
        /// <summary>
        /// Index of this field
        /// </summary>
        [HideInInspector]
        public uint index
        {
            get
            {
                return itemWrapper.index;
            }
        }

        [SerializeField]
        [HideInInspector]
        public int[] _equipTypes;
        public InventoryEquipType[] equipTypes
        {
            get
            {
                InventoryEquipType[] types = new InventoryEquipType[_equipTypes.Length];
                for (int i = 0; i < types.Length; i++)
                {
                    types[i] = ItemManager.database.equipTypes.FirstOrDefault(o => o.ID == _equipTypes[i]);
                }

                return types;
            }
        }

        private CharacterUI _characterCollection;
        public CharacterUI characterCollection
        {
            get
            {
                if (_characterCollection == null)
                {
                    _characterCollection = GetComponentsInParent<CharacterUI>(true).FirstOrDefault();
                    Assert.IsNotNull(_characterCollection, "CharacterUI couldn't be found in parent. Equippable field error!");
                }

                return _characterCollection;
            }
        }

        private InventoryUIItemWrapperBase _itemWrapper;
        public InventoryUIItemWrapperBase itemWrapper
        {
            get
            {
                if (_itemWrapper == null)
                    _itemWrapper = GetComponent<InventoryUIItemWrapperBase>();

                return _itemWrapper;
            }
        }


        protected void Awake()
        {
            
        }

        protected void Start()
        {
            
        }
    }
}
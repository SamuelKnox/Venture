using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Models;
using UnityEngine.UI;

namespace Devdog.InventorySystem.UI
{
    /// <summary>
    /// Used to define a category of stats.
    /// </summary>
    public partial class InventoryCharacterStatCategoryUI : MonoBehaviour, IPoolableObject
    {
        /// <summary>
        /// Name of the category
        /// </summary>
        [SerializeField]
        protected Text categoryName;

        [SerializeField]
        protected Button foldButton;

        [InventoryRequired]
        public RectTransform container;

        /// <summary>
        /// Check if the item is folded or not.
        /// </summary>
        protected bool isVisible = true;

        private LayoutElement containerLayoutElement;


        protected virtual void Awake()
        {
            containerLayoutElement = container.GetComponent<LayoutElement>();
            if (containerLayoutElement == null)
            {
                containerLayoutElement = container.gameObject.AddComponent<LayoutElement>();
            }

            if(foldButton != null)
            {
                foldButton.onClick.AddListener(OnFoldButtonClicked);
            }
        }

        protected virtual void OnFoldButtonClicked()
        {
            isVisible = !isVisible;
            containerLayoutElement.gameObject.SetActive(isVisible);
        }

        public virtual void Repaint(string categoryName)
        {
            this.categoryName.text = categoryName;
        }

        public virtual void Reset()
        {
            isVisible = true;
        }
    }
}
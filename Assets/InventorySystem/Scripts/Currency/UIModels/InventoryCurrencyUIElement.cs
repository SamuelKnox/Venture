using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.InventorySystem.Models
{
    [Serializable]
    [AddComponentMenu("InventorySystem/UI/Inventory Currency UI Element")]
    public struct InventoryCurrencyUIElement
    {
        [Header("Options")]
        [SerializeField]
        private bool overrideStringFormat;

        [SerializeField]
        private string overrideStringFormatString;

        [SerializeField]
        private bool hideWhenEmpty;

        [Header("Audio & Visuals")]
        [SerializeField]
        private Text amount;

        [SerializeField]
        private Image icon;

        

        public void Repaint(InventoryCurrencyLookup lookup)
        {
            SetActive(true);

            if (amount != null)
            {
                if(lookup.amount <= 0f && hideWhenEmpty)
                {
                    amount.gameObject.SetActive(false);
                }

                amount.text = lookup.ToString(1.0f, overrideStringFormat ? overrideStringFormatString : "");
            }

            if (icon != null)
            {
                if(lookup.amount <= 0f && hideWhenEmpty)
                {
                    icon.gameObject.SetActive(false);
                }

                icon.sprite = lookup.currency.icon;
            }
        }

        public void Reset()
        {
            if (amount != null)
                amount.text = "0";

            if(hideWhenEmpty)
            {
                SetActive(false);
            }
        }

        private void SetActive(bool active)
        {
            if (amount != null)
                amount.gameObject.SetActive(active);

            if (icon != null)
                icon.gameObject.SetActive(active);
        }
    }
}

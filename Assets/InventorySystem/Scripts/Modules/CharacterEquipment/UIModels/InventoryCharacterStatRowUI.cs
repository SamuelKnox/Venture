using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Models;
using UnityEngine.UI;

namespace Devdog.InventorySystem.UI
{
    /// <summary>
    /// Used to define a row of stats.
    /// </summary>
    public partial class InventoryCharacterStatRowUI : MonoBehaviour, IPoolableObject
    {
        [SerializeField]
        protected Text statName;

        [SerializeField]
        protected Text statValue;

        [SerializeField]
        protected Image statIcon;

        public IInventoryCharacterStat currentStat { get; protected set; }
        public bool hideStatNameIfIconIsPresent = false;

        public virtual void Repaint(IInventoryCharacterStat characterStat)
        {
            currentStat = characterStat;
            if (statName != null)
            {
                statName.text = currentStat.statName;
                statName.color = currentStat.color;

                statName.gameObject.SetActive(true);
            }

            if (statValue != null)
            {
                statValue.text = currentStat.ToString();
                statValue.color = currentStat.color;
            }

            if (statIcon != null)
            {
                statIcon.sprite = currentStat.icon;
                statIcon.color = currentStat.color;
                statIcon.gameObject.SetActive(true);

                if (currentStat.icon == null)
                {
                    statIcon.gameObject.SetActive(false);
                }
                else
                {
                    if (hideStatNameIfIconIsPresent && statName != null)
                    {
                        statName.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void Reset()
        {
            if (statName != null)
            {
                statName.gameObject.SetActive(false);
            }

            if (statValue != null)
            {
                statValue.text = string.Empty;
                statValue.color = Color.white;
            }

            if (statIcon != null)
            {
                statIcon.gameObject.SetActive(false);
            }
        }
    }
}
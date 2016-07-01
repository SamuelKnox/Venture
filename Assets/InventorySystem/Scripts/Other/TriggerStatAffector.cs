using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Models;


namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Actions/Trigger Stat Affector")]
    public class TriggerStatAffector : MonoBehaviour
    {
        public delegate void Change(InventoryPlayer player, IInventoryCharacterStat stat);
        public event Change OnEnter;
        public event Change OnStay;
        public event Change OnExit;


        [Header("Stat")]
        public InventoryItemPropertyReference property;

        [Header("Enter")]
        public bool changeOnEnter = true;
        public float enterChangeAmount = 10f;
        
        [Header("Stay")]
        public bool changeOnStay = true;
        public float onStayChangeInterval = 1.0f; // Deal damage every N seconds
        public float onStayChangeAmount = 2f;

        [Header("Exit")]
        public bool changeOnExit = true;
        public float onExitChangeAmount = 2f;
        
        [Header("Audio & Visuals")]
        [SerializeField]
        private InventoryAudioClip _audioClipOnDamage;

        [SerializeField]
        private GameObject _particleEffect;

        [SerializeField]
        private Vector3 _particleOffset;
        

        private Coroutine _coroutine;
        private WaitForSeconds _onStayWaitForSeconds;

        protected virtual void Awake()
        {
            _onStayWaitForSeconds = new WaitForSeconds(onStayChangeInterval);
        }


        protected void OnTriggerEnter(Collider other)
        {
            var player = InventoryPlayerManager.instance.currentPlayer;
            if (player != null && other.gameObject == player.gameObject)
            {
                if (player.characterCollection != null)
                {
                    if (changeOnEnter)
                    {
                        ChangeStat(player, enterChangeAmount);
                    }

                    if (changeOnStay)
                    {
                        _coroutine = StartCoroutine(_OnStay());
                    }

                    if (OnEnter != null)
                    {
                        var stat = player.characterCollection.stats.Get(property.property.category, property.property.name);
                        OnEnter(player, stat);
                    }
                }               
            }
        }

        protected virtual IEnumerator _OnStay()
        {
            // Keeps going forever untill StopCoroutine is called.
            while (true)
            {
                yield return _onStayWaitForSeconds;

                var player = InventoryPlayerManager.instance.currentPlayer;

                if (player.characterCollection != null)
                {
                    ChangeStat(player, onStayChangeAmount);

                    if (OnStay != null)
                    {
                        var stat = player.characterCollection.stats.Get(property.property.category, property.property.name);
                        OnStay(player, stat);
                    }
                }
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            var player = InventoryPlayerManager.instance.currentPlayer;
            if (other.gameObject == player.gameObject)
            {
                if (player.characterCollection != null)
                {
                    if (changeOnExit)
                    {
                        ChangeStat(player, onExitChangeAmount);
                    }

                    if (_coroutine != null)
                    {
                        StopCoroutine(_coroutine);
                    }

                    if (OnExit != null)
                    {
                        var stat = player.characterCollection.stats.Get(property.property.category, property.property.name);
                        OnExit(player, stat);
                    }
                }
            }
        }

        private void ChangeStat(InventoryPlayer player, float amount)
        {
            var stat = player.characterCollection.stats.Get(property.property.category, property.property.name);
            stat.ChangeCurrentValueRaw(amount);

            InventoryAudioManager.AudioPlayOneShot(_audioClipOnDamage);

            if (_particleEffect != null)
            {
                var particles = Instantiate(_particleEffect);
                particles.transform.position = player.transform.position + _particleOffset;

                Destroy(particles.gameObject, 1.0f);
            }
        }
    }
}
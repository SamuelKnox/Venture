using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.QuestSystemPro.Demo
{
    [RequireComponent(typeof(NavMeshAgent))]
    public partial class MyCustomMonster : MonoBehaviour
    {
        public float walkSpeed = 4.0f;
        public float walkRadius = 10.0f;

        [NonSerialized]
        private WaitForSeconds waitTime = new WaitForSeconds(1.0f);

        private Vector3 _aimPosition;
        private NavMeshAgent _agent;

        public bool isDead
        {
            get { return health <= 0; }
        }

        private int _health = 100;
        public int health
        {
            get { return _health; }
            protected set
            {
                _health = value;
            }
        }

        public void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = walkSpeed;
            _agent.enabled = true;

            StartCoroutine(_ChooseNewLocation());
        }

        private IEnumerator _ChooseNewLocation()
        {
            while (true)
            {
                ChooseNewLocation();
                yield return waitTime;
            }
        }

        public virtual void ChooseNewLocation()
        {
            if (isDead)
                return;

            _aimPosition = UnityEngine.Random.insideUnitCircle * walkRadius;
            _agent.SetDestination(transform.position + _aimPosition);
        }

        public void OnMouseDown()
        {
            health -= 50;

            if (isDead)
                Die(); // Ah it died!
        }

        protected virtual void Die()
        {
            if (!isDead)
                return;

            QuestLogger.Log("You killed it!");

            if (_agent.isOnNavMesh)
            {
                _agent.Stop();
                StartCoroutine(SinkIntoGround());
            }
        }

        protected virtual IEnumerator SinkIntoGround()
        {
            yield return waitTime;
            _agent.enabled = false; // To allow for sinking
            float timer = 0.0f;

            while (timer < 2.0f)
            {
                yield return null;

                transform.Translate(0, -1.0f * Time.deltaTime, 0.0f);
                timer += Time.deltaTime;
            }

            Destroy(gameObject);
        }
    }
}

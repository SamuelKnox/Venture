using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Damage))]
public abstract class Weapon : Item
{
    [Tooltip("Damage over time applied by this weapon")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    protected float damageOverTime = 0.0f;

    [Tooltip("Rate at which damage over time is increased")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    protected float damageOverTimeRateIncrease = 0.0f;

    [Tooltip("Sockets for roons")]
    [SerializeField]
    private RoonSocket[] roonSockets;

    protected Damage damage;

    protected virtual void Awake()
    {
        damage = GetComponent<Damage>();
        damage.SetActive(false);
        SetUnique(true);
    }

    protected virtual void OnValidate()
    {
        CheckForDuplicateSockets();
        ValidateRoonSocketTypes();
    }

    /// <summary>
    /// Gets the roon of the respective type from this item
    /// </summary>
    /// <param name="roonType">Type of Roon to get</param>
    /// <returns>The roon</returns>
    public Roon GetRoon(RoonType roonType)
    {
        var roonSocket = roonSockets.Where(s => s.GetRoonType() == roonType).FirstOrDefault();
        if (roonSocket == null)
        {
            Debug.LogError(gameObject + " cannot hold a Roon Socket of type " + roonType + "!", gameObject);
            return null;
        }
        return roonSocket.GetRoon();
    }

    /// <summary>
    /// Gets all roons in this weapon
    /// </summary>
    /// <returns>All roons</returns>
    public Roon[] GetRoons()
    {
        var roons = new List<Roon>();
        foreach (var roonSocket in roonSockets)
        {
            var roon = roonSocket.GetRoon();
            if (roon)
            {
                roons.Add(roon);
            }
        }
        return roons.ToArray();
    }

    /// <summary>
    /// Detaches all roons from this weapon
    /// </summary>
    public void DetachAllRoons()
    {
        foreach (var roon in GetRoons())
        {
            DetachRoon(roon);
        }
    }

    /// <summary>
    /// Sets the roon for this item's respective roon socket
    /// </summary>
    /// <param name="roon">Roon to set</param>
    public void SetRoon(Roon roon)
    {
        bool gameplaySceneActive = SceneManager.GetActiveScene() == SceneManager.GetSceneByName(SceneNames.Venture);
        var oldRoon = GetRoon(roon.GetRoonType());
        if (oldRoon && gameplaySceneActive)
        {
            oldRoon.Deactivate(this);
        }
        var roonSocket = roonSockets.Where(s => s.GetRoonType() == roon.GetRoonType()).FirstOrDefault();
        if (roonSocket == null)
        {
            Debug.LogError("There is no roon socket that can hold " + roon + " in " + gameObject + "!", gameObject);
            return;
        }
        roonSocket.SetRoon(roon);
        if (gameplaySceneActive)
        {
            roon.Activate(this);
        }
    }

    /// <summary>
    /// Removes the roon from its socket
    /// </summary>
    /// <param name="roon">Roon to remove</param>
    public void DetachRoon(Roon roon)
    {
        if (!roon)
        {
            Debug.LogError("Cannot detach a null roon!", gameObject);
            return;
        }
        bool gameplaySceneActive = SceneManager.GetActiveScene() == SceneManager.GetSceneByName(SceneNames.Venture);
        if (gameplaySceneActive)
        {
            roon.Deactivate(this);
        }
        var roonSocket = roonSockets.Where(s => s.GetRoonType() == roon.GetRoonType()).FirstOrDefault();
        roonSocket.SetRoon(null);
    }

    /// <summary>
    /// Deactivates all roons on this weapon
    /// </summary>
    public void DeactivateRoons()
    {
        foreach (var roon in GetRoons())
        {
            roon.Deactivate(this);
        }
    }

    /// <summary>
    /// Gets the damage over time effect from this weapon
    /// </summary>
    /// <returns>Damage Over Time</returns>
    public float GetDamageOverTime()
    {
        return damageOverTime;
    }

    /// <summary>
    /// Gets the increase in the rate at which damage is taken over time
    /// </summary>
    /// <returns>Damage over time rate increase</returns>
    public float GetDamageOverTimeRateIncrease()
    {
        return damageOverTimeRateIncrease;
    }

    /// <summary>
    /// Gets the types of roons this item can hold, as determined by its roon sockets
    /// </summary>
    /// <returns>Types of roons this item can hold</returns>
    public RoonType[] GetRoonSocketTypes()
    {
        return roonSockets.Select(s => s.GetRoonType()).ToArray();
    }

    /// <summary>
    /// Makes sure valid roon socket types are being used for the specific weapon type
    /// </summary>
    protected abstract void ValidateRoonSocketTypes();

    /// <summary>
    /// Asserts that there is only one type of each roon socket at most
    /// </summary>
    private void CheckForDuplicateSockets()
    {
        var roonTypes = roonSockets.Select(s => s.GetRoonType());
        if (roonTypes.Count() != roonTypes.Distinct().Count())
        {
            Debug.LogError("You cannot have more than one Roon Socket of the same type on one item!", gameObject);
            return;
        }
    }

    [Serializable]
    private class RoonSocket
    {
        [Tooltip("Type of roon this socket can hold")]
        [SerializeField]
        private RoonType roonType;

        [Tooltip("Roon in this socket")]
        [SerializeField]
        private Roon roon;

        /// <summary>
        /// Gets the type of roon this socket can hold
        /// </summary>
        /// <returns>Roon socket type</returns>
        public RoonType GetRoonType()
        {
            return roonType;
        }

        /// <summary>
        /// Gets the Roon for this Socket
        /// </summary>
        /// <returns>Roon in Socket</returns>
        public Roon GetRoon()
        {
            return roon;
        }

        /// <summary>
        /// Sets the roon for this socket
        /// </summary>
        /// <param name="roon">Roon to set</param>
        public void SetRoon(Roon roon)
        {
            if (this.roon)
            {
                this.roon.SetEquipped(false);
            }
            this.roon = roon;
            if (this.roon)
            {
                this.roon.SetEquipped(true);
            }
        }
    }
}
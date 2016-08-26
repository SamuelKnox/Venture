using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Equipment : Item
{
    [Tooltip("Damage over time applied on contact with this equipment")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    protected float damageOverTime = 0.0f;

    [Tooltip("Rate at which damage over time is increased")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    protected float damageOverTimeRateIncrease = 0.0f;

    [Tooltip("Sockets for runes")]
    [SerializeField]
    private RuneSocket[] runeSockets;

    protected virtual void OnValidate()
    {
        CheckForDuplicateSockets();
    }

    /// <summary>
    /// Sets the damage over time for this equipment
    /// </summary>
    /// <param name="damageOverTime">Damage over Time to set</param>
    public abstract void SetDamageOverTime(float damageOverTime);

    /// <summary>
    /// Sets the rate at which damage is applied over time is increased
    /// </summary>
    /// <param name="damageOverTimeRateIncrease">Rate of damage over time application increase</param>
    public abstract void SetDamageOverTimeRateIncrease(float damageOverTimeRateIncrease);

    /// <summary>
    /// Gets the rune of the respective type from this item
    /// </summary>
    /// <param name="runeType">Type of Rune to get</param>
    /// <returns>The rune</returns>
    public Rune GetRune(RuneType runeType)
    {
        var runeSocket = runeSockets.Where(s => s.GetRuneType() == runeType).FirstOrDefault();
        if (runeSocket == null)
        {
            Debug.LogError(gameObject + " cannot hold a Rune Socket of type " + runeType + "!", gameObject);
            return null;
        }
        return runeSocket.GetRune();
    }

    /// <summary>
    /// Gets all runes in this equipment
    /// </summary>
    /// <returns>All runes</returns>
    public Rune[] GetRunes()
    {
        var runes = new List<Rune>();
        foreach (var runeSocket in runeSockets)
        {
            var rune = runeSocket.GetRune();
            if (rune)
            {
                runes.Add(rune);
            }
        }
        return runes.ToArray();
    }

    /// <summary>
    /// Detaches all runes from this equipment
    /// </summary>
    public void DetachAllRunes()
    {
        foreach (var rune in GetRunes())
        {
            DetachRune(rune);
        }
    }

    /// <summary>
    /// Sets the rune for this item's respective rune socket
    /// </summary>
    /// <param name="rune"></param>
    public void SetRune(Rune rune)
    {
        bool gameplaySceneActive = SceneManager.GetActiveScene() == SceneManager.GetSceneByName(SceneNames.Venture);
        var oldRune = GetRune(rune.GetRuneType());
        if (oldRune && gameplaySceneActive)
        {
            oldRune.Deactivate(this);
        }
        var runeSocket = runeSockets.Where(s => s.GetRuneType() == rune.GetRuneType()).FirstOrDefault();
        if (runeSocket == null)
        {
            Debug.LogError("There is no rune socket that can hold " + rune + " in " + gameObject + "!", gameObject);
            return;
        }
        runeSocket.SetRune(rune);
        if (gameplaySceneActive)
        {
            rune.Activate(this);
        }
    }

    /// <summary>
    /// Removes the rune from its socket
    /// </summary>
    /// <param name="rune">Rune to remove</param>
    public void DetachRune(Rune rune)
    {
        if (!rune)
        {
            Debug.LogError("Cannot detach a null rune!", gameObject);
            return;
        }
        bool gameplaySceneActive = SceneManager.GetActiveScene() == SceneManager.GetSceneByName(SceneNames.Venture);
        if (gameplaySceneActive)
        {
            rune.Deactivate(this);
        }
        var runeSocket = runeSockets.Where(s => s.GetRuneType() == rune.GetRuneType()).FirstOrDefault();
        runeSocket.SetRune(null);
    }

    /// <summary>
    /// Deactivates all runes on this equipment
    /// </summary>
    public void DeactivateRunes()
    {
        foreach (var rune in GetRunes())
        {
            rune.Deactivate(this);
        }
    }

    /// <summary>
    /// Gets the damage over time effect from this equipment
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
    /// Gets the types of runes this item can hold, as determined by its rune sockets
    /// </summary>
    /// <returns>Types of runes this item can hold</returns>
    public RuneType[] GetRuneSocketTypes()
    {
        return runeSockets.Select(s => s.GetRuneType()).ToArray();
    }

    /// <summary>
    /// Asserts that there is only one type of each rune socket at most
    /// </summary>
    private void CheckForDuplicateSockets()
    {
        var runeTypes = runeSockets.Select(s => s.GetRuneType());
        if (runeTypes.Count() != runeTypes.Distinct().Count())
        {
            Debug.LogError("You cannot have more than one Rune Socket of the same type on one item!", gameObject);
            return;
        }
    }

    [Serializable]
    private class RuneSocket
    {
        [Tooltip("Type of rune this socket can hold")]
        [SerializeField]
        private RuneType runeType;

        [Tooltip("Rune in this socket")]
        [SerializeField]
        private Rune rune;

        /// <summary>
        /// Gets the type of rune this socket can hold
        /// </summary>
        /// <returns>Rune socket type</returns>
        public RuneType GetRuneType()
        {
            return runeType;
        }

        /// <summary>
        /// Gets the Rune for this Socket
        /// </summary>
        /// <returns>Rune in Socket</returns>
        public Rune GetRune()
        {
            return rune;
        }

        /// <summary>
        /// Sets the rune for this socket
        /// </summary>
        /// <param name="rune">Rune to set</param>
        public void SetRune(Rune rune)
        {
            if (this.rune)
            {
                this.rune.SetEquipped(false);
            }
            this.rune = rune;
            if (this.rune)
            {
                this.rune.SetEquipped(true);
            }
        }
    }
}
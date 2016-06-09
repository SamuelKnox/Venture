using UnityEngine;

public static class TeamUtility
{
    /// <summary>
    /// Checks whether not the two Game Objects are friendly with eachother
    /// </summary>
    /// <param name="gameObject0">The first game object</param>
    /// <param name="gameObject1">The second game object</param>
    /// <returns>Whether or not they are friendly</returns>
    public static bool IsFriendly(GameObject gameObject0, GameObject gameObject1)
    {
        return gameObject0.tag == gameObject1.tag && gameObject0.tag != Tags.Neutral.ToString() && gameObject1.tag != Tags.Neutral.ToString();
    }
}
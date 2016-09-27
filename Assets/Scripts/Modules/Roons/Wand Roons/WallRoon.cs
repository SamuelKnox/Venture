using UnityEngine;

public class WallRoon : WandRoon
{
    [Tooltip("Wall prefab for the level 1 wall")]
    [SerializeField]
    private Wall wallLevel1;

    [Tooltip("Wall prefab for the level 2 wall")]
    [SerializeField]
    private Wall wallLevel2;

    [Tooltip("Wall prefab for the level 3 wall")]
    [SerializeField]
    private Wall wallLevel3;

    [Tooltip("Wall prefab for the level 4 wall")]
    [SerializeField]
    private Wall wallLevel4;

    [Tooltip("Layers which wall can be placed on")]
    [SerializeField]
    private LayerMask obstructionLayers;

    [Tooltip("How far in front of the player the wall is created")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float range = 3.0f;

    /// <summary>
    /// Activates the wall roon
    /// </summary>
    /// <param name="weapon">Weapon the roon is attached to</param>
    public override void Activate(Weapon weapon) { }

    /// <summary>
    /// Casts a wall in front of the player
    /// </summary>
    /// <param name="wand">Wand spell is cast from</param>
    public override void ActivateAbility(Wand wand, Vector2 direction)
    {
        var origin = (Vector2)wand.transform.position + new Vector2(PlayerManager.Player.GetFacingDirection() * range, 0.0f);

        var raycastAll = Physics2D.RaycastAll(origin, Vector2.down, Mathf.Infinity, obstructionLayers);
        Debug.Log(raycastAll.Length);
        var raycastDirection = raycastAll.Length % 2 == 0 ? Vector2.down : Vector2.up;
        Debug.Log(raycastDirection);
        var raycastHit2D = Physics2D.Raycast(origin, raycastDirection, Mathf.Infinity, obstructionLayers);
        var spawnPoint = raycastHit2D.point;

        Wall wallInstance = null;
        switch ((int)GetSpecialValue())
        {
            case 1:
                wallInstance = Instantiate(wallLevel1);
                break;
            case 2:
                wallInstance = Instantiate(wallLevel2);
                break;
            case 3:
                wallInstance = Instantiate(wallLevel3);
                break;
            case 4:
                wallInstance = Instantiate(wallLevel4);
                break;
            default:
                Debug.LogError("An invalid wall level was used!", gameObject);
                return;
        }
        var wallCollider = wallInstance.GetComponent<Collider2D>();
        if (wallCollider)
        {
            spawnPoint.y += wallCollider.bounds.extents.y;
        }
        wallInstance.transform.position = spawnPoint;
        Destroy(wallInstance.gameObject, GetBaseValue());
    }

    /// <summary>
    /// Deactivates the roon
    /// </summary>
    /// <param name="weapon">Weapon roon is attached to</param>
    public override void Deactivate(Weapon weapon) { }
}
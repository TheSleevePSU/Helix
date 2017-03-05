using UnityEngine;
using System.Collections;
using System;

public class Move : Action
{
    public bool ignoreEnemies;
    public bool ignorePlayers;
    public bool teleport;
    //private LayerMask layerMask;
    public Vector3 position;

    public Move(Vector3 _position, bool _ignoreEnemies = false, bool _ignorePlayers = false, bool _teleport = false, Sprite _previewSprite = default(Sprite))
    {
        position = _position;
        ignoreEnemies = _ignoreEnemies;
        ignorePlayers = _ignorePlayers;
        //layerMask = LayerMask.NameToLayer("Unwalkable");
        teleport = _teleport;
        previewSprite = _previewSprite;
    }

    public override bool CanPerform(Vector3 worldPosition)
    {
        Collider2D[] c2d = Physics2D.OverlapPointAll(worldPosition);
        foreach (Collider2D c in c2d)
        {
            Enemy e = c.GetComponent<Enemy>();
            if (e != null && !ignoreEnemies)
            {
                return false;
            }
            Player p = c.GetComponent<Player>();
            if (p != null && !ignorePlayers)
            {
                return false;
            }
            PlaceSaver ps = c.GetComponent<PlaceSaver>();
            if (ps != null)
            {
                return false;
            }
            Tile t = c.GetComponent<Tile>();
            if (t != null)
            {
                if (!t.isWalkable)
                {
                    return false;
                }
            }
        }
        if (c2d.Length == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

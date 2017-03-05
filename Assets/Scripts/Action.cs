using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Action
{
    public int coolDown;
    public bool consumable = false;
    public Sprite previewSprite;

    public abstract bool CanPerform(Vector3 actorPosition);
}
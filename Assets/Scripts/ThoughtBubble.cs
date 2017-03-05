using UnityEngine;
using System.Collections;

public class ThoughtBubble : MonoBehaviour
{
    public Sprite patrol;
    public Sprite hunt;
    public Sprite aim;
    public Sprite attack;
    public Sprite cooldown;

    private SpriteRenderer sr;

    // Use this for initialization
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = patrol;
    }
    
    public void SetSprite(Sprite s)
    {
        sr.sprite = s;
    }
}

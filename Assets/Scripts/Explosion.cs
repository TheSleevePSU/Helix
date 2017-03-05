using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    public AudioClip explosionSound;

    // Use this for initialization
    void Start()
    {
        Destroy(this.gameObject, 0.615f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player p = other.gameObject.GetComponent<Player>();
        if (p != null)
        {
            p.SendMessage("HitByEnemyActiveObject", this);
        }
        Enemy e = other.gameObject.GetComponent<Enemy>();
        if (e != null)
        {
            e.SendMessage("HitByPlayerActiveObject", this);
        }
        Barrel b = other.gameObject.GetComponent<Barrel>();
        if (b != null)
        {
            b.SendMessage("HitByActiveObject", this);
        }
    }
}
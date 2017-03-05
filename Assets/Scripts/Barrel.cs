using UnityEngine;
using System.Collections;

public class Barrel : MonoBehaviour
{
    public GameObject explosion;

    public void HitByActiveObject(object other)
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}

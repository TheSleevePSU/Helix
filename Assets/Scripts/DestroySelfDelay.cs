using UnityEngine;
using System.Collections;

public class DestroySelfDelay : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject, GameController.instance.secondsPerTurn);
    }
}

using UnityEngine;
using System.Collections;

public class CameraSimpleFollow : MonoBehaviour
{
    public bool follow;
    public Transform target;
    public float smooth = 5.0f;

    void Update()
    {
        if (follow)
        {
            float originalZ = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * smooth);
            transform.position = new Vector3(transform.position.x, transform.position.y, originalZ);
        }
        else
        {
            float originalZ = transform.position.z;
            transform.position = new Vector3(target.position.x, target.position.y, originalZ);
        }
    }
}
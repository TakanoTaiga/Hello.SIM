using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public GameObject robotoObject;
    private Vector3 offset;

    void Start()
    {
        offset = this.transform.position - robotoObject.transform.position;
    }

    void LateUpdate()
    {
        this.transform.position = Vector3.Lerp(transform.position, robotoObject.transform.position + offset, 0.1f * Time.deltaTime);
    }
}

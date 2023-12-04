using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public GameObject robotoObject;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = this.transform.position - robotoObject.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = Vector3.Lerp(transform.position, robotoObject.transform.position + offset, 0.1f * Time.deltaTime);
    }
}

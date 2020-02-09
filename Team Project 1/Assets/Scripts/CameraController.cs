using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player,scrollLimitL,scrollLimitR;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Quit when the escape key is pressed
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, offset.y, offset.z);
        if (scrollLimitL != null)
        {
            if (transform.position.x < scrollLimitL.transform.position.x)
            {
                transform.position = new Vector3(scrollLimitL.transform.position.x, offset.y, offset.z);
            }
        }
        if (scrollLimitR != null)
        {
            if (transform.position.x > scrollLimitR.transform.position.x)
            {
                transform.position = new Vector3(scrollLimitR.transform.position.x, offset.y, offset.z);
            }
        }
    }
}

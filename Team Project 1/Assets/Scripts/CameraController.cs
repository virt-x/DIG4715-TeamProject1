using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
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
    }
}

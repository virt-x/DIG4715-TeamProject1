using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableController : MonoBehaviour
{
    public GameObject spawn, hitSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack"))
        {
            Instantiate(hitSound, new Vector3(transform.position.x, transform.position.y), new Quaternion());
            if (spawn != null)
            {
                Instantiate(spawn, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}

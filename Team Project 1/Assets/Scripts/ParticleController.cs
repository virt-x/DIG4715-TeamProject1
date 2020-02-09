using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    float age;
    // Start is called before the first frame update
    void Start()
    {
        age = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > age + 0.5f)
        {
            Destroy(gameObject);
        }
    }
}

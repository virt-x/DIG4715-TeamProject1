using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundKiller : MonoBehaviour
{
    private float age;
    // Start is called before the first frame update
    void Start()
    {
        age = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (age + 1 < Time.time)
        {
            Destroy(gameObject);
        }
    }
}

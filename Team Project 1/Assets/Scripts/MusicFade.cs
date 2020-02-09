using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFade : MonoBehaviour
{
    private bool fade;
    // Start is called before the first frame update
    void Start()
    {
        fade = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fade)
        {
            gameObject.GetComponent<AudioSource>().volume -= 0.5f * Time.deltaTime;
        }
    }

    public void StartFade()
    {
        fade = true;
    }
}

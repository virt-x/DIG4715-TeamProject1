using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawn, mainCamera;
    private float spawnDelay;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary") && (Mathf.Abs(mainCamera.transform.position.x - transform.position.x) > 14.5) && spawn.name != "Medusa")
        {
            if (transform.childCount == 0)
            {
                Instantiate(spawn, transform.position, transform.rotation, transform);
            }
        }
        else if (spawn.name == "Medusa" && collision.CompareTag("Player") && Time.time > spawnDelay)
        {
            float facing = Mathf.Sign(Random.value - 0.5f);
            GameObject medusa = Instantiate(spawn, new Vector3(mainCamera.transform.position.x + (15 * facing), (collision.transform.position.y + mainCamera.transform.position.y) / 2 + (Random.value * 12 - 6)), transform.rotation, transform);
            if (facing < 0)
            {
                medusa.transform.localScale = new Vector3(-medusa.transform.localScale.x, medusa.transform.localScale.y, medusa.transform.localScale.z);
            }
            spawnDelay = Time.time + 1.5f + Random.value * 3;
        }
    }
}

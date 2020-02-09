using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    public int id;
    public GameObject magic, mana, breakSound;
    private PlayerController player;
    private float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        lifetime = Time.time;
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
        if (id == 3 || id == 4 || id == 5 || id == 6)
        {
            if (player.activeCharacter == player.characterB)
            {
                Instantiate(magic, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
        if (player.activeCharacter.Subweapon == id - 2 && player.activeCharacter.Subweapon > 0)
        {
            Instantiate(mana, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lifetime + 5 < Time.time)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Instantiate(breakSound, new Vector3(transform.position.x, transform.position.y), new Quaternion());
            Destroy(gameObject);
        }
    }
}

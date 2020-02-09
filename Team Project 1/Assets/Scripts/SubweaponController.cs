using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubweaponController : MonoBehaviour
{
    public int id;
    private float startTime;
    public GameObject rosaryFlame, breakSound;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        if (id != 6)
        {
            transform.localScale = new Vector3(GameObject.FindGameObjectsWithTag("Player")[0].transform.localScale.x * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        if (id == 2 || id == 3)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(transform.localScale.x * 0.05f, 0.1f), ForceMode2D.Impulse);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (id == 1)
        {
            transform.position += new Vector3(0.25f * transform.localScale.x, 0);
        }
        if (id == 2 || id == 3 || id == 4 || id == 5)
        {
            transform.Rotate(0, 0, 30);
        }
        if (id == 4)
        {
            transform.position += new Vector3(0.1f * transform.localScale.x * ((startTime + 2) - Time.time), 0);
        }
        if (id == 5)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length != 0)
            {
                GameObject target = GameObject.FindGameObjectsWithTag("Enemy")[0];
                gameObject.GetComponent<Rigidbody2D>().AddForce((target.transform.position - transform.position).normalized * 0.25f);
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(transform.localScale.x * 0.05f, 0f));
            }
            if (startTime + 5f < Time.time)
            {
                Destroy(gameObject);
            }
        }
        if (id == 6)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(transform.localScale.x * 0.1f, 0f));
            if (startTime + 1f < Time.time)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (id == 1)
        {
            if (collision.CompareTag("Floor") || collision.CompareTag("Enemy"))
            {
                Destroy(gameObject);
            }
        }
        if (id == 3)
        {
            if (collision.CompareTag("Enemy"))
            {
                Instantiate(breakSound, new Vector3(transform.position.x, transform.position.y), new Quaternion());
                Destroy(gameObject);
            }
            if (collision.CompareTag("Floor"))
            {
                Instantiate(breakSound, new Vector3(transform.position.x, transform.position.y), new Quaternion());
                GameObject flame = Instantiate(rosaryFlame, new Vector3(transform.position.x, transform.position.y + 0.5f), new Quaternion());
                flame.transform.localScale = new Vector3(flame.transform.localScale.x * Mathf.Sign(transform.localScale.x), flame.transform.localScale.y, flame.transform.localScale.z);
                Destroy(gameObject);
            }
        }
        if (id == 4 && (startTime + 2f) < Time.time)
        {
            if (collision.CompareTag("Player"))
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int health, damage;
    public float speed;
    public string enemyName;
    private Rigidbody2D body;
    public GameObject player;
    private int aiOption;
    private float optionDelay, damageTime;
    private bool floor;
    private Color baseColor;
    private SpriteRenderer spriteRenderer;
    public ParticleSystem particles;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        optionDelay = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > optionDelay)
        {
            aiOption = 0;
        }

        if (enemyName == "skeleton" && aiOption == 0)
        {
            float option = Random.value * 10;
            if (option < 4)
            {
                aiOption = 1;
            }
            else if (option < 8)
            {
                aiOption = 2;
            }
            else
            {
                aiOption = 3;
            }
            optionDelay = Time.time + 0.5f + Random.value;
        }

        if (enemyName == "skeleton")
        {
            if (player.transform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
            else if (player.transform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
        }

        if (enemyName == "medusa")
        {
            transform.position += new Vector3(transform.localScale.x * speed, ((((Time.time - optionDelay) % 1) - 0.5f) * Mathf.Sign(((Time.time - optionDelay) % 2) - 1)) * 0.2f);
        }
    }

    void FixedUpdate()
    {if (Time.time > damageTime)
        {
            if (aiOption == 1)
            {
                transform.position += new Vector3(-speed, 0);
            }
            if (aiOption == 2)
            {
                transform.position += new Vector3(speed, 0);
            }
            if (aiOption == 3)
            {
                if (Vector3.Distance(player.transform.position, transform.position) < 3f && floor == true)
                {
                    body.AddForce(new Vector2(0, 0.5f), ForceMode2D.Impulse);
                    floor = false;
                    aiOption = -1;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack"))
        {
            health--;
            damageTime = Time.time + 0.5f;
            StartCoroutine(DamageFlash());
            if (enemyName == "skeleton")
            {
                body.AddForce(new Vector2(0.1f * -1 * Mathf.Sign(gameObject.transform.localScale.x), 0.1f), ForceMode2D.Impulse);
            }
        }
        if (health < 1)
        {
            Instantiate(particles, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            floor = true;
        }
    }

    IEnumerator DamageFlash()
    {
        for(int i = 16; i > -1; i--)
        {
            if (i % 4 == 3 || i % 4 == 2)
            {
                spriteRenderer.color = new Color(0.1f, 0.1f, 1);
            }
            else
            {
                spriteRenderer.color = baseColor;
            }
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            Destroy(gameObject);
        }
    }
}

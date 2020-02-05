using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body;
    public float speed, jump;
    private bool floor, damaged;
    private float movement;
    private GameCharacter characterA, characterB, activeCharacter, inactiveCharacter;
    public Animator animator;
    public GameObject healthMeter, manaMeter;
    private float swapDelay,damageDelay;
    // Start is called before the first frame update
    void Start()
    {
        characterA = new GameCharacter(16, 8);
        characterB = new GameCharacter(8, 16);
        activeCharacter = characterA;
        inactiveCharacter = characterB;
        floor = false;
        body = GetComponent<Rigidbody2D>();
        InitializeMeters();
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    void FixedUpdate()
    {
        if (Time.time > damageDelay + 0.5f)
        {
            damaged = false;
            animator.SetBool("Damaged", false);
        }

        if (!(Time.time > swapDelay + 0.5f) || damaged)
        {
            return;
        }

        movement = Input.GetAxisRaw("Horizontal");

        if (movement > 0)
        {
            gameObject.transform.localScale = new Vector3(1, 1);
            animator.SetBool("Walking", true);
        }
        else if (movement < 0)
        {
            gameObject.transform.localScale = new Vector3(-1, 1);
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }

        if (floor || activeCharacter == characterB)
        {
            transform.position += new Vector3(movement * speed, 0);
        }
        else
        {
            transform.position += new Vector3(movement * speed * 0.05f, 0);
        }

        if (Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x,gameObject.transform.position.y - 1), new Vector2(0.45f, 0.2f), 0f, 1 << 8) != null)
        {
            floor = true;
            animator.SetBool("Floor", floor);
        }
        else
        {
            floor = false;
            animator.SetBool("Floor", floor);
        }

        if (Input.GetButton("Jump") && floor)
        {
            body.AddForce(new Vector2(movement * 0.1f, jump), ForceMode2D.Impulse);
            floor = false;
            animator.SetBool("Floor", floor);
        }
    }

    void LateUpdate()
    {
        if (Input.GetButton("Fire2") && floor && (Time.time > swapDelay + 0.5f))
        {
            animator.SetBool("Walking", false);
            swapDelay = Time.time;
            movement = 0;
            floor = false;
            GameCharacter swap = activeCharacter;
            activeCharacter = inactiveCharacter;
            inactiveCharacter = swap;
            if (activeCharacter == characterA)
            {
                animator.SetBool("Character", false);
            }
            else if (activeCharacter == characterB)
            {
                animator.SetBool("Character", true);
            }
            InitializeMeters();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && Time.time > damageDelay + 0.5f)
        {
            damaged = true;
            animator.SetBool("Damaged", true);
            damageDelay = Time.time;
            body.AddForce(new Vector2(0.2f * -1 * gameObject.transform.localScale.x, jump * 1.1f), ForceMode2D.Impulse);
            activeCharacter.Health--;
            UpdateMeters();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            floor = false;
            animator.SetBool("Floor", floor);
        }
    }

    private void InitializeMeters()
    {
        int count = activeCharacter.MaxHealth;
        foreach(Transform child in healthMeter.transform)
        {
            if (count > 0)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
            count--;
        }
        count = activeCharacter.MaxMana;
        foreach (Transform child in manaMeter.transform)
        {
            if (count > 0)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
            count--;
        }
        UpdateMeters();
    }
    private void UpdateMeters()
    {
        int count = activeCharacter.Health;
        foreach (Transform child in healthMeter.transform)
        {
            if (count > 0)
            {
                child.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                child.localScale = new Vector3(1, 0.1f, 1);
            }
            count--;
        }
        count = activeCharacter.Mana;
        foreach (Transform child in manaMeter.transform)
        {
            if (count > 0)
            {
                child.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                child.localScale = new Vector3(1, 0.1f, 1);
            }
            count--;
        }
    }
}

public class GameCharacter
{
    private int health, maxHealth, mana, maxMana, subweapon;

    public int Health
    {
        get { return health; }
        set { health = value; }
    }
    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }
    public int Mana
    {
        get { return mana; }
        set { mana = value; }
    }
    public int MaxMana
    {
        get { return maxMana; }
        set { maxMana = value; }
    }
    public int Subweapon
    {
        get { return subweapon; }
        set { subweapon = value; }
    }

    public GameCharacter(int hp, int mp)
    {
        health = hp;
        maxHealth = hp;
        mana = 0;
        maxMana = mp;
        subweapon = 0;
    }
}

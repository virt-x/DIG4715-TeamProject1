using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body;
    public float speed, jump;
    private bool floor, damaged, win, level;
    private float movement;
    public GameCharacter characterA, characterB, activeCharacter, inactiveCharacter;
    public Animator animator;
    public GameObject healthMeter, manaMeter, attackBox, spearProjectile, hammerProjectile, rosaryProjectile, boomerangProjectile, magicProjectile, l2scrollLimitL, l2scrollLimitR, l2spawn;
    private float swapDelay, damageDelay, attackDelay, gameTime, deathDelay;
    public UnityEngine.UI.Text timeText, subText;
    public UnityEngine.UI.Image subImage, portraitImage, levelPanel;
    public Sprite spear, hammer, rosary, boomerang, magic, portraitA, portraitB;
    // Start is called before the first frame update
    void Start()
    {
        level = false;
        win = false;
        attackBox.SetActive(false);
        characterA = new GameCharacter(16, 8);
        characterB = new GameCharacter(8, 16);
        activeCharacter = characterA;
        inactiveCharacter = characterB;
        floor = false;
        body = GetComponent<Rigidbody2D>();
        InitializeMeters();
        deathDelay = 0;
        gameTime = 46 + Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (win)
        {
            return;
        }
        animator.SetFloat("SwapTime", swapDelay + 0.5f - Time.time);
        animator.SetInteger("Health", activeCharacter.Health);
        attackBox.transform.position = transform.position;
        attackBox.transform.localScale = transform.localScale;
        if (Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1), new Vector2(0.9f, 0.2f), 0f, 1 << 8) != null)
        {
            floor = true;
            animator.SetBool("Floor", floor);
        }
        else
        {
            floor = false;
            animator.SetBool("Floor", floor);
        }
        if (Time.time > damageDelay + 0.5f)
        {
            damaged = false;
            animator.SetBool("Damaged", false);
        }
        if (Time.time > attackDelay + 0.5f)
        {
            animator.SetBool("Attacking", false);
            animator.SetBool("Sub", false);
            attackBox.SetActive(false);
        }

        movement = Input.GetAxisRaw("Horizontal");

        timeText.text = "TIME-" + (gameTime - Time.time > 10?"":"0") + (gameTime - Time.time > 0? Mathf.Floor(gameTime - Time.time).ToString():"0");
        if (gameTime - Time.time < 0)
        {
            characterA.Health = 0;
            characterB.Health = 0;
            UpdateMeters();
        }

        if (activeCharacter.Health < 1)
        {
            damaged = true;
            animator.SetBool("Walking", false);
            movement = 0;
            if (floor && deathDelay == 0)
            {
                deathDelay = Time.time;
            }
            if (deathDelay > 0 && Time.time > deathDelay + 2f && inactiveCharacter.Health > 0)
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
                    portraitImage.sprite = portraitB;
                }
                else if (activeCharacter == characterB)
                {
                    animator.SetBool("Character", true);
                    portraitImage.sprite = portraitA;
                }
                if (inactiveCharacter.Health < 1)
                {
                    portraitImage.color = new Color(1, 1, 1, 0);
                }
                InitializeMeters();
                UpdateSubweapon();
            }
            else if (deathDelay > 0 && Time.time > deathDelay + 2f && inactiveCharacter.Health < 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(3);
            }
        }
        else
        {
            deathDelay = 0;
        }
    }
    
    void FixedUpdate()
    {
        if (!(Time.time > swapDelay + 0.5f) || !(Time.time > attackDelay + 0.5f) || damaged || (!(Time.time > deathDelay + 2f) && deathDelay != 0) || win)
        {
            return;
        }

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
            //transform.position += new Vector3(movement * speed, 0);
            body.AddForce(new Vector2(movement * 10 * speed, 0));
            if (body.velocity.x > speed)
            {
                body.velocity = new Vector2(speed, body.velocity.y);
            }
            if (body.velocity.x < -speed)
            {
                body.velocity = new Vector2(-speed, body.velocity.y);
            }
        }
        else
        {
            transform.position += new Vector3(movement * speed * 0.001f, 0);
        }

        

        if (Input.GetButton("Jump") && floor)
        {
            body.velocity = Vector2.zero;
            body.AddForce(new Vector2(movement * 0.05f, jump), ForceMode2D.Impulse);
            if (body.velocity.x > speed / 4)
            {
                body.velocity = new Vector2(speed / 4, body.velocity.y);
            }
            if (body.velocity.x < -speed / 4)
            {
                body.velocity = new Vector2(-speed / 4, body.velocity.y);
            }
            floor = false;
            animator.SetBool("Floor", floor);
        }
    }

    void LateUpdate()
    {
        if (win)
        {
            return;
        }
        if (Input.GetButton("Fire1") && (Time.time > attackDelay + 0.5f))
        {
            animator.SetBool("Attacking", true);
            StartCoroutine(AttackAnimationDelay());
            attackDelay = Time.time;
            movement = 0;
        }
        if (Input.GetButton("Fire2") && (Time.time > attackDelay + 0.5f))
        {
            if (activeCharacter.Subweapon != 0)
            {
                if (activeCharacter.Mana < 1 || (activeCharacter.Subweapon == 5 && activeCharacter.Mana < 3))
                {
                    StartCoroutine(FlashTheMana());
                }
                else
                {
                    animator.SetBool("Sub", true);
                    attackDelay = Time.time;
                    movement = 0;
                    StartCoroutine(SubWeaponDelay());
                    activeCharacter.Mana -= (activeCharacter.Subweapon == 5 ? 3 : 1);
                    UpdateMeters();
                }
            }
        }
        if (Input.GetButton("Fire3") && floor && (Time.time > swapDelay + 0.5f) && inactiveCharacter.Health > 0)
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
                portraitImage.sprite = portraitB;
            }
            else if (activeCharacter == characterB)
            {
                animator.SetBool("Character", true);
                portraitImage.sprite = portraitA;
            }
            if (inactiveCharacter.Health < 1)
            {
                portraitImage.color = new Color(1, 1, 1, 0);
            }
            InitializeMeters();
            UpdateSubweapon();
        }
    }

    IEnumerator AttackAnimationDelay()
    {
        yield return new WaitForSeconds(0.25f);
        attackBox.SetActive(true);
        yield break;
    }

    IEnumerator SubWeaponDelay()
    {
        yield return new WaitForSeconds(0.25f);
        switch (activeCharacter.Subweapon)
        {
            case 1:
                Instantiate(spearProjectile, transform.position + new Vector3(0.5f, 0.25f, 0f), transform.rotation);
                break;
            case 2:
                Instantiate(hammerProjectile, transform.position + new Vector3(0.5f, 0.25f, 0f), transform.rotation);
                break;
            case 3:
                Instantiate(rosaryProjectile, transform.position + new Vector3(0.5f, 0.25f, 0f), transform.rotation);
                break;
            case 4:
                Instantiate(boomerangProjectile, transform.position + new Vector3(0.5f, 0.25f, 0f), transform.rotation);
                break;
            case 5:
                Instantiate(magicProjectile, transform.position + new Vector3(0.5f, 0.25f, 0f), transform.rotation);
                break;
        }
        yield break;
    }

    IEnumerator FlashTheMana()
    {
        ManaFlash(true);
        yield return new WaitForSeconds(0.1f);
        ManaFlash(false);
        yield return new WaitForSeconds(0.1f);
        ManaFlash(true);
        yield return new WaitForSeconds(0.1f);
        ManaFlash(false);
        yield return new WaitForSeconds(0.1f);
        ManaFlash(true);
        yield return new WaitForSeconds(0.1f);
        ManaFlash(false);
        yield break;
    }
    void ManaFlash(bool color)
    {
        foreach (UnityEngine.UI.Image cell in manaMeter.GetComponentsInChildren<UnityEngine.UI.Image>())
        {
            if (color)
            {
                cell.color = new Color(1, 1, 1);
            }
            else
            {
                cell.color = new Color32(31, 63, 127, 255);
            }
        }
    }


    void OnTriggerEnter2d(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && Time.time > damageDelay + 0.5f)
        {
            damaged = true;
            animator.SetBool("Damaged", true);
            damageDelay = Time.time;
            body.velocity = Vector2.zero;
            body.AddForce(new Vector2(0.2f * -1 * gameObject.transform.localScale.x, jump * 1.1f), ForceMode2D.Impulse);
            activeCharacter.Health -= collision.gameObject.GetComponent<EnemyController>().damage;
            UpdateMeters();
        }
        if (collision.CompareTag("Win"))
        {
            win = true;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Win"))
        {
            animator.SetBool("Walking", false);
            win = true;
            StartCoroutine(LevelUp());
        }
        if (win)
        {
            return;
        }
        if (collision.collider.CompareTag("Enemy") && Time.time > damageDelay + 0.5f)
        {
            damaged = true;
            animator.SetBool("Damaged", true);
            damageDelay = Time.time;
            body.velocity = Vector2.zero;
            body.AddForce(new Vector2(0.2f * -1 * gameObject.transform.localScale.x, jump * 1.1f), ForceMode2D.Impulse);
            activeCharacter.Health -= collision.gameObject.GetComponent<EnemyController>().damage;
            UpdateMeters();
        }

        if (collision.collider.CompareTag("Pickup"))
        {
            int id = collision.collider.GetComponent<PickupController>().id;
            switch (id)
            {
                case 0:
                    PickupAddResource(1, true);
                    UpdateMeters();
                    break;
                case 1:
                    PickupAddResource(5, true);
                    UpdateMeters();
                    break;
                case 2:
                    PickupAddResource(3, false);
                    UpdateMeters();
                    break;
                case 3:
                    if (activeCharacter == characterA)
                    {
                        activeCharacter.Subweapon = 1;
                        UpdateSubweapon();
                    }
                    break;
                case 4:
                    if (activeCharacter == characterA)
                    {
                        activeCharacter.Subweapon = 2;
                        UpdateSubweapon();
                    }
                    break;
                case 5:
                    if (activeCharacter == characterA)
                    {
                        activeCharacter.Subweapon = 3;
                        UpdateSubweapon();
                    }
                    break;
                case 6:
                    if (activeCharacter == characterA)
                    {
                        activeCharacter.Subweapon = 4;
                        UpdateSubweapon();
                    }
                    break;
                case 7:
                    if (activeCharacter == characterB)
                    {
                        activeCharacter.Subweapon = 5;
                        UpdateSubweapon();
                    }
                    break;
            }
        }
    }

    IEnumerator LevelUp()
    {
        yield return new WaitForSeconds(3);
        levelPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        if (level)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
        else
        {
            activeCharacter.Health = activeCharacter.MaxHealth;
            inactiveCharacter.Health = inactiveCharacter.MaxHealth;
            portraitImage.color = new Color(1, 1, 1, 1);
            CameraController camera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<CameraController>();
            camera.scrollLimitL = l2scrollLimitL;
            camera.scrollLimitR = l2scrollLimitR;
            camera.offset = new Vector3(camera.offset.x, camera.offset.y - 50, camera.offset.z);
            transform.position = l2spawn.transform.position;
            gameTime = 46 + Time.time;
            UpdateMeters();
            win = false;
            level = true;
            levelPanel.gameObject.SetActive(false);
        }
    }

    void PickupAddResource(int value, bool meter)
    {
        for(int i = value;i > 0; i--)
        {
            if (meter)
            {
                if (activeCharacter.Mana < activeCharacter.MaxMana)
                {
                    activeCharacter.Mana++;
                }
                else if (inactiveCharacter.Mana < inactiveCharacter.MaxMana && inactiveCharacter.Health > 0)
                {
                    inactiveCharacter.Mana++;
                }
            }
            else
            {
                if (activeCharacter.Health < activeCharacter.MaxHealth)
                {
                    activeCharacter.Health++;
                }
                else if (inactiveCharacter.Health < inactiveCharacter.MaxHealth && inactiveCharacter.Health > 0)
                {
                    inactiveCharacter.Health++;
                }
            }
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
    void OnTriggerExit2D(Collider2D collision)
    {
        if (win)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Boundary"))
        {
            animator.SetBool("Damaged", true);
            damageDelay = Time.time;
            activeCharacter.Health -= 3;
            body.velocity = Vector2.zero;
            transform.position = new Vector3(transform.position.x - 2, 8);
            UpdateMeters();
        }
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

    private void UpdateSubweapon()
    {
        switch (activeCharacter.Subweapon)
        {
            case 0:
                subImage.sprite = null;
                subImage.color = new Color(1, 1, 1, 0);
                subText.text = "NO SUBWEAPON";
                break;
            case 1:
                subImage.sprite = spear;
                subImage.color = new Color(1, 1, 1, 1);
                subText.text = "SPEAR";
                break;
            case 2:
                subImage.sprite = hammer;
                subImage.color = new Color(1, 1, 1, 1);
                subText.text = "HAMMER";
                break;
            case 3:
                subImage.sprite = rosary;
                subImage.color = new Color(1, 1, 1, 1);
                subText.text = "ROSARY";
                break;
            case 4:
                subImage.sprite = boomerang;
                subImage.color = new Color(1, 1, 1, 1);
                subText.text = "BOOMERANG";
                break;
            case 5:
                subImage.sprite = magic;
                subImage.color = new Color(1, 1, 1, 1);
                subText.text = "MAGIC";
                break;
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

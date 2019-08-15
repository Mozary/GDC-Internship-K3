using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rb2d;
    public Collider2D col2d;
    public ParticleSystem Particle;
    public TrailRenderer Trail;
    public Animator animator;
    public CharacterController2D controller;

    [SerializeField] private Transform FirePoint;
    [SerializeField] private Transform SlashPoint;
    [SerializeField] private GameObject Arrow;
    [SerializeField] private GameObject Slash;
    [SerializeField] private float runSpeed = 15f;
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float dodgeCooldown = 2f;
    [SerializeField] private float chargeCooldown = 10f;
    [SerializeField] private float healTimeAndCooldown = 1f;

    //HUD Related
    private float Countdown_dodge;
    private float Countdown_heal;
    private float Countdown_charge;
    private int CollectedHerb = 0;
    private float health;
    private bool RangedMode = false;

    float movement = 0f;
    float constantSpeed;
    bool jump = false;
    bool onGround = true;
    bool Immovable = false;
    bool canDodge = true;
    bool invulnerable = false;
    bool blinded = false;
    [HideInInspector]
    public bool jumpDown = false; //bool untuk turun dari sub tilemap

    Coroutine ActiveCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();

        constantSpeed = runSpeed;
        health = maxHealth;
        Countdown_dodge = dodgeCooldown;
        Countdown_heal = healTimeAndCooldown;
        Countdown_charge = chargeCooldown;

    }
    // Update is called once per frame
    void Update()
    {
        if (!Immovable)
        {
            movement = Input.GetAxisRaw("Horizontal") * runSpeed;
        }
        else
        {
            movement = 0;
        }
        animator.SetFloat("Speed", Mathf.Abs(movement));
        if (Input.GetButtonDown("Jump") && !Input.GetKey("down")) //// get key down diganti crounch)
        {
            if (!Immovable)
            {
                jump = true;
                animator.SetTrigger("jumping");
                animator.SetBool("isJumping", true);
            }
        }
        if (Input.GetButtonDown ("Attack")) {
            Attack();
        }
        if (Input.GetButtonDown("Switch") && !CheckPendingAttack())
        {
            RangedMode = !RangedMode;
            animator.SetBool("RangedMode", RangedMode);
        }
        if (animator.GetBool("attack1") || animator.GetBool("attack2") || animator.GetBool("attack3") ||animator.GetBool("Firing"))
        {
            if (!Immovable)
            {
                StartCoroutine("OnCompleteAttackAnimation");
            }
        }
        if (Input.GetButtonDown("Dodge"))
        {
            if(!Immovable && canDodge && !animator.GetBool("isJumping"))
            {
                animator.SetTrigger("isDodging");
                StartCoroutine("Dodge");
            }
        }
        if (Input.GetButtonDown("Heal"))
        {
            if (!Immovable && Countdown_heal>=healTimeAndCooldown && !animator.GetBool("isJumping") && CollectedHerb>0 && health<maxHealth)
            {
                animator.SetTrigger("heal");
                StartCoroutine("Heal");
            }
        }
        if (Input.GetButtonDown("Special"))
        {
            if (!Immovable && Countdown_charge >= chargeCooldown && !animator.GetBool("isJumping") && !RangedMode)
            {
                animator.SetTrigger("isCharging");
                StartCoroutine("Charge");
            }
        }

        //// input untuk turun dari sub tilemap
        ///  pilihan huruf hanya sementara
        if (Input.GetKey("down")/*Input.GetButtonDown("Crounch")*/)
        {
            if (Input.GetKeyDown("up")/*Input.GetButtonDown("Jump")*/)
            {
                jumpDown = true;
            }
        }
    }
    void FixedUpdate()
    {
        if (!col2d.IsTouchingLayers())
        {
            onGround = false;
        }
        controller.Move(movement * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
    public void OnLand()
    {
        if (!onGround && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerJump"))
        {
            animator.SetBool("isJumping", false);
            onGround = true;
        }
    }
    public void Attack()
    {
        if (movement == 0 && !animator.GetBool("isJumping") && !RangedMode)
        {
            
            if (animator.GetBool("attack2") == true)
            {
                animator.SetBool("attack3", true);
            }
            else if (animator.GetBool("attack1") == true)
            {
                animator.SetBool("attack2", true);
            }
            else
            {
                animator.SetBool("attack1", true);
            }
        }
        else if (movement == 0 && !animator.GetBool("isJumping") && RangedMode)
        {
            animator.SetBool("Firing", true);
        }
    }
    public void RangedAttack()
    {
        if (movement == 0 && !animator.GetBool("isJumping") && RangedMode)
        {
            animator.SetBool("Firing", true);
        }

    }
    public bool CheckPendingAttack()
    {
        if (animator.GetBool("attack1") || animator.GetBool("attack2") || animator.GetBool("attack3") || animator.GetBool("Firing"))
        {
            return true;
        }
        else return false;
    }
    IEnumerator Dodge()
    {
        Trail.emitting = true;
        Countdown_dodge = 0;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDodge"))
        {
            yield return null;
        }
        Immovable = true;
        canDodge = false;
        float direction =Mathf.Abs(transform.localScale.x)/transform.localScale.x;
        rb2d.AddForce(new Vector2(direction*-125, -5f));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDodge"))
        {
            yield return null;
        }
        Immovable = false;
        Trail.emitting = false;
        while (Countdown_dodge < dodgeCooldown)
        {
            Countdown_dodge += Time.deltaTime;
            yield return null;
        }
        canDodge = true;
    }
    IEnumerator Charge()
    {
        Immovable = true;
        ParticleSystem.MainModule setting = Particle.main;
        ParticleSystem.MainModule temp = setting;
        setting.startColor = Color.red;
        Particle.Play();

        yield return new WaitForSeconds(1.5f);
        Particle.Stop();
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCharge"))
        {
            Trail.emitting = true;
            invulnerable = true;
            transform.Find("Special").gameObject.SetActive(true);
            this.gameObject.layer = LayerMask.NameToLayer("Enemy");

            float direction = Mathf.Abs(transform.localScale.x) / transform.localScale.x;
            rb2d.AddForce(new Vector2(direction * 300, -2.5f));
            Immovable = false;
            Attack();
            Attack();
            Attack();

            Countdown_charge = 0;
            yield return new WaitForSeconds(3f);
            StartCoroutine(ChargeCooling());
            this.gameObject.layer = LayerMask.NameToLayer("Player");
            transform.Find("Special").gameObject.SetActive(false);
            invulnerable = false;
            Trail.emitting = false;
        }
    }
    IEnumerator ChargeCooling()
    {
        while (Countdown_charge < chargeCooldown)
        {
            Countdown_charge += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator Heal()
    {
        Immovable = true;
        ParticleSystem.MainModule setting = Particle.main;
        ParticleSystem.MainModule temp = setting;
        setting.startColor = Color.green;
        animator.SetBool("stunned", true);
        CollectedHerb -= 1;
        Particle.Play();
        Countdown_heal = 0;
        while (Countdown_heal < healTimeAndCooldown)
        {
            Countdown_heal += Time.deltaTime;
            if (health < maxHealth)
            {
                health += Time.deltaTime*1.5f;
            }
            yield return null;
        }
        Particle.Stop();
        animator.SetBool("stunned", false);
        Immovable = false;
    }
    IEnumerator OnCompleteAttackAnimation()
    {
        Immovable = true;
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerFireBow"))
        {
            Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
            GameObject clone = Instantiate(Slash, SlashPoint.position, Slash.transform.rotation);
            clone.transform.localScale *= SlashDirection.x;
            clone.GetComponent<Rigidbody2D>().velocity = SlashDirection * 5f;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            movement = 0f;
            yield return null;
        }
        Immovable = false;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerFireBow"))
        {
            Vector3 FireDirection = new Vector3(transform.localScale.x, 0,0).normalized;
            GameObject clone =  Instantiate(Arrow, FirePoint.position, Arrow.transform.rotation);
            clone.transform.localScale *= FireDirection.x;
            clone.GetComponent<Rigidbody2D>().velocity= FireDirection*10f;
            animator.SetBool("Firing", false);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack1"))
        {
            animator.SetBool("attack1", false);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack2"))
        {
            animator.SetBool("attack2", false);
            animator.SetBool("attack1", false);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack3"))
        {
            animator.SetBool("attack3", false);
            animator.SetBool("attack2", false);
            animator.SetBool("attack1", false);
        }
    }
    IEnumerator Hurt()
    {
        float flashTime = 0.1f;
        Color mycolour = GetComponent<SpriteRenderer>().color;
        mycolour.g = 0f;
        mycolour.b = 0f;
        GetComponent<SpriteRenderer>().color = mycolour;
        while (flashTime > 0)
        {
            yield return new WaitForSeconds(flashTime);
            flashTime = 0;
        }
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    IEnumerator Stunned()
    {
        animator.SetTrigger("isHurt");
        animator.SetBool("stunned",true);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerHurt") && !animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAirHurt"))
        {
            yield return null;
        }
        Immovable = true;
        StartCoroutine(Hurt());
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        if (health <= 0)
        {
            animator.SetTrigger("isDying");
            runSpeed = 0;
            constantSpeed = 0;
            rb2d.isKinematic = true;
            Collider2D[]cols = this.GetComponents<Collider2D>();
            foreach (Collider2D element in cols)
            {
                element.enabled = false;
            }
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDie"))
            {
                yield return null;
            }
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }
            Color mycolour = GetComponent<SpriteRenderer>().color;
            float colourfade = 1;
            yield return new WaitForSeconds(2f);
            while (colourfade > 0)
            {
                colourfade -= 0.03f;
                mycolour.a = colourfade;
                GetComponent<SpriteRenderer>().color = mycolour;
                yield return null;
            }
            Destroy(this.gameObject);
            Debug.Log("GAME OVER");
        }
        else
        {
            animator.SetBool("stunned", false);
            Immovable = false;
        }
    }
    IEnumerator BlindDamage()
    {
        blinded = true;
        transform.Find("FOV").Find("NearVision").gameObject.SetActive(true);
        transform.Find("FOV").Find("FarVision").gameObject.SetActive(false);

        yield return new WaitForSeconds(10f);

        transform.Find("FOV").Find("NearVision").gameObject.SetActive(false);
        transform.Find("FOV").Find("FarVision").gameObject.SetActive(true);
        blinded = false;
    }
    IEnumerator BurnDamage()
    {
        int counter = 0;
        while(counter < 5)
        {
            TakeDamage(0.3f);
            counter++;
            yield return new WaitForSeconds(0.2f);
        }
    }
    IEnumerator IceDamage()
    {
        runSpeed = constantSpeed / 2;
        yield return new WaitForSeconds(3f);
        runSpeed = constantSpeed;
    }
    public void TakeFireDamage()
    {
        StartCoroutine(BurnDamage());
    }
    public void TakeIceDamage()
    {
        StartCoroutine(IceDamage());
    }
    public void TakeBlindDamage()
    {
        if (!blinded)
        {
            StartCoroutine(BlindDamage());
        }
        
    }
    public void TakeDamage(float damage)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDodge") && !invulnerable && health>0)
        {
            if (ActiveCoroutine != null)
            {
                StopCoroutine(ActiveCoroutine);
            }
            animator.SetBool("attack3", false);
            animator.SetBool("attack2", false);
            animator.SetBool("attack1", false);
            animator.SetBool("Firing", false);
            health -= damage;
            ActiveCoroutine = StartCoroutine("Stunned");
        }
    }
    public float GetNormalizedHealth()
    {
        return health / maxHealth;
    }
    public float GetNormalizedDodgeCooldown()
    {
        return Countdown_dodge / dodgeCooldown;
    }
    public float GetNormalizedHealCooldown()
    {
        return Countdown_heal / healTimeAndCooldown;
    }
    public float GetNormalizedChargeCooldown()
    {
        return Countdown_charge / chargeCooldown;
    }
    public bool GetRangedCheck()
    {
        return RangedMode;
    }
    public int GetHerbAmount()
    {
        return CollectedHerb;
    }
    public void AddCollectedHerb()
    {
        CollectedHerb += 1;
    }
    public void SetHerb(int value)
    {
        CollectedHerb = value;
    }
    public bool IsIdle()
    {
        return (movement == 0);
    }
    public void Freeze()
    {
        Immovable = true;
        invulnerable = true;
    }
    public void UnFreeze()
    {
        Immovable = false;
        invulnerable = false;
    }
}

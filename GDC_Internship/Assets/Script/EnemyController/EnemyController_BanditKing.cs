using System.Collections;
using UnityEngine;

public class EnemyController_BanditKing : MonoBehaviour
{
    enum Weapon
    {
        Ranged,
        Melee
    }

    [SerializeField] private Animator animator;
    [SerializeField] private Transform SlashPoint;
    [SerializeField] private GameObject Slash;
    [SerializeField] private Transform Firepoint;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject Herb;

    [SerializeField] private float maxSpeed = 50;
    [SerializeField] private float health = 40;
    [SerializeField] private float argoRange = 3f;
    [SerializeField] private float meleeRange = 0.55f;
    [SerializeField] private float rangedRange = 4f;

    private Rigidbody2D rb2d;
    private Transform dummyTarget;
    private Transform target = null;
    private Vector3 m_Velocity = Vector3.zero;
    private Weapon Mode = Weapon.Melee;

    private float SmoothMovement = 0.05f;
    private float direction = 1f;
    private float targetDistance;
    private float maxHealth;

    private bool hadap_kanan = true;
    private bool onGround = true;
    private bool AttackCheck = false;
    private bool StunCheck = false;
    private bool Invulnerable = false;
    private bool Immovable = false;
    private bool gotHit = false;

    private Coroutine StunTimer = null;
    private Transform HealthBar;
    

    // Start is called before the first frame update
    void Start()
    {
        dummyTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb2d = this.GetComponent<Rigidbody2D>();
        HealthBar = transform.Find("HealthBar");
        maxHealth = health;
    }
    // Update is called once per frame
    void Update()
    {
        HealthBar.localScale = new Vector3(Mathf.Clamp(health / maxHealth, 0, maxHealth), HealthBar.localScale.y, HealthBar.localScale.z);
        if (target == null && dummyTarget)
        {
            float dummyDistance = Vector2.Distance(transform.position, dummyTarget.position);
            if (dummyDistance <= argoRange)
            {
                target = dummyTarget;
                dummyTarget = null;
            }
        }
        if (target != null)
        {
            targetDistance = Vector2.Distance(transform.position, target.position);
            
            if(targetDistance > rangedRange && !StunCheck && !AttackCheck && !Immovable && Mode != Weapon.Ranged)
            {
                StartCoroutine(SwitchWeapon());
            }
            else if (targetDistance <= rangedRange && Mode ==  Weapon.Ranged && !StunCheck && !AttackCheck && !Immovable)
            {
                animator.SetBool("attack", true);
                StartCoroutine(RangedAttack());
            }
            else if (targetDistance <= meleeRange && Mode == Weapon.Melee &&  !StunCheck && !AttackCheck && !Immovable)
            {
                animator.SetBool("attack", true);
                StartCoroutine(MeleeAttack());
            }
        }
    }
    void FixedUpdate()
    {
        if (rb2d != null && !AttackCheck && !StunCheck && target != null && !Immovable)
        {
            CloseDistance();
            if (target.position.x > transform.position.x && !hadap_kanan) Flip();
            else if (target.position.x < transform.position.x && hadap_kanan) Flip();
        }
        else
        {
            animator.SetFloat("speed", 0f);
        }
    }
    void CloseDistance()
    {
        if (targetDistance > meleeRange && !StunCheck & !AttackCheck && !Immovable)
        {
            Vector3 moveVelocity = new Vector2(direction * maxSpeed * Time.deltaTime, rb2d.velocity.y);
            animator.SetFloat("speed", Mathf.Abs(moveVelocity.x));
            rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, moveVelocity, ref m_Velocity, SmoothMovement);
        }
        else { animator.SetFloat("speed", 0f); }
    }
    private void Flip()
    {
        hadap_kanan = !hadap_kanan;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        direction *= -1;
    }
    IEnumerator MeleeAttack()
    {
        AttackCheck = true;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("KingMeleeAttack"))
        {
            yield return null;
        }
        Invulnerable = true;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <0.25)
        {

            yield return null;
        }

        Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
        GameObject clone = Instantiate(Slash, SlashPoint.position, Slash.transform.rotation);
        clone.transform.localScale *= SlashDirection.x;
        clone.GetComponent<Rigidbody2D>().velocity = SlashDirection * 2f;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            yield return null;
        }
        Invulnerable = false;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            if (StunCheck)
            {
                AttackCheck = false;
                animator.SetBool("attack", false);
                yield break;
            }
            yield return null;
        }
        AttackCheck = false;
        animator.SetBool("attack", false);
    }

    IEnumerator RangedAttack()
    {
        AttackCheck = true;
        if (animator.GetBool("loaded") == false)
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("KingReload"))
            {
                yield return null;
            }
            while (animator.GetCurrentAnimatorStateInfo(0).IsName("KingReload"))
            {
                if (StunCheck && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5)
                {
                    AttackCheck = false;
                    animator.SetBool("attack", false);
                    yield break;
                }
                yield return null;
            }
            animator.SetBool("loaded", true);
        }
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("KingRangedAttack"))
        {
            yield return null;
        }
        Invulnerable = true;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime<0.5)
        {
            yield return null;
        }
        float direction = Mathf.Abs(transform.localScale.x) / transform.localScale.x;
        rb2d.AddForce(new Vector2(direction * -125, -100f));

        Vector3 FireDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
        GameObject clone = Instantiate(Bullet, Firepoint.position, Firepoint.transform.rotation);
        clone.GetComponent<Rigidbody2D>().velocity = FireDirection * 25f;
        Invulnerable = false;
        animator.SetBool("loaded", false);

        if (targetDistance <= meleeRange + 1f)
        {
            StartCoroutine(SwitchWeapon());
        }
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("KingRangedAttack")|| animator.GetCurrentAnimatorStateInfo(0).IsName("KingReload"))
        {
            if (StunCheck)
            {
                AttackCheck = false;
                animator.SetBool("attack", false);
                yield break;
            }
            yield return null;
        }
        AttackCheck = false;
        animator.SetBool("loaded", true);
        animator.SetBool("attack", false);
    }

    IEnumerator SwitchWeapon()
    {
        Immovable = true;
        animator.SetTrigger("switch");
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("KingSwitchToRanged") && !animator.GetCurrentAnimatorStateInfo(0).IsName("KingSwitchToMelee"))
        {
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            if (StunCheck)
            {
                Immovable = false;
                yield break;
            }
            yield return null;
        }
        Invulnerable = true;
        if (Mode == Weapon.Melee)
        {
            animator.SetBool("ranged", true);
            Mode = Weapon.Ranged;
        }
        else if (Mode == Weapon.Ranged)
        {
            animator.SetBool("ranged", false);
            Mode = Weapon.Melee;
        }
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("KingMeleeIdle") && !animator.GetCurrentAnimatorStateInfo(0).IsName("KingRangedIdle"))
        {
            yield return null;
        }
        Immovable = false;
        Invulnerable = false;
        yield return null;
    }

    IEnumerator Hurt()
    {
        float flashTime = 0.05f;
        Color mycolour = GetComponent<SpriteRenderer>().color;
        mycolour.g = 0f;
        mycolour.b = 0f;
        GetComponent<SpriteRenderer>().color = mycolour;
        yield return new WaitForSeconds(flashTime);
        GetComponent<SpriteRenderer>().color = Color.white;
        Invulnerable = false;
}
    IEnumerator Stunned()
    {
        StunCheck = true;
        animator.SetTrigger("isHurt");
        StartCoroutine(Hurt());
        if (health <= 0 && Immovable)
        {
            Immovable = true;
            DropHerb();DropHerb();DropHerb();DropHerb();DropHerb();
            animator.SetTrigger("isDying");
            rb2d.isKinematic = true;
            Collider2D[] cols = this.GetComponents<Collider2D>();
            foreach (Collider2D element in cols)
            {
                element.enabled = false;
            }
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("KingMeleeDie") && !animator.GetCurrentAnimatorStateInfo(0).IsName("KingRangedDie"))
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
        }
        else
        {
            while(!animator.GetCurrentAnimatorStateInfo(0).IsName("KingMeleeHurt") && !animator.GetCurrentAnimatorStateInfo(0).IsName("KingRangedHurt"))
            {
                yield return null;
            }
            while (animator.GetCurrentAnimatorStateInfo(0).IsName("KingMeleeHurt") || animator.GetCurrentAnimatorStateInfo(0).IsName("KingRangedHurt"))
            {
                yield return null;
            }
            StunCheck = false;
        }
    }
    private void DropHerb()
    {
        Vector3 DropDirection = new Vector3(-transform.localScale.x, 1, 0).normalized;
        GameObject clone = Instantiate(Herb, transform.position, transform.rotation);
        clone.GetComponent<Rigidbody2D>().velocity = DropDirection * 1f;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack"  && !Invulnerable)
        {
            Invulnerable = true;
            health -= 1;
            if (StunTimer != null)
            {
                StopCoroutine(StunTimer);
            }
            StunTimer = StartCoroutine(Stunned());
        }
    }
}

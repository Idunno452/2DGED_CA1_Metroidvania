using UnityEngine;

public class Enemies : MonoBehaviour
{
    [SerializeField] private float walkTime;
    [SerializeField] private float idleTime;
    [SerializeField] private GameObject projectile;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private float RayCastHeight = 0f;
    private bool attacking = false;
    private float attackCooldown = 0;
    private int direction = 1;
    private bool isWalking = false;
    private Rigidbody2D rb;
    private Animator animator;
    private float elapsedTime;
    [SerializeField] private int strength = 3;
    private bool isInvincible;
    private float invincibleCount;
    private bool pause = false;
    private AudioSource aSource;
    private bool takingDamage = false;
    private float damageTime = 0;
    private float deathTime = 0;
    private bool dead = false;
    private float attackCountdown = 0;   // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
       animator = GetComponent<Animator>();
       aSource = GetComponent<AudioSource>();
       animator.SetInteger("direction", direction);
       animator.SetFloat("move", 0);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D inRangeForward = Physics2D.Raycast(rb.transform.position, new Vector2(direction, RayCastHeight), 10f, LayerMask.GetMask("Player"));
        RaycastHit2D inRangeBackward = Physics2D.Raycast(rb.transform.position, new Vector2(direction*-1, RayCastHeight), 10f, LayerMask.GetMask("Player"));
        if(inRangeForward.collider != null)
        {
            if(inRangeForward.collider.GetComponent<PlayerScript>() != null && !pause && !dead)
            {
                Vector2 position = rb.transform.position;
                position.x += (3*direction) * Time.deltaTime;
                rb.transform.position = position;
            }
        }
        else if(inRangeBackward.collider != null)
        {
            if(inRangeBackward.collider.GetComponent<PlayerScript>() != null)
            {
                direction *= -1;
                animator.SetInteger("direction", direction);
                animator.SetFloat("move", direction);
            }
        }
        else
        {
            if (isWalking && elapsedTime < walkTime)
            {
                Vector2 position = rb.transform.position;
                position.x += (2*direction) * Time.deltaTime;
                rb.transform.position = position;
            }
            else if(!isWalking && elapsedTime > idleTime)
            {
                isWalking = true;
                elapsedTime = 0;
                direction *= -1;
                animator.SetInteger("direction", direction);
                animator.SetFloat("move", direction);
                aSource.Play();
            }
            else if(isWalking && elapsedTime > walkTime)
            {
                isWalking = false;
                elapsedTime = 0;
                animator.SetFloat("move", 0);
                aSource.Pause();
            }
            elapsedTime += Time.deltaTime;
        }

        if(isInvincible)
        {
            invincibleCount += Time.deltaTime;
            if(invincibleCount > 0.4f)
            {
                isInvincible = false;
                invincibleCount = 0;
            }
        }

        if(attacking)
        {   
            attackCooldown += Time.deltaTime;
            if(attackCooldown > 0.45)
            {
                pause = false;
                animator.SetBool("attacking",false);
            }
            if(attackCooldown > 2)
            {
                attacking = false;
                attackCooldown = 0;
            }
        }
        if(takingDamage)
        {   
            damageTime += Time.deltaTime;
            if(damageTime > 0.45)
            {
                takingDamage = false;
                animator.SetBool("takingDamage",false);
                damageTime = 0;
            }
        }
        if(dead)
        {
            deathTime += Time.deltaTime;
            if(deathTime > 1)
            {
                Destroy(this.gameObject);
                return;
            }
        }
    }
    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, new Vector2(direction, 0), 4f, LayerMask.GetMask("Player"));
        if(hit.collider != null)
        {
            if(hit.collider.GetComponent<PlayerScript>() != null && !attacking && attackCountdown > 1f)
            {
                aSource.PlayOneShot(attackSound);
                attack();
                attackCooldown = 0;
            }
            attackCountdown += Time.fixedDeltaTime;
        }
        else
        {
            attackCountdown = 0;
        }
        
    }
    private void attack()
    {
        animator.SetBool("attacking",true);
        attacking = true;
        pause = true;
        GameObject go = Instantiate(projectile, rb.transform.position, Quaternion.identity);
        EnemyAttack pro = go.GetComponent<EnemyAttack>();
        pro.Launch(new Vector2(direction, 1), 0);
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player_Attack" && !isInvincible)
        {
            hit();
            Vector2 away = collision.transform.position - transform.position;
            away.y = 0f;
            rb.AddForce(away*-1, ForceMode2D.Impulse);
            attackCountdown = 0;
        }
    }
    
    private void hit()
    {
        strength--;
        animator.SetBool("takingDamage",true);
        takingDamage = true;
        if(strength <= 0)
        {
            animator.SetBool("dead",true);
            dead = true;
        }
        isInvincible = true;
    }
}

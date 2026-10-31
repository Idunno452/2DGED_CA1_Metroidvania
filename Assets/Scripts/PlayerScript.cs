using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public int speed = 2;
    private Vector2 move;
    private Rigidbody2D rb;
    private int direction = 1;
    private bool jumpApex = false;
    private Animator animator;
    public float jumpHeight = 2.4f;
    private bool isJumping = false;
    private bool isFalling = false;
    private int score = 0;
    private int jump = 0;
    private int lives = 3;
    private Vector2 lastPosition;
    private AudioSource aSource;
    private bool playing = false;
    private bool swordCharge = false;
    private bool wallJump = false;
    private bool takingDamage = false;
    private float damageTime = 0;
    private bool grounded = false;
    private bool isInvincible = false;
    private float invincibleCount = 0;
    private bool attacking = false;
    private bool attacking2 = false;
    private float attackCooldown = 0;
    private double heldTime = 0;
    public bool chargeLvl2 = false;
    public bool chargeLvl3 = false;
    public InputAction attackAction;
    private bool helded = false;
    private bool unhelded = false;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameManager gm;
    //[SerializeField] private AudioClip throwSound;
    //[SerializeField] private AudioClip collectSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        lastPosition = rb.transform.position;
        aSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (InputSystem.actions["Jump"].IsPressed() && jump <= 1)
        {
            InputSystem.actions["Jump"].Reset();
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(0, Mathf.Sqrt(-2 * Physics2D.gravity.y * jumpHeight)), ForceMode2D.Impulse);
            isJumping = true;
            jumpApex = false;
            grounded = false;
            jump++;
        }
        
        if(InputSystem.actions["Attack"].IsPressed() && !swordCharge)
        {
            Attack();
        }
        
        if(helded && swordCharge)
        {
            heldTime += Time.deltaTime;
            if(heldTime > 0.5 && heldTime < 1)
            {
                chargeLvl2 = true;
                animator.SetBool("Charge2",true);
            }
            else if(heldTime > 1)
            {
                chargeLvl3 = true;
                animator.SetBool("Charge3",true);
            }
        }
        if(unhelded && swordCharge)
        {
            unhelded = false;
            heldTime = 0;
            Attack();
        }
        
        move = InputSystem.actions["Move"].ReadValue<Vector2>();
        if(move.x != 0)
        {
            direction = move.x < 0 ? -1 : 1;
            animator.SetInteger("Direction",direction);
        
        }
        
        if(jumpApex && !grounded)
        {
            animator.SetBool("Fall",true);
            animator.SetBool("Jump",false);
        }
        else if(rb.linearVelocity.magnitude != 0 && !grounded)
        {
            animator.SetBool("Fall",false);
            animator.SetBool("Jump",true);
            if(rb.linearVelocity.magnitude < 0.5)
            {
                jumpApex = true;
            }
        }
        if(isInvincible)
        {
            invincibleCount += Time.deltaTime;
            if(invincibleCount > 1)
            {
                isInvincible = false;
                invincibleCount = 0; 
            }
        }
        
        if(chargeLvl3 && attacking)
        {
            attackCooldown += Time.deltaTime;
            if(attackCooldown > 0.4)
            {
                animator.SetBool("Charge3",false);
                animator.SetBool("Charge2",false);
                animator.SetBool("Attacking",false);
            }
            if(attackCooldown > 1.2)
            {
                chargeLvl3 = false;
                chargeLvl2 = false;
                attacking = false;
                attackCooldown = 0;
            }
        }
        else if(chargeLvl2 && attacking)
        {
            attackCooldown += Time.deltaTime;
            if(attackCooldown > 0.4)
            {
                animator.SetBool("Charge2",false);
                animator.SetBool("Attacking",false);
            }
            if(attackCooldown > 1.2)
            {
                chargeLvl2 = false;
                attacking = false;
                attackCooldown = 0;
            }
        }
        else if(attacking)
        {   
            attackCooldown += Time.deltaTime;
            if(attackCooldown > 0.4 && !attacking2)
            {
                if(InputSystem.actions["Attack"].IsPressed() && !attacking2)
                {
                    Attack2();
                }
                else
                {
                    animator.SetBool("Attacking",false);
                    attacking = false;
                    attackCooldown = 0;
                }
            }
            if(attackCooldown > 0.95)
            {
                animator.SetBool("Attacking",false);
                animator.SetBool("Attacking2",false);
            }
            if(attackCooldown > 1.2)
            {
                attacking = false;
                attacking2 = false;
                attackCooldown = 0;
            }
        }
   
        animator.SetFloat("Move", move.x);
        if (takingDamage)
        {   
            damageTime += Time.deltaTime;
            if(damageTime > 0.45)
            {
                takingDamage = false;
                animator.SetBool("TakingDamage",false);
                damageTime = 0;
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rb.transform.position;
        position.x += (move.x*speed) * Time.fixedDeltaTime;
        rb.transform.position = position;
    }
    private void Attack()
    {
        if(chargeLvl3)
        {
            animator.SetBool("Charge3",true);
            animator.SetBool("Attacking",true);
            animator.SetBool("Charging",false);
            attacking = true;
            GameObject go = Instantiate(projectile, rb.transform.position, Quaternion.identity);
            PlayerAttack pro = go.GetComponent<PlayerAttack>();
            pro.Launch(new Vector2(direction, 1), 0);
        }
        else if(chargeLvl2)
        {
            animator.SetBool("Charge2",true);
            animator.SetBool("Attacking",true);
            animator.SetBool("Charging",false);
            attacking = true;
            GameObject go = Instantiate(projectile, rb.transform.position, Quaternion.identity);
            PlayerAttack pro = go.GetComponent<PlayerAttack>();
            pro.Launch(new Vector2(direction, 1), 0);
        }
        else
        {
            animator.SetBool("Attacking",true);
            animator.SetBool("Charging",false);
            attacking = true;
            GameObject go = Instantiate(projectile, rb.transform.position, Quaternion.identity);
            PlayerAttack pro = go.GetComponent<PlayerAttack>();
            pro.Launch(new Vector2(direction, 1), 0);
        }
    }
    private void Attack2()
    {
        animator.SetBool("Attacking2",true);
        attacking2 = true;
        GameObject go = Instantiate(projectile, rb.transform.position, Quaternion.identity);
        PlayerAttack pro = go.GetComponent<PlayerAttack>();
        pro.Launch(new Vector2(direction, 1), 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground" || grounded)
        {
            isJumping = false;
            animator.SetBool("Jump",false);
            isFalling = false;
            animator.SetBool("Fall",false);
            jump = 0;
            jumpApex=false;
        }
        if(collision.gameObject.tag == "Wall" && wallJump)
        {
            isJumping = false;
            animator.SetBool("Jump",false);
            isFalling = false;
            animator.SetBool("Fall",false);
            jump = 0;
            jumpApex=false;
        }

        if((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Enemy1_Attack") && !isInvincible && !attacking)
        {
            lives--;
            takingDamage = true;
            grounded = true;
            isInvincible = true;
            animator.SetBool("TakingDamage",true);
            gm.updateLives(lives);
            if(lives == 0)
                {
                    SceneManager.LoadScene("MenuScene");
                }
            Vector2 away = collision.transform.position - transform.position;
            away.y = 0f;
            rb.AddForce(away*-1, ForceMode2D.Impulse);
        }
        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Boulder" && chargeLvl3 && attacking)
            {
                Destroy(collision.gameObject);
            }
    }
    public void ChargeGain()
    {
        swordCharge = true;
    }
    public void WallJumpGain()
    {
        wallJump = true;
    }
    private void AttackTime(InputAction.CallbackContext context)
    {
        if(context.started && swordCharge)
        {
            helded = true;
            animator.SetBool("Charging",true);
        }
        if(context.canceled)
        {
            helded = false;
            unhelded = true;
        }
    }
    private void OnEnable()
    {   
        attackAction = InputSystem.actions["Attack"];

        attackAction.started += AttackTime;
        attackAction.canceled += AttackTime;
    }
    private void OnDisable()
    {
        if(attackAction != null)
        {
            attackAction.started -= AttackTime;
            attackAction.canceled -= AttackTime;
        
        }
    }
}

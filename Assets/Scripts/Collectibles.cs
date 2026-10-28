using UnityEngine;

public class Collectibles : MonoBehaviour
{
    [SerializeField] PlayerScript player;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && this.tag == "swordCharge")
        {
            animator.SetBool("IsOpened",true);
            player.ChargeGain();
        }
        if(other.gameObject.tag == "Player" && this.tag == "wallJump")
        {
            animator.SetBool("IsOpened",true);
            player.WallJumpGain();
        }
    }
}

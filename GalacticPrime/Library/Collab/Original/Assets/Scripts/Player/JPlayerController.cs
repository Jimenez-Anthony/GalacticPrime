using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPlayerController : MonoBehaviour, IDeathController
{
    [Header("Movement")]
    public float speed;
    public float jumpForce = 24f;
    public float lowJumpForce = 6f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public bool airSteer = true;
    public float movementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;
    private float counterJumpForce = -3f;
    public  bool isJumping = false;
    public bool jumpKeyHeld = false;

    [Space(10)]
    [Header("Attack")]
    public float jabCooldown = 0.5f;
    public float jabDistance = 1f;
    public int jabDamage = 1;
    private float jabTimer = 0f;

    private Rigidbody2D rb2d;
    private Animator anim;

    // Movement assistance variables
    private float inputHorizontal;
    private float inputVertical;
    private bool grounded;

    // Flip variables
    private bool facingRight = true;

    // Ladder
    [Space(10)]
    [Header("Interactions")]
    private bool isClimbing;
    private float gravityScale;
    public LayerMask ladderLayer;

    private bool dead = false;

    public float knockBackAmount = 0f;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        gravityScale = rb2d.gravityScale;
    }

    void FixedUpdate()
    {
        if (dead)
            return;

        //GroundCheck();

        if (isJumping) {
            if (!jumpKeyHeld && rb2d.velocity.y > 0) {
                rb2d.AddForce(new Vector2(0f, counterJumpForce), ForceMode2D.Impulse);
            }
            if (rb2d.velocity.y < 0) {
                isJumping = false;
            }
        }

        if (grounded || airSteer) {
            inputHorizontal = Input.GetAxisRaw("Horizontal");

            if (inputHorizontal > 0 && !facingRight) {
                Flip();
            }
            else if (inputHorizontal < 0 && facingRight) {
                Flip();
            }
        }

        float verticalKnockback = knockBackAmount / 2f;

        //if (grounded) {
        //    rb2d.velocity = new Vector2(inputHorizontal * speed * Time.fixedDeltaTime + knockBackAmount, rb2d.velocity.y + verticalKnockback);
        //}
        //else {
        //    rb2d.velocity = new Vector2(inputHorizontal * (speed / 1.5f) * Time.fixedDeltaTime + knockBackAmount, rb2d.velocity.y + verticalKnockback);
        //}
        Vector2 targetVel = new Vector2(inputHorizontal * speed * Time.fixedDeltaTime + knockBackAmount, rb2d.velocity.y + verticalKnockback);
        rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, targetVel, ref m_Velocity, movementSmoothing);

        RaycastHit2D hitInfo = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), Vector2.up, 0f, ladderLayer);

        if (hitInfo.collider != null) {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) {
                isClimbing = true;
            }
        }
        else {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
                isClimbing = false;
            }
        }

        if (isClimbing && hitInfo.collider != null) {
            inputVertical = Input.GetAxisRaw("Vertical");
            rb2d.velocity = new Vector2(rb2d.velocity.x, inputVertical *(speed / 2) * Time.fixedDeltaTime);
            rb2d.gravityScale = 0;
        }
        else {
            rb2d.gravityScale = gravityScale;
        }

        knockBackAmount = 0f;
    }

    void Update() {
        if (dead)
            return;

        GroundCheck();

        if (jabTimer > 0)
            jabTimer -= Time.deltaTime;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
            anim.SetBool("IsWalking", true);
        }
        else {
            anim.SetBool("IsWalking", false);
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            jumpKeyHeld = true;
        }
        else if (Input.GetKeyUp(KeyCode.W)) {
            jumpKeyHeld = false;
        }

        if (grounded && !isClimbing && Input.GetKeyDown(KeyCode.W)) {
            StartCoroutine(Jump());
        }

        // Jab
        if (jabTimer <= 0f && Input.GetKeyDown(KeyCode.J)) {
            jabTimer = jabCooldown;
            StartCoroutine(Jab());
        }
    }

    void GroundCheck() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.GetChild(0).position, 0.2f, groundLayer);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].transform.gameObject != gameObject) {
                print(colliders[i].transform.name);
                isJumping = false;
                grounded = true;
                return;
            }
        }
        grounded = false;
    }

    IEnumerator Jump() {
        anim.SetBool("IsJumping", true);
        isJumping = true;
        grounded = false;
        AudioManager.instance.Play("PlayerSlash");
        //if (Input.GetKey(KeyCode.LeftShift)) {
        //    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, lowJumpForce), ForceMode2D.Impulse);
        //}
        //else {
        //    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        //}
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        yield return new WaitForSeconds(.8f);
        anim.SetBool("IsJumping", false);
    }

    IEnumerator Jab() {
        anim.SetBool("IsJabbing", true);
        yield return new WaitForSeconds(0.6f);
        Vector3 dir;
        if (facingRight) {
            dir = Vector3.right;
        }
        else {
            dir = Vector3.left;
        }
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(transform.GetChild(1).position, dir, jabDistance);
        foreach (RaycastHit2D hitInfo in hitInfos) {
            //print(hitInfo.transform.gameObject.name);
            if (hitInfo.transform == transform || hitInfo.transform == transform.GetChild(1)) {
                continue;
            }
            JHealthController health = hitInfo.transform.gameObject.GetComponent<JHealthController>();
            if (health == null) {
                continue;
            }
            else {
                health.TakeDamage(jabDamage);
                break;
            }
        }
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("IsJabbing", false);
    }

    void Flip() {
        facingRight = !facingRight;
        float xScale = transform.localScale.x;
        transform.localScale = new Vector3(-xScale, transform.localScale.y, transform.localScale.z);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        grounded = collision.gameObject.tag == "Ground";
    }

    void OnCollisionExit2D(Collision2D collision) {
        grounded = collision.gameObject.tag != "Ground";
    }

    public void TakeKnockback(float amount) {
        knockBackAmount = amount;
    }

    public void OnDeath() {
        dead = true;
        rb2d.velocity = Vector3.zero;
        anim.SetBool("IsAlive", false);
        if (GameMaster.instance.playerRespawns) {
            StartCoroutine(DebugRespawn());
        }
        else {
            Destroy(gameObject, 1.5f);
        }

    }

    IEnumerator DebugRespawn() {
        yield return new WaitForSeconds(1.5f);
        GetComponent<JHealthController>().hp = GetComponent<JHealthController>().maxHP;
        dead = false;
        anim.SetBool("IsAlive", true);

        //if (GameMaster.instance.generateRandomMap) {
        //    GameMaster.instance.levelGenerator.GenerateRandomMap();
        //}
        //else {
        //    transform.position = new Vector3(8.68f, -0.71f, 0);
        //}

        transform.position = new Vector3(8.68f, -0.71f, 0);


    }
}

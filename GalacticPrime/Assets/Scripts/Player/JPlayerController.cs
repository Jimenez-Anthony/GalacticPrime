using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPlayerController : MonoBehaviour, IDeathController, IGetsHurt
{
    public bool pause = false;

    [Header("Movement")]
    public float speed;
    public float jumpForce;
    public float jumpTime;
    public float lowJumpForce;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public bool airSteer = true;
    public float movementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;

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
    public bool grounded;
    private bool isJumping;
    private float jumpTimer;

    // Flip variables
    public bool facingRight = true;

    // Ladder
    [Space(10)]
    [Header("Interactions")]
    public bool isClimbing;
    private float gravityScale;
    public LayerMask ladderLayer;

    private bool dead = false;

    public float knockBackAmount = 0f;

    // Items
    public Inventory inventory;
    public bool holding = false;
    private float useItemCD = 0.2f;
    private float useItemTimer;

    // Fall Damage
    private float fallingDuration;
    float animSpeed;


    // Knock Back
    private bool gettingKnockedBack = false;
    public bool stunned = false;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();

        gravityScale = rb2d.gravityScale;
        useItemTimer = useItemCD;
        animSpeed = anim.speed;
    }

    void FixedUpdate()
    {
        if (dead)
            return;

        if (pause)
            return;

        //GroundCheck();

        int horizontal = 0;
        if (Input.GetKey(Keys.left) && Input.GetKey(Keys.right)) {
            horizontal = 0;
        }
        else if (Input.GetKey(Keys.left)) {
            horizontal = -1;
        }
        else if (Input.GetKey(Keys.right)) {
            horizontal = 1;
        }

        if (grounded || airSteer) {
            //inputHorizontal = Input.GetAxisRaw("Horizontal");
            inputHorizontal = horizontal;

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
        if (!gettingKnockedBack && !stunned)
            rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, targetVel, ref m_Velocity, movementSmoothing);

        RaycastHit2D hitInfo = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), Vector2.up, 0f, ladderLayer);

        if (!stunned && hitInfo.collider != null) {
            if (Input.GetKeyDown(Keys.ladderUp) || Input.GetKeyDown(Keys.ladderDown)) {
                anim.SetBool("IsClimbing", true);
                isClimbing = true;
            }
        }
        else {
            if (Input.GetKey(Keys.left) || Input.GetKey(Keys.right) || stunned) {
                anim.SetBool("IsClimbing", false);
                isClimbing = false;
            }
        }

        int vertical = 0;
        if (Input.GetKey(Keys.ladderUp) && Input.GetKey(Keys.ladderDown)) {
            vertical = 0;
        }
        else if (Input.GetKey(Keys.ladderDown)) {
            vertical = -1;
        }
        else if (Input.GetKey(Keys.ladderUp)) {
            vertical = 1;
        }

        if (isClimbing && hitInfo.collider != null) {
            //inputVertical = Input.GetAxisRaw("Vertical");
            inputVertical = vertical;
            rb2d.velocity = new Vector2(rb2d.velocity.x, inputVertical *(speed / 2) * Time.fixedDeltaTime);
            rb2d.gravityScale = 0;
        }
        else {
            rb2d.gravityScale = gravityScale;
        }

        knockBackAmount = 0f;

        //// Jump
        //if (jump)
        //{
        //    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        //    jump = false;
        //}
        //if (jumpCancel)
        //{
        //    if (rb2d.velocity.y > lowJumpForce)
        //    {
        //        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, lowJumpForce), ForceMode2D.Impulse);
        //        jumpCancel = false;
        //    }

        //}

    }

    public IEnumerator KnockbackDuration(float duration) {
        gettingKnockedBack = true;
        yield return new WaitForSeconds(duration);
        gettingKnockedBack = false;
    }

    public IEnumerator StunDuration(float duration) {
        stunned = true;
        //float animSpeed = anim.speed;
        anim.speed = 0;
        yield return new WaitForSeconds(duration);
        stunned = false;
        anim.speed = animSpeed;
    }

    void Update() {
        if (dead)
            return;

        if (pause)
            return;

        GroundCheck();

        if (jabTimer > 0)
            jabTimer -= Time.deltaTime;

        if (useItemTimer > 0) {
            useItemTimer -= Time.deltaTime;
        }

        if (Input.GetKey(Keys.left) || Input.GetKey(Keys.right)) {
            anim.SetBool("IsWalking", true);
        }
        else {
            anim.SetBool("IsWalking", false);
        }

        // Falling
        //if (rb2d.velocity.y < 0) {
        //    fallingDuration += Time.deltaTime;
        //}
        //else {
        //    if (fallingDuration > 1f) {
        //        GetComponent<JHealthController>().TakeDamage((int)((fallingDuration - 1f) * 5f));
        //    }
        //    fallingDuration = 0;
        //}

        //// Jump
        //if (grounded && Input.GetKeyDown(KeyCode.Space)) {
        //    jump = true;
        //    StartCoroutine(Jump());
        //}
        //if (!grounded && Input.GetKeyUp(KeyCode.Space))
        //{
        //    jumpCancel = true;
        //}

        // Jab
        if (!holding && !stunned && jabTimer <= 0f && inventory.selectedSlot == -1 && Input.GetKeyDown(Keys.useItem)) {
            jabTimer = jabCooldown;
            StartCoroutine(Jab());
        }

        // Use Item
        if (!stunned && useItemTimer <= 0f && inventory.selectedSlot != -1 && Input.GetKeyDown(Keys.useItem)) {
            useItemTimer = useItemCD;
            if (transform.GetChild(3).GetChild(0).GetComponent<IItem>() != null) {
                transform.GetChild(3).GetChild(0).GetComponent<IItem>().Use();
            }
        }

        if (!stunned && grounded && Input.GetKeyDown(Keys.jump)) {
            anim.SetBool("IsJumping", true);
            GameMaster.instance.gameStats.jumpTimes++;
            AudioManager.instance.Play("PlayerSlash");
            isJumping = true;
            isClimbing = false;
            anim.SetBool("IsClimbing", false);
            jumpTimer = jumpTime;
            rb2d.velocity = Vector2.up * jumpForce;
        }

        if (!stunned && Input.GetKey(Keys.jump) && isJumping) {
            if (jumpTimer > 0f) {
                rb2d.velocity = Vector2.up * jumpForce;
                jumpTimer -= Time.deltaTime;
            }
            else {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(Keys.jump)) {
            isJumping = false;
            anim.SetBool("IsJumping", false);
        }
    }

    void GroundCheck() {
        //RaycastHit2D[] colliders = Physics2D.RaycastAll(transform.GetChild(0).position, Vector2.down, 0.1f, groundLayer);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.GetChild(0).position, 0.23f, groundLayer);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].transform.gameObject != gameObject) {
                grounded = true;
                return;
            }
        }
        grounded = false;
    }

    IEnumerator Jump() {
        anim.SetBool("IsJumping", true);
        grounded = false;
        AudioManager.instance.Play("PlayerSlash");
        yield return new WaitForSeconds(.8f);
        anim.SetBool("IsJumping", false);
    }
    //GetComponent<Rigidbody2D>().AddForce(new Vector2(0, lowJumpForce), ForceMode2D.Impulse);
    IEnumerator Jab() {
        anim.SetBool("IsJabbing", true);
        yield return new WaitForSeconds(0.2f);
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
        if (!dead) {
            dead = true;
            rb2d.velocity = Vector3.zero;
            anim.SetBool("IsAlive", false);
            StartCoroutine(LevelOver());
        }
    }

    public void OnHurt() {
        StartCoroutine(Hurt());
    }

    IEnumerator Hurt() {
        //print("player hurt");
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.7f);
        yield return new WaitForSeconds(0.15f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }

    IEnumerator LevelOver() {
        yield return new WaitForSeconds(1.5f);

        if (GameMaster.instance.playerRespawns) {
            GetComponent<JHealthController>().hp = 10;
            dead = false;
            anim.SetBool("IsAlive", true);
            transform.position = GameMaster.instance.playerRespawnPoint;
            //GameMaster.instance.levelGenerator.GenerateRandomMap();
        }
        else {
            AudioManager.instance.StopSounds();
            AudioManager.instance.Play("MinecraftHurt");
            AudioManager.instance.Play("MinecraftDeathMusic");
            GameMaster.instance.GameOver();
        }
    }


}

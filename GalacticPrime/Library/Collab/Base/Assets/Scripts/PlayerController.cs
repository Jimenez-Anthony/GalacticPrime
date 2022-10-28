using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Direction { LEFT, RIGHT };
public class PlayerController : MonoBehaviour {
    [UnityEngine.HideInInspector]
    public bool pause = false;
    [UnityEngine.HideInInspector]
    public int gameStage = 0;

    public PlayerHealth playerCurrentHealth;
    public float ammo;
    public Image ammoImage;

    public float speed = 5.0f;
    public float jumpPower = 7.0f;

    public bool grounded = false;
    public LayerMask ground;

    public Animator anim;

    public GameObject bulletToRight, bulletToLeft;
    Vector2 bulletPos;
    public float firerate = 0.1f;
    private float nextFire = 0.0f;

    public GameObject slashRight, slashLeft;
    public float slashCooldown = 0.5f;
    private float slashTimer;

    private Transform _transform;
    private Rigidbody2D _rigidbody;
    private Direction playerDirection = Direction.RIGHT;
    private bool facingRight;

    public AudioClip shootSound;
    public AudioClip jumpSound;
    public AudioClip slashSound;
    private AudioSource source;

    private Rigidbody2D rb;
    [UnityEngine.HideInInspector]
    public float dashLength;

    public Direction PlayerDirection {
        get{
            return playerDirection;
        }
    }

	// Use this for initialization
	void Start () {
        _transform = GetComponent(typeof(Transform)) as Transform;
        _rigidbody = GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        ammo = 20;
        anim.SetBool("IsAlive", true);
        source = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        slashTimer = slashCooldown;
    }
	
	// Update is called once per frame
	void Update()
	{
        if (pause) return;

        //MovePlayer();
        Jump();
        Shoot();
        Flip();
        Jab();

       /* if(playerCurrentHealth  0f)
        {
            health -= dmg;
            if (health <= 0)
            {
                anim.SetBool("IsAlive", false);  // dying when health 0
                Destroy(gameObject, (float).8);
            }
        } */
    }

    // JIMMY - Player movement is now controlled under fixed update, since you are using physics to control movement
    void FixedUpdate() {
        if (pause) return;
        MovePlayer();
    }

    void MovePlayer(){
        float translate = Input.GetAxis("Horizontal") * speed;
        _rigidbody.velocity = new Vector2(translate + dashLength, _rigidbody.velocity.y);
        //_transform.Translate(translate, 0, 0);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
            anim.SetBool("IsWalking", true);
        }
        else {
            anim.SetBool("IsWalking", false);
        }

        if (translate > 0){
            playerDirection = Direction.RIGHT;
        } else if (translate < 0){
            playerDirection = Direction.LEFT;
        }
        dashLength = 0;
    }

    void Flip(){
        if(playerDirection == Direction.LEFT){
            Vector3 theScale = transform.localScale;
            theScale.x = -6;
            transform.localScale = theScale;
        }
        if (playerDirection == Direction.RIGHT)
        {
            Vector3 theScale = transform.localScale;
            theScale.x = 6;
            transform.localScale = theScale;
        }
    }

    void Jump(){
        if((Input.GetKeyDown(KeyCode.W) && grounded))
        {
            StartCoroutine("Jumping");
        }
    }

    IEnumerator Jumping()
    {
        anim.SetBool("IsJumping", true);
        source.PlayOneShot(slashSound);
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
        yield return new WaitForSeconds(.8f);
        anim.SetBool("IsJumping", false);
    }

    void Jab(){
        slashTimer -= Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.J) && slashTimer <= 0f)
        {
            slashTimer = slashCooldown;
            StartCoroutine("Slashing");
        }
    }

    IEnumerator Slashing()
    {
        bulletPos = transform.position;
        if (playerDirection == Direction.RIGHT)
        {
            anim.SetBool("IsJabbing", true);
            bulletPos += new Vector2(+.7f, +0.05f);
            Instantiate(slashRight, bulletPos, Quaternion.identity);
            slashRight.transform.parent = gameObject.transform;
            //source.PlayOneShot(jumpSound);
            AudioManager.instance.Play("PlayerSlash");
            yield return new WaitForSeconds(.1f);
            anim.SetBool("IsJabbing", false);
            
        }
        else if (playerDirection == Direction.LEFT)
        {
            anim.SetBool("IsJabbing", true);
            bulletPos += new Vector2(-.7f, +0.05f);
            Instantiate(slashLeft, bulletPos, Quaternion.identity);
            slashLeft.transform.parent = gameObject.transform;
            //source.PlayOneShot(jumpSound);
            AudioManager.instance.Play("PlayerSlash");
            yield return new WaitForSeconds(.1f);
            anim.SetBool("IsJabbing", false);
            
        }
    } 


    void Shoot() {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextFire && ammo > 0){
            nextFire = Time.time + firerate;
            Fire();
            //source.PlayOneShot(shootSound);
            AudioManager.instance.Play("PlayerLaser");
            ammo -= 1;
            ammoImage.fillAmount = (ammo / 20);
        }
    }

    void Fire() {
        bulletPos = transform.position;
        if(playerDirection == Direction.RIGHT && ammo > 0)
        {
            bulletPos += new Vector2(+.7f, +0.05f);
            Instantiate(bulletToRight, bulletPos, Quaternion.identity);
        }else if (ammo > 0)
        {
            bulletPos += new Vector2(-.7f, +0.05f);
            Instantiate(bulletToLeft, bulletPos, Quaternion.identity);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //_rigidbody.velocity = Vector3.zero;
        if (col.gameObject.tag == "Ground"){
            grounded = true;
        }

    }


    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }

}


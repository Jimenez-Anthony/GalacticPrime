using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash : MonoBehaviour {

    private Rigidbody2D rb2d;

    public float dashCooldown = 3f;
    public float dashForce = 60f;
    public GameObject clone;
    private float dashTimer;
    private JPlayerController pc;
    public Image coolDownImage;
    bool isCooldown;

    public Animator anim;

    public AudioClip dashSound;
    private AudioSource source;


    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        dashTimer = dashCooldown;
        anim.SetBool("IsDashing", false);
        source = GetComponent<AudioSource>();
        pc = GetComponent<JPlayerController>();
        isCooldown = true;
    }

    void Update() {
        if (dashTimer > 0f) {
            dashTimer -= Time.deltaTime;
        }
        anim.SetBool("IsDashingL", false);
        anim.SetBool("IsDashingR", false);
        if (Input.GetKeyDown(Keys.dash) && dashTimer <= 0f && !pc.stunned) {
            dashTimer = dashCooldown;
            source.PlayOneShot(dashSound);
            if (transform.localScale.x < 0f) {
                isCooldown = true;
                //Debug.Log("left");
                anim.SetBool("IsDashingR", true);
                GameObject clone = Instantiate(this.clone, transform.position, Quaternion.Euler(-180, 0, -180)) as GameObject;
                clone.transform.localScale = new Vector3(6f, 6f, 1f);
                Destroy(clone, 0.25f);
                pc.KnockbackDuration(0.2f);
                rb2d.AddForce(new Vector3(-1 * dashForce, 0f, 0f), ForceMode2D.Impulse);
                //pc.dashLength = -1f * dashDistance;
            }
            else {
                isCooldown = true;
                //Debug.Log("right");
                anim.SetBool("IsDashingR", true);
                GameObject clone = Instantiate(this.clone, transform.position, transform.rotation) as GameObject;
                clone.transform.localScale = new Vector3(6f, 6f, 1f);
                Destroy(clone, 0.25f);
                rb2d.AddForce(new Vector3(1 * dashForce, 0f, 0f), ForceMode2D.Impulse);
                //pc.dashLength = dashDistance;
            }
        }

        if (isCooldown)
        {
            coolDownImage.fillAmount = (dashCooldown - dashTimer) / dashCooldown;
            if(coolDownImage.fillAmount >= 1f)
            {
                //coolDownImage.fillAmount = 0;
                coolDownImage.fillAmount = 1f;
                isCooldown = false;
            }

            if(coolDownImage.fillAmount < 1)
            {
                coolDownImage.color = new Color32(255, 255, 255, 255);
            }
            else
            {
                coolDownImage.color = new Color32(148, 255, 124, 255);
            }
        }

        // JIMMY - Removed because its all crap
        //if (direction == 0) {
        //    if (Input.GetKeyDown(KeyCode.Q) && Time.timeScale == 1f) {
        //        direction = 1;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.E) && Time.timeScale == 1f) {
        //        direction = 2;
        //    }
        //}
        //else {
        //    if (dashTime <= 0) {
        //        direction = 0;
        //        dashTime = startDashTime;
        //        rb.velocity = Vector2.zero;
        //    }
        //    else {
        //        dashTime -= Time.deltaTime;

        //        if (direction == 1) {
        //            StartCoroutine("DashingL");
        //        }
        //        else if (direction == 2) {
        //            StartCoroutine("DashingR");
        //        }
        //    }
        //}
    }

    //IEnumerator DashingL()
    //{
    //    anim.SetBool("IsDashingL", true);
    //    source.PlayOneShot(dashSound);
    //    rb.velocity = new Vector2(-1 * dashSpeed * 100f, rb.velocity.y);
    //    //rb.velocity = Vector2.left * dashSpeed;
    //    yield return new WaitForSeconds(.2f);
    //    anim.SetBool("IsDashingL", false);
    //}

    //IEnumerator DashingR()
    //{
    //    anim.SetBool("IsDashingR", true);
    //    source.PlayOneShot(dashSound);
    //    rb.velocity = new Vector2(dashSpeed, rb.velocity.y);
    //    //rb.velocity = Vector2.right * dashSpeed;
    //    yield return new WaitForSeconds(.2f);
    //    anim.SetBool("IsDashingR", false);
    //}

}

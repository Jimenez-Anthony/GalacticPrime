using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArrow : MonoBehaviour
{

    private float xScale;
    public int faceDir = 0;
    private float speed = 13f;
    public int damage = 0;
    private Rigidbody2D rb2d;

    public Vector3 targetLoc;

    bool landed = false;
    float landedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        xScale = transform.localScale.x;
        rb2d = GetComponent<Rigidbody2D>();
        landed = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(xScale * -faceDir, transform.localScale.y, transform.localScale.z);

        if (Vector3.Distance(transform.position, targetLoc) > 0.5f) {
            Vector3 diff = (targetLoc - transform.position).normalized;
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            if (faceDir == -1) {
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 180);
            }
            else {
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
            }
        }

        if (targetLoc != null) {
            //print(targetLoc.x);
            rb2d.position = Vector3.MoveTowards(transform.position, targetLoc, speed * Time.deltaTime);
        }

        if (transform.position == targetLoc && !landed) {
            Destroy(GetComponent<Collider2D>());
            landed = true;
        }

        if (landed && landedTime < 1f) {
            landedTime += Time.deltaTime;
        }
        else if (landed) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.tag == "Player") {
            collision.GetComponent<JHealthController>().TakeDamage(damage, JHealthController.DAMAGETYPE.Enemy);
            Destroy(gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
            Destroy(GetComponent<Collider2D>());
            //Destroy(gameObject);
            rb2d.bodyType = RigidbodyType2D.Static;
            landed = true;
        }

    }
}

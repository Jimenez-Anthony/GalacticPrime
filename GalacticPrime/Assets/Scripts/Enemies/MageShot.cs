using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageShot : MonoBehaviour
{

    private float xScale;
    public int faceDir = 0;
    private float speed = 13f;
    public int damage = 0;
    private Rigidbody2D rb2d;

    public Vector3 targetLoc;

    private float timer = 0f;

    void Start()
    {
        xScale = transform.localScale.x;
        rb2d = GetComponent<Rigidbody2D>();
        timer = 0f;
        //targetLoc = GameMaster.instance.transform.position;
        //faceDir = -1;
    }

    void Update()
    {
        transform.localScale = new Vector3(xScale * -faceDir, transform.localScale.y, transform.localScale.z);

        if (Vector3.Distance(transform.position, targetLoc) > 0.5f) {
            Vector3 diff = (targetLoc - transform.position).normalized;
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            if (faceDir == -1) {
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
            }
            else {
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
            }
        }

        timer += Time.deltaTime;
        if (timer > 5f) {
            Destroy(gameObject);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetLoc, speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<MageBehavior>() != null) {
            return;
        }
        JHealthController health = collision.GetComponent<JHealthController>();
        if (health != null) {
            health.TakeDamage(damage, JHealthController.DAMAGETYPE.Enemy);
            Destroy(gameObject);
        }
        JStatusController status = collision.GetComponent<JStatusController>();
        if (status != null) {
            status.witherTimer = 10f;
            if (status.witherAmount == 0) {
                status.witherAmount = 1;
                health.armor -= 1;
            }
        }
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        rb.AddForce((collision.transform.position - transform.position).normalized * 10f, ForceMode2D.Impulse);
    }
}

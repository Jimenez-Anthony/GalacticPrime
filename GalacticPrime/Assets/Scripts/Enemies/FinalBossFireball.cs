using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossFireball : MonoBehaviour
{

    public Vector3 targetLoc;
    public GameObject explosionParticles;
    public float speed = 20f;
    public float range = 3f;
    public bool targetSelected = false;

    private float xScale;
    public int faceDir;

    void Start()
    {
        targetSelected = false;
        xScale = transform.localScale.x;
    }

    void Update()
    {
        if (targetSelected) {
            Vector3 diff = (targetLoc - transform.position).normalized;
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            if (faceDir == -1) {
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 180 + 45);
            }
            else {
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 45);
            }
            transform.LookAt(targetLoc);
        }

        if (targetSelected) {
            //print(targetLoc.x);
            transform.position = Vector3.MoveTowards(transform.position, targetLoc, speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Building") {
            GetComponent<AudioSource>().Play();
            if (Vector3.Distance(transform.position, GameMaster.instance.player.transform.position) < range) {
                GameMaster.instance.player.GetComponent<JHealthController>().TakeDamage(7, JHealthController.DAMAGETYPE.Enemy);
            }
            GameObject particles = Instantiate(explosionParticles, transform.position, explosionParticles.transform.rotation) as GameObject;
            particles.transform.localScale = new Vector3(3f, 3f, 3f);
            Destroy(particles, 1f);
            Destroy(gameObject);
        }
    }
}

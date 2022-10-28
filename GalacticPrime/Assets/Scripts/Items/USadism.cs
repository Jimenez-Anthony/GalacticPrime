using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USadism : MonoBehaviour, IItem {

    public float range = 25f;

    public GameObject particles;
    public SadismHealing healing;

    private AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    public float GetCooldown() {
        return 0f;
    }

    public void Use() {
        transform.SetParent(null);
        GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(UseSadism());
    }

    IEnumerator UseSadism() {
        audio.PlayOneShot(audio.clip);

        GameMaster.instance.player.GetComponent<JStatusController>().ApplyStun(0.5f);
        yield return new WaitForSeconds(0.5f);

        GameObject particlesClone = Instantiate(particles, transform.position, particles.transform.rotation);
        Destroy(particlesClone, 1f);

        yield return new WaitForSeconds(0.4f);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D col in colliders) {
            if (col.gameObject.tag == "Enemy") {
                JHealthController health = col.GetComponent<JHealthController>();
                if (health == null) {
                    continue;
                }
                JStatusController status = col.GetComponent<JStatusController>();
                if (status != null) {
                    status.ApplyStun(0.5f);
                }

                SadismHealing healingClone = Instantiate(healing, col.transform.position, Quaternion.identity) as SadismHealing;
            }
        }


        Destroy(gameObject);
        GameMaster.instance.inventory.UseReusableItem();
    }
}

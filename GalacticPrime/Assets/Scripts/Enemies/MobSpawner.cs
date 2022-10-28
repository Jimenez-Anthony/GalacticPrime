using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour, IDeathController, IGetsHurt {


    public float spawnCooldown = 5f;
    private float nextSpawnCooldown;
    private float spawnTimer;
    public bool rateScalesWithDistance = false;
    public float scaleFactor = 1f;
    public float minimumCooldown = 0.5f;
    public float randomPercentage = 0.5f;

    public GameObject spawn;
    public GameObject spawnParticles;
    public GameObject spawnOnDeath;

    private SpriteRenderer sprite;
    private Transform player;

    private bool dead = false;

    // Spin
    private float degrees;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        spawnTimer = CalculateNextSpawnTime();
        player = GameMaster.instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, degrees * Time.deltaTime));

        if (spawnTimer > 0f) {
            spawnTimer -= Time.deltaTime;
        }
        else {
            Spawn();
            spawnTimer = CalculateNextSpawnTime();
        }
    }

    float CalculateNextSpawnTime() {
        player = GameMaster.instance.player.transform;
        float cooldown = spawnCooldown;
        if (rateScalesWithDistance) {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            cooldown += (distance - 10f) / 5 * scaleFactor;
            if (distance > 20f) {
                cooldown *= 1.5f;
            }
        }
        if (randomPercentage != 0f) {
            cooldown += (Random.value - 0.5f) * randomPercentage * spawnCooldown;
        }
        if (cooldown < minimumCooldown) {
            cooldown = minimumCooldown;
        }
        print("Spawn cooldown: " + cooldown);
        degrees = -1440f / cooldown;
        return cooldown;
    }

    void Spawn() {
        Instantiate(spawn, transform.position, Quaternion.identity);
        GameObject particles = Instantiate(spawnParticles, new Vector3(transform.position.x, transform.position.y + 1, 0f), Quaternion.identity) as GameObject;
        Destroy(particles, 1f);
    }

    public void OnDeath() {
        if (!dead) {
            dead = true;
            Instantiate(spawnOnDeath, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void OnHurt() {
        StartCoroutine(OnHurtAnim());
    }

    IEnumerator OnHurtAnim() {
        //AudioManager.instance.Play("SkeletonHit");
        //audioSource.PlayOneShot(audios[1]);
        sprite.color = new Color32(255, 122, 122, 255);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color32(255, 255, 255, 255);
    }
}

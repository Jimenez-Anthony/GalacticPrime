using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCrossfire : MonoBehaviour, IItem
{

    public int damage = 2;
    public int pallets = 32;
    public float fireRate = 0.2f;
    public int energyCost = 30;

    private float fireCooldown;
    private float fireTimer;

    public JBullet bullet;
    public Transform firePoint;
    public GameObject muzzleFlash;
    public string fireSound;

    private JPlayerController playerCont;
    private Rigidbody2D playerRb;
    public CrossfireLine line;
    private Animator anim;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {

        playerCont = GameMaster.instance.player.GetComponent<JPlayerController>();
        playerRb = GameMaster.instance.player.GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        //line = GetComponent<LineRenderer>();
        anim = GetComponent<Animator>();
        fireCooldown = 1f / fireRate;
        fireTimer = 0f;
        firePoint = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (fireTimer > 0f) {
            fireTimer -= Time.deltaTime;
        }
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

    }

    public void Use() {
        if (fireTimer <= 0f) {
            if (GameMaster.instance.playerEnergy.UseEnergy(energyCost)) {
                fireTimer = fireCooldown;
                StartCoroutine(Shoot());
            }
            else {
                print("Insufficient energy");
            }
        }
    }

    IEnumerator Shoot() {
        anim.SetTrigger("Fire");
        audioSource.PlayOneShot(audioSource.clip);
        yield return new WaitForSeconds(0.3f);
        float row = 0;
        float col = 0;
        float offset = 0.1f;

        GameObject flash = Instantiate(muzzleFlash, firePoint, false) as GameObject;
        flash.transform.localScale = new Vector3(3f, 3f, 1f);
        Destroy(flash, 0.2f);

        int faceDir = 0;
        Vector3 dir;
        if (playerCont.facingRight) {
            dir = Vector3.right;
            faceDir = 1;
        }
        else {
            dir = Vector3.left;
            faceDir = -1;
        }

        for (int i = 0; i < pallets; i++) {
            row = Random.Range(-0.2f, 0.2f);
            col = Random.Range(-0.4f, 0.4f);

            Vector3 firePos = new Vector3(firePoint.position.x + row, firePoint.position.y + col, 0f);

            float yVel = Random.Range(-150, 150);

            CrossfireLine clone = Instantiate(line, firePos, Quaternion.identity) as CrossfireLine;
            clone.startPos = firePos;
            clone.damage = damage;
            clone.yVel = yVel;
            clone.faceDir = faceDir;
            
            //clone.GetComponent<Rigidbody2D>().AddForce(dir * 1f, ForceMode2D.Impulse);

            Destroy(clone.gameObject, 0.2f);
        }

        GameMaster.instance.inventory.UseDurability(1);

        StartCoroutine(playerCont.KnockbackDuration(0.2f));
        playerCont.GetComponent<Rigidbody2D>().AddForce(new Vector2(faceDir * -1 * 100f, 5f), ForceMode2D.Impulse);
    }

    public float GetCooldown() {
        return fireTimer / fireCooldown;
    }
}

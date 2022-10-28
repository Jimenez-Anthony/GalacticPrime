using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UHook : MonoBehaviour, IItem
{
    private JPlayerController playerCont;
    private DistanceJoint2D joint;
    private Vector3 targetPos;
    private Rigidbody2D rb2d;
    private LineRenderer line;
    bool thrown = false;
    bool targeting = false;
    private float life = 0f;
    bool destroy = false;

    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        playerCont = GameMaster.instance.player.GetComponent<JPlayerController>();
        joint = playerCont.GetComponent<DistanceJoint2D>();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        thrown = false;
        targeting = false;

    }

    // Update is called once per frame
    void Update()
    {
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

        if (thrown) {
            life += Time.deltaTime;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, playerCont.transform.position);
            line.startColor = Color.white;
            line.endColor = Color.white;
        }

        if (life > 1.5f) {
            joint.enabled = false;

            if (destroy) {
                Destroy(this.gameObject);
            }
            else {
                UseUpHook();
            }
        }

        if (targeting && Input.GetKeyUp(KeyCode.J) && !thrown) {
            if (GameMaster.instance.inventory.GetSelectedCount() == 1) {
                transform.parent = null;
            }

            targeting = false;
            thrown = true;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, playerCont.transform.position);
            line.enabled = true;
            line.startColor = Color.white;
            line.endColor = Color.white;
            rb2d = gameObject.AddComponent<Rigidbody2D>() as Rigidbody2D;
            rb2d.gravityScale = 0f;
            rb2d.AddForce((GameMaster.instance.targeter.GetTargetPosition() - transform.position).normalized * 15f, ForceMode2D.Impulse);

            if (GameMaster.instance.inventory.RemoveSelectedItem()) {
                destroy = true;
            }
            else {
                destroy = false;
            }
        }
        else if (targeting && Input.GetKeyUp(KeyCode.J)) {
            targeting = false;
            GameMaster.instance.targeter.GetTargetPosition();
        }
    }

    void UseUpHook() {
        Destroy(rb2d);
        line.enabled = false;
        thrown = false;
        life = 0f;
        transform.localPosition = new Vector3(0f, 0.04f, 0f);
    }

    public void Use() {
        GameMaster.instance.targeter.StartTargeting();
        targeting = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        //print(collision.gameObject.name);
        if (collision.gameObject.tag == "Ground" && rb2d != null) {
            rb2d.bodyType = RigidbodyType2D.Static;
            targetPos = transform.position;
            joint.enabled = true;
            joint.connectedAnchor = transform.position;
            joint.distance = 1f;
        }
        //if (thrown && life > 1f && collision.gameObject.tag == "Player") {
        //    joint.enabled = false;
        //    if (destroy) {
        //        Destroy(this.gameObject);
        //    }
        //    else {
        //        UseUpHook();
        //    }
        //}
    }

    public float GetCooldown() {
        return 0f;
    }

    void OnDestroy() {
        joint.enabled = false;
    }
}

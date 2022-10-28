using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{

    public float health;
    public Animator targetanim;

    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        targetanim.SetBool("IsAlive", true);
        player = GameObject.FindObjectOfType<PlayerController>();
    }

    public void Damage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            targetanim.SetBool("IsAlive", false);
            player.gameStage = 1;
            Destroy(gameObject, (float)0.33);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Bullet"))
        {
            Destroy(other.gameObject);
            Damage(1);
        }

     
        //if (health <= 0)
        //{
        //    Destroy(gameObject);
        //}
    }
}
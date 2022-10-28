using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGoldenApple : MonoBehaviour, IItem
{
    public int amount = 5;
    public GameObject healParticles;
    public float duration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

    }

    public void Use() {
        JHealthController health = GameMaster.instance.player.GetComponent<JHealthController>();
        health.Heal(amount);
        GameObject particles = Instantiate(healParticles, GameMaster.instance.player.transform.GetChild(0), false);
        particles.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Destroy(particles, 2f);

        JStatusController status = GameMaster.instance.player.GetComponent<JStatusController>();
        status.ApplyInvulnerability(duration);
        if (GameMaster.instance.inventory.RemoveSelectedItem()) {
            Destroy(this.gameObject);
        }


    }

    public float GetCooldown() {
        return 0f;
    }
}

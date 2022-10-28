using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIreliaGun : MonoBehaviour, IItem {

    public float cooldown = 2f;
    private float timer;
    public int damage = 10;
    public int energyCost = 10;

    public GameObject marker;
    private LineRenderer line;

    private bool firstMakerPlaced = false;
    private GameObject firstMarker;
    private Vector3 firstTarget;
    private bool firstTargetReached = false;
    private bool secondMarkerPlaced = false;
    private GameObject secondMarker;
    private Vector3 secondTarget;
    private bool secondTargetReached = false;

    private bool charging = false;
    private Vector3 targetLoc;

    private float markerDistance = 0f;
    private int dir;
    private bool startDamage = false;

    void Start()
    {
        timer = 0f;
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        startDamage = false;
    }

    void Update()
    {
        if (timer > 0 && !firstMakerPlaced && !charging && !secondMarkerPlaced) {
            timer -= Time.deltaTime;
        }
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());


        dir = 0;
        if (GameMaster.instance.player.transform.position.x - transform.position.x > 0) {
            dir = -1;
        }
        else {
            dir = 1;
        }

        if (startDamage) {
            //print("damaging");
            GetComponent<AudioSource>().Play();
            line.SetPosition(0, firstMarker.transform.position);
            line.SetPosition(1, secondMarker.transform.position);

            if (line.startWidth < 0.47) {
                line.startWidth = line.startWidth + Time.deltaTime * 2.5f;
                line.endWidth = line.endWidth + Time.deltaTime * 2.5f;
            }

            if (Vector3.Distance(firstMarker.transform.position, secondMarker.transform.position) > 0.05f) {
                //print("moving markers");
                firstMarker.transform.position = Vector3.MoveTowards(firstMarker.transform.position, secondMarker.transform.position, 20f * Time.deltaTime);
                secondMarker.transform.position = Vector3.MoveTowards(secondMarker.transform.position, firstMarker.transform.position, 20f * Time.deltaTime);
            }
            else {
                line.enabled = false;
                Destroy(firstMarker);
                Destroy(secondMarker);
                startDamage = false;
                firstMakerPlaced = false;
                secondMarkerPlaced = false;
                firstTargetReached = false;
                secondTargetReached = false;
            }
        }

        if (!startDamage && firstMakerPlaced && Vector3.Distance(firstMarker.transform.position, firstTarget) > 0.05f) {
            //print("current: " + firstMarker.transform.position.x + ", " + firstMarker.transform.position.y);
            firstMarker.transform.position = Vector3.MoveTowards(firstMarker.transform.position, firstTarget, 15f * Time.deltaTime);
            firstTargetReached = false;
        }
        else {
            firstTargetReached = true;
        }

        if (!startDamage && secondMarkerPlaced && Vector3.Distance(secondMarker.transform.position, secondTarget) > 0.05f) {
            //print("current: " + secondMarker.transform.position.x + ", " + secondMarker.transform.position.y);
            secondMarker.transform.position = Vector3.MoveTowards(secondMarker.transform.position, secondTarget, 15f * Time.deltaTime);
            secondTargetReached = false;
        }
        else {
            secondTargetReached = true;
        }

        if (firstMakerPlaced && firstTargetReached && secondMarkerPlaced && secondTargetReached && !startDamage) {
            startDamage = true;
            line.enabled = true;
            line.startWidth = 0.01f;
            line.endWidth = 0.01f;
            line.SetPosition(0, firstMarker.transform.position);
            line.SetPosition(1, secondMarker.transform.position);
            firstMarker.GetComponent<Marker>().damage = damage;
            secondMarker.GetComponent<Marker>().damage = damage;

        }

        if (Input.GetKey(KeyCode.J) && charging && markerDistance < 8f) {
            markerDistance += Time.deltaTime * 10f;
            //print(markerDistance);
        }

        if (Input.GetKeyUp(KeyCode.J) && charging) {
            charging = false;
            targetLoc.y = transform.position.y;
            targetLoc.x = transform.position.x + markerDistance * dir;

            if (!firstMakerPlaced) {
                if (GameMaster.instance.playerEnergy.UseEnergy(energyCost)) {
                    GameMaster.instance.inventory.UseDurability(1);
                    firstTarget = targetLoc;
                    //print("target: " + firstTarget.x + ", " + firstTarget.y);
                    firstMarker = Instantiate(marker, transform.position, Quaternion.identity) as GameObject;
                    firstMakerPlaced = true;
                }
                else {
                    charging = false;
                    print("Insufficient energy");
                }
            }
            else if (!secondMarkerPlaced) {
                secondTarget = targetLoc;
                //print("target: " + secondTarget.x + ", " + secondTarget.y);
                secondMarker = Instantiate(marker, transform.position, Quaternion.identity) as GameObject;
                secondMarkerPlaced = true;
            }

        }
    }

    public void Use() {
        if (timer <= 0f) {
            markerDistance = 0f;
            targetLoc = transform.position;
            charging = true;
        }
    }

    void OnDestroy() {
        markerDistance = 0f;
        if (firstMarker != null) {
            Destroy(firstMarker);
        }
        if (secondMarker != null) {
            Destroy(secondMarker);
        }
    }

    public float GetCooldown() {
        return timer / cooldown;
    }
}

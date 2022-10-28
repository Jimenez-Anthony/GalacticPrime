using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonKingBehavior : MonoBehaviour
{
    public float followRange = 5f;

    public GameObject ranger;
    public GameObject minion;

    private GameObject rangerInstance;
    private GameObject minionInstance;

    public Transform home;
    private JEnemyAI ai;
    private Transform player;

    public ItemStack loot;
    public DroppedItem droppedItem;
    public Sprite buffIcon;
    public string buffDescription;

    // Start is called before the first frame update
    void Start()
    {
        ai = GetComponent<JEnemyAI>();
        player = GameMaster.instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(home.transform.position, player.transform.position) <= followRange) {
            ai.target = player;
            if (rangerInstance == null || Vector3.Distance(rangerInstance.transform.position, transform.position) > 15f) {
                rangerInstance = Instantiate(ranger, transform.GetChild(3).position, Quaternion.identity) as GameObject;
            }
            if (minionInstance == null || Vector3.Distance(minionInstance.transform.position, transform.position) > 15f) {
                minionInstance = Instantiate(minion, transform.GetChild(2).position, Quaternion.identity) as GameObject;
            }
        }
        else {
            ai.target = home;
        }
    }

    void OnDestroy() {
        if (droppedItem != null && home != null) {
            //DroppedItem lootClone = Instantiate(droppedItem, new Vector3(home.transform.position.x, home.transform.position.y + 1, 0f), Quaternion.identity) as DroppedItem;
            //lootClone.itemStack = loot;
            LevelManager.instance.bossKills++;
        }

        if (!LevelManager.instance.skeletonKingBuff) {
            LevelManager.instance.skeletonKingBuff = true;
            GameMaster.instance.ShowBuffPanel(buffIcon, buffDescription);
            GameMaster.instance.player.GetComponent<JHealthController>().naturalRegenAmount += 1;
        }
    }
}

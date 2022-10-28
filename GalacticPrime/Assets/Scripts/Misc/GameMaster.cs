using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;

public class GameMaster : MonoBehaviour
{

    public static GameMaster instance;

    public enum GAMESTATE {paused, over, ingame, won, loading};
    public GAMESTATE gameState;

    public bool generateRandomMap;
    public bool startLevelOnStart = false;
    public bool playerRespawns;
    public Vector3 playerRespawnPoint;

    public GameObject player;
    public JEnergyController playerEnergy;
    public Inventory inventory;
    public Targeter targeter;
    public JProceduralGenerator levelGenerator;
    public Tilemap tilemap;
    public JMapGenerator pathMapGenerator;
    public LevelManager levelManager;
    public LootManager lootManager;
    public MobManager mobManager;
    public DroppedItem droppedItem;
    //public LineRenderer lineRenderer;

    public ItemStack[] crateDrops;

    public GameStats gameStats;

    // UI
    public GameObject pauseMenu;
    public GameObject loadingScreen;
    public Slider loadingBar;
    public GameObject caveBackground;
    public GameObject surfaceBackground;
    public GameObject sewerBackground;
    public GameObject cityBackground;
    public GameObject levelComplete;
    public GameObject areaComplete;
    public GameObject levelDesciption;
    public GameObject bossPanel;
    public GameObject buffPanel;
    public Transform buffs;

    void Awake() {
        // Singleton Pattern
        //if (instance != null) {
        //    return;
        //}
        instance = this;
        //DontDestroyOnLoad(gameObject);
        NewMap();

    }

    void NewMap() {
        levelGenerator = FindObjectOfType<JProceduralGenerator>();
        levelManager = LevelManager.instance;
        player = FindObjectOfType<JPlayerController>().gameObject;
        inventory = player.GetComponent<Inventory>();
        playerEnergy = player.GetComponent<JEnergyController>();
        targeter = FindObjectOfType<Targeter>();
        tilemap = FindObjectOfType<Tilemap>();
        pathMapGenerator = FindObjectOfType<JMapGenerator>();
        lootManager = GetComponent<LootManager>();
        mobManager = transform.GetChild(0).GetComponent<MobManager>();
        //lineRenderer = GetComponent<LineRenderer>();

        loadingBar = loadingScreen.transform.GetChild(0).GetComponent<Slider>();

        gameStats = new GameStats();

        if (levelManager != null) {
            levelManager.LoadLevelSettings();
        }
    }

    void Start() {
        //gameState = GAMESTATE.ingame;
        if (startLevelOnStart) {
            StartLevel();
        }

        if (levelManager != null && levelManager.world == 0) {
            AudioManager.instance.Play("Tutorial");
            gameStats = new GameStats();
        }

    }

    public void StartLevel() {
        AudioManager.instance.Stop("MinecraftDeathMusic");
        pauseMenu.SetActive(false);
        if (levelManager != null) {
            if (levelManager.world == 1) {
                surfaceBackground.SetActive(true);
            }
            else if (levelManager.world == 2) {
                caveBackground.SetActive(true);
            }
            else if (levelManager.world == 3) {
                sewerBackground.SetActive(true);
            }
            else if (levelManager.world == 4) {
                cityBackground.SetActive(true);
            }
        }
        if (levelGenerator != null && generateRandomMap) {
            levelGenerator.GenerateRandomMap();
        }
        pathMapGenerator.SetupPathfinding();

        if (generateRandomMap) {
            mobManager.SpawnMobs();
        }

        gameState = GAMESTATE.ingame;
        //gameStats.startTime = Time.time;
    }

    public void GameOver() {
        if (gameState != GAMESTATE.won) {
            gameState = GAMESTATE.over;
            inventory.Deselect();
            pauseMenu.SetActive(true);
        }
    }

    public void GameWon() {
        gameState = GAMESTATE.won;
        inventory.Deselect();
        LevelManager.instance.AddStats();
        levelComplete.SetActive(true);

        float currentTime = gameStats.timeTaken;

        levelComplete.transform.GetChild(1).GetComponent<TMP_Text>().text  = FormatTime(currentTime);
    }

    string FormatTime(float time) {
        int intTime = (int)time;
        int minutes = intTime / 60;
        int seconds = intTime % 60;
        float fraction = time * 1000;
        fraction = (fraction % 1000);
        string timeText = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
        return timeText;
    }


    public void RestartLevel() {
        AudioManager.instance.StopSounds();
        //levelManager.LoadLevel(SceneManager.GetActiveScene().name);
        inventory.inventory = null;
        LevelManager.instance.Restartlevel();
        //NewMap();
        //StartLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GAMESTATE.ingame) {
            gameStats.timeTaken += Time.deltaTime;
        }
        
        if (gameState == GAMESTATE.over && Input.GetKeyDown(Keys.restartLevel)) {
            TryAgain();
        }
        if (gameState == GAMESTATE.won && Input.GetKeyDown(KeyCode.Return)) {
            if (areaComplete.activeSelf) {
                NextLevel();
            }
            else {
                EndLevel();
            }
        }
    }

    public void EndLevel() {
        levelComplete.SetActive(false);
        if (LevelManager.instance.world == 1 && LevelManager.instance.level == LevelManager.instance.surfaceLevels.Length) {
            areaComplete.SetActive(true);
            return;
        }
        if (LevelManager.instance.world == 2 && LevelManager.instance.level == LevelManager.instance.caveLevels.Length) {
            areaComplete.SetActive(true);
            return;
        }
        if (LevelManager.instance.world == 3 && LevelManager.instance.level == LevelManager.instance.sewerLevels.Length) {
            areaComplete.SetActive(true);
            return;
        }
        NextLevel();
    }

    public void NextLevel() {
        gameState = GAMESTATE.loading;
        levelComplete.SetActive(false);
        areaComplete.SetActive(false);
        levelManager.NextLevel();
    }

    public void TryAgain() {
        pauseMenu.SetActive(false);
        gameState = GAMESTATE.loading;
        RestartLevel();
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ShowBuffPanel(Sprite img, string description) {
        buffPanel.GetComponent<BuffUI>().SetBuff(img, description);
        StartCoroutine(DisplayBuffPanel());
    }

    IEnumerator DisplayBuffPanel() {
        buffPanel.SetActive(true);
        yield return new WaitForSeconds(5f);
        buffPanel.SetActive(false);
    }

    public void GivePlayerItem(ItemStack stack) {
        if (stack.item.itemType == Item.ITEMTYPE.Usable || stack.item.itemType == Item.ITEMTYPE.Consumable) {
            print("[Game Master] Giving player item: " + stack.count + " x " + stack.item.name);
        }
        else {
            print("[Game Master] Giving player item: " + stack.durability + " x " + stack.item.name);
        }

        DroppedItem clone = Instantiate(droppedItem, player.transform.position, Quaternion.identity);
        clone.itemStack = stack;
    }


}



[System.Serializable]
public class GameStats {
    public int damageTaken = 0;
    public int damageDealt = 0;
    public int amountHealed = 0;
    public int cratesBorken = 0;
    public float timeTaken = 0f;
    public int jumpTimes = 0;
}

[System.Serializable]
public static class Keys {
    public static KeyCode left = KeyCode.A;
    public static KeyCode right = KeyCode.D;
    public static KeyCode jump = KeyCode.Space;
    public static KeyCode ladderUp = KeyCode.W;
    public static KeyCode ladderDown = KeyCode.S;
    public static KeyCode useItem = KeyCode.J;
    public static KeyCode openCrate = KeyCode.E;
    public static KeyCode dropItem = KeyCode.Q;
    public static KeyCode restartLevel = KeyCode.Return;
    public static KeyCode itemPanel = KeyCode.Tab;
    public static KeyCode dash = KeyCode.F;
    public static KeyCode panCamera = KeyCode.LeftShift;
    public static KeyCode nextItem = KeyCode.K;
    public static KeyCode previousItem = KeyCode.I;
    public static KeyCode advanceDialogue = KeyCode.N;
}

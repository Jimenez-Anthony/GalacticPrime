using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public bool skipTutorial = false;

    public LevelSettings[] surfaceLevels;
    public LevelSettings[] caveLevels;
    public LevelSettings[] sewerLevels;
    public LevelSettings cityLevel;

    private GameObject loadingScreen;

    // Player info
    public ItemStack[] storedInventory;
    public int maxHP;
    public int hp;
    public int hpRegenAmount;
    public int maxEnergy;
    public int energyRegen;
    public int armor;
    public float dashCD;
    public int timesDied;
    public int totalTimesDied;

    public bool infinite = false;
    public int inifiniteTries = 0;
    public bool bossMode = false;

    // Level Info
    public int world = -1;
    public int level = -1;

    // Boss Buffs
    public bool skeletonKingBuff = false;
    private bool previousSekelton = false;
    public bool centicleBuff = false;
    private bool previousCenticle = false;
    public bool stalkerBuff = false;
    private bool previousStalker = false;
    public bool rocketManBuff = false;
    private bool previousRocket = false;
    private bool died = false;

    public Item[] allItems;

    // Stat Tracking
    public GameStats levelStats;
    public GameStats totalStats;
    public int bossKills;


    void Awake() {
        // Singleton Pattern
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        storedInventory = null;
        infinite = false;
        inifiniteTries = 0;
        timesDied = 0;
        totalStats = new GameStats();

        skeletonKingBuff = false;
        centicleBuff = false;
        stalkerBuff = false;
        rocketManBuff = false;
        //SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    public void LoadLevel(string name) {
        StartCoroutine(LoadAsynchronosly(name));
    }

    public void Restartlevel() {
        inifiniteTries = 0;
        timesDied++;
        died = true;
        if (world == 0) {
            //storedInventory = GameMaster.instance.inventory.inventory;
            LoadTutorial();
        }
        else {
            StartCoroutine(LoadAsynchronosly("GameScene"));
        }
    }

    public void NextLevel() {
        AudioManager.instance.StopSounds();

        storedInventory = new ItemStack[GameMaster.instance.inventory.inventory.Length];
        maxHP = GameMaster.instance.player.GetComponent<JHealthController>().maxHP;
        hp = GameMaster.instance.player.GetComponent<JHealthController>().hp;
        hpRegenAmount = GameMaster.instance.player.GetComponent<JHealthController>().naturalRegenAmount;
        armor = GameMaster.instance.player.GetComponent<JHealthController>().armor;
        maxEnergy = GameMaster.instance.playerEnergy.maxEnergy;
        energyRegen = GameMaster.instance.playerEnergy.energyRegenAmount;
        dashCD = GameMaster.instance.player.GetComponent<Dash>().dashCooldown;

        if (infinite) {
            inifiniteTries++;
            StartCoroutine(LoadAsynchronosly("GameScene"));
            return;
        }

        if (world == 0) {
            world = 1;
            level = 1;
            levelStats = new GameStats();
            totalTimesDied += timesDied;
            timesDied = 0;
            bossKills = 0;
            StartCoroutine(LoadAsynchronosly("GameScene"));
            return;
        }
        level++;
        if (world == 1 && level > surfaceLevels.Length) {
            world = 2;
            level = 1;
            levelStats = new GameStats();
            totalTimesDied += timesDied;
            timesDied = 0;
            bossKills = 0;
            maxHP += 5;
        }
        if (world == 2 && level > caveLevels.Length) {
            world = 3;
            level = 1;
            levelStats = new GameStats();
            totalTimesDied += timesDied;
            timesDied = 0;
            bossKills = 0;
            maxHP += 5;
        }
        if (world == 3 && level > sewerLevels.Length) {
            AudioManager.instance.StopSounds();
            SceneManager.LoadScene("TutTransition");
            world = -3;
            level = 1;
        }
        if (world == -3) {
            world = 4;
            levelStats = new GameStats();
            totalTimesDied += timesDied;
            timesDied = 0;
            bossKills = 0;
            maxHP += 5;
        }
        else if (world == 4) {
            print("Game Finished");
            world = -2;
            EndGame();
            return;
        }

        previousRocket = rocketManBuff;
        previousStalker = stalkerBuff;
        previousCenticle = centicleBuff;
        previousSekelton = skeletonKingBuff;

        for (int i = 0; i < storedInventory.Length; i++) {
            if (GameMaster.instance.inventory.inventory[i] != null && GameMaster.instance.inventory.inventory[i].item != null) {
                storedInventory[i] = new ItemStack(GameMaster.instance.inventory.inventory[i].item, GameMaster.instance.inventory.inventory[i].count, GameMaster.instance.inventory.inventory[i].durability);
            }
        }
        StartCoroutine(LoadAsynchronosly("GameScene"));
    }

    public void AddStats() {
        if (world > 0 && !infinite && !bossMode) {
            levelStats.amountHealed += GameMaster.instance.gameStats.amountHealed;
            levelStats.damageTaken += GameMaster.instance.gameStats.damageTaken;
            levelStats.damageDealt += GameMaster.instance.gameStats.damageDealt;
            levelStats.cratesBorken += GameMaster.instance.gameStats.cratesBorken;
            levelStats.jumpTimes += GameMaster.instance.gameStats.jumpTimes;
            levelStats.timeTaken += GameMaster.instance.gameStats.timeTaken;
        }
    }

    public void EndGame() {
        bossMode = false;
        world = -2;
        AudioManager.instance.StopSounds();
        Destroy(this.gameObject);
        SceneManager.LoadScene("EndCard");
    }

    IEnumerator LoadAsynchronosly(string name) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);

        GameObject loadingScreen;
        if (GameMaster.instance == null) {
            loadingScreen = FindObjectOfType<MainMenu>().loadingScreen;
        }
        else {
            loadingScreen = GameMaster.instance.loadingScreen;
        }

        loadingScreen.SetActive(true);
        loadingScreen.transform.GetChild(2).gameObject.SetActive(true);

        while (!operation.isDone) {
            loadingScreen.transform.GetChild(0).GetComponent<Slider>().value = operation.progress;
            if (operation.progress >= 0.8) {
                loadingScreen.transform.GetChild(1).GetComponent<TMP_Text>().text = "Creating Tilemap...";
            }
            else {
                loadingScreen.transform.GetChild(1).GetComponent<TMP_Text>().text = "Loading Level...";
            }
            yield return null;
        }


    }

    void Update() {
        if (world == -1 && Input.GetKeyDown(KeyCode.Return)) {
            StartGame();
        }

        if (GameMaster.instance != null) {
            if (skeletonKingBuff) {
                GameMaster.instance.buffs.GetChild(0).gameObject.SetActive(true);
            }
            else {
                GameMaster.instance.buffs.GetChild(0).gameObject.SetActive(false);
            }

            if (centicleBuff) {
                GameMaster.instance.buffs.GetChild(1).gameObject.SetActive(true);
            }
            else {
                GameMaster.instance.buffs.GetChild(1).gameObject.SetActive(false);
            }

            if (stalkerBuff) {
                GameMaster.instance.buffs.GetChild(2).gameObject.SetActive(true);
            }
            else {
                GameMaster.instance.buffs.GetChild(2).gameObject.SetActive(false);
            }

            if (rocketManBuff) {
                GameMaster.instance.buffs.GetChild(3).gameObject.SetActive(true);
            }
            else {
                GameMaster.instance.buffs.GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    public void ToggleTutorial() {
        if (!skipTutorial) {
            skipTutorial = true;
        }
        else {
            skipTutorial = false;
        }
    }

    public void LoadTutorial() {
        SceneManager.LoadScene("TutorialScene");
        //StartCoroutine(LoadAsynchronosly("AisForAnaru"));
    }

    public void StartGame() {
        AudioManager.instance.StopSounds();

        if (!skipTutorial) {
            world = 0;
            level = 0;

            SceneManager.LoadScene("Intro");
        }
        else {
            world = 1;
            level = 1;
            StartCoroutine(LoadAsynchronosly("GameScene"));
            //LoadLevelSettings(world, level);
        }
    }

    public void StartInfinite() {
        AudioManager.instance.StopSounds();
        inifiniteTries = 0;
        if (Random.value < 0.5f) {
            world = 2;
            level = 3;
        }
        else {
            world = 2;
            level = 3;
        }
        infinite = true;
        StartCoroutine(LoadAsynchronosly("GameScene"));
    }

    public void StartBossBattle() {
        AudioManager.instance.StopSounds();
        world = 4;
        level = 1;
        skeletonKingBuff = true;
        centicleBuff = true;
        stalkerBuff = true;
        rocketManBuff = true;
        bossMode = true;
        StartCoroutine(LoadAsynchronosly("GameScene"));
    }

    public void LoadLevelSettings() {
        if (world <= 0) {
            return;
        }
        LevelSettings lvlSettings = null;
        if (world == 1) {
            lvlSettings = surfaceLevels[level - 1];
        }
        else if (world == 2) {
            lvlSettings = caveLevels[level - 1];
        }
        else if (world == 3) {
            lvlSettings = sewerLevels[level - 1];
        }
        else if (world == 4) {
            lvlSettings = cityLevel;
        }

        GameMaster.instance.levelGenerator.type = lvlSettings.lvlType;
        GameMaster.instance.levelGenerator.tile = lvlSettings.baseTile;
        GameMaster.instance.levelGenerator.width = lvlSettings.mapSize.x;
        GameMaster.instance.levelGenerator.height = lvlSettings.mapSize.y;
        GameMaster.instance.levelGenerator.GetComponent<SurfaceDecorGenerator>().enable = lvlSettings.generateGrass;
        GameMaster.instance.levelGenerator.GetComponent<CrateGenerator>().enable = lvlSettings.generateCrates;
        GameMaster.instance.levelGenerator.GetComponent<SpikesGenerator>().enable = lvlSettings.generateSpikes;
        GameMaster.instance.levelGenerator.GetComponent<SpikesGenerator>().chance = lvlSettings.spikeChance;
        GameMaster.instance.levelGenerator.GetComponent<LadderGenerator>().enable = lvlSettings.generateLadders;

        GameMaster.instance.levelGenerator.GetComponent<DartTrapGenerator>().enable = lvlSettings.generateArrowTrap;
        GameMaster.instance.levelGenerator.GetComponent<HealingStationGenerator>().enable = lvlSettings.generateHealingStation;
        GameMaster.instance.levelGenerator.GetComponent<HealingStationGenerator>().chance = lvlSettings.healingStationChance;
        GameMaster.instance.levelGenerator.GetComponent<RechargingStationGenerator>().enable = lvlSettings.generateRechargingStation;
        GameMaster.instance.levelGenerator.GetComponent<RechargingStationGenerator>().chance = lvlSettings.rechargingStationChance;
        GameMaster.instance.levelGenerator.generateStrucutres = lvlSettings.generateStrcuture;

        GameMaster.instance.levelGenerator.GetComponent<SkeletonKingStructure>().enabled = lvlSettings.spawnSkeletonKing;
        if (skeletonKingBuff) {
            GameMaster.instance.levelGenerator.GetComponent<SkeletonKingStructure>().enabled = false;
        }
        GameMaster.instance.levelGenerator.GetComponent<SkeletonKingStructure>().chance = lvlSettings.skeletonKingChance;

        GameMaster.instance.levelGenerator.GetComponent<MechRoomStructure>().enabled = lvlSettings.spawnMechPortal;
        if (centicleBuff) {
            GameMaster.instance.levelGenerator.GetComponent<MechRoomStructure>().enabled = false;
        }
        GameMaster.instance.levelGenerator.GetComponent<MechRoomStructure>().chance = lvlSettings.mechPortalChance;

        GameMaster.instance.levelGenerator.GetComponent<StalkerRoomStructure>().enabled = lvlSettings.generateStalker;
        if (stalkerBuff) {
            GameMaster.instance.levelGenerator.GetComponent<StalkerRoomStructure>().enabled = false;
        }

        GameMaster.instance.levelGenerator.GetComponent<RocketManRoomStructure>().enabled = lvlSettings.generateRocketMan;
        if (rocketManBuff) {
            GameMaster.instance.levelGenerator.GetComponent<RocketManRoomStructure>().enabled = false;
        }


        GameMaster.instance.mobManager.spawns = lvlSettings.mobSpawns;
        GameMaster.instance.mobManager.spawnChance = lvlSettings.mobSpawnChance;

        if (lvlSettings.overrideCrateLootable) {
            GameMaster.instance.lootManager.crate = lvlSettings.crateLootTable;
        }
        if (lvlSettings.overrideGoldenCrateLootable) {
            GameMaster.instance.lootManager.goldenCrate = lvlSettings.goldenCrateLootTable;
        }

        GameMaster.instance.inventory.inventory = storedInventory;
        //GameMaster.instance.levelGenerator.width = 100;
        print("[Level Manager] Finished loading level settings for " + world + "-" + level);

        string levelDescription = "";
        if (infinite) {
            levelDescription = "Infinite" + "-" + inifiniteTries;
        }
        else if (world == 1) {
            levelDescription = "Surface " + world + "-" + level;
        }
        else if (world == 2) {
            levelDescription = "Cave " + world + "-" + level;
        }
        else if (world == 3) {
            levelDescription = "Sewer " + world + "-" + level;
        }
        else if (world == 4) {
            levelDescription = "Final Boss";
        }
        else if (world == 0) {
            levelDescription = "Tutorial";
        }

        GameMaster.instance.levelDesciption.transform.GetChild(0).GetComponent<TMP_Text>().text = levelDescription;
        GameMaster.instance.levelDesciption.GetComponent<Animator>().SetTrigger("NewLevel");

        if (!infinite && !bossMode) {
            RestorePlayerStats();
        }

        if (bossMode) {
            if (skeletonKingBuff && GameMaster.instance.player.GetComponent<JHealthController>().naturalRegenAmount < 1) {
                GameMaster.instance.player.GetComponent<JHealthController>().naturalRegenAmount = 1;
            }
            if (centicleBuff && GameMaster.instance.playerEnergy.maxEnergy < 100) {
                GameMaster.instance.playerEnergy.maxEnergy = 100;
                GameMaster.instance.playerEnergy.energyRegenAmount += 1;
            }
            if (stalkerBuff && GameMaster.instance.player.GetComponent<Dash>().dashCooldown > 1) {
                GameMaster.instance.player.GetComponent<Dash>().dashCooldown = 1;
            }
            if (rocketManBuff && !GameMaster.instance.player.GetComponent<JHealthController>().environmentalImmune) {
                GameMaster.instance.player.GetComponent<JHealthController>().environmentalImmune = true;
                GameMaster.instance.player.GetComponent<JHealthController>().IncreaseMaxHealth(5);
            }
            GameMaster.instance.player.GetComponent<JHealthController>().IncreaseMaxHealth(15);
            StartCoroutine(SpawnRandomWeapons());
        }

        if (infinite) {
            if (world == 2) {
                GameMaster.instance.player.GetComponent<JHealthController>().IncreaseMaxHealth(5);
            }
            else {
                GameMaster.instance.player.GetComponent<JHealthController>().IncreaseMaxHealth(10);
            }
        }

        if (world == 1) {
            AudioManager.instance.Play("Surface");
        }
        else if (world == 2) {
            AudioManager.instance.Play("Cave");
        }
        else if (world == 3) {
            AudioManager.instance.Play("Sewer");
        }
        else if (world == 4) {
            AudioManager.instance.Play("Fyeah");
        }
        else {
            AudioManager.instance.Play("Tutorial");
        }
    }

    void RestorePlayerStats() {
        if (world > 1 || level > 1) {
            GameMaster.instance.player.GetComponent<JHealthController>().maxHP = maxHP;
            GameMaster.instance.player.GetComponent<JHealthController>().naturalRegenAmount = hpRegenAmount;
            GameMaster.instance.player.GetComponent<JHealthController>().armor = armor;
            if (hp > maxHP) {
                GameMaster.instance.player.GetComponent<JHealthController>().hp = hp;
            }
            GameMaster.instance.playerEnergy.maxEnergy = maxEnergy;
            GameMaster.instance.playerEnergy.energyRegenAmount = energyRegen;
            GameMaster.instance.player.GetComponent<Dash>().dashCooldown = dashCD;
        }

        if (died) {
            died = false;
            if (skeletonKingBuff && !previousRocket) {
                GameMaster.instance.player.GetComponent<JHealthController>().naturalRegenAmount = 1;
            }
            if (centicleBuff && !previousCenticle) {
                GameMaster.instance.playerEnergy.maxEnergy += 50;
                GameMaster.instance.playerEnergy.energyRegenAmount += 1;
            }
            if (stalkerBuff && !previousStalker) {
                GameMaster.instance.player.GetComponent<Dash>().dashCooldown = 1;
            }
            if (rocketManBuff && !previousRocket) {
                GameMaster.instance.player.GetComponent<JHealthController>().environmentalImmune = true;
                GameMaster.instance.player.GetComponent<JHealthController>().IncreaseMaxHealth(5);
            }
        }
    }

    IEnumerator SpawnRandomWeapons() {
        yield return new WaitForSeconds(0.25f);
        Item selected = allItems[(int)Random.Range(0f, allItems.Length)];
        for (int i = 0; i < 2; i++) {
            while (selected.itemType != Item.ITEMTYPE.Weapon) {
                selected = allItems[(int)Random.Range(0f, allItems.Length)];
            }
            GameMaster.instance.GivePlayerItem(new ItemStack(selected, 10, selected.durability));
            selected = allItems[(int)Random.Range(0f, allItems.Length)];
        }
        for (int i = 0; i < 2; i++) {
            while (selected.itemType != Item.ITEMTYPE.Usable && selected.itemType != Item.ITEMTYPE.Consumable) {
                selected = allItems[(int)Random.Range(0f, allItems.Length)];
            }
            GameMaster.instance.GivePlayerItem(new ItemStack(selected, 10, selected.durability));
            selected = allItems[(int)Random.Range(0f, allItems.Length)];
        }
        if (Random.Range(0f, 1f) < 0.5f) {
            while (selected.itemType != Item.ITEMTYPE.CharacterUpgrade) {
                selected = allItems[(int)Random.Range(0f, allItems.Length)];
            }
            GameMaster.instance.GivePlayerItem(new ItemStack(selected, 10, selected.durability));
        }
    }

    //void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
    //    Debug.Log("Level Loaded: ");
    //    Debug.Log(scene.name);
    //    Debug.Log(mode);
    //    if (world > 0) {
    //        GameMaster.instance.StartLevel();
    //    }

    //}

}

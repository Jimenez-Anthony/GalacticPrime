using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour {

    public Image itemImage;
    public DroppedItem droppedItem;

    //public GameObject inventory;
    public GameObject[] slots;
    public ItemStack[] inventory;
    public int selectedSlot = -1;

    private float pickupTimer;
    private float selectTimer;

    void Start() {
        pickupTimer = 0f;
        if (LevelManager.instance.storedInventory == null || LevelManager.instance.infinite) {
            inventory = new ItemStack[slots.Length];
        }
        else {
            print("[Game Master] Reloaded inventory");
            inventory = new ItemStack[slots.Length];
            for (int i = 0; i < inventory.Length; i++) {
                if (LevelManager.instance.storedInventory[i] != null && LevelManager.instance.storedInventory[i].item != null) {
                    inventory[i] = new ItemStack(LevelManager.instance.storedInventory[i].item, LevelManager.instance.storedInventory[i].count, LevelManager.instance.storedInventory[i].durability);
                }
            }
            //inventory = LevelManager.instance.storedInventory;
            UpdateGUI();
        }

    }

    void Update() {
        if (pickupTimer > 0f) {
            pickupTimer -= Time.deltaTime;
        }
        if (selectTimer > 0f) {
            selectTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (inventory[0] != null && inventory[0].item != null) {
                if (selectedSlot != 0) {
                    Deselect();
                    Select(0);
                }
                else {
                    Deselect();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            if (inventory[1] != null && inventory[1].item != null) {
                if (selectedSlot != 1) {
                    Deselect();
                    Select(1);
                }
                else {
                    Deselect();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            if (inventory[2] != null && inventory[2].item != null) {
                if (selectedSlot != 2) {
                    Deselect();
                    Select(2);
                }
                else {
                    Deselect();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            if (inventory[3] != null && inventory[3].item != null) {
                if (selectedSlot != 3) {
                    Deselect();
                    Select(3);
                }
                else {
                    Deselect();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            if (inventory[4] != null) {
                if (selectedSlot != 4 && inventory[4].item != null) {
                    Deselect();
                    Select(4);
                }
                else {
                    Deselect();
                }
            }
        }

        if (Input.GetKeyDown(Keys.nextItem)) {
            if (selectedSlot != -1) {
                int nextSlot = selectedSlot + 1;
                if (nextSlot > 4) {
                    nextSlot = 0;
                }
                if (inventory[nextSlot] != null) {
                    Deselect();
                    Select(nextSlot);
                }
            }

        }
        if (Input.GetKeyDown(Keys.previousItem)) {
            if (selectedSlot != -1) {
                int nextSlot = selectedSlot - 1;
                if (nextSlot < 0) {
                    nextSlot = inventory.Length - 1;
                }
                if (inventory[nextSlot] != null) {
                    Deselect();
                    Select(nextSlot);
                }
            }

        }

        if (Input.GetKeyDown(Keys.dropItem) && selectedSlot != -1) {
            Drop(selectedSlot);
            Deselect();
        }
    }

    public void UpdateCooldown(float percentage) {
        if (selectedSlot != -1) {
            slots[selectedSlot].transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = percentage;
        }
    }

    public void Deselect(bool keepObject = false) {
        selectedSlot = -1;
        foreach (GameObject slot in slots) {
            if (slot.transform.GetChild(1).name == "ItemSelection") {
                slot.transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (slot.transform.GetChild(0).name == "ItemSelection") {
                slot.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        if (!keepObject && transform.GetChild(3).childCount > 0) {
            Destroy(transform.GetChild(3).GetChild(0).gameObject);
            GameMaster.instance.player.GetComponent<Animator>().SetBool("IsHolding", false);
        }
    }

    public void Select(int slot) {
        if (GameMaster.instance.player.GetComponent<JPlayerController>().stunned) {
            return;
        }
        if (selectTimer > 0f) {
            return;
        }
        else {
            selectTimer = 0.1f;
        }

        selectedSlot = slot;
        slots[slot].transform.GetChild(1).gameObject.SetActive(true);
        slots[slot].transform.GetChild(2).gameObject.SetActive(true);
        GameObject clone = Instantiate(inventory[slot].item.prefab, transform.GetChild(3), false) as GameObject;
        clone.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        GameMaster.instance.player.GetComponent<Animator>().SetBool("IsHolding", true);

        if (inventory[selectedSlot].item.itemType != Item.ITEMTYPE.Weapon) {
            UpdateCooldown(0f);
        }
    }

    public void UseReusableItem() {
        if (!RemoveSelectedItem()) {
            Select(selectedSlot);
            UpdateCount(selectedSlot);
        }
    }

    public void Drop(int slot) {
        Deselect();
        JPlayerController playerCont = GetComponent<JPlayerController>();
        int dir = 0;
        if (playerCont.facingRight) {
            dir = 1;
        }
        else {
            dir = -1;
        }
        DroppedItem dropped = Instantiate(droppedItem, new Vector3(transform.position.x + dir, transform.position.y, 0f), Quaternion.identity);
        dropped.itemStack = inventory[slot];
        dropped.pickUPTimer = 0.5f;
        inventory[slot] = null;
        slots[slot].transform.GetChild(2).gameObject.SetActive(false);
        Destroy(slots[slot].transform.GetChild(0).gameObject);
        if (dropped.itemStack.item.itemType == Item.ITEMTYPE.CharacterUpgrade) {
            IItemPassive passive = dropped.itemStack.item.prefab.GetComponent<IItemPassive>();
            passive.OnUnequip();
        }
        Deselect();
        pickupTimer = 0.1f;
    }

    public void DestroySelected() {
        inventory[selectedSlot] = null;
        slots[selectedSlot].transform.GetChild(2).gameObject.SetActive(false);
        Destroy(slots[selectedSlot].transform.GetChild(0).gameObject);
        Deselect(true);
    }

    public bool PickUpItem(ItemStack itemStack) {
        if (pickupTimer > 0f) {
            return false;
        }

        if (itemStack.item.itemType == Item.ITEMTYPE.Weapon) {
            for (int i = 0; i <= 1; i++) {
                if (inventory[i] == null) {
                    //Deselect();
                    print("[Inventory] Added new " + itemStack.durability + " " + itemStack.item.name);
                    AddItemToSlot(i, itemStack);
                    AddItemToUI(i, itemStack);
                    UpdateCount(i);
                    return true;
                }
            }
        }
        else if (itemStack.item.itemType == Item.ITEMTYPE.CharacterUpgrade) {
            for (int i = 4; i <= 4; i++) {
                if (inventory[i] == null) {
                    //Deselect();
                    print("[Inventory] Added new " + itemStack.item.name);
                    AddItemToSlot(i, itemStack);
                    AddItemToUI(i, itemStack);
                    UpdateCount(i);
                    return true;
                }
            }
        }
        else {
            for (int i = 2; i <= 3; i++) {
                if (inventory[i] != null && inventory[i].item == itemStack.item && inventory[i].count < 10) {
                    print("[Inventory] Added " + itemStack.count + " to " + itemStack.item.name);
                    inventory[i].count += itemStack.count;
                    UpdateCount(i);
                    return true;
                }
            }
            for (int i = 2; i <= 3; i++) {
                if (inventory[i] == null) {
                    //Deselect();
                    print("[Inventory] Added new " + itemStack.count + " " + itemStack.item.name);
                    AddItemToSlot(i, itemStack);
                    AddItemToUI(i, itemStack);
                    UpdateCount(i, true);
                    return true;
                }
            }
        }

        return false;
    }

    public void UpdateGUI() {
        for (int i = 0; i < inventory.Length; i++) {
            if (inventory[i] != null && inventory[i].item != null) {
                AddItemToUI(i, inventory[i]);
                UpdateCount(i);
            }
        }
    }

    public bool RemoveSelectedItem() {
        inventory[selectedSlot].count -= 1;
        UpdateCount(selectedSlot);
        if (inventory[selectedSlot].count <= 0) {
            DestroySelected();
            Deselect();
            return true;
        }
        return false;
    }

    public int GetSelectedCount() {
        return inventory[selectedSlot].count;
    }

    void AddItemToSlot(int slot, ItemStack itemStack) {
        inventory[slot] = itemStack;
        if (itemStack.item.itemType == Item.ITEMTYPE.CharacterUpgrade) {
            IItemPassive passive = itemStack.item.prefab.GetComponent<IItemPassive>();
            passive.OnEquip();
        }
    }

    void AddItemToUI(int i, ItemStack itemStack) {
        Image img = Instantiate(itemImage, slots[i].transform, false) as Image;
        img.transform.SetSiblingIndex(0);
        if (inventory[i].item.itemType == Item.ITEMTYPE.Usable || inventory[i].item.itemType == Item.ITEMTYPE.Consumable || inventory[i].item.itemType == Item.ITEMTYPE.Weapon) {
            slots[i].transform.GetChild(2).gameObject.SetActive(true);
        }
        img.sprite = itemStack.item.icon;
    }

    void UpdateCount(int i, bool newItem = false) {
        if (slots[i].transform.GetChild(2).GetComponent<TMP_Text>() != null) {
            if (inventory[i].item.itemType == Item.ITEMTYPE.Usable || inventory[i].item.itemType == Item.ITEMTYPE.Consumable) {
                slots[i].transform.GetChild(2).gameObject.SetActive(true);
                slots[i].transform.GetChild(2).GetComponent<TMP_Text>().text = inventory[i].count.ToString();
            }
            if (inventory[i].item.itemType == Item.ITEMTYPE.Weapon) {
                slots[i].transform.GetChild(2).gameObject.SetActive(true);
                slots[i].transform.GetChild(2).GetComponent<TMP_Text>().text = inventory[i].durability.ToString();
            }
        }
        else {
            if (inventory[i].item.itemType == Item.ITEMTYPE.Usable || inventory[i].item.itemType == Item.ITEMTYPE.Consumable) {
                slots[i].transform.GetChild(3).gameObject.SetActive(true);
                slots[i].transform.GetChild(3).GetComponent<TMP_Text>().text = inventory[i].count.ToString();
            }
            if (inventory[i].item.itemType == Item.ITEMTYPE.Weapon) {
                slots[i].transform.GetChild(3).gameObject.SetActive(true);
                slots[i].transform.GetChild(3).GetComponent<TMP_Text>().text = inventory[i].durability.ToString();
            }
        }
    }

    public void UseDurability(int amount) {
        UseDurability(selectedSlot, amount);
    }

    public void UseDurability(int slot, int amount) {
        if (LevelManager.instance.world == 4) {
            return;
        }

        inventory[slot].durability -= amount;
        if (inventory[slot].durability <= 0) {
            if (inventory[slot].item.itemType == Item.ITEMTYPE.CharacterUpgrade) {
                IItemPassive passive = inventory[slot].item.prefab.GetComponent<IItemPassive>();
                passive.OnUnequip();
            }
            inventory[slot] = null;
            slots[slot].transform.GetChild(2).gameObject.SetActive(false);
            Destroy(slots[slot].transform.GetChild(0).gameObject);
            Deselect();
        }
        UpdateCount(slot);
    }

    public ItemStack GetSelectedItem() {
        return inventory[selectedSlot];
    }

}

[System.Serializable]
public class ItemStack {
    public Item item;
    public int count;
    public int durability;

    public ItemStack(Item _item, int _count, int _durability) {
        item = _item;
        count = _count;
        durability = _durability;
    }
}

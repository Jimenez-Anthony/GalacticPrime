using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{

    private Inventory inventory;
    public GameObject panel;
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemDescription;

    private void Start() {
        inventory = GameMaster.instance.inventory;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Keys.itemPanel) && inventory.selectedSlot != -1) {
            panel.SetActive(true);
            ItemStack itemStack = inventory.GetSelectedItem();
            itemImage.sprite = itemStack.item.icon;
            itemName.text = itemStack.item.name;
            string description = "";
            if (itemStack.item.itemType == Item.ITEMTYPE.Weapon) {
                description += "Weapon\n\n";
            }
            else if (itemStack.item.itemType == Item.ITEMTYPE.CharacterUpgrade) {
                description += "Character Upgrade\n\n";
            }
            else if (itemStack.item.itemType == Item.ITEMTYPE.Consumable) {
                description += "Consumable\n\n";
            }
            else if (itemStack.item.itemType == Item.ITEMTYPE.Usable) {
                description += "Utility\n\n";
            }

            description += itemStack.item.description;

            itemDescription.text = description;
        }

        if (Input.GetKeyUp(Keys.itemPanel)) {
            panel.SetActive(false);
        }
    }
}

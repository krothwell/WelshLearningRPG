﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityUtilities;


namespace GameUI {
    /// <summary>
    /// Responsible for displaying inventory items and how they should be moved
    /// to and from UI slots, and the game world. 
    /// </summary>
    public class PlayerInventoryUI : MonoBehaviour {
        GameObject items;
        GameObject closeBtn;
        GameObject panel;
        GameObject selectedItem;
        public GameObject[] equippedItemRigSlots = new GameObject[15];
        GameObject ui;
        //bool followCursor = false;
        public GameObject inventoryItemPrefab;
        // Use this for initialization
        void Start() {
            ui = GameObject.Find("UI");
            panel = transform.FindChild("Panel").gameObject;
            items = panel.transform.FindChild("ItemSlots").gameObject;
        }

        void Update() {
            if (selectedItem != null) {
                SetSelectedItemToCursor();
            }
        }

        public void OpenInventory() {
            panel.SetActive(true);
        }

        public void CloseInventory() {
            //TODO: Check if delay click select from MouseSelection.cs can be used to delay selecting world isntead.
            Invoke("CloseInventoryNow", 0.1f);//delayed so that character doesn't begin walking to button press
        }

        public void CloseInventoryNow() {
 
            panel.SetActive(false);

        }

        public void RecieveItem(WorldItem worldItem) {
            foreach (Transform inventorySlot in items.transform) {
                if (inventorySlot.childCount <= 0) {
                    worldItem.transform.SetParent(inventorySlot, false);
                    break;
                }
            }

        }

        public bool IsItemSelected() {
            return (selectedItem != null);
        }

        public void AttemptToPickUpItemInSlot(GameObject slot) {
            if (slot.transform.childCount > 0) {
                    SelectItem(slot);
            }
        }

        public void AttemptToPutItemInSlot(GameObject slot) {
            if (slot.transform.childCount <= 0) {
                if (slot.HasComponent<PlayerEquipmentSlot>()) {
                    if (slot.GetComponent<PlayerEquipmentSlot>().itemType
                        == selectedItem.GetComponent<WorldItem>().itemType) {
                        InsertSelectedItemToSlot(slot, true);
                    }
                }
                else if (slot.HasComponent<PlayerInventorySlot>()) {
                    InsertSelectedItemToSlot(slot);
                }
            }
        }

        private void SetSelectedItemToCursor() {
            Vector2 mousePos = new Vector2(
                Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            selectedItem.GetComponent<RectTransform>().position = mousePos;
        }

        public void SelectItem(GameObject slot) {
            if (selectedItem == null) {
                selectedItem = slot.transform.GetChild(0).gameObject;
                //selectedItemParent = slot.transform; //incase next action fails it can go back here
                if (slot.HasComponent<PlayerEquipmentSlot>()) {
                    selectedItem.GetComponent<WorldItem>().UnequipFromPlayerModel();
                }
                //followCursor = true;
                selectedItem.GetComponent<BoxCollider2D>().enabled = false;
                selectedItem.transform.SetParent(ui.transform);
            }
        }

        public void InsertSelectedItemToSlot(GameObject selectedSlot, bool equip = false) {
            if (equip) {
                selectedItem.GetComponent<WorldItem>().EquipToPlayerModel();
            }
            //followCursor = false;
            selectedItem.transform.SetParent(selectedSlot.transform, false);
            selectedItem.transform.localPosition = new Vector3(0f, 0f, 0f);
            selectedItem = null;
        }

    }
}
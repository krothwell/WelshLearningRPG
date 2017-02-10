﻿using UnityEngine;
using UnityEngine.UI;
using DataUI.Utilities;
using System.Collections;
using DbUtilities;
using UnityUtilities;
namespace DataUI {
    namespace ListItems {
        public class Dialogue : UIInputListItem, ISelectableUI, IEditableUI {
            DialogueUI dialogueUI;
            
            Toggle activeToggle;
            Image inputBG;
            GameObject options, saveDialogue;

            
            bool editable;
            private string myDescription;
            public string MyDescription {
                get { return myDescription; }
                set { myDescription = value; }
            }
            private string myID;
            public string MyID {
                get { return myID; }
                set { myID = value; }
            }
            private bool active;
            public bool Active {
                get { return active; }
                set { active = value; }
            }
            // Use this for initialization
            void Start() {
                dialogueUI = FindObjectOfType<DialogueUI>();
                options = transform.FindChild("DialogueOptions").gameObject;
                saveDialogue = transform.FindChild("Save").gameObject;
                activeToggle = transform.GetComponentInChildren<Toggle>();
            }

            void OnMouseUp() {
                if (!editable) {
                    dialogueUI.ToggleSelectionTo(gameObject.GetComponent<Dialogue>(), dialogueUI.selectedDialogue);
                }
            }

            public void SelectSelf() {
                DisplayOptions();
                SetInputColour(Colours.colorDataUIInputSelected);
                dialogueUI.DisplayCharsRelatedToDialogue();
                dialogueUI.DisplayNodesRelatedToDialogue();
                dialogueUI.HidePlayerChoices();
                dialogueUI.HidePlayerChoiceResults();
            }

            public void DeselectSelf() {
                HideOptions();
                SetInputColour(Color.white);
                GetInputField().text = myDescription;
                activeToggle.isOn = active;
                GetInputField().readOnly = true;
                activeToggle.interactable = false;
                saveDialogue.SetActive(false);
                editable = false;
            }

            private void DisplayOptions() {
                if (!editable) {
                    options.SetActive(true);
                }
            }

            private void HideOptions() {
                options.SetActive(false);
            }

            public void DeleteDialogue() {
                string[,] fields = { { "DialogueIDs", myID } };
                DbCommands.DeleteTupleInTable("Dialogues",
                                             fields);
                Destroy(gameObject);
                dialogueUI.HideCharsRelatedToDialogue();
                dialogueUI.HideNodesRelatedToDialogue();
            }

            public void SetEditable() {
                editable = true;
                GetInputField().readOnly = false;
                activeToggle.interactable = true;
                HideOptions();
                saveDialogue.SetActive(true);
                GetInputField().Select();
            }


            public void SaveEdits() {
                string isActive = activeToggle.isOn ? "1" : "0";
                print(isActive);
                string[,] fields = new string[,]{
                                            { "DialogueDescriptions", GetInputField().text },
                                            { "Active", isActive }
                                         };
                DbCommands.UpdateTableTuple("Dialogues",
                                         "DialogueIDs = " + myID,
                                         fields);
                MyDescription = GetInputField().text;
                active = activeToggle.isOn;
                editable = false;
                GetInputField().readOnly = true;
                activeToggle.interactable = false;
                saveDialogue.SetActive(false);
                dialogueUI.ToggleSelectionTo(GetComponent<Dialogue>(), dialogueUI.selectedDialogue);
            }


        }
    }
}
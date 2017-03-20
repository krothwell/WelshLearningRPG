﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GameUI.ListItems;
using UIUtilities;
using DbUtilities;
using UnityUtilities;
using System;

/// <summary>
/// When certain in-game interactions take place, this class is responsible for 
/// communicating with game objects and the database to determine the display of
/// pre-stored dialogue, quest input (e.g. a quest may ask you to introduce 
/// yourself to 5 different npcs – when you start a dialogue with a new npc, the
/// quest action will be included in the dialogue options) and combat input 
/// (when a combat ability is used it may test the player asking him to 
/// translate something in Welsh. It also provides side effects including 
/// character portrait changing depending on speaker, and break down of player 
/// input results (% of answer that the player input that is correct).
/// </summary>
namespace GameUI {
    public class DialogueUI : UIController {
        bool submissionScored = false;
    	private string testEnglish;
    	private string testWelsh;
    	public string TestEnglish {
    		get {return testEnglish;}
    		set {testEnglish = value;}
    	}
    	public string TestWelsh {
    		get {return testWelsh;}
    		set {testWelsh = value;}
    	}
    	Text answerTxt, percentageTxt;
        GameObject answerField;
        InputField answerInput;
        GameObject submitBtn;
        CombatUI combatUI;
        Text btnTxt;
        Animator animator;
        QuestsUI questsUI;
        NewWelshLearnedUI newWelshLearnedUI;
        Image objPortrait;
        protected RectTransform contentPanel;

        public GameObject 
            dialogueHolderPrefab,
            characterSpeakingPrefab, 
            inGameDialogueNode, 
            inGamePlayerChoice, 
            endDialogueBtn, 
            spacer, 
            emptyBlockPrefab;
        private ScrollRect dialogueScroller;
        private GameObject currentDialogueHolder, currentDialogueNode, currentCharSpeaking;
        private string currentCharID, currentDialogueID;
        private PlayerCharacter player;
        public Character currentChar;
        private Sprite currentPortrait;
        private NPCs npcs;

        void Awake() {
            dialogueScroller = transform.GetComponentInChildren<ScrollRect>();
        }
        // Use this for initialization
        void Start() {
            questsUI = FindObjectOfType<QuestsUI>();
            newWelshLearnedUI = FindObjectOfType<NewWelshLearnedUI>();
            npcs = FindObjectOfType<NPCs>();
            player = FindObjectOfType<PlayerCharacter>();
            panel = transform.FindChild("Panel").gameObject;
            answerField = panel.transform.FindChild("InputField").gameObject;
            answerInput = answerField.GetComponent<InputField>();
            answerTxt = answerInput.transform.FindChild("Text").GetComponent<Text>();
            submitBtn = panel.transform.FindChild("SubmitBtn").gameObject;
            percentageTxt = panel.transform.FindChild("PercentageTxt").GetComponent<Text>();
            combatUI = FindObjectOfType<CombatUI>();
            btnTxt = submitBtn.transform.FindChild("Text").gameObject.GetComponent<Text>();
            player = FindObjectOfType<PlayerCharacter>();
            objPortrait = panel.transform.FindChild("CharacterPortrait").GetComponent<Image>();
        }

        // Update is called once per frame
        void Update() {

        }

        public void StartNewDialogue(Character character) {
            ResetLowerDialogueContainer();
            currentChar = character;
            currentCharID = character.CharacterName;
            SetDialogueID();
            DisplayFirstDialogueNode();
        }

        public void StartNewDialogue(string dialogueID) {
            ResetLowerDialogueContainer();
            currentCharID = DbCommands.GetFieldValueFromTable("CharacterDialogues", "CharacterNames", "DialogueIDs = " + dialogueID);
            currentChar = npcs.GetCharacterFromName(currentCharID);
            currentDialogueID = dialogueID;
            DisplayFirstDialogueNode();
        }

        public void ResetLowerDialogueContainer() {
            foreach (Transform dialogue in dialogueScroller.gameObject.transform) {
                Destroy(dialogue.gameObject);
            }
            currentDialogueHolder = null;
        }

        private void DisplayFirstDialogueNode() {
            if (!IsDialogueComplete()) {
                SetInUse();
                currentDialogueHolder = Instantiate(dialogueHolderPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
                currentDialogueHolder.transform.SetParent(dialogueScroller.gameObject.transform, false);
                dialogueScroller.GetComponent<ScrollRect>().content = currentDialogueHolder.GetComponent<RectTransform>();
                string[] nodeArray = DbCommands.GetTupleFromTable("DialogueNodes", "DialogueIDs = " + currentDialogueID, "NodeIDs ASC");
                DisplayDialogueNode(nodeArray);
            }
            else {
                SetNotInUse();
            }
        }

        private bool IsDialogueComplete() {
            string dialogueComplete = DbCommands.GetFieldValueFromTable("ActivatedDialogues", "Completed", "DialogueIDs = " + currentDialogueID + " AND SaveIDs = 0");
            //no character dialogue so return true
            if(dialogueComplete == "") {
                return true;
            } else {
                return Convert.ToBoolean(int.Parse(dialogueComplete));
            }
        }

        private string GetSpeakersName(string nodeID) {
            string overrideName = DbCommands.GetFieldValueFromTable("DialogueNodes", "CharacterSpeaking", "NodeIDs = " + nodeID);
            if (overrideName == "") {
                return currentChar.CharacterName;
            }
            else if (overrideName == "!Player") {
                return player.GetMyName();
            }
            else {
                return overrideName;
            }
        }

        public void DisplayDialogueNode(string[] nodeArray) {
            //check if node character override is there and use the override name if so.
            string charName = GetSpeakersName(nodeArray[0]);
            InsertCharName(charName);
            GameObject speakerScrollObj = currentCharSpeaking.gameObject;
            SetCurrentPortraitFromName(charName);
            DialogueNode dialogueNode = (Instantiate(
                inGameDialogueNode,
                new Vector3(0f, 0f, 0f),
                Quaternion.identity) as GameObject).GetComponent<DialogueNode>();
            dialogueNode.MyID = nodeArray[0];
            dialogueNode.MyText = nodeArray[1];
            dialogueNode.transform.SetParent(currentDialogueHolder.transform, false);
            dialogueNode.GetComponent<DialogueNode>().SetDisplay();
            DisplayNodeChoices(nodeArray[0]);
            ScrollToDialogueElement(speakerScrollObj);
        }

        public void SetCurrentPortraitFromName(string charName) {
            if (charName != player.GetMyName()) {
                Character charOfName = npcs.GetCharacterFromName(charName);
                currentPortrait = charOfName.GetMyPortrait();
                SetObjectPortrait(currentPortrait);
            }
            else {
                SetObjectPortrait(player.GetPlayerPortrait());
                currentPortrait = player.GetPlayerPortrait();
            }

        }

        public void SetCurrentPortrait() {
            SetObjectPortrait(currentPortrait);
        }

        private void DisplayNodeChoices(string nodeID) {
            InsertSpacer();
            //check if player is node character override, if not then the name can be inserted
            if (GetSpeakersName(nodeID) != player.GetMyName()) {
                InsertCharName(player.GetMyName());
            }
            string nodeChoiceQry = "SELECT * FROM PlayerChoices WHERE NodeIDs = " + nodeID + ";";
            AppendDisplayFromDb(nodeChoiceQry, currentDialogueHolder.transform, BuildNodeChoice);
            string displayEndDialogueIndicator = (DbCommands.GetFieldValueFromTable("DialogueNodes", "EndDialogueOption", "DialogueIDs = " + nodeID));
            int choicesInt = DbCommands.GetCountFromTable("PlayerChoices", "NodeIDs = " + nodeID);
            if (displayEndDialogueIndicator == "1" || choicesInt == 0) {
                InsertEndDialogue();
            }
        }

        private Transform BuildNodeChoice(string[] strArray) {
            string idStr = strArray[0];
            string choiceText = strArray[1];
            string nextNode = strArray[3];
            GameObject choice = Instantiate(inGamePlayerChoice, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            choice.transform.GetComponentInChildren<Text>().text = "\t" + choiceText;
            choice.GetComponent<PlayerChoice>().MyID = idStr;
            choice.GetComponent<PlayerChoice>().MyText = choiceText;
            choice.GetComponent<PlayerChoice>().MyNextNode = nextNode;
            choice.GetComponent<PlayerChoice>().SetDialogueUI(GetComponent<DialogueUI>());
            return choice.transform;
        }

        public void DestroyInteractiveChoices() {
            foreach (Transform component in currentDialogueHolder.transform) {
                Button buttonCheck = component.GetComponent<Button>();
                if (buttonCheck != null) {
                    if (buttonCheck.interactable) {
                        Destroy(component.gameObject);
                    }
                }
            }
        }

        public void InsertSpacer() {
            GameObject newSpacer = Instantiate(spacer, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            newSpacer.transform.SetParent(currentDialogueHolder.transform, false);
        }

        public void InsertCharName(string name) {
            GameObject charSpeaking = Instantiate(characterSpeakingPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            charSpeaking.GetComponent<Text>().text = "<b>" + name + "</b>:";
            charSpeaking.transform.SetParent(currentDialogueHolder.transform, false);
            currentCharSpeaking = charSpeaking;
        }

        public void InsertEndDialogue() {
            GameObject endDialogue = Instantiate(endDialogueBtn, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            endDialogue.transform.SetParent(currentDialogueHolder.transform, false);
            endDialogue.GetComponent<Text>().text = "\t<i>End Dialogue</i>";
        }


        private void SetDialogueID() {
            string qry = "SELECT CharacterDialogues.DialogueIDs FROM CharacterDialogues "
                        + "INNER JOIN ActivatedDialogues ON CharacterDialogues.DialogueIDs = ActivatedDialogues.DialogueIDs "
                        + "WHERE CharacterDialogues.CharacterNames = " + DbCommands.GetParameterNameFromValue(currentCharID)
                            + " AND ActivatedDialogues.Completed = 0"
                            + " AND ActivatedDialogues.SaveIDs = 0";
            //Debugging.PrintDbQryResults(qry);
            Debugging.PrintDbTable("ActivatedDialogues");
            currentDialogueID = DbCommands.GetFieldValueFromQry(qry, "DialogueIDs", currentCharID);
            currentDialogueID = (currentDialogueID == "") ? "-1" : currentDialogueID;

            
        }

        public void ScrollToDialogueElement(GameObject element) {
            UICommands.SnapScrollToContentChild(element, dialogueScroller);
        }

        public GameObject GetCurrentSpeaker() {
            return currentCharSpeaking;
        }
        
    	public void SetPercentageCorrect() {
    		int welshLength = testWelsh.Length;
    		int percentage = 0;
    		int countCorrect = 0;
    		string answer = answerTxt.text;
    		for(int i = 0; i < welshLength; i++) {
    			if (i < answer.Length) {
    				if (answer[i] == testWelsh[i]) {
    					countCorrect++;
    				} 
    			} else { break; }
    		}
    
    		percentage = (int)Mathf.Round((100f/welshLength) * countCorrect);
    		Debug.Log(countCorrect);
    		percentageTxt.text = percentage.ToString() + "%";
    
    	}
    	
    	public void ActivateAnswerField() {
            //answerField.SetActive(true);
            //answerInput.Select();
        }
        
        public void SetInUse() {
            animator = GetComponent<Animator>();
            animator.SetBool("InUse", true);
        }


        public void SetNotInUse() {
            animator = GetComponent<Animator>();
            animator.SetBool("InUse", false);
            player.DestroySelectionCircleOfInteractiveObject();
            player.playerStatus = PlayerCharacter.PlayerStatus.passive;
        }
        
        public void SetBtnText(string newText) {
            btnTxt.text = newText;
        }
        
        public void ManageLowerUISubmission() {
            if (submissionScored) {
                if (combatUI.currentAbility == (CombatUI.CombatAbilities.strike)) { 
                    //player.StrikeSelectedEnemy();
                    combatUI.ToggleCombatUI();
                    submissionScored = false;
                    SetBtnText("Submit answer");
                    SetNotInUse();
                }
            } else {
                SetPercentageCorrect();
                submissionScored = true;
                SetBtnText("Finish move");
            }
        }
    
        public void SetObjectPortrait(Sprite portrait) {
            objPortrait.sprite = portrait;
        }

        public void MarkDialogueComplete(string choiceID) {
            bool isDialogueComplete = Convert.ToBoolean(DbCommands.GetCountFromTable("PlayerChoices", "ChoiceIDs = " + choiceID + " AND MarkDialogueCompleted = 1"));
            if (isDialogueComplete) {
                int activatedDialogueCount = DbCommands.GetCountFromTable("ActivatedDialogues", "SaveIDs = 0 AND Completed = 0 AND DialogueIDs = " + currentDialogueID);
                if (activatedDialogueCount > 0) {
                    DbCommands.UpdateTableField("ActivatedDialogues", "Completed", "1", "SaveIDs = 0 AND DialogueIDs = " + currentDialogueID);
                }
            }
        }

        public void ActivateQuests(string choiceID) {
            int countQuestActivateResults = DbCommands.GetCountFromQry(DbQueries.GetQuestActivateCountFromChoiceIDqry(choiceID));
            if (countQuestActivateResults > 0) {
                List<string[]> questsActivatedList;
                DbCommands.GetDataStringsFromQry(DbQueries.GetCurrentActivateQuestsPlayerChoiceResultQry(choiceID), out questsActivatedList);
                foreach (string[] activatedQuest in questsActivatedList) {
                    questsUI.InsertActivatedQuest(activatedQuest[1]);
                }
            }
        }

        public void ActivateQuestTasks(string choiceID) {
            int countTaskActivateResults = DbCommands.GetCountFromQry(DbQueries.GetTaskActivateCountFromChoiceIDqry(choiceID));
            if (countTaskActivateResults > 0) {
                print("Task ACTIVATING!!!!");
                List<string[]> tasksActivatedList;
                DbCommands.GetDataStringsFromQry(DbQueries.GetCurrentActivateTasksPlayerChoiceResultQry(choiceID), out tasksActivatedList);
                foreach (string[] activatedTask in tasksActivatedList) {
                    questsUI.InsertActivatedTask(activatedTask[1], activatedTask[3], activatedTask[2]);
                }
            }
        }

        public void ActivateNewWelsh(string choiceID) {
            ActivateNewGrammar(choiceID);
            ActivateNewVocab(choiceID);
            newWelshLearnedUI.DisplayNewWelsh(choiceID);
        }

        public void ActivateNewGrammar(string choiceID) {
            int countGrammarActivateResults = DbCommands.GetCountFromQry(DbQueries.GetGrammarActivateCountFromChoiceIDqry(choiceID));
            if (countGrammarActivateResults > 0) {
                print("Grammar ACTIVATING!!!!");
                List<string[]> grammarActivatedList;
                DbCommands.GetDataStringsFromQry(DbQueries.GetCurrentActivateGrammarPlayerChoiceResultQry(choiceID), out grammarActivatedList);
                foreach (string[] activatedGrammar in grammarActivatedList) {
                    newWelshLearnedUI.InsertDiscoveredGrammar(activatedGrammar[1]);
                }
            }

        }

        public void ActivateNewVocab(string choiceID) {
            int countVocabActivateResults = DbCommands.GetCountFromQry(DbQueries.GetVocabActivateCountFromChoiceIDqry(choiceID));
            if (countVocabActivateResults > 0) {
                print("Vocab ACTIVATING!!!!");
                List<string[]> vocabActivatedList;
                DbCommands.GetDataStringsFromQry(DbQueries.GetCurrentActivateVocabPlayerChoiceResultQry(choiceID), out vocabActivatedList);
                foreach (string[] activatedVocab in vocabActivatedList) {
                    newWelshLearnedUI.InsertDiscoveredVocab(activatedVocab[2], activatedVocab[3]);
                }
            }
        }

        public int GetChoiceResultsCount(string choiceID) {
            int countChoiceResults = DbCommands.GetCountFromTable("PlayerChoiceResults", "ChoiceIDs = " + choiceID);
            return countChoiceResults;
        }

    }
}
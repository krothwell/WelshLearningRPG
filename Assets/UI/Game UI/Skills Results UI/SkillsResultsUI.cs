﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DbUtilities;
using GameUI.ListItems;

public class SkillsResultsUI : UIController {
    public GameObject WelshVocabUpdatePrefab, WelshGrammarUpdatePrefab;
    DialogueTestDataController testData;
    Text percentageCorrect, resultLabel, talliesShift, testType, skillPointsGained, answerGiven;
    Transform summaryBreakdown, welshTestOverview, mainOverview, welshSkillsList;
	void Awake () {
        summaryBreakdown = GetPanel().transform.FindChild("SummaryBreakdown");
        welshTestOverview = GetPanel().transform.FindChild("WelshTestOverview");
        mainOverview = summaryBreakdown.FindChild("MainOverview");
        percentageCorrect = mainOverview.FindChild("PercentageCorrect").GetComponentInChildren<Text>();
        resultLabel = mainOverview.FindChild("ResultLbl").GetComponent<Text>();
        talliesShift = mainOverview.FindChild("TalliesShift").GetComponent<Text>();
        testType = mainOverview.FindChild("TestType").GetComponent<Text>();
        skillPointsGained = mainOverview.FindChild("SkillPointsGained").GetComponent<Text>();
        answerGiven = summaryBreakdown.FindChild("AnswerGiven").GetComponent<Text>();
        welshSkillsList = welshTestOverview.GetComponentInChildren<VerticalLayoutGroup>().transform;
    }

    public void DisplayResults(DialogueTestDataController testDataController) {
        DisplayComponents();
        testData = testDataController;
        SetPercentageCorrect();
        SetResultLabel();
        SetTestType();
        SetTalliesShift();
        SetSkillPointsGained();
        SetAnswerGiven();
        DisplayWelshSkills();
    }

    public void SetPercentageCorrect() {
        percentageCorrect.text = testData.GetAnswerPercentageCorrect().ToString() + "%";
    }

    public void SetResultLabel() {
        resultLabel.text = testData.GetResultString();
    }

    public void SetTalliesShift() {
        int tallyShift = testData.GetTallyShiftTotal();
        string modifier;
        if (tallyShift < 0) {
            modifier = "-";
        }
        else if (tallyShift > 0) {
            modifier = "+";
        } else {
            modifier = "";
        }
        talliesShift.text = modifier + Math.Abs(tallyShift).ToString();
    }

    public void SetTestType() {
        testType.text = testData.GetTestTypeString();
    }

    public void SetSkillPointsGained() {
        skillPointsGained.text = testData.GetSkillPointsGainedTotal().ToString();
    }

    public void SetAnswerGiven() {
        answerGiven.text = "\"" + testData.GetPlayerAnswer() + "\"";
    }

    public void DisplayWelshSkills() {
        EmptyDisplay(welshSkillsList);
        DisplayVocabUpdate();
        DisplayGrammarUpdate();
    }

    public void DisplayVocabUpdate() {
        Transform vocab = BuildVocabUpdate(testData.GetVocabUpdateData());
        vocab.SetParent(welshSkillsList, false);
    }

    public void DisplayGrammarUpdate() {
        Dictionary<int, string[]> grammarDetailsDict = testData.GetGrammarUpdateDict();
        foreach(KeyValuePair<int, string[]> pair in grammarDetailsDict) {
            string grammarID = pair.Key.ToString();
            string[] grammarDbData = DbCommands.GetTupleFromTable("VocabGrammar", "RuleIDs = " + grammarID);
            string[] grammarData = new string[6];
            grammarData[0] = grammarID; 
            grammarData[1] = grammarDbData[1]; //summary description
            grammarData[2] = grammarDbData[2]; //detailed description
            grammarData[3] = pair.Value[0]; //tally
            grammarData[4] = testData.GetTallyModifier().ToString(); //tally increasing or decreasing
            grammarData[5] = pair.Value[1]; //skill incrementing;
            Transform grammar = BuildGrammarUpdate(grammarData);
            grammar.SetParent(welshSkillsList, false);
        }
    }

    public Transform BuildVocabUpdate(string[] vocabData) {
        string eng = vocabData[0];
        string cym = vocabData[1];
        string tally = vocabData[2];
        WelshVocabUpdate welshVocab = (
            Instantiate(WelshVocabUpdatePrefab, new Vector3(0f, 0f), Quaternion.identity)
            ).GetComponent<WelshVocabUpdate>();
        welshVocab.InitialiseMe(eng, cym, tally, testData.GetTallyModifier(), testData.IsVocabSkillIncremented(), testData.GetTestType());
        return welshVocab.transform;
    }

    public Transform BuildGrammarUpdate(string[] vocabData) {
        string grammarID = vocabData[0];
        string summary = vocabData[1];
        string description = vocabData[2];
        string tally = vocabData[3];
        string tallyModifier = vocabData[4];
        string skillIncrementer = vocabData[5];
        WelshGrammarUpdate welshGrammar = (
            Instantiate(WelshGrammarUpdatePrefab, new Vector3(0f, 0f), Quaternion.identity)
            ).GetComponent<WelshGrammarUpdate>();
        welshGrammar.InitialiseMe(grammarID, summary, description, tally, tallyModifier, skillIncrementer);
        return welshGrammar.transform;
    }

}

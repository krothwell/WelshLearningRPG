﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using DbUtilities;
using System.Collections.Generic;
using UnityUtilities;

/// <summary>
/// An abstract class which provides default methods for the display of UI lists
/// by retrieving data from the database and default methods to display menus
/// and other UI related components.
/// </summary>
public class UIController : MonoBehaviour {
    protected GameObject panel;
    /// <summary>
    /// The menu toggle groups dictionary provides a consistent approach when displaying a UI menu where only one
    /// UI controller should be displayed at a time from the group. 
    /// A list of UIControllers can be added to the dictionary with a string key for referencing.
    /// </summary>
    private Dictionary<string, UIController> menuToggleGroups;
    /// <summary>
    /// The selection toggle groups dictionary provides a consistent approach when selecting a selectable UI list element
    /// where only one list element should be selected at a time. 
    /// A list of classes which implement the selectable interface can be added to the dictionary with a string key for referencing.
    /// </summary>
    private Dictionary<string, ISelectableUI> selectionToggleGroups;
    /// <summary>
    /// Clear current list and then get a list of data queried from the database and uses an anonymous function to create a new list 
    /// of items from the data retrieved.
    /// </summary>
    /// <param name="query">Used to return records from Db</param>
    /// <param name="display">The unity game object transform / where the visible UI list items will be put in the heirarchy</param>
    /// <param name="buildItem">Anonymous function selected and used to initialise the list items </param>
    /// <param name="qryParameters">The values of the parameters to be added (a function is used when building the query and on the values in this
    /// function to give the same name.</param>
    public void FillDisplayFromDb(string query,
                                  Transform display,
                                  Func<string [], Transform> buildItem,
                                  //string search = null,
                                  params string[] qryParameterValues) {
        EmptyDisplay(display);
        print(query);
        AppendDisplayFromDb(query,display,buildItem, qryParameterValues);
    }

    /// <summary>
    /// An anonymous function to create a new list from array data generated by a query and append it to a game objects transform.
    /// </summary>
    /// <param name="query">Used to return records from Db</param>
    /// <param name="display">The unity game object transform / where the visible UI list items will be put in the heirarchy</param>
    /// <param name="buildItem">Anonymous function selected and used to initialise the list items </param>
    /// <param name="qryParameters">The values of the parameters to be added (a function is used when building the query and on the values in this
    /// function to give the same name.</param>
    public void AppendDisplayFromDb(string query, Transform display, Func<string[], Transform> buildItem, params string[] qryParameterValues) {

        List<string[]> stringArrayList = new List<string[]>();
        DbCommands.GetDataStringsFromQry(query, out stringArrayList, qryParameterValues);
        Debugging.PrintListOfStrArrays(stringArrayList);
        foreach (string[] stringArray in stringArrayList) {
            Transform item = buildItem(stringArray);
            item.SetParent(display, false);
        }
    }

    public void FillDisplayFromTransform(Transform goHolder,
                                         Transform display,
                                         Func<GameObject, Transform> buildItem) {
        EmptyDisplay(display);
        foreach (Transform gObj in goHolder) {
            Transform item = buildItem(gObj.gameObject);
            item.SetParent(display, false);
        }
    }

    public void EmptyDisplay(Transform display) {
        foreach (Transform item in display) {
            Destroy(item.gameObject);
        }
    }

    public void DeselectIfClickingAnotherListItem(string itemName, GameObject go, Action deselectFunction) {
        /* if another dialogue is selected that is not this dialogue, then this dialogue should be deselected */
        if (Input.GetMouseButtonUp(0)) {
            if (MouseSelection.IsClickedGameObjectName(itemName) && MouseSelection.IsClickedDifferentGameObjectTo(go)) {
                deselectFunction();
            }
        }
    }

    public void ActivateSelf() {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// A delay is used so that when the UI is deactivated the click used to deactivate it won't cause side effect behaviour
    /// e.g. player walking to where the button was clicked.
    /// </summary>
    public void DeactivateSelf() {
        gameObject.SetActive(false);
        MouseSelection.DelayNextClickSelect();
    }

    /// <summary>
    /// All the components desired to be hidden should be stored in a panel on the next level down in the heirarchy.
    /// "Panel" should be used as the GameObject name so that it can always be found when this function is called.
    /// </summary>
    public void DisplayComponents() {
        panel = transform.FindChild("Panel").gameObject;
        panel.SetActive(true);
    }

    /// <summary>
    /// All the components desired to be hidden should be stored in a panel on the next level down in the heirarchy.
    /// "Panel" should be used as the GameObject name so that it can always be found when this function is called.
    /// </summary>
    public void HideComponents() {
        GameObject panel = transform.FindChild("Panel").gameObject;
        panel.SetActive(false);
    }

    public GameObject GetPanel() {
        return transform.FindChild("Panel").gameObject;
    }

    public void CreateNewMenuToggleGroup(string groupName) {
        if (menuToggleGroups == null) {
            menuToggleGroups = new Dictionary<string, UIController>();
        }
        if (!menuToggleGroups.ContainsKey(groupName)) {
            menuToggleGroups.Add(groupName, null);
        }
        else {
            Debug.Log("You tried to create a new menu toggle group which already exists");
        }
    }

    public UIController GetToggledMenuFromGroup(string groupName) {
        return menuToggleGroups[groupName];
    }

    public void ToggleMenuTo(UIController menuToDisplay, string inGroup) {
        if ((GetToggledMenuFromGroup(inGroup) != null)
        && (GetToggledMenuFromGroup(inGroup) != menuToDisplay)) {
            GetToggledMenuFromGroup(inGroup).HideComponents();
            menuToggleGroups[inGroup] = menuToDisplay;
            menuToDisplay.DisplayComponents();
        }
        else {
            menuToggleGroups[inGroup] = menuToDisplay;
            menuToDisplay.DisplayComponents();
        }
    }

    public void CreateSelectionToggleGroup(string groupName) {
        if (selectionToggleGroups == null) {
            selectionToggleGroups = new Dictionary<string, ISelectableUI>();
        }
        if (!selectionToggleGroups.ContainsKey(groupName)) {
            selectionToggleGroups.Add(groupName, null);
        } else {
            Debug.Log(groupName + ": You tried to create a selection toggle group which already exists");
        }
    }

    public ISelectableUI GetSelectedItemFromGroup(string groupName) {
        return selectionToggleGroups[groupName];
    }

    public void ToggleSelectionTo(ISelectableUI selection, string inGroup) {
        Debugging.PrintDictionary(selectionToggleGroups);
        print(GetSelectedItemFromGroup(inGroup));
        if (GetSelectedItemFromGroup(inGroup) != null) {
            if (!GetSelectedItemFromGroup(inGroup).Equals(null)) {
                if (GetSelectedItemFromGroup(inGroup) != selection) {
                    print(GetSelectedItemFromGroup(inGroup));
                    GetSelectedItemFromGroup(inGroup).DeselectSelf();
                    selectionToggleGroups[inGroup] = selection;
                    selection.SelectSelf();
                }
            } else {
                selectionToggleGroups[inGroup] = selection;
                selection.SelectSelf();
            }
        } else {
            selectionToggleGroups[inGroup] = selection;
            selection.SelectSelf();
        }
    }
}
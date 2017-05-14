﻿using UnityEngine;
using System.Collections;
using GameUI;
using GameUtilities.Display;
public class Scenery1 : MonoBehaviour {

    PlayerInventoryUI inventory;
    SpriteRenderer[] mySprites;
    // Use this for initialization
    void Start () {
        mySprites = transform.FindChild("CentreOfGravity").GetComponentsInChildren<SpriteRenderer>();
        SetMyLayer();
    }

    public void SetMyLayer() {
        int layerOrder = ImageLayerOrder.GetOrderInt(gameObject);
        print(mySprites);
        ImageLayerOrder.SetOrderOnSpriteObjectArray(mySprites, layerOrder);
        ImageLayerOrder.SetZ(gameObject);
    }

}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterMovementDecision : CharacterDecision {
    protected Vector2 targetPosition;
    protected CharMovementController movementController;
    public CharMovementController MovementController {
        get { return movementController; }
    }
    protected CharacterMovement movementType;
    public CharacterMovement MovementType {
        get { return movementType; }
    }
	// Update is called once per frame
	void Update () {
        movementController.CheckToMakeMovement();
        CheckToEndMovement();
    }

    void Awake() {
        movementController = GetComponent<CharMovementController>();
    }


    public override void ProcessDecision() {
        movementController.SetMovementDecision(this);
        movementController.ProcessMovement(movementType);
    }

    public override void EndDecision() {
        movementController.StopMoving();
        movementType.StopAction();
        myCharacter.EndSelection();
        if (gameObject != null) {
            Destroy(gameObject);
        }
    }

    public void SetMovementType(bool dblClick) {
        if(dblClick) {
            movementType = new RunMovement(myCharacter.GetMyAnimator(), 3f);
        } else {
            movementType = new WalkMovement(myCharacter.GetMyAnimator(), 1f);
        }
    }

    /// <summary>
    /// If the character reaches the target destination or within interaction distance of the object
    /// they are moving towards, or their status has changed then the movement can end.
    /// </summary>
    public abstract void CheckToEndMovement();

}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeAbilityAction : CharAbilityAction {

    public override void MakeAction() {
        MyAnimator.SetTrigger("Strike");
        Destroy(gameObject);
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataUI {
    namespace ListItems {
        public class DialogueNodeSpeaker : MonoBehaviour {
            DialogueUI dui;
            private string characterName;
            public string CharacterName {
                get { return characterName; }
                set { characterName = value; }
            }
            private string sceneName;
            public string SceneName {
                get { return sceneName; }
                set { sceneName = value; }
            }

            void Start() {
                dui = FindObjectOfType<DialogueUI>();
            }

            public void LinkDialogueToOverride() {
                dui.SetDialogueCharacterOverride(gameObject);
                dui.DeactivateNewCharacterOverride();
            }

        }
    }
}
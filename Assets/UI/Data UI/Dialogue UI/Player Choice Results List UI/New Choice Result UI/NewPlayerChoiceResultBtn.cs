﻿using UnityEngine;
using System.Collections;

namespace DataUI {
    namespace ListItems {

        public class NewPlayerChoiceResultBtn : MonoBehaviour {

            DialogueUI dui;
            private string myID;
            public string MyID {
                get { return myID; }
                set { myID = value; }
            }

            private string myText;
            public string MyText {
                get { return myText; }
                set { myText = value; }
            }

            void Start() {
                dui = FindObjectOfType<DialogueUI>();
            }

            void OnMouseUp() {
                dui.SetSelectedChoiceResultOption(gameObject);
                dui.InsertNewChoiceResult();
            }
        }
    }
}
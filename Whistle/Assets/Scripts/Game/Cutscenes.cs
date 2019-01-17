using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Characters;

namespace Whistle.Cutscenes {

    public delegate IEnumerator Cutscene();

    public class CutsceneControl {
        //These are methods you can use to aid with developing and controlling cutscenes.

        public void End() {
            //USE THIS METHOD AT THE END OF ALL CUTSCENES! It is important as to allow the game to unlock and make room for a new cutscene.
            GameController.cutsceneRunning = false;
        }
    }
}

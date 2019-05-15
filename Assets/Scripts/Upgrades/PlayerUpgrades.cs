using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

public class PlayerUpgrades {
    //Player upgrades consist of passives and actives. This script contains data on all of them for easy access and so the game can easily manage them.

    delegate bool PlayerBehavior(Player player);

    void Apply() {
        PlayerBehavior test = player => {

            return true;
        };
    }
}

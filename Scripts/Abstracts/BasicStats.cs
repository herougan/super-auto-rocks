using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicStats
{
    public int health {get; set;}
    public int mana {get; set;}
    public int attack {get; set;}
    public int magic {get; set;}
    //
    public int dodge {get; set;}   // hit_inc  /=  Max(1 + dodge% - accuracy%, 1) 
    public int accuracy {get; set;}
    public int armour {get; set;}      // dmg  /=  Min(1 + armour% - penetrate%, 1)
    public int magicArmour {get; set;}

    public BasicStats() {
        health = 0;
        mana = 0;
        attack = 0;
        magic = 0;
        dodge = 0;
        accuracy = 0;
        armour = 0;
        magicArmour = 0;
    }

    public BasicStats(BasicStats stats) {
        health = stats.health;
        mana = stats.mana;
        attack = stats.attack;
        magic = stats.magic;
        dodge = stats.dodge;
        accuracy = stats.accuracy;
        armour = stats.armour;
        magicArmour = stats.magicArmour;
    }

    // Growth
    public int healthInc {get; set;}
    public int manaInc {get; set;}
    public int attackInc {get; set;}
    public int magicInc {get; set;}
    public int dodgeInc {get; set;}
    public int accuracyInc {get; set;}
    public int armourInc {get; set;}
    public int magicArmourInc {get; set;}
}
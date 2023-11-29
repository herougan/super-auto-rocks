using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedStats
{
    public const int MAX_VALUE = 99;
    public const int MIN_VALUE = -99;
    
    public int penetration;
    public int attackSpeed;
    public int critRate;
    public int critDamage;
    //
    public int manaEfficiency;
    public int lifesteal;
    public int manaburn;
    //
    public int starpower;

    public AdvancedStats() {
        penetration = 0;
        attackSpeed = 0;
        critRate = 0;
        critDamage = 0;

        manaEfficiency = 0;
        lifesteal = 0;
        manaburn = 0;

        starpower = 0;
    }

    public AdvancedStats(AdvancedStats stats) {
        penetration = stats.penetration;
        attackSpeed = stats.attackSpeed;
        critRate = stats.critRate;
        critDamage = stats.critDamage;

        manaEfficiency = stats.manaEfficiency;
        lifesteal = stats.lifesteal;
        manaburn = stats.manaburn;

        starpower = stats.starpower;
    }
}

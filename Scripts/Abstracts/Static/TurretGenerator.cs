using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TurretGenerator
{
    // Turret definition modified in the function
    
    public static Turret CreateTurret(TurretName name, int index = 0) {
        switch (name) {
            case TurretName.Empty:
                return CreateEmpty();
            case TurretName.Emerald:
                return CreateEmeraldTurret();
            case TurretName.Lapis:
                return CreateLapisTurret();
            case TurretName.Ruby:
                return CreateRubyTurret();
            case TurretName.Sapphire:
                return CreateSapphireTurret();
            case TurretName.Topaz:
                return CreateTopazTurret();
            case TurretName.Amethyst:
                return CreateAmethystTurret();
            case TurretName.Garnet:
                return CreateGarnetTurret();
            case TurretName.Jade:
                return CreateJadeTurret();
            case TurretName.Diamond:
                return CreateDiamondTurret();
            case TurretName.Opal:
                return CreateOpalTurret();
            case TurretName.Rubble:
                return CreateRubble();
                //
            case TurretName.RedBuff:
                return CreateRedBuff();
            case TurretName.BlueBuff:
                return CreateBlueBuff();
            case TurretName.YellowBuff:
                return CreateYellowBuff();
            case TurretName.GreenBuff:
                return CreateGreenBuff();
            case TurretName.PurpleBuff:
                return CreatePurpleBuff();
            case TurretName.RedBuffRubble:
                return CreateRubbleRedBuff();
            case TurretName.BlueBuffRubble:
                return CreateRubbleBlueBuff();
            case TurretName.YellowBuffRubble:
                return CreateRubbleYellowBuff();
            case TurretName.GreenBuffRubble:
                return CreateRubbleGreenBuff();
            case TurretName.PurpleBuffRubble:
                return CreateRubblePurpleBuff();
                //
            case TurretName.Rock:
                return CreateRockTurret();
            default:
                return CreateRubble();
        }
    }
    #region Tier 0

    public static Turret DestroyRubbleTo(TurretName name) {
        switch (name) { 
            default:
            case TurretName.Rubble:
                return CreateEmpty();           
            case TurretName.RedBuffRubble:
                return CreateRedBuff();
            case TurretName.BlueBuffRubble:
                return CreateBlueBuff();
            case TurretName.YellowBuffRubble:
                return CreateYellowBuff();
            case TurretName.GreenBuffRubble:
                return CreateGreenBuff();
            case TurretName.PurpleBuffRubble:
                return CreatePurpleBuff();
            }
    }

    public static Turret CreateEmpty(int index = 0) {
        Turret turret = new Turret(TurretName.Empty);
        turret.index = index;
        return turret;
    }

    public static Turret CreateRubble(int index = 0) {
        Turret turret = new(TurretName.Rubble)
        {
            index = index
        };
        return turret;
    }

    public static Turret CreateRubbleBlueBuff(int index = 0) {
        Turret turret = new Turret(TurretName.BlueBuffRubble);
        turret.index = index;
        return turret;
    }

    public static Turret CreateRubbleRedBuff(int index = 0) {
        Turret turret = new Turret(TurretName.RedBuffRubble);
        turret.index = index;
        return turret;
    }

    public static Turret CreateRubbleYellowBuff(int index = 0) {
        Turret turret = new Turret(TurretName.YellowBuffRubble);
        turret.index = index;
        return turret;
    }

    public static Turret CreateRubbleGreenBuff(int index = 0) {
        Turret turret = new Turret(TurretName.GreenBuffRubble);
        turret.index = index;
        return turret;
    }

    public static Turret CreateRubblePurpleBuff(int index = 0) {
        Turret turret = new Turret(TurretName.PurpleBuffRubble);
        turret.index = index;
        return turret;
    }

    public static Turret CreateBlueBuff(int index = 0) {
        Turret turret = new Turret(TurretName.BlueBuff);
        turret.index = index;
        return turret;
    }

    public static Turret CreateRedBuff(int index = 0) {
        Turret turret = new Turret(TurretName.RedBuff);
        turret.index = index;
        return turret;
    }

    public static Turret CreateYellowBuff(int index = 0) {
        Turret turret = new Turret(TurretName.YellowBuff);
        turret.index = index;
        return turret;
    }

    public static Turret CreateGreenBuff(int index = 0) {
        Turret turret = new Turret(TurretName.GreenBuff);
        turret.index = index;
        return turret;
    }

    public static Turret CreatePurpleBuff(int index = 0) {
        Turret turret = new Turret(TurretName.PurpleBuff);
        turret.index = index;
        return turret;
    }

    #endregion Tier 0

    #region Tier I

    /* Gems */
    public static Turret CreateEmeraldTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Emerald);
        turret.index = index;

        // Stats
        turret.stats.health = 16;
        turret.stats.armour = 0;        
        turret.stats.attack = 0;

        // Moves
        BattleAction normalAttack = new BattleAction
        {
            time = 2500,
            attackCount = 1,
            type = BattleAction.Type.Attack,
        };
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateLapisTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Lapis);
        turret.index = index;

        // Stats
        turret.stats.health = 4;
        turret.stats.armour = 0;        
        turret.stats.attack = 4;

        // Moves
        BattleAction normalAttack = new BattleAction
        {
            time = 1600,
            attackCount = 1,
            type = BattleAction.Type.Attack,
        };
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateRubyTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Ruby);
        turret.index = index;

        // Stats
        turret.stats.health = 6;
        turret.stats.armour = 0;        
        turret.stats.attack = 1;

        // Moves
        BattleAction normalAttack = new BattleAction
        {
            time = 1500,
            attackCount = 3,
            type = BattleAction.Type.Attack,
        };
        normalAttack.cooldown = normalAttack.time;
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateSapphireTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Sapphire);
        turret.index = index;

        // Stats
        turret.stats.health = 1;
        turret.stats.armour = 0;        
        turret.stats.attack = 4;

        // Moves
        BattleAction normalAttack = new BattleAction
        {
            time = 1500,
            attackCount = 1,
            type = BattleAction.Type.Attack,
        };

        return turret;
    }

    public static Turret CreateTopazTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Topaz);
        turret.index = index;

        // Stats
        turret.stats.health = 4;
        turret.stats.armour = 2;        
        turret.stats.attack = 1;

        // Moves
        BattleAction normalAttack = new BattleAction
        {
            time = 1800,
            attackCount = 1,
            type = BattleAction.Type.Attack,
        };

        return turret;
    }

    public static Turret CreateAmethystTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Amethyst);
        turret.index = index;

        // Stats
        turret.stats.health = 4;
        turret.stats.armour = 0;        
        turret.stats.attack = 3;

        // Moves
        BattleAction normalAttack = new BattleAction();
        normalAttack.time = 1000;
        normalAttack.cooldown = normalAttack.time;
        normalAttack.type = BattleAction.Type.Attack;
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateGarnetTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Garnet);
        turret.index = index;

        // Stats
        turret.stats.health = 5;
        turret.stats.armour = 0;        
        turret.stats.attack = 3;

        // Moves
        BattleAction normalAttack = new BattleAction();
        normalAttack.time = 1200;
        normalAttack.cooldown = normalAttack.time;
        normalAttack.type = BattleAction.Type.Attack;
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateJadeTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Jade);
        turret.index = index;

        // Stats
        turret.stats.health = 6;
        turret.stats.armour = 0;        
        turret.stats.attack = 3;

        // Moves
        BattleAction normalAttack = new BattleAction();
        normalAttack.time = 1400;
        normalAttack.cooldown = normalAttack.time;
        normalAttack.type = BattleAction.Type.Attack;
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateDiamondTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Diamond);
        turret.index = index;

        // Stats
        turret.stats.health = 10;
        turret.stats.armour = 2;        
        turret.stats.attack = 5;

        // Moves
        BattleAction normalAttack = new BattleAction();
        normalAttack.time = 3000;   // Displayed as 1000/3000 * 10 = 3.3 attack-count-value (10s)
        normalAttack.cooldown = normalAttack.time;
        normalAttack.type = BattleAction.Type.Attack;
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateOpalTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Opal);
        turret.index = index;

        return turret;
    }

    /* Elements */
    public static Turret CreateRockTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Rock);
        turret.index = index;

        // Stats
        turret.stats.health = 10;
        turret.stats.armour = 0;        
        turret.stats.attack = 3;

        // Moves
        BattleAction normalAttack = new BattleAction();
        normalAttack.time = 1500;
        normalAttack.cooldown = normalAttack.time;
        normalAttack.type = BattleAction.Type.Attack;
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateFireTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Fire);
        turret.index = index;

        return turret;
    }

    public static Turret CreatePoisonTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Poison);
        turret.index = index;

        return turret;
    }

    public static Turret CreateNatureTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Nature);
        turret.index = index;

        return turret;
    }

    public static Turret CreateWaterTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Water);
        turret.index = index;

        return turret;
    }

    public static Turret CreateIceTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Ice);
        turret.index = index;

        return turret;
    }

    public static Turret CreateMetalTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Metal);
        turret.index = index;

        return turret;
    }

    public static Turret CreateWindTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Wind);
        turret.index = index;

        return turret;
    }

    public static Turret CreateLightningTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Lightning);
        turret.index = index;

        return turret;
    }

    public static Turret CreateGoldTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Gold);
        turret.index = index;

        return turret;
    }
    
    public static Turret CreateWoodTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Wood);
        turret.index = index;

        return turret;
    }

    #endregion Tier I

    #region Tier II
        
    /* Basic Combine */
    public static Turret CreateBoneTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Bone);
        turret.index = index;

        // Stats
        turret.stats.health = 16;
        turret.stats.armour = 0;        
        turret.stats.attack = 0;

        // Moves
        BattleAction normalAttack = new BattleAction
        {
            time = 2500,
            attackCount = 1,
            type = BattleAction.Type.Attack,
        };
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateBugTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Bone);
        turret.index = index;

        // Stats
        turret.stats.health = 16;
        turret.stats.armour = 0;        
        turret.stats.attack = 0;

        // Moves
        BattleAction normalAttack = new BattleAction
        {
            time = 2500,
            attackCount = 1,
            type = BattleAction.Type.Attack,
        };
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateCoalTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Bone);
        turret.index = index;

        // Stats
        turret.stats.health = 16;
        turret.stats.armour = 0;        
        turret.stats.attack = 0;

        // Moves
        BattleAction normalAttack = new BattleAction
        {
            time = 2500,
            attackCount = 1,
            type = BattleAction.Type.Attack,
        };
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateMeatTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Meat);
        turret.index = index;

        // Stats
        turret.stats.health = 16;
        turret.stats.armour = 0;        
        turret.stats.attack = 0;

        // Moves
        BattleAction normalAttack = new BattleAction
        {
            time = 2500,
            attackCount = 1,
            type = BattleAction.Type.Attack,
        };
        turret.skills.Add(normalAttack);

        return turret;
    }

    public static Turret CreateSeedTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Meat);
        turret.index = index;

        // Stats
        turret.stats.health = 16;
        turret.stats.armour = 0;        
        turret.stats.attack = 0;

        // Moves
        BattleAction normalAttack = new BattleAction
        {
            time = 2500,
            attackCount = 1,
            type = BattleAction.Type.Attack,
        };
        turret.skills.Add(normalAttack);

        return turret;
    }

    /*  Light/Dark */    
    public static Turret CreateLightTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Light);
        turret.index = index;

        return turret;
    }

    public static Turret CreateDarkTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Dark);
        turret.index = index;

        return turret;
    }

    public static Turret CreateManaTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Mana);
        turret.index = index;

        return turret;
    }

    public static Turret CreateLifeCrystalTurret(int index = 0) {
        Turret turret = new Turret(TurretName.LifeCrystal);
        turret.index = index;

        return turret;
    }


    public static Turret CreateMagicTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Magic);
        turret.index = index;

        return turret;
    }

    public static Turret CreateMachineTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Machine);
        turret.index = index;

        return turret;
    }
    
    public static Turret CreateObsidianTurret(int index = 0) {
        Turret turret = new Turret(TurretName.Obsidian);
        turret.index = index;

        return turret;
    }

    #endregion Tier II

    #region Tier III

    #endregion Tier III

    #region Tier IV
    #endregion Tier IV

    #region Tier V
    #endregion Tier V

    #region Loading

    /*
        If stats are put in some table somewhere, mods can be made etc.

        And it might be cleaner to put stats in a separate file. 
        But at the same time, if turret creation ever gets complex,
        it might get increasingly hard to encode that into a file.
    */
    // public Dictionary<TurretName, string> turretStats = new Dictionary<TurretName, string>() {

    // };
    // void LoadStats() {
        
    // }

    #endregion
}

public enum TurretName {
    Empty,
    Rubble,
    RedBuffRubble,
    BlueBuffRubble,
    YellowBuffRubble,
    GreenBuffRubble,
    PurpleBuffRubble,
    RedBuff,
    BlueBuff,
    YellowBuff,
    GreenBuff,
    PurpleBuff,
    //
    Topaz,
    Sapphire,
    Emerald,
    Lapis,
    Ruby,
    Jade,
    Garnet,
    Amethyst,
    Opal,
    Diamond,
    //
    Fire,
    Gold,
    Ice,
    Lightning,
    Metal,
    Nature,
    Rock,
    Water,
    Wind,
    Wood,
    //
    Light,
    Dark,
    Mana,
    LifeCrystal,
    Magic,
    Meat,
    Bone,
    Seed,
    Bug,
    Coal,
    Poison,
    //
    Machine,
    Obsidian,
    Fruit,
}

/*

Ideas:

 - Ability -
Turret that eats shop items
Turret that gets stronger on spawns, on death
Turret that generates mana
Turret that copies all buffs from nearby turrets
Gilded One (T5): Takes in all remaining Gold OR depends on remaining Gold
Black Gold aka Oil = Gold + Coal
Swarm: E.g. Swarm-Fire (any fire-affinity tower), or Swarm-Fire Tower -> For each Fire tower, gain *x

 - Name -
Other metals: (Some Metals should be in Tier II)
	Platinum
	Silver
	Uranium
		Unobtanium
Other gems: (Some Gems should be in Tier II)
    Quartz
    Aquamarine
    Agate
		Tourmaline
		Corundum
		Tanzanite
	Peridot
	Jade
		Alexandrite
	Zircon
		Spinel
		Moonstone
		Sunstone
		Malachite
	Jasper
	Onyx
		Angelite
		Hematite
	Citrine
		Amazonite
		Serpentine
		Selenite
		Goldstone
	Calcite
		Infinity Gem
Other crystals:
	Salt
	Sugar
	MSG

 - Desc -
Turret border dictates tier. (1-5)
In the future, turrets may be actual cut-out style drawings.

 - Tier V -
Gems - None
Elements - None

Nuke            -   5
Colossus        -   5
Gilded          -   5
Elder           -   5
Divinity        -   5
Abyssal         -   5
Space           -   5
Time            -   5
Trifecta        -   4 or 5
Core            -   4 or 5
Plasma          -   4 or 5
Quantum         -   4 or 5
Constellation   -   4 or 5
Dragon          -   4 or 5
Devourer        -   4 or 5
Portal          -   4 or 5
Mystic          -   3, 4 or 5
Robot           -   3, 4 or 5

Most things should be Tier 2.
Tier 1 Base Set should contain 10+ Turrets at most
168 Pieces for now, including Rubble.

*/
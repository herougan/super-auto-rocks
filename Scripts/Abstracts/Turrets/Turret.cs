using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret
{
    /* Meta Stats */
    // Identity
    public int ID;
    public TurretName name;
    // Info
    public Type type;
    public Rarity rarity;
    public Player player;


    /* Battle Stats */
    public int level;
    public int tier;
    public BasicStats stats;
    public AdvancedStats advStats;
    public StatusStats status;
    public int index;

    /* Stats */

    // count curr max ID

    // but when ID released, add onto "released_ID". always check released_ID b4 assigning new. priority to user's items

    // use object pooling    

    public static int count = 0;
    public bool locked = false;

    /* Skills */
    public List<BattleAction> skills = new List<BattleAction>();
    public List<BattleTrigger> triggers = new List<BattleTrigger>();
    public List<Aura> auras = new List<Aura>();
    public List<Buff> buffs = new List<Buff>();

    /* State */
    public bool mvpLast = false;
    public bool frozenLast = false;
    public bool frozen = false;

    void LevelUp() {
        // ++stats.health;
        // ++stats.attack; // depends
    }

    public enum Type {
        None,
        Healer,
        Attacker,
        AOE,
        Defender,
        Caster,
        Useless,
        Unknown,
        //
        Class,
    }

    /* Battle */
    void GetNextTarget() {

    }

    public Turret(TurretName name, int level = 1, int index = 0) {
        this.name = name;
        this.level = level;
        this.index = index;
        this.tier = 0;
        stats = new BasicStats();
        advStats = new AdvancedStats();
        status = new StatusStats();
        this.ID = ++count;
    }

    public Turret(Turret turret) {
        
        level = turret.level;
        stats = new BasicStats(turret.stats);
        advStats = new AdvancedStats(turret.advStats);
        status = new StatusStats(turret.status);
        index = turret.index;
        tier = turret.tier;

        foreach (BattleAction skill in turret.skills) {
            skills.Add(new BattleAction(skill));
        }
        foreach (BattleTrigger trigger in turret.triggers) {
            triggers.Add(new BattleTrigger(trigger));
        }
        foreach (Aura aura in turret.auras) {
            auras.Add(new Aura(aura));
        }
        foreach (Buff buff in turret.buffs) {
            buffs.Add(new Buff(buff));
        }

        ID = ++count;
        name = turret.name;
        type = turret.type;
        rarity = turret.rarity;
        player = turret.player;
        locked = turret.locked;
    }

    public void OverwriteTurret(Turret turret) {
        
        level = turret.level;
        stats = new BasicStats(turret.stats);
        advStats = new AdvancedStats(turret.advStats);
        status = new StatusStats(turret.status);
        // index = turret.index;
        tier = turret.tier;

        skills.Clear();
        triggers.Clear();
        auras.Clear();
        buffs.Clear();
        foreach (BattleAction skill in turret.skills) {
            skills.Add(new BattleAction(skill));
        }
        foreach (BattleTrigger trigger in turret.triggers) {
            triggers.Add(new BattleTrigger(trigger));
        }
        foreach (Aura aura in turret.auras) {
            auras.Add(new Aura(aura));
        }
        foreach (Buff buff in turret.buffs) {
            buffs.Add(new Buff(buff));
            // TODO unless it is a forever buff aka MVP buff
        }

        name = turret.name;
        type = turret.type;
        rarity = turret.rarity;
        // player stays the same
        // locked stays the same
    }
    public override string ToString() {
        return $"{player.name}'s {name} Turret @{Util.ColRow(index)} [{stats.attack}, {stats.magic}, {stats.health}]";
    }
}
public enum Rarity {
    Godlike,
    Demonic,
    Mythic,
    Legendary,
    Heroic,
    Epic,
    SuperRare,
    Rare,
    Uncommon,
    Common,
    Standard,
    Trash,
}
public enum Element {
    Fire,
    Water,
    Earth,
    Wind,
    //
    Wood,
    Ice,
    Thunder,
    Metal,
    //
    Lava,
    Crystal,
    Poison,
    Corrosion,
    //
    Dark,
    Light,
    Life,
    //
    Space,
    Time,
    Entropy,
    //
    Chaos,
    Fate,
    Destiny,
}

// Optimisation:
// Time-based and Effect-based triggers should be checked separately.
// For now, loop through all triggers.

// Unlike SAP -> no combining except levelling
// BPB -> combining no levelling
// GTD -> combining and levelling 
// Therefore, alternate recipes and alot of variation in the higher tiers
// so like <20 basics, maybe about 10 at the higher rounds, but 100 combinations

// Combining turrets keep ALL their previous buffs including the MVP buff
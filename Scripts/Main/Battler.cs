using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Battler
{
    List<BattleAction> battleActions = new List<BattleAction>();

    #region Init

    void Init() {
        
    }

    #endregion Init

    #region Animation

    bool anim = true;
    AnimationController animator;
    public void HookAnimator(AnimationController animator) {
        this.animator = animator;
    }

    /* Animation Shortcuts */
    public AnimationDetail AttackAnim(Turret source, Turret target, int damage, int attackCount = 1) {
        AnimationDetail anim = new AnimationDetail(AnimationDetail.Type.Attack);
        anim.value = attackCount;
        anim.count = damage;
        //
        anim.source = source;
        anim.target = target;
        anim.resultValue = target.stats.health;
        return anim;
    }

    public AnimationDetail DeathAnim(Turret turret) {
        AnimationDetail anim = new AnimationDetail(AnimationDetail.Type.Death);
        anim.source = turret;
        return anim;
    }

    public AnimationDetail SpawnAnim(Turret turret) {
        // Replace old empty turret sprite
        AnimationDetail anim = new AnimationDetail(AnimationDetail.Type.Spawn);
        anim.source = turret;
        return anim;
    }

    /* Common */
    public AnimationDetail BounceAnim(Turret turret, float strength) {
        AnimationDetail anim = new AnimationDetail(AnimationDetail.Type.Bounce);
        anim.source = turret;
        anim.strength = strength;
        return anim;
    }

    #endregion Animation

    #region Game

        #region Constants

        public static int staggerTime = 100;

        #endregion Constants

        #region State

        /* Flags */
        public bool g_game_end = false;
        public bool g_player_won = false;
        public bool g_player_lost = false;
        public bool g_player_draw = false;
        bool g_turret_died = false;

        List<int> releasedID = new List<int>();

        public enum GameType {
            Normal,
            Weekly,
        }
        GameType gameType;

        /* Game Pieces */
        /* History */
        List<HistoryAction> history = new List<HistoryAction>();

        /* Main Player */
        Player player; // Do not modify turrets from here. They are part of the "player board state"
        Turret[] turrets; // Modify these instead.

        /* Enemy */
        Player enemy; // Do not modify turrets from here. They are part of the "enemy board state"
        Turret[] enemyTurrets; // Modify these instead.

        /* Battle State Variables */
        List<Interval> intervals = new List<Interval>();
        List<BattleAction> actions = new List<BattleAction>();

        #endregion State

        #region Main
        /* Main */
        public void BattleGame(Player player, Player enemy) {

            // Globals
            g_game_end = false;
        
            this.player = player;
            this.enemy = enemy;
            turrets = new Turret[BoardController.BOARD_SIZE];
            enemyTurrets = new Turret[BoardController.BOARD_SIZE];
            for (int i = 0; i < BoardController.BOARD_SIZE; ++i) {
                turrets[i] = new Turret(this.player.turrets[i]);
                enemyTurrets[i] = new Turret(this.enemy.turrets[i]);
            }

            Debug.Log($"{turrets[0].player} vs {enemyTurrets[0].player}");

            // Generate Board
            // playerBoard.GenerateBoard(player);
            // enemyBoard.GenerateBoard(enemy);
            if (anim) animator.QueueAnimation(new AnimationDetail(AnimationDetail.Type.SetBoard, player)); // Player's turrets at the start
            if (anim) animator.QueueAnimation(new AnimationDetail(AnimationDetail.Type.SetEnemyBoard, enemy));

            InitActions();
            SaltCooldowns(); // Start at different time depending on map position
            CheckGameEnd();

            int count = 16384; // 2^14
            while (!g_game_end) {
                RunNextAction();
                if (--count < 0) break;
            }
            Debug.Log($"Game over [{g_game_end}, {g_player_lost}, {g_player_won}, {g_player_draw}]");

            EndBattle();
        }

        public void InitActions() {
            // Init Start of Game effects
            foreach (Turret turret in turrets) {
                foreach (BattleTrigger trigger in turret.triggers) {
                    if (trigger.type == BattleTrigger.Type.StartOfTurn) {
                        TriggerAction(trigger);
                    }
                }
            }
            foreach (Turret turret in enemyTurrets) {
                foreach (BattleTrigger trigger in turret.triggers) {
                    if (trigger.type == BattleTrigger.Type.StartOfTurn) {
                        TriggerAction(trigger);
                    }
                }
            }

            // Init Auras
            foreach (Turret turret in turrets) {
                foreach (Aura aura in turret.auras) {
                    //
                }
            }
            foreach (Turret turret in turrets) {
                foreach (Buff buff in turret.buffs) {
                    //
                }
            }
            foreach (Turret turret in enemyTurrets) {
                foreach (Aura aura in turret.auras) {
                    //
                }
            }
            foreach (Turret turret in enemyTurrets) {
                foreach (Buff buff in turret.buffs) {
                    //
                }
            }

            // Init Actions
            foreach (Turret turret in turrets) {
                foreach (BattleAction skill in turret.skills) {
                    if (skill.cooldown > 0) {
                        RegisterAction(turret, skill);
                    }
                }
            }
            foreach (Turret turret in enemyTurrets) {
                foreach (BattleAction skill in turret.skills) {
                    if (skill.cooldown > 0) {
                        RegisterAction(turret, skill);
                    }
                }
            }

            SortActions();
        }
        
        public void SaltCooldowns() {
            for (int i = 0; i < turrets.Length; ++i) {
                foreach (BattleAction action in actions) {
                    if (action.source == turrets[i]) {
                        action.cooldown += i * staggerTime;
                    }
                }
            }

            for (int i = 0; i < enemyTurrets.Length; ++i) {
                foreach (BattleAction action in actions) {
                    if (action.source == enemyTurrets[i]) {
                        action.cooldown += i * staggerTime;
                    }
                }
            }
        }

        #endregion Main

        #region Scheduling
        /*
            State-based: Look for next closest event. Do until nothing to be done.
            If states change e.g. attack speed increases, that itself is an event
            An event may update other events. But between events, nothing may change.
        */
        public void RunNextAction() {

            bool cooldown_change = false;

            if (actions.Count == 0) {
                Debug.Log("Game End!");
                g_game_end = true; // Somehow no more actions
                g_player_draw = true; // Will default this to draw -> should not happen.
                return;
            }
            BattleAction first = actions.First();

            if (anim) animator.QueueAnimation(new AnimationDetail(AnimationDetail.Type.Wait, first.cooldown));
            RunAction(first);            
            first.time = RecalculateTime(first); // Attack speed and slow effects
            //
            foreach (BattleAction action in actions) {
                if (action == first) continue;
                // deduct timer
                action.cooldown -= first.cooldown;

                // recalculate if needed
                if (cooldown_change) {
                    action.cooldown = RecalculateCooldown(action);
                }
            }
            first.cooldown = first.time;

            SortActions();

            // Run all other actions that are happening "at the same time"
            while (actions.First().cooldown <= 0) {
                BattleAction action = actions.First();
                action.time = RecalculateTime(action); // Attack speed and slow effects
                action.cooldown = action.time;

                RunAction(action);

                SortActions();
            }

            // Apply death effects now
            KillTurrets();
        }

        public void RunAction(BattleAction action) {

            switch (action.type) {
                case BattleAction.Type.Attack:
                
                    // Find target
                    Turret target = GetNextTarget(action.source);
                    if (target == null) break;
                    if (target != action.target) { // If target changed
                        action.target = target;
                        // Recalculate interval
                        action.interval.InitValue(action.source, action.target);
                    }
                    string result = $"{action.source} attacks {action.target}!";

                    // Update interval
                    action.interval.value += 100 + action.target.stats.dodge - action.source.stats.accuracy;
                    // Do attack
                    if (action.interval.value >= 100) {
                        if (action.source.stats.attack > 0) {
                            action.interval.value -= 100;
                            // Attack
                            int damage = Math.Max(1, action.source.stats.attack - action.target.stats.armour);
                            int totalDamage = damage * action.attackCount;
                            action.target.stats.health -= damage;

                            result += $"\n...dealing {totalDamage} damage!"; 
                            if (action.attackCount > 1) result += $" in {action.attackCount} hits!";
                            result += $"\n{action.target} Turret's health dropped to {action.target.stats.health}!";
                            // Anim
                            if (anim) animator.QueueAnimation(AttackAnim(action.source, action.target, damage, action.attackCount));
                            
                            // Check attack triggers
                            foreach (BattleTrigger trigger in action.target.triggers) {
                                if (trigger.type == BattleTrigger.Type.OnAttacked) {
                                    TriggerAction(trigger);
                                }
                            }
                            foreach (BattleTrigger trigger in action.source.triggers) {
                                if (trigger.type == BattleTrigger.Type.OnAttack) {
                                    TriggerAction(trigger);
                                }
                            }
                        }
                    }
                    // Final value should lie between 0 to 100.
                    action.interval.value = Util.Clamp(action.interval.value, 0, 100);
                    Debug.Log(result);

                    break;
                case BattleAction.Type.Buff:

                    break;
                case BattleAction.Type.Skill:

                    break;
            }
        }
        
        public void SortActions() {
            actions.Sort((BattleAction a1, BattleAction a2) => {
                    return a1.cooldown - a2.cooldown;
            });
        }

        public void RegisterAction(Turret turret, BattleAction action) {
            action.source = turret;
            action.target = GetNextTarget(action.source);

            // Adjust time to new stats
            action.time = RecalculateTime(action);
            action.cooldown = action.time;

            actions.Add(action); // need to put "time to occur",
        }

        public void TriggerAction(BattleTrigger trigger) {

            // Periodic actions trigger each other
        }

        public void KillTurret(Turret turret) {
            Debug.Log($"{turret} Turret died!");
            if (anim) animator.QueueAnimation(DeathAnim(turret));
            turret.name = TurretName.Empty;
            turret.stats = new BasicStats();
            turret.advStats = new AdvancedStats();
            turret.status = new StatusStats();

            // Remove all actions related to turret; implicit meaning: if you destroy a turret
            actions.RemoveAll(a => a.source == turret);

            CheckGameEnd();
        }

        public void KillTurrets() {
            foreach (Turret turret in turrets) {
                if (turret.stats.health <= 0 && IsKillable(turret))
                    KillTurret(turret);
            }
            foreach (Turret turret in enemyTurrets) {
                if (turret.stats.health <= 0 && IsKillable(turret))
                    KillTurret(turret);
            }
        }

            #region Fatigue

            #endregion Fatigue

        #endregion Scheduling

        #region Interactions

        public void Attack(Turret source, Turret target) {

        }

        #endregion Interactions

        #region Mathematics
        public int RecalculateTime(BattleAction action) {
            int t = action.time;
            int s = 0;
            switch (action.type) {
                case BattleAction.Type.Attack:
                    s = CalculateAttackSpeed(action.source);
                    t = t * (100 + action.attackSpeed) / (100 + s);
                    action.attackSpeed = s;
                    return t;
                case BattleAction.Type.Buff:
                    return t;
                case BattleAction.Type.Skill:
                    return t;
                default:
                    return t;
            }
        }

        public int CalculateAttackSpeed(Turret turret) {
            return 0;
        }
        
        public int RecalculateCooldown(BattleAction action) {
            int c = action.cooldown;
            int s = 0;
            switch (action.type) {
                case BattleAction.Type.Attack:
                    s = CalculateAttackSpeed(action.source);
                    c = c * (100 + action.attackSpeed) / (100 + s);
                    action.attackSpeed = s;
                    return c;
                case BattleAction.Type.Buff:
                    return c;
                case BattleAction.Type.Skill:
                    return c;
                default:
                    return c;
            }
        }

        public void ChangeOfState() {

        }

        public bool CheckGameEnd() {
            // Check only on every turret loss
            g_player_lost = false;
            g_player_won = false;
            g_player_draw = false;
            g_game_end = false;

            // Player lost
            bool no_more_turrets = true;
            foreach (Turret turret in turrets) {
                if (turret.stats.health > 0 && IsTargetable(turret)) {
                    no_more_turrets = false;
                    break;
                }
            }
            if (no_more_turrets) {
                g_player_lost = true;
                g_game_end = true;
            }

            no_more_turrets = true;
            // Enemy lost
            foreach (Turret turret in enemyTurrets) {
                if (turret.stats.health > 0 && IsTargetable(turret)) {
                    no_more_turrets = false;
                    break;
                }
            }
            if (no_more_turrets) {
                g_player_won = true;
                g_game_end = true;
            }

            if (g_player_won && g_player_lost) {
                g_player_draw = true;
                g_player_lost = false;
                g_player_won = false;
            }

            return g_game_end;
        }

        public static bool IsKillable(Turret turret) {
            if (turret.name == TurretName.Empty)
                return false;
            if (turret.name == TurretName.Rubble)
                return false;
            if (turret.name == TurretName.RedBuff)
                return false;
            if (turret.name == TurretName.BlueBuff)
                return false;
            if (turret.name == TurretName.YellowBuff)
                return false;
            if (turret.name == TurretName.RedBuffRubble)
                return false;
            if (turret.name == TurretName.BlueBuffRubble)
                return false;
            if (turret.name == TurretName.YellowBuffRubble)
                return false;
            return true;
        }

        public static bool IsEmpty(Turret turret) {
            if (turret.name == TurretName.Empty)
                return true;
            if (turret.name == TurretName.PurpleBuff)
                return true;
            if (turret.name == TurretName.GreenBuff)
                return true;
            if (turret.name == TurretName.RedBuff)
                return true;
            if (turret.name == TurretName.BlueBuff)
                return true;
            if (turret.name == TurretName.YellowBuff)
                return true;
            return false;
        }

        public static bool IsTargetable(Turret turret) {
            if (!IsKillable(turret)) return false;
            if (turret.stats.health <= 0) return false;
            return true;
        }

        public static bool IsRubble(Turret turret) {
            if (turret.name == TurretName.Rubble)
                return true;
            if (turret.name == TurretName.RedBuffRubble)
                return true;
            if (turret.name == TurretName.BlueBuffRubble)
                return true;
            if (turret.name == TurretName.YellowBuffRubble)
                return true;
            if (turret.name == TurretName.GreenBuffRubble)
                return true;
            if (turret.name == TurretName.PurpleBuffRubble)
                return true;
            return false;
        }

        public Turret GetNextTarget(Turret source) {
            /*
                When displaying the board, the enemy's board is flipped.
                But in the maths, both boards have the same indicing pattern.
                It is not:

                0 3 6   6 3 0
                1 4 7   7 4 1
                2 5 8   8 5 2
            */

            (int c, int r) = Util.ColRow(source.index);
            // Check this Row
            for (int i = 2; i >= 0; --i) {
                if (IsTargetable(GetTurrets(source)[Util.Index(i, r)])) {
                    return (source.player.type == Player.Type.Player ? enemyTurrets : turrets)[Util.Index(i, r)];
                }
            }
            // Check other rows, starting from the top
            for (int i = r - 1; i >= 0; --i) {
                for (int u = 2; u >= 0; --u) {
                    if (IsTargetable(GetTurrets(source)[Util.Index(u, i)])) {
                        return (source.player.type == Player.Type.Player ? enemyTurrets : turrets)[Util.Index(u, i)];
                    }
                }
            }
            // Check bottom rows
            for (int i = r + 1; i < 3; ++i) {
                for (int u = 2; u >= 0; --u) {
                    if (IsTargetable(GetTurrets(source)[Util.Index(u, i)])) {
                        return (source.player.type == Player.Type.Player ? enemyTurrets : turrets)[Util.Index(u, i)];
                    }
                }
            }

            return null;
        }

        public Turret DestroyTurret(Turret turret) {
            turret.name = TurretName.Empty;
            turret.stats.health = 0;
            if (anim) animator.QueueAnimation(DeathAnim(turret));
            
            return turret;
        }

        #endregion Mathematics
    
        #region Board
        /* Board */
        public static Turret[] GetStartingTurrets(GameType gameType = GameType.Normal) {
            switch (gameType) {
                default:
                    break;
            }
            Turret[] startingTurrets = new Turret[BoardController.BOARD_SIZE];
            /* 
                0 3 6
                1 4 7
                2 5 8
            */
            // Ordinal
            startingTurrets[0] = TurretGenerator.CreateRubble(0);
            startingTurrets[1] = TurretGenerator.CreateRubble(1);
            startingTurrets[2] = TurretGenerator.CreateRubbleBlueBuff(2);    // Blue
            //
            startingTurrets[3] = TurretGenerator.CreateRubbleRedBuff(3);     // Red
            startingTurrets[4] = TurretGenerator.CreateEmpty(4);             // Middle
            startingTurrets[5] = TurretGenerator.CreateRubble(5);
            //
            startingTurrets[6] = TurretGenerator.CreateRubble(6);
            startingTurrets[7] = TurretGenerator.CreateRubble(7);
            startingTurrets[8] = TurretGenerator.CreateRubbleYellowBuff(8);  // Yellow

            return startingTurrets;
        }

        public Turret[] GetTurrets(Player player) {
            return (player.type == Player.Type.Player ? enemyTurrets : turrets);
        }
        public Turret[] GetTurrets(Turret source) {
            return (source.player.type == Player.Type.Player ? enemyTurrets : turrets);
        }

        public static Turret[] GetStartingWeeklyTurrets() {
            Turret[] startingTurrets = new Turret[BoardController.BOARD_SIZE];
            /* 
                0 3 6
                1 4 7
                2 5 8
            */
            // Ordinal
            startingTurrets[0] = TurretGenerator.CreateRubble(0);
            startingTurrets[1] = TurretGenerator.CreateRubblePurpleBuff(1);
            startingTurrets[2] = TurretGenerator.CreateRubbleBlueBuff(2);    // Blue
            //
            startingTurrets[3] = TurretGenerator.CreateRubbleRedBuff(3);     // Red
            startingTurrets[4] = TurretGenerator.CreateRubble(4);             // Middle
            startingTurrets[5] = TurretGenerator.CreateRubbleGreenBuff(5);
            //
            startingTurrets[6] = TurretGenerator.CreateRubble(6);
            startingTurrets[7] = TurretGenerator.CreateRubble(7);
            startingTurrets[8] = TurretGenerator.CreateRubbleYellowBuff(8);  // Yellow

            return startingTurrets;
        }

        #endregion Board

        #region End
        /* End */
        public void EndBattle() {
            if (anim) animator.QueueAnimation(new AnimationDetail(AnimationDetail.Type.EndGame));
            if (anim) animator.QueueAnimation(new AnimationDetail(AnimationDetail.Type.ClearBoard));
        }
        #endregion End

    #endregion Game

    #region History

    public class HistoryAction {
        public BattleAction action;
        public int timestamp;
        public BattleState currentState;
    }

    #endregion History
}

#region Enum, Classes

/*
    Design Goal: Make the game as simple as we can, while having some depth. Do not have too many statuses, buffs, and complex combos.
*/

public class Interval {
    // This represents an ongoing relationship between two entities
    // e.g. A missed B or A poisoned B
    // Miss-factor is calculated over time, so effectively 50% miss translates to 50% missed hits, but yet it is completely predictable

    public int value;
    public BattleAction.Type type;
    public Turret source;
    public Turret target;
    //
    public int count; // Number of times triggered
    public int cooldown;

    public void InitValue(Turret source, Turret target) {
        switch (type) {
            case BattleAction.Type.Attack:
                value = Util.Clamp(value / 2 + (100 + source.stats.accuracy - target.stats.dodge) / 2, 0, 100);
                break;
            case BattleAction.Type.Buff:
                value = Util.Clamp(value / 2 + (100 + source.stats.accuracy - target.stats.dodge) / 2, 0, 100);
                break;
            case BattleAction.Type.Skill:
                value = Util.Clamp(value / 2 + (100 + source.stats.accuracy - target.stats.dodge) / 2, 0, 100);
                break;
            default:
                break;
        }
    }   
    
    public void Attack() {
        
    }

    public void Apply() {

    }
}

public class BattleAction {
    // Every interaction in Battle is recorded here
    // Time in milliseconds
    public int cooldown;
    public int time; // Original cooldown time
    public BattlePriority priority;
    public Turret source;
    public Turret target;
    // Type
    public Type type;
    public int attackSpeed; // 5 means 105/100 // Starting attackSpeed, affected by turret's stats
    public int attackCount;

    // Thing to do
    public Buff buff;
    public Aura aura; // Trigger on or off
    public Debuff debuff;
    public Curse curse;

    // Interval
    public Interval interval;

    public enum Type {
        Attack,
        Skill,
        Buff,
    }

    public BattleAction() {
        cooldown = 0;
        time = 0;
        priority = BattlePriority.Main;
        type = Type.Attack;
        attackSpeed = 0;
        attackCount = 1;
        interval = new Interval();
    }

    public BattleAction(BattleAction action) {
        cooldown = action.cooldown;
        time = action.time;
        priority = action.priority;
        // Only for out-battle abilities
        if (action.source != null) source = action.source;
        if (action.target != null) target = action.target; // Points to the same one without pointing to the Battle ones
        type = action.type;
        attackSpeed = action.attackSpeed;        
        attackCount = action.attackCount;
        if (action.buff != null) buff = action.buff;
        if (action.aura != null) aura = action.aura;
        if (action.debuff != null) debuff = action.debuff;
        if (action.curse != null) curse = action.curse;
        if (action.interval != null) interval = action.interval;
        else interval = new Interval();
    }
}  

public class Buff {
    public bool trait; // Does it move to the next generation
    public bool unique; // Can it stack
    public Type type;
    public enum Type {

    }

    public Buff() {

    }

    public Buff(Buff buff) {
        trait = buff.trait;
        unique = buff.unique;
    }
}

public class Aura {
    public bool on;
    public Type type;
    public enum Type {

    }
    
    public Aura() {

    }

    public Aura(Aura aura) {
        on = aura.on;
        type = aura.type;
    }
}

public class Status {
    public enum Type {
    }
}

public class Debuff {
    public enum Type {
    }
}

public class Curse {
    // Moves to nearest ally unit on death
    public enum Type {
    }
}

public class BattleTrigger {

    public List<int> triggerParams = new List<int>();
    public BattleAction action;
    public BattlePriority priority;
    public Type type;

    public enum Type {
        StartOfTurn,
        //
        OnAttacked,
        OnAttack,
        //
        OnMainEffectTrigger,
    }

    public BattleTrigger() {

    }

    public BattleTrigger(BattleTrigger trigger) {
        triggerParams = new List<int>(trigger.triggerParams);
        action = new BattleAction(action);
        priority = trigger.priority;
        type = trigger.type;
    }
}

public class BattleResult {
    public enum Type {

    }
}

public class BattleState {
    public enum Type {

    }
}

public enum BattlePriority {
    Main,
    Sub,
    FromEffect,
    FromItem, // etc.
}

public class BattleAnimations {
    public enum Type {

    }
}

#endregion Enum, Classes
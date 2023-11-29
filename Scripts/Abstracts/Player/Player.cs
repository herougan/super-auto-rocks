using System;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    #region Init
    /* Variables */
    public Guid id {get; private set;}
    public string name {get; private set;}
    public Turret[] turrets {get; private set;} // Turrets to spawn in battle
    public int gold;
    public int electrum;
    public int round;
    public int life;
    public Type type;
    public int bouldersDestroyed = 0;
    public int length; // Number of shop items or Neutral items

    //
    static int count;

	// Combinations
	public List<Recipe> recipes = new List<Recipe>();
	public List<Recipe> lockedRecipes = new List<Recipe>();

    public Player(string name = "") {
        if (name == "") this.name = $"player-{count}";
        this.id = Guid.NewGuid();
        this.name = name;
        this.turrets = new Turret[BoardController.BOARD_SIZE];

        gold = 0; electrum = 0;
        round = 0;
        life = 0;
        ++count;
    }

    public Player(Guid id, string name) {
        this.id = id;
        this.name = name;
        this.turrets = new Turret[BoardController.BOARD_SIZE];

        gold = 0; electrum = 0;
        round = 1;
        ++count;
    }

    public Player(Player player) {
        Debug.LogError("Error! Not implemented");
    }

    public static Player InitEmpty() {
        Player empty = new Player();
        empty.type = Player.Type.Neutral;
        for (int i = 0; i < empty.turrets.Length; ++i) empty.turrets[i] = TurretGenerator.CreateRubble();
        return empty;
    }

    #endregion Init

    #region Set
    public void SetName(string name) {
        this.name = name;
    }

    public void SetTurrets(Turret[] turrets) {
        this.turrets = turrets;
        foreach (Turret turret in this.turrets) {
            turret.player = this;
        }
    }

    public override string ToString()
    {
        if (type == Type.Shop) return $"Shop ({length})";
        else
        return $"{name} ({type}); [{gold}g, {round}r, {life}hp, {CountTurrets()}n]";
    }

    public int CountTurrets() {
        int c = 0;
        foreach (Turret turret in turrets) {
            if (Battler.IsTargetable(turret)) ++c;
        }
        return c;
    }

    public void SetRecipes((List<Recipe>, List<Recipe>) allRecipes) {
        recipes = allRecipes.Item1;
        lockedRecipes = allRecipes.Item2;
    }

    #endregion Set
    
    #region State
    public enum Type {
        Player,
        Enemy,
        Shop,
        Neutral,
    }
    
    #endregion State
}

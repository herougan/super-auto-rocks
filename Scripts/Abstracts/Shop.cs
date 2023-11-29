using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Shop
{
	#region Variables, Constants
	/* Constants */
	public const int BASIC_INCOME = 10;
	public Player shopPlayer = new Player("Shop") {
		life = 999,
		type = Player.Type.Shop,
		length = 3,
	};

    /* Stats */
    // Note: T1 T2 T3 refers to the tier 1 basic turret groups 1, 2, and 3. Tier 2 and above turrets can only be gotten through combinations
    List<List<int>> Tier_Mix = new List<List<int>>() {
        new List<int>(){   1,   0,  0,}, // 1
        new List<int>(){   1,   0,  0,}, // 2
        new List<int>(){   2,   1,  0,}, // 3 (Round 5)
        new List<int>(){   2,   1,  0,}, // 4 // 15.5%, 13.3%
        new List<int>(){   3,   2,  1,}, // 5 (Round 9)
        new List<int>(){   3,   2,  1,}, // 6
        new List<int>(){   3,   2,  1,}, // 7 // 10%, 11.1%, 8.3%
    };
    List<int> T1_Mix = new List<int>() {    1,  1,  1,  1,  1,};
    List<int> T2_Mix = new List<int>() {    1,  1,  1,};
    List<int> T3_Mix = new List<int>() {    1,  1,};

    List<TurretName> T1_Names = new List<TurretName>() {	TurretName.Emerald,	TurretName.Lapis,	TurretName.Ruby,	TurretName.Sapphire,	TurretName.Topaz};
    List<TurretName> T2_Names = new List<TurretName>() {	TurretName.Amethyst,TurretName.Garnet,	TurretName.Jade,	};
    List<TurretName> T3_Names = new List<TurretName>() {	TurretName.Diamond,	TurretName.Opal,	};

	List<List<int>> TierLevelMix = new List<List<int>>() {
		new List<int>() {	0,	1,	}, // Dummy, rounds always start at 1.
					//		0	1	2	3	4	5
		new List<int>() {	0,	100,	},					// 1
		new List<int>() {	0,	80,	20},					// 2
		new List<int>() {	0,	60,	30,	10,	},				// 3
		new List<int>() {	0,	40,	32,	20,	8,	},			// 4
		new List<int>() {	0,	20,	33,	30,	12,	5,},		// 5
		new List<int>() {	0,	10,	25,	35,	20,	10,},		// 6
		new List<int>() {	0,	5,	20,	30,	30,	15,},		// 7
		new List<int>() {	0,	1,	10,	25,	40,	25,},		// 8
	};
	
	List<int> TierRounds = new List<int>() {
	//	1,	2,	3,	4,	5,	6	7
		1,	3,	5,	7,	9,	12,	20,
	};
	Dictionary<int, int> TierIncome = new Dictionary<int, int>() {
		[0] = 0,
		[1] = 10,
		[2] = 11,
		[3] = 12,
		[4] = 13,
		[5] = 15,
		[6] = 17,
		[7] = 20,
	};	
	Dictionary<int, int> TierSlots = new Dictionary<int, int>() {
		[0] = 0,
		[1] = 3,
		[2] = 4,
		[3] = 4,
		[4] = 5,
		[5] = 5,
		[6] = 6,
		[7] = 7,
	};

	/* Costs */
	public const int ROLL_COST = 1;
	public const int BASE_TURRET_COST = 5;
	public const int TURRET_COST_PER_LEVEL = 2;
	public const int BASE_BOULDER_COST = 5;
	public const int BASE_FREEZE_COST = 1;

	//
	public const int BASE_SELL_COST = 2;
	public const int SELL_COST_PER_LEVEL = 3;

	/* Current State */
	public List<Turret> shopTurrets {get; private set;}
	public List<int> frozen = new List<int>();
	public string message;
	public bool g_error = false;
	public bool g_success = false;
	public int lastCost = 0;
	public int lastGain = 0;

	#endregion Variables, Constants

	#region Shop Actions
    /* Shop Actions */
    public void Buy(int shopIndex, Player player, int boardIndex) {
		// Get Cost
		lastCost = GetBuyCost(player, shopIndex);
		if (player.gold < lastCost) {
			g_success = false;
			return;
		}
		// Deduct
		g_success = true;
		player.gold -= lastCost;
		// Update
		// if (Battler.IsEmpty(player.turrets[boardIndex])) {}
		// if (player.turrets[boardIndex].name == TurretName.YellowBuff) {}
		// if (player.turrets[boardIndex].name == TurretName.RedBuff) {}
		// if (player.turrets[boardIndex].name == TurretName.BlueBuff) {}
		// if (player.turrets[boardIndex].name == TurretName.GreenBuff) {}
		// if (player.turrets[boardIndex].name == TurretName.PurpleBuff) {}

		// Unfreeze
		shopTurrets[shopIndex].frozenLast = false;
		shopTurrets[shopIndex].frozen = false;

		// Overwrite
		player.turrets[boardIndex].OverwriteTurret(TurretGenerator.CreateTurret(shopTurrets[shopIndex].name));
		Debug.Log($"Bought {player.turrets[boardIndex]}");
		player.turrets[boardIndex].player = player;
        player.SetRecipes(Combiner.GetAllPossibleCombinations(player)); // Update recipes
    }

    public void Sell(Player player, int index) {
		int value = GetPrice(player, index);
		player.gold += value;
		lastGain = value;

		player.turrets[index].OverwriteTurret(TurretGenerator.CreateEmpty());
        player.SetRecipes(Combiner.GetAllPossibleCombinations(player)); // Update recipes
    }

	public void Combine(Player player, Recipe recipe, List<int> indices) {

        player.SetRecipes(Combiner.GetAllPossibleCombinations(player)); // Update recipes
	}

	public void Fuse(Player player, List<int> indices) {
		
        player.SetRecipes(Combiner.GetAllPossibleCombinations(player)); // Update recipes
	}

	public void Freeze(Player player, int shopIndex) {
		int lastCost = GetFreezeCost(player, shopIndex);
		if (player.gold < lastCost) {
			g_success = false;
			return;
		}

		// Deduct
		g_success = true;
		player.gold -= lastCost;

		shopTurrets[shopIndex].frozen = true;
	}

	public void Unfreeze(Player player, int shopIndex) {
		if (shopTurrets[shopIndex].frozen) {

			shopTurrets[shopIndex].frozen = false;

			if (!shopTurrets[shopIndex].frozenLast) {
				player.gold += GetFreezeCost(player, shopIndex);
				shopTurrets[shopIndex].frozenLast = false;
			}
			g_success = true;
		} else {
			g_success = false;
			g_error = true;
		}
	}

	public void DestroyBoulder(Player player, int index) {
		lastCost = GetBoulderDestroyCost(player);

		if (player.gold < lastCost) {
			g_success = false;
			return;
		}

		// Deduct
		g_success = true;
		player.gold -= lastCost;
		++player.bouldersDestroyed;

		player.turrets[index].OverwriteTurret(TurretGenerator.DestroyRubbleTo(player.turrets[index].name));
		
	}

    public List<Turret> Reroll (Player player) {
		lastCost = ROLL_COST;
		if (player.gold < lastCost) {
			g_error = true;
			g_success = false;
			return new List<Turret>() {};
		}
		// Deduct
		g_success = true;
		player.gold -= lastCost;
		return Roll(player);
    }

    public List<Turret> Roll (Player player) {
		g_success = false;
		shopTurrets.Clear();
		int tier = GetTier(player);
		int slots = GetSlots(player);

		UpdateFreeze();

		for (int i = 0; i < slots; ++i) {
			int currTurretTier = Util.RandomWeightedIndex(Tier_Mix[tier]);
			Turret t;

			switch (currTurretTier) {
				default:
				case 0:
					t = TurretGenerator.CreateTurret(T1_Names[Util.RandomWeightedIndex(T1_Mix)]);
					break;
				case 1:
					t = TurretGenerator.CreateTurret(T2_Names[Util.RandomWeightedIndex(T2_Mix)]);
					break;
				case 2:
					t = TurretGenerator.CreateTurret(T3_Names[Util.RandomWeightedIndex(T3_Mix)]);
					break;
			}

			t.level = Util.RandomWeightedIndex(TierLevelMix[tier]);
			t.player = shopPlayer;
			t.index = i;
			shopTurrets.Add(t);
		}

		return shopTurrets;
    }

    /* Shop Effects */
    public void ApplyStartOfTurnEffects() {

    }

    public void ApplyAuraEffects() {
        
    }

    public enum Type {
        Buy,
        Sell,
        Reroll,
        Combine,
        ClassBuy,
        DestroyRubble,
        //
        RollFuse,               // E2 + E2 + E2 = E3 in the bag automatically or manually
        RevertRollFuse,         // Revert back
        //
        EatPiece,               // Consumer turret can eat pieces in your shop
    }
	
	public void ApplyEndShop() {
		UpdateFreeze();
	}

	public void ApplyStartShop() {

	}

	public void UpdateFreeze() {
		foreach (Turret turret in shopTurrets) {
			if (turret.frozen) turret.frozenLast = true;
			else turret.frozenLast = false;
		}
	}

	#endregion Shop Actions

	#region Mathematics

	int GetTier(Player player) {
		int tier = 0;
		for (int i = 0; i < TierRounds.Count; ++i) {
			if (player.round < TierRounds[i]) break;
			++tier;
		}
		return tier;
	}

	int GetSlots(Player player) {
		shopPlayer.length = TierSlots[player.round];
		return TierSlots[player.round];
	}

	int GetRollCost(Player player) {
		int cost = ROLL_COST;
		foreach (Turret t in player.turrets) {

		}
		return cost;
	}

	int GetBuyCost(Player player, int shopIndex) {
		int cost = 0;
		foreach (Turret t in player.turrets) {
			// Search for discounts
		}
		
		// 5, 8, 13, 20, 34
		cost += BASE_TURRET_COST + /* shopTurrets[shopIndex].tier * */ shopTurrets[shopIndex].level * shopTurrets[shopIndex].level - 1;

		cost = Util.Clamp(cost, 0, 99);
		return cost;
	}	

	int GetBoulderDestroyCost(Player player) {
		int cost = BASE_BOULDER_COST;

		cost += player.bouldersDestroyed;

		return cost;
	}

    public int GetPrice(Player player, int index) {
		int value = 0;
		foreach (Turret _turret in player.turrets) {
			// might change turret cost
			// might trigger something
		}

		value = BASE_SELL_COST + SELL_COST_PER_LEVEL * (player.turrets[index].level - 1);

		return value;
	}

	public int GetFreezeCost(Player player, int shopIndex) {
		int cost = BASE_FREEZE_COST;
		return cost;
	}

	public void Income(Player player) {
		foreach (Turret turret in player.turrets) {
			// If gold generating
		}

		player.gold += BASIC_INCOME +	TierIncome[GetTier(player)];
	}

	#endregion Mathematics

    #region Shop Listing

    static int[] t1_1 = new int[]   {10, 10, 10, 10, 10,}; // 59%
    static int[] t1_2 = new int[]   {10, 10, 10,}; //  29%
    static int[] t1_3 = new int[]   {10, 10,}; // 11%
    static int[] t1_dist = new int[] {10, 5, 2,};


    #endregion Shop Listing

	#region Init

	public Shop() {
		shopTurrets = new List<Turret>();
	}

	#endregion
}

public class ShopBoardTrigger {
    // Triggers happen outside of battle
    public List<int> triggerParams = new List<int>();
    public ShopBoardAction action;
    public Type type;

    public enum Type {
        OnBuy,
        OnSell,
        OnCombine,
        OnEnd,
    }
}

public class ShopBoardAction {

}

public class ShopItem {
    int idx;
    TurretName name;
    int level;
}

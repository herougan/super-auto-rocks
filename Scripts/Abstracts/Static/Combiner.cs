using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

/*
3 kinds of combining

1) Same + Same = LvL++ or (S+S+S in hand = LvL++, S+S+S+S in hand = LvL+=2, ...)    -> Fusing
2) S-1 + S-2 + S-3 = S-Special-1                                                    ->
3) A-1 + B-2 + C-3 = D-1                                                            -> Combining

The turrets' levels have to be exactly correct to combine. E.g. S1+S2+S1 makes nothing but S1+S1+S1 makes 
Advanced (Tier II and beyond) towers can combine too!

The recipe defined in the static list is the "Base". If you have the materials for the recipe but
they are all exactly one level higher, you may fuse into a higher levelled version of the tower.
Towers get almost 2x stronger at each level.
1) Buff is improved by one grade (stackable with different levels but improves linearly)
2) Base stats are doubled

General rules, for simplicity:

1) Each recipe requires 3 turrets exactly.
2) Combinations are strongly thematic, so that the player doesn't have to remember every single detail
3) Non thematic esp strange effects like copying are reserved for the advanced gems e.g. copying, stealth, and stealing would lie on Opal, let's say. And it's intuitive

*/

public class Combinant {
	public TurretName name;
	public int quantity = 1;
	public int level = 1;
}
public class Recipe {
	public TurretName result;
	public int level = 1;
	public List<Combinant> combinants = new List<Combinant>();

	public Recipe(TurretName result) {
		this.result = result;
	}
	public bool Includes(Turret turret) {
		foreach (Combinant combinant in combinants) {
			if (combinant.name == turret.name && combinant.level == turret.level) return true;
		}
		return false;
	}

	public List<TurretName> GetCombinantNames() {
		List<TurretName> names = new List<TurretName>();
		foreach (Combinant combinant in combinants) {
			names.Add(combinant.name);
		}
		return names;
	}

	public static Recipe UpRecipe(Recipe recipe) {
		List<Combinant> combinants = new List<Combinant>(recipe.combinants);
		foreach (Combinant combinant in combinants) {
			++combinant.level;
		}
		return new Recipe(recipe.result) {
			combinants = combinants,
			level = recipe.level + 1,
		};
	}

	public static Recipe UpRecipe(Recipe recipe, int level) {
		List<Combinant> combinants = new List<Combinant>(recipe.combinants);
		foreach (Combinant combinant in combinants) {
			combinant.level = level;
		}
		return new Recipe(recipe.result) {
			combinants = combinants,
			level = level,
		};
	}

	public override string ToString()
	{
		return $"{result} (LvL - {level})";
	}
}

public static class Combiner
{
	public static readonly List<Recipe> recipes = new List<Recipe>() {
		#region Tier II
		new Recipe(TurretName.Bone) {
			combinants = new List<Combinant>() {
				new Combinant() {   name = TurretName.Ruby },
				new Combinant() {   name = TurretName.Topaz },
				new Combinant() {   name = TurretName.Sapphire },
			},
		},
		new Recipe(TurretName.Bug) {
			combinants = new List<Combinant>() {
				new Combinant() {   name = TurretName.Topaz },
				new Combinant() {   name = TurretName.Sapphire },
				new Combinant() {   name = TurretName.Lapis },
			},
		},
		new Recipe(TurretName.Coal) {
			combinants = new List<Combinant>() {
				new Combinant() {   name = TurretName.Ruby },
				new Combinant() {   name = TurretName.Topaz },
				new Combinant() {   name = TurretName.Emerald },
			},
		},
		new Recipe(TurretName.Meat) {
			combinants = new List<Combinant>() {
				new Combinant() {   name = TurretName.Ruby },
				new Combinant() {   name = TurretName.Emerald },
				new Combinant() {   name = TurretName.Lapis },
			},
		},
		new Recipe(TurretName.Seed) {
			combinants = new List<Combinant>() {
				new Combinant() {   name = TurretName.Sapphire },
				new Combinant() {   name = TurretName.Emerald },
				new Combinant() {   name = TurretName.Lapis },
			},
		},
		#endregion Tier II
		#region Tier III
		#endregion Tier III
		#region Tier IV
		#endregion Tier IV
		#region Tier V
		#endregion Tier V
	};
	public static (List<Recipe>, List<Recipe>) GetPossibleCombinations(Player player, Turret turret, List<Recipe> possibleCombinations, List<Recipe> lockedCombinations) {
		List<Recipe> indivComb = new List<Recipe>();
		List<Recipe> lockedIndivComb = new List<Recipe>();
		if (turret.locked) return (indivComb, lockedIndivComb); // Locked turrets have no recipes

		// Check if turret contained in combinations
		foreach (Recipe recipe in possibleCombinations) {
			if (recipe.Includes(turret)) indivComb.Add(recipe);
		}
		foreach (Recipe recipe in lockedCombinations) {
			if (recipe.Includes(turret)) lockedIndivComb.Add(recipe);
		}

		return (indivComb, lockedIndivComb);
	}
	public static (List<Recipe>, List<Recipe>) GetAllPossibleCombinations(Player player) {
		List<Recipe> possibleCombinations = new List<Recipe>();
		List<Recipe> lockedCombinations = new List<Recipe>();

		foreach (Recipe recipe in recipes) {
	
			Dictionary<int, bool> possible = new Dictionary<int, bool>();
			Dictionary<int, bool> lockPossible = new Dictionary<int, bool>();
			
			Debug.Log(recipe.result);
			Dictionary<int, (List<TurretName>, List<int>, List<int>)> requirements = new Dictionary<int, (List<TurretName>, List<int>, List<int>)>(); // Store if requirements are met for ANY level-variant
			// Count turrets           
			foreach (Combinant combinant in recipe.combinants) {
				foreach (Turret turret in player.turrets) {
					if (turret.name == combinant.name) {

						int level = turret.level - combinant.level + 1;
						if (level > 0) {
							if (!requirements.ContainsKey(level)) {
								requirements[level] = (recipe.GetCombinantNames(), new List<int>(new int[recipe.combinants.Count]), new List<int>(new int[recipe.combinants.Count]));
							}
							for (int i = 0; i < requirements[level].Item1.Count; ++i) {
								if (turret.name == requirements[level].Item1[i]) {
									requirements[level].Item2[i] += turret.locked ? 0 : 1;
									requirements[level].Item3[i] += 1;
								}
							}
						}
					}
				}
			}

			// Check requirements
			foreach (var kvp in requirements) {
				Debug.Log($"=== LvL {kvp.Key} ===");
				Util.PrintTurretNameList(kvp.Value.Item1);
				Util.PrintIntList(kvp.Value.Item2);
				foreach (Combinant combinant in recipe.combinants) {
					Debug.Log($"Requires: {combinant.quantity} {combinant.name}");
					// Set as default true
					if (!possible.ContainsKey(kvp.Key)) possible[kvp.Key] = true;
					if (!lockPossible.ContainsKey(kvp.Key)) lockPossible[kvp.Key] = true;

					// Check if this combinant requirement hit
					for (int i = 0; i < kvp.Value.Item1.Count; ++i) {
						if (combinant.name == kvp.Value.Item1[i]) {
							if (kvp.Value.Item2[i] < combinant.quantity) { // Unlocked
								possible[kvp.Key] = false;
								lockPossible[kvp.Key] = false;
								Debug.Log("Failed");
							} else if (kvp.Value.Item3[i] < combinant.quantity) { // Locked
								lockPossible[kvp.Key] = false;
								Debug.Log("Lock Failed");
							}
						}
					}
				}
			}

			// Check if requirements met
			foreach (var kvp in possible) {
				if (kvp.Value) {
					if (kvp.Key > 1) {
						possibleCombinations.Add(Recipe.UpRecipe(recipe, kvp.Key));
						if (lockPossible.ContainsKey(kvp.Key)) lockPossible[kvp.Key] = false;
					}
					else {
						possibleCombinations.Add(recipe);
						if (lockPossible.ContainsKey(kvp.Key)) lockPossible[kvp.Key] = false;
					}
				}
			}
			foreach (var kvp in lockPossible) {
				if (kvp.Value) {
					if (kvp.Key > 1)
						lockedCombinations.Add(Recipe.UpRecipe(recipe, kvp.Key));
					else
						lockedCombinations.Add(recipe);
				}
			}
		}

		// Remove duplicates
		// lockedCombinations.RemoveAll((recipe) => possibleCombinations.Contains(recipe)); // Just in case

		return (possibleCombinations, lockedCombinations);
	}
	public static List<Recipe> GetFusionCombinations(Player player) {
		List<Recipe> turrets = new List<Recipe>();

		foreach (Turret turret in player.turrets) {

		}

		return turrets;
	}
	public static List<Recipe> GetShopFusionCombinations(Shop shop) {
		List<Recipe> turrets = new List<Recipe>();

		foreach (Turret turret in shop.shopTurrets) {

		}

		return turrets;
	}
}

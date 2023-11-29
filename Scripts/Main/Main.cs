using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;

/* Battle__Main */
public class Main : MonoBehaviour
{
    #region Constants

    public const string BASE_SPRITE = "slot_base";

    #endregion

    #region Player
    Player player;
    Shop shop;
    Battler battler;
    
    #endregion Player

    #region Init

    bool playerLoaded = false;    
    public AnimationController animator;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init() {
        // Load General
        InitUIElements();
        InitGameObjects();
        InitPlayerObjects();

        // Load Player
        player = GenerateTestPlayer();
        SetupPlayer(player);
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetBoard, player));
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.InitPlayerStatsBar, player));
        playerLoaded = true;

        // Load Shop
        shop = new Shop();
        shop.Roll(player);
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetShop, shop));

        // Load Battler
        battler = new Battler();
        battler.HookAnimator(animator);
    }

    #endregion Init

    #region Game

    void SetupPlayer(Player player) {
        player.gold = 10;
        player.life = 5;
        player.round = 1;
    }

    #endregion Game

    #region Anim

        #region Fx

        #endregion

    #endregion Anim
    
    #region UI

        #region Elements
        public int _____UI_____; // For delineation in the Editor
        public GameObject uiPlayButton;
        public GameObject uiNextBattleButton;
        public GameObject uiQuitButton;
        public GameObject uiResultText;
        public TurretContextController turretContextController;
        public CombinerContextController combinerContextController;

        public void InitUIElements() {

        }

        #endregion Elements

    /* Variables */
    public bool contextActionInTransit = false;
    
    /* Buttons */
    public void UI__NextFight() {
        if (!playerLoaded) return;

        uiQuitButton.SetActive(false);
        uiNextBattleButton.SetActive(false);

        // Fetch enemy
        Player enemy = GenerateTestEnemy();
        battler.BattleGame(player, enemy);

        // Result
        uiQuitButton.SetActive(true);
        uiResultText.SetActive(true);
        
        if (battler.g_player_draw) animator.QueueAnimation(new AnimationDetail(AnimationDetail.Type.UIResult, "Draw"));
        if (battler.g_player_won) animator.QueueAnimation(new AnimationDetail(AnimationDetail.Type.UIResult, "Victory"));
        if (battler.g_player_lost) animator.QueueAnimation(new AnimationDetail(AnimationDetail.Type.UIResult, "Defeat"));

        animator.QueueAnimation(new AnimationDetail(AnimationDetail.Type.SetBoard, player));
    }

    public void UI__IncreaseSpeed() {

    }

    public void UI__DecreaseSpeed() {
        
    }

    public void UI__DoTest() {
        uiPlayButton.SetActive(false);
        player = GenerateTestPlayer();
        playerLoaded = true;
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetBoard, player));
    }

    public void UI__DoClear() {
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.ClearBoard));
    }

    public void UI__Reroll() {
        shop.Reroll(player);
        if (shop.g_error) {
            shop.g_error = false;
            Debug.Log("Not enough money rerolling the shop!");
        } else
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetShop, shop));
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetGold) {value = player.gold});
    }

    /* Mouse */
    public void UI__ShopItemClick(TurretController tc) {
        // if (contextActionInTransit) return;
        // Debug.Log($"{tc.turret} clicked");
        // List<TurretContextAction> actions = TurretContextGenerator.GenerateContextActions(tc.turret);
        // turretContextController.Activate(tc, actions);
    }

    public void UI__BoardTurretClick(TurretController tc) {
        if (contextActionInTransit) return;
        Debug.Log($"{tc.turret} clicked");
        if (Battler.IsEmpty(tc.turret)) return;
        List<TurretContextAction> actions = TurretContextGenerator.GenerateContextActions(tc.turret);
        turretContextController.Activate(tc, actions);
        (List<Recipe> indivRecipes, List<Recipe> lockedIndivRecipes) = Combiner.GetPossibleCombinations(player, tc.turret, player.recipes, player.lockedRecipes);
        combinerContextController.Activate(tc, indivRecipes, lockedIndivRecipes);
    }

    public void UI__EnemyBoardTurretClick(TurretController tc) {
        if (contextActionInTransit) return;
        Debug.Log($"{tc.turret} clicked");
        Debug.Log($"Click Enemy Turret: {tc.turret}");
    }

    public IEnumerator ActivateContextWindowInTime(TurretController tc, List<TurretContextAction> actions) {
        yield return new WaitForSeconds(0.01f);
        if (contextActionInTransit) yield break;
        turretContextController.Activate(tc, actions);
    }

    public IEnumerator DeactivateContextWindowInTime() {
        yield return new WaitForSeconds(0.01f);
        if (contextActionInTransit) yield break;
        turretContextController.Deactivate();
    }
    
    public void UI__ShopItemDrag(TurretController tc) {
        Debug.Log($"{tc.turret} drag start");
        StartCoroutine(DeactivateContextWindowInTime()); // Fired faster than "transit hold down" fires (from turret context menus' buttons)
    }

    public void UI__ShopItemDrop(TurretController tc) {
        Debug.Log($"{tc.turret} drop end");

        if (!_closestFound) { // BoardbaseGlow__OnHover fails to find the closest slot
            animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.ResetShopItemPos) {
                index = tc.shopIndex,
                gameObject = tc.gameObject,
            });
            if (contextActionInTransit) return;
            List<TurretContextAction> actions = TurretContextGenerator.GenerateContextActions(tc.turret);
            turretContextController.Activate(tc, actions);
            return;
        }

        // Check if board slot is occupied
        shop.Buy(tc.shopIndex, player, lastSlotBaseIndex);
        if (!shop.g_success) {
            animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.ResetShopItemPos) {
                index = tc.shopIndex,
                gameObject = tc.gameObject,
            });
            if (contextActionInTransit) return;
            List<TurretContextAction> actions = TurretContextGenerator.GenerateContextActions(tc.turret);
            turretContextController.Activate(tc, actions);
            return;
        }
        lastSlotBase.GetComponent<SpriteRenderer>().color = Color.white;

        if (!shop.g_success) return;

        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetBoard, player));
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SpendShopItemPos) {
            index = tc.shopIndex,
            gameObject = tc.gameObject,
        });
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetGold) {value = player.gold});

        UI__BoardTurretClick(boardObject.GetComponent<BoardController>().turrets[lastSlotBaseIndex].GetComponent<TurretController>());
        _closestApproach = int.MaxValue;
        _closestFound = false;
    }

    public void UI__LockFromCombining(TurretController tc) {
        // tc.turret.index;

    }

    public void UI__ClickRubble(TurretController tc) {

    }

    /* Context Actions */
    public void UI__TurretContextAction(TurretController tc, TurretContextAction action) {
        // While contextActionInTransit, no further context windows can be created or context actions can occur
        contextActionInTransit = true;

        // Do action - Anim
        switch (action.type) {
            case TurretContextAction.Type.Sell:
                shop.Sell(player, tc.turret.index);
                Debug.Log($"Context action [{action.type}] clicked - gaining {shop.lastGain}g");
                animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetBoard, player));
                break;
            case TurretContextAction.Type.Combine:
                // Combiner.GetPossibleCombinations(player, turret);
                // Turret combined = Combiner.GetNextCombination();
                // Turret combined = action.turret;
                // animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetBoard, player));
                break;
            case TurretContextAction.Type.Lock:
                player.turrets[tc.turret.index].locked = true;
                animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.Lock) {index = tc.turret.index});
                break;
            case TurretContextAction.Type.Unlock:
                player.turrets[tc.turret.index].locked = false;
                animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.Unlock) {index = tc.turret.index});
                break;
            case TurretContextAction.Type.DestroyRubble:
                shop.DestroyBoulder(player, tc.turret.index);
                Debug.Log($"Context action [{action.type}] clicked - for {shop.lastCost}g");
                animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetBoard, player));
                break;
            case TurretContextAction.Type.Freeze:
                shop.Freeze(player, tc.turret.index);
                Debug.Log($"Context action [{action.type}] clicked - for {shop.lastCost}g");
                shopObject.GetComponent<ShopController>().GetShopItemObject(tc.turret.index).GetComponent<TurretController>().Freeze(true);
                break;
            case TurretContextAction.Type.Unfreeze:
                shop.Unfreeze(player, tc.turret.index);
                Debug.Log($"Context action [{action.type}] clicked.");
                shopObject.GetComponent<ShopController>().GetShopItemObject(tc.turret.index).GetComponent<TurretController>().Freeze(false);
                break;
        }
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetPlayer, player));
        // StartCoroutine(DisableContextActionInTransitInTime());
        // StartCoroutine(DeactivateContextWindowInTime());
        contextActionInTransit = false;
        turretContextController.Deactivate();
    }

    public IEnumerator DisableContextActionInTransitInTime() {
        yield return new WaitForSeconds(0.01f);
        contextActionInTransit = false;
    }

    /* Continuous Reactive */
    Vector3 _dist = Vector3.zero;
    float _closestApproach = int.MaxValue;
    float _maximalDistanceFromBoard = 29;
    float _maximalDistanceFromSlot = 4;
    GameObject lastSlotBase; int lastSlotBaseIndex;
    bool _closestFound = false;
    // 
    float minDraggingTime = 0.1f;
    float currDraggingTime = 0.0f;

    public void UI__BoardBaseGlow__OnHover(GameObject draggedObject) {

        float sqrtDist = (boardObject.transform.position - draggedObject.transform.position).sqrMagnitude;
        _closestApproach = int.MaxValue;
        _closestFound = false;

        if (sqrtDist > _maximalDistanceFromBoard) {
            if (lastSlotBase) lastSlotBase.GetComponent<SpriteRenderer>().color = Color.white;
            return;
        }

        for (int i = 0; i < animator.board.turrets.Length; ++i) {
            
            _dist = draggedObject.transform.position - animator.board.slotBases[i].transform.position;
            sqrtDist = _dist.sqrMagnitude;

            if (sqrtDist > _maximalDistanceFromSlot) {
                continue;
            }
            
            if (sqrtDist < _closestApproach) {
                if (Battler.IsEmpty(player.turrets[i])) {
                    _closestApproach = sqrtDist;
                    _closestFound = true;                    
                    if (lastSlotBase && lastSlotBase != animator.board.slotBases[i])
                        lastSlotBase.GetComponent<SpriteRenderer>().color = Color.white;
                    lastSlotBase = animator.board.slotBases[i];
                    lastSlotBaseIndex = i;
                    lastSlotBase.GetComponent<SpriteRenderer>().color = Color.yellow;
                }
            }
        }

        // if (_closestFound) Debug.Log($"{animator.board.turrets[lastSlotBaseIndex].GetComponent<TurretController>().turret}:::::, {player.turrets[lastSlotBaseIndex]}:::::, {Battler.IsEmpty(player.turrets[lastSlotBaseIndex])}");
        
        if (!_closestFound && lastSlotBase) lastSlotBase.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void UI__NoneClicked() {
        // StartCoroutine(DeactivateContextWindowInTime());
        turretContextController.Deactivate();
    }

    /* Dev */

    public void DevUI__SetGold(int gold) {
        player.gold = gold;
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetGold) {value = gold});
    }

    public void DevUI__SetLife(int life) {
        player.life = life;
        animator.DoAnimationNow(new AnimationDetail(AnimationDetail.Type.SetLife) {value = life});
    }

    /* UI Questions */
    public static bool IsShopDraggable(Turret t) {
        if (t.name == TurretName.Empty) return false;
        if (t.player.type != Player.Type.Shop) return false;
        return true;
    }

    #endregion UI

    #region Physical 

        #region Elements
        
        public GameObject _____Elements_____;
        public GameObject boardObject;
        public GameObject enemyBoardObject;
        public GameObject shopObject;

        public void InitGameObjects() {
            InitPlayerObjects();
        }

        public void InitPlayerObjects() {
            boardObject.gameObject.SetActive(true);
            enemyBoardObject.gameObject.SetActive(false);            
        }

        public void InitEnemyObjects() {

        }

        #endregion Elements

    #endregion Physical

    #region Resources

    /* Prefabs */
    public GameObject _____Resources_____;
    public GameObject boardPrefab;
    public GameObject turretPrefab;

    /* Sprites */
    static Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();
    public static Sprite GetSprite(string name) {
        if (spriteDict.ContainsKey(name)) return spriteDict[name];
        else {
            spriteDict[name] = Resources.Load<Sprite>($"Sprites/Turrets/{name.ToLower()}");
            return spriteDict[name];
        } 
    }

    public static Sprite GetSprite(TurretName turretName) {
        return GetSprite(turretName.ToString());
    }

    public static Sprite EmptySprite() {
        return GetSprite("empty");
    }

    #endregion Resources

    #region Debug

    public bool g_anim_debug = true;
    public bool g_anim_battle = true;
    public bool g_anim_shop = true;
    public bool g_anim_player = true;
    public bool g_anim_network = true;
    public enum DebugType {
        anim,
        battle,
        shop,
        player,
        network,
    }
    public static void DebugLog(string message, DebugType type) {

    }

    #endregion Debug

    #region Test

    Player GenerateTestPlayer() {
        Player player = new Player();
        player.type = Player.Type.Player;
        player.SetName("Player");
        player.SetTurrets(Battler.GetStartingTurrets());
        // player.turrets[0] = TurretGenerator.CreateRockTurret(0);
        // player.turrets[1] = TurretGenerator.CreateRockTurret(1);
        // player.turrets[2] = TurretGenerator.CreateDiamondTurret(2);
        // player.turrets[3] = TurretGenerator.CreateRockTurret(3);
        // player.turrets[4] = TurretGenerator.CreateRockTurret(4);
        // player.turrets[5] = TurretGenerator.CreateRockTurret(5);
        // player.turrets[6] = TurretGenerator.CreateRockTurret(6);
        // player.turrets[7] = TurretGenerator.CreateRockTurret(7);
        // player.turrets[8] = TurretGenerator.CreateRockTurret(8);
        foreach (Turret turret in player.turrets) {
            turret.player = player;
        }
        player.SetRecipes(Combiner.GetAllPossibleCombinations(player));

        return player;
    }

    Player GenerateTestEnemy(int round = 0) {
        Player enemy = Player.InitEmpty();
        enemy.type = Player.Type.Enemy;
        enemy.SetName("Enemy");
        enemy.SetTurrets(Battler.GetStartingTurrets());
        // enemy.turrets[0] = TurretGenerator.CreateDiamondTurret(0);
        // enemy.turrets[1] = TurretGenerator.CreateRubyTurret(1);
        // enemy.turrets[2] = TurretGenerator.CreateRubyTurret(2);
        // enemy.turrets[3] = TurretGenerator.CreateRockTurret(3);
        // enemy.turrets[4] = TurretGenerator.CreateRockTurret(4);
        // enemy.turrets[5] = TurretGenerator.CreateRockTurret(5);
        // enemy.turrets[6] = TurretGenerator.CreateAmethystTurret(6);
        // enemy.turrets[7] = TurretGenerator.CreateRockTurret(7);
        // enemy.turrets[8] = TurretGenerator.CreateRockTurret(8);
        foreach (Turret turret in enemy.turrets) {
            turret.player = enemy;
        }
        enemy.SetRecipes(Combiner.GetAllPossibleCombinations(enemy));

        return enemy;
    }

    #endregion Test
}

/*
Tooltips
Borders
*/
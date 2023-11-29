using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

#region Enum, Classes
public class AnimationDetail  {
    public Type type;
    // Information
    public int time;
    public int value;
    public int count;
    public int index;
    public float strength;
    public bool contemperaneous = false;
    public Player player;
    public Player enemy;

    // Turret
    public Turret source;
    public Turret target;
    public List<Turret> turrets;
    

    // General Object
    public GameObject gameObject;

    // Result
    public int resultValue;
    public string message;

    public AnimationDetail(Type type, int time) {
        this.type = type;
        this.time = time;
    }
    
    public AnimationDetail(Type type, Player player) {
        this.type = type;
        this.player = player;
    }

    public AnimationDetail(Type type, string message) {
        this.type = type;
        this.message = message;
    }

    public AnimationDetail(Type type, Shop shop) {
        this.type = type;
        this.turrets = shop.shopTurrets;
    }

    public AnimationDetail(Type type) {
        this.type = type;
    }

    public enum Type {
        // General
        Wait,
        // Turret
        Attack,
        Bounce, // The bouncing of the gif only.
        Death,
        Spawn,
        // Skills
        Nuke,
        // Board
        EndGame,
        SetBoard,
        SetEnemyBoard,
        ClearBoard,
        Lock,
        Unlock,
        // Shop
        SetShop,
        ResetShopItemPos,
        SpendShopItemPos,
        // Stats Bar
        InitPlayerStatsBar,
        SetPlayer,
        SetGold,
        SetLife,
        DeductGold, // comes with fx
        DeductLife, // comes with fx
        // UI
        UIResult,
        VictoryResult,
        DefeatResult,
        DrawResult,
    }

}
#endregion Enum, Classes
public class AnimationController : MonoBehaviour
{

    #region World

    /* Collections */
    public GameObject floatingTextCollection;
    public GameObject fxCollection;

    /* Controllers */
    public BoardController board;
    public BoardController enemyBoard;
    public TooltipController tooltips;
    public UIController uiController;
    public StatsController statsController;
    public ShopController shopController;

    /* Prefabs */
    public GameObject floatingText;

    #endregion World

    #region UI

    public GameObject UIResultText;

    #endregion UI

    #region Init
    bool g_anim = true;
    bool g_playing = false;
    Queue<AnimationDetail> animations = new Queue<AnimationDetail>();

    // Start is called before the first frame update
    void Start()
    {
        board.playerType = Player.Type.Player;
        enemyBoard.playerType = Player.Type.Enemy;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(LoopRun());
    }

    IEnumerator LoopRun() {
        if (animations.Count > 0 && !g_playing) {
            g_playing = true;
            AnimationDetail anim = animations.Dequeue();
            yield return RunAnimation(anim);
            g_playing = false;
        }
    }

    #endregion Init

    #region Settings
    public float speed = 1.0f;

    #endregion Settings

    #region Animation
    // AnimQueue
    
    public void QueueAnimation(AnimationDetail anim) {
        animations.Enqueue(anim);
    }

    public void DoAnimationNow(AnimationDetail anim) {
        StartCoroutine(RunAnimation(anim));
    }

    // TODOs: SetStat instead of SetHealth but Stats mustn't be by reference
    IEnumerator RunAnimation(AnimationDetail anim) {
        // Debug.Log($"Run anim: {anim.type}");
        switch (anim.type) {
            // General
            case AnimationDetail.Type.Wait:
                yield return new WaitForSeconds((float) anim.time / 1000);
                break;
            // Turret
            case AnimationDetail.Type.Attack:
                // Determine who is attacking
                if (anim.source.player.type == Player.Type.Player) {
                    GameObject turretObject = board.GetTurretObject(anim.source.index);
                    GameObject opposingObject = enemyBoard.GetTurretObject(anim.target.index);

                    // Anim
                    StartCoroutine(turretObject.GetComponent<TurretController>().Reaction__BounceAnim());
                    StartCoroutine(opposingObject.GetComponent<TurretController>().Reaction__HurtAnim());
                    for (int i = 0; i < anim.value; ++i) {
                        yield return new WaitForSeconds(0.05f);
                        StartCoroutine(DamageText(opposingObject, anim.count, Player.Type.Enemy));
                    }
                    opposingObject.GetComponent<TurretController>().SetHealth(anim.resultValue);
                } else {
                    GameObject turretObject = enemyBoard.GetTurretObject(anim.source.index);
                    GameObject opposingObject = board.GetTurretObject(anim.target.index);

                    // Anim
                    StartCoroutine(turretObject.GetComponent<TurretController>().Reaction__BounceAnim());
                    StartCoroutine(opposingObject.GetComponent<TurretController>().Reaction__HurtAnim());
                    for (int i = 0; i < anim.value; ++i) {
                        yield return new WaitForSeconds(0.05f);
                        StartCoroutine(DamageText(opposingObject, anim.count, Player.Type.Player));                    
                    }
                    

                    opposingObject.GetComponent<TurretController>().SetHealth(anim.resultValue);

                }
                break;
            case AnimationDetail.Type.Death:
                if (anim.source.player.type == Player.Type.Player) {
                    GameObject turretObject = board.GetTurretObject(anim.source.index);
                    StartCoroutine(turretObject.GetComponent<TurretController>().Reaction__DeathAnim());
                } else {
                    GameObject turretObject = enemyBoard.GetTurretObject(anim.source.index);
                    StartCoroutine(turretObject.GetComponent<TurretController>().Reaction__DeathAnim());
                }
                break;
            case AnimationDetail.Type.Spawn:
                if (anim.source.player.type == Player.Type.Player) {
                } else {
                }
                break;
            //
            case AnimationDetail.Type.Bounce:
                if (anim.source.player.type == Player.Type.Player) {
                    GameObject turretObject = board.GetTurretObject(anim.index);
                    turretObject.GetComponent<TurretController>().Reaction__BounceAnim();
                } else {
                    GameObject turretObject = enemyBoard.GetTurretObject(anim.index);
                    turretObject.GetComponent<TurretController>().Reaction__BounceAnim();
                }
                break;
            // Skills
            case AnimationDetail.Type.Nuke:
                break;
            // Board
            case AnimationDetail.Type.EndGame:
                yield return new WaitForSeconds(1.0f);
                break;
            case AnimationDetail.Type.SetBoard:
                board.GenerateBoard(anim.player);
                break;
            case AnimationDetail.Type.SetEnemyBoard:
                enemyBoard.GenerateBoard(anim.player);
                break;
            case AnimationDetail.Type.ClearBoard:
                board.ClearBoard();
                enemyBoard.ClearBoard();
                break;
            case AnimationDetail.Type.Lock:
                board.GetTurretObject(anim.index).GetComponent<TurretController>().SetLock(true);
                break;
            case AnimationDetail.Type.Unlock:
                board.GetTurretObject(anim.index).GetComponent<TurretController>().SetLock(false);
                break;
            // Shop
            case AnimationDetail.Type.SetShop:
                shopController.SetShopItems(anim.turrets);

                break;
            case AnimationDetail.Type.ResetShopItemPos:
                anim.gameObject.transform.position = shopController.GetShopitemPosition(anim.index);
                break;
            case AnimationDetail.Type.SpendShopItemPos:
                anim.gameObject.transform.position = shopController.GetShopitemPosition(anim.index);
                anim.gameObject.GetComponent<TurretController>().OverwriteTurret(TurretGenerator.CreateEmpty());
                break;
            // Stat Bar
            case AnimationDetail.Type.InitPlayerStatsBar:
                statsController.SetPlayer(anim.player);
                break;
            case AnimationDetail.Type.SetPlayer:
                statsController.SetGold(anim.player.gold);
                statsController.SetLife(anim.player.life);
                break;
            case AnimationDetail.Type.SetGold:
                statsController.SetGold(anim.value);
                break;
            case AnimationDetail.Type.SetLife:
                statsController.SetLife(anim.value);
                break;
            // UI            
            case AnimationDetail.Type.UIResult:
                UIResultText.GetComponent<TextMeshProUGUI>().text = anim.message;
                break;
        }
        yield break;
    }

        #region Common
        IEnumerator DamageText(GameObject turretObject, int damage, Player.Type playerType) {

            float totalTime = 5.0f / speed;
            float time = 0;
            
            Vector3 v = new Vector3(1.0f + UnityEngine.Random.Range(0.5f, 2.5f), 15.0f);
            Vector3 a = new Vector3(0f, -50.0f);

            if (playerType == Player.Type.Player) {
                GameObject floater = Instantiate<GameObject>(floatingText, turretObject.transform.position, Quaternion.identity, floatingTextCollection.transform);
                floater.GetComponentInChildren<TextMeshProUGUI>().text = $"{damage}";
                v.x = -v.x;

                while (time < totalTime) {

                    floater.transform.position += v * Time.fixedDeltaTime;
                    v += a * Time.fixedDeltaTime;

                    time += Time.fixedDeltaTime;
                    yield return new WaitForSeconds(Time.fixedDeltaTime);
                }
                Destroy(floater);

            } else {
                GameObject floater = Instantiate<GameObject>(floatingText, turretObject.transform.position, Quaternion.identity, floatingTextCollection.transform);
                floater.GetComponentInChildren<TextMeshProUGUI>().text = $"{damage}";

                while (time < totalTime) {

                    floater.transform.position += v * Time.fixedDeltaTime;
                    v += a * Time.fixedDeltaTime;

                    time += Time.fixedDeltaTime;
                    yield return new WaitForSeconds(Time.fixedDeltaTime);
                }
                Destroy(floater);
            }
            yield break;
        }

        #endregion Common

    #endregion Animation

    #region Util

    public bool IsDone() {
        return animations.Count == 0;
    }

    public BoardController GetBoard(Player player) {
        if (player.type == Player.Type.Player)
            return board;
        else    return enemyBoard;
    }

    #endregion Util
}

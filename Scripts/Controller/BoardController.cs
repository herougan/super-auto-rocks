using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental;

public class BoardController : MonoBehaviour
{
    #region Const
    public const int BOARD_SIZE = 9;
    public static int count = 0;
    public Player.Type playerType;

    #endregion Const
    
    #region Init

    public int id;
    public GameObject[] turrets = new GameObject[BOARD_SIZE];
    public GameObject[] slotBases = new GameObject[BOARD_SIZE];
    public Vector3[] boardPositions = new Vector3[BOARD_SIZE];
    
    // UI
    public GameObject nameText;

    public void Init() {
        id = count;
        ++count;
    }

    public void Awake() {
        for (int i = 0; i < turrets.Length; ++i) {
            boardPositions[i] = turrets[i].transform.position;
        }
    }

    public void Update() {

    }

    #endregion Init

    #region Board 

    public void GenerateBoard(Player player) {
        gameObject.SetActive(true);
        int idx = 0;
        for (int i = 0; i < BOARD_SIZE; ++i) {
            if (playerType == Player.Type.Enemy)
                idx = Util.FlipX(i); // Flip physical index if it's opponent's board
            else
                idx = i;

            if (player.turrets.Length <= idx) {
                Debug.Log($"Player-{player.id}'s turret array is incomplete (@{idx}). (Need size {BOARD_SIZE}, received {player.turrets.Length})");
                break;
            }

            // Draw turrets
            if (player.turrets[idx].name != TurretName.Empty) {
                turrets[idx].GetComponent<TurretController>().SetSprite(Main.GetSprite($"{player.turrets[i].name}"));
                turrets[idx].GetComponent<TurretController>().SetStats(player.turrets[i].stats);
                turrets[idx].GetComponent<TurretController>().SetActive(true);
                turrets[idx].GetComponent<TurretController>().owner = player;
                turrets[idx].GetComponent<TurretController>().turret = player.turrets[i];
                turrets[idx].name = $"{player.turrets[i]} ({playerType}_board_{i})";
            } else {
                turrets[idx].GetComponent<TurretController>().SetActive(false);
                turrets[idx].name = $"Empty ({playerType}_board_{i})";
            }
        }

        nameText.GetComponent<TextMeshProUGUI>().text = player.name;
    }

    public GameObject GetTurretObject(int idx) {
        if (playerType == Player.Type.Player) {
            return turrets[idx];
        } else {
            return turrets[Util.FlipX(idx)];
        }
    }

    public void ClearBoard() {
        foreach (GameObject turret in turrets) {
            turret.GetComponent<TurretController>().Clear();
        }
    }

    #endregion Board

    #region Animation   

    public IEnumerator AnimateTurret(int idx, AnimationDetail.Type type) {
        StartCoroutine(turrets[idx].GetComponent<TurretController>().Reaction__AttackAnim());
        yield break;
    }

    #endregion Animation
}
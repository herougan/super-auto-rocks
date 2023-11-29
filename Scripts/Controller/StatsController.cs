using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    #region World
    // Game Objects
    public GameObject goldCollection;
    public GameObject heartsCollection;
    public GameObject goldText;

    // Resources
    public GameObject coinPrefab;
    public GameObject heartPrefab;
    
    // List
    public List<GameObject> coins = new List<GameObject>();
    public List<GameObject> hearts = new List<GameObject>();

    #endregion World

    #region Interactions

    /* Settings */
    public Vector3 coinStartingPos;
    public Vector3 heartStartingPos;
    
    /* State */
    public int currentGold = 0;
    public int currentHealth = 0;

    /* Physical Settings */
    public float coin_x = 0.08f;
    public float coin_y = 0.08f;
    public float heart_x = 0.35f;
    public float heart_y = 0.3f;
    //
    int coin_per_row = 20;
    int heart_per_row = 10;

    // TODO once coins > 100, change 100 coins into an electrum

    public void SetGold(int gold) {
        if (gold > currentGold) {
            for (int i = currentGold; i < gold; ++i) {
                SpawnCoin(i);
            }
        } else {
            for (int i = currentGold; i > gold; --i) {
                GameObject coin = coins.Last();
                coins.Remove(coin);
                Destroy(coin);
            }
        }
        currentGold = gold;
        goldText.GetComponent<TextMeshProUGUI>().text = $"({gold})";
    }

    public void SetLife(int health) {
        if (health > currentHealth) {
            for (int i = currentHealth; i < health; ++i) {
                SpawnHeart(i);
            }
        } else {
            for (int i = currentHealth; i > health; --i) {
                GameObject heart = hearts.Last();
                hearts.Remove(heart);
                Destroy(heart);
            }
        }
        currentHealth = health;
    }

    public void SpawnCoin(int coinIndex) {
        int col = coinIndex % coin_per_row;
        int row = coinIndex / coin_per_row;
        GameObject coin = Instantiate<GameObject>(coinPrefab, goldCollection.transform.position + new Vector3(col * coin_x, -row * coin_y, 0) + coinStartingPos, Quaternion.identity, goldCollection.transform);
        coins.Add(coin);
    }

    public void SpawnHeart(int heartIndex) {
        int col = heartIndex % heart_per_row;
        int row = heartIndex / heart_per_row;
        GameObject heart = Instantiate<GameObject>(heartPrefab, heartsCollection.transform.position + new Vector3(col * heart_x, -row * heart_y, 0) + heartStartingPos, Quaternion.identity, heartsCollection.transform);
        hearts.Add(heart);
    }

    #endregion Interactions

    #region Init

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayer(Player player) {
        SetGold(player.gold);
        SetLife(player.life);
    }

    #endregion Init
}

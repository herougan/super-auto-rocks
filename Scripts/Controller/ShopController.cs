using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public List<GameObject> shopTurrets;
    public GameObject classTurret;
    //
    public string layerName = "Shop";

    private List<Vector3> startingItemPositions = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < shopTurrets.Count; ++i) {
            startingItemPositions.Add(shopTurrets[i].transform.position);
            shopTurrets[i].GetComponent<TurretController>().SetDefaultLayer(layerName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetShopItems(List<Turret> turrets) {
        classTurret.SetActive(false);
        for (int i = 0; i < turrets.Count; ++i) {
            shopTurrets[i].SetActive(true);
            shopTurrets[i].GetComponent<TurretController>().SetTurret(turrets[i]);
            shopTurrets[i].GetComponent<TurretController>().turret.index = turrets[i].index;
            shopTurrets[i].GetComponent<TurretController>().shopIndex = i;
            shopTurrets[i].GetComponent<TurretController>().owner = turrets[i].player;
            shopTurrets[i].name = $"{turrets[i]} ({i})";
        }
        for (int i = turrets.Count; i < shopTurrets.Count; ++i) {
            shopTurrets[i].SetActive(false);
        }
    }

    public Vector3 GetShopitemPosition(int index) {
        return startingItemPositions[index];
    }

    public GameObject GetShopItemObject(int index) {
        return shopTurrets[index];
    }

    public void SetClassItem() {

    }

    public void RemoveClassItem() {

    }
}

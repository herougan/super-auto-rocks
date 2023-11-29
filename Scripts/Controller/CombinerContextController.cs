using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombinerContextController : MonoBehaviour
{
    /* Resources */
    public GameObject optionPrefab;

    /* Controllers */
    public Main mainController;

    /* Components */
    public List<GameObject> options;
    
    /* Settings */
    public Vector3 offset;

    public void Activate(TurretController tc, List<Recipe> recipes, List<Recipe> lockedRecipes) {
        Debug.Log(gameObject);

        int count = recipes.Count + lockedRecipes.Count;
        gameObject.SetActive(true);

        Debug.Log($"Recipes received: {recipes.Count}, {lockedRecipes.Count}, {options.Count}");

        // Set Active
        for (int i = options.Count - 1; i >= count; --i) {
            options[i].SetActive(false);
        }
        for (int i = options.Count; i < count; ++i) {
            GameObject option = (GameObject) Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
            options.Add(option);
        }
        for (int i = 0; i < count; ++i) {
            options[i].SetActive(true);
        }

        for (int i = 0; i < count; ++i) {
            if (i < recipes.Count) {
                options[i].GetComponent<Image>().sprite = Main.GetSprite(recipes[i].result);
                options[i].GetComponent<Image>().color = Color.white;
                // Unlock symbol TODO
            } else {
                options[i].GetComponent<Image>().sprite = Main.GetSprite(lockedRecipes[i - recipes.Count].result);
                options[i].GetComponent<Image>().color = Color.grey;
                // Lock symbol
            }
        }

        transform.position = tc.gameObject.transform.position + offset;
    }

    public void Deactivate() {
        ClearAll();
        gameObject.SetActive(false);
    }

    public void ClearAll() {
        for (int i = 0; i < options.Count; ++i) {
            options[i].SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        options = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

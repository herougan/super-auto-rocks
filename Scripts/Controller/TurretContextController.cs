using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurretContextController : MonoBehaviour
{
    /* Physical */
    public List<GameObject> buttons;
    public Main mainController;
    public CombinerContextController contextController;

    /* State */
    public TurretController turret; // Turret that triggered this menu

    /* Variables */
    public Vector3 offset = Vector3.zero;
    public float y_offset = 0.35f;

    public void ClearAll() {
        for (int i = 0; i < buttons.Count; ++i) {
            buttons[i].SetActive(false);
        }
    }

    public void BuildActions() {

    }

    public void Activate(TurretController tc, List<TurretContextAction> actions) {

        // Load button options
        turret = tc;
        transform.position = turret.transform.position + offset;
        for (int i = 0; i < actions.Count; ++i) {

            // Visual
            buttons[i].SetActive(true);
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = TurretContextGenerator.tcaToString[actions[i].type];
            buttons[i].transform.position = transform.position + new Vector3(0, actions.Count / 2.0f * y_offset - i * y_offset, 0);

            // Listener and action
            TurretContextAction new_action = actions[i];
            buttons[i].GetComponent<Button>().onClick.RemoveAllListeners();            
            buttons[i].GetComponent<Button>().onClick.AddListener(delegate{
                mainController.UI__TurretContextAction(tc, new_action);
            });
        }
        for (int i = actions.Count; i < buttons.Count; ++i) {
            buttons[i].SetActive(false);
        }
        gameObject.SetActive(true);

        // Load combinable options
        
    }

    public void OnTransit() {
        mainController.contextActionInTransit = true;
    }

    public void OffTransit() {
        mainController.contextActionInTransit = false;
    }

    public void Deactivate() {
        ClearAll();
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

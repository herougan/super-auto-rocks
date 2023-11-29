using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public Main mainController;
    public GameObject goldTextInput;
    public GameObject healthTextInput;
    
    /* Dev */
    public void DevUI__SetGold() {
        if (goldTextInput.GetComponent<TMP_InputField>().text == "") return;
        mainController.DevUI__SetGold(int.Parse(goldTextInput.GetComponent<TMP_InputField>().text));
    }

    public void DevUI__SetLife() {
        if (healthTextInput.GetComponent<TMP_InputField>().text == "") return;
        mainController.DevUI__SetLife(int.Parse(healthTextInput.GetComponent<TMP_InputField>().text));
    }
}

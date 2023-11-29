using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera _main;
    public Main mainController;

    /* State */
    bool clickedContextable = false;

    // Start is called before the first frame update
    void Start()
    {
        _main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick(InputAction.CallbackContext context) {
        if (mainController.contextActionInTransit) return;

        if (context.started) {

            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // RaycastHit hit;
            // Physics.Raycast(ray, out hit);
            // if (hit.collider != null) {
            //     Debug.Log(hit.collider.gameObject);
            // }
        } else if (context.canceled) {

            clickedContextable = false;
            
            RaycastHit2D rayHit = Physics2D.GetRayIntersection(_main.ScreenPointToRay(Mouse.current.position.ReadValue()));
            if (!rayHit.collider) {
                mainController.UI__NoneClicked();
                return;
            }

            // Check if turret
            TurretController tc = rayHit.collider.gameObject.GetComponent<TurretController>();
            if (tc) {

                clickedContextable = true;
                // If Shop item
                if (tc.owner.type == Player.Type.Shop && tc.turret.name != TurretName.Empty) mainController.UI__ShopItemClick(tc);

                // If Board item
                if (tc.owner.type == Player.Type.Player) mainController.UI__BoardTurretClick(tc);

                // If Enemy board item
                if (tc.owner.type == Player.Type.Enemy) mainController.UI__EnemyBoardTurretClick(tc);

            } else {
                mainController.UI__NoneClicked();
            }
        }
    }

    // Object
    Vector3 offset;
    string layerName;
    GameObject objectToDrag;
    Vector3 originalPosition;
    DraggedType objectType;
    //
    private bool dragging = false;
    private string buttonControlPath = "/Mouse/leftButton";
    private string deltaControlPath = "/Mouse/delta";

    private void StartDrag() {

        RaycastHit2D rayHit = Physics2D.GetRayIntersection(_main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;
        objectToDrag = rayHit.collider.gameObject;

        // Shop Turret
        if (objectToDrag.GetComponent<TurretController>() && Main.IsShopDraggable(objectToDrag.GetComponent<TurretController>().turret)) {
            originalPosition = objectToDrag.transform.position;
            offset = objectToDrag.transform.position - _main.ScreenToWorldPoint(Input.mousePosition);
            objectToDrag.GetComponent<TurretController>().g_dragged = true;
            objectToDrag.GetComponent<TurretController>().SetLayer("DragAndDrop");            

            dragging = true;
            mainController.UI__ShopItemDrag(objectToDrag.GetComponent<TurretController>());
        } else return;

		// Combinable Turret


		// Class Turret
    }

	// Unintended: Holding your mouse and entering the shopItem object also counts as a start drag
    public void ClickHoldRelease(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (context.control.path == buttonControlPath)
            {
                if (dragging) {
                    StartDrag();
                }
            }
        }
        else if (context.performed)
        {
            if (context.control.path == buttonControlPath)
            {
                if (dragging) {
                    objectToDrag.transform.position = _main.ScreenToWorldPoint(Input.mousePosition) + offset;
					mainController.UI__BoardBaseGlow__OnHover(objectToDrag);
                } else { // sometimes the first click is in performed not started
                    StartDrag();
                }
            }
        }
        else if (context.canceled)
        {
            if (context.control.path == deltaControlPath || context.control.path == buttonControlPath)
            {
                if (dragging) {
                    dragging = false;
                    objectToDrag.GetComponent<TurretController>().g_dragged = true;
                    objectToDrag.GetComponent<TurretController>().ResetLayer();
                    mainController.UI__ShopItemDrop(objectToDrag.GetComponent<TurretController>());
                }
            }
        }
    }
    enum DraggedType {
        Turret,
    }
}

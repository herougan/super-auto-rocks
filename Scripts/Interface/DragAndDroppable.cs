using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DragAndDroppable : MonoBehaviour
{
    private bool dragging = false;
    private Vector3 offset;
    string layerName;

    void Update() {
        if (dragging) {
            // Move object
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        }
    }

    private void OnMouseDown() {
        // Record offset
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        layerName = gameObject.GetComponent<SpriteRenderer>().sortingLayerName;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "DragAndDrop";
        dragging = true;
    }

    private void OnMouseUp() {
        // Stop dragging
        dragging = false;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
    }
}
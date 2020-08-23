using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disc : MonoBehaviour
{
    [HideInInspector] public control control;    
    [HideInInspector] public Vector2Int pos;

    private void OnMouseUp()
    {
        control.click(pos);
    }
}

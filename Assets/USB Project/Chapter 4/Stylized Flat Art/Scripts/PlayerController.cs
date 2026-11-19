using System;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform PlayerPosition;
    private Renderer myRenderer;
    
    private static readonly int PositionID = Shader.PropertyToID("_Position");
    
    void Update()
    {
        if (!PlayerPosition) return;
        
        if (PlayerPosition.hasChanged)
        {
            myRenderer = GetComponent<Renderer>();
            myRenderer.sharedMaterial.SetVector(PositionID, PlayerPosition.position);
        }
    }
}

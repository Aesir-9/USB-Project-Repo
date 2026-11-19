using UnityEngine;
using UnityEngine.Serialization;

public class SimpleColorController : MonoBehaviour
{
    public ComputeShader Shader;
    public Texture Albedo;
    
    private RenderTexture baseColor;
    private int textureSize = 256;
    private Renderer renderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseColor = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
        baseColor.enableRandomWrite = true;
        baseColor.Create();
        
        renderer = GetComponent<Renderer>();
        renderer.enabled = true;
        
        Shader.SetTexture(0, "Result", baseColor);
        Shader.SetTexture(0, "Albedo", Albedo);
        
        renderer.material.SetTexture("_BaseMap", baseColor);
        Shader.Dispatch(0, textureSize / 8, textureSize / 8, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

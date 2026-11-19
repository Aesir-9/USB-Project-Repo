using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Renderer))]
public class BrushController : MonoBehaviour
{
    public ComputeShader Shader;
    public Camera Camera;
    
    public Color CanvasColor = Color.white;
    public Color BrushColor = Color.black;
    [Range(1f, 64f)] public float BrushRadius = 12f;
    [Range(0f, 32f)] public float BrushSmooth = 4f;
    
    private int textureSize = 512;
    private RenderTexture canvas;
    private Renderer renderer;
    private int clearKernel;
    private int paintKernel;

    private static readonly int CanvasID = UnityEngine.Shader.PropertyToID("Canvas");
    private static readonly int ClearColorID = UnityEngine.Shader.PropertyToID("ClearColor");
    private static readonly int BrushColorID = UnityEngine.Shader.PropertyToID("BrushColor");
    private static readonly int BrushPositionID = UnityEngine.Shader.PropertyToID("BrushPosition");
    private static readonly int BrushRadiusID = UnityEngine.Shader.PropertyToID("BrushRadius");
    private static readonly int BrushSmoothID = UnityEngine.Shader.PropertyToID("BrushSmooth");

    private void Start()
    {
        renderer = GetComponent<Renderer>();

        if (Camera == null)
        {
            Camera = Camera.main;
        }
        
        canvas = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
        canvas.enableRandomWrite = true;
        canvas.Create();

        clearKernel = Shader.FindKernel("CSClear");
        paintKernel = Shader.FindKernel("CSPaint");

        ClearCanvas();

        renderer.material.mainTexture = canvas;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Paint();
        }
    }

    private void ClearCanvas()
    {
        Shader.SetTexture(clearKernel, CanvasID, canvas);
        Shader.SetVector(ClearColorID, CanvasColor);

        int groups = Mathf.CeilToInt(textureSize / 8f);
        Shader.Dispatch(clearKernel, groups, groups, 1);
    }

    private void Paint()
    {
        Ray ray = Camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        if (hit.collider.gameObject != gameObject) return;

        Vector2 uv = hit.textureCoord;
        Vector2 pixelPosition = new Vector2(uv.x * textureSize, uv.y * textureSize);

        Shader.SetTexture(paintKernel, CanvasID, canvas);
        Shader.SetVector(BrushColorID, BrushColor);
        Shader.SetVector(BrushPositionID, pixelPosition);
        Shader.SetFloat(BrushRadiusID, BrushRadius);
        Shader.SetFloat(BrushSmoothID, BrushSmooth);

        int groups = Mathf.CeilToInt(textureSize / 8f);
        Shader.Dispatch(paintKernel, groups, groups, 1);
    }

    private void OnDestroy()
    {
        if (canvas != null)
        {
            canvas.Release();
        }
    }
}

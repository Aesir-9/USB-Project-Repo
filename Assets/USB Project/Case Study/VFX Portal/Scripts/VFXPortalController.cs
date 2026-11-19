using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))][ExecuteAlways]
// ReSharper disable once CheckNamespace
public class VFXPortalController : MonoBehaviour
{
    private ParticleSystem _ps;
    private Renderer _renderer;
    private int _numParticles;
    
    private const int MaxParticles = 100;
    
    private readonly ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[MaxParticles];
    private readonly Vector4[] _particlesPosition = new Vector4[MaxParticles];
    private readonly float[] _particlesSize = new float[MaxParticles];

    static readonly int ParticlesAmount = Shader.PropertyToID("_ParticlesAmount");
    static readonly int ParticlesSize = Shader.PropertyToID("_ParticlesSize");
    static readonly int ParticlesPosition = Shader.PropertyToID("_ParticlesPosition");
    
    void OnEnable()
    {
        _ps = GetComponent<ParticleSystem>();
        _renderer = _ps.GetComponent<Renderer>();
    }
    
    void Update()
    {
        _numParticles = _ps.particleCount;

        if (_numParticles == 0)
        {
            return;
        }

        _ps.GetParticles(_particles, MaxParticles);

        int i = 0;
        foreach (var particle in _particles)
        {
            _particlesPosition[i] = particle.position;
            _particlesSize[i] = particle.GetCurrentSize(_ps);
            ++i;
        }
        
        _renderer.sharedMaterial.SetFloat(ParticlesAmount, _numParticles);
        _renderer.sharedMaterial.SetFloatArray(ParticlesSize, _particlesSize);
        _renderer.sharedMaterial.SetVectorArray(ParticlesPosition, _particlesPosition);
    }
}

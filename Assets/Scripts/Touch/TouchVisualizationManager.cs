using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace VRAvatar.Touch
{
    /// <summary>
    /// Manages visual feedback for touch interactions
    /// Creates and manages particle effects and visual indicators
    /// </summary>
    public class TouchVisualizationManager : MonoBehaviour
    {
        [Header("Visual Settings")]
        public GameObject touchEffectPrefab;
        public Material touchMaterial;
        public float effectDuration = 2f;
        public float effectScale = 0.1f;
        
        [Header("Particle Settings")]
        public bool useParticles = true;
        public ParticleSystem particleSystemPrefab;
        public int particleCount = 50;
        
        [Header("Glow Settings")]
        public bool useGlowEffect = true;
        public Shader glowShader;
        public float glowIntensity = 2f;
        
        // Effect tracking
        private Dictionary<Vector3, GameObject> activeEffects = new Dictionary<Vector3, GameObject>();
        private Dictionary<Vector3, ParticleSystem> activeParticles = new Dictionary<Vector3, ParticleSystem>();
        private ObjectPool<GameObject> effectPool;
        private ObjectPool<ParticleSystem> particlePool;
        
        private void Start()
        {
            InitializePools();
            CreateDefaultPrefabs();
        }
        
        private void InitializePools()
        {
            // Initialize object pools for performance
            effectPool = new ObjectPool<GameObject>(CreateTouchEffect, 20);
            
            if (useParticles)
            {
                particlePool = new ObjectPool<ParticleSystem>(CreateParticleEffect, 10);
            }
        }
        
        private void CreateDefaultPrefabs()
        {
            // Create default touch effect prefab if none assigned
            if (touchEffectPrefab == null)
            {
                touchEffectPrefab = CreateDefaultTouchEffect();
            }
            
            // Create default particle system if none assigned
            if (useParticles && particleSystemPrefab == null)
            {
                particleSystemPrefab = CreateDefaultParticleSystem();
            }
        }
        
        /// <summary>
        /// Show visual touch effect at specified position
        /// </summary>
        public void ShowTouchEffect(Vector3 position, Color color, float intensity)
        {
            // Show main touch effect
            GameObject effect = effectPool.Get();
            effect.transform.position = position;
            effect.transform.localScale = Vector3.one * (effectScale * (0.5f + intensity * 0.5f));
            
            // Configure material
            Renderer renderer = effect.GetComponent<Renderer>();
            if (renderer != null && touchMaterial != null)
            {
                Material instanceMaterial = new Material(touchMaterial);
                instanceMaterial.color = color;
                
                if (useGlowEffect && glowShader != null)
                {
                    instanceMaterial.shader = glowShader;
                    instanceMaterial.SetFloat("_GlowIntensity", glowIntensity * intensity);
                }
                
                renderer.material = instanceMaterial;
            }
            
            activeEffects[position] = effect;
            
            // Show particle effect
            if (useParticles)
            {
                ShowParticleEffect(position, color, intensity);
            }
            
            // Auto-hide after duration
            StartCoroutine(HideEffectAfterDelay(position, effectDuration));
        }
        
        /// <summary>
        /// Hide touch effect at specified position
        /// </summary>
        public void HideTouchEffect(Vector3 position)
        {
            // Hide main effect
            if (activeEffects.ContainsKey(position))
            {
                GameObject effect = activeEffects[position];
                activeEffects.Remove(position);
                effectPool.Return(effect);
            }
            
            // Hide particle effect
            if (activeParticles.ContainsKey(position))
            {
                ParticleSystem particles = activeParticles[position];
                activeParticles.Remove(position);
                particles.Stop();
                StartCoroutine(ReturnParticlesAfterStopped(particles));
            }
        }
        
        private void ShowParticleEffect(Vector3 position, Color color, float intensity)
        {
            ParticleSystem particles = particlePool.Get();
            particles.transform.position = position;
            
            // Configure particle system
            var main = particles.main;
            main.startColor = color;
            main.maxParticles = Mathf.RoundToInt(particleCount * intensity);
            main.startSpeed = 2f * intensity;
            main.startLifetime = effectDuration;
            
            var emission = particles.emission;
            emission.rateOverTime = particleCount * intensity / effectDuration;
            
            var shape = particles.shape;
            shape.radius = effectScale * intensity;
            
            particles.Play();
            activeParticles[position] = particles;
        }
        
        private GameObject CreateTouchEffect()
        {
            GameObject effect = new GameObject("TouchEffect");
            
            // Add sphere mesh
            MeshRenderer renderer = effect.AddComponent<MeshRenderer>();
            MeshFilter filter = effect.AddComponent<MeshFilter>();
            filter.mesh = CreateSphereMesh();
            
            if (touchMaterial != null)
            {
                renderer.material = touchMaterial;
            }
            
            // Add animation component
            TouchEffectAnimator animator = effect.AddComponent<TouchEffectAnimator>();
            
            effect.SetActive(false);
            return effect;
        }
        
        private ParticleSystem CreateParticleEffect()
        {
            GameObject particles = new GameObject("TouchParticles");
            ParticleSystem ps = particles.AddComponent<ParticleSystem>();
            
            // Configure default particle settings
            var main = ps.main;
            main.startLifetime = effectDuration;
            main.startSpeed = 2f;
            main.startSize = 0.05f;
            main.maxParticles = particleCount;
            
            var emission = ps.emission;
            emission.rateOverTime = 0; // We'll use bursts
            
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = effectScale;
            
            particles.SetActive(false);
            return ps;
        }
        
        private GameObject CreateDefaultTouchEffect()
        {
            GameObject prefab = new GameObject("DefaultTouchEffect");
            
            // Create sphere
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.SetParent(prefab.transform);
            sphere.transform.localPosition = Vector3.zero;
            
            // Remove collider
            Destroy(sphere.GetComponent<Collider>());
            
            // Create default material
            Material mat = new Material(Shader.Find("Standard"));
            mat.SetFloat("_Mode", 3); // Transparent
            mat.color = new Color(1, 1, 1, 0.5f);
            sphere.GetComponent<Renderer>().material = mat;
            
            return prefab;
        }
        
        private ParticleSystem CreateDefaultParticleSystem()
        {
            GameObject particles = new GameObject("DefaultTouchParticles");
            return particles.AddComponent<ParticleSystem>();
        }
        
        private Mesh CreateSphereMesh()
        {
            // Create a simple sphere mesh
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Mesh mesh = temp.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(temp);
            return mesh;
        }
        
        private IEnumerator HideEffectAfterDelay(Vector3 position, float delay)
        {
            yield return new WaitForSeconds(delay);
            HideTouchEffect(position);
        }
        
        private IEnumerator ReturnParticlesAfterStopped(ParticleSystem particles)
        {
            yield return new WaitUntil(() => !particles.isPlaying);
            particlePool.Return(particles);
        }
        
        /// <summary>
        /// Clear all active visual effects
        /// </summary>
        public void ClearAllEffects()
        {
            var positions = new List<Vector3>(activeEffects.Keys);
            foreach (var position in positions)
            {
                HideTouchEffect(position);
            }
        }
    }
    
    /// <summary>
    /// Simple animator for touch effects
    /// </summary>
    public class TouchEffectAnimator : MonoBehaviour
    {
        private Vector3 originalScale;
        private float animationTime = 0f;
        
        private void OnEnable()
        {
            originalScale = transform.localScale;
            animationTime = 0f;
        }
        
        private void Update()
        {
            animationTime += Time.deltaTime;
            
            // Pulse animation
            float pulse = Mathf.Sin(animationTime * 5f) * 0.2f + 1f;
            transform.localScale = originalScale * pulse;
            
            // Fade out over time
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                color.a = Mathf.Lerp(1f, 0f, animationTime / 2f);
                renderer.material.color = color;
            }
        }
    }
    
    /// <summary>
    /// Simple object pool for performance
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private Queue<T> pool = new Queue<T>();
        private System.Func<T> createFunc;
        
        public ObjectPool(System.Func<T> createFunction, int initialSize = 10)
        {
            createFunc = createFunction;
            
            // Pre-populate pool
            for (int i = 0; i < initialSize; i++)
            {
                T obj = createFunc();
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }
        
        public T Get()
        {
            T obj;
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else
            {
                obj = createFunc();
            }
            
            obj.gameObject.SetActive(true);
            return obj;
        }
        
        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}
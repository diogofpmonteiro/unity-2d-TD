using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    public static event Action<Platform> OnPlatformClicked;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private GameObject shadowPrefab; 

    private GameObject shadowInstance;
    private SpriteRenderer platformRenderer;
    private bool hasTower = false;

    public static bool towerPanelOpen { get; set; } = false; 
    
    private void Awake() {
        platformRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Pre-instantiate shadow and hide it
        if (shadowPrefab != null)
        {
            shadowInstance = Instantiate(shadowPrefab, transform.position, Quaternion.identity);
            shadowInstance.SetActive(false);
        }
    }

    private void Update()
    {
        if (towerPanelOpen || Time.timeScale == 0f) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero,
                                                        Mathf.Infinity, platformLayerMask);

            if (raycastHit.collider != null)
            {
                Platform platform = raycastHit.collider.GetComponent<Platform>();
                if (platform != null)
                {
                    OnPlatformClicked?.Invoke(platform);
                }
            }
        }
    }

    public void PlaceTower(TowerData data)
    {
        Instantiate(data.prefab, transform.position + Vector3.up * 0.25f, Quaternion.identity, transform);
        hasTower = true;
        HideShadow();
    }
    
   private void OnMouseEnter()
    {
        if (!hasTower && shadowPrefab != null)
        {
            shadowInstance.SetActive(true);
            platformRenderer.color = new Color(1f, 1f, 1f, 0.3f); // semi-transparent
        }
    }

    private void OnMouseExit()
    {
        HideShadow();
    }

    private void HideShadow()
    {
        if (shadowInstance != null)
        {
            shadowInstance.SetActive(false);
        }
        platformRenderer.color = Color.white;
    }
}

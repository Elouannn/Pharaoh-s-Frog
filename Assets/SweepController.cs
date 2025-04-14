using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;

public class UIScanlineContinuousController : MonoBehaviour
{
    [Header("Références")]
    public Image targetImage;               // Image UI à contrôler

    [Header("Animation du balayage")]
    public bool animateScanline = true;
    public float scanSpeed = 0.5f;          // Vitesse de déplacement de la bande

    [Header("Propriétés du shader")]
    [Range(0, 1)]
    public float scanlinePosition = 0f;     // Position manuelle si l'animation est désactivée
    [Range(0.001f, 0.5f)]
    public float bandWidth = 0.1f;          // Largeur de la bande

    // Variables internes
    private Material imageMaterial;
    private float currentPosition = 0f;

    public Texture2D NewTexture;

    // Propriétés pour changer la texture dynamiquement
    public Texture2D CurrentTexture
    {
        get
        {
            if (targetImage != null && targetImage.material != null)
                return targetImage.material.GetTexture("_MainTex") as Texture2D;
            return null;
        }
        set
        {
            if (targetImage != null && targetImage.material != null)
            {
                Sprite newSprite = Sprite.Create(
                    value,
                    new Rect(0, 0, value.width, value.height),
                    new Vector2(0.5f, 0.5f)
                );
                targetImage.sprite = newSprite;
                targetImage.material.SetTexture("_MainTex", value);
            }
        }
    }

    void Start()
    {
        if (targetImage == null)
        {
            // Essayer d'obtenir l'image sur le même GameObject
            targetImage = GetComponent<Image>();

            if (targetImage == null)
            {
                Debug.LogError("UIScanlineContinuousController: Aucune Image UI assignée ou trouvée!");
                return;
            }
        }

        // Créer une instance du matériau
        if (targetImage.material != null && targetImage.material.shader.name == "Custom/UIScanlineContinuous")
        {
            imageMaterial = targetImage.material;
        }
        else
        {
            // Trouver le shader
            Shader scanlineShader = Shader.Find("Custom/UIScanlineContinuous");

            if (scanlineShader == null)
            {
                Debug.LogError("UIScanlineContinuousController: Shader 'Custom/UIScanlineContinuous' introuvable!");
                return;
            }

            // Créer un nouveau matériau
            imageMaterial = new Material(scanlineShader);
            targetImage.material = imageMaterial;
        }

        // Initialiser la position du balayage
        currentPosition = scanlinePosition;
        UpdateShaderProperties();
        SetTexture(NewTexture);
    }

    void Update()
    {
        if (imageMaterial == null) return;

        if (animateScanline)
        {
            // Mettre à jour la position - mouvement continu de gauche à droite
            currentPosition += scanSpeed * Time.deltaTime;

            // Réinitialiser quand on atteint la fin (ou un peu après pour éviter les sauts visuels)
            if (currentPosition >= 1f + bandWidth)
                currentPosition = -bandWidth; // Commencer hors écran à gauche
        }
        else
        {
            // Utiliser la position manuelle
            currentPosition = scanlinePosition;
        }

        UpdateShaderProperties();
    }

    // Mise à jour des propriétés du shader
    void UpdateShaderProperties()
    {
        imageMaterial.SetFloat("_ScanlinePos", currentPosition);
        imageMaterial.SetFloat("_ScanlineWidth", bandWidth);
    }

    // Méthode publique pour changer la texture
    public void SetTexture(Texture2D newTexture)
    {
        CurrentTexture = newTexture;
    }
}
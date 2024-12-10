using UnityEngine;
using UnityEngine.UI;

public class changeSprite : MonoBehaviour
{
    public Sprite normalSprite; // Default sprite
    public Sprite highlightedSprite; // Highlighted sprite

    private Image buttonImage; // Reference to the Image component of the button

    // Called when the script is initialized
    void Start()
    {
        // Get the Image component attached to this GameObject
        buttonImage = GetComponent<Image>();

        // Set the button's sprite to the normal one on startup
        if (buttonImage != null && normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }

    // This function changes the sprite to the highlighted version
    public void OnPointerEnter()
    {
        if (buttonImage != null && highlightedSprite != null)
        {
            buttonImage.sprite = highlightedSprite;
        }
    }

    // This function changes the sprite back to the normal version
    public void OnPointerExit()
    {
        if (buttonImage != null && normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this for TextMeshPro support

public class PlayerSlider : MonoBehaviour
{
    public Slider playerSlider; // Reference to the slider
    public TMP_Text playerCountText; // Reference to the TextMeshPro text displaying the number of players

    void Start()
    {
        // Initialize the text to match the slider's initial value
        UpdatePlayerCount(playerSlider.value);

        // Add a listener to detect changes in the slider value
        playerSlider.onValueChanged.AddListener(delegate { UpdatePlayerCount(playerSlider.value); });
    }

    // Update the text based on the slider's value
    void UpdatePlayerCount(float value)
    {
        playerCountText.text = $"Players: {value}";
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeText : MonoBehaviour
{
    [SerializeField]private string volumeName;
    [SerializeField]private string textIntro; //sound: or music:
    [SerializeField]private TextMeshProUGUI txt;


    private void Update()
    {
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        float volumeValue = PlayerPrefs.GetFloat(volumeName) * 100;
        txt.text = textIntro + volumeValue.ToString() + "%";
    }
}

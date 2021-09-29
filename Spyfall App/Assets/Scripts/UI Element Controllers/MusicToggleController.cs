using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

public class MusicToggleController : MonoBehaviour
{
    [SerializeField] private Sprite musicIcon;
    [SerializeField] private Sprite mutedMusicIcon;
    [SerializeField] private List<ProceduralImage> images = new List<ProceduralImage>();
    private ManageAudio manageAudio;
    private Toggle toggle;

    private void Awake() {
        manageAudio = FindObjectOfType<ManageAudio>();
        toggle = GetComponent<Toggle>();
    }

    // called from inspector
    public void OnToggleChanged() {
        if (toggle.isOn) {
            images.ForEach(image => image.sprite = mutedMusicIcon);
            manageAudio.Mute("bg_music", true);
        } else {
            images.ForEach(image => image.sprite = musicIcon);
            manageAudio.Unmute("bg_music", true);
        }
    }
}

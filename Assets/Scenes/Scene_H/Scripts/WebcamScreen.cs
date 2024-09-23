using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamScreen : MonoBehaviour
{
    public Renderer display;
    WebCamTexture camTexture;
    private int currentIndex = 0;

    private void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(devices[i].name);
        }

        if (camTexture != null)
        {
            display.material.mainTexture = null;
            camTexture.Stop();
            camTexture = null;
        }

        WebCamDevice device = WebCamTexture.devices[currentIndex];
        camTexture = new WebCamTexture(device.name);
        camTexture.Play();
    }

    void Update()
    {
        if (camTexture != null && camTexture.isPlaying)
        {
            display.material.mainTexture = camTexture;
            display.material.mainTextureScale = new Vector2(-2.55f, 1.01f); // X축으로 뒤집음
        }
    }
}

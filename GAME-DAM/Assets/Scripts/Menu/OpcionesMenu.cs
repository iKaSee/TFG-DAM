using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; 

public class OpcionesMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolucionDropdown;
    Resolution[] resoluciones;

    void Start()
    {
        resoluciones = Screen.resolutions;
        resolucionDropdown.ClearOptions();
        List<string> opciones = new List<string>();
        int resolucionActualIndex = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string opcion = resoluciones[i].width + " x " + resoluciones[i].height;
            opciones.Add(opcion);

            if (resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActualIndex = i;
            }
        }
        resolucionDropdown.AddOptions(opciones);
        resolucionDropdown.value = resolucionActualIndex;
        resolucionDropdown.RefreshShownValue();
    }

   public void SetMasterVol(float volumen)
{
    audioMixer.SetFloat("MasterVol", Mathf.Log10(volumen) * 20);
}

public void SetMusicaVol(float volumen)
{
    audioMixer.SetFloat("MusicaVol", Mathf.Log10(volumen) * 20);
}

public void SetVFXVol(float volumen)
{
    audioMixer.SetFloat("VFXVol", Mathf.Log10(volumen) * 20);
}

    public void SetResolucion(int indexResolucion)
{
    Resolution res = resoluciones[indexResolucion];
    Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    
    Debug.Log("Cambiando resolución a: " + res.width + " x " + res.height);
}
}
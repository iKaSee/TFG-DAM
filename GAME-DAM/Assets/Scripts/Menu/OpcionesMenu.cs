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

    // --- Referencias a los botones y sliders visuales ---
    [Header("Referencias UI")]
    public Slider sliderMaster;
    public Slider sliderMusica;
    public Slider sliderVFX;
    public Toggle togglePantallaCompleta;
    // -----------------------------------------------------------

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
        
        // --- CARGAR LOS DATOS GUARDADOS AL INICIO ---
        
        // 1. Cargar Volumen (Por defecto 1, que suele ser el máximo en sliders de 0 a 1)
        float volMasterGuardado = PlayerPrefs.GetFloat("MasterVol", 1f);
        float volMusicaGuardado = PlayerPrefs.GetFloat("MusicaVol", 1f);
        float volVFXGuardado = PlayerPrefs.GetFloat("VFXVol", 1f);

        // Colocar visualmente las barritas en su sitio
        if (sliderMaster != null) sliderMaster.value = volMasterGuardado;
        if (sliderMusica != null) sliderMusica.value = volMusicaGuardado;
        if (sliderVFX != null) sliderVFX.value = volVFXGuardado;

        // Aplicar el sonido de verdad
        SetMasterVol(volMasterGuardado);
        SetMusicaVol(volMusicaGuardado);
        SetVFXVol(volVFXGuardado);

        // 2. Cargar Pantalla Completa 
        int pantallaCompletaGuardada = PlayerPrefs.GetInt("PantallaCompleta", 1);
        bool esPantallaCompleta = (pantallaCompletaGuardada == 1);
        
        // Colocar visualmente el tick en el menú
        if (togglePantallaCompleta != null) togglePantallaCompleta.isOn = esPantallaCompleta;
        SetPantallaCompleta(esPantallaCompleta);

        // 3. Cargar Resolución 
        int resolucionGuardada = PlayerPrefs.GetInt("ResolucionIndex", resolucionActualIndex);
        resolucionDropdown.value = resolucionGuardada;
        resolucionDropdown.RefreshShownValue();
        // ---------------------------------------------------
    }

    public void SetMasterVol(float volumen)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(volumen) * 20);
        // --- Guardar en memoria ---
        PlayerPrefs.SetFloat("MasterVol", volumen);
        PlayerPrefs.Save();
    }

    public void SetMusicaVol(float volumen)
    {
        audioMixer.SetFloat("MusicaVol", Mathf.Log10(volumen) * 20);
        // --- Guardar en memoria ---
        PlayerPrefs.SetFloat("MusicaVol", volumen);
        PlayerPrefs.Save();
    }

    public void SetVFXVol(float volumen)
    {
        audioMixer.SetFloat("VFXVol", Mathf.Log10(volumen) * 20);
        // --- NUEVO: Guardar en memoria ---
        PlayerPrefs.SetFloat("VFXVol", volumen);
        PlayerPrefs.Save();
    }

    public void SetPantallaCompleta(bool estaActivado)
    {
        if (estaActivado)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.fullScreen = false;
        }
        
        // --- Guardar en memoria (1 si es true, 0 si es false) ---
        PlayerPrefs.SetInt("PantallaCompleta", estaActivado ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetResolucion(int indexResolucion)
    {
        Resolution res = resoluciones[indexResolucion];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
        Debug.Log("Resolución: " + res.width + "x" + res.height);
        
        // --- NUEVO: Guardar en memoria ---
        PlayerPrefs.SetInt("ResolucionIndex", indexResolucion);
        PlayerPrefs.Save();
    }
}
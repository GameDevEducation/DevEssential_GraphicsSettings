using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicsConfigUI : MonoBehaviour
{
    [SerializeField] TMP_Dropdown QualityLevelDropdown;
    [SerializeField] TMP_Dropdown ResolutionDropdown;
    [SerializeField] TextMeshProUGUI PixelLightCountLabel;
    [SerializeField] Slider PixelLightCountSlider;

    Resolution[] CachedResolutions;
    Resolution Default_Resolution;

    int Default_QualityLevel;
    int Default_ResolutionIndex;
    int Default_PixelLightCount;

    // Start is called before the first frame update
    void Start()
    {
        Default_QualityLevel = QualitySettings.GetQualityLevel();

        // add in the quality level options
        QualityLevelDropdown.ClearOptions();
        QualityLevelDropdown.AddOptions(new List<string>(QualitySettings.names));
        QualityLevelDropdown.SetValueWithoutNotify(Default_QualityLevel);

        // build up the resolution data
        CachedResolutions = Screen.resolutions;
        List<string> resolutionStrings = new List<string>();
        foreach(var resolution in CachedResolutions)
        {
            var resolutionString = resolution.width + "x" + resolution.height + " @" + resolution.refreshRate;
            resolutionStrings.Add(resolutionString);
        }

        var currentResString = Screen.currentResolution.width + "x" + Screen.currentResolution.height + " @" + Screen.currentResolution.refreshRate;
        Default_ResolutionIndex = resolutionStrings.IndexOf(currentResString);

        // if the current resolution isn't stored - add it
        if (Default_ResolutionIndex < 0)
        {
            resolutionStrings.Add(currentResString);
            Default_ResolutionIndex = resolutionStrings.Count - 1;
            Default_Resolution = Screen.currentResolution;
        }
        else
            Default_Resolution = CachedResolutions[Default_ResolutionIndex];

        // add in the resolution options
        ResolutionDropdown.ClearOptions();
        ResolutionDropdown.AddOptions(resolutionStrings);
        ResolutionDropdown.SetValueWithoutNotify(Default_ResolutionIndex);

        // update the UI for the pixel light count
        Default_PixelLightCount = QualitySettings.pixelLightCount;
        PixelLightCountSlider.SetValueWithoutNotify(Default_PixelLightCount);
        PixelLightCountLabel.text = "Pixel Light Count [" + QualitySettings.pixelLightCount + "]";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnQualityLevelChanged(int newLevel)
    {
        QualitySettings.SetQualityLevel(newLevel);

        // re-fetch the pixel light count setting as it will be different due to the change in quality settings
        Default_PixelLightCount = QualitySettings.pixelLightCount;
        PixelLightCountSlider.SetValueWithoutNotify(Default_PixelLightCount);
        PixelLightCountLabel.text = "Pixel Light Count [" + QualitySettings.pixelLightCount + "]";
    }

    public void OnResolutionChanged(int newResolution)
    {
        // is the new resolution the default one that was not part of the main list?
        if (newResolution >= CachedResolutions.Length)
            SetResolution(Default_Resolution);
        else
            SetResolution(CachedResolutions[newResolution]);
    }

    void SetResolution(Resolution newResolution)
    {
        Screen.SetResolution(newResolution.width, newResolution.height, true, newResolution.refreshRate);
    }

    public void OnPixelLightCountChanged(float newNumber)
    {
        QualitySettings.pixelLightCount = Mathf.RoundToInt(newNumber);
        PixelLightCountLabel.text = "Pixel Light Count [" + QualitySettings.pixelLightCount + "]";
    }

    public void OnApply()
    {

    }

    public void OnReset()
    {
        // reset quality settings first as they may impact other values
        QualitySettings.SetQualityLevel(Default_QualityLevel);
        Default_PixelLightCount = QualitySettings.pixelLightCount;
        QualityLevelDropdown.SetValueWithoutNotify(Default_QualityLevel);

        // reset the resolution
        SetResolution(Default_Resolution);
        ResolutionDropdown.SetValueWithoutNotify(Default_ResolutionIndex);

        // pixel light setting is only fetched rather than re-applied as 
        // it will have a new default due to the quality setting change.
        QualitySettings.pixelLightCount = Default_PixelLightCount;
        PixelLightCountSlider.SetValueWithoutNotify(Default_PixelLightCount);
        PixelLightCountLabel.text = "Pixel Light Count [" + QualitySettings.pixelLightCount + "]";
    }
}

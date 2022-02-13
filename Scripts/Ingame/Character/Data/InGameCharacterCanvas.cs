using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class InGameCharacterCanvas : NetworkBehaviour
{
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private Slider energySlider;

    [SerializeField] private Slider attachedHealthSlider;
    [SerializeField] private Slider canvasHealthSlider;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        playerCanvas.enabled = true;
    }

    public void updateEnergy(float value, bool isInterpolate) {
        float normalizedValue = value / 100f;

        if (!isInterpolate)
        {
            energySlider.value = normalizedValue;
            return;
        }
        energySlider.value = Mathf.Lerp(energySlider.value, normalizedValue, 1.5f);
    }

    public void updateHealth(float value)
    {
        float normalizedValue = value / 100f;
        attachedHealthSlider.value = normalizedValue;
        canvasHealthSlider.value = normalizedValue;
    }

    private void OnDestroy() {
        playerCanvas.enabled = false;
    }
}

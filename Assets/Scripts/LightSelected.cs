using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSelected : MonoBehaviour
{
    [SerializeField] private GameObject selectedVisual;

    public void ToggleSelectedVisual()
    {
        selectedVisual.SetActive(!selectedVisual.activeInHierarchy);
    }
}

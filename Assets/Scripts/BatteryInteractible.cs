using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryInteractible : MonoBehaviour, IInteractible
{
    public void Interact()
    {
        Destroy(this.gameObject);
    }
}

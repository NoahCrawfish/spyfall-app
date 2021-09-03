using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCustomLocationController : MonoBehaviour
{
    public void CallAddCustomLocation() {
        transform.parent.parent.GetComponent<CustomLocationSetController>().AddCustomLocation();
    }
}

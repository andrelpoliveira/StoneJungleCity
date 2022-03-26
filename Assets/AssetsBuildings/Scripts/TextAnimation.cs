using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    public TextMesh     production, shadow;

    // Start is called before the first frame update
    void Start()
    {
        production.GetComponent<Renderer>().sortingLayerName = "HUD";
        production.GetComponent<Renderer>().sortingOrder = 99;

        shadow.GetComponent<Renderer>().sortingLayerName = "HUD";
        shadow.GetComponent<Renderer>().sortingOrder = 98;

        Destroy(this.gameObject, 0.5f);
    }

}

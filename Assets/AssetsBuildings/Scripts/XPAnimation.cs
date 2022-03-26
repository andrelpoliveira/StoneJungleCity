using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPAnimation : MonoBehaviour
{
    public float            posY;
    public Rigidbody2D      xpRb;
    public bool             isKick;


    // Start is called before the first frame update
    void Start()
    {
        xpRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < posY && isKick == false)
        {
            isKick = true;
            xpRb.velocity = Vector2.zero;
            xpRb.AddForce(new Vector2(35, 500));
            Destroy(this.gameObject, 1);
        }
    }
}

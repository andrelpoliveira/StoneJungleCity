using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemAnimation : MonoBehaviour
{
    public float            posY;
    public Rigidbody2D      gemRb;
    public bool             isKick;

    // Start is called before the first frame update
    void Start()
    {
        gemRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < posY && isKick == false)
        {
            isKick = true;
            gemRb.velocity = Vector2.zero;
            gemRb.AddForce(new Vector2(0, 600));
            Destroy(this.gameObject, 1);
        }
    }
}

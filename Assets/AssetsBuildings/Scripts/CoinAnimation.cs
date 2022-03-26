using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    public float        posY; // Posição Y no instantiate
    public Rigidbody2D  coinRb;
    private bool        isKick;

    // Start is called before the first frame update
    void Start()
    {
        coinRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < posY && isKick == false)
        {
            isKick = true;
            coinRb.velocity = Vector2.zero;
            coinRb.AddForce(new Vector2(-35, 300));
            Destroy(this.gameObject, 1);
        }
    }
}

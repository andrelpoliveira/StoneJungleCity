using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


public class MonetizationManager : MonoBehaviour
{
    [HideInInspector]
    public GameController       _GameController;

    void Start()
    {
        _GameController = FindObjectOfType(typeof(GameController)) as GameController;
        DontDestroyOnLoad(this);
    }
    
    public void OnPurchaseComplete(Product product)
    {
        if(product.definition.id.Equals("gemspack1"))
        {
            _GameController.getGems(20);
        }
        if(product.definition.id.Equals("gemspack2"))
        {
            _GameController.getGems(50);
        }
        if(product.definition.id.Equals("gemspack3"))
        {
            _GameController.getGems(100);
        }
        if(product.definition.id.Equals("coinspack1"))
        {
            _GameController.getCoin(100000);
        }
        if (product.definition.id.Equals("coinspack2"))
        {
            _GameController.getCoin(500000);
        }
        if (product.definition.id.Equals("coinspack3"))
        {
            _GameController.getCoin(1000000);
        }
        if (product.definition.id.Equals("packsurprise"))
        {
            _GameController.getCoin(2000000);
            _GameController.getGems(120);
        }
    }
}

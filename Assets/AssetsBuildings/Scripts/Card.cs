using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Collection/Card")]
public class Card : ScriptableObject
{
    public int      idCard;
    public string   cardName;
    public Sprite   spriteCard;
    public Sprite   shadowCard;
    public Rarity   rarityCard;

    public bool     isLiberate;
    public int      levelCard = 1;
    public double   production;
    public float    timeProduction;

    public int      productionMultiplier = 1;
    public float    productionReduction = 1;

    public bool     isMax;

    public void reset()
    {
        if(idCard == 0) { isLiberate = true; } else { isLiberate = false; }
        levelCard = 1;
        productionMultiplier = 1;
        productionReduction = 1;
        isMax = false;
    }


}

using UnityEngine;

[CreateAssetMenu(fileName = "New Slot", menuName = "Collection/Slot")]

public class Slots : ScriptableObject
{
    [HideInInspector]
    public GameController   _GameController;
    public int              idSlot;

    public Card             slotCard;
    public Card             initialCard;

    public bool             isPurchased;
    public bool             isMax;
    public bool             isAutoProduction;

    public double           slotPrice;
    public double           slotProduction;
    public int              xpProduction;
    public float            slotTimeProduction;
    public int              slotLevel = 1;
    public int              upgrades;
    public int              totalUpgrades;

    public double           upgradePrice;

    public int              slotProductionMultiplier = 1;
    public float            slotProductionReduction = 1;

    public void reset() 
    {
        if(idSlot == 0) { isPurchased = true; } else { isPurchased = false; }
        slotLevel = 1;
        upgrades = 0;
        totalUpgrades = 0;
        slotCard = initialCard;
        slotProductionMultiplier = 1;
        slotProductionReduction = 1;
        isAutoProduction = false;

    }

    public void StartSlotsScriptable()
    {
        int mult = 1;

        if(totalUpgrades >0) { mult = totalUpgrades; }

        slotProduction = slotCard.production * slotCard.productionMultiplier * slotProductionMultiplier * mult * _GameController.multiplierBonus * _GameController.multiplierBonusTemp;
        slotTimeProduction = slotCard.timeProduction / slotCard.productionReduction / slotProductionReduction / _GameController.reductionBonus / _GameController.reductionBonusTemp;
        //Upgrade Price Cálculo
        upgradePrice = slotProduction * slotProductionMultiplier * 1.5f;

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState
{
    GAMEPLAY, COLLECTION, UPGRADE, CUT, BOOSTER
}

public enum Rarity
{
    COMMOM, RARE, EPIC, LEGEND
}


public class GameController : MonoBehaviour
{
    public bool             isReset;
    public bool             is_organization;

    [SerializeField]
    private SlotController[] _SlotController;
    private SlotController  slotcontrol;

    [Header("Gerenciamento HUD")]
    public Sprite           ico_gem;
    public Sprite[]         icoCoin; //0 - Moeda Inativa, 1 - Moeda Ativa
    public Sprite[]         slotBg;  //0 - Inativo, 1 - Ativo
    public Sprite[]         bgUpgrade; // 0- inativo, 1 Ativo, 2 Maximizado
    public Color[]          colorText; // 0 - inativa, 1 - ativa
    public Sprite[]         bg_card;    //0 - COMUM, 1 - RARA, 2 - �PICO, 3 - LEND�RIA, 4 - N�O TEM

    public CardCollection[] slot_collection;


    [Space]
    [Header("HUD Gameplay")]
    public GameObject       panelGamePlay;
    public GameObject       panelFume;
    public TextMeshProUGUI  coinTxt;
    public TextMeshProUGUI  gemsTxt;
    public TextMeshProUGUI  xpTxt;
    public Image            barXp;
    public TextMeshProUGUI  lvlTxt;
    public int[]            xpMax;

    public GameObject       panelQuest;
    public TextMeshProUGUI  questTxt;
    public bool             isQuest;    //Booleana para verificar se est� em quest
    public int              idQuest;     //Indice da quest atual
    [TextArea]
    public string[]         questDescription;   //Descri��o da Quest

    [Space]
    [Header("CUT Compra")]
    public GameObject       panelBuy;
    public TextMeshProUGUI  buyDescriptiontxt;
    public Image            icoBuild;

    [Space]
    [Header("HUD Booster")]
    public GameObject       panel_open_booster;
    public OpenSuitCase     _OpenSuitCase;
    public TMP_Text         booster_price_txt;

    [Space]
    [Header("HUD Bot�es")]
    public GameObject       btnUpgrade;
    public GameObject       btnCollections;
    public GameObject       panel_collection;

    [Space]
    [Header("Scriptables")]
    public Card[]           cards;
    public Slots[]          slots;

    [HideInInspector]
    public List<Card> card_common;
    [HideInInspector]
    public List<Card> card_rare;
    [HideInInspector]
    public List<Card> card_epic;
    [HideInInspector]
    public List<Card> card_legendary;

    [Space]
    [Header("Prefabs")]
    public GameObject       coinPrefab;
    public GameObject       textPrefab;
    public GameObject       xpPrefab;
    public GameObject       textxpPrefab;
    public GameObject       gemPrefab;
    public GameObject       textgemPrefab;

    [Space]
    [Header("Vari�veis GamePlay")]
    public GameState        currentState;
    [SerializeField]
    private double          coins, coinsAccumulated;
    private int             gems, gemsAccumulated;
    [SerializeField]private int             xp, xpAccumulated;
    private float           fillAmountXp;
    public int              level;
    public string[]         accumulated;
    public int[]            progressSlot;
    public float            delayLoopUpgrade, delayBetweenUpgrade;
    public int[]            progress_card;
    public double[]         max_reward_rarity;
    public int              qtd_suit_common;
    public double           suit_price;
    public int[]            suit_price_gems;

    [Space]
    [Header("B�nus de GamePlay")]
    public int              multiplierBonus;
    public int              multiplierBonusTemp;
    public float            reductionBonus;
    public float            reductionBonusTemp;


    // Start is called before the first frame update
    void Start()
    {
        if(isReset == true) { resete(); }

        barXp.fillAmount = 0;
        panelFume.SetActive(false);
        panelQuest.SetActive(false);
        panel_collection.SetActive(false);
        panel_open_booster.SetActive(false);
        _OpenSuitCase._GameController = this;

        // Teste se est� em quest
        if (isQuest == true) { panelQuest.SetActive(true); }

        //Busca do objeto
        _SlotController = FindObjectsOfType(typeof(SlotController)) as SlotController[];
        slotcontrol = FindObjectOfType(typeof(SlotController)) as SlotController;
        foreach(SlotController s in _SlotController)
        {
            s._GameController = this;
            s._Slots._GameController = this;

            s.StartSlot();
        }

        foreach (Card c in cards)
        {
            switch (c.rarityCard)
            {
                case Rarity.COMMOM:
                    card_common.Add(c);
                    break;

                case Rarity.RARE:
                    card_rare.Add(c);
                    break;

                case Rarity.EPIC:
                    card_epic.Add(c);
                    break;

                case Rarity.LEGEND:
                    card_legendary.Add(c);
                    break;
            }
        }

        booster_price_txt.text = currencyConverterCoin(suit_price);
    }

    public void getCoin(double qtdCoin)
    {
        coins += qtdCoin;
        if(qtdCoin > 0) { coinsAccumulated += qtdCoin; }
        coinTxt.text = currencyConverterCoin(coins);
    }

    public void getGems(int qtdGems)
    {
        gems += qtdGems;
        if(gems > 0) { gemsAccumulated += qtdGems; }
        gemsTxt.text = currencyConverterGem(gems);
    }

    public void getXp(int qtdXp)
    {
        xp += qtdXp;
        if(qtdXp > 0) { xpAccumulated += qtdXp; }
        if(xp < xpMax[level]) //Levels
        {
            Debug.Log(level);
            xpTxt.text = currencyConverterXP(xp) + "/" + xpMax[level].ToString();
            if(xpAccumulated <= 0.1f)
            {
                fillAmountXp = 1;
            }
            else
            {
                fillAmountXp = (float)xp / xpMax[level];
            }
        }
        if (xp >= xpMax[level])
        {
            Debug.Log("Update");
            xp = xp - xpMax[level];
            updateLevel();
            
            if (xpAccumulated <= 0.1f)
            {
                fillAmountXp = 1;
            }
            else
            {
                fillAmountXp = (float)xp / xpMax[level];
            }
            
        }
        
        barXp.fillAmount = fillAmountXp;
    }

    public string currencyConverterCoin(double valor)
    {
        string r = "";
        string valorTemp = "";
        double temp = 0;

        if (valor >= 1e+18D)
        {
            temp = valor / 1e+18D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "QQ";
        }
        if (valor >= 1e+15D)
        {
            temp = valor / 1e+15D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "Q";
        }
        if (valor >= 1e+12D)
        {
            temp = valor / 1e+12D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "T";
        }
        if (valor >= 1e+9D)
        {
            temp = valor / 1e+9D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "B";
        }
        if (valor >= 1e+6D)
        {
            temp = valor / 1e+6D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "M";
        }
        if (valor >= 1e+3D)
        {
            temp = valor / 1e+3D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "K";
        }
        else
        {
            r = valor.ToString("N0");
        }


        return r;
    }
    public string currencyConverterXP(int valor)
    {
        return valor.ToString();
    }
    public string currencyConverterGem(int valor)
    {
        return valor.ToString();
    }

    private string removeZero(string valor)
    {
        string r = "";
        accumulated = valor.Split(',');
        if(accumulated.Length == 1)
        {
            accumulated = valor.Split('.');
        }

        if(accumulated[1] != "0")
        {
            r = accumulated[0] + "." + accumulated[1];
        }
        else
        {
            r = accumulated[0];
        }

        return r;
    }

    void updateLevel()
    {
        level += 1;
        barXp.fillAmount = fillAmountXp;
        xpTxt.text = currencyConverterXP(xp) + "/" + xpMax[level].ToString();
        lvlTxt.text = level.ToString();
        if (level == 3)
        {
            slotcontrol.upgradeStatus(0);
            getGems(5);
            slotcontrol.gemCollect();
        }
        else if (level == 8)
        {
            slotcontrol.upgradeStatus(1);
            getGems(15);
            slotcontrol.gemCollect();

        }
    }
    private void resete()
    {
        if(coins <= 0)
        {
            coins = 0;
            xp = 0;
            gems = 0;
            coinsAccumulated = 0;
            xpAccumulated = 0;
            gemsAccumulated = 0;
        }
        foreach(Card c in cards)
        {
            c.reset();
        }

        foreach(Slots s in slots)
        {
            s.reset();
        }
        print("resete");
    }

    public bool checkCoin(double qtd)
    {
        bool check = false;

        if(coins >= qtd) { check = true; }

        return check;
    }

    public bool checkGems(int qtd)
    {
        bool check = false;

        if (gems >= qtd) { check = true; }

        return check;
    }

    public void changeGameState(GameState newState)
    {
        currentState = newState;
        foreach(SlotController s in _SlotController)
        {
            s.checkGameState();
        }
    }

    public void upgradeMode()
    {
        switch(currentState)
        {
            case GameState.GAMEPLAY:
                changeGameState(GameState.UPGRADE);
                panelFume.SetActive(true);
                break;

            case GameState.UPGRADE:
                changeGameState(GameState.GAMEPLAY);
                panelFume.SetActive(false);
                break;
        }

        foreach (SlotController s in _SlotController)
        {
            s.upgradeModeSlot();
        }
    }

    public void BuySlot(Slots s, SlotController sc)
    {
        changeGameState(GameState.CUT);
        panelGamePlay.SetActive(false);
        panelFume.SetActive(true);

        icoBuild.sprite = s.slotCard.spriteCard;
        buyDescriptiontxt.text = "Liberou <color=#00FFFF>" + s.slotCard.cardName + "</color>";

        panelBuy.SetActive(true);

        getCoin(s.slotPrice * -1);
        
        s.isPurchased = true;
        s.slotCard.isLiberate = true;
        s.StartSlotsScriptable();

        sc.StartSlot();
    }
    public void CloseCut()
    {
        changeGameState(GameState.GAMEPLAY);
        panelBuy.SetActive(false);
        panelFume.SetActive(false);
        panelGamePlay.SetActive(true);
    }

    public void GetBooster(int id_booster)
    {
        switch (id_booster)
        {
            case 0: // Maleta Comun
                if (!checkCoin(suit_price)) { return; }

                getCoin(suit_price * -1);
                _OpenSuitCase.suit_rarity = Rarity.COMMOM;
                _OpenSuitCase.qtd_rewards = 3;
                _OpenSuitCase.Start();
                qtd_suit_common += 1;
                suit_price = (suit_price * qtd_suit_common) + (float)coinsAccumulated / 2;
                booster_price_txt.text = currencyConverterCoin(suit_price);
                break;

            case 1: // Maleta Rara
                if (!checkGems(suit_price_gems[0])) { return; }

                getGems(suit_price_gems[0] * -1);
                _OpenSuitCase.suit_rarity = Rarity.RARE;
                _OpenSuitCase.qtd_rewards = 5;
                _OpenSuitCase.Start();
                break;

            case 2: // Maleta �pica
                if (!checkGems(suit_price_gems[1])) { return; }

                getGems(suit_price_gems[1] * -1);
                _OpenSuitCase.suit_rarity = Rarity.EPIC;
                _OpenSuitCase.qtd_rewards = 7;
                _OpenSuitCase.Start();
                break;

            case 3: // Maleta Lend�ria
                if (!checkGems(suit_price_gems[2])) { return; }

                getGems(suit_price_gems[2] * -1);
                _OpenSuitCase.suit_rarity = Rarity.LEGEND;
                _OpenSuitCase.qtd_rewards = 10;
                _OpenSuitCase.Start();
                break;
        }

        changeGameState(GameState.BOOSTER);
        panel_open_booster.SetActive(true);
    }

    public void OpenCollection()
    {
        switch (currentState)
        {
            case GameState.GAMEPLAY:
                UpgradeCollection();
                changeGameState(GameState.COLLECTION);
                panelFume.SetActive(true);
                panel_collection.SetActive(true);
                break;

            case GameState.COLLECTION:
                changeGameState(GameState.GAMEPLAY);
                panelFume.SetActive(false);
                panel_collection.SetActive(false);
                break;
        }
    }

    void UpgradeCollection()
    {
        int i = 0;

        if (!is_organization)
        {

            foreach (Card c in cards)
            {
                if (slot_collection[i]._GameController == null) { slot_collection[i]._GameController = this; }
                if (slot_collection[i]._Card == null) { slot_collection[i]._Card = c; }

                slot_collection[i].UpgradeInfoCard();
                slot_collection[i].gameObject.SetActive(true);
                i++;
            }
        }
        else // Organiza cartas por raridade, precisa ativar
        {
            foreach (Card c in card_common)
            {
                if (slot_collection[i]._GameController == null) { slot_collection[i]._GameController = this; }
                if (slot_collection[i]._Card == null) { slot_collection[i]._Card = c; }

                slot_collection[i].UpgradeInfoCard();
                slot_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_rare)
            {
                if (slot_collection[i]._GameController == null) { slot_collection[i]._GameController = this; }
                if (slot_collection[i]._Card == null) { slot_collection[i]._Card = c; }

                slot_collection[i].UpgradeInfoCard();
                slot_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_epic)
            {
                if (slot_collection[i]._GameController == null) { slot_collection[i]._GameController = this; }
                if (slot_collection[i]._Card == null) { slot_collection[i]._Card = c; }

                slot_collection[i].UpgradeInfoCard();
                slot_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_legendary)
            {
                if (slot_collection[i]._GameController == null) { slot_collection[i]._GameController = this; }
                if (slot_collection[i]._Card == null) { slot_collection[i]._Card = c; }

                slot_collection[i].UpgradeInfoCard();
                slot_collection[i].gameObject.SetActive(true);
                i++;
            }
        }
    }

    public double getCoinAccumulated()
    {
        return coinsAccumulated;
    }

    public double getGemsAccumulated()
    {
        return gemsAccumulated;
    }

    public void getCard(Card c, int qtd)
    {
        if(!c.isLiberate) { c.isLiberate = true; }

        if (!c.isMax)
        {
            c.card_collected += qtd;
            
            if(c.card_collected >= progress_card[c.levelCard - 1])
            {
                int dif = c.card_collected - progress_card[c.levelCard - 1];
                c.card_collected = dif;
                c.levelCard += 1;

                switch (c.levelCard)
                {
                    case 2:
                        c.productionMultiplier = 5;
                        break;

                    case 3:
                        c.productionReduction = 5;
                        c.isMax = true;

                        if(dif > 0) { MaxRewardRarity(c.rarityCard); }
                        
                        c.card_collected = 0;
                        break;
                }

                UpdateSlot();
            }
        }
        else
        {
            // Recompensa por carta maximizada
            MaxRewardRarity(c.rarityCard);
        }
        
        UpgradeCollection();
    }

    void MaxRewardRarity(Rarity r)
    {
        switch (r)
        {
            case Rarity.COMMOM:
                getCoin(max_reward_rarity[0]);
                break;

            case Rarity.RARE:
                getCoin(max_reward_rarity[1]);
                break;

            case Rarity.EPIC:
                getCoin(max_reward_rarity[2]);
                break;

            case Rarity.LEGEND:
                getCoin(max_reward_rarity[3]);
                break;
        }
    }
    
    void UpdateSlot()
    {
        foreach (Slots s in slots)
        {
            if(s._GameController == null) { s._GameController = this; }

            s.StartSlotsScriptable();
        }
    }
}

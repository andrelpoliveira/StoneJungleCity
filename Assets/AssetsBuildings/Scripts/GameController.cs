using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    GAMEPLAY, COLLECTION, UPGRADE, CUT
}

public enum Rarity
{
    COMMOM, RARE, EPIC, LEGEND
}


public class GameController : MonoBehaviour
{
    public bool             isReset;

    [SerializeField]
    private SlotController[] _SlotController;
    private SlotController  slotcontrol;

    [Header("Gerenciamento Sprites HUD")]
    public Sprite[]         icoCoin; //0 - Moeda Inativa, 1 - Moeda Ativa
    public Sprite[]         slotBg;  //0 - Inativo, 1 - Ativo
    public Sprite[]         bgUpgrade; // 0- inativo, 1 Ativo, 2 Maximizado
    public Color[]          colorText; // 0 - inativa, 1 - ativa

    [Header("HUD Gameplay")]
    public GameObject       panelFume;
    public Text             coinTxt;
    public Text             gemsTxt;
    public Text             xpTxt;
    public Image            barXp;
    public Text             lvlTxt;
    public int[]            xpMax;

    public GameObject       panelQuest;
    public Text             questTxt;
    public bool             isQuest;    //Booleana para verificar se est� em quest
    public int              idQuest;     //Indice da quest atual
    [TextArea]
    public string[]         questDescription;   //Descri��o da Quest


    [Header("HUD Bot�es")]
    public GameObject       btnUpgrade;
    public GameObject       btnCollections;

    [Header("Scriptables")]
    public Card[]           cards;
    public Slots[]          slots;

    [Header("Prefabs")]
    public GameObject       coinPrefab;
    public GameObject       textPrefab;
    public GameObject       xpPrefab;
    public GameObject       textxpPrefab;
    public GameObject       gemPrefab;
    public GameObject       textgemPrefab;

    //Vari�veis de GamePlay
    [Header("Vari�veis GamePlay")]
    public GameState        currentState;
    private double          coins, coinsAccumulated;
    private int             gems, gemsAccumulated;
    private int             xp, xpAccumulated;
    private float           fillAmountXp;
    public int              level;
    public string[]         accumulated;
    public int[]            progressSlot;
    public float            delayLoopUpgrade, delayBetweenUpgrade;


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
        else if (xp == xpMax[level])
        {
            xp = 0;
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
            r = accumulated[0] + "," + accumulated[1];
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
        coinsAccumulated = 0;
        xpAccumulated = 0;
        gemsAccumulated = 0;
        foreach(Card c in cards)
        {
            c.reset();
        }

        foreach(Slots s in slots)
        {
            s.reset();
        }
    }

    public bool checkCoin(double qtd)
    {
        bool check = false;

        if(coins >= qtd) { check = true; }

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

    public void BuySlot(Slots s)
    {

    }
}

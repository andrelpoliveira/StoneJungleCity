using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;

public enum GameState
{
    GAMEPLAY, COLLECTION, UPGRADE, CUT, BOOSTER, SHOPPING, MARKET, CHOOSE
}

public enum Rarity
{
    COMMOM, RARE, EPIC, LEGEND
}


public class GameController : MonoBehaviour
{
    private FirebaseFirestore db;
    private ListenerRegistration listenerRegistration;
    
    [HideInInspector]
    public string user;
    public bool isReset;
    public bool is_organization;

    [SerializeField]
    private SlotController[] _SlotController;
    private SlotController slotcontrol;
    private SlotController slot_choose;
    private LoginAuth log_auth;
    
    [Header("Gerenciamento de painel")]
    public GameObject panelGamePlay;
    public GameObject panelFume;
    public GameObject panelQuest;
    public GameObject panelBuy;
    public GameObject panel_reward;
    public GameObject panel_purchase;
    public GameObject panel_open_booster;
    public GameObject panel_market_game;
    public GameObject panel_collection;
    public GameObject panel_choose_card;

    [Space]
    [Header("Gerenciamento HUD")]
    public Sprite ico_gem;
    public Sprite ico_suit;
    public Sprite[] icoCoin; //0 - Moeda Inativa, 1 - Moeda Ativa
    public Sprite[] slotBg;  //0 - Inativo, 1 - Ativo
    public Sprite[] bgUpgrade; // 0- inativo, 1 Ativo, 2 Maximizado
    public Color[] colorText; // 0 - inativa, 1 - ativa
    public Sprite[] bg_card;    //0 - COMUM, 1 - RARA, 2 - ÉPICO, 3 - LENDÁRIA, 4 - NÃO TEM
    public CardCollection[] slot_collection;
    public Sprite[] constructions;

    [Space]
    [Header("HUD Gameplay")]
    public TextMeshProUGUI coinTxt;
    public TextMeshProUGUI gemsTxt;
    public TextMeshProUGUI xpTxt;
    public Image barXp;
    public TextMeshProUGUI lvlTxt;
    public int[] xpMax;
    public Sprite ico_construct;


    public TextMeshProUGUI questTxt;
    public bool isQuest;    //Booleana para verificar se está em quest
    public int idQuest;     //Indice da quest atual
    [TextArea]
    public string[] questDescription;   //Descrição da Quest
    public GameObject qtd_bags;
    public TMP_Text qtd_bags_txt;

    [Space]
    [Header("CUT Compra")]
    
    public TextMeshProUGUI buyDescriptiontxt;
    public Image icoBuild;

    [Space]
    [Header("CUT Reward")]
    
    public TextMeshProUGUI reward_description;
    public Image ico_reward;
    private string prev_window;

    [Space]
    [Header("HUD Booster")]
    
    public OpenSuitCase _OpenSuitCase;
    public TMP_Text booster_price_txt;
    public GameObject[] qtd_booster_bags;
    public TMP_Text[] qtd_booster_bags_txt;

    [Space]
    [Header("HUD Botões")]
    public GameObject btnUpgrade;
    public GameObject btn_market;
    

    [Space]
    [Header("Scriptables")]
    public Card[] cards;
    public Slots[] slots;

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
    public GameObject coinPrefab;
    public GameObject textPrefab;
    public GameObject xpPrefab;
    public GameObject textxpPrefab;
    public GameObject gemPrefab;
    public GameObject textgemPrefab;

    [Space]
    [Header("Variáveis GamePlay")]
    public GameState currentState;
    [SerializeField]
    private double coins, coinsAccumulated;
    private int gems, gemsAccumulated;
    [SerializeField] private int xp, xpAccumulated;
    private float fillAmountXp;
    public int level;
    public string[] accumulated;
    public int[] progressSlot;
    public float delayLoopUpgrade, delayBetweenUpgrade;
    public int[] progress_card;
    public double[] max_reward_rarity;
    public int qtd_suit_common;
    public double suit_price;
    public int[] suit_price_gems;
    public int[] suit_bags; // 0 - Comum 1 - rara 2 - épica 3 - lenadária 
    

    [Space]
    [Header("Bônus de GamePlay")]
    public int multiplierBonus;
    public int multiplierBonusTemp;
    public float reductionBonus;
    public float reductionBonusTemp;

    [HideInInspector]
    public SlotController slot_card;
    public CardCollection[] choose_collection;
    public CardCollection[] inventory_collection;

    [Space]
    [Header("Button Menu")]
    public Animator menuBar;
    public GameObject menuGame;
    [HideInInspector]
    public bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        log_auth = FindObjectOfType(typeof(LoginAuth)) as LoginAuth;
        user = log_auth.user;
        db = FirebaseFirestore.DefaultInstance;

        LoadGame();

        if (isReset == true) { resete(); }

        barXp.fillAmount = 0;
        panelFume.SetActive(false);
        panelQuest.SetActive(false);
        panel_market_game.SetActive(false);
        panel_open_booster.SetActive(false);
        panel_reward.SetActive(false);
        panel_purchase.SetActive(false);
        _OpenSuitCase._GameController = this;

        // Teste se está em quest
        if (isQuest == true)
        {
            panelQuest.SetActive(true);

            if (idQuest == 0)
            {
                btnUpgrade.SetActive(false);
                btn_market.SetActive(false);
                questTxt.text = questDescription[idQuest];
            }
        }

        //Busca do objeto
        _SlotController = FindObjectsOfType(typeof(SlotController)) as SlotController[];
        slotcontrol = FindObjectOfType(typeof(SlotController)) as SlotController;
        foreach (SlotController sc in _SlotController)
        {
            sc._GameController = this;
            sc._Slots._GameController = this;

            sc.StartSlot();
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

            LoadCard(c);

            foreach (Slots s in slots)
            {
                LoadSlot(s);
            }
        }

        booster_price_txt.text = currencyConverterCoin(suit_price);
        coinTxt.text = currencyConverterCoin(coins);
        gemsTxt.text = currencyConverterGem(gems);

        CheckBags();
        //getGems(100);
        //getCoin(1e+5);
    }

    void OnDestroy()
    {
        listenerRegistration.Stop();
    }

    public void getCoin(double qtdCoin)
    {
        coins += qtdCoin;
        if (qtdCoin > 0) { coinsAccumulated += qtdCoin; }
        coinTxt.text = currencyConverterCoin(coins);

        //Missão inicial
        if (isQuest && idQuest == 0 && coinsAccumulated >= 20)
        {
            UpDataQuest();
        }
        SaveGame();
    }

    public void getGems(int qtdGems)
    {
        gems += qtdGems;
        if (gems > 0) { gemsAccumulated += qtdGems; }
        gemsTxt.text = currencyConverterGem(gems);

        SaveGame();
    }

    public void getXp(int qtdXp)
    {
        xp += qtdXp;
        if (qtdXp > 0) { xpAccumulated += qtdXp; }
        if (xp < xpMax[level]) //Levels
        {
            xpTxt.text = currencyConverterXP(xp) + "/" + xpMax[level].ToString();
            if (xpAccumulated <= 0.1f)
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

        SaveGame();
    }

    public string currencyConverterCoin(double valor)
    {
        string r = "";
        string valorTemp = "";
        double temp = 0;

        if (valor >= 1e+30D)
        {
            temp = valor / 1e+30D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "ff";
        }
        else if (valor >= 1e+27D)
        {
            temp = valor / 1e+27D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "ee";
        }
        else if (valor >= 1e+24D)
        {
            temp = valor / 1e+24D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "dd";
        }
        else if (valor >= 1e+21D)
        {
            temp = valor / 1e+21D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "cc";
        }
        else if (valor >= 1e+18D)
        {
            temp = valor / 1e+18D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "bb";
        }
        else if (valor >= 1e+15D)
        {
            temp = valor / 1e+15D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "aa";
        }
        else if (valor >= 1e+12D)
        {
            temp = valor / 1e+12D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "T";
        }
        else if (valor >= 1e+9D)
        {
            temp = valor / 1e+9D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "B";
        }
        else if (valor >= 1e+6D)
        {
            temp = valor / 1e+6D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "M";
        }
        else if (valor >= 1e+3D)
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
        if (accumulated.Length == 1)
        {
            accumulated = valor.Split('.');
        }

        if (accumulated[1] != "0")
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
            OpenCutReward(ico_gem, "Voce Recebeu <color=#ffff00>5</color> gemas", "game");
            slotcontrol.gemCollect();
        }
        else if (level == 8)
        {
            slotcontrol.upgradeStatus(1);
            getGems(15);
            OpenCutReward(ico_gem, "Voce Recebeu <color=#ffff00>15</color> gemas", "game");
            slotcontrol.gemCollect();

        }
    }
    private void resete()
    {
        if (coins <= 0)
        {
            coins = 0;
            xp = 0;
            gems = 0;
            coinsAccumulated = 0;
            xpAccumulated = 0;
            gemsAccumulated = 0;
        }
        foreach (Card c in cards)
        {
            c.reset();
        }

        foreach (Slots s in slots)
        {
            s.reset();
        }
    }

    public bool checkCoin(double qtd)
    {
        bool check = false;

        if (coins >= qtd) { check = true; }

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
        foreach (SlotController s in _SlotController)
        {
            s.checkGameState();
        }
    }

    public void upgradeMode()
    {
        switch (currentState)
        {
            case GameState.GAMEPLAY:
                changeGameState(GameState.UPGRADE);
                panelFume.SetActive(true);
                break;

            case GameState.UPGRADE:
                changeGameState(GameState.GAMEPLAY);
                panelFume.SetActive(false);
                break;

            case GameState.CUT:
                changeGameState(GameState.UPGRADE);
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

        icoBuild.sprite = ico_construct;
        buyDescriptiontxt.text = "Liberou <color=#00FFFF>" + "Terreno" + "</color>";

        panelBuy.SetActive(true);

        getCoin(s.slotPrice * -1);

        s.isPurchased = true;
        s.slotCard.isLiberate = true;
        s.is_ground = true;
        s.StartSlotsScriptable();

        sc.StartSlot();
        sc.buildSprite.sprite = ico_construct;

        SaveSlot(s);
        // Missão inicial
        if (isQuest && idQuest == 2)
        {
            UpDataQuest();
        }
    }

    public void BuyCard(CardCollection card)
    {
        if (card._Card.rarityCard == Rarity.LEGEND)
        {
            if (checkGems(int.Parse(card.price_txt.text)))
            {
                getGems(int.Parse(card.price_txt.text) * -1);

                changeGameState(GameState.CUT);
                panelGamePlay.SetActive(false);
                panel_market_game.SetActive(false);
                panelFume.SetActive(true);

                icoBuild.sprite = card.sprite_card.sprite;
                buyDescriptiontxt.text = "Liberou <color=#00FFFF>" + card.card_name.text + "</color>";

                panelBuy.SetActive(true);

                getCard(card._Card, 1);
            }
        }
        else
        {
            if (checkCoin(card.price_doub))
            {
                getCoin(card.price_doub * -1);

                changeGameState(GameState.CUT);
                panelGamePlay.SetActive(false);
                panel_market_game.SetActive(false);
                panelFume.SetActive(true);

                icoBuild.sprite = card.sprite_card.sprite;
                buyDescriptiontxt.text = "Liberou <color=#00FFFF>" + card.card_name.text + "</color>";

                panelBuy.SetActive(true);

                getCard(card._Card, 1);
            }
        }
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
            case 0: // Maleta Comum
                if (suit_bags[id_booster] <= 0)
                {
                    if (!checkCoin(suit_price))
                    {
                        return;
                    }
                    else
                    {
                        getCoin(suit_price * -1);
                        qtd_suit_common += 1;
                        suit_price = (suit_price * qtd_suit_common) + (float)coinsAccumulated / 2;
                        booster_price_txt.text = currencyConverterCoin(suit_price);
                    }
                }
                else
                {
                    suit_bags[id_booster] -= 1;
                }

                _OpenSuitCase.suit_rarity = Rarity.COMMOM;
                _OpenSuitCase.qtd_rewards = 3;
                _OpenSuitCase.Start();


                if (isQuest && idQuest == 3)
                {
                    UpDataQuest();
                }
                break;

            case 1: // Maleta Rara
                if (suit_bags[id_booster] <= 0)
                {
                    if (!checkGems(suit_price_gems[id_booster]))
                    {
                        return;
                    }
                    else
                    {
                        getGems(suit_price_gems[id_booster] * -1);
                    }
                }
                else
                {
                    suit_bags[id_booster] -= 1;
                }

                _OpenSuitCase.suit_rarity = Rarity.RARE;
                _OpenSuitCase.qtd_rewards = 5;
                _OpenSuitCase.Start();
                break;

            case 2: // Maleta Épica
                if (suit_bags[id_booster] <= 0)
                {
                    if (!checkGems(suit_price_gems[id_booster]))
                    {
                        return;
                    }
                    else
                    {
                        getGems(suit_price_gems[id_booster] * -1);
                    }
                }
                else
                {
                    suit_bags[id_booster] -= 1;
                }

                _OpenSuitCase.suit_rarity = Rarity.EPIC;
                _OpenSuitCase.qtd_rewards = 7;
                _OpenSuitCase.Start();
                break;

            case 3: // Maleta Lendária
                if (suit_bags[id_booster] <= 0)
                {
                    if (!checkGems(suit_price_gems[id_booster]))
                    {
                        return;
                    }
                    else
                    {
                        getGems(suit_price_gems[id_booster] * -1);
                    }
                }
                else
                {
                    suit_bags[id_booster] -= 1;
                }

                _OpenSuitCase.suit_rarity = Rarity.LEGEND;
                _OpenSuitCase.qtd_rewards = 10;
                _OpenSuitCase.Start();
                break;
        }

        CheckBags();
        changeGameState(GameState.BOOSTER);
        panel_open_booster.SetActive(true);
    }

    public void OpenMarketGame ()
    {
        CheckBags();

        switch (currentState)
        {
            case GameState.GAMEPLAY:
                UpgradeMarketGame();
                changeGameState(GameState.MARKET);
                panelFume.SetActive(true);
                panel_market_game.SetActive(true);
                break;

            case GameState.MARKET:
                changeGameState(GameState.GAMEPLAY);
                panelFume.SetActive(false);
                panel_market_game.SetActive(false);
                break;
        }
    }

    void UpgradeMarketGame()
    {
        int i = 0;

        if (!is_organization)
        {

            foreach (Card c in cards)
            {
                if (slot_collection[i]._GameController == null) { slot_collection[i]._GameController = this; }
                if (slot_collection[i]._Card == null) { slot_collection[i]._Card = c; }

                slot_collection[i].UpgradeInfoCard(i, "collection");
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

                slot_collection[i].UpgradeInfoCard(i, "collection");
                slot_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_rare)
            {
                if (slot_collection[i]._GameController == null) { slot_collection[i]._GameController = this; }
                if (slot_collection[i]._Card == null) { slot_collection[i]._Card = c; }

                slot_collection[i].UpgradeInfoCard(i, "collection");
                slot_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_epic)
            {
                if (slot_collection[i]._GameController == null) { slot_collection[i]._GameController = this; }
                if (slot_collection[i]._Card == null) { slot_collection[i]._Card = c; }

                slot_collection[i].UpgradeInfoCard(i, "collection");
                slot_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_legendary)
            {
                if (slot_collection[i]._GameController == null) { slot_collection[i]._GameController = this; }
                if (slot_collection[i]._Card == null) { slot_collection[i]._Card = c; }

                slot_collection[i].UpgradeInfoCard(i, "collection");
                slot_collection[i].gameObject.SetActive(true);
                i++;
            }
        }
    }

    void UpgradeInventory()
    {
        int i = 0;

        if (!is_organization)
        {

            foreach (Card c in cards)
            {
                if (inventory_collection[i]._GameController == null) { inventory_collection[i]._GameController = this; }
                if (inventory_collection[i]._Card == null) { inventory_collection[i]._Card = c; }

                inventory_collection[i].UpgradeInfoCard(i, "inventory");
                inventory_collection[i].gameObject.SetActive(true);
                i++;
            }
        }
        else // Organiza cartas por raridade, precisa ativar
        {
            foreach (Card c in card_common)
            {
                if (inventory_collection[i]._GameController == null) { inventory_collection[i]._GameController = this; }
                if (inventory_collection[i]._Card == null) { inventory_collection[i]._Card = c; }

                inventory_collection[i].UpgradeInfoCard(i, "inventory");
                inventory_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_rare)
            {
                if (inventory_collection[i]._GameController == null) { inventory_collection[i]._GameController = this; }
                if (inventory_collection[i]._Card == null) { inventory_collection[i]._Card = c; }

                inventory_collection[i].UpgradeInfoCard(i, "inventory");
                inventory_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_epic)
            {
                if (inventory_collection[i]._GameController == null) { inventory_collection[i]._GameController = this; }
                if (inventory_collection[i]._Card == null) { inventory_collection[i]._Card = c; }

                inventory_collection[i].UpgradeInfoCard(i, "inventory");
                inventory_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_legendary)
            {
                if (inventory_collection[i]._GameController == null) { inventory_collection[i]._GameController = this; }
                if (inventory_collection[i]._Card == null) { inventory_collection[i]._Card = c; }

                inventory_collection[i].UpgradeInfoCard(i, "inventory");
                inventory_collection[i].gameObject.SetActive(true);
                i++;
            }
        }
    }

    void UpgradeChoose()
    {
        int i = 0;

        if (!is_organization)
        {

            foreach (Card c in cards)
            {
                if (choose_collection[i]._GameController == null) { choose_collection[i]._GameController = this; }
                if (choose_collection[i]._Card == null) { choose_collection[i]._Card = c; }

                choose_collection[i].UpgradeInfoCard(i, "inventory");
                choose_collection[i].gameObject.SetActive(true);
                i++;
            }
        }
        else // Organiza cartas por raridade, precisa ativar
        {
            foreach (Card c in card_common)
            {
                if (choose_collection[i]._GameController == null) { choose_collection[i]._GameController = this; }
                if (choose_collection[i]._Card == null) { choose_collection[i]._Card = c; }

                choose_collection[i].UpgradeInfoCard(i, "inventory");
                choose_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_rare)
            {
                if (choose_collection[i]._GameController == null) { choose_collection[i]._GameController = this; }
                if (choose_collection[i]._Card == null) { choose_collection[i]._Card = c; }

                choose_collection[i].UpgradeInfoCard(i, "inventory");
                choose_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_epic)
            {
                if (choose_collection[i]._GameController == null) { choose_collection[i]._GameController = this; }
                if (choose_collection[i]._Card == null) { choose_collection[i]._Card = c; }

                choose_collection[i].UpgradeInfoCard(i, "inventory");
                choose_collection[i].gameObject.SetActive(true);
                i++;
            }

            foreach (Card c in card_legendary)
            {
                if (choose_collection[i]._GameController == null) { choose_collection[i]._GameController = this; }
                if (choose_collection[i]._Card == null) { choose_collection[i]._Card = c; }

                choose_collection[i].UpgradeInfoCard(i, "inventory");
                choose_collection[i].gameObject.SetActive(true);
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
                OpenCutReward(c.spriteCard, "<color=#ffff00>" + c.cardName + "</color> subiu de level", "booster");
            }
        }
        else
        {
            // Recompensa por carta maximizada
            MaxRewardRarity(c.rarityCard);
        }
        
        UpgradeMarketGame();
        SaveCard(c);
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

    public void UpDataQuest()
    {
        idQuest += 1;

        switch (idQuest)
        {
            case 1:
                btnUpgrade.SetActive(true);
                break;

            case 3:
                btn_market.SetActive(true);
                break;
        }

        if(idQuest < questDescription.Length)
        {
            questTxt.text = questDescription[idQuest];
        }
        else
        {
            panelQuest.SetActive(false);
            isQuest = false;
        }

        SaveGame();
    }

    public void OpenCutReward(Sprite ico, string txt, string prev)
    {
        prev_window = prev;
        ico_reward.sprite = ico;
        reward_description.text = txt;
        prev_window = prev;

        panelGamePlay.SetActive(false);
        panelFume.SetActive(true);
        panel_reward.SetActive(true);

        changeGameState(GameState.CUT);
    }

    public void CloseCutReward()
    {
        panel_reward.SetActive(false);

        switch (prev_window)
        {
            case "upgrade":
                panelGamePlay.SetActive(true);
                changeGameState(GameState.GAMEPLAY);
                upgradeMode();
                break;

            case "booster":
                panelGamePlay.SetActive(true);
                changeGameState(GameState.BOOSTER);
                break;

            case "game":
                panelGamePlay.SetActive(true);
                changeGameState(GameState.GAMEPLAY);
                break;
        }

    }

    public void CheckBags()
    {
        int q = 0;
        foreach (int ii in suit_bags)
        {
            q += ii;
        }

        if (q > 0)
        {
            qtd_bags.SetActive(true);
        }
        else
        {
            qtd_bags.SetActive(false);
        }

        qtd_bags_txt.text = q.ToString();

        int i = 0;
        foreach (int b in suit_bags)
        {
            qtd_booster_bags_txt[i].text = b.ToString();

            if(b > 0)
            {
                // Ligando quantidade de maletas que tem
                qtd_booster_bags[i].SetActive(true);
            }
            else
            {
                // Desligando quantidade de maletas que tem
                qtd_booster_bags[i].SetActive(false);
            }

            i++;
        }
    }

    public void GetBags(int id_bag, int qtd_bag)
    {
        suit_bags[id_bag] += qtd_bag;
        CheckBags();
        string type_bag = "";

        switch (id_bag)
        {
            case 0:
                type_bag = "Maleta Comum";
                break;

            case 1:
                type_bag = "Maleta Rara";
                break;

            case 2:
                type_bag = "Maleta Epica";
                break;

            case 3:
                type_bag = "Maleta Lendaria";
                break;
        }
        OpenCutReward(ico_suit, "Recebeu <color=#ffff00>" + qtd_bag + " </color>" + type_bag, "upgrade");

        SaveGame();
    }

    public void OpenInAppPurchase()
    {
        switch (currentState)
        {
            case GameState.GAMEPLAY:
                panelFume.SetActive(true);
                panel_purchase.SetActive(true);
                changeGameState(GameState.SHOPPING);
                break;

            case GameState.SHOPPING:
                panelFume.SetActive(false);
                panel_purchase.SetActive(false);
                changeGameState(GameState.GAMEPLAY);
                break;
        }
    }

    public void OpenChooseCard(SlotController slot)
    {
        switch (currentState)
        {
            case GameState.GAMEPLAY:
                panelFume.SetActive(true);
                panel_choose_card.SetActive(true);
                changeGameState(GameState.CHOOSE);
                UpgradeChoose();
                break;

            case GameState.CHOOSE:
                panelFume.SetActive(false);
                panel_choose_card.SetActive(false);
                changeGameState(GameState.GAMEPLAY);
                break;
        }

        slot_choose = slot;
    }

    public void OpenCollection()
    {
        switch (currentState)
        {
            case GameState.GAMEPLAY:
                panelFume.SetActive(true);
                panel_collection.SetActive(true);
                changeGameState(GameState.COLLECTION);
                UpgradeInventory();
                break;

            case GameState.COLLECTION:
                panelFume.SetActive(false);
                panel_collection.SetActive(false);
                changeGameState(GameState.GAMEPLAY);
                break;
        }
    }

    public void OpenMenuBar()
    {
        switch (currentState)
        {
            case GameState.GAMEPLAY:
                menuBar = menuGame.GetComponent<Animator>();
                if (isOpen)
                {
                    menuBar.Play("CloseMenu");
                    isOpen = false;
                    changeGameState(GameState.GAMEPLAY);
                }
                else
                {
                    menuBar.Play("OpenMenu");
                    isOpen = true;
                    changeGameState(GameState.GAMEPLAY);
                }
                break;

            case GameState.CHOOSE:
                OpenChooseCard(slot_choose);
                changeGameState(GameState.GAMEPLAY);
                break;
        }
    }

    public void ChooseCard(CardCollection card)
    {
        if (card._Card.isLiberate)
        {
            slot_choose.buildSprite.sprite = card.sprite_card.sprite;

            changeGameState(GameState.GAMEPLAY);
            panel_choose_card.SetActive(false);
            panelGamePlay.SetActive(true);
            panelFume.SetActive(false);
            SaveSlot(slot_choose._Slots);
        }
    }

    public void SaveGame()
    {
        // Passando informações para serem salvas
        SaveGame save = new SaveGame
        {
            gold = coins,
            gold_accumulated = coinsAccumulated,
            gems = gems,
            gems_accumulated = gemsAccumulated,
            qtd_suitcase_common = qtd_suit_common,
            suitcase_price = suit_price,
            multiplier_bonus = multiplierBonus,
            reductor_bonus = reductionBonus,
            is_quest = isQuest,
            id_quest = idQuest,
            xp = xp,
            xp_accumulated = xpAccumulated
        };

        // Salvando no banco
        DocumentReference count_ref = db.Collection(user).Document("game");
        count_ref.SetAsync(save).ContinueWithOnMainThread(task => {

            Debug.Log("Salvou o jogo");
        });
    }

    public void SaveCard(Card c)
    {
        // Passando informações para serem salvas
        SaveCard save = new SaveCard
        {
            is_liberate = c.isLiberate,
            is_max = c.isMax,
            card_collected = c.card_collected,
            level_card = c.levelCard,
            production_multiplier = c.productionMultiplier,
            production_reduction = c.productionReduction
        };

        // Salvando no banco
        DocumentReference count_ref = db.Collection(user).Document(c.cardName);
        count_ref.SetAsync(save).ContinueWithOnMainThread(task => {

            Debug.Log("Salvou a carta " + c.name);
        });
    }

    public void SaveSlot(Slots s)
    {
        string temp = null;

        foreach (SlotController sc in _SlotController)
        {
            if (sc._Slots.idSlot == s.idSlot)
            {
                temp = sc.buildSprite.sprite.name;
            }
        }
        // Passando informações para serem salvas
        SaveSlot save = new SaveSlot
        {
            id_slot = s.idSlot,
            is_auto_production = s.isAutoProduction,
            is_max = s.isMax,
            is_purchased = s.isPurchased,
            slot_level = s.slotLevel,
            slot_production_multiplier = s.slotProductionMultiplier,
            slot_production_reduction = s.slotProductionReduction,
            total_upgrades = s.totalUpgrades,
            upgrades = s.upgrades,
            upgrade_price = s.upgradePrice,
            build_sprite = temp,
        };

        // Salvando no banco
        DocumentReference count_ref = db.Collection(user).Document(s.name);
        count_ref.SetAsync(save).ContinueWithOnMainThread(task => {

            Debug.Log("Salvou slot " + s.name);
        });
    }

    public void LoadGame()
    {
        // Leitura dos dados do game
        listenerRegistration = db.Collection(user).Document("game").Listen(snapshot =>
        {
            if (!snapshot.Exists) { return; }

            SaveGame counter = snapshot.ConvertTo<SaveGame>();
            coins = counter.gold;
            coinsAccumulated = counter.gold_accumulated;
            gems = counter.gems;
            gemsAccumulated = counter.gems_accumulated;
            multiplierBonus = counter.multiplier_bonus;
            reductionBonus = counter.reductor_bonus;
            qtd_suit_common = counter.qtd_suitcase_common;
            suit_price = counter.suitcase_price;
            isQuest = counter.is_quest;
            idQuest = counter.id_quest;
            xp = counter.xp;
            xpAccumulated = counter.xp_accumulated;
        });
    }

    public void LoadCard(Card c)
    {
        // Leitura dos dados das cartas
        listenerRegistration = db.Collection(user).Document(c.cardName).Listen(snapshot =>
        {
            if (!snapshot.Exists) { return; }

            SaveCard counter = snapshot.ConvertTo<SaveCard>();
            c.isLiberate = counter.is_liberate;
            c.isMax = counter.is_max;
            c.card_collected = counter.card_collected;
            c.levelCard = counter.card_collected;
            c.productionMultiplier = counter.production_multiplier;
            c.productionReduction = counter.production_reduction;
        });
    }

    public void LoadSlot(Slots s)
    {
        Sprite temp = null;

        // Leitura dos dados das cartas
        listenerRegistration = db.Collection(user).Document(s.name).Listen(snapshot =>
        {
            if (!snapshot.Exists) { return; }

            SaveSlot counter = snapshot.ConvertTo<SaveSlot>();
            s.idSlot = counter.id_slot;
            s.isAutoProduction = counter.is_auto_production;
            s.isMax = counter.is_max;
            s.isPurchased = counter.is_purchased;
            s.slotLevel = counter.slot_level;
            s.slotProductionMultiplier = counter.slot_production_multiplier;
            s.slotProductionReduction = counter.slot_production_reduction;
            s.upgrades = counter.upgrades;
            s.totalUpgrades = counter.total_upgrades;
            s.upgradePrice = counter.upgrade_price;
            s.StartSlotsScriptable();

            foreach (var sprite in constructions)
            {
                if(counter.build_sprite == sprite.name)
                {
                    temp = sprite;
                }
            }

            foreach (SlotController sc in _SlotController)
            {
                if (sc._Slots.idSlot == counter.id_slot)
                {
                    sc.StartSlot();
                    sc.buildSprite.sprite = temp;
                }
            }
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotController : MonoBehaviour
{
    private Animator animator;

    [HideInInspector]
    public GameController   _GameController;

    [Header("Painel Compra")]
    public GameObject       panelPurchase;
    public Image            iconCoinPurchase;
    public TextMeshProUGUI  pricePurchaseTxt;

    [Space]
    [Header("HUD Terrain")]
    public GameObject       huds;
    public SpriteRenderer   bgSlot;
    public SpriteRenderer   buildSprite;
    public Transform        hudPosition;

    [Space]
    [Header("HUD Produção")]
    public GameObject       panelProduction;
    public Image            icoCoinProduction;
    public Image            loadBar;
    public TextMeshProUGUI  productionTxt;

    [Space]
    [Header("HUD Upgrade")]
    public GameObject       panelUpgrade;
    public Image            bgUpgrade;
    public Image            progressBar;
    public TextMeshProUGUI  progressTxt;
    public Image            icoCoinUpgrade;
    public TextMeshProUGUI  priceUpgradeTxt;
    public TextMeshProUGUI  slotLevelTxt;

    [Space]
    [Header("HUD Purchase")]
    public GameObject       ico_coin_purchase;
    public GameObject       price_purchase;

    [Space]
    [Header("Slots GamePlay")]
    public Slots            _Slots;
    public double           coin;
    private float           tempTime;
    private float           fillAmount;
    private int             xp;
    private int             gem;

    //Controle inicial
    private bool isInitalized;
    private bool isLoop;

    // Start is called before the first frame update
    public void StartSlot()
    {
        huds.transform.position = hudPosition.position;
        _Slots.StartSlotsScriptable();

        animator = GetComponent<Animator>();

        //Gerenciamento dos huds
        if(_Slots.isPurchased == false)
        {
            panelProduction.SetActive(false);
            panelUpgrade.SetActive(false);
            panelPurchase.SetActive(true);
            buildSprite.enabled = false;
            pricePurchaseTxt.text = _GameController.currencyConverterCoin(_Slots.slotPrice);
        }
        else if(_Slots.isPurchased == true)
        {
            panelProduction.SetActive(true);
            panelPurchase.SetActive(false);
            panelUpgrade.SetActive(false);
            buildSprite.sprite = _Slots.slotCard.spriteCard;
            buildSprite.enabled = true;
        }

        UpgradeHudSlot();
        isInitalized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isInitalized == false) { return; }
        
        //Comandos
        if(_Slots.isPurchased == true)
        {
            if(coin == 0)
            {
                coinproduction();
            }
            else if(coin > 0 && _Slots.isAutoProduction == true)
            {
                coinproduction();
            }
            UpgradeHudSlot();
        }
        else if(_Slots.isPurchased == false)
        {
            if(_GameController.checkCoin(_Slots.slotPrice) == true)
            {
                bgSlot.sprite = _GameController.slotBg[1];
                iconCoinPurchase.sprite = _GameController.icoCoin[1];
                pricePurchaseTxt.colorGradient = new VertexGradient(_GameController.colorText[1]);
            }
            else
            {
                bgSlot.sprite = _GameController.slotBg[0];
                iconCoinPurchase.sprite = _GameController.icoCoin[0];
                pricePurchaseTxt.colorGradient = new VertexGradient(_GameController.colorText[0]);
            }
        }

    }

    void coinproduction()
    {
        tempTime += Time.deltaTime;

        if(_Slots.slotTimeProduction <= 0.1f)
        {
            fillAmount = 1;
        }
        else
        {
            fillAmount = tempTime / (_Slots.slotTimeProduction / _GameController.reductionBonus / _GameController.reductionBonusTemp);
        }

        loadBar.fillAmount = fillAmount;

        if(tempTime >= _Slots.slotTimeProduction / _GameController.reductionBonus / _GameController.reductionBonusTemp)
        {
            tempTime = 0;
            coin += _Slots.slotProduction * _GameController.multiplierBonus * _GameController.multiplierBonusTemp;
            xp += _Slots.xpProduction;
            productionTxt.text = _GameController.currencyConverterCoin(coin);
        }

        if(coin > 0) { icoCoinProduction.gameObject.SetActive(true); } else { icoCoinProduction.gameObject.SetActive(false); }
    }

    void coinCollect()
    {
        if(coin <= 0) { return; }
        _GameController.getCoin(coin);
        
        //Instanciar Moeda
        GameObject tempCoin = Instantiate(_GameController.coinPrefab, hudPosition.position, hudPosition.localRotation);
        tempCoin.GetComponent<CoinAnimation>().posY = hudPosition.position.y;
        tempCoin.GetComponent<Rigidbody2D>().AddForce(new Vector2(-35,400));
        animator.SetTrigger("Colect");

        //Instanciar texto
        GameObject tempText = Instantiate(_GameController.textPrefab, hudPosition.position + new Vector3(0,1.5f,0), hudPosition.localRotation);
        TextAnimation t = tempText.GetComponent<TextAnimation>();
        t.production.text = "+" + _GameController.currencyConverterCoin(coin);
        t.shadow.text = "+" + _GameController.currencyConverterCoin(coin);
        t.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 300));

        xpCollect();

        coin = 0;
        productionTxt.text = coin.ToString();

    }

    void xpCollect()
    {
        _GameController.getXp(xp);


        //Instanciar XP
        GameObject tempXP = Instantiate(_GameController.xpPrefab, hudPosition.position, hudPosition.localRotation);
        tempXP.GetComponent<XPAnimation>().posY = hudPosition.position.y;
        tempXP.GetComponent<Rigidbody2D>().AddForce(new Vector2(35, 500));

        //Instanciar texto
        GameObject tempTextXp = Instantiate(_GameController.textxpPrefab, hudPosition.position + new Vector3(0.5f, 1.5f, 0), hudPosition.localRotation);
        TextAnimation tx = tempTextXp.GetComponent<TextAnimation>();
        tx.production.text = "+" + _GameController.currencyConverterXP(xp);
        tx.shadow.text = "+" + _GameController.currencyConverterXP(xp);
        tx.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 500));

        xp = 0;
    }

    public void gemCollect()
    {
        _GameController.getGems(gem);

        //Instanciar Gem
        GameObject tempGem = Instantiate(_GameController.gemPrefab, hudPosition.position, hudPosition.localRotation);
        tempGem.GetComponent<GemAnimation>().posY = hudPosition.position.y;
        tempGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 600));

        //Intanciar texto
        GameObject tempTextgem = Instantiate(_GameController.textxpPrefab, hudPosition.position + new Vector3(0.5f, 1.5f, 0), hudPosition.localRotation);
        TextAnimation tx = tempTextgem.GetComponent<TextAnimation>();
        tx.production.text = "+" + _GameController.currencyConverterGem(gem);
        tx.shadow.text = "+" + _GameController.currencyConverterGem(gem);
        tx.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 600));

        gem = 0;
    }

    public void checkGameState()
    {
        switch(_GameController.currentState)
        {
            case GameState.GAMEPLAY:
                if (_GameController.isOpen == true)
                {
                    huds.SetActive(true);
                    _GameController.slot_controller3.huds.SetActive(false);
                    ico_coin_purchase.SetActive(false);
                    price_purchase.SetActive(false);
                    _GameController.panelFume.SetActive(false);
                }
                else
                {
                    huds.SetActive(true);
                    _GameController.slot_controller3.huds.SetActive(true);
                    ico_coin_purchase.SetActive(true);
                    price_purchase.SetActive(true);
                    _GameController.panelFume.SetActive(false);
                }
                
                break;

            case GameState.CUT:
                huds.SetActive(false);
                _GameController.panelFume.SetActive(true);
                break;

            case GameState.MARKET:
                huds.SetActive(false);
                _GameController.panelFume.SetActive(true);
                break;

            case GameState.UPGRADE:
                ico_coin_purchase.SetActive(false);
                price_purchase.SetActive(false);
                break;

            case GameState.SHOPPING:
                huds.SetActive(false);
                ico_coin_purchase.SetActive(false);
                price_purchase.SetActive(false);
                break;

            case GameState.CHOOSE:
                huds.SetActive(false);
                break;

            case GameState.COLLECTION:
                huds.SetActive(false);
                break;
        }
    }

    public void upgradeSlot()
    {
        if(_Slots.isMax == true) { return; }

        _Slots.upgrades += 1;
        _Slots.totalUpgrades += 1;

        _GameController.getCoin(_Slots.upgradePrice * -1);

        if(_Slots.slotLevel == 1 && _Slots.upgrades >= _GameController.progressSlot[_Slots.slotLevel -1])
        {
            upgradeStatus(0);

            //Missão inicial
            if (_GameController.isQuest && _GameController.idQuest == 1 )
            {
                _GameController.UpDataQuest();
            }
        }
        else if (_Slots.slotLevel == 2 && _Slots.upgrades >= _GameController.progressSlot[_Slots.slotLevel - 1])
        {
            upgradeStatus(1);
            _Slots.isAutoProduction = true;
            _GameController.GetBags(0, 2);
        }
        else if (_Slots.slotLevel == 3 && _Slots.upgrades >= _GameController.progressSlot[_Slots.slotLevel - 1])
        {
            upgradeStatus(0);
        }
        else if (_Slots.slotLevel == 4 && _Slots.upgrades >= _GameController.progressSlot[_Slots.slotLevel - 1])
        {
            upgradeStatus(1);
            _GameController.getGems(25);
            _GameController.OpenCutReward(_GameController.ico_gem, "Recebeu <color=#ffff00>25</color> gemas", "upgrade");
        }
        else if (_Slots.slotLevel == 5 && _Slots.upgrades >= _GameController.progressSlot[_Slots.slotLevel - 1])
        {
            upgradeStatus(0);
        }
        else if (_Slots.slotLevel == 6 && _Slots.upgrades >= _GameController.progressSlot[_Slots.slotLevel - 1])
        {
            upgradeStatus(1);
        }
        else if (_Slots.slotLevel == 7 && _Slots.upgrades >= _GameController.progressSlot[_Slots.slotLevel - 1])
        {
            upgradeStatus(0);
        }
        else if (_Slots.slotLevel == 8 && _Slots.upgrades >= _GameController.progressSlot[_Slots.slotLevel - 1])
        {
            upgradeStatus(1);
        }
        else if (_Slots.slotLevel == 9 && _Slots.upgrades >= _GameController.progressSlot[_Slots.slotLevel - 1])
        {
            upgradeStatus(1);
            upgradeStatus(0);
            _GameController.getGems(25);
            _GameController.OpenCutReward(_GameController.ico_gem, "Recebeu <color=#ffff00>25</color> gemas", "upgrade");
            _Slots.isMax = true;
        }

        _Slots.StartSlotsScriptable();
        UpgradeHudSlot();
        _GameController.SaveSlot(_Slots);
    }

    public void upgradeStatus(int i)
    {
        // i = 0: MULTIPLICADOR
        // i = 1: REDUTOR
        if (_Slots.slotLevel == 10)
        {
            _Slots.slotProductionReduction += 2;
            _Slots.slotProductionMultiplier += 2;
            return;
        }
        _Slots.slotLevel += 1;
        _Slots.upgrades = 0;

        switch(i)
        {
            case 0:
                if(_Slots.slotLevel == 2)
                {
                    _Slots.slotProductionMultiplier += 1;
                }
                else
                {
                    _Slots.slotProductionMultiplier += 2;
                }
                break;

            case 1:
                if(_Slots.slotLevel == 3)
                {
                    _Slots.slotProductionReduction += 1;
                }
                else
                {
                    _Slots.slotProductionReduction += 2;
                }
                break;
        }
    }

    void UpgradeHudSlot()
    {

        slotLevelTxt.text = _Slots.slotLevel.ToString();
        priceUpgradeTxt.text = _GameController.currencyConverterCoin(_Slots.upgradePrice);
        progressTxt.text = _Slots.totalUpgrades.ToString();

        float fillAmount = 0;
        if(_Slots.upgrades > 0)
        {
            fillAmount = (float)_Slots.upgrades / _GameController.progressSlot[_Slots.slotLevel -1];
        }

        progressBar.fillAmount = fillAmount;

        if(_Slots.isMax == true)
        {
            bgUpgrade.sprite = _GameController.bgUpgrade[2];
        }
        else
        {
            //Possui coin para evoluir
            if(_GameController.checkCoin(_Slots.upgradePrice) == true)
            {
                bgUpgrade.sprite = _GameController.bgUpgrade[1];
                priceUpgradeTxt.color = _GameController.colorText[1];
                icoCoinUpgrade.sprite = _GameController.icoCoin[1];
            }
            else //Não possui coin
            {
                bgUpgrade.sprite = _GameController.bgUpgrade[0];
                priceUpgradeTxt.color = _GameController.colorText[0];
                icoCoinUpgrade.sprite = _GameController.icoCoin[0];
            }
        }

    }

    public void upgradeModeSlot()
    {
        if(_Slots.isPurchased == false) { return; }

        UpgradeHudSlot();

        switch(_GameController.currentState)
        {
            case GameState.UPGRADE:
                panelProduction.SetActive(false);
                panelUpgrade.SetActive(true);
                break;

            case GameState.GAMEPLAY:
                panelProduction.SetActive(true);
                panelUpgrade.SetActive(false);
                break;
        }
        
    }

    private void OnMouseEnter()
    {
        if(_GameController.currentState == GameState.GAMEPLAY && _Slots.isPurchased == true)
        {
            coinCollect();
        }
    }

    private void OnMouseDown()
    {
        // Controle de seleção da carta para terreno
        if (_GameController.currentState == GameState.GAMEPLAY && _Slots.isPurchased == true && _Slots.is_ground == true)
        {
            _GameController.OpenChooseCard(this);
        }
        else if(_GameController.currentState == GameState.GAMEPLAY && _Slots.isPurchased == false && _GameController.checkCoin(_Slots.slotPrice) == true)
        {
           _GameController.BuySlot(_Slots, this);
            
        }
        else if (_GameController.currentState == GameState.GAMEPLAY && _Slots.isPurchased == true)
        {
            coinCollect();
        }
    }

    public void OnPointerDown()
    {
        StartCoroutine("loopUpgrade");
    }

    public void OnPointerUp()
    {
        StopCoroutine("loopUpgrade");
        isLoop = false;
    }

    IEnumerator loopUpgrade()
    {
        if(_GameController.checkCoin(_Slots.upgradePrice) == true &&_Slots.isMax == false)
        {
            upgradeSlot();
        }

        if(isLoop == false)
        {
            yield return new WaitForSeconds(_GameController.delayLoopUpgrade);
            isLoop = true;
        }

        yield return new WaitForSeconds(_GameController.delayBetweenUpgrade);
        
        if(isLoop == true && _GameController.currentState == GameState.UPGRADE)
        {
            StartCoroutine("loopUpgrade"); 
        }
        else
        {
            isLoop = false;
        }

    }
}

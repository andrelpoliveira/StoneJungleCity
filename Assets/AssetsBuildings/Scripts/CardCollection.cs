using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardCollection : MonoBehaviour
{
    [HideInInspector]
    public GameController _GameController;
    [HideInInspector]
    public Card _Card;
    
    public TMP_Text card_name;
    public TMP_Text price_txt;
    [HideInInspector]
    public double price_doub;
    public TMP_Text level_card;
    public TMP_Text progress_card;

    public Image sprite_card;
    public Image bg_card;
    public Image ico_price;
    public Image progress_bar;
    public Image progress_bg;

    public void UpgradeInfoCard(int id, string stage)
    {
        if(stage == "inventory")
        {
            ico_price.gameObject.SetActive(false);
            price_txt.gameObject.SetActive(false);

            if (_Card.isLiberate == false)
            {
                bg_card.sprite = _GameController.bg_card[4];
                sprite_card.sprite = _Card.shadowCard;
                card_name.text = "?";
                level_card.text = "0";
                progress_card.text = "";
                progress_bar.fillAmount = 0;
            }
            else
            {
                switch (_Card.rarityCard)
                {
                    case Rarity.COMMOM:
                        bg_card.sprite = _GameController.bg_card[0];
                        break;

                    case Rarity.RARE:
                        bg_card.sprite = _GameController.bg_card[1];
                        break;

                    case Rarity.EPIC:
                        bg_card.sprite = _GameController.bg_card[2];
                        break;

                    case Rarity.LEGEND:
                        bg_card.sprite = _GameController.bg_card[3];
                        break;
                }
                
                sprite_card.sprite = _Card.initial_sprite_card;
                card_name.text = _Card.cardName;
                level_card.text = _Card.levelCard.ToString();
                progress_card.text = _Card.card_collected + "/" + _GameController.progress_card[_Card.levelCard - 1];

                float fill_amount = 0;

                if (_Card.card_collected > 0)
                {
                    fill_amount = (float)_Card.card_collected / _GameController.progress_card[_Card.levelCard - 1];
                }

                progress_bar.fillAmount = fill_amount;
            }
        }
        else if(stage == "collection")
        {
            progress_bar.gameObject.SetActive(false);
            progress_card.gameObject.SetActive(false);
            progress_bg.gameObject.SetActive(false);

            switch (_Card.rarityCard)
            {
                case Rarity.COMMOM:
                    bg_card.sprite = _GameController.bg_card[0];
                    price_txt.text = _GameController.currencyConverterCoin(_GameController.slots[id].slotPrice / 3);
                    price_doub = _GameController.slots[id].slotPrice / 3;
                    ico_price.sprite = _GameController.icoCoin[1];
                    break;

                case Rarity.RARE:
                    bg_card.sprite = _GameController.bg_card[1];
                    price_txt.text = _GameController.currencyConverterCoin(_GameController.slots[id].slotPrice / 3);
                    price_doub = _GameController.slots[id].slotPrice / 3;
                    ico_price.sprite = _GameController.icoCoin[1];
                    break;

                case Rarity.EPIC:
                    bg_card.sprite = _GameController.bg_card[2];
                    price_txt.text = _GameController.currencyConverterCoin(_GameController.slots[id].slotPrice / 3);
                    price_doub = _GameController.slots[id].slotPrice / 3;
                    ico_price.sprite = _GameController.icoCoin[1];
                    break;

                case Rarity.LEGEND:
                    bg_card.sprite = _GameController.bg_card[3];
                    price_txt.text = _GameController.currencyConverterCoin(150);
                    ico_price.sprite = _GameController.ico_gem;
                    break;
            }

            sprite_card.sprite = _Card.initial_sprite_card;
            card_name.text = _Card.cardName;
        }
        
    }
}

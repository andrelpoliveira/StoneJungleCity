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
    public TMP_Text level_card;
    public TMP_Text progress_card;

    public Image sprite_card;
    public Image bg_card;
    public Image progress_bar;

    public void UpgradeInfoCard()
    {
        if(_Card.isLiberate == false)
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

            sprite_card.sprite = _Card.spriteCard;
            card_name.text = _Card.cardName;
            level_card.text = _Card.levelCard.ToString();
            progress_card.text = _Card.card_collected + "/" + _GameController.progress_card[_Card.levelCard - 1];

            float fill_amount = 0;

            if(_Card.card_collected > 0)
            {
                fill_amount = (float)_Card.card_collected / _GameController.progress_card[_Card.levelCard - 1];
            }

            progress_bar.fillAmount = fill_amount;
        }
    }
}

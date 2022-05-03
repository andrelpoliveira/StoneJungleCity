using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardInfo : MonoBehaviour
{
    public GameController _GameController;

    public Image ico_reward;
    public Image bg_reward;
    public TMP_Text reward_description;
    public Card _Card;

    private double qtd;
    
    public void ShowReward(int id_reward)
    {
        //0 - ouro 1 - gema 2 - carta
        // aqui decide logica de recompensas

        switch (id_reward)
        {
            case 0:
                qtd = Random.Range((float)_GameController.getCoinAccumulated() / 5, (float)_GameController.getCoinAccumulated() / 3);
                _GameController.getCoin(qtd);
                ico_reward.sprite = _GameController.icoCoin[1];
                bg_reward.sprite = _GameController.bg_card[1];
                reward_description.text = _GameController.currencyConverterCoin(qtd) + "Moedas";
                break;

            case 1:
                qtd = Random.Range(10, 25);
                _GameController.getGems((int)qtd);
                ico_reward.sprite = _GameController.ico_gem;
                bg_reward.sprite = _GameController.bg_card[0];
                reward_description.text = qtd.ToString() + "Gemas";
                break;

            case 2:
                break;
        }
    }
}

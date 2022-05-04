using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpenSuitCase : MonoBehaviour
{
    [HideInInspector]
    public GameController _GameController;
    [HideInInspector]
    public Rarity suit_rarity;
    [HideInInspector]
    public int qtd_rewards;
    public TMP_Text qtd_reward_txt;
    public GameObject btn_suit_case;
    public GameObject btn_close;
    public GameObject[] slot_reward;

    private int id_reward;
    private bool is_get_rare;
    private bool is_get_epic;
    private bool is_get_legendary;

    public void Start()
    {
        btn_suit_case.SetActive(true);
        btn_close.SetActive(false);
        qtd_reward_txt.text = qtd_rewards.ToString();
        id_reward = 0;

        foreach (GameObject s in slot_reward)
        {
            s.SetActive(false);
            s.GetComponent<RewardInfo>()._GameController = _GameController;
        }

        is_get_rare = false;
        is_get_epic = false;
        is_get_legendary = false;
    }

    public void OpenCase()
    {
        if(qtd_rewards > 0)
        {
            switch (suit_rarity)
            {
                case Rarity.COMMOM:
                    RandomReward();
                    break;

                case Rarity.RARE:
                    if (!is_get_rare)
                    {
                        is_get_rare = true;
                        getCard(Rarity.RARE);
                    }
                    else
                    {
                        RandomReward();
                    }
                    break;

                case Rarity.EPIC:
                    if (!is_get_epic)
                    {
                        is_get_epic = true;
                        getCard(Rarity.EPIC);
                    }
                    else if (!is_get_rare)
                    {
                        is_get_rare = true;
                        getCard(Rarity.RARE);
                    }
                    else
                    {
                        RandomReward();
                    }
                    break;

                case Rarity.LEGEND:
                    if (!is_get_legendary)
                    {
                        is_get_legendary = true;
                        getCard(Rarity.LEGEND);
                    }
                    else if (!is_get_epic)
                    {
                        is_get_epic = true;
                        getCard(Rarity.EPIC);
                    }
                    else if (!is_get_rare)
                    {
                        is_get_rare = true;
                        getCard(Rarity.RARE);
                    }
                    else
                    {
                        RandomReward();
                    }
                    break;
            }
        }
        id_reward += 1;
        qtd_rewards -= 1;
        qtd_reward_txt.text = qtd_rewards.ToString();

        if (qtd_rewards == 0)
        {
            btn_suit_case.SetActive(false);
            btn_close.SetActive(true);
        }
    }

    void RandomReward()
    {
        int rand = Random.Range(0, 100);

        // Recompensa com 75% de chances
        if(rand >= 25)
        {
            // Ouro
            slot_reward[id_reward].GetComponent<RewardInfo>().ShowReward(0);
            slot_reward[id_reward].gameObject.SetActive(true);
        }
        // Recompensa com 20% de chances
        else if (rand >= 5)
        {
            // Gema
            slot_reward[id_reward].GetComponent<RewardInfo>().ShowReward(1);
            slot_reward[id_reward].gameObject.SetActive(true);
        }
        // Recompensa com 5% de chances
        else
        {
            // Carta
            RandomCard();
            slot_reward[id_reward].GetComponent<RewardInfo>().ShowReward(2);
            slot_reward[id_reward].gameObject.SetActive(true);
        }
    }

    void RandomCard()
    {
        int rand = Random.Range(0, 100);

        // Recompensa com 1% de chance
        if(rand >= 99)
        {
            // Lendária
            getCard(Rarity.LEGEND);
        }
        // Recompensa com 5% de chance
        else if (rand >= 94)
        {
            // Épica
            getCard(Rarity.EPIC);
        }
        // Recompensa com 20% de chance
        else if (rand >= 74)
        {
            // Rara
            getCard(Rarity.RARE);
        }
        // Recompensa com 74% de chance
        else if(rand < 74)
        {
            // Comun
            getCard(Rarity.COMMOM);
        }
    }

    void getCard(Rarity r)
    {
        RewardInfo r_info = slot_reward[id_reward].GetComponent<RewardInfo>();

        // Sorteando carta da base das cartas
        switch (r)
        {
            case Rarity.COMMOM:
                r_info._Card = _GameController.card_common[Random.Range(0, _GameController.card_common.Count)];
                break;

            case Rarity.RARE:
                r_info._Card = _GameController.card_rare[Random.Range(0, _GameController.card_rare.Count)];
                break;

            case Rarity.EPIC:
                r_info._Card = _GameController.card_epic[Random.Range(0, _GameController.card_epic.Count)];
                break;

            case Rarity.LEGEND:
                r_info._Card = _GameController.card_legendary[Random.Range(0, _GameController.card_legendary.Count)];
                break;
        }

        r_info.ShowReward(2);
        slot_reward[id_reward].SetActive(true);
    }

    public void CloseReward()
    {
        _GameController.changeGameState(GameState.COLLECTION);
        _GameController.panel_open_booster.SetActive(false);
    }
}

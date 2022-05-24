using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangesUi : MonoBehaviour
{
    public Image load_bar;

    [Space]
    [Header("Panel")]
    public GameObject btn_panel;
    public GameObject login_panel;
    public GameObject register_panel;
    public GameObject forget_panel;
    public GameObject panel_entrada;
    public GameObject panel_geral;

    [Space]
    [Header("Login")]
    public TMP_InputField email_log_input;
    public TMP_InputField password_log_input;
    
    [Space]
    [Header("Register")]
    public TMP_InputField user_reg_input;
    public TMP_InputField email_reg_input;
    public TMP_InputField password_reg_input;
    public TMP_InputField verify_reg_input;
    
    [Space]
    [Header("Forget")]
    public TMP_InputField email_rec_input;

    public IEnumerator Load()
    {
        panel_entrada.SetActive(true);
        panel_geral.SetActive(false);
        load_bar.fillAmount = 0;
        yield return new WaitForSeconds(1.2f);
        load_bar.fillAmount = 0.333f;
        yield return new WaitForSeconds(1.2f);
        load_bar.fillAmount = 0.66f;
        yield return new WaitForSeconds(1.14f);
        load_bar.fillAmount = 1;
    }

    public void BackBtn()
    {
        ResetInputs();
        register_panel.SetActive(false);
        forget_panel.SetActive(false);
        login_panel.SetActive(false);
        btn_panel.SetActive(true);
    }

    public void ChangeLogin()
    {
        ResetInputs();
        btn_panel.SetActive(false);
        login_panel.SetActive(true);
    }

    public void ChangeRegister()
    {
        ResetInputs();
        btn_panel.SetActive(false);
        register_panel.SetActive(true);
        forget_panel.SetActive(false);
        login_panel.SetActive(false);
    }

    public void ChangeForget()
    {
        ResetInputs();
        btn_panel.SetActive(false);
        register_panel.SetActive(false);
        forget_panel.SetActive(true);
        login_panel.SetActive(false);
    }

    void ResetInputs()
    {
        email_log_input.text = "";
        password_log_input.text = "";
        user_reg_input.text = "";
        email_reg_input.text = "";
        password_reg_input.text = "";
        verify_reg_input.text = "";
        email_rec_input.text = "";
    }
}

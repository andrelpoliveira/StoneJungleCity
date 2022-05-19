using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangesUi : MonoBehaviour
{
    [Header("Panel")]
    public GameObject btn_panel;
    public GameObject login_panel;
    public GameObject register_panel;
    public GameObject forget_panel;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityEngine.UI;
using TMPro;

public class SyncLoginRegister : MonoBehaviour
{
    [Header("App Config")]
    public string               apiUrl;
    public string               tableDynamoDB;

    [Space]
    [Header("Inputs Register User")]
    public GameObject           usernameRegister;
    public GameObject           passwordRegister;
    public GameObject           passwordConfirmRegister;
    public GameObject           emailRegister;
    public GameObject           textRegister;

    [Space]
    [Header("Inputs Login User")]
    public GameObject           emailLogin;
    public GameObject           passwordLogin;


    [ContextMenu("Login")]
    public void Login()
    {
        LoginSignUpModel loginSignUpModel = new LoginSignUpModel
        {
            table = tableDynamoDB,
            email = emailLogin.GetComponent<TMP_InputField>().text,
            password = passwordLogin.GetComponent<TMP_InputField>().text
        };

        string jsonBody = JsonUtility.ToJson(loginSignUpModel);

        RestClient.Post(apiUrl, jsonBody).Then(response =>
        {
            print($"response:: {response.Text}");
        }).Catch(error =>
        {
            print($"error:: {error.Message}");
        });
    }

    [ContextMenu("Signup")]
    public void Signup()
    {
        if(passwordRegister.GetComponent<TMP_InputField>().text == passwordConfirmRegister.GetComponent<TMP_InputField>().text)
        {
            LoginSignUpModel loginSignUpModel = new LoginSignUpModel
            {
                table = tableDynamoDB,
                username = usernameRegister.GetComponent<TMP_InputField>().text,
                password = passwordRegister.GetComponent<TMP_InputField>().text,
                email = emailRegister.GetComponent<TMP_InputField>().text
            };

            string jsonBody = JsonUtility.ToJson(loginSignUpModel);

            RestClient.Put(apiUrl, jsonBody).Then(response =>
            {
                print($"response:: {response.Text}");
            }).Catch(error =>
            {
                print($"error:: {error.Message}");
            });
        }
        else { textRegister.GetComponent<TMP_Text>().text = "Senha não confere!!"; }
        
    }

}

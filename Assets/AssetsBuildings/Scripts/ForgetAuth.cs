using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;

public class ForgetAuth : MonoBehaviour
{
    public TMP_InputField email_input;
    public TMP_Text message_txt;

    public void ForgetButton()
    {
        StartCoroutine(StartForget(email_input.text));
    }

    private IEnumerator StartForget(string email)
    {
        var ForgetTask = FireBaseAuthenticator.instance.auth.SendPasswordResetEmailAsync(email);
        yield return new WaitUntil(predicate: () => ForgetTask.IsCompleted);

        if(ForgetTask.Exception != null)
        {
            HandleForgetErro(ForgetTask.Exception);
        }
        else
        {
            message_txt.text = $"Email enviado para {email}";
        }
    }

    void HandleForgetErro(System.AggregateException forget_exception)
    {
        Debug.LogWarning(message: $"Erro de recuperação devido {forget_exception}");
        FirebaseException firebaseException = forget_exception.GetBaseException() as FirebaseException;
        AuthError erroCode = (AuthError)firebaseException.ErrorCode;
        message_txt.text = DefineForgetError(erroCode);
    }

    string DefineForgetError(AuthError erro_code)
    {
        switch (erro_code)
        {
            case AuthError.None:
                return message_txt.text = "Campo vazio";
            
            case AuthError.InvalidEmail:
                return message_txt.text = "Email inváldio";
            
                break;
            case AuthError.MissingEmail:
                return message_txt.text = "Email vazio";
            
            default:
                return message_txt.text = "Erro desconhecido";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using TMPro;

public class LoginAuth : MonoBehaviour
{
    public TMP_InputField email_input;
    public TMP_InputField password_input;
    public TMP_Text message_txt;
    [HideInInspector]
    public string user;

    public void LoginButton()
    {
        StartCoroutine(StartLogin(email_input.text, password_input.text));
    }

    private IEnumerator StartLogin(string email, string password)
    {
        var LoginTask = FireBaseAuthenticator.instance.auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if(LoginTask.Exception != null)
        {
            HandleLoginErrors(LoginTask.Exception);
        }
        else
        {
            LoginUser(LoginTask);
        }
    }

    void HandleLoginErrors(System.AggregateException login_exception)
    {
        Debug.LogWarning(message: $"Falha ao fazer login devido {login_exception}");
        FirebaseException firebase_exception = login_exception.GetBaseException() as FirebaseException;
        AuthError error_code = (AuthError)firebase_exception.ErrorCode;

        message_txt.text = DefineLoginErrorMessage(error_code);
    }

    string DefineLoginErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                return "Email vazio";

            case AuthError.MissingPassword:
                return "Senha vazia";

            case AuthError.InvalidEmail:
                return "Email não cadastrado";

            case AuthError.UserNotFound:
                return "Email ou Senha Errado";

            default:
                return "Erro desconhecido";
        }
    }

    void LoginUser(System.Threading.Tasks.Task<Firebase.Auth.FirebaseUser> login_task)
    {
        FireBaseAuthenticator.instance.user = login_task.Result;
        Debug.LogFormat("Logado com Sucesso");
        message_txt.text = "Logado";
        user = login_task.Result.DisplayName;
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("CityInterior");
    }
}

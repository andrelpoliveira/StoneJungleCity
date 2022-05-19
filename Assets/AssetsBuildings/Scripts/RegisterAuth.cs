using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using TMPro;

public class RegisterAuth : MonoBehaviour
{
    public TMP_InputField user_register;
    public TMP_InputField email_register;
    public TMP_InputField password_register;
    public TMP_InputField verify_register;
    public TMP_Text message_txt;

    public void RegisterButton()
    {
        StartCoroutine(StartRegister(email_register.text, password_register.text, user_register.text));
    }

    private IEnumerator StartRegister(string email, string password, string user)
    {
        if (!CheckRegistration())
        {
            var RegisterTask = FireBaseAuthenticator.instance.auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                HandleRegisterError(RegisterTask.Exception);
            }
            else
            {
                StartCoroutine(RegisterUser(RegisterTask, user, email, password));
            }
        }
    }

    bool CheckRegistration()
    {
        if (user_register.text == "")
        {
            message_txt.text = "Nome de usuário vazio";
            return true;
        }
        else if (password_register.text != verify_register.text)
        {
            message_txt.text = "Senhas diferentes";
            return true;
        }
        else
        {
            return false;
        }
    }

    void HandleRegisterError(System.AggregateException register_exception)
    {
        Debug.LogWarning(message: $"Falha ao tentar se registra {register_exception}");
        FirebaseException firebaseException = register_exception.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseException.ErrorCode;
        message_txt.text = DefineRegisterErrorMessage(errorCode);
    }

    string DefineRegisterErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                return "Email vazio ou errado";

            case AuthError.MissingPassword:
                return "Senha vazia ou errada";

            case AuthError.WeakPassword:
                return "Senha fraca";

            case AuthError.InvalidEmail:
                return "Email inválido";

            case AuthError.EmailAlreadyInUse:
                return "Email já esta cadastrado";

            default:
                return "Falha ao se cadastrar devido erro desconhecido";
        }
    }

    private IEnumerator RegisterUser(System.Threading.Tasks.Task<Firebase.Auth.FirebaseUser> registerTask, string display_name, string email, string password)
    {
        FireBaseAuthenticator.instance.user = registerTask.Result;

        if (FireBaseAuthenticator.instance.user != null)
        {
            UserProfile profile = new UserProfile { DisplayName = display_name };
            var ProfileTask = FireBaseAuthenticator.instance.user.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

            if (ProfileTask.Exception != null)
            {
                HandleProfileErro(ProfileTask.Exception);
            }
            else
            {
                //Aqui parte de login após termino do cadastro
                message_txt.text = "Cadastro Realizado";
                SceneManager.LoadScene("CityInterior");
            }
        }
    }

    void HandleProfileErro(System.AggregateException profileException)
    {
        Debug.LogWarning(message: $"Falha no registro devido {profileException}");
        FirebaseException firebaseException = profileException.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseException.ErrorCode;
        message_txt.text = "Falha no registro";
    }
}

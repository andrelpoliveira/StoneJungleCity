using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;

public class FireBaseAuthenticator : MonoBehaviour
{
    public static FireBaseAuthenticator instance;
    public DependencyStatus dependency_status;
    public FirebaseAuth auth;
    public FirebaseUser user;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependency_status = task.Result;

            if(dependency_status == DependencyStatus.Available)
            {
                InitializeFireBase();
            }
            else
            {
                Debug.LogError("Erro com as dependencias do firebase: " + dependency_status);
            }
        });
    }

    void InitializeFireBase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }
}

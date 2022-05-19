using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
public class FireBaseDBManager : MonoBehaviour
{
    public TMP_Text gold;

    private FirebaseFirestore db;
    private ListenerRegistration listenerRegistration;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        
        // Leitura do banco
        /*listenerRegistration = db.Collection("counters").Document("user").Listen(snapshot => 
        {
            User counter = snapshot.ConvertTo<User>();
            gold.text = counter.count.ToString();
        });*/
        GetData();
    }

    public void OnHandleClick()
    {
        // Passando informações para serem salvas
        int old_count = int.Parse(gold.text);
        User user = new User
        {
            count = old_count + 1,
            updated_by = "fred"
        };

        //gold.text = old_count.ToString();

        // Salvando no banco
        DocumentReference count_ref = db.Collection("counters").Document("user");
        count_ref.SetAsync(user).ContinueWithOnMainThread(task => {

            Debug.Log("atualizou");
            GetData();
         });
    }

    void OnDestroy()
    {
        //listenerRegistration.Stop();
    }

    void GetData()
    {
        db.Collection("counters").Document("user").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            User counter = task.Result.ConvertTo<User>();
            gold.text = counter.count.ToString();
            print("passou");
        });
    }
}

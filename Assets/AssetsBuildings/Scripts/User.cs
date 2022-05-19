using Firebase.Firestore;

[FirestoreData]
public struct User 
{
    [FirestoreProperty]
    public int count { get; set; }

    [FirestoreProperty]
    public string updated_by { get; set; }
}


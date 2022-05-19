using Firebase.Firestore;

[FirestoreData]
public struct SaveGame
{
    [FirestoreProperty]
    public double gold { get; set; }
    [FirestoreProperty]
    public double gold_accumulated { get; set; }
    [FirestoreProperty]
    public int gems { get; set; }
    [FirestoreProperty]
    public int gems_accumulated { get; set; }
    [FirestoreProperty]
    public int qtd_suitcase_common { get; set; }
    [FirestoreProperty]
    public double suitcase_price { get; set; }
    [FirestoreProperty]
    public int multiplier_bonus { get; set; }
    [FirestoreProperty]
    public float reductor_bonus { get; set; }
    [FirestoreProperty]
    public bool is_quest { get; set; }
    [FirestoreProperty]
    public int id_quest { get; set; }
    [FirestoreProperty]
    public int xp { get; set; }
    [FirestoreProperty]
    public int xp_accumulated { get; set; }
}

[FirestoreData]
public struct SaveCard
{
    [FirestoreProperty]
    public bool is_liberate { get; set; }
    [FirestoreProperty]
    public int level_card { get; set; }
    [FirestoreProperty]
    public int card_collected { get; set; }
    [FirestoreProperty]
    public int production_multiplier { get; set; }
    [FirestoreProperty]
    public float production_reduction { get; set; }
    [FirestoreProperty]
    public bool is_max { get; set; }
}

[FirestoreData]
public struct SaveSlot
{
    [FirestoreProperty]
    public int id_slot { get; set; }
    [FirestoreProperty]
    public bool is_purchased { get; set; }
    [FirestoreProperty]
    public bool is_max { get; set; }
    [FirestoreProperty]
    public bool is_auto_production { get; set; }
    [FirestoreProperty]
    public int slot_level { get; set; }
    [FirestoreProperty]
    public int upgrades { get; set; }
    [FirestoreProperty]
    public int total_upgrades { get; set; }
    [FirestoreProperty]
    public double upgrade_price { get; set; }
    [FirestoreProperty]
    public int slot_production_multiplier { get; set; }
    [FirestoreProperty]
    public float slot_production_reduction { get; set; }
    [FirestoreProperty]
    public string build_sprite { get; set; }
    
}

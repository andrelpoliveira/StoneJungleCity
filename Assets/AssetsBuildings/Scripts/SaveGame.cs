using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Save", menuName = "Save Game/Save")]

public class SaveGame : ScriptableObject
{
    public double gold;
    public double gold_accumulated;

    [Header("Modifadores")]
    public int multiplier_bonus;
    public int multiplier_temporary;

    public int reductor_bonus;
    public int reductor_temporary;
}

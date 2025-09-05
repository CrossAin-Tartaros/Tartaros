using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField] public bool[] runeOwned;
    [SerializeField] public int coin;
    [SerializeField] public int health;
    [SerializeField] public int shieldCount;
}

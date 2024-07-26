using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageRecorder : MonoBehaviour
{
    PlayerController playerController;

    public Dictionary<string, int> DamageRecord { get; set; } = new();

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.PlayerDamaged += OnPlayerDamaged;
    }

    void OnDestroy()
    {
        if (playerController != null)
        {
            playerController.PlayerDamaged -= OnPlayerDamaged;
        }
    }

    void OnPlayerDamaged(object sender, PlayerDamagedEventArgs e)
    {
        if (DamageRecord.ContainsKey(e.damageName))
        {
            DamageRecord[e.damageName] += e.damage;
        }
        else
        {
            DamageRecord.Add(e.damageName, e.damage);
        }
    }
}

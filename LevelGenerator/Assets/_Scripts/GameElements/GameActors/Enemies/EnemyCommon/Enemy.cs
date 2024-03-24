using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefeatedEventArgs
{
    public Enemy enemy;
    public Position roomPosition;
}

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour, IDamageable, IMortal
{
    [SerializeField] int life;
    [SerializeField] EnemyTargetManager targetManager;
    [SerializeField] EnemyLocation location;
    [SerializeField] string characterSoundName;

    public event Action OnDamageTaken;
    public EventHandler<EnemyDefeatedEventArgs> OnDefeated;

    void Start()
    {
        AudioManager.Instance.AddCharacterSoundInstance(characterSoundName, gameObject.GetInstanceID());
    }

    void OnDestroy()
    {
        AudioManager.Instance.RemoveCharacterSoundInstance(gameObject.GetInstanceID());
        EnemyDefeatedEventArgs e = new()
        {
            enemy = this,
            roomPosition = location.RoomPosition,
        };
        OnDefeated?.Invoke(this, e);
    }

    public void SetTargetPlayer(Transform target, Location targetLocation)
    {
        targetManager.Player = target;
        targetManager.PlayerLocation = targetLocation;
    }

    public void TakeDamage(int damage)
    {
        if (life - damage <= 0)
        {
            Die();
        }
        else
        {
            life -= damage;
            OnDamageTaken?.Invoke();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}

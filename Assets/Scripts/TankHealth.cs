 using UnityEngine;

public class TankHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public bool isDead = false;

    public GameObject deathEffect;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float dmg)
    {
        Debug.Log($"TakeDamage called with dmg: {dmg}");
        if (isDead) return;
        currentHealth -= dmg;
        Debug.Log($"Current health: {currentHealth}");
        if (currentHealth <= 0f)
            Die();
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        if (isDead) return;
        isDead = true;

        var combat = GetComponent<TankCombatSystem>();
        if (combat != null) combat.enabled = false;

        var move = GetComponent<UnitMovements>();
        if (move != null) move.enabled = false;

        var outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;

        var drawer = GetComponent<TankPathDrawer>();
        if (drawer != null) drawer.HidePath();

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity, transform);

        Debug.Log($"{gameObject.name} has died.");
    }
}

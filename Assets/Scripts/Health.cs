using System;
using UnityEngine;

public class Health {
    public int maxHealth;
    public int currentHealth;

    public event Action<float> OnHealthChanged;

    public Health(int health) {
        maxHealth = health;
        currentHealth = health;
    }

    public void TakeDamage(int damage) {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);
    }

    public void Heal(int amount) {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);
    }

    public float GetHealthNormalized() {
        if (currentHealth <= 0) return 0f;
        if (maxHealth <= 0) return 0f;
        
        float normalized = (float)currentHealth / maxHealth;
        // Se a vida estiver muito baixa (menos de 1%), considerar como morto
        if (normalized < 0.01f) return 0f;
        
        return normalized;
    }
}
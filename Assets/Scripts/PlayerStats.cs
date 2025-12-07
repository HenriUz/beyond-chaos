using UnityEngine;

public class PlayerStatsData {
    
    public int maxHealth = 100;
    public int maxSpecialCharges = 3;
    public int currentHealth = 100;
    public int specialCharges = 0;
    public int baseDamage = 25;
    public float normalAttackMultiplier = 1.0f;
    public float specialAttackMultiplier = 2.5f;
    public float damageVariationMax = 1.2f;
    
    public int CalculateDamage(bool isSpecialAttack) {
        float multiplier = isSpecialAttack ? specialAttackMultiplier : normalAttackMultiplier;
        float baseValue = baseDamage * multiplier;
        float variation = UnityEngine.Random.Range(1.0f, damageVariationMax);
        return Mathf.RoundToInt(baseValue * variation);
    }
    
    public void AddSpecialCharge() {
        if (specialCharges < maxSpecialCharges) {
            specialCharges++;
        }
    }
    
    public bool CanUseSpecial() {
        return specialCharges > 0;
    }
    
    public void UseSpecialCharge() {
        if (specialCharges > 0) {
            specialCharges--;
        }
    }
    
    public void Reset() {
        currentHealth = maxHealth;
        specialCharges = 0;
    }
}

public class PlayerStats : MonoBehaviour {
    public int maxHealth = 100;
    public int maxSpecialCharges = 3;
    public int currentHealth = 100;
    public int specialCharges = 0;
    public int baseDamage = 25;
    public float normalAttackMultiplier = 1.0f;
    public float specialAttackMultiplier = 2.5f;
    public float damageVariationMax = 1.2f;

    public PlayerStatsData GetData() {
        return new PlayerStatsData {
            maxHealth = this.maxHealth,
            maxSpecialCharges = this.maxSpecialCharges,
            currentHealth = this.currentHealth,
            specialCharges = this.specialCharges,
            baseDamage = this.baseDamage,
            normalAttackMultiplier = this.normalAttackMultiplier,
            specialAttackMultiplier = this.specialAttackMultiplier,
            damageVariationMax = this.damageVariationMax
        };
    }
}
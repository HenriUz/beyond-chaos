using UnityEngine;

public class EnemyStatsData {
    public int maxHealth = 100;
    public int baseDamage = 10;
    public float attackSpeedMultiplier = 1f;
    public float attack2ChancePercent = 30f;
    public float attack3ChancePercent = 20f;
    public float damageVariationMax = 1.15f;
    public float attack1Multiplier = 1.0f;
    public float attack2Multiplier = 1.5f;
    public float attack3Multiplier = 2.0f;
    
    public int CalculateDamage(CharacterBase.AttackType attackType) {
        float damageMultiplier = attackType switch {
            CharacterBase.AttackType.Attack2 => attack2Multiplier,
            CharacterBase.AttackType.Attack3 => attack3Multiplier,
            _ => attack1Multiplier
        };
        
        float baseValue = baseDamage * damageMultiplier;
        float variation = UnityEngine.Random.Range(1.0f, damageVariationMax);
        return Mathf.RoundToInt(baseValue * variation);
    }
}

public class EnemyStats : MonoBehaviour {
    public int maxHealth = 100;
    public int baseDamage = 10;
    public float attackSpeedMultiplier = 1f;
    public float attack2ChancePercent = 30f;
    public float attack3ChancePercent = 20f;
    public float damageVariationMax = 1.15f;
    public float attack1Multiplier = 1.0f;
    public float attack2Multiplier = 1.5f;
    public float attack3Multiplier = 2.0f;

    public EnemyStatsData GetData() {
        return new EnemyStatsData {
            maxHealth = this.maxHealth,
            baseDamage = this.baseDamage,
            attackSpeedMultiplier = this.attackSpeedMultiplier,
            attack2ChancePercent = this.attack2ChancePercent,
            attack3ChancePercent = this.attack3ChancePercent,
            damageVariationMax = this.damageVariationMax,
            attack1Multiplier = this.attack1Multiplier,
            attack2Multiplier = this.attack2Multiplier,
            attack3Multiplier = this.attack3Multiplier
        };
    }
}

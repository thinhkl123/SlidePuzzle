using UnityEngine;

public class WaterHuntBossModel
{
    public WaterHuntBossView WaterHuntBossView;
    public int Health = 3;
    
    public void SetView(WaterHuntBossView waterHuntBossView)
    {
        this.WaterHuntBossView = waterHuntBossView;
        this.WaterHuntBossView.SetHealth(Health);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        this.WaterHuntBossView.SetHealth(Health);
    }
}

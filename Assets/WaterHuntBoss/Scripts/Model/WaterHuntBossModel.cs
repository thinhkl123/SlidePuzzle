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
        Debug.Log(Health);
        if (Health <= 0)
        {
            this.WaterHuntBossView.SetHide();
        }
        else
        {
            this.WaterHuntBossView.SetHealth(Health);
        }
    }
}

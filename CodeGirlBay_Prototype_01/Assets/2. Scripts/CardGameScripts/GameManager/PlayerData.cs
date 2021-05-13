public class PlayerData
{
    private int playerHP = 0;
    private int playerMaxHP = 0;


    public void CreateNewPlayer()
    {
        playerMaxHP = 100;
        playerHP = 100;
    }

    public int GetPlayerMaxHP()
    {
        return playerMaxHP;
    }

    public int GetPlayerHP()
    {
        return playerHP;
    }

    public void Damage10PlayerHP()
    {
        playerHP -= 10;
        if (0 > playerHP)
            playerHP = 0;
    }

    public void Heal10PlayerHP()
    {
        playerHP += 10;
        if (playerMaxHP < playerHP)
            playerHP = playerMaxHP;
    }

    public void HitDamage(int damage)
    {
        playerHP -= damage;
        if (0 > playerHP)
            playerHP = 0;
    }
}
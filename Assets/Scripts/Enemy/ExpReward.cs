using UnityEngine;

public class ExpReward : MonoBehaviour
{
    [SerializeField] private int expValue = 10;

    public void GrantExp()
    {
        PlayerEXP.Instance.AddExp(expValue);
    }
}

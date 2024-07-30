using UnityEngine;

public class Snake : Enemy
{
    [SerializeField] DashController dashController;
    [SerializeField] AIVision aiVision;

    void Update()
    {
        if (aiVision.PlayerVisible)
        {
            dashController.TryDash();
        }
    }
}

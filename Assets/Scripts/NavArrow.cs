using UnityEngine;

public class NavArrow : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.instance.currentTarget != null)
        {
            transform.LookAt(GameManager.instance.currentTarget);
        }
    }
}

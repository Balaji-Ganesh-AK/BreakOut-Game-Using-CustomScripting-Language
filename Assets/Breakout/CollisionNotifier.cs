using UnityEngine;

public class CollisionNotifier : MonoBehaviour
{
    private BreakOutGameController _controller;

    #region Unity Functions
    private void OnTriggerEnter(Collider other)
    {
        if (_controller == null)
        {
            Debug.Log("sfd");
        }

        if (_controller != null)
        {
            _controller.CollisionTrigger(gameObject, other.gameObject);

        }

    }

    #endregion

    #region External Functions

    public void SetController(BreakOutGameController controller) => _controller = controller;

    #endregion
}
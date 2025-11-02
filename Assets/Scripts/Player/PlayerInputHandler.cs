using UnityEngine;
using UnityEngine.InputSystem;

namespace WaveMagicSurvivor.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerController playerController;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            
            if (playerController == null)
            {
                Debug.LogError("PlayerInputHandler: PlayerController component not found!");
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.OnMove(context);
            }
        }
    }
}


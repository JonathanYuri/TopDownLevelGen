using UnityEngine;

public class InputManager : MonoBehaviour
{
    bool inputEnabled = true;
    public void DisableInput() => inputEnabled = false;
    public void EnableInput() => inputEnabled = true;
    public bool IsInputEnabled() => inputEnabled;
}

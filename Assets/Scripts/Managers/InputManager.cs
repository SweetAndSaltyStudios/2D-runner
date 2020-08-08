using UnityEngine;

public class InputManager : Singelton<InputManager> 
{
    public bool Tap 
    {
        get 
        {
            return Input.GetMouseButtonDown(0);
        }
    }

    private void Awake() 
    {
     
    }
}

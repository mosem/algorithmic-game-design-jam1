using UnityEngine;

namespace Flight {
[CreateAssetMenu(menuName = "Controller/PlayerController")]

public class PlayerController : Controller {

	public string horizontalCtrl = "Horizontal_P1";
	public string verticalCtrl = "Vertical_P1";

	public override Vector2 GetInputVelocity ()
	{
		return new Vector2(Input.GetAxis(horizontalCtrl), Input.GetAxis(verticalCtrl));
	}

	public void Awake()
    {
        Debug.Log("Keyboard.Awake");
    }

    public void OnDestroy()
    {
        Debug.Log("Keyboard.OnDestroy");
    }
}

}

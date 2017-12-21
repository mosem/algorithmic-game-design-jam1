using UnityEngine;

namespace Flight {

public class PlayerController : Controller {

	public string horizontalCtrl = "Horizontal_P1";
	public string verticalCtrl = "Vertical_P1";

	public override Vector2 GetInputVelocity ()
	{
			return new Vector2(Input.GetAxis(horizontalCtrl), Input.GetAxis(verticalCtrl)).normalized;
	}

	public void Awake()
    {

    }

    public void OnDestroy()
    {

    }
}

}

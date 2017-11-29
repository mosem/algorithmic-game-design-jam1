
using UnityEngine;

namespace Flight {
/// <summary>
/// A base class that allows to get input for a player.
/// </summary>
public abstract class Controller : MonoBehaviour {

	public abstract Vector2 GetInputVelocity();
}
}

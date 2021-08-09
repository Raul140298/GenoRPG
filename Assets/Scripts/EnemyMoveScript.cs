using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveScript : MonoBehaviour
{
	Vector2 mov;
	public Animator anim;
	public bool isntMove;
	public ObjectPathMovementScript pathmovement;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (!this.gameObject.name.Contains("bowsette"))
		{
			if (isntMove == false) mov = pathmovement.velocity;
			//Si se está moviendo
			if (mov != Vector2.zero)
			{
				if (mov.x > 0) anim.SetFloat("movX", 1);
				else anim.SetFloat("movX", -1);

				if (mov.y > 0) anim.SetFloat("movY", 1);
				else anim.SetFloat("movY", -1);

				anim.SetBool("walking", true);
			}
			else
			{
				//anim.SetFloat("movX", -1);
				//anim.SetFloat("movY", -1);
				anim.SetBool("walking", false);
			}
		}
	}
}

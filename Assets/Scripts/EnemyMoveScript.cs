using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveScript : MonoBehaviour
{
	Vector3 mov;
	public Vector3 [] destino = new Vector3[4];
	public Animator anim;
	public int i = 0;
	public float distance = 0.1f;
	// Start is called before the first frame update
	void Start()
    {
		mov = new Vector3(0, 0, 0);

		destino[0] = new Vector3(this.transform.position.x - distance * 2f, this.transform.position.y + distance * 1f, this.transform.position.z);
		destino[1] = new Vector3(this.transform.position.x, this.transform.position.y + distance * 2f, this.transform.position.z);
		destino[2] = new Vector3(this.transform.position.x + distance * 2f, this.transform.position.y + distance * 1f, this.transform.position.z);
		destino[3] = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
		//destino[4] = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
		StartCoroutine(wait(1.2f));
	}

	// Update is called once per frame
	void Update()
	{
		ManageMovement();
	}

	void FixedUpdate()
	{
		//Si se está moviendo
		if (mov != Vector3.zero)
		{
			anim.SetFloat("movX", mov.x);
			anim.SetFloat("movY", mov.y);
			anim.SetBool("walking", true);
		}
		else
		{
			//anim.SetFloat("movX", -1);
			//anim.SetFloat("movY", -1);
			anim.SetBool("walking", false);
		}
	}

	void ManageMovement()
	{
		this.transform.position = Vector3.MoveTowards(
					this.transform.position,
					this.transform.position + mov,
					0.4f * Time.deltaTime);

		//switch (i)
		//{
		//	case 0:
		//		upLeft();
		//		break;
		//	case 1:
		//		upRight();
		//		break;
		//	case 2:
		//		downRight();
		//		break;
		//	case 3:
		//		downLeft();
		//		break;
		//	default:
		//		print("Error de movimiento enemigo\n");
		//		break;
		//}

		//if (this.transform.position == destino[i])
		//{
		//	i++;
		//	if (i == 4) i = 0;
		//}
	}

	void upLeft()
	{
		mov = new Vector3(-2, 1, 0);
	}

	void upRight()
	{
		mov = new Vector3(2, 1, 0);
	}

	void downLeft()
	{
		mov = new Vector3(-2, -1, 0);
	}

	void downRight()
	{
		mov = new Vector3(2, -1, 0);
	}

	IEnumerator wait(float x)
	{
		i++;
		if (i == 4) i = 0;
		switch (i)
		{
			case 0:
				upLeft();
				break;
			case 1:
				upRight();
				break;
			case 2:
				downRight();
				break;
			case 3:
				downLeft();
				break;
			default:
				print("Error de movimiento enemigo\n");
				break;
		}
		yield return new WaitForSeconds(x);
		StartCoroutine(wait(x));
	}
}

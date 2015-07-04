using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public Vector2[] path;
	public float speed = 1f;

	private int currentNode = 0;
    private bool isActive = true;
    private Vector2 move_dir;

	private float time = 0f;

    public int EndDamage = 1;

	void Start () {

	}

	void Update () {
        if (!isActive) return;
        Vector2 position = transform.position;
        Vector2 nodePos = path[currentNode];
        Vector2 positionDelta = nodePos - position;

        float dot_product = move_dir.x * positionDelta.x + move_dir.y * positionDelta.y;
        if (dot_product <= 0)
        {
            if (++currentNode == path.Length)
            {
                //OnEndReached.Invoke(gameObject);
                //GetComponent<Animator>().SetBool("Explode", true);
                Die();
                isActive = false;
            }
            else
            {
                nodePos = path[currentNode];
                move_dir = (nodePos - position).normalized;
                transform.position = position + move_dir * speed * Time.deltaTime;
                //_animator.SetFloat("SpeedX", move_dir.x);
                //_animator.SetFloat("SpeedY", move_dir.y);
                //_animator.SetBool("horizontal_move", Mathf.Abs(move_dir.x) > 0.1f);
            }
        } 
		else 
		{
            transform.position = position + move_dir * speed * Time.deltaTime;
		}
	}

    public void Die()
    {
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int _coins;
    private int playerBounds;

    // Getter for coins
    public int Coins { get { return _coins; } }

    // Start is called before the first frame update
    void Start()
    {
        playerBounds = GameManager.Instance.playerBounds;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Vector3 goal = Vector3.zero;
            // Up
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                goal = transform.position + Vector3.forward;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            // Left
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                goal = transform.position + Vector3.left;
                transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            // Right
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                goal = transform.position + Vector3.right;
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            // Down
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                goal = transform.position + Vector3.back;
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            if (goal != Vector3.zero && goal.x > -playerBounds && goal.x < playerBounds)
			{
                AudioManager.Instance.Play("Player Jump");
                transform.position = goal;
            }
        }
    }

	private void OnTriggerEnter(Collider other)
	{
        if(other.tag == "Collectible")
		{
            _coins++;
            other.GetComponent<Coin>().PickUp();
		}
        else if(other.tag == "Vehicle")
        {
            Die();
        }
    }

    private void Die()
	{
        //Destroy(gameObject);
        AudioManager.Instance.Play("Player Death");
        GameManager.Instance.GameOver();
	}
}

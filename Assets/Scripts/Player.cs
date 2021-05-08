using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int _coins;
    private int playerBounds;

    [Header("Particles")]
    public GameObject deathParticles;

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
        // Handle moving the player
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            Move(transform.position + Vector3.forward, 0);
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            Move(transform.position + Vector3.left, -90);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            Move(transform.position + Vector3.right, 90);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            Move(transform.position + Vector3.back, 180);
    }

	private void Move(Vector3 pos, float rot)
    {
        AudioManager.Instance.Play("Player Jump");
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, rot, 0);
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
        Instantiate(deathParticles, transform.position, Quaternion.identity, GameManager.Instance.transform);
        Destroy(gameObject);
	}
}

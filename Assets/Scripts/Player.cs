using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Singleton pattern
    private static Player _instance;
    public static Player Instance { get { return _instance; } }

    private int _coins;
    private int _steps;
    private int playerBounds;

    [Header("Particles")]
    public GameObject deathParticles;
    public ParticleSystem moveParticles;

    // Getter for coins
    public int Coins { get { return _coins; } }
    public int Steps { get { return _steps; } }

	private void Awake()
	{
        // Singleton pattern
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

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
        // Dont go left/right out of bounds
        if (pos.x <= -playerBounds || pos.x >= playerBounds)
            return;

        // Dont go back behind start line
        if (pos.z < 0)
            return;

        // Check if there is an obstacle here
        foreach(GameObject o in GameManager.Instance.Lanes[(int)pos.z].obstacles)
            if(o.transform.position.x == pos.x) 
                return;

        // Actually move
        AudioManager.Instance.Play("Player Jump");
        moveParticles.Play();
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, rot, 0);

        // Check for increased steps
        if (pos.z > _steps)
            _steps = (int)pos.z;
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

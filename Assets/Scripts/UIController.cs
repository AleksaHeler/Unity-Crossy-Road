using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI stepsText;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        coinsText.text = "0";
        stepsText.text = "0";

        player = Player.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            coinsText.text = player.Coins.ToString();
            stepsText.text = player.Steps.ToString();
        }
    }
}

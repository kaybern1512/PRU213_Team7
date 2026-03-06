using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinsToGive=1;

    private TextMeshProUGUI coinText;


    private void Start()
    {
        coinText=GameObject.Find("CoinText").GetComponent<TextMeshProUGUI>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.coins += coinsToGive;
            coinText.text = player.coins.ToString();
            Destroy(gameObject);
        }
    }

}

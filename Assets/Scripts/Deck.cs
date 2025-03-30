using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;
    
    public TextMeshProUGUI bancaText;
    public TextMeshProUGUI apuestaText;

    public int[] values = new int[52];
    int cardIndex = 0;

    int banca = 1000;
    int apuesta = 0;


    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        for (int i = 0; i < 52; i++)
        {
            int val = i % 13 + 1;
            if (val > 10)
            {
                val = 10;
            }
            values[i] = val;
        }

    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */
        for (int i = 0; i < faces.Length; i++)
        {
            int rand = Random.Range(0, faces.Length);

            // Intercambiar imágenes
            Sprite tempFace = faces[i];
            faces[i] = faces[rand];
            faces[rand] = tempFace;

            // Intercambiar valores
            int tempVal = values[i];
            values[i] = values[rand];
            values[rand] = tempVal;
        }
    }

    void StartGame()
    {
        if (apuesta == 0 || apuesta > banca)
        {
            finalMessage.text = "Debes hacer una apuesta válida antes de jugar.";
            ToggleButtons(false);
            return;
        }

        banca -= apuesta;
        ActualizarBanca();



        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
        }
        int playerPoints = player.GetComponent<CardHand>().points;
        int dealerPoints = dealer.GetComponent<CardHand>().points;

        if (playerPoints == 21 && dealerPoints != 21)
        {
            finalMessage.text = "¡Blackjack! Has ganado.";
            ToggleButtons(false);
        }
        else if (dealerPoints == 21 && playerPoints != 21)
        {
            finalMessage.text = "El dealer tiene Blackjack. Pierdes.";
            ToggleButtons(false);
        }
        else if (dealerPoints == 21 && playerPoints == 21)
        {
            finalMessage.text = "Empate con Blackjack.";
            ToggleButtons(false);
        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
        int playerPoints = player.GetComponent<CardHand>().points;
        int totalCartas = values.Length - cardIndex;

        int malas = 0;
        int buenas = 0;
        int entre17y21 = 0;

        for (int i = cardIndex; i < values.Length; i++)
        {
            int suma = playerPoints + values[i];

            if (suma > 21)
            {
                malas++;
            }
            else if (suma >= 17 && suma <= 21)
            {
                entre17y21++;
            }
            else
            {
                buenas++;
            }
        }

        int probMalas = (malas * 100) / totalCartas;
        int probMedias = (entre17y21 * 100) / totalCartas;

        probMessage.text = "P >21: " + probMalas + "%\n" + "P 17-21: " + probMedias + "%";

    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        GameObject primeraCartaDealer = dealer.GetComponent<CardHand>().cards[0];
        primeraCartaDealer.GetComponent<CardModel>().ToggleFace(true);


        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        int playerPoints = player.GetComponent<CardHand>().points;

        if (playerPoints > 21)
        {
            finalMessage.text = "Te pasaste. Pierdes.";
            ToggleButtons(false);
        }

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        GameObject primeraCartaDealer = dealer.GetComponent<CardHand>().cards[0];
        primeraCartaDealer.GetComponent<CardModel>().ToggleFace(true);


        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
        int dealerPoints = dealer.GetComponent<CardHand>().points;

        while (dealerPoints < 17)
        {
            PushDealer();
            dealerPoints = dealer.GetComponent<CardHand>().points;
        }

        int playerPoints = player.GetComponent<CardHand>().points;

        if (dealerPoints > 21 || playerPoints > dealerPoints)
        {
            finalMessage.text = "¡Ganaste!";
            banca += apuesta * 2;
        }
        else if (dealerPoints < playerPoints)
        {
            finalMessage.text = "¡Ganaste!";
            banca += apuesta * 2;
        }
        else if (dealerPoints > playerPoints)
        {
            finalMessage.text = "Perdiste.";
        }
        else
        {
            finalMessage.text = "Empate.";
            banca += apuesta; // en empate, devuelve lo apostado
        } 
        
        ToggleButtons(false);

        ActualizarBanca();


    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";

        probMessage.text = "";

        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();

        apuesta = 0;
        ActualizarBanca();

    }

    void ToggleButtons(bool state)
    {
        hitButton.interactable = state;
        stickButton.interactable = state;
    }

    void ActualizarBanca()
    {
        bancaText.text = "Banca: " + banca + "€";
        apuestaText.text = "Apuesta: " + apuesta + "€";

    }

    public void Apostar10()
    {
        if (banca >= 10)
        {
            apuesta += 10;
            ActualizarBanca();
        }
    }

    public void ResetApuesta()
    {
        apuesta = 0;
        ActualizarBanca();
    }

}

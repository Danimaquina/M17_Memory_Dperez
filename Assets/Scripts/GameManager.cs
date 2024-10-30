using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Material[] Stickers;
    public GameObject[] cards;
    private CardController PrimerTocado = null;
    private CardController SegundoTocado;
    private CardController carta;
    public GameObject UIStart;
    public GameObject Scenary;
    public GameObject End;
    public GameObject Congratulation;
    public TextMeshProUGUI time;
    public TextMeshProUGUI timeEnd;
    public TextMeshProUGUI attempts;
    public TextMeshProUGUI BestTime;
    public TextMeshProUGUI BestTimeCopy;
    private int Try;
    private float cronoTime;
    private float HolyCounter;
    private bool interaccionHabilitada = true;
    private bool isActive;
    private int contador = 0;
    private AudioSource audioSourceE;
    private AudioSource audioSourceC;
    private AudioSource audioSourceS;
    private AudioSource audioSourceV;
    public AudioClip Error;
    public AudioClip Correcto;
    public AudioClip Victoria;
    public AudioClip Inicio;




    void Start()
    {
        // Buscar el objeto con el tag "MainCamera" y obtener su AudioSource
        GameObject camara = GameObject.FindGameObjectWithTag("MainCamera");
        AudioSource[] audioSources = camara.GetComponents<AudioSource>();
        audioSourceE = audioSources[2];
        audioSourceC = audioSources[1];
        audioSourceS= audioSources[4];
        audioSourceV = audioSources[5];


        // Inicializamos todos los componentes
        HolyCounter = PlayerPrefs.GetFloat("BestScore", 60.0f); // Cargar el mejor tiempo guardado, o 30 si no existe
        cronoTime = 0.0f;
        Try = 0;
        attempts.text = "Attempts: " + Try;
        time.text = "Time: " + FormatearTiempo(cronoTime);
        BestTime.text = "BestTime: " + FormatearTiempo(HolyCounter);
        BestTimeCopy.text = "BestTime: " + FormatearTiempo(HolyCounter);
        
        Congratulation.SetActive(false);
        End.SetActive(false);
        Scenary.SetActive(false);
        UIStart.SetActive(true);


        // Fase de mezclado i asignacion de cada carta
        List<Material> stickersDuplicados = new List<Material>();

        // Duplicar materiales
        for (int i = 0; i < Stickers.Length; i++)
        {
            stickersDuplicados.Add(Stickers[i]);
            stickersDuplicados.Add(Stickers[i]);
        }

        // Barajar los materiales duplicados
        Barajar(stickersDuplicados);

        // Asignar el material y el valor correspondiente a cada carta
        for (int i = 0; i < cards.Length; i++)
        {
            // Obtener el componente Renderer de la carta actual
            Renderer renderer = cards[i].GetComponent<Renderer>();

            if (renderer != null)
            {
                Material materialAsignado = stickersDuplicados[i];
                renderer.material = materialAsignado;

                // Encontrar el índice del material asignado en la lista de stickers originales
                int pairValueIndex = System.Array.IndexOf(Stickers, materialAsignado);

                // Obtener el controlador de la carta y asignarle el valor de pareja
                CardController cardController = cards[i].GetComponent<CardController>();

                // Asignar el índice del material como el valor de la pareja en el controlador
                cardController.pairValue = pairValueIndex;

                // Asignar el GameManager actual a la carta para que tenga referencia al gestor del juego
                cardController.gameManager = this;
            }
        }
    }

    private void Update()
    {
        // Creamos temporizador i ademas le añadimos un estilo a este
        if (Scenary.activeSelf)
        {
            cronoTime += Time.deltaTime;
            time.text = "Time: " + FormatearTiempo(cronoTime);
        }

        attempts.text = "Attempts: " + Try;

        contador = 0;

        // Comprobar cuántas cartas están activas
        for (int i = 0; i < cards.Length; i++)
        {
            GameObject carta = cards[i].transform.parent.parent.gameObject;

            if (!carta.activeSelf)
            {
                contador++;
            }
        }

        // Si todas las cartas han desaparecido mostramos la pantalla final
        if (contador == 16)
        {
            Scenary.SetActive(false);
            UIStart.SetActive(false);

            // Comprobar si hemos roto el record
            if (cronoTime < HolyCounter)
            {
                HolyCounter = cronoTime; // Actualiza el mejor tiempo
                PlayerPrefs.SetFloat("BestScore", HolyCounter); // Guardar el nuevo mejor tiempo
                PlayerPrefs.Save();
                BestTime.text = "BestTime: " + FormatearTiempo(HolyCounter);
                Congratulation.SetActive(true);
                BestTimeCopy.text = "BestTime: " + FormatearTiempo(HolyCounter); // Muestra el nuevo mejor tiempo
                audioSourceV.PlayOneShot(Victoria);

            }
            else
            {
                // si no hemos roto el record mostramos la pantalla de END normal
                End.SetActive(true);
                timeEnd.text = "Time: " + FormatearTiempo(cronoTime);
            }
        }
    }

    // Creacion de metodos
    
    // Metodo que define que pasa cuando cuando se tocan las cartas
    // Ademas de definir que pasara en caso de que el par sea correcto o incorrecto
    public void ReceiveCardValue(CardController card)
    {
        if (!interaccionHabilitada) return; // Si la interacción está deshabilitada, no hace nada

        if (PrimerTocado == null)
        {
            PrimerTocado = card; // Guarda la referencia de la carta tocada
        }
        else
        {
            // Aumenta el contador de intentos en cada intento
            Try++;

            // Verifica si las 2 cartas selecionadas tiene el mismo PairValue (valor)
            // Pero evitando que sea la misma carta exacta
            if (PrimerTocado.pairValue == card.pairValue && PrimerTocado != card) 
            {
                // Correcto
                Invoke("Eliminacion", 1);
                DeshabilitarInteraccionTemporal(1.0f); // Deshabilita la interacción por 1 segundo
                carta = card;
                audioSourceC.PlayOneShot(Correcto);

            }
            else
            {
                // Differente
                SegundoTocado = card;
                DeshabilitarInteraccionTemporal(1.0f); // Deshabilita la interacción por 1 segundo
                Invoke("Esconderse", 1);
                audioSourceE.PlayOneShot(Error);

            }
        }

        // Actualiza el contador de intentos en la UI
        attempts.text = "Attempts: " + Try;
    }


    // Funcion para marcar las cartas tocadas
    private void MarkCardAsTouched(CardController card)
    {
        GameObject cartaObject = card.transform.parent.parent.gameObject;
        card.HasSidoTocado = true;
        card.DisableInteraction();
        cartaObject.SetActive(false);
    }

    // Funcion para volver a esconder las cartas en caso de que sea incorrecto
    private void Esconderse()
    {
        if (PrimerTocado != null)
        {
            GameObject carta1 = PrimerTocado.transform.parent.parent.gameObject;
            Animator anima1 = carta1.GetComponent<Animator>();
           
            if (anima1 != null)
            {
                anima1.SetTrigger("aplazado");
            }
        }

        if (SegundoTocado != null)
        {
            GameObject carta2 = SegundoTocado.transform.parent.parent.gameObject;
            Animator anima2 = carta2.GetComponent<Animator>();
           
            if (anima2 != null)
            {
                anima2.SetTrigger("aplazado");
            }
        }

        PrimerTocado = null; // Reinicia 
        SegundoTocado = null; // Reinicia 
    }

    // Funcion para retirar las cartas de la partida
    private void Eliminacion()
    {
        MarkCardAsTouched(PrimerTocado);
        MarkCardAsTouched(carta); // Marca también la segunda carta
        PrimerTocado = null; // Reinicia 

    }

    // Método para deshabilitar la interacción
    public void DeshabilitarInteraccionTemporal(float tiempo)
    {
        interaccionHabilitada = false; 
        Invoke("HabilitarInteraccion", tiempo); 
    }

    // Método para volver a habilitar la interacción
    private void HabilitarInteraccion()
    {
        interaccionHabilitada = true;
    }

    // Metod para saber en que situacion se encuentra la interacción
    public bool IsInteraccionHabilitada()
    {
        return interaccionHabilitada;
    }

    
    // Funcion para definir el inicio
    public void Empezar()
    {
        Scenary.SetActive(true);
        UIStart.SetActive(false);
        audioSourceS.PlayOneShot(Inicio);
    }
    
    // Funcion para reiniciar el juego
    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Comanda para reiniciar el juego
    }
    
    // Convierte el tiempo en formato minutos:segundos
    private string FormatearTiempo(float tiempo)
    {
        int minutos = Mathf.FloorToInt(tiempo / 60); // Convierte a minutos
        int segundos = Mathf.FloorToInt(tiempo % 60); // El resto son los segundos

        return minutos.ToString("00") + ":" + segundos.ToString("00");
    }
    
    
    // Funcion usada para barajar las cartas en cada partida
    void Barajar<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    
    // Esta funcion restablece la Best Score a 2 minutos en caso de que se toque el boton
    public void ResetBestTime()
    {
        HolyCounter = 120.0f;
        PlayerPrefs.SetFloat("BestScore", HolyCounter);
        PlayerPrefs.Save();

        BestTime.text = "BestTime: " + FormatearTiempo(HolyCounter);
        BestTimeCopy.text = "BestTime: " + FormatearTiempo(HolyCounter);

        Debug.Log("Best Score has been reset to 2 minutes.");
    }

   
}

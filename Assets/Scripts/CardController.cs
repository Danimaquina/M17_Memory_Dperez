using UnityEngine;
using UnityEngine.Serialization;

public class CardController : MonoBehaviour
{
    public int pairValue;
    public bool HasSidoTocado = false;
    public GameManager gameManager;
    public AudioClip Tocar;
    private AudioSource audioSource;

    void Start()
    {
        // Buscar el objeto con el tag "MainCamera" y obtener su AudioSource
        GameObject camara = GameObject.FindGameObjectWithTag("MainCamera");
        AudioSource[] audioSources = camara.GetComponents<AudioSource>();
        audioSource = audioSources[3]; 
    }

    void OnMouseDown()
    {
        if (gameManager != null && !gameManager.IsInteraccionHabilitada()) return; // No permitir interacción si está deshabilitada
        
      
        // Si has sido tocado
        if (!HasSidoTocado)
        {
            // Devuelve el valor de esta carta al Game Manager
            gameManager?.ReceiveCardValue(this);
            
            audioSource.PlayOneShot(Tocar);
            
            GameObject carta = gameObject.transform.parent.parent.gameObject;
            Animator anima = carta.GetComponent<Animator>();
            anima.SetTrigger("Tocado");
        }
    }

    
    //  Funcio que desabilita el collider per que no poguem interactuar amb les cartes
    public void DisableInteraction()
    {
        GetComponent<Collider>().enabled = false;
    }
}
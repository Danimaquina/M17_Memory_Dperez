using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretVisibilityUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;  // El CanvasGroup del objeto para controlar la visibilidad y la interacción
    private string currentInput = ""; // Almacena la entrada del teclado
    private string secretCodeShow = "EMPEROR"; // Código secreto para mostrar el objeto
    private string secretCodeHide = "SUN"; // Código secreto para ocultar el objeto

    void Start()
    {
        // Obtiene el componente CanvasGroup del objeto para manejar la visibilidad
        canvasGroup = GetComponent<CanvasGroup>();

        // Si no tiene un CanvasGroup, añadimos uno
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Oculta el objeto al principio
        Hide();
    }

    void Update()
    {
        // Verifica las entradas del teclado
        foreach (char c in Input.inputString)
        {
            // Añade la letra que se acaba de escribir
            currentInput += c.ToString().ToUpper();

            // Si la entrada actual tiene más caracteres que el código más largo, descarta los primeros caracteres
            int maxLength = Mathf.Max(secretCodeShow.Length, secretCodeHide.Length);
            if (currentInput.Length > maxLength)
            {
                currentInput = currentInput.Substring(currentInput.Length - maxLength);
            }

            // Si se introduce el código para mostrar, se muestra el objeto
            if (currentInput.EndsWith(secretCodeShow))
            {
                Show();
            }

            // Si se introduce el código para ocultar, se oculta el objeto
            if (currentInput.EndsWith(secretCodeHide))
            {
                Hide();
            }
        }
    }

    // Función para mostrar el objeto (en un Canvas)
    private void Show()
    {
        canvasGroup.alpha = 1;  // Hace visible el objeto
        canvasGroup.interactable = true; // Habilita la interacción
        canvasGroup.blocksRaycasts = true; // Permite que el objeto reciba eventos de clic
    }

    // Función para ocultar el objeto (en un Canvas)
    private void Hide()
    {
        canvasGroup.alpha = 0;  
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = false; 
    }
}
using UnityEngine;

public class MoverComMundo : MonoBehaviour
{
    private ControladorPulo playerScript;

    void Start()
    {
        playerScript = FindFirstObjectByType<ControladorPulo>();
    }

    void Update()
    {
        // Agora o script só tem UMA função: obedecer a velocidade e se mover!
        if (playerScript != null)
        {
            transform.Translate(Vector3.down * playerScript.velocidadeMundo * Time.deltaTime);
        }
    }
}
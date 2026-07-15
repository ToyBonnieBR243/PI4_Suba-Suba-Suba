using UnityEngine;

public class MoverComMundo : MonoBehaviour
{
    private ControladorPulo playerScript;

    void Start()
    {
        playerScript = FindFirstObjectByType<ControladorPulo>();
    }

    void LateUpdate()
    {
        if (playerScript != null)
        {
            // O Segredo: Movemos pela distância exata calculada (variacaoMundo).
            // Retiramos o Time.deltaTime porque essa variação já é o movimento puro!
            transform.Translate(Vector3.down * playerScript.variacaoMundo, Space.World);
        }
    }
}
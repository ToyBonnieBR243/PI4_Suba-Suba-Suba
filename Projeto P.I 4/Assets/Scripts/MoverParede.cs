using UnityEngine;

public class MoverComMundo : MonoBehaviour
{
    private ControladorPulo playerScript;

    // Agora é uma lista (Array) para guardar os dois lados da parede
    private SpriteRenderer[] meusSprites;

    [Header("Configurações Visuais")]
    public bool mudaDeCor = false;

    void Start()
    {
        playerScript = FindFirstObjectByType<ControladorPulo>();

        if (mudaDeCor)
        {
            // O segredo: procura o componente visual neste objeto E em todos os filhos dele!
            meusSprites = GetComponentsInChildren<SpriteRenderer>();
        }
    }

    void LateUpdate()
    {
        if (playerScript == null) return;

        // 1. O Movimento Exato
        transform.Translate(Vector3.down * playerScript.variacaoMundo, Space.World);

        // 2. A Lógica da Cor Atualizada
        // Agora verificamos se a lista de sprites achou pelo menos 1 imagem
        if (mudaDeCor && meusSprites != null && meusSprites.Length > 0 && playerScript.listaDeCores.Length > 0)
        {
            float alturaDestaParede = playerScript.alturaAtual + (transform.position.y - playerScript.transform.position.y);
            Color corEscolhida = playerScript.listaDeCores[0];

            for (int i = 0; i < playerScript.alturasDasCores.Length; i++)
            {
                if (alturaDestaParede >= playerScript.alturasDasCores[i])
                {
                    if (i + 1 < playerScript.listaDeCores.Length)
                    {
                        corEscolhida = playerScript.listaDeCores[i + 1];
                    }
                }
            }

            // Aplica a cor final em TODOS os pedaços de parede encontrados
            foreach (SpriteRenderer pedacoDaParede in meusSprites)
            {
                pedacoDaParede.color = corEscolhida;
            }
        }
    }
}
using UnityEngine;

public class ParallaxFundo : MonoBehaviour
{
    [Header("Camada 1: Fila de Progressão")]
    [Tooltip("Arraste aqui o objeto vazio que contém o Chão, Nuvens e Foguetes")]
    public Transform filaDeProgresso;
    public float multiplicadorFila = 0.5f; // Velocidade que a Terra e as nuvens passam

    [Header("Camada 0: Estrelas Infinitas")]
    public Transform estrela1;
    public Transform estrela2;
    public float alturaEstrela = 10f;
    public float multiplicadorEstrelas = 0.1f; // Bem mais lento para dar sensação de distância enorme

    private ControladorPulo playerScript;
    private float posicaoInicialFilaY;

    void Start()
    {
        playerScript = FindFirstObjectByType<ControladorPulo>();

        // Salva a posição exata da Terra para ela nunca subir além disso
        if (filaDeProgresso != null)
        {
            posicaoInicialFilaY = filaDeProgresso.position.y;
        }

        // Empilha a segunda imagem de estrelas
        if (estrela1 != null && estrela2 != null)
        {
            estrela2.position = new Vector3(estrela1.position.x, estrela1.position.y + alturaEstrela, estrela1.position.z);
        }
    }

    void LateUpdate()
    {
        if (playerScript == null) return;

        // 1. Move a Fila de Progressão (A jornada até sair da Via Láctea)
        if (filaDeProgresso != null)
        {
            float velFila = playerScript.velocidadeMundo * multiplicadorFila;
            filaDeProgresso.Translate(Vector3.down * velFila * Time.deltaTime, Space.World);

            // A TRAVA: Impede que o chão suba e mostre o vazio embaixo do mapa quando o jogador cai
            if (filaDeProgresso.position.y > posicaoInicialFilaY)
            {
                filaDeProgresso.position = new Vector3(filaDeProgresso.position.x, posicaoInicialFilaY, filaDeProgresso.position.z);
            }
        }

        // 2. Move as Estrelas Infinitas (Sempre em Loop)
        if (estrela1 != null && estrela2 != null)
        {
            float velEstrelas = playerScript.velocidadeMundo * multiplicadorEstrelas;
            estrela1.Translate(Vector3.down * velEstrelas * Time.deltaTime, Space.World);
            estrela2.Translate(Vector3.down * velEstrelas * Time.deltaTime, Space.World);

            // Se a tela descer (Jogador Pulando)
            if (estrela1.position.y <= -alturaEstrela)
            {
                estrela1.position = new Vector3(estrela1.position.x, estrela2.position.y + alturaEstrela, estrela1.position.z);
            }
            else if (estrela2.position.y <= -alturaEstrela)
            {
                estrela2.position = new Vector3(estrela2.position.x, estrela1.position.y + alturaEstrela, estrela2.position.z);
            }

            // Se a tela subir (Jogador Caindo)
            if (estrela1.position.y >= alturaEstrela)
            {
                estrela1.position = new Vector3(estrela1.position.x, estrela2.position.y - alturaEstrela, estrela1.position.z);
            }
            else if (estrela2.position.y >= alturaEstrela)
            {
                estrela2.position = new Vector3(estrela2.position.x, estrela1.position.y - alturaEstrela, estrela2.position.z);
            }
        }
    }
}
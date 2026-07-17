using UnityEngine;
using TMPro; // Biblioteca essencial para controlar a UI (Textos) na tela!

public class ControladorPulo : MonoBehaviour
{
    [Header("Configurações do Pulo")]
    public float forcaMaxima = 15f;
    public float forcaMinima = 5f;
    public float taxaDeCarga = 20f;
    public float gravidade = 25f;

    [Header("Configurações da Queda")]
    public float velocidadeMaximaQueda = -2f;

    [Header("Configurações das Faixas")]
    public float distanciaFaixa = 1.5f;
    public float velocidadeTroca = 15f;

    [Header("Sistema de Cores das Paredes")]
    [Tooltip("Em quais alturas a cor deve mudar? Ex: 350, 700")]
    public float[] alturasDasCores = { 350f };

    [Tooltip("Lista de Cores. Precisa ter UMA cor a mais que a lista de alturas!")]
    public Color[] listaDeCores;

    [Header("Medidor de Altura (Câmera)")]
    [Tooltip("Distância que o player precisa subir para travar no meio da tela")]
    public float alturaParaCentralizar = 3.5f;

    [Header("Checkpoints (Áreas Seguras)")]
    public float[] alturasDeCheckpoint = { 350f };
    public GameObject prefabChaoSeguro;

    [Tooltip("Quantos metros antes o checkpoint deve aparecer na tela?")]
    public float distanciaPreSpawn = 50f;

    [Tooltip("Ajuste negativo para o chão ficar nos pés e não no meio do personagem")]
    public float ajustePeY = -1.5f;

    [Header("Interface (UI)")]
    public TextMeshProUGUI textoAltura; // Onde vamos arrastar o texto da tela
    public TextMeshProUGUI textoPontos;
    public TextMeshProUGUI textoMultiplicador;
    public float pontosPorMetro = 1f;
    public float multiplicadorAtual = 1.0f;

    // O medidor real e absoluto de altura do jogo
    [HideInInspector] public float alturaAtual = 0f;
    [HideInInspector] public float velocidadeVirtual = 0f;

    // A velocidade que os outros scripts vão ler
    [HideInInspector] public float velocidadeMundo = 0f;
    [HideInInspector] public float variacaoMundo = 0f;
    
    
    private float posicaoMundoAnterior = 0f;
    private float chaoAtual = 0f; // Começa no zero
    private int proximoCheckpointIndex = 0; // Controla qual é o próximo da lista
    private bool checkpointSpawnado = false; // Avisa se a imagem já foi criada
    private float cargaAtual = 0f;
    private bool estaPulando = false;
    private int faixaAtual = 0;
    private float yInicial;
    private float alturaMaxima = 0f;

    void Start()
    {
        // Salva exatamente a posição Y onde o personagem começou (em cima do chão zero)
        yInicial = transform.position.y;
        posicaoMundoAnterior = Mathf.Max(0f, alturaAtual - alturaParaCentralizar);
    }

    void Update()
    {
        // 1. O Toque (Freio + Pulo Mínimo Garantido)
        if (Input.GetMouseButtonDown(0))
        {
            estaPulando = false;
            velocidadeVirtual = 0f;
            cargaAtual = forcaMinima; // A grande mágica: já começa com a força mínima!
        }

        // 2. Carregar Força (Segurar a tela)
        if (Input.GetMouseButton(0) && !estaPulando)
        {
            cargaAtual += taxaDeCarga * Time.deltaTime;

            if (cargaAtual > forcaMaxima)
            {
                cargaAtual = forcaMaxima;
            }
        }

        // 3. Pular (Soltar o dedo)
        if (Input.GetMouseButtonUp(0) && !estaPulando)
        {
            velocidadeVirtual = cargaAtual;
            cargaAtual = 0f;
            estaPulando = true;
        }

        // 4. A Gravidade e o Medidor de Altura
        if (estaPulando)
        {
            velocidadeVirtual -= gravidade * Time.deltaTime;
            if (velocidadeVirtual < velocidadeMaximaQueda) velocidadeVirtual = velocidadeMaximaQueda;

            // O MEDIDOR ENTRA AQUI: Acompanha a subida e a descida
            alturaAtual += velocidadeVirtual * Time.deltaTime;

            // A trava dinâmica! Se bater no chão atual (0, 350, 700...), pousa perfeitamente.
            if (alturaAtual <= chaoAtual)
            {
                alturaAtual = chaoAtual;
                velocidadeVirtual = 0f;
                estaPulando = false;
            }
        }

        // 5. Troca de Faixas
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) faixaAtual--;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) faixaAtual++;

        faixaAtual = Mathf.Clamp(faixaAtual, -1, 1);
        float posicaoAlvoX = faixaAtual * distanciaFaixa;

        // 6. A MÁGICA DA CÂMERA (Quem se move: O Player ou o Mundo?)
        float alturaVisualY = yInicial;

        if (alturaAtual < alturaParaCentralizar)
        {
            // O player ainda não chegou no meio da tela
            velocidadeMundo = 0f; // Congela o mundo
            alturaVisualY = yInicial + alturaAtual; // Sobe o personagem fisicamente
        }
        else
        {
            // O player passou do meio da tela
            velocidadeMundo = velocidadeVirtual; // O mundo herda a velocidade do pulo e passa a descer
            alturaVisualY = yInicial + alturaParaCentralizar; // O personagem trava no ar
        }

        // Aplica o movimento misto (X desliza suave, Y é exato para não tremer)
        float novoX = Mathf.Lerp(transform.position.x, posicaoAlvoX, velocidadeTroca * Time.deltaTime);
        transform.position = new Vector3(novoX, alturaVisualY, transform.position.z);

        // 7. Atualizando os Textos e Pontos
        // Trava para os pontos não diminuírem se o personagem cair
        if (alturaAtual > alturaMaxima)
        {
            alturaMaxima = alturaAtual;
        }

        if (textoAltura != null)
        {
            textoAltura.text = Mathf.FloorToInt(alturaAtual).ToString() + "M";
        }

        if (textoPontos != null)
        {
            // Pega a altura máxima, multiplica pela sua variável e corta os decimais
            int pontos = Mathf.FloorToInt(alturaMaxima * pontosPorMetro * multiplicadorAtual);
            textoPontos.text = "Pts: " + pontos.ToString();
        }

        if (textoMultiplicador != null)
        {
            // O "F1" força o Unity a sempre mostrar uma casa decimal (ex: 1.0, 1.5, 2.0)
            textoMultiplicador.text = multiplicadorAtual.ToString("F1") + "X";
        }

        // 8. Gerador de Checkpoints (Spawns Seguros)
        if (proximoCheckpointIndex < alturasDeCheckpoint.Length)
        {
            float alturaAlvo = alturasDeCheckpoint[proximoCheckpointIndex];

            // FASE 1: Cria o Acampamento visualmente antes do jogador chegar lá
            if (alturaAtual >= alturaAlvo - distanciaPreSpawn && !checkpointSpawnado)
            {
                if (prefabChaoSeguro != null)
                {
                    // Calcula a distância exata que falta para chegar lá
                    float distanciaFaltante = alturaAlvo - alturaAtual;

                    // Ele nasce acima do jogador + o ajuste de alinhamento dos pés
                    Vector3 posChao = new Vector3(0f, transform.position.y + distanciaFaltante + ajustePeY, 0f);

                    Instantiate(prefabChaoSeguro, posChao, Quaternion.identity);
                }
                checkpointSpawnado = true; // Trava para não criar clones infinitos
            }

            // FASE 2: A trava invisível de salvamento só sobe quando o jogador cruzar a marca
            if (alturaAtual >= alturaAlvo)
            {
                chaoAtual = alturaAlvo; // O novo "fundo do poço" agora é aqui
                proximoCheckpointIndex++;
                checkpointSpawnado = false; // Destrava para o próximo checkpoint (ex: 700m)
            }
        }

        // 9. Cálculo exato do movimento do mundo
        float posicaoMundoAtual = Mathf.Max(0f, alturaAtual - alturaParaCentralizar);
        variacaoMundo = posicaoMundoAtual - posicaoMundoAnterior;
        posicaoMundoAnterior = posicaoMundoAtual;
    }
}
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

    [Header("Medidor de Altura (Câmera)")]
    [Tooltip("Distância que o player precisa subir para travar no meio da tela")]
    public float alturaParaCentralizar = 3.5f;

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

    private float cargaAtual = 0f;
    private bool estaPulando = false;
    private int faixaAtual = 0;
    private float yInicial;
    private float alturaMaxima = 0f;

    void Start()
    {
        // Salva exatamente a posição Y onde o personagem começou (em cima do chão zero)
        yInicial = transform.position.y;
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

            // A trava do Chão Original! Se bater no zero, ele pousa perfeitamente.
            if (alturaAtual <= 0f)
            {
                alturaAtual = 0f;
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
    }
}
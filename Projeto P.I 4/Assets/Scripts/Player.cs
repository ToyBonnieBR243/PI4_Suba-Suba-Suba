using UnityEngine;

public class ControladorPulo : MonoBehaviour
{
    [Header("Configurações do Pulo")]
    public float forcaMaxima = 15f;     // O limite máximo da força do pulo
    public float taxaDeCarga = 20f;     // O quão rápido a força carrega enquanto segura a tela
    public float gravidade = 25f;       // A força que puxa o personagem de volta pra baixo

    [Header("Configurações da Queda")]
    // Um número negativo pequeno deixa a queda super suave e lenta!
    public float velocidadeMaximaQueda = -2f;

    // Essa variável é pública mas escondida do Inspector. 
    // Ela dita a velocidade do mundo inteiro!
    [HideInInspector]
    public float velocidadeVirtual = 0f;

    private float cargaAtual = 0f;
    private bool estaPulando = false;

    void Update()
    {
        // 1. O Freio de Mão (Agarrar na parede durante a queda/pulo)
        if (Input.GetMouseButtonDown(0) && estaPulando)
        {
            estaPulando = false;      // Desliga a gravidade
            velocidadeVirtual = 0f;   // Freia o cenário imediatamente
            cargaAtual = 0f;          // Prepara a carga para o próximo pulo
        }

        // 2. Carregando o Pulo (Tocar e segurar a tela)
        if (Input.GetMouseButton(0) && !estaPulando)
        {
            cargaAtual += taxaDeCarga * Time.deltaTime;

            if (cargaAtual > forcaMaxima)
            {
                cargaAtual = forcaMaxima;
            }
        }

        // 3. O Salto (Soltar o dedo)
        if (Input.GetMouseButtonUp(0) && !estaPulando)
        {
            velocidadeVirtual = cargaAtual;
            cargaAtual = 0f;
            estaPulando = true;
        }

        // 4. A Gravidade e Queda Lenta
        if (estaPulando)
        {
            velocidadeVirtual -= gravidade * Time.deltaTime;

            // Se a velocidade de queda passar do limite, nós travamos ela nesse limite lento
            if (velocidadeVirtual < velocidadeMaximaQueda)
            {
                velocidadeVirtual = velocidadeMaximaQueda;
            }
        }
    }
}
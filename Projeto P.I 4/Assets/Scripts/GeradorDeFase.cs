using UnityEngine;

public class GeradorFase : MonoBehaviour
{
    [Header("Configurações")]
    public GameObject prefabParede;
    public Transform paredeInicial;
    public float alturaDoBloco = 1f;

    [Tooltip("Atenção: Garanta que este número seja pelo menos 45 no Inspector!")]
    public int quantidadeDeBlocos = 45;

    private Transform[] blocos;
    private float alturaTotal;
    private float limiteInferior;
    private float limiteSuperior;

    void Start()
    {
        blocos = new Transform[quantidadeDeBlocos];
        blocos[0] = paredeInicial;

        // Calcula o tamanho exato da esteira inteira
        alturaTotal = quantidadeDeBlocos * alturaDoBloco;

        // A Mágica: Centraliza o cilindro perfeitamente no meio da tela (Y = 0)
        limiteInferior = -(alturaTotal / 2f);
        limiteSuperior = (alturaTotal / 2f);

        // Gera o resto dos blocos
        for (int i = 1; i < quantidadeDeBlocos; i++)
        {
            Vector3 novaPos = new Vector3(paredeInicial.position.x, paredeInicial.position.y + (i * alturaDoBloco), 0);
            GameObject novoBloco = Instantiate(prefabParede, novaPos, Quaternion.identity);
            blocos[i] = novoBloco.transform;
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < blocos.Length; i++)
        {
            Transform bloco = blocos[i];

            // Se desceu demais, joga para o topo
            while (bloco.position.y < limiteInferior)
            {
                bloco.position = new Vector3(bloco.position.x, bloco.position.y + alturaTotal, 0);
            }

            // Se subiu demais, joga para a base
            while (bloco.position.y > limiteSuperior)
            {
                bloco.position = new Vector3(bloco.position.x, bloco.position.y - alturaTotal, 0);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InspectorManager : MonoBehaviour
{
    public static InspectorManager Instance;

    [Header("Cursores")]
    public Texture2D cursorSeta;
    public Texture2D cursorMao;
    public Vector2 hotspot = Vector2.zero;

    [Header("Elementos de UI")]
    public GameObject painelInspecao;
    public Image displayImageZoom;
    public TextMeshProUGUI displayTextoFala;

    [Header("Detetive Gabriel")]
    public Image displayGabriel;
    public Sprite spriteGabrielIdle;
    public Sprite spriteGabrielAssustado;
    public Sprite spriteGabrielProtecao;

    [Header("Controle Jumpscare")]
    public RectTransform rectGabriel;
    public GameObject painelCreditos;
    public RectTransform fadePretoRect;
    public Button botaoAvancarFala;

    [Header("Ordem de Camadas")]
    public GameObject caixaTexto;

    private HashSet<string> pistasColetadas = new HashSet<string>();
    private bool inspecionando = false;
    private bool eventoClimaxAtivo = false;
    private int etapaClimax = 0;

    private Vector2 posicaoOriginalGabriel;
    private Vector2 tamanhoOriginalGabriel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        DefinirCursorSeta();
        painelInspecao.SetActive(false);
        if (painelCreditos != null) painelCreditos.SetActive(false);
        if (fadePretoRect != null) fadePretoRect.gameObject.SetActive(false);

        if (rectGabriel != null)
        {
            posicaoOriginalGabriel = rectGabriel.anchoredPosition;
            tamanhoOriginalGabriel = rectGabriel.sizeDelta;
        }

        if (botaoAvancarFala != null)
        {
            botaoAvancarFala.gameObject.SetActive(false);
        }

        DefinirGabrielIdle();
    }

    public void DefinirCursorSeta()
    {
        Cursor.SetCursor(cursorSeta, hotspot, CursorMode.Auto);
    }

    public void DefinirCursorMao()
    {
        if (!inspecionando)
        {
            Cursor.SetCursor(cursorMao, hotspot, CursorMode.Auto);
        }
    }

    public void DefinirGabrielIdle()
    {
        if (displayGabriel != null && spriteGabrielIdle != null)
        {
            displayGabriel.sprite = spriteGabrielIdle;
            displayGabriel.preserveAspect = true;
        }
    }

    public void RegistrarPistaColetada(string idPista)
    {
        pistasColetadas.Add(idPista);
    }

    public void AbrirInspecao(Sprite imagemGrande, string falaDetetive, Sprite expressaoGabriel = null)
    {
        if (eventoClimaxAtivo) return;

        inspecionando = true;
        DefinirCursorSeta();

        if (displayGabriel != null)
        {
            displayGabriel.transform.SetAsLastSibling();
        }

        if (botaoAvancarFala != null)
        {
            botaoAvancarFala.gameObject.SetActive(false);
        }

        displayImageZoom.gameObject.SetActive(true);
        displayImageZoom.sprite = imagemGrande;
        displayImageZoom.preserveAspect = true;

        displayTextoFala.text = falaDetetive;

        if (displayGabriel != null && expressaoGabriel != null)
        {
            displayGabriel.sprite = expressaoGabriel;
            displayGabriel.preserveAspect = true;
        }

        painelInspecao.SetActive(true);
    }

    public void FecharInspecao()
    {
        if (eventoClimaxAtivo) return;

        painelInspecao.SetActive(false);
        inspecionando = false;
        DefinirCursorSeta();
        DefinirGabrielIdle();

        if (pistasColetadas.Count >= 4)
        {
            IniciarClimax();
        }
    }

    private void IniciarClimax()
    {
        eventoClimaxAtivo = true;
        etapaClimax = 1;

        displayImageZoom.gameObject.SetActive(false);

        rectGabriel.anchoredPosition = new Vector2(0f, rectGabriel.anchoredPosition.y);
        rectGabriel.localScale = new Vector3(2f, 2f, 1f);

        displayGabriel.sprite = spriteGabrielAssustado;
        displayGabriel.preserveAspect = true;

        displayTextoFala.text = "Espere... Eu ouvi um barulho vindo da janela. Tinha... tinha um vulto ali?!";

        if (botaoAvancarFala != null)
        {
            botaoAvancarFala.gameObject.SetActive(true);
        }

        painelInspecao.SetActive(true);
        painelInspecao.transform.SetAsLastSibling();
    }

    public void CliqueAvancarClimax()
    {
        if (!eventoClimaxAtivo) return;

        if (etapaClimax == 1)
        {
            etapaClimax = 2;

            if (botaoAvancarFala != null)
            {
                botaoAvancarFala.gameObject.SetActive(false);
            }

            StartCoroutine(ExecutarJumpscare());
        }
    }

    private IEnumerator ExecutarJumpscare()
    {
        displayGabriel.sprite = spriteGabrielProtecao;
        displayGabriel.preserveAspect = true;

        displayTextoFala.text = "NÃO! AFASTE-SE DE MIM! AAAAAAHHH!!";

        //faz o personagem tremer na tela por 1.2 segundos
        yield return StartCoroutine(TremerGabriel(1.2f, 25f));

        yield return new WaitForSeconds(3.8f);

        yield return StartCoroutine(AnimarFadeCentro(1.5f));

        if(displayGabriel != null) displayGabriel.gameObject.SetActive(false);
        if (painelInspecao != null) painelInspecao.SetActive(false);

        if (painelCreditos != null)
        {
            painelCreditos.transform.SetAsLastSibling();
            painelCreditos.SetActive(true);
        }
    }

    private IEnumerator TremerGabriel(float duracao, float intensidade)
    {
        Vector2 posBase = rectGabriel.anchoredPosition;
        float decorrido = 0f;

        while (decorrido < duracao)
        {
            float offsetX = Random.Range(-intensidade, intensidade);
            float offsetY = Random.Range(-intensidade, intensidade);

            rectGabriel.anchoredPosition = posBase + new Vector2(offsetX, offsetY);

            decorrido += Time.deltaTime;
            yield return null;
        }

        rectGabriel.anchoredPosition = posBase;
    }

    private IEnumerator AnimarFadeCentro(float tempo)
    {
        if (fadePretoRect == null) yield break;

        fadePretoRect.gameObject.SetActive(true);
        fadePretoRect.localScale = Vector3.zero;

        float decorrido = 0f;

        while (decorrido < tempo)
        {
            float progresso = decorrido / tempo;
            float escala = Mathf.Lerp(0f, 35f, progresso * progresso);
            fadePretoRect.localScale = new Vector3(escala, escala, 1f);

            decorrido += Time.deltaTime;
            yield return null;
        }
        fadePretoRect.localScale = new Vector3(35f, 35f, 1f);
    }
}

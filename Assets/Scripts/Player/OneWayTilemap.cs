using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlatformEffector2D))]
public class OneWayTilemap : MonoBehaviour
{
    private PlatformEffector2D platformEffector;
    [SerializeField] private float waitTime = 0.3f;

    private void Awake()
    {
        platformEffector = GetComponent<PlatformEffector2D>();
    }

    private void Update()
    {
        // Kiedy gracz wciska 'S' lub strza³kê w dó³
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(DropThrough());
        }
    }

    private IEnumerator DropThrough()
    {
        // Obracamy k¹t o 180 stopni - platformy wpuszczaj¹ gracza w dó³
        platformEffector.rotationalOffset = 180f;

        yield return new WaitForSeconds(waitTime);

        // Przywracamy normaln¹ blokadê od góry
        platformEffector.rotationalOffset = 0f;
    }
}
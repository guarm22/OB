using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard1 : CustomDivergence
{
    public List<GameObject> pieces;
    public List<Vector3> movements;

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if (activate)
        {
            StartCoroutine(MovePieces(activate));
        }
        else {
            StartCoroutine(MovePieces(activate));
        }
    }

private IEnumerator MovePieces(bool activate)
{
    for (int i = 0; i < pieces.Count; i++)
    {
        if(activate) {
            StartCoroutine(MoveOverTime(pieces[i], movements[i], 1f));
        }
        else {
            StartCoroutine(MoveOverTime(pieces[i], -movements[i], 1f));
        
        }
        yield return new WaitForSeconds(1f);
    }
}

    private IEnumerator MoveOverTime(GameObject piece, Vector3 movement, float duration)
    {
        Vector3 startPosition = piece.transform.position;
        Vector3 targetPosition = startPosition + movement;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            piece.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        piece.transform.position = targetPosition;
    }
}
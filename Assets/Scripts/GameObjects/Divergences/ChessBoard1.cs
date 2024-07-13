using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard1 : CustomDivergence
{
    public List<GameObject> pieces;
    public List<Vector3> movements;
    public List<Vector3> newPositions;
    public List<Vector3> originalPositions;


    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if (activate)
        {
            //instantly move back to original position
            //this is in case this divergence was activated before and the pieces are not in their original position
            StartCoroutine(MovePieces(false, .1f, .1f));

            originalPositions = new List<Vector3>();
            newPositions = new List<Vector3>();
            foreach(GameObject piece in pieces) {
                originalPositions.Add(piece.transform.position);
                newPositions.Add(piece.transform.position + movements[pieces.IndexOf(piece)]);
            }
            StartCoroutine(MovePieces(activate, 1f, DivergenceControl.Instance.DivergenceInterval));
        }
        else {
            StopAllCoroutines();
            StartCoroutine(MovePieces(activate, .2f, .4f));
        }
    }

    private IEnumerator MovePieces(bool activate, float duration, float delay)
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if(activate) {
                if(pieces[i].transform.position == originalPositions[i]) {
                    StartCoroutine(MoveOverTime(pieces[i], newPositions[i], duration));
                }
            }

            else {
                if(pieces[i].transform.position != originalPositions[i]) {
                    StartCoroutine(MoveOverTime(pieces[i], originalPositions[i], duration));
                }
            }
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator MoveOverTime(GameObject piece, Vector3 newPos, float duration)
    {
        Vector3 startPosition = piece.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            piece.transform.position = Vector3.Lerp(startPosition, newPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        piece.transform.position = newPos;
    }
}
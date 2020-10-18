using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemMenuController : MonoBehaviour
{
    public GameObject EmptyRowPrefab;
    public Transform rowParent;
    PlayerController pc;
    List<GameObject> rows;

    private void OnEnable()
    {
        pc = GameController.instance.GetCurrentPlayer();
        List<BaseCard> cards = pc.cards;
        if (cards == null)
        {
            return;
        }
        rows = new List<GameObject>();
        int numCards = cards.Count;

        int i = 0;
        do
        {
            GameObject newRow = Instantiate(EmptyRowPrefab, rowParent);
            rows.Add(newRow);
            for (int j =0; j < 4; j ++)
            {
                int globalIndex = j + i * 4;
                if (globalIndex == numCards)
                {
                    break;
                }
                GameObject item = newRow.transform.GetChild(j).gameObject;
                item.SetActive(true);
                item.GetComponent<UseItemController>().Initialize(cards[globalIndex].type);


            }
            i++;
        } while (i <= numCards / 4);
    }

    private void OnDisable()
    {
        foreach (GameObject o in rows)
        {
            Destroy(o);
        }
        rows = null;
    }
}

using UnityEngine;

public class RepeatSprite : MonoBehaviour
{
    [SerializeField, Range(1, 20)] private int copiesEachSide = 4;

    private void Start()
    {
        float width = GetComponent<SpriteRenderer>().bounds.size.x;

        for (int i = -copiesEachSide; i <= copiesEachSide; i++)
        {
            if (i == 0) continue;

            // Копія стає дитиною того самого шару (BG_Far, BG_Mid...), тому паралакс рухає її разом
            GameObject copy = Instantiate(gameObject, transform.parent);
            Destroy(copy.GetComponent<RepeatSprite>());   // щоб копії не плодили копій
            copy.transform.position = transform.position + Vector3.right * width * i;
        }
    }
}
using UnityEngine;

public class Cell : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int x;
    public int y;
    public int type;

    public void UpdateCell(int x, int y, int type, Sprite wallSprite, Sprite pathSprite, Sprite goalSprite, Sprite visitedSprite)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        UpdateSprite(wallSprite, pathSprite, goalSprite, visitedSprite);
    }

    public void SetType(int newType, Sprite wallSprite, Sprite pathSprite, Sprite goalSprite, Sprite visitedSprite)
    {
        type = newType;
        UpdateSprite(wallSprite, pathSprite, goalSprite, visitedSprite);
    }

    private void UpdateSprite(Sprite wallSprite, Sprite pathSprite, Sprite goalSprite, Sprite visitedSprite)
    {
        switch (type)
        {
            case 0:
                spriteRenderer.sprite = pathSprite;
                break;
            case 1:
                spriteRenderer.sprite = wallSprite;
                break;
            case 3:
                spriteRenderer.sprite = goalSprite;
                break;
            case 4:
                spriteRenderer.sprite = visitedSprite;
                break;
        }
    }
}
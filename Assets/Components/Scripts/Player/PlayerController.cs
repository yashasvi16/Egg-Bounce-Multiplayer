using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Control Settings")]
    [SerializeField] float moveSpeed = 10;
    [SerializeField] float maxX;
    private float clickedScreenX;
    private float clickedPlayerX;

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            clickedScreenX = Input.mousePosition.x;
            clickedPlayerX = transform.position.x;
        }
        else if (Input.GetMouseButton(0))
        {
            float xDifference = Input.mousePosition.x - clickedScreenX;
            xDifference /= Screen.width;
            xDifference *= moveSpeed;

            float newXPosition = clickedPlayerX + xDifference;
            newXPosition = Mathf.Clamp(newXPosition, -maxX, maxX); 

            transform.position = new Vector2(newXPosition, transform.position.y);
        }
    }
}

using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("게임 시작!");
    }

    public float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");

        transform.Translate(
            Vector3.right * h * speed * Time.deltaTime
            );
    }
}

using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.3f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        Debug.Log("HeartPickup activo en: " + transform.position);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter tocado por: " + other.gameObject.name + " tag: " + other.tag);

        if (!other.CompareTag("Player"))
        {
            Debug.Log("No es Player, ignorando");
            return;
        }

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        Debug.Log("PlayerHealth encontrado: " + (ph != null));

        if (ph != null)
            ph.HealHeart();

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("COLISION (no trigger) con: " + other.gameObject.name);
    }
}
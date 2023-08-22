using UnityEngine;

public class BladeItem : MonoBehaviour
{
    public float rotateSpeed;

    private AudioClip pickupClip;

    private void Start()
    {
        pickupClip = Resources.Load<AudioClip>("AudioClips/ItemPickup");
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            AudioSource.PlayClipAtPoint(pickupClip, transform.position);
            other.GetComponent<CharacterBaseController>().PickUpItem(transform);
            Destroy(gameObject, 1);

        }
    }
}

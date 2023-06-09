using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    private float _fireDuration = 0.6f;

    public float _contactDamage = 0.5f;

    public float _coolDown = 0.5f;

    void Start()
    {
        StartCoroutine(PlayAnimation(Random.Range(0f, 1f)));
    }

    IEnumerator PlayAnimation(float time)
    {
        Animator animator = GetComponent<Animator>();

        yield return new WaitForSeconds(time);

        animator.SetBool("Playing", true);
    }

    public void DestroyFire()
    {
        Destroy(gameObject.GetComponentInChildren<Collider>());
        StartCoroutine(FadeFire(_fireDuration));
    }

    IEnumerator FadeFire(float fadeDuration)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Color initialColor = spriteRenderer.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

        GameObject.FindGameObjectWithTag("FireSFX").GetComponent<AudioSource>().Play();

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            spriteRenderer.material.color = Color.Lerp(
                initialColor,
                targetColor,
                elapsedTime / fadeDuration
            );
            yield return null;
        }
        Destroy(gameObject);
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        AnimalController animal = other.gameObject.GetComponent<AnimalController>();
        if (animal != null && other.gameObject.tag != "Enemy")
            StartCoroutine(TakeDamage(animal));
    }

    private void OnTriggerExit(Collider other)
    {
        AnimalController animal = other.gameObject.GetComponent<AnimalController>();
        if (animal != null && other.gameObject.tag != "Enemy")
            StopAllCoroutines();
    }

    IEnumerator TakeDamage(AnimalController animal)
    {
        while (true)
        {
            animal.BeDamaged(_contactDamage);
            yield return new WaitForSecondsRealtime(_coolDown);
        }
    }

    void Destroy()
    {
        StopAllCoroutines();
    }
}

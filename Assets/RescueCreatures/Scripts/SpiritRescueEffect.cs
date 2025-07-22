using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class SpiritRescueEffect : MonoBehaviour
{
    public GameObject beforeSprite;
    public GameObject afterSprite;
    public VisualEffect rescueParticles;
    public AudioSource sfxRescue;
    public Light2D healingLight;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            PlayRescueEffect();
        }
    }

    public void PlayRescueEffect()
    {
        // Fade out sprite A
        var spriteA = beforeSprite.GetComponent<SpriteRenderer>();
        spriteA.DOFade(0, 1f).OnComplete(() =>
        {
            beforeSprite.SetActive(false);

            // Play particle + sound
            if (rescueParticles != null) rescueParticles.Play();
            if (sfxRescue != null) sfxRescue.Play();

            // Activate sprite B and fade in
            afterSprite.SetActive(true);
            var spriteB = afterSprite.GetComponent<SpriteRenderer>();
            spriteB.color = new Color(1, 1, 1, 0); // start transparent
            spriteB.DOFade(1, 1f);

            // Optional: scale bounce effect
            afterSprite.transform.localScale = Vector3.zero;
            afterSprite.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        });
    }


    public void PlayHealingEffect()
    {
        StartCoroutine(HealingCoroutine());
    }

    private IEnumerator HealingCoroutine()
    {
        float duration = 1.5f;
        float t = 0;

        // Fade in ánh sáng
        while (t < duration)
        {
            t += Time.deltaTime;
            healingLight.intensity = Mathf.Lerp(0, 1.5f, t / duration);
            healingLight.pointLightOuterRadius = Mathf.Lerp(0.1f, 3f, t / duration);
            yield return null;
        }

        // Fade out ánh sáng
        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            healingLight.intensity = Mathf.Lerp(1.5f, 0, t / duration);
            healingLight.pointLightOuterRadius = Mathf.Lerp(3f, 0.1f, t / duration);
            yield return null;
        }

        healingLight.enabled = false;
    }
}
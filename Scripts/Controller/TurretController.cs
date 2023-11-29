using System;
using System.Collections;
using System.ComponentModel.Design;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    #region Init   

    // Main objects 
    public GameObject spriteObject;
    public GameObject borderObject;
    public GameObject canvasObject;
    // Stat Objects
    public GameObject attackObject;
    public GameObject magicObject;
    public GameObject healthObject;
    public GameObject manaObject;
    public GameObject armourObject;
    // Fx Objects
    public GameObject lockObject;    

    /* Properties */
    private string defaultLayerName;
    public int shopIndex;

    void Start()
    {
        defaultLayerName = canvasObject.GetComponent<Canvas>().sortingLayerName;
    }
   
    /* State */
    public Player owner;
    public Turret turret;
    public bool g_dragged = false;

    /* Colouring */
    public Color rageColour = Color.red;
    public Color frozenColour = Color.cyan;
    public Color lastColour = Color.white;
    public Color lockedColour = Color.grey;

    #endregion Init

    #region Turret

    public void SetSprite(Sprite sprite) {
        spriteObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }
    public void SetActive(bool active = true) {
        gameObject.SetActive(active);
    }
    
    // Stats
    public void SetStats(int attack, int magic, int health, int mana, int armour) {

        attackObject.GetComponentInChildren<TextMeshProUGUI>().text = attack.ToString();
        magicObject.GetComponentInChildren<TextMeshProUGUI>().text = magic.ToString();
        healthObject.GetComponentInChildren<TextMeshProUGUI>().text = health.ToString();
        manaObject.GetComponentInChildren<TextMeshProUGUI>().text = mana.ToString();
        armourObject.GetComponentInChildren<TextMeshProUGUI>().text = armour.ToString();

        if (attack != 0 && attack > magic) {
            attackObject.SetActive(true);
        } else attackObject.SetActive(false);
        if (magic != 0 && magic > attack) {
            magicObject.SetActive(true);
        } else magicObject.SetActive(false);

        if (health != 0) {
            healthObject.SetActive(true);
        } else healthObject.SetActive(false);
        if (mana != 0) {
            manaObject.SetActive(true);
        } else manaObject.SetActive(false);
        if (armour != 0) {
            armourObject.SetActive(true);
        } else armourObject.SetActive(false);
    }
    public void SetStats(BasicStats stats) {
        SetStats(stats.attack, stats.magic, stats.health, stats.mana, stats.armour);
    }
    public void SetHealth(int health) {
        healthObject.GetComponentInChildren<TextMeshProUGUI>().text = health.ToString();
        if (health > 0) {
            healthObject.SetActive(true);
        } else healthObject.SetActive(false);
    }
    
    // All
    public void SetTurret(Turret turret) {
        if (turret.name == TurretName.Empty) SetSprite(Main.GetSprite(Main.BASE_SPRITE));
        else SetSprite(Main.GetSprite(turret.name));
        SetStats(turret.stats);
        owner = turret.player;
        this.turret = turret;
    }
    public void OverwriteTurret(Turret turret) {        
        if (turret.name == TurretName.Empty) SetSprite(Main.GetSprite(Main.BASE_SPRITE));
        else SetSprite(Main.GetSprite(turret.name));
        SetStats(turret.stats);
        turret.player = owner;
        this.turret = turret;
    }
    
    // Border
    public void SetBorder(Color c) {
        borderObject.SetActive(true);
        borderObject.GetComponent<SpriteRenderer>().color = c;
    }
    public void DisableBorder() {
        borderObject.SetActive(false);
    }

    // Lock
    public void SetLock(bool active = true) {
        if (active) {
            spriteObject.GetComponent<SpriteRenderer>().color = lockedColour;
            lockObject.SetActive(true);
        } else {
            spriteObject.GetComponent<SpriteRenderer>().color = lastColour;
            lockObject.SetActive(false);
        }
    }

    public void Clear() {

        attackObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
        magicObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
        healthObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
        manaObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
        armourObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
        attackObject.SetActive(false);
        magicObject.SetActive(false);
        healthObject.SetActive(false);
        manaObject.SetActive(false);
        armourObject.SetActive(false);
        
        SetSprite(Main.EmptySprite());
        spriteObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void SetLayer(string layerName) {
        SpriteRenderer[] renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        Canvas[] canvases = gameObject.GetComponentsInChildren<Canvas>();

        foreach (SpriteRenderer renderer in renderers) {
            renderer.sortingLayerName = layerName;
        }
        foreach (Canvas canvas in canvases) {
            canvas.sortingLayerName = layerName;
        }

        // canvasObject.GetComponent<Canvas>().sortingLayerName = layerName;
        // borderObject.GetComponent<SpriteRenderer>().sortingLayerName = layerName;

        // attackObject.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
        // magicObject.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
        // healthObject.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
        // manaObject.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
        // armourObject.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
        // spriteObject.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
    }

    public void SetDefaultLayer(string layerName) {
        SetLayer(layerName);
        defaultLayerName = layerName;
    }

    public void Freeze(bool frozen = true) {
        if (frozen) {
            spriteObject.GetComponent<SpriteRenderer>().color = frozenColour;
        }
        else {
            spriteObject.GetComponent<SpriteRenderer>().color = lastColour; // lastColour;
        }
    }

    public void ResetLayer() {
        SetLayer(defaultLayerName);
    }

    #endregion Turret

    #region Global

    bool animRunning = false;

    #endregion Global

    #region Reaction

    /* Note physics anims are in seconds (s) not milliseconds (ms). */
    public IEnumerator Reaction__HurtAnim(float speed = 1.0f) {
        animRunning = true;

        float totalTime = 0.07f / speed; // seconds
        float time = 0;
        float t = 0; float theta = 0; // parameterisation

        SpriteRenderer rend = spriteObject.GetComponent<SpriteRenderer>();
        Color original = Color.white;
        Color red = Color.red;

        while (time < totalTime) {
            t = time / totalTime;

            theta = t * (float) Math.PI;
            rend.color = red * (float) Math.Sin(theta) + (float) (1 - Math.Sin(theta)) * original;
            
            time += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        rend.color = original;

        animRunning = false;
        yield break;
    }
    
    public IEnumerator Reaction__CritHurtAnim(float speed = 1.0f) {
        animRunning = true;
        animRunning = false;
        yield break;
    }

    public IEnumerator Reaction__TriggerAnim(float speed = 1.0f) {
        animRunning = true;
        animRunning = false;
        yield break;
    }

    public IEnumerator Reaction__AttackAnim(float speed = 1.0f) {
        animRunning = true;
        StartCoroutine(Reaction__BounceAnim());
        animRunning = false;
        yield break;
    }

    public IEnumerator Reaction_MissAnim(float speed = 1.0f) {
        animRunning = true;
        animRunning = false;
        yield break;
    }

    public IEnumerator Reaction__CritAnim(float speed = 1.0f) {
        animRunning = true;
        animRunning = false;
        yield break;
    }

    // Do in terms of: 'relative to current state' -> then anims won't entangle up wrongly TODO
    // e.g. don't start with the current scale and last scale.
    // Start with a scalar, and tune the scalar to current scale * scalar
    public IEnumerator Reaction__DeathAnim(float speed = 1.0f) {
        animRunning = true;

        // Fade out
        float totalTime = 0.5f / speed; // seconds
        float time = 0;
        float t = 0; float theta = 0; // parameterisation

        SpriteRenderer rend = spriteObject.GetComponent<SpriteRenderer>();
        Color original = Color.white;
        Color fade = original;
        fade.a = 0;

        while (time < totalTime) {
            t = time / totalTime;

            theta = t * (float) Math.PI;
            rend.color = fade * (float) Math.Sin(theta) + (float) (1 - Math.Sin(theta)) * original;
            
            time += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        yield return new WaitForSeconds(0.05f);
        
        // Then empty
        Clear();
        SetSprite(Main.GetSprite("empty"));
        rend.color = original;

        // Then move it back
    
        animRunning = false;
        yield break;
    }

    /* Friendly */
    public IEnumerator Reaction__Cute() {
        yield break;
    }

    public IEnumerator Reaction__Idle1() {
            yield break;
    }

    public IEnumerator Reaction__Idle2() {
            yield break;
    }

    public IEnumerator Reaction__Idle3() {
            yield break;
    }

    public IEnumerator Reaction__LookAtNewFriend() {
            yield break;
    }

    public IEnumerator Reaction__LookAtDraggedFriend() {
            yield break;
    }

    /* Base */
    public IEnumerator Reaction__BounceAnim(float speed = 1.0f) {
        animRunning = true;
        float totalTime = 0.12f / speed; // seconds
        float time = 0;
        float t = 0; float theta = 0; // parameterisation

        Vector3 currScale = spriteObject.transform.localScale;
        Vector3 enlargedScale = currScale * 1.2f;

        while (time < totalTime) {
            t = time / totalTime;

            theta = t * (float) Math.PI;
            spriteObject.transform.localScale = enlargedScale * (float) Math.Sin(theta) + (float) (1 - Math.Sin(theta)) * currScale;
            
            time += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        spriteObject.transform.localScale = currScale;

        animRunning = false;
        yield break;
    }

    public IEnumerator Reaction__Shrink(float speed = 1.0f) {
        // TODO
        yield break;
    }

    public IEnumerator Reaction__Vibrate(float vibrate = 1.0f, float speed = 1.0f) {

        // Vibration distance
        float initialVibrate = vibrate / 2;
        float currVibrate = initialVibrate;
        // Animation time
        float totalTime = 1.0f / speed;
        float time = 0;
        float t = 0;
        // Rate of moving
        float rate = totalTime / 25;
        float rateTime = 0;

        Vector3 prevVector = new Vector3(0, 0, 0);

        while (time < totalTime) {
            time += Time.fixedDeltaTime;
            rateTime += Time.fixedDeltaTime;

            if (rateTime > rate) {
                rateTime = 0;
                t = time/totalTime;
                currVibrate = (float) Math.Sin(t * Math.PI) * vibrate + (float) (1-Math.Sin(t * Math.PI)) * initialVibrate;

                // Reset position, then move it randomly
                transform.position -= prevVector; // Return
                prevVector = new Vector3(UnityEngine.Random.Range(-currVibrate, currVibrate), UnityEngine.Random.Range(-currVibrate, currVibrate), 0); // Randomise
                transform.position += prevVector; // Move
            }

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        transform.position -= prevVector;

        yield break;
    }

    #endregion Reaction
}

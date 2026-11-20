using UnityEngine;

public class GameplayMusic : MonoBehaviour
{
    public AudioSource music;
    public TestEnemyAIScript[] enemies;
    public PlayerInteractor player;
    public float normalVolume = .07f;
    public float chaseVolume = .10f;

    void Update()
    {
        float volume = normalVolume;
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.isChasing) { volume = chaseVolume; }
        }
        if (player.collectTokenSound.isPlaying || player.ventIn.isPlaying || player.ventOut.isPlaying)
        {
            volume *= .45f;
        }
        if (!player.LMS.canInput) { volume = .03f; }
        music.volume = Mathf.MoveTowards(music.volume,volume,Time.unscaledDeltaTime * .4f);
    }
}

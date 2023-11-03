using System.Linq;
using UnityEngine;

public class MagDrop : MonoBehaviour
{
    [SerializeField]
    private GameObject[] magazinePrefab;
    [SerializeField]
    private Transform[] spawnPoint;
    private GameObject instatiatedMagazine;
    private string activeGun;
    private EnemyAi enemyAi;

    private void Awake()
    {
        enemyAi = GetComponent<EnemyAi>();
        activeGun = enemyAi.activeGun.name;
    }
    public void MagazineDrop()
    {
        GameObject magPrefab = magazinePrefab.First(x => x.name == activeGun + " Mag");
        instatiatedMagazine = Instantiate(magPrefab);
        Transform sP = spawnPoint.First(x => x.name == activeGun + "_MagPositionLeft");
        instatiatedMagazine.transform.SetPositionAndRotation(sP.position, sP.rotation);
        Destroy(instatiatedMagazine, 5f);
    }
}

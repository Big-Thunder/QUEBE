using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemyStateIndicator : MonoBehaviour
{
    public EnemyBehaviour enemyBehaviour;
    public Material FollowPlayerMatt;
    public Material TakeCoverMatt;
    public MeshRenderer meshRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyBehaviour = GetComponent<EnemyBehaviour>();
        // meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyBehaviour.canFollowPlayer)
        {
            meshRenderer.material = FollowPlayerMatt;
        }
        else
        {
            meshRenderer.material = TakeCoverMatt;
        }
    }
}

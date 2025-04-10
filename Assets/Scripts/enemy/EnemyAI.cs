
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target; // Jugador
    private NavMeshAgent agent;
    public float stoppingDistance = 2.0f; // Distancia mínima para detenerse
    public static bool shoot;

    public bool isSurviveActive = true; // Variable booleana para controlar si el enemigo puede moverse
    private Animator animate;
    public bool perseguir;
    
    public APUNTADO apuntar;
    public bool muerto;
    public int randomnum;
    public bool gun,ar,sub;
    void Start()
    {
        apuntar = GetComponentInChildren<APUNTADO>();
        randomnum = Random.Range(0, 1);
        animate = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent.stoppingDistance = stoppingDistance;

        if (target == null)
        {
            Debug.LogWarning("No se encontró un jugador en la escena.");
        }
    }
    private void OnEnable()
    {
        muerto= false;
        
    }

    private void OnDisable()
    {
        muerto = false;
    }
    public void Muerto()
    { 
    muerto = true;
    }
    public void vivo()
    {
        muerto = false;
    }
    void Update()
    {
        if(animate !=null) animate.SetBool("Correr", perseguir);
        if (animate != null) animate.SetBool("morido", muerto);
        
        if (animate != null) animate.SetBool("gun", gun);
        if (animate != null) animate.SetBool("AR", ar);
        if (animate != null) animate.SetBool("Sub", sub);
        // Comprobar si la variable isSurviveActive es false
        if (!isSurviveActive || muerto)
        {
            agent.isStopped = true;
            return; // Si isSurviveActive es false, no hacer nada
        }

        // Lógica de movimiento y comportamiento del enemigo si isSurviveActive es true
        if (target != null && !shoot && !muerto)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance > stoppingDistance)
            {perseguir = true;
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else
            {
                perseguir = false;
                agent.isStopped = true; // Detenerse cuando está cerca
            }
        }
        else
        {
            perseguir = false;
            agent.isStopped = true; // No moverse si está disparando o no hay objetivo
        }
    }
}

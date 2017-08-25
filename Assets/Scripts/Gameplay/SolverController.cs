
using UnityEngine;

public class SolverController : BotController {

    protected override void PathEndAction()
    {
        base.PathEndAction();
        Debug.Log("solver found path end");
        Destroy(this.gameObject);
    }

}

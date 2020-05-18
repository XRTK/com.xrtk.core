using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Start()
    {
        var operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        operation.completed += Operation_completed;
    }

    private void Operation_completed(AsyncOperation operation)
    {
        operation.completed -= Operation_completed;
        Destroy(gameObject);
    }
}

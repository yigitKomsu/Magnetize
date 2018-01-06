using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {
    [SerializeField]
    private Animator _animator;
    public int sequence = 0;

    public void Continue()
    {
        sequence++;
        _animator.SetTrigger("Skip");
        if(sequence > 10)
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            SceneManager.LoadScene(ProjectConstants.MainMenu);
        }
    }

}

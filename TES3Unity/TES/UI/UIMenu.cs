using UnityEngine;
using UnityEngine.SceneManagement;

namespace TES3Unity.UI
{
    public class UIMenu : UIWindow
    {
        public void Resume() => NotifyCloseRequest();

        public void Save()
        {
            var tes = TES3Engine.Inst;
            var player = GameObject.FindWithTag("Player");

            var save = TES3Save.Get();
            save.Save(tes.CurrentCell, player.transform);
        }

        public void Quit()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}

namespace TES3Unity.UI
{
    public class UIJournal : UIBook
    {
        protected override void Start()
        {
            base.Start();

            _page1.text = "A journey begins...";
            _page2.text = "";
        }
    }
}

using UnityEngine;

namespace TacticalBreach.UI
{
    public sealed class ResultScreenController : MonoBehaviour
    {
        [SerializeField] private string successTitleKey = "ui.result.success.title";
        [SerializeField] private string failTitleKey = "ui.result.fail.title";
        [SerializeField] private string successBodyKey = "ui.result.success.body";
        [SerializeField] private string failBodyKey = "ui.result.fail.body";

        public string GetTitleKey(bool success)
        {
            return success ? successTitleKey : failTitleKey;
        }

        public string GetBodyKey(bool success)
        {
            return success ? successBodyKey : failBodyKey;
        }
    }
}

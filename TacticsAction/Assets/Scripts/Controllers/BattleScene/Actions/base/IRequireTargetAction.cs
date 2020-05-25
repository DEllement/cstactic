using Model;

namespace Controllers.BattleScene.Actions
{
    public interface IRequireTargetAction : IActionCommand
    {
        void OnTargetsOver();
        void OnTargetsSelected();
    }
}
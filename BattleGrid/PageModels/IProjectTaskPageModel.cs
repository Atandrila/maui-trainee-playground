using BattleGrid.Models;
using CommunityToolkit.Mvvm.Input;

namespace BattleGrid.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}
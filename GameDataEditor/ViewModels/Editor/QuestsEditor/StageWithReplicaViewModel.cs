using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using GameDataEditor.Models;
using MarkOne.Scripts.GameCore.Quests.Characters;
using MarkOne.Scripts.GameCore.Quests.QuestStages;

namespace GameDataEditor.ViewModels.Editor.QuestsEditor;

public class StageWithReplicaViewModel : ViewModelBase
{
    private EnumValueModel<CharacterType> _selectedCharacter;
    private Answer? _selectedAnswer;

    public QuestStageWithReplica stage { get; }
    public ObservableCollection<EnumValueModel<CharacterType>> characters { get; }
    public ObservableCollection<Answer> answers { get; } = new ObservableCollection<Answer>();
    public EnumValueModel<CharacterType> selectedCharacter
    {
        get => _selectedCharacter;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedCharacter, value);
            stage.replica.characterType = value.value;
        }
    }
    public Answer? selectedAnswer
    {
        get => _selectedAnswer;
        set => this.RaiseAndSetIfChanged(ref _selectedAnswer, value);
    }

    public ReactiveCommand<Unit, Unit> addNewAnswerCommand { get; }
    public ReactiveCommand<Unit, Unit> removeAnswerCommand { get; }

    public StageWithReplicaViewModel(QuestStageWithReplica stage)
    {
        this.stage = stage;
        characters = EnumValueModel<CharacterType>.CreateCollection();
        _selectedCharacter = characters.First(x => x.value == stage.replica.characterType);

        foreach (var answer in stage.replica.answers)
        {
            answers.Add(answer);
        }

        addNewAnswerCommand = ReactiveCommand.Create(AddNewAnswer);
        removeAnswerCommand = ReactiveCommand.Create(RemoveSelectedAnswer);
    }

    private void AddNewAnswer()
    {
        var answer = new Answer();
        stage.replica.answers.Add(answer);
        answers.Add(answer);
    }

    private void RemoveSelectedAnswer()
    {
        if (_selectedAnswer == null)
            return;

        var answer = _selectedAnswer;
        stage.replica.answers.Remove(answer);
        answers.Remove(answer);
    }

}

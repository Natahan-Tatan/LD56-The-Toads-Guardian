using Godot;
using System;

namespace Game
{
    public class EndPanel : PanelContainer
    {
#region Signals
#endregion

#region Exported Properties
#endregion

#region Public Properties
        public int CountSpawned
        {
            set 
            {
                if(_labelSpawned != null)
                {
                    _countSpawned = value;
                    _labelSpawned.Text = value.ToString();
                }
            }
        }
        private int _countSpawned = 0;

        public int CountArrived
        {
            set 
            {
                if(_labelArrived != null)
                {
                    _countArrived = value;
                    _labelArrived.Text = value.ToString();
                }
            }
        }
        private int _countArrived = 0;

        public int CountDied
        {
            set 
            {
                if(_labelArrived != null)
                {
                    _labelDied.Text = value.ToString();
                }
            }
        }

        public bool Pass 
        {
            get => _pass;
        }
        private bool _pass = false;
#endregion

#region Internal Properties
        private Label _labelSpawned;
        private Label _labelArrived;
        private Label _labelDied;
        private Label _labelRatio;
        private Rank _rank;
        private Label _resultPASS;
        private Label _resultFAIL;
        private AnimationPlayer _animPlayer;
#endregion

#region Public Methods
        public override void _Ready()
        {
            _labelSpawned = this.GetNode<Label>("VBoxContainer/HBoxContainer/VBoxContainer/CountSpawned/Number");
            _labelArrived = this.GetNode<Label>("VBoxContainer/HBoxContainer/VBoxContainer/CountArrived/Number");
            _labelDied = this.GetNode<Label>("VBoxContainer/HBoxContainer/VBoxContainer/CountDead/Number");
            _labelRatio = this.GetNode<Label>("VBoxContainer/HBoxContainer/VBoxContainer/Ratio/Number");

            _rank = this.GetNode<Rank>("VBoxContainer/HBoxContainer/Rank");

            _resultPASS = this.GetNode<Label>("VBoxContainer/ResultContainer/PASS");
            _resultFAIL = this.GetNode<Label>("VBoxContainer/ResultContainer/FAIL");

            _animPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
        }

        public void DisplayResult()
        {
            if(_animPlayer != null)
            {
                _animPlayer.Play("RESET");
            }

            int ratio = Mathf.RoundToInt(((float)_countArrived / (float)_countSpawned) * 100);

            if(_labelRatio != null)
            {
                _labelRatio.Text = $"{ratio} %";
            }

            Rank.Type rank = Rank.Type.S;
            _pass = true;

            if(ratio == 100)
            {
                rank = Rank.Type.S;
            }
            else if(ratio >= 90)
            {
                rank = Rank.Type.A;
            }
            else if(ratio >= 75)
            {
                rank = Rank.Type.B;
            }
            else if(ratio >= 50)
            {
                rank = Rank.Type.C;
            }
            else
            {
                rank = Rank.Type.F;
                _pass = false;
            }

            if(_rank != null)
            {
                _rank.RankType = rank;
            }

            if(_resultPASS != null && _resultFAIL != null)
            {
                if(_pass)
                {
                    _resultPASS.Visible = true;
                    _resultFAIL.Visible = false;
                }
                else
                {
                    _resultPASS.Visible = false;
                    _resultFAIL.Visible = true;
                }
            }

            this.Visible = true;

            if(_animPlayer != null)
            {
                _animPlayer.Play("Unveilling");
            }
        }
#endregion

#region Internal Methods
#endregion

#region Signals Hooks
        public void _on_Game_LevelFinished(int countSpawned, int countArrived, int CountDead)
        {
            this.CountSpawned = countSpawned;
            this.CountArrived = countArrived;
            this.CountDied = CountDead;

            this.DisplayResult();
        }

#endregion
    }
}

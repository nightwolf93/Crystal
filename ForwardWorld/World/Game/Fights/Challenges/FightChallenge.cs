using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal.WorldServer.World.Game.Fights.Challenges
{
    public abstract class FightChallenge
    {
        public Fight ChallengedFight { get; set; }
        public FightTeam ChallengedTeam { get; set; }

        public bool IsAlive = true;
        public bool Success = false;

        public Fighter Target { get; set; }

        public virtual int EarnedExp { get { return 0; } }
        public virtual int EarnedDrop { get { return 0; } }

        public Enums.ChallengesTypeID Type { get; set; }

        public FightChallenge(Fight fight, FightTeam team, Enums.ChallengesTypeID type)
        {
            this.ChallengedFight = fight;
            this.ChallengedTeam = team;
            this.Type = type;
        }

        public bool ChallengeMustHaveTarget
        {
            get
            {
                //TODO !
                return true;
            }
        }

        public virtual void OnMove(Fighter fighter, int value) { }
        public virtual void OnMosterDie(Fighter fighter, Fighter monster) { }
        public virtual void OnHeal(Fighter healer, Fighter healed) { }
        public virtual void OnEndTurn(Fighter fighter) { }

        public void ChallengeSuccess()
        {
            if (this.IsAlive)
            {
                this.Success = true;
                this.IsAlive = false;
                this.ChallengedTeam.Send("GdOK" + (int)this.Type);
            }
        }

        public void ChallengeFailed(Fighter failer)
        {
            if (this.IsAlive)
            {
                this.Success = false;
                this.IsAlive = false;
                this.ChallengedTeam.Send("GdKO" + (int)this.Type);
                this.ChallengedTeam.Send("Im0188;" + failer.Nickname);
            }
        }

        public void ShowChallenges()
        {
            StringBuilder builder = new StringBuilder("Gd");
            builder.Append((int)this.Type)
                   .Append(";")
                   .Append(this.ChallengeMustHaveTarget ? "1" : "0")
                   .Append(";")
                   .Append(this.Target != null ? this.Target.ID.ToString() + ";" : ";")//TODO : Target
                   .Append(this.EarnedExp.ToString())
                   .Append(";0;")
                   .Append(this.EarnedDrop.ToString())
                   .Append(";0;")
                   .Append((int)this.Type);
            this.ChallengedTeam.Send(builder.ToString());
        }
    }
}

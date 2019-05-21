using System.Runtime.Serialization;

namespace CakeBot.Helper.Modules.BF4.Model
{
    [DataContract]
    public class Bf4Model
    {
        [DataMember]
        public PlayerData player { get; set; }
        [DataMember]
        public PlayerStats stats { get; set; }
    }

    public class PlayerData
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string plat { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string blPlayer { get; set; }

    }

    public class PlayerStats
    {
        [DataMember]
        public int skill { get; set; }
        [DataMember]
        public int rank { get; set; }
        [DataMember]
        public int timeplayed { get; set; }
        [DataMember]
        public int kills { get; set; }
        [DataMember]
        public int deaths { get; set; }
        [DataMember]
        public int headshots { get; set; }
        //Accuracy
        [DataMember]
        public int shotsFired { get; set; }
        [DataMember]
        public int shotsHit { get; set; }
        //Win loss
        [DataMember]
        public int numRounds { get; set; }
        [DataMember]
        public int numLosses { get; set; }
        [DataMember]
        public int numWins { get; set; }
        [DataMember]
        public int killStreakBonus { get; set; }

    }
    // Lets test.


}

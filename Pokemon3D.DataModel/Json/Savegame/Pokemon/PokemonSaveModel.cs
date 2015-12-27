﻿using System.Runtime.Serialization;
using Pokemon3D.DataModel.Json.Pokemon;

// Disable Code Analysis for warning CS0649: Field is never assigned to, and will always have its default value.
#pragma warning disable 0649

namespace Pokemon3D.DataModel.Json.Savegame.Pokemon
{
    [DataContract]
    public class PokemonSaveModel : JsonDataModel<PokemonSaveModel>
    {
        [DataMember(Order = 0)]
        public int Id;

        [DataMember(Order = 1, Name = "Gender")]
        private string _gender;

        public PokemonGender Gender
        {
            get { return ConvertStringToEnum<PokemonGender>(_gender); }
            set { _gender = value.ToString(); }
        }

        [DataMember(Order = 2)]
        public PokemonStatSetModel IVs;

        [DataMember(Order = 3)]
        public bool IsShiny;

        [DataMember(Order = 4)]
        public int AbilityId;

        [DataMember(Order = 5)]
        public int NatureId;

        [DataMember(Order = 6)]
        public string OT;

        [DataMember(Order = 7)]
        public PokemonCatchInfo CatchInfo;

        [DataMember(Order = 8)]
        public string PersonalityValue;

        [DataMember(Order = 9)]
        public int HP;

        [DataMember(Order = 10)]
        public int Experience;

        [DataMember(Order = 11)]
        public string Nickname;

        [DataMember(Order = 12)]
        public int Friendship;

        [DataMember(Order = 13)]
        public HeldItemModel Item;

        [DataMember(Order = 14, Name = "Status")]
        private string _status;

        public PokemonStatus Status
        {
            get { return ConvertStringToEnum<PokemonStatus>(_status); }
            set { _status = value.ToString(); }
        }

        [DataMember(Order = 15)]
        public PokemonStatSetModel EVs;

        [DataMember(Order = 16)]
        public int EggSteps;

        [DataMember(Order = 17)]
        public string AdditionalData;

        [DataMember(Order = 18)]
        public PokemonMoveModel[] Moves;

        public override object Clone()
        {
            var clone = (PokemonSaveModel)MemberwiseClone();
            clone.IVs = IVs.CloneModel();
            clone.CatchInfo = CatchInfo.CloneModel();
            clone.EVs = EVs.CloneModel();
            clone.Item = Item.CloneModel();
            clone.Moves = (PokemonMoveModel[])Moves.Clone();
            return clone;
        }
    }
}
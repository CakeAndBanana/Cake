﻿using System;

namespace CakeBot.Helper.Modules.MAL
{
    public enum MalAnimeGenreEnum
    {
        Action = 1,
        Adventure = 2,
        Cars = 3,
        Comedy = 4,
        Dementia = 5,
        Demons = 6,
        Mystery = 7,
        Drama = 8,
        Ecchi = 9,
        Fantasy = 10,
        Game = 11,
        Hentai = 12,
        Historical = 13,
        Horror = 14,
        Kids = 15,
        Magic = 16,
        MartialArts = 17,
        Mecha = 18,
        Music = 19,
        Parody = 20,
        Samurai = 21,
        Romance = 22,
        School = 23,
        SciFi = 24,
        Shoujo = 25,
        ShoujoAi = 26,
        Shounen = 27,
        ShounenAi = 28,
        Space = 29,
        Sports = 30,
        SuperPower = 31,
        Vampire = 32,
        Yaoi = 33,
        Yuri = 34,
        Harem = 35,
        SliceOfLife = 36,
        Supernatural = 37,
        Military = 38,
        Police = 39,
        Psychological = 40,
        Thriller = 41,
        Seinen = 42,
        Josei = 43
    }

    public enum MalMangaGenreEnum
    {
        Action = 1,
        Adventure = 2,
        Cars = 3,
        Comedy = 4,
        Dementia = 5,
        Demons = 6,
        Mystery = 7,
        Drama = 8,
        Ecchi = 9,
        Fantasy = 10,
        Game = 11,
        Hentai = 12,
        Historical = 13,
        Horror = 14,
        Kids = 15,
        Magic = 16,
        MartialArts = 17,
        Mecha = 18,
        Music = 19,
        Parody = 20,
        Samurai = 21,
        Romance = 22,
        School = 23,
        SciFi = 24,
        Shoujo = 25,
        ShoujoAi = 26,
        Shounen = 27,
        ShounenAi = 28,
        Space = 29,
        Sports = 30,
        SuperPower = 31,
        Vampire = 32,
        Yaoi = 33,
        Yuri = 34,
        Harem = 35,
        SliceOfLife = 36,
        Supernatural = 37,
        Military = 38,
        Police = 39,
        Psychological = 40,
        Seinen = 41,
        Josei = 42,
        Doujinshi = 43,
        GenderBender = 44,
        Thriller = 45
    }

    public class MalEnumHelper
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}

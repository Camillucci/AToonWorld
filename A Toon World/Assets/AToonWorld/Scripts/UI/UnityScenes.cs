﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.UI
{
    public static class UnityScenes
    {
        #region Paths

        public const string LevelsPath = "AToonWorld/Scenes/Levels/";
        public const string MenusPath = "AToonWorld/Scenes/Menus/";
        public const string AchievementsPath = "/Achievements";
        public const string CollectiblesPath = "/Collectibles";
        public static readonly string[] AchievementPaths = {
            AchievementsPath + "/Time",
            AchievementsPath + "/Deaths",
            AchievementsPath + "/InkUsage",
        };
        public const string TimePath = "/Time";
        public const string DeathsPath = "/Deaths";
        public const string InkUsagePath = "/InkUsage";

        #endregion
        
        #region StandaloneScenes

        public const string MainMenu = "AToonWorld/Scenes/Menus/MainMenu";
        public const string LevelsMenu = "AToonWorld/Scenes/Menus/LevelsMenu";

        #endregion

        #region Levels

        // List of all levels with paths, valid index start from 1
        public static readonly string[] Levels = {
            "We count levels from number one",
            LevelsPath + "build_tutorial_000",
            LevelsPath + "damage_tutorial_002",
            LevelsPath + "climb_tutorial_v2_003",
            LevelsPath + "death_cave_01",
            LevelsPath + "climb_tutorial_001",
            MenusPath + "ThanksForPlaying",
        };

        #endregion
    }
}

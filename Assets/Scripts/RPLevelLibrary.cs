using System.Collections.Generic;
using UnityEngine;

public static class RPLevelLibrary
{
    private static readonly Vector3 Start = new Vector3(0f, 0.35f, -5.35f);

    public static List<RPLevelData> CreateLevels()
    {
        List<RPLevelData> levels = new List<RPLevelData>();

        AddHandmadeAppStoreLevels(levels);

        for (int i = levels.Count + 1; i <= 150; i++)
        {
            levels.Add(CreateGeneratedChallengeLevel(i));
        }

        return levels;
    }

    private static void AddHandmadeAppStoreLevels(List<RPLevelData> levels)
    {
        // Chapter 1: Learn the shot.
        levels.Add(Level(1, "Straight Shot", 1,
            T(0f, 4.65f)
        ));

        levels.Add(Level(2, "Soft Pull", 1,
            T(0f, 3.15f)
        ));

        levels.Add(Level(3, "First Angle", 1,
            T(2.8f, 4.05f)
        ));

        levels.Add(Level(4, "Two Glasses", 1,
            T(-1.55f, 4.65f, 1.55f, 4.65f)
        ));

        levels.Add(Level(5, "Clean Pair", 2,
            T(-3.2f, 4.30f, 3.2f, 4.30f)
        ));

        // Chapter 2: Rail usage.
        levels.Add(Level(6, "Rail Kiss", 1,
            T(3.75f, 3.75f)
        ));

        levels.Add(Level(7, "Left Rail", 1,
            T(-3.75f, 3.75f)
        ));

        levels.Add(Level(8, "Center Block", 1,
            T(3.55f, 4.35f),
            W(0f, 1.15f, 3.2f, 0.35f)
        ));

        levels.Add(Level(9, "Mirror Block", 1,
            T(-3.55f, 4.35f),
            W(0f, 1.15f, 3.2f, 0.35f)
        ));

        levels.Add(Level(10, "Side Choice", 2,
            T(-3.35f, 4.35f, 3.35f, 4.35f),
            W(0f, 1.15f, 3.0f, 0.35f)
        ));

        // Chapter 3: Two-shot logic.
        levels.Add(Level(11, "Split Table", 2,
            T(-3.4f, 4.25f, 3.4f, 4.25f),
            W(0f, 0.95f, 0.35f, 4.2f)
        ));

        levels.Add(Level(12, "Narrow Lane", 2,
            T(0f, 4.95f),
            W(-1.35f, 1.15f, 0.35f, 4.2f,
               1.35f, 1.15f, 0.35f, 4.2f)
        ));

        levels.Add(Level(13, "Offset Lane", 2,
            T(2.9f, 4.65f),
            W(-1.35f, 1.00f, 0.35f, 3.8f,
               1.55f, 2.15f, 0.35f, 2.8f)
        ));

        levels.Add(Level(14, "Corner Skill", 1,
            T(-3.75f, 4.60f),
            W(-2.0f, 1.6f, 0.35f, 3.8f,
               1.7f, 2.8f, 3.0f, 0.35f)
        ));

        levels.Add(Level(15, "Three Pops", 2,
            T(-3.05f, 4.30f, 0f, 4.85f, 3.05f, 4.30f),
            W(0f, 1.0f, 2.2f, 0.35f)
        ));

        // Chapter 4: Bumpers.
        levels.Add(Level(16, "First Bumper", 1,
            T(0f, 4.85f),
            WB(0f, 0.25f, 1.45f, 0.32f)
        ));

        levels.Add(Level(17, "Bumper Bank", 1,
            T(3.55f, 4.20f),
            WB(-1.45f, 0.20f, 1.45f, 0.32f)
        ));

        levels.Add(Level(18, "Double Bumper", 2,
            T(-3.45f, 4.30f, 3.45f, 4.30f),
            WB(-1.5f, 0.3f, 1.5f, 0.32f,
                1.5f, 0.3f, 1.5f, 0.32f)
        ));

        levels.Add(Level(19, "Bumper Pocket", 2,
            T(0f, 4.75f, 3.6f, 3.35f),
            W(2.0f, 0.0f, 0.35f, 3.4f)
            .AddRangeReturn(WB(-1.8f, -0.85f, 1.8f, 0.32f))
        ));

        levels.Add(Level(20, "Brass Rhythm", 2,
            T(-3.4f, 4.4f, 0f, 4.85f, 3.4f, 4.4f),
            WB(-2.0f, 0.2f, 1.4f, 0.32f,
                2.0f, 0.2f, 1.4f, 0.32f)
        ));

        // Chapter 5: Breakable glass.
        levels.Add(Level(21, "Glass Gate", 2,
            T(0f, 4.95f),
            WG(0f, 2.1f, 2.3f, 0.28f)
        ));

        levels.Add(Level(22, "Break First", 2,
            T(3.35f, 4.50f),
            WG(0f, 2.25f, 2.5f, 0.28f),
            W(2.2f, 0.3f, 0.35f, 3.2f)
        ));

        levels.Add(Level(23, "Glass and Rail", 2,
            T(-3.45f, 4.45f, 3.45f, 4.45f),
            WG(0f, 2.40f, 2.7f, 0.28f)
        ));

        levels.Add(Level(24, "Glass Door", 3,
            T(0f, 4.9f, 3.35f, 3.40f),
            WG(0f, 2.4f, 2.6f, 0.28f),
            W(2.0f, 0.0f, 0.35f, 3.2f)
        ));

        levels.Add(Level(25, "Break and Bounce", 2,
            T(-3.4f, 4.55f, 3.4f, 4.55f),
            WG(0f, 2.5f, 2.5f, 0.28f),
            WB(0f, -0.65f, 1.5f, 0.32f)
        ));

        // Chapter 6: App-store-quality mixed puzzles.
        levels.Add(Level(26, "Double Lane", 2,
            T(-3.4f, 4.95f, 3.4f, 4.95f),
            W(0f, 1.0f, 0.35f, 5.0f),
            WB(-2.65f, 2.2f, 1.8f, 0.32f,
                2.65f, 2.2f, 1.8f, 0.32f)
        ));

        levels.Add(Level(27, "Precision", 2,
            T(-4.0f, 4.70f, 4.0f, 4.70f),
            W(-2.1f, 1.5f, 0.35f, 4.0f,
               2.1f, 1.5f, 0.35f, 4.0f),
            WB(0f, -1.0f, 2.0f, 0.32f)
        ));

        levels.Add(Level(28, "Table Trick", 3,
            T(-3.7f, 4.5f, 0f, 4.85f, 3.7f, 4.5f),
            W(-1.4f, 1.5f, 0.35f, 3.0f,
               1.4f, -0.5f, 0.35f, 3.0f),
            WG(0f, 3.1f, 2.2f, 0.30f)
        ));

        levels.Add(Level(29, "Gold Route", 3,
            T(-3.65f, 4.50f, 0f, 4.95f, 3.65f, 4.50f),
            W(0f, 1.8f, 2.7f, 0.35f),
            WB(-2.8f, -0.5f, 1.5f, 0.32f,
                2.8f, -0.5f, 1.5f, 0.32f),
            WG(0f, 3.15f, 2.6f, 0.28f)
        ));

        levels.Add(Level(30, "First Mastery", 3,
            T(-3.8f, 4.80f, -1.2f, 4.95f, 1.2f, 4.95f, 3.8f, 4.80f),
            W(0f, 1.5f, 2.9f, 0.35f,
              -2.8f, -0.7f, 0.35f, 3.2f,
               2.8f, -0.7f, 0.35f, 3.2f),
            WB(0f, -2.2f, 1.8f, 0.32f),
            WG(0f, 3.2f, 2.6f, 0.28f)
        ));
    }

    private static RPLevelData Level(int number, string title, int parShots, List<Vector3> targets, params List<RPWallData>[] wallGroups)
    {
        RPLevelData level = new RPLevelData();
        level.levelNumber = number;
        level.title = title;
        level.parShots = parShots;
        level.ballStart = Start;
        level.targets.AddRange(targets);

        if (wallGroups != null)
        {
            foreach (List<RPWallData> group in wallGroups)
            {
                if (group != null)
                {
                    level.walls.AddRange(group);
                }
            }
        }

        return level;
    }

    private static List<Vector3> T(params float[] values)
    {
        List<Vector3> result = new List<Vector3>();

        for (int i = 0; i + 1 < values.Length; i += 2)
        {
            result.Add(new Vector3(values[i], 0.35f, values[i + 1]));
        }

        return result;
    }

    private static List<RPWallData> W(params float[] values)
    {
        return Walls(RPObstacleType.NormalWall, values);
    }

    private static List<RPWallData> WB(params float[] values)
    {
        return Walls(RPObstacleType.MetalBumper, values);
    }

    private static List<RPWallData> WG(params float[] values)
    {
        return Walls(RPObstacleType.BreakableGlass, values);
    }

    private static List<RPWallData> Walls(RPObstacleType type, params float[] values)
    {
        List<RPWallData> result = new List<RPWallData>();

        for (int i = 0; i + 3 < values.Length; i += 4)
        {
            result.Add(new RPWallData(
                new Vector3(values[i], 0.45f, values[i + 1]),
                new Vector3(values[i + 2], 0.9f, values[i + 3]),
                type
            ));
        }

        return result;
    }

    private static List<RPWallData> AddRangeReturn(this List<RPWallData> original, List<RPWallData> extra)
    {
        original.AddRange(extra);
        return original;
    }

    private static RPLevelData CreateGeneratedChallengeLevel(int levelNumber)
    {
        RPLevelData level = new RPLevelData();
        level.levelNumber = levelNumber;
        level.title = "Challenge " + levelNumber;
        level.ballStart = Start;
        level.parShots = Mathf.Clamp(2 + (levelNumber - 30) / 15, 2, 5);

        int targetCount = Mathf.Clamp(3 + levelNumber / 12, 3, 7);
        float spread = Mathf.Lerp(1.9f, 4.0f, Mathf.Clamp01(levelNumber / 90f));

        for (int i = 0; i < targetCount; i++)
        {
            float t = targetCount == 1 ? 0.5f : i / (float)(targetCount - 1);
            float x = Mathf.Lerp(-spread, spread, t);
            float z = 4.85f - (i % 3) * 1.15f;
            x += Mathf.Sin(levelNumber * 0.37f + i * 1.4f) * 0.55f;
            level.targets.Add(new Vector3(x, 0.35f, z));
        }

        level.walls.Add(new RPWallData(new Vector3(-2.7f, 0.45f, -0.6f), new Vector3(0.35f, 0.9f, 3.2f)));
        level.walls.Add(new RPWallData(new Vector3(2.7f, 0.45f, 0.8f), new Vector3(0.35f, 0.9f, 3.2f)));

        if (levelNumber % 2 == 0)
            level.walls.Add(new RPWallData(new Vector3(0f, 0.45f, 1.6f), new Vector3(2.4f, 0.9f, 0.35f)));

        if (levelNumber % 3 == 0)
            level.walls.Add(new RPWallData(new Vector3(0f, 0.45f, -2.4f), new Vector3(1.8f, 0.9f, 0.32f), RPObstacleType.MetalBumper));

        if (levelNumber % 5 == 0)
            level.walls.Add(new RPWallData(new Vector3(0f, 0.45f, 3.0f), new Vector3(2.4f, 0.9f, 0.28f), RPObstacleType.BreakableGlass));

        return level;
    }
}
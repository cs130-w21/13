/// Static class that holds all of the constant values for the game
/// Ex: access with Constants.Costs.TURN
public static class Constants
{
    public static class Costs 
    {
        public const int TURN = 3;
        public const int MOVE = 5;
        public const int MINE = 10;
        public const int PLACE = 7;
        public const int BOOSTED_MINE = 5;
        public const int BOOSTED_MOVE = 2;
    }

    public static class Points 
    {
        public const int SMALL = 5;
        public const int MEDIUM = 10;
        public const int LARGE = 20;
    }

    public static class Robot
    {
        public const int BASE_BATTERY_CHARGE = 50;
        public const int BATTERY_PACK_BONUS = 25;
    }

    public static class Game
    {
        public const int TURN_DURATION_SECS = 60;
        public const int MAX_TURNS = 10;
        public const int TARGET_SCORE = 100;
        public const float ACTION_SPEED = 0.3f;
        public const float ACTION_PAUSE_BETWEEN = 0.2f;
    }

    public static class Board
    {
        public const int BOARD_WIDTH = 20;
        public const int BOARD_HEIGHT = 16;
    }
}

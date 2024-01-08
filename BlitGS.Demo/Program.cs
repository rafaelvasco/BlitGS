using BlitGS.Demo;
using BlitGS.Engine;

const int CANVAS_WIDTH = 1920 / 4;
const int CANVAS_HEIGHT = 1080 / 4;

var gameConfig = new GameConfig()
{
    Title = "Demo1",
    WindowWidth = CANVAS_WIDTH * 2,
    WindowHeight = CANVAS_HEIGHT * 2,
    CanvasWidth = CANVAS_WIDTH,
    CanvasHeight = CANVAS_HEIGHT
};

using var game = new Demo(gameConfig);
game.Start();
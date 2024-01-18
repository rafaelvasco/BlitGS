using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace BlitGS.Engine;

public static class GameLoop
{
    public static bool Running { get; private set; }
    
    public static double UpdateRate
    {
        get => _updateRate;
        set => ResetLoop(value);
    }

    public static bool UnlockFrameRate { get; set; } = false;
    
    public static bool IsActive { get; internal set; } = true;

    public static TimeSpan InactiveSleepTime
    {
        get => _inactiveSleepTime;
        set
        {
            if (value < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("The time must be positive.", default(Exception));

            _inactiveSleepTime = value;
        }
    }

    internal static void Init()
    {
        ResetLoop(DefaultFrameRate);
        
        var time60Hz = Platform.GetPerfFreq() / 60;
        _snapFreqs =
        [
            time60Hz, //60fps
            time60Hz * 2, //30fps
            time60Hz * 3, //20fps
            time60Hz * 4, //15fps
            (time60Hz + 1) / 2 //120fps
        ];

        _timeAverager = new double[TimeHistoryCount];

        for (var i = 0; i < TimeHistoryCount; i++)
        {
            _timeAverager[i] = _desiredFrametime;
        }
    }
    
    public static void SuppressDraw()
    {
        _suppressDraw = true;
    }

    private static void ResetLoop(double desiredUpdateRate)
    {
        _updateRate = desiredUpdateRate;

        _frameAccum = 0;
        _prevFrameTime = 0;
        _fixedDeltatime = 1.0 / _updateRate;
        _desiredFrametime = Platform.GetPerfFreq() / _updateRate;
        _vsyncMaxError = Platform.GetPerfCounter() * 0.0002;
    }

    public static void Start(Game game)
    {
        Running = true;
        _prevFrameTime = Platform.GetPerfCounter();
        _frameAccum = 0;
        Tick(game);
    }

    public static void Terminate()
    {
        Running = false;
        SuppressDraw();
    }

    public static void Tick(Game game)
    {
        if (!IsActive && InactiveSleepTime.TotalMilliseconds >= 1.0)
        {
            Thread.Sleep((int)_inactiveSleepTime.TotalMilliseconds);
        }

        var currentFrameTime = Platform.GetPerfCounter();

        var deltaTime = currentFrameTime - _prevFrameTime;

        _prevFrameTime = currentFrameTime;

        // Handle unexpected timer anomalies (overflow, extra slow frames, etc)
        if (deltaTime > _desiredFrametime * 8)
        {
            deltaTime = _desiredFrametime;
        }

        if (deltaTime < 0)
        {
            deltaTime = 0;
        }

        // VSync Time Snapping
        for (var i = 0; i < _snapFreqs.Length; ++i)
        {
            var snapFreq = _snapFreqs[i];

            if (!(Math.Abs(deltaTime - snapFreq) < _vsyncMaxError)) continue;
            
            deltaTime = snapFreq;
            break;
        }

        // Delta Time Averaging
        for (var i = 0; i < TimeHistoryCount - 1; ++i)
        {
            _timeAverager[i] = _timeAverager[i + 1];
        }

        _timeAverager[TimeHistoryCount - 1] = deltaTime;

        deltaTime = 0;

        for (var i = 0; i < TimeHistoryCount; ++i)
        {
            deltaTime += _timeAverager[i];
        }

        deltaTime /= TimeHistoryCount;

        // Add To Accumulator
        _frameAccum += deltaTime;

        // Spiral of Death Protection
        if (_frameAccum > _desiredFrametime * 8)
        {
            _resync = true;
        }

        // Timer Resync Requested
        if (_resync)
        {
            _frameAccum = 0;
            deltaTime = _desiredFrametime;
            _resync = false;
        }

        Platform.ProcessEvents();

        // Unlocked Frame Rate, Interpolation Enabled
        if (UnlockFrameRate)
        {
            var consumedDeltaTime = deltaTime;

            while (_frameAccum >= _desiredFrametime)
            {
                game.TickFixedUpdate((float)_fixedDeltatime);

                // Cap Variable Update's dt to not be larger than fixed update, 
                // and interleave it (so game state can always get animation frame it needs)
                if (consumedDeltaTime > _desiredFrametime)
                {
                    TickInput();
                    game.TickUpdate((float)_fixedDeltatime);
                    consumedDeltaTime -= _desiredFrametime;
                }

                _frameAccum -= _desiredFrametime;
            }

            TickInput();
            game.TickUpdate((float)(consumedDeltaTime / Platform.GetPerfFreq()));

            if (!_suppressDraw)
            {
                game.TickFrame((float)(_frameAccum / _desiredFrametime));
                Canvas.Flip();
            }
            else
            {
                _suppressDraw = false;
            }
        }
        // Locked Frame Rate, No Interpolation
        else
        {
            while (_frameAccum >= _desiredFrametime * UpdateMult)
            {
                for (var i = 0; i < UpdateMult; ++i)
                {
                    TickInput();
                    game.TickFixedUpdate((float)_fixedDeltatime);
                    game.TickUpdate((float)_fixedDeltatime);

                    _frameAccum -= _desiredFrametime;
                }
            }

            if (!_suppressDraw)
            {
                game.TickFrame(1.0f);
                Canvas.Flip();
            }
            else
            {
                _suppressDraw = false;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void TickInput()
    {
        Keyboard.UpdateState();
    }

    private const double DefaultFrameRate = 60;
    private const int TimeHistoryCount = 4;
    private const int UpdateMult = 1;

    private static bool _resync = true;

    private static double _fixedDeltatime;
    private static double _desiredFrametime;
    private static double _vsyncMaxError;
    private static double[] _snapFreqs = null!;
    private static double[] _timeAverager = null!;
    private static double _updateRate;
    private static bool _suppressDraw;

    private static TimeSpan _inactiveSleepTime = TimeSpan.FromSeconds(0.02);

    private static double _prevFrameTime;
    private static double _frameAccum;
}
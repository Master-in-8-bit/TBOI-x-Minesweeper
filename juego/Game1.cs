using System;
using System.IO;
using System.Net.WebSockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;



namespace juego;


public class Game1 : Core
{

    private AnimatedSprite pilkanobRun;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Vector2 pilkanobPosition;
    private Tilemap _tilemap;
    private Rectangle _roomBounds;
    private Vector2 prepilkanobPosition = Vector2.Zero; 
    private Texture2D overlay;
    private const float MOVEMENT_SPEED = 5.0f;
    private Vector2 mouseGridPos;
    private Vector2 preMouseGridPos;
    private Vector2 clickPosLeft;
    private Vector2 clickPosRight;

    public Game1() : base("juego", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        base.Initialize();
        Rectangle screenBounds = GraphicsDevice.PresentationParameters.Bounds;

       _roomBounds = new Rectangle(
            (int)_tilemap.TileWidth,
            (int)_tilemap.TileHeight,
            screenBounds.Width - (int)_tilemap.TileWidth * 2,
            screenBounds.Height - (int)_tilemap.TileHeight * 2
        );

        // Initial slime position will be the center tile of the tile map.
        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;
        pilkanobPosition = new Vector2(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);
    }

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
        overlay = Content.Load<Texture2D>("images/overlay");
        
        TextureAtlas fondoatlas = TextureAtlas.FromFile(Content, "images/tileset.xml");
        TextureAtlas pilkanobatlas = TextureAtlas.FromFile(Content, "images/pilkanob.xml");

        // Create the tilemap from the XML configuration file.
        _tilemap = Tilemap.FromFile(Content, "images/tilemap.xml");
        _tilemap.Scale = new Vector2(2.5f, 2.5f);

        pilkanobRun = pilkanobatlas.CreateAnimatedSprite("pilkanob_Run");
        pilkanobRun.Scale = new Vector2(4.0f, 4.0f);
        pilkanobPosition = new Vector2(50,50);

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        
        CheckMove(gameTime);
        CheckKeyboardInput();
        CheckMouseInput();
        Rectangle screenBounds = new Rectangle(
            0,
            0,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight
        );
        Circle pilkabounds = new Circle(
            (int)(pilkanobPosition.X + (pilkanobRun.Width * 0.5f)),
            (int)(pilkanobPosition.Y + (pilkanobRun.Height * 0.5f)),
            (int) (pilkanobRun.Width * 0.5f)
        );
        if (pilkabounds.Left < _roomBounds.Left)
        {
            _tilemap.SetTile(1, 1, 0);
            pilkanobPosition.X = _roomBounds.Left;
        }
        else if (pilkabounds.Right > _roomBounds.Right)
        {
            _tilemap.SetTile(1, 1, 1);
            pilkanobPosition.X = _roomBounds.Right - pilkanobRun.Width;
        }

        if (pilkabounds.Top < _roomBounds.Top)
        {
            _tilemap.SetTile(1, 1, 2);
            pilkanobPosition.Y = _roomBounds.Top;
        }
        else if (pilkabounds.Bottom > _roomBounds.Bottom)
        {
            _tilemap.SetTile(1, 1, 3);
            pilkanobPosition.Y = _roomBounds.Bottom - pilkanobRun.Height;
        }
        
        if (mouseGridPos.X > 0 && mouseGridPos.X < 15 && mouseGridPos.Y > 0 && mouseGridPos.Y < 8 && preMouseGridPos.X > 0 && preMouseGridPos.X < 15 && preMouseGridPos.Y > 0 && preMouseGridPos.Y < 8)
        {
            _tilemap.SetTile((int)preMouseGridPos.X, (int)preMouseGridPos.Y, 0);
            _tilemap.SetTile((int)mouseGridPos.X, (int)mouseGridPos.Y, 1);
            _tilemap.SetTile((int)clickPosLeft.X, (int)clickPosLeft.Y, 2);
            _tilemap.SetTile((int)clickPosRight.X, (int)clickPosRight.Y, 3);
        }

        base.Update(gameTime);
    }
    private void CheckMove(GameTime gameTime)
    {
        if(pilkanobPosition != prepilkanobPosition)
        {
            pilkanobRun.Update(gameTime);
        }
        else
        {
            pilkanobRun._currentFrame = 3;
            pilkanobRun.Update(gameTime);
        }
        prepilkanobPosition = pilkanobPosition;
    }
    private void CheckKeyboardInput()
    {
        // If the space key is held down, the movement speed increases by 1.5
        float speed = MOVEMENT_SPEED;
        if (Input.Keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        // If the W or Up keys are down, move the slime up on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up))
        {
            pilkanobPosition.Y -= speed;
        }

        // if the S or Down keys are down, move the slime down on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down))
        {
            pilkanobPosition.Y += speed;
        }

        // If the A or Left keys are down, move the slime left on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left))
        {
            pilkanobPosition.X -= speed;
        }

        // If the D or Right keys are down, move the slime right on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right))
        {
            pilkanobPosition.X += speed;
        }
    }
    void CheckMouseInput()
    {
        preMouseGridPos = mouseGridPos;
        Vector2 mousePosition = Input.Mouse.Position.ToVector2();
        Console.WriteLine((int)mousePosition.X/80 + ", " + (int)mousePosition.Y/80);
        mouseGridPos = new Vector2(Math.Abs((int)mousePosition.X/80), Math.Abs((int)mousePosition.Y/80));
        if (Input.Mouse.CurrentState.LeftButton == ButtonState.Pressed) clickPosLeft = new Vector2(Math.Abs((int)mousePosition.X/80), Math.Abs((int)mousePosition.Y/80));
        if (Input.Mouse.CurrentState.RightButton == ButtonState.Pressed) clickPosRight = new Vector2(Math.Abs((int)mousePosition.X/80), Math.Abs((int)mousePosition.Y/80));
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _tilemap.Draw(SpriteBatch);
        pilkanobRun.Draw(SpriteBatch, pilkanobPosition);
        SpriteBatch.Draw(overlay, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, new Vector2(3.7f, 3.2f), SpriteEffects.None, 1.0f);
        SpriteBatch.End();
        base.Draw(gameTime);
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pong
{
    public class Game1 : Game
    {
        //MySprite ball;
        //MySprite face;
        MyInput myInput;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Player player;

        //public Vector2 faceTexturesize = new Vector2(50, 29);



        class MyInput
        {
            private Dictionary<Keys, string> myInputDict = new Dictionary<Keys, string>(){
                                  {Keys.Left, "left"},
                                  {Keys.Right, "right"},
                                  {Keys.Up, "up"},
                                  {Keys.Down, "down"} };
            private List<string> keyQueue = new List<string>();
            private string lastKeyPress = "none";

            public MyInput()
            {
            }

            private bool IsExit()
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    return true;
                return false;
            }

            public bool Update()
            {
                if (IsExit())
                {
                    keyQueue.Clear();
                    return false;
                }

                var kstate = Keyboard.GetState();
                
                foreach(KeyValuePair<Keys, string> currKey in myInputDict)
                {
                    // Append the current keypress. remove if already there.
                    if (kstate.IsKeyDown(currKey.Key))
                    {
                        if (!myInputDict.ContainsKey(currKey.Key))
                            keyQueue.Append(currKey.Value);
                        lastKeyPress = currKey.Value;
                    } 
                    else
                    //if (kstate.IsKeyUp(currKey.Key))
                    {
                        if (keyQueue.Count > 0)
                            keyQueue.Remove(currKey.Value);
                    }
                }
                return true;
            }

            private string GetDirectionInput()
            {
                if (keyQueue.Count > 0)
                    return keyQueue[0];
                return lastKeyPress;
            }
            public Vector2 GetCurrentInputDirection()
            {
                string dir_str = GetDirectionInput();
                if (dir_str == "up") return new Vector2(0, 1);
                if (dir_str == "down") return new Vector2(0, -1);
                if (dir_str == "left") return new Vector2(1, 0);
                if (dir_str == "right") return new Vector2(-1, 0);
                //if (dir_str == "none")
                return new Vector2(0, 0);
            }

        }




        class MySprite
        {
            private Texture2D texture;
            private Vector2 tileSize = new Vector2(80, 80);
            private Vector2 position = new Vector2();
            private Vector2 gridPosition = new Vector2();
            private float speed = 100f;

            public Vector2 GridPosition
            {
                get
                {
                    return gridPosition;
                }

                set
                {
                    gridPosition = value;
                    position = GetCenteredPosition();
                }
            }

            public Texture2D Texture { get => texture; set => texture = value; }
            public float Speed { get => speed; set => speed = value; }
            public Vector2 Position { get => position;}

            public MySprite(Vector2 position, Texture2D texture = null)
            {
                GridPosition = position;
                Texture = texture;
            }

            private Vector2 GetSpriteCenter()
            {
                return new Vector2(Texture.Width / 2, Texture.Height / 2);
            }

            private Vector2 GetTileCenter()
            {
                return new Vector2(tileSize.X / 2, tileSize.Y / 2);
            }

            private Vector2 GetCenteredPosition()
            {
                //return new Vector2(GetCenter().X + position.X * texture.Width, GetCenter().Y + position.Y * texture.Height);
                return (gridPosition * tileSize) + GetTileCenter();
            }

            public void Draw(SpriteBatch batch)
            {
                batch.Draw(Texture, Position, null, Color.White,0, GetSpriteCenter(), Vector2.One,SpriteEffects.None,0f);
            }

            public void SetPXPos(Vector2 pos)
            {
                position = pos;
            }

            public void Move(Vector2 dir, float deltaTime)
            {
                position.Y -= dir.Y * speed * deltaTime;
                position.X -= dir.X * speed * deltaTime;
            }
        }

        class Player
        {

            private string[] textureNames = { "blue_body_squircle", "face_a" };
            private MySprite ball;
            private MySprite face;

            public string[] TextureNames { get => textureNames; }


            public Player()
            {
                ball = new MySprite(Vector2.Zero);
                face = new MySprite(Vector2.Zero);
            }

            public void SetTextures(string name, Texture2D texture)
            {
                if (name == textureNames[0])
                    ball.Texture = texture;
                if (name == textureNames[1])
                    face.Texture = texture;
            }

            public void Draw(SpriteBatch batch)
            {
                ball.Draw(batch);
                face.Draw(batch);
            }

            public void Move(Vector2 dir, float deltaTime)
            {
                ball.Move(dir, deltaTime);
                face.SetPXPos(ball.Position);
            }
        }





        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Player();
            myInput = new MyInput();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            foreach (string a in player.TextureNames)
            {
                player.SetTextures(a, Content.Load<Texture2D>(a));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (!myInput.Update())
                Exit();

            Vector2 direction = myInput.GetCurrentInputDirection();

            player.Move(direction, (float)gameTime.ElapsedGameTime.TotalSeconds);

            // the base update - do not touch
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black); // CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            player.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

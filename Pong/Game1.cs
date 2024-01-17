using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

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
                    // Append the current keypress.
                    if (kstate.IsKeyDown(currKey.Key))
                    {
                        if (!keyQueue.Contains(currKey.Value))
                        {
                            keyQueue.Add(currKey.Value);

                        }
                        lastKeyPress = currKey.Value;
                    } 
                    else
                    {
                        keyQueue.Remove(currKey.Value);
                    }
                }
                return true;
            }

            private string GetDirectionInput()
            {
                
                if (keyQueue.Count > 0)
                {
                    return keyQueue[0];
                }
                return lastKeyPress;
            }
            public Vector2 GetCurrentInputDirection()
            {
                string dir_str = GetDirectionInput();
                if (dir_str == "up") return new Vector2(0, -1);
                if (dir_str == "down") return new Vector2(0, 1);
                if (dir_str == "left") return new Vector2(-1, 0);
                if (dir_str == "right") return new Vector2(1, 0);

                return new Vector2(0, 0);
            }

        }




        class MySprite
        {
            private Texture2D texture;
            private Vector2 tileSize = new Vector2(80, 80);
            private Vector2 position = new Vector2();
            private Vector2 gridPosition = new Vector2();
            private Vector2 gridEndPosition = new Vector2();
            private Vector2 gridMoveDirection = new Vector2(); 

            private float speed = 200f;

            public Texture2D Texture { get => texture; set => texture = value; }
            public float Speed { get => speed; set => speed = value; }
            public Vector2 Position { get => position; }

            /// <summary>
            /// Marks start of move grid position
            /// </summary>
            /// <returns></returns>
            public Vector2 GetGridPosition()
            {
                return gridPosition;
            }

            /// <summary>
            /// This changes position to be centerd on that grid and marks start of move grid position
            /// </summary>
            /// <param name="value"></param>
            public void SetGridPosition(Vector2 value)
            {
                gridPosition = value;
                position = GetCenteredPositionFromGridPos(gridPosition);
            }

            public Vector2 GetGridEndPosition()
            {
                return gridEndPosition;
            }

            public void SetGridEndPosition(Vector2 value)
            {
                gridEndPosition = value;
            }

            public MySprite(Vector2 position, Texture2D texture = null)
            {
                SetGridPosition(position);
                SetGridEndPosition(position);
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

            private Vector2 GetCenteredPositionFromGridPos(Vector2 gridPos)
            {
                //return new Vector2(GetCenter().X + position.X * texture.Width, GetCenter().Y + position.Y * texture.Height);
                return (gridPos * tileSize) + GetTileCenter();
            }

            /// <summary>
            /// Draw the texture at the centered position set by the sprite
            /// </summary>
            /// <param name="batch"></param>
            public void Draw(SpriteBatch batch)
            {
                batch.Draw(Texture, Position, null, Color.White,0, GetSpriteCenter(), Vector2.One,SpriteEffects.None,0f);
            }

            /// <summary>
            /// Allows direct setting of position - useful when not using Move i.e. second+ sprite
            /// </summary>
            /// <param name="pos"></param>
            public void SetPXPos(Vector2 pos)
            {
                position = pos;
            }

            /// <summary>
            /// Move follows delta time, speed, and direction
            /// Use GridMove if wanting grid movement
            /// </summary>
            /// <param name="dir"></param>
            /// <param name="deltaTime"></param>
            public void Move(Vector2 dir, float deltaTime)
            {
                position.Y += dir.Y * speed * deltaTime;
                position.X += dir.X * speed * deltaTime;
            }

            private bool IsEndOfMove(float dt_distance)
            {
                return Vector2.Distance(position, GetCenteredPositionFromGridPos(gridEndPosition)) < dt_distance; //abs(math.dist(self.rect.center, self.end_move_pos)) <= abs(dt_distance)
            }
            /// <summary>
            /// Move follows delta time, speed, and direction
            /// Keeps track of a grid move and try's to keep contineous movement smooth
            /// Use Move if not concerened about Grid
            /// </summary>
            /// <param name="dir"></param>
            /// <param name="deltaTime"></param>
            public void GridMove(Vector2 dir, float deltaTime)
            {
                if (IsEndOfMove(deltaTime * speed)) 
                {
                    gridMoveDirection = dir;
                    SetGridPosition(gridEndPosition);
                    SetGridEndPosition(gridEndPosition + dir);
                }
                else
                    Move(gridMoveDirection, deltaTime);
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
                //Move first sprite set the rest
                ball.GridMove(dir, deltaTime);
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

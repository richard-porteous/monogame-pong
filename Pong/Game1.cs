using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong
{
    public class Game1 : Game
    {
        MySprite ball;
        MySprite face;
        
        public Vector2 faceTexturesize = new Vector2(50, 29);

        class MySprite
        {
            private Texture2D texture;
            private Vector2 tileSize = new Vector2(80, 80);
            private Vector2 position = new Vector2();
            private Vector2 gridPosition = new Vector2();

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
                batch.Draw(Texture, position, null, Color.White,0, GetSpriteCenter(), Vector2.One,SpriteEffects.None,0f);
            }
        }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ball = new MySprite(Vector2.Zero);
            face = new MySprite(Vector2.Zero);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ball.Texture = Content.Load<Texture2D>("blue_body_squircle");
            face.Texture = Content.Load<Texture2D>("face_a");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black); // CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            ball.Draw(_spriteBatch); //.Draw(squircleTexture, new Vector2(0, 0), Color.White);

            face.Draw(_spriteBatch); //.Draw(faceTexture, new Vector2(squircleTexture.Width/2 - faceTexturesize.X / 2, squircleTexture.Height / 2 - faceTexturesize.Y / 2), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

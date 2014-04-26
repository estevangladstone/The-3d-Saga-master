﻿#region Library Imports

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

#endregion

namespace Ship
{
    class LirouShip : CObject
    {
        #region Fields    
        
        float moveSpeed, maxSpeed = 2.5f, acceleration = 0.025f, rotatingSpeed;
        Matrix modelMatrix;
        Vector3 velocity;

        #region Get / Set

        public Vector3 Position
        {
            get
            {
                Vector3 pos = modelMatrix.Translation;
                return pos;
            }
            set
            {
                modelMatrix.Translation = value;
            }
        }

        public float MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
            set
            {
                moveSpeed = value;
            }
        }

        public float RotatingSpeed
        {
            get
            {
                return rotatingSpeed;
            }
            set
            {
                rotatingSpeed = value;
            }
        }
        #endregion

        #endregion

        #region Constructors

        public LirouShip(Vector3 m_position, Vector3 rotation, float moveSpeed, float rotatingSpeed, float scale)
        {
            this.position = m_position;
            this.rotation = rotation;
            this.moveSpeed = moveSpeed;
            this.rotatingSpeed = rotatingSpeed;
            this.scale = scale;
            this.id = "ship";

            // the Zuera starts here
            this.modelMatrix = Matrix.Identity;
            this.modelMatrix *= Matrix.CreateScale(scale);
            this.modelMatrix *= Matrix.CreateRotationX(rotation.X);
            this.modelMatrix *= Matrix.CreateRotationY(rotation.Y);
            this.modelMatrix *= Matrix.CreateRotationZ(rotation.Z);
            this.modelMatrix *= Matrix.CreateTranslation(m_position);

        }

        #endregion

        #region Methods

        public void Update(KeyboardState kb, Camera camera, List<CObject> cobjects)
        {
            #region Follow Camera
            if (kb.IsKeyDown(Keys.Space))
            {
                position = modelMatrix.Translation;
                if (modelMatrix.Right.X * 2 > -(camera.Rotation.Y * Math.PI * 2))
                {
                    modelMatrix *= Matrix.CreateFromAxisAngle(Vector3.Normalize(modelMatrix.Right), -rotatingSpeed);
                }

                if (modelMatrix.Right.X * 2 < - (camera.Rotation.Y * Math.PI * 2))
                {
                    modelMatrix *= Matrix.CreateFromAxisAngle(Vector3.Normalize(modelMatrix.Right), rotatingSpeed);
                }

                if (modelMatrix.Up.Y > camera.Rotation.X)
                {
                    modelMatrix *= Matrix.CreateFromAxisAngle(Vector3.Normalize(modelMatrix.Up), -rotatingSpeed);
                }

                if (modelMatrix.Up.Y < camera.Rotation.X)
                {
                    modelMatrix *= Matrix.CreateFromAxisAngle(Vector3.Normalize(modelMatrix.Up), rotatingSpeed);
                }
                modelMatrix.Translation = position;
            }
            #endregion

            #region InputHandling

            // Move para esquerda
            if (kb.IsKeyDown(Keys.A))
            {
                // é preciso salvar a posicao(translation) em que a nave se enconta, se nao o comando abaixo modifica a posicao tbm. Depois de rotacionar a nave a posicao eh novamente inserida na matriz
                position = modelMatrix.Translation;
                modelMatrix *= Matrix.CreateFromAxisAngle(Vector3.Normalize(modelMatrix.Up), rotatingSpeed);
                modelMatrix.Translation = position;
            }

            // Move para direita
            if (kb.IsKeyDown(Keys.D))
            {
                // é preciso salvar a posicao(translation) em que a nave se enconta, se nao o comando abaixo modifica a posicao tbm. Depois de rotacionar a nave a posicao eh novamente inserida na matriz
                position = modelMatrix.Translation;
                modelMatrix *= Matrix.CreateFromAxisAngle(Vector3.Normalize(modelMatrix.Up), -rotatingSpeed);
                modelMatrix.Translation = position;
            }

            // Move para baixo
            if (kb.IsKeyDown(Keys.W))
            {
                // é preciso salvar a posicao(translation) em que a nave se enconta, se nao o comando abaixo modifica a posicao tbm. Depois de rotacionar a nave a posicao eh novamente inserida na matriz
                position = modelMatrix.Translation;
                modelMatrix *= Matrix.CreateFromAxisAngle(Vector3.Normalize(modelMatrix.Right), -rotatingSpeed);
                modelMatrix.Translation = position;
            }

            // Move para cima
            if (kb.IsKeyDown(Keys.S))
            {
                // é preciso salvar a posicao(translation) em que a nave se enconta, se nao o comando abaixo modifica a posicao tbm. Depois de rotacionar a nave a posicao eh novamente inserida na matriz
                position = modelMatrix.Translation;
                modelMatrix *= Matrix.CreateFromAxisAngle(Vector3.Normalize(modelMatrix.Right), rotatingSpeed);
                modelMatrix.Translation = position;
            }

            // Gira no eixo Z
            if (kb.IsKeyDown(Keys.Q))
            {
                // é preciso salvar a posicao(translation) em que a nave se enconta, se nao o comando abaixo modifica a posicao tbm. Depois de rotacionar a nave a posicao eh novamente inserida na matriz
                position = modelMatrix.Translation;
                modelMatrix *= Matrix.CreateFromAxisAngle(Vector3.Normalize(modelMatrix.Forward), -rotatingSpeed);
                modelMatrix.Translation = position;
            }

            // Move para cima
            if (kb.IsKeyDown(Keys.E))
            {
                // é preciso salvar a posicao(translation) em que a nave se enconta, se nao o comando abaixo modifica a posicao tbm. Depois de rotacionar a nave a posicao eh novamente inserida na matriz
                position = modelMatrix.Translation;
                modelMatrix *= Matrix.CreateFromAxisAngle(Vector3.Normalize(modelMatrix.Forward), rotatingSpeed);
                modelMatrix.Translation = position;
            }

            if (kb.IsKeyDown(Keys.LeftShift))
            {
                // Acceleration
                if (moveSpeed < maxSpeed)
                {
                    moveSpeed += acceleration;
                }

                // Aqui ele multiplica parte da matriz(a que indica o eixo Z POSITIVO que indica a parte da frente da nave) pela moveSpeed para gerar uma nova posicao em Z para a nave
                // tudo isso eh multiplicado por 500 por que multiplicar apenas pela moveSpeed fazia ela se mover muito devagar, mas isso eh soh provisorio
                velocity = modelMatrix.Forward * moveSpeed * 500.0f;
            }

            else if (kb.IsKeyDown(Keys.K))
            {
                // Acceleration
                if (moveSpeed < (maxSpeed) / 2)
                {
                    moveSpeed += acceleration;
                }
                // Aqui ele multiplica parte da matriz(a que indica o eixo Z NEGATIVO que indica a parte de trás da nave) pela moveSpeed para gerar uma nova posicao em Z para a nave
                // tudo isso eh multiplicado por 500 por que multiplicar apenas pela moveSpeed fazia ela se mover muito devagar, mas isso eh soh provisorio
                velocity = modelMatrix.Backward * moveSpeed * 500.0f;
            }
            else
            {
            
            }


            // Deceleration
            if (moveSpeed > 0)
            {
                if (moveSpeed - acceleration / 2 < Constants.ZERO)
                {
                    moveSpeed = 0;
                }
                else
                {
                    moveSpeed -= acceleration / 2;
                }
                // Aqui ele multiplica parte da matriz(a que indica o eixo Z POSITIVO que indica a parte da frente da nave) pela moveSpeed para gerar uma nova posicao em Z para a nave
                // tudo isso eh multiplicado por 500 por que multiplicar apenas pela moveSpeed fazia ela se mover muito devagar, mas isso eh soh provisorio
                velocity = modelMatrix.Forward * moveSpeed * 500.0f;
            }

            // Deceleration
            if (moveSpeed > 0)
            {
                if (moveSpeed - acceleration / 2 < Constants.ZERO)
                {
                    moveSpeed = 0;
                }
                else
                {
                    moveSpeed -= acceleration / 2;
                }
                // Aqui ele multiplica parte da matriz(a que indica o eixo Z NEGATIVO que indica a parte de trás da nave) pela moveSpeed para gerar uma nova posicao em Z para a nave
                // tudo isso eh multiplicado por 500 por que multiplicar apenas pela moveSpeed fazia ela se mover muito devagar, mas isso eh soh provisorio
                velocity = modelMatrix.Backward * moveSpeed * 500.0f;
            }

            // Aqui a nova posicao que foi criada para a nave e que esta armazenada em 'velocity' é inserida na matriz, mudando a posicao da nave
            // Translation é o campo que indica a posicao da nave no 'world', deem uma olhada em matrizes rotacionais que cvs entendem, eu nao sei explicar bem. 
            modelMatrix *= Matrix.CreateTranslation(velocity);

            #endregion            

            #region Collisions
            foreach (CObject cobject in cobjects)
            {
                if (isCollidingBS(cobject.Model) && cobject != (CObject)this)
                {
                    //Console.WriteLine("READY TO GO");
                    //Console.WriteLine("READY TO GO");
                    //Console.WriteLine("READY TO GO");
                    //Console.WriteLine("READY TO GO");
                }
            }
            #endregion
        }

        public void LoadContent(ContentManager Content)
        {
            model = Content.Load<Model>("Models\\p1_wedge");            
        }

        public void Draw(float aspectRatio, Matrix view)
        {
            //Matrix[] transforms = new Matrix[model.Bones.Count];
            //model.CopyAbsoluteBoneTransformsTo(transforms);            

            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    Matrix[] transforms = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(transforms);

                    effect.EnableDefaultLighting();

                    // modifiquei aqui tbm ~Estevan
                    effect.World = transforms[mesh.ParentBone.Index] * modelMatrix;
                    effect.View = view;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 1000000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        #endregion
    }
}
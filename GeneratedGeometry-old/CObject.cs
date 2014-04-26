#region Library Imports

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
    class CObject
    {
        #region Fields
        protected Model model;
        protected Vector3 position, rotation, specs;

        /////////////////////////////////////////// O QUE É ESSE specs ?????? ///////////////////////////////////////////////////

        protected float scale;
        protected string id;
        protected int i = 0;
        protected Matrix[] transforms;
        #region Get/Set
        public float Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public Vector3 Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }
        public Model Model
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
            }
        }
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        public Matrix[] Transforms
        {
            get
            {
                return transforms;
            }
            set
            {
                transforms = value;
            }
        }
        public Vector3 Specs
        {
            get
            {
                return specs;
            }
            set
            {
                specs = value;
            }
        }
        #endregion

        #endregion
        
        #region Methods

        #region Constructor/Destructor
        public CObject(Model nmodel, Vector3 nposition, Vector3 nrotation, float nscale, string nid)
        {
            model = nmodel;
            position = nposition;
            rotation = nrotation;
            scale = nscale;
            id = nid;
        }
        public CObject()
        {
            model = null;
            position = Vector3.Zero;
            rotation = Vector3.Zero;
            scale = 0.0f;
            id = "null";
        }
        #endregion

        #region Collision
        /*private BoundingBox createMerged(Model model) // Use it for low quality computers.
        {
            BoundingBox b = new BoundingBox();
            BoundingBox aux;
            foreach (ModelMesh mesh in model.Meshes)
            {
                aux = BoundingBox.CreateFromSphere(mesh.BoundingSphere);
                BoundingBox.CreateMerged(b, aux);
            }
            return b;
        }*/
        /*protected bool isCollidingBB(Model model2)
        {
            BoundingBox BB1, BB2;
            BB1 = CalculateBoundingBox(this.model);
            BB2 = CalculateBoundingBox(model2);
            if (BB1.Intersects(BB2))
            {
                Console.WriteLine(BB1.GetCorners());
                Console.WriteLine(BB2.GetCorners());
                Console.WriteLine(BB1.Min + " - " + BB1.Max);
                Console.WriteLine(BB2.Min + " - " + BB2.Max);
                return true;
            }
            return false;
        }*/
        protected bool isCollidingBB(CObject cobject2)
        {
            BoundingBox BB1, BB2;
            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere m1BoundingSphere = mesh.BoundingSphere;
                m1BoundingSphere.Center = position;
                m1BoundingSphere.Radius *= this.scale;
                BB1 = BoundingBox.CreateFromSphere(m1BoundingSphere);
                foreach (ModelMesh mesh2 in cobject2.Model.Meshes)
                {
                    BoundingSphere m2BoundingSphere = mesh2.BoundingSphere;
                    m2BoundingSphere.Center = cobject2.position;
                    if (cobject2.ID == "floor" || cobject2.ID.Contains("wall"))
                    {
                        m2BoundingSphere.Radius *= cobject2.scale * 30.0f;
                    }
                    else
                        m2BoundingSphere.Radius *= cobject2.scale;
                    BB2 = BoundingBox.CreateFromSphere(m2BoundingSphere);

                    if (BB1.Intersects(BB2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected bool isCollidingWalls(CObject cobject2)
        {
            BoundingBox BB1, BB2;
            foreach (ModelMesh mesh in model.Meshes) {
                BoundingSphere m1BoundingSphere = mesh.BoundingSphere;
                m1BoundingSphere.Center = position;
                m1BoundingSphere.Radius *= this.scale;
                BB1 = BoundingBox.CreateFromSphere(m1BoundingSphere);
                foreach (ModelMesh mesh2 in cobject2.Model.Meshes) 
                {
                    BB2 = CalculateBoundingBox(cobject2);
                    if (cobject2.ID == "floor")
                    {
                        if (i == 0)
                        {
                            Console.WriteLine(BB2.Max);
                            Console.WriteLine(BB2.Min);
                            Console.WriteLine(position);
                        }
                        i++;
                        if (i > 10)
                            i = 0;
                    }
                    if (BB1.Intersects(BB2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected bool isCollidingBS(CObject cobject2, int i)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere m1BoundingSphere = mesh.BoundingSphere;
                m1BoundingSphere.Center = position;
                m1BoundingSphere.Radius *= this.scale;
                foreach (ModelMesh mesh2 in cobject2.Model.Meshes)
                {
                    BoundingSphere m2BoundingSphere = mesh2.BoundingSphere;
                    m2BoundingSphere.Center = cobject2.position;
                    m2BoundingSphere.Radius *= cobject2.scale;
                    if (m1BoundingSphere.Intersects(m2BoundingSphere))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected BoundingBox CalculateBoundingBox(CObject cobject)
        {
            Vector3 min = new Vector3(cobject.position.X, cobject.position.Y - cobject.specs.Y, cobject.position.Z);
            Vector3 max = new Vector3(cobject.position.X, cobject.position.Y + cobject.specs.Y, cobject.position.Z);
            min.X -= cobject.specs.X;
            min.Z -= cobject.specs.Z;
            max.X += cobject.specs.X;
            max.Z += cobject.specs.Z;
            /*min.X -= (rotation.Y == 0) ? cobject.specs.X : cobject.specs.Z;
            min.Z -= (rotation.Y == 0) ? cobject.specs.Z : cobject.specs.X;
            max.X += (rotation.Y == 0) ? cobject.specs.X : cobject.specs.Z;
            max.Z += (rotation.Y == 0) ? cobject.specs.Z : cobject.specs.X;*/
            return new BoundingBox(min, max);
        }

        protected BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }
            // Create and return bounding box
            return new BoundingBox(min, max);
        }
        #endregion

        #region Drawing
        public void Draw(Matrix view, Matrix projection, float aspectRatio)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    transforms = new Matrix[model.Bones.Count]; 
                    model.CopyAbsoluteBoneTransformsTo(transforms);

                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateRotationY(rotation.Y)
                        * Matrix.CreateRotationX(rotation.X)
                        * Matrix.CreateRotationZ(rotation.Z)
                        * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 1000000.0f);

                }

                mesh.Draw();
            }
        }

        public void DrawTerrain(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    // Set the specular lighting to match the sky color.
                    effect.SpecularColor = new Vector3(0.6f, 0.4f, 0.2f);
                    effect.SpecularPower = 8;

                    // Set the fog to match the distant mountains
                    // that are drawn into the sky texture.
                    effect.FogEnabled = true;
                    effect.FogColor = new Vector3(0.15f);
                    effect.FogStart = 100;
                    effect.FogEnd = 320;
                }

                mesh.Draw();
            }
        }
        #endregion
        #endregion
    }
}

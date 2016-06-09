using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.GraphicsWidgets
{
    /*

        Ugh, I hate copy-pasting code

        Copied verbatim from -
        http://stackoverflow.com/questions/13984176/how-to-draw-graph-from-a-list-of-points

        TODO: Clean up & make not suck
    */

    [Serializable]
    public class Graph
    {
        public enum GraphType
        {
            Line,
            Fill
        }

        private readonly BasicEffect _effect;
        private Vector2 _scale = new Vector2(1.0f, 1.0f);
        private short[] lineListIndices;
        private short[] triangleStripIndices;

        /// <summary>
        ///     Determines whether the drawn graph will be line only, or filled
        /// </summary>
        public GraphType Type { get; set; }

        /// <summary>
        ///     The bottom left position of the graph
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        ///     The size of the graph.
        ///     The graph values will be scaled horizontally to fill width (Size.X)
        ///     Vertically, the values will be scaled based on MaxValue property, where the position of the value that is equal to
        ///     MaxValue will be Size.Y
        /// </summary>
        public Point Size { get; set; }

        /// <summary>
        ///     Determines the vertical scaling of the graph.
        ///     The value that is equal to MaxValue will be displayed at the top of the graph (at point Size.Y)
        /// </summary>
        public float MaxValue { get; set; }

        public Graph(GraphicsDevice graphicsDevice, Point size)
        {
            _effect = new BasicEffect(graphicsDevice)
            {
                View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up),
                Projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width,
                    graphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f),
                World = Matrix.Identity,
                VertexColorEnabled = true
            };
            MaxValue = 1;
            Size = size;
            if (size.Y <= 0)
            {
                Size = new Point(size.X, 1);
            }
            if (size.X <= 0)
            {
                Size = new Point(1, size.Y);
            }

            Type = GraphType.Line;
        }

        private void UpdateWorld()
        {
            _effect.World = Matrix.CreateScale(_scale.X, _scale.Y, 1.0f)
                            * Matrix.CreateRotationX(MathHelper.Pi)
                //flips the graph so that the higher values are above. Makes bottom left the graph origin.
                            * Matrix.CreateTranslation(new Vector3(Position, 0));
        }

        /// <summary>
        ///     Draws the values in given order, with specific color for each value
        /// </summary>
        /// <param name="values">Value/color pairs to draw, in order from left to right</param>
        public void Draw(List<Tuple<float, Color>> values)
        {
            if (values.Count < 2)
            {
                return;
            }

            //creates scaling (for the transformation) based on the number of points to draw
            float xScale = Size.X / (float) values.Count;
            float yScale = Size.Y / MaxValue;

            _scale = new Vector2(xScale, yScale);
            UpdateWorld();

            //different point lists for different types of graphs
            if (Type == GraphType.Line)
            {
                VertexPositionColor[] pointList = new VertexPositionColor[values.Count];
                for (int i = 0; i < values.Count; i++)
                {
                    pointList[i] =
                        new VertexPositionColor(
                            new Vector3(i, values[i].Item1 < MaxValue ? values[i].Item1 : MaxValue, 0), values[i].Item2);
                }

                DrawLineList(pointList);
            }
            else if (Type == GraphType.Fill)
            {
                VertexPositionColor[] pointList = new VertexPositionColor[values.Count * 2];
                for (int i = 0; i < values.Count; i++)
                {
                    //The vertices are created so that the triangles are inverted (back facing). When rotated they will become front facing.
                    //This is done to avoid changing rasterizer state to CullMode.CullClockwiseFace.
                    pointList[i * 2 + 1] =
                        new VertexPositionColor(
                            new Vector3(i, values[i].Item1 < MaxValue ? values[i].Item1 : MaxValue, 0), values[i].Item2);
                    pointList[i * 2] = new VertexPositionColor(new Vector3(i, 0, 0), values[i].Item2);
                }

                DrawTriangleStrip(pointList);
            }
        }

        /// <summary>
        ///     Draws the values in given order, in specified color
        /// </summary>
        /// <param name="values">Values to draw, in order from left to right</param>
        /// <param name="color">Color of the entire graph</param>
        public void Draw(List<float> values, Color color)
        {
            if (values.Count < 2)
            {
                return;
            }

            float xScale = Size.X / (float) values.Count;
            float yScale = Size.Y / MaxValue;

            _scale = new Vector2(xScale, yScale);
            UpdateWorld();

            if (Type == GraphType.Line)
            {
                VertexPositionColor[] pointList = new VertexPositionColor[values.Count];
                for (int i = 0; i < values.Count; i++)
                {
                    pointList[i] =
                        new VertexPositionColor(new Vector3(i, values[i] < MaxValue ? values[i] : MaxValue, 0), color);
                }

                DrawLineList(pointList);
            }
            else if (Type == GraphType.Fill)
            {
                VertexPositionColor[] pointList = new VertexPositionColor[values.Count * 2];
                for (int i = 0; i < values.Count; i++)
                {
                    pointList[i * 2 + 1] =
                        new VertexPositionColor(new Vector3(i, values[i] < MaxValue ? values[i] : MaxValue, 0), color);
                    pointList[i * 2] = new VertexPositionColor(new Vector3(i, 0, 0), color);
                }

                DrawTriangleStrip(pointList);
            }
        }

        private void DrawLineList(VertexPositionColor[] pointList)
        {
            //indices updated only need to be updated when the number of points has changed
            if (lineListIndices == null || lineListIndices.Length != ((pointList.Length * 2) - 2))
            {
                lineListIndices = new short[(pointList.Length * 2) - 2];
                for (int i = 0; i < pointList.Length - 1; i++)
                {
                    lineListIndices[i * 2] = (short) (i);
                    lineListIndices[(i * 2) + 1] = (short) (i + 1);
                }
            }

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _effect.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    pointList,
                    0,
                    pointList.Length,
                    lineListIndices,
                    0,
                    pointList.Length - 1
                    );
            }
        }

        private void DrawTriangleStrip(VertexPositionColor[] pointList)
        {
            if (triangleStripIndices == null || triangleStripIndices.Length != pointList.Length)
            {
                triangleStripIndices = new short[pointList.Length];
                for (int i = 0; i < pointList.Length; i++)
                {
                    triangleStripIndices[i] = (short) i;
                }
            }

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _effect.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    pointList,
                    0,
                    pointList.Length,
                    triangleStripIndices,
                    0,
                    pointList.Length - 2
                    );
            }
        }
    }
}